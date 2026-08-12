/*
name: All accs bank all
description: banks all items on all accs in the "thefamily.txt" file.
tags: bank, army, all
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Tools/BankAllItems.cs
using Skua.Core.Interfaces;

public class ArmyBankAllItems
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static BankAllItems BAI
    {
        get => _BAI ??= new BankAllItems();
        set => _BAI = value;
    }
    private static BankAllItems _BAI;
    private static CoreArmyLite Army
    {
        get => _Army ??= new CoreArmyLite();
        set => _Army = value;
    }
    private static CoreArmyLite _Army;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        // AllBankAll();
        Bot.Log("OneClient scripts area bit fucked, and i just cant fix the \"not logging into full server\" issue.. so they will be disabled -- please use the normal varients of the scripts.");


        Core.SetOptions(false);
    }

    public void AllBankAll()
    {
        Bot.Options.ReloginServer =
            (Bot.Options.ReloginServer ?? "Twilly") ?? new[] { "Twilly", "Twig" }[new Random().Next(2)];

        while (!Bot.ShouldExit && Army.doForAll())
        {
            BAI.BankAll(true, true, false, string.Empty);
            Core.Logger("All \"Allowed\" items banked, onto the next Acc");

            // if either are enabled set them to empty, so the next acc doesnt try to
            if (Core.FarmGearOn && Core.FarmGear.Length > 0)
                Core.FarmGear = [];
            if (Core.SoloGearOn && Core.SoloGear.Length > 0)
                Core.SoloGear = [];
        }
    }
}
