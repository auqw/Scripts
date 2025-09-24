//cs_include Scripts/WIP/CoreUltras.cs

using System;
using System.Dynamic;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraMain
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreUltras Core = new();

    public void ScriptMain(IScriptInterface bot)
    {
        Core.Boot();

        TestUltras();

        Bot.Stop();
    }

    void TestUltras()
    {
        KillUltraWarden("ArchPaladin");
        KillUltraEngineer();
        KillChampionDrakath("ArchPaladin");
    }

    // 🠗 perspective on all ultras with no forge enhancements, only health vamp enhancements 🠗
    // tought to fight because it lock skills and bot keeps using locked skills; wip skills system
    void KillUltraEzrajal()
    {
        Core.Join("ultraezrajal");
        Core.WaitForArmy(3);
        Core.ChooseBestCell("Ultra Ezrajal");
        Core.EnableSkills();

        while (Core.MonsterAlive("Ultra Ezrajal") && !Bot.ShouldExit)
            Core.Attack("Ultra Ezrajal");
    }

    // suggestion: StoneCrusher as taunter
    void KillUltraWarden(string taunterClass)
    {
        if (Core.HasClassEquipped(taunterClass))
            Core.GetScrollOfEnrage();

        Core.Join("ultrawarden");
        Core.WaitForArmy(3);
        Core.ChooseBestCell("Ultra Warden");
        Core.EnableSkills();

        while (Core.MonsterAlive("Ultra Warden") && !Bot.ShouldExit)
            if (Core.HasClassEquipped(taunterClass))
                Core.UltraWardenTaunter();
            else
                Core.Attack("Ultra Warden");
    }

    void KillUltraEngineer()
    {
        Core.Join("ultraengineer");
        Core.WaitForArmy(3);
        Core.ChooseBestCell("Ultra Engineer");
        Core.EnableSkills();

        while (Core.MonsterAlive("Ultra Engineer") && !Bot.ShouldExit)
            Core.KillWithPriority("Ultra Engineer", 3, "Defense Drone", 2, "Attack Drone", 1);
    }

    void KillChampionDrakath(string taunterClass)
    {
        if (Core.HasClassEquipped(taunterClass))
            Core.GetScrollOfEnrage();

        Core.Join("championdrakath");
        Core.WaitForArmy(3);
        Core.ChooseBestCell("Champion Drakath");
        Core.EnableSkills();

        while (Core.MonsterAlive("Champion Drakath") && !Bot.ShouldExit)
        {
            if (Core.HasClassEquipped(taunterClass))
                Core.DrakathTaunter();
            else
                Core.Attack("Champion Drakath");
        }
    }

    // suggestion: Chaos Avenger and Legion DoomKnight as taunter
    void KillUltraDage(string primaryTaunter, string secondaryTaunter)
    {
        if (Core.HasClassEquipped(primaryTaunter) || Core.HasClassEquipped(secondaryTaunter))
            Core.GetScrollOfEnrage();

        Bot.Events.ExtensionPacketReceived += Core.ZoneSetListener;

        Core.Join("ultradage");
        Core.WaitForArmy(3);
        Core.ChooseBestCell("Dage the Dark Lord");
        Core.EnableSkills();

        while (Core.MonsterAlive("Dage the Dark Lord") && !Bot.ShouldExit)
        {
            if (Core.HasClassEquipped(primaryTaunter))
                Core.TauntCycle(primaryTaunter, "Dage the Dark Lord", "Focus", 250);
            else if (Core.HasClassEquipped(secondaryTaunter))
                Core.TauntCycle(secondaryTaunter, "Dage the Dark Lord", "Focus", 700);
            else
                Core.Attack("Dage the Dark Lord");
        }
    }

    // suggestion: Lord of Order and ArchPaladin as taunter
    // will die a couple of times until it find the harmony
    void KillUltraAvatarTyndarius(string primaryTaunter, string secondaryTaunter)
    {
        if (Core.HasClassEquipped(primaryTaunter) || Core.HasClassEquipped(secondaryTaunter))
            Core.GetScrollOfEnrage();

        Core.Join("ultratyndarius");
        Core.WaitForArmy(3);
        Core.ChooseBestCell("Ultra Avatar Tyndarius");
        Core.EnableSkills();

        while (Core.MonsterAlive("Ultra Avatar Tyndarius") && !Bot.ShouldExit)
        {
            if (Core.HasClassEquipped(primaryTaunter))
                Core.TauntCycle(primaryTaunter, "Ultra Avatar Tyndarius", "Focus", 250);
            else if (Core.HasClassEquipped(secondaryTaunter))
                Core.TauntCycle(secondaryTaunter, "Ultra Avatar Tyndarius", "Focus", 700);
            else
                Core.KillWithPriority("Ultra Avatar Tyndarius", 2, "Ultra Fire Orb", 3, "Ultra Fire Orb", 1);
        }
    }

    void KillXyfrag(string taunterClass)
    {
        if (Core.HasClassEquipped(taunterClass))
        {
            Core.GetScrollOfEnrage();
            Bot.Events.ExtensionPacketReceived += Core.ChargeListener;
        }

        Core.Join("voidxyfrag");
        Core.WaitForArmy(6);
        Core.ChooseBestCell("Xyfrag");
        Core.EnableSkills();

        while (Core.MonsterAlive("Xyfrag") && !Bot.ShouldExit)
        {
            if (Core.HasClassEquipped(taunterClass))
                Core.TauntCharge(taunterClass, "Xyfrag", "Focus", 250);
            else
                Core.Attack("Xyfrag");
        }
    }
}
