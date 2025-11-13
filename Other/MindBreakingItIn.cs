/*
name: Mind Breaking It In
description: Farms "All Drops" From Quest: "Mind Breaking It In".
tags: mind breaking it in, mindbreaker, drops
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class MindBreakingItIn
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetRewards();

        Core.SetOptions(false);
    }

    public void GetRewards()
    {
        List<ItemBase> RewardOptions = Core.EnsureLoad(7672).Rewards;
        List<string> RewardsList = new();
        List<string> RewardList = RewardOptions.Select(x => x.Name).ToList();
        string[] Rewards = RewardList.ToArray();

        if (Core.CheckInventory(Rewards))
            return;

        if (!Core.CheckInventory("MindBreaker"))
        {
            //Member Bonus 12k AC Shop will check if the player have the achievment
            Adv.BuyItem("Battleon", 373, "MindBreaker");
            Adv.RankUpClass("MindBreaker");
        }

        for (int i = 0; i < Rewards.Length; i++)
        {
            if (!Core.CheckInventory(Rewards[i]))
            {
                Core.Logger($"Getting {Rewards[i]}");
                //Mind-Breaking It In 7672
                Core.EnsureAccept(7672);
                Core.EquipClass(ClassType.Farm);
                Core.HuntMonster("Somnia", "Deorysa", "Dream Devourers Vanquished", 50, false);
                Core.EquipClass(ClassType.Solo);
                Core.HuntMonster("Somnia", "NightWyrm", "Nightwyrm Vanquished");
                Core.EnsureCompleteChoose(7672, new[] { Rewards[i] });
            }
        }
        Core.ToBank(Rewards);
    }
}
