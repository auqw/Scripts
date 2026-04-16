/*
name: Elders Blood (Army)
description: One-client version of the Elders' Blood daily
tags: daily, dailies, army, elders, blood, vhl, void highlord, oneclient
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class ArmyEldersBlood
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

        // EldersBlood();
        Bot.Log("OneClient scripts area bit fucked, and i just cant fix the \"not logging into full server\" issue.. so they will be disabled -- please use the normal varients of the scripts.");


        Core.SetOptions(false);
    }

    public void EldersBlood()
    {
        Bot.Options.ReloginServer =
            (Bot.Options.ReloginServer ?? "Twilly") ?? new[] { "Twilly", "Twig" }[new Random().Next(2)];

        while (!Bot.ShouldExit && Army.doForAll())
        {
            if (Bot.Inventory.FreeSlots <= 0)
                continue;
            Dailies.EldersBlood();
        }
    }
}
