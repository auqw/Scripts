/*
name: UltraDrago
description: Ultra King Drago helper with taunter classes and priority adds.
tags: Ultra
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs

using System.ComponentModel;
using System.Reflection;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

#region Fast Comp

/// <summary>
/// Fast Composition - Maximum damage output for speed
/// </summary>
// Chrono ShadowSlayer
// ├─ Class: Lucky
// ├─ Helm: Vim / Forge
// ├─ Weapon: Valiance
// └─ Cape: Vainglory / Lament
//
// Legion Revenant
// ├─ Class: Wizard
// ├─ Helm: Pneuma
// ├─ Weapon: Valiance / Ravenous / Arcana
// └─ Cape: Vainglory
//
// ArchPaladin
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Valiance
// └─ Cape: Lament
//
// Lord of Order
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Awe Blast / Valiance
// └─ Cape: Absolution

#endregion

#region Safe Comp

/// <summary>
/// Safe Composition - Balanced survivability and damage
/// </summary>
// Chaos Avenger
// ├─ Class: Lucky
// ├─ Helm: Anima
// ├─ Weapon: Valiance
// └─ Cape: Vainglory
//
// Legion Revenant
// ├─ Class: Wizard
// ├─ Helm: Pneuma
// ├─ Weapon: Valiance / Ravenous / Arcana
// └─ Cape: Vainglory
//
// ArchPaladin
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Valiance
// └─ Cape: Lament
//
// Lord of Order
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Awe Blast / Valiance
// └─ Cape: Absolution

#endregion

#region F2P Fast

/// <summary>
/// F2P Fast Composition - Budget-friendly speed setup
/// </summary>
// King's Echo
// ├─ Class: Lucky
// ├─ Helm: Examen
// ├─ Weapon: Ravenous
// └─ Cape: Vainglory
//
// Legion Revenant
// ├─ Class: Wizard
// ├─ Helm: Pneuma
// ├─ Weapon: Valiance / Ravenous / Arcana
// └─ Cape: Vainglory
//
// ArchPaladin
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Valiance
// └─ Cape: Lament
//
// Lord of Order
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Awe Blast / Valiance
// └─ Cape: Absolution

#endregion

#region Other DPS Options

/// <summary>
/// Other DPS Options - Alternative single-class configurations
/// </summary>
// Arcana Invoker
// ├─ Class: Lucky
// ├─ Helm: Examen / Forge
// ├─ Weapon: Ravenous / Valiance
// └─ Cape: Vainglory
//
// Archfiend
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Ravenous
// └─ Cape: Vainglory
//
// Lich
// ├─ Class: Lucky
// ├─ Helm: Examen
// ├─ Weapon: Ravenous
// └─ Cape: Penitence
//
// Sentinel
// ├─ Class: Lucky
// ├─ Helm: Anima
// ├─ Weapon: Ravenous
// └─ Cape: Vainglory

#endregion

public class UltraDrago
{
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private CoreBots C => CoreBots.Instance;
    private static CoreAstravia Astravia
    {
        get => _Astravia ??= new CoreAstravia();
        set => _Astravia = value;
    }
    private static CoreAstravia _Astravia;
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraDrago";

    // User options
    public List<IOption> Options = new()
    {
        new Option<DragoComp>(
            "DoEquipClasses",
            "Automatically Equip Classes",
            "Auto-equip classes across all 4 clients\n"
                + "Fast: CSS / LR / AP / LOO\n"
                + "Safe: CAv / LR / AP / LOO\n"
                + "F2P Fast: KE / LR / AP / LOO\n"
                + "Unselected = off (use whatever classes you already have equipped).",
            DragoComp.Unselected
        ),
        new Option<bool>("DoEnh", "Do Enhancements",  "Auto-Enhance Gear properly for the fight", true),
        CoreBots.Instance.SkipOptions,
    };

    List<string> TaunterGroup1 = new[] { "ArchPaladin" }.ToList();
    bool isTaunterGroup1 = false;
    List<string> TaunterGroup2 = new[] { "Lord of Order", "Lich", "Sentinel" }.ToList();
    bool isTaunterGroup2 = false;
    public void ScriptMain(IScriptInterface bot)
    {
        C.OneTimeMessage(
            "WARNING",
            "Please use the classes in the options to ensure proper role functionality.\n"
                + "We've allowed you to choose 'Current Class', but it's recommended to select a specific role for optimal (safe) performance.\n"
                + "Current Class will Focus Boss -> Left Summon -> Right Summon",
            true,
            true
        );

        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();

        Core.Boot();
        Prep();

        // Detect taunter role AFTER sync-equip so the class is correct
        if (TaunterGroup1.Contains(Bot.Player.CurrentClass!.Name))
            isTaunterGroup1 = true;
        if (TaunterGroup2.Contains(Bot.Player.CurrentClass.Name))
            isTaunterGroup2 = true;

        C.EnsureComplete(8397);
        Fight();
    }


    void Prep()
    {
        C.Join("whitemap");
        Astravia.AstraviaJudgement();

        // Sync-equip classes if a comp is selected
        DragoComp comp = Bot.Config!.Get<DragoComp>("DoEquipClasses");
        if (comp != DragoComp.Unselected)
        {
            string[] classes = comp switch
            {
                DragoComp.Fast => new[] { C.CheckInventory("Chrono ShadowSlayer") ? "Chrono ShadowSlayer" : "Chrono ShadowHunter", "Legion Revenant", "ArchPaladin", "Lord of Order" },
                DragoComp.Safe => new[] { "Chaos Avenger", "Legion Revenant", "ArchPaladin", "Lord of Order" },
                DragoComp.F2PFast => new[] { "King's Echo", "Legion Revenant", "ArchPaladin", "Lord of Order" },
                _ => new[] { "ArchPaladin", "Legion Revenant", "Chaos Avenger", "Lord of Order" },
            };

            Ultra.EquipClassSync(classes, 4, "drago_class.sync");
        }

        Adv.GearStore(EnhAfter: true);
        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnhs();

        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");

        // Taunter check uses current (possibly sync-assigned) class
        if (TaunterGroup1.Contains(Bot.Player.CurrentClass?.Name ?? string.Empty)
            || TaunterGroup2.Contains(Bot.Player.CurrentClass?.Name ?? string.Empty))
            Ultra.GetScrollOfEnrage();
    }

    void Fight()
    {
        const string map = "ultradrago";
        const string boss = "King Drago";
        const string leftSummon = "Bowmaster Algie"; // Right summon (Bow)
        const string rightSummon = "Executioner Dene"; // Left summon (Axe)

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);
        if (!Bot.Quests.IsUnlocked(8397))
            Bot.Quests.UpdateQuest(8395);
        C.AddDrop("King Drago Insignia");

        Core.Join(map);
        C.EnsureAccept(8397);
        Ultra.WaitForArmy(3, "ultra_drago.sync");
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        // ===== MAIN LOOP =====
        while (!Bot.ShouldExit)
        {
            // Dead → wait for respawn
            if (!Bot.Player!.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Drago Dethroned", 1), syncPath))
            {
                C.JumpWait();
                C.Logger("All players finished farm.");
                if (Bot.Quests.IsUnlocked(8397))
                    C.EnsureComplete(8397);
                Bot.Wait.ForPickup("King Drago Insignia");
                if (Bot.Config!.Get<bool>("DoEnh"))
                    Adv.GearStore(true, true);
                break;
            }

            if (isTaunterGroup1 && Ultra.MonsterAlive(rightSummon))
            {
                // ArchPaladin taunts the left summon (Axe)
                while (!Bot.ShouldExit)
                {
                    Ultra.Taunt(
                        Bot.Player?.CurrentClass?.Name!,
                        rightSummon,
                        "aura",
                        250,
                        "Focus"
                    );
                    if (!Ultra.MonsterAlive(rightSummon))
                        break;
                }
                continue;
            }

            if (isTaunterGroup2 && Ultra.MonsterAlive(rightSummon))
            {
                // LordOfOrder loops taunt with ArchPaladin (left summon)
                while (!Bot.ShouldExit)
                {
                    Ultra.Taunt(Bot.Player?.CurrentClass?.Name!,
                        rightSummon,
                        "aura",
                        700,
                        "Focus"
                    );
                    if (!Ultra.MonsterAlive(rightSummon))
                        break;
                }
                continue;
            }

            Core.KillWithPriority(boss, leftSummon, rightSummon);
            Bot.Skills.UseSkill(5);
        }
    }


    void DoEnhs()
    {
        string className = Bot.Player!.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        switch (className)
        {
            // Chrono ShadowSlayer
            case "Chrono ShadowSlayer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Vim,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Legion Revenant
            case "Legion Revenant":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // ArchPaladin
            case "ArchPaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            // Lord of Order
            case "Lord of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Awe_Blast,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            // Chaos Avenger
            case "Chaos Avenger":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // King's Echo
            case "King's Echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Arcana Invoker
            case "Arcana Invoker":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Archfiend
            case "Archfiend":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Lich
            case "Lich":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            // Sentinel
            case "Sentinel":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;
        }
    }

    public enum DragoComp
    {
        Unselected,
        Fast,
        Safe,
        F2PFast,
    }
}
