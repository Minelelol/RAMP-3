// Script to ignore all .cine (cinematic) tags during porting  
using TagTool.Cache;  
using TagTool.Porting;  
using System.Linq;  
  
// Get the current porting context  
if (PortingContext != null)  
{  
    // Find all .cine tags (group tag: cine)  
    var cinematicTags = PortingContext.BlamCache.TagCache.FindAllInGroup("cine");  
      
    int ignoredCount = 0;  
    foreach (var tag in cinematicTags)  
    {  
        if (tag != null)  
        {  
            PortingContext.IgnoreBlamTags.Add(tag.Index);  
            ignoredCount++;  
        }  
    }  
      
    Console.WriteLine($"Added {ignoredCount} .cine tags to ignore list.");  
}  
else  
{  
    Console.WriteLine("Error: PortingContext not found. Make sure you're in a porting context.");  
}