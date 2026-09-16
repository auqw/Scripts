/*
name: Do All Four Harbingers
description: Runs Halosis, Bello, Fame, Mors, and Anethyxos Absolution in sequence.
tags: four harbingers, fourharbingers, halosis, bello, fame, mors, anethyxos, absolution, boss, farm, lonewolf12
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Prototypes/FourHarbingers/Halosis.cs
//cs_include Scripts/Prototypes/FourHarbingers/Bello.cs
//cs_include Scripts/Prototypes/FourHarbingers/Fame.cs
//cs_include Scripts/Prototypes/FourHarbingers/Mors.cs
//cs_include Scripts/Prototypes/FourHarbingers/AnethyxosAbsolution.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class DoAllFourHarbingers
{
    public string OptionsStorage = "FourHarbingers_DoAll";
    public bool DontPreconfigure = true;

    public List<IOption> Options = new()
    {
        new Option<bool>("UsePotions", "Use Potions", "Use optimized potions during each fight.", true),
        new Option<bool>("DoEnhancements", "Do Enhancements", "Apply optimized enhancements before each fight.", true),
        CoreBots.Instance.SkipOptions,
    };

    private readonly IScriptInterface Bot = IScriptInterface.Instance;
    private readonly CoreBots Core = CoreBots.Instance;

    public void ScriptMain(IScriptInterface bot)
    {
        if (!Bot.Config.Get<bool>(CoreBots.Instance.SkipOptions))
            Bot.Config.Configure();

        Core.SetOptions(disableClassSwap: true);
        try
        {
            RunAll();
        }
        finally
        {
            Core.SetOptions(false);
        }
    }

    private void RunAll()
    {
        Core.Logger("Starting Four Harbingers sequence...");

        Halosis halosis = new() { DoAllMode = true, FarmQuantity = 1 };
        halosis.ScriptMain(Bot);

        Bello bello = new() { DoAllMode = true, FarmQuantity = 1 };
        bello.ScriptMain(Bot);

        Fame fame = new() { DoAllMode = true, FarmQuantity = 1 };
        fame.ScriptMain(Bot);

        Mors mors = new() { DoAllMode = true, FarmQuantity = 1 };
        mors.ScriptMain(Bot);

        AnethyxosAbsolution absolution = new() { DoAllMode = true, FarmQuantity = 1 };
        absolution.ScriptMain(Bot);

        Core.Logger("Four Harbingers sequence complete.");
    }
}
