/*
name: The Wheel of Doom (Army)
description: One-client version of the Wheel of Doom
tags: daily, dailies, army, wheel, doom, oneclient
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class ArmyWheelofDoom
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreArmyLite Army
    {
        get => _Army ??= new CoreArmyLite();
        set => _Army = value;
    }
    private static CoreArmyLite _Army;
    private static CoreDailies Dailies
    {
        get => _Dailies ??= new CoreDailies();
        set => _Dailies = value;
    }
    private static CoreDailies _Dailies;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        WheelOfDoom();

        Core.SetOptions(false);
    }

    public void WheelOfDoom()
    {
        Bot.Options.ReloginServer =
            (Bot.Options.ReloginServer ?? "Twilly") ?? new[] { "Twilly", "Twig" }[new Random().Next(2)];

        while (!Bot.ShouldExit && Army.doForAll())
        {
            if (Bot.Inventory.FreeSlots <= 0)
                continue;

            Dailies.WheelofDoom();
        }
    }
}
