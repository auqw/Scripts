/*
name: null
description: null
tags: null
*/

//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreFarms2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreAdvanced2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreStory2.cs

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Auras;
using Skua.Core.Models.Factions;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Players;
using Skua.Core.Models.Quests;
using Skua.Core.Models.Shops;
using Skua.Core.Models.Skills;

public class CoreEngine2
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreAdvanced2 Adv
    {
        get => _Adv ??= new CoreAdvanced2();
        set => _Adv = value;
    }
    private static CoreAdvanced2 _Adv;
    private CoreBots2 C => CoreBots2.Instance;

    readonly ConcurrentDictionary<string, object?> _cache = new();
    readonly ConcurrentDictionary<string, DateTime> _throttle = new();

    CancellationTokenSource? _cts;
    Task? _runSkills;

    private static CoreEngine2? _instance;
    public static CoreEngine2 Instance => _instance ??= new CoreEngine2();

    public TimeSpan ThrottleDuration { get; set; } = TimeSpan.FromSeconds(3);
    public event Action<string, string>? OnSignal;

    #region Settings

    public int D1 = 250;
    public int D2 = 700;
    public int D3 = 1400;
    public int D4 = 2800;

    bool NotUltraDage()
    {
        string mapName = Bot.Map.Name ?? "";
        string targetName = Bot.Player.Target?.Name ?? "";
        bool onDageMap = mapName.Equals("ultradage", StringComparison.OrdinalIgnoreCase);
        bool targetingDage = targetName.Equals("Dage the Dark Lord", StringComparison.OrdinalIgnoreCase);
        return !(onDageMap || targetingDage);
    }

    bool NotUltraDarkon()
    {
        string mapName = Bot.Map.Name ?? "";
        string targetName = Bot.Player.Target?.Name ?? "";
        bool onDarkonMap = mapName.Equals("ultradarkon", StringComparison.OrdinalIgnoreCase);
        bool targetingDarkon = targetName.Equals("Darkon the Conductor", StringComparison.OrdinalIgnoreCase);
        return !(onDarkonMap || targetingDarkon);
    }

    bool NotUltraSpeaker()
    {
        string mapName = Bot.Map.Name ?? "";
        string targetName = Bot.Player.Target?.Name ?? "";
        bool onSpeakerMap = mapName.Equals("ultraspeaker", StringComparison.OrdinalIgnoreCase);
        bool targetingSpeaker = targetName.Equals("The First Speaker", StringComparison.OrdinalIgnoreCase);
        return !(onSpeakerMap || targetingSpeaker);
    }

    bool NotKolr()
    {
        string mapName = Bot.Map.Name ?? "";
        string targetName = Bot.Player.Target?.Name ?? "";
        bool onKolrMap = mapName.Contains("flameusurper", StringComparison.OrdinalIgnoreCase);
        bool targetingKolr = targetName.Contains("Kolr", StringComparison.OrdinalIgnoreCase);
        return !(onKolrMap || targetingKolr);
    }

    bool KolrLowHp()
    {
        if (NotKolr())
            return false;

        var target = Bot.Player.Target;
        if (target == null || target.MaxHP <= 0)
            return false;

        return (double)target.HP / target.MaxHP * 100 < 70;
    }

    public void Boot()
    {
        if (_runSkills?.Status == TaskStatus.Running)
            return;

        OnSignal += (category, message) =>
        {
            Bot.Log($"[{category}] {message}");
        };

        Bot.Events.ScriptStopping += OnScriptStopping;
        Bot.UltraBossHelper.DisableCounterAttack();

        Bot.Skills.Stop();
        _cts = new CancellationTokenSource();
        _runSkills = Task.Run(() => SkillsAsync(_cts.Token));
        Log("SKUA", "System online");

        Chill();

        Bot.Bank.Open();

        if (Bot.Bank.Items == null || Bot.Bank.Items.Count == 0)
        {
            Bot.Bank.Load();
            Bot.Wait.ForTrue(() => (Bot.Bank.Items?.Count ?? 0) > 0, 20);
        }

        Bot.Options.SafeTimings = true;
        Bot.Options.InfiniteRange = true;
        Bot.Options.SkipCutscenes = true;
        Bot.Lite.HidePlayers = true;
        C.Join("whitemap-100000");
    }

    public void BootNoSkills()
    {
        // Properly kill any lingering SkillsAsync task from a previous run
        try
        {
            _cts?.Cancel();
        }
        catch { }
        if (_runSkills != null)
        {
            try { _runSkills.Wait(1000); } catch { }
            _runSkills = null;
        }
        try { _cts?.Dispose(); } catch { }
        _cts = null;

        OnSignal += (category, message) =>
        {
            Bot.Log($"[{category}] {message}");
        };

        Bot.Events.ScriptStopping += OnScriptStopping;
        Bot.UltraBossHelper.DisableCounterAttack();

        Bot.Skills.Stop();
        Log("SKUA", "System online");

        Chill();

        Bot.Bank.Open();

        if (Bot.Bank.Items == null || Bot.Bank.Items.Count == 0)
        {
            Bot.Bank.Load();
            Bot.Wait.ForTrue(() => (Bot.Bank.Items?.Count ?? 0) > 0, 20);
        }

        Bot.Options.SafeTimings = true;
        Bot.Options.InfiniteRange = true;
        Bot.Options.SkipCutscenes = true;
        Bot.Lite.HidePlayers = true;
        C.Join("whitemap-100000");
    }

    bool OnScriptStopping(Exception? e)
    {
        DisableSkills();
        C.JumpWait();
        C.Logger("System offline");

        Bot.Lite.HidePlayers = false;
        OnSignal = null;

        _cache.Clear();
        _throttle.Clear();
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

        while (!CanCompleteFullCheck(questId) && !Bot.ShouldExit)
            Bot.Sleep(2000);

        if (CanCompleteFullCheck(questId))
        {
            Bot.Quests.Complete(questId);
            Bot.Wait.ForQuestComplete(questId);
        }
    }

    public void KillQuest(int questId, string map, string monster) =>
        KillQuestCore(
            questId,
            map,
            monster, /*item*/
            "", /*quantity*/
            1, /*isTemp*/
            true,
            /*useBestGear*/false, /*altJump*/
            false, /*jumpCell*/
            null, /*pad*/
            "Left", /*priority*/
            false
        );

    public void KillQuest(int questId, string map, string monster, string jumpCell) =>
        KillQuestCore(
            questId,
            map,
            monster, /*item*/
            "", /*quantity*/
            1, /*isTemp*/
            true,
            /*useBestGear*/false, /*altJump*/
            false, /*jumpCell*/
            jumpCell, /*pad*/
            "Left", /*priority*/
            false
        );

    public void KillQuest(
        int questId,
        string map,
        string monster,
        string jumpCell,
        string jumpPad
    ) =>
        KillQuestCore(
            questId,
            map,
            monster, /*item*/
            "", /*quantity*/
            1, /*isTemp*/
            true,
            /*useBestGear*/false, /*altJump*/
            false, /*jumpCell*/
            jumpCell, /*pad*/
            jumpPad, /*priority*/
            false
        );

    public void KillQuest(
        int questId,
        string map,
        string monster,
        int quantity = 1,
        bool isTemp = true,
        bool useBestGear = false,
        bool altJump = false
    ) =>
        KillQuestCore(
            questId,
            map,
            monster, /*item*/
            "",
            quantity,
            isTemp,
            useBestGear,
            altJump,
            /*jumpCell*/null, /*pad*/
            "Left", /*priority*/
            false
        );

    public void KillQuest(
        int questId,
        string map,
        string monster,
        int quantity,
        bool isTemp,
        bool useBestGear,
        bool altJump,
        string? jumpCell,
        string jumpPad,
        bool priority
    ) =>
        KillQuestCore(
            questId,
            map,
            monster, /*item*/
            "",
            quantity,
            isTemp,
            useBestGear,
            altJump,
            jumpCell,
            jumpPad,
            priority
        );

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
        bool priority = false
    )
    {
        if (!IsAvailable(questId))
            return;

        var q = Bot.Quests.EnsureLoad(questId);
        if (q is null)
            return;

        if (string.IsNullOrWhiteSpace(item))
        {
            var reqs = q.Requirements;
            if (reqs is null || reqs.Count == 0)
                return;
            if (reqs.Count != 1 || string.IsNullOrWhiteSpace(reqs[0]?.Name))
                return;
            item = reqs[0]!.Name!;
        }

        if (quantity <= 1 && q.Requirements is not null)
        {
            var r = q.Requirements.FirstOrDefault(x =>
                !string.IsNullOrWhiteSpace(x?.Name)
                && x!.Name.Equals(item, StringComparison.OrdinalIgnoreCase)
            );
            if (r is not null && r.Quantity > 0)
                quantity = r.Quantity;
        }

        PreloadQuestAccept(questId);
        Join(map);

        ForItem(
            monster,
            map,
            item,
            quantity,
            isTemp,
            useBestGear,
            altJump,
            jumpCell,
            jumpPad,
            priority
        );

        if (CanCompleteFullCheck(questId))
        {
            Chill(false);
            Bot.Quests.EnsureComplete(questId);
            Bot.Wait.ForQuestComplete(questId);
        }
    }

    public bool IsAvailable(int id)
    {
        Quest? quest = Bot.Quests.EnsureLoad(id);

        if (quest is null)
        {
            Log("QUEST", $"[{id}] not found.");
            return false;
        }

        if (Bot.Quests.IsDailyComplete(quest))
        {
            Log("QUEST", $"{quest.Name} [{id}] is already marked as daily complete.");
            return false;
        }

        if (!Bot.Quests.IsUnlocked(quest))
        {
            Log("QUEST", $"{quest.Name} [{id}] is locked.");
            return false;
        }

        if (quest.Upgrade && !Bot.Player.IsMember)
        {
            Log("QUEST", $"{quest.Name} [{id}] requires membership.");
            return false;
        }

        if (Bot.Player.Level < quest.Level)
        {
            Log(
                "QUEST",
                $"{quest.Name} [{id}] requires level {quest.Level}, current {Bot.Player.Level}."
            );
            return false;
        }

        if (quest.RequiredClassID > 0)
        {
            int cp = Bot.Flash.CallGameFunction<int>(
                "world.myAvatar.getCPByID",
                quest.RequiredClassID
            );
            if (cp < quest.RequiredClassPoints)
            {
                Log(
                    "QUEST",
                    $"{quest.Name} [{id}] requires {quest.RequiredClassPoints} CP, current {cp}."
                );
                return false;
            }
        }

        if (quest.RequiredFactionId > 1)
        {
            int rep = Bot.Flash.CallGameFunction<int>(
                "world.myAvatar.getRep",
                quest.RequiredFactionId
            );
            if (rep < quest.RequiredFactionRep)
            {
                Log(
                    "QUEST",
                    $"{quest.Name} [{id}] requires faction rep {quest.RequiredFactionRep}, current {rep}."
                );
                return false;
            }
        }

        if (!quest.AcceptRequirements.All(r => Owned(r.Name, r.Quantity)))
        {
            Log("QUEST", $"{quest.Name} [{id}] missing required items.");
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
            Log("QUEST", $"Quest [{id}] not found.");
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
            _ => "Unknown",
        };

        Bot.Send.Packet($"%xt%zm%updateQuest%{Bot.Map.RoomID}%41%{id}%");

        Log("ALIGNMENT", $"Switched to {alignment} ({id}).");
    }

    public bool HasBeenCompleted(string storyName, int lastQuestId)
    {
        if (Bot.Quests.HasBeenCompleted(lastQuestId))
        {
            Log("STORY", $"Skipping storyline '{storyName}' — already completed.");
            return false;
        }

        Log("STORY", $"Starting storyline '{storyName}'.");
        return true;
    }

    private void PreloadQuestAccept(int questId)
    {
        if (Bot.Quests.IsInProgress(questId))
            return;

        var q0 = Bot.Quests.EnsureLoad(questId);
        if (q0 is null || (q0.Upgrade && !Bot.Player.IsMember))
            return;

        Bot.Quests.EnsureAccept(questId);
        Bot.Wait.ForQuestAccept(questId);

        var ids = Enumerable.Range(questId, 10).ToArray();

        for (int tries = 0; tries < 3; tries++)
        {
            Bot.Quests.Load(ids);
            Bot.Sleep(500);
        }

        int slot = q0.Slot;
        var seenReqs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var r0 = q0.Requirements?.FirstOrDefault()?.Name;
        if (!string.IsNullOrWhiteSpace(r0))
            seenReqs.Add(r0);

        bool changed;
        do
        {
            changed = false;
            foreach (int id in ids)
            {
                if (id == questId)
                    continue;

                if (
                    Bot.Quests.TryGetQuest(id, out var q)
                    && q is not null
                    && q.Slot == slot
                    && q.Value != -1
                    && !(q.Upgrade && !Bot.Player.IsMember)
                    && !Bot.Quests.IsInProgress(id)
                )
                {
                    var reqName = q.Requirements?.FirstOrDefault()?.Name;
                    if (!string.IsNullOrWhiteSpace(reqName) && seenReqs.Contains(reqName))
                        continue; // same requirement already active

                    Bot.Quests.EnsureAccept(id);
                    Bot.Wait.ForQuestAccept(id);
                    Bot.Sleep(700);
                    if (!string.IsNullOrWhiteSpace(reqName))
                        seenReqs.Add(reqName);
                    changed = true;
                }
            }
        } while (changed);
    }

    #endregion

    #region Items

    public void ForItem(
        string monsters,
        string? map,
        object key,
        int quantity = 1,
        bool isTemp = false,
        bool useBestGear = false,
        bool alt = false,
        string? cell = null,
        string pad = "Left",
        bool priority = false
    )
    {
        if (key is null || quantity <= 0)
            return;

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
            _ => () => 0,
        };

        Action pullFromBank = () =>
        {
            if (isTemp)
                return;
            if (key is int id)
                InBank(id);
            else if (key is string s)
                InBank(s);
        };

        Action pickupKey = () =>
        {
            if (key is int id)
                Pickup(id);
            else if (key is string s)
                Pickup(s);
        };

        pullFromBank();
        pickupKey();

        string keyLabel = key is int id2
            ? (GetDropItem(id2)?.Name ?? $"Item#{id2}")
            : (key.ToString() ?? "Item");

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

        if (!string.IsNullOrWhiteSpace(map))
            Join(map);
        ChooseBestCell(monsters, alt, cell, pad);
        if (useBestGear)
            ChooseBestGear(monsters);

        Log("FARMING", $"⚔️ {string.Join(", ", targets)} → {quantity}× {keyLabel}");

        EnableSkills();

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
                KillAmong(targetKeys);
        }

        DisableSkills();
        Chill();
    }

    void EquipBestClassCore<T>(
        IEnumerable<(T key, int rank)> prefs,
        Func<T, bool> owned,
        Func<T, bool> equipped,
        Action<T> equip
    )
    {
        if (prefs == null)
            return;
        bool maxed = IsCurrentClassMaxRank();

        foreach (var (k, r) in prefs)
        {
            try
            {
                if (!owned(k))
                    continue;

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
        EquipBestClassCore(
            priorities,
            owned: n => !string.IsNullOrWhiteSpace(n) && Owned(n, 1),
            equipped: n => HasClassEquipped(n),
            equip: n =>
            {
                if (!Bot.Inventory.IsEquipped(n))
                    Bot.Inventory.EquipItem(n);
            }
        );

    public void EquipBestClass(List<(int id, int rank)> priorities) =>
        EquipBestClassCore(
            priorities,
            owned: id => (Bot.Inventory.Items?.Any(i => i?.ID == id) ?? false),
            equipped: id =>
            {
                var items = Bot.Inventory.Items;
                var it = items?.FirstOrDefault(i => i?.ID == id);
                return it != null && HasClassEquipped(it.Name);
            },
            equip: id =>
            {
                if (!Bot.Inventory.IsEquipped(id))
                    Bot.Inventory.EquipItem(id);
            }
        );

    public bool InBank(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || !Bot.Bank.Contains(name))
            return false;
        try
        {
            Chill();
            Bot.Bank.ToInventory(name);
            Bot.Wait.ForBankToInventory(name);
            Log("BANK", $"🏦→🎒 {name}");
            return true;
        }
        catch
        {
            Log("BANK", $"❌ Move failed: {name}");
            return false;
        }
    }

    public bool InBank(int id)
    {
        if (id <= 0 || !Bot.Bank.Contains(id))
            return false;
        try
        {
            Chill();
            Bot.Bank.ToInventory(id);
            Bot.Wait.ForBankToInventory(id);
            Log("BANK", $"🏦→🎒 #{id}");
            return true;
        }
        catch
        {
            Log("BANK", $"❌ Move failed: #{id}");
            return false;
        }
    }

    public bool ToBank(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || !Bot.Inventory.Contains(name))
        {
            Log("BANK", $"❌ Not in inv: {name}");
            return false;
        }
        try
        {
            Chill();
            Bot.Inventory.ToBank(name);
            Bot.Wait.ForInventoryToBank(name);
            Log("BANK", $"🎒→🏦 {name}");
            return true;
        }
        catch
        {
            Log("BANK", $"❌ Move failed: {name}");
            return false;
        }
    }

    public bool ToBank(int id)
    {
        if (id <= 0 || !Bot.Inventory.Contains(id))
        {
            Log("BANK", $"❌ Not in inv: #{id}");
            return false;
        }
        try
        {
            Chill();
            Bot.Inventory.ToBank(id);
            Bot.Wait.ForInventoryToBank(id);
            Log("BANK", $"🎒→🏦 #{id}");
            return true;
        }
        catch
        {
            Log("BANK", $"❌ Move failed: #{id}");
            return false;
        }
    }

    public int Owned(string name, bool isTemp = false) =>
        string.IsNullOrWhiteSpace(name)
            ? 0
            : (
                isTemp ? Bot.TempInv?.GetQuantity(name) ?? 0 : Bot.Inventory?.GetQuantity(name) ?? 0
            );

    public int Owned(int id, bool isTemp = false) =>
        id <= 0
            ? 0
            : (isTemp ? Bot.TempInv?.GetQuantity(id) ?? 0 : Bot.Inventory?.GetQuantity(id) ?? 0);

    public bool Owned(string name, int quantity, bool isTemp = false) =>
        Owned(name, isTemp) >= quantity;

    public bool Owned(int id, int quantity, bool isTemp = false) => Owned(id, isTemp) >= quantity;

    public void EquipRandomClassAndReequip(int holdMs = 1000)
    {
        bool IsClass(InventoryItem it)
        {
            try
            {
                if (it is ItemBase ib)
                {
                    if (ib.Category == ItemCategory.Class)
                        return true;
                    if (
                        !string.IsNullOrWhiteSpace(ib.CategoryString)
                        && ib.CategoryString.Equals("Class", StringComparison.OrdinalIgnoreCase)
                    )
                        return true;
                }
            }
            catch { }

            var catProp = it.GetType().GetProperty("CategoryString");
            if (catProp != null)
            {
                var cs = catProp.GetValue(it) as string;
                if (
                    !string.IsNullOrWhiteSpace(cs)
                    && cs.Equals("Class", StringComparison.OrdinalIgnoreCase)
                )
                    return true;
            }

            return false;
        }

        var inv = Bot.Inventory.Items;
        if (inv == null)
            return;

        int curId = -1;
        string? curName = null;
        foreach (var it in inv)
        {
            if (it == null)
                continue;
            if (it.Equipped == true && IsClass(it))
            {
                curId = it.ID;
                curName = it.Name;
                break;
            }
        }
        if (curId <= 0 && string.IsNullOrWhiteSpace(curName))
        {
            Log("CLASS", "❌ No current class found");
            return;
        }

        var candidates = new List<InventoryItem>();
        foreach (var it in inv)
        {
            if (it == null)
                continue;
            if (!IsClass(it))
                continue;
            if (it.Equipped == true)
                continue;
            candidates.Add(it);
        }
        if (candidates.Count == 0)
        {
            Log("CLASS", "❌ No alternate class available");
            return;
        }

        var rng = new Random(unchecked((int)Environment.TickCount));
        var rnd = candidates[rng.Next(0, candidates.Count)];

        if (!Bot.Inventory.IsEquipped(rnd.ID))
        {
            for (int t = 0; t < 3 && !Bot.ShouldExit; t++)
            {
                Bot.Inventory.EquipItem(rnd.ID);
                Bot.Sleep(500);
                if (Bot.Inventory.IsEquipped(rnd.ID))
                    break;
            }
        }
        if (!Bot.Inventory.IsEquipped(rnd.ID))
        {
            Log("CLASS", $"❌ Failed to equip {rnd.Name}");
            return;
        }

        Log("CLASS", $"🔀 Swapped to {rnd.Name}");
        if (holdMs > 0)
            Bot.Sleep(holdMs);

        if (curId > 0)
        {
            for (int t = 0; t < 3 && !Bot.ShouldExit; t++)
            {
                if (Bot.Inventory.IsEquipped(curId))
                    break;
                Bot.Inventory.EquipItem(curId);
                Bot.Sleep(500);
            }
            if (Bot.Inventory.IsEquipped(curId))
            {
                Log("CLASS", $"↩️ Back to {curName ?? ("#" + curId)}");
                return;
            }
        }

        if (!string.IsNullOrWhiteSpace(curName))
        {
            for (int t = 0; t < 3 && !Bot.ShouldExit; t++)
            {
                if (Bot.Inventory.IsEquipped(curName))
                    break;
                Bot.Inventory.EquipItem(curName);
                Bot.Sleep(500);
            }
            if (Bot.Inventory.IsEquipped(curName))
                Log("CLASS", $"↩️ Back to {curName}");
            else
                Log("CLASS", $"❌ Failed to re-equip {curName}");
        }
    }

    #endregion

    #region Best Enhancement

    public InventoryItem? ChooseBestEnhancement(string itemGroup, params string[] priority)
    {
        if (priority == null || priority.Length == 0)
            return null;
        if (Bot?.Inventory == null || Bot.Bank == null || Bot.Player == null)
            return null;

        string Norm(string g)
        {
            if (string.IsNullOrWhiteSpace(g))
                return g;
            return g.Trim().ToLowerInvariant() switch
            {
                "weapon" => "Weapon",
                "helm" or "he" => "he",
                "back" or "ba" or "cape" => "ba",
                "class" or "co" => "co",
                "pet" or "pe" => "pe",
                _ => g,
            };
        }

        int N(int? v, int d = -1) => v ?? d;

        string? Enh(int id) =>
            id switch
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
                _ => null,
            };

        string? WeaponTrait(int id) =>
            id switch
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
                _ => null,
            };

        bool GroupMatch(InventoryItem i, string grp)
        {
            var g = i?.ItemGroup;
            return !string.IsNullOrWhiteSpace(g)
                && grp != null
                && g.Equals(grp, StringComparison.OrdinalIgnoreCase);
        }

        bool MatchWant(InventoryItem i, string want, string grp)
        {
            if (i == null || string.IsNullOrWhiteSpace(want))
                return false;

            var pat = Enh(N(i.EnhancementPatternID));
            if (!string.IsNullOrEmpty(pat) && pat.Equals(want, StringComparison.OrdinalIgnoreCase))
                return true;

            if (grp.Equals("Weapon", StringComparison.OrdinalIgnoreCase))
            {
                var tr = WeaponTrait(N(i.ProcID));
                if (
                    !string.IsNullOrEmpty(tr) && tr.Equals(want, StringComparison.OrdinalIgnoreCase)
                )
                    return true;
            }
            return false;
        }

        InventoryItem? FindIn(
            IEnumerable<InventoryItem> src,
            string want,
            string grp,
            bool memOnlyIfUpgradeAllowed
        )
        {
            if (src == null)
                return null;
            foreach (var i in src)
            {
                if (i == null)
                    continue;
                if (!GroupMatch(i, grp))
                    continue;
                if (i.Upgrade && !memOnlyIfUpgradeAllowed)
                    continue;
                if (MatchWant(i, want, grp))
                    return i;
            }
            return null;
        }

        bool Equip(InventoryItem? it)
        {
            if (it == null)
                return false;
            if (Bot.Inventory.IsEquipped(it.ID))
                return true;
            for (int t = 0; t < 3 && !Bot.ShouldExit; t++)
            {
                Bot.Inventory.EquipItem(it.ID);
                Bot.Sleep(500);
                if (Bot.Inventory.IsEquipped(it.ID))
                    return true;
            }
            return false;
        }

        var grp = Norm(itemGroup);
        bool mem = Bot.Player.IsMember == true;

        var wants = new List<string>();
        foreach (var p in priority ?? Array.Empty<string>())
            if (!string.IsNullOrWhiteSpace(p))
                wants.Add(p);

        Log("ENHANCEMENT", $"🛠️ {grp}: {string.Join(", ", wants)}");

        foreach (var want in wants)
        {
            // try inventory
            var hit = FindIn(Bot.Inventory.Items, want, grp, mem);
            if (hit != null)
            {
                if (Equip(hit))
                {
                    Log("ENHANCEMENT", $"✅ {grp}: {hit.Name} ({want})");
                    return hit;
                }
                Log("ENHANCEMENT", $"❌ Equip failed (inv): {hit.Name} ({want})");
            }

            // try bank
            var fromBank = FindIn(Bot.Bank.Items, want, grp, mem);
            if (fromBank != null)
            {
                Log("ENHANCEMENT", $"🏦 Pulling {fromBank.Name} ({want})");
                InBank(fromBank.Name);
                Bot.Sleep(500);

                InventoryItem? pulled =
                    Bot.Inventory.Items.FirstOrDefault(i => i?.ID == fromBank.ID) ?? FindIn(
                        Bot.Inventory.Items,
                        want,
                        grp,
                        mem
                    );

                if (pulled == null)
                {
                    Log(
                        "ENHANCEMENT",
                        $"❌ Pull failed (not in inventory): {fromBank.Name} ({want})"
                    );
                }
                else if (Equip(pulled))
                {
                    var equipped = Bot.Inventory.Items?.FirstOrDefault(i =>
                        i != null && Bot.Inventory.IsEquipped(i.ID)
                    );
                    Log("ENHANCEMENT", $"✅ {grp}: {equipped?.Name ?? pulled?.Name} ({want})");
                    return equipped ?? pulled;
                }
                else
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
        public static MonsterKey FromName(string? name)
        {
            if (name != null)
                return new(Name: name);
            return new();
        }

        public static MonsterKey? FromId(int? id)
        {
            if (id != null)
                return new(Id: id);
            return null;
        }

        public static MonsterKey? FromMapId(int? mapId)
        {
            if (mapId != null)
                return new(MapId: mapId);
            return null;
        }
    }

    IEnumerable<Monster> Match(MonsterKey k)
    {
        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

        var list = Bot?.Monsters?.MapMonsters;
        if (list == null)
            yield break;

        foreach (var m in list)
        {
            if (m == null)
                continue;
            if (k.MapId.HasValue && m.MapID != k.MapId.Value)
                continue;
            if (
                !string.IsNullOrWhiteSpace(k.Name)
                && !string.Equals(m.Name, k.Name, StringComparison.OrdinalIgnoreCase)
            )
                continue;
            if (k.Id.HasValue && m.ID != k.Id.Value)
                continue;
            yield return m;
        }
    }

    public bool IsMonsterAliveByName(string monName)
    {
        bool isAlive = false;

        monName = monName.ToLower();

        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
        try
        {
            string? jsonData = Bot.Flash.Call("availableMonsters");
            if (string.IsNullOrEmpty(jsonData))
            {
                Bot.Log("No monster data available.");
                return false;
            }
            var monsters = JArray.Parse(jsonData);
            if (monsters.Count == 0)
            {
                return false;
            }

            foreach (var monster in monsters)
            {
                if (monster["strMonName"]?.ToString().ToLower() == monName)
                {
                    var intState = monster["intState"]?.ToString();

                    if (string.IsNullOrEmpty(intState) || intState == "1" || intState == "2")
                    {
                        isAlive = true;
                    }
                }
            }

            return isAlive;
        }
        catch
        {
            return true;
        }
    }

    public bool IsAlive(MonsterKey k)
    {
        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

        foreach (var m in Match(k))
            if (m.Alive)
                return true;
        return false;
    }

    public bool IsAliveByMapId(int? mapId = null, string? name = null, int? id = null)
    {
        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
        return IsAlive(new MonsterKey(mapId, name, id));
    }

    void Attack(MonsterKey k)
    {
        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
        if (k.MapId.HasValue)
            Bot.Combat.Attack(k.MapId.Value);
        else if (k.Id.HasValue)
            Bot.Combat.Attack(k.Id.Value);
        else if (!string.IsNullOrWhiteSpace(k.Name))
            Bot.Combat.Attack(k.Name);
    }

    public void Kill(MonsterKey k)
    {
        var target = Match(k)
            .Where(m => m.Alive)
            .OrderBy(m => m.HP)
            .ThenBy(m => m.MapID)
            .FirstOrDefault();

        if (target == null)
            return;

        var hpKey = MonsterKey.FromMapId(target.MapID);
        Attack(hpKey!);
        Bot.Sleep(D1);
    }

    public void KillAmong(params MonsterKey[] keys)
    {
        if (keys == null || keys.Length == 0)
            return;

        var myCell = Bot.Player.Cell;

        var target = keys.SelectMany(k => Match(k))
            .Where(m =>
                m.Alive && string.Equals(m.Cell, myCell, StringComparison.OrdinalIgnoreCase)
            )
            .OrderBy(m => m.HP)
            .ThenBy(m => m.MapID)
            .FirstOrDefault();

        if (target == null)
            return;

        var hpKey = MonsterKey.FromMapId(target.MapID);
        Attack(hpKey!);
        Bot.Sleep(D1);
    }

    Monster? LowestHpTarget(params MonsterKey[] keys)
    {
        Monster? best = null;
        foreach (var k in keys)
        {
            foreach (var m in Match(k))
            {
                if (!m.Alive)
                    continue;
                if (best == null || m.HP < best.HP)
                    best = m;
            }
        }
        return best;
    }

    public void KillWithPriority(params MonsterKey[] keys)
    {
        if (keys == null || keys.Length == 0)
        {
            Bot.Sleep(D1);
            return;
        }

        foreach (var k in keys)
        {
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
            if (IsAlive(k))
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                Attack(k);
                Bot.Sleep(D1);
                return;
            }
        }

        Bot.Sleep(D1);
    }

    public void Kill(string name) => Kill(MonsterKey.FromName(name));

    public void Kill(params string[]? names)
    {
        if (names == null || names.Length == 0)
            return;
        var tmp = new List<MonsterKey>(names.Length);
        foreach (var n in names)
            if (!string.IsNullOrWhiteSpace(n))
                tmp.Add(MonsterKey.FromName(n));
        if (tmp.Count == 0)
            return;
        KillWithPriority(tmp.ToArray());
    }

    public void KillAtMapId(int? mapId)
    {
        var key = MonsterKey.FromMapId(mapId);
        if (key != null)
            Kill(key);
    }

    public void KillWithPriority(string? primaryName, string? priorityName1) =>
        KillWithPriority(MonsterKey.FromName(priorityName1)!, MonsterKey.FromName(primaryName)!);

    public void KillWithPriority(int? primaryId, int? priorityId1) =>
        KillWithPriority(MonsterKey.FromId(priorityId1)!, MonsterKey.FromId(primaryId)!);

    public void KillWithPriorityAtMapId(int? primaryMapId, int? priorityMapId1) =>
        KillWithPriority(
            MonsterKey.FromMapId(priorityMapId1)!,
            MonsterKey.FromMapId(primaryMapId)!
        );

    public void KillWithPriority(
        string? primaryName = null,
        string? priorityName1 = null,
        string? priorityName2 = null
    ) =>
        KillWithPriority(
            MonsterKey.FromName(priorityName1),
            MonsterKey.FromName(priorityName2),
            MonsterKey.FromName(primaryName)
        );

    public void KillWithPriorityAtMapId(int primaryMapId, int priorityMapId1, int priorityMapId2) =>
        KillWithPriority(
            MonsterKey.FromMapId(priorityMapId1)!,
            MonsterKey.FromMapId(priorityMapId2)!,
            MonsterKey.FromMapId(primaryMapId)!
        );

    #endregion

    #region Factions

    int Rank(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return 0;
        var list = Bot?.Reputation?.FactionList;
        if (list == null)
            return 0;

        foreach (var f in list)
        {
            var n = f?.Name;
            if (
                !string.IsNullOrWhiteSpace(n)
                && string.Equals(n, name, StringComparison.OrdinalIgnoreCase)
            )
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
        Chill();
        Bot.Sleep(D2);
        Cast(5);
        Bot.Sleep(D2);
    }

    public void EquipConsumable(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        Chill();
        if (!C.CheckInventory(name))
            return;

        WhiteMap();
        TryEquipConsumableItem(name);
    }

    public void EquipEnrage()
    {
        const string scroll = "Scroll of Enrage";

        if (!C.CheckInventory(scroll))
            return;

        Chill();
        WhiteMap();
        TryEquipConsumableItem(scroll);
    }

    public void EquipDecay()
    {
        const string scroll = "Scroll of Decay";

        if (!C.CheckInventory(scroll))
            return;

        Chill();
        WhiteMap();
        TryEquipConsumableItem(scroll);
    }

    private bool TryEquipConsumableItem(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        if (!Bot.Inventory.TryGetItem(name, out InventoryItem? item))
            return false;

        if (Bot.Inventory.IsEquipped(name) && item.ID > 0)
        {
            Bot.Send.Packet($"%xt%zm%unequipItem%{Bot.Map.RoomID}%{item.ID}%");
            Bot.Sleep(D1);
        }

        for (int attempt = 0; attempt < 3; attempt++)
        {
            Bot.Inventory.EquipUsableItem(name);
            Bot.Sleep(D3);
            Bot.Wait.ForItemEquip(name);
            if (Bot.Inventory.IsEquipped(name))
                return true;

            if (item.ID > 0)
            {
                Bot.Send.Packet($"%xt%zm%unequipItem%{Bot.Map.RoomID}%{item.ID}%");
                Bot.Sleep(500);
            }
        }

        return Bot.Inventory.IsEquipped(name);
    }

    #endregion

    #region Best Gear

    public record Gear(string Name, string Group, bool FromBank, double All, double Race);

    public void ChooseBestGear(string? names)
    {
        if (
            Bot?.Monsters?.MapMonsters == null
            || Bot?.Inventory?.Items == null
            || Bot?.Bank?.Items == null
        )
            return;

        bool IsSelectedMonster(string? mName, HashSet<string> set)
        {
            if (string.IsNullOrWhiteSpace(mName))
                return false;
            if (set == null || set.Count == 0)
                return true; // "*" or empty -> all
            return set.Contains(mName);
        }

        string NormalizeRace(string? r)
        {
            if (string.IsNullOrWhiteSpace(r))
                return "allDmg";
            if (r.Equals("None", StringComparison.OrdinalIgnoreCase))
                return "allDmg";
            return r;
        }

        double Meta(string meta, string key)
        {
            if (string.IsNullOrWhiteSpace(meta))
                return 0;
            var kAll = key.Equals("allDmg", StringComparison.OrdinalIgnoreCase);
            var span = meta.AsSpan();
            int i = 0,
                len = span.Length;
            while (i < len)
            {
                int j = i;
                while (j < len && span[j] != '\n' && span[j] != '\r' && span[j] != ',')
                    j++;
                var token = span.Slice(i, j - i).ToString();
                i = j + 1;

                int colon = token.IndexOf(':');
                if (colon <= 0)
                    continue;

                var k = token.Substring(0, colon).Trim();
                var vStr = token.Substring(colon + 1).Trim();

                if (
                    !(
                        k.Equals(key, StringComparison.OrdinalIgnoreCase)
                        || (kAll && k.Equals("dmgAll", StringComparison.OrdinalIgnoreCase))
                    )
                )
                    continue;

                if (
                    double.TryParse(
                        vStr,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out var v
                    )
                )
                    return Math.Max(0, v - 1);
            }
            return 0;
        }

        HashSet<string> ParseNameSet(string s)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(s) || s.Trim() == "*")
                return set; // empty => "select all"
            int i = 0;
            var span = s.AsSpan();
            while (i < span.Length)
            {
                int j = i;
                while (j < span.Length && span[j] != ',')
                    j++;
                var piece = span.Slice(i, j - i).ToString().Trim();
                if (piece.Length > 0)
                    set.Add(piece);
                i = j + 1;
            }
            return set;
        }

        bool IsValidGroup(string g)
        {
            if (string.IsNullOrWhiteSpace(g))
                return false;
            if (g.Equals("Weapon", StringComparison.OrdinalIgnoreCase))
                return true;
            var gl = g.ToLowerInvariant();
            return gl == "he" || gl == "ba" || gl == "co" || gl == "pe";
        }

        bool Equipped(string name)
        {
            var items = Bot.Inventory.Items;
            if (items == null)
                return false;
            foreach (var it in items)
                if (
                    it?.Equipped == true
                    && it.Name != null
                    && name.Equals(it.Name, StringComparison.OrdinalIgnoreCase)
                )
                    return true;
            return false;
        }

        void Equip(Gear g)
        {
            if (string.IsNullOrWhiteSpace(g.Name))
                return;
            for (int t = 0; t < 3 && !Bot.ShouldExit; t++)
            {
                if (g.FromBank)
                    InBank(g.Name);
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

        var selected = ParseNameSet(names ?? string.Empty);
        string race = "allDmg";
        {
            var mobs = Bot.Monsters.MapMonsters;
            var raceCount = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            if (mobs != null)
            {
                foreach (var m in mobs)
                {
                    var mn = m?.Name;
                    if (!IsSelectedMonster(mn, selected))
                        continue;
                    var r = NormalizeRace(m?.Race);
                    if (!raceCount.TryGetValue(r, out var c))
                        raceCount[r] = 1;
                    else
                        raceCount[r] = c + 1;
                }
            }
            int maxRaceCount = 0;
            foreach (var kv in raceCount)
                if (kv.Value > maxRaceCount)
                {
                    maxRaceCount = kv.Value;
                    race = kv.Key;
                }
            Log("GEAR", $"🎯 Target race: {race}");
        }

        // scan inventory + bank
        var bank = Bot.Bank.Items ?? Enumerable.Empty<InventoryItem>();
        var inv = Bot.Inventory.Items ?? Enumerable.Empty<InventoryItem>();

        var bankNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var b in bank)
            if (!string.IsNullOrWhiteSpace(b?.Name))
                bankNames.Add(b.Name);

        var bestAll = new Dictionary<string, Gear>(StringComparer.OrdinalIgnoreCase);
        var bestRace = new Dictionary<string, Gear>(StringComparer.OrdinalIgnoreCase);

        void Consider(InventoryItem it)
        {
            if (it == null || string.IsNullOrWhiteSpace(it.ItemGroup))
                return;
            if (!IsValidGroup(it.ItemGroup))
                return;
            if (it.Upgrade && !(Bot?.Player?.IsMember == true))
                return;

            var name = it.Name ?? "";
            var grp = it.ItemGroup;
            var fromBank = bankNames.Contains(name);

            var all = Meta(it.Meta, "allDmg");
            var rac = Meta(it.Meta, race);

            if (all <= 0 && rac <= 0)
                return;

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

        foreach (var it in inv)
            Consider(it);
        foreach (var it in bank)
            Consider(it);

        if (bestAll.Count == 0 && bestRace.Count == 0)
        {
            Log("GEAR", "❌ No gear with bonuses found");
            return;
        }

        // choose best combo
        Gear? bestA = null,
            bestR = null;
        double bestSum = double.MinValue;

        foreach (var kvA in bestAll)
        {
            var ga = kvA.Value;
            foreach (var kvR in bestRace)
            {
                var gr = kvR.Value;
                if (ga.Group.Equals(gr.Group, StringComparison.OrdinalIgnoreCase))
                    continue;
                double sum = ga.All + gr.Race;
                if (sum > bestSum)
                {
                    bestSum = sum;
                    bestA = ga;
                    bestR = gr;
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
        Gear? bestItem = null;
        double bestScore = double.MinValue;

        foreach (var kv in bestAll)
        {
            var g = kv.Value;
            var s = g.All > g.Race ? g.All : g.Race;
            if (s > bestScore)
            {
                bestScore = s;
                bestItem = g;
            }
        }
        foreach (var kv in bestRace)
        {
            var g = kv.Value;
            var s = g.All > g.Race ? g.All : g.Race;
            if (s > bestScore)
            {
                bestScore = s;
                bestItem = g;
            }
        }

        if (bestItem != null)
        {
            Log("GEAR", $"✨ Best: {bestItem.Name}");
            Equip(bestItem);
        }
    }

    #endregion

    #region Auras

    private IEnumerable<Aura?>? GetAuraCollection(bool self)
    {
        try
        {
            return self ? Bot?.Self?.Auras : Bot?.Target?.Auras;
        }
        catch
        {
            return null;
        }
    }

    public Aura? GetAuraByName(string auraName, bool self)
    {
        if (string.IsNullOrWhiteSpace(auraName))
            return null;

        try
        {
            return GetAuraCollection(self)
                ?.FirstOrDefault(a => a?.Name != null && auraName.Equals(a.Name, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return null;
        }
    }

    public bool HasAura(string auraName, bool self = false)
        => GetAuraByName(auraName, self) != null;

    public bool HasAnyAura(IEnumerable<string> auraNames, bool self = false)
        => auraNames != null && auraNames.Any(name => !string.IsNullOrWhiteSpace(name) && HasAura(name, self));

    public bool HasAnyAuraOtherThan(string auraName, bool self = false)
    {
        if (string.IsNullOrWhiteSpace(auraName))
            return false;

        try
        {
            return GetAuraCollection(self)
                ?.Any(a => a?.Name != null && !auraName.Equals(a.Name, StringComparison.OrdinalIgnoreCase)) == true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Returns stacks as a float (preserves fractional stacks); 0 if missing.
    /// </summary>
    public float GetAuraStacksFloat(string auraName, bool self = false)
    {
        if (string.IsNullOrWhiteSpace(auraName))
            return 0f;

        try
        {
            return GetAuraCollection(self)
                ?.FirstOrDefault(a => a?.Name == auraName)?.Value ?? 0f;
        }
        catch
        {
            return 0f;
        }
    }

    /// <summary>
    /// Returns stacks as int (rounded), +1, for legacy code compatibility.
    /// </summary>
    public int GetAuraStacks(string auraName, bool self = false)
        => (int)Math.Round(GetAuraStacksFloat(auraName, self)) + 1;


    /// <summary>
    /// Returns remaining seconds of an aura; 0 if missing or expired.
    /// </summary>
    public int GetAuraSecondsRemaining(string auraName, bool self = false)
    {
        var aura = GetAuraByName(auraName, self);
        return aura != null && aura.UnixTimeStamp > 0 && aura.Duration > 0
            ? Math.Max(0, (int)(DateTimeOffset.FromUnixTimeMilliseconds(aura.UnixTimeStamp)
                                 .AddSeconds(aura.Duration) - DateTimeOffset.UtcNow).TotalSeconds)
            : 0;
    }

    /// <summary>
    /// Checks if the aura has at least the specified quantity of stacks (int or float).
    /// Returns false if aura is missing.
    /// </summary>
    public bool Stacks(string auraName, float quantity, bool self = false)
        => !string.IsNullOrWhiteSpace(auraName)
           && quantity > 0f
           && ((self
                ? Bot.Self.Auras.FirstOrDefault(a => a?.Name == auraName)?.Value
                : Bot.Target.Auras.FirstOrDefault(a => a?.Name == auraName)?.Value) ?? 0f) >= quantity;

    /// <summary>
    /// Returns true if the aura has less than or equal to the specified duration in seconds remaining.
    /// </summary>
    public bool Left(string auraName, int duration, bool self = false)
        => !string.IsNullOrWhiteSpace(auraName) && duration >= 0
           && GetAuraSecondsRemaining(auraName, self) <= duration;

    #endregion

    #region Shop

    public bool BuyItem(
        object itemKey,
        int shopId,
        string map,
        int quantity = 1,
        bool ensureMap = true,
        bool calculateRemaining = true,
        bool skipIfHaveEnough = true,
        bool considerBank = true,
        bool checkGold = true,
        bool checkLevel = true,
        bool checkInvSpace = true,
        int loadTimeoutMs = 5000
    )
    {
        if (itemKey is not (int or string))
        {
            Log("SHOP", "❌ Invalid item key type.");
            return false;
        }
        if (quantity <= 0)
            return false;
        if (Bot == null || Bot.Player == null || Bot.Shops == null)
            return false;

        bool EnsureMapJoin(string m)
        {
            if (!ensureMap)
                return true;
            if (string.IsNullOrWhiteSpace(m))
                return true;
            if (Bot.Map?.Name?.Equals(m, StringComparison.OrdinalIgnoreCase) == true)
                return true;

            Join(m);
            return Bot.Map?.Name?.Equals(m, StringComparison.OrdinalIgnoreCase) == true;
        }

        bool LoadShop(int id)
        {
            for (int attempt = 0; attempt < 3 && !Bot.ShouldExit; attempt++)
            {
                int prevCache = Bot.Shops.LoadedCache?.Count ?? 0;
                Bot.Shops.Load(id);

                long start = Environment.TickCount64;
                while (!Bot.ShouldExit && Environment.TickCount64 - start < loadTimeoutMs)
                {
                    int items = Bot.Shops.Items?.Count ?? 0;
                    int cache = Bot.Shops.LoadedCache?.Count ?? 0;

                    if (items > 0 || cache > prevCache)
                        return true;

                    Bot.Sleep(50);
                }
            }
            return false;
        }

        ShopItem? FindItem(object key)
        {
            var list = Bot?.Shops?.Items;
            if (list == null)
                return null;

            if (key is int id && id > 0)
            {
                foreach (ShopItem? item in list)
                    if (item != null && item.ID == id)
                        return item;

                return null;
            }

            if (key is string name && !string.IsNullOrWhiteSpace(name))
            {
                foreach (ShopItem? item in list)
                    if (
                        !string.IsNullOrWhiteSpace(item?.Name)
                        && item.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                    )
                        return item;

                return null;
            }

            return null;
        }

        int Have(string name)
        {
            if (considerBank)
                InBank(name);
            return Owned(name);
        }

        int Need(string name, int want)
        {
            int cur = Have(name);
            if (skipIfHaveEnough && cur >= want)
                return 0;
            return calculateRemaining ? Math.Max(0, want - cur) : want;
        }

        bool HasInvSpace() => (Bot.Inventory?.FreeSlots ?? 0) > 0;

        // -------- FLOW --------

        if (!EnsureMapJoin(map))
        {
            Log("SHOP", $"❌ Failed to join {map}");
            return false;
        }

        if (!LoadShop(shopId))
        {
            Log("SHOP", $"❌ Failed to load shop {shopId}");
            return false;
        }

        ShopItem? item = FindItem(itemKey);
        if (item == null)
        {
            Log("SHOP", $"❌ Item not found: {itemKey}");
            return false;
        }

        string name = item.Name;
        if (string.IsNullOrWhiteSpace(name))
        {
            Log("SHOP", "❌ Item has no valid name.");
            return false;
        }

        int need = Need(name, quantity);
        if (need == 0)
            return true;

        long price = (long)item.Cost * need;
        if (checkGold && Bot.Player.Gold < price)
        {
            Log("SHOP", $"💰 Not enough gold: need {price}, have {Bot.Player.Gold}");
            return false;
        }

        if (checkLevel && Bot.Player.Level < item.Level)
        {
            Log("SHOP", $"⬆️ Level {item.Level}+ required");
            return false;
        }

        if (checkInvSpace && !HasInvSpace())
        {
            Log("SHOP", "📦 Inventory full");
            return false;
        }

        int before = Owned(name); // inventory only

        Bot.Shops.BuyItem(item.ID, item.ShopItemID, need);

        long t0 = Environment.TickCount64;
        while (Environment.TickCount64 - t0 < 2000)
        {
            if (Owned(name) > before)
                break;
            Bot.Sleep(50);
        }

        int gained = Owned(name) - before;
        bool success = gained > 0;

        Log("SHOP", success ? $"🛒 Purchased {gained}x {name}" : $"❌ Purchase failed: {name}");

        return success;
    }

    #endregion

    #region Drops

    bool HasDrop(object key)
    {
        // Check by ID
        if (key is int id && id > 0)
        {
            ItemBase[]? infos = Bot.Drops.CurrentDropInfos.ToArray();
            if (infos == null)
                return false;

            foreach (ItemBase? item in infos)
                if (item?.ID == id)
                    return true;

            return false;
        }

        // Check by name
        if (key is string name && !string.IsNullOrWhiteSpace(name))
        {
            string[]? names = Bot?.Drops?.CurrentDrops.ToArray();
            if (names != null)
                foreach (string n in names)
                    if (string.Equals(n, name, StringComparison.OrdinalIgnoreCase))
                        return true;

            ItemBase[]? infos = Bot?.Drops?.CurrentDropInfos.ToArray();
            if (infos != null)
                foreach (ItemBase? item in infos)
                    if (string.Equals(item?.Name, name, StringComparison.OrdinalIgnoreCase))
                        return true;

            return false;
        }

        return false;
    }

    ItemBase? GetDropItem(object key)
    {
        ItemBase[]? infos = Bot.Drops.CurrentDropInfos.ToArray();
        if (infos == null || infos.Length == 0)
            return null;

        // Match by numeric ID
        if (key is int id && id > 0)
        {
            foreach (ItemBase? item in infos)
                if (item?.ID == id)
                    return item;

            return null;
        }

        // Match by item name
        if (key is string name && !string.IsNullOrWhiteSpace(name))
        {
            foreach (ItemBase? item in infos)
                if (string.Equals(item?.Name, name, StringComparison.OrdinalIgnoreCase))
                    return item;

            return null;
        }

        return null;
    }

    public void Pickup(params object[] keys)
    {
        if (keys == null || keys.Length == 0)
            return;

        RejectExcept(keys);

        foreach (object key in keys)
        {
            // Handle numeric IDs
            if (key is int id && id > 0)
            {
                if (HasDrop(id))
                {
                    Bot.Drops.Pickup(id);
                    Bot.Sleep(D1);
                }

                continue;
            }

            // Handle item names
            if (key is string name && !string.IsNullOrWhiteSpace(name))
            {
                if (HasDrop(name))
                {
                    Bot.Drops.Pickup(name);
                    Bot.Sleep(D1);
                }
            }
        }
    }

    public bool WaitForDrop(object key, int timeout = 30000)
    {
        // Only allow int or string keys
        if (key is not (int or string))
            return false;

        long start = Environment.TickCount64;

        while (!Bot.ShouldExit && !HasDrop(key) && Environment.TickCount64 - start < timeout)
            Bot.Sleep(D1); // Sleep delay to avoid CPU spin

        return HasDrop(key);
    }

    public bool HasAny(params object[] keys)
    {
        if (keys == null || keys.Length == 0)
            return false;

        foreach (object key in keys)
        {
            // Match valid int IDs
            if (key is int id && id > 0 && HasDrop(id))
                return true;

            // Match valid string names
            if (key is string name && !string.IsNullOrWhiteSpace(name) && HasDrop(name))
                return true;
        }

        return false;
    }

    public void RejectExcept(params object[] keys)
    {
        static string? ToName(object k, Func<object, ItemBase?> getDrop)
        {
            return k switch
            {
                int id when id > 0 => getDrop(id)?.Name ?? id.ToString(),
                string s when !string.IsNullOrWhiteSpace(s) => s,
                _ => null,
            };
        }

        var names = keys
            ?.Select(k => ToName(k, GetDropItem))
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        Bot.Flash.Call(
            "rejectExcept",
            (names != null && names.Length > 0) ? string.Join(",", names) : ""
        );
    }

    #endregion

    #region Player

    public double GetHealthPercentage()
    {
        if (Bot?.Player == null || Bot.Player.MaxHealth <= 0)
            return 0;
        return (double)Bot.Player.Health / Bot.Player.MaxHealth * 100;
    }

    public double GetManaPercentage()
    {
        if (Bot?.Player == null || Bot.Player.MaxMana <= 0)
            return 0;
        return (double)Bot.Player.Mana / Bot.Player.MaxMana * 100;
    }

    // Returns true if current HP is below the given threshold.
    public bool IsHealthLow(double percentage = 30)
    {
        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
        return Bot.Player.Alive && GetHealthPercentage() < percentage;
    }

    // Returns true if current MP is below the given threshold.
    public bool IsManaLow(double percentage = 30)
    {
        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
        return Bot.Player.Alive && GetManaPercentage() < percentage;
    }

    // Returns true if current HP is above the given threshold.
    public bool IsHealthHigh(double percentage = 90)
    {
        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
        return Bot.Player.Alive && GetHealthPercentage() > percentage;
    }

    // Returns true if current MP is above the given threshold.
    public bool IsManaHigh(double percentage = 90)
    {
        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
        return Bot.Player.Alive && GetManaPercentage() > percentage;
    }

    public bool IsFullHealth()
    {
        if (Bot?.Player == null)
            return false;
        return Bot.Player.Health >= Bot.Player.MaxHealth;
    }

    public bool IsFullMana()
    {
        if (Bot?.Player == null)
            return false;
        return Bot.Player.Mana >= Bot.Player.MaxMana;
    }

    public bool IsFullHealthAndMana()
    {
        return IsFullHealth() && IsFullMana();
    }

    public bool IsDead()
    {
        if (Bot?.Player == null)
            return true; // Assume dead if can't check
        return Bot.Player.State == 0;
    }

    public bool IsIdle()
    {
        if (Bot?.Player == null)
            return false;
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
        return Bot?.Player?.CurrentClass?.Name ?? "No Class";
    }

    public bool HasClassEquipped(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
            return false;

        C.DebugLogger(this);

        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

        return Bot.Player.CurrentClass?.Name?.Equals(
            className,
            StringComparison.OrdinalIgnoreCase
        ) ?? false;
    }

    public bool IsCurrentClassMaxRank() => Bot.Player.CurrentClassRank >= 10;

    public bool IsInCell(string cellName)
    {
        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
        if (string.IsNullOrWhiteSpace(cellName))
            return false;
        return Bot?.Player?.Cell?.Equals(cellName, StringComparison.OrdinalIgnoreCase) == true;
    }

    public bool NeedsRest(double healthThreshold = 50, double manaThreshold = 50) =>
        IsHealthLow(healthThreshold) || IsManaLow(manaThreshold);

    public bool ShouldRest() => !Bot.Player.InCombat && !IsFullHealthAndMana();

    public string GetTargetName()
    {
        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
        return Bot?.Player?.Target?.Name ?? string.Empty;
    }

    public double GetTargetHealthPercentage()
    {
        var target = Bot?.Player?.Target;
        if (target == null || target.MaxHP <= 0)
            return 0;
        return (double)target.HP / target.MaxHP * 100;
    }

    public bool IsTargetAlive()
    {
        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
        return Bot?.Player?.Target?.Alive == true;
    }

    public bool IsTargetHealthLow(double percentage = 30)
    {
        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
        return GetTargetHealthPercentage() < percentage;
    }

    public bool HasEnoughGold(int amount) => Bot.Player.Gold >= amount;

    public PlayerStats GetPlayerStats() => Bot?.Player?.Stats ?? new PlayerStats();

    public int GetStatValue(string statName)
    {
        if (string.IsNullOrWhiteSpace(statName))
            return 0;

        var stats = Bot?.Player?.Stats;
        if (stats == null)
            return 0;

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
            _ => 0,
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

    public bool IsReadyForCombat() => Bot.Player.Alive && Bot.Player.Loaded;

    public double GetLowestHpPercentage()
    {
        var names = Bot?.Map?.PlayerNames;
        if (names == null || names.Count == 0)
            return 100.0;

        double lowest = 100.0;
        foreach (var playerName in names)
        {
            if (string.IsNullOrWhiteSpace(playerName))
                continue;
            try
            {
                int hp = Bot!.Flash.GetGameObject<int>($"world.uoTree.{playerName}.intHP");
                int maxHp = Bot.Flash.GetGameObject<int>($"world.uoTree.{playerName}.intHPMax");
                if (maxHp > 0 && hp >= 0)
                {
                    double pct = (double)hp / maxHp * 100.0;
                    if (pct < lowest)
                        lowest = pct;
                }
            }
            catch { }
        }
        return lowest;
    }

    public bool IsArmyHealthLow(double percentage = 30.0)
    {
        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

        return GetLowestHpPercentage() < percentage;
    }

    public bool InLoadedMap(string name) =>
        Bot?.Map?.Loaded == true
        && Bot.Map.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) == true;

    #endregion

    #region Map

    string? _bestCell = null;
    string _bestPad = "Left";

    public void Join(
        string map,
        string cell = "Enter",
        string pad = "Spawn",
        bool publicRoom = false,
        int? roomNumber = null
    )
    {
        if (string.IsNullOrWhiteSpace(map))
            return;

        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

        string mapName = map.Split('-')[0].Trim();
        string target =
            publicRoom ? mapName
            : roomNumber is int n ? $"{mapName}-{n}"
            : map.Contains("-") ? map
            : $"{mapName}-{GenerateRoomID()}";

        if (InLoadedMap(mapName))
        {
            if (!string.IsNullOrWhiteSpace(cell) && !IsInCell(cell))
                Bot.Map.Jump(cell, pad);
            return;
        }

        Chill();

        for (int i = 0; i < 5 && !Bot.ShouldExit && !InLoadedMap(mapName); i++)
        {
            Bot.Send.Packet(
                $"%xt%zm%cmd%{Bot.Map.RoomID}%tfer%{Bot.Player.Username}%{target}%{cell}%{pad}%"
            );

            long end = Environment.TickCount64 + 8000; // up to 8s per try
            while (!Bot.ShouldExit && !InLoadedMap(mapName) && Environment.TickCount64 < end)
                Bot.Sleep(100);

            if (!InLoadedMap(mapName))
                Bot.Sleep(300);
        }

        if (InLoadedMap(mapName))
        {
            Log("MAP", $"🌍 Joined {mapName}");
            if (!string.IsNullOrWhiteSpace(cell) && !IsInCell(cell))
                Bot.Map.Jump(cell, pad);
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
                machineId =
                    Microsoft.Win32.Registry.GetValue(
                        @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography",
                        "MachineGuid",
                        null
                    ) as string
                    ?? Environment.MachineName;
            }
            catch
            {
                machineId = Environment.MachineName;
            }

            string seed = machineId;
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(seed));
            uint roomSeed = BitConverter.ToUInt32(hash, 0);
            return (int)(roomSeed % 99000) + 1000;
        }
    }

    public void ChooseBestCell(
        string? monsterNames,
        bool alt = false,
        string? setCell = null,
        string setPad = "Spawn"
    )
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
            .Where(m =>
                wildcard
                || names.Any(name =>
                    string.Equals(m.Name ?? string.Empty, name, StringComparison.OrdinalIgnoreCase)
                )
            )
            .ToList();

        if (monsters.Count == 0)
        {
            Log("MAP", "❌ No matching monsters found");
            return;
        }

        string? targetCell =
            !string.IsNullOrWhiteSpace(setCell) ? setCell
            : alt ? monsters.FirstOrDefault()?.Cell
            : monsters
                .GroupBy(m => m.Cell, StringComparer.Ordinal)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(targetCell))
        {
            Log("MAP", "❌ No valid target cell");
            return;
        }

        var mapCells = new HashSet<string>(
            Bot.Map.Cells as IEnumerable<string> ?? Array.Empty<string>(),
            StringComparer.Ordinal
        );
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

    public void Chill(bool sleepMore = true)
    {
        Bot.Combat.CancelAutoAttack();
        Bot.Combat.CancelTarget();

        var cells = Bot.Map.Cells ?? new List<string>();
        var mobs = Bot.Monsters?.MapMonsters ?? new List<Monster>();

        string safeCell =
            cells
                .Where(c =>
                    !string.IsNullOrWhiteSpace(c)
                    && !c.Equals("Wait", StringComparison.OrdinalIgnoreCase)
                    && !c.Equals("Blank", StringComparison.OrdinalIgnoreCase)
                    && !c.StartsWith("Cut", StringComparison.OrdinalIgnoreCase)
                )
                .OrderBy(c => mobs.Count(m => m?.Cell == c))
                .FirstOrDefault()
            ?? Bot.Player.Cell;

        string pad = string.IsNullOrWhiteSpace(Bot.Player.Pad) ? "Left" : Bot.Player.Pad;

        Log("CHILL", $"🍃 Safe cell: {safeCell} ({pad})");

        while (!Bot.ShouldExit && Bot.Player.State == 2)
        {
            if (!IsInCell(safeCell))
                Bot.Map.Jump(safeCell, pad);

            Bot.Sleep(D1);
        }

        if (sleepMore)
            Bot.Sleep(D3);
    }

    bool HasChanged<T>(string key, T newValue)
    {
        if (string.IsNullOrWhiteSpace(key))
            return false;

        if (
            _cache.TryGetValue(key, out var prev)
            && prev is T p
            && EqualityComparer<T>.Default.Equals(p, newValue)
        )
            return false;

        _cache[key] = newValue;
        return true;
    }

    public void Log(string category, string message)
    {
        if (string.IsNullOrWhiteSpace(category) || string.IsNullOrWhiteSpace(message))
            return;
        try
        {
            OnSignal?.Invoke(category, message);
        }
        catch { }
    }

    #endregion

    #region Skills

    readonly int skillsDelay = 100;

    async Task SkillsAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested && !Bot.ShouldExit)
        {
            try
            {
                Skills();
            }
            catch { }

            if (Bot.ShouldExit)
            {
                Bot.Skills.Stop();
                break;
            }

            try
            {
                await Task.Delay(skillsDelay, token);
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch { }
        }

        if (Bot.ShouldExit)
            Bot.Skills.Stop();
    }

    void Skills()
    {
        if (!Bot.Player.Alive)
            Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

        if (!Bot.Player.HasTarget)
            return;

        string? className = Bot.Player.CurrentClass?.Name?.ToLower() ?? default;
        if (string.IsNullOrWhiteSpace(className))
            return;

        switch (className)
        {
            #region Ultra Classes
            // Ultra classes
            case "king's echo":
                KingsEcho();
                break;
            case "bard":
                BardClass();
                break;
            case "scion of flames":
                ScionofFlamesClass();
                break;
            case "arachnomancer":
                ArachnomancerClass();
                break;
            case "dragonsoul shinobi":
                DragonSoulShinobi();
                break;
            case "yami no ronin":
                YamiNoRonin();
                break;
            case "Warrior":
                Warrior();
                break;
            case "archfiend":
                ArchFiend();
                break;
            case "legion revenant":
            case "legion revenant (ioda)":
                LegionRevenantClass();
                break;
            case "archpaladin":
                ArchPaladinClass();
                break;
            case "stonecrusher":
                StoneCrusherClass();
                break;
            case "infinity titan":
                InfinityTitanClass();
                break;
            case "lord of order":
                LordOfOrderClass();
                break;
            case "void highlord":
            case "void highlord (ioda)":
                VoidHighlordClass();
                break;
            case "chaos avenger":
                ChaosAvengerClass();
                break;
            case "lightcaster":
                LightCasterClass();
                break;
            case "legion doomknight":
                LegionDoomKnightClass();
                break;
            case "dragon of time":
                DragonOfTimeClass();
                break;
            case "archmage":
                ArchmageClass();
                break;
            case "verus doomknight":
                VerusDoomKnight();
                break;
            case "arcana invoker":
                ArcanaInvokerClass();
                break;
            #endregion

            #region Chrono Classes
            // Chrono classes
            case "archivist of time":
                ArchivistofTime();
                break;
            case "chrono dragonknight":
            case "chrono dataknight":
                ChronoDataKnightClass();
                break;
            case "shadowstalker of time":
            case "shadowweaver of time":
            case "shadowwalker of time":
                ShadowWeaverOfTimeClass();
                break;
            case "continuum chronomancer":
            case "quantum chronomancer":
                QuantumChronomancerClass();
                break;
            case "nechronomancer":
            case "necrotic chronomancer":
                NecroticChronomancerClass();
                break;
            case "legion paladin":
            case "obsidian paladin chronomancer":
                ObsidianPaladinChronomancerClass();
                break;
            case "chrono shadowslayer":
            case "chrono shadowhunter":
                ChronoShadowSlayerClass();
                break;
            case "Phantom Chronmancer":
            case "Phantasm Chronmancer":
                PhantomPhantasmChronomancer();
                break;

            #endregion

            #region  Common classes
            // Common classes
            case "master ranger":
                MasterRangerClass();
                break;
            case "dragonslayer general":
                DragonslayerGeneralClass();
                break;
            case "cryomancer":
                CryomancerClass();
                break;
            case "dragon knight":
                DragonKnightClass();
                break;
            case "shaman":
                ShamanClass();
                break;
            case "evolved shaman":
                EvolvedShamanClass();
                break;
            case "dark legendary hero":
                DarkLegendaryHeroClass();
                break;
            case "necromancer":
                NecromancerClass();
                break;
            case "chrono assassin":
                ChronoAssassinClass();
                break;
            case "guardian":
                GuardianClass();
                break;
            case "great thief":
                GreatThiefClass();
                break;
            case "chaos slayer berserker":
            case "chaos slayer cleric":
            case "chaos slayer mystic":
            case "chaos slayer thief":
            case "chaos slayer":
                ChaosSlayerClass();
                break;
            #endregion

            #region Basic classes
            // Basic classes
            case "mage":
            case "mage (rare)":
                MageClass();
                break;
            case "dragonslayer":
                DragonslayerClass();
                break;
            #endregion

            default:
                // use base 1-2-3-4
                BasicClass();
                break;
        }
    }

    void BasicClass()
    {
        if (Cast(1))
            return;
        if (Cast(2))
            return;
        if (Cast(3))
            return;
        if (Cast(4))
            return;
    }

    // --- ultra classes ---------------------------------------------------------------

    int bardStep = 0;
    string? bardLastTargetName;
    readonly int[] bardSequence = { 1, 4, 2, 3, 1, 2, 3, 4 };

    void BardClass()
    {
        if (bardStep < 0 || bardStep >= bardSequence.Length)
            bardStep = 0;

        var currentTargetName = Bot.Player.Target?.Name;
        if (!string.Equals(bardLastTargetName, currentTargetName, StringComparison.Ordinal))
        {
            bardLastTargetName = currentTargetName;
            bardStep = 0;
        }

        int skillIndex = bardSequence[bardStep];
        if (Bot.Skills?.CanUseSkill(skillIndex) != true)
            return;

        Bot.Log($"[Bard] Casting skill {skillIndex} at sequence position {bardStep}");
        if (TryUseSkill(skillIndex))
        {
            bardStep = (bardStep + 1) % bardSequence.Length;
            Bot.Log($"[Bard] Cast success skill {skillIndex}, advancing bardStep to {bardStep}");
        }
    }

    void Warrior()
    {
        if (Cast(4))
            return;
        if (Cast(3))
            return;
        if (Cast(2))
            return;
        if (Cast(1))
            return;
    }

    void DragonSoulShinobi()
    {
        if (Cast(4))
            return;
        
        if (Cast(1))
            return;
    }

    void KingsEcho()
    {
        int energyStacks = GetAuraStacks("Residual Energy", true);

        if (NotUltraDarkon() && 
        Bot.Player.Mana < 26)
        {
            if (Cast(4)) 
                return;
        }

        if ((!NotUltraDarkon()) && 
        Bot.Player.Mana < 37)
        {
            if (Cast(4)) 
                return;
        }

        if (NotUltraDage() && 
        (!NotUltraDarkon() 
        || !NotUltraSpeaker() 
        || !NotKolr())  && 
        KolrLowHp() && 
        !HasAura("Putrefaction", true) && 
        (IsHealthLow(80) && Left("Royal Resolve", 1, true)))
        {
            if (Cast(3))
                return;
        }

        if (energyStacks >= 23)
        {
            if (Cast(4))
                return;
        }

        if (Cast(2)) 
            return;

        if (Cast(1)) 
            return;
    }

    void ScionofFlamesClass()
    {
        if (NotUltraDage() && (IsHealthLow(50) || IsArmyHealthLow(50) || HasAura("Purifying Flames", true)))
        {
            if (Cast(3))
                return;
        }

        if (Cast(0))
            return;

        if (Left("Transcending Flames", 2, true))
        {
            if (Cast(4))
                return;
        }

        if (!HasAura("Fuel The Flame", true))
        {
            if (Cast(1))
                return;
        }

        if (HasAura("Transcending Flames", true))
        {
            if (Cast(2))
                return;
        }
    }

    void ArachnomancerClass()
    {
        if (IsHealthHigh(40) && Left("Panic", 1))
        {
            if (Cast(3))
                return;
        }

        if (IsHealthHigh(70) && Left("Cocooned", 1))
        {
            if (Cast(4))
                return;
        }
        
        if (Cast(2))
            return;

        if (Cast(1))
            return;


    }

    void YamiNoRonin()
    {
        if (Cast(1))
            return;

        if (Cast(4))
            return;
        
        if (IsHealthLow(70))
        {
            if (Cast(3))
                return;
        }
    }

    void ArchFiend()
    {
        if (Cast(3))
            return;

        if (Cast(4)) 
            return;

        if (Cast(2))
            return;

        if (Cast(1))
            return;
    }

    void LegionRevenantClass()
    {
        if (Cast(3))
            return;
        if (Cast(2))
            return;
        if (Cast(1))
            return;
        if (Cast(4))
            return;
    }
    void LordOfOrderClass()
    {
        if (NotUltraDage() && NotKolr() && (IsHealthLow(80) || IsArmyHealthLow(80) || Left("Resurgence", 2, true)))
        {
            if (Cast(2))
                return;
        }

        if (!NotKolr() && 
        !HasAura("Putrefaction", true) && 
        (IsHealthLow(70) || IsArmyHealthLow(70)))
        {
            if (Cast(2))
                return;
        }

        if (Left("Clarity", 2, true))
            if (Cast(3))
                return;

        if (Left("Empowerment", 2, true))
            if (Cast(1))
                return;

        if (Cast(4))
            return;
    }

    void PhantomPhantasmChronomancer()
    {
        if (Cast(3)) return;
        if (Cast(2)) return;
        if (Cast(1)) return;
        if (Cast(2)) return;
        if (Cast(1)) return;
        if (Cast(2)) return;
        if (Cast(1)) return;
        if (Cast(2)) return;
        if (Cast(1)) return;
        if (Cast(3)) return;
        if (Cast(1)) return;
        if (Cast(3)) return;
        if (Cast(2)) return;
        if (Cast(4)) return;
    }

    void StoneCrusherClass()
    {
        if (NotUltraDage())
        {
            if (Cast(3))
                return;
        }

        if (Cast(2))
            return;

        if (Cast(4))
            return;

        if (Cast(1))
            return;
    }

    void InfinityTitanClass()
    {
        if (Cast(2))
            return;

        if (NotUltraDage())
        {
            if (Cast(3))
                return;
        }

        if (Cast(4))
            return;


        if (Cast(1))
        return;
    }

    void ArchPaladinClass()
    {
        if (NotUltraDage() && NotUltraSpeaker() && (IsHealthLow(65) || IsArmyHealthLow(65) || Left("Hymn of Light", 2, true)))
        {
            if (Cast(2))
                return;
        }

        if (NotUltraDage() && !NotUltraSpeaker())
        {
            if (Cast(2))
                return;
        }

        if (!HasAura("Righteous Seal"))
            if (Cast(3))
                return;

        if (HasAura("Righteous Seal") && Left("Righteous Seal", 2))
            if (Cast(4))
                return;

        if (Cast(1))
            return;
    }

    void VoidHighlordClass()
    {
        if (IsHealthHigh(60))
        {
            if (Cast(3))
                return;
        }

        if (HasAura("Unshackled", true))
        {
            if (Cast(4))
                return;
        }

        if (IsHealthHigh(60))
        {
            if (Cast(1))
                return;
        }

        if (Cast(2))
            return;
    }

    void ChaosAvengerClass()
    {
        if (Cast(0))
            return;
        if (Cast(3))
            return;
        if (Cast(4))
            return;
        if (Cast(2))
            return;
        if (Cast(1))
            return;

    }

    void LightCasterClass()
    {
        if (IsHealthLow(85) || IsArmyHealthLow(85) || Left("Illuminated", 1, true))
            if (Cast(3))
                return;
        if (Cast(4))
            return;
        if (Cast(1))
            return;
        if (Cast(2))
            return;
    }

    void LegionDoomKnightClass()
    {
        if (Cast(4))
            return;
        if (Cast(1))
            return;
        if (Cast(2))
            return;
        if (Cast(3))
            return;
    }

    void DragonOfTimeClass()
    {
        if (IsHealthLow(60))
        {
            if (Cast(2))
                return;
        }

        if (HasAura("Searing Timeline", true) && HasAura("Convergence", true))
        {
            if (Cast(4))
                return;
        }

        if (Cast(1))
            return;

        if (Cast(3))
            return;

        if (Cast(2))
            return;
    }

void ArchmageClass()
    {
        if (IsManaLow(30))
            if (Cast(2))
                return;
        if (
            HasAura("Arcane Flux", true)
            && !HasAura("Corporeal Ascension", true)
            && !HasAura("Astral Ascension", true)
        )
            if (Cast(4))
                return;
        if (HasAura("Corporeal Ascension", true) && !HasAura("Astral Ascension", true))
            if (Cast(4))
                return;
        if (Cast(1))
            return;
        if (Cast(3))
            return;
    }

    void VerusDoomKnight()
    {
        if (IsHealthLow(70))
            if (Cast(2))
                return;

        if (Cast(1))
            return;
        
        if (NotUltraSpeaker())
        {
            if (Cast(1))
                return;
        }

        if (NotUltraSpeaker())
        {
            if (Cast(4))
                return;
        }

        if (Cast(3))
            return;

        if (Cast(2))
            return;
    }

    bool HasArcana()
    {
        return
            HasAura("I - The Magician", true) ||
            HasAura("II - The High Priestess", true) ||
            HasAura("III - The Empress", true) ||
            HasAura("IV - The Emperor", true) ||
            HasAura("V - The Hierophant", true) ||
            HasAura("VI - The Lovers", true) ||
            HasAura("VII - The Chariot", true) ||
            HasAura("VIII - Justice", true) ||
            HasAura("IX - The Hermit", true) ||
            HasAura("X - Wheel of Fortune", true) ||
            HasAura("XI - Strength", true) ||
            HasAura("XII - The Hanged Man", true) ||
            HasAura("XIII - Death", true) ||
            HasAura("XIV - Temperance", true) ||
            HasAura("XV - The Devil", true) ||
            HasAura("XVI - The Tower", true) ||
            HasAura("XVII - The Star", true) ||
            HasAura("XVIII - The Moon", true) ||
            HasAura("XIX - The Sun", true) ||
            HasAura("XX - Judgement", true);
    }

    void ArcanaInvokerClass()
    {
        if (!HasAura("XXI - The World", true) &&
            (!HasAura("0 - The Fool", true) || !HasArcana()))
        {
            if (Cast(1))
                return;
        }

        if (HasAura("XX - Judgement", true))
        {
            if (Cast(1))
                return;
        }

        if (GetAuraStacksFloat("End of the World", true) > 22f)
        {
            if (Cast(1))
                return;
        }

        if (Cast(2))
            return;

        if (Cast(4))
            return;

        if (Cast(3))
            return;
    }

private bool TryCastRotation(int[] sequence, ref int step)
{
    if (sequence == null || sequence.Length == 0)
        return false;

    int attempts = 0;
    while (attempts < sequence.Length)
    {
        int index = sequence[step];
        if (Cast(index))
        {
            step++;
            if (step >= sequence.Length)
                step = 0;
            return true;
        }

        step++;
        if (step >= sequence.Length)
            step = 0;
        attempts++;
    }

    return false;
}

    // --- chrono classes ---------------------------------------------------------------

    void ArchivistofTime()
    {
        if (Cast(0)) return;
        if (Cast(2)) return;
        if (Cast(2)) return;
        if (Cast(0)) return;
        if (Cast(2)) return;
        if (Cast(3)) return;
        if (Cast(0)) return;
        if (Cast(1)) return;
        if (Cast(1)) return;
        if (Cast(0)) return;
        if (Cast(1)) return;
        if (Cast(1)) return;
        if (Cast(0)) return;
        if (Cast(1)) return;
        if (Cast(1)) return;
        if (Cast(0)) return;
        if (Cast(1)) return;
        if (Cast(1)) return;
        if (Cast(0)) return;
        if (Cast(1)) return;
        if (Cast(1)) return;
    }


    void ChronoDataKnightClass()
    {
        if (Stacks("Temporal Rift", 4, true))
            if (Cast(4))
                return;
        if (Cast(1))
            return;
        if (Cast(2))
            return;
        if (Cast(3))
            return;
    }

    void ShadowWeaverOfTimeClass()
    {
        if (IsHealthLow(50) || IsManaLow(30))
            if (Cast(3))
                return;
        if (Stacks("Chaos Rift", 4, true))
            if (Cast(4))
                return;
        if (Cast(1))
            return;
        if (Cast(2))
            return;
    }

    void QuantumChronomancerClass()
    {
        if (Stacks("Temporal Rift", 4, true))
            if (Cast(3))
                return;
        if (HasAura("Quantum Restructure", true))
            if (Cast(4))
                return;
        if (Cast(2))
            return;
        if (Cast(1))
            return;
    }

    void NecroticChronomancerClass()
    {
        if (Stacks("Chaos Rift", 4, true))
            if (Cast(3))
                return;
        if (Left("Debilitated", 2))
            if (Cast(4))
                return;
        if (Cast(2))
            return;
        if (Cast(1))
            return;
    }

    void ObsidianPaladinChronomancerClass()
    {
        if (IsHealthLow(50) || IsArmyHealthLow(50))
            if (Cast(3))
                return;
        if (IsHealthLow(80) || IsArmyHealthLow(80))
            if (Cast(2))
                return;
        if (Stacks("Temporal Rift", 4, true))
            if (Cast(4))
                return;
        if (Cast(1))
            return;
    }

    private bool _alt23;

    void ChronoShadowSlayerClass()
    {
        // Snapshot
        int mana = Bot.Player.Mana;
        int rift = (int)(Bot.Self.GetAura("Temporal Rift")?.Value ?? 0);
        int rounds = (int)(Bot.Self.GetAura("Rounds Empty")?.Value ?? 0);

        // High mana phase
        if (mana > 20)
        {
            if (rift >= 4)
            {
                if (Cast(4))
                {
                    Bot.Wait.ForTrue(() => (int)(Bot.Self.GetAura("Temporal Rift")?.Value ?? 0) == 0, 20);
                    return;
                }
            }

            if (_alt23)
            {
                if (Cast(2))
                {
                    _alt23 = false;
                    Bot.Sleep(100);
                    return;
                }
            }
            else
            {
                if (Cast(3))
                {
                    _alt23 = true;
                    Bot.Sleep(100);
                    return;
                }
            }
        }

        // Low mana phase use nuke ability
        if (mana < 20 && Bot.Skills.CanUseSkill(4))
        {
            if (Cast(4))
            {
                Bot.Sleep(100);
                return;
            }
        }

        // Reload
        else if (mana < 20 && rounds > 0)
        {
            if (Cast(1))
            {
                Bot.Wait.ForTrue(() => !Bot.Self.HasActiveAura("Rounds Empty"), 20);
                return;
            }
        }
    }

    // --- common classes ---------------------------------------------------------------

    void MasterRangerClass()
    {
        if (HasAura("Vampiric Shot", true))
            if (Cast(3))
                return;
        if (Stacks("Marks", 6, true))
            if (Cast(4))
                return;
        if (Stacks("Marks", 3, true))
            if (Cast(2))
                return;
        if (Cast(1))
            return;
    }

    void DragonslayerGeneralClass()
    {
        if (HasAura("General's Dragonbane"))
            if (Cast(2))
                return;
        if (HasAura("General's Dragonbane"))
            if (Cast(3))
                return;
        if (Cast(4))
            return;
        if (Cast(1))
            return;
    }

    void CryomancerClass()
    {
        if (IsHealthLow(60) && HasAura("Polar Vortex", true))
            if (Cast(3))
                return;
        if (HasAura("Frozen") && HasAura("Polar Vortex", true))
            if (Cast(2))
                return;
        if (Cast(1))
            return;
        if (Cast(4))
            return;
    }

    void DragonslayerClass()
    {
        if (HasAura("Dragonbane") && !HasAura("Infected Wound"))
            if (Cast(2))
                return;
        if (HasAura("Dragonbane") && !HasAura("Weakened"))
            if (Cast(3))
                return;
        if (Cast(4))
            return;
        if (Cast(1))
            return;
    }

    void DragonKnightClass()
    {
        if (Cast(1))
            return;
        if (HasAura("Flammable"))
            if (Cast(4))
                return;
        if (Cast(2))
            return;
        if (HasAura("Dumbfounded"))
            if (Cast(3))
                return;
    }

    void ShamanClass()
    {
        if (Left("Elemental Embrace", 2))
            if (Cast(4))
                return;

        if (Bot.Player.Mana > 30)
        {
            if (Cast(1))
                return;
        }

        if (Bot.Player.Mana > 30)
        {
            if (Cast(2))
                return;
        }
    }

    void EvolvedShamanClass()
    {
        if (IsHealthLow(80) || IsArmyHealthLow(80))
            if (Cast(3))
                return;
        if (Left("Elemental Grasp", 5))
            if (Cast(4))
                return;
        if (Cast(2))
            return;
        if (Cast(1))
            return;
    }

    void DarkLegendaryHeroClass()
    {
        if (NotUltraDage() && (IsHealthLow(30) || IsArmyHealthLow(30)))
            if (Cast(4))
                return;
        if (Cast(3))
            return;
        if (Cast(2))
            return;
        if (Cast(1))
            return;
    }

    void NecromancerClass()
    {
        if (IsManaLow(90) && IsHealthHigh(80) && !HasAura("Deadly Frenzy", true))
            if (Cast(3))
                return;
        if (IsManaLow(30) && IsHealthHigh(80) && HasAura("Deadly Frenzy", true))
            if (Cast(3))
                return;
        if (IsManaHigh(80) && IsHealthHigh(80))
            if (Cast(4))
                return;
        if (HasAura("Deadly Frenzy", true))
            if (Cast(1))
                return;
        if (Cast(2))
            return;
    }

    void ChronoAssassinClass()
    {
        if (HasAura("Reverse Time", true))
        {
            if (Cast(4))
                return;
        }
        else
        {
            if (Cast(3))
                return;
            if (Cast(1))
                return;
        }
        if (Cast(2))
            return;
    }

    void GuardianClass()
    {
        if (
            (HasAura("Hypercritical", true) || HasAura("Void Imbue", true))
            && Stacks("Guardian Spirit", 15, true)
        )
            if (Cast(4))
                return;
        if (IsManaLow(70))
            if (Cast(3))
                return;
        if (Cast(1))
            return;
        if (Cast(2))
            return;
    }

    void GreatThiefClass()
    {
        if (HasAura("Hidden Blade", true))
            if (Cast(4))
                return;
        if (Cast(3))
            return;
        if (Cast(1))
            return;
        if (Cast(2))
            return;
    }

    void ChaosSlayerClass()
    {
        if (
            (HasAura("Impasse") || HasAura("Delusion") || HasAura("Angustied"))
            && !HasAura("Corageous", true)
        )
            if (Cast(4))
                return;
        if (Cast(3))
            return;
        if (Cast(1))
            return;
        if (Cast(2))
            return;
    }

    // --- basic classes ---------------------------------------------------------------

    void MageClass()
    {
        if (Left("Arcane Shield", 2, true))
            if (Cast(4))
                return;
        if (Cast(1))
                return;
        if (Cast(2))
                return;
        if (Cast(3))
            return;
    }

    // --- helpers ---------------------------------------------------------------

    public void Enhancements(bool IsTaunter = false)
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
            "Mage",
            "Warrior",
        };

        var taunterclasses = new HashSet<string> { "Chaos Avenger", "ArchPaladin" };

        // Only equip if current class is NOT in the list
        string? currentClass = Bot.Player!.CurrentClass?.Name;
        if (
            !IsTaunter
            && !string.IsNullOrEmpty(currentClass)
            && !acceptableDPSClasses.Contains(currentClass)
        )
        {
            string? classToEquip = Bot
                .Inventory.Items.Concat(Bot.Bank.Items)
                .FirstOrDefault(x => acceptableDPSClasses.Contains(x.Name))
                ?.Name;

            if (!string.IsNullOrEmpty(classToEquip))
                C.Equip(classToEquip);
        }
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
        }
    }

    public bool Cast(int index) =>
        index >= 0 && index <= 5 && Bot?.Skills != null && Bot.Skills.CanUseSkill(index) && TryUseSkill(index);

    private bool TryUseSkill(int index)
    {
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

        if (_runSkills != null)
        {
            try { _runSkills.Wait(TimeSpan.FromSeconds(2)); } catch { }
            try { _runSkills.Dispose(); } catch { }
        }

        try { _cts?.Dispose(); } catch { }

        Bot.Skills.Stop();
        Bot.Combat.CancelAutoAttack();
        Bot.Combat.CancelTarget();

        _runSkills = null;
        _cts = null;
    }

    public void EnableSkills()
    {
        if (_runSkills != null && !_runSkills.IsCompleted)
            return;

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

    private Dictionary<string, string> _classRotationMode = new Dictionary<string, string>(
        StringComparer.OrdinalIgnoreCase
    );

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

