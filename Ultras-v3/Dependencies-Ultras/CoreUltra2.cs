/*
name: null
description: null
tags: null
*/

//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreEngine2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreStory2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreFarms2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreAdvanced2.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class CoreUltra2
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine2 Core => CoreEngine2.Instance;
    private CoreBots2 C => CoreBots2.Instance;
    private static CoreAdvanced2 Adv
    {
        get => _Adv ??= new CoreAdvanced2();
        set => _Adv = value;
    }
    private static CoreAdvanced2 _Adv;
    private static CoreStory2 Story
    {
        get => _Story ??= new CoreStory2();
        set => _Story = value;
    }
    private static CoreStory2 _Story;

    public void Test() => Bot.Log("NewCore interface OK!");

    public void Taunt(
        string className,
        string target,
        string mode,
        int delayMs = 0,
        string? aura = null
    )
    {
        if (string.IsNullOrWhiteSpace(className) || string.IsNullOrWhiteSpace(target))
            return;

        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

        if (!Bot.Inventory.IsEquipped(className))
            return;

        Bot.Combat.Attack(target);
        if (delayMs > 0)
            Bot.Sleep(delayMs);

        switch (mode)
        {
            case "aura":
                if (!string.IsNullOrWhiteSpace(aura) && Core.GetAuraSecondsRemaining(aura) < 1)
                {
                    Bot.Log("UseTaunt");
                    UseTaunt();
                }
                break;

            case "charge":
                if (!string.IsNullOrWhiteSpace(aura) && _chargeDetected && !Bot.Target.Auras.Any(x => x != null && x.Name.ToLower() == aura.ToLower()))
                    UseTaunt();
                break;
        }
    }

    public void Taunt(
        string className,
        int target,
        string mode,
        int delayMs = 0,
        string? aura = null
    )
    {
        if (string.IsNullOrWhiteSpace(className) || target <= 0)
            return;
        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
        if (!Bot.Inventory.IsEquipped(className))
            return;
        Bot.Combat.Attack(target);
        if (delayMs > 0)
            Bot.Sleep(delayMs);
        switch (mode)
        {
            case "aura":
                if (!string.IsNullOrWhiteSpace(aura) && Core.GetAuraSecondsRemaining(aura) < 1)
                {
                    Bot.Log("UseTaunt");
                    UseTaunt();
                }
                break;
            case "charge":
                if (_chargeDetected && !Bot.Target.Auras.Any(x => x != null && x.Name == "Focus"))
                    UseTaunt();
                break;
        }
    }

    public void KillWithPriority(
        string primaryName,
        int primaryMapId,
        string priorityName1,
        int priorityMapId1,
        string priorityName2,
        int priorityMapId2
    )
    {
        if (string.IsNullOrWhiteSpace(primaryName))
            return;
        if (
            !string.IsNullOrWhiteSpace(priorityName1)
            && Core.IsAliveByMapId(priorityMapId1, name: priorityName1)
        )
        {
            if (!Bot.Player.Alive)
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
            KillByMapId(priorityMapId1, name: priorityName1);
        }
        else if (
            !string.IsNullOrWhiteSpace(priorityName2)
            && Core.IsAliveByMapId(priorityMapId2, name: priorityName2)
        )
        {
            if (!Bot.Player.Alive)
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
            KillByMapId(priorityMapId2, name: priorityName2);
        }
        else
        {
            if (!Bot.Player.Alive)
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
            KillByMapId(primaryMapId, name: primaryName);
        }
        Bot.Sleep(Core.D1);
    }

    public void KillByMapId(int mapId, string? name = null, int? id = null)
    {
        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

        if (Core.IsAliveByMapId(mapId, name, id))
        {
            Bot.Combat.Attack(mapId);
            Bot.Sleep(250);
        }
    }

    public bool MonsterAlive(string name) =>
     (Bot.Player.Alive || Bot.Wait.ForTrue(() => Bot.Player.Alive, 20))
     && !string.IsNullOrWhiteSpace(name)
     && Bot.Monsters?.MapMonsters?.Any(m => m?.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) == true && m.HP > 0) == true;

    public void UltraWardenTaunter()
    {
        const string mob = "Ultra Warden";
        Bot.Combat.Attack(mob);

        var t = Bot.Player.Target;
        if (t == null)
            return;
        if (t.HP <= 0)
            return;
        if (t.MaxHP <= 0)
            return;

        int hp = t.HP;
        int max = t.MaxHP;

        int pct = hp * 100 / max;
        int band5 = (pct / 5) * 5;

        const string key = "warden.bands";
        var seen = (AppDomain.CurrentDomain.GetData(key) as HashSet<int>) ?? new HashSet<int>();

        if (!seen.Contains(band5))
        {
            double exactPct = (hp / (double)max) * 100;

            seen.Add(band5);
            AppDomain.CurrentDomain.SetData(key, seen);

            while (!Bot.ShouldExit
            && MonsterAlive(mob)
            && !Bot.Target.Auras.Any(a => a != null && a.Name == "Focus"))
            {
                Core.UsePotion();
            }

        }

        Bot.Sleep(150);
    }

    public void DrakathTaunter()
    {
        Bot.Combat.Attack("Champion Drakath");
        var dummy = Bot.Player.Target;
        if (dummy == null || dummy.HP <= 0)
            return;

        int[] bands = { 90, 80, 70, 60, 50, 40, 30, 20, 10 };
        double wiggle = 1.5;
        int lastBand = int.MaxValue;
        double oldPct = 100.0;
        long oldTicks = 0;

        object? tmp = AppDomain.CurrentDomain.GetData("drakath.lastThreshold");
        if (tmp != null)
            lastBand = (int)tmp;

        tmp = AppDomain.CurrentDomain.GetData("drakath.prevPercentage");
        if (tmp != null)
            oldPct = (double)tmp;

        tmp = AppDomain.CurrentDomain.GetData("drakath.lastFireTicks");
        if (tmp != null)
            oldTicks = (long)tmp;

        double nowPct = Core.GetTargetHealthPercentage();
        long nowTicks = DateTime.UtcNow.Ticks;
        bool cooledDown = new TimeSpan(nowTicks - oldTicks).TotalMilliseconds >= 1200;

        bool triggered = false;
        int hitBand = 0;

        foreach (int band in bands)
        {
            if (band < lastBand)
            {
                double hi = band + wiggle;
                double lo = band - wiggle;
                bool wasHigh = oldPct > hi;
                bool inZone = nowPct >= lo && nowPct <= hi;

                if (wasHigh && inZone)
                {
                    triggered = true;
                    hitBand = band;
                    break;
                }
            }
        }

        if (cooledDown && triggered)
        {
            AppDomain.CurrentDomain.SetData("drakath.lastThreshold", hitBand);
            AppDomain.CurrentDomain.SetData("drakath.lastFireTicks", nowTicks);

            DateTime giveUp = DateTime.UtcNow.AddMilliseconds(3000);

            while (!Bot.ShouldExit && DateTime.UtcNow < giveUp)
            {
                if (!Bot.Player.HasTarget)
                    Bot.Combat.Attack("Champion Drakath");
                UseTaunt();
                if (!Bot.Player.HasTarget)
                    Bot.Combat.Attack("Champion Drakath");
                if (Bot.Target.Auras.Any(a => a != null && a.Name == "Focus"))
                    break;
                Bot.Sleep(120);
            }
        }

        AppDomain.CurrentDomain.SetData("drakath.prevPercentage", nowPct);
        Bot.Sleep(120);
    }

    // Waits for `quantity` + 1  (quant + self) so for 1 player, wait for 0(+1)
    public void WaitForArmy(
        int quantity,
        string syncFilePath = "army_sync.sync",
        int bufferTimeMs = 3000,
        int tickMs = 500,
        int timeoutMs = 0
    )
    {
        if (Bot?.Map == null)
            return;

        // --- Resolve safe writable sync path ---
        string FindHome(string path)
        {
            try
            {
                path = Environment.ExpandEnvironmentVariables(path);

                // Allow absolute path if directory exists
                string? dir = Path.GetDirectoryName(path);
                if (Path.IsPathRooted(path) && !string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                    return path;

                // Default to %AppData%\Skua
                string baseDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Skua"
                );
                string full = Path.Combine(baseDir, Path.GetFileName(path));
                Directory.CreateDirectory(baseDir);

                for (int attempt = 0; attempt < 5; attempt++)
                {
                    try
                    {
                        using FileStream fs = new(
                            full,
                            FileMode.OpenOrCreate,
                            FileAccess.Write,
                            FileShare.ReadWrite
                        );
                        return full;
                    }
                    catch (IOException)
                    {
                        Bot?.Sleep(50);
                    }
                }
                return full;
            }
            catch (Exception ex)
            {
                Bot?.Log($"[WaitForArmy] Path resolution failed: {ex.Message}");
                return Path.GetFullPath(path);
            }
        }

        // --- File I/O Helpers ---
        string[] Slurp(string path)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    using FileStream fs = new(
                        path,
                        FileMode.OpenOrCreate,
                        FileAccess.Read,
                        FileShare.ReadWrite
                    );
                    using StreamReader sr = new(fs);
                    return sr.ReadToEnd()
                        .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                }
                catch (IOException)
                {
                    Bot?.Sleep(50);
                }
                catch
                {
                    break;
                }
            }
            return Array.Empty<string>();
        }

        void Yeet(string path, string[] lines)
        {
            for (int i = 0; i < 15; i++)
            {
                try
                {
                    File.WriteAllLines(path, lines);
                    return;
                }
                catch (IOException)
                {
                    Bot?.Sleep(50);
                }
                catch
                {
                    return;
                }
            }
        }

        // Each line: username|class:ready:timestamp
        void Poke(string path, string key, bool ready)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            string[] keyParts = key.Split('|');
            if (keyParts.Length < 2 || string.IsNullOrWhiteSpace(keyParts[0]))
                return;

            string username = keyParts[0];
            string className = keyParts[1];

            List<string> lines = Slurp(path).ToList();

            // purge broken historical entries (|Peasant etc.)
            lines.RemoveAll(l => l.StartsWith("|", StringComparison.Ordinal));

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string entry = $"{username}|{className}:{(ready ? 1 : 0)}:{now}";

            int idx = lines.FindIndex(l =>
            {
                string[] parts = l.Split(':');
                if (parts.Length < 1)
                    return false;

                string[] existingKey = parts[0].Split('|');
                return existingKey.Length >= 1 && existingKey[0] == username;
            });

            if (idx >= 0)
                lines[idx] = entry;
            else
                lines.Add(entry);

            Yeet(path, lines.ToArray());
        }


        int HowMany(string path)
        {
            string[] lines = Slurp(path);
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            const int staleThreshold = 600; // 10 minutes
            List<string> valid = new();

            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length < 3)
                    continue;

                string key = parts[0];
                string status = parts[1];
                if (!long.TryParse(parts[2], out long ts))
                    continue;

                if (now - ts <= staleThreshold)
                    valid.Add(line);
            }

            // Rewrite file only if we cleaned something out
            if (valid.Count != lines.Length)
                Yeet(path, valid.ToArray());

            return valid.Count(l => l.Split(':')[1] == "1");
        }

        // --- Initialize sync file ---
        string syncFile = FindHome(syncFilePath);
        try
        {
            string? dir = Path.GetDirectoryName(syncFile);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            if (
                !File.Exists(syncFile)
                || (DateTime.UtcNow - File.GetLastWriteTimeUtc(syncFile)).TotalMinutes > 15
                || Slurp(syncFile).All(l => l.EndsWith(":1"))
            )
                File.WriteAllText(syncFile, "");
        }
        catch (Exception ex)
        {
            Bot?.Log($"[WaitForArmy] Sync file setup failed: {ex.Message}");
        }

        string me =
            $"{Bot?.Player?.Username ?? "Nobody"}|{Bot?.Player?.CurrentClass?.Name ?? "Peasant"}".Replace(
                ":",
                "-"
            );
        int need = quantity == 0 ? 1 : Math.Max(1, quantity) + 1;

        Poke(syncFile, me, false);

        Stopwatch clock = Stopwatch.StartNew();
        int lastReady = -1;

        // --- Wait for army readiness ---
        while (!Bot!.ShouldExit)
        {
            int ready = HowMany(syncFile);
            if (ready != lastReady)
            {
                lastReady = ready;
                Bot?.Log($"[WaitForArmy] Ready: {ready}/{need}");
            }

            if (ready >= need)
            {
                Bot?.Log("[WaitForArmy] All members ready!");
                break;
            }

            Poke(syncFile, me, true);

            if (timeoutMs > 0 && clock.ElapsedMilliseconds >= timeoutMs)
            {
                Bot?.Log("[WaitForArmy] Timeout reached — continuing anyway.");
                break;
            }

            Bot?.Sleep(tickMs);
        }

        if (Bot?.ShouldExit == true)
        {
            try
            {
                File.WriteAllText(syncFile, "");
            }
            catch { }
            return;
        }

        // --- Warmup spam to keep clients responsive ---
        DateTime spam = DateTime.UtcNow.AddMilliseconds(2000);
        while (!Bot!.ShouldExit && DateTime.UtcNow < spam)
        {
            Bot.Skills.UseSkill(3);
            Bot?.Sleep(300);
            Bot.Skills.UseSkill(2);
            Bot?.Sleep(300);
            Bot.Skills.UseSkill(1);
            Bot?.Sleep(300);
        }

        Bot?.Sleep(bufferTimeMs);

        try
        {
            File.WriteAllText(syncFile, "");
        }
        catch { }
    }

    // private static bool _syncInitialized = false;
    // private bool startNewRun = false;

    public bool CheckArmyProgress(
       string itemName,
       int targetQuantity,
       bool isTemp,
       string syncFilePath = "army_sync.sync"
   )
    {
        // Expected format: KEY:current:target:TYPE:timestamp
        // Example: Player1|ArchPaladin:50:100:TEMP:1735574400

        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

        string syncFile = ResolveSyncPath(syncFilePath);
        string myKey = $"{Bot.Player.Username}|{Bot.Player.CurrentClass?.Name ?? "Peasant"}".Replace(":", "-");
        int myQty = isTemp
            ? (Bot?.TempInv?.GetQuantity(itemName) ?? 0)
            : (Bot?.Inventory?.GetQuantity(itemName) ?? 0);

        // Update my progress
        UpdateEntry(syncFile, myKey, $"{myQty}:{targetQuantity}:{(isTemp ? "TEMP" : "INV")}");

        // Read file and check all members
        string[] lines = ReadLines(syncFile);
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        const int staleThreshold = 600; // 10 min

        int activeMembers = 0;
        int completedMembers = 0;

        foreach (string line in lines)
        {
            string[] parts = line.Split(':');
            // Format: KEY:current:target:TYPE:timestamp (5 parts minimum)
            if (parts.Length < 5)
                continue;

            // Parse quantities
            if (!int.TryParse(parts[1], out int current))
                continue;
            if (!int.TryParse(parts[2], out int target))
                continue;

            // Parse timestamp (last part)
            if (!long.TryParse(parts[4], out long ts))
                continue;

            // Skip stale entries
            if (now - ts > staleThreshold)
                continue;

            activeMembers++;
            if (current >= target)
                completedMembers++;
        }

        return activeMembers > 0 && completedMembers == activeMembers;
    }

    public bool CheckArmyProgressBool(Func<bool> condition, string syncFilePath = "army_sync.sync")
    {
        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

        if (string.IsNullOrWhiteSpace(Bot.Player.Username))
            Bot.Wait.ForTrue(() => !string.IsNullOrWhiteSpace(Bot.Player.Username), 20);

        string? username = Bot.Player.Username;
        string? className = Bot.Player.CurrentClass?.Name;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(className))
            return false;

        string syncFile = ResolveSyncPath(syncFilePath);
        string myKey = $"{username}|{className}".Replace(":", "-");

        bool myCondition = condition();
        UpdateEntry(syncFile, myKey, myCondition ? "1" : "0");

        string[] lines = ReadLines(syncFile);
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        const int staleThreshold = 600;

        int activeMembers = 0;
        int completedMembers = 0;

        foreach (string line in lines)
        {
            string[] parts = line.Split(':');
            if (parts.Length < 3)
                continue;

            string[] keyParts = parts[0].Split('|');
            if (keyParts.Length < 1 || string.IsNullOrWhiteSpace(keyParts[0]))
                continue;

            if (!int.TryParse(parts[1], out int status))
                continue;

            if (!long.TryParse(parts[2], out long ts))
                continue;

            if (now - ts > staleThreshold)
                continue;

            activeMembers++;
            if (status == 1)
                completedMembers++;
        }

        return activeMembers > 0 && completedMembers == activeMembers;
    }

    public void ClearSyncFile(string filePath)
    {
        try
        {
            // If file doesn't exist → create it empty.
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "");
                Bot?.Log($"[ArmySync] Created fresh sync file: {filePath}");
                return;
            }

            // If file exists but is already empty → do nothing.
            FileInfo fi = new(filePath);
            if (fi.Length == 0)
            {
                Bot?.Log("[ArmySync] Sync file already empty — no action needed.");
                return;
            }

            // Clear it.
            File.WriteAllText(filePath, "");
            Bot?.Log("[ArmySync] Sync file cleared.");
        }
        catch (Exception ex)
        {
            Bot?.Log($"[ArmySync] ERROR clearing sync file: {ex.Message}");
        }
    }

    // -------------------------------------------------------
    // Resolve a safe writable path for the sync file
    // -------------------------------------------------------
    public string ResolveSyncPath(string path)
    {
        try
        {
            string expanded = Environment.ExpandEnvironmentVariables(path);
            string? dir = Path.GetDirectoryName(expanded);

            if (Path.IsPathRooted(expanded) && !string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                return expanded;

            string baseDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Skua",
                "Options"
            );

            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string final = Path.Combine(baseDir, Path.GetFileName(path));

            // Ensure file exists (retry for Windows lock)
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    if (!File.Exists(final))
                    {
                        using FileStream fs = new(
                            final,
                            FileMode.OpenOrCreate,
                            FileAccess.Write,
                            FileShare.ReadWrite
                        );
                    }
                    return final;
                }
                catch (IOException)
                {
                    Bot?.Sleep(50);
                }
            }

            return final;
        }
        catch
        {
            return Path.GetFullPath(path);
        }
    }

    // -------------------------------------------------------
    // Read all lines from a file with retry
    // -------------------------------------------------------
    public string[] ReadLines(string path)
    {
        for (int i = 0; i < 12; i++)
        {
            try
            {
                using FileStream fs = new(
                    path,
                    FileMode.OpenOrCreate,
                    FileAccess.Read,
                    FileShare.ReadWrite
                );
                using StreamReader sr = new(fs);
                string raw = sr.ReadToEnd();
                return raw.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch (IOException)
            {
                Bot?.Sleep(50);
            }
        }
        return Array.Empty<string>();
    }

    // -------------------------------------------------------
    // Insert or update a key in the sync file
    // -------------------------------------------------------
    public void UpdateEntry(string path, string key, string payload)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        for (int attempt = 0; attempt < 50; attempt++)
        {
            try
            {
                // Open the file exclusively for the full read/modify/write cycle.
                // This avoids lost updates when multiple clients are writing the same sync file.
                List<string> lines = new List<string>();
                string stamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                string entry = $"{key}:{payload}:{stamp}";

                using (var fs = new FileStream(
                    path,
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite,
                    FileShare.None))
                {
                    using (var reader = new StreamReader(fs, leaveOpen: true))
                    {
                        string? line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                                lines.Add(line);
                        }
                    }

                    int idx = lines.FindIndex(l =>
                    {
                        string[] parts = l.Split(':');
                        return parts.Length > 0 &&
                            parts[0].Equals(key, StringComparison.OrdinalIgnoreCase);
                    });

                    if (idx >= 0)
                        lines[idx] = entry;
                    else
                        lines.Add(entry);

                    fs.SetLength(0);
                    fs.Position = 0;

                    using (var writer = new StreamWriter(fs))
                    {
                        foreach (var line in lines)
                        {
                            writer.WriteLine(line);
                        }
                        writer.Flush();
                        fs.Flush(true);
                    }
                }

                return; // Success
            }
            catch (IOException)
            {
                Bot?.Sleep(100 + (attempt * 20));
            }
        }

        Bot?.Log($"[ArmySync] Failed to update {path} after retries");
    }

    // -------------------------------------------------------
    // Sync-based class equipping for army comps
    // -------------------------------------------------------
    public string EquipClassSync(
        string[][] classSlots,
        int armySize,
        string syncFilePath = "class_assign.sync",
        bool allowDuplicates = false,
        Dictionary<string, string>? preferredUsernameAssignments = null
    )
    {
        if (classSlots == null || classSlots.Length == 0 || armySize < 1)
            return string.Empty;

        string syncFile = ResolveSyncPath(syncFilePath);

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
            if (C.CheckInventory(cls, toInv: true))
                myClasses.Add(cls);
        }

        string username = Bot.Player.Username;
        string payload = string.Join(",", myClasses);
        UpdateEntry(syncFile, username, payload);
        Bot?.Log(
            "[EquipClassSync] Account owns: " + (myClasses.Count > 0 ? payload : "NONE of the needed classes")
        );

        string FormatAccountName(string user)
        {
            string[] lines = ReadLines(syncFile);
            var users = lines
                .Select(l => l.Split(':'))
                .Where(parts => parts.Length > 0)
                .Select(parts => parts[0])
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(u => u, StringComparer.OrdinalIgnoreCase)
                .ToList();

            for (int i = 0; i < users.Count; i++)
            {
                if (string.Equals(users[i], user, StringComparison.OrdinalIgnoreCase))
                    return $"Account-{i + 1}";
            }

            return user;
        }

        // Wait for all members to register
        const int staleThreshold = 600;
        int lastCount = -1;

        while (!Bot!.ShouldExit)
        {
            string[] lines = ReadLines(syncFile);
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            int validCount = 0;

            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length < 3)
                    continue;
                if (!long.TryParse(parts[parts.Length - 1], out long ts))
                    continue;
                if (now - ts <= staleThreshold)
                    validCount++;
            }

            if (validCount != lastCount)
            {
                lastCount = validCount;
                Bot?.Log($"[EquipClassSync] Registered: {validCount}/{armySize}");
            }

            if (validCount >= armySize)
                break;

            // Re-poke to keep entry fresh
            UpdateEntry(syncFile, username, payload);
            Bot?.Sleep(500);
        }

        if (Bot?.ShouldExit == true)
            return string.Empty;

        // Phase 2: Mark self as READY with classes preserved
        // Format: username:READY|class1,class2,...:timestamp
        string readyPayload = $"READY|{string.Join(",", myClasses)}";
        UpdateEntry(syncFile, username, readyPayload);
        Bot?.Log($"[EquipClassSync] {FormatAccountName(username)} marked READY, waiting for all...");

        // Wait for all clients to be READY (ensures no more class-list writes)
        while (!Bot!.ShouldExit)
        {
            string[] lines = ReadLines(syncFile);
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            int readyCount = 0;

            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length < 3)
                    continue;
                // Check if payload starts with "READY|"
                if (!parts[1].StartsWith("READY|", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (!long.TryParse(parts[parts.Length - 1], out long ts))
                    continue;
                if (now - ts <= staleThreshold)
                    readyCount++;
            }

            if (readyCount >= armySize)
            {
                Bot?.Log($"[EquipClassSync] All {readyCount} clients READY. Reading final state...");
                break;
            }

            Bot?.Sleep(300);
        }

        if (Bot?.ShouldExit == true)
            return string.Empty;

        // Small buffer to ensure file system has flushed all writes
        Bot?.Sleep(500);

        // Parse entries: player - owned classes
        Dictionary<string, List<string>> playerClasses =
            new(StringComparer.OrdinalIgnoreCase);
        {
            string[] lines = ReadLines(syncFile);
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

        Bot?.Log($"[EquipClassSync] {playerClasses.Count} player(s) registered. Assigning classes...");

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
                    Bot?.Log($"[EquipClassSync] Preferred assignment requested for '{preferredUser}' but that user is not registered.");
                    continue;
                }

                if (!userClasses.Any(c => c.Equals(preferredClass, StringComparison.OrdinalIgnoreCase)))
                {
                    Bot?.Log($"[EquipClassSync] Preferred assignment for '{preferredUser}' cannot be honored because they do not own '{preferredClass}'.");
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
                    Bot?.Log($"[EquipClassSync] Preferred slot {s} > {FormatAccountName(preferredUser)} ({preferredClass})");
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
                Bot?.Log($"[EquipClassSync] Slot {s} > {FormatAccountName(best)} ({acceptedClass})");
                filled = true;
                break;
            }

            if (!filled)
                Bot?.Log($"[EquipClassSync] WARNING: No candidate for slot {s} ({string.Join("/", classSlots[s])})!");
        }

        // Find this client's assignment
        if (!assignments.TryGetValue(username, out string? myClass) || string.IsNullOrEmpty(myClass))
        {
            C.Logger($"[EquipClassSync] {FormatAccountName(username)} was not assigned any class! Check that all of your accounts own the required classes.", "EquipClassSync", true, true);
            return string.Empty;
        }

        Bot?.Log($"[EquipClassSync] - Equipping: {myClass}");
        C.Equip(myClass);
        Bot?.Sleep(1000);

        // Clear sync file for next run
        try { File.WriteAllText(syncFile, ""); } catch { }

        return myClass;
    }

    /// <summary>
    /// one class per slot overload (no alternates).
    /// </summary>
    public string EquipClassSync(
        string[] classes,
        int armySize,
        string syncFilePath = "class_assign.sync",
        bool allowDuplicates = false
    )
    {
        string[][] wrapped = new string[classes.Length][];
        for (int i = 0; i < classes.Length; i++)
            wrapped[i] = new[] { classes[i] };
        return EquipClassSync(wrapped, armySize, syncFilePath, allowDuplicates);
    }

    // --- next set ---------------------------------------------------------------

    public void GetScrollOfEnrage()
    {
        const int desiredCount = 200;

        if (!Core.Faction("SpellCrafting", 5))
            return;

        const string parchment = "Mystic Parchment";
        const string ink = "Zealous Ink";
        const string scroll = "Scroll of Enrage";

        while (!C.CheckInventory(scroll, desiredCount))
        {
            // Mats
            Core.ForItem("Undead Infantry", "underworld", parchment, 2);
            Core.BuyItem(ink, 549, "dragonrune", 5, calculateRemaining: false);

            // Craft
            Core.Join("spellcraft");
            Bot.Drops.Add(scroll);
            Bot.Send.Packet("%xt%zm%crafting%1%spellOnStart%7%1555%Spell%");
            Bot.Sleep(5000);
            Bot.Send.Packet("%xt%zm%crafting%1%spellComplete%7%2330%Enrage%");

            Core.WaitForDrop(scroll, 10000);
            Core.Pickup(scroll);
            Bot.Drops.Remove(scroll);

            if (Bot.ShouldExit)
                break;
        }

        Core.EquipEnrage();
    }

    public void UseTaunt()
    {
        // Dead → wait for respawn
        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

        while (!Bot.ShouldExit && !Bot.Target.Auras.Any(a => a != null && a.Name == "Focus"))
        {
            Core.Cast(5);
            Bot.Sleep(100);
        }
    }

    public void UseTaunt(int target)
    {
        // Attack the target first
        Bot.Combat.Attack(target);
        Bot.Sleep(200);

        // Then apply taunt
        while (!Bot.ShouldExit && !Bot.Target.Auras.Any(a => a != null && a.Name == "Focus"))
        {
            Core.Cast(5);
            Bot.Sleep(200);
        }
    }

    public void GetScrollOfDecay()
    {
        if (!Core.Faction("SpellCrafting", 5))
            return;

        const string parchment = "Mystic Parchment";
        const string ink = "Zealous Ink";
        const string scroll = "Scroll of Decay";

        while (!C.CheckInventory(scroll, 20))
        {
            Core.ForItem("Undead Infantry", "underworld", parchment, 2);
            Core.BuyItem(ink, 549, "dragonrune", 5, calculateRemaining: false);

            Core.Join("spellcraft");
            Bot.Drops.Add(scroll);
            Bot.Send.Packet("%xt%zm%crafting%1%spellOnStart%7%1555%Spell%");
            Bot.Sleep(5000);
            Bot.Send.Packet("%xt%zm%crafting%1%spellComplete%7%2331%Decay%");

            Core.WaitForDrop(scroll, 5000);
            Core.Pickup(scroll);
        }

        Core.EquipConsumable(scroll);
    }

    public void GetDivineElixir()
    {
        Core.ForItem("Xavier Lionfang", "poisonforest", "Divine Elixir");
        Core.EquipConsumable("Divine Elixir");
        Core.UsePotion();
    }

    /// <summary>
    /// Sends the player to their house. Falls back to yulgar if no house is set up.
    /// Call this at the end of any Ultra fight loop after completing the quest.
    /// </summary>
    public void JoinHouse()
    {
        if (Bot.Player.InCombat || Bot.Player.HasTarget)
            C.JumpWait();

        try
        {
            if (Bot.House.Items.Any(h => h.Equipped))
            {
                string? toSend = null;

                void modifyPacket(dynamic packet)
                {
                    try
                    {
                        string type = packet["params"].type;
                        dynamic data = packet["params"].dataObj;
                        if (type is "json" && data.houseData is not null)
                            toSend = packet.ToString();
                    }
                    catch { }
                }

                Bot.Events.ExtensionPacketReceived += modifyPacket;
                Bot.Send.Packet($"%xt%zm%house%1%{Bot.Player.Username}%");

                if (Bot.Wait.ForTrue(() => toSend != null, 20))
                    Bot.Send.ClientPacket(toSend!, "json");

                Bot.Wait.ForMapLoad("house");
                Bot.Events.ExtensionPacketReceived -= modifyPacket;

                for (int i = 0; i < 7; i++)
                    Bot.Send.ClientServer(" ", "");

                if (!string.Equals(Bot.Map.Name, "house", StringComparison.OrdinalIgnoreCase))
                {
                    for (int attempt = 0; attempt < 3 && !string.Equals(Bot.Map.Name, "house", StringComparison.OrdinalIgnoreCase); attempt++)
                    {
                        Bot.Sleep(1000);
                        C.JumpWait();

                        toSend = null;
                        Bot.Events.ExtensionPacketReceived += modifyPacket;
                        Bot.Send.Packet($"%xt%zm%house%1%{Bot.Player.Username}%");

                        if (Bot.Wait.ForTrue(() => toSend != null, 20))
                            Bot.Send.ClientPacket(toSend!, "json");

                        Bot.Wait.ForMapLoad("house");
                        Bot.Events.ExtensionPacketReceived -= modifyPacket;

                        for (int i = 0; i < 7; i++)
                            Bot.Send.ClientServer(" ", "");
                    }
                }

                if (!string.Equals(Bot.Map.Name, "house", StringComparison.OrdinalIgnoreCase))
                {
                    C.Logger("[CoreUltra] House join did not succeed after retries. Staying in current map.");
                }
            }
            else
            {
                C.Logger("[CoreUltra] No house available, falling back to Yulgar.");
                C.Join("yulgar-100000");
            }
        }
        catch (Exception ex)
        {
            C.Logger($"[CoreUltra] JoinHouse failed: {ex.Message}. Falling back to Yulgar.");
            C.Join("yulgar-100000");
        }
    }

    #region Alchemy

    public void UseAlchemyPotions(params string[] names)
    {
        if (names.Length == 0)
            return;

        static string Aura(string x) =>
            x switch
            {
                "Might Tonic" => "Might",
                "Sage Tonic" => "Sage",
                _ => x,
            };

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var raw in names)
        {
            if (string.IsNullOrWhiteSpace(raw))
                continue;
            if (!seen.Add(raw))
                continue;

            var aura = Aura(raw);
            if (Bot.Self.Auras.Any(a => a != null && a.Name == aura))
            {
                Core.Log("POTION", $"🟢 Already on: {aura}");
                continue;
            }

            Core.Log("POTION", $"🧪 Queueing: {raw} ({aura})");
            BuyAlchemyPotion(raw);

            for (
                int tries = 0;
                tries < 3
                    && !Bot.Self.Auras.Any(a => a != null && a.Name == aura)
                    && !Bot.ShouldExit;
                tries++
            )
            {
                Core.EquipConsumable(raw);
                if (Bot.Inventory.IsEquipped(raw))
                {
                    Core.UsePotion();
                    long t0 = Environment.TickCount64;
                    while (
                        !Bot.ShouldExit
                        && !Bot.Self.Auras.Any(a => a.Name == aura)
                        && Environment.TickCount64 - t0 < 1500
                    )
                        Bot.Sleep(50);
                }
                else
                    Bot.Sleep(200);
            }

            if (Bot.Self.Auras.Any(a => a.Name == aura))
                Core.Log("POTION", $"✅ Applied: {aura}");
            else
                Core.Log("POTION", $"❌ Nope: {raw} ({aura})");
        }
    }

    public void BuyAlchemyPotion(string n)
    {
        if (string.IsNullOrWhiteSpace(n) || C.CheckInventory(n))
        {
            if (!string.IsNullOrWhiteSpace(n))
                Core.Log("POTION", $"🧴 Have: {n}");
            return;
        }

        int shop = 2036;
        string map = "alchemyacademy";
        string voucher = "Gold Voucher 500k";

        void NeedV(int want)
        {
            int miss = Math.Max(0, want - Bot.Inventory.GetQuantity(voucher));
            if (miss > 0)
            {
                Core.Log("POTION", $"💰 Need {miss}× {voucher}");
                Core.BuyItem(voucher, shop, map, miss);
            }
        }

        void Grab(int count)
        {
            Core.Log("POTION", $"🛒 {n} ×{count}");
            Core.BuyItem(n, shop, map, count, calculateRemaining: false);
        }

        switch (n)
        {
            case "Might Tonic":
                if (!Core.Faction("Alchemy", 8))
                {
                    Core.Log("POTION", "⛔ Alchemy rep 8 required");
                    return;
                }
                NeedV(2);
                Grab(10);
                break;

            case "Sage Tonic":
                if (!Core.Faction("Alchemy", 8))
                {
                    Core.Log("POTION", "⛔ Alchemy rep 8 required");
                    return;
                }
                NeedV(2);
                Grab(10);
                break;

            case "Potent Malevolence Elixir":
                NeedV(4);
                Grab(8);
                break;

            case "Potent Battle Elixir":
                NeedV(4);
                Grab(8);
                break;

            case "Potent Honor Potion":
                if (!Core.Faction("Good", 10))
                {
                    Core.Log("POTION", "⛔ Good rep 10 required");
                    return;
                }
                NeedV(1);
                Grab(5);
                break;

            default:
                Core.Log("POTION", $"❓ Unknown: {n}");
                return;
        }
    }

    public string GetBestTonicPotion()
    {
        var str = Core.GetStatValue("STR");
        var intel = Core.GetStatValue("INT");
        var pick = str > intel ? "Might Tonic" : "Sage Tonic";
        Core.Log("Potion", $"🧪 Tonic → {pick} (STR {str}, INT {intel})");
        return pick;
    }

    public string GetBestElixirPotion()
    {
        var str = Core.GetStatValue("STR");
        var intel = Core.GetStatValue("INT");
        var pick = str > intel ? "Potent Battle Elixir" : "Potent Malevolence Elixir";
        Core.Log("Potion", $"🧪 Elixir → {pick} (STR {str}, INT {intel})");
        return pick;
    }

    #endregion

    // --- next set ---------------------------------------------------------------

    #region Listeners

    private volatile bool _chargeDetected;
    private int _chargeSeq;

    public async void GenericChargeListener(dynamic packet)
    {
        try
        {
            if (packet?["params"]?.type?.ToString() != "json")
                return;
            dynamic data = packet["params"].dataObj;
            if (data?.cmd?.ToString() != "ct")
                return;

            var anims = data?.anims as System.Collections.IEnumerable;
            if (anims == null)
                return;

            foreach (var anim in anims)
            {
                if (
                    (anim as dynamic)
                        ?.animStr?.ToString()
                        ?.Equals("Charge", StringComparison.OrdinalIgnoreCase) == true
                )
                {
                    _chargeDetected = true;

                    // mark this as the latest charge
                    int mySeq = Interlocked.Increment(ref _chargeSeq);

                    // wait 3s; only clear if no newer charge happened
                    await Task.Delay(3000);
                    if (mySeq == _chargeSeq)
                        _chargeDetected = false;

                    break;
                }
            }
        }
        catch { }
    }

    public void Enhancements()
    {
        string? playerName = Bot.Player?.Username;
        if (string.IsNullOrWhiteSpace(playerName))
        {
            C.Logger("[ERROR] Unable to determine player name.", stopBot: true);
            return;
        }
        var acceptableDPSClasses = new HashSet<string>
        {
            "Legion Revenant",
            "Chrono ShadowSlayer",
            "Lich",
            "Archfiend",
            "Quantum Chronomancer",
            "Hollowborn Vindicator",
            "Arachnomancer",
            "Infinity Knight",
            "Verus DoomKnight",
            "King's Echo",
            "Phantom Chronomancer",
            "Great Thief",
        };

        // Cache currently equipped items
        InventoryItem? weaponItem = Bot.Inventory?.Items?.FirstOrDefault(x =>
            x?.Equipped == true && Adv.WeaponCatagories.Contains(x.Category)
        );
        InventoryItem? helmItem = Bot.Inventory?.Items?.FirstOrDefault(x =>
            x?.Equipped == true && x.Category == ItemCategory.Helm
        );
        InventoryItem? capeItem = Bot.Inventory?.Items?.FirstOrDefault(x =>
            x?.Equipped == true && x.Category == ItemCategory.Cape
        );
        string weapon = weaponItem?.Name ?? "";
        string helm = helmItem?.Name ?? "";
        string cape = capeItem?.Name ?? "";
        string className = Bot.Player?.CurrentClass?.Name ?? "";
        C.Logger(
            $"[Enhancement]\nClass: {className}\nWeapon: {weapon}\nHelm: {helm}\nCape: {cape}",
            "info"
        );
        // Apply enhancement rules per role
        switch (className.ToLower())
        {
            case "chaos avenger":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Anima);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Vainglory);
                break;

            case "archpaladion":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Forge);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Lament);
                break;

            case "legion revenant":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Pneuma);
                Adv.EnhanceItem(className, EnhancementType.Wizard);
                Adv.EnhanceItem(weapon, EnhancementType.Wizard, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Wizard, CapeSpecial.Vainglory);
                break;

            case "archfiend":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Forge);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Vainglory);
                break;

            case "arachnomancer":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Anima);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Vainglory);
                break;

            case "king's echo":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Pneuma);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Lament);
                break;

            case "chrono shadowslayer":
            case "chrono shadowhunter":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Vim);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Health_Vamp);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Lament);
                break;

            case "quantum chronomancer":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Anima);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Vainglory);
                break;

            case "phantom chronomancer":
                Adv.EnhanceItem(helm, EnhancementType.Wizard, hSpecial: HelmSpecial.Pneuma);
                Adv.EnhanceItem(className, EnhancementType.Wizard);
                Adv.EnhanceItem(weapon, EnhancementType.Wizard, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Wizard, CapeSpecial.Vainglory); // or Lament if needed
                break;

            case "infinity knight":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Pneuma);
                Adv.EnhanceItem(className, EnhancementType.Wizard);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Wizard, CapeSpecial.Vainglory);
                break;

            case "lich":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Examen);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Ravenous);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Penitence);
                break;

            case "verus doomknight":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Anima);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Vainglory);
                break;

            case "hollowborn vindicator":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Forge);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Penitence);
                break;

            case "great thief":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Forge); // or Vim if needed
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Dauntless); // or Lucky HealthVamp
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Vainglory);
                break;

            case "dragon of time":
                Adv.EnhanceItem(helm, EnhancementType.Wizard, hSpecial: HelmSpecial.Pneuma); // or Vim if needed
                Adv.EnhanceItem(className, EnhancementType.Wizard);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Elysium); // or Lucky HealthVamp
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Vainglory);
                break;

            case "arcana invoker":
                Adv.EnhanceItem(helm, EnhancementType.Wizard, hSpecial: HelmSpecial.Examen); // or Vim if needed
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(
                    weapon,
                    EnhancementType.Lucky,
                    wSpecial: Adv.uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Valiance
                ); // or Lucky HealthVamp
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Vainglory);
                break;
        }
    }

    // You can either make a secondary void or add all of the below to the first one under the first item.
    public void ArmyHandler(
        string map,
        int[] QuestIDs,
        string WaitForArmysyncPath,
        string AggroCell,
        CheckType checkType,
        string? Itemname = null,
        int quant = 0,
        bool isTemp = false,
        bool UseBool = false,
        Func<bool>? condition = null,
        int PlayerCount = 0,
        string? QuestReward = null
    )
    {
        // Sync file used to keep track of what accs are done.
        string syncPath = ResolveSyncPath(WaitForArmysyncPath);
        ClearSyncFile(WaitForArmysyncPath);

        // Log Players in current army.
        C.Logger($"Players in Curreny Army: {PlayerCount}");

        // Uncomment below, and add any questids that are used for this portion
        // C.RegisterQuests(1,2,3);
        if (QuestReward != null)
            C.AddDrop(QuestReward);

        Core.Join(map);

        C.Jump(AggroCell, "Left");

        // Don't Touch vv
        if (PlayerCount > 1)
            // Dont make this the same as the syncPath
            WaitForArmy(PlayerCount - 1, WaitForArmysyncPath);
        Bot.Player.SetSpawnPoint();
        Bot.Sleep(1500);
        Bot.Options.AggroMonsters = true;
        // Pick a variant below ( multiple can be used as long as the sync files are different.)

        if (UseBool && condition != null)
        {
            // Bool variant
            while (!Bot.ShouldExit)
            {
                // Replace the `Bot.Player.Level >= 100` below with the bool
                // you want want all accs to have true, leave the rest of this alone.
                if (CheckArmyProgressBool(condition, syncPath))
                {
                    Bot.Options.AggroMonsters = false;
                    C.Jump("Enter", "Spawn");
                    C.Logger("All players finished farm.");
                    break;
                }
                // Dead → wait for respawn
                if (!Bot.Player.Alive)
                {
                    Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                    continue;
                }

                Bot.Combat.Attack("*");
                Bot.Sleep(500);
            }
            return;
        }

        if (Itemname != null)
        {
            //Int variant
            while (!Bot.ShouldExit)
            {
                // Replace `Itemname` with the wanted item
                // Replace the 500 with the quantity you desire
                // Replace `false` if the item is a temp item with `true` or leave as `false` for non-temp items.
                if (CheckArmyProgress(Itemname, quant, false, syncPath))
                {
                    Bot.Options.AggroMonsters = false;
                    C.Jump("Enter", "Spawn");
                    C.Logger("All players finished farm.");
                    break;
                }

                // Dead → wait for respawn
                if (!Bot.Player.Alive)
                {
                    Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                    continue;
                }

                Bot.Combat.Attack("*");
                Bot.Sleep(500);
            }
            return;
        }
    }

    public enum CheckType
    {
        Bool = 1,
        Item = 2,
    }

    #endregion

    /// <summary>
    /// Joins the player's house and keeps retrying until successful.
    /// Unlike <see cref="JoinHouse"/>, this never gives up — it loops until the map is "house".
    /// Call this when you absolutely must land in the house before proceeding.
    /// </summary>
    public void PersistentJoinHouse()
    {
        if (Bot.Player.InCombat || Bot.Player.HasTarget)
            C.JumpWait();

        // No house equipped → fall back to Yulgar (can't retry what doesn't exist)
        if (!Bot.House.Items.Any(h => h.Equipped))
        {
            C.Logger("[CoreUltra] No house available, falling back to Yulgar.");
            C.Join("yulgar-100000");
            return;
        }

        while (!Bot.ShouldExit)
        {
            try
            {
                // Go to whitemap to disengage from combat/boss map
                C.Join("whitemap-100000");
                // De-aggro after joining safe map
                C.JumpWait();

                string? toSend = null;

                void modifyPacket(dynamic packet)
                {
                    try
                    {
                        string type = packet["params"].type;
                        dynamic data = packet["params"].dataObj;
                        if (type is "json" && data.houseData is not null)
                            toSend = packet.ToString();
                    }
                    catch { }
                }

                Bot.Events.ExtensionPacketReceived += modifyPacket;
                Bot.Send.Packet($"%xt%zm%house%1%{Bot.Player.Username}%");

                if (Bot.Wait.ForTrue(() => toSend != null, 20))
                    Bot.Send.ClientPacket(toSend!, "json");

                Bot.Wait.ForMapLoad("house");
                Bot.Events.ExtensionPacketReceived -= modifyPacket;

                for (int i = 0; i < 7; i++)
                    Bot.Send.ClientServer(" ", "");

                if (string.Equals(Bot.Map.Name, "house", StringComparison.OrdinalIgnoreCase))
                    return; // success
            }
            catch (Exception ex)
            {
                C.Logger($"[CoreUltra] PersistentJoinHouse failed: {ex.Message}");
            }
            
            Bot.Sleep(1000);
        }
    }
}

