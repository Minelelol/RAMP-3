// AffixTriggerVolumeNames.cs  
// Run with: CS < "AffixTriggerVolumeNames.cs"  
// Run from inside the scenario tag's own edit context  
  
Scenario scnr = Definition;   // the exact tag/definition already loaded for this shell context  
CachedTag scnrTag = Tag;  
  
Console.WriteLine($"[Debug] Editing tag: {scnrTag.Name}.{scnrTag.Group}");  
Console.WriteLine($"[Debug] TriggerVolumes count: {scnr.TriggerVolumes.Count}");  
  
var nameCounts = new Dictionary<string, int>();  
  
foreach (var volume in scnr.TriggerVolumes)  
{  
    string baseName = Cache.StringTable.GetString(volume.Name);  
  
    if (baseName == null)  
    {  
        Console.WriteLine("[Debug] Skipped a volume with null/unresolved Name StringId.");  
        continue;  
    }  
  
    // Skip names that already end in a numeric suffix (e.g. "_1")  
    // so re-running the script doesn't double-suffix them.  
    var match = System.Text.RegularExpressions.Regex.Match(baseName, @"^(.*)_(\d+)$");  
    string root = match.Success ? match.Groups[1].Value : baseName;  
  
    int count = nameCounts.TryGetValue(root, out var c) ? c + 1 : 1;  
    nameCounts[root] = count;  
  
    string newName = $"{root}_{count}";  
  
    if (newName == baseName)  
        continue;  
  
    volume.Name = Cache.StringTable.GetOrAddString(newName);  
    Console.WriteLine($"Renamed '{baseName}' -> '{newName}'");  
}  
  
using (Stream stream = Cache.OpenCacheReadWrite())  
    Cache.Serialize(stream, scnrTag, scnr);  
  
Cache.SaveStrings();  
Console.WriteLine("Done. Re-enter the tag context (exit and navigate back in) to see refreshed field values.");