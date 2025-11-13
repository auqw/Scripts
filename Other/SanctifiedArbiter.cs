/*
name: Sanctified Arbiter Set
description: Farms "All Drops" From Quest: "One Holy Discovery".
tags: sanctified arbiter, drops, one holy discovery
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class SanctifiedArbiter
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

        GetAll();

        Core.SetOptions(false);
    }

    public void GetAll()
    {
        List<ItemBase> RewardOptions = Core.EnsureLoad(8114).Rewards;
        List<string> RewardsList = new();
        List<string> RewardList = RewardOptions.Select(x => x.Name).ToList();
        string[] Rewards = RewardList.ToArray();

        if (Core.CheckInventory(Rewards))
            return;

        for (int i = 0; i < Rewards.Length; i++)
        {
            if (!Core.CheckInventory(Rewards[i]))
            {
                Core.Logger($"Getting \"{Rewards[i]}\"");
                //One Holy Discovery 8114
                Core.EnsureAccept(8114);

                Core.EquipClass(ClassType.Farm);
                Farm.BattleUnderB("Bone Dust", 500);
                Core.HuntMonster(
                    "Doomwood",
                    "Doomwood Ectomancer",
                    "Raw Essence of the Undead",
                    10
                );

                Core.EnsureCompleteChoose(8114, new[] { Rewards[i] });
            }
            Core.ToBank(Rewards[i]);
        }
    }
}
