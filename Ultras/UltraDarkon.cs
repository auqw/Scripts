/*
name: UltraDarkon
description: Ultra Darkon spam taunt
tags: ultra, darkon, taunt, spam, Ultra Darkon, ultra darkon
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Skua.Core.Interfaces;
using Skua.Core.Options;

// Light Caster:
// Weapon: Ravenous / Praxis
// Class: Lucky
// Helm: Pneuma
// Cape: Penitence / Lament
// Scroll: Enrage

// Legion Revenant:
// Weapon: Valiance / Ravenous / Arcana
// Class: Wizard
// Helm: Pneuma
// Cape: Penitence
// Scroll: Enrage

// Lord of Order:
// Weapon: Lucky Aweblast / Valiance
// Class: Lucky
// Helm: Forge
// Cape: Absolution
// Scroll: Enrage

// StoneCrusher:
// Weapon: Valiance
// Class: Fighter
// Helm: Anima
// Cape: Absolution
// Scroll: Enrage
// Potion: Divine Elixir

public class UltraDarkon
{
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private CoreBots C => CoreBots.Instance;
    private static CoreAdvanced _Adv;
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEnginev1 Core = new();
    public CoreUltrav1 Ultra = new();
    string? className = null;


    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraDarkon";
    // User options
    public List<IOption> Options = new()
    {
        new Option<DarkonComp>(
            "DoEquipClasses",
            "Automatically Equip Classes",
            "Auto-equip classes across all 4 clients\n"
                + "Recommended: LC / LR / LOO / SC\n"
                + "Unselected = off (use whatever classes you already have equipped).",
            DarkonComp.Unselected
        ),
        new Option<bool>("DoEnh", "Do Enhancements",  "Auto-Enhance Gear properly for the fight", true),
        CoreBots.Instance.SkipOptions,
    };
    bool taunting = true;

    public void ScriptMain(IScriptInterface bot)
    {
        C.Logger("This script uses the `spam taunt method.. and works..maybe ^_^");
        className = Bot.Player.CurrentClass?.Name?.ToLower();
        Core.Boot();
        Prep();
        Kill();
        C.SetOptions(false);
    }

    void Prep()
    {
        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();

        // Sync-equip classes if a comp is selected
        DarkonComp comp = Bot.Config!.Get<DarkonComp>("DoEquipClasses");
        if (comp != DarkonComp.Unselected)
        {
            string[] classes = comp switch
            {
                DarkonComp.Recommended => new[] { "LightCaster", "Legion Revenant", "Lord of Order", "StoneCrusher" },
                DarkonComp.Alternative => new[] { "ArchPaladin", "Legion Revenant", "Lord of Order", "StoneCrusher" },
                _ => throw new InvalidOperationException($"Unhandled DarkonComp value: {comp}")
            };

            Ultra.EquipClassSync(classes, 4, "darkon_class.sync");
        }

        if (Bot.Player.CurrentClass?.Name == "Alpha Omega" || Bot.Player.CurrentClass?.Name == "Alpha DOOMmega")
            taunting = false;

        Adv.GearStore(EnhAfter: true);
        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnhs();

        if (Bot.Player.CurrentClass!.Name == "Stonecrusher")
        {
            C.HuntMonster("poisonforest", "Xavier Lionfang", "Divine Elixir", 10, isTemp: false);
            Ultra.UseAlchemyPotions("Divine Elixir");
        }
        else
            Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.GetScrollOfEnrage();
        Bot.Sleep(2500);
    }

    void Kill()
    {
        if (!C.isCompletedBefore(8733))
        {
            C.Logger("Quest 8733 (\"The World\") not completed. Run: `\"Story\\ElegyofMadness(Darkon)\\0CompleteAll.cs` to be able to complete the quest, or just let this run to help get the kill.");

            Bot.Quests.UpdateQuest(8733);
        }

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        C.EnsureAccept(8746);

        C.AddDrop("Darkon Insignia");

        Core.Join("ultradarkon");
        Ultra.WaitForArmy(3, "Ultra_Darkon.sync");
        var (bestCell, bestPad) = Core.ChooseBestCell("Darkon the Conductor");
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();
        while (!Bot.ShouldExit)
        {
            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Darkon the Conductor Defeated", 1), syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                C.EnsureComplete(8746);
                Bot.Wait.ForPickup("Darkon Insignia");
                C.Logger("Restoring enhancements!");

                if (Bot.Config!.Get<bool>("DoEnh"))
                    Adv.GearStore(true, true);
                break;
            }

            // Dead → wait for respawn
            if (Bot.Player?.Alive == false)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                if (Bot.Player!.CurrentClass?.Name == "Stonecrusher")
                {
                    Ultra.UseAlchemyPotions("Divine Elixir");
                    Bot.Sleep(2500);
                    Core.EquipEnrage();
                }
                continue;
            }

            if (Bot.Player?.Cell != bestCell)
            {
                Bot.Sleep(200);
                C.Jump(bestCell!, bestPad!);
                Bot.Wait.ForCellChange(bestCell!);
                continue;
            }



            if (!Bot.Player!.HasTarget)
                Bot.Combat.Attack("*");
            Bot.Sleep(200);

            // Spam Taunt here
            if (taunting &&
                !Bot.Target.Auras.Any(x => x != null && x.Name == "Focus")
                && Bot.Skills.CanUseSkill(5)
            )
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
            // Light Caster
            case "LightCaster":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky, // Class
                    hSpecial: HelmSpecial.Pneuma, // Helm
                    wSpecial: WeaponSpecial.Ravenous, // Weapon
                    cSpecial: CapeSpecial.Penitence // Cape
                );
                break;

            // Legion Revenant
            case "Legion Revenant":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard, // Class
                    hSpecial: HelmSpecial.Pneuma, // Helm
                    wSpecial: WeaponSpecial.Valiance, // Weapon
                    cSpecial: CapeSpecial.Penitence // Cape
                );
                break;

            // Lord of Order
            case "Lord of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky, // Class
                    hSpecial: HelmSpecial.Forge, // Helm
                    wSpecial: WeaponSpecial.Valiance, // Weapon
                    cSpecial: CapeSpecial.Absolution // Cape
                );
                break;

            // StoneCrusher
            case "StoneCrusher":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter, // Class
                    hSpecial: HelmSpecial.Anima, // Helm
                    wSpecial: WeaponSpecial.Valiance, // Weapon
                    cSpecial: CapeSpecial.Absolution // Cape
                );
                break;
            // ===== NEW ADDITIONS =====

            // Chrono ShadowSlayer / Chrono ShadowHunter
            case "Chrono ShadowSlayer":
            case "Chrono ShadowHunter":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky, // Class
                    hSpecial: HelmSpecial.Forge, // Helm
                    wSpecial: WeaponSpecial.Arcanas_Concerto, // Weapon
                    cSpecial: CapeSpecial.Lament // Cape
                );
                break;

            // Paladin Chronomancer
            case "Paladin Chronomancer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard, // Class
                    hSpecial: HelmSpecial.None, // Helm
                    wSpecial: WeaponSpecial.Mana_Vamp, // Weapon
                    cSpecial: CapeSpecial.Absolution // Cape
                );
                break;

            case "ArchPaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky, // Class
                    hSpecial: HelmSpecial.None, // Helm
                    wSpecial: WeaponSpecial.Valiance, // Weapon
                    cSpecial: CapeSpecial.None // Cape
                );
                break;

            // Alpha Omega / Alpha DOOMmega
            case "Alpha Omega":
            case "Alpha DOOMmega":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky, // Class
                    hSpecial: HelmSpecial.Vim, // Helm
                    wSpecial: WeaponSpecial.Praxis, // Weapon
                    cSpecial: CapeSpecial.Avarice // Cape
                );
                break;

            // Arcana Invoker
            case "Arcana Invoker":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky, // Class
                    hSpecial: HelmSpecial.Forge, // Helm
                    wSpecial: WeaponSpecial.Ravenous, // Weapon
                    cSpecial: CapeSpecial.Penitence // Cape
                );
                break;

            // Lich
            case "Lich":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky, // Class
                    hSpecial: HelmSpecial.Examen, // Helm
                    wSpecial: WeaponSpecial.Ravenous, // Weapon
                    cSpecial: CapeSpecial.Penitence // Cape
                );
                break;

            // Hollowborn VIndicator
            case "Hollowborn Vindicator":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky, // Class
                    hSpecial: HelmSpecial.Forge, // Helm
                    wSpecial: WeaponSpecial.Dauntless, // Weapon
                    cSpecial: CapeSpecial.Penitence // Cape
                );
                break;

            // King's Echo
            case "King's Echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky, // Class
                    hSpecial: HelmSpecial.Examen, // Helm
                    wSpecial: WeaponSpecial.Ravenous, // Weapon
                    cSpecial: CapeSpecial.Lament // Cape
                );
                break;
        }
    }

    public enum DarkonComp
    {
        Unselected,
        Recommended,
        Alternative,
    }
}
