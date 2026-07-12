/*
name: Tug and Pull
description: Completes the 'Tug and Pull' quest [10791] for the selected reward(s), drops Void Soul.
tags: hollowborn, hollowborn nation, Tug and Pull, void soul
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
//cs_include Scripts/Hollowborn/HollowbornOblivionBlade.cs
//cs_include Scripts/Nation/Various/ShadowLegacyofNulgath.cs
//cs_include Scripts/Nation/AFDL/WillpowerExtraction.cs
//cs_include Scripts/Nation/Various/GoldenHanzoVoid.cs
//cs_include Scripts/Nation/AFDL/NulgathDemandsWork.cs
//cs_include Scripts/Nation/VHL/CoreVHL.cs
//cs_include Scripts/Good/BLOD/CoreBLOD.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Nation/Various/EnchantedNulgathNationHouse.cs
//cs_include Scripts/Hollowborn/CoreHolowborn.cs

using Skua.Core.Interfaces;
using Skua.Core.Options;

public class TugandPull
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
    private static MuddledPast MP
    {
        get => _MP ??= new MuddledPast();
        set => _MP = value;
    }
    private static MuddledPast _MP;
    private static InTheFiendsShadow ITFS
    {
        get => _ITFS ??= new InTheFiendsShadow();
        set => _ITFS = value;
    }
    private static InTheFiendsShadow _ITFS;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    private static NulgathDemandsWork NDW
    {
        get => _NDW ??= new NulgathDemandsWork();
        set => _NDW = value;
    }
    private static NulgathDemandsWork _NDW;

    private static CoreVHL CVHL
    {
        get => _CVHL ??= new CoreVHL();
        set => _CVHL = value;
    }
    private static CoreVHL _CVHL;
    private static JuggernautItemsofNulgath Jug
    {
        get => _Jug ??= new JuggernautItemsofNulgath();
        set => _Jug = value;
    }
    private static JuggernautItemsofNulgath _Jug;
    private static EnhancedNulgathNationHouse ennh
    {
        get => _ennh ??= new EnhancedNulgathNationHouse();
        set => _ennh = value;
    }
    private static EnhancedNulgathNationHouse _ennh;
    private static CoreHollowborn HB
    {
        get => _HB ??= new CoreHollowborn();
        set => _HB = value;
    }
    private static CoreHollowborn _HB;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        DoTugandPull();

        Core.SetOptions(false);
    }

    public void DoTugandPull(bool QuestOnly = false)
    {
        if (QuestOnly && Core.isCompletedBefore(10792))
        {
            Core.Logger("\"Tug and Pull\" Already complete");
            return;
        }

        if (!Core.isCompletedBefore(10791))
        {
            Core.Logger("Completing previous quest *once*");
            MP.DoMuddledPast(true);
        }

        // Accept requirements
        Jug.JuggItems(JuggernautItemsofNulgath.RewardsSelection.Nulgath_Armor);
        Jug.JuggItems(JuggernautItemsofNulgath.RewardsSelection.Battlefiend_Blade_of_Nulgath);
        GetNulgathHouseGuest();
        HB.HardcoreContract();

        Core.AddDrop(Rewards);

        if (QuestOnly)
        {
            Core.EnsureAccept(QuestID);

            ITFS.FiendsShadow(InTheFiendsShadow.Rewards.None, false, true, 100);
            NDW.NDWQuest(new[] { "Archfiend Essence Fragment" }, 4);
            CVHL.VHLChallenge(8, false);
            if (Core.CheckInventory(" The Mortal Coil"))
                Core.Logger("Missing \" The Mortal Coil\" From Nulgath, you must farm this yourself via an army then return to this script", messageBox: true, stopBot: true);
            else Core.EnsureComplete(QuestID);
            return;
        }

        while (!Bot.ShouldExit && !Core.CheckInventory(Core.InitializeWithRetries(() => Core.QuestRewards(QuestID))))
        {
            Core.EnsureAccept(QuestID);

            ITFS.FiendsShadow(InTheFiendsShadow.Rewards.None, false, true, 100);
            NDW.NDWQuest(new[] { "Archfiend Essence Fragment" }, 4);
            CVHL.VHLChallenge(8, false);
            if (Core.CheckInventory(" The Mortal Coil"))
                Core.Logger("Missing \" The Mortal Coil\" From Nulgath, you must farm this yourself via an army then return to this script", messageBox: true, stopBot: true);
            else Core.EnsureComplete(QuestID);
        }
    }

    private const int QuestID = 10792;
    string[] Rewards =
    {
        "Hollowborn Legacy of Nulgath",
        "Hollowborn Legacy of Nulgath Hood",
        "Hollowborn Cloak of Nulgath",
        "Hollowborn Spear of Nulgath",
        "Hollowborn Fiend Cloak of Nulgath",
        "Hollowborn Oblivion Blade Battle Pet"
    };

    public void GetNulgathHouseGuest()
    {
        if (Core.CheckInventory("Nulgath House Guest"))
        {
            Core.Logger($"All rewards already owned");
            return;
        }

        // Required to accept
        ennh.GetENNH();


        // Track only relevant rewards from quest 5661
        Core.AddDrop("Nulgath House Guest");

        // Loop until all desired rewards are in inventory
        while (!Bot.ShouldExit && !Core.CheckInventory("Nulgath House Guest"))
        {
            Core.EnsureAccept(5661);

            Nation.Supplies("Unidentified 4");
            Nation.FarmTaintedGem(1);
            Nation.FarmDarkCrystalShard(1);
            Nation.EssenceofNulgath(1);
            Nation.FarmGemofNulgath(1);

            Core.EnsureComplete(5661);

        }


    }
}
