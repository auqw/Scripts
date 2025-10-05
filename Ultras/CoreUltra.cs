//cs_include Scripts/Ultras/CoreEngine.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class CoreUltra
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine Core = new();

    public void Test()
        => Bot.Log("NewCore interface OK!");

    public void TauntCycle(string name, string monster, string aura, int checkDelay)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(monster) || string.IsNullOrWhiteSpace(aura)) return;
        if (Bot?.Combat == null) return;
        if (Core.HasClassEquipped(name))
        {
            int effect = Core.GetAuraSecondsRemaining(aura);
            Bot.Combat.Attack(monster);
            if (checkDelay > 0) Bot.Sleep(checkDelay);
            if (effect < 2) Core.UsePotion();
        }
    }

    public void TauntCharge(string name, string monster, string aura, int checkDelay)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(monster)) return;
        if (Bot?.Combat == null) return;
        if (Core.HasClassEquipped(name))
        {
            Bot.Combat.Attack(monster);
            if (checkDelay > 0) Bot.Sleep(checkDelay);
            if (_chargeDetected) Core.UsePotion();
        }
    }

    public void KillWithPriority(string primaryName, int primaryMapId, string priorityName1, int priorityMapId1, string priorityName2, int priorityMapId2)
    {
        if (string.IsNullOrWhiteSpace(primaryName)) return;
        if (!string.IsNullOrWhiteSpace(priorityName1) && Core.IsAliveByMapId(priorityMapId1, name: priorityName1)) KillByMapId(priorityMapId1, name: priorityName1);
        else if (!string.IsNullOrWhiteSpace(priorityName2) && Core.IsAliveByMapId(priorityMapId2, name: priorityName2)) KillByMapId(priorityMapId2, name: priorityName2);
        else KillByMapId(primaryMapId, name: primaryName);
        Bot.Sleep(Core.D1);
    }

    public void KillByMapId(int mapId, string? name = null, int? id = null)
    {
        if (Bot?.Combat == null) return;
        if (Core.IsAliveByMapId(mapId, name, id))
        {
            Bot.Combat.Attack(mapId);
            Bot.Sleep(250);
        }
    }

    public bool MonsterAlive(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        if (Bot?.Monsters?.MapMonsters == null) return false;

        return Bot.Monsters.MapMonsters
            .Any(m => m?.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) == true && m.Alive);
    }

    public void UltraWardenTaunter()
    {
        if (Bot == null) return;
        if (Bot.Combat == null) return;
        if (Bot.Player == null) return;

        Bot.Combat.Attack("Ultra Warden");

        var enemy = Bot.Player.Target;
        if (enemy == null) return;
        if (enemy.HP == null) return;
        if (enemy.HP <= 0) return;
        if (enemy.MaxHP <= 0) return;

        int hpNow = enemy.HP;
        int hpMax = enemy.MaxHP;

        int percentage = hpNow * 100;
        percentage = percentage / hpMax;

        int band = percentage / 5;
        band = band * 5;

        HashSet<int> bandsUsed;

        object savedBands = AppDomain.CurrentDomain.GetData("warden.usedThresholds");
        if (savedBands == null) bandsUsed = new HashSet<int>();
        else bandsUsed = (HashSet<int>)savedBands;

        bool alreadyUsed = bandsUsed.Contains(band);

        if (!alreadyUsed)
        {
            double exactPercent = hpNow;
            exactPercent = exactPercent / hpMax;
            exactPercent = exactPercent * 100;

            bandsUsed.Add(band);
            AppDomain.CurrentDomain.SetData("warden.usedThresholds", bandsUsed);

            bool keepGoing = true;
            while (keepGoing)
            {
                bool wardenAlive = MonsterAlive("Ultra Warden");
                bool haveFocus = Core.HasAura("Focus");
                bool shouldStop = Bot.ShouldExit;

                if (!wardenAlive) keepGoing = false;
                else if (haveFocus) keepGoing = false;
                else if (shouldStop) keepGoing = false;
                else Core.UsePotion();
            }
        }

        Bot.Sleep(150);
    }

    public void DrakathTaunter()
    {
        if (Bot == null) return;
        if (Bot.Combat == null) return;
        if (Bot.Player == null) return;

        int[] hpLevels = new int[9];
        hpLevels[0] = 18000000;
        hpLevels[1] = 16000000;
        hpLevels[2] = 14000000;
        hpLevels[3] = 12000000;
        hpLevels[4] = 10000000;
        hpLevels[5] = 8000000;
        hpLevels[6] = 6000000;
        hpLevels[7] = 4000000;
        hpLevels[8] = 2000000;

        int[] extraHP = new int[9];
        extraHP[0] = 200000;
        extraHP[1] = 200000;
        extraHP[2] = 200000;
        extraHP[3] = 200000;
        extraHP[4] = 200000;
        extraHP[5] = 120000;
        extraHP[6] = 120000;
        extraHP[7] = 120000;
        extraHP[8] = 120000;

        EnsureDrakathTarget();

        var enemy = Bot.Player.Target;
        if (enemy == null) return;
        if (enemy.HP == null) return;
        if (enemy.HP <= 0) return;

        int lastLevel = int.MaxValue;
        int oldHP = int.MaxValue;
        long oldTime = 0;

        object tempLastLevel = AppDomain.CurrentDomain.GetData("drakath.lastThreshold");
        if (tempLastLevel != null) lastLevel = (int)tempLastLevel;

        object tempOldHP = AppDomain.CurrentDomain.GetData("drakath.prevHp");
        if (tempOldHP != null) oldHP = (int)tempOldHP;

        object tempOldTime = AppDomain.CurrentDomain.GetData("drakath.lastFireTicks");
        if (tempOldTime != null) oldTime = (long)tempOldTime;

        int nowHP = enemy.HP;
        long nowTime = DateTime.UtcNow.Ticks;

        long timeDifference = nowTime - oldTime;
        TimeSpan timeGap = new TimeSpan(timeDifference);
        double millisecondsWaited = timeGap.TotalMilliseconds;

        bool enoughTimeWaited = false;
        if (millisecondsWaited >= 1200) enoughTimeWaited = true;

        bool crossedThreshold = false;
        int whichLevel = 0;
        int whichExtra = 0;

        int i = 8;
        while (i >= 0)
        {
            int checkLevel = hpLevels[i];
            int checkExtra = extraHP[i];

            bool belowLastLevel = false;
            if (checkLevel < lastLevel) belowLastLevel = true;

            if (belowLastLevel)
            {
                int upperLimit = checkLevel + checkExtra;

                bool wasAbove = false;
                bool nowBelow = false;

                if (oldHP > upperLimit) wasAbove = true;
                if (nowHP <= upperLimit) nowBelow = true;

                if (wasAbove && nowBelow)
                {
                    crossedThreshold = true;
                    whichLevel = checkLevel;
                    whichExtra = checkExtra;
                    i = -1;
                }
            }

            i = i - 1;
        }

        if (enoughTimeWaited && crossedThreshold)
        {
            string message1 = "Crossed into band " + whichLevel.ToString("N0");
            string message2 = " (hp now " + nowHP.ToString("N0") + "). Attempting Focus...";
            Core.Log("Drakath", message1 + message2);

            AppDomain.CurrentDomain.SetData("drakath.lastThreshold", whichLevel);
            AppDomain.CurrentDomain.SetData("drakath.lastFireTicks", nowTime);

            DateTime stopTime = DateTime.UtcNow.AddMilliseconds(1800);
            int howManyTries = 0;

            bool keepTrying = true;
            while (keepTrying)
            {
                bool drakathAlive = MonsterAlive("Champion Drakath");
                bool shouldExit = Bot.ShouldExit;
                bool timeLeft = DateTime.UtcNow < stopTime;

                if (!drakathAlive || shouldExit || !timeLeft) keepTrying = false;
                else
                {
                    EnsureDrakathTarget();
                    Core.UsePotion();
                    howManyTries = howManyTries + 1;

                    bool gotFocus = Core.HasAura("Focus");
                    if (gotFocus) keepTrying = false;
                    else Bot.Sleep(120);
                }
            }

            bool hasFocusNow = Core.HasAura("Focus");

            int finalHP = 0;
            if (Bot.Player.Target != null) if (Bot.Player.Target.HP != null) finalHP = Bot.Player.Target.HP;

            if (hasFocusNow)
            {
                string msg = "Focus obtained at " + finalHP.ToString("N0") + " HP (tries: " + howManyTries + ")";
                Core.Log("Drakath", msg);
            }
            else
            {
                string msg = "Warning: Failed to get Focus (tries: " + howManyTries + ", hp now " + finalHP.ToString("N0") + ")";
                Core.Log("Drakath", msg);
            }
        }

        AppDomain.CurrentDomain.SetData("drakath.prevHp", nowHP);

        Bot.Sleep(120);
    }

    private void EnsureDrakathTarget()
    {
        var monsterList = new List<Skua.Core.Models.Monsters.Monster>();

        if (Bot != null && Bot.Monsters != null && Bot.Monsters.CurrentMonsters != null) monsterList = Bot.Monsters.CurrentMonsters.ToList();
        else if (Bot != null && Bot.Monsters != null && Bot.Monsters.MapMonsters != null) monsterList = Bot.Monsters.MapMonsters.ToList();
        else monsterList = new List<Skua.Core.Models.Monsters.Monster>();

        Skua.Core.Models.Monsters.Monster foundDrakath = null;

        int i = 0;
        while (i < monsterList.Count)
        {
            var monster = monsterList[i];

            if (monster != null)
            {
                bool rightName = false;
                if (monster.Name == "Champion Drakath") rightName = true;

                bool isAlive = false;
                if (monster.Alive) isAlive = true;

                if (rightName && isAlive)
                {
                    foundDrakath = monster;
                    i = monsterList.Count;
                }
            }

            i = i + 1;
        }

        if (foundDrakath != null)
        {
            int monsterID = foundDrakath.MapID;
            Bot.Combat.Attack(monsterID);
            return;
        }

        Bot.Combat.Attack("Champion Drakath");
    }

    public void DontAttack()
    {
        if (Bot?.Combat == null || Bot?.Map == null || Bot?.Player == null) return;

        Bot.Combat.CancelAutoAttack();
        Bot.Combat.CancelTarget();

        Core.DisableSkills();
        Bot.Sleep(5000);
        Core.EnableSkills();
    }

    public void WaitForArmy(int quantity, string syncFilePath = "army_sync.txt", int bufferTimeMs = 3000, int tickMs = 500, int timeoutMs = 0)
    {
        if (Bot == null || Bot.Map == null) return;

        string ResolvePath(string requestedPath)
        {
            string EnsureWritable(string fullPath)
            {
                try
                {
                    var dir = Path.GetDirectoryName(fullPath);
                    if (string.IsNullOrWhiteSpace(dir)) return string.Empty;
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    using (var fs = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                        fs.Flush(true);
                    return fullPath;
                }
                catch (Exception ex) { Core.Log("Army", $"[sync-path] {ex.Message}"); return string.Empty; }
            }

            if (Path.IsPathRooted(requestedPath))
            {
                var ok = EnsureWritable(requestedPath);
                if (!string.IsNullOrEmpty(ok)) return ok;
            }

            var bases = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SkuaSync"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "SkuaSync"),
                Path.Combine(Path.GetTempPath(), "SkuaSync")
            };
            foreach (var b in bases)
            {
                var ok = EnsureWritable(Path.Combine(b, requestedPath));
                if (!string.IsNullOrEmpty(ok)) return ok;
            }
            return EnsureWritable(Path.GetFullPath(requestedPath));
        }

        void Truncate(string filePath)
        {
            try { using var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite); fs.SetLength(0); fs.Flush(true); }
            catch (Exception ex) { Core.Log("Army", $"[sync-trunc] {ex.Message}"); }
        }

        bool IsAllTrue(string filePath)
        {
            try
            {
                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var sr = new StreamReader(fs, Encoding.UTF8, true, 1024, leaveOpen: true);
                var text = sr.ReadToEnd();
                if (string.IsNullOrWhiteSpace(text)) return false;

                bool sawAny = false;
                foreach (var raw in text.Split('\n'))
                {
                    var line = raw.Trim();
                    if (line.Length == 0 || line.StartsWith("#")) continue;
                    if (line.EndsWith(": true", StringComparison.Ordinal)) { sawAny = true; continue; }
                    if (line.Contains(":")) return false;
                }
                return sawAny;
            }
            catch { return false; }
        }

        void InitFile(string filePath, int maxAgeMinutes)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    using var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fs.Flush(true);
                    return;
                }
                var last = File.GetLastWriteTimeUtc(filePath);
                if (DateTime.UtcNow - last > TimeSpan.FromMinutes(maxAgeMinutes) || IsAllTrue(filePath))
                    Truncate(filePath);
            }
            catch (Exception ex) { Core.Log("Army", $"[sync-init] {ex.Message}"); Truncate(filePath); }
        }

        void GetOrCreateEntry(string filePath, string key)
        {
            for (int attempt = 0; attempt < 12; attempt++)
            {
                try
                {
                    using var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                    string text;
                    using (var sr = new StreamReader(fs, Encoding.UTF8, true, 1024, leaveOpen: true))
                        text = sr.ReadToEnd();

                    var lines = new List<string>();
                    if (!string.IsNullOrWhiteSpace(text))
                        foreach (var raw in text.Split('\n'))
                        {
                            var ln = raw.Trim();
                            if (ln.Length > 0) lines.Add(ln);
                        }

                    // If our exact key already exists, do nothing
                    int idx = lines.FindIndex(l => l.StartsWith(key + ":", StringComparison.Ordinal));
                    if (idx >= 0) return;

                    // Otherwise add "Key: false"
                    lines.Add($"{key}: false");

                    fs.SetLength(0);
                    using (var sw = new StreamWriter(fs, Encoding.UTF8, 1024, leaveOpen: true))
                    {
                        for (int i = 0; i < lines.Count; i++)
                        {
                            sw.Write(lines[i]);
                            if (i < lines.Count - 1) sw.Write("\n");
                        }
                        sw.Flush();
                    }
                    fs.Flush(true);
                    return;
                }
                catch (IOException) { Bot.Sleep(40); }
                catch (Exception ex) { Core.Log("Army", $"[sync-get] {ex.Message}"); return; }
            }
            Core.Log("Army", "[sync-get] retries exhausted.");
        }

        void SetReady(string filePath, string entryName, bool ready)
        {
            var want = $"{entryName}: {(ready ? "true" : "false")}";
            for (int attempt = 0; attempt < 8; attempt++)
            {
                try
                {
                    using var fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    string text; using (var sr = new StreamReader(fs, Encoding.UTF8, true, 1024, leaveOpen: true)) text = sr.ReadToEnd();

                    var lines = new List<string>();
                    if (!string.IsNullOrWhiteSpace(text))
                        foreach (var raw in text.Split('\n')) { var ln = raw.Trim(); if (ln.Length > 0) lines.Add(ln); }

                    bool found = false;
                    for (int i = 0; i < lines.Count; i++)
                        if (lines[i].StartsWith(entryName + ":", StringComparison.Ordinal)) { lines[i] = want; found = true; break; }
                    if (!found) lines.Add(want);

                    fs.SetLength(0);
                    using (var sw = new StreamWriter(fs, Encoding.UTF8, 1024, leaveOpen: true))
                    {
                        for (int i = 0; i < lines.Count; i++) { sw.Write(lines[i]); if (i < lines.Count - 1) sw.Write("\n"); }
                        sw.Flush();
                    }
                    fs.Flush(true);
                    return;
                }
                catch (IOException) { Bot.Sleep(40); }
                catch (Exception ex) { Core.Log("Army", $"[sync-set] {ex.Message}"); return; }
            }
            Core.Log("Army", "[sync-set] retries exhausted.");
        }

        int CountReady(string filePath)
        {
            for (int attempt = 0; attempt < 8; attempt++)
            {
                try
                {
                    if (!File.Exists(filePath)) return 0;
                    using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    string text; using (var sr = new StreamReader(fs, Encoding.UTF8, true, 1024, leaveOpen: true)) text = sr.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(text)) return 0;

                    int c = 0;
                    foreach (var raw in text.Split('\n'))
                        if (raw.TrimEnd().EndsWith(": true", StringComparison.Ordinal)) c++;
                    return c;
                }
                catch (IOException) { Bot.Sleep(40); }
                catch (Exception ex) { Core.Log("Army", $"[sync-read] {ex.Message}"); return 0; }
            }
            Core.Log("Army", "[sync-read] retries exhausted."); return 0;
        }

        void FinalSpamAndStart()
        {
            Core.Log("Army", "Final prep: spamming skills before pull...");
            var until = DateTime.UtcNow.AddMilliseconds(1200);
            while (DateTime.UtcNow < until && !Bot.ShouldExit)
            {
                if (!Core.IsManaLow(50))
                {
                    Bot.Skills.UseSkill(1); Bot.Sleep(300);
                    Bot.Skills.UseSkill(2); Bot.Sleep(300);
                    Bot.Skills.UseSkill(3);
                }
                Bot.Sleep(100);
            }

            const int startMs = 3000;
            Core.Log("Army", $"Everyone ready! Starting in {startMs}ms...");
            Bot.Sleep(startMs);
        }

        string RosterKey()
        {
            string user = (Bot?.Player?.Username ?? Guid.NewGuid().ToString("N").Substring(0, 6)).Trim();
            string cls = (Bot?.Player?.CurrentClass?.Name ?? "UnknownClass").Trim();

            string San(string s) => s.Replace(":", "-").Replace("\r", "").Replace("\n", "").Trim();

            return $"{San(user)} | {San(cls)}";
        }

        string path = ResolvePath(syncFilePath);
        Core.Log("Army", $"Sync file: {path}");
        InitFile(path, maxAgeMinutes: 15);

        string myName = RosterKey();

        GetOrCreateEntry(path, myName);
        SetReady(path, myName, false);

        int needed = Math.Max(1, quantity) + 1;

        var timer = new Stopwatch(); timer.Start();
        while (true)
        {
            if (Bot.ShouldExit) { Truncate(path); return; }
            int playersNow = Bot.Map.PlayerCount;
            if (playersNow >= needed) break;

            Core.Log("Army", $"Waiting for army: {Math.Max(0, playersNow - 1)}/{quantity} players in map");

            if (timeoutMs > 0 && timer.ElapsedMilliseconds >= timeoutMs)
            {
                Core.Log("Army", "Timeout while waiting for players to join map.");
                break;
            }
            Bot.Sleep(tickMs);
        }
        if (Bot.ShouldExit) { Truncate(path); return; }

        SetReady(path, myName, true);
        Core.Log("Army", $"Marked ready: {myName}");

        while (!Bot.ShouldExit)
        {
            int ready = CountReady(path);
            Core.Log("Army", $"Sync: {ready}/{needed} ready");
            if (ready >= needed) break;
            Bot.Sleep(tickMs);
        }
        if (Bot.ShouldExit) { Truncate(path); return; }

        FinalSpamAndStart();
        Truncate(path);
        Core.Log("Army", "GO!");
    }

    // --- next set ---------------------------------------------------------------

    public void GetScrollOfEnrage()
    {
        if (!Core.Faction("SpellCrafting", 5)) return;

        const string parchment = "Mystic Parchment";
        const string ink = "Zealous Ink";
        const string scroll = "Scroll of Enrage";

        if (Core.Owned(scroll) < 10)
        {
            // Mats
            Core.ForItem("Undead Infantry", "underworld", parchment, 2);
            Core.BuyItem(ink, 549, "dragonrune", 5, calculateRemaining: false);

            // Craft
            Core.Join("spellcraft");
            Bot.Drops.Add(scroll);
            Bot.Send.Packet("%xt%zm%crafting%1%spellOnStart%7%1555%Spell%"); Bot.Sleep(5000);
            Bot.Send.Packet("%xt%zm%crafting%1%spellComplete%7%2330%Enrage%");

            Core.WaitForDrop(scroll, 10000);
            Core.Pickup(scroll);
        }

        Core.EquipConsumable(scroll);
        Core.EquipRandomClassAndReequip();
    }

    public void GetScrollOfDecay()
    {
        if (!Core.Faction("SpellCrafting", 5)) return;

        const string parchment = "Mystic Parchment";
        const string ink = "Zealous Ink";
        const string scroll = "Scroll of Decay";

        while (Core.Owned(scroll) < 10 && !Bot.ShouldExit)
        {
            Core.ForItem("Undead Infantry", "underworld", parchment, 2);
            Core.BuyItem(ink, 549, "dragonrune", 5, calculateRemaining: false);

            Core.Join("spellcraft");
            Bot.Drops.Add(scroll);
            Bot.Send.Packet("%xt%zm%crafting%1%spellOnStart%7%1555%Spell%"); Bot.Sleep(5000);
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

    public void UseAlchemyPotions(params string[] names)
    {
        if (names == null || names.Length == 0) return;

        string Aura(string p) => p switch { "Might Tonic" => "Might", "Sage Tonic" => "Sage", _ => p };

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var raw in names)
        {
            if (string.IsNullOrWhiteSpace(raw)) continue;
            if (!seen.Add(raw)) continue;

            var aura = Aura(raw);
            if (Core.HasAura(aura, true)) continue;

            BuyAlchemyPotion(raw);

            for (int t = 0; t < 3 && !Core.HasAura(aura, true) && !Bot.ShouldExit; t++)
            {
                Core.EquipConsumable(raw);
                if (Bot.Inventory.IsEquipped(raw))
                {
                    Core.UsePotion();
                    long t0 = Environment.TickCount64;
                    while (!Bot.ShouldExit && !Core.HasAura(aura, true) && Environment.TickCount64 - t0 < 1500)
                        Bot.Sleep(50);
                }
                else Bot.Sleep(200);
            }
        }
    }

    public void BuyAlchemyPotion(string n)
    {
        if (string.IsNullOrWhiteSpace(n) || Core.Owned(n) >= 1) return;

        int S = 2036;
        string M = "alchemyacademy";
        string GV = "Gold Voucher 500k";

        void Vouchers(int need)
        {
            int missing = Math.Max(0, need - Core.Owned(GV));
            if (missing > 0) Core.BuyItem(GV, S, M, missing);
        }

        void Bundle(int size) =>
            Core.BuyItem(n, S, M, size, calculateRemaining: false);

        switch (n)
        {
            case "Might Tonic":
                if (!Core.Faction("Alchemy", 8)) return;
                Vouchers(2); Bundle(10);
                break;

            case "Sage Tonic":
                if (!Core.Faction("Alchemy", 8)) return;
                Vouchers(2); Bundle(10);
                break;

            case "Potent Malevolence Elixir":
                Vouchers(4); Bundle(8);
                break;

            case "Potent Battle Elixir":
                Vouchers(4); Bundle(8);
                break;

            case "Potent Honor Potion":
                if (!Core.Faction("Good", 10)) return;
                Vouchers(1); Bundle(5);
                break;

            default: return;
        }
    }

    public string GetBestTonicPotion()
    {
        var str = Core.GetStatValue("STR");
        var intel = Core.GetStatValue("INT");
        return str > intel ? "Might Tonic" : "Sage Tonic";
    }

    public string GetBestElixirPotion()
    {
        var str = Core.GetStatValue("STR");
        var intel = Core.GetStatValue("INT");
        return str > intel ? "Potent Battle Elixir" : "Potent Malevolence Elixir";
    }

    // --- next set ---------------------------------------------------------------

    #region Listeners

    private volatile bool _chargeDetected;
    private int _chargeSeq;

    public async void GenericChargeListener(dynamic packet)
    {
        try
        {
            if (packet?["params"]?.type?.ToString() != "json") return;
            dynamic data = packet["params"].dataObj;
            if (data?.cmd?.ToString() != "ct") return;

            var anims = data?.anims as System.Collections.IEnumerable;
            if (anims == null) return;

            foreach (var anim in anims)
            {
                if ((anim as dynamic)?.animStr?.ToString()?.Equals("Charge", StringComparison.OrdinalIgnoreCase) == true)
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

    public async void UltraDageListener(dynamic packet)
    {
        if (packet?["params"]?.type?.ToString() != "json") return;

        dynamic data = packet["params"].dataObj;
        if (data?.cmd?.ToString() != "event") return;

        string zone = data?.args?.zoneSet?.ToString();

        if (string.Equals(zone, "A", System.StringComparison.OrdinalIgnoreCase))
        { Bot.Send.Packet($"%xt%zm%mv%{Bot.Map.RoomID}%122%411%8%"); return; }

        if (string.Equals(zone, "B", System.StringComparison.OrdinalIgnoreCase))
        { Bot.Send.Packet($"%xt%zm%mv%{Bot.Map.RoomID}%856%422%8%"); return; }

        if (string.IsNullOrEmpty(zone))
        { Bot.Send.Packet($"%xt%zm%mv%{Bot.Map.RoomID}%491%421%8%"); return; }
    }

    #endregion
}
