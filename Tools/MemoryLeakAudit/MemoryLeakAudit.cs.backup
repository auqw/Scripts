using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class MemoryLeakAudit
{
    static void Main()
    {
        // Base folder
        string scriptsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents", "Skua", "Scripts");

        // Logs folder
        string logsFolder = Path.Combine(scriptsFolder, "Tools", "MemoryLeakAudit", "Logs");
        Directory.CreateDirectory(logsFolder);
        string logFile = Path.Combine(logsFolder, $"MemoryLeakAudit_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

        var excludedFolders = new[] { "WIP", "SkuaScriptsGenerator", "obj", "bin", "Templates", "Tools" };

        // Regex patterns
        var eventAssignRegex = new Regex(@"^\s*Bot\.Events\.\w+\s*\+=\s*.*;", RegexOptions.Compiled);
        var eventRemoveRegex = new Regex(@"^\s*Bot\.Events\.\w+\s*-=\s*.*;", RegexOptions.Compiled);
        var collectionRegex = new Regex(@"\b(List|Dictionary|HashSet)<.*?>\s+(\w+)\s*=", RegexOptions.Compiled);
        var staticCollectionRegex = new Regex(@"\bstatic\s+(List|Dictionary|HashSet)<.*?>\s+(\w+)\s*=", RegexOptions.Compiled);
        var newAllocRegex = new Regex(@"\b=\s*new\s+\w+\s*\(", RegexOptions.Compiled);
        var asyncTimerRegex = new Regex(@"\b(Task|Timer|CancellationTokenSource)\s+\w+", RegexOptions.Compiled);

        int eventCount = 0, collectionCount = 0, staticCollectionCount = 0, newAllocCount = 0, asyncTimerCount = 0;
        int leakedEventCount = 0;

        string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        using StreamWriter writer = new StreamWriter(logFile, false);

        // Get all .cs files
        var files = Directory.GetFiles(scriptsFolder, "*.cs", SearchOption.AllDirectories)
                             .Where(f => !excludedFolders.Any(ex => f.Split(Path.DirectorySeparatorChar)
                                                                   .Any(part => part.Equals(ex, StringComparison.OrdinalIgnoreCase))))
                             .ToArray();

        foreach (var file in files)
        {
            string[] lines = File.ReadAllLines(file);
            var unsubscribedEvents = lines
                .Where(l => !l.TrimStart().StartsWith("//") && eventRemoveRegex.IsMatch(l))
                .Select(l => l.Trim())
                .ToHashSet();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (line.StartsWith("//")) continue; // ignore comments

                string displayFile = file.Replace(userFolder, "%USERPROFILE%");

                // Event assignments
                if (eventAssignRegex.IsMatch(line))
                {
                    eventCount++;
                    // Only report leaked events (never unsubscribed)
                    if (!unsubscribedEvents.Contains(line.Replace("+=", "-=")))
                    {
                        string msg = $"[Leaked Event] {displayFile}({i + 1}): {line}";
                        Console.WriteLine(msg);
                        writer.WriteLine(msg);
                        leakedEventCount++;
                    }
                }

                // Collections (excluding public List<IOption>)
                if (collectionRegex.IsMatch(line) && !Regex.IsMatch(line, @"public\s+List<IOption>\s+\w+\s*="))
                {
                    string msg = $"[Collection] {displayFile}({i + 1}): {line}";
                    Console.WriteLine(msg);
                    writer.WriteLine(msg);
                    collectionCount++;
                }

                // Static collections
                if (staticCollectionRegex.IsMatch(line))
                {
                    string msg = $"[Static Collection] {displayFile}({i + 1}): {line}";
                    Console.WriteLine(msg);
                    writer.WriteLine(msg);
                    staticCollectionCount++;
                }

                // New allocations
                if (newAllocRegex.IsMatch(line))
                {
                    string msg = $"[New Alloc] {displayFile}({i + 1}): {line}";
                    Console.WriteLine(msg);
                    writer.WriteLine(msg);
                    newAllocCount++;
                }

                // Async/Timer objects
                if (asyncTimerRegex.IsMatch(line))
                {
                    string msg = $"[Async/Timer] {displayFile}({i + 1}): {line}";
                    Console.WriteLine(msg);
                    writer.WriteLine(msg);
                    asyncTimerCount++;
                }
            }
        }

        writer.WriteLine();
        writer.WriteLine("--- Memory Leak Audit Summary ---");
        writer.WriteLine($"Event assignments: {eventCount}");
        writer.WriteLine($"Collections (excluding public List<IOption>): {collectionCount}");
        writer.WriteLine($"Static collections: {staticCollectionCount}");
        writer.WriteLine($"New allocations: {newAllocCount}");
        writer.WriteLine($"Async/Timer objects: {asyncTimerCount}");
        writer.WriteLine($"Potential leaked events (never unsubscribed in file): {leakedEventCount}");
        writer.WriteLine("--------------------------------");
        writer.WriteLine($"Audit complete. Log saved to: {logFile}");

        Console.WriteLine("\n--- Memory Leak Audit Summary ---");
        Console.WriteLine($"Event assignments: {eventCount}");
        Console.WriteLine($"Collections (excluding public List<IOption>): {collectionCount}");
        Console.WriteLine($"Static collections: {staticCollectionCount}");
        Console.WriteLine($"New allocations: {newAllocCount}");
        Console.WriteLine($"Async/Timer objects: {asyncTimerCount}");
        Console.WriteLine($"Potential leaked events (never unsubscribed in file): {leakedEventCount}");
        Console.WriteLine("--------------------------------");
        Console.WriteLine($"Audit complete. Log saved to: {logFile}");
    }
}
