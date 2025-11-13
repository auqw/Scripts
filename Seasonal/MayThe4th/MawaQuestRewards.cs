/*
name: Mawa's Quests Rewards
description: This will farm the rewards from Mawa's Quests in /eventhub.
tags: mawa, seasonal, may the 4th,quest reward,its a trap,droid for sale,get me parts, trobba fett house guard
*/
//cs_include Scripts/CoreBots.cs
// //cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class MawaQuestRewards
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetRewards();

        Core.SetOptions(false);
    }

    public void GetRewards()
    {
        DroidForSale();
        GetMeParts();
    }

    public void DroidForSale()
    {
        if (Core.CheckInventory(Core.QuestRewards(9745)) || !Core.isSeasonalMapActive("murdermoon"))
            return;

        Core.AddDrop(Core.QuestRewards(9745));

        //Droid For Sale (9745)
        string[] reqs = Core.QuestRequirements<string>(9745);
        Farm.Voucher("Gold Voucher 25k", 1);
        Core.HuntMonster("murdermoon", "Tempest Soldier", reqs[1], isTemp: false);
        Core.HuntMonster("twigguhunt", "Infantry Droid", reqs[2], isTemp: false);
        Core.ChainComplete(9745);
        Bot.Wait.ForDrop(Core.QuestRewards(9745)[0]);
    }

    public void GetMeParts()
    {
        if (
            Core.CheckInventory(Core.QuestRewards(10241)) || !Core.isSeasonalMapActive("murdermoon")
        )
            return;

        Core.AddDrop(Core.QuestRewards(10241));

        //Get Me Parts (10241)
        Core.HuntMonsterQuest(
            10241,
            ("twigguhunt", "Infantry Droid", ClassType.Farm),
            ("murdermoon", "Tempest Soldier", ClassType.Farm),
            ("murdermoon", "Fifth Sepulchure", ClassType.Solo),
            ("eventhub", "Moon Guard", ClassType.Solo)
        );
        Bot.Wait.ForDrop(Core.QuestRewards(10241)[0]);
    }
}
