/*
name: Bard
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
using Skua.Core.Interfaces;

public class Bard
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
    private static Core13LoC LOC
    {
        get => _LOC ??= new Core13LoC();
        set => _LOC = value;
    }
    private static Core13LoC _LOC;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetBard();

        Core.SetOptions(false);
    }

    public void GetBard(bool rankUpClass = true)
    {
        if (Core.CheckInventory("Bard") || !Core.IsMember)
            return;

        LOC.Kimberly();
        Farm.MythsongREP(4);

        Core.BuyItem("mythsong", 186, "Bard");

        if (rankUpClass)
            Adv.RankUpClass("Bard");
    }
}
