/*
name: null
description: null
tags: null
version: 1.4.4.4
*/
//cs_include Scripts/CoreBots.cs
using Skua.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class MemoryLeakAudit
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    static void Main()
    {
        // Base folder - use current directory if running from Scripts folder
        string scriptsFolder = Directory.GetCurrentDirectory();
        if (!scriptsFolder.EndsWith("Scripts"))
            scriptsFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Documents",
                "Skua",
                "Scripts"
            );

        Console.WriteLine($"Analyzing scripts in: {scriptsFolder}");

        // Logs folder
        string logsFolder = Path.Combine(scriptsFolder, "Tools", "MemoryLeakAudit", "Logs");
        Directory.CreateDirectory(logsFolder);
        string logFile = Path.Combine(
            logsFolder,
            $"MemoryLeakAudit_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
        );

        // Collection expression instead of `new[] { }`
        string[] excludedFolders =
        [
            "WIP",
            "SkuaScriptsGenerator",
            "obj",
            "bin",
            "Templates",
            "Tools",
            "Logs",
        ];

        // All regex patterns live on one object instead of a positional params array,
        // so AnalyzeFile no longer depends on remembering an index order.
        LeakPatterns patterns = new();

        MemoryLeakStats stats = new();
        Dictionary<string, int> riskFiles = new();

        string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        using StreamWriter writer = new(logFile, false);
        writer.WriteLine($"Memory Leak Audit Report - {DateTime.Now}");
        writer.WriteLine($"Analyzing: {scriptsFolder}");
        writer.WriteLine("=" + new string('=', 50));
        writer.WriteLine();

        // Get all .cs files
        string[] files = Directory
            .GetFiles(scriptsFolder, "*.cs", SearchOption.AllDirectories)
            .Where(f =>
                !excludedFolders.Any(ex =>
                    f.Split(Path.DirectorySeparatorChar)
                        .Any(part => part.Equals(ex, StringComparison.OrdinalIgnoreCase))
                )
            )
            .ToArray();

        Console.WriteLine($"Found {files.Length} C# files to analyze...");

        foreach (string file in files)
        {
            int fileRisk = AnalyzeFile(file, writer, userFolder, stats, patterns);

            if (fileRisk > 0)
                riskFiles[file] = fileRisk;
        }

        // Summary
        WriteSummary(writer, stats, riskFiles, logFile);
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("MEMORY LEAK AUDIT SUMMARY");
        Console.WriteLine("=" + new string('=', 60));
        Console.WriteLine($"Total files analyzed: {files.Length}");
        Console.WriteLine($"Event subscriptions found: {stats.EventCount}");
        Console.WriteLine($"Potentially leaked events: {stats.LeakedEventCount} ⚠️");
        Console.WriteLine($"Task.Run calls without cancellation: {stats.TaskRunCount} ⚠️");
        Console.WriteLine($"Timer creations: {stats.TimerCount} ⚠️");
        Console.WriteLine($"Handler registrations: {stats.HandlerCount}");
        Console.WriteLine($"Static collections: {stats.StaticCollectionCount}");
        Console.WriteLine($"Files with high risk: {riskFiles.Count(kv => kv.Value >= 5)}");
        Console.WriteLine(
            $"Files with medium risk: {riskFiles.Count(kv => kv.Value >= 3 && kv.Value < 5)}"
        );
        Console.WriteLine("=" + new string('=', 60));
        Console.WriteLine($"Full report saved to: {logFile}");

        if (stats.LeakedEventCount > 0 || stats.TaskRunCount > 5)
        {
            Console.WriteLine("\n⚠️  WARNING: Potential memory leaks detected!");
            Console.WriteLine("Review the full report for details.");
        }
    }

    static int AnalyzeFile(
        string file,
        StreamWriter writer,
        string userFolder,
        MemoryLeakStats stats,
        LeakPatterns patterns
    )
    {
        int fileRiskScore = 0;
        string content = File.ReadAllText(file);
        string[] lines = File.ReadAllLines(file);
        string displayFile = file.Replace(userFolder, "%USERPROFILE%").Replace("\\", "/");

        // Find all event unsubscriptions
        HashSet<string> unsubscribedEvents = lines
            .Where(l => !l.TrimStart().StartsWith("//") && patterns.EventRemove.IsMatch(l))
            .Select(l => l.Trim().Replace("-=", "+="))
            .ToHashSet();

        bool hasFileIssues = false;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("//") || string.IsNullOrWhiteSpace(line))
                continue;

            // Event assignments
            if (patterns.EventAssign.IsMatch(line) && !unsubscribedEvents.Contains(line))
            {
                stats.EventCount++;
                if (!hasFileIssues)
                {
                    writer.WriteLine($"\n🔍 FILE: {displayFile}");
                    hasFileIssues = true;
                }
                writer.WriteLine($"  ⚠️  [LEAKED EVENT] Line {i + 1}: {line}");
                stats.LeakedEventCount++;
                fileRiskScore += 3;
            }
            else if (patterns.EventAssign.IsMatch(line))
            {
                // Subscribed and later unsubscribed - still counts toward the total, no warning
                stats.EventCount++;
            }

            // Task.Run without proper disposal/cancellation
            if (patterns.TaskRun.IsMatch(line))
            {
                stats.TaskRunCount++;
                bool hasCancellation = content.Contains("CancellationToken");
                if (!hasCancellation)
                {
                    if (!hasFileIssues)
                    {
                        writer.WriteLine($"\n🔍 FILE: {displayFile}");
                        hasFileIssues = true;
                    }
                    writer.WriteLine($"  ⚠️  [TASK WITHOUT CANCELLATION] Line {i + 1}: {line}");
                    fileRiskScore += 2;
                }
            }

            // Timer creations
            if (patterns.Timer.IsMatch(line))
            {
                stats.TimerCount++;
                if (!hasFileIssues)
                {
                    writer.WriteLine($"\n🔍 FILE: {displayFile}");
                    hasFileIssues = true;
                }
                writer.WriteLine($"  ⚠️  [TIMER CREATION] Line {i + 1}: {line}");
                fileRiskScore += 2;
            }

            // Handler registrations
            if (patterns.Handler.IsMatch(line))
            {
                stats.HandlerCount++;
                if (!hasFileIssues)
                {
                    writer.WriteLine($"\n🔍 FILE: {displayFile}");
                    hasFileIssues = true;
                }
                writer.WriteLine($"  ℹ️  [HANDLER REGISTRATION] Line {i + 1}: {line}");
            }

            // Static collections
            if (patterns.StaticCollection.IsMatch(line))
            {
                stats.StaticCollectionCount++;
                if (!hasFileIssues)
                {
                    writer.WriteLine($"\n🔍 FILE: {displayFile}");
                    hasFileIssues = true;
                }
                writer.WriteLine($"  ℹ️  [STATIC COLLECTION] Line {i + 1}: {line}");
                fileRiskScore += 1;
            }
        }

        return fileRiskScore;
    }

    static void WriteSummary(
        StreamWriter writer,
        MemoryLeakStats stats,
        Dictionary<string, int> riskFiles,
        string logFile
    )
    {
        writer.WriteLine("\n" + "=" + new string('=', 60));
        writer.WriteLine("MEMORY LEAK AUDIT SUMMARY");
        writer.WriteLine("=" + new string('=', 60));
        writer.WriteLine($"Event subscriptions found: {stats.EventCount}");
        writer.WriteLine($"Potentially leaked events: {stats.LeakedEventCount}");
        writer.WriteLine($"Task.Run calls: {stats.TaskRunCount}");
        writer.WriteLine($"Timer creations: {stats.TimerCount}");
        writer.WriteLine($"Handler registrations: {stats.HandlerCount}");
        writer.WriteLine($"Static collections: {stats.StaticCollectionCount}");
        writer.WriteLine();

        if (riskFiles.Any())
        {
            List<KeyValuePair<string, int>> highRisk = riskFiles
                .Where(kv => kv.Value >= 5)
                .OrderByDescending(kv => kv.Value)
                .ToList();
            List<KeyValuePair<string, int>> mediumRisk = riskFiles
                .Where(kv => kv.Value is >= 3 and < 5)
                .OrderByDescending(kv => kv.Value)
                .ToList();

            writer.WriteLine("HIGH RISK FILES (Score >= 5):");
            foreach (KeyValuePair<string, int> file in highRisk)
                writer.WriteLine($"  {file.Key} (Risk Score: {file.Value})");

            writer.WriteLine("\nMEDIUM RISK FILES (Score 3-4):");
            foreach (KeyValuePair<string, int> file in mediumRisk)
                writer.WriteLine($"  {file.Key} (Risk Score: {file.Value})");
        }

        writer.WriteLine("\nRECOMMENDATIONS:");
        writer.WriteLine("1. Review all leaked event subscriptions and add proper unsubscription");
        writer.WriteLine("2. Add CancellationToken support to Task.Run operations");
        writer.WriteLine("3. Ensure Timer objects are properly disposed");
        writer.WriteLine("4. Consider implementing IDisposable for Core classes");
        writer.WriteLine($"\nReport generated: {DateTime.Now}");
    }
}

/// <summary>
/// Compiled regex patterns used by the audit. Previously these were built in Main,
/// passed through a `params Regex[]` and pulled back out by hand-counted index in
/// AnalyzeFile (fragile - reordering the array silently mismatched the names).
/// Four of the original patterns (TaskCreation, Collection, Dispose, Using) were
/// declared but never actually consulted anywhere in the analysis logic, so they've
/// been dropped rather than carried along as dead code.
/// </summary>
sealed class LeakPatterns
{
    public Regex EventAssign { get; }
    public Regex EventRemove { get; }
    public Regex TaskRun { get; }
    public Regex Timer { get; }
    public Regex StaticCollection { get; }
    public Regex Handler { get; }

    public LeakPatterns()
    {
        EventAssign = new(
            @"^\s*Bot\.Events\.\w+\s*\+=\s*.*[^;]*;?\s*$",
            RegexOptions.Compiled | RegexOptions.Multiline
        );
        EventRemove = new(
            @"^\s*Bot\.Events\.\w+\s*-=\s*.*[^;]*;?\s*$",
            RegexOptions.Compiled | RegexOptions.Multiline
        );
        TaskRun = new(@"\bTask\.Run\s*\(", RegexOptions.Compiled);
        Timer = new(@"new\s+(Timer|System\.Threading\.Timer)\s*\(", RegexOptions.Compiled);
        StaticCollection = new(
            @"\bstatic\s+(List|Dictionary|HashSet|Queue|Stack)<.*?>\s+(\w+)\s*=",
            RegexOptions.Compiled
        );
        Handler = new(@"Bot\.Handlers\.RegisterHandler\s*\(", RegexOptions.Compiled);
    }
}

class MemoryLeakStats
{
    public int EventCount { get; set; }
    public int LeakedEventCount { get; set; }
    public int TaskRunCount { get; set; }
    public int TimerCount { get; set; }
    public int HandlerCount { get; set; }
    public int StaticCollectionCount { get; set; }
}