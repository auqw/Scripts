/*
name: Memory of my Father
description: Farms quest 10835 until all four random rewards are owned.
tags: memory of my father, drakath, crownsreach, slugwrath, quest rewards
*/
//cs_include Scripts/CoreBots.cs
using Skua.Core.Interfaces;

#nullable enable

public class MemoryOfMyFather
{
    private CoreBots Core => CoreBots.Instance;
    private IScriptInterface Bot => IScriptInterface.Instance;

    private readonly string[] Rewards =
    {
        "Royal Dictator's Formal Wear",
        "King Drakath Slugwrath Morph",
        "Royal Dictator's Ceremonial Cape",
        "Commemorative Statue of Slugwrath"
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(Rewards);
        Core.SetOptions();
        try
        {
            GetRewards();
        }
        finally
        {
            Core.SetOptions(false);
        }
    }

    private void GetRewards()
    {
        Core.EquipClass(ClassType.Solo);
        Bot.Drops.Add(Rewards);

        while (!Bot.ShouldExit && !Core.CheckInventory(Rewards, toInv: false))
        {
            Core.EnsureAccept(10835);
            Core.HuntMonster("hiddenduat", "Pharaoh Neith", "Pharaoh Neith Defeated");
            Core.HuntMonster("deepforest", "Aberrant Horror", "Aberrant Horror Defeated");
            Core.HuntMonster("wardwarf", "Ultra D'wain Jonsen", "Ultra D'wain Jonsen Defeated");
            Core.HuntMonster("queenbattle", "Proto Chaos Champion", "Proto Chaos Champion Defeated");
            Core.EnsureComplete(10835);

            foreach (string reward in Rewards)
                Bot.Wait.ForPickup(reward);
        }
    }
}
