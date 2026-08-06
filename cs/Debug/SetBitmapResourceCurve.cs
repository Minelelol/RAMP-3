using TagTool.Cache.HaloOnline;  
using TagTool.Bitmaps;  

if (Args.Count < 2)
{
    Console.WriteLine("Incorrect Usage!");
    Console.WriteLine("Correct Usage: CS < SetBitmapCurveResource.cs <image_index> <curve_flag>");
    return;
}

if (!int.TryParse(Args[0], out int imageIndex) || imageIndex >= Definition.Images.Count)
{
    Console.WriteLine($"Invalid image index: {Args[0]}.");
    return;
}

if (!Enum.TryParse<BitmapImageCurve>(Args[1], ignoreCase: true, out BitmapImageCurve curve))
{
    Console.WriteLine($"Invalid curve flag: {Args[1]}.");
    Console.WriteLine("Valid curve flag: Unknown, xRGB, Gamma2, Linear, OffsetLog, sRGB, Rec709");
    return;
}

var resourceDefinition = Cache.ResourceCache.GetBitmapTextureInteropResource(Definition.HardwareTextures[imageIndex]);
if (resourceDefinition == null)
{
    Console.WriteLine("No bitmap resource found.");
    return;
}

// Update the curve flag in the resource definition
resourceDefinition.Texture.Definition.Bitmap.Curve = curve;
  
// Update the curve flag in the tag definition
Definition.Images[imageIndex].Curve = curve;

// Replace the resource with the updated definition
var hoCache = (GameCacheHaloOnlineBase)Cache;
hoCache.ResourceCaches.ReplaceResource(Definition.HardwareTextures[imageIndex].HaloOnlinePageableResource, resourceDefinition);
  
// Serialize the tag
using (var stream = Cache.OpenCacheReadWrite())
    Cache.Serialize(stream, Tag, Definition);

Console.WriteLine($"Successfully set the curve flag to {curve} for image index {imageIndex}.");