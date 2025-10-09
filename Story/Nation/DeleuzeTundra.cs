/*
name: Deleuze Tundra Story
description: This will finish the deleuzetundra Story.
tags: story, quest, deleuzetundra
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
//cs_include Scripts/Story/Nation/VoidChasm.cs
using Skua.Core.Interfaces;

public class DeleuzeTundraStory
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }    private static CoreStory _Story;
    private static VoidChasm VC { get => _VC ??= new VoidChasm(); set => _VC = value; }
    private static VoidChasm _VC;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        DeleuzeTundra();

        Core.SetOptions(false);
    }

    public void DeleuzeTundra()
    {
        if (Core.isCompletedBefore(10031))
            return;

        VC.Storyline();

        Story.PreLoad(this);
        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
    "Empty Creature", // UseableMonsters[0],
	"Nation Outrider", // UseableMonsters[1],
	"Oblivion Magus", // UseableMonsters[2],
	"Oblivion's Herald", // UseableMonsters[3],
	"Blighted Zubami", // UseableMonsters[4]
};
        #endregion Useable Monsters

        // 10023 | Empty Eyes
        if (!Story.QuestProgression(10023))
        {
            Core.HuntMonsterQuest(10023,
                ("deleuzetundra", UseableMonsters[0], ClassType.Farm));
        }


        // 10024 | Infectious Sleep
        if (!Story.QuestProgression(10024))
        {
            Core.HuntMonsterQuest(10024,
                ("deleuzetundra", UseableMonsters[1], ClassType.Farm));
        }


        // 10025 | Cadaveric Spasm
        if (!Story.QuestProgression(10025))
        {
            Core.EnsureAccept(10025);
            Story.MapItemQuest(10025, "deleuzetundra", 14048, 5);
            Story.MapItemQuest(10025, "deleuzetundra", 14049);
        }


        // 10026 | Nihil Scholars
        if (!Story.QuestProgression(10026))
        {
            Core.HuntMonsterQuest(10026,
                ("deleuzetundra", UseableMonsters[2], ClassType.Farm));
        }


        // 10027 | Id and Superego
        if (!Story.QuestProgression(10027))
        {
            Story.MapItemQuest(10027, "deleuzetundra", new[] { 14050, 14051 });
        }


        // 10028 | Sleepflying
        if (!Story.QuestProgression(10028))
        {
            Story.MapItemQuest(10028, "deleuzetundra", 14052);
            Story.KillQuest(10028, "deleuzetundra", UseableMonsters[0]);
        }


        // 10029 | Battle-Weary
        if (!Story.QuestProgression(10029))
        {
            Story.MapItemQuest(10029, "deleuzetundra", 14053, 5);
            Story.KillQuest(10029, "deleuzetundra", UseableMonsters[1]);
        }


        // 10030 | Self-Destructive Spell
        if (!Story.QuestProgression(10030))
        {
            Story.MapItemQuest(10030, "deleuzetundra", 14054);
            Story.KillQuest(10030, "deleuzetundra", UseableMonsters[2]);
        }


        // 10031 | Allure of the Void
        if (!Story.QuestProgression(10031))
        {
            Core.HuntMonsterQuest(10031,
                ("deleuzetundra", UseableMonsters[3], ClassType.Solo));
        }
    }
}
