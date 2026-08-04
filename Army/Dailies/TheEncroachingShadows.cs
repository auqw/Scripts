/*
name: The Encroaching Shadows Army
description: Runs the non-member The Encroaching Shadows daily with a synchronized configurable army.
tags: daily, NSOD, necrotic, sword, doom, encroaching, shadows, void, aura, army
*/

// Contributor: Haennix (Discord: deus0204)

//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreEnginev3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreUltrav3.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs

using System;
using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class TheEncroachingShadows
{
    private const int QuestID = 8653;
    private const int MaxArmySize = 7;
    private const string SyncPrefix = "TheEncroachingShadowsArmy";

    private static readonly string[] SyncNames =
    {
        "start",
        "classes_owned",
        "classes_equipped",
        "enhancements",
        "daily_status",
        "quest_ready",
        "icewing_arrived",
        "icewing_progress",
        "hydra_arrived",
        "hydra_progress",
        "flibbi_arrived",
        "flibbi_progress",
        "turnin",
        "finished",
    };

    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots Core => CoreBots.Instance;
    private static CoreEnginev3 Engine => _Engine ??= new CoreEnginev3();
    private static CoreEnginev3 _Engine;
    private static CoreAdvanced Adv => _Adv ??= new CoreAdvanced();
    private static CoreAdvanced _Adv;

    private readonly CoreUltrav3 Ultra = new();
    private List<string> _armyMembers = new();

    private int ArmySize => Math.Clamp(Bot.Config!.Get<int>("ArmySize"), 1, MaxArmySize);

    private sealed class ClassSlot
    {
        public int Index { get; init; }
        public string ClassName { get; init; } = string.Empty;
        public string PreferredUsername { get; init; } = string.Empty;
    }

    public bool DontPreconfigure = true;
    public string OptionsStorage = "TheEncroachingShadowsArmy";

    public List<IOption> Options = new()
    {
        new Option<int>(
            "ArmySize",
            "Army Size",
            "How many characters are in the army (1-7, including this character). Fill Class 5-7 when using more than four.",
            4
        ),
        new Option<string>(
            "Class1",
            "Class 1",
            "Preset class 1. Use ClassName,Username to force this class onto an account, or only ClassName for automatic assignment.",
            "King's Echo"
        ),
        new Option<string>(
            "Class2",
            "Class 2",
            "Preset class 2. Use ClassName,Username to force this class onto an account, or only ClassName for automatic assignment.",
            "Legion Revenant"
        ),
        new Option<string>(
            "Class3",
            "Class 3",
            "Preset class 3. Use ClassName,Username to force this class onto an account, or only ClassName for automatic assignment.",
            "Lord Of Order"
        ),
        new Option<string>(
            "Class4",
            "Class 4",
            "Preset class 4. Use ClassName,Username to force this class onto an account, or only ClassName for automatic assignment.",
            "Verus DoomKnight"
        ),
        new Option<string>(
            "Class5",
            "Class 5",
            "Required when Army Size is 5 or more. Use ClassName,Username or only ClassName.",
            ""
        ),
        new Option<string>(
            "Class6",
            "Class 6",
            "Required when Army Size is 6 or more. Use ClassName,Username or only ClassName.",
            ""
        ),
        new Option<string>(
            "Class7",
            "Class 7",
            "Required when Army Size is 7. Use ClassName,Username or only ClassName.",
            ""
        ),
        new Option<bool>(
            "DoEnh",
            "Do Enhancements",
            "Apply the class-specific enhancement preset before starting the daily.",
            true
        ),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions(disableClassSwap: true);
        Engine.Boot();

        try
        {
            PrepareSyncFiles();
            _armyMembers = WaitForArmyStart();

            if (!AssignAndEquipClasses())
                return;

            if (Bot.Config!.Get<bool>("DoEnh"))
                ApplyEnhancements();

            WaitForPhase("enhancements", "READY");
            RunDaily();
        }
        finally
        {
            Engine.DisableSkills();
            Bot.Combat.CancelTarget();
            Bot.StopSync();
            Core.SetOptions(false);
        }
    }

    private void RunDaily()
    {
        Core.Logger("Army Daily: The Encroaching Shadows [8653]");
        Core.AddDrop(
            "Void Aura",
            "(Necro) Scroll of Dark Arts",
            "Glacial Pinion",
            "Hydra Eyeball",
            "Flibbitigiblets"
        );

        Core.EnsureLoad(QuestID);
        bool allAlreadyComplete = PublishDailyStatus();

        if (allAlreadyComplete)
        {
            Core.Logger($"All {ArmySize} accounts have already completed today's non-member NSoD daily.");
            WaitForPhase("finished", "DONE");
            return;
        }

        WaitForQuestReady();

        CompleteRequirement(
            stageName: "icewing",
            map: "icewing",
            monster: "Warlord Icewing",
            item: "Glacial Pinion",
            quantity: 1
        );

        CompleteRequirement(
            stageName: "hydra",
            map: "hydrachallenge",
            monster: "Hydra Head 90",
            item: "Hydra Eyeball",
            quantity: 3
        );

        CompleteRequirement(
            stageName: "flibbi",
            map: "voidflibbi",
            monster: "Flibbitiestgibbet",
            item: "Flibbitigiblets",
            quantity: 1
        );

        TurnInQuestTogether();
        WaitForPhase("finished", "DONE");
        Core.Logger($"All {ArmySize} accounts completed The Encroaching Shadows.");
    }

    private void CompleteRequirement(
        string stageName,
        string map,
        string monster,
        string item,
        int quantity
    )
    {
        JoinArmyMap(map, monster);
        WaitForPhase($"{stageName}_arrived", "READY");

        string progressPath = SyncPath($"{stageName}_progress");
        int lastReady = -1;
        bool waitingSafely = false;

        while (!Bot.ShouldExit)
        {
            bool localReady = IsDailyComplete() || Core.CheckInventory(item, quantity);
            Ultra.UpdateEntry(progressPath, Username(), localReady ? "1" : "0");

            Dictionary<string, string> states = ReadFreshStates(progressPath, 30);
            int ready = _armyMembers.Count(name =>
                states.TryGetValue(name, out string? value) && value == "1"
            );

            if (ready != lastReady)
            {
                lastReady = ready;
                Core.Logger($"[{item}] Army ready: {ready}/{ArmySize}");
            }

            if (ready >= ArmySize)
            {
                DisengageFromCombat();
                Core.Logger($"All {ArmySize} accounts finished {item} ({quantity}).");
                Bot.Sleep(250);
                return;
            }

            if (localReady)
            {
                if (!waitingSafely)
                {
                    DisengageFromCombat();
                    waitingSafely = true;
                }
                else
                    Bot.Sleep(250);
            }
            else
                Fight(monster, map);
        }
    }

    private void DisengageFromCombat()
    {
        Bot.Options.AttackWithoutTarget = false;
        Bot.Options.AggroMonsters = false;
        Bot.Options.AggroAllMonsters = false;
        Bot.Combat.CancelAutoAttack();
        Bot.Combat.CancelTarget();

        if (!Bot.Player.Alive)
            return;

        Engine.Chill(sleepMore: false);
        Bot.Player.SetSpawnPoint();
    }

    private void JoinArmyMap(string map, string monster)
    {
        // Moving through whitemap forces every client out of any old public/private instance.
        Engine.Join("whitemap");
        Engine.Join(map);
        Engine.ChooseBestCell(monster);
        Bot.Player.SetSpawnPoint();
    }

    private void Fight(string monster, string map)
    {
        if (!Bot.Player.Alive)
        {
            Bot.Sleep(250);
            return;
        }

        if (!string.Equals(Bot.Map.Name, map, StringComparison.OrdinalIgnoreCase))
        {
            JoinArmyMap(map, monster);
            return;
        }

        bool hasCorrectTarget =
            Bot.Player.HasTarget
            && Bot.Player.Target != null
            && Bot.Player.Target.Alive
            && string.Equals(
                Bot.Player.Target.Name,
                monster,
                StringComparison.OrdinalIgnoreCase
            );

        if (!hasCorrectTarget)
        {
            Bot.Combat.CancelTarget();
            Bot.Combat.Attack(monster);
        }

        Bot.Sleep(250);
    }

    private bool PublishDailyStatus()
    {
        string path = SyncPath("daily_status");
        int lastRegistered = -1;

        while (!Bot.ShouldExit)
        {
            Ultra.UpdateEntry(path, Username(), IsDailyComplete() ? "1" : "0");
            Dictionary<string, string> states = ReadFreshStates(path, 30);
            int registered = _armyMembers.Count(states.ContainsKey);

            if (registered != lastRegistered)
            {
                lastRegistered = registered;
                Core.Logger($"Daily status registered: {registered}/{ArmySize}");
            }

            if (registered >= ArmySize)
                return _armyMembers.All(name => states[name] == "1");

            Bot.Sleep(250);
        }

        return false;
    }

    private void WaitForQuestReady()
    {
        string path = SyncPath("quest_ready");
        int lastReady = -1;

        while (!Bot.ShouldExit)
        {
            bool localReady = IsDailyComplete() || Bot.Quests.IsInProgress(QuestID);
            if (!localReady)
            {
                Core.EnsureAccept(QuestID);
                Bot.Sleep(750);
                localReady = Bot.Quests.IsInProgress(QuestID);
            }

            Ultra.UpdateEntry(path, Username(), localReady ? "1" : "0");
            Dictionary<string, string> states = ReadFreshStates(path, 30);
            int ready = _armyMembers.Count(name =>
                states.TryGetValue(name, out string? value) && value == "1"
            );

            if (ready != lastReady)
            {
                lastReady = ready;
                Core.Logger($"Quest 8653 ready: {ready}/{ArmySize}");
            }

            if (ready >= ArmySize)
                return;

            Bot.Sleep(500);
        }
    }

    private void TurnInQuestTogether()
    {
        string path = SyncPath("turnin");
        int lastComplete = -1;

        while (!Bot.ShouldExit)
        {
            bool localComplete = IsDailyComplete();
            if (!localComplete && Bot.Quests.CanComplete(QuestID))
            {
                if (Core.EnsureComplete(QuestID))
                    Bot.Wait.ForPickup("Void Aura");
                localComplete = IsDailyComplete();
            }

            Ultra.UpdateEntry(path, Username(), localComplete ? "1" : "0");
            Dictionary<string, string> states = ReadFreshStates(path, 30);
            int complete = _armyMembers.Count(name =>
                states.TryGetValue(name, out string? value) && value == "1"
            );

            if (complete != lastComplete)
            {
                lastComplete = complete;
                Core.Logger($"Quest turn-ins complete: {complete}/{ArmySize}");
            }

            if (complete >= ArmySize)
                return;

            Bot.Sleep(500);
        }
    }

    private List<ClassSlot>? GetConfiguredClassSlots()
    {
        List<ClassSlot> slots = new();

        for (int i = 1; i <= ArmySize; i++)
        {
            string raw = Bot.Config!.Get<string>($"Class{i}") ?? string.Empty;
            string[] parts = raw.Split(
                new[] { ',' },
                2,
                StringSplitOptions.None
            );
            string className = parts[0].Trim();
            string preferredUsername =
                parts.Length > 1
                    ? parts[1].Trim().Replace(":", "-")
                    : string.Empty;

            if (string.IsNullOrWhiteSpace(className))
            {
                Core.Logger(
                    $"Class {i} is required when Army Size is {ArmySize}. Fill Class1 through Class{ArmySize}.",
                    stopBot: true
                );
                return null;
            }

            slots.Add(
                new ClassSlot
                {
                    Index = i - 1,
                    ClassName = className,
                    PreferredUsername = preferredUsername,
                }
            );
        }

        return slots;
    }

    private bool AssignAndEquipClasses()
    {
        List<ClassSlot>? classSlots = GetConfiguredClassSlots();
        if (classSlots == null)
            return false;

        string[] requiredClasses = classSlots
            .Select(slot => slot.ClassName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        string path = SyncPath("classes_owned");
        List<string> ownedClasses = requiredClasses
            .Where(className => Core.CheckInventory(className, toInv: true))
            .ToList();
        string payload = string.Join(",", ownedClasses);
        int lastRegistered = -1;
        Dictionary<string, string> states = new(StringComparer.OrdinalIgnoreCase);

        while (!Bot.ShouldExit)
        {
            Ultra.UpdateEntry(path, Username(), payload);
            states = ReadFreshStates(path, 30);
            int registered = _armyMembers.Count(states.ContainsKey);

            if (registered != lastRegistered)
            {
                lastRegistered = registered;
                Core.Logger($"Class ownership registered: {registered}/{ArmySize}");
            }

            if (registered >= ArmySize)
                break;

            Bot.Sleep(250);
        }

        if (Bot.ShouldExit)
            return false;

        Dictionary<string, HashSet<string>> ownership = new(StringComparer.OrdinalIgnoreCase);
        foreach (string member in _armyMembers)
        {
            string classList = states.TryGetValue(member, out string? value) ? value : string.Empty;
            ownership[member] = classList
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(className => className.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        Dictionary<string, string>? assignments = BuildClassAssignments(ownership, classSlots);
        if (assignments == null || !assignments.TryGetValue(Username(), out string? myClass))
        {
            Core.Logger(
                $"Unable to assign the configured lineup: {string.Join(", ", classSlots.Select(slot => slot.ClassName))}. Verify class ownership and any ClassName,Username assignments.",
                stopBot: true
            );
            return false;
        }

        Core.Logger($"Assigned class: {myClass}");
        Core.Equip(myClass);
        Bot.Wait.ForTrue(
            () => string.Equals(
                Bot.Player.CurrentClass?.Name,
                myClass,
                StringComparison.OrdinalIgnoreCase
            ),
            20
        );

        if (!string.Equals(Bot.Player.CurrentClass?.Name, myClass, StringComparison.OrdinalIgnoreCase))
        {
            Core.Logger($"Failed to equip assigned class: {myClass}", stopBot: true);
            return false;
        }

        WaitForClassComposition(classSlots);
        return true;
    }

    private Dictionary<string, string>? BuildClassAssignments(
        Dictionary<string, HashSet<string>> ownership,
        List<ClassSlot> classSlots
    )
    {
        ClassSlot[] roleOrder = classSlots
            .OrderBy(slot =>
                string.IsNullOrWhiteSpace(slot.PreferredUsername)
                    ? ownership.Count(entry => entry.Value.Contains(slot.ClassName))
                    : ownership.TryGetValue(slot.PreferredUsername, out HashSet<string>? classes)
                        && classes.Contains(slot.ClassName) ? 1 : 0
            )
            .ThenBy(slot => slot.Index)
            .ToArray();
        string[] players = _armyMembers.OrderBy(name => name, StringComparer.OrdinalIgnoreCase).ToArray();
        Dictionary<string, string> assignments = new(StringComparer.OrdinalIgnoreCase);
        HashSet<string> usedPlayers = new(StringComparer.OrdinalIgnoreCase);

        bool AssignRole(int index)
        {
            if (index >= roleOrder.Length)
                return true;

            ClassSlot slot = roleOrder[index];
            IEnumerable<string> candidates = string.IsNullOrWhiteSpace(slot.PreferredUsername)
                ? players
                : players.Where(player =>
                    string.Equals(player, slot.PreferredUsername, StringComparison.OrdinalIgnoreCase));

            foreach (string player in candidates)
            {
                if (usedPlayers.Contains(player) || !ownership[player].Contains(slot.ClassName))
                    continue;

                usedPlayers.Add(player);
                assignments[player] = slot.ClassName;

                if (AssignRole(index + 1))
                    return true;

                assignments.Remove(player);
                usedPlayers.Remove(player);
            }

            return false;
        }

        return AssignRole(0) ? assignments : null;
    }

    private void WaitForClassComposition(List<ClassSlot> classSlots)
    {
        string path = SyncPath("classes_equipped");
        int lastReady = -1;

        while (!Bot.ShouldExit)
        {
            string currentClass = Bot.Player.CurrentClass?.Name ?? string.Empty;
            Ultra.UpdateEntry(path, Username(), currentClass);
            Dictionary<string, string> states = ReadFreshStates(path, 30);
            int ready = _armyMembers.Count(states.ContainsKey);

            if (ready != lastReady)
            {
                lastReady = ready;
                Core.Logger($"Classes equipped: {ready}/{ArmySize}");
            }

            if (ready >= ArmySize)
            {
                Dictionary<string, int> expectedComposition = classSlots
                    .GroupBy(slot => slot.ClassName, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Count(),
                        StringComparer.OrdinalIgnoreCase
                    );
                Dictionary<string, int> actualComposition = _armyMembers
                    .Select(name => states[name])
                    .GroupBy(className => className, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Count(),
                        StringComparer.OrdinalIgnoreCase
                    );

                bool compositionMatches =
                    expectedComposition.Count == actualComposition.Count
                    && expectedComposition.All(pair =>
                        actualComposition.TryGetValue(pair.Key, out int count)
                        && count == pair.Value
                    );

                if (compositionMatches)
                    return;
            }

            Bot.Sleep(250);
        }
    }

    private void ApplyEnhancements()
    {
        string className = Bot.Player.CurrentClass?.Name ?? string.Empty;
        Core.Logger($"Applying NSoD army enhancements for {className}...");

        switch (className)
        {
            case "King's Echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: Adv.uElysium() ? WeaponSpecial.Elysium : WeaponSpecial.Mana_Vamp,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "Legion Revenant":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.None,
                    wSpecial: Adv.uElysium() ? WeaponSpecial.Elysium : WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "Lord Of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: Adv.uArcanasConcerto()
                        ? WeaponSpecial.Arcanas_Concerto
                        : WeaponSpecial.Awe_Blast,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "Verus DoomKnight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Lacerate,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            default:
                Core.Logger($"No dedicated preset for {className}; using SmartEnhance.");
                Adv.SmartEnhance(className);
                break;
        }
    }

    private void PrepareSyncFiles()
    {
        foreach (string syncName in SyncNames)
            Ultra.ClearSyncFile(SyncPath(syncName));
    }

    private List<string> WaitForArmyStart()
    {
        string path = SyncPath("start");
        int lastReady = -1;

        while (!Bot.ShouldExit)
        {
            Ultra.UpdateEntry(path, Username(), "READY");
            Dictionary<string, string> states = ReadFreshStates(path, 10);
            int ready = states.Count;

            if (ready != lastReady)
            {
                lastReady = ready;
                Core.Logger($"Waiting for army: {ready}/{ArmySize}");
            }

            if (ready == ArmySize)
            {
                List<string> members = states.Keys
                    .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                    .ToList();
                Bot.Sleep(750);
                Ultra.UpdateEntry(path, Username(), "READY");

                if (ReadFreshStates(path, 10).Count == ArmySize)
                {
                    Core.Logger($"All {ArmySize} army accounts are online.");
                    return members;
                }
            }
            else if (ready > ArmySize)
            {
                Core.Logger($"Too many clients are using this sync set ({ready}/{ArmySize}).");
            }

            Bot.Sleep(250);
        }

        return new List<string>();
    }

    private void WaitForPhase(string syncName, string payload)
    {
        string path = SyncPath(syncName);
        int lastReady = -1;

        while (!Bot.ShouldExit)
        {
            Ultra.UpdateEntry(path, Username(), payload);
            Dictionary<string, string> states = ReadFreshStates(path, 30);
            int ready = _armyMembers.Count(name =>
                states.TryGetValue(name, out string? value)
                && string.Equals(value, payload, StringComparison.OrdinalIgnoreCase)
            );

            if (ready != lastReady)
            {
                lastReady = ready;
                Core.Logger($"[{syncName}] Ready: {ready}/{ArmySize}");
            }

            if (ready >= ArmySize)
            {
                Bot.Sleep(500);
                return;
            }

            Bot.Sleep(250);
        }
    }

    private Dictionary<string, string> ReadFreshStates(string path, int staleSeconds)
    {
        Dictionary<string, string> states = new(StringComparer.OrdinalIgnoreCase);
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        foreach (string line in Ultra.ReadLines(path))
        {
            string[] parts = line.Split(':');
            if (parts.Length < 3 || !long.TryParse(parts[^1], out long timestamp))
                continue;

            if (now - timestamp > staleSeconds)
                continue;

            if (!string.IsNullOrWhiteSpace(parts[0]))
                states[parts[0]] = parts[1];
        }

        return states;
    }

    private bool IsDailyComplete() => Bot.Quests.IsDailyComplete(QuestID);

    private string SyncPath(string syncName) =>
        Ultra.ResolveSyncPath($"{SyncPrefix}_{syncName}.sync");

    private string Username() => (Bot.Player.Username ?? string.Empty).Replace(":", "-");
}
