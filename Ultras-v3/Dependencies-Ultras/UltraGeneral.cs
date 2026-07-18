//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreAdvanced2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreUltra2.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Skua.Core.Interfaces;

public class UltraGeneral
{
    public static void EquipPresetClasses(dynamic ultra, IScriptInterface bot, string syncFilePath)
    {
        EquipUltraDailyPresetClasses(ultra, bot, syncFilePath);
    }

    public static void EquipUltraDailyPresetClasses(dynamic ultra, IScriptInterface bot, string syncFilePath, int armySize = 0)
    {
        if (ultra == null || bot == null || string.IsNullOrWhiteSpace(syncFilePath))
            return;

        armySize = armySize > 0
            ? armySize
            : Math.Max(1, bot.Config!.Get<int>("ArmySize"));

        var presetEntries = new[]
        {
            bot.Config!.Get<string>("Class1"),
            bot.Config.Get<string>("Class2"),
            bot.Config.Get<string>("Class3"),
            bot.Config.Get<string>("Class4"),
            bot.Config.Get<string>("Class5"),
            bot.Config.Get<string>("Class6"),
            bot.Config.Get<string>("Class7"),
        }
        .Select(cl => cl?.Trim())
        .Where(cl => !string.IsNullOrEmpty(cl))
        .Select(ParseClassOption)
        .Where(entry => !string.IsNullOrEmpty(entry.ClassName))
        .ToArray();

        if (presetEntries.Length == 0)
            return;

        var presetClasses = presetEntries.Select(entry => entry.ClassName).ToArray();
        var preferredAssignments = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in presetEntries)
        {
            if (!string.IsNullOrEmpty(entry.Username) && !preferredAssignments.ContainsKey(entry.Username))
                preferredAssignments[entry.Username] = entry.ClassName;
        }

        bool allowDuplicates = presetClasses.Length < armySize || presetClasses.Distinct(StringComparer.OrdinalIgnoreCase).Count() < presetClasses.Length;

        string[][] classSlots;
        if (presetEntries.Length >= armySize)
        {
            classSlots = presetEntries
                .Take(armySize)
                .Select(entry => new[] { entry.ClassName })
                .ToArray();
        }
        else
        {
            classSlots = Enumerable.Range(0, armySize)
                .Select(_ => presetClasses)
                .ToArray();
        }

        ultra.EquipClassSync(
            classSlots,
            armySize,
            syncFilePath,
            allowDuplicates,
            preferredAssignments.Count > 0 ? preferredAssignments : null
        );
    }

    public static void EquipUltraDailyPresetClassesSafe(dynamic ultra, IScriptInterface bot, string syncFilePath, int armySize = 0, int timeoutMs = 90000)
    {
        if (ultra == null || bot == null || string.IsNullOrWhiteSpace(syncFilePath))
            return;

        armySize = armySize > 0
            ? armySize
            : Math.Max(1, bot.Config!.Get<int>("ArmySize"));

        var presetEntries = new[]
        {
            bot.Config!.Get<string>("Class1"),
            bot.Config.Get<string>("Class2"),
            bot.Config.Get<string>("Class3"),
            bot.Config.Get<string>("Class4"),
            bot.Config.Get<string>("Class5"),
            bot.Config.Get<string>("Class6"),
            bot.Config.Get<string>("Class7"),
        }
        .Select(cl => cl?.Trim())
        .Where(cl => !string.IsNullOrEmpty(cl))
        .Select(ParseClassOption)
        .Where(entry => !string.IsNullOrEmpty(entry.ClassName))
        .ToArray();

        if (presetEntries.Length == 0)
            return;

        var presetClasses = presetEntries.Select(entry => entry.ClassName).ToArray();
        var preferredAssignments = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in presetEntries)
        {
            if (!string.IsNullOrEmpty(entry.Username) && !preferredAssignments.ContainsKey(entry.Username))
                preferredAssignments[entry.Username] = entry.ClassName;
        }

        bool allowDuplicates = presetClasses.Length < armySize || presetClasses.Distinct(StringComparer.OrdinalIgnoreCase).Count() < presetClasses.Length;

        string[][] classSlots;
        if (presetEntries.Length >= armySize)
        {
            classSlots = presetEntries
                .Take(armySize)
                .Select(entry => new[] { entry.ClassName })
                .ToArray();
        }
        else
        {
            classSlots = Enumerable.Range(0, armySize)
                .Select(_ => presetClasses)
                .ToArray();
        }

        string syncFile = ultra.ResolveSyncPath(syncFilePath);

        try
        {
            if (
                File.Exists(syncFile)
                && (DateTime.UtcNow - File.GetLastWriteTimeUtc(syncFile)).TotalMinutes > 15
            )
                File.WriteAllText(syncFile, "");
        }
        catch { }

        HashSet<string> allNeeded = new(StringComparer.OrdinalIgnoreCase);
        foreach (string[] slot in classSlots)
            foreach (string cls in slot)
                allNeeded.Add(cls);

        List<string> myClasses = new();
        foreach (string cls in allNeeded)
        {
            if (CoreBots2.Instance.CheckInventory(cls, toInv: true))
                myClasses.Add(cls);
        }

        string username = bot.Player?.Username ?? Guid.NewGuid().ToString();
        string payload = string.Join(",", myClasses);
        bot.Log($"[EquipClassSync] {username} owns: {(myClasses.Count > 0 ? payload : "NONE of the needed classes")}");

        ultra.UpdateEntry(syncFile, username, payload);

        const int staleThreshold = 600;
        int lastCount = -1;
        long waitStartMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        while (!bot.ShouldExit)
        {
            string[] lines = ultra.ReadLines(syncFile);
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            int validCount = 0;

            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length < 3)
                    continue;
                if (!long.TryParse(parts[^1], out long ts))
                    continue;
                if (now - ts <= staleThreshold)
                    validCount++;
            }

            if (validCount != lastCount)
            {
                lastCount = validCount;
                bot.Log($"[EquipClassSync] Registered: {validCount}/{armySize}");
            }

            if (validCount >= armySize)
                break;

            if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - waitStartMs >= timeoutMs)
            {
                bot.Log($"[EquipClassSync] Timeout waiting for class registration after {timeoutMs / 1000}s; continuing with {validCount}/{armySize}.");
                break;
            }

            ultra.UpdateEntry(syncFile, username, payload);
            bot.Sleep(500);
        }

        if (bot.ShouldExit)
            return;

        string readyPayload = $"READY|{string.Join(",", myClasses)}";
        ultra.UpdateEntry(syncFile, username, readyPayload);
        bot.Log($"[EquipClassSync] {username} marked READY, waiting for all...");

        waitStartMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        while (!bot.ShouldExit)
        {
            string[] lines = ultra.ReadLines(syncFile);
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            int readyCount = 0;

            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length < 3)
                    continue;
                if (!parts[1].StartsWith("READY|", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (!long.TryParse(parts[^1], out long ts))
                    continue;
                if (now - ts <= staleThreshold)
                    readyCount++;
            }

            if (readyCount >= armySize)
            {
                bot.Log($"[EquipClassSync] All {readyCount} clients READY. Reading final state...");
                break;
            }

            if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - waitStartMs >= timeoutMs)
            {
                bot.Log($"[EquipClassSync] Timeout waiting for READY state after {timeoutMs / 1000}s; continuing with {readyCount}/{armySize}.");
                break;
            }

            bot.Sleep(300);
        }

        if (bot.ShouldExit)
            return;

        bot.Sleep(500);

        Dictionary<string, List<string>> playerClasses = new(StringComparer.OrdinalIgnoreCase);
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

                if (!long.TryParse(parts[^1], out long ts))
                    continue;
                if (now - ts > staleThreshold)
                    continue;

                string classListStr = payloadPart;
                if (payloadPart.StartsWith("READY|", StringComparison.OrdinalIgnoreCase))
                    classListStr = payloadPart.Substring(6);

                playerClasses[playerName] = classListStr
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            }
        }

        bot.Log($"[EquipClassSync] {playerClasses.Count} player(s) registered. Assigning classes...");

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

        if (preferredAssignments != null && preferredAssignments.Count > 0)
        {
            foreach (var kvp in preferredAssignments)
            {
                string preferredUser = kvp.Key.Trim();
                string preferredClass = kvp.Value.Trim();

                if (string.IsNullOrEmpty(preferredUser) || string.IsNullOrEmpty(preferredClass))
                    continue;

                if (!playerClasses.TryGetValue(preferredUser, out List<string>? userClasses))
                {
                    bot.Log($"[EquipClassSync] Preferred assignment requested for '{preferredUser}' but that user is not registered.");
                    continue;
                }

                if (!userClasses.Any(c => c.Equals(preferredClass, StringComparison.OrdinalIgnoreCase)))
                {
                    bot.Log($"[EquipClassSync] Preferred assignment for '{preferredUser}' cannot be honored because they do not own '{preferredClass}'.");
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
                    bot.Log($"[EquipClassSync] Preferred slot {s} > {preferredUser} ({preferredClass})");
                    break;
                }
            }
        }

        List<string> sortedPlayers = playerClasses
            .Keys.OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
            .ToList();

        for (int s = 0; s < classSlots.Length; s++)
        {
            if (filledSlots.Contains(s))
                continue;

            bool filled = false;

            foreach (string acceptedClass in classSlots[s])
            {
                int used = classUsedCount.GetValueOrDefault(acceptedClass, 0);
                if (allowDuplicates)
                {
                    int max = classMaxCount.GetValueOrDefault(acceptedClass, 0);
                    if (used >= max)
                        continue;
                }
                else
                {
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
                bot.Log($"[EquipClassSync] Slot {s} > {best} ({acceptedClass})");
                filled = true;
                break;
            }

            if (!filled)
                bot.Log($"[EquipClassSync] WARNING: No candidate for slot {s} ({string.Join("/", classSlots[s])})!");
        }

        if (!assignments.TryGetValue(username, out string? myClass) || string.IsNullOrEmpty(myClass))
        {
            CoreBots2.Instance.Logger($"[EquipClassSync] {username} was not assigned any class! Check that all of your accounts own the required classes.", "EquipClassSync", true, true);
            return;
        }

        bot.Log($"[EquipClassSync] - Equipping: {myClass}");
        CoreBots2.Instance.Equip(myClass);
        bot.Sleep(1000);

        try { File.WriteAllText(syncFile, ""); } catch { }
    }


    public static void PublishUltraDailyBossNeeds(dynamic ultra, IScriptInterface bot, string syncFilePath, IEnumerable<string> neededBosses)
    {
        if (ultra == null || bot == null || string.IsNullOrWhiteSpace(syncFilePath))
            return;

        string path = ultra.ResolveSyncPath(syncFilePath);
        string username = bot.Player?.Username ?? Guid.NewGuid().ToString();
        string payload = string.Join(",", neededBosses
            .Select(b => b?.Trim())
            .Where(b => !string.IsNullOrEmpty(b)));

        ultra.UpdateEntry(path, username, payload);
    }

    public static void PublishUltraDailyBossStatuses(dynamic ultra, IScriptInterface bot, string syncFilePath, IEnumerable<KeyValuePair<string, bool>> bossStatuses)
    {
        if (ultra == null || bot == null || string.IsNullOrWhiteSpace(syncFilePath))
            return;

        string path = ultra.ResolveSyncPath(syncFilePath);
        string username = bot.Player?.Username ?? Guid.NewGuid().ToString();
        string payload = string.Join(",", bossStatuses.Select(kvp => $"{kvp.Key}={(kvp.Value ? "true" : "false")}"));

        ultra.UpdateEntry(path, username, payload);
    }

    public static int GetUltraDailyBossParticipantCount(dynamic ultra, IScriptInterface bot, string syncFilePath, int defaultCount)
    {
        if (ultra == null || bot == null || string.IsNullOrWhiteSpace(syncFilePath))
            return Math.Max(1, defaultCount);

        string path = ultra.ResolveSyncPath(syncFilePath);
        string[] lines = ultra.ReadLines(path);
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        const int staleThreshold = 600;

        int count = 0;
        foreach (string line in lines)
        {
            string[] parts = line.Split(':');
            if (parts.Length < 3)
                continue;

            if (!long.TryParse(parts[parts.Length - 1], out long ts))
                continue;

            if (now - ts > staleThreshold)
                continue;

            count++;
        }

        return count > 0
            ? Math.Max(1, count)
            : Math.Max(1, defaultCount);
    }

    public static bool EnsureCompleteDailyQuestSafe(IScriptInterface bot, int questId)
    {
        if (bot == null || questId <= 0)
            return false;

        Quest? quest = bot.Quests.EnsureLoad(questId);
        if (quest == null)
            return false;

        if (bot.Quests.IsDailyComplete(questId))
            return true;

        if (quest.Once && bot.Quests.HasBeenCompleted(questId))
            return true;

        if (!bot.Quests.IsInProgress(questId) && !bot.Quests.CanComplete(questId))
        {
            if (!CoreBots2.Instance.EnsureAccept(questId))
                return false;
        }

        return CoreBots2.Instance.EnsureComplete(questId);
    }

    public static bool EnsureAcceptOnce(IScriptInterface bot, int questId)
    {
        if (bot == null || questId <= 0)
            return false;

        Quest? quest = bot.Quests.EnsureLoad(questId);
        if (quest == null)
            return false;

        if (bot.Quests.IsDailyComplete(questId))
            return true;

        if (quest.Once && bot.Quests.HasBeenCompleted(questId))
            return true;

        if (bot.Quests.IsInProgress(questId))
            return true;

        const int maxAttempts = 5;
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            if (CoreBots2.Instance.EnsureAccept(questId))
                return true;

            if (attempt < maxAttempts)
                bot.Sleep(1000);
        }

        return false;
    }

    public static bool EnsureCompleteOnce(IScriptInterface bot, int questId, int itemId = -1)
    {
        if (bot == null || questId <= 0)
            return false;

        Quest? quest = bot.Quests.EnsureLoad(questId);
        if (quest == null)
            return false;

        if (bot.Quests.IsDailyComplete(questId))
            return true;

        if (quest.Once && bot.Quests.HasBeenCompleted(questId))
            return true;

        if (!bot.Quests.IsInProgress(questId) && !bot.Quests.CanComplete(questId))
        {
            if (!EnsureAcceptOnce(bot, questId))
                return false;
        }

        const int maxAttempts = 5;
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            if (CoreBots2.Instance.EnsureComplete(questId, itemId))
                return true;

            if (attempt < maxAttempts)
                bot.Sleep(1000);
        }

        return false;
    }

    public static bool IsWholeArmyDead(dynamic ultra, IScriptInterface bot, string syncFilePath)
    {
        return UpdateAndCheckWholeArmyState(ultra, bot, syncFilePath, !bot.Player.Alive, "1");
    }

    public static bool IsWholeArmyAlive(dynamic ultra, IScriptInterface bot, string syncFilePath)
    {
        return UpdateAndCheckWholeArmyState(ultra, bot, syncFilePath, bot.Player.Alive, "1");
    }

    private static bool UpdateAndCheckWholeArmyState(dynamic ultra, IScriptInterface bot, string syncFilePath, bool localState, string expectedValue)
    {
        if (ultra == null || bot == null || string.IsNullOrWhiteSpace(syncFilePath))
            return false;

        string? username = bot.Player?.Username;
        if (string.IsNullOrWhiteSpace(username))
            return false;

        string key = username.Replace(":", "-");
        string syncFile = ultra.ResolveSyncPath(syncFilePath);
        ultra.UpdateEntry(syncFile, key, localState ? "1" : "0");

        string[] lines = ultra.ReadLines(syncFile);
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        const int staleThreshold = 600;
        int activeMembers = 0;
        int matchingMembers = 0;

        foreach (string line in lines)
        {
            string[] parts = line.Split(':');
            if (parts.Length < 3)
                continue;
            if (!long.TryParse(parts[2], out long ts))
                continue;
            if (now - ts > staleThreshold)
                continue;

            activeMembers++;
            if (parts[1] == expectedValue)
                matchingMembers++;
        }

        return activeMembers > 0 && activeMembers == matchingMembers;
    }

    private static (string ClassName, string? Username) ParseClassOption(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return (string.Empty, null);

        var parts = raw.Split(new[] { ',' }, 2, StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.Trim())
            .Where(part => !string.IsNullOrEmpty(part))
            .ToArray();

        if (parts.Length == 0)
            return (string.Empty, null);

        return parts.Length == 1
            ? (parts[0], null)
            : (parts[0], parts[1]);
    }

    public static bool IsQuestGreen(IScriptInterface bot, int questId)
    {
        if (bot == null || questId <= 0)
            return false;

        Quest? quest = bot.Quests.EnsureLoad(questId);
        if (quest == null)
            return false;

        if (bot.Quests.IsDailyComplete(questId))
            return false;

        if (quest.Once && bot.Quests.HasBeenCompleted(questId))
            return false;

        if (!bot.Quests.IsInProgress(questId))
            return false;

        return bot.Quests.CanComplete(questId);
    }

    public static bool IsQuestComplete(IScriptInterface bot, int questId)
    {
        if (bot == null || questId <= 0)
            return false;

        if (bot.Quests.IsDailyComplete(questId))
            return true;

        Quest? quest = bot.Quests.EnsureLoad(questId);
        if (quest == null)
            return false;

        if (quest.Active)
        {
            if (bot.Quests.CanComplete(questId))
                return true;

            if (quest.Slot > 0 && quest.Value > 0)
            {
                int currentValue = bot.Flash.CallGameFunction<int>("world.getQuestValue", quest.Slot);
                return currentValue >= quest.Value;
            }

            return false;
        }

        return quest.Once && bot.Quests.HasBeenCompleted(questId);
    }

    #region New
    public static bool ArmyWipeHelperWithTaunters(
        CoreUltra2 ultra,
        IScriptInterface bot,
        string wipeDeadSyncFile,
        string wipeAliveSyncFile,
        ref bool armyWipeDetected,
        ref DateTime fightStartTime,
        ref DateTime lastTauntTime)
    {
        if (ultra == null || bot == null || string.IsNullOrWhiteSpace(wipeDeadSyncFile) || string.IsNullOrWhiteSpace(wipeAliveSyncFile))
            return false;

        bool allDead = IsWholeArmyDead(ultra, bot, wipeDeadSyncFile);
        if (allDead)
        {
            if (!armyWipeDetected)
                CoreBots2.Instance.Logger("Army wipe detected — all clients dead.");
            armyWipeDetected = true;
        }

        if (!armyWipeDetected)
            return false;

        bool allAlive = IsWholeArmyAlive(ultra, bot, wipeAliveSyncFile);
        if (allAlive)
        {
            CoreBots2.Instance.Logger("Army wipe recovered — all clients alive again.");
            ultra.ClearSyncFile(wipeDeadSyncFile);
            ultra.ClearSyncFile(wipeAliveSyncFile);
            bot.Combat.CancelTarget();
            armyWipeDetected = false;
            fightStartTime = DateTime.UtcNow;
            lastTauntTime = DateTime.MinValue;
            return true;
        }

        bot.Combat.CancelTarget();
        CoreBots2.Instance.Logger("Army wipe active — waiting for everyone to respawn before fighting.");
        bot.Sleep(250);
        return true;
    }

    public static bool ArmyWipeHelperWithNoTaunters(
        CoreUltra2 ultra,
        IScriptInterface bot,
        string wipeDeadSyncFile,
        string wipeAliveSyncFile,
        ref bool armyWipeDetected)
    {
        if (ultra == null || bot == null || string.IsNullOrWhiteSpace(wipeDeadSyncFile) || string.IsNullOrWhiteSpace(wipeAliveSyncFile))
            return false;

        bool allDead = IsWholeArmyDead(ultra, bot, wipeDeadSyncFile);
        if (allDead)
        {
            if (!armyWipeDetected)
                CoreBots2.Instance.Logger("Army wipe detected — all clients dead.");
            armyWipeDetected = true;
        }

        if (!armyWipeDetected)
            return false;

        bool allAlive = IsWholeArmyAlive(ultra, bot, wipeAliveSyncFile);
        if (allAlive)
        {
            CoreBots2.Instance.Logger("Army wipe recovered — all clients alive again.");
            ultra.ClearSyncFile(wipeDeadSyncFile);
            ultra.ClearSyncFile(wipeAliveSyncFile);
            bot.Combat.CancelTarget();
            armyWipeDetected = false;
            return true;
        }

        bot.Combat.CancelTarget();
        CoreBots2.Instance.Logger("Army wipe active — waiting for everyone to respawn before fighting.");
        bot.Sleep(250);
        return true;
    }
    #endregion

    public static bool IsQuestTurnable(IScriptInterface bot, int questId)
    {
        if (bot == null || questId <= 0)
            return false;

        return bot.Quests.CanComplete(questId);
    }

    /// <summary>
    /// Completes a quest without loading it or checking requirements.
    /// Retries up to 5 times with 1-second intervals on failure.
    /// </summary>
    public static bool CompleteQuest(IScriptInterface bot, int questId, int itemId = -1)
    {
        if (bot == null || questId <= 0)
            return false;

        const int maxAttempts = 5;
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            if (bot.Quests.EnsureComplete(questId, itemId))
                return true;

            if (attempt < maxAttempts)
                bot.Sleep(1000);
        }

        return false;
    }

    public static void EquipWarriorClass()
    {
        CoreBots2.Instance.Equip("Warrior");
    }

}

