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

public class CoreEngine
{
    public IScriptInterface Bot => IScriptInterface.Instance;

    readonly ConcurrentDictionary<string, object> _cache = new();
    readonly ConcurrentDictionary<string, DateTime> _throttle = new();

    CancellationTokenSource _cts;
    Task _runSkills;

    public TimeSpan ThrottleDuration { get; set; } = TimeSpan.FromSeconds(3);
    public event Action<string, string> OnSignal;

    #region Settings

    public int D1 = 250;
    public int D2 = 700;
    public int D3 = 1400;
    public int D4 = 2800;

    public void Boot()
    {
        if (_runSkills?.Status == TaskStatus.Running)
            return;

        OnSignal += (category, message) => { Bot.Log($"[{category}] {message}"); };

        Bot.Events.ScriptStopping += OnScriptStopping;
        Bot.UltraBossHelper.DisableCounterAttack();

        _cts = new CancellationTokenSource();
        _runSkills = Task.Run(() => SkillsAsync(_cts.Token));

        Log("SKUA", "System online");

        Chill();

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
    }

    bool OnScriptStopping(Exception e)
    {
        Log("SKUA", "System offline");

        Bot.Lite.HidePlayers = false;

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

    #region Quest

    public void WaitQuest(int questId)
    {
        if (Bot.Quests.HasBeenCompleted(questId))
            return;

        if (!IsAvailable(questId))
            return;

        PreloadQuestAccept(questId);

        while (!CanCompleteFullCheck(questId) && !Bot.ShouldExit)
            Bot.Sleep(2000);

        if (CanCompleteFullCheck(questId))
        {
            Bot.Quests.Complete(questId);
            Bot.Wait.ForQuestComplete(questId);
        }
    }

    public void KillQuest(int questId, string map, string monster, string item, int quantity, string jumpCell)
        => KillQuestCore(questId, map, monster, item, quantity, true, false, false, jumpCell, "Left", false);

    public void KillQuest(int questId, string map, string monster, string item, int quantity, string jumpCell, string jumpPad)
        => KillQuestCore(questId, map, monster, item, quantity, true, false, false, jumpCell, jumpPad, false);

    public void KillQuest(int questId, string map, string monster, string item, int quantity = 1, bool isTemp = true, bool useBestGear = false, bool altJump = false)
        => KillQuestCore(questId, map, monster, item, quantity, isTemp, useBestGear, altJump, null, "Left", false);

    public void KillQuest(int questId, string map, string monster, string item, int quantity, bool isTemp, bool useBestGear, bool altJump, string? jumpCell, string jumpPad, bool priority)
        => KillQuestCore(questId, map, monster, item, quantity, isTemp, useBestGear, altJump, jumpCell, jumpPad, priority);

    private void KillQuestCore(
    int questId,
    string map,
    string monster,
    string item,
    int quantity,
    bool isTemp,
    bool useBestGear,
    bool altJump,
    string? jumpCell,
    string jumpPad = "Left",
    bool priority = false)
    {
        Log("KillQuest", $"BEGIN: questId={questId}, map={map}, monster={monster}, item={item}, " +
                         $"qtyArg={quantity}, isTemp={isTemp}, bestGear={useBestGear}, altJump={altJump}, " +
                         $"cell={(jumpCell ?? "null")}, pad={jumpPad}, priority={priority}");

        try
        {
            if (Bot.Quests.HasBeenCompleted(questId))
            {
                Log("KillQuest", $"Quest {questId} already completed → early return.");
                return;
            }

            if (!IsAvailable(questId))
            {
                Log("KillQuest", $"Quest {questId} not available → early return (see IsAvailable logs).");
                return;
            }

            Quest? quest = Bot.Quests.EnsureLoad(questId);
            if (quest is null)
            {
                Log("KillQuest", $"EnsureLoad failed: quest {questId} is null → abort.");
                return;
            }

            string slotInfo = quest.GetType().GetProperty("Slot") is null ? "" : $" Slot={quest.GetType().GetProperty("Slot")!.GetValue(quest)}";

            Log("KillQuest", $"Loaded quest: '{quest.Name}' [ID={quest.ID}{slotInfo}] " +
                             $"Reqs={quest.Requirements?.Count ?? 0}, AcceptReqs={quest.AcceptRequirements?.Count ?? 0}");

            int originalQty = quantity;
            if (quantity <= 1 && quest.Requirements is not null && quest.Requirements.Count > 0)
            {
                var match = quest.Requirements.FirstOrDefault(r =>
                    r?.Name != null &&
                    item != null &&
                    string.Equals(r.Name, item, StringComparison.OrdinalIgnoreCase));

                if (match is not null)
                {
                    Log("KillQuest", $"Requirement match found for '{item}': requiredQty={match.Quantity}");
                    if (match.Quantity > 0)
                        quantity = match.Quantity;
                    else
                        Log("KillQuest", $"Requirement quantity is 0 or invalid; keeping qty={quantity}");
                }
                else
                {
                    Log("KillQuest", $"No requirement match for '{item}' in quest requirements; keeping qty={quantity}");
                }
            }

            if (quantity != originalQty)
                Log("KillQuest", $"Quantity auto-adjusted: {originalQty} → {quantity}");

            Log("KillQuest", $"Preloading / accepting quest {questId}…");
            PreloadQuestAccept(questId);

            Log("KillQuest", $"Joining map: {map}…");
            Join(map);
            Log("KillQuest", $"Join done. CurrentRoom={Bot.Map.Name} RoomID={Bot.Map.RoomID}");

            Log("KillQuest", $"ForItem BEGIN → monsters='{monster}', map='{map}', key='{item}', " +
                             $"qty={quantity}, isTemp={isTemp}, bestGear={useBestGear}, alt={altJump}, " +
                             $"cell={(jumpCell ?? "null")}, pad={jumpPad}, priority={priority}");

            ForItem(monster, map, item, quantity, isTemp, useBestGear, altJump, jumpCell, jumpPad, priority);

            Log("KillQuest", "ForItem END");

            bool canComplete = CanCompleteFullCheck(questId);
            Log("KillQuest", $"Completion check for quest {questId}: {canComplete}");

            if (canComplete)
            {
                Log("KillQuest", "Chill() + EnsureComplete + Wait.ForQuestComplete + Sleep(500) sequence START");
                Chill();
                Bot.Quests.EnsureComplete(questId);
                Bot.Wait.ForQuestComplete(questId);
                Bot.Sleep(500);
                Log("KillQuest", "Turn-in sequence DONE");
            }
            else
            {
                Log("KillQuest", $"Cannot complete quest {questId} yet — requirements not fully met.");
            }
        }
        catch (Exception ex)
        {
            Log("KillQuest", $"ERROR: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            throw;
        }
        finally
        {
            Log("KillQuest", $"END questId={questId}");
        }
    }

    public bool IsAvailable(int id)
    {
        Quest? quest = Bot.Quests.EnsureLoad(id);

        if (quest is null)
        {
            Log("Quest", $"[{id}] not found.");
            return false;
        }

        if (Bot.Quests.IsDailyComplete(quest))
        {
            Log("Quest", $"{quest.Name} [{id}] is already marked as daily complete.");
            return false;
        }

        if (!Bot.Quests.IsUnlocked(quest))
        {
            Log("Quest", $"{quest.Name} [{id}] is locked.");
            return false;
        }

        if (quest.Upgrade && !Bot.Player.IsMember)
        {
            Log("Quest", $"{quest.Name} [{id}] requires membership.");
            return false;
        }

        if (Bot.Player.Level < quest.Level)
        {
            Log("Quest", $"{quest.Name} [{id}] requires level {quest.Level}, current {Bot.Player.Level}.");
            return false;
        }

        if (quest.RequiredClassID > 0)
        {
            int cp = Bot.Flash.CallGameFunction<int>("world.myAvatar.getCPByID", quest.RequiredClassID);
            if (cp < quest.RequiredClassPoints)
            {
                Log("Quest", $"{quest.Name} [{id}] requires {quest.RequiredClassPoints} CP, current {cp}.");
                return false;
            }
        }

        if (quest.RequiredFactionId > 1)
        {
            int rep = Bot.Flash.CallGameFunction<int>("world.myAvatar.getRep", quest.RequiredFactionId);
            if (rep < quest.RequiredFactionRep)
            {
                Log("Quest", $"{quest.Name} [{id}] requires faction rep {quest.RequiredFactionRep}, current {rep}.");
                return false;
            }
        }

        if (!quest.AcceptRequirements.All(r => Owned(r.Name, r.Quantity)))
        {
            Log("Quest", $"{quest.Name} [{id}] missing required items.");
            return false;
        }

        return true;
    }

    public bool CanCompleteFullCheck(int id)
    {
        if (Bot.Quests.CanComplete(id))
            return true;

        Quest? quest = Bot.Quests.EnsureLoad(id);
        if (quest is null)
        {
            Log("Quest", $"Quest [{id}] not found.");
            return false;
        }

        List<ItemBase> requirements = new();
        requirements.AddRange(quest.Requirements);
        requirements.AddRange(quest.AcceptRequirements);

        if (requirements.Count == 0)
            return true;

        foreach (ItemBase item in requirements)
        {
            if (Owned(item.Name, item.Quantity, false))
                continue;

            return false;
        }

        return true;
    }

    public void SwitchAlignment(int id)
    {
        string alignment = id switch
        {
            1 => "Good",
            2 => "Evil",
            3 => "Chaos",
            _ => "Unknown"
        };

        Bot.Send.Packet($"%xt%zm%updateQuest%{Bot.Map.RoomID}%41%{id}%");

        Log("Alignment", $"Switched to {alignment} ({id}).");
    }

    public bool IsMember(string npcName)
    {
        bool member = Bot.Player.IsMember;

        Log("Membership", $"Skipping {npcName} quests — requires membership.");
        return false;
    }

    public bool HasBeenCompleted(string storyName, int lastQuestId)
    {
        if (Bot.Quests.HasBeenCompleted(lastQuestId))
        {
            Log("Storyline", $"Skipping storyline '{storyName}' — already completed.");
            return false;
        }

        Log("Storyline", $"Starting storyline '{storyName}'.");
        return true;
    }

    private void PreloadQuestAccept(int questId)
    {
        int[] blacklistedQuestIds = { 2920, 2922, 2924, 2926, 2928, 2930 };

        if (!Bot.Quests.IsInProgress(questId))
        {
            bool hadNoActive = (Bot.Quests.Active?.Count ?? 0) == 0;
            var firstQ = Bot.Quests.EnsureLoad(questId);

            Bot.Quests.EnsureAccept(questId);
            Bot.Wait.ForQuestAccept(questId);

            if (hadNoActive && firstQ is not null && !blacklistedQuestIds.Contains(questId))
            {
                int slot = firstQ.Slot;
                var ids = Enumerable.Range(questId, 10).ToArray();
                Bot.Quests.Load(ids);
                Bot.Sleep(1000);

                var sameSlotIds = new List<int>();
                foreach (int id in ids)
                {
                    if (id == questId)
                        continue;

                    if (blacklistedQuestIds.Contains(id))
                        continue;

                    if (Bot.Quests.TryGetQuest(id, out var q) && q is not null && q.Slot == slot && !Bot.Quests.HasBeenCompleted(id))
                        sameSlotIds.Add(id);
                }

                foreach (int id in sameSlotIds)
                {
                    if (!Bot.Quests.IsInProgress(id) && !Bot.Quests.HasBeenCompleted(id) && !blacklistedQuestIds.Contains(id))
                    {
                        Bot.Quests.EnsureAccept(id);
                        Bot.Wait.ForQuestAccept(id);
                        Bot.Sleep(1000);
                    }
                }
            }
        }
    }

    #endregion

    #region Items

    public void ForItem(string monsters, string? map, object key, int quantity = 1, bool isTemp = false, bool useBestGear = false, bool alt = false, string? cell = null, string pad = "Left", bool priority = false)
    {
        if (key is null || quantity <= 0) return;

        var targets = (monsters ?? string.Empty)
            .Replace('|', ',')
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .Where(t => t.Length > 0)
            .ToArray();

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
        pickupKey();

        string keyLabel = key is int id2 ? (GetDropItem(id2)?.Name ?? $"Item#{id2}") : (key.ToString() ?? "Item");

        int haveNow = qty();
        if (haveNow >= quantity)
        {
            Log("FARMING", $"✅ Already have {haveNow}× {keyLabel} (need {quantity})");
            return;
        }

        if (targets.Length == 0)
        {
            Log("FARMING", "❌ No monster targets");
            DisableSkills();
            Chill();
            return;
        }

        if (!string.IsNullOrWhiteSpace(map)) Join(map);
        ChooseBestCell(monsters, alt, cell, pad);
        if (useBestGear) ChooseBestGear(monsters);

        Log("FARMING", $"⚔️ {string.Join(", ", targets)} → {quantity}× {keyLabel}");

        EnableSkills();
        int i = 0;

        while (!Bot.ShouldExit)
        {
            if (qty() >= quantity)
            {
                Log("SUCCESS", $"✅ Got {quantity}× {keyLabel}");
                DisableSkills();
                Chill();
                return;
            }

            pickupKey();

            if (priority)
                KillWithPriority(prioKeys);
            else
                Kill(targetKeys[i++ % targetKeys.Length]);
        }

        DisableSkills();
        Chill();
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

                if (equipped(k) && (maxed || Bot.Player.CurrentClassRank >= r))
                {
                    Log("CLASS", $"✅ Already using suitable class: {k}");
                    return;
                }

                Log("CLASS", $"🎓 Equipping: {k}");
                equip(k);
                Bot.Sleep(D3);
                return;
            }
            catch { }
        }

        Log("CLASS", "❌ No preferred class owned");
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
        try { Chill(); Bot.Bank.ToInventory(name); Bot.Sleep(D2); Log("BANK", $"🏦→🎒 {name}"); return true; } catch { Log("BANK", $"❌ Move failed: {name}"); return false; }
    }

    public bool InBank(int id)
    {
        if (id <= 0 || !Bot.Bank.Contains(id)) return false;
        try { Chill(); Bot.Bank.ToInventory(id); Bot.Sleep(D2); Log("BANK", $"🏦→🎒 #{id}"); return true; } catch { Log("BANK", $"❌ Move failed: #{id}"); return false; }
    }

    public bool ToBank(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || !Bot.Inventory.Contains(name)) { Log("BANK", $"❌ Not in inv: {name}"); return false; }
        try { Chill(); Bot.Inventory.ToBank(name); Bot.Sleep(D2); Log("BANK", $"🎒→🏦 {name}"); return true; } catch { Log("BANK", $"❌ Move failed: {name}"); return false; }
    }

    public bool ToBank(int id)
    {
        if (id <= 0 || !Bot.Inventory.Contains(id)) { Log("BANK", $"❌ Not in inv: #{id}"); return false; }
        try { Chill(); Bot.Inventory.ToBank(id); Bot.Sleep(D2); Log("BANK", $"🎒→🏦 #{id}"); return true; } catch { Log("BANK", $"❌ Move failed: #{id}"); return false; }
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
        if (curId <= 0 && string.IsNullOrWhiteSpace(curName)) { Log("CLASS", "❌ No current class found"); return; }

        var candidates = new List<InventoryItem>();
        foreach (var it in inv)
        {
            if (it == null) continue;
            if (!IsClass(it)) continue;
            if (it.Equipped == true) continue;
            candidates.Add(it);
        }
        if (candidates.Count == 0) { Log("CLASS", "❌ No alternate class available"); return; }

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
        if (!Bot.Inventory.IsEquipped(rnd.ID)) { Log("CLASS", $"❌ Failed to equip {rnd.Name}"); return; }

        Log("CLASS", $"🔀 Swapped to {rnd.Name}");
        if (holdMs > 0) Bot.Sleep(holdMs);

        if (curId > 0)
        {
            for (int t = 0; t < 3 && !Bot.ShouldExit; t++)
            {
                if (Bot.Inventory.IsEquipped(curId)) break;
                Bot.Inventory.EquipItem(curId);
                Bot.Sleep(500);
            }
            if (Bot.Inventory.IsEquipped(curId)) { Log("CLASS", $"↩️ Back to {curName ?? ("#" + curId)}"); return; }
        }

        if (!string.IsNullOrWhiteSpace(curName))
        {
            for (int t = 0; t < 3 && !Bot.ShouldExit; t++)
            {
                if (Bot.Inventory.IsEquipped(curName)) break;
                Bot.Inventory.EquipItem(curName);
                Bot.Sleep(500);
            }
            if (Bot.Inventory.IsEquipped(curName)) Log("CLASS", $"↩️ Back to {curName}");
            else Log("CLASS", $"❌ Failed to re-equip {curName}");
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
        foreach (var p in priority ?? Array.Empty<string>())
            if (!string.IsNullOrWhiteSpace(p)) wants.Add(p);

        Log("ENHANCEMENT", $"🛠️ {grp}: {string.Join(", ", wants)}");

        foreach (var want in wants)
        {
            // try inventory
            var hit = FindIn(Bot.Inventory.Items, want, grp, mem);
            if (hit != null)
            {
                if (Equip(hit)) { Log("ENHANCEMENT", $"✅ {grp}: {hit.Name} ({want})"); return hit; }
                Log("ENHANCEMENT", $"❌ Equip failed (inv): {hit.Name} ({want})");
            }

            // try bank
            var fromBank = FindIn(Bot.Bank.Items, want, grp, mem);
            if (fromBank != null)
            {
                Log("ENHANCEMENT", $"🏦 Pulling {fromBank.Name} ({want})");
                InBank(fromBank.Name);
                Bot.Sleep(500);

                InventoryItem pulled = Bot.Inventory.Items?.FirstOrDefault(i => i?.ID == fromBank.ID)
                                       ?? FindIn(Bot.Inventory.Items, want, grp, mem);

                if (Equip(pulled))
                {
                    var equipped = Bot.Inventory.Items?.FirstOrDefault(i => i != null && Bot.Inventory.IsEquipped(i.ID));
                    Log("ENHANCEMENT", $"✅ {grp}: {(equipped?.Name ?? pulled?.Name)} ({want})");
                    return equipped ?? pulled;
                }
                Log("ENHANCEMENT", $"❌ Equip failed (bank): {fromBank.Name} ({want})");
            }

            Bot.Sleep(500);
        }

        Log("ENHANCEMENT", $"🚫 No {grp} matched: {string.Join(", ", wants)}");
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
    public void KillAtMapId(int mapId) => Kill(MonsterKey.FromMapId(mapId));

    public void KillWithPriority(string primaryName, string priorityName1)
        => KillWithPriority(MonsterKey.FromName(priorityName1), MonsterKey.FromName(primaryName));
    public void KillWithPriority(int primaryId, int priorityId1)
        => KillWithPriority(MonsterKey.FromId(priorityId1), MonsterKey.FromId(primaryId));
    public void KillWithPriorityAtMapId(int primaryMapId, int priorityMapId1)
        => KillWithPriority(MonsterKey.FromMapId(priorityMapId1), MonsterKey.FromMapId(primaryMapId));

    public void KillWithPriority(string primaryName, string priorityName1, string priorityName2)
        => KillWithPriority(MonsterKey.FromName(priorityName1), MonsterKey.FromName(priorityName2), MonsterKey.FromName(primaryName));
    public void KillWithPriorityAtMapId(int primaryMapId, int priorityMapId1, int priorityMapId2)
        => KillWithPriority(MonsterKey.FromMapId(priorityMapId1), MonsterKey.FromMapId(priorityMapId2), MonsterKey.FromMapId(primaryMapId));

    // --- helpers -----------------------------------------------------------------

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

    public bool Faction(string name, int minRank = 0)
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

    public void EquipConsumable(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;

        DisableSkills();
        Chill();
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
                if (Equipped(g.Name))
                {
                    Log("GEAR", $"✅ Equipped {g.Name}");
                    return;
                }
            }
            Log("GEAR", $"❌ Failed to equip {g.Name}");
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
            Log("GEAR", $"🎯 Target race: {race}");
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

        if (bestAll.Count == 0 && bestRace.Count == 0)
        {
            Log("GEAR", "❌ No gear with bonuses found");
            return;
        }

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
            Log("GEAR", $"🧩 Combo: {bestA.Name} + {bestR.Name}");
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

        if (bestItem != null)
        {
            Log("GEAR", $"✨ Best: {bestItem.Name}");
            Equip(bestItem);
        }
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

    #region Shop

    public bool BuyItem(object itemKey, int shopId, string map, int quantity = 1, bool ensureMap = true, bool calculateRemaining = true, bool skipIfHaveEnough = true, bool considerBank = true, bool checkGold = true, bool checkLevel = true, bool checkInvSpace = true, int loadTimeoutMs = 5000)
    {
        if (itemKey is not (int or string)) { Log("SHOP", "❌ Invalid item key type."); return false; }
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
        if (!EnsureMapJoin(map)) { Log("SHOP", $"❌ Failed to join {map}"); return false; }
        if (!LoadShop(shopId)) { Log("SHOP", $"❌ Failed to load shop {shopId}"); return false; }

        var it = FindItem(itemKey);
        if (it == null) { Log("SHOP", $"❌ Item not found: {itemKey}"); return false; }

        string itemName = it.Name;
        if (string.IsNullOrWhiteSpace(itemName)) { Log("SHOP", "❌ Item has no valid name."); return false; }

        int need = Need(itemName, quantity);
        if (need == 0) return true;

        long price = (long)it.Cost * need;
        if (checkGold && Bot.Player.Gold < price)
        { Log("SHOP", $"💰 Not enough gold: need {price}, have {Bot.Player.Gold}"); return false; }

        if (checkLevel && Bot.Player.Level < it.Level)
        { Log("SHOP", $"⬆️ Level {it.Level}+ required"); return false; }

        if (checkInvSpace && !HasInvSpace())
        { Log("SHOP", "📦 Inventory full"); return false; }

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
        Log("SHOP", ok ? $"🛒 Purchased {gained}x {itemName}" : $"❌ Purchase failed: {itemName}");
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
                    foreach (var i in infos) if (i?.ID == id) return true;
                    return false;
                }
            case string s when !string.IsNullOrWhiteSpace(s):
                {
                    var names = Bot?.Drops?.CurrentDrops;
                    if (names != null) foreach (var n in names) if (string.Equals(n, s, StringComparison.OrdinalIgnoreCase)) return true;
                    var infos = Bot?.Drops?.CurrentDropInfos;
                    if (infos != null) foreach (var i in infos) if (string.Equals(i?.Name, s, StringComparison.OrdinalIgnoreCase)) return true;
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
                foreach (var i in infos) if (i?.ID == id) return i;
                break;
            case string s when !string.IsNullOrWhiteSpace(s):
                foreach (var i in infos) if (string.Equals(i?.Name, s, StringComparison.OrdinalIgnoreCase)) return i;
                break;
        }
        return null;
    }

    public void Pickup(params object[] keys)
    {
        if (keys == null || keys.Length == 0) return;
        foreach (var k in keys)
        {
            switch (k)
            {
                case int id when id > 0:
                    if (HasDrop(id))
                    {
                        Bot.Drops.Pickup(id);
                        Bot.Sleep(D1);
                    }
                    break;
                case string s when !string.IsNullOrWhiteSpace(s):
                    if (HasDrop(s))
                    {
                        Bot.Drops.Pickup(s);
                        Bot.Sleep(D1);
                    }
                    break;
            }
        }
    }

    public bool WaitForDrop(object key, int timeout = 30000)
    {
        if (key is not (int or string)) return false;
        long t0 = Environment.TickCount64;
        while (!Bot.ShouldExit && !HasDrop(key) && Environment.TickCount64 - t0 < timeout) Bot.Sleep(D1);
        return HasDrop(key);
    }

    public bool HasAny(params object[] keys)
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

    public double GetLowestHpPercentage()
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

    public bool IsArmyHealthLow(double percentage = 30.0)
        => GetLowestHpPercentage() < percentage;

    public bool InLoadedMap(string name) =>
       Bot?.Map?.Loaded == true && Bot.Map.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) == true;

    #endregion

    #region Map

    string _bestCell = null;
    string _bestPad = "Left";

    public void Join(string map, string cell = "Enter", string pad = "Spawn", bool publicRoom = false, int? roomNumber = null)
    {
        if (string.IsNullOrWhiteSpace(map) || Bot?.Map == null || Bot?.Player == null) return;

        string mapName = map.Split('-')[0].Trim();
        string target = publicRoom ? mapName
                      : roomNumber is int n ? $"{mapName}-{n}"
                      : map.Contains("-") ? map
                      : $"{mapName}-{GenerateRoomID()}";

        if (InLoadedMap(mapName))
        {
            if (!string.IsNullOrWhiteSpace(cell) && !IsInCell(cell)) Bot.Map.Jump(cell, pad);
            return;
        }

        Chill();

        for (int i = 0; i < 5 && !Bot.ShouldExit && !InLoadedMap(mapName); i++)
        {
            Bot.Send.Packet($"%xt%zm%cmd%{Bot.Map.RoomID}%tfer%{Bot.Player.Username}%{target}%{cell}%{pad}%");

            long end = Environment.TickCount64 + 8000; // up to 8s per try
            while (!Bot.ShouldExit && !InLoadedMap(mapName) && Environment.TickCount64 < end) Bot.Sleep(100);

            if (!InLoadedMap(mapName)) Bot.Sleep(300);
        }

        if (InLoadedMap(mapName))
        {
            Log("MAP", $"🌍 Joined {mapName} ({target})");
            if (!string.IsNullOrWhiteSpace(cell) && !IsInCell(cell)) Bot.Map.Jump(cell, pad);
        }
        else
        {
            Log("MAP", $"❌ Failed to join {mapName} ({target})");
        }

        int GenerateRoomID()
        {
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
        {
            Log("MAP", "❌ No matching monsters found");
            return;
        }

        string targetCell =
            !string.IsNullOrWhiteSpace(setCell) ? setCell
            : alt ? monsters.FirstOrDefault()?.Cell
            : monsters.GroupBy(m => m.Cell, StringComparer.Ordinal)
                      .OrderByDescending(g => g.Count())
                      .Select(g => g.Key)
                      .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(targetCell))
        {
            Log("MAP", "❌ No valid target cell");
            return;
        }

        var mapCells = new HashSet<string>(Bot.Map.Cells as IEnumerable<string> ?? Array.Empty<string>(),
                                           StringComparer.Ordinal);
        if (!mapCells.Contains(targetCell))
        {
            Log("MAP", $"❌ Cell not in map: {targetCell}");
            return;
        }

        _bestCell = targetCell;
        _bestPad = pad;

        if (!string.Equals(Bot.Player.Cell, targetCell, StringComparison.Ordinal))
        {
            Log("MAP", $"⁀➴ Jumping to '{targetCell}' ({pad})");
            Bot.Map.Jump(targetCell, pad);
            Bot.Player.SetSpawnPoint();
        }
        else
        {
            Log("MAP", $"✅ Already in {targetCell}");
        }
    }

    void WhiteMap() => Join("whitemap");

    #endregion

    #region Utils

    public void Chill()
    {
        if (Bot?.Combat == null || Bot?.Map == null || Bot?.Player == null) return;

        Bot.Combat.CancelAutoAttack();
        Bot.Combat.CancelTarget();

        var cells = Bot.Map.Cells ?? new List<string>();
        var mobs = Bot.Monsters?.MapMonsters ?? new List<Monster>();

        string safeCell = cells
            .Where(c => !string.IsNullOrWhiteSpace(c)
                     && !c.Equals("Wait", StringComparison.OrdinalIgnoreCase)
                     && !c.Equals("Blank", StringComparison.OrdinalIgnoreCase)
                     && !c.StartsWith("Cut", StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => mobs.Count(m => m?.Cell == c))
            .FirstOrDefault() ?? Bot.Player.Cell;

        string pad = string.IsNullOrWhiteSpace(Bot.Player.Pad) ? "Left" : Bot.Player.Pad;

        Log("CHILL", $"🍃 Safe cell: {safeCell} ({pad})");

        while (!Bot.ShouldExit && Bot.Player.State == 2)
        {
            if (!IsInCell(safeCell))
                Bot.Map.Jump(safeCell, pad);

            Bot.Sleep(D1);
        }

        Bot.Sleep(D3);
    }

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
}