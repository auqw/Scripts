/*
name: UltraSpeaker
description: null
tags: Ultra
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Skua.Core.Interfaces;
using Skua.Core.Options;

// LR:
// "Weapon: Arcana"
// "Class: Wizard"
// "Helm: Wizard"
// "Cape: Penitence"
// "Scroll: Enrage"
// "Pots: Sage, Male";

// AP:
// "Weapon: Lacerate"
// "Class: Luck"
// "Helm: Luck"
// "Cape: Penitence"
// "Scroll: Enrage"
// "Pots: Fate, Battle";

// LOO:
// "Weapon: Valiance"
// "Class: Luck"
// "Helm: Luck"
// "Cape: Penitence"
// "Scroll: Enrage"
// "Pots: Fate, Battle";

// VDK:
// "Weapon: Valiance"
// "Class: Luck"
// "Helm: Anima"
// "Cape: Penitence"
// "Pots: Fate, Battle, Honor";

public class UltraSpeaker
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots CoreBot => CoreBots.Instance;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();
    string? className = null;
    private bool inZone = false;
    private int timeWait;
    private bool forceSkill = false;
    private int skillToForce;
    private string skills;

    public void ScriptMain(IScriptInterface bot)
    {
        // need custom skill, so can't use EnableSkills()
        Core.Boot();
        className = Bot.Player.CurrentClass?.Name?.ToLower();
        Core.DisableSkills();
        Bot.Events.ExtensionPacketReceived += UltraSpeakerListener;
        Bot.Events.ScriptStopping += OnScriptStopping;
        // Ultra.GetScrollOfEnrage();
        Core.EquipEnrage();
        setSKill();
        Kill();

        Bot.Events.ExtensionPacketReceived -= UltraSpeakerListener;
        Bot.Stop();

        bool OnScriptStopping(Exception? e)
        {
            Bot.Events.ExtensionPacketReceived -= UltraSpeakerListener;
            return true;
        }
    }

    void Kill()
    {
        Bot.Quests.UpdateQuest(9125);
        Core.Join("ultraspeaker");
        Ultra.WaitForArmy(3, "ultra_speaker.sync");
        Core.ChooseBestCell("The First Speaker");
        // Core.EnableSkills();

        string[] skillList = skills.Split(',');
        int[] intSkillList = skillList.Select(int.Parse).ToArray();
        int skillIndex = 0;

        while (!Bot.ShouldExit && Core.IsMonsterAliveByName("The First Speaker"))
        {
            if (Core.IsInCell("Boss"))
            {
                int targetX = inZone ? 203 : 100;
                int targetY = inZone ? 301 : 321;

                if (Bot.Player.Position.X != targetX)
                    _walkFlash(targetX, targetY);
            }

            if (
                (className == "lord of order" || className == "archpaladin")
                && (Core.IsArmyHealthLow(75) || Core.IsHealthLow(75))
            )
                Bot.Skills.UseSkill(2);

            if (!Bot.Player.HasTarget)
            {
                Core.Log("ATTACK", "ATTACKING");
                Bot.Combat.Attack("The First Speaker");
            }
            else
            {
                if (forceSkill)
                {
                    // Bot.Sleep(timeWait);
                    while (!Bot.ShouldExit)
                    {
                        if (Bot.Skills.CanUseSkill(skillToForce))
                        {
                            Bot.Skills.UseSkill(skillToForce);
                            break;
                        }
                        Bot.Sleep(100);
                    }
                    forceSkill = false;
                }

                int currentSkill = intSkillList[skillIndex];
                if (Bot.Skills.CanUseSkill(currentSkill))
                    Bot.Skills.UseSkill(currentSkill);
                skillIndex = (skillIndex + 1) % intSkillList.Length;

            }
            Bot.Sleep(100);
        }
        Core.Log("LOG", "FINISHED");
    }

    void _walkFlash(int X, int Y) => Bot.Flash.Call("walkTo", X, Y, 8);

    async void UltraSpeakerListener(dynamic packet)
    {
        string type = packet["params"].type;
        dynamic data = packet["params"].dataObj;
        if (type is not null and "json")
        {
            string cmd = data.cmd;
            switch (cmd)
            {
                case "event":
                    string zone = data.args?["zoneSet"]!;
                    // Core.Log("ZONE", $"THE ZONE IS {zone}");
                    if (zone is not null && zone == "A" && className == "legion revenant")
                    {
                        Core.Log("ZONE", "FORCE SKILL 1");
                        setForceSkill(1);
                        return;
                    }
                    break;

                case "ct":
                    var anims = data.anims as System.Collections.IEnumerable;
                    if (anims == null)
                        return;
                    foreach (var a in anims)
                    {
                        string? msg = (a as dynamic)?.msg?.ToString();
                        if (!string.IsNullOrEmpty(msg))
                        {
                            if (msg.ToLower().Contains("listen") || msg.ToLower().Contains("truth"))
                            {
                                var act = whatAction();

                                speakerCounter++;

                                if (className == act.Item2)
                                {
                                    if (act.Item3 == "IN")
                                    {
                                        inZone = true;
                                    }
                                    if (act.Item3 == "OUT")
                                    {
                                        inZone = false;
                                    }
                                }

                                if (className == act.Item1)
                                {
                                    setForceSkill(5, act.Item4);
                                }

                            }
                        }
                    }
                    break;
            }
        }
    }

    private int speakerCounter = 0;

    private (string?, string?, string?, int) whatAction()
    {
        // who taunt, who zone, in/out, wait skillLR
        switch (speakerCounter)
        {
            case 0:
                return ("legion revenant", null, null, 0);
            case 1:
                return ("lord of order", "verus doomknight", "IN", 0);
            case 2:
                return ("archpaladin", "verus doomknight", "OUT", 0);
            case 3:
            case 7:
                return ("lord of order", null, null, 500);
            case 4:
                return (null, "legion revenant", "IN", 0);
            case 5:
                return ("legion revenant", "legion revenant", "OUT", 500);
            case 8:
                return (null, "archpaladin", "IN", 0);
            case 9:
                return ("archpaladin", "archpaladin", "OUT", 0);
            case 10:
                return ("legion revenant", null, null, 500);
            case 11:
                return (null, "lord of order", "IN", 0);
            case 12:
                return ("lord of order", "lord of order", "OUT", 500);
            case 14:
                return ("legion revenant", null, null, 0);
            case 15:
                speakerCounter = 1;
                return ("lord of order", "verus doomknight", "IN", 500);
        }
        return (null, null, null, 0);
    }

    private void setForceSkill(int skill, int time = 0)
    {
        forceSkill = true;
        skillToForce = skill;
        timeWait = time;
    }

    private void setSKill()
    {
        switch (className)
        {
            case "legion revenant":
                skills = "2,3,4";
                break;

            case "archpaladin":
            case "lord of order":
            case "verus doomknight":
                skills = "1,2,3,4";
                break;
        }
    }
}