using System;  
using System.IO;  
using TagTool.Cache;  
using TagTool.Cache.HaloOnline;  
using TagTool.Tags;  
using TagTool.Tags.Definitions;  
  
var effectGroup = new Tag("effe");  
var modifiedCount = 0;  
  
Console.WriteLine("Scanning modpackage effect tags for InverseLodFeatherIn values...");  
  
foreach (var tag in Cache.TagCache.TagTable)  
{  
    if (tag == null || !tag.IsInGroup(effectGroup))  
        continue;  
  
    // Only process tags that are actually in the modpackage (not base cache references)  
    var cachedTagHO = tag as CachedTagHaloOnline;  
    if (cachedTagHO == null || cachedTagHO.IsEmpty())  
        continue;  
  
    var tagName = tag.Name ?? $"0x{tag.Index:X4}";  
    Console.WriteLine($"Processing modpackage tag {tagName}...");  
      
    using (var stream = Cache.OpenCacheReadWrite())  
    {  
        var effect = Cache.Deserialize<Effect>(stream, tag);  
        var hasChanges = false;  
          
        foreach (var eventBlock in effect.Events)  
        {  
            foreach (var particleSystem in eventBlock.ParticleSystems)  
            {  
                if (particleSystem.InverseLodFeatherIn == 10000.0f)  
                {  
                    Console.WriteLine($"  Found InverseLodFeatherIn = 10000 in particle system, setting to 1");  
                    particleSystem.InverseLodFeatherIn = 1.0f;  
                    hasChanges = true;  
                }  
            }  
        }  
          
        if (hasChanges)  
        {  
            Cache.Serialize(stream, tag, effect);  
            modifiedCount++;  
        }  
    }  
}  
  
Console.WriteLine($"Complete! Modified {modifiedCount} modpackage effect tags.");