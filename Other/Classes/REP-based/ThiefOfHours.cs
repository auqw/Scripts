/*
name: ThiefOfHours
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class ThiefOfHours
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetToH();

        Core.SetOptions(false);
    }

    public void GetToH(bool rankUpClass = true)
    {
        if (Core.CheckInventory("Thief of Hours"))
            return;

        Farm.ChronoSpanREP();

        Core.BuyItem("thespan", 439, "Thief of Hours");

        if (rankUpClass)
            Adv.RankUpClass("Thief of Hours");
    }
}
