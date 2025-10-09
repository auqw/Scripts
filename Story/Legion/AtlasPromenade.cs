/*
name: Atlas Promenade Story
description: This will finish General Rand's questline in /atlaspromenade.
tags: story, quest, legion,dage,atlas,promenade,atlas-promenade,general rand,rand,atlaspromenade
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
using Skua.Core.Interfaces;

public class AtlasPromenade
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }    private static CoreStory _Story;
    private static SevenCircles SCW { get => _SCW ??= new SevenCircles(); set => _SCW = value; }    private static SevenCircles _SCW;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Storyline();

        Core.SetOptions(false);
    }


    public void Storyline()
    {
        if (Core.isCompletedBefore(10113))
            return;

        SCW.Circles();
        SCW.CirclesWar();
        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
    "Twisted Warrior", // UseableMonsters[0],
	"Lethe Wraith", // UseableMonsters[1],
	"Atlas Knight", // UseableMonsters[2],
	"Atlas Light Magus", // UseableMonsters[3],
	"Wrath Guard", // UseableMonsters[4],
	"Usurper Lord Slaine", // UseableMonsters[5]
};
        #endregion Useable Monsters

        // 10105 | Legion Traditions
        if (!Story.QuestProgression(10105))
        {
            Core.HuntMonsterQuest(10105,
                ("atlaspromenade", UseableMonsters[0], ClassType.Farm));
        }


        // 10106 | Two-Faced Reflection
        Story.MapItemQuest(10106, "atlaspromenade", 14204);


        // 10107 | Past Disturbed
        if (!Story.QuestProgression(10107))
        {
            Core.HuntMonsterQuest(10107,
                ("atlaspromenade", UseableMonsters[1], ClassType.Farm));
        }


        // 10108 | Atlas Lies in Wait
        Story.MapItemQuest(10108, "atlaspromenade", 14205);


        // 10109 | Preparedness Next to Godliness
        if (!Story.QuestProgression(10109))
        {
            Core.EquipClass(ClassType.Solo);
            Story.MapItemQuest(10109, "atlaspromenade", 14206);
            Story.KillQuest(10109, "atlaspromenade", UseableMonsters[2]);
        }


        // 10110 | Golden Youth
        if (!Story.QuestProgression(10110))
        {
            Core.EquipClass(ClassType.Solo);
            Story.MapItemQuest(10110, "atlaspromenade", 14207);
            Story.KillQuest(10110, "atlaspromenade", UseableMonsters[3]);
        }


        // 10111 | Old Souls
        if (!Story.QuestProgression(10111))
        {
            Core.HuntMonsterQuest(10111,
                ("atlaspromenade", UseableMonsters[3], ClassType.Solo),
                ("atlaspromenade", UseableMonsters[2], ClassType.Solo));
        }


        // 10114 | Digging Too Deep
        Story.MapItemQuest(10114, "atlaspromenade", 14208);


        // 10112 | Dis-graces
        if (!Story.QuestProgression(10112))
        {
            Core.HuntMonsterQuest(10112,
                ("atlaspromenade", UseableMonsters[4], ClassType.Farm));
        }


        // 10113 | Lord of Scars
        if (!Story.QuestProgression(10113))
        {
            Core.HuntMonsterQuest(10113,
                ("atlaspromenade", UseableMonsters[5], ClassType.Solo));
        }



    }
}
