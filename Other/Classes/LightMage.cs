/*
name: LightMage
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class LightMage
{
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

        GetLM();

        Core.SetOptions(false);
    }

    public void GetLM(bool rankUpClass = true)
    {
        if (Core.CheckInventory("LightMage"))
            return;

        Core.BuyItem("celestialrealm", 1353, "Evolved LightCaster");
        Core.BuyItem("celestialrealm", 1613, "LightMage Class Token A");
        Core.BuyItem("celestialrealm", 1612, "LightMage", shopItemID: 5987);

        if (rankUpClass)
            Adv.RankUpClass("LightMage");
    }
}
