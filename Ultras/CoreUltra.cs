/*
name: null
description: null
tags: null
*/

//cs_include Scripts/Ultras/CoreEngine.cs
using System.Diagnostics;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class CoreUltra
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine Core = new();

    public void Test()
        => Bot.Log("NewCore interface OK!");

    public void Taunt(string className, string target, string mode, int delayMs = 0, string? aura = null)
    {
        if (string.IsNullOrWhiteSpace(className) ||
            string.IsNullOrWhiteSpace(target))
            return;

        if (Bot?.Combat == null || !Core.HasClassEquipped(className))
            return;

        Bot.Combat.Attack(target);
        if (delayMs > 0) Bot.Sleep(delayMs);

        switch (mode)
        {
            case "aura":
                if (!string.IsNullOrWhiteSpace(aura) && Core.GetAuraSecondsRemaining(aura) < 1)
                    UseTaunt();
                break;

            case "charge":
                if ((_chargeDetected) && !Core.HasAura("Focus"))
                    UseTaunt();
                break;
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

        const string mob = "Ultra Warden";
        Bot.Combat.Attack(mob);

        var t = Bot.Player.Target;
        if (t == null) return;
        if (t.HP <= 0) return;
        if (t.MaxHP <= 0) return;

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

            bool go = true;
            while (go)
            {
                bool alive = MonsterAlive(mob);
                bool focused = Core.HasAura("Focus");
                bool exit = Bot.ShouldExit;

                if (!alive || focused || exit) go = false;
                else Core.UsePotion();
            }
        }

        Bot.Sleep(150);
    }

    public void DrakathTaunter()
    {
        if (Bot == null || Bot.Combat == null || Bot.Player == null) return;
        Bot.Combat.Attack("Champion Drakath");
        var dummy = Bot.Player.Target;
        if (dummy == null || dummy.HP <= 0) return;

        int[] bands = { 90, 80, 70, 60, 50, 40, 30, 20, 10 };
        double wiggle = 1.5;
        int lastBand = int.MaxValue;
        double oldPct = 100.0;
        long oldTicks = 0;

        object? tmp = AppDomain.CurrentDomain.GetData("drakath.lastThreshold");
        if (tmp != null) lastBand = (int)tmp;

        tmp = AppDomain.CurrentDomain.GetData("drakath.prevPercentage");
        if (tmp != null) oldPct = (double)tmp;

        tmp = AppDomain.CurrentDomain.GetData("drakath.lastFireTicks");
        if (tmp != null) oldTicks = (long)tmp;

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

            while (DateTime.UtcNow < giveUp && !Bot.ShouldExit)
            {
                Bot.Combat.Attack("Champion Drakath");
                UseTaunt();
                if (Core.HasAura("Focus")) break;
                Bot.Sleep(120);
            }
        }

        AppDomain.CurrentDomain.SetData("drakath.prevPercentage", nowPct);
        Bot.Sleep(120);
    }

    public void WaitForArmy(int quantity, string syncFilePath = "army_sync.sync", int bufferTimeMs = 3000, int tickMs = 500, int timeoutMs = 0)
    {
        if (Bot?.Map == null) return;

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
                string baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Skua");
                string full = Path.Combine(baseDir, Path.GetFileName(path));
                Directory.CreateDirectory(baseDir);
                File.AppendAllText(full, ""); // ensure file exists
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
                    using FileStream fs = new(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                    using StreamReader sr = new(fs);
                    return sr.ReadToEnd()
                             .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                }
                catch (IOException) { Bot?.Sleep(50); }
                catch { break; }
            }
            return Array.Empty<string>();
        }

        void Yeet(string path, string[] lines)
        {
            for (int i = 0; i < 15; i++)
            {
                try { File.WriteAllLines(path, lines); return; }
                catch (IOException) { Bot?.Sleep(50); }
                catch { return; }
            }
        }

        // Each line: key:ready:timestamp
        void Poke(string path, string key, bool ready)
        {
            List<string> lines = Slurp(path).ToList();
            string entry = $"{key}:{(ready ? "1" : "0")}:{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
            int idx = lines.FindIndex(l => l.StartsWith(key + ":"));
            if (idx >= 0) lines[idx] = entry;
            else lines.Add(entry);
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
                if (parts.Length < 3) continue;

                string key = parts[0];
                string status = parts[1];
                if (!long.TryParse(parts[2], out long ts)) continue;

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

            if (!File.Exists(syncFile) ||
                (DateTime.UtcNow - File.GetLastWriteTimeUtc(syncFile)).TotalMinutes > 15 ||
                Slurp(syncFile).All(l => l.EndsWith(":1")))
                File.WriteAllText(syncFile, "");
        }
        catch (Exception ex)
        {
            Bot?.Log($"[WaitForArmy] Sync file setup failed: {ex.Message}");
        }

        string me = $"{Bot?.Player?.Username ?? "Nobody"}|{Bot?.Player?.CurrentClass?.Name ?? "Peasant"}".Replace(":", "-");
        int need = Math.Max(1, quantity) + 1;

        Poke(syncFile, me, false);

        Stopwatch clock = Stopwatch.StartNew();
        int lastReady = -1;

        // --- Wait for army readiness ---
        while (!Bot.ShouldExit)
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

            Bot.Sleep(tickMs);
        }

        if (Bot.ShouldExit)
        {
            try { File.WriteAllText(syncFile, ""); } catch { }
            return;
        }

        // --- Warmup spam to keep clients responsive ---
        DateTime spam = DateTime.UtcNow.AddMilliseconds(2000);
        while (DateTime.UtcNow < spam && !Bot.ShouldExit)
        {
            Bot.Skills.UseSkill(3); Bot.Sleep(300);
            Bot.Skills.UseSkill(2); Bot.Sleep(300);
            Bot.Skills.UseSkill(1); Bot.Sleep(300);
        }

        Bot.Sleep(bufferTimeMs);

        try { File.WriteAllText(syncFile, ""); } catch { }
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

    public void UseTaunt()
    {
        Core.DisableSkills();

        while (!Core.HasAura("Focus"))
        {
            Bot.Skills.UseSkill(5);
            Bot.Sleep(100);
        }

        Core.EnableSkills();
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

    #region Alchemy

    public void UseAlchemyPotions(params string[] names)
    {
        if (names == null || names.Length == 0) return;

        string Aura(string x) => x switch
        {
            "Might Tonic" => "Might",
            "Sage Tonic" => "Sage",
            _ => x
        };

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var raw in names)
        {
            if (string.IsNullOrWhiteSpace(raw)) continue;
            if (!seen.Add(raw)) continue;

            var aura = Aura(raw);
            if (Core.HasAura(aura, true))
            {
                Core.Log("POTION", $"🟢 Already on: {aura}");
                continue;
            }

            Core.Log("POTION", $"🧪 Queueing: {raw} ({aura})");
            BuyAlchemyPotion(raw);

            for (int tries = 0; tries < 3 && !Core.HasAura(aura, true) && !Bot.ShouldExit; tries++)
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

            if (Core.HasAura(aura, true)) Core.Log("POTION", $"✅ Applied: {aura}");
            else Core.Log("POTION", $"❌ Nope: {raw} ({aura})");
        }
    }

    public void BuyAlchemyPotion(string n)
    {
        if (string.IsNullOrWhiteSpace(n) || Core.Owned(n) >= 1)
        {
            if (!string.IsNullOrWhiteSpace(n)) Core.Log("POTION", $"🧴 Have: {n}");
            return;
        }

        int shop = 2036;
        string map = "alchemyacademy";
        string voucher = "Gold Voucher 500k";

        void NeedV(int want)
        {
            int miss = Math.Max(0, want - Core.Owned(voucher));
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
                if (!Core.Faction("Alchemy", 8)) { Core.Log("POTION", "⛔ Alchemy rep 8 required"); return; }
                NeedV(2); Grab(10);
                break;

            case "Sage Tonic":
                if (!Core.Faction("Alchemy", 8)) { Core.Log("POTION", "⛔ Alchemy rep 8 required"); return; }
                NeedV(2); Grab(10);
                break;

            case "Potent Malevolence Elixir":
                NeedV(4); Grab(8);
                break;

            case "Potent Battle Elixir":
                NeedV(4); Grab(8);
                break;

            case "Potent Honor Potion":
                if (!Core.Faction("Good", 10)) { Core.Log("POTION", "⛔ Good rep 10 required"); return; }
                NeedV(1); Grab(5);
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

    #endregion
}