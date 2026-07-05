// Script to ignore all blam tags in the specified group during porting
using TagTool.Cache;
using TagTool.Porting;
using System.Linq;

if (Args.Count < 1)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Error: Incorrect Usage!");
    Console.WriteLine("Correct Usage: CS < IgnoreBlamTagGroup.cs <tag_group>");
    Console.ResetColor();
    return;
}

if (PortingContext != null)
{

    // Validate the tag group before using it
    if (!PortingContext.BlamCache.TagCache.TryParseGroupTag(Args[0], out var groupTag))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: Invalid tag group '{Args[0]}'.");
        Console.ResetColor();
        return;
    }

    // Find all tags in the specified group
    var tags = PortingContext.BlamCache.TagCache.FindAllInGroup(groupTag);

    int ignoredCount = 0;
    foreach (var tag in tags)
    {
        if (tag != null)
        {
            PortingContext.IgnoreBlamTags.Add(tag.Index);
            ignoredCount++;
        }
    }

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Added {ignoredCount} {Args[0]} tags to ignore list.");
    Console.ResetColor();
}
else
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Error: PortingContext not found. Make sure you're in a porting context.");
    Console.ResetColor();
}