//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreAdvanced2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreUltra2.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Skua.Core.Interfaces;

public class UltraCustomClassSync
{
    public static string CustomClassSync(
        dynamic ultra,
        IScriptInterface bot,
        string[][] classSlots,
        int armySize,
        string syncFilePath = "class_assign.sync",
        bool allowDuplicates = false,
        Dictionary<string, string>? preferredUsernameAssignments = null
    )
    {
        if (classSlots == null || classSlots.Length == 0 || armySize < 1)
            return string.Empty;

        string syncFile = ultra.ResolveSyncPath(syncFilePath);

        // Clear stale file (>15 min old)
        try
        {
            if (
                File.Exists(syncFile)
                && (DateTime.UtcNow - File.GetLastWriteTimeUtc(syncFile)).TotalMinutes > 15
            )
                File.WriteAllText(syncFile, "");
        }
        catch { }

        // Collect every unique class name across all slots
        HashSet<string> allNeeded = new(StringComparer.OrdinalIgnoreCase);
        foreach (string[] slot in classSlots)
            foreach (string cls in slot)
                allNeeded.Add(cls);

        // Check which of those this client owns (inventory + bank)
        List<string> myClasses = new();
        foreach (string cls in allNeeded)
        {
            if (CoreBots2.Instance.CheckInventory(cls, toInv: true))
                myClasses.Add(cls);
        }

        string username = bot.Player.Username;
        string payload = $"READY|{string.Join(",", myClasses)}";
        ultra.UpdateEntry(syncFile, username, payload);
        bot.Log(
            "[CustomClassSync] Account owns: " + (myClasses.Count > 0 ? string.Join(",", myClasses) : "NONE of the needed classes")
        );

        // Wait for all members to register (with 5 min timeout)
        const int staleThreshold = 600; // 10 minutes
        const int registrationTimeoutSec = 300;
        DateTime waitStart = DateTime.UtcNow;
        int lastCount = -1;

        while (!bot.ShouldExit)
        {
            if ((DateTime.UtcNow - waitStart).TotalSeconds > registrationTimeoutSec)
            {
                bot.Log($"[CustomClassSync] Timed out after {registrationTimeoutSec}s waiting for {armySize} clients (got {lastCount}). Aborting.");
                bot.StopSync();
                return string.Empty;
            }

            string[] lines = ultra.ReadLines(syncFile);
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            int validCount = 0;

            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length < 3)
                    continue;
                if (!parts[1].StartsWith("READY|", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (!long.TryParse(parts[parts.Length - 1], out long ts))
                    continue;
                if (now - ts <= staleThreshold)
                    validCount++;
            }

            if (validCount != lastCount)
            {
                lastCount = validCount;
                bot.Log($"[CustomClassSync] Registered: {validCount}/{armySize}");
            }

            if (validCount >= armySize)
                break;

            // Re-poke to keep entry fresh
            ultra.UpdateEntry(syncFile, username, payload);
            bot.Sleep(500);
        }

        if (bot.ShouldExit)
            return string.Empty;

        // Small buffer to ensure file system has flushed all writes
        bot.Sleep(500);

        // Build sorted account list once for friendly log labels
        string[] _rawLines = ultra.ReadLines(syncFile);
        List<string> _sortedAccounts = _rawLines
            .Select(l => l.Split(':')[0])
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(u => u, StringComparer.OrdinalIgnoreCase)
            .ToList();

        string AccountLabel(string user)
        {
            int idx = _sortedAccounts.FindIndex(a => string.Equals(a, user, StringComparison.OrdinalIgnoreCase));
            return idx >= 0 ? $"Account-{idx + 1}" : user;
        }

        // Parse entries: player - owned classes
        Dictionary<string, List<string>> playerClasses =
            new(StringComparer.OrdinalIgnoreCase);
        {
            string[] lines = ultra.ReadLines(syncFile);
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length < 3)
                    continue;

                string playerName = parts[0];
                string payloadPart = parts[1];

                if (!long.TryParse(parts[parts.Length - 1], out long ts))
                    continue;
                if (now - ts > staleThreshold)
                    continue;

                // Handle both formats: "READY|class1,class2" or "class1,class2"
                string classListStr = payloadPart;
                if (payloadPart.StartsWith("READY|", StringComparison.OrdinalIgnoreCase))
                    classListStr = payloadPart.Substring(6); // Remove "READY|" prefix

                playerClasses[playerName] = classListStr
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            }
        }

        bot.Log($"[CustomClassSync] {playerClasses.Count} player(s) registered. Assigning classes...");

        // Pre-count how many times each class appears across all slot definitions
        // This determines the max allowed duplicates for each class
        Dictionary<string, int> classMaxCount = new(StringComparer.OrdinalIgnoreCase);
        if (allowDuplicates)
        {
            foreach (string[] slot in classSlots)
            {
                foreach (string cls in slot)
                {
                    if (!classMaxCount.ContainsKey(cls))
                        classMaxCount[cls] = 0;
                    classMaxCount[cls]++;
                }
            }
        }

        Dictionary<string, string> assignments = new(StringComparer.OrdinalIgnoreCase);
        HashSet<string> assignedPlayers = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, int> classUsedCount = new(StringComparer.OrdinalIgnoreCase);
        HashSet<int> filledSlots = new();

        if (preferredUsernameAssignments != null && preferredUsernameAssignments.Count > 0)
        {
            foreach (var kvp in preferredUsernameAssignments)
            {
                string preferredUser = kvp.Key.Trim();
                string preferredClass = kvp.Value.Trim();

                if (string.IsNullOrEmpty(preferredUser) || string.IsNullOrEmpty(preferredClass))
                    continue;

                if (!playerClasses.TryGetValue(preferredUser, out List<string>? userClasses))
                {
                    bot.Log($"[CustomClassSync] Preferred assignment requested for '{preferredUser}' but that user is not registered.");
                    continue;
                }

                if (!userClasses.Any(c => c.Equals(preferredClass, StringComparison.OrdinalIgnoreCase)))
                {
                    bot.Log($"[CustomClassSync] Preferred assignment for '{preferredUser}' cannot be honored because they do not own '{preferredClass}'.");
                    continue;
                }

                for (int s = 0; s < classSlots.Length; s++)
                {
                    if (filledSlots.Contains(s))
                        continue;

                    if (!classSlots[s].Any(c => c.Equals(preferredClass, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    int used = classUsedCount.GetValueOrDefault(preferredClass, 0);
                    if (!allowDuplicates && used >= 1)
                        continue;

                    if (allowDuplicates)
                    {
                        int max = classMaxCount.GetValueOrDefault(preferredClass, 0);
                        if (used >= max)
                            continue;
                    }

                    assignments[preferredUser] = preferredClass;
                    assignedPlayers.Add(preferredUser);
                    classUsedCount[preferredClass] = used + 1;
                    filledSlots.Add(s);
                    bot.Log($"[CustomClassSync] Preferred slot {s} > {AccountLabel(preferredUser)} ({preferredClass})");
                    break;
                }
            }
        }

        // Deterministic greedy assignment
        // Alpha-sort players so every client computes the identical result.
        List<string> sortedPlayers = playerClasses
            .Keys.OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
            .ToList();

        for (int s = 0; s < classSlots.Length; s++)
        {
            if (filledSlots.Contains(s))
                continue;

            bool filled = false;

            // Try each accepted class in preference order
            foreach (string acceptedClass in classSlots[s])
            {
                // Check if this class can still be used
                int used = classUsedCount.GetValueOrDefault(acceptedClass, 0);
                if (allowDuplicates)
                {
                    int max = classMaxCount.GetValueOrDefault(acceptedClass, 0);
                    if (used >= max)
                        continue;
                }
                else
                {
                    // No duplicates: each class used at most once
                    if (used >= 1)
                        continue;
                }

                List<string> candidates = sortedPlayers
                    .Where(p =>
                        !assignedPlayers.Contains(p)
                        && playerClasses[p].Any(c =>
                            c.Equals(acceptedClass, StringComparison.OrdinalIgnoreCase)
                        )
                    )
                    .ToList();

                if (candidates.Count == 0)
                    continue;

                // Most-constrained candidate first (fewest open slots it can fill),
                // tiebreak alphabetical.
                string best = candidates
                    .OrderBy(p =>
                    {
                        int canFill = 0;
                        for (int s2 = 0; s2 < classSlots.Length; s2++)
                        {
                            if (filledSlots.Contains(s2))
                                continue;
                            if (
                                classSlots[s2].Any(c =>
                                    playerClasses[p].Any(pc =>
                                        pc.Equals(c, StringComparison.OrdinalIgnoreCase)
                                    )
                                )
                            )
                                canFill++;
                        }
                        return canFill;
                    })
                    .ThenBy(p => p, StringComparer.OrdinalIgnoreCase)
                    .First();

                assignments[best] = acceptedClass;
                assignedPlayers.Add(best);
                classUsedCount[acceptedClass] = used + 1;
                filledSlots.Add(s);
                bot.Log($"[CustomClassSync] Slot {s} > {AccountLabel(best)} ({acceptedClass})");
                filled = true;
                break;
            }

            if (!filled)
                bot.Log($"[CustomClassSync] WARNING: No candidate for slot {s} ({string.Join("/", classSlots[s])})!");
        }

        // Find this client's assignment
        if (!assignments.TryGetValue(username, out string? myClass) || string.IsNullOrEmpty(myClass))
        {
            CoreBots2.Instance.Logger($"[CustomClassSync] {AccountLabel(username)} was not assigned any class! Check that all of your accounts own the required classes.", "CustomClassSync", true, true);
            bot.StopSync();
            return string.Empty;
        }

        bot.Log($"[CustomClassSync] - Equipping: {myClass}");
        CoreBots2.Instance.Equip(myClass);
        bot.Sleep(1000);

        // Clear sync file for next run
        try { File.WriteAllText(syncFile, ""); } catch { }

        return myClass;
    }

    public static string CustomClassSync(
        dynamic ultra,
        IScriptInterface bot,
        string[] classes,
        int armySize,
        string syncFilePath = "class_assign.sync",
        bool allowDuplicates = false
    )
    {
        string[][] wrapped = new string[classes.Length][];
        for (int i = 0; i < classes.Length; i++)
            wrapped[i] = new[] { classes[i] };
        return CustomClassSync(ultra, bot, wrapped, armySize, syncFilePath, allowDuplicates);
    }
}
