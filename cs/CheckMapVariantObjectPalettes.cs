// CheckMapVariantObjectPalettes.cs  
// Run with: CS < "CheckMapVariantObjectPalettes.cs"  
// Must be run while a Reach cache is open as the porting cache  
// (i.e. Cache = destination/base cache, PortingCache = Reach cache)  
  
CachedTag blamScnrTag = PortingCache.TagCache.NonNull().FirstOrDefault(x => x.Group.Tag == "scnr");  
  
if (blamScnrTag == null)  
{  
    Console.WriteLine("No scenario tag ('scnr') was found in the currently opened porting cache.");  
}  
else  
{  
    using (Stream blamCacheStream = PortingCache is GameCacheModPackage package  
        ? package.OpenCacheRead(null)  
        : PortingCache.OpenCacheRead())  
    {  
        var scnr = PortingCache.Deserialize<Scenario>(blamCacheStream, blamScnrTag);  
  
        int missingCount = 0;  
  
        void CheckPalette(string paletteLabel, List<Scenario.ScenarioPaletteEntry> palette)  
        {  
            if (palette == null)  
                return;  
  
            foreach (var entry in palette)  
            {  
                var blamObjectTag = entry.Object;  
  
                if (blamObjectTag == null)  
                    continue;  
  
                var existingTag = Cache.TagCache.GetTag(blamObjectTag.Name, blamObjectTag.Group.Tag);  
  
                if (existingTag == null)  
                {  
                    Console.WriteLine($"[{paletteLabel}] Missing in base cache: '{blamObjectTag}'");  
                    missingCount++;  
                }  
            }  
        }  
  
        CheckPalette("SceneryPalette", scnr.SceneryPalette);  
        CheckPalette("CratePalette", scnr.CratePalette);  
  
        Console.WriteLine($"Done. {missingCount} tag(s) referenced by SceneryPalette/CratePalette are missing from the base cache.");  
    }  
}