/*
name: LegendaryElementalWarrior[mem]
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/Glacera.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
using Skua.Core.Interfaces;

public class LegendaryElementalWarrior
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
    private static GlaceraStory GlaceraStory
    {
        get => _GlaceraStory ??= new GlaceraStory();
        set => _GlaceraStory = value;
    }
    private static GlaceraStory _GlaceraStory;
    private static Core13LoC LOC
    {
        get => _LOC ??= new Core13LoC();
        set => _LOC = value;
    }
    private static Core13LoC _LOC;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetLEW();

        Core.SetOptions(false);
    }

    public void GetLEW(bool rankUpClass = true)
    {
        if (Core.CheckInventory("Legendary Elemental Warrior"))
            return;
        if (!Core.IsMember)
        {
            Core.Logger("Class requires member to buy without ACs.");
            return;
        }

        //Required Stories
        LOC.Lionfang();
        GlaceraStory.DoAll();

        Adv.BuyItem("Curio", 807, 22190);
        Adv.BuyItem("Curio", 807, 22191);
        Adv.BuyItem("Curio", 807, 22192);
        Adv.BuyItem("Curio", 807, 22193);
        Adv.BuyItem("Curio", 807, 22194);
        Adv.BuyItem("Curio", 807, 22187);
        Adv.BuyItem("Curio", 807, 22188);
        Adv.BuyItem("Curio", 807, 22189);

        Core.BuyItem("Curio", 809, "Legendary Elemental Warrior", shopItemID: 2412);

        if (rankUpClass)
            Adv.RankUpClass("Legendary Elemental Warrior");
    }
}
