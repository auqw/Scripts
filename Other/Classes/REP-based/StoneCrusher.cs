/*
name: StoneCrusher
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/BrightOak.cs
using Skua.Core.Interfaces;

public class StoneCrusher
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
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
    private static BrightOak Oak
    {
        get => _Oak ??= new BrightOak();
        set => _Oak = value;
    }
    private static BrightOak _Oak;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetSC();

        Core.SetOptions(false);
    }

    public void GetSC(bool rankUpClass = true)
    {
        if (Core.CheckInventory("StoneCrusher"))
        {
            if (rankUpClass)
                Adv.RankUpClass("StoneCrusher");
            return;
        }

        LOC.Kimberly();
        Oak.doall();
        Farm.MythsongREP();
        Adv.BuyItem("Gaiazor", 1210, 33394, shopItemID: 4222);

        if (rankUpClass)
            Adv.RankUpClass("StoneCrusher");
    }
}
