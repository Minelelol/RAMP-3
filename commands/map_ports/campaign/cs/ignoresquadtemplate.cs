// Script to ignore all squad_template tags during porting  
using TagTool.Cache;  
using TagTool.Porting;  
using System.Linq;  
  
// Get the current porting context (it's a property of the script context)  
if (PortingContext != null)  
{  
    // Find all squad_template tags (group tag: sqtm)  
    var squadTemplateTags = PortingContext.BlamCache.TagCache.FindAllInGroup("sqtm");  
      
    int ignoredCount = 0;  
    foreach (var tag in squadTemplateTags)  
    {  
        if (tag != null)  
        {  
            PortingContext.IgnoreBlamTags.Add(tag.Index);  
            ignoredCount++;  
        }  
    }  
      
    Console.WriteLine($"Added {ignoredCount} squad_template tags to ignore list.");  
}  
else  
{  
    Console.WriteLine("Error: PortingContext not found. Make sure you're in a porting context.");  
}