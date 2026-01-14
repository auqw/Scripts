/*
name: EternalInversionist
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class EternalInversionist
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
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
    private static CoreToD TOD
    {
        get => _TOD ??= new CoreToD();
        set => _TOD = value;
    }
    private static CoreToD _TOD;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetEI();

        Core.SetOptions(false);
    }

    public void GetEI(bool rankUpClass = true)
    {
        if (Core.CheckInventory(35602))
            return;

        TOD.FourthDimensionalPyramid();
        Adv.BuyItem("fourdpyramid", 1275, "Eternal Inversionist", shopItemID: 21138);
        if (rankUpClass)
        {
            Adv.GearStore(EnhAfter: true);
            Core.Equip("Eternal Inversionist");
            Adv.RankUpClass("Eternal Inversionist");
            Adv.GearStore(true, EnhAfter: true);
        }
    }
}
