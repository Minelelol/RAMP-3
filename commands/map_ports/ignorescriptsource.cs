// Script to ignore all tags referenced in Script Source File References  
using TagTool.Cache;  
using TagTool.Porting;  
using TagTool.Tags.Definitions;  
using System.Linq;  
  
// Get the current porting context  
if (PortingContext != null)  
{  
    // Find the scenario tag (scnr)  
    var scenarioTag = PortingContext.BlamCache.TagCache.FindFirstInGroup("scnr");  
      
    if (scenarioTag != null)  
    {  
        // Open a stream from the cache and deserialize the scenario  
        using (var stream = PortingContext.BlamCache.OpenCacheRead())  
        {  
            var scenario = PortingContext.BlamCache.Deserialize<Scenario>(stream, scenarioTag);  
              
            int ignoredCount = 0;  
              
            // Iterate through all script source file references  
            if (scenario.ScriptSourceFileReferences != null)  
            {  
                foreach (var reference in scenario.ScriptSourceFileReferences)  
                {  
                    if (reference?.Instance != null)  
                    {  
                        PortingContext.IgnoreBlamTags.Add(reference.Instance.Index);  
                        ignoredCount++;  
                        Console.WriteLine($"Added {reference.Instance} to ignore list.");  
                    }  
                }  
            }  
              
            Console.WriteLine($"Added {ignoredCount} tags from Script Source File References to ignore list.");  
        }  
    }  
    else  
    {  
        Console.WriteLine("Error: No scenario tag found in cache.");  
    }  
}  
else  
{  
    Console.WriteLine("Error: PortingContext not found. Make sure you're in a porting context.");  
}