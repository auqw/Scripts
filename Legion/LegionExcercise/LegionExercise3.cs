/*
name: Legion Exercise Number 3
description: This script will complete "Legion Exercise Number 3" quest.
tags: legion, exercise, legion exercise, 3, judgement hammer
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class LegionExercise3
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreLegion Legion
    {
        get => _Legion ??= new CoreLegion();
        set => _Legion = value;
    }
    private static CoreLegion _Legion;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    private string[] Rewards = { "Judgement Hammer", "Legion Token" };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Exercise(Rewards);

        Core.SetOptions(false);
    }

    public void Exercise(string[] item)
    {
        if (Core.CheckInventory(item))
            return;
        Core.AddDrop(item);

        Legion.JoinLegion();
        Core.BuyItem("underworld", 216, "Undead Champion");

        Core.Logger(
            "Disclaimer: Percentages are randomized, just made purely for fun. i cba making it an actualy %age"
        );

        int Dice = Bot.Random.Next(1, 101);
        //-------------------------------------------------------------------------------------------------------

        int i = 1;
        var displayPercentage = $"{(decimal)Dice / 100:P}";

        Core.Logger(
            $"Potato Prediction Inc. Decided: {displayPercentage} is The Chance for Desired Rewards."
        );

        while (!Bot.ShouldExit)
        {
            Core.EnsureAccept(823);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("Uppercity", "Chaos Egg", "Chaos Egg", 24, isTemp: false);
            Core.KillMonster("chaosmarsh", "Forest3", "Left", "Chaos Spider", "Chaorrupted Essence", 50, isTemp: false, publicRoom: false);

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("Underworld", "Dreadfiend Of Nulgath", "Darkness Core", publicRoom: false);

            Core.EnsureComplete(823);
            if (Bot.Drops.Exists("Judgement Hammer"))
                Bot.Wait.ForPickup("Judgement Hammer");

            if (Core.CheckInventory("Judgement Hammer"))
            {
                Core.Logger($"{Rewards} Aquired");
                Core.Logger($"Farming Took {i++} Times");
            }
        }


        if (Dice > i++)
            Core.Logger($"Perdiction: [{Dice}] was Higher... Congratulations!");

        Core.Logger($"Perdiction: [{Dice}] was lower... sorry it took so long D:!");

        Core.ToBank(Rewards);
    }
}
