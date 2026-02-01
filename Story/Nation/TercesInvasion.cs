/*
name: Terces Invasion
description: This will finish the Terces Invasion storyline.
tags: story, quest, tercesinvasion, nation,nulgath, terces, invasion, jadzia
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Nation/DeleuzeTundra.cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
//cs_include Scripts/Story/Nation/VoidChasm.cs
//cs_include Scripts/Story/Nation/OblivionTundra.cs
//cs_include Scripts/Story/Nation/tercesarchive.cs
using Skua.Core.Interfaces;

public class TercesInvasion
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static TercesArchive TA
    {
        get => _TA ??= new TercesArchive();
        set => _TA = value;
    }
    private static TercesArchive _TA;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        StoryLine();

        Core.SetOptions(false);
    }

    public void StoryLine()
    {
        if (Core.isCompletedBefore(10581))
            return;

        TA.StoryLine();
        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
    "Bloodthirsty Void", // UseableMonsters[0],
	"Vampiric Devourer", // UseableMonsters[1],
	"Void Cultist", // UseableMonsters[2],
	"Archfiend Casimir", // UseableMonsters[3],
	"Archfiend Vigneron", // UseableMonsters[4],
	"Rodeleros Soldier", // UseableMonsters[5],
	"Infinity Pool", // UseableMonsters[6],
	"Elemental Fiend", // UseableMonsters[7],
	"Archfiend Rodeleros", // UseableMonsters[8]
};
        #endregion Useable Monsters

        // 10572 | Walking Leeches
        if (!Story.QuestProgression(10572))
        {
            Core.HuntMonsterQuest(10572,
                ("tercesinvasion", UseableMonsters[0], ClassType.Farm));
        }


        // 10573 | Taste for Evil
        if (!Story.QuestProgression(10573))
        {
            Core.HuntMonsterQuest(10573,
                ("tercesinvasion", UseableMonsters[1], ClassType.Farm));
        }


        // 10574 | Infinite Knowledge
        if (!Story.QuestProgression(10574))
        {
            Core.HuntMonsterQuest(10574,
                ("tercesinvasion", UseableMonsters[2], ClassType.Farm));
        }


        // 10575 | Return to Oblivion
        if (!Story.QuestProgression(10575))
        {
            Story.MapItemQuest(10575, "tercesinvasion", 15432);
            Core.EquipClass(ClassType.Solo);
            Story.KillQuest(10575, "tercesinvasion", UseableMonsters[3]);
        }


        // 10576 | Blood Thinner
        if (!Story.QuestProgression(10576))
        {
            Story.MapItemQuest(10576, "tercesinvasion", 15433);
            Story.KillQuest(10576, "tercesinvasion", UseableMonsters[4]);
        }


        // 10577 | Fiendish Wind
        if (!Story.QuestProgression(10577))
        {
            Core.HuntMonsterQuest(10577,
                ("tercesinvasion", UseableMonsters[5], ClassType.Farm));
        }


        // 10578 | Nulgath's Contingency Plan
        if (!Story.QuestProgression(10578))
        {
            Story.MapItemQuest(10578, "tercesinvasion", new[] { 15434, 15435, 15436 });
        }


        // 10579 | Dead Air
        if (!Story.QuestProgression(10579))
        {
            Core.HuntMonsterQuest(10579,
                ("tercesinvasion", UseableMonsters[7], ClassType.Farm));
        }


        // 10580 | Billowing Hate
        if (!Story.QuestProgression(10580))
        {
            Core.HuntMonsterQuest(10580,
                ("tercesinvasion", UseableMonsters[5], ClassType.Farm),
                ("tercesinvasion", UseableMonsters[7], ClassType.Farm));
        }


        // 10581 | The Swordmaiden
        if (!Story.QuestProgression(10581))
        {
            Core.HuntMonsterQuest(10581,
                ("tercesinvasion", UseableMonsters[8], ClassType.Solo));
        }


    }



}