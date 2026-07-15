// DecalScale.cs
// Usage: CS < DecalScale.cs [scenario tag]
//        
// For each sbsp referenced in the scenario:
//   - Reads the Scale from each RuntimeDecal
//   - Looks up the decs tag from the scnr DecalPalette via PaletteIndex
//   - Clears the DecalPalette and rebuilds it in original palette order:
//       scale == 0  -> uses original palette entry as-is (no duplication, no radius change)
//       scale != 0  -> duplicate decs tag, set Radius directly to scale
//                      (RuntimeMaxRadius is NOT changed)
//                      if any decal def has Multiply or DoubleMultiply blend mode,
//                      MaxOverlapping is incremented by 1; otherwise set to 6
//   - Updates PaletteIndex on each RuntimeDecal in-place

using TagTool.Tags.Definitions.Common;
using TagTool.Cache.HaloOnline;

if (Args.Count < 1)
{
    Console.WriteLine("Usage: CS < DecalScale.cs [scenario tag]");
    return;
}

// ── Load scenario ──────────────────────────────────────────────────────────
var scnrTag = Cache.TagCache.GetTag(Args[0]);
if (scnrTag == null)
{
    Console.WriteLine($"Error: Could not find scenario tag '{Args[0]}'");
    return;
}

Scenario scnr;
using (var stream = Cache.OpenCacheRead())
    scnr = Cache.Deserialize<Scenario>(stream, scnrTag);

// Snapshot the original palette before we clear it
var originalPalette = scnr.DecalPalette.ToList();
Console.WriteLine($"Original decal palette: {originalPalette.Count} entries.");

// ── Load all sbsps ─────────────────────────────────────────────────────────
var sbspData = new List<(CachedTag tag, ScenarioStructureBsp sbsp)>();
using (var stream = Cache.OpenCacheRead())
{
    foreach (var bspRef in scnr.StructureBsps)
    {
        if (bspRef?.StructureBsp == null) continue;
        var sbsp = Cache.Deserialize<ScenarioStructureBsp>(stream, bspRef.StructureBsp);
        sbspData.Add((bspRef.StructureBsp, sbsp));
        Console.WriteLine($"  Loaded sbsp '{bspRef.StructureBsp.Name}' " +
                          $"({sbsp.RuntimeDecals.Count} runtime decals)");
    }
}

// ── Build new palette ──────────────────────────────────────────────────────
// Key: (originalPaletteIndex, scale) -> new palette index
// scale == 0  -> use original palette entry as-is (no duplication)
// scale != 0  -> duplicate decs tag with Radius set to scale
var mapping    = new Dictionary<(short, float), short>();
var newPalette = new List<TagReferenceBlock>();

using (var stream = Cache.OpenCacheReadWrite())
{
    // Pass 1: collect all unique (PaletteIndex, Scale) combinations
    var allKeys = new HashSet<(short, float)>();
    foreach (var (_, sbsp) in sbspData)
        foreach (var decal in sbsp.RuntimeDecals)
            allKeys.Add((decal.PaletteIndex, decal.Scale));

    // Handle scale==0: use original palette entry as-is (no duplication, no radius change)
    var zeroScaleKeys = allKeys
        .Where(k => k.Item2 == 0.0f)
        .OrderBy(k => k.Item1)
        .ToList();

    foreach (var key in zeroScaleKeys)
    {
        var (pi, _) = key;

        if (pi < 0 || pi >= originalPalette.Count || originalPalette[pi].Instance == null)
        {
            Console.WriteLine($"  Warning: PaletteIndex {pi} (scale=0) out of range or null " +
                              $"— adding null entry.");
            mapping[key] = (short)newPalette.Count;
            newPalette.Add(new TagReferenceBlock { Instance = null });
            continue;
        }

        // Reuse if this original tag instance is already in the new palette        
        var existing = newPalette.FindIndex(e => e.Instance == originalPalette[pi].Instance);
        if (existing >= 0)
        {
            mapping[key] = (short)existing;
        }
        else
        {
            short newIndex = (short)newPalette.Count;
            mapping[key] = newIndex;
            newPalette.Add(new TagReferenceBlock { Instance = originalPalette[pi].Instance });
            Console.WriteLine($"  [{newIndex}] {originalPalette[pi].Instance.Name}.decs " +
                              $"(scale=0, original palette[{pi}])");
        }
    }

    // Pass 2: build palette for scale != 0 in original palette order        
    // Sort by original PaletteIndex first, then by Scale as tiebreaker.        
    var orderedKeys = allKeys        
        .Where(k => k.Item2 != 0.0f)
        .OrderBy(k => k.Item1)
        .ThenBy(k => k.Item2)
        .ToList();

    foreach (var key in orderedKeys)
    {
        var (pi, scale) = key;

        // ── Validate original palette index ────────────────────────────
        if (pi < 0 || pi >= originalPalette.Count)
        {
            Console.WriteLine($"  Warning: PaletteIndex {pi} out of range " +
                              $"— adding null entry.");
            mapping[key] = (short)newPalette.Count;
            newPalette.Add(new TagReferenceBlock { Instance = null });
            continue;
        }

        var originalDecsTag = originalPalette[pi].Instance;
        if (originalDecsTag == null)
        {
            Console.WriteLine($"  Warning: Original palette[{pi}] is null " +
                              $"— adding null entry.");
            mapping[key] = (short)newPalette.Count;
            newPalette.Add(new TagReferenceBlock { Instance = null });
            continue;
        }

        // ── Duplicate decs tag with scale suffix ───────────────────────
        var originalDecs = Cache.Deserialize<DecalSystem>(stream, originalDecsTag);

        // e.g. scale 1.5    -> "_x1_5000"        
        //      scale 0.25   -> "_x0_2500"        
        string scaleStr = scale.ToString("F4")
                               .Replace(".", "_")
                               .Replace("-", "n");
        string baseName = originalDecsTag.Name ?? $"0x{originalDecsTag.Index:X4}";
        string newName  = $"{baseName}_x{scaleStr}";

        // Reuse if already created (idempotent re-runs)
        if (!Cache.TagCache.TryGetCachedTag($"{newName}.decs", out CachedTag newDecsTag))
            newDecsTag = Cache.TagCache.AllocateTag<DecalSystem>(newName);

        // ── Set Radius directly to scale ───────────────────────────────
        foreach (var def in originalDecs.Decal)
        {
            def.Radius = new Bounds<float>(scale, scale);
        }
        originalDecs.RuntimeMaxRadius = scale;

        // ── Set MaxOverlapping based on the blend mode ─────────────────────
        bool hasMultiplyBlend = originalDecs.Decal.Any(d =>
            d.RenderMethod?.ShaderProperties != null &&
            d.RenderMethod.ShaderProperties.Count > 0 &&
            (d.RenderMethod.ShaderProperties[0].BlendMode ==
                 RenderMethod.RenderMethodPostprocessBlock.BlendModeValue.Multiply ||
             d.RenderMethod.ShaderProperties[0].BlendMode ==
                 RenderMethod.RenderMethodPostprocessBlock.BlendModeValue.DoubleMultiply));

        if (hasMultiplyBlend)
        {
            originalDecs.MaxOverlapping += 1;
            Console.WriteLine($"  MaxOverlapping incremented to {originalDecs.MaxOverlapping} " +
                              $"(Multiply/DoubleMultiply blend detected in '{newName}')");
        }
        else
        {
            originalDecs.MaxOverlapping = 6;
            Console.WriteLine($"  MaxOverlapping set to 6 " +
                              $"(non-Multiply blend mode in '{newName}')");
        }

        Cache.Serialize(stream, newDecsTag, originalDecs);

        short newIdx = (short)newPalette.Count;
        mapping[key] = newIdx;
        newPalette.Add(new TagReferenceBlock { Instance = newDecsTag });

        Console.WriteLine($"  [{newIdx}] {newName}.decs  " +
                          $"(scale={scale}, from '{originalDecsTag.Name}')");
    }

    // ── Replace scnr decal palette ─────────────────────────────────────────
    scnr.DecalPalette = newPalette;
    Cache.Serialize(stream, scnrTag, scnr);
    Console.WriteLine($"\nScenario decal palette rebuilt ({newPalette.Count} entries).");

    // ── Update PaletteIndex on every runtime decal (preserve original order) ──
    foreach (var (sbspTag, sbsp) in sbspData)
    {
        foreach (var decal in sbsp.RuntimeDecals)
        {
            var key = (decal.PaletteIndex, decal.Scale);
            if (mapping.TryGetValue(key, out short newPaletteIdx))
                decal.PaletteIndex = newPaletteIdx;
        }

        Cache.Serialize(stream, sbspTag, sbsp);
        Console.WriteLine($"Saved sbsp: {sbspTag.Name} " +
                          $"({sbsp.RuntimeDecals.Count} decals).");
    }

    // Save tag name list (HaloOnline only)
    if (Cache is GameCacheHaloOnlineBase hoCache)
        hoCache.SaveTagNames();
}

Console.WriteLine("\nReach Decal Scaling complete.");