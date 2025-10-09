/*
name: Pirate Alliance Story
description: Completes the storyline in /piratealliance
tags: pirate alliance,alliance,tlapd,piratealliance, story, pirate,captain rubharb
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/PirateHunt.cs
using Skua.Core.Interfaces;

public class PirateAlliance
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static PirateHuntStory PH { get => _PH ??= new PirateHuntStory(); set => _PH = value; }
    private static PirateHuntStory _PH;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Storyline();

        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(10409))
            return;

        PH.PirateHuntSaga();
        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
    "PunkStream Crew", // UseableMonsters[0],
	"Thalassarath Pirate", // UseableMonsters[1],
	"S.S. Phantom Pirate", // UseableMonsters[2],
	"Captain Wary", // UseableMonsters[3],
	"Captain Bloch", // UseableMonsters[4],
	"Captain Squalus", // UseableMonsters[5],
	"Phantom Jaws", // UseableMonsters[6]
};
        #endregion Useable Monsters

        // 10400 | A Pirate's Stake
        Story.MapItemQuest(10400, "piratealliance", 14926);
        Story.KillQuest(10400, "piratealliance", UseableMonsters[0]);



        // 10401 | Thalassalings
        if (!Story.QuestProgression(10401))
        {
            Core.HuntMonsterQuest(10401,
                ("piratealliance", UseableMonsters[1], ClassType.Farm));
        }


        // 10402 | No Tales
        if (!Story.QuestProgression(10402))
        {
            Core.HuntMonsterQuest(10402,
                ("piratealliance", UseableMonsters[2], ClassType.Farm));
        }


        // 10403 | Live on Punt
        if (!Story.QuestProgression(10403))
        {
            Core.HuntMonsterQuest(10403,
                ("piratealliance", UseableMonsters[3], ClassType.Solo));
        }


        // 10404 | A Growing Crew
        if (!Story.QuestProgression(10404))
        {
            Core.HuntMonsterQuest(10404,
                ("piratealliance", UseableMonsters[1], ClassType.Farm));
        }

        // 10406 | Bloch-Head
        if (!Story.QuestProgression(10406))
        {
            Core.HuntMonsterQuest(10406,
                ("piratealliance", UseableMonsters[4], ClassType.Solo));
        }


        // 10405 | A Right and Salty Death
        if (!Story.QuestProgression(10405))
        {
            Core.HuntMonsterQuest(10405,
                ("piratealliance", UseableMonsters[2], ClassType.Farm));
        }


        // 10407 | Slip 'n' Slide
        Story.MapItemQuest(10407, "piratealliance", 14927, 5);


        // 10408 | Red Sea Spurdog
        if (!Story.QuestProgression(10408))
        {
            Core.HuntMonsterQuest(10408,
                ("piratealliance", UseableMonsters[5], ClassType.Solo));
        }


        // 10409 | Blood Bloom
        if (!Story.QuestProgression(10409))
        {
            Core.HuntMonsterQuest(10409,
                ("piratealliance", UseableMonsters[6], ClassType.Solo));
        }


    }
}
