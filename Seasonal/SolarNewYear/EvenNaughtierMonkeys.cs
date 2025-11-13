/*
name: Hanzo Orb Quests
description: Does the quests from either the Astral Orb Pet, or the Crimson Orb Pet
tags: astral, crimson, orb, pet, quests
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Seasonal/SolarNewYear/WaterWar.cs
using Skua.Core.Interfaces;

public class EvenNaughtierMonkeys
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static WaterWar WW
    {
        get => _WW ??= new WaterWar();
        set => _WW = value;
    }
    private static WaterWar _WW;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Rewards();

        Core.SetOptions(false);
    }

    public void Rewards()
    {
        WW.StoryLine();

        var rewards = Core.QuestRewards(6821);
        Core.AddDrop(rewards);

        Core.RegisterQuests(6821);
        foreach (string item in rewards)
        {
            while (!Bot.ShouldExit && !Core.CheckInventory(item))
                Core.HuntMonster("waterwar", "Temple Gibbon", "Clean Rag", 7);
        }
        Core.CancelRegisteredQuests();
    }
}
