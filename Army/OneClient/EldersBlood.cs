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
    private static CoreArmyLite Army { get => _Army ??= new CoreArmyLite(); set => _Army = value; }
    private static CoreArmyLite _Army;
    private static CoreDailies Dailies { get => _Dailies ??= new CoreDailies(); set => _Dailies = value; }
    private static CoreDailies _Dailies;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        EldersBlood();

        Core.SetOptions(false);
    }

    public void EldersBlood()
    {
        while (!Bot.ShouldExit && Army.doForAll())
        {
            if (Bot.Inventory.FreeSlots <= 0)
                continue;
            Dailies.EldersBlood();
        }
    }
}
