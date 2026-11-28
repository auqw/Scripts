/*
name: Purgatorio Charade
description: Completes Zazul's Purgatorio Charade quest and optionally farms all of its rewards.
tags: zazul, darkcarnival, friday the 13th, quest rewards
*/
//cs_include Scripts/CoreBots.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

#nullable enable

public class PurgatorioCharade
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    public string OptionsStorage = "PurgatorioCharade";
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
        int[] rewardIds = Core.EnsureLoad(4103).Rewards.Select(reward => reward.ID).ToArray();
        Bot.Drops.Add(rewardIds);

        while (!Bot.ShouldExit && (!infiniteFarm || !Core.CheckInventory(rewardIds, toInv: false)))
        {
            Core.EnsureAccept(4103);
            Core.HuntMonster("fotia", "Amia the Cult Leader", "Amia's Limbo Token");
            Core.HuntMonster("mercutio", "Mercutio", "Mercutio's 2nd Circle Token");
            Core.HuntMonster("styx", "Cerberus", "Cerberus' 3rd Circle Token");
            Core.HuntMonster("styx", "Wrathful Soul", "Wrathful Soul's 5th Circle Token");
            Core.HuntMonster("poisonforest", "Bandit", "Mercenary's 4th Circle Token");
            Core.HuntMonster("grimskullannex", "Grim Mage", "Grim Mage's 6th Circle Token");
            Core.HuntMonster("bloodtitan", "Blood Titan", "Blood Titan's 7th Circle Token", publicRoom: true);
            Core.HuntMonster("oaklore", "Undead Thief", "Undead Thief's 8th Circle Token");
            Core.HuntMonster("dreamnexus", "Khasaanda", "Khasaanda's 9th Circle Token");
            Core.EnsureComplete(4103);
            Bot.Wait.ForPickup("*");

            if (!infiniteFarm)
                break;
        }
    }
}
