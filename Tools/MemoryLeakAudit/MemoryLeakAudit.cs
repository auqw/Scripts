using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

class MemoryLeakAudit
{
    static void Main()
    {
        // Base folder - use current directory if running from Scripts folder
        string scriptsFolder = Directory.GetCurrentDirectory();
        if (!scriptsFolder.EndsWith("Scripts"))
        {
            scriptsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents", "Skua", "Scripts");
        }

        Console.WriteLine($"Analyzing scripts in: {scriptsFolder}");

        // Logs folder
        string logsFolder = Path.Combine(scriptsFolder, "Tools", "MemoryLeakAudit", "Logs");
        Directory.CreateDirectory(logsFolder);
        string logFile = Path.Combine(logsFolder, $"MemoryLeakAudit_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

        var excludedFolders = new[] { "WIP", "SkuaScriptsGenerator", "obj", "bin", "Templates", "Tools", "Logs" };

        // Improved regex patterns
        var eventAssignRegex = new Regex(@"^\s*Bot\.Events\.\w+\s*\+=\s*.*[^;]*;?\s*$", RegexOptions.Compiled | RegexOptions.Multiline);
        var eventRemoveRegex = new Regex(@"^\s*Bot\.Events\.\w+\s*-=\s*.*[^;]*;?\s*$", RegexOptions.Compiled | RegexOptions.Multiline);
        var taskRunRegex = new Regex(@"\bTask\.Run\s*\(", RegexOptions.Compiled);
        var taskCreationRegex = new Regex(@"new\s+Task\s*\(", RegexOptions.Compiled);
        var timerRegex = new Regex(@"new\s+(Timer|System\.Threading\.Timer)\s*\(", RegexOptions.Compiled);
        var collectionRegex = new Regex(@"\b(List|Dictionary|HashSet|Queue|Stack)<.*?>\s+(\w+)\s*=", RegexOptions.Compiled);
        var staticCollectionRegex = new Regex(@"\bstatic\s+(List|Dictionary|HashSet|Queue|Stack)<.*?>\s+(\w+)\s*=", RegexOptions.Compiled);
        var handlerRegex = new Regex(@"Bot\.Handlers\.RegisterHandler\s*\(", RegexOptions.Compiled);
        var disposeRegex = new Regex(@"\.Dispose\s*\(\s*\)", RegexOptions.Compiled);
        var usingRegex = new Regex(@"^\s*using\s*\(", RegexOptions.Compiled);

        var stats = new MemoryLeakStats();
        var riskFiles = new Dictionary<string, int>();

        string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        using StreamWriter writer = new StreamWriter(logFile, false);
        writer.WriteLine($"Memory Leak Audit Report - {DateTime.Now}");
        writer.WriteLine($"Analyzing: {scriptsFolder}");
        writer.WriteLine("=" + new string('=', 50));
        writer.WriteLine();

        // Get all .cs files
        var files = Directory.GetFiles(scriptsFolder, "*.cs", SearchOption.AllDirectories)
                             .Where(f => !excludedFolders.Any(ex => f.Split(Path.DirectorySeparatorChar)
                                                                   .Any(part => part.Equals(ex, StringComparison.OrdinalIgnoreCase))))
                             .ToArray();

        Console.WriteLine($"Found {files.Length} C# files to analyze...");

        foreach (var file in files)
        {
            var fileRisk = AnalyzeFile(file, writer, userFolder, stats, 
                eventAssignRegex, eventRemoveRegex, taskRunRegex, taskCreationRegex, 
                timerRegex, collectionRegex, staticCollectionRegex, handlerRegex, 
                disposeRegex, usingRegex);
            
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
        Console.WriteLine($"Files with medium risk: {riskFiles.Count(kv => kv.Value >= 3 && kv.Value < 5)}");
        Console.WriteLine("=" + new string('=', 60));
        Console.WriteLine($"Full report saved to: {logFile}");

        if (stats.LeakedEventCount > 0 || stats.TaskRunCount > 5)
        {
            Console.WriteLine("\n⚠️  WARNING: Potential memory leaks detected!");
            Console.WriteLine("Review the full report for details.");
        }
    }

    static int AnalyzeFile(string file, StreamWriter writer, string userFolder, MemoryLeakStats stats,
        params Regex[] regexes)
    {
        var (eventAssignRegex, eventRemoveRegex, taskRunRegex, taskCreationRegex, 
             timerRegex, collectionRegex, staticCollectionRegex, handlerRegex, 
             disposeRegex, usingRegex) = (regexes[0], regexes[1], regexes[2], regexes[3], 
                                          regexes[4], regexes[5], regexes[6], regexes[7], 
                                          regexes[8], regexes[9]);

        int fileRiskScore = 0;
        string content = File.ReadAllText(file);
        string[] lines = File.ReadAllLines(file);
        string displayFile = file.Replace(userFolder, "%USERPROFILE%").Replace("\\", "/");

        // Find all event unsubscriptions
        var unsubscribedEvents = lines
            .Where(l => !l.TrimStart().StartsWith("//") && eventRemoveRegex.IsMatch(l))
            .Select(l => l.Trim().Replace("-=", "+="))
            .ToHashSet();

        bool hasFileIssues = false;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("//") || string.IsNullOrWhiteSpace(line)) continue;

            // Event assignments
            if (eventAssignRegex.IsMatch(line))
            {
                stats.EventCount++;
                if (!unsubscribedEvents.Contains(line))
                {
                    if (!hasFileIssues)
                    {
                        writer.WriteLine($"\n🔍 FILE: {displayFile}");
                        hasFileIssues = true;
                    }
                    writer.WriteLine($"  ⚠️  [LEAKED EVENT] Line {i + 1}: {line}");
                    stats.LeakedEventCount++;
                    fileRiskScore += 3;
                }
            }

            // Task.Run without proper disposal/cancellation
            if (taskRunRegex.IsMatch(line))
            {
                stats.TaskRunCount++;
                // Check if there's a CancellationToken in the vicinity
                bool hasCancellation = content.Contains("CancellationToken") || 
                                     line.Contains("cancellation") || 
                                     line.Contains("CancellationToken");
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
            if (timerRegex.IsMatch(line))
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
            if (handlerRegex.IsMatch(line))
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
            if (staticCollectionRegex.IsMatch(line))
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

    static void WriteSummary(StreamWriter writer, MemoryLeakStats stats, Dictionary<string, int> riskFiles, string logFile)
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
            writer.WriteLine("HIGH RISK FILES (Score >= 5):");
            foreach (var file in riskFiles.Where(kv => kv.Value >= 5).OrderByDescending(kv => kv.Value))
            {
                writer.WriteLine($"  {file.Key} (Risk Score: {file.Value})");
            }

            writer.WriteLine("\nMEDIUM RISK FILES (Score 3-4):");
            foreach (var file in riskFiles.Where(kv => kv.Value >= 3 && kv.Value < 5).OrderByDescending(kv => kv.Value))
            {
                writer.WriteLine($"  {file.Key} (Risk Score: {file.Value})");
            }
        }

        writer.WriteLine("\nRECOMMENDATIONS:");
        writer.WriteLine("1. Review all leaked event subscriptions and add proper unsubscription");
        writer.WriteLine("2. Add CancellationToken support to Task.Run operations");
        writer.WriteLine("3. Ensure Timer objects are properly disposed");
        writer.WriteLine("4. Consider implementing IDisposable for Core classes");
        writer.WriteLine($"\nReport generated: {DateTime.Now}");
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