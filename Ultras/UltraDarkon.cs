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

// Lord Of Order:
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
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();
    string? className = null;

    public void ScriptMain(IScriptInterface bot)
    {
        C.Logger("This script uses the `spam taunt method.. and works..maybe ^_^");
        className = Bot.Player.CurrentClass?.Name?.ToLower();
        Core.Boot();
        Core.EnableSkills();
        Prep();
        Kill();
        C.SetOptions(false);
    }

    void Prep()
    {
        if (Bot.Player.CurrentClass.Name == "Stonecrusher")
        {
            C.HuntMonster("poisonforest", "Xavier Lionfang", "Divine Elixir", 10, isTemp: false);
            Ultra.UseAlchemyPotions("Divine Elixir");
        }
        else
            Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.GetScrollOfEnrage();
        Bot.Sleep(2500);
        Core.EquipEnrage();
        DoEnhs();
    }

    void Kill()
    {
        if (!C.isCompletedBefore(8746))
            C.Logger("Quest 8746 not unlocked.");

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        C.EnsureAccept(8746);
        C.AddDrop("Darkon Insignia");
        Bot.Quests.UpdateQuest(8746);
        Core.Join("ultradarkon");
        Ultra.WaitForArmy(3, "Ultra_Darkon.sync");
        Core.ChooseBestCell("Darkon the Conductor");

        while (!Bot.ShouldExit)
        {
            if (Ultra.CheckArmyProgress("Darkon the Conductor Defeated", 1, true, syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                C.EnsureComplete(8746);
                Bot.Wait.ForPickup("Darkon Insignia");
                C.Logger("Restoring enhancements!");
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

            if (!Bot.Player!.HasTarget)
                Bot.Combat.Attack("*");
            Bot.Sleep(200);

            // Spam Taunt here
            if (!Bot.Self.Auras.Any(x => x != null && x.Name == "Focus")
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

            case "LightCaster":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Penitence,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Ravenous);
                break;

            case "Legion Revenant":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Penitence,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Ravenous);
                break;

            case "Lord Of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Penitence,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Ravenous);
                break;

            case "StoneCrusher":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Penitence,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Valiance);
                break;


            case "Chrono ShadowSlayer":
            case "Chrono ShadowHunter":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Lament,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Arcanas_Concerto);
                break;

            case "Paladin Chronomancer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    cSpecial: CapeSpecial.Absolution,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Mana_Vamp);
                break;

            case "Alpha Omega":
            case "Alpha DOOMmega":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Avarice,
                    hSpecial: HelmSpecial.Vim,
                    wSpecial: WeaponSpecial.Praxis);
                break;


            case "Arcana Invoker":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Penitence,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous);
                break;

            case "Lich":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Penitence,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous);
                break;

            case "Hollowborn Vindicator":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Penitence,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Dauntless);
                break;

            case "King's Echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: CapeSpecial.Lament,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous);
                break;
        }
    }

}
