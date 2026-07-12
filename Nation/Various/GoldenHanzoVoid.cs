/*
name: GoldenHanzoVoid
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Shops;

public class GoldenHanzoVoid
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetGHV();

        Core.SetOptions(false);
    }

    public void GetGHV()
    {
        if (Core.CheckInventory("Golden Hanzo Void"))
            return;

        Nation.ApprovalAndFavor(50, 0);
        Nation.FarmTaintedGem(45);
        Nation.TheAssistant("Dark Crystal Shard", 45);
        Nation.FarmDiamondofNulgath(75);
        Nation.FarmVoucher(false);

        // Buy GHV
        Adv.BuyItem("evilwarnul", 456, 27843, shopItemID: 2694);
        Bot.Wait.ForItemBuy();
    }
}
