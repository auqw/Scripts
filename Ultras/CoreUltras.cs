using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Skua.Core.Interfaces;
using Skua.Core.Models.Auras;
using Skua.Core.Models.Players;
using Skua.Core.Models.Factions;
using Skua.Core.Models.Quests;
using Skua.Core.Models.Items;
using Skua.Core.Models.Shops;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Skills;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class CoreUltras
{
    public IScriptInterface Bot => IScriptInterface.Instance;

    readonly ConcurrentDictionary<string, object> _cache = new();
    readonly ConcurrentDictionary<string, DateTime> _throttle = new();

    CancellationTokenSource _cts;
    Task _runSkills;

    public TimeSpan ThrottleDuration { get; set; } = TimeSpan.FromSeconds(3);
    public event Action<string, string> OnSignal;

    #region Settings

    int D1 = 250;
    int D2 = 700;
    int D3 = 1400;
    int D4 = 2800;

    public void Boot()
    {
        if (_runSkills?.Status == TaskStatus.Running)
            return;

        OnSignal += (category, message) => { Bot.Log($"[{category}] {message}"); };

        Bot.Events.ScriptStopping += OnScriptStopping;
        Bot.UltraBossHelper.DisableCounterAttack();

        _cts = new CancellationTokenSource();
        _runSkills = Task.Run(() => SkillsAsync(_cts.Token));

        StopAttack();

        Bot.Bank.Open();

        if (Bot.Bank.Items == null || Bot.Bank.Items.Count == 0)
        {
            Bot.Bank.Load();
            Bot.Wait.ForTrue(() => Bot.Bank.Items.Count > 0, 20);
        }

        Bot.Options.SafeTimings = true;
        Bot.Options.InfiniteRange = true;
        Bot.Options.SkipCutscenes = true;
        Bot.Lite.HidePlayers = true;

        Log("CORE", "System online");
    }

    bool OnScriptStopping(Exception e)
    {
        Log("CORE", "System offline");

        Bot.Lite.HidePlayers = false;
        Bot.Events.ExtensionPacketReceived -= ChargeListener;

        _cts?.Cancel();
        _runSkills?.Wait(TimeSpan.FromSeconds(2));

        OnSignal = null;

        _cache.Clear();
        _throttle.Clear();

        _cts?.Dispose();
        _runSkills?.Dispose();

        return true;
    }

    #endregion

    #region Items

    public void ForItem(string monsters, string? map, object key, int quantity = 1, bool isTemp = false, bool useBestGear = false, bool alt = false, string? cell = null, string pad = "Left", bool priority = false)
    {
        if (key is null || quantity <= 0) return;
        if (!string.IsNullOrWhiteSpace(map))
            Join(map);
        var targets = (monsters ?? string.Empty)
            .Replace('|', ',')
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .Where(t => t.Length > 0)
            .ToArray();
        ChooseBestCell(monsters, alt, cell, pad);
        if (useBestGear) ChooseBestGear(monsters);
        MonsterKey[] prioKeys = Array.Empty<MonsterKey>();
        if (priority && targets.Length > 0)
            prioKeys = targets.Select(MonsterKey.FromName).ToArray();

        MonsterKey[] targetKeys = targets.Select(MonsterKey.FromName).ToArray();

        Func<int> qty = key switch
        {
            int id => () => Owned(id, isTemp),
            string s => () => Owned(s, isTemp),
            _ => () => 0
        };
        Action pullFromBank = () =>
        {
            if (isTemp) return;
            if (key is int id) InBank(id);
            else if (key is string s) InBank(s);
        };
        Action pickupKey = () =>
        {
            if (key is int id) Pickup(id);
            else if (key is string s) Pickup(s);
        };
        pullFromBank();
        if (qty() >= quantity) return;
        string keyLabel = key is int id2 ? (GetDropItem(id2)?.Name ?? $"Item#{id2}") : key.ToString() ?? "Item";
        Log("FARMING", $"Killing {monsters} for {quantity}x {keyLabel}");
        if (targets.Length == 0)
        {
            Log("FARMING", "No targets specified; aborting.");
            DisableSkills();
            StopAttack();
            return;
        }
        EnableSkills();
        int i = 0;
        while (!Bot.ShouldExit)
        {
            if (qty() >= quantity)
            {
                Log("SUCCESS", $"Acquired {quantity}x {keyLabel}");
                DisableSkills();
                StopAttack();
                return;
            }
            pickupKey();
            if (priority)
            {
                KillWithPriority(prioKeys);
            }
            else
            {
                var idx = i++ % targetKeys.Length;
                Kill(targetKeys[idx]);
            }
        }
        DisableSkills();
        StopAttack();
    }

    void EquipBestClassCore<T>(IEnumerable<(T key, int rank)> prefs, Func<T, bool> owned, Func<T, bool> equipped, Action<T> equip)
    {
        if (prefs == null) return;
        bool maxed = IsCurrentClassMaxRank();

        foreach (var (k, r) in prefs)
        {
            try
            {
                if (!owned(k)) continue;
                if (equipped(k) && (maxed || Bot.Player.CurrentClassRank >= r)) return;

                equip(k);
                Bot.Sleep(D3);
                return;
            }
            catch { }
        }
    }

    public void EquipBestClass(List<(string name, int rank)> priorities) =>
        EquipBestClassCore(priorities,
            owned: n => !string.IsNullOrWhiteSpace(n) && Owned(n, 1),
            equipped: n => HasClassEquipped(n),
            equip: n => { if (!Bot.Inventory.IsEquipped(n)) Bot.Inventory.EquipItem(n); });

    public void EquipBestClass(List<(int id, int rank)> priorities) =>
        EquipBestClassCore(priorities,
            owned: id => Bot.Inventory.Items.Any(i => i?.ID == id),
            equipped: id =>
            {
                var it = Bot.Inventory.Items.FirstOrDefault(i => i?.ID == id);
                return it != null && HasClassEquipped(it.Name);
            },
            equip: id => { if (!Bot.Inventory.IsEquipped(id)) Bot.Inventory.EquipItem(id); });

    public bool InBank(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || !Bot.Bank.Contains(name)) return false;
        try { StopAttack(); Bot.Bank.ToInventory(name); Bot.Sleep(D2); return true; } catch { return false; }
    }

    public bool InBank(int id)
    {
        if (id <= 0 || !Bot.Bank.Contains(id)) return false;
        try { StopAttack(); Bot.Bank.ToInventory(id); Bot.Sleep(D2); return true; } catch { return false; }
    }

    public bool ToBank(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || !Bot.Inventory.Contains(name)) return false;
        try { StopAttack(); Bot.Inventory.ToBank(name); Bot.Sleep(D2); return true; } catch { return false; }
    }

    public bool ToBank(int id)
    {
        if (id <= 0 || !Bot.Inventory.Contains(id)) return false;
        try { StopAttack(); Bot.Inventory.ToBank(id); Bot.Sleep(D2); return true; } catch { return false; }
    }

    public int Owned(string name, bool isTemp = false)
    {
        try { return string.IsNullOrWhiteSpace(name) ? 0 : (isTemp ? Bot.TempInv?.GetQuantity(name) ?? 0 : Bot.Inventory?.GetQuantity(name) ?? 0); }
        catch { return 0; }
    }

    public int Owned(int id, bool isTemp = false)
    {
        try { return id <= 0 ? 0 : (isTemp ? Bot.TempInv?.GetQuantity(id) ?? 0 : Bot.Inventory?.GetQuantity(id) ?? 0); }
        catch { return 0; }
    }

    public bool Owned(string name, int quantity, bool isTemp = false) => Owned(name, isTemp) >= quantity;
    public bool Owned(int id, int quantity, bool isTemp = false) => Owned(id, isTemp) >= quantity;

    public void EquipRandomClassAndReequip(int holdMs = 1000)
    {
        if (Bot?.Inventory == null || Bot.Player == null) return;

        bool IsClass(InventoryItem it)
        {
            if (it == null) return false;

            try
            {
                if (it is ItemBase ib)
                {
                    if (ib.Category == Skua.Core.Models.Items.ItemCategory.Class) return true;
                    if (!string.IsNullOrWhiteSpace(ib.CategoryString) &&
                        ib.CategoryString.Equals("Class", StringComparison.OrdinalIgnoreCase)) return true;
                }
            }
            catch { }

            var catProp = it.GetType().GetProperty("CategoryString");
            if (catProp != null)
            {
                var cs = catProp.GetValue(it) as string;
                if (!string.IsNullOrWhiteSpace(cs) &&
                    cs.Equals("Class", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        var inv = Bot.Inventory.Items;
        if (inv == null) return;

        int curId = -1;
        string curName = null;
        foreach (var it in inv)
        {
            if (it == null) continue;
            if (it.Equipped == true && IsClass(it))
            {
                curId = it.ID;
                curName = it.Name;
                break;
            }
        }
        if (curId <= 0 && string.IsNullOrWhiteSpace(curName)) return;

        var candidates = new List<InventoryItem>();
        foreach (var it in inv)
        {
            if (it == null) continue;
            if (!IsClass(it)) continue;
            if (it.Equipped == true) continue;
            candidates.Add(it);
        }
        if (candidates.Count == 0) return;

        var rng = new Random(unchecked((int)Environment.TickCount));
        var rnd = candidates[rng.Next(0, candidates.Count)];

        if (!Bot.Inventory.IsEquipped(rnd.ID))
        {
            for (int t = 0; t < 3 && !Bot.ShouldExit; t++)
            {
                Bot.Inventory.EquipItem(rnd.ID);
                Bot.Sleep(500);
                if (Bot.Inventory.IsEquipped(rnd.ID)) break;
            }
        }
        if (!Bot.Inventory.IsEquipped(rnd.ID)) return;

        if (holdMs > 0) Bot.Sleep(holdMs);

        if (curId > 0)
        {
            for (int t = 0; t < 3 && !Bot.ShouldExit; t++)
            {
                if (Bot.Inventory.IsEquipped(curId)) break;
                Bot.Inventory.EquipItem(curId);
                Bot.Sleep(500);
            }
            if (Bot.Inventory.IsEquipped(curId)) return;
        }

        if (!string.IsNullOrWhiteSpace(curName))
        {
            for (int t = 0; t < 3 && !Bot.ShouldExit; t++)
            {
                if (Bot.Inventory.IsEquipped(curName)) break;
                Bot.Inventory.EquipItem(curName);
                Bot.Sleep(500);
            }
        }
    }

    #endregion

    #region Best Enhancement

    public InventoryItem ChooseBestEnhancement(string itemGroup, params string[] priority)
    {
        if (priority == null || priority.Length == 0) return null;
        if (Bot?.Inventory == null || Bot.Bank == null || Bot.Player == null) return null;

        string Norm(string g)
        {
            if (string.IsNullOrWhiteSpace(g)) return g;
            switch (g.Trim().ToLowerInvariant())
            {
                case "weapon": return "Weapon";
                case "helm":
                case "he": return "he";
                case "back":
                case "ba":
                case "cape": return "ba";
                case "class":
                case "co": return "co";
                case "pet":
                case "pe": return "pe";
                default: return g;
            }
        }

        int N(int? v, int d = -1) => v ?? d;

        string Enh(int id)
        {
            switch (id)
            {
                case 1: return "Adventurer";
                case 2: return "Fighter";
                case 3: return "Thief";
                case 4: return "Armsman";
                case 5: return "Hybrid";
                case 6: return "Wizard";
                case 7: return "Healer";
                case 8: return "Spellbreaker";
                case 9: return "Lucky";
                case 10: return "Forge";
                case 11: return "Absolution";
                case 12: return "Avarice";
                case 23: return "Depths";
                case 24: return "Vainglory";
                case 25: return "Vim";
                case 26: return "Examen";
                case 27: return "Pneuma";
                case 28: return "Anima";
                case 29: return "Penitence";
                case 30: return "Lament";
                case 32: return "Hearty";
                default: return null;
            }
        }

        string WeaponTrait(int id)
        {
            switch (id)
            {
                case 2: return "Spiral Carve";
                case 3: return "Awe Blast";
                case 4: return "Health Vamp";
                case 5: return "Mana Vamp";
                case 6: return "Powerword Die";
                case 7: return "Lacerate";
                case 8: return "Smite";
                case 9: return "Valiance";
                case 10: return "Arcana's Concerto";
                case 11: return "Acheron";
                case 12: return "Elysium";
                case 13: return "Praxis";
                case 14: return "Dauntless";
                case 15: return "Ravenous";
                default: return null;
            }
        }

        bool GroupMatch(InventoryItem i, string grp)
        {
            var g = i?.ItemGroup;
            return !string.IsNullOrWhiteSpace(g) &&
                   grp != null &&
                   g.Equals(grp, StringComparison.OrdinalIgnoreCase);
        }

        bool MatchWant(InventoryItem i, string want, string grp)
        {
            if (i == null || string.IsNullOrWhiteSpace(want)) return false;

            var pat = Enh(N(i.EnhancementPatternID));
            if (!string.IsNullOrEmpty(pat) &&
                pat.Equals(want, StringComparison.OrdinalIgnoreCase))
                return true;

            if (grp.Equals("Weapon", StringComparison.OrdinalIgnoreCase))
            {
                var tr = WeaponTrait(N(i.ProcID));
                if (!string.IsNullOrEmpty(tr) &&
                    tr.Equals(want, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        InventoryItem FindIn(IEnumerable<InventoryItem> src, string want, string grp, bool memOnlyIfUpgradeAllowed)
        {
            if (src == null) return null;
            foreach (var i in src)
            {
                if (i == null) continue;
                if (!GroupMatch(i, grp)) continue;
                if (i.Upgrade && !memOnlyIfUpgradeAllowed) continue;
                if (MatchWant(i, want, grp)) return i;
            }
            return null;
        }

        bool Equip(InventoryItem it)
        {
            if (it == null) return false;
            if (Bot.Inventory.IsEquipped(it.ID)) return true;
            for (int t = 0; t < 3 && !Bot.ShouldExit; t++)
            {
                Bot.Inventory.EquipItem(it.ID);
                Bot.Sleep(500);
                if (Bot.Inventory.IsEquipped(it.ID)) return true;
            }
            return false;
        }

        var grp = Norm(itemGroup);
        bool mem = Bot.Player.IsMember == true;

        var wants = new List<string>();
        if (priority != null)
            foreach (var p in priority)
                if (!string.IsNullOrWhiteSpace(p)) wants.Add(p);

        foreach (var want in wants)
        {
            var inv = Bot.Inventory.Items;
            var hit = FindIn(inv, want, grp, mem);
            if (Equip(hit)) return hit;

            var bank = Bot.Bank.Items;
            var fromBank = FindIn(bank, want, grp, mem);
            if (fromBank != null)
            {
                InBank(fromBank.Name);
                Bot.Sleep(500);

                var inv2 = Bot.Inventory.Items;
                InventoryItem pulled = null;
                if (inv2 != null)
                {
                    foreach (var i in inv2)
                        if (i != null && i.ID == fromBank.ID) { pulled = i; break; }
                }
                if (pulled == null)
                {
                    pulled = FindIn(inv2, want, grp, mem);
                }

                if (Equip(pulled))
                {
                    var inv3 = Bot.Inventory.Items;
                    if (inv3 != null)
                    {
                        foreach (var i in inv3)
                            if (i != null && Bot.Inventory.IsEquipped(i.ID))
                                return i;
                    }
                    return pulled;
                }
            }

            Bot.Sleep(500);
        }

        Log("Enhancement", $"No {grp} matched: {string.Join(", ", wants)}");
        return null;
    }

    #endregion

    #region Combat

    public record MonsterKey(int? MapId = null, string? Name = null, int? Id = null)
    {
        public static MonsterKey FromName(string name) => new(Name: name);
        public static MonsterKey FromId(int id) => new(Id: id);
        public static MonsterKey FromMapId(int mapId) => new(MapId: mapId);
    }

    IEnumerable<Monster> Match(MonsterKey k)
    {
        var list = Bot?.Monsters?.MapMonsters;
        if (list == null) yield break;

        foreach (var m in list)
        {
            if (m == null) continue;
            if (k.MapId.HasValue && m.MapID != k.MapId.Value) continue;
            if (!string.IsNullOrWhiteSpace(k.Name) &&
                !string.Equals(m.Name, k.Name, StringComparison.OrdinalIgnoreCase)) continue;
            if (k.Id.HasValue && m.ID != k.Id.Value) continue;
            yield return m;
        }
    }

    public bool IsAlive(MonsterKey k)
    {
        foreach (var m in Match(k))
            if (m.Alive) return true;
        return false;
    }

    public bool IsAliveByMapId(int? mapId = null, string? name = null, int? id = null) =>
        IsAlive(new MonsterKey(mapId, name, id));

    void Attack(MonsterKey k)
    {
        if (k.MapId.HasValue) Bot.Combat.Attack(k.MapId.Value);
        else if (k.Id.HasValue) Bot.Combat.Attack(k.Id.Value);
        else if (!string.IsNullOrWhiteSpace(k.Name)) Bot.Combat.Attack(k.Name);
    }

    public void Kill(MonsterKey k)
    {
        var target = Match(k)
            .Where(m => m.Alive)
            .OrderBy(m => m.HP)
            .ThenBy(m => m.MapID)
            .FirstOrDefault();

        if (target == null) return;

        var hpKey = MonsterKey.FromMapId(target.MapID);
        Attack(hpKey);
        Bot.Sleep(D1);
    }

    Monster? LowestHpTarget(params MonsterKey[] keys)
    {
        Monster? best = null;
        foreach (var k in keys)
        {
            foreach (var m in Match(k))
            {
                if (!m.Alive) continue;
                if (best == null || m.HP < best.HP)
                    best = m;
            }
        }
        return best;
    }

    public void KillWithPriority(params MonsterKey[] keys)
    {
        if (keys == null || keys.Length == 0) { Bot.Sleep(D1); return; }

        var target = LowestHpTarget(keys);
        if (target == null) { Bot.Sleep(D1); return; }

        var targetKey = MonsterKey.FromMapId(target.MapID);
        Bot.Combat.Attack(target.MapID);
        Bot.Sleep(D1);
    }

    public void KillUntilDead(MonsterKey k)
    {
        var target = LowestHpTarget(k);
        if (target == null) return;

        var targetKey = MonsterKey.FromMapId(target.MapID);

        while (!Bot.ShouldExit)
        {
            bool alive = false;
            foreach (var m in Match(targetKey))
            {
                if (m.MapID == target.MapID && m.Alive) { alive = true; break; }
            }
            if (!alive) break;

            Bot.Combat.Attack(target.MapID);
            Bot.Sleep(D1);
        }
    }

    public void AttackFor(MonsterKey boss, int ms)
    {
        long end = Environment.TickCount64 + ms;
        while (!Bot.ShouldExit && Environment.TickCount64 < end)
        {
            var target = LowestHpTarget(boss);
            if (target == null) break;

            Bot.Combat.Attack(target.MapID);
            Bot.Skills.UseSkill(5);
            Bot.Sleep(D1);
        }
    }

    public void Kill(string name) => Kill(MonsterKey.FromName(name));
    public void Kill(params string[] names)
    {
        if (names == null || names.Length == 0) return;
        var tmp = new List<MonsterKey>(names.Length);
        foreach (var n in names)
            if (!string.IsNullOrWhiteSpace(n)) tmp.Add(MonsterKey.FromName(n));
        if (tmp.Count == 0) return;
        KillWithPriority(tmp.ToArray());
    }
    public void Kill(int id) => Kill(MonsterKey.FromId(id));
    public void KillAtMapId(int mapId) => Kill(MonsterKey.FromMapId(mapId));

    public void KillWithPriority(string primaryName, string priorityName1)
        => KillWithPriority(MonsterKey.FromName(priorityName1), MonsterKey.FromName(primaryName));
    public void KillWithPriority(int primaryId, int priorityId1)
        => KillWithPriority(MonsterKey.FromId(priorityId1), MonsterKey.FromId(primaryId));
    public void KillWithPriorityAtMapId(int primaryMapId, int priorityMapId1)
        => KillWithPriority(MonsterKey.FromMapId(priorityMapId1), MonsterKey.FromMapId(primaryMapId));

    public void KillWithPriority(string primaryName, string priorityName1, string priorityName2)
        => KillWithPriority(MonsterKey.FromName(priorityName1), MonsterKey.FromName(priorityName2), MonsterKey.FromName(primaryName));
    public void KillWithPriority(int primaryId, int priorityId1, int priorityId2)
        => KillWithPriority(MonsterKey.FromId(priorityId1), MonsterKey.FromId(priorityId2), MonsterKey.FromId(primaryId));
    public void KillWithPriorityAtMapId(int primaryMapId, int priorityMapId1, int priorityMapId2)
        => KillWithPriority(MonsterKey.FromMapId(priorityMapId1), MonsterKey.FromMapId(priorityMapId2), MonsterKey.FromMapId(primaryMapId));

    // --- helpers -----------------------------------------------------------------

    readonly HashSet<string> _preparedMonsters = new(StringComparer.OrdinalIgnoreCase);
    bool _harpyHooked = false;

    void MonsterSetup(string monsterName)
    {
        if (string.IsNullOrWhiteSpace(monsterName)) return;
        string m = monsterName.ToLowerInvariant();

        if (m.Contains("ultra chaos harpy") || m.Contains("chaos harpy"))
        {
            if (!_harpyHooked)
            {
                Bot.Events.ExtensionPacketReceived += ChaosHarpyListener;
                _harpyHooked = true;
            }
            const string Pot = "Shriekward Potion";
            if (Owned(Pot) < 1) BuyItem(Pot, 774, "mirrorportal", 30); // itemKey, shopId, map, qty
            EquipConsumable(Pot);
        }
        else if (m.Contains("ultra xiang") || m.Contains("chaos lord xiang"))
        {
            var classes = new List<(string name, int rank)>
        {
            ("Dragon of Time", 10),
            ("Healer (Rare)", 1),
            ("Healer", 1)
        };
            EquipBestClass(classes);
        }
        else if (m.Contains("doomkitten"))
        {
            var classes = new List<(string name, int rank)>
        {
            ("Dragon of Time", 10),
            ("Legion Revenant", 10),
            ("Blaze Binder", 10)
        };
            EquipBestClass(classes);
        }
    }

    string? ResolveMonsterName(MonsterKey k)
    {
        if (!string.IsNullOrWhiteSpace(k.Name)) return k.Name;

        var list = Bot?.Monsters?.MapMonsters;
        if (list == null) return null;

        foreach (var m in list)
        {
            if (m == null) continue;
            if (k.Id.HasValue && m.ID == k.Id.Value) return m.Name;
            if (k.MapId.HasValue && m.MapID == k.MapId.Value) return m.Name;
        }
        return null;
    }

    void EnsureMonsterSetup(MonsterKey k)
    {
        var name = ResolveMonsterName(k);
        if (string.IsNullOrWhiteSpace(name)) return;
        if (_preparedMonsters.Add(name)) MonsterSetup(name);
    }

    void ResetMonsterSetupCache() => _preparedMonsters.Clear();

    #endregion

    #region Factions

    int Rank(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return 0;
        var list = Bot?.Reputation?.FactionList;
        if (list == null) return 0;

        foreach (var f in list)
        {
            var n = f?.Name;
            if (!string.IsNullOrWhiteSpace(n) &&
                string.Equals(n, name, StringComparison.OrdinalIgnoreCase))
                return f?.Rank ?? 0;
        }
        return 0;
    }

    bool Faction(string name, int minRank = 0)
    {
        var need = minRank < 0 ? 0 : minRank;
        return Rank(name) >= need;
    }

    #endregion

    #region Potions & Scrolls

    public void UsePotion()
    {
        DisableSkills();
        try
        {
            Bot.Sleep(D2);
            Bot.Skills.UseSkill(5);
            Bot.Sleep(D2);
        }
        finally { EnableSkills(); }
    }

    public void GetScrollOfEnrage()
    {
        if (!Faction("SpellCrafting", 5)) return;

        const string parchment = "Mystic Parchment";
        const string ink = "Zealous Ink";
        const string scroll = "Scroll of Enrage";

        if (Owned(scroll) < 10)
        {
            // Mats
            ForItem("Undead Infantry", "underworld", parchment, 2);
            BuyItem(ink, 549, "dragonrune", 5, calculateRemaining: false);

            // Craft
            Join("spellcraft");
            Bot.Drops.Add(scroll);
            Bot.Send.Packet("%xt%zm%crafting%1%spellOnStart%7%1555%Spell%"); Bot.Sleep(5000);
            Bot.Send.Packet("%xt%zm%crafting%1%spellComplete%7%2330%Enrage%");

            WaitForDrop(scroll, 10000);
            Pickup(scroll);
        }

        EquipConsumable(scroll);
        EquipRandomClassAndReequip();
    }

    public void GetScrollOfDecay()
    {
        if (!Faction("SpellCrafting", 5)) return;

        const string parchment = "Mystic Parchment";
        const string ink = "Zealous Ink";
        const string scroll = "Scroll of Decay";

        while (Owned(scroll) < 10 && !Bot.ShouldExit)
        {
            ForItem("Undead Infantry", "underworld", parchment, 2);
            BuyItem(ink, 549, "dragonrune", 5, calculateRemaining: false);

            Join("spellcraft");
            Bot.Drops.Add(scroll);
            Bot.Send.Packet("%xt%zm%crafting%1%spellOnStart%7%1555%Spell%"); Bot.Sleep(5000);
            Bot.Send.Packet("%xt%zm%crafting%1%spellComplete%7%2331%Decay%");

            WaitForDrop(scroll, 5000);
            Pickup(scroll);
        }

        EquipConsumable(scroll);
    }

    public void GetDivineElixir()
    {
        ForItem("Xavier Lionfang", "poisonforest", "Divine Elixir");
        EquipConsumable("Divine Elixir");
        UsePotion();
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
            if (HasAura(aura, true)) continue;

            BuyAlchemyPotion(raw);

            for (int t = 0; t < 3 && !HasAura(aura, true) && !Bot.ShouldExit; t++)
            {
                EquipConsumable(raw);
                if (Bot.Inventory.IsEquipped(raw))
                {
                    UsePotion();
                    long t0 = Environment.TickCount64;
                    while (!Bot.ShouldExit && !HasAura(aura, true) && Environment.TickCount64 - t0 < 1500)
                        Bot.Sleep(50);
                }
                else Bot.Sleep(200);
            }
        }
    }

    public void BuyAlchemyPotion(string n)
    {
        if (string.IsNullOrWhiteSpace(n) || Owned(n) >= 1) return;

        int S = 2036;
        string M = "alchemyacademy";
        string GV = "Gold Voucher 500k";

        void Vouchers(int need)
        {
            int missing = Math.Max(0, need - Owned(GV));
            if (missing > 0) BuyItem(GV, S, M, missing);
        }

        void Bundle(int size) =>
            BuyItem(n, S, M, size, calculateRemaining: false);

        switch (n)
        {
            case "Might Tonic":
                if (!Faction("Alchemy", 8)) return;
                Vouchers(2); Bundle(10);
                break;

            case "Sage Tonic":
                if (!Faction("Alchemy", 8)) return;
                Vouchers(2); Bundle(10);
                break;

            case "Potent Malevolence Elixir":
                Vouchers(4); Bundle(8);
                break;

            case "Potent Battle Elixir":
                Vouchers(4); Bundle(8);
                break;

            case "Potent Honor Potion":
                if (!Faction("Good", 10)) return;
                Vouchers(1); Bundle(5);
                break;

            default: return;
        }
    }

    public string GetBestTonicPotion()
    {
        var str = GetStatValue("STR");
        var intel = GetStatValue("INT");
        return str > intel ? "Might Tonic" : "Sage Tonic";
    }

    public string GetBestElixirPotion()
    {
        var str = GetStatValue("STR");
        var intel = GetStatValue("INT");
        return str > intel ? "Potent Battle Elixir" : "Potent Malevolence Elixir";
    }

    public void EquipConsumable(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;

        DisableSkills();
        StopAttack();
        try
        {
            if (Owned(name) < 1) return;
            if (Bot.Inventory.IsEquipped(name)) return;

            WhiteMap();
            Bot.Inventory.EquipUsableItem(name);
            Bot.Sleep(D3);
        }
        finally
        {
            EnableSkills();
        }
    }

    #endregion

    #region Best Gear

    public record Gear(string Name, string Group, bool FromBank, double All, double Race);

    public void ChooseBestGear(string names)
    {
        if (Bot?.Monsters?.MapMonsters == null || Bot?.Inventory?.Items == null || Bot?.Bank?.Items == null) return;

        bool IsSelectedMonster(string mName, HashSet<string> set)
        {
            if (string.IsNullOrWhiteSpace(mName)) return false;
            if (set == null || set.Count == 0) return true; // "*" or empty -> all
            return set.Contains(mName);
        }

        string NormalizeRace(string r)
        {
            if (string.IsNullOrWhiteSpace(r)) return "allDmg";
            if (r.Equals("None", StringComparison.OrdinalIgnoreCase)) return "allDmg";
            return r;
        }

        double Meta(string meta, string key)
        {
            if (string.IsNullOrWhiteSpace(meta)) return 0;
            var kAll = key.Equals("allDmg", StringComparison.OrdinalIgnoreCase);
            var span = meta.AsSpan();
            int i = 0, len = span.Length;
            while (i < len)
            {
                int j = i;
                while (j < len && span[j] != '\n' && span[j] != '\r' && span[j] != ',') j++;
                var token = span.Slice(i, j - i).ToString();
                i = j + 1;

                int colon = token.IndexOf(':');
                if (colon <= 0) continue;

                var k = token.Substring(0, colon).Trim();
                var vStr = token.Substring(colon + 1).Trim();

                if (!(k.Equals(key, StringComparison.OrdinalIgnoreCase) ||
                     (kAll && k.Equals("dmgAll", StringComparison.OrdinalIgnoreCase))))
                    continue;

                if (double.TryParse(vStr, System.Globalization.NumberStyles.Float,
                                    System.Globalization.CultureInfo.InvariantCulture, out var v))
                    return Math.Max(0, v - 1);
            }
            return 0;
        }

        HashSet<string> ParseNameSet(string s)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(s) || s.Trim() == "*") return set; // empty => "select all"
            int i = 0; var span = s.AsSpan();
            while (i < span.Length)
            {
                int j = i;
                while (j < span.Length && span[j] != ',') j++;
                var piece = span.Slice(i, j - i).ToString().Trim();
                if (piece.Length > 0) set.Add(piece);
                i = j + 1;
            }
            return set;
        }

        bool IsValidGroup(string g)
        {
            if (string.IsNullOrWhiteSpace(g)) return false;
            if (g.Equals("Weapon", StringComparison.OrdinalIgnoreCase)) return true;
            var gl = g.ToLowerInvariant();
            return gl == "he" || gl == "ba" || gl == "co" || gl == "pe";
        }

        bool Equipped(string name)
        {
            var items = Bot.Inventory.Items;
            if (items == null) return false;
            foreach (var it in items)
                if (it?.Equipped == true && it.Name != null &&
                    name.Equals(it.Name, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        void Equip(Gear g)
        {
            if (string.IsNullOrWhiteSpace(g.Name)) return;
            for (int t = 0; t < 3 && !Bot.ShouldExit; t++)
            {
                if (g.FromBank) InBank(g.Name);
                Bot.Inventory.EquipItem(g.Name);
                Bot.Sleep(500);
                if (Equipped(g.Name)) break;
            }
        }

        var selected = ParseNameSet(names);
        string race = "allDmg";
        {
            var mobs = Bot.Monsters.MapMonsters;
            var raceCount = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            if (mobs != null)
            {
                foreach (var m in mobs)
                {
                    var mn = m?.Name;
                    if (!IsSelectedMonster(mn, selected)) continue;
                    var r = NormalizeRace(m?.Race);
                    if (!raceCount.TryGetValue(r, out var c)) raceCount[r] = 1;
                    else raceCount[r] = c + 1;
                }
            }
            int maxRaceCount = 0;
            foreach (var kv in raceCount)
                if (kv.Value > maxRaceCount) { maxRaceCount = kv.Value; race = kv.Key; }
        }

        // scan inventory + bank
        var bank = Bot.Bank.Items ?? Enumerable.Empty<InventoryItem>();
        var inv = Bot.Inventory.Items ?? Enumerable.Empty<InventoryItem>();

        var bankNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var b in bank) if (!string.IsNullOrWhiteSpace(b?.Name)) bankNames.Add(b.Name);

        var bestAll = new Dictionary<string, Gear>(StringComparer.OrdinalIgnoreCase);
        var bestRace = new Dictionary<string, Gear>(StringComparer.OrdinalIgnoreCase);

        void Consider(InventoryItem it)
        {
            if (it == null || string.IsNullOrWhiteSpace(it.ItemGroup)) return;
            if (!IsValidGroup(it.ItemGroup)) return;
            if (it.Upgrade && !(Bot?.Player?.IsMember == true)) return;

            var name = it.Name ?? "";
            var grp = it.ItemGroup;
            var fromBank = bankNames.Contains(name);

            var all = Meta(it.Meta, "allDmg");
            var rac = Meta(it.Meta, race);

            if (all <= 0 && rac <= 0) return;

            var g = new Gear(name, grp, fromBank, all, rac);

            if (all > 0)
            {
                if (!bestAll.TryGetValue(grp, out var curA) || g.All > curA.All)
                    bestAll[grp] = g;
            }
            if (rac > 0)
            {
                if (!bestRace.TryGetValue(grp, out var curR) || g.Race > curR.Race)
                    bestRace[grp] = g;
            }
        }

        foreach (var it in inv) Consider(it);
        foreach (var it in bank) Consider(it);

        if (bestAll.Count == 0 && bestRace.Count == 0) return;

        // choose best combo
        Gear bestA = null, bestR = null;
        double bestSum = double.MinValue;

        foreach (var kvA in bestAll)
        {
            var ga = kvA.Value;
            foreach (var kvR in bestRace)
            {
                var gr = kvR.Value;
                if (ga.Group.Equals(gr.Group, StringComparison.OrdinalIgnoreCase)) continue;
                double sum = ga.All + gr.Race;
                if (sum > bestSum)
                {
                    bestSum = sum; bestA = ga; bestR = gr;
                }
            }
        }

        if (bestA != null && bestR != null)
        {
            Equip(bestA);
            Equip(bestR);
            return;
        }

        // single best item overall
        Gear bestItem = null;
        double bestScore = double.MinValue;

        foreach (var kv in bestAll)
        {
            var g = kv.Value;
            var s = g.All > g.Race ? g.All : g.Race;
            if (s > bestScore) { bestScore = s; bestItem = g; }
        }
        foreach (var kv in bestRace)
        {
            var g = kv.Value;
            var s = g.All > g.Race ? g.All : g.Race;
            if (s > bestScore) { bestScore = s; bestItem = g; }
        }

        if (bestItem != null) Equip(bestItem);
    }

    #endregion

    #region Shop

    public bool BuyItem(object itemKey, int shopId, string map, int quantity = 1, bool ensureMap = true, bool calculateRemaining = true, bool skipIfHaveEnough = true, bool considerBank = true, bool checkGold = true, bool checkLevel = true, bool checkInvSpace = true, int loadTimeoutMs = 5000)
    {
        if (itemKey is not (int or string)) { Log("Shop", "Invalid item key type."); return false; }
        if (quantity <= 0) return false;
        if (Bot == null || Bot.Player == null || Bot.Shops == null) return false;

        bool EnsureMapJoin(string m)
        {
            if (!ensureMap) return true;
            if (string.IsNullOrWhiteSpace(m)) return true;
            if (Bot.Map?.Name?.Equals(m, StringComparison.OrdinalIgnoreCase) == true) return true;
            Join(m);
            return Bot.Map?.Name?.Equals(m, StringComparison.OrdinalIgnoreCase) == true;
        }

        bool LoadShop(int id)
        {
            for (int attempt = 0; attempt < 3 && !Bot.ShouldExit; attempt++)
            {
                int cache0 = Bot.Shops.LoadedCache?.Count ?? 0;
                Bot.Shops.Load(id);

                long t0 = Environment.TickCount64;
                while (!Bot.ShouldExit && Environment.TickCount64 - t0 < loadTimeoutMs)
                {
                    int items = Bot.Shops.Items?.Count ?? 0;
                    int cache = Bot.Shops.LoadedCache?.Count ?? 0;
                    if (items > 0 || cache > cache0) return true;
                    Bot.Sleep(50);
                }
            }
            return false;
        }

        ShopItem FindItem(object key)
        {
            var list = Bot?.Shops?.Items;
            if (list == null) return null;

            switch (key)
            {
                case int id when id > 0:
                    foreach (var it in list)
                        if (it != null && it.ID == id) return it;
                    break;

                case string s when !string.IsNullOrWhiteSpace(s):
                    foreach (var it in list)
                    {
                        var n = it?.Name;
                        if (!string.IsNullOrWhiteSpace(n) &&
                            n.Equals(s, StringComparison.OrdinalIgnoreCase))
                            return it;
                    }
                    break;
            }
            return null;
        }

        int Have(string name)
        {
            if (considerBank) InBank(name);
            return Owned(name);
        }

        int Need(string name, int want)
        {
            int cur = Have(name);
            if (skipIfHaveEnough && cur >= want) return 0;
            return calculateRemaining ? Math.Max(0, want - cur) : want;
        }

        bool HasInvSpace() => (Bot.Inventory?.FreeSlots ?? 0) > 0;

        // ----- flow -----
        if (!EnsureMapJoin(map)) { Log("Shop", $"Failed to join {map}"); return false; }
        if (!LoadShop(shopId)) { Log("Shop", $"Failed to load shop {shopId}"); return false; }

        var it = FindItem(itemKey);
        if (it == null) { Log("Shop", $"Item not found: {itemKey}"); return false; }

        string itemName = it.Name;
        if (string.IsNullOrWhiteSpace(itemName)) { Log("Shop", "Item has no valid name."); return false; }

        int need = Need(itemName, quantity);
        if (need == 0) return true;

        long price = (long)it.Cost * need;
        if (checkGold && Bot.Player.Gold < price)
        { Log("Shop", $"Gold needed {price}, have {Bot.Player.Gold}"); return false; }

        if (checkLevel && Bot.Player.Level < it.Level)
        { Log("Shop", $"Level {it.Level}+ required"); return false; }

        if (checkInvSpace && !HasInvSpace())
        { Log("Shop", "No inventory space"); return false; }

        int before = Owned(itemName); // inventory only
        Bot.Shops.BuyItem(it.ID, it.ShopItemID, need);

        long t0c = Environment.TickCount64;
        while (Environment.TickCount64 - t0c < 2000)
        {
            if (Owned(itemName) > before) break;
            Bot.Sleep(50);
        }

        int gained = Owned(itemName) - before;
        bool ok = gained > 0;
        Log("Shop", ok ? $"Purchased {gained}x {itemName}" : $"Purchase failed: {itemName}");
        return ok;
    }

    #endregion

    #region Drops

    bool HasDrop(object key)
    {
        switch (key)
        {
            case int id when id > 0:
                {
                    var infos = Bot?.Drops?.CurrentDropInfos;
                    if (infos == null) return false;
                    foreach (var i in infos)
                        if (i?.ID == id) return true;
                    return false;
                }

            case string s when !string.IsNullOrWhiteSpace(s):
                {
                    // fast check
                    var names = Bot?.Drops?.CurrentDrops;
                    if (names != null)
                        foreach (var n in names)
                            if (string.Equals(n, s, StringComparison.OrdinalIgnoreCase))
                                return true;

                    // confirm
                    var infos = Bot?.Drops?.CurrentDropInfos;
                    if (infos != null)
                        foreach (var i in infos)
                            if (string.Equals(i?.Name, s, StringComparison.OrdinalIgnoreCase))
                                return true;

                    return false;
                }

            default:
                return false;
        }
    }

    ItemBase GetDropItem(object key)
    {
        var infos = Bot?.Drops?.CurrentDropInfos;
        if (infos == null) return null;

        switch (key)
        {
            case int id when id > 0:
                foreach (var i in infos)
                    if (i?.ID == id) return i;
                break;

            case string s when !string.IsNullOrWhiteSpace(s):
                foreach (var i in infos)
                    if (string.Equals(i?.Name, s, StringComparison.OrdinalIgnoreCase))
                        return i;
                break;
        }
        return null;
    }

    void Pickup(params object[] keys)
    {
        if (keys == null || keys.Length == 0) return;

        foreach (var k in keys)
        {
            switch (k)
            {
                case int id when id > 0:
                    if (HasDrop(id)) { Bot.Drops.Pickup(id); Bot.Sleep(D1); }
                    break;

                case string s when !string.IsNullOrWhiteSpace(s):
                    if (HasDrop(s)) { Bot.Drops.Pickup(s); Bot.Sleep(D1); }
                    break;
            }
        }
    }

    bool WaitForDrop(object key, int timeout = 30000)
    {
        if (key is not (int or string)) return false;

        long t0 = Environment.TickCount64;
        while (!Bot.ShouldExit && !HasDrop(key) && Environment.TickCount64 - t0 < timeout)
            Bot.Sleep(D1);

        return HasDrop(key);
    }

    bool HasAny(params object[] keys)
    {
        if (keys == null || keys.Length == 0) return false;

        foreach (var k in keys)
        {
            switch (k)
            {
                case int id when id > 0:
                    if (HasDrop(id)) return true;
                    break;
                case string s when !string.IsNullOrWhiteSpace(s):
                    if (HasDrop(s)) return true;
                    break;
            }
        }
        return false;
    }

    #endregion

    #region Player

    public double GetHealthPercentage()
    {
        if (Bot?.Player == null || Bot.Player.MaxHealth <= 0) return 0;
        return (double)Bot.Player.Health / Bot.Player.MaxHealth * 100;
    }

    public double GetManaPercentage()
    {
        if (Bot?.Player == null || Bot.Player.MaxMana <= 0) return 0;
        return (double)Bot.Player.Mana / Bot.Player.MaxMana * 100;
    }

    public bool IsHealthLow(double percentage = 30)
    {
        return GetHealthPercentage() < percentage;
    }

    public bool IsManaLow(double percentage = 30)
    {
        return GetManaPercentage() < percentage;
    }

    public bool IsHealthHigh(double percentage = 90)
    {
        return GetHealthPercentage() > percentage;
    }

    public bool IsManaHigh(double percentage = 90)
    {
        return GetManaPercentage() > percentage;
    }

    public bool IsFullHealth()
    {
        if (Bot?.Player == null) return false;
        return Bot.Player.Health >= Bot.Player.MaxHealth;
    }

    public bool IsFullMana()
    {
        if (Bot?.Player == null) return false;
        return Bot.Player.Mana >= Bot.Player.MaxMana;
    }

    public bool IsFullHealthAndMana()
    {
        return IsFullHealth() && IsFullMana();
    }

    public bool IsDead()
    {
        if (Bot?.Player == null) return true; // Assume dead if can't check
        return Bot.Player.State == 0;
    }

    public bool IsIdle()
    {
        if (Bot?.Player == null) return false;
        return Bot.Player.State == 1;
    }

    public double GetDistanceTo(int x, int y)
    {
        if (Bot?.Player == null) return double.MaxValue;

        int deltaX = Bot.Player.X - x;
        int deltaY = Bot.Player.Y - y;
        return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    public bool IsInRangeOf(int x, int y, double range = 50)
    {
        return GetDistanceTo(x, y) <= range;
    }

    public string GetCurrentClassName()
    {
        return Bot?.Player?.CurrentClass?.Name ?? "No Class";
    }

    public bool HasClassEquipped(string className)
    {
        if (string.IsNullOrWhiteSpace(className)) return false;
        return Bot?.Player?.CurrentClass?.Name?.Equals(className, StringComparison.OrdinalIgnoreCase) == true;
    }

    public bool IsCurrentClassMaxRank()
    {
        if (Bot?.Player == null) return false;
        return Bot.Player.CurrentClassRank >= 10;
    }

    public bool IsInCell(string cellName)
    {
        if (string.IsNullOrWhiteSpace(cellName)) return false;
        return Bot?.Player?.Cell?.Equals(cellName, StringComparison.OrdinalIgnoreCase) == true;
    }

    public bool NeedsRest(double healthThreshold = 50, double manaThreshold = 50)
    {
        return IsHealthLow(healthThreshold) || IsManaLow(manaThreshold);
    }

    public bool ShouldRest()
    {
        if (Bot?.Player == null) return false;
        return !Bot.Player.InCombat && !IsFullHealthAndMana();
    }

    public string GetTargetName()
    {
        return Bot?.Player?.Target?.Name ?? string.Empty;
    }

    public double GetTargetHealthPercentage()
    {
        var target = Bot?.Player?.Target;
        if (target == null || target.MaxHP <= 0) return 0;
        return (double)target.HP / target.MaxHP * 100;
    }

    public bool IsTargetAlive()
    {
        return Bot?.Player?.Target?.Alive == true;
    }

    public bool IsTargetHealthLow(double percentage = 30)
    {
        return GetTargetHealthPercentage() < percentage;
    }

    public bool HasEnoughGold(int amount)
    {
        if (Bot?.Player == null) return false;
        return Bot.Player.Gold >= amount;
    }

    public PlayerStats GetPlayerStats()
    {
        return Bot?.Player?.Stats ?? new PlayerStats();
    }

    public int GetStatValue(string statName)
    {
        if (string.IsNullOrWhiteSpace(statName)) return 0;

        var stats = Bot?.Player?.Stats;
        if (stats == null) return 0;

        return statName.ToUpper() switch
        {
            "STR" or "STRENGTH" => stats.Strength,
            "WIS" or "WISDOM" => stats.Wisdom,
            "DEX" or "DEXTERITY" => stats.Dexterity,
            "END" or "ENDURANCE" => stats.Endurance,
            "INT" or "INTELLECT" => stats.Intellect,
            "LCK" or "LUCK" => stats.Luck,
            "AP" or "ATTACKPOWER" => stats.AttackPower,
            "SP" or "SPELLPOWER" => stats.SpellPower,
            _ => 0
        };
    }

    public float GetCriticalChance()
    {
        return Bot?.Player?.Stats?.CriticalChance ?? 0f;
    }

    public float GetCriticalMultiplier()
    {
        return Bot?.Player?.Stats?.CriticalMultiplier ?? 0f;
    }

    public float GetEvasionChance()
    {
        return Bot?.Player?.Stats?.EvasionChance ?? 0f;
    }

    public float GetHaste()
    {
        return Bot?.Player?.Stats?.Haste ?? 0f;
    }

    public bool IsReadyForCombat()
    {
        if (Bot?.Player == null) return false;
        return Bot.Player.Alive && Bot.Player.Loaded;
    }

    #endregion

    #region Map

    string _bestCell = null;
    string _bestPad = "Left";

    public void Join(string map, string cell = "Enter", string pad = "Spawn", bool publicRoom = false, int? roomNumber = null)
    {
        if (string.IsNullOrWhiteSpace(map) || Bot?.Map == null || Bot?.Player == null) return;

        string mapName = map.Split('-')[0].Trim();

        string target = publicRoom ? mapName
            : roomNumber.HasValue ? $"{mapName}-{roomNumber.Value}"
            : map.Contains("-") ? map
            : $"{mapName}-{GenerateRoomID()}";

        if (Bot.Map.Name?.Equals(mapName, StringComparison.OrdinalIgnoreCase) == true)
        {
            if (!string.IsNullOrWhiteSpace(cell))
            {
                Bot.Map.Jump(cell, pad);
                Bot.Wait.ForCellChange(cell);
            }
            return;
        }

        StopAttack();

        int attempts = 0;
        while (!Bot.ShouldExit && attempts < 5)
        {
            Bot.Send.Packet($"%xt%zm%cmd%{Bot.Map.RoomID}%tfer%{Bot.Player.Username}%{target}%{cell}%{pad}%");
            long t0 = Environment.TickCount64;
            while (!Bot.ShouldExit && Environment.TickCount64 - t0 < 8000)
            {
                if (Bot.Map.Name?.Equals(mapName, StringComparison.OrdinalIgnoreCase) == true)
                    goto joined;
                Bot.Sleep(100);
            }

            attempts++;
            Bot.Sleep(300);
        }

    joined:
        ResetMonsterSetupCache();
    }

    public void ChooseBestCell(string monsterNames, bool alt = false, string setCell = null, string setPad = "Spawn")
    {
        var names = (monsterNames ?? string.Empty)
            .Split(new[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(n => n.Trim())
            .Where(n => n.Length > 0)
            .ToArray();

        bool wildcard = names.Length == 0 || (names.Length == 1 && names[0] == "*");
        string pad = string.IsNullOrWhiteSpace(setPad) ? "Left" : setPad;

        var monsters = (Bot.Monsters.MapMonsters ?? Enumerable.Empty<Monster>())
            .Where(m => m != null && !string.IsNullOrWhiteSpace(m.Cell))
            .Where(m => wildcard || names.Any(name =>
                string.Equals(m.Name ?? string.Empty, name, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        if (monsters.Count == 0)
            return;

        string targetCell =
            !string.IsNullOrWhiteSpace(setCell) ? setCell
            : alt ? monsters.FirstOrDefault()?.Cell
            : monsters.GroupBy(m => m.Cell, StringComparer.Ordinal)
                      .OrderByDescending(g => g.Count())
                      .Select(g => g.Key)
                      .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(targetCell))
            return;

        var mapCells = new HashSet<string>(Bot.Map.Cells as IEnumerable<string> ?? Array.Empty<string>(),
                                           StringComparer.Ordinal);
        if (!mapCells.Contains(targetCell))
            return;

        _bestCell = targetCell;
        _bestPad = pad;

        if (!string.Equals(Bot.Player.Cell, targetCell, StringComparison.Ordinal))
        {
            Bot.Map.Jump(targetCell, pad);
            Bot.Wait.ForCellChange(targetCell);
            Bot.Player.SetSpawnPoint();
        }
    }

    int GenerateRoomID()
    {
        // Creates a stable room ID (1000-99999) based on machine only
        // All accounts on same PC share the same room permanently
        string machineId;
        try
        {
            machineId = Microsoft.Win32.Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography", "MachineGuid", null
            ) as string ?? Environment.MachineName;
        }
        catch { machineId = Environment.MachineName; }

        string seed = machineId;
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(seed));
        uint roomSeed = BitConverter.ToUInt32(hash, 0);
        return (int)(roomSeed % 99000) + 1000;
    }

    double GetLowestHpPercentage()
    {
        var names = Bot?.Map?.PlayerNames;
        if (names == null || names.Count == 0) return 100.0;

        double lowest = 100.0;
        foreach (var playerName in names)
        {
            if (string.IsNullOrWhiteSpace(playerName)) continue;
            try
            {
                int hp = Bot.Flash.GetGameObject<int>($"world.uoTree.{playerName}.intHP");
                int maxHp = Bot.Flash.GetGameObject<int>($"world.uoTree.{playerName}.intHPMax");
                if (maxHp > 0 && hp >= 0)
                {
                    double pct = (double)hp / maxHp * 100.0;
                    if (pct < lowest) lowest = pct;
                }
            }
            catch { }
        }
        return lowest;
    }

    bool IsArmyHealthLow(double percentage = 30.0) => GetLowestHpPercentage() < percentage;

    void WhiteMap() => Join("whitemap");

    #endregion

    #region Skills

    readonly int skillsDelay = 50;

    async Task SkillsAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                Skills();
            }
            catch { }

            try
            {
                await Task.Delay(skillsDelay, token);
            }
            catch (TaskCanceledException) { break; }
            catch
            {
                try
                {
                    await Task.Delay(1000, token);
                }
                catch (TaskCanceledException) { break; }
                catch { }
            }
        }
    }

    bool NotUltraDage() =>
        !string.Equals(Bot.Map.Name, "ultradage", StringComparison.OrdinalIgnoreCase);

    void Skills()
    {
        if (Bot?.Player == null) return;
        if (!Bot.Player.HasTarget) return;

        string className = Bot.Player.CurrentClass?.Name?.ToLower();
        if (string.IsNullOrWhiteSpace(className)) return;

        switch (className)
        {
            // Ultra classes
            case "legion revenant": LegionRevenantClass(); break;
            case "archpaladin": ArchPaladinClass(); break;
            case "stonecrusher": StoneCrusherClass(); break;
            case "infinity titan": InfinityTitanClass(); break;
            case "lord of order": LordsOfOrderClass(); break;
            case "void highlord": VoidHighlordClass(); break;
            case "chaos avenger": ChaosAvengerClass(); break;
            case "lightcaster": LightCasterClass(); break;
            case "legion doomknight": LegionDoomKnightClass(); break;
            case "dragon of time": DragonOfTimeClass(); break;
            case "archmage": ArchmageClass(); break;
            case "verus doomknight": VerusDoomKnight(); break;
            case "arcana invoker": ArcanaInvokerClass(); break;

            // Chrono classes
            case "chrono dragonknight": case "chrono dataknight": ChronoDataKnightClass(); break;
            case "shadowstalker of time": case "shadowweaver of time": ShadowWeaverOfTimeClass(); break;
            case "continuum chronomancer": case "quantum chronomancer": QuantumChronomancerClass(); break;
            case "nechronomancer": case "necrotic chronomancer": NecroticChronomancerClass(); break;
            case "legion paladin": case "obsidian paladin chronomancer": ObsidianPaladinChronomancerClass(); break;
            case "chrono shadowslayer": ChronoShadowSlayerClass(); break;

            // Common classes
            case "master ranger": MasterRangerClass(); break;
            case "dragonslayer general": DragonslayerGeneralClass(); break;
            case "cryomancer": CryomancerClass(); break;
            case "dragon knight": DragonKnightClass(); break;
            case "shaman": ShamanClass(); break;
            case "evolved shaman": EvolvedShamanClass(); break;
            case "dark legendary hero": DarkLegendaryHeroClass(); break;
            case "necromancer": NecromancerClass(); break;
            case "chrono assassin": ChronoAssassinClass(); break;
            case "guardian": GuardianClass(); break;
            case "great thief": GreatThiefClass(); break;
            case "chaos slayer berserker":
            case "chaos slayer cleric":
            case "chaos slayer mystic":
            case "chaos slayer thief": ChaosSlayerClass(); break;

            // Basic classes
            case "mage": MageClass(); break;
            case "dragonslayer": DragonslayerClass(); break;

            default:
                // No rotation available - just return silently
                break;
        }
    }

    // --- ultra classes ---------------------------------------------------------------

    void LegionRevenantClass()
    {
        if (Cast(3)) return;
        if (Cast(2)) return;
        if (Cast(1)) return;
        if (Cast(4)) return;
    }

    void LordsOfOrderClass()
    {
        if ((IsHealthLow(85) || IsArmyHealthLow(85)) && NotUltraDage())
            if (Cast(2)) return;
        if (Cast(4)) return;
        if (Left("Empowerment", 1, true))
            if (Cast(1)) return;
        if (Left("Clarity", 1, true))
            if (Cast(3)) return;
    }

    void StoneCrusherClass()
    {
        var mode = GetMode("StoneCrusher");

        if (mode == "Ultra")
        {
            if (IsHealthLow(80) || IsArmyHealthLow(80) && HasAura("Magnitude", true))
                if (Cast(3)) return;
            if (Left("Dissonance", 1, true))
                if (Cast(2)) return;
            if (Cast(4)) return;
            if (Cast(1)) return;
        }
        else
        {
            if (IsHealthLow(80) || IsArmyHealthLow(80))
                if (Cast(3)) return;
            if (HasAura("Magnitude", true))
                if (Cast(4)) return;
            if (Left("Dissonance", 1, true))
                if (Cast(2)) return;
            if (Cast(1)) return;
        }
    }

    void InfinityTitanClass()
    {
        var mode = GetMode("InfinityTitan");

        if (mode == "Ultra")
        {
            if (IsHealthLow(80) || IsArmyHealthLow(80) && HasAura("Anima", true))
                if (Cast(3)) return;
            if (Left("Universal Power", 1, true))
                if (Cast(2)) return;
            if (Cast(4)) return;
            if (Cast(1)) return;
        }
        else
        {
            if (IsHealthLow(80) || IsArmyHealthLow(80))
                if (Cast(3)) return;
            if (HasAura("Anima", true))
                if (Cast(4)) return;
            if (Left("Universal Power", 1, true))
                if (Cast(2)) return;
            if (Cast(1)) return;
        }
    }

    void ArchPaladinClass()
    {
        if ((IsHealthLow(70) || IsArmyHealthLow(70)) && NotUltraDage())
            if (Cast(2)) return;
        if (!HasAura("Righteous Seal"))
            if (Cast(4)) return;
        if (Cast(3)) return;
        if (Cast(1)) return;
    }

    void VoidHighlordClass()
    {
        if (HasAura("Unshackled", true))
            if (Cast(4)) return;
        if (IsHealthHigh(60))
            if (Cast(1)) return;
        if (Cast(2)) return;
        if (IsHealthHigh(60))
            if (Cast(3)) return;
    }

    void ChaosAvengerClass()
    {
        if (Cast(2)) return;
        if (Cast(4)) return;
        if (Cast(1)) return;
        if (Cast(3)) return;
    }

    void LightCasterClass()
    {
        if (IsHealthLow(85) || IsArmyHealthLow(85) || Left("Illuminated", 1, true))
            if (Cast(3)) return;
        if (Cast(4)) return;
        if (Cast(1)) return;
        if (Cast(2)) return;
    }

    void LegionDoomKnightClass()
    {
        if (Cast(4)) return;
        if (Cast(1)) return;
        if (Cast(2)) return;
        if (Cast(3)) return;
    }

    void DragonOfTimeClass()
    {
        if (IsHealthLow(95))
            if (Cast(2)) return;
        if (HasAura("Convergence", true))
            if (Cast(3)) return;
        if (IsHealthHigh(60))
            if (Cast(4)) return;
        if (Cast(1)) return;
    }

    void ArchmageClass()
    {
        if (IsManaLow(30))
            if (Cast(2)) return;
        if (HasAura("Arcane Flux", true) && !HasAura("Corporeal Ascension", true) && !HasAura("Astral Ascension", true))
            if (Cast(4)) return;
        if (HasAura("Corporeal Ascension", true) && !HasAura("Astral Ascension", true))
            if (Cast(4)) return;
        if (Cast(1)) return;
        if (Cast(3)) return;
    }

    void VerusDoomKnight()
    {
        if (IsHealthLow(50))
            if (Cast(2)) return;
        if (Stacks("Doom", 10, true))
            if (Cast(4)) return;
        if (Cast(1)) return;
        if (Cast(2)) return;
        if (Cast(3)) return;
    }

    void ArcanaInvokerClass()
    {
        void standardRotation()
        {
            if (Cast(2)) return;
            if (Cast(4)) return;
            if (Cast(3)) return;
        }

        if (HasAura("XXI - The World", true))
        {
            if (Left("XXI - The World", 8, true))
                if (Cast(1)) return;

            standardRotation();
        }
        else
        {
            bool hasJudgement = HasAura("XX - Judgement", true);
            bool hasFool = HasAura("0 - The Fool", true);
            bool needsFool = !hasFool || !HasAnyAuraOtherThan("0 - The Fool", true);

            if ((hasJudgement && hasFool) || needsFool)
                if (Cast(1)) return;

            if (hasFool)
            {
                standardRotation();
            }
        }
    }

    // --- chrono classes ---------------------------------------------------------------

    void ChronoDataKnightClass()
    {
        if (Stacks("Temporal Rift", 4, true))
            if (Cast(4)) return;
        if (Cast(1)) return;
        if (Cast(2)) return;
        if (Cast(3)) return;
    }

    void ShadowWeaverOfTimeClass()
    {
        if (IsHealthLow(50) || IsManaLow(30))
            if (Cast(3)) return;
        if (Stacks("Chaos Rift", 4, true))
            if (Cast(4)) return;
        if (Cast(1)) return;
        if (Cast(2)) return;
    }

    void QuantumChronomancerClass()
    {
        if (Stacks("Temporal Rift", 4, true))
            if (Cast(3)) return;
        if (HasAura("Quantum Restructure", true))
            if (Cast(4)) return;
        if (Cast(2)) return;
        if (Cast(1)) return;
    }

    void NecroticChronomancerClass()
    {
        if (Stacks("Chaos Rift", 4, true))
            if (Cast(3)) return;
        if (Left("Debilitated", 2))
            if (Cast(4)) return;
        if (Cast(2)) return;
        if (Cast(1)) return;
    }

    void ObsidianPaladinChronomancerClass()
    {
        if (IsHealthLow(50) || IsArmyHealthLow(50))
            if (Cast(3)) return;
        if (IsHealthLow(80) || IsArmyHealthLow(80))
            if (Cast(2)) return;
        if (Stacks("Temporal Rift", 4, true))
            if (Cast(4)) return;
        if (Cast(1)) return;
    }

    void ChronoShadowSlayerClass()
    {
        if (HasAura("Rounds Empty", true))
            if (Cast(1)) return;
        if (HasAura("Gunslinger Stance", true))
            if (Cast(0)) return;
        if (Stacks("Temporal Rift", 4, true))
            if (Cast(1)) return;
        if (HasAura("Chaos Rift", true) && !HasAura("Gunslinger Stance", true))
            if (Cast(4)) return;
        if (!HasAura("FMJ Rounds", true) && !HasAura("Tracer Rounds", true))
            if (Cast(3)) return;
    }

    // --- common classes ---------------------------------------------------------------

    void MasterRangerClass()
    {
        if (HasAura("Vampiric Shot", true))
            if (Cast(3)) return;
        if (Stacks("Marks", 6, true))
            if (Cast(4)) return;
        if (Stacks("Marks", 3, true))
            if (Cast(2)) return;
        if (Cast(1)) return;
    }

    void DragonslayerGeneralClass()
    {
        if (HasAura("General's Dragonbane"))
            if (Cast(2)) return;
        if (HasAura("General's Dragonbane"))
            if (Cast(3)) return;
        if (Cast(4)) return;
        if (Cast(1)) return;
    }

    void CryomancerClass()
    {
        if (IsHealthLow(60) && HasAura("Polar Vortex", true))
            if (Cast(3)) return;
        if (HasAura("Frozen") && HasAura("Polar Vortex", true))
            if (Cast(2)) return;
        if (Cast(1)) return;
        if (Cast(4)) return;
    }

    void DragonslayerClass()
    {
        if (HasAura("Dragonbane") && !HasAura("Infected Wound"))
            if (Cast(2)) return;
        if (HasAura("Dragonbane") && !HasAura("Weakened"))
            if (Cast(3)) return;
        if (Cast(4)) return;
        if (Cast(1)) return;
    }

    void DragonKnightClass()
    {
        if (Cast(1)) return;
        if (HasAura("Flammable"))
            if (Cast(4)) return;
        if (Cast(2)) return;
        if (HasAura("Dumbfounded"))
            if (Cast(3)) return;
    }

    void ShamanClass()
    {
        if (Left("Elemental Embrace", 5))
            if (Cast(4)) return;
        if (HasAura("Elemental Embrace"))
            if (Cast(3)) return;
        if (HasAura("Scorched Spirit"))
            if (Cast(2)) return;
        if (Cast(1)) return;
    }

    void EvolvedShamanClass()
    {
        if (IsHealthLow(80) || IsArmyHealthLow(80))
            if (Cast(3)) return;
        if (Left("Elemental Grasp", 5))
            if (Cast(4)) return;
        if (Cast(2)) return;
        if (Cast(1)) return;
    }

    void DarkLegendaryHeroClass()
    {
        if (IsHealthLow(30) || IsArmyHealthLow(30))
            if (Cast(4)) return;
        if (Cast(3)) return;
        if (Cast(2)) return;
        if (Cast(1)) return;
    }

    void NecromancerClass()
    {
        if (IsManaLow(90) && IsHealthHigh(80) && !HasAura("Deadly Frenzy", true))
            if (Cast(3)) return;
        if (IsManaLow(30) && IsHealthHigh(80) && HasAura("Deadly Frenzy", true))
            if (Cast(3)) return;
        if (IsManaHigh(80) && IsHealthHigh(80))
            if (Cast(4)) return;
        if (HasAura("Deadly Frenzy", true))
            if (Cast(1)) return;
        if (Cast(2)) return;
    }

    void ChronoAssassinClass()
    {
        if (HasAura("Reverse Time", true))
        {
            if (Cast(4)) return;
        }
        else
        {
            if (Cast(3)) return;
            if (Cast(1)) return;
        }
        if (Cast(2)) return;
    }

    void GuardianClass()
    {
        if ((HasAura("Hypercritical", true) || HasAura("Void Imbue", true)) && Stacks("Guardian Spirit", 15, true))
            if (Cast(4)) return;
        if (IsManaLow(70))
            if (Cast(3)) return;
        if (Cast(1)) return;
        if (Cast(2)) return;
    }

    void GreatThiefClass()
    {
        if (HasAura("Hidden Blade", true))
            if (Cast(4)) return;
        if (Cast(3)) return;
        if (Cast(1)) return;
        if (Cast(2)) return;
    }

    void ChaosSlayerClass()
    {
        if ((HasAura("Impasse") || HasAura("Delusion") || HasAura("Angustied")) && !HasAura("Corageous", true))
            if (Cast(4)) return;
        if (Cast(3)) return;
        if (Cast(1)) return;
        if (Cast(2)) return;
    }

    // --- basic classes ---------------------------------------------------------------

    void MageClass()
    {
        if (Left("Arcane Shield", 1, true))
            if (Cast(4)) return;
        if (HasAura("Frozen Blood"))
            if (Cast(1)) return;
        if (HasAura("Scorched"))
            if (Cast(3)) return;
        if (Cast(2)) return;
    }

    // --- helpers ---------------------------------------------------------------

    public bool Cast(int index)
    {
        if (index < 1 || index > 4) return false;
        if (Bot?.Skills == null) return false;

        if (!Bot.Skills.CanUseSkill(index)) return false;

        try
        {
            Bot.Skills.UseSkill(index);
            return true;
        }
        catch { return false; }
    }

    public void DisableSkills()
    {
        try
        {
            _cts?.Cancel();
        }
        catch { }
        finally
        {
            _runSkills = null;
            _cts = null;
        }
    }

    public void EnableSkills()
    {
        if (_runSkills != null && !_runSkills.IsCompleted) return;

        try
        {
            _cts = new CancellationTokenSource();
            _runSkills = Task.Run(() => SkillsAsync(_cts.Token));
        }
        catch
        {
            _cts?.Dispose();
            _cts = null;
            _runSkills = null;
        }
    }

    private Dictionary<string, string> _classRotationMode = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public void SetClassRotation(string className, string mode)
    {
        if (string.IsNullOrWhiteSpace(className) || string.IsNullOrWhiteSpace(mode))
            return;

        _classRotationMode[className] = mode;
        Log("Rotation", $"{className} set to {mode} mode");
    }

    private string GetMode(string className) =>
        _classRotationMode.TryGetValue(className, out var mode) ? mode : "Default";

    #endregion

    #region Auras

    public IEnumerable<Aura> GetAuras(bool self) =>
        (self ? Bot?.Self?.Auras : Bot?.Target?.Auras) ?? Enumerable.Empty<Aura>();

    public Aura GetAuraByName(string auraName, bool self)
    {
        if (string.IsNullOrWhiteSpace(auraName)) return null;
        return GetAuras(self).FirstOrDefault(a => a != null &&
            !string.IsNullOrWhiteSpace(a.Name) &&
            auraName.Equals(a.Name, StringComparison.OrdinalIgnoreCase));
    }

    public bool HasAura(string auraName, bool self = false)
    {
        return GetAuraByName(auraName, self) != null;
    }

    public bool HasAnyAura(List<string> auraNames, bool self = false)
    {
        if (auraNames == null || auraNames.Count == 0) return false;
        foreach (string aura in auraNames)
        {
            if (!string.IsNullOrWhiteSpace(aura) && HasAura(aura, self))
                return true;
        }
        return false;
    }

    public bool HasAnyAuraOtherThan(string auraName, bool self = false)
    {
        if (string.IsNullOrWhiteSpace(auraName)) return false;

        var auras = GetAuras(self);
        return auras.Any(a => a != null &&
                            !string.IsNullOrWhiteSpace(a.Name) &&
                            !auraName.Equals(a.Name, StringComparison.OrdinalIgnoreCase));
    }

    public int GetAuraStacks(string auraName, bool self = false)
    {
        if (string.IsNullOrWhiteSpace(auraName)) return 0;
        try
        {
            object v = self ? Bot?.Self?.GetAuraValue(auraName)
                            : Bot?.Target?.GetAuraValue(auraName);
            if (v == null) return 0;

            int rawValue = v is int i ? i
                 : v is long l ? (int)l
                 : v is double d ? (int)Math.Round(d)
                 : v is float f ? (int)Math.Round(f)
                 : int.TryParse(v.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var n) ? n
                 : 0;

            return rawValue + 1;
        }
        catch { return 0; }
    }

    public int GetAuraSecondsRemaining(string auraName, bool self = false)
    {
        if (string.IsNullOrWhiteSpace(auraName)) return 0;
        var aura = GetAuraByName(auraName, self);
        if (aura == null || aura._timeStamp <= 0 || aura.Duration <= 0) return 0;
        try
        {
            var applied = DateTimeOffset.FromUnixTimeMilliseconds(aura._timeStamp);
            var expires = applied.AddSeconds(aura.Duration);
            var remaining = (int)(expires - DateTimeOffset.Now).TotalSeconds;
            return Math.Max(0, remaining);
        }
        catch { return 0; }
    }

    public bool Stacks(string name, int quantity, bool self = false)
    {
        if (string.IsNullOrWhiteSpace(name) || quantity <= 0) return false;
        int stacks = GetAuraStacks(name, self);
        return stacks >= quantity;
    }

    public bool Left(string name, int duration, bool self = false)
    {
        if (string.IsNullOrWhiteSpace(name) || duration < 0) return false;
        int rem = GetAuraSecondsRemaining(name, self); // returns 0 if expired/missing
        return rem <= duration;
    }

    #endregion

    #region Listeners

    private volatile bool _chargeDetected;
    private int _chargeCount;

    void PulseCharge(int ms = 2000)
    {
        System.Threading.Interlocked.Increment(ref _chargeCount);
        _chargeDetected = true;
        Task.Run(async () =>
        {
            await Task.Delay(ms);
            if (System.Threading.Interlocked.Decrement(ref _chargeCount) <= 0)
            {
                _chargeCount = 0;
                _chargeDetected = false;
            }
        });
    }

    public void ChargeListener(dynamic packet)
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
                    PulseCharge(2000);
                    break;
                }
            }
        }
        catch { }
    }

    void ChaosHarpyListener(dynamic packet)
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
                    Task.Run(async () =>
                    {
                        DisableSkills();
                        await Task.Delay(500);
                        Bot.Skills.UseSkill(5);
                        await Task.Delay(500);
                        EnableSkills();
                    });
                    break;
                }
            }
        }
        catch { }
    }

    #endregion

    #region Experimental Ultras

    public void TauntCycle(string name, string monster, string aura, int checkDelay)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(monster) || string.IsNullOrWhiteSpace(aura)) return;
        if (Bot?.Combat == null) return;
        if (HasClassEquipped(name))
        {
            int effect = GetAuraSecondsRemaining(aura);
            Bot.Combat.Attack(monster);
            if (checkDelay > 0) Bot.Sleep(checkDelay);
            if (effect < 2) UsePotion();
        }
    }

    public void TauntCharge(string name, string monster, string aura, int checkDelay)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(monster)) return;
        if (Bot?.Combat == null) return;
        if (HasClassEquipped(name))
        {
            Bot.Combat.Attack(monster);
            if (checkDelay > 0) Bot.Sleep(checkDelay);
            if (_chargeDetected) UsePotion();
        }
    }

    public void KillWithPriority(string primaryName, int primaryMapId, string priorityName1, int priorityMapId1, string priorityName2, int priorityMapId2)
    {
        if (string.IsNullOrWhiteSpace(primaryName)) return;
        if (!string.IsNullOrWhiteSpace(priorityName1) && IsAliveByMapId(priorityMapId1, name: priorityName1)) KillByMapId(priorityMapId1, name: priorityName1);
        else if (!string.IsNullOrWhiteSpace(priorityName2) && IsAliveByMapId(priorityMapId2, name: priorityName2)) KillByMapId(priorityMapId2, name: priorityName2);
        else KillByMapId(primaryMapId, name: primaryName);
        Bot.Sleep(D1);
    }

    public void KillByMapId(int mapId, string? name = null, int? id = null)
    {
        if (Bot?.Combat == null) return;
        if (IsAliveByMapId(mapId, name, id))
        {
            Bot.Combat.Attack(mapId);
            Bot.Sleep(250);
        }
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
                bool haveFocus = HasAura("Focus");
                bool shouldStop = Bot.ShouldExit;

                if (!wardenAlive) keepGoing = false;
                else if (haveFocus) keepGoing = false;
                else if (shouldStop) keepGoing = false;
                else UsePotion();
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
        extraHP[0] = 180000;
        extraHP[1] = 180000;
        extraHP[2] = 180000;
        extraHP[3] = 180000;
        extraHP[4] = 180000;
        extraHP[5] = 100000;
        extraHP[6] = 100000;
        extraHP[7] = 100000;
        extraHP[8] = 100000;

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
            Log("Drakath", message1 + message2);

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
                    UsePotion();
                    howManyTries = howManyTries + 1;

                    bool gotFocus = HasAura("Focus");
                    if (gotFocus) keepTrying = false;
                    else Bot.Sleep(120);
                }
            }

            bool hasFocusNow = HasAura("Focus");

            int finalHP = 0;
            if (Bot.Player.Target != null) if (Bot.Player.Target.HP != null) finalHP = Bot.Player.Target.HP;

            if (hasFocusNow)
            {
                string msg = "Focus obtained at " + finalHP.ToString("N0") + " HP (tries: " + howManyTries + ")";
                Log("Drakath", msg);
            }
            else
            {
                string msg = "Warning: Failed to get Focus (tries: " + howManyTries + ", hp now " + finalHP.ToString("N0") + ")";
                Log("Drakath", msg);
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

    // --- helpers ---------------------------------------------------

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
                catch (Exception ex) { Log("Army", $"[sync-path] {ex.Message}"); return string.Empty; }
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
            catch (Exception ex) { Log("Army", $"[sync-trunc] {ex.Message}"); }
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
            catch (Exception ex) { Log("Army", $"[sync-init] {ex.Message}"); Truncate(filePath); }
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
                catch (Exception ex) { Log("Army", $"[sync-get] {ex.Message}"); return; }
            }
            Log("Army", "[sync-get] retries exhausted.");
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
                catch (Exception ex) { Log("Army", $"[sync-set] {ex.Message}"); return; }
            }
            Log("Army", "[sync-set] retries exhausted.");
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
                catch (Exception ex) { Log("Army", $"[sync-read] {ex.Message}"); return 0; }
            }
            Log("Army", "[sync-read] retries exhausted."); return 0;
        }

        void FinalSpamAndStart()
        {
            Log("Army", "Final prep: spamming skills before pull...");
            var until = DateTime.UtcNow.AddMilliseconds(1200);
            while (DateTime.UtcNow < until && !Bot.ShouldExit)
            {
                if (!IsManaLow(50))
                {
                    Bot.Skills.UseSkill(1); Bot.Sleep(300);
                    Bot.Skills.UseSkill(2); Bot.Sleep(300);
                    Bot.Skills.UseSkill(3);
                }
                Bot.Sleep(100);
            }

            const int startMs = 3000;
            Log("Army", $"Everyone ready! Starting in {startMs}ms...");
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
        Log("Army", $"Sync file: {path}");
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

            Log("Army", $"Waiting for army: {Math.Max(0, playersNow - 1)}/{quantity} players in map");

            if (timeoutMs > 0 && timer.ElapsedMilliseconds >= timeoutMs)
            {
                Log("Army", "Timeout while waiting for players to join map.");
                break;
            }
            Bot.Sleep(tickMs);
        }
        if (Bot.ShouldExit) { Truncate(path); return; }

        SetReady(path, myName, true);
        Log("Army", $"Marked ready: {myName}");

        while (!Bot.ShouldExit)
        {
            int ready = CountReady(path);
            Log("Army", $"Sync: {ready}/{needed} ready");
            if (ready >= needed) break;
            Bot.Sleep(tickMs);
        }
        if (Bot.ShouldExit) { Truncate(path); return; }

        FinalSpamAndStart();
        Truncate(path);
        Log("Army", "GO!");
    }

    public bool MonsterAlive(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        if (Bot?.Monsters?.MapMonsters == null) return false;

        return Bot.Monsters.MapMonsters
            .Any(m => m?.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) == true && m.Alive);
    }

    public void StopAttack()
    {
        if (Bot?.Combat == null || Bot?.Map == null || Bot?.Player == null) return;

        Bot.Combat.CancelAutoAttack();
        Bot.Combat.CancelTarget();

        var cellsList = Bot.Map.Cells ?? new List<string>();
        var monstersList = Bot.Monsters?.MapMonsters ?? new List<Monster>();

        string safeCell = cellsList
            .Where(c => !string.IsNullOrWhiteSpace(c) &&
                       !c.Equals("Wait", StringComparison.OrdinalIgnoreCase) &&
                       !c.Equals("Blank", StringComparison.OrdinalIgnoreCase) &&
                       !c.StartsWith("Cut", StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => monstersList.Count(m => m?.Cell == c))
            .FirstOrDefault() ?? Bot.Player.Cell;

        string pad = string.IsNullOrWhiteSpace(Bot.Player.Pad) ? "Left" : Bot.Player.Pad;

        // hop until we're out
        while (!Bot.ShouldExit && Bot.Player.State == 2)
        {
            if (!string.Equals(Bot.Player.Cell, safeCell, StringComparison.Ordinal))
            {
                Bot.Map.Jump(safeCell, pad);
                Bot.Wait.ForCellChange(safeCell);
            }
            Bot.Sleep(D1);
        }
        Bot.Sleep(D3);
    }

    public void DontAttack()
    {
        if (Bot?.Combat == null || Bot?.Map == null || Bot?.Player == null) return;

        Bot.Combat.CancelAutoAttack();
        Bot.Combat.CancelTarget();

        DisableSkills();
        Bot.Sleep(5000);
        EnableSkills();
    }

    #endregion

    bool HasChanged<T>(string key, T newValue)
    {
        if (string.IsNullOrWhiteSpace(key)) return false;

        if (_cache.TryGetValue(key, out var prev) && prev is T p &&
            EqualityComparer<T>.Default.Equals(p, newValue))
            return false;

        _cache[key] = newValue;
        return true;
    }

    public bool Log(string category, string message)
    {
        if (string.IsNullOrWhiteSpace(category) || string.IsNullOrWhiteSpace(message))
            return false;

        category = category.Trim();
        message = message.Trim();

        if (!HasChanged(category, message))
            return false;

        var key = $"{category}:{message}";
        var now = DateTime.UtcNow;

        if (_throttle.TryGetValue(key, out var last) && now - last < ThrottleDuration)
            return false;

        _throttle[key] = now;
        OnSignal?.Invoke(category, message);
        return true;
    }
}