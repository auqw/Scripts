/*
name: Death Knight Lord Post-update farming script
description: Death Knight Lord, post-update farming script, deathknight lord, golden deathknight lord, silver deathknight lord, dkl, member, class
tags: death knight lord, member, class
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs
//cs_include Scripts/Other/MergeShops/BonecastleTowerMerge.cs

using Skua.Core.Interfaces;

public class DeathKnightLord
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    public static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    public static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    public static CoreToD ToD { get => _ToD ??= new CoreToD(); set => _ToD = value; }
    private static CoreToD _ToD;
    public static BonecastleTowerMerge BCM { get => _BCM ??= new BonecastleTowerMerge(); set => _BCM = value; }
    private static BonecastleTowerMerge _BCM;
    public static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;


    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        DKL();

        Core.SetOptions(false);
    }

    public void DKL(bool rankUpClass = true)
    {
        // Check if DeathKnight Lord is already owned
        if (Core.CheckInventory("DeathKnight Lord"))
        {
            Core.Logger("DeathKnight Lord class is already owned");
            if (rankUpClass)
                Adv.RankUpClass("DeathKnight Lord");
            return;
        }

        // Member check
        if (!Core.IsMember)
        {
            Core.Logger("The DeathKnight Lord is a member-only item. You will not be able to obtain it otherwise.");
            return;
        }

        BCM.BuyAllMerge("DeathKnight Lord");

        if (rankUpClass)
            Adv.RankUpClass("DeathKnight Lord");

    }
}
