/*
name: Dead Ringer... Master
description: Completes Zazul's Dead Ringer... Master quest and optionally farms all of its rewards.
tags: zazul, darkcarnival, friday the 13th, quest rewards
*/
//cs_include Scripts/CoreBots.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

#nullable enable

public class DeadRingerMaster
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    public string OptionsStorage = "DeadRingerMaster";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<bool>("InfiniteFarm", "Infinite Farm", "Repeat until you own every quest reward. If off, complete the quest once.", false),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        try
        {
            FarmRewards();
        }
        finally
        {
            Core.SetOptions(false);
        }
    }

    private void FarmRewards()
    {
        if (!Core.isSeasonalMapActive("darkcarnival"))
            return;

        bool infiniteFarm = Bot.Config?.Get<bool>("InfiniteFarm") == true;
        int[] rewardIds = Core.EnsureLoad(4104).Rewards.Select(reward => reward.ID).ToArray();
        Bot.Drops.Add(rewardIds);

        while (!Bot.ShouldExit && (!infiniteFarm || !Core.CheckInventory(rewardIds, toInv: false)))
        {
            Core.EnsureAccept(4104);
            Core.HuntMonster("underrealm", "Death", "Dante's Divine Tragedy");
            Core.EnsureComplete(4104);
            Bot.Wait.ForPickup("*");

            if (!infiniteFarm)
                break;
        }
    }
}
