// CheckModPackageObjectDependencies.cs  
// Run with: CS < "CheckModPackageObjectDependencies.cs"  
// Run while the mod package is opened as the current cache (Cache = mod package cache)  
  
var groupsToCheck = new[] { "scen", "bloc", "ctrl", "mach" };  
  
var allTags = Cache.TagCache.NonNull()  
    .Cast<TagTool.Cache.HaloOnline.CachedTagHaloOnline>()  
    .ToList();  
  
int checkedCount = 0;  
int noDependentsCount = 0;  
  
foreach (var tag in allTags)  
{  
    if (tag == null)  
        continue;  
  
    if (!groupsToCheck.Contains(tag.Group.Tag.ToString()))  
        continue;  
  
    // Skip stub tags that just proxy to the base cache -  
    // these aren't actually defined in the mod package itself.  
    if (tag.IsEmpty())  
        continue;  
  
    checkedCount++;  
  
    // "ListOn" logic: find every tag whose Dependencies set contains this tag's Index.  
    var dependents = allTags.Where(t => t.Dependencies.Contains(tag.Index)).ToList();  
  
    if (dependents.Count == 0)  
    {  
        Console.WriteLine($"[{tag.Group.Tag}] No dependents: '{tag}'");  
        noDependentsCount++;  
    }  
}  
  
Console.WriteLine($"Done. {noDependentsCount} of {checkedCount} forge tag(s) in the mod package have no dependents.");