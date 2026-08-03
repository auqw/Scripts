/*
name: Yes Ma'am
description: Runs the Back-to-School 'Yes Ma'am!' test quest chain [10815 - 10823].
tags: back-to-school, yes maam, seasonal, quest chain, extra credit
*/
//cs_include Scripts/CoreBots.cs
using Skua.Core.Interfaces;

public class YesMaam
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        DoChain();

        Core.SetOptions(false);
    }

    public void DoChain()
    {
        if (!Core.isSeasonalMapActive("extracredit"))
            return;

        Core.AddDrop("1st Grade School Supplies");
        Core.EquipClass(ClassType.Farm);

        // Test #1: Physical Education
        Test(10815, () =>
        {
            Core.GetMapItem(15995, 1, "swordhaven");
            Core.GetMapItem(15996, 1, "mythsong");
            Core.GetMapItem(15997, 1, "sandsea");
            Core.GetMapItem(15998, 1, "embersea");
        });

        // Test #2: History
        Test(10816, () =>
        {
            Core.KillMonster("bloodtusk", "r3", "Center", "Crystal-Rock", "Crystal-Rock Collected", 2);
            Core.KillMonster("bloodtusk", "r3", "Center", "Rock", "Rock Collected", 2);
        });

        // Test #3: Math
        Test(10817, () =>
        {
            Core.KillMonster("blackhorn", "r3", "Left", "Tomb Spider", "Tomb Spider Legs", 25);
            Core.KillMonster("boxes", "Fort2", "Left", "Grizzlespit", "Grizzlespit Beard Hair", 500);
            Core.KillMonster("arcangrove", "LeftBack", "Left", "Gorillaphant", "Gorillaphant Hair", 1000);
        });

        // Test #4: Reading and Writing
        Test(10818, () =>
        {
            Core.GetMapItem(15999, 3, "extracredit");
            Core.KillMonster("extracredit", "r5", "Left", "Cute But Evil: A+", "Denied Cat From Eating Homework", 1);
        });

        // Test #5: Science
        Test(10819, () => Core.GetMapItem(16000, 1, "swordhaven"));

        // Outsourcing for Resources
        Test(10820, () =>
        {
            Core.KillMonster("deltavlab", "r2", "Left", "Lab Guard", "Steel Pieces", 6);
            Core.KillMonster("redfurvalley", "r2", "Right", "*", "Drone Wires", 12);
        });

        // Test #Subject
        Test(10821, () =>
            Core.KillMonster("orctown", "Tower", "Right", "General Porkon", "General Porkon \"Convinced\"", 1));

        // Return to Yes Ma'am
        Test(10822, () => Core.GetMapItem(16001, 1, "extracredit"));

        // Test #5.5: Battle
        Test(10823, () =>
            Core.KillMonster("extracredit", "r6", "Left", "Mind-Controlled Porkon", "Mind-Controlled Porkon Defeated", 1));
    }

    private void Test(int questID, Action steps)
    {
        if (Core.isCompletedBefore(questID))
            return;

        Core.EnsureAccept(questID);
        steps();
        Core.EnsureComplete(questID);
    }
}
