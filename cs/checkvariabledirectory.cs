using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

Console.WriteLine();
Console.WriteLine("----------------------------");
Console.WriteLine("| Variable Path Validation |");
Console.WriteLine("----------------------------");
Console.WriteLine();

int pathCount = 0;
int validCount = 0;
int invalidCount = 0;
var invalidPaths = new List<string>();

foreach (var variable in ContextStack.ArgumentVariables)
{
    string varName = variable.Key;
    string varValue = variable.Value;
    
    if (string.IsNullOrWhiteSpace(varValue) || varValue.Length < 2)
        continue;
    
    bool isPath = varValue.Contains("\\") ||
                  varValue.Contains("/") ||
                  (varValue.Length > 1 && varValue[1] == ':') ||
                  varValue.StartsWith(@"\\");
    
    if (!isPath)
        continue;
    
    pathCount++;
    Console.WriteLine($"Variable: {varName}");
    Console.WriteLine($"  Value: {varValue}");
    
    try
    {
        if (File.Exists(varValue) || Directory.Exists(varValue))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Valid path");
            Console.ResetColor();
            validCount++;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Path does NOT exist");
            Console.ResetColor();
            invalidPaths.Add($"{varName} = {varValue}");
            invalidCount++;
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error checking path: {ex.Message}");
        Console.ResetColor();
        invalidPaths.Add($"{varName} = {varValue} (Error: {ex.Message})");
        invalidCount++;
    }
    
    Console.WriteLine();
}

Console.WriteLine("-------------------------");
Console.WriteLine("| Variable Path Summary |");
Console.WriteLine("-------------------------");
Console.WriteLine($"Total path-like variables: {pathCount}");
Console.WriteLine($"Valid paths: {validCount}");
Console.WriteLine($"Invalid paths: {invalidCount}");

if (invalidCount > 0)
{
    string errorMsg = $"The following path variable(s) does not exist:\n" +
                     string.Join("\n", invalidPaths.Select(p => $"  - {p}"));
    
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("ERROR: " + errorMsg);
    Console.ResetColor();

    Environment.Exit(1);
}