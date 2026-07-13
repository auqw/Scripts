/*
name: HollowbornOrbQuests
description: Completes the Quests from the Hollowborn Orb - with a choosable option
tags: hollowborn, hollowborn nation, hollowborn orb, hexed void, blood for blood, frontline fiend, empty void
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
//cs_include Scripts/Hollowborn/HollowbornNation/5RolesUnreversed.cs
//cs_include Scripts/Hollowborn/HollowbornOblivionBlade.cs
//cs_include Scripts/Nation/Various/ShadowLegacyofNulgath.cs
//cs_include Scripts/Nation/AFDL/WillpowerExtraction.cs
//cs_include Scripts/Nation/Various/GoldenHanzoVoid.cs
//cs_include Scripts/Nation/AFDL/NulgathDemandsWork.cs
//cs_include Scripts/Nation/VHL/CoreVHL.cs

using Skua.Core.Interfaces;
using Skua.Core.Options;

public class HollowbornOrbQuests
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
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
    private static RolesUnreversed RU
    {
        get => _RU ??= new RolesUnreversed();
        set => _RU = value;
    }
    private static RolesUnreversed _RU;
    private static InTheFiendsShadow ITFS
    {
        get => _ITFS ??= new InTheFiendsShadow();
        set => _ITFS = value;
    }
    private static InTheFiendsShadow _ITFS;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "HollowbornOrbQuests";
    public List<IOption> Options =
    [
        CoreBots.Instance.SkipOptions,
        new Option<Rewards>(
            "RewardSelect",
            "Choose Your Quest",
            "Which quest to complete from the Hollowborn Orb. Choose All to complete every quest.",
            Rewards.All
        ),
    ];

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        DoThing(Bot.Config!.Get<Rewards>("RewardSelect"));

        Core.SetOptions(false);
    }

    public void DoThing(Rewards reward = Rewards.All)
    {
        Rewards[] chosen = reward == Rewards.All
            ? [.. QuestIDs.Keys]
            : new[] { reward };

        foreach (Rewards selectedReward in chosen)
        {
            int questId = QuestIDs[selectedReward];

            string[]? rewards = Core.InitializeWithRetries(() => Core.QuestRewards(questId));

            if (rewards != null)
                Core.AddDrop(rewards);

            DoQuest(questId, selectedReward, rewards ?? []);
        }
    }

    private void DoQuest(int questId, Rewards reward, string[] rewards)
    {

        if (!Core.CheckInventory("Hollowborn Void Orb"))
        {
            RU.DoRolesUnreversed(RolesUnreversed.Rewards.Hollowborn_Void_Orb);
        }

        while (!Bot.ShouldExit && !Core.CheckInventory(rewards))
        {
            Core.EnsureAccept(questId);

            // Blood Gem of the Archfiend X10
            Nation.FarmBloodGem(10);

            // Unidentified 13 X1
            Nation.FarmUni13(1);

            // Unidentified for this quest x1
            Nation.TheAssistant(UniforQuest(questId));

            // Hollowborn Void of Nulgath
            ITFS.FiendsShadow(InTheFiendsShadow.Rewards.Hollowborn_Void_of_Nulgath, quant: 1);

            // There is no Myself x1 (Empty Void only)
            if (reward == Rewards.Empty_Void)
            {
                // Fake Complete 'Defeat the 12 Lords of Chaos!' to access map.
                Bot.Quests.UpdateQuest(3879);

                while (!Bot.ShouldExit && !Core.CheckInventory(33306))
                    Core.KillMonster("chaoslord", "r2", "Left", "*");
            }

            Core.EnsureComplete(questId);
        }
    }

    static string? UniforQuest(int questId)
    {
        return questId switch
        {
            10794 => Nation.Uni(22),
            10795 => Nation.Uni(23),
            10796 => Nation.Uni(24),
            10798 => Nation.Uni(25),
            _ => null,
        };
    }

    private static readonly Dictionary<Rewards, int> QuestIDs = new()
    {
        { Rewards.Hexed_Void, 10794 },
        { Rewards.Blood_for_Blood, 10795 },
        { Rewards.Frontline_Fiend, 10796 },
        { Rewards.Empty_Void, 10798 },
    };

    public enum Rewards
    {
        All,
        Hexed_Void,
        Blood_for_Blood,
        Frontline_Fiend,
        Empty_Void,
        None
    }
}