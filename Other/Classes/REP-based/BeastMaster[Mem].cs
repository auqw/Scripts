/*
name: BeastMaster[Mem]
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class BeastMaster
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

        GetBM();

        Core.SetOptions(false);
    }

    public void GetBM(bool rankUpClass = true)
    {
        if (Core.CheckInventory("BeastMaster"))
            return;

        Adv.BuyItem("northpointe", 976, "BeastMaster", shopItemID: 16031);

        if (rankUpClass)
            Adv.RankUpClass("BeastMaster");
    }
}
