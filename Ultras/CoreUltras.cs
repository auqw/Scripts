using System;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
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

        if (Bot.Bank.Items is null)
            Bot.Bank.Load();
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

    string[] ParseNames(string monsters)
    {
        return monsters?.Split('|', StringSplitOptions.RemoveEmptyEntries)
               ?? Array.Empty<string>();
    }

    private void ForItemCore(string monsters, string map, int quantity, bool isTemp, bool useBestGear, bool alt, string? cell, string pad, bool priority, Action ensureInBank, Func<int> ownedCount, Action pickup, string itemLabel)
    {
        if (quantity <= 0) return;

        if (!string.IsNullOrWhiteSpace(map))
            Join(map);

        ChooseBestCell(monsters, alt, cell, pad);
        if (useBestGear) ChooseBestGear(monsters);
        var m = ParseNames(monsters) ?? Array.Empty<string>();
        ensureInBank();
        if (ownedCount() >= quantity) return;
        Alert("FARMING", $"Killing {monsters} for {quantity}x {itemLabel}");
        EnableSkills();
        var i = 0;
        while (!Bot.ShouldExit)
        {
            if (_chargeDetected) UsePotion();
            if (ownedCount() >= quantity)
            {
                Alert("SUCCESS", $"Acquired {quantity}x {itemLabel}");
                DisableSkills();
                StopAttack();
                return;
            }
            pickup();
            if (priority)
            {
                if (m.Length > 0)
                    KillWithPriority(m.Select(MonsterKey.FromName).ToArray());
            }
            else
            {
                if (m.Length > 0)
                {
                    Kill(m[i % m.Length]);
                    i++;
                }
            }
        }
    }

    public void ForItem(string monsters, string map, string name, int quantity = 1, bool isTemp = false, bool useBestGear = false, bool alt = false, string? cell = null, string pad = "Left", bool priority = false)
    {
        if (string.IsNullOrWhiteSpace(name) || quantity <= 0) return;
        ForItemCore(
            monsters, map, quantity, isTemp, useBestGear, alt, cell, pad, priority,
            ensureInBank: () => { if (!isTemp) InBank(name); },
            ownedCount: () => Owned(name, isTemp),
            pickup: () => PickupItems(name),
            itemLabel: name
        );
    }

    public void ForItem(string monsters, string map, int itemId, int quantity = 1, bool isTemp = false, bool useBestGear = false, bool alt = false, string? cell = null, string pad = "Left", bool priority = false)
    {
        if (itemId <= 0 || quantity <= 0) return;
        var itemLabel = GetDropItem(itemId)?.Name ?? $"Item#{itemId}";
        ForItemCore(
            monsters, map, quantity, isTemp, useBestGear, alt, cell, pad, priority,
            ensureInBank: () => { if (!isTemp) InBank(itemId); },
            ownedCount: () => Owned(itemId, isTemp),
            pickup: () => PickupItems(itemId),
            itemLabel: itemLabel
        );
    }

    public void EquipBestClass(List<(string name, int rank)> priorities)
    {
        if (priorities == null || priorities.Count == 0) return;

        string currentClassName = GetCurrentClassName();
        bool isMaxRank = IsCurrentClassMaxRank();

        foreach (var (name, rank) in priorities)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name) || Owned(name) < 1) continue;

                if (HasClassEquipped(name))
                {
                    if (isMaxRank || Bot.Player.CurrentClassRank >= rank) return;
                }

                if (!Bot.Inventory.IsEquipped(name))
                {
                    Bot.Inventory.EquipItem(name);
                    Bot.Sleep(D3);
                    return;
                }
            }
            catch { continue; }
        }
    }

    public void EquipBestClass(List<(int id, int rank)> priorities)
    {
        if (priorities == null || priorities.Count == 0) return;

        string currentClassName = GetCurrentClassName();
        bool isMaxRank = IsCurrentClassMaxRank();

        foreach (var (id, rank) in priorities)
        {
            try
            {
                if (id <= 0 || Owned(id) < 1) continue;

                var item = Bot.Inventory.Items.FirstOrDefault(i => i.ID == id);
                if (item == null) continue;

                if (HasClassEquipped(item.Name))
                {
                    if (isMaxRank || Bot.Player.CurrentClassRank >= rank) return;
                }

                if (!Bot.Inventory.IsEquipped(id))
                {
                    Bot.Inventory.EquipItem(id);
                    Bot.Sleep(D3);
                    return;
                }
            }
            catch { continue; }
        }
    }

    public bool InBank(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        if (!Bot.Bank.Contains(name)) return false;

        try
        {
            int quantity = Bot.Bank.GetQuantity(name);
            StopAttack();

            Bot.Bank.ToInventory(name);
            Bot.Sleep(D2);
            return true;
        }
        catch { return false; }
    }

    public bool InBank(int id)
    {
        if (id <= 0) return false;
        if (!Bot.Bank.Contains(id)) return false;

        try
        {
            int quantity = Bot.Bank.GetQuantity(id);
            var item = Bot.Bank.Items.FirstOrDefault(i => i.ID == id);
            string itemName = item?.Name ?? $"Item#{id}";

            StopAttack();

            Bot.Bank.ToInventory(id);
            Bot.Sleep(D2);
            return true;
        }
        catch { return false; }
    }

    public bool ToBank(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        if (!Bot.Inventory.Contains(name)) return false;

        try
        {
            int quantity = Bot.Inventory.GetQuantity(name);
            StopAttack();

            Bot.Inventory.ToBank(name);
            Bot.Sleep(D2);
            return true;
        }
        catch { return false; }
    }

    public bool ToBank(int id)
    {
        if (id <= 0) return false;
        if (!Bot.Inventory.Contains(id)) return false;

        try
        {
            int quantity = Bot.Inventory.GetQuantity(id);
            var item = Bot.Inventory.Items.FirstOrDefault(i => i.ID == id);
            string itemName = item?.Name ?? $"Item#{id}";

            StopAttack();

            Bot.Inventory.ToBank(id);
            Bot.Sleep(D2);
            return true;
        }
        catch { return false; }
    }

    public int Owned(string name, bool isTemp = false)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name)) return 0;
            if (isTemp) return Bot.TempInv?.GetQuantity(name) ?? 0;
            return Bot.Inventory?.GetQuantity(name) ?? 0;
        }
        catch { return 0; }
    }

    public int Owned(int id, bool isTemp = false)
    {
        try
        {
            if (id <= 0) return 0;
            if (isTemp) return Bot.TempInv?.GetQuantity(id) ?? 0;
            return Bot.Inventory?.GetQuantity(id) ?? 0;
        }
        catch { return 0; }
    }

    #endregion

    #region Find Item by Enhancement

    string EnhancementName(int id) => id switch
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

    bool EnhancementIs(InventoryItem i, string name) =>
        EnhancementName(i?.EnhancementPatternID ?? -1)?
            .Equals(name, StringComparison.OrdinalIgnoreCase) == true;

    InventoryItem EnsureItemWithEnhancement(int patternId)
    {
        var inv = Bot.Inventory.Items?.OfType<InventoryItem>() ?? Enumerable.Empty<InventoryItem>();
        var hit = inv.FirstOrDefault(i => i?.EnhancementPatternID == patternId);
        if (hit != null) return hit;

        var bank = Bot.Bank.Items?.OfType<InventoryItem>() ?? Enumerable.Empty<InventoryItem>();
        var fromBank = bank.FirstOrDefault(i => i?.EnhancementPatternID == patternId);
        if (fromBank == null) return null;

        ToBank(fromBank.Name);

        inv = Bot.Inventory.Items?.OfType<InventoryItem>() ?? Enumerable.Empty<InventoryItem>();
        return inv.FirstOrDefault(i => i?.EnhancementPatternID == patternId);
    }

    InventoryItem EnsureItemWithEnhancement(string enhancementName)
    {
        if (string.IsNullOrWhiteSpace(enhancementName)) return null;

        var inv = Bot.Inventory.Items?.OfType<InventoryItem>() ?? Enumerable.Empty<InventoryItem>();
        var hit = inv.FirstOrDefault(i => EnhancementIs(i, enhancementName));
        if (hit != null) return hit;

        var bank = Bot.Bank.Items?.OfType<InventoryItem>() ?? Enumerable.Empty<InventoryItem>();
        var fromBank = bank.FirstOrDefault(i => EnhancementIs(i, enhancementName));
        if (fromBank == null) return null;

        ToBank(fromBank.Name);

        inv = Bot.Inventory.Items?.OfType<InventoryItem>() ?? Enumerable.Empty<InventoryItem>();
        return inv.FirstOrDefault(i => EnhancementIs(i, enhancementName));
    }

    #endregion

    #region Combat

    public record MonsterKey(int? MapId = null, string? Name = null, int? Id = null)
    {
        public static MonsterKey FromName(string name) => new(Name: name);
        public static MonsterKey FromId(int id) => new(Id: id);
        public static MonsterKey FromMapId(int mapId) => new(MapId: mapId);
    }

    private bool IsAliveByMapId(int? mapId = null, string? name = null, int? id = null)
    {
        var monsters = Bot.Monsters.MapMonsters ?? Enumerable.Empty<Monster>();

        if (mapId.HasValue)
            monsters = monsters.Where(m => m.MapID == mapId.Value);
        if (name != null)
            monsters = monsters.Where(m => m.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) == true);
        if (id.HasValue)
            monsters = monsters.Where(m => m.ID == id.Value);

        return monsters.Any(m => m.Alive);
    }

    private bool IsAlive(MonsterKey key)
        => IsAliveByMapId(key.MapId, key.Name, key.Id);

    public void Attack(MonsterKey key)
    {
        if (key.Id.HasValue) Bot.Combat.Attack(key.Id.Value);
        else if (key.MapId.HasValue) Bot.Combat.Attack(key.MapId.Value);
        else if (key.Name != null) Bot.Combat.Attack(key.Name);
    }

    public void Kill(MonsterKey key)
    {
        if (!IsAlive(key)) return;
        EnsureMonsterSetup(key);
        Attack(key);
        Bot.Sleep(D1);
    }

    public void KillWithPriority(params MonsterKey[] keys)
    {
        foreach (var k in keys)
        {
            if (IsAlive(k))
            {
                EnsureMonsterSetup(k);
                Attack(k);
                Bot.Sleep(D1);
                return;
            }
        }
        Bot.Sleep(D1);
    }

    // --- overloads ---------------------------------------------------------------

    public void Kill(string name)
        => Kill(MonsterKey.FromName(name));

    public void Kill(params string[] names)
    {
        if (names == null || names.Length == 0) return;
        var keys = names
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(MonsterKey.FromName)
            .ToArray();

        if (keys.Length == 0) return;
        KillWithPriority(keys);
    }

    public void Kill(int id)
        => Kill(MonsterKey.FromId(id));

    public void KillAtMapId(int mapId)
        => Kill(MonsterKey.FromMapId(mapId));

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

    private readonly HashSet<string> _preparedMonsters = new(StringComparer.OrdinalIgnoreCase);

    private void MonsterSetup(string monsterName)
    {
        if (string.IsNullOrWhiteSpace(monsterName)) return;

        string m = monsterName.ToLowerInvariant();

        if (m.Contains("ultra chaos harpy") || m.Contains("chaos harpy"))
            SetupChaosHarpy();
        else if (m.Contains("ultra xiang") || m.Contains("chaos lord xiang"))
            SetupChaosXiang();
        else if (m.Contains("doomkitten"))
            SetupDoomKitten();

        void SetupChaosHarpy()
        {
            Bot.Events.ExtensionPacketReceived += ChaosHarpyListener;
            const string Pot = "Shriekward Potion";
            if (Owned(Pot) < 1) BuyItem("mirrorportal", 774, Pot, 30);
            EquipConsumable(Pot);
        }

        void SetupChaosXiang()
        {
            var classes = new List<(string name, int rank)>
            {
                ("Dragon of Time", 10),
                ("Healer (Rare)", 1),
                ("Healer", 1)
            };
            EquipBestClass(classes);
        }

        void SetupDoomKitten()
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

    private string? ResolveMonsterName(MonsterKey key)
    {
        if (!string.IsNullOrWhiteSpace(key.Name))
            return key.Name;

        var list = Bot.Monsters.MapMonsters ?? Enumerable.Empty<Monster>();
        if (key.Id.HasValue)
            return list.FirstOrDefault(m => m.ID == key.Id.Value)?.Name;
        if (key.MapId.HasValue)
            return list.FirstOrDefault(m => m.MapID == key.MapId.Value)?.Name;

        return null;
    }

    private void EnsureMonsterSetup(MonsterKey key)
    {
        var name = ResolveMonsterName(key);
        if (string.IsNullOrWhiteSpace(name)) return;

        if (_preparedMonsters.Add(name))
            MonsterSetup(name);
    }

    private void ResetMonsterSetupCache() => _preparedMonsters.Clear();

    #endregion

    #region Factions

    public List<Faction> GetAllFactions()
    {
        return Bot.Reputation.FactionList;
    }

    public int FactionRank(string name)
    {
        var faction = Bot.Reputation.FactionList.FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return faction?.Rank ?? 0;
    }

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
        if (FactionRank("SpellCrafting") < 5) return;

        if (Owned("Scroll of Enrage") < 10)
        {
            ForItem("Undead Infantry", "underworld", "Mystic Parchment", 2);
            Join("dragonrune");
            Bot.Shops.Load(549);
            if (Owned("Zealous Ink") < 1)
                Bot.Shops.BuyItem(13286, 1639, 5);

            Join("spellcraft");
            Bot.Send.Packet("%xt%zm%crafting%1%spellOnStart%7%1555%Spell%"); Bot.Sleep(5000);
            Bot.Send.Packet("%xt%zm%crafting%1%spellComplete%7%2330%Enrage%"); Bot.Sleep(D3);
            Bot.Drops.Pickup("Scroll of Enrage");
        }
        EquipConsumable("Scroll of Enrage");
    }

    public void GetScrollOfDecay()
    {
        if (!Bot.Reputation.HasRank("SpellCrafting", 5)) return;

        while (Owned("Scroll of Decay") < 10)
        {
            ForItem("Undead Infantry", "underworld", "Mystic Parchment", 2);
            Join("dragonrune");
            Bot.Shops.Load(549);
            if (Owned("Zealous Ink") < 1)
                Bot.Shops.BuyItem("Zealous Ink", 5);

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

    public void UseAlchemyPotions(params string[] potionNames)
    {
        if (potionNames == null || potionNames.Length == 0) return;
        foreach (string potion in potionNames)
        {
            if (string.IsNullOrWhiteSpace(potion)) continue;
            Alert("DEBUG", $"Checking potion: {potion}");
            if (HasAura(potion, self: true))
            {
                Alert("DEBUG", $"{potion} aura already active, skipping");
                continue;
            }
            try
            {
                Alert("DEBUG", $"Buying {potion}");
                BuyAlchemyPotion(potion);
                Alert("DEBUG", $"Equipping {potion}");
                EquipConsumable(potion);
                if (Bot.Inventory.IsEquipped(potion))
                {
                    Alert("DEBUG", $"Using {potion}");
                    UsePotion();
                    Bot.Sleep(1000); //* Add a small delay*
                }
                else
                {
                    Alert("DEBUG", $"Failed to equip {potion}");
                }
            }
            catch (Exception ex)
            {
                Alert("ERROR", $"Exception with {potion}: {ex.Message}");
            }
        }
    }

    public void BuyAlchemyPotion(string name)
    {
        if (Owned(name) >= 1) return;

        switch (name)
        {
            case "Might Tonic":
                /*Join("alchemyacademy");
                Bot.Shops.Load(2036);
                if (Owned(61043) < 2)
                    Bot.Shops.BuyItem(61043, 8421, 2);
                if (Owned(11623) < 1)
                    Bot.Shops.BuyItem(11623, 8798, 10);*/
                BuyItem("alchemyacademy", 2036, "Gold Voucher 500k", 2);
                BuyItem("alchemyacademy", 2036, "Might Tonic", 10, calculateRemaining: false);
                break;
            case "Sage Tonic":
                /*Join("alchemyacademy");
                Bot.Shops.Load(2036);
                if (Owned(61043) < 2)
                    Bot.Shops.BuyItem(61043, 8421, 2);
                if (Owned(11635) < 1)
                    Bot.Shops.BuyItem(11635, 8800, 10);*/
                BuyItem("alchemyacademy", 2036, "Gold Voucher 500k", 2);
                BuyItem("alchemyacademy", 2036, "Sage Tonic", 10, calculateRemaining: false);
                break;
            case "Potent Malevolence Elixir":
                /*Join("alchemyacademy");
                Bot.Shops.Load(2036);
                if (Owned(61043) < 4)
                    Bot.Shops.BuyItem(61043, 8421, 4);
                if (Owned(11745) < 1)
                    Bot.Shops.BuyItem(11745, 9825, 8);*/
                BuyItem("alchemyacademy", 2036, "Gold Voucher 500k", 4);
                BuyItem("alchemyacademy", 2036, "Potent Malevolence Elixir", 8, calculateRemaining: false);
                break;
            case "Potent Battle Elixir":
                /*Join("alchemyacademy");
                Bot.Shops.Load(2036);
                if (Owned(61043) < 4)
                    Bot.Shops.BuyItem(61043, 8421, 4);
                if (Owned(11741) < 1)
                    Bot.Shops.BuyItem(11741, 9824, 8);*/
                BuyItem("alchemyacademy", 2036, "Gold Voucher 500k", 4);
                BuyItem("alchemyacademy", 2036, "Potent Battle Elixir", 8, calculateRemaining: false);
                break;
            case "Potent Honor Potion":
                /*Join("alchemyacademy");
                Bot.Shops.Load(2036);
                if (Owned(61043) < 1)
                    Bot.Shops.BuyItem(61043, 8421, 1);
                if (Owned(11736) < 1)
                    Bot.Shops.BuyItem(11736, 8826, 5);*/
                BuyItem("alchemyacademy", 2036, "Gold Voucher 500k");
                BuyItem("alchemyacademy", 2036, "Potent Honor Potion", 5, calculateRemaining: false);
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

            ToSafeMap();
            Bot.Inventory.EquipUsableItem(name);
            Bot.Sleep(D3);
            EnableSkills();
        }
        catch { }
    }

    #endregion

    #region Best Gear

    public void ChooseBestGear(string monsterNames)
    {
        if (Bot?.Monsters == null || Bot?.Inventory == null || Bot?.Bank == null) return;

        var monsters = GetMonsters(monsterNames).Where(m => m?.Race != null).ToList();
        if (!monsters.Any()) return;

        string race = monsters.GroupBy(m => m.Race)
                             .OrderByDescending(g => g.Count())
                             .First().Key;

        if (race?.Equals("None", StringComparison.OrdinalIgnoreCase) == true)
            race = "allDmg";

        var items = GetItems(race).ToList();
        if (!items.Any()) return;

        var combo = items.Where(a => a.All > 0)
                        .SelectMany(a => items.Where(r => r.Race > 0 && r.Group != a.Group)
                                              .Select(r => (a, r, Total: a.All + r.Race)))
                        .OrderByDescending(x => x.Total)
                        .FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(combo.a.Name))
        {
            EquipGear(combo.a);
            EquipGear(combo.r);
        }
        else
        {
            var bestItem = items.OrderByDescending(i => Math.Max(i.Race, i.All)).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(bestItem.Name))
            {
                EquipGear(bestItem);
            }
        }
    }

    IEnumerable<Monster> GetMonsters(string names)
    {
        if (Bot?.Monsters?.MapMonsters == null) return Enumerable.Empty<Monster>();

        if (string.IsNullOrEmpty(names) || names == "*")
            return Bot.Monsters.MapMonsters;

        var nameArray = names.Split(',', StringSplitOptions.RemoveEmptyEntries)
                             .Select(n => n?.Trim())
                             .Where(n => !string.IsNullOrWhiteSpace(n))
                             .ToArray();

        if (nameArray.Length == 0) return Bot.Monsters.MapMonsters;

        return Bot.Monsters.MapMonsters.Where(m => m?.Name != null &&
            nameArray.Any(n => n.Equals(m.Name, StringComparison.OrdinalIgnoreCase)));
    }

    IEnumerable<(string Name, string Group, bool FromBank, double All, double Race)> GetItems(string race)
    {
        if (Bot?.Inventory?.Items == null || Bot?.Bank?.Items == null)
            return Enumerable.Empty<(string, string, bool, double, double)>();

        if (string.IsNullOrWhiteSpace(race)) race = "allDmg";

        var validGroups = new[] { "Weapon", "he", "ba", "co", "pe" };

        var inventoryItems = Bot.Inventory.Items ?? Enumerable.Empty<InventoryItem>();
        var bankItems = Bot.Bank.Items ?? Enumerable.Empty<InventoryItem>();

        return inventoryItems.Concat(bankItems)
            .Where(i => i != null &&
                       !string.IsNullOrWhiteSpace(i.ItemGroup) &&
                       validGroups.Contains(i.ItemGroup) &&
                       (!i.Upgrade || Bot?.Player?.IsMember == true))
            .Select(i => (
                Name: i.Name ?? "",
                Group: i.ItemGroup ?? "",
                FromBank: bankItems.Contains(i),
                All: ParseMeta(i.Meta, "allDmg"),
                Race: ParseMeta(i.Meta, race)
            ))
            .Where(x => x.All > 0 || x.Race > 0);
    }

    double ParseMeta(string meta, string key)
    {
        if (string.IsNullOrWhiteSpace(meta) || string.IsNullOrWhiteSpace(key)) return 0;

        try
        {
            return meta.Split('\n', '\r')
                      .SelectMany(line => line.Split(','))
                      .Where(pair => !string.IsNullOrWhiteSpace(pair) && pair.Contains(':'))
                      .Select(pair => pair.Split(':'))
                      .Where(parts => parts.Length == 2 &&
                             (parts[0]?.Trim()?.Equals(key, StringComparison.OrdinalIgnoreCase) == true ||
                              (key == "allDmg" && parts[0]?.Trim()?.Equals("dmgAll", StringComparison.OrdinalIgnoreCase) == true)))
                      .Select(parts => double.TryParse(parts[1]?.Trim(), out var v) ? Math.Max(0, v - 1) : 0)
                      .FirstOrDefault();
        }
        catch { return 0; }
    }

    void EquipGear((string Name, string Group, bool FromBank, double All, double Race) item)
    {
        if (string.IsNullOrWhiteSpace(item.Name)) return;

        if (item.FromBank) InBank(item.Name);

        // Bot.Inventory.EquipItem(item.Name);
    }

    #endregion

    #region Shop

    public bool BuyItem(string itemName, int shopId, string map, int quantity = 1, bool calculateRemaining = true, bool skipIfHaveEnough = true, bool considerBank = true)
    {
        if (string.IsNullOrWhiteSpace(itemName) || quantity <= 0)
        {
            Alert("Shop", $"Invalid parameters: itemName='{itemName}', quantity={quantity}");
            return false;
        }

        try
        {
            if (!string.IsNullOrWhiteSpace(map) && !JoinMapSafely(map))
            {
                Alert("Shop", $"Failed to join map: {map}");
                return false;
            }

            if (!LoadShopSafely(shopId))
            {
                Alert("Shop", $"Failed to load shop: {shopId}");
                return false;
            }

            if (considerBank) BankItemSafely(itemName);

            int current = GetCurrentQuantity(itemName);

            // Check if we already have enough (applies to both calculateRemaining modes)
            if (skipIfHaveEnough && current >= quantity)
            {
                Alert("Shop", $"Already have enough {itemName}: {current}/{quantity}");
                return true;
            }

            int buyQuantity = quantity;
            if (calculateRemaining)
            {
                buyQuantity = Math.Max(0, quantity - current);
                Alert("Shop", $"Have {current}, need {quantity}, buying {buyQuantity}");
                if (buyQuantity == 0) return true;
            }
            else
            {
                Alert("Shop", $"Buying exact quantity: {quantity} (have {current})");
            }

            var item = GetValidatedShopItem(itemName);
            if (item == null)
            {
                Alert("Shop", $"Item not found in shop: {itemName}");
                return false;
            }

            Alert("Shop", $"Found item: {item.Name} (Cost: {item.Cost})");

            if (!ValidatePurchaseRequirements(item, buyQuantity))
            {
                Alert("Shop", $"Purchase requirements not met");
                return false;
            }

            return ExecutePurchase(item, buyQuantity);
        }
        catch (Exception ex)
        {
            Alert("Shop", $"Exception: {ex.Message}");
            return false;
        }
    }

    private bool JoinMapSafely(string map)
    {
        if (Bot?.Map?.Name?.Equals(map, StringComparison.OrdinalIgnoreCase) == true) return true;
        Join(map);
        Bot.Sleep(D4);
        return Bot?.Map?.Name?.Equals(map, StringComparison.OrdinalIgnoreCase) == true;
    }

    private bool LoadShopSafely(int shopId)
    {
        Bot.Shops.Load(shopId);
        Bot.Sleep(D4);
        bool loaded = Bot?.Shops?.Items?.Any() == true;
        Alert("Shop", $"Shop {shopId} loaded: {loaded} ({Bot?.Shops?.Items?.Count ?? 0} items)");
        return loaded;
    }

    private bool BankItemSafely(string itemName)
    {
        try { InBank(itemName); Bot.Sleep(D3); return true; }
        catch { return false; }
    }

    private int GetCurrentQuantity(string itemName)
    {
        try { return Owned(itemName, isTemp: false); }
        catch { return 0; }
    }

    private ShopItem GetValidatedShopItem(string itemName)
    {
        return Bot?.Shops?.Items?.FirstOrDefault(i =>
            i?.Name?.Equals(itemName, StringComparison.OrdinalIgnoreCase) == true);
    }

    private bool ValidatePurchaseRequirements(ShopItem item, int quantity)
    {
        if (Bot?.Player == null)
        {
            Alert("Shop", "Bot.Player is null");
            return false;
        }

        long totalCost = (long)item.Cost * quantity;
        if (Bot.Player.Gold < totalCost)
        {
            Alert("Shop", $"Insufficient gold: need {totalCost}, have {Bot.Player.Gold}");
            return false;
        }

        if (Bot.Player.Level < item.Level)
        {
            Alert("Shop", $"Level too low: need {item.Level}, have {Bot.Player.Level}");
            return false;
        }

        if (Bot?.Inventory?.FreeSlots <= 0)
        {
            Alert("Shop", "No inventory space");
            return false;
        }

        Alert("Shop", $"Requirements met: gold={Bot.Player.Gold}, level={Bot.Player.Level}");
        return true;
    }

    private bool ExecutePurchase(ShopItem item, int quantity)
    {
        int before = GetCurrentQuantity(item.Name);
        Alert("Shop", $"Attempting purchase: {quantity} of {item.Name} (before: {before})");

        Bot.Shops.BuyItem(item.ID, item.ShopItemID, quantity);
        Bot.Sleep(D3);

        int after = GetCurrentQuantity(item.Name);
        bool success = after > before;
        Alert("Shop", $"Purchase result: {success} (after: {after}, bought: {after - before})");

        return success;
    }

    #endregion

    #region Drops

    public bool HasDrop(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        return Bot?.Drops?.CurrentDrops?.Any(d =>
            string.Equals(d, name, StringComparison.OrdinalIgnoreCase)) ?? false;
    }

    public bool HasDrop(int id)
    {
        if (id <= 0) return false;
        return Bot?.Drops?.CurrentDropInfos?.Any(i => i.ID == id) ?? false;
    }

    public ItemBase GetDropItem(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        return Bot?.Drops?.CurrentDropInfos?.FirstOrDefault(i =>
            string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public ItemBase GetDropItem(int id)
    {
        if (id <= 0) return null;
        return Bot?.Drops?.CurrentDropInfos?.FirstOrDefault(i => i.ID == id);
    }

    public void PickupItems(params string[] names)
    {
        if (names == null || names.Length == 0) return;

        foreach (string name in names)
        {
            if (string.IsNullOrWhiteSpace(name)) continue;

            if (HasDrop(name))
            {
                Bot.Drops.Pickup(name);
                Bot.Sleep(D1);
            }
        }
    }

    public void PickupItems(params int[] ids)
    {
        if (ids == null || ids.Length == 0) return;

        foreach (int id in ids)
        {
            if (id <= 0) continue;

            if (HasDrop(id))
            {
                var item = GetDropItem(id);
                if (item != null)
                {
                    Bot.Drops.Pickup(id);
                    Bot.Sleep(D1);
                }
            }
        }
    }

    public bool WaitForDrop(string name, int timeout = 30000)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;

        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            while (!HasDrop(name) && sw.ElapsedMilliseconds < timeout)
                Bot.Sleep(D1);

            return HasDrop(name);
        }
        finally
        {
            sw?.Stop();
        }
    }

    public bool WaitForDrop(int id, int timeout = 30000)
    {
        if (id <= 0) return false;

        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            while (!HasDrop(id) && sw.ElapsedMilliseconds < timeout)
                Bot.Sleep(D1);

            return HasDrop(id);
        }
        finally
        {
            sw?.Stop();
        }
    }

    public bool HasAnyDrop(params string[] names)
    {
        if (names == null || names.Length == 0) return false;
        return names.Any(name => !string.IsNullOrWhiteSpace(name) && HasDrop(name));
    }

    public bool HasAnyDrop(params int[] ids)
    {
        if (ids == null || ids.Length == 0) return false;
        return ids.Any(id => id > 0 && HasDrop(id));
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
        if (string.IsNullOrWhiteSpace(map)) return;
        if (Bot?.Map == null || Bot?.Player == null) return;

        string mapName = map.Split('-')[0].Trim();

        var supportMaps = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "championdrakath", "ultraezrajal", "ultrawarden", "ultraengineer", "ultradage",
            "ultratyndarius"
        };
        supportMode = supportMaps.Contains(mapName);

        string target = publicRoom
            ? mapName
            : roomNumber.HasValue
                ? $"{mapName}-{roomNumber.Value}"
                : (map.Contains("-") ? map : $"{mapName}-{GenerateRoomID(10000, 100000)}");

        if (Bot.Map.Name?.Equals(mapName, StringComparison.OrdinalIgnoreCase) == true)
            return;

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
        if (Bot?.Monsters == null || Bot?.Map == null || Bot?.Player == null) return;

        var names = (monsterNames?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>())
            .Select(n => n?.Trim())
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .ToArray();

        bool wildcard = names.Length == 0 || (names.Length == 1 && names[0] == "*");
        string pad = string.IsNullOrWhiteSpace(setPad) ? "Left" : setPad;

        var monsters = (Bot.Monsters.MapMonsters ?? Enumerable.Empty<Monster>())
            .Where(m => m != null && !string.IsNullOrWhiteSpace(m.Cell))
            .Where(m => wildcard || names.Any(name =>
                (m.Name ?? "").Equals(name, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        if (monsters.Count == 0) return;

        string targetCell =
            !string.IsNullOrWhiteSpace(setCell) ? setCell
            : alt ? monsters.First().Cell
            : monsters.GroupBy(m => m.Cell)
                      .OrderByDescending(g => g.Count())
                      .First().Key;

        var mapCells = Bot.Map.Cells as IEnumerable<string> ?? Array.Empty<string>();
        if (!mapCells.Contains(targetCell)) return;

        _bestCell = targetCell;
        _bestPad = pad;

        if (!string.IsNullOrWhiteSpace(targetCell) &&
            !string.Equals(Bot.Player.Cell, targetCell, StringComparison.Ordinal))
        {
            try
            {
                Bot.Map.Jump(targetCell, pad);
                Bot.Wait.ForCellChange(targetCell);
                Bot.Player.SetSpawnPoint();
            }
            catch { }
        }
    }

    int GenerateRoomID(int min = 1000, int max = 10000)
    {
        if (min >= max) (min, max) = (1000, 10000);

        static string GetStableMachineId()
        {
            try
            {
                var v = Microsoft.Win32.Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography", "MachineGuid", null
                ) as string;
                if (!string.IsNullOrWhiteSpace(v)) return v;
            }
            catch { /* non-Windows fallback */ }

            return $"{Environment.MachineName}|{Environment.UserName}";
        }

        var id = GetStableMachineId();
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(id));

        int seed = BitConverter.ToInt32(hash, 0);
        uint nonNeg = seed == int.MinValue ? 0u : (uint)Math.Abs(seed);

        int range = (max - min + 1);
        return (int)(nonNeg % range) + min;
    }

    double GetLowestHpPercentage()
    {
        if (Bot?.Map?.PlayerNames == null) return 100.0;

        var playerNames = Bot.Map.PlayerNames;
        if (playerNames.Count == 0) return 100.0;

        double lowestHpPercentage = 100.0;

        foreach (string playerName in playerNames)
        {
            if (string.IsNullOrWhiteSpace(playerName)) continue;

            try
            {
                int currentHp = Bot.Flash.GetGameObject<int>($"world.uoTree.{playerName}.intHP");
                int maxHp = Bot.Flash.GetGameObject<int>($"world.uoTree.{playerName}.intHPMax");

                if (maxHp <= 0 || currentHp < 0) continue;

                double hpPercentage = (double)currentHp / maxHp * 100.0;

                if (hpPercentage < lowestHpPercentage)
                {
                    lowestHpPercentage = hpPercentage;
                }
            }
            catch { continue; }
        }

        return lowestHpPercentage;
    }

    void ToSafeMap() => Join("whitemap");

    #endregion

    #region Skills

    readonly int skillsDelay = 50;
    public bool supportMode = false;

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
            case "lord of order": LordsOfOrderClass(); break;
            case "void highlord": VoidHighlordClass(); break;
            case "chaos avenger": ChaosAvengerClass(); break;
            case "lightcaster": LightCasterClass(); break;
            case "legion doomknight": LegionDoomKnightClass(); break;
            case "dragon of time": DragonOfTimeClass(); break;
            case "archmage": ArchmageClass(); break;

            // Chrono classes
            case "chrono dataknight": ChronoDataKnightClass(); break;
            case "shadowweaver of time": ShadowWeaverOfTimeClass(); break;
            case "quantum chronomancer": QuantumChronomancerClass(); break;

            // Common classes
            case "master ranger": MasterRangerClass(); break;
            case "dragonslayer general": DragonslayerGeneralClass(); break;
            case "cryomancer": CryomancerClass(); break;
            case "dragon knight": DragonKnightClass(); break;

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
        double myHealth = GetHealthPercentage();
        double anyHealth = GetLowestHpPercentage();

        int Empowerment = GetAuraSecondsRemaining("Empowerment", true);
        int Clarity = GetAuraSecondsRemaining("Clarity", true);

        if (supportMode)
        {
            bool NoxiousDecay = HasAura("Noxious Decay", true);

            if (Cast(4)) return;
            if (anyHealth < 90 || myHealth < 90 && !NoxiousDecay)
                if (Cast(2)) return;
            if (Empowerment <= 2)
                if (Cast(1)) return;
            if (Clarity <= 2)
                if (Cast(3)) return;
        }
        else
        {
            if (Cast(4)) return;
            if (anyHealth < 70 || myHealth < 70)
                if (Cast(2)) return;
            if (Empowerment <= 2)
                if (Cast(1)) return;
            if (Clarity <= 2)
                if (Cast(3)) return;
            if (Cast(4)) return;
        }
    }

    void StoneCrusherClass()
    {
        double myHealth = GetHealthPercentage();
        double anyHealth = GetLowestHpPercentage();

        int Dissonance = GetAuraSecondsRemaining("Dissonance", true);
        bool Magnitude = HasAura("Magnitude", true);

        if (anyHealth < 95 || myHealth < 95)
            if (Cast(3)) return;
        if (Dissonance <= 2)
            if (Cast(2)) return;
        if (Magnitude)
            if (Cast(4)) return;
        if (Cast(1)) return;
    }

    void ArchPaladinClass()
    {
        double myHealth = GetHealthPercentage();
        double anyHealth = GetLowestHpPercentage();

        bool RighteousSeal = HasAura("Righteous Seal");
        bool NoxiousDecay = HasAura("Noxious Decay", true);

        if (supportMode)
        {
            if (anyHealth < 85 || myHealth < 85 && !NoxiousDecay)
                if (Cast(2)) return;
            if (!RighteousSeal)
                if (Cast(4)) return;
            if (Cast(3)) return;
            if (Cast(1)) return;
        }
        else
        {
            if (anyHealth < 85 || myHealth < 85)
                if (Cast(2)) return;
            if (RighteousSeal)
                if (Cast(4)) return;
            if (Cast(3)) return;
            if (Cast(1)) return;
        }
    }

    void VoidHighlordClass()
    {
        double myHealth = GetHealthPercentage();

        bool Unshackled = HasAura("Unshackled", true);

        if (Unshackled)
            if (Cast(4)) return;
        if (myHealth > 50)
            if (Cast(1)) return;
        if (Cast(2)) return;
        if (myHealth > 50)
            if (Cast(3)) return;
    }

    void ChaosAvengerClass()
    {
        if (Bot.Map.Name == "ultradage")
        {
            bool Focus = HasAura("Focus");

            if (Cast(2)) return;
            if (Cast(4)) return;
            if (!Focus)
                if (Cast(5)) return;
            if (Cast(1)) return;
            if (Cast(3)) return;
        }
        else
        {
            if (Cast(2)) return;
            if (Cast(4)) return;
            if (Cast(1)) return;
            if (Cast(3)) return;
        }
    }

    void LightCasterClass()
    {
        double myHealth = GetHealthPercentage();
        double anyHealth = GetLowestHpPercentage();

        bool NoxiousDecay = HasAura("Noxious Decay", true); // Ultra Dage

        if (anyHealth < 85 || myHealth < 85 && !NoxiousDecay)
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
        double myHealth = GetHealthPercentage();

        bool Convergence = HasAura("Convergence", true);
        bool Discordance = HasAura("Discordance", true);

        if (myHealth < 95)
            if (Cast(2)) return;
        if (Convergence)
            if (Cast(3)) return;
        if (myHealth > 60)
            if (Cast(4)) return;
        if (Cast(1)) return;
    }

    void ArchmageClass()
    {
        double myHealth = GetHealthPercentage();
        double myMana = GetManaPercentage();

        bool Cryostasis = HasAura("Cryostasis");
        bool ArcaneFlux = HasAura("Arcane Flux", true);

        if (!Cryostasis || myMana < 30)
            if (Cast(2)) return;
        if (ArcaneFlux && myHealth >= 30)
            if (Cast(4)) return;
        if (Cast(3)) return;
        if (Cast(1)) return;
    }

    // --- chrono classes ---------------------------------------------------------------

    void ChronoDataKnightClass()
    {
        int TemporalRift = GetAuraStacks("Temporal Rift", true);

        if (TemporalRift == 4)
            if (Cast(4)) return;
        if (Cast(1)) return;
        if (Cast(2)) return;
        if (Cast(3)) return;
    }

    void ShadowWeaverOfTimeClass()
    {
        double myHealth = GetHealthPercentage();
        double myMana = GetManaPercentage();

        int ChaosRift = GetAuraStacks("Chaos Rift", true);

        if (myHealth < 50 || myMana < 30)
            if (Cast(3)) return;
        if (ChaosRift == 4)
            if (Cast(4)) return;
        if (Cast(1)) return;
        if (Cast(2)) return;
    }

    void QuantumChronomancerClass()
    {
        bool QuantumRestructure = HasAura("Quantum Restructure", true);
        int TemporalRift = GetAuraStacks("Temporal Rift", true);

        if (TemporalRift == 4)
            if (Cast(3)) return;
        if (QuantumRestructure)
            if (Cast(4)) return;
        if (Cast(2)) return;
        if (Cast(1)) return;
    }

    // --- common classes ---------------------------------------------------------------

    void MasterRangerClass()
    {
        int Marks = GetAuraStacks("Marks", true);
        bool VampiricShot = HasAura("Vampiric Shot", true);

        if (VampiricShot)
            if (Cast(3)) return;
        if (Marks == 6)
            if (Cast(4)) return;
        if (Marks == 4)
            if (Cast(2)) return;
        if (Cast(1)) return;
    }

    void DragonslayerGeneralClass()
    {
        bool Dragonsbane = HasAura("General's Dragonbane");

        if (Dragonsbane)
            if (Cast(2)) return;
        if (Dragonsbane)
            if (Cast(3)) return;
        if (Cast(4)) return;
        if (Cast(1)) return;
    }

    void CryomancerClass()
    {
        double myHealth = GetHealthPercentage();

        bool PolarVortex = HasAura("Polar Vortex", true);
        bool Frozen = HasAura("Frozen");

        if (myHealth < 60 && PolarVortex)
            if (Cast(3)) return;
        if (PolarVortex && Frozen)
            if (Cast(2)) return;
        if (Cast(1)) return;
        if (Cast(4)) return;
    }

    void DragonslayerClass()
    {
        bool InfectedWound = HasAura("Infected Wound");
        bool Weakened = HasAura("Weakened");
        bool Dragonbane = HasAura("Dragonbane");

        if (Dragonbane && !InfectedWound)
            if (Cast(2)) return;
        if (Dragonbane && !Weakened)
            if (Cast(3)) return;
        if (Cast(4)) return;
        if (Cast(1)) return;
    }

    void DragonKnightClass()
    {
        bool Flammable = HasAura("Flammable");
        bool Dumbfounded = HasAura("Dumbfounded");

        if (Cast(1)) return;
        if (Flammable)
            if (Cast(4)) return;
        if (Cast(2)) return;
        if (Dumbfounded)
            if (Cast(3)) return;
    }

    // --- basic classes ---------------------------------------------------------------

    void MageClass()
    {
        bool Scorched = HasAura("Scorched");
        bool FrozenBlood = HasAura("Frozen Blood");
        int ArcaneShield = GetAuraSecondsRemaining("Arcane Shield");

        if (ArcaneShield < 2)
            if (Cast(4)) return;
        if (FrozenBlood)
            if (Cast(1)) return;
        if (Scorched)
            if (Cast(3)) return;
        if (Cast(2)) return;
    }

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

    public bool HasAnyAura(List<string> auraNames)
    {
        if (auraNames == null || auraNames.Count == 0) return false;

        foreach (string aura in auraNames)
        {
            if (!string.IsNullOrWhiteSpace(aura) && HasAura(aura))
                return true;
        }
        return false;
    }

    public int GetAuraStacks(string auraName, bool self = false)
    {
        if (string.IsNullOrWhiteSpace(auraName)) return 0;

        try
        {
            object v = self ? Bot?.Self?.GetAuraValue(auraName)
                            : Bot?.Target?.GetAuraValue(auraName);

            if (v == null) return 0;

            return v is int i ? i
                 : v is long l ? (int)l
                 : v is double d ? (int)Math.Round(d)
                 : v is float f ? (int)Math.Round(f)
                 : int.TryParse(v.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var n) ? n
                 : 0;
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

    #endregion

    #region Listeners

    private volatile bool _chargeDetected;
    private int _chargeCount;

    private bool ChargeDetected
    {
        get => _chargeDetected;
        set
        {
            if (_chargeDetected == value) return;
            _chargeDetected = value;
            Bot.Log($"[PACKET] chargeDetected={value.ToString().ToLowerInvariant()}");
        }
    }

    public async Task PulseChargeAsync(int ms = 2000)
    {
        System.Threading.Interlocked.Increment(ref _chargeCount);
        ChargeDetected = true;

        try
        {
            await Task.Delay(ms).ConfigureAwait(false);
        }
        finally
        {
            if (System.Threading.Interlocked.Decrement(ref _chargeCount) <= 0)
            {
                _chargeCount = 0;
                ChargeDetected = false;
            }
        }
    }

    public async void ChargeListener(dynamic packet)
    {
        try
        {
            if (packet?["params"]?.type?.ToString() != "json") return;

            dynamic data = packet["params"].dataObj;
            if (data?.cmd?.ToString() != "ct") return;

            var anims = data?.anims as System.Collections.IEnumerable;
            if (anims == null) return;

            foreach (var a in anims)
            {
                string animStr = (a as dynamic)?.animStr?.ToString();
                if (!string.IsNullOrEmpty(animStr) &&
                    animStr.Equals("Charge", StringComparison.OrdinalIgnoreCase))
                {
                    await PulseChargeAsync(2000).ConfigureAwait(false);
                    break;
                }
            }
        }
        catch (Exception ex) { }
    }

    async void ChaosHarpyListener(dynamic packet)
    {
        try
        {
            if (packet?["params"]?.type?.ToString() != "json") return;

            dynamic data = packet["params"].dataObj;
            if (data?.cmd?.ToString() != "ct") return;

            var anims = data?.anims as System.Collections.IEnumerable;
            if (anims == null) return;

            foreach (var a in anims)
            {
                string animStr = (a as dynamic)?.animStr?.ToString();
                if (!string.IsNullOrEmpty(animStr) &&
                    animStr.Equals("Charge", StringComparison.OrdinalIgnoreCase))
                {
                    DisableSkills();
                    Bot.Sleep(D1);
                    await Task.Delay(500);
                    Bot.Skills.UseSkill(5);
                    await Task.Delay(500);
                    EnableSkills();
                    break;
                }
            }
        }
        catch (Exception ex) { }
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

            if (checkDelay > 0)
                Bot.Sleep(checkDelay);

            if (effect < 2)
                UsePotion();
        }
    }

    public void TauntCharge(string name, string monster, string aura, int checkDelay)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(monster)) return;
        if (Bot?.Combat == null) return;

        if (HasClassEquipped(name))
        {
            Bot.Combat.Attack(monster);

            if (checkDelay > 0)
                Bot.Sleep(checkDelay);

            if (_chargeDetected)
                UsePotion();
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
        const string THRESHOLD_KEY = "drakath.lastThreshold";

        if (Bot?.Combat == null || Bot?.Player == null) return;

        var bands = new (int thr, int rng)[] {
            (18_000_000, 200_000), (16_000_000, 200_000), (14_000_000, 200_000),
            (12_000_000, 200_000), (10_000_000, 200_000), (8_000_000, 120_000),
            (6_000_000, 120_000), (4_000_000, 120_000), (2_000_000, 120_000)
        };

        Bot.Combat.Attack("Champion Drakath");
        var t = Bot.Player.Target;
        if (t?.HP == null || t.HP <= 0) return;

        int hp = t.HP;
        var lastThreshold = AppDomain.CurrentDomain.GetData(THRESHOLD_KEY) as int? ?? int.MinValue;
        var matchingBand = Array.Find(bands, band => Math.Abs(hp - band.thr) <= band.rng);

        if (matchingBand != default && lastThreshold != matchingBand.thr)
        {
            AppDomain.CurrentDomain.SetData(THRESHOLD_KEY, matchingBand.thr);

            while (MonsterAlive("Champion Drakath") && !HasAura("Focus") && !Bot.ShouldExit)
                UsePotion();
        }

        Bot.Sleep(150);
    }

    public void KillWithPriority(string primaryName, int primaryMapId, string priorityName1, int priorityMapId1, string priorityName2, int priorityMapId2)
    {
        if (string.IsNullOrWhiteSpace(primaryName)) return;

        if (!string.IsNullOrWhiteSpace(priorityName1) && IsAliveByMapId(priorityMapId1, name: priorityName1))
            KillByMapId(priorityMapId1, name: priorityName1);
        else if (!string.IsNullOrWhiteSpace(priorityName2) && IsAliveByMapId(priorityMapId2, name: priorityName2))
            KillByMapId(priorityMapId2, name: priorityName2);
        else
            KillByMapId(primaryMapId, name: primaryName);

        Bot.Sleep(D1);
    }

    // --- helpers ---------------------------------------------------

    public void KillByMapId(int mapId, string? name = null, int? id = null)
    {
        if (Bot?.Combat == null) return;

        if (IsAliveByMapId(mapId, name, id))
        {
            Bot.Combat.Attack(mapId);
            Bot.Sleep(250);
        }
    }

    public void WaitForArmy(int quantity, int bufferTimeMs = 3000)
    {
        if (Bot?.Map == null) return;

        int required = quantity + 1;

        while (!Bot.ShouldExit && Bot.Map.PlayerCount < required)
        {
            int others = Math.Max(0, Bot.Map.PlayerCount - 1);
            Alert("Army", $"Waiting for army: {others}/{quantity} players ready");

            Bot.Skills.UseSkill(1);
            Bot.Skills.UseSkill(2);
            Bot.Skills.UseSkill(3);
            Bot.Sleep(1000);
        }

        if (Bot.ShouldExit) return;

        Alert("Army", $"All players ready ({Bot.Map.PlayerCount}), final buffing...");
        Bot.Skills.UseSkill(1);
        Bot.Skills.UseSkill(2);
        Bot.Skills.UseSkill(3);

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
