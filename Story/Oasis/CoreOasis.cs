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
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
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

    private static CoreAOR AOR
    {
        get => _AOR ??= new CoreAOR();
        set => _AOR = value;
    }
    private static CoreAOR _AOR;

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
        CarcossaCabins();
        Whitetigerpoint();
        VermillionCliffs();
    }

    public void Whitetigerpoint()
    {
        if (Core.isCompletedBefore(10329))
            return;

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Astral Spirit", // UseableMonsters[0],
            "Byakko Cub", // UseableMonsters[1],
            "Rigel Stray", // UseableMonsters[2],
            "Lunar Haze", // UseableMonsters[3],
            "Byakko", // UseableMonsters[4]
        };
        #endregion Useable Monsters

        // 10320 | Stellar Dynamics
        if (!Story.QuestProgression(10320))
        {
            Core.HuntMonsterQuest(10320, ("whitetigerpoint", UseableMonsters[0], ClassType.Farm));
        }

        // 10321 | Hokey Horoscope
        if (!Story.QuestProgression(10321))
        {
            Story.MapItemQuest(10321, "whitetigerpoint", 14656);
            Story.MapItemQuest(10321, "whitetigerpoint", 14657, 2);
        }

        // 10322 | Tokaki
        if (!Story.QuestProgression(10322))
        {
            Core.HuntMonsterQuest(10322, ("whitetigerpoint", UseableMonsters[1], ClassType.Farm));
        }

        // 10323 | Ekie
        if (!Story.QuestProgression(10323))
        {
            Story.MapItemQuest(10323, "whitetigerpoint", 14658);
            Story.MapItemQuest(10323, "whitetigerpoint", 14659, 2);
        }

        // 10324 | Subaru
        if (!Story.QuestProgression(10324))
        {
            Core.HuntMonsterQuest(
                10324,
                ("whitetigerpoint", UseableMonsters[1], ClassType.Farm),
                ("whitetigerpoint", UseableMonsters[0], ClassType.Farm)
            );
        }

        // 10325 | Kagasuki
        if (!Story.QuestProgression(10325))
        {
            Core.HuntMonsterQuest(10325, ("whitetigerpoint", UseableMonsters[2], ClassType.Farm));
        }

        // 10326 | Amefuri
        if (!Story.QuestProgression(10326))
        {
            Core.HuntMonsterQuest(10326, ("whitetigerpoint", UseableMonsters[3], ClassType.Farm));
        }

        // 10327 | Tatara
        if (!Story.QuestProgression(10327))
        {
            Story.MapItemQuest(10327, "whitetigerpoint", 14660);
        }

        // 10328 | Toroki
        if (!Story.QuestProgression(10328))
        {
            Core.HuntMonsterQuest(
                10328,
                ("whitetigerpoint", UseableMonsters[3], ClassType.Farm),
                ("whitetigerpoint", UseableMonsters[2], ClassType.Farm)
            );
        }

        // 10329 | Komokuten
        if (!Story.QuestProgression(10329))
        {
            Core.HuntMonsterQuest(10329, ("whitetigerpoint", UseableMonsters[4], ClassType.Solo));
        }
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
                ("duatpalace", UseableMonsters[2], ClassType.Farm));
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

    public void CarcossaCabins()
    {
        if (Core.isCompletedBefore(10759))
            return;

        AOR.ForgeCitrinitas();
        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Citrinitas Elemental", // UseableMonsters[0],
            "Citrinitas Match", // UseableMonsters[1],
            "Evolved Lifeform", // UseableMonsters[2],
            "Doom Leech", // UseableMonsters[3],
            "Clementine", // UseableMonsters[4]
        };
        #endregion Useable Monsters

        // 10750 | Flickering Hyades
        if (!Story.QuestProgression(10750))
        {
            Story.MapItemQuest(10750, "carcossacabins", 15859);
            Core.HuntMonsterQuest(10750,
                ("carcossacabins", UseableMonsters[0], ClassType.Farm));
        }

        // 10751 | Red Dawn, Blue Starlight
        if (!Story.QuestProgression(10751))
        {
            Story.MapItemQuest(10751, "carcossacabins", new[] { 15860, 15861 });
            Core.HuntMonsterQuest(10751,
                ("carcossacabins", UseableMonsters[1], ClassType.Farm));
        }

        // 10752 | Shadow of Paradise
        if (!Story.QuestProgression(10752))
        {
            Story.MapItemQuest(10752, "carcossacabins", new[] { 15862, 15863 });
        }

        // 10753 | Bed of the Rhine
        if (!Story.QuestProgression(10753))
        {
            Core.HuntMonsterQuest(10753,
                ("carcossacabins", UseableMonsters[1], ClassType.Farm));
        }

        // 10754 | Liederkreis Lorelei
        if (!Story.QuestProgression(10754))
        {
            Story.MapItemQuest(10754, new[] { (15864, 1, "carcossacabins"), (15865, 3, "carcossacabins") });
        }

        // 10755 | Lacking in Blessings
        if (!Story.QuestProgression(10755))
        {
            Core.HuntMonsterQuest(10755,
                ("carcossacabins", UseableMonsters[2], ClassType.Solo));
        }

        // 10756 | Hate and Yearning
        if (!Story.QuestProgression(10756))
        {
            Story.MapItemQuest(10756, "carcossacabins", new[] { 15866, 15867 });
        }

        // 10757 | Dark Matter Radiation
        if (!Story.QuestProgression(10757))
        {
            Core.HuntMonsterQuest(10757,
                ("carcossacabins", UseableMonsters[3], ClassType.Solo));
        }


        // 10758 | Amnion Alchemy
        if (!Story.QuestProgression(10758))
        {
            Core.HuntMonsterQuest(10758,
                ("carcossacabins", UseableMonsters[2], ClassType.Solo),
                ("carcossacabins", UseableMonsters[3], ClassType.Solo));
        }


        // 10759 | Self-Infested
        if (!Story.QuestProgression(10759))
        {
            Core.HuntMonsterQuest(10759,
                ("carcossacabins", UseableMonsters[4], ClassType.Solo));
        }
    }

    public void VermillionCliffs()
    {
        if (Core.isCompletedBefore(10772))
            return;

        Whitetigerpoint();

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Star Sweet", // UseableMonsters[0],
            "Tejat Muse Moth", // UseableMonsters[1],
            "Dawn Knight", // UseableMonsters[2],
            "Vermillion Phoenix", // UseableMonsters[3],
            "Suzaku", // UseableMonsters[4]
        };
        #endregion Useable Monsters

        // 10765 | Tamahome
        if (!Story.QuestProgression(10765))
        {
            Core.EnsureAccept(10765);
            Core.GetMapItem(15885, 6, "vermillioncliffs");
            Core.KillMonster("vermillioncliffs", "r4", "Left", "*", "Sweet Konpeito", 8);
            Core.EnsureComplete(10765);

        }

        // 10766 | Tasuki
        if (!Story.QuestProgression(10766))
        {
            Story.MapItemQuest(10766, "vermillioncliffs", 15886, 6);
            Core.HuntMonsterQuest(10766,
                ("vermillioncliffs", UseableMonsters[1], ClassType.Farm));
        }

        // 10767 | Mitsutake
        if (!Story.QuestProgression(10767))
        {
            Core.HuntMonsterQuest(10767,
                ("vermillioncliffs", UseableMonsters[0], ClassType.Farm),
                ("vermillioncliffs", UseableMonsters[1], ClassType.Farm));
        }

        // 10768 | Nuriko
        if (!Story.QuestProgression(10768))
        {
            Story.MapItemQuest(10768, "vermillioncliffs", new[] { 15887, 15888 });
        }

        // 10769 | Chiriko
        if (!Story.QuestProgression(10769))
        {
            Core.HuntMonsterQuest(10769,
                ("vermillioncliffs", UseableMonsters[2], ClassType.Farm));
        }

        // 10770 | Chichiri
        if (!Story.QuestProgression(10770))
        {
            Core.HuntMonsterQuest(10770,
                ("vermillioncliffs", UseableMonsters[3], ClassType.Farm));
        }

        // 10771 | Hotohori
        if (!Story.QuestProgression(10771))
        {
            Core.HuntMonsterQuest(10771,
                ("vermillioncliffs", UseableMonsters[2], ClassType.Solo),
                ("vermillioncliffs", UseableMonsters[3], ClassType.Solo));
        }

        // 10772 | Zochoten
        if (!Story.QuestProgression(10772))
        {
            Core.HuntMonsterQuest(10772,
                ("vermillioncliffs", UseableMonsters[4], ClassType.Solo));
        }


    }
}