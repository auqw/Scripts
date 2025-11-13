/*
name: Free Daily Boost Daily
description: Free Daily Boost
tags: daily, free daily boost, member, boost, free, 60 minutes, 60 min, sixty minutes
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreDailies.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class FreeDailyBoost
{
    public string OptionsStorage = "FreeDailyBoost(mem)";
    public bool DontPreconfigure = true;

    public List<IOption> Options = new()
    {
        new Option<DailyBoostRewards>(
            "BoostReward",
            "Choose Your Daily Boost Reward",
            "Select the reward to apply (XP, Gold, Rep, Class Boost)",
            DailyBoostRewards.LowestQuantOwned
        ), // Single option for reward
        CoreBots.Instance.SkipOptions, // Skip options when set
    };

    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreDailies Daily
    {
        get => _Daily ??= new CoreDailies();
        set => _Daily = value;
    }
    private static CoreDailies _Daily;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        // Get the selected reward from the script options and log it
        DailyBoostRewards selectedReward = Bot.Config!.Get<DailyBoostRewards>("BoostReward");

        // Call FreeDailyBoost, passing in the selected reward
        Daily.FreeDailyBoost(selectedReward);

        Core.SetOptions(false);
    }
}
