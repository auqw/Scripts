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
using Skua.Core.Models.Auras;
using Skua.Core.Options;

#region Fast Comp

/// <summary>
/// Fast Composition - Maximum damage output for speed
/// </summary>
// ArchPaladin
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Valiance
// └─ Cape: Lament
//
// Legion Revenant
// ├─ Class: Wizard
// ├─ Helm: Pneuma
// ├─ Weapon: Arcana's Concerto
// └─ Cape: Vainglory
//
// Quantum Chronomancer
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Praxis
// └─ Cape: Penitence
//
// Lord of Order
// ├─ Class: Lucky
// ├─ Helm: Forge
// ├─ Weapon: Valiance
// └─ Cape: Penitence

#endregion

#region Safe Comp

/// <summary>
/// Safe Composition - Original balanced setup
/// </summary>
// Legion Revenant
// ├─ Class: Wizard
// ├─ Helm: Wizard
// ├─ Weapon: Arcana/Valiance
// └─ Cape: Penitence
// └─ Scroll: Enrage
//
// ArchPaladin
// ├─ Class: Lucky
// ├─ Helm: Luck
// ├─ Weapon: Lacerate/Valiance
// └─ Cape: Penitence
// └─ Scroll: Enrage
//
// Lord of Order
// ├─ Class: Lucky
// ├─ Helm: Luck
// ├─ Weapon: Valiance
// └─ Cape: Penitence
// └─ Scroll: Enrage
//
// Verus DoomKnight (or other DPS)
// ├─ Class: Lucky
// ├─ Helm: Anima
// ├─ Weapon: Valiance
// └─ Cape: Penitence
// └─ Scroll: Enrage

#endregion

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
        new Option<SpeakerComp>(
            "DoEquipClasses",
            "Automatically Equip Classes",
            "Auto-equip classes across all 4 clients\n"
                + "Fast: AP / LR / QCM / LOO\n"
                + "Safe: LR / AP / LOO / VDK\n"
                + "SafeV2: LR / AP / LOO / SC\n"
                + "Unselected = off (use whatever classes you already have equipped).",
            SpeakerComp.Unselected
        ),
        new Option<bool>("DoEnh", "Do Enhancements",  "Auto-Enhance Gear properly for the fight", true),
        CoreBots.Instance.SkipOptions,
    };


    public void ScriptMain(IScriptInterface bot)
    {
        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();

        C.Logger("This script uses the `corner spam taunt method.. and works ^_^");
        className = Bot.Player.CurrentClass?.Name?.ToLower();
        Core.Boot();
        Prep();
        Kill();
        C.SetOptions(false);
    }

    void Prep()
    {
        // Sync-equip classes if a comp is selected
        SpeakerComp comp = Bot.Config!.Get<SpeakerComp>("DoEquipClasses");
        if (comp != SpeakerComp.Unselected)
        {
            string[] classes = comp switch
            {
                SpeakerComp.Fast => new[] { "ArchPaladin", "Legion Revenant", "Quantum Chronomancer", "Lord of Order" },
                SpeakerComp.Safe => new[] { "Legion Revenant", "ArchPaladin", "Lord of Order", "Verus DoomKnight" },
                SpeakerComp.SafeV2 => new[] { "Legion Revenant", "ArchPaladin", "Lord of Order", "StoneCrusher" },
                _ => new[] { "ArchPaladin", "Legion Revenant", "Lord of Order", "Verus DoomKnight" },
            };

            Ultra.EquipClassSync(classes, 4, "speaker_class.sync");
        }

        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnh();
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.GetScrollOfEnrage();
    }


    void Kill()
    {
        if (!Bot.Quests.IsUnlocked(9173))
            Bot.Log("Ultra Quest isn't unlocked, we'll fakeunlock it so you can atleast get the quest drop");

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);
        Bot.Options.DisableCollisions = true;
        C.EnsureAccept(9173);
        C.AddDrop("The First Speaker Silenced");
        if (!Bot.Quests.IsUnlocked(9173))
            Bot.Quests.UpdateQuest(9125);
        Core.Join("ultraspeaker");
        Ultra.WaitForArmy(3, "ultra_speaker.sync");
        Core.ChooseBestCell("The First Speaker");
        Core.EnableSkills();

        while (!Bot.ShouldExit)
        {
            // Dead → wait for respawn
            if (!Bot.Player!.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            // Check if all players are done
            bool allComplete = Ultra.CheckArmyProgressBool(
                () => Bot.Inventory.Contains("The First Speaker Silenced", 1),
                syncPath
            );

            if (allComplete)
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                if (Bot.Quests.IsDailyComplete(9173))
                {
                    C.Logger("Weekly already complete, try again Friday morning");
                    if (Bot.Config!.Get<bool>("DoEnh"))
                        return;
                }
                else C.EnsureComplete(9173);
                if (Bot.Config!.Get<bool>("DoEnh"))
                    Adv.GearStore(true, true);
                break;
            }

            // If we're petrified, wait
            if (Bot.Self.Auras.Any(a => a.Name == "Stasis"))
            {
                Core.DisableSkills();
                Bot.Wait.ForTrue(() => Bot.Self.Auras.Any(a => a.Name == "Stasis"), 20);
                Core.EnableSkills();
                continue;
            }

            // Position management in Boss cell
            if (Bot.Player?.Cell == "Boss")
            {
                int minX = 0, maxX = 100;
                int minY = 485, maxY = 500;

                bool isInBox =
                    Bot.Player.Position.X >= minX &&
                    Bot.Player.Position.X <= maxX &&
                    Bot.Player.Position.Y >= minY &&
                    Bot.Player.Position.Y <= maxY;

                if (!isInBox)
                {
                    Random rand = new();
                    int randomX = rand.Next(minX, maxX + 1);
                    int randomY = rand.Next(minY, maxY + 1);
                    Bot.Player.WalkTo(randomX, randomY);
                    Bot.Sleep(500);
                }
            }

            // Combat logic - only attack if monster exists
            if (Bot.Monsters.CurrentMonsters.Any(m => m.Name == "The First Speaker" && m.Alive))
            {
                Bot.Combat.Attack("The First Speaker");

                // Use skill 5 if Focus aura is not present
                if (!Bot.Target.Auras.Any(a => a.Name == "Focus") && Bot.Skills.CanUseSkill(5))
                {
                    Bot.Skills.UseSkill(5);
                }
            }
            Bot.Sleep(500);
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

    public enum SpeakerComp
    {
        Unselected,
        Fast,
        Safe,
        SafeV2,
    }
}
