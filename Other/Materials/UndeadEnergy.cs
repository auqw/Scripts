/*
name: UndeadEnergy
description: Farms max Undead Energy
tags: UndeadEnergy, Undead Energy
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class UndeadEnergy
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        MaxUE();

        Core.SetOptions(false);
    }

    public void MaxUE(int quant = 3)
    {
        Farm.BattleUnderB("Undead Energy");
    }
}
