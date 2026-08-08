/*
name: Swindle Bulk
description: Farms Tainted Gem's until max stack using Swindle Bulk
tags: taintedgem, tainted, gem, swindle, bulk, nation, farm
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
using Skua.Core.Interfaces;

public class SwindleBulk
{
    public CoreBots Core => CoreBots.Instance;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Nation.SwindleBulk();

        Core.SetOptions(false);
    }
}
