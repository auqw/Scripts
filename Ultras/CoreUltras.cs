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

    int d1 = 250;
    int d2 = 700;
    int d3 = 1400;
    int d4 = 2800;

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

    private void ForItemCore(string monsters, int quantity, bool isTemp, bool useBestGear, bool alt, string? cell, string pad, bool priority, Action ensureInBank, Func<int> ownedCount, Action pickup, string itemLabel)
    {
        if (quantity <= 0) return;

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

    public void ForItem(string monsters, string name, int quantity = 1, bool isTemp = false, bool useBestGear = false, bool alt = false, string? cell = null, string pad = "Left", bool priority = false)
    {
        if (string.IsNullOrWhiteSpace(name) || quantity <= 0) return;

        ForItemCore(
            monsters, quantity, isTemp, useBestGear, alt, cell, pad, priority,
            ensureInBank: () => { if (!isTemp) InBank(name); },
            ownedCount: () => Owned(name, isTemp),
            pickup: () => PickupItems(name),
            itemLabel: name
        );
    }

    public void ForItem(string monsters, int itemId, int quantity = 1, bool isTemp = false, bool useBestGear = false, bool alt = false, string? cell = null, string pad = "Left", bool priority = false)
    {
        if (itemId <= 0 || quantity <= 0) return;

        var itemLabel = GetDropItem(itemId)?.Name ?? $"Item#{itemId}";

        ForItemCore(
            monsters, quantity, isTemp, useBestGear, alt, cell, pad, priority,
            ensureInBank: () => { if (!isTemp) InBank(itemId); },
            ownedCount: () => Owned(itemId, isTemp),
            pickup: () => PickupItems(itemId),
            itemLabel: itemLabel
        );
    }

    public void EquipBestClass(List<(string name, int rank)> priorities)
    {
        string currentClassName = GetCurrentClassName();
        bool isMaxRank = IsCurrentClassMaxRank();

        foreach (var (name, rank) in priorities)
        {
            if (Owned(name) < 1) continue;

            if (HasClassEquipped(name))
            {
                if (isMaxRank || Bot.Player.CurrentClassRank >= rank) return;
            }

            if (!Bot.Inventory.IsEquipped(name))
            {
                Bot.Inventory.EquipItem(name);
                Bot.Sleep(d3);
                return;
            }
        }
    }

    public void EquipBestClass(List<(int id, int rank)> priorities)
    {
        string currentClassName = GetCurrentClassName();
        bool isMaxRank = IsCurrentClassMaxRank();

        foreach (var (id, rank) in priorities)
        {
            if (Owned(id) < 1) continue;
            var item = Bot.Inventory.Items.FirstOrDefault(i => i.ID == id);
            if (item == null) continue;

            if (HasClassEquipped(item.Name))
            {
                if (isMaxRank || Bot.Player.CurrentClassRank >= rank) return;
            }

            if (!Bot.Inventory.IsEquipped(id))
            {
                Bot.Inventory.EquipItem(id);
                Bot.Sleep(d3);
                return;
            }
        }
    }

    public void EquipConsumable(string name)
    {
        StopAttack();
        if (Owned(name) < 1) return;
        if (Bot.Inventory.IsEquipped(name)) return;
        Bot.Inventory.EquipUsableItem(name);
        Bot.Sleep(d3);
    }

    public bool InBank(string name)
    {
        if (Bot.Bank.Contains(name))
        {
            int quantity = Bot.Bank.GetQuantity(name);
            StopAttack();
            Bot.Bank.ToInventory(name);
            Bot.Sleep(d2);
            Alert("BANK", $"Moved {quantity}x {name} from bank to inventory");
            return true;
        }
        return false;
    }

    public bool InBank(int id)
    {
        if (Bot.Bank.Contains(id))
        {
            int quantity = Bot.Bank.GetQuantity(id);
            var item = Bot.Bank.Items.FirstOrDefault(i => i.ID == id);
            string itemName = item?.Name ?? $"Item#{id}";
            StopAttack();
            Bot.Bank.ToInventory(id);
            Bot.Sleep(d2);
            Alert("BANK", $"Moved {quantity}x {itemName} from bank to inventory");
            return true;
        }
        return false;
    }

    public bool ToBank(string name)
    {
        if (Bot.Inventory.Contains(name))
        {
            int quantity = Bot.Inventory.GetQuantity(name);
            StopAttack();
            Bot.Inventory.ToBank(name);
            Bot.Sleep(d2);
            Alert("INVENTORY", $"Moved {quantity}x {name} from inventory to bank");
            return true;
        }
        return false;
    }

    public bool ToBank(int id)
    {
        if (Bot.Inventory.Contains(id))
        {
            int quantity = Bot.Inventory.GetQuantity(id);
            var item = Bot.Inventory.Items.FirstOrDefault(i => i.ID == id);
            string itemName = item?.Name ?? $"Item#{id}";
            StopAttack();
            Bot.Inventory.ToBank(id);
            Bot.Sleep(d2);
            Alert("INVENTORY", $"Moved {quantity}x {itemName} from inventory to bank");
            return true;
        }
        return false;
    }

    public int Owned(string name, bool isTemp = false)
    {
        if (string.IsNullOrWhiteSpace(name)) return 0;
        if (isTemp) return Bot.TempInv?.GetQuantity(name) ?? 0;
        return Bot.Inventory?.GetQuantity(name) ?? 0;
    }

    public int Owned(int id, bool isTemp = false)
    {
        if (id <= 0) return 0;
        if (isTemp) return Bot.TempInv?.GetQuantity(id) ?? 0;
        return Bot.Inventory?.GetQuantity(id) ?? 0;
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

    private void Attack(MonsterKey key)
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
        Bot.Sleep(d1);
    }

    public void KillWithPriority(params MonsterKey[] keys)
    {
        foreach (var k in keys)
        {
            if (IsAlive(k))
            {
                EnsureMonsterSetup(k);
                Attack(k);
                Bot.Sleep(d1);
                return;
            }
        }
        Bot.Sleep(d1);
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

    #region Potions & Scrolls

    public void UsePotion()
    {
        DisableSkills();
        Bot.Sleep(d1);
        Bot.Skills.UseSkill(5);
        Bot.Sleep(d2);
        EnableSkills();
    }

    public void GetScrollOfEnrage()
    {
        DisableSkills();
        if (!Bot.Reputation.HasRank("SpellCrafting", 5)) return;

        while (Owned("Scroll of Enrage") < 10)
        {
            if (Owned("Zealous Ink") < 1)
            {
                if (Owned("Mystic Parchment") < 2)
                {
                    Join("underworld");
                    ForItem("Undead Infantry", "Mystic Parchment", 2);
                }
                BuyItem("dragonrune", 549, "Zealous Ink", 5);
            }

            Join("spellcraft");
            Bot.Drops.Add("Scroll of Enrage");
            Bot.Send.Packet("%xt%zm%crafting%1%spellOnStart%7%1555%Spell%"); Bot.Sleep(5000);
            Bot.Send.Packet("%xt%zm%crafting%1%spellComplete%7%2330%Enrage%");
        }

        Join("dragonrune");
        EquipConsumable("Scroll of Enrage");
        EnableSkills();
    }

    public void GetScrollOfDecay()
    {
        DisableSkills();
        if (!Bot.Reputation.HasRank("SpellCrafting", 5)) return;

        while (Owned("Scroll of Decay") < 10)
        {
            if (Owned("Zealous Ink") < 1)
            {
                if (Owned("Mystic Parchment") < 2)
                {
                    Join("underworld");
                    ForItem("Undead Infantry", "Mystic Parchment", 2);
                }
                BuyItem("dragonrune", 549, "Zealous Ink", 5);
            }

            Join("spellcraft");
            Bot.Drops.Add("Scroll of Decay");
            Bot.Send.Packet("%xt%zm%crafting%1%spellOnStart%7%1555%Spell%"); Bot.Sleep(5000);
            Bot.Send.Packet("%xt%zm%crafting%1%spellComplete%7%2331%Decay%");
        }

        Join("dragonrune");
        EquipConsumable("Scroll of Decay");
        EnableSkills();
    }

    public void GetDivineElixir()
    {
        DisableSkills();
        if (Owned("Divine Elixir") < 1)
        {
            Join("poisonforest");
            ForItem("Xavier Lionfang", "Divine Elixir");
        }

        Join("battleon");
        EquipConsumable("Divine Elixir");
        UsePotion();
        EnableSkills();
    }

    #endregion

    #region Best Gear

    public void ChooseBestGear(string monsterNames)
    {
        var monsters = GetMonsters(monsterNames).Where(m => m?.Race != null).ToList();
        if (!monsters.Any()) return;

        string race = monsters.GroupBy(m => m.Race)
                             .OrderByDescending(g => g.Count())
                             .First().Key;

        if (race.Equals("None", StringComparison.OrdinalIgnoreCase)) race = "allDmg";

        var items = GetItems(race).ToList();
        if (!items.Any()) return;

        // Try combo first
        var combo = items.Where(a => a.All > 0)
                        .SelectMany(a => items.Where(r => r.Race > 0 && r.Group != a.Group)
                                              .Select(r => (a, r, Total: a.All + r.Race)))
                        .OrderByDescending(x => x.Total)
                        .FirstOrDefault();

        if (combo.a.Name != null)
        {
            Equip(combo.a);
            Equip(combo.r);
        }
        else
        {
            Equip(items.OrderByDescending(i => Math.Max(i.Race, i.All)).First());
        }
    }

    IEnumerable<Monster> GetMonsters(string names) =>
       string.IsNullOrEmpty(names) || names == "*"
           ? Bot.Monsters.MapMonsters ?? Enumerable.Empty<Monster>()
           : (Bot.Monsters.MapMonsters ?? Enumerable.Empty<Monster>())
               .Where(m => names.Split(',').Any(n => n.Trim().Equals(m.Name, StringComparison.OrdinalIgnoreCase)));

    IEnumerable<(string Name, string Group, bool FromBank, double All, double Race)> GetItems(string race) =>
       Bot.Inventory.Items.Concat(Bot.Bank.Items)
           .Where(i => new[] { "Weapon", "he", "ba", "co", "pe" }.Contains(i.ItemGroup) &&
                      (!i.Upgrade || Bot.Player.IsMember))
           .Select(i => (i.Name, i.ItemGroup, Bot.Bank.Items.Contains(i),
                        ParseMeta(i.Meta, "allDmg"), ParseMeta(i.Meta, race)))
           .Where(x => x.Item4 > 0 || x.Item5 > 0);

    double ParseMeta(string meta, string key) =>
       string.IsNullOrWhiteSpace(meta) ? 0 :
       meta.Split('\n', '\r').SelectMany(line => line.Split(','))
           .Where(pair => pair.Contains(':'))
           .Select(pair => pair.Split(':'))
           .Where(parts => parts.Length == 2 &&
                  (parts[0].Trim().Equals(key, StringComparison.OrdinalIgnoreCase) ||
                   (key == "allDmg" && parts[0].Trim().Equals("dmgAll", StringComparison.OrdinalIgnoreCase))))
           .Select(parts => double.TryParse(parts[1].Trim(), out var v) ? v - 1 : 0)
           .FirstOrDefault();

    void Equip((string Name, string Group, bool FromBank, double All, double Race) item)
    {
        if (item.FromBank) InBank(item.Name);
        // TODO: Actual equip logic
    }

    #endregion

    #region Shop

    private bool EnsureShopLoaded(string map, int shopId)
    {
        if (!string.IsNullOrWhiteSpace(map))
            Join(map);

        Bot.Shops.Load(shopId);
        Bot.Sleep(800);

        return Bot.Shops.IsLoaded && Bot.Shops.ID == shopId;
    }

    public bool BuyItem(string map, int shopId, string itemName, int quantity, bool considerBank = true)
    {
        if (string.IsNullOrWhiteSpace(itemName) || quantity <= 0)
            return false;

        if (!EnsureShopLoaded(map, shopId))
            return false;

        if (considerBank)
            InBank(itemName);

        int have = Owned(itemName, isTemp: false);
        int need = Math.Max(0, quantity - have);
        if (need == 0)
            return true;

        var item = GetShopItem(itemName);
        if (item == null)
            return false;

        if (!CanBuyItem(itemName))
            return false;

        Bot.Shops.BuyItem(item.ID, item.ShopItemID, need);
        Bot.Sleep(400);
        return true;
    }

    public ShopItem GetShopItem(string name)
    {
        return Bot.Shops.Items.FirstOrDefault(i =>
            i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public bool CanBuyItem(string itemName)
    {
        var item = GetShopItem(itemName);
        if (item == null) return false;

        return Bot.Player.Gold >= item.Cost &&
               Bot.Player.Level >= item.Level &&
               MeetsRepRequirement(itemName);
    }

    public bool MeetsRepRequirement(string itemName)
    {
        var item = GetShopItem(itemName);
        if (item == null || string.IsNullOrEmpty(item.Faction))
            return true;

        return Bot.Reputation.GetRank(item.Faction) >= item.RequiredReputation;
    }

    #endregion

    #region Drops

    public bool HasDrop(string name) =>
        Bot.Drops.CurrentDrops.Any(d =>
            string.Equals(d, name, StringComparison.OrdinalIgnoreCase));

    public bool HasDrop(int id) =>
        Bot.Drops.CurrentDropInfos.Any(i => i.ID == id);

    public ItemBase GetDropItem(string name) =>
        Bot.Drops.CurrentDropInfos.FirstOrDefault(i =>
            string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase));

    public ItemBase GetDropItem(int id) =>
        Bot.Drops.CurrentDropInfos.FirstOrDefault(i => i.ID == id);

    public void PickupItems(params string[] names)
    {
        foreach (string name in names)
            if (HasDrop(name))
                Bot.Drops.Pickup(name);
    }

    public void PickupItems(params int[] ids)
    {
        foreach (int id in ids)
            if (HasDrop(id))
            {
                var item = GetDropItem(id);
                if (item != null)
                    Bot.Drops.Pickup(item.Name);
            }
    }

    public void WaitForDrop(string name, int timeout = 30000)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (!HasDrop(name) && sw.ElapsedMilliseconds < timeout)
            Bot.Sleep(100);
    }

    public void WaitForDrop(int id, int timeout = 30000)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (!HasDrop(id) && sw.ElapsedMilliseconds < timeout)
            Bot.Sleep(100);
    }

    public bool HasAnyDrop(params string[] names) =>
        names.Any(name => HasDrop(name));

    public bool HasAnyDrop(params int[] ids) =>
        ids.Any(id => HasDrop(id));

    #endregion

    #region Player

    public double GetHealthPercentage()
    {
        return Bot.Player.MaxHealth > 0 ? (double)Bot.Player.Health / Bot.Player.MaxHealth * 100 : 0;
    }

    public double GetManaPercentage()
    {
        return Bot.Player.MaxMana > 0 ? (double)Bot.Player.Mana / Bot.Player.MaxMana * 100 : 0;
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
        return Bot.Player.Health >= Bot.Player.MaxHealth;
    }

    public bool IsFullMana()
    {
        return Bot.Player.Mana >= Bot.Player.MaxMana;
    }

    public bool IsFullHealthAndMana()
    {
        return IsFullHealth() && IsFullMana();
    }

    public bool IsDead()
    {
        return Bot.Player.State == 0;
    }

    public bool IsIdle()
    {
        return Bot.Player.State == 1;
    }

    public double GetDistanceTo(int x, int y)
    {
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
        return Bot.Player.CurrentClass?.Name ?? "No Class";
    }

    public bool HasClassEquipped(string className)
    {
        return Bot.Player.CurrentClass?.Name?.Equals(className, StringComparison.OrdinalIgnoreCase) == true;
    }

    public bool IsCurrentClassMaxRank()
    {
        return Bot.Player.CurrentClassRank >= 10;
    }

    public bool IsInCell(string cellName)
    {
        return Bot.Player.Cell?.Equals(cellName, StringComparison.OrdinalIgnoreCase) == true;
    }

    public bool NeedsRest(double healthThreshold = 50, double manaThreshold = 50)
    {
        return IsHealthLow(healthThreshold) || IsManaLow(manaThreshold);
    }

    public bool ShouldRest()
    {
        return !Bot.Player.InCombat && !IsFullHealthAndMana();
    }

    public string GetTargetName()
    {
        return Bot.Player.Target?.Name ?? string.Empty;
    }

    public double GetTargetHealthPercentage()
    {
        if (Bot.Player.Target == null || Bot.Player.Target.MaxHP == 0) return 0;
        return (double)Bot.Player.Target.HP / Bot.Player.Target.MaxHP * 100;
    }

    public bool IsTargetAlive()
    {
        return Bot.Player.Target?.Alive == true;
    }

    public bool IsTargetHealthLow(double percentage = 30)
    {
        return GetTargetHealthPercentage() < percentage;
    }

    public bool HasEnoughGold(int amount)
    {
        return Bot.Player.Gold >= amount;
    }

    public PlayerStats GetPlayerStats()
    {
        return Bot.Player.Stats ?? new PlayerStats();
    }

    public int GetStatValue(string statName)
    {
        var stats = Bot.Player.Stats;
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
        return Bot.Player.Stats?.CriticalChance ?? 0f;
    }

    public float GetCriticalMultiplier()
    {
        return Bot.Player.Stats?.CriticalMultiplier ?? 0f;
    }

    public float GetEvasionChance()
    {
        return Bot.Player.Stats?.EvasionChance ?? 0f;
    }

    public float GetHaste()
    {
        return Bot.Player.Stats?.Haste ?? 0f;
    }

    public bool IsReadyForCombat()
    {
        return Bot.Player.Alive && Bot.Player.Loaded;
    }

    #endregion

    #region Map

    string _bestCell = null;
    string _bestPad = "Left";

    public void Join(string map, string cell = "Enter", string pad = "Spawn", bool publicRoom = false, int? roomNumber = null)
    {
        if (string.IsNullOrWhiteSpace(map)) return;

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

        if (Bot.Map.Name.Equals(mapName, StringComparison.OrdinalIgnoreCase))
            return;

        while (!Bot.ShouldExit && !Bot.Map.Name.Equals(mapName, StringComparison.OrdinalIgnoreCase))
        {
            StopAttack();
            Bot.Send.Packet($"%xt%zm%cmd%{Bot.Map.RoomID}%tfer%{Bot.Player.Username}%{target}%{cell}%{pad}%");
            Bot.Wait.ForMapLoad(mapName);
        }
        ResetMonsterSetupCache();
    }

    public void ChooseBestCell(string monsterNames, bool alt = false, string setCell = null, string setPad = "Spawn")
    {
        var names = (monsterNames?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>())
            .Select(n => n?.Trim())
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .ToArray();

        bool wildcard = names.Length == 0 || (names.Length == 1 && names[0] == "*");
        string pad = string.IsNullOrWhiteSpace(setPad) ? "Left" : setPad;

        // Get monsters directly without using helper methods
        var monsters = (Bot.Monsters.MapMonsters ?? Enumerable.Empty<Monster>())
            .Where(m => m != null && !string.IsNullOrWhiteSpace(m.Cell))
            .Where(m => wildcard || names.Any(name =>
                (m.Name ?? "").Equals(name, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        if (monsters.Count == 0)
            return;

        string targetCell =
            !string.IsNullOrWhiteSpace(setCell) ? setCell
            : alt ? monsters.First().Cell
            : monsters.GroupBy(m => m.Cell)
                      .OrderByDescending(g => g.Count())
                      .First().Key;

        var mapCells = Bot.Map.Cells as IEnumerable<string> ?? Array.Empty<string>();
        if (!mapCells.Contains(targetCell))
            return;

        _bestCell = targetCell;
        _bestPad = pad;

        if (!string.IsNullOrWhiteSpace(targetCell) && !string.Equals(Bot.Player.Cell, targetCell, StringComparison.Ordinal))
        {
            Bot.Map.Jump(targetCell, pad);
            Bot.Wait.ForCellChange(targetCell);
            Bot.Player.SetSpawnPoint();
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
            catch { /* non-Windows placeh */ }

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
        try
        {
            List<string> playerNames = Bot.Map.PlayerNames;
            if (playerNames == null || playerNames.Count == 0) return 100.0;
            double lowestHpPercentage = 100.0;

            foreach (string playerName in playerNames)
            {
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
                catch (System.Exception playerEx)
                {
                    continue;
                }
            }

            return lowestHpPercentage;
        }
        catch (System.Exception ex)
        {
            return 100.0;
        }
    }

    #endregion

    #region Skills

    readonly int skillsDelay = 50;
    public bool supportMode = false;

    public bool Cast(int index) // basic
    {
        if (!Bot.Skills.CanUseSkill(index))
            return false;

        Bot.Skills.UseSkill(index);

        return true;
    }

    public void DisableSkills()
    {
        _cts?.Cancel();
        _runSkills = null;
        _cts = null;
    }

    public void EnableSkills()
    {
        if (_runSkills != null && !_runSkills.IsCompleted)
            return;

        _cts = new CancellationTokenSource();
        _runSkills = Task.Run(() => SkillsAsync(_cts.Token));
    }

    async Task SkillsAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                Skills();
            }
            catch (Exception ex)
            {
                Alert("ERROR", $"Skills error: {ex.Message}");
            }

            try
            {
                await Task.Delay(skillsDelay, token);
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Alert("ERROR", $"Delay error: {ex.Message}");
                await Task.Delay(1000);
            }
        }
    }

    void Skills()
    {
        if (!Bot.Player.HasTarget) return;

        string c = Bot.Player.CurrentClass?.Name;
        string className = c.ToLower();

        switch (className)
        {
            // --- ultra classes ---------------------------------------------------------------
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

            // --- chrono classes ---------------------------------------------------------------
            case "chrono dataknight": ChronoDataKnightClass(); break;
            case "shadowweaver of time": ShadowWeaverOfTime(); break;

            // --- common classes ---------------------------------------------------------------
            case "master ranger": MasterRangerClass(); break;
            case "dragonslayer general": DragonslayerGeneralClass(); break;
            case "cryomancer": CryomancerClass(); break;

            // --- basic classes ---------------------------------------------------------------
            case "mage": MageClass(); break;
            case "dragonslayer": DragonslayerClass(); break;

            default: Alert("SKILL", $"No rotation for: {className}"); break;
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
        bool Focus = HasAura("Focus");

        if (_chargeDetected)
        {
            Bot.Sleep(8000);
            if (Cast(4)) return;
        }
        if (!Focus)
            if (Cast(5)) return;
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

    void ShadowWeaverOfTime()
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

    public Aura GetAuraByName(string auraName, bool self) =>
         string.IsNullOrWhiteSpace(auraName) ? null : GetAuras(self).FirstOrDefault(a => a != null &&
         !string.IsNullOrWhiteSpace(a.Name) && auraName.Equals(a.Name, StringComparison.OrdinalIgnoreCase));

    public bool HasAura(string auraName, bool self = false)
    {
        return GetAuraByName(auraName, self) != null;
    }

    public bool HasAnyAura(List<string> auraNames)
    {
        foreach (string aura in auraNames)
        {
            if (HasAura(aura))
                return true;
        }
        return false;
    }

    public int GetAuraStacks(string auraName, bool self = false)
    {
        if (string.IsNullOrWhiteSpace(auraName)) return 0;

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

    public int GetAuraSecondsRemaining(string auraName, bool self = false)
    {
        var aura = GetAuraByName(auraName, self);
        if (aura == null || aura.Timestamp <= 0 || aura.Duration <= 0)
            return 0;

        var applied = DateTimeOffset.FromUnixTimeMilliseconds(aura.Timestamp);
        var expires = applied.AddSeconds(aura.Duration);
        var remaining = (int)(expires - DateTimeOffset.Now).TotalSeconds;
        return Math.Max(0, remaining);
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
        catch (Exception ex)
        {
            Bot.Log($"[PACKET] ChargeListener error: {ex.Message}");
        }
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
                    //await PulseChargeAsync(2000).ConfigureAwait(false);
                    DisableSkills();
                    Bot.Sleep(d1);
                    await Task.Delay(500);
                    Bot.Skills.UseSkill(5);
                    await Task.Delay(500);
                    EnableSkills();
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Bot.Log($"[PACKET] ChargeListener error: {ex.Message}");
        }
    }

    #endregion

    #region Experimental Ultras

    public void TauntCycle(string name, string monster, string aura, int checkDelay)
    {
        if (HasClassEquipped(name))
        {
            int Effect = GetAuraSecondsRemaining(aura);
            Bot.Combat.Attack(monster); Bot.Sleep(checkDelay);
            if (Effect < 2) UsePotion();
        }
    }

    public void TauntCharge(string name, string monster, string aura, int checkDelay)
    {
        if (HasClassEquipped(name))
        {
            Bot.Combat.Attack(monster); Bot.Sleep(checkDelay);
            if (_chargeDetected) UsePotion();
        }
    }

    public void UltraWardenTaunter() // custom
    {
        const string USED_THRESHOLDS_KEY = "warden.usedThresholds";

        Bot.Combat.Attack("Ultra Warden");
        var t = Bot?.Player?.Target;
        if (t?.HP == null || t.HP <= 0 || t.MaxHP <= 0) return;

        int currentHp = t.HP;
        int maxHp = t.MaxHP;

        int currentThreshold = (currentHp * 100) / maxHp;
        int thresholdBand = (currentThreshold / 5) * 5;

        var usedObj = AppDomain.CurrentDomain.GetData(USED_THRESHOLDS_KEY);
        var usedThresholds = usedObj as HashSet<int> ?? new HashSet<int>();

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

    public void DrakathTaunter() // custom
    {
        const string THRESHOLD_KEY = "drakath.lastThreshold";
        var bands = new (int thr, int rng)[] {
                    (18_000_000, 120_000), (16_000_000, 120_000), (14_000_000, 120_000),
                    (12_000_000, 120_000), (10_000_000, 120_000), (8_000_000, 80_000),
                    (6_000_000, 80_000), (4_000_000, 80_000), (2_000_000, 80_000)
                };

        Bot.Combat.Attack("Champion Drakath");
        var t = Bot?.Player?.Target;
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
        if (IsAliveByMapId(priorityMapId1, name: priorityName1))
            KillByMapId(priorityMapId1, name: priorityName1);
        else if (IsAliveByMapId(priorityMapId2, name: priorityName2))
            KillByMapId(priorityMapId2, name: priorityName2);
        else
            KillByMapId(primaryMapId, name: primaryName);
        Bot.Sleep(d1);
    }

    // --- helpers ---------------------------------------------------

    /*private bool IsAliveByMapId(int? mapId = null, string? name = null, int? id = null)
    {
        var monsters = Bot.Monsters.MapMonsters ?? Enumerable.Empty<Monster>();

        if (mapId.HasValue)
            monsters = monsters.Where(m => m.MapID == mapId.Value);
        if (name != null)
            monsters = monsters.Where(m => m.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) == true);
        if (id.HasValue)
            monsters = monsters.Where(m => m.ID == id.Value);

        return monsters.Any(m => m.Alive);
    }*/

    public void KillByMapId(int mapId, string? name = null, int? id = null)
    {
        if (IsAliveByMapId(mapId, name, id))
        {
            Bot.Combat.Attack(mapId);
            Bot.Sleep(250);
        }
    }

    public void WaitForArmy(int quantity)
    {
        int need = Math.Max(0, quantity);
        int required = need + 1; // + you

        if (Bot.Map.PlayerCount >= required)
        {
            int othersNow = Math.Max(0, Bot.Map.PlayerCount - 1);
            Alert("ARMY", $"Ready: have {othersNow} others + you (needed {need}).");
            return;
        }

        while (!Bot.ShouldExit && Bot.Map.PlayerCount < required)
        {
            int others = Math.Max(0, Bot.Map.PlayerCount - 1);
            Alert("ARMY", $"Waiting for {need} additional players. Current: {others} others + you");
        }

        if (!Bot.ShouldExit)
        {
            int othersNow = Math.Max(0, Bot.Map.PlayerCount - 1);
            Alert("ARMY", $"Ready: have {othersNow} others + you (needed {need}).");
        }
    }

    public bool MonsterAlive(string name)
    {
        return Bot.Monsters.MapMonsters
            .Any(m => m.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) == true && m.Alive);
    }

    public void StopAttack()
    {
        Bot.Combat.CancelAutoAttack();
        Bot.Combat.CancelTarget();

        var cellsList = Bot.Map.Cells ?? new List<string>();
        var monstersList = Bot.Monsters.MapMonsters ?? new List<Monster>();

        string safeCell = cellsList
            .Where(c => !string.IsNullOrWhiteSpace(c)
                        && !c.Equals("Wait", StringComparison.OrdinalIgnoreCase)
                        && !c.Equals("Blank", StringComparison.OrdinalIgnoreCase)
                        && !c.StartsWith("Cut", StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => monstersList.Count(m => m != null && m.Cell == c))
            .FirstOrDefault() ?? Bot.Player.Cell;

        string pad = string.IsNullOrWhiteSpace(Bot.Player.Pad) ? "Left" : Bot.Player.Pad;

        // hop until we’re out
        while (!Bot.ShouldExit && Bot.Player.State == 2)
        {
            if (!string.Equals(Bot.Player.Cell, safeCell, StringComparison.Ordinal))
            {
                Bot.Map.Jump(safeCell, pad);
                Bot.Wait.ForCellChange(safeCell);
            }
            Bot.Sleep(d1);
        }
        Bot.Sleep(d3);
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
