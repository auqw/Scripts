/*
name: Infernal Arena LW
description: Completes Infernal Arena with custom combat behavior for Deadly Duo through Naal.
tags: story, quest, queen of monsters, infernal arena, deadly duo, naal, lonewolf12
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/CelestialArena.cs

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Skills;
using Skua.Core.Options;
using System.Reflection;

public class InfernalArenaLW
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    private static CoreAdvanced Adv { get => _adv ??= new CoreAdvanced(); set => _adv = value; }
    private static CoreAdvanced _adv;

    private static CoreStory Story { get => _story ??= new CoreStory(); set => _story = value; }
    private static CoreStory _story;

    private static CelestialArenaQuests CelestialArena { get => _celestialArena ??= new CelestialArenaQuests(); set => _celestialArena = value; }
    private static CelestialArenaQuests _celestialArena;

    public string OptionsStorage = "InfernalArenaLW";
    public bool DontPreconfigure = true;
    public string[] MultiOptions = { "Setup", "Farm" };

    public List<IOption> Setup = new()
    {
        new Option<ClassChoice>("ClassChoice", "Choose Class",
            "Optimized: Uses AP and Shaman to beat all."
                + "\nArchPaladin: Beats all except for Azalith's Scythe."
                + "\nVoid Highlord: Beats all except for Cervus Malus."
                + "\nShaman: Beats Azalith's Scythe.", ClassChoice.Optimized),
        new Option<bool>("UsePotions", "Use Potions?", "Use the selected class potion setup for the custom fights.", true),
        new Option<bool>("DoEnhancements", "Do Enhancements?",
            "Apply the selected class enhancement setup"
                + "\nArchPaladin: Lucky, Penitence cape, and Lucky Health Vamp."
                + "\nVoid Highlord: Lucky with Ravenous, Anima, and Vainglory when unlocked."
                + "\nShaman: Wizard with Elysium and Absolution when unlocked.", true),
    };

    public List<IOption> Farm = new()
    {
        new Option<bool>("FarmDeadlyDuo", "Farm Deadly Duo?", "Farm Deadly Duo repeatedly.", false),
        new Option<bool>("FarmCervusMalus", "Farm Cervus Malus?", "Farm Cervus Malus repeatedly.", false),
        new Option<bool>("FarmKeyOfSholemoh", "Farm Key of Sholemoh?", "Farm Key of Sholemoh repeatedly.", false),
        new Option<bool>("FarmAzalithsScythe", "Farm Azalith's Scythe?", "Farm Azalith's Scythe repeatedly.", false),
        new Option<bool>("FarmNaal", "Farm Na'al?", "Farm Na'al repeatedly.", false),
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        Bot.Config?.Configure();
        Core.SetOptions();

        try
        {
            Run();
        }
        finally
        {
            Core.SetOptions(false);
        }
    }

    private void Run()
    {
        int selectedFarmCount = SelectedFarmCount();
        if (selectedFarmCount > 1)
        {
            Core.Logger("WARNING: Select only one Farm option at a time. Stopping the script.");
            return;
        }

        bool farmSelected = selectedFarmCount == 1;
        Story.PreLoad(this);

        if (farmSelected)
        {
            if (!TryGetSelectedClass(out ClassChoice classChoice))
                return;

            if (!PrepareSelectedFarm(classChoice))
                return;

            while (!Bot.ShouldExit)
            {
                if (!RunSelectedFarms(classChoice))
                    return;
            }

            return;
        }

        if (Core.isCompletedBefore(9377))
            return;

        if (!Core.isCompletedBefore(9372))
        {
            CelestialArena.DoAll();
            CompleteStandardFights();
        }

        if (!Core.isCompletedBefore(9372))
            return;

        if (!TryGetSelectedClass(out ClassChoice selectedClass))
            return;

        CompleteProgression(selectedClass);
    }

    private void CompleteStandardFights()
    {
        Story.KillQuest(9356, "infernalarena", "Infernal Mage");
        Story.KillQuest(9357, "infernalarena", "Infernal Revenger");
        Story.KillQuest(9358, "infernalarena", "Infernal Harbinger");
        Story.KillQuest(9359, "infernalarena", "First Fallen");
        Story.KillQuest(9360, "infernalarena", "Fallen Warlord");
        Story.KillQuest(9361, "infernalarena", "Malicious Maw");
        Story.KillQuest(9362, "infernalarena", "Wicked Rotfinger");
        Story.KillQuest(9363, "infernalarena", "Dark Devourax");
        Story.KillQuest(9364, "infernalarena", "Corrupt Terror");
        Story.KillQuest(9365, "infernalarena", "Infernal Abominator");
        Story.KillQuest(9366, "infernalarena", "Twisted Harpy");
        Story.KillQuest(9367, "infernalarena", "Infernal Izotz");
        Story.KillQuest(9368, "infernalarena", "Infernal Krampus");
        Story.KillQuest(9369, "infernalarena", "Destructive Defiler");
        Story.KillQuest(9370, "infernalarena", "Infernal Naga");
        Story.KillQuest(9371, "infernalarena", "Accursed Agape");
        Story.KillQuest(9372, "infernalarena", "Accursed Apephyrx");
    }

    private bool CompleteProgression(ClassChoice classChoice)
    {
        if (!Core.isCompletedBefore(9373) && !FightDeadlyDuo(classChoice))
            return false;
        if (!Core.isCompletedBefore(9374) && !FightCervusMalus(classChoice))
            return false;
        if (!Core.isCompletedBefore(9375) && !FightKeyOfSholemoh(classChoice))
            return false;
        if (!Core.isCompletedBefore(9376) && !FightAzalithsScythe(classChoice))
            return false;
        return Core.isCompletedBefore(9377) || FightNaal(classChoice);
    }

    private bool RunSelectedFarms(ClassChoice classChoice)
    {
        if (FarmEnabled("FarmDeadlyDuo") && !FightDeadlyDuo(classChoice, prepareLoadout: false))
            return false;
        if (FarmEnabled("FarmCervusMalus") && !FightCervusMalus(classChoice, prepareLoadout: false))
            return false;
        if (FarmEnabled("FarmKeyOfSholemoh") && !FightKeyOfSholemoh(classChoice, prepareLoadout: false))
            return false;
        if (FarmEnabled("FarmAzalithsScythe") && !FightAzalithsScythe(classChoice, prepareLoadout: false))
            return false;
        if (FarmEnabled("FarmNaal") && !FightNaal(classChoice, prepareLoadout: false))
            return false;
        return true;
    }

    private bool PrepareSelectedFarm(ClassChoice classChoice)
    {
        ClassChoice farmClass = ResolveClass(classChoice, FarmEnabled("FarmAzalithsScythe"));
        if (!EquipClass(farmClass, out _))
            return false;

        bool useRevitalize = FarmEnabled("FarmKeyOfSholemoh") || FarmEnabled("FarmAzalithsScythe")
            || (classChoice == ClassChoice.Void_Highlord && FarmEnabled("FarmNaal"));
        PrepareLoadout(farmClass, useRevitalize);
        return true;
    }

    private int SelectedFarmCount() =>
        new[] { "FarmDeadlyDuo", "FarmCervusMalus", "FarmKeyOfSholemoh", "FarmAzalithsScythe", "FarmNaal" }.Count(FarmEnabled);

    private bool FarmEnabled(string option) =>
        Bot.Config?.Get<bool>("Farm", option) ?? false;

    private bool TryGetSelectedClass(out ClassChoice classChoice)
    {
        classChoice = Bot.Config?.Get<ClassChoice>("Setup", "ClassChoice") ?? ClassChoice.Optimized;
        if (classChoice != ClassChoice.Unselected)
            return true;

        Core.Logger("WARNING: Choose Optimized, ArchPaladin, Void Highlord, or Shaman for the custom fights.");
        return false;
    }

    private ClassChoice ResolveClass(ClassChoice classChoice, bool useShaman) =>
        classChoice == ClassChoice.Optimized
            ? useShaman ? ClassChoice.Shaman : ClassChoice.ArchPaladin
            : classChoice;

    public void FarmDeadlyDuo(string item, int quantity, bool isTemp = false) =>
        FarmBossItem(ClassChoice.ArchPaladin, "Deadly Duo", item, quantity, isTemp, "1 | 2 | 4",
            () => { if (Bot.Player.HasTarget && Bot.Target.HasActiveAura("Eating")) UseConditionalSkill(3); });

    public void FarmCervusMalus(string item, int quantity, bool isTemp = false) =>
        FarmBossItem(ClassChoice.ArchPaladin, "Cervus Malus", item, quantity, isTemp, "3 | 1 | 4",
            () => { if (!Bot.Self.HasActiveAura("Contrarium")) UseConditionalSkill(2); });

    public void FarmKeyOfSholemoh(string item, int quantity, bool isTemp = false) =>
        FarmBossItem(ClassChoice.ArchPaladin, "Key of Sholemoh", item, quantity, isTemp, "3 | 1 | 4",
            () => { if (PlayerHealthBelow(0.50)) UseConditionalSkill(2); });

    public void FarmAzalithsScythe(string item, int quantity, bool isTemp = false) =>
        FarmBossItem(ClassChoice.Shaman, "Azalith's Scythe", item, quantity, isTemp, "1 | 2", () => { },
            attackWithoutTarget: true);

    public void FarmNaal(string item, int quantity, bool isTemp = false) =>
        FarmBossItem(ClassChoice.ArchPaladin, "Na'al", item, quantity, isTemp, "1",
            () =>
            {
                if (Bot.Player.HasTarget && Bot.Target.HasActiveAura("Restrained"))
                    return;

                if (Bot.Self.HasActiveAura("Veni"))
                    UseConditionalSkill(3);
                else if (Bot.Self.HasActiveAura("Vidi"))
                    UseConditionalSkill(4);
                else if (PlayerHealthBelow(0.67))
                    UseConditionalSkill(2);
            },
            comboSelector: () => Bot.Player.HasTarget && Bot.Target.HasActiveAura("Restrained") ? "1 | 2" : "1");

    private void FarmBossItem(ClassChoice classChoice, string monsterName, string item, int quantity,
        bool isTemp, string initialCombo, Action mechanics, bool attackWithoutTarget = false,
        Func<string>? comboSelector = null)
    {
        if (string.IsNullOrWhiteSpace(item) || quantity <= 0 || HasFarmItem(item, quantity, isTemp))
            return;

        Adv.GearStore(EnhAfter: true);

        try
        {
            if (!isTemp)
                Core.AddDrop(item);

            Core.FarmingLogger(item, quantity);

            if (!EquipClass(classChoice, out string className, logFailure: false))
            {
                string requiredClass = classChoice == ClassChoice.Shaman ? "Shaman" : "ArchPaladin";
                Core.Logger($"WARNING: {requiredClass} is unavailable. Falling back to the standard farm method for {monsterName}.");
                Core.EquipClass(ClassType.Solo);
                Core.HuntMonster("infernalarena", monsterName, item, quantity, isTemp: isTemp);
                return;
            }

            PrepareLoadout(classChoice, usePotions: false, doEnhancements: true);
            FightBoss(classChoice, className, monsterName, initialCombo, mechanics, attackWithoutTarget,
                comboSelector, () => HasFarmItem(item, quantity, isTemp));
        }
        finally { Adv.GearStore(true, true); }
    }

    private bool HasFarmItem(string item, int quantity, bool isTemp) =>
        isTemp ? Bot.TempInv.Contains(item, quantity) : Core.CheckInventory(item, quantity);

    private bool FightDeadlyDuo(ClassChoice classChoice, bool prepareLoadout = true) =>
        FightQuest(
            classChoice,
            9373, "Deadly Duo", "Deadly Duo", "Duo Construct Defeated", "1 | 2 | 4",
            () => { if (Bot.Player.HasTarget && Bot.Target.HasActiveAura("Eating")) UseConditionalSkill(3); },
            prepareLoadout: prepareLoadout);

    private bool FightCervusMalus(ClassChoice classChoice, bool prepareLoadout = true) =>
        FightQuest(
            classChoice,
            9374, "Cervus Malus", "Cervus Malus", "Cervus Construct Defeated", "3 | 1 | 4",
            () => { if (!Bot.Self.HasActiveAura("Contrarium")) UseConditionalSkill(2); },
            prepareLoadout: prepareLoadout);

    private bool FightKeyOfSholemoh(ClassChoice classChoice, bool prepareLoadout = true) =>
        FightQuest(
            classChoice,
            9375, "Key of Sholemoh", "Key of Sholemoh", "Key Construct Defeated", "3 | 1 | 4",
            () => { if (PlayerHealthBelow(0.50)) UseConditionalSkill(2); },
            useRevitalize: true,
            prepareLoadout: prepareLoadout);

    private bool FightAzalithsScythe(ClassChoice classChoice, bool prepareLoadout = true) =>
        FightQuest(
            classChoice,
            9376, "Azaliths Scythe", "Azalith's Scythe", "Scythe Construct Defeated", "3 | 1 | 4",
            () => { if (PlayerHealthBelow(0.50)) UseConditionalSkill(2); },
            useRevitalize: true,
            attackWithoutTarget: true,
            prepareLoadout: prepareLoadout);

    private bool FightNaal(ClassChoice classChoice, bool prepareLoadout = true) =>
        FightQuest(
            classChoice,
            9377, "Naal", "Na'al", "Na'al Defeated", "1",
            () =>
            {
                if (Bot.Player.HasTarget && Bot.Target.HasActiveAura("Restrained"))
                    return;

                if (Bot.Self.HasActiveAura("Veni"))
                    UseConditionalSkill(3);
                else if (Bot.Self.HasActiveAura("Vidi"))
                    UseConditionalSkill(4);
                else if (PlayerHealthBelow(0.67))
                    UseConditionalSkill(2);
            },
            useRevitalize: classChoice == ClassChoice.Void_Highlord,
            comboSelector: () => Bot.Player.HasTarget && Bot.Target.HasActiveAura("Restrained") ? "1 | 2" : "1",
            prepareLoadout: prepareLoadout);

    private bool FightQuest(ClassChoice classChoice, int questID, string logName, string monsterName,
        string questItem, string initialCombo, Action mechanics, bool useRevitalize = false,
        bool attackWithoutTarget = false, Func<string>? comboSelector = null, bool prepareLoadout = true)
    {
        classChoice = ResolveClass(classChoice, questID == 9376);

        if (!EquipClass(classChoice, out string className))
            return false;

        if (prepareLoadout)
            PrepareLoadout(classChoice, useRevitalize);
        else
            ApplyPotions(useRevitalize);

        if (!Core.EnsureAccept(questID))
        {
            Core.Logger($"WARNING: {logName} quest could not be accepted.");
            return false;
        }

        Core.Logger($"Fighting {logName}.");

        if (!Bot.TempInv.Contains(questItem) &&
            !FightBoss(classChoice, className, monsterName, initialCombo, mechanics, attackWithoutTarget,
                comboSelector, () => Bot.TempInv.Contains(questItem)))
            return false;

        if (!Core.EnsureComplete(questID))
        {
            Core.Logger($"WARNING: {logName} quest could not be completed.");
            return false;
        }

        return true;
    }

    private bool FightBoss(ClassChoice classChoice, string className, string monsterName,
        string initialCombo, Action mechanics, bool attackWithoutTarget,
        Func<string>? comboSelector, Func<bool> stopCondition)
    {
        const string map = "infernalarena";
        bool previousAttackWithoutTarget = Bot.Options.AttackWithoutTarget;
        string activeCombo = initialCombo;

        try
        {
            Core.Join(map);
            Bot.Options.AttackWithoutTarget = attackWithoutTarget;
            Bot.Skills.Stop();
            if (!Bot.Wait.ForTrue(() => !Bot.Skills.TimerRunning, 100))
            {
                Core.Logger("WARNING: The previous skill timer did not stop. The fight cannot start safely.");
                return false;
            }

            if (classChoice == ClassChoice.Void_Highlord)
                Bot.Skills.StartAdvanced(className, autoEquip: false, ClassUseMode.Def);
            else if (classChoice == ClassChoice.Shaman)
                Bot.Skills.StartAdvanced("1 | 2");
            else
                Bot.Skills.StartAdvanced(initialCombo);

            while (!Bot.ShouldExit && !stopCondition())
            {
                if (!Bot.Player.Alive)
                {
                    Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                    continue;
                }

                if (!string.Equals(Bot.Map.Name, map, StringComparison.OrdinalIgnoreCase))
                    Core.Join(map);

                Monster? boss = Bot.Monsters.MapMonsters.FirstOrDefault(monster => monster.Alive
                    && string.Equals(monster.Name, monsterName, StringComparison.OrdinalIgnoreCase));

                if (boss == null)
                {
                    Bot.Sleep(250);
                    continue;
                }

                if (!string.Equals(Bot.Player.Cell, boss.Cell, StringComparison.OrdinalIgnoreCase))
                    Core.Jump(boss.Cell, "Left");

                if (!Bot.Player.HasTarget || Bot.Player.Target?.MapID != boss.MapID)
                    Bot.Combat.Attack(boss.MapID);

                bool targetingBoss = Bot.Player.HasTarget && Bot.Player.Target?.MapID == boss.MapID;
                if (targetingBoss && classChoice == ClassChoice.ArchPaladin)
                {
                    if (comboSelector != null)
                        SetAdvancedCombo(comboSelector(), ref activeCombo);

                    mechanics();
                }
                else if (targetingBoss && classChoice == ClassChoice.Shaman)
                {
                    int elementalEmbraceRemaining = Bot.Target.GetAura("Elemental Embrace")?.RemainingTime ?? 0;
                    if (elementalEmbraceRemaining <= 5)
                        UseConditionalSkill(4);
                }

                Bot.Sleep(100);
            }
        }
        finally
        {
            Bot.Skills.Stop();
            Bot.Options.AttackWithoutTarget = previousAttackWithoutTarget;
        }

        return stopCondition();
    }

    private void SetAdvancedCombo(string combo, ref string activeCombo)
    {
        if (string.Equals(combo, activeCombo, StringComparison.Ordinal))
            return;

        Bot.Skills.LoadAdvanced(combo);
        if (Bot.Skills.OverrideProvider != null)
            Bot.Skills.SetProvider(Bot.Skills.OverrideProvider);

        if (!Bot.Skills.TimerRunning)
            Bot.Skills.Start();

        activeCombo = combo;
    }

    private void UseConditionalSkill(int skill)
    {
        if (Bot.Skills.CanUseSkill(skill))
            Bot.Skills.UseSkill(skill);
    }

    private bool PlayerHealthBelow(double percentage) =>
        Bot.Player.Health < Bot.Player.MaxHealth * percentage;

    private bool EquipClass(ClassChoice classChoice, out string className, bool logFailure = true)
    {
        className = classChoice switch
        {
            ClassChoice.ArchPaladin when Core.CheckInventory("ArchPaladin") => "ArchPaladin",
            ClassChoice.Void_Highlord when Core.CheckInventory("Void Highlord (IoDA)") => "Void Highlord (IoDA)",
            ClassChoice.Void_Highlord when Core.CheckInventory("Void Highlord") => "Void Highlord",
            ClassChoice.Shaman when Core.CheckInventory("Shaman") => "Shaman",
            _ => string.Empty,
        };

        if (string.IsNullOrEmpty(className))
        {
            string missingClass = classChoice switch
            {
                ClassChoice.Void_Highlord => "Void Highlord",
                ClassChoice.Shaman => "Shaman",
                _ => "ArchPaladin",
            };
            if (logFailure)
                Core.Logger($"WARNING: {missingClass} was not found.");
            return false;
        }

        Core.Equip(className);
        Bot.Wait.ForItemEquip(className);

        if (string.Equals(Bot.Player.CurrentClass?.Name, className, StringComparison.OrdinalIgnoreCase))
            return true;

        if (logFailure)
            Core.Logger($"WARNING: {className} could not be equipped.");
        return false;
    }

    private void PrepareLoadout(ClassChoice classChoice, bool useRevitalize = false,
        bool? usePotions = null, bool? doEnhancements = null)
    {
        if (doEnhancements ?? (Bot.Config?.Get<bool>("Setup", "DoEnhancements") ?? true))
        {
            Bot.Sleep(3000);
            ApplyEnhancements(classChoice);
        }

        bool shouldUsePotions = usePotions ?? (Bot.Config?.Get<bool>("Setup", "UsePotions") ?? true);
        if (!shouldUsePotions)
            return;

        PreparePotion("Fate Tonic", "Gold Voucher 500k", 4, 500_000, 10, requiredAlchemyRank: 8);
        PreparePotion(useRevitalize ? "Potent Revitalize Elixir" : "Potent Battle Elixir",
            "Gold Voucher 500k", useRevitalize ? 8 : 4, 500_000, useRevitalize ? 20 : 8);
        PreparePotion("Felicitous Philtre", "Gold Voucher 100k", 2, 100_000, 25);

        ApplyPotions(useRevitalize, true);
    }

    private void ApplyPotions(bool useRevitalize, bool? usePotions = null)
    {
        if (!(usePotions ?? (Bot.Config?.Get<bool>("Setup", "UsePotions") ?? true)))
            return;

        UsePotion("Fate Tonic", "Fate");
        string elixir = useRevitalize ? "Potent Revitalize Elixir" : "Potent Battle Elixir";
        UsePotion(elixir, elixir);
        UsePotion("Felicitous Philtre", "Felicitous Philtre");
    }

    private void ApplyEnhancements(ClassChoice classChoice)
    {
        try
        {
            if (Core.CBOBool("DisableAutoEnhance", out bool disabled) && disabled)
                Core.Logger("AutoEnhance is disabled in CoreBots Options. The custom Infernal Arena enhancement setup will override it.");

            CapeSpecial capeEnhancement;
            HelmSpecial helmEnhancement;
            WeaponSpecial weaponEnhancement;
            EnhancementType baseEnhancement;

            if (classChoice == ClassChoice.Void_Highlord)
            {
                bool ravenousAvailable = Adv.uRavenous();
                bool animaAvailable = Adv.uAnima();
                bool vaingloryAvailable = Adv.uVainglory();

                weaponEnhancement = ravenousAvailable ? WeaponSpecial.Ravenous : WeaponSpecial.Health_Vamp;
                helmEnhancement = animaAvailable ? HelmSpecial.Anima : HelmSpecial.None;
                capeEnhancement = vaingloryAvailable ? CapeSpecial.Vainglory : CapeSpecial.None;
                baseEnhancement = EnhancementType.Lucky;

                if (!ravenousAvailable)
                    Core.Logger("WARNING: Ravenous is not unlocked. Lucky Health Vamp will be used on the weapon instead.");
                if (!animaAvailable)
                    Core.Logger("WARNING: Anima is not unlocked. Lucky will be used on the helm instead.");
                if (!vaingloryAvailable)
                    Core.Logger("WARNING: Vainglory is not unlocked. Lucky will be used on the cape instead.");
            }
            else if (classChoice == ClassChoice.Shaman)
            {
                bool absolutionAvailable = Adv.uAbsolution();
                bool elysiumAvailable = Adv.uElysium();
                capeEnhancement = absolutionAvailable ? CapeSpecial.Absolution : CapeSpecial.None;
                helmEnhancement = HelmSpecial.None;
                weaponEnhancement = elysiumAvailable ? WeaponSpecial.Elysium : WeaponSpecial.Mana_Vamp;
                baseEnhancement = EnhancementType.Wizard;

                if (!absolutionAvailable)
                    Core.Logger("WARNING: Absolution is not unlocked. Wizard will be used on the cape instead.");
                if (!elysiumAvailable)
                    Core.Logger("WARNING: Elysium is not unlocked. Wizard Mana Vamp will be used on the weapon instead.");
            }
            else
            {
                bool penitenceAvailable = Adv.uPenitence();
                capeEnhancement = penitenceAvailable ? CapeSpecial.Penitence : CapeSpecial.None;
                helmEnhancement = HelmSpecial.None;
                weaponEnhancement = WeaponSpecial.Health_Vamp;
                baseEnhancement = EnhancementType.Lucky;

                if (!penitenceAvailable)
                    Core.Logger("WARNING: Penitence is not unlocked. Lucky will be used on the cape instead.");
            }

            if (weaponEnhancement is WeaponSpecial.Health_Vamp or WeaponSpecial.Mana_Vamp && !Adv.uAwe())
            {
                string aweEnhancement = weaponEnhancement == WeaponSpecial.Mana_Vamp ? "Mana Vamp" : "Health Vamp";
                Core.Logger($"WARNING: {aweEnhancement} is not unlocked. Enhancement setup will continue without stopping the script.");
            }

            MethodInfo? autoEnhance = typeof(CoreAdvanced).GetMethod("AutoEnhance", BindingFlags.Instance | BindingFlags.NonPublic);

            if (autoEnhance == null)
                throw new MissingMethodException(nameof(CoreAdvanced), "AutoEnhance");

            List<InventoryItem> equippedItems = Bot.Inventory.Items.FindAll(item => item.Equipped
                && (item.Category is ItemCategory.Class or ItemCategory.Helm or ItemCategory.Cape || item.ItemGroup == "Weapon"));

            autoEnhance.Invoke(Adv, new object[] { equippedItems, baseEnhancement, capeEnhancement,
                helmEnhancement, weaponEnhancement, false });

            bool baseApplied = Bot.Inventory.Items
                .Where(item => item.Equipped
                    && (item.Category is ItemCategory.Class or ItemCategory.Helm or ItemCategory.Cape || item.ItemGroup == "Weapon"))
                .Where(item => item.ItemGroup != "Weapon")
                .Where(item => capeEnhancement == CapeSpecial.None || item.Category != ItemCategory.Cape)
                .Where(item => helmEnhancement == HelmSpecial.None || item.Category != ItemCategory.Helm)
                .All(item => item.EnhancementPatternID == (int)baseEnhancement);

            InventoryItem? cape = Bot.Inventory.Items.FirstOrDefault(item => item.Equipped && item.Category == ItemCategory.Cape);
            int expectedCapeEnhancement = capeEnhancement == CapeSpecial.None ? (int)baseEnhancement : (int)capeEnhancement;
            bool capeApplied = cape == null || cape.EnhancementPatternID == expectedCapeEnhancement;

            InventoryItem? helm = Bot.Inventory.Items.FirstOrDefault(item => item.Equipped && item.Category == ItemCategory.Helm);
            int expectedHelmEnhancement = helmEnhancement == HelmSpecial.None ? (int)baseEnhancement : (int)helmEnhancement;
            bool helmApplied = helm == null || helm.EnhancementPatternID == expectedHelmEnhancement;

            InventoryItem? weapon = Bot.Inventory.Items.FirstOrDefault(item => item.Equipped && item.ItemGroup == "Weapon");
            int expectedWeaponEnhancement = (int)weaponEnhancement is > 0 and <= 6 ? (int)baseEnhancement : 10;
            bool weaponApplied = weapon != null
                && weapon.EnhancementPatternID == expectedWeaponEnhancement
                && Core.GetItemProperty<int>(weapon, "ProcID") == (int)weaponEnhancement;

            if (!baseApplied || !capeApplied || !helmApplied || !weaponApplied)
                Core.Logger("WARNING: The selected enhancements could not be applied to every equipped item. Continuing with the current enhancements.");
        }
        catch (Exception ex)
        {
            Bot.Log($"Class enhancement setup failed: {ex}");
            Core.Logger("WARNING: Class enhancement setup failed. Continuing with the current enhancements.");
        }
    }

    private void PreparePotion(string itemName, string voucherName, int voucherQuantity,
        int voucherCost, int purchaseQuantity, int requiredAlchemyRank = 0)
    {
        try
        {
            if (Bot.Inventory.Contains(itemName))
                return;

            if (Bot.Bank.Contains(itemName))
            {
                if (Bot.Inventory.FreeSlots <= 0)
                {
                    WarnPotion(itemName, "no free inventory slot is available");
                    return;
                }

                Bot.Bank.EnsureToInventory(itemName);
                Bot.Wait.ForTrue(() => Bot.Inventory.Contains(itemName), 20);

                if (!Bot.Inventory.Contains(itemName))
                    WarnPotion(itemName, "it could not be moved from the bank");
                return;
            }

            if (requiredAlchemyRank > 0 && !Bot.Reputation.HasRank("Alchemy", requiredAlchemyRank))
            {
                WarnPotion(itemName, $"Alchemy rank {requiredAlchemyRank} is required");
                return;
            }

            bool voucherNeedsInventorySlot = !Bot.Inventory.Contains(voucherName);
            int requiredSlots = voucherNeedsInventorySlot ? 2 : 1;

            if (Bot.Inventory.FreeSlots < requiredSlots)
            {
                WarnPotion(itemName, $"{requiredSlots} free inventory slots are required");
                return;
            }

            if (Bot.Bank.Contains(voucherName))
            {
                Bot.Bank.EnsureToInventory(voucherName);
                Bot.Wait.ForTrue(() => Bot.Inventory.Contains(voucherName), 20);
            }

            int missingVouchers = Math.Max(0, voucherQuantity - Bot.Inventory.GetQuantity(voucherName));
            int requiredGold = missingVouchers * voucherCost;

            if (Bot.Player.Gold < requiredGold)
            {
                WarnPotion(itemName, $"{requiredGold} gold is required");
                return;
            }

            Core.Join("alchemyacademy");
            Bot.Shops.Load(2036);

            if (!Bot.Shops.IsLoaded || Bot.Shops.ID != 2036)
            {
                WarnPotion(itemName, "the potion shop could not be loaded");
                return;
            }

            if (missingVouchers > 0)
            {
                Bot.Shops.BuyItem(voucherName, missingVouchers);
                Bot.Wait.ForTrue(() => Bot.Inventory.GetQuantity(voucherName) >= voucherQuantity, 20);
            }

            if (Bot.Inventory.GetQuantity(voucherName) < voucherQuantity)
            {
                WarnPotion(itemName, "the required vouchers could not be purchased");
                return;
            }

            Core.BuyItem("alchemyacademy", 2036, itemName, purchaseQuantity);
            Bot.Wait.ForTrue(() => Bot.Inventory.Contains(itemName), 20);

            if (!Bot.Inventory.Contains(itemName))
                WarnPotion(itemName, "it could not be purchased");
        }
        catch (Exception ex)
        {
            Bot.Log($"Potion preparation failed for {itemName}: {ex}");
            WarnPotion(itemName, "preparation failed");
        }
    }

    private void UsePotion(string itemName, string auraName)
    {
        try
        {
            if (Bot.Self.HasActiveAura(auraName))
                return;

            if (!Bot.Inventory.Contains(itemName))
            {
                WarnPotion(itemName, "it is not in the inventory");
                return;
            }

            Bot.Inventory.EquipUsableItem(itemName);
            Bot.Wait.ForItemEquip(itemName);

            if (!Bot.Inventory.IsEquipped(itemName))
            {
                WarnPotion(itemName, "it could not be equipped");
                return;
            }

            Bot.Sleep(500);

            bool applied = false;
            for (int attempt = 0; attempt < 3 && !Bot.ShouldExit; attempt++)
            {
                int quantityBefore = Bot.Inventory.GetQuantity(itemName);
                Core.UsePotion();

                long started = Environment.TickCount64;
                while (!Bot.ShouldExit && Environment.TickCount64 - started < 1500)
                {
                    if (Bot.Self.HasActiveAura(auraName) || Bot.Inventory.GetQuantity(itemName) < quantityBefore)
                    {
                        applied = true;
                        break;
                    }

                    Bot.Sleep(50);
                }

                if (applied)
                    break;

                Bot.Sleep(250);
            }

            if (!applied)
                WarnPotion(itemName, "its effect could not be verified");
        }
        catch (Exception ex)
        {
            Bot.Log($"Potion use failed for {itemName}: {ex}");
            WarnPotion(itemName, "use failed");
        }
    }

    private void WarnPotion(string itemName, string reason) =>
        Core.Logger($"WARNING: {itemName} was skipped because {reason}. Continuing without it.");

    private enum ClassChoice
    {
        Optimized,
        ArchPaladin,
        Void_Highlord,
        Shaman,
        Unselected,
    }
}
