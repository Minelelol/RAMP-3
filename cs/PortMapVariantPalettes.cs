// PortMapVariantPalettes.cs
// Run with: CS < "PortMapVariantPalettes.cs"
// Must be run while a Reach cache is open as the porting cache
// (i.e. Cache = destination, PortingCache = Reach cache, PortingContext = active PortingContext)

CachedTag blamScnrTag = PortingCache.TagCache.NonNull().FirstOrDefault(x => x.Group.Tag == "scnr");

if (blamScnrTag == null)
{
    Console.WriteLine("No scenario tag ('scnr') was found in the currently opened porting cache.");
}
else
{
    var excludedPaletteNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "ff_weapons_human",
        "ff_weapons_covenant",
        "ff_armor_abilities",
        "ff_vehicles",
        "ff_spawning",
        "ff_objectives"
    };

    using (Stream cacheStream = ((GameCacheHaloOnlineBase)Cache).OpenCacheReadWrite())
    using (Stream blamCacheStream = PortingCache is GameCacheModPackage package  
        ? package.OpenCacheRead(cacheStream)
        : PortingCache.OpenCacheRead())
    {
        var scnr = PortingCache.Deserialize<Scenario>(blamCacheStream, blamScnrTag);

        if (scnr.MapVariantPalettes == null || scnr.MapVariantPalettes.Count == 0)
        {
            Console.WriteLine("Scenario has no Map Variant Palettes.");
        }
        else
        {
            using (var portScope = PortingContext.CreateScope(PortingFlags.Default))
            {
                var portedTags = new HashSet<CachedTag>();
                int portedCount = 0;
                int skippedCount = 0;
                int skippedPaletteCount = 0;

                foreach (var palette in scnr.MapVariantPalettes)
                {
                    var paletteName = PortingCache.StringTable.GetString(palette.Name);

                    if (excludedPaletteNames.Contains(paletteName))
                    {
                        Console.WriteLine($"Skipping excluded palette '{paletteName}'.");
                        skippedPaletteCount++;
                        continue;
                    }

                    foreach (var entry in palette.Entries)
                    {
                        var entryName = PortingCache.StringTable.GetString(entry.Name);

                        foreach (var variant in entry.Variants)
                        {
                            var blamObjectTag = variant.Object;

                            if (blamObjectTag == null)
                                continue;

                            if (!portedTags.Add(blamObjectTag))
                            {
                                skippedCount++;
                                continue;
                            }

                            try
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"Porting '{blamObjectTag}' (palette '{paletteName}', entry '{entryName}')...");
                                Console.ResetColor();
                                PortingContext.ConvertTag(cacheStream, blamCacheStream, blamObjectTag);
                                portedCount++;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ex.GetType().Name} while porting '{blamObjectTag}': {ex.Message}");
                            }
                        }
                    }
                }

                //Console.ForegroundColor = ConsoleColor.Green;
                //Console.WriteLine($"Done. Ported {portedCount} distinct tag(s), skipped {skippedCount} duplicate reference(s), excluded {skippedPaletteCount} palette(s).");
                //Console.ResetColor();
            }
        }
    }
}