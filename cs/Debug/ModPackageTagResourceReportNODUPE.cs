using System;  
using System.Collections.Generic;  
using System.IO;  
using System.Linq;  
using TagTool.Cache;  
using TagTool.Cache.HaloOnline;  
using TagTool.Cache.ModPackages;  
using TagTool.Common;  
  
// Record struct to uniquely identify resources  
readonly record struct ResourceDesc(ResourceLocation Location, int Index);  
  
var visitedResources = new HashSet<ResourceDesc>();  
var resourceList = new List<PageableResource>();  
var visitedTags = new HashSet<CachedTagHaloOnline>();  
  
// Check if we're in a mod package context  
if (!(Cache is GameCacheModPackage modCache))  
{  
    Console.WriteLine("Error: This script must be run in a mod package context.");  
    Console.WriteLine("Use 'OpenModPackage' or 'CreateModPackage' first.");  
    return false;  
}  
  
Console.WriteLine("Scanning mod package: {0}", modCache.BaseModPackage.Metadata.Name);  
  
// Get all tags from the mod package cache  
var tags = Cache.TagCache.TagTable.OfType<CachedTagHaloOnline>().ToList();  
  
using (var cacheStream = Cache.OpenCacheRead())  
{  
    foreach (var tag in tags)  
    {  
        CollectTagResources(Cache, cacheStream, tag, resourceList, visitedTags, visitedResources);  
    }  
}  
  
// Output the report  
if (resourceList.Count == 0)  
{  
    Console.WriteLine("No mod package resources found.");  
    return true;  
}  
  
Console.WriteLine("\nMod Package Resource Report (Deduplicated):");  
Console.WriteLine("Total unique mod resources: {0}", resourceList.Count);  
  
long totalUncompressed = 0;  
long totalCompressed = 0;  
var typeSummary = new Dictionary<TagResourceTypeGen3, ResourceTypeSummary>();  
  
foreach (var resource in resourceList)  
{  
    var uncompressedSize = resource.Page.UncompressedBlockSize;  
    var compressedSize = resource.Page.CompressedBlockSize;  
    totalUncompressed += uncompressedSize;  
    totalCompressed += compressedSize;  
  
    if (!typeSummary.ContainsKey(resource.Resource.ResourceType))  
        typeSummary.Add(resource.Resource.ResourceType, new ResourceTypeSummary());  
      
    typeSummary[resource.Resource.ResourceType].UncompressedSize += uncompressedSize;  
    typeSummary[resource.Resource.ResourceType].CompressedSize += compressedSize;  
    typeSummary[resource.Resource.ResourceType].Count++;  
}  
  
Console.WriteLine("\nTotal Compressed: {0}", FormatSize(totalCompressed));  
Console.WriteLine("Total Uncompressed: {0}", FormatSize(totalUncompressed));  
Console.WriteLine("\nBy Type:");  
foreach (var pair in typeSummary.OrderBy(x => x.Key))  
{  
    Console.WriteLine("  {0,-16} Count: {1,5}  Compressed: {2,12}  Uncompressed: {3,12}",  
        pair.Key, pair.Value.Count, FormatSize(pair.Value.CompressedSize), FormatSize(pair.Value.UncompressedSize));  
}  
  
return true;  
  
// Helper methods  
void CollectTagResources(GameCache cache, Stream stream, CachedTagHaloOnline tag,   
    List<PageableResource> resourcesList, HashSet<CachedTagHaloOnline> visitedTags,   
    HashSet<ResourceDesc> visitedResources)  
{  
    if (tag == null || !visitedTags.Add(tag))  
        return;  
  
    var data = cache.Deserialize(stream, tag);  
    CollectTagResources(cache, stream, data, resourcesList, visitedTags, visitedResources);  
}  
  
void CollectTagResources(GameCache cache, Stream stream, object data,   
    List<PageableResource> resourcesList, HashSet<CachedTagHaloOnline> visitedTags,  
    HashSet<ResourceDesc> visitedResources)  
{  
    switch (data)  
    {  
        case PageableResource resource:  
            var location = resource.GetLocation();  
            // Only count resources in the mod package (Mods location)  
            if (location == ResourceLocation.Mods)  
            {  
                var key = new ResourceDesc(location, resource.Page.Index);  
                if (visitedResources.Add(key))  
                    resourcesList.Add(resource);  
            }  
            break;  
              
        case TagStructure tagStruct:  
            foreach (var field in tagStruct.GetTagFieldEnumerable(cache.Version, cache.Platform))  
                CollectTagResources(cache, stream, field.GetValue(data), resourcesList, visitedTags, visitedResources);  
            break;  
              
        case IList list when !(list is byte[]):  
            foreach (var element in list)  
                CollectTagResources(cache, stream, element, resourcesList, visitedTags, visitedResources);  
            break;  
              
        case CachedTagHaloOnline tagRef:  
            CollectTagResources(cache, stream, tagRef, resourcesList, visitedTags, visitedResources);  
            break;  
    }  
}  
  
string FormatSize(double size)  
{  
    const double KB = 1024;  
    const double MB = KB * 1024;  
  
    if (size < KB)  
        return $"{size} B";  
    if (size < MB)  
        return $"{size / KB:0.0} KB";  
    return $"{size / MB:0.0} MB";  
}  
  
class ResourceTypeSummary  
{  
    public long CompressedSize;  
    public long UncompressedSize;  
    public int Count;  
}