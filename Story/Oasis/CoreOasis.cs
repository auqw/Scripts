/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
using Skua.Core.Interfaces;

public class CoreOasis
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

    private static Core13LoC LOC
    {
        get => _LOC ??= new Core13LoC();
        set => _LOC = value;
    }
    private static Core13LoC _LOC;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.RunCore();
    }

    public void DoAll()
    {
        DuatPalace();
        CrocRiver();
        CrulonWed();
        MeresankhChambers();
    }

    public void DuatPalace()
    {
        if (Core.isCompletedBefore(10747))
            return;

        LOC.Tibicenas();
        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Sphinx Guardian", // UseableMonsters[0],
            "Duat Scorpion", // UseableMonsters[1],
            "Royal Sandsea Guard", // UseableMonsters[2],
            "Guardian Hound", // UseableMonsters[3]
        };
        #endregion Useable Monsters

        // 10741 | Household Deathstalkers
        if (!Story.QuestProgression(10741))
        {
            Core.HuntMonsterQuest(10741,
                ("duatpalace", UseableMonsters[1], ClassType.Farm));
        }

        // 10742 | Vexation of the Sphinx
        if (!Story.QuestProgression(10742))
        {
            Core.HuntMonsterQuest(10742,
                ("duatpalace", UseableMonsters[0], ClassType.Farm));
        }

        // 10743 | In Search of Djehuty
        if (!Story.QuestProgression(10743))
        {
            Core.HuntMonsterQuest(10743,
                ("duatpalace", UseableMonsters[3], ClassType.Farm));
        }

        // 10744 | Lord of Faiyum
        if (!Story.QuestProgression(10744))
        {
            Core.HuntMonsterQuest(10744,
                ("crocriver", "Sobekemsaph ", ClassType.Solo));
        }

        // 10745 | The Closing of the Casket
        if (!Story.QuestProgression(10745))
        {
            Core.HuntMonsterQuest(10745,
                ("duatpalace", UseableMonsters[0], ClassType.Farm));
        }


        // 10746 | Words Sting
        if (!Story.QuestProgression(10746))
        {
            Core.HuntMonsterQuest(10746,
                ("duatpalace", UseableMonsters[1], ClassType.Farm));
        }


        // 10747 | Majestic Anubians
        if (!Story.QuestProgression(10747))
        {
            Core.HuntMonsterQuest(10747,
                ("duatpalace", UseableMonsters[3], ClassType.Farm));
        }


    }

    public void CrocRiver()
    {
        if (Core.isCompletedBefore(9539))
            return;

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Sobekemsaph", // UseableMonsters[0]
            "Loamy Lamia", // UseableMonsters[1]
            "Fluvial Lamia", // UseableMonsters[2]
            "Golmoth", // UseableMonsters[3]
            "Flaming Harpy" // UseableMonsters[4
        };
        #endregion Useable Monsters

        // 9537 | Remnants of Apophis
        if (!Story.QuestProgression(9537))
        {
            Core.HuntMonsterQuest(9537,
                ("djinnguard", UseableMonsters[1], ClassType.Solo),
                ("djinnguard", UseableMonsters[2], ClassType.Solo));
        }


        // 9538 | Follower of Faiyum
        if (!Story.QuestProgression(9538))
        {
            Core.HuntMonsterQuest(9538,
                ("djinnguard", UseableMonsters[3], ClassType.Solo),
                ("djinnguard", UseableMonsters[4], ClassType.Solo));
        }


        // 9539 | Lord of the Oasis
        if (!Story.QuestProgression(9539))
        {
            Core.HuntMonsterQuest(9539,
                ("crocriver", UseableMonsters[0], ClassType.Solo));
        }


    }

    public void CrulonWed()
    {
        if (Core.isCompletedBefore(9850))
            return;

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "King Almoravid", // UseableMonsters[0]
            "Jaan al Nair", // UseableMonsters[1]
            "Silver Elemental" // UseableMonsters[2]
        };
        #endregion Useable Monsters

        // 9848 | The Red Rival
        if (!Story.QuestProgression(9848))
        {
            Core.HuntMonsterQuest(9848,
                ("djinnguard", UseableMonsters[1], ClassType.Solo));
        }


        // 9849 | Pale Invocation
        if (!Story.QuestProgression(9849))
        {
            Core.HuntMonsterQuest(9849,
                ("towerofmirrors", UseableMonsters[2], ClassType.Solo));
        }


        // 9850 | Moon's Self-Reflection
        if (!Story.QuestProgression(9850))
        {
            Core.HuntMonsterQuest(9850,
                ("crulonwed", UseableMonsters[0], ClassType.Solo));
        }

    }

    public void MeresankhChambers()
    {
        if (Core.isCompletedBefore(10544) || Core.isCompletedBefore(10545))
            return;

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Queen Meresankh", // UseableMonsters[0]
        };
        #endregion Useable Monsters
        if (!Core.IsMember)
        {
            // 10544 | The Ninth Queen's Curse
            if (!Story.QuestProgression(10544))
            {
                Core.HuntMonsterQuest(10544,
                    ("MeresankhChambers", UseableMonsters[0], ClassType.Solo));
            }
        }
        else
        {
            // 10545 | The Ninth Queen's Curse (Legend)
            if (!Story.QuestProgression(10545))
            {
                Core.HuntMonsterQuest(10545,
                    ("MeresankhChambers", UseableMonsters[0], ClassType.Solo));
            }
        }

    }
}