/*
name: UltraSpeaker
description: Ultra First Speaker helper with zoning, taunt timing, and custom rotation.
tags: Ultra
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

// LR:
// "Weapon: Arcana/Valiance"
// "Class: Wizard"
// "Helm: Wizard"
// "Cape: Penitence"
// "Scroll: Enrage"

// AP:
// "Weapon: Lacerate/Valiance"
// "Class: Luck"
// "Helm: Luck"
// "Cape: Penitence"
// "Scroll: Enrage"

// LOO:
// "Weapon: Valiance"
// "Class: Luck"
// "Helm: Luck"
// "Cape: Penitence"
// "Scroll: Enrage"

// VDK/Other dps
// "Weapon: Valiance"
// "Class: Luck"
// "Helm: Anima"
// "Cape: Penitence"
// "Scroll: Enrage"

public class UltraSpeaker
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

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraSpeaker";
    public List<IOption> Options = new()
    {
        new Option<bool>("DoEnh", "Do Enhancements",  "Auto-Enhance Gear properly for the fight", true),
        CoreBots.Instance.SkipOptions,
    };


    public void ScriptMain(IScriptInterface bot)
    {
        C.Logger("This script uses the `corner spam taunt method.. and works ^_^");
        className = Bot.Player.CurrentClass?.Name?.ToLower();
        Core.Boot();
        Core.EnableSkills();
        Prep();
        Kill();
        C.SetOptions(false);
    }

    void Prep()
    {
        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnh();
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.GetScrollOfEnrage();
    }

    static bool IsInBox(int x, int y) =>
    x >= 0 && x <= 100
    && y >= 485 && y <= 500;

    void Kill()
    {
        if (Bot.Quests.IsDailyComplete(9173))
            C.Logger("Weekly already complete try again Friday morning");

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        C.EnsureAccept(9173);
        C.AddDrop("The First Speaker Silenced");
        Bot.Quests.UpdateQuest(9125);
        Core.Join("ultraspeaker");
        Ultra.WaitForArmy(3, "ultra_speaker.sync");
        Core.ChooseBestCell("The First Speaker");
        Bot.Options.DisableCollisions = true;

        while (!Bot.ShouldExit)
        {
            if (Ultra.CheckArmyProgressBool(() => C.CheckInventory("The First Speaker Silenced", 1), syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                C.EnsureComplete(9173);
                break;
            }

            // Dead → wait for respawn
            if (Bot.Player?.Alive == false)
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);


            // Put the player in a random spot within ((x=0,y=0), (x=101,y=101)) — corner box
            if (Bot.Player?.Cell == "Boss")
            {
                int randomX = Random.Shared.Next(0, 101); // 0–100 inclusive
                int randomY = Random.Shared.Next(485, 501); // 485–500 inclusive

                if (!IsInBox(randomX, randomY))
                    Bot.Player.WalkTo(randomX, randomY);
            }

            if (!Bot.Player!.HasTarget)
            {
                Bot.Combat.Attack("*");
                Bot.Sleep(500);
            }

            if (!Bot.Self.Auras.Any(x => x.Name == "Focus"))
            {
                if (Bot.Skills.CanUseSkill(5))
                {
                    Bot.Sleep(Random.Shared.Next(500, 1001));
                    Bot.Skills.UseSkill(5);
                }
            }
        }
    }

    void DoEnh()
    {
        string className = Bot.Player!.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        switch (className.ToLower())
        {
            case "chrono shadowslayer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "legion revenant":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    wSpecial: WeaponSpecial.Arcanas_Concerto,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "archpaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "lord of order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "quantum chronomancer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Praxis,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "verus doomknight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "sentinel":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "archfiend":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "king's echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "void highlord":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;
        }
    }

}
