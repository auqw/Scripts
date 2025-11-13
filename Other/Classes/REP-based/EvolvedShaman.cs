/*
name: EvolvedShaman
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class EvolvedShaman
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

        GetES();

        Core.SetOptions(false);
    }

    public void GetES(bool rankUpClass = true)
    {
        if (Core.CheckInventory("Evolved Shaman"))
            return;

        Farm.ArcangroveREP();

        Core.BuyItem("arcangrove", 214, "Evolved Shaman", shopItemID: 6396);

        if (rankUpClass)
            Adv.RankUpClass("Evolved Shaman");
    }
}
