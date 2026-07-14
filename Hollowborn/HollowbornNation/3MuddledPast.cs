/*
name: Muddled Past
description: Completes the 'MuddledPast' quest [10789] for the selected reward(s), drops Void Soul.
tags: hollowborn, hollowborn nation, Muddled Past, void soul
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
//cs_include Scripts/Hollowborn/Materials/HollowSoul.cs
//cs_include Scripts/Hollowborn/HollowbornNation/1InTheFiendsShadow.cs
//cs_include Scripts/Hollowborn/HollowbornNation/2FiendsPurgatory.cs
//cs_include Scripts/Hollowborn/HollowbornOblivionBlade.cs
//cs_include Scripts/Nation/Various/ShadowLegacyofNulgath.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class MuddledPast
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static CoreHollowborn HB
    {
        get => _HB ??= new CoreHollowborn();
        set => _HB = value;
    }
    private static CoreHollowborn _HB;
    private static HollowSoul HSoul
    {
        get => _HSoul ??= new HollowSoul();
        set => _HSoul = value;
    }
    private static HollowSoul _HSoul;
    private static FiendsPurgatory FP
    {
        get => _FP ??= new FiendsPurgatory();
        set => _FP = value;
    }
    private static FiendsPurgatory _FP;
    private static HollowbornOblivionBlade HOB
    {
        get => _HOB ??= new HollowbornOblivionBlade();
        set => _HOB = value;
    }
    private static HollowbornOblivionBlade _HOB;
    private static ShadowLegacyofNulgath SLoN
    {
        get => _SLoN ??= new ShadowLegacyofNulgath();
        set => _SLoN = value;
    }
    private static ShadowLegacyofNulgath _SLoN;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static InTheFiendsShadow ITFS
    {
        get => _ITFS ??= new InTheFiendsShadow();
        set => _ITFS = value;
    }
    private static InTheFiendsShadow _ITFS;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        DoMuddledPast();

        Core.SetOptions(false);
    }

    public void DoMuddledPast(bool QuestOnly = false)
    {
        if (QuestOnly && Core.isCompletedBefore(QuestID))
        {
            Core.Logger("\"Muddle Past\" Already complete");
            return;
        }

        if (!Core.isCompletedBefore(QuestID))
        {
            Core.Logger("Completing previous quest *once*");
            FP.Purgatory(FiendsPurgatory.Rewards.None, true);
        }

        //Get Quest Requirement
        HOB.GetBlade();
        HB.HardcoreContract();

        Bot.Drops.Add(Core.QuestRewards(QuestID));

        if (QuestOnly)
        {
            Core.EnsureAccept(QuestID);

            ITFS.FiendsShadow(InTheFiendsShadow.Rewards.None, false, true, 50);
            Adv.BuyItem("tercessuinotlim", 68, "Fiend Cloak of Nulgath");
            SLoN.GetSLoN();
            HSoul.GetYaSoulsHeeeere(50);
            Nation.FarmTaintedGem(350);
            Nation.FarmDarkCrystalShard(150);
            Nation.FarmDiamondofNulgath(700);
            Nation.FarmGemofNulgath(150);

            Core.EnsureComplete(QuestID);
            return;
        }

        while (!Bot.ShouldExit && !Core.CheckInventory(Core.InitializeWithRetries(() => Core.QuestRewards(QuestID))))
        {
            Core.EnsureAccept(QuestID);

            ITFS.FiendsShadow(InTheFiendsShadow.Rewards.None, false, true, 50);
            Adv.BuyItem("tercessuinotlim", 68, "Fiend Cloak of Nulgath");
            SLoN.GetSLoN();
            HSoul.GetYaSoulsHeeeere(50);
            Nation.FarmTaintedGem(350);
            Nation.FarmDarkCrystalShard(150);
            Nation.FarmDiamondofNulgath(700);
            Nation.FarmGemofNulgath(150);

            Core.EnsureComplete(QuestID);
        }
    }

    private const int QuestID = 10791;

}
