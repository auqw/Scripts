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
        Bot.Bank.Load();
        Bot.Wait.ForTrue(() => Bot.Bank.Items.Any(), 20);

        Bot.Options.SafeTimings = true;
        Bot.Options.InfiniteRange = true;
        Bot.Options.SkipCutscenes = true;
        Bot.Lite.HidePlayers = true;

        Alert("CORE", "System online");
    }

    bool OnScriptStopping(Exception e)
    {
        Alert("CORE", "System offline");

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

    int Qty(object key, bool temp = false) =>
        key is int id ? Owned(id, temp) :
        key is string s ? Owned(s, temp) : 0;

    bool PullFromBank(object key)
    {
        return key is int id ? InBank(id)
             : key is string s ? InBank(s)
             : false;
    }

    public void ForItem(string monsters, string map, object key, int quantity = 1, bool isTemp = false, bool useBestGear = false, bool alt = false, string? cell = null, string pad = "Left", bool priority = false)
    {
        if (key is null || quantity <= 0) return;

        if (!string.IsNullOrWhiteSpace(map)) Join(map);
        ChooseBestCell(monsters, alt, cell, pad);
        if (useBestGear) ChooseBestGear(monsters);

        var targets = monsters?.Split('|', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        if (!isTemp) PullFromBank(key);

        if (Qty(key, isTemp) >= quantity) return;

        Alert("FARMING", $"Killing {monsters} for {quantity}x {(key is int id ? (GetDropItem(id)?.Name ?? $"Item#{id}") : key)}");
        EnableSkills();

        int i = 0;
        while (!Bot.ShouldExit)
        {
            if (Qty(key, isTemp) >= quantity)
            {
                Alert("SUCCESS", $"Acquired {quantity}x {(key is int id2 ? (GetDropItem(id2)?.Name ?? $"Item#{id2}") : key)}");
                DisableSkills();
                StopAttack();
                return;
            }

            Pickup(key);

            if (targets.Length > 0)
            {
                if (priority)
                    KillWithPriority(targets.Select(MonsterKey.FromName).ToArray());
                else
                    Kill(targets[i++ % targets.Length]);
            }
        }
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

    #endregion

    #region Best Enhancement

    public InventoryItem ChooseBestEnhancement(string itemGroup, params string[] priority)
    {
        if (priority == null || priority.Length == 0) return null;

        string Norm(string g) => g?.ToLower() switch
        {
            "weapon" => "Weapon",
            "helm" or "he" => "he",
            "back" or "ba" or "cape" => "ba",
            "class" or "co" => "co",
            "pet" or "pe" => "pe",
            _ => g
        };

        int N(int? v, int d = -1) => v ?? d; // works whether source is int or int?

        string Enh(int id) => id switch
        {
            1 => "Adventurer",
            2 => "Fighter",
            3 => "Thief",
            4 => "Armsman",
            5 => "Hybrid",
            6 => "Wizard",
            7 => "Healer",
            8 => "Spellbreaker",
            9 => "Lucky",
            10 => "Forge",
            11 => "Absolution",
            12 => "Avarice",
            23 => "Depths",
            24 => "Vainglory",
            25 => "Vim",
            26 => "Examen",
            27 => "Pneuma",
            28 => "Anima",
            29 => "Penitence",
            30 => "Lament",
            32 => "Hearty",
            _ => null
        };

        string WeaponTrait(int id) => id switch
        {
            2 => "Spiral Carve",
            3 => "Awe Blast",
            4 => "Health Vamp",
            5 => "Mana Vamp",
            6 => "Powerword Die",
            7 => "Lacerate",
            8 => "Smite",
            9 => "Valiance",
            10 => "Arcana's Concerto",
            11 => "Acheron",
            12 => "Elysium",
            13 => "Praxis",
            14 => "Dauntless",
            15 => "Ravenous",
            _ => null
        };

        bool Match(InventoryItem i, string want, string grp)
        {
            if (i == null || string.IsNullOrWhiteSpace(want)) return false;
            if (Enh(N(i.EnhancementPatternID))?.Equals(want, StringComparison.OrdinalIgnoreCase) == true) return true;
            return grp.Equals("Weapon", StringComparison.OrdinalIgnoreCase) &&
                   WeaponTrait(N(i.ProcID))?.Equals(want, StringComparison.OrdinalIgnoreCase) == true;
        }

        InventoryItem Find(IEnumerable<InventoryItem> src, string want, string grp, bool mem) =>
            (src ?? Enumerable.Empty<InventoryItem>())
            .FirstOrDefault(i =>
                i != null &&
                i.ItemGroup?.Equals(grp, StringComparison.OrdinalIgnoreCase) == true &&
                (mem || !i.Upgrade) &&
                Match(i, want, grp));

        bool Equip(InventoryItem it)
        {
            if (it == null) return false;
            if (Bot.Inventory.IsEquipped(it.ID)) return true;
            for (int t = 0; t < 3; t++)
            {
                Bot.Inventory.EquipItem(it.ID);
                Bot.Sleep(500);
                if (Bot.Inventory.IsEquipped(it.ID)) return true;
            }
            return false;
        }

        var grp = Norm(itemGroup);
        bool mem = Bot?.Player?.IsMember == true;

        foreach (var want in priority.Where(s => !string.IsNullOrWhiteSpace(s)))
        {
            var hit = Find(Bot.Inventory.Items?.OfType<InventoryItem>(), want, grp, mem);
            if (Equip(hit)) return hit;

            var fromBank = Find(Bot.Bank.Items?.OfType<InventoryItem>(), want, grp, mem);
            if (fromBank != null)
            {
                InBank(fromBank.Name);
                Bot.Sleep(500);
                if (Equip(Find(Bot.Inventory.Items?.OfType<InventoryItem>(), want, grp, mem)))
                    return Bot.Inventory.Items?.OfType<InventoryItem>()
                        .FirstOrDefault(i => i != null && Bot.Inventory.IsEquipped(i.ID));
            }
            Bot.Sleep(500);
        }

        Alert("Enhancement", $"No {grp} matched: {string.Join(", ", priority)}");
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
        var list = Bot?.Monsters?.MapMonsters ?? Enumerable.Empty<Monster>();
        if (k.MapId.HasValue) list = list.Where(m => m.MapID == k.MapId.Value);
        if (!string.IsNullOrWhiteSpace(k.Name)) list = list.Where(m => m.Name?.Equals(k.Name, StringComparison.OrdinalIgnoreCase) == true);
        if (k.Id.HasValue) list = list.Where(m => m.ID == k.Id.Value);
        return list;
    }

    public bool IsAlive(MonsterKey k) => Match(k).Any(m => m.Alive);

    public bool IsAliveByMapId(int? mapId = null, string? name = null, int? id = null) =>
        IsAlive(new MonsterKey(mapId, name, id));

    void Attack(MonsterKey k)
    {
        if (k.Id.HasValue) Bot.Combat.Attack(k.Id.Value);
        else if (k.MapId.HasValue) Bot.Combat.Attack(k.MapId.Value);
        else if (!string.IsNullOrWhiteSpace(k.Name)) Bot.Combat.Attack(k.Name);
    }

    public void Kill(MonsterKey k)
    {
        if (!IsAlive(k)) return;
        EnsureMonsterSetup(k);
        Attack(k);
        Bot.Sleep(D1);
    }

    public void KillWithPriority(params MonsterKey[] keys)
    {
        var k = keys?.FirstOrDefault(IsAlive);
        if (k is null) { Bot.Sleep(D1); return; }
        EnsureMonsterSetup(k);
        Attack(k);
        Bot.Sleep(D1);
    }

    public void KillUntilDead(MonsterKey k)
    {
        while (!Bot.ShouldExit && IsAlive(k))
        {
            Attack(k);
            Bot.Sleep(D1);
        }
    }

    public void AttackFor(MonsterKey boss, int ms)
    {
        long end = Environment.TickCount64 + ms;
        while (!Bot.ShouldExit && IsAlive(boss) && Environment.TickCount64 < end)
        {
            Attack(boss);
            Bot.Skills.UseSkill(5);
            Bot.Sleep(D1);
        }
    }

    public void Kill(string name) => Kill(MonsterKey.FromName(name));
    public void Kill(params string[] names)
    {
        if (names == null || names.Length == 0) return;
        var ks = names.Where(n => !string.IsNullOrWhiteSpace(n)).Select(MonsterKey.FromName).ToArray();
        if (ks.Length == 0) return;
        KillWithPriority(ks);
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

    // --- helpers ---------------------------------------------------------------

    readonly HashSet<string> _preparedMonsters = new(StringComparer.OrdinalIgnoreCase);

    void MonsterSetup(string monsterName)
    {
        if (string.IsNullOrWhiteSpace(monsterName)) return;
        string m = monsterName.ToLowerInvariant();

        if (m.Contains("ultra chaos harpy") || m.Contains("chaos harpy"))
        {
            Bot.Events.ExtensionPacketReceived += ChaosHarpyListener;
            const string Pot = "Shriekward Potion";
            if (Owned(Pot) < 1) BuyItem("mirrorportal", 774, Pot, 30);
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

        var list = Bot?.Monsters?.MapMonsters ?? Enumerable.Empty<Monster>();
        if (k.Id.HasValue) return list.FirstOrDefault(m => m.ID == k.Id.Value)?.Name;
        if (k.MapId.HasValue) return list.FirstOrDefault(m => m.MapID == k.MapId.Value)?.Name;
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

    int Rank(string name) =>
        string.IsNullOrWhiteSpace(name) ? 0 :
        (Bot?.Reputation?.FactionList ?? new List<Faction>())
            .FirstOrDefault(f => !string.IsNullOrWhiteSpace(f?.Name) &&
                                 name.Equals(f.Name, StringComparison.OrdinalIgnoreCase))
            ?.Rank ?? 0;

    bool Faction(string name, int minRank = 0) => Rank(name) >= Math.Max(0, minRank);

    #endregion

    #region Potions & Scrolls

    public void UsePotion()
    {
        DisableSkills();
        try
        {
            Bot.Sleep(D1);
            Bot.Skills.UseSkill(5);
            Bot.Sleep(D2);
        }
        finally { EnableSkills(); }
    }

    public void GetScrollOfEnrage()
    {
        if (!Faction("SpellCrafting", 5)) return;

        if (Owned("Scroll of Enrage") < 10)
        {
            ForItem("Undead Infantry", "underworld", "Mystic Parchment", 2);
            BuyItem("Zealous Ink", 549, "dragonrune", 5, calculateRemaining: false);

            Join("spellcraft");
            Bot.Send.Packet("%xt%zm%crafting%1%spellOnStart%7%1555%Spell%"); Bot.Sleep(5000);
            Bot.Send.Packet("%xt%zm%crafting%1%spellComplete%7%2330%Enrage%");
            Bot.Drops.Pickup("Scroll of Enrage");
        }
        EquipConsumable("Scroll of Enrage");
    }

    public void GetScrollOfDecay()
    {
        if (!Faction("SpellCrafting", 5)) return;

        while (Owned("Scroll of Decay") < 10)
        {
            ForItem("Undead Infantry", "underworld", "Mystic Parchment", 2);
            BuyItem("Zealous Ink", 549, "dragonrune", 5, calculateRemaining: false);

            Join("spellcraft");
            Bot.Drops.Add("Scroll of Decay");
            Bot.Send.Packet("%xt%zm%crafting%1%spellOnStart%7%1555%Spell%"); Bot.Sleep(5000);
            Bot.Send.Packet("%xt%zm%crafting%1%spellComplete%7%2331%Decay%");
        }

        EquipConsumable("Scroll of Decay");
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

        string Aura(string p) => p switch
        {
            "Might Tonic" => "Might",
            "Sage Tonic" => "Sage",
            _ => p
        };

        foreach (var p in names.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var aura = Aura(p);
            if (HasAura(aura, true)) continue;

            BuyAlchemyPotion(p);

            for (int t = 0; t < 3 && !HasAura(aura, true); t++)
            {
                EquipConsumable(p);
                if (Bot.Inventory.IsEquipped(p))
                {
                    UsePotion();
                    int t0 = Environment.TickCount;
                    while (Environment.TickCount - t0 < 1500 && !HasAura(aura, true)) Bot.Sleep(50);
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

        try
        {
            DisableSkills();
            StopAttack();

            if (Owned(name) < 1) return;
            if (Bot.Inventory.IsEquipped(name)) return;

            WhiteMap();
            Bot.Inventory.EquipUsableItem(name);
            Bot.Sleep(D3);
            EnableSkills();
        }
        catch { }
    }

    #endregion

    #region Best Gear

    public record Gear(string Name, string Group, bool FromBank, double All, double Race);

    public void ChooseBestGear(string names)
    {
        if (Bot?.Monsters?.MapMonsters == null || Bot?.Inventory?.Items == null || Bot?.Bank?.Items == null) return;

        IEnumerable<Monster> PickMonsters(string s)
        {
            var all = Bot.Monsters.MapMonsters ?? Enumerable.Empty<Monster>();
            if (string.IsNullOrWhiteSpace(s) || s == "*") return all;
            var set = new HashSet<string>(
                s.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()),
                StringComparer.OrdinalIgnoreCase);
            return all.Where(m => m?.Name != null && set.Contains(m.Name));
        }

        double Meta(string meta, string key)
        {
            if (string.IsNullOrWhiteSpace(meta)) return 0;
            foreach (var t in meta.Split('\n', '\r', ','))
            {
                var p = t.Split(':'); if (p.Length != 2) continue;
                var k = p[0].Trim();
                if (!(k.Equals(key, StringComparison.OrdinalIgnoreCase) ||
                     (key == "allDmg" && k.Equals("dmgAll", StringComparison.OrdinalIgnoreCase)))) continue;
                return double.TryParse(p[1].Trim(), out var v) ? Math.Max(0, v - 1) : 0;
            }
            return 0;
        }

        IEnumerable<Gear> Items(string race)
        {
            race = string.IsNullOrWhiteSpace(race) || race.Equals("None", StringComparison.OrdinalIgnoreCase) ? "allDmg" : race;
            var valid = new HashSet<string>(new[] { "Weapon", "he", "ba", "co", "pe" });
            var inv = Bot.Inventory.Items ?? Enumerable.Empty<InventoryItem>();
            var bank = Bot.Bank.Items ?? Enumerable.Empty<InventoryItem>();
            var bset = new HashSet<InventoryItem>(bank);
            bool mem = Bot?.Player?.IsMember == true;

            return inv.Concat(bank)
                      .Where(i => i != null &&
                                  !string.IsNullOrWhiteSpace(i.ItemGroup) &&
                                  valid.Contains(i.ItemGroup) &&
                                  (!i.Upgrade || mem))
                      .Select(i => new Gear(i.Name ?? "", i.ItemGroup ?? "", bset.Contains(i),
                                            Meta(i.Meta, "allDmg"), Meta(i.Meta, race)))
                      .Where(g => g.All > 0 || g.Race > 0);
        }

        bool Equipped(string name) =>
            Bot?.Inventory?.Items?.Any(i => i?.Name == name && i.Equipped == true) == true;

        void Equip(Gear g)
        {
            if (string.IsNullOrWhiteSpace(g.Name)) return;
            for (int t = 0; t < 3; t++)
            {
                if (g.FromBank) InBank(g.Name);
                Bot.Inventory.EquipItem(g.Name);
                Bot.Sleep(500);
                if (Equipped(g.Name)) break;
            }
        }

        // --- pick race ------------------------------------------------------------
        string race = PickMonsters(names)
            .Where(m => !string.IsNullOrWhiteSpace(m?.Race))
            .GroupBy(m => m.Race)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(race) || race.Equals("None", StringComparison.OrdinalIgnoreCase))
            race = "allDmg";

        // --- choose best ----------------------------------------------------------
        var items = Items(race).ToList();
        if (items.Count == 0) return;

        var bestAll = items.Where(i => i.All > 0).GroupBy(i => i.Group).Select(g => g.OrderByDescending(x => x.All).First()).ToList();
        var bestRace = items.Where(i => i.Race > 0).GroupBy(i => i.Group).Select(g => g.OrderByDescending(x => x.Race).First()).ToList();

        var combo = (from a in bestAll
                     from r in bestRace
                     where a.Group != r.Group
                     orderby a.All + r.Race descending
                     select (a, r)).FirstOrDefault();

        if (combo.a != null && !string.IsNullOrWhiteSpace(combo.a.Name))
        {
            Equip(combo.a);
            Equip(combo.r);
        }
        else
        {
            Equip(items.OrderByDescending(i => Math.Max(i.Race, i.All)).First());
        }
    }

    #endregion

    #region Shop

    public bool BuyItem(string itemName, int shopId, string map, int quantity = 1, bool calculateRemaining = true, bool skipIfHaveEnough = true, bool considerBank = true)
    {
        bool EnsureMap(string m)
        {
            if (string.IsNullOrWhiteSpace(m)) return true;
            if (Bot?.Map?.Name?.Equals(m, StringComparison.OrdinalIgnoreCase) == true) return true;
            Join(m); return Bot?.Map?.Name?.Equals(m, StringComparison.OrdinalIgnoreCase) == true;
        }

        bool LoadShop(int id)
        {
            if (Bot?.Shops == null) return false;
            for (int a = 0; a < 3 && !Bot.ShouldExit; a++)
            {
                int c0 = Bot.Shops.LoadedCache?.Count ?? 0;
                Bot.Shops.Load(id);
                int t0 = Environment.TickCount;
                while (!Bot.ShouldExit && Environment.TickCount - t0 < 5000)
                {
                    if ((Bot.Shops.Items?.Count ?? 0) > 0) return true;
                    if ((Bot.Shops.LoadedCache?.Count ?? 0) > c0) return true;
                    Bot.Sleep(50);
                }
            }
            return false;
        }

        int Have(string name, bool bank)
        {
            if (bank) InBank(name);
            return Owned(name);
        }

        int Need(string name, int want)
        {
            int cur = Have(name, considerBank);
            if (skipIfHaveEnough && cur >= want) return 0;
            return calculateRemaining ? Math.Max(0, want - cur) : want;
        }

        ShopItem Find(string name) =>
            Bot?.Shops?.Items?.FirstOrDefault(i => i?.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) == true);

        // ---- guards ----------------------------------------------------------------
        if (string.IsNullOrWhiteSpace(itemName) || quantity <= 0) return false;
        if (!EnsureMap(map)) { Alert("Shop", $"Failed to join {map}"); return false; }
        if (!LoadShop(shopId)) { Alert("Shop", $"Failed to load shop {shopId}"); return false; }

        int need = Need(itemName, quantity);
        if (need == 0) return true;

        var it = Find(itemName);
        if (it == null) { Alert("Shop", $"Item not found: {itemName}"); return false; }

        long cost = (long)it.Cost * need;
        if (Bot.Player.Gold < cost) { Alert("Shop", $"Gold needed {cost}, have {Bot.Player.Gold}"); return false; }
        if (Bot.Player.Level < it.Level) { Alert("Shop", $"Level {it.Level}+ required"); return false; }
        if (Bot.Inventory.FreeSlots <= 0) { Alert("Shop", "No inventory space"); return false; }

        // ---- buy + confirm ---------------------------------------------------------
        int before = Have(itemName, bank: false);
        Bot.Shops.BuyItem(it.ID, it.ShopItemID, need);

        int t0c = Environment.TickCount;
        while (Environment.TickCount - t0c < 2000) { if (Owned(itemName) > before) break; Bot.Sleep(50); }

        int gained = Owned(itemName) - before;
        bool ok = gained > 0;
        Alert("Shop", ok ? $"Purchased {gained}x {itemName}" : $"Purchase failed: {itemName}");
        return ok;
    }

    #endregion

    #region Drops

    bool HasDrop(object key) => key switch
    {
        int id when id > 0 =>
            Bot?.Drops?.CurrentDropInfos?.Any(i => i?.ID == id) == true,

        string s when !string.IsNullOrWhiteSpace(s) =>
            (Bot?.Drops?.CurrentDrops?.Any(d => string.Equals(d, s, StringComparison.OrdinalIgnoreCase)) == true) ||
            (Bot?.Drops?.CurrentDropInfos?.Any(i => string.Equals(i?.Name, s, StringComparison.OrdinalIgnoreCase)) == true),

        _ => false
    };

    ItemBase GetDropItem(object key) => key switch
    {
        int id when id > 0 =>
            Bot?.Drops?.CurrentDropInfos?.FirstOrDefault(i => i?.ID == id),

        string s when !string.IsNullOrWhiteSpace(s) =>
            Bot?.Drops?.CurrentDropInfos?.FirstOrDefault(i => string.Equals(i?.Name, s, StringComparison.OrdinalIgnoreCase)),

        _ => null
    };

    void Pickup(params object[] keys)
    {
        if (keys == null || keys.Length == 0) return;
        foreach (var k in keys.Where(HasDrop))
        {
            if (k is int id) Bot.Drops.Pickup(id);
            else if (k is string s) Bot.Drops.Pickup(s);
            Bot.Sleep(D1);
        }
    }

    bool WaitForDrop(object key, int timeout = 30000)
    {
        if (key is not (int or string)) return false;
        var t0 = Environment.TickCount;
        while (!Bot.ShouldExit && !HasDrop(key) && Environment.TickCount - t0 < timeout)
            Bot.Sleep(D1);
        return HasDrop(key);
    }

    bool HasAny(params object[] keys) =>
        keys?.Any(HasDrop) == true;

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

        if (Bot.Map.Name?.Equals(mapName, StringComparison.OrdinalIgnoreCase) == true) return;

        while (!Bot.ShouldExit && Bot.Map.Name?.Equals(mapName, StringComparison.OrdinalIgnoreCase) != true)
        {
            StopAttack();
            try
            {
                Bot.Send.Packet($"%xt%zm%cmd%{Bot.Map.RoomID}%tfer%{Bot.Player.Username}%{target}%{cell}%{pad}%");
                Bot.Wait.ForMapLoad(mapName);
            }
            catch { break; }
        }
        ResetMonsterSetupCache();
    }

    public void ChooseBestCell(string monsterNames, bool alt = false, string setCell = null, string setPad = "Spawn")
    {
        if (Bot?.Monsters?.MapMonsters == null || Bot?.Map?.Cells == null || Bot?.Player == null) return;

        var names = string.IsNullOrWhiteSpace(monsterNames)
            ? Array.Empty<string>()
            : monsterNames.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(s => s.Trim())
                          .Where(s => s.Length > 0)
                          .ToArray();

        bool wildcard = names.Length == 0 || (names.Length == 1 && names[0] == "*");
        string pad = string.IsNullOrWhiteSpace(setPad) ? "Left" : setPad;

        var mons = Bot.Monsters.MapMonsters
            .Where(m => m != null && !string.IsNullOrWhiteSpace(m.Cell) &&
                        (wildcard || names.Any(n => string.Equals(m.Name, n, StringComparison.OrdinalIgnoreCase))))
            .ToList();
        if (mons.Count == 0) return;

        string target = !string.IsNullOrWhiteSpace(setCell) ? setCell
                     : alt ? mons[0].Cell
                     : mons.GroupBy(m => m.Cell, StringComparer.OrdinalIgnoreCase)
                           .OrderByDescending(g => g.Count())
                           .First().Key;

        var cells = (Bot.Map.Cells as IEnumerable<string>) ?? Array.Empty<string>();
        if (!cells.Contains(target)) return;

        _bestCell = target;
        _bestPad = pad;

        if (!string.Equals(Bot.Player.Cell, target, StringComparison.Ordinal))
        {
            try { Bot.Map.Jump(target, pad); Bot.Wait.ForCellChange(target); Bot.Player.SetSpawnPoint(); }
            catch { }
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
        if (Bot?.Map?.PlayerNames?.Count == 0) return 100.0;

        double lowest = 100.0;

        foreach (var playerName in Bot.Map.PlayerNames.Where(p => !string.IsNullOrWhiteSpace(p)))
        {
            try
            {
                int hp = Bot.Flash.GetGameObject<int>($"world.uoTree.{playerName}.intHP");
                int maxHp = Bot.Flash.GetGameObject<int>($"world.uoTree.{playerName}.intHPMax");

                if (maxHp > 0 && hp >= 0)
                    lowest = Math.Min(lowest, (double)hp / maxHp * 100.0);
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
        if (IsManaLow(30) || !HasAura("Cryostasis"))
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
        if (IsManaLow(40))
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
        Alert("Rotation", $"{className} set to {mode} mode");
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
        if (string.IsNullOrWhiteSpace(excludeAura)) return false;

        var auras = GetAuras(self);
        return auras.Any(a => a != null &&
                            !string.IsNullOrWhiteSpace(a.Name) &&
                            !excludeAura.Equals(a.Name, StringComparison.OrdinalIgnoreCase));
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
        const string USED_THRESHOLDS_KEY = "warden.usedThresholds";

        if (Bot?.Combat == null || Bot?.Player == null) return;

        Bot.Combat.Attack("Ultra Warden");
        var t = Bot.Player.Target;
        if (t?.HP == null || t.HP <= 0 || t.MaxHP <= 0) return;

        int currentHp = t.HP;
        int maxHp = t.MaxHP;
        int currentThreshold = (currentHp * 100) / maxHp;
        int thresholdBand = (currentThreshold / 5) * 5;

        HashSet<int> usedThresholds;
        var usedObj = AppDomain.CurrentDomain.GetData(USED_THRESHOLDS_KEY);
        usedThresholds = usedObj as HashSet<int> ?? new HashSet<int>();

        if (!usedThresholds.Contains(thresholdBand))
        {
            double percentRemaining = ((double)currentHp / maxHp) * 100;
            usedThresholds.Add(thresholdBand);
            AppDomain.CurrentDomain.SetData(USED_THRESHOLDS_KEY, usedThresholds);

            while (MonsterAlive("Ultra Warden") && !HasAura("Focus") && !Bot.ShouldExit)
                UsePotion();
        }

        Bot.Sleep(150);
    }

    public void DrakathTaunter()
    {
        const string LAST_THR_KEY = "drakath.lastThreshold";
        const string PREV_HP_KEY = "drakath.prevHp";
        const string LAST_FIRE_KEY = "drakath.lastFireTicks";

        if (Bot?.Combat == null || Bot?.Player == null) return;

        var bands = new (int thr, int rng)[] {
            (18_000_000, 180_000), (16_000_000, 180_000), (14_000_000, 180_000),
            (12_000_000, 180_000), (10_000_000, 180_000), ( 8_000_000, 100_000),
            ( 6_000_000, 100_000), ( 4_000_000, 100_000), ( 2_000_000, 100_000)
        };

        EnsureDrakathTarget();

        var t = Bot.Player.Target;
        if (t?.HP == null || t.HP <= 0) return;

        int lastThreshold = AppDomain.CurrentDomain.GetData(LAST_THR_KEY) as int? ?? int.MaxValue;
        int prevHp = AppDomain.CurrentDomain.GetData(PREV_HP_KEY) as int? ?? int.MaxValue;
        long lastFireTicks = AppDomain.CurrentDomain.GetData(LAST_FIRE_KEY) as long? ?? 0L;

        int hp = t.HP;
        long nowTicks = DateTime.UtcNow.Ticks;
        bool cooldownOver = (new TimeSpan(nowTicks - lastFireTicks).TotalMilliseconds >= 1200); // 1.2s debounce

        var band = Array.FindLast(bands, b =>
            b.thr < lastThreshold &&
            prevHp > (b.thr + b.rng) &&
            hp <= (b.thr + b.rng));

        if (cooldownOver && band != default)
        {
            Alert("Drakath", $"Crossed into band {band.thr:N0} (hp now {hp:N0}). Attempting Focus...");
            AppDomain.CurrentDomain.SetData(LAST_THR_KEY, band.thr);
            AppDomain.CurrentDomain.SetData(LAST_FIRE_KEY, nowTicks);

            var end = DateTime.UtcNow.AddMilliseconds(1800);
            int tries = 0;
            while (MonsterAlive("Champion Drakath") && !Bot.ShouldExit && DateTime.UtcNow < end)
            {
                EnsureDrakathTarget();
                UsePotion();
                tries++;
                if (HasAura("Focus")) break;
                Bot.Sleep(120);
            }

            if (HasAura("Focus"))
                Alert("Drakath", $"Focus obtained at {Bot.Player.Target?.HP:N0} HP (tries: {tries})");
            else
                Alert("Drakath", $"Warning: Failed to get Focus (tries: {tries}, hp now {Bot.Player.Target?.HP:N0})");
        }

        AppDomain.CurrentDomain.SetData(PREV_HP_KEY, hp);

        Bot.Sleep(120);
    }

    private void EnsureDrakathTarget()
    {
        var list =
            Bot?.Monsters?.CurrentMonsters
            ?? Bot?.Monsters?.MapMonsters
            ?? Enumerable.Empty<Skua.Core.Models.Monsters.Monster>();

        var drak = list.FirstOrDefault(m => m != null && m.Name == "Champion Drakath" && m.Alive);

        if (drak != null)
        {
            Bot.Combat.Attack(drak.MapID);
            return;
        }

        Bot.Combat.Attack("Champion Drakath");
    }

    // --- helpers ---------------------------------------------------

    public void WaitForArmy(int quantity, int bufferTimeMs = 3000, int tickMs = 500, int timeoutMs = 0)
    {
        if (Bot?.Map == null) return;

        int required = Math.Max(1, quantity) + 1;
        var sw = Stopwatch.StartNew();

        while (!Bot.ShouldExit &&
               Bot.Map.PlayerCount < required &&
               (timeoutMs <= 0 || sw.ElapsedMilliseconds < timeoutMs))
        {
            int others = Math.Max(0, Bot.Map.PlayerCount - 1);
            Alert("Army", $"Waiting for army: {others}/{quantity} players ready");

            if (!IsManaLow(50))
            {
                Bot.Skills.UseSkill(1);
                Bot.Skills.UseSkill(2);
                Bot.Skills.UseSkill(3);
                Bot.Player.Rest(true);
            }

            Bot.Sleep(tickMs);
        }

        if (Bot.ShouldExit) return;

        Alert("Army", $"All players ready ({Bot.Map.PlayerCount}), final buffing...");
        if (!IsManaLow(50))
        {
            Bot.Skills.UseSkill(1);
            Bot.Skills.UseSkill(2);
            Bot.Skills.UseSkill(3);
            Bot.Player.Rest(true);
        }

        Alert("Army", $"Waiting {bufferTimeMs}ms for coordination...");
        Bot.Sleep(bufferTimeMs);
        Alert("Army", "Ready to proceed!");
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

    public bool Alert(string category, string message)
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