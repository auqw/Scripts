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

// VDK/Other dps:
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
    private bool inZone = false;
    private int timeWait;
    private bool forceSkill = false;
    private int skillToForce;
    private string skills = "1,2,3,4";

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
        Ultra.GetScrollOfEnrage();
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Core.EquipEnrage();
    }

    void Kill()
    {
        if (!C.isCompletedBefore(9173))
            C.Logger("Quest 9173 not unlocked.");

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
            if (Ultra.CheckArmyProgress("The First Speaker Silenced", 1, false, syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                C.EnsureComplete(9173);
                break;
            }

            // Dead → wait for respawn
            if (Bot.Player?.Alive == false)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            // Put the player in a random spot within ((x=0,y=0), (x=101, y=101)) -- in the corner.
            if (Bot.Player?.Cell == "Boss")
            {
                // Define box boundaries (0,0 to 101,101)
                int minX = 0;
                int maxX = 100;
                int minY = 485;
                int maxY = 500;

                // Check if player is within the box
                bool isInBox =
                    Bot.Player.Position.X >= minX
                    && Bot.Player.Position.X <= maxX
                    && Bot.Player.Position.Y >= minY
                    && Bot.Player.Position.Y <= maxY;

                // If not in box, move to random location within box
                if (!isInBox)
                {
                    Random rand = new();
                    int randomX = rand.Next(minX, maxX + 1);
                    int randomY = rand.Next(minY, maxY + 1);
                    Bot.Player.WalkTo(randomX, randomY);
                }
            }

            if (!Bot.Player.HasTarget)
                Bot.Combat.Attack("*");
            Bot.Sleep(200);
            if (
                !Bot.Self.Auras.Any(x => x != null && x.Name == "Focus")
                && Bot.Skills.CanUseSkill(5)
            )
                Bot.Skills.UseSkill(5);
        }
    }
}
