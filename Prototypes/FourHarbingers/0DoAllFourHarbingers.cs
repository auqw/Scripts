/*
name: Do All Four Harbingers
description: Completes the Four Harbingers story using the optimized boss setups.
tags: four harbingers, fourharbingers, story, quest, complete, all, halosis, bello, fame, mors, anethyxos, lonewolf12
*/
//cs_include Scripts/CoreBots.cs
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
        new Option<ClassChoice>("ClassChoice", "Choose Class", "Optimized uses:\n\nHalosis - Dragon of Time\nBello - ArchPaladin\nFame - Yami no Ronin\nMors - Legion Revenant\nAnethyxos - King's Echo\n\nMore class options are available in the individual boss scripts.", ClassChoice.Optimized),
        new Option<bool>("UsePotions", "Use Potions", "Use the optimized potions during each fight.", true),
        new Option<bool>("DoEnhancements", "Do Enhancements", "Apply the optimized enhancements before each fight.", true),
        new Option<bool>("FarmBosses", "Farm Bosses?", "Boss farming is available in the individual boss scripts.", false),
        CoreBots.Instance.SkipOptions,
    };

    private IScriptInterface Bot = IScriptInterface.Instance;
    private CoreBots Core = CoreBots.Instance;

    public void ScriptMain(IScriptInterface bot)
    {
        if (bot.Config != null)
            bot.Config.Configure();

        if (Bot.Config!.Get<bool>("FarmBosses"))
            Core.Logger("To farm bosses, run the individual Four Harbingers boss scripts. They also contain more class choices. Continuing the story without farming.", messageBox: true);

        if (!Core.isCompletedBefore(10850))
        {
            Halosis halosis = new();
            halosis.DoAllMode = true;
            halosis.ScriptMain(bot);
            if (!Core.isCompletedBefore(10850))
                return;
        }

        if (!Core.isCompletedBefore(10851))
        {
            Bello bello = new();
            bello.DoAllMode = true;
            bello.ScriptMain(bot);
            if (!Core.isCompletedBefore(10851))
                return;
        }

        if (!Core.isCompletedBefore(10852))
        {
            Fame fame = new();
            fame.DoAllMode = true;
            fame.ScriptMain(bot);
            if (!Core.isCompletedBefore(10852))
                return;
        }

        if (!Core.isCompletedBefore(10853))
        {
            Mors mors = new();
            mors.DoAllMode = true;
            mors.ScriptMain(bot);
            if (!Core.isCompletedBefore(10853))
                return;
        }

        if (!Core.isCompletedBefore(10854))
        {
            AnethyxosAbsolution absolution = new();
            absolution.DoAllMode = true;
            absolution.ScriptMain(bot);
            if (!Core.isCompletedBefore(10854))
                return;
        }

        Core.Logger("Four Harbingers story complete.");
    }

    private enum ClassChoice
    {
        Optimized,
    }
}
