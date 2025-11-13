/*
name: HorcEvader
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class HorcEvader
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

        GetHE();

        Core.SetOptions(false);
    }

    public void GetHE(bool rankUpClass = true)
    {
        if (Core.CheckInventory("Horc Evader"))
            return;

        Adv.BuyItem("bloodtusk", 308, "Horc Evader");

        if (rankUpClass)
            Adv.RankUpClass("Horc Evader");
    }
}
