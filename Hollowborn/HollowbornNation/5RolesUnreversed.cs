/*
name: Roles Unreversed
description: Completes the 'Roles Unreversed' quest [10793], drops Void Soul.
tags: hollowborn, hollowborn nation, Roles Unreversed, void soul
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
//cs_include Scripts/Hollowborn/HollowbornNation/3MuddledPast.cs
//cs_include Scripts/Hollowborn/HollowbornNation/4TugandPull.cs
//cs_include Scripts/Hollowborn/HollowbornOblivionBlade.cs
//cs_include Scripts/Nation/Various/ShadowLegacyofNulgath.cs
//cs_include Scripts/Nation/AFDL/WillpowerExtraction.cs
//cs_include Scripts/Nation/Various/GoldenHanzoVoid.cs
//cs_include Scripts/Nation/AFDL/NulgathDemandsWork.cs
//cs_include Scripts/Nation/VHL/CoreVHL.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class RolesUnreversed
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static HollowSoul HSoul
    {
        get => _HSoul ??= new HollowSoul();
        set => _HSoul = value;
    }
    private static HollowSoul _HSoul;
    private static TugandPull TaP
    {
        get => _TaP ??= new TugandPull();
        set => _TaP = value;
    }
    private static TugandPull _TaP;
    private static HollowbornOblivionBlade HOB
    {
        get => _HOB ??= new HollowbornOblivionBlade();
        set => _HOB = value;
    }
    private static HollowbornOblivionBlade _HOB;

    private static NulgathDemandsWork NDW
    {
        get => _NDW ??= new NulgathDemandsWork();
        set => _NDW = value;
    }
    private static NulgathDemandsWork _NDW;
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
    private static CoreHollowborn HB
    {
        get => _HB ??= new CoreHollowborn();
        set => _HB = value;
    }
    private static CoreHollowborn _HB;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "RolesUnreversed";
    public List<IOption> Options = new()
    {
        CoreBots.Instance.SkipOptions,
        new Option<Rewards>(
            "RewardSelect",
            "Choose Your Reward",
            "Which selectable reward to farm from 'Roles Unreversed'. Choose All to farm every selectable reward.",
            Rewards.All
        ),
    };
    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        DoRolesUnreversed();

        Core.SetOptions(false);
    }

    public void DoRolesUnreversed(Rewards reward = Rewards.All, bool QuestOnly = false)
    {
        if (!Core.isCompletedBefore(QuestID))
        {
            Core.Logger("Completing previous quest *once*");
            TaP.DoTugandPull(QuestOnly: true);
        }

        string[] chosenReward = [];
        if (!QuestOnly)
            chosenReward = reward == Rewards.All
                ? SelectableRewards
                : new[] { reward.ToString().Replace('_', ' ') };

        if (!QuestOnly && Core.CheckInventory(chosenReward))
            return;

        if (QuestOnly && Core.isCompletedBefore(QuestID))
            return;

        HB.HardcoreContract();
        Core.AddDrop(Core.QuestRewards(QuestID));

        if (QuestOnly)
        {
            /*
            Unidentified 36 x5  
            Fresh Soul x50
            */
            HB.FreshSouls(5, 50);
            return;
        }

        while (!Bot.ShouldExit && !Core.CheckInventory(Core.QuestRewards(QuestID)))
        {
            Core.EnsureAccept(QuestID);

            /*
            Unidentified 36 x5  
            Fresh Soul x50
            */
            HB.FreshSouls(5, 50);

            Core.EnsureCompleteChoose(QuestID, chosenReward);
        }
    }

    private const int QuestID = 10793;
    public enum Rewards
    {
        All,
        Hollowborn_Hood_of_Nulgath,
        Hollowborn_Legacy_of_Nulgath_Horns,
        Hollowborn_Skull_of_Nulgath,
        Dual_Hollowborn_Spear_of_Nulgath,
        Dual_Hollowborn_Phoenix_Blade_of_Nulgath,
        Dual_Hollowborn_Overfiend_Blade_of_Nulgath,
        Dual_Hollowborn_DragonBlade_of_Nulgath,
        Dual_Hollowborn_Void_Sword,
        Dual_Hollowborn_Soulreaper_of_Nulgath,
        Hollowborn_Hands_of_Nulgath,
        Hollowborn_Mini_Nulgath_Battle_Pet,
        Hollowborn_Nulgath_Larvae,
        Hollowborn_Void_Orb,
        Hollowborn_Shadow_Morph_of_the_Oversoul,
        None
    }
    private readonly string[] SelectableRewards =
{
    "Hollowborn Hood of Nulgath",
    "Hollowborn Legacy of Nulgath Horns",
    "Hollowborn Skull of Nulgath",
    "Dual Hollowborn Spear of Nulgath",
    "Dual Hollowborn Phoenix Blade of Nulgath",
    "Dual Hollowborn Overfiend Blade of Nulgath",
    "Dual Hollowborn DragonBlade of Nulgath",
    "Dual Hollowborn Void Sword",
    "Dual Hollowborn Soulreaper of Nulgath",
    "Hollowborn Hands of Nulgath",
    "Hollowborn Mini-Nulgath Battle Pet",
    "Hollowborn Nulgath Larvae",
    "Hollowborn Void Orb",
    "Hollowborn Shadow Morph of the Oversoul",
};
}
