// List all ignored blam tags  
var portingContext = (TagTool.Porting.PortingContext)PortingContext;  
var blamCache = portingContext.BlamCache;  
  
Console.WriteLine($"Ignored Blam Tags ({portingContext.IgnoreBlamTags.Count}):");  
Console.WriteLine("------------------------------------------------");  
  
if (portingContext.IgnoreBlamTags.Count == 0)  
{  
    Console.WriteLine("No tags are currently ignored.");  
}  
else  
{  
    foreach (var tagIndex in portingContext.IgnoreBlamTags)  
    {  
        if (tagIndex >= 0 && tagIndex < blamCache.TagCache.TagTable.Count())  
        {  
            var tag = blamCache.TagCache.TagTable.ElementAt(tagIndex);  
            if (tag != null)  
            {  
                Console.WriteLine($"[0x{tagIndex:X4}] {tag}");  
            }  
            else  
            {  
                Console.WriteLine($"[0x{tagIndex:X4}] <null tag>");  
            }  
        }  
    }  
}