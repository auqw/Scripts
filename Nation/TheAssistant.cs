/*
name: TheAssistant
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class TheAssistant
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;

    public string OptionsStorage = "TheAssistant";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<SwindlesReturnReward>(
            "ChooseReward",
            "Choose Your Quest Reward",
            "if `returnPolicyDuringSupplies` is enabled in CoreBot Options, Choose the Reward here",
            (int)SwindlesReturnReward.None
        ),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        dothing();

        Core.SetOptions(false);
    }

    public void dothing()
    {
        if (Bot.Config == null)
        {
            Core.Logger("Config is null.");
            return;
        }

        if (Bot.Config.Get<SwindlesReturnReward>("ChooseReward") == SwindlesReturnReward.None)
            return;

        SwindlesReturnReward Reward = Bot.Config.Get<SwindlesReturnReward>("ChooseReward");

        Nation.TheAssistant(null, 1000, true, Reward);
    }
}
