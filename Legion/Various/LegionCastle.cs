/*
name: LegionCastle
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Legion/CoreLegion.cs

using Skua.Core.Interfaces;

public class GetLegionCastle
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreDailies Daily
    {
        get => _Daily ??= new CoreDailies();
        set => _Daily = value;
    }
    private static CoreDailies _Daily;
    private static CoreLegion Legion
    {
        get => _Legion ??= new CoreLegion();
        set => _Legion = value;
    }
    private static CoreLegion _Legion;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        AllYourBaseAreBelongToUs();

        Core.SetOptions(false);
    }

    public void AllYourBaseAreBelongToUs()
    {
        if (Core.CheckInventory("Legion Castle"))
            return;

        Legion.FarmLegionToken(25000);
        if (!Core.CheckInventory("Shadow Shroud", 15))
            Daily.ShadowShroud();

        if (Core.CheckInventory("Shadow Shroud", 15))
            Core.BuyItem("Underworld", 238, "Legion Castle");
    }
}
