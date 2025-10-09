/*
name: Oblivion Tundra Story
description: This script will complete the storyline in /obliviontundra.
tags: obliviontundra, oblivion, tundra, nation, nulgath, story, the hushed
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Nation/DeleuzeTundra.cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
//cs_include Scripts/Story/Nation/VoidChasm.cs
using Skua.Core.Interfaces;

public class OblivionTundra
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static DeleuzeTundraStory DT { get => _DT ??= new DeleuzeTundraStory(); set => _DT = value; }
    private static DeleuzeTundraStory _DT;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Storyline();

        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(10045))
            return;

        DT.DeleuzeTundra();

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
    "Empty Creature", // UseableMonsters[0],
	"Hushed", // UseableMonsters[1],
	"Withered Archfiend", // UseableMonsters[2],
	"Oblivion Magus ", // UseableMonsters[3],
	"Ettin Fiend", // UseableMonsters[4],
	"Archfiend Oblivion", // UseableMonsters[5],
	"Infinity Pool", // UseableMonsters[6]
};
        #endregion Useable Monsters

        // 10036 | Indignant Stubbornness
        Story.MapItemQuest(10036, "obliviontundra", 14064);
        Story.KillQuest(10036, "obliviontundra", UseableMonsters[0]);


        // 10037 | Over the Edge
        Story.MapItemQuest(10037, new[] {
            (14065, 5, "obliviontundra"),
            (14066, 1, "obliviontundra")
            });


        // 10038 | Restless Pride
        if (!Story.QuestProgression(10038))
        {
            Core.HuntMonsterQuest(10038,
                ("obliviontundra", UseableMonsters[1], ClassType.Farm));
        }


        // 10039 | Greywater
        if (!Story.QuestProgression(10039))
        {
            Core.HuntMonsterQuest(10039,
                ("obliviontundra", UseableMonsters[1], ClassType.Farm),
                ("obliviontundra", UseableMonsters[0], ClassType.Farm));
        }


        // 10040 | A Fiendish Waste
        if (!Story.QuestProgression(10040))
        {
            Core.HuntMonsterQuest(10040,
                ("obliviontundra", UseableMonsters[2], ClassType.Solo));
        }


        // 10041 | Inevitable Departure
        Story.MapItemQuest(10041, new[] {
            (14067, 3, "obliviontundra"),
            (14068, 1, "obliviontundra")
            });


        // 10042 | Emptiness Embodied
        if (!Story.QuestProgression(10042))
        {
            Core.HuntMonsterQuest(10042,
                ("obliviontundra", UseableMonsters[3], ClassType.Farm));
        }


        // 10043 | Ice Ages Past
        if (!Story.QuestProgression(10043))
        {
            Core.HuntMonsterQuest(10043,
                ("obliviontundra", UseableMonsters[4], ClassType.Farm));
        }


        // 10044 | Stirring Darkness
        if (!Story.QuestProgression(10044))
        {
            Core.HuntMonsterQuest(10044,
                ("obliviontundra", UseableMonsters[3], ClassType.Farm),
                ("obliviontundra", UseableMonsters[4], ClassType.Farm));
        }


        // 10045 | Remember to Breathe
        if (!Story.QuestProgression(10045))
        {
            Core.HuntMonsterQuest(10045,
                ("obliviontundra", UseableMonsters[5], ClassType.Solo));
        }


    }
}
