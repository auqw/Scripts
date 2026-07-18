//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraGeneral.cs

using System;
using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class UltraQueue
{
    public static IEnumerable<string> GetSharedBossQueue(dynamic ultra, IScriptInterface bot, dynamic C, IEnumerable<string> bosses, string bossSyncFile, string participantSyncFile, Func<string, bool> isBossComplete)
    {
        if (bot == null)
            return bosses;

        // Ultras-v3 are designed for a fixed 4-player army queue.
        const int armySize = 4;
        string syncPath = ultra.ResolveSyncPath(bossSyncFile);
        string user = bot.Player?.Username ?? Guid.NewGuid().ToString();

        string payload = string.Join(",", bosses.Select(b => $"{b}={(isBossComplete(b) ? "true" : "false")}"));
        ultra.UpdateEntry(syncPath, user, payload);

        string participantPath = ultra.ResolveSyncPath(participantSyncFile);
        if (ShouldClearParticipantSync(participantPath))
            ultra.ClearSyncFile(participantPath);
        else
            CleanupStaleParticipants(ultra, C, participantPath);

        ultra.UpdateEntry(participantPath, user, "1");

        int lastCount = -1;
        const int staleThreshold = 600;
        DateTime waitStarted = DateTime.UtcNow;
        const int timeoutMs = 60000;

        while (!bot.ShouldExit)
        {
            int count = 0;
            foreach (string line in ultra.ReadLines(participantPath))
            {
                string[] parts = line.Split(':');
                if (parts.Length < 3)
                    continue;

                if (!long.TryParse(parts[^1], out long ts))
                    continue;

                if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - ts > staleThreshold)
                    continue;

                count++;
            }

            if (count != lastCount)
            {
                lastCount = count;
                C.Logger($"[AllUltras-v3] Registered participants: {count}/{armySize}", "Info");
            }

            if (count >= armySize)
                break;

            if ((DateTime.UtcNow - waitStarted).TotalMilliseconds >= timeoutMs && count > 0)
            {
                C.Logger($"[AllUltras-v3] Wait timeout reached — proceeding with {count}/{armySize} registered clients.", "Warning");
                break;
            }

            ultra.UpdateEntry(syncPath, user, payload);
            ultra.UpdateEntry(participantPath, user, "1");
            bot.Sleep(250);
        }

        if (bot.ShouldExit)
            return bosses;

        // Ensure all accounts have written their latest payload before reading
        ultra.UpdateEntry(syncPath, user, payload);
        bot.Sleep(3000);

        var allBossComplete = bosses.ToDictionary(b => b, b => true, StringComparer.OrdinalIgnoreCase);
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        foreach (string line in ultra.ReadLines(syncPath))
        {
            string[] parts = line.Split(':');
            if (parts.Length < 3)
                continue;

            if (!long.TryParse(parts[^1], out long ts))
                continue;

            if (now - ts > staleThreshold)
                continue;

            string payloadLine = parts[1];
            var statuses = payloadLine.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Split(new[] { '=' }, 2))
                .Where(pair => pair.Length == 2)
                .ToDictionary(pair => pair[0].Trim(), pair => pair[1].Trim(), StringComparer.OrdinalIgnoreCase);

            foreach (string boss in bosses)
            {
                if (!statuses.TryGetValue(boss, out string? rawValue)
                    || !bool.TryParse(rawValue, out bool complete)
                    || !complete)
                {
                    allBossComplete[boss] = false;
                }
            }
        }

        return bosses.Where(b => !allBossComplete[b]).ToList();
    }

    private static bool ShouldClearParticipantSync(string path)
    {
        try
        {
            if (!System.IO.File.Exists(path))
                return true;

            return (DateTime.UtcNow - System.IO.File.GetLastWriteTimeUtc(path)).TotalMinutes > 10;
        }
        catch
        {
            return false;
        }
    }

    private static void CleanupStaleParticipants(dynamic ultra, dynamic C, string path)
    {
        try
        {
            string[] lines = ultra.ReadLines(path);
            if (lines.Length == 0)
                return;

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            const int staleThreshold = 600;
            var freshLines = new List<string>();

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(':');
                if (parts.Length < 3)
                    continue;

                if (!long.TryParse(parts[^1], out long ts))
                    continue;

                if (now - ts <= staleThreshold)
                    freshLines.Add(line);
            }

            if (freshLines.Count == 0)
            {
                ultra.ClearSyncFile(path);
                return;
            }

            if (freshLines.Count != lines.Length)
            {
                System.IO.File.WriteAllText(path, string.Join(Environment.NewLine, freshLines));
                C.Logger($"[AllUltras-v3] Cleaned stale participant entries; {freshLines.Count} remain.", "Info");
            }
        }
        catch (Exception ex)
        {
            C.Logger($"[AllUltras-v3] Failed to cleanup participant sync: {ex.Message}", "Warning");
        }
    }
}
