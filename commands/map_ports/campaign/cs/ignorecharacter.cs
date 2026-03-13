// Script to ignore all character tags during porting  
using TagTool.Cache;  
using TagTool.Porting;  
using System.Linq;  
  
// Get the current porting context (it's a property of the script context)  
if (PortingContext != null)  
{  
    // Find all character tags (group tag: char)  
    var characterTags = PortingContext.BlamCache.TagCache.FindAllInGroup("char");  
      
    int ignoredCount = 0;  
    foreach (var tag in characterTags)  
    {  
        if (tag != null)  
        {  
            PortingContext.IgnoreBlamTags.Add(tag.Index);  
            ignoredCount++;  
        }  
    }  
      
    Console.WriteLine($"Added {ignoredCount} character tags to ignore list.");  
}  
else  
{  
    Console.WriteLine("Error: PortingContext not found. Make sure you're in a porting context.");  
}