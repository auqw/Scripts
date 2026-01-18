/*
name: BloodTitan[Mem]
description: farms Bt then gets and ranks the blood titan class if you'r a member
tags: blood titan, blood titan tokens, member, class
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Other/MergeShops/BloodTitanMerge[mem].cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/TitanAttack.cs
using Skua.Core.Interfaces;

public class BloodTitan
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static BloodTitanMerge BTM
    {
        get => _BTM ??= new BloodTitanMerge();
        set => _BTM = value;
    }
    private static BloodTitanMerge _BTM;
    private static TitanAttackStory Tas
    {
        get => _Tas ??= new TitanAttackStory();
        set => _Tas = value;
    }
    private static TitanAttackStory _Tas;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Getclass();

        Core.SetOptions(false);
    }

    public void Getclass(bool rankUpClass = true)
    {
        if (Core.CheckInventory(16641, toInv: false))
            return;

        Tas.DoAll();
        BTM.BuyAllMerge("Blood Titan");

        if (rankUpClass)
            Adv.RankUpClass("Blood Titan");
    }
}
