/*
name: FiendVoid
description: This will finish the FiendVoid quest.
tags: story, quest, FiendVoid, nation
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Nation/DeleuzeTundra.cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
//cs_include Scripts/Story/Nation/VoidChasm.cs
//cs_include Scripts/Story/Nation/OblivionTundra.cs
//cs_include Scripts/Story/Nation/tercesarchive.cs
using Skua.Core.Interfaces;

public class FiendVoid
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

        FiendVoid_Questline();

        Core.SetOptions(false);
    }

    public void FiendVoid_Questline()
    {
        if (Core.isCompletedBefore(10569))
            return;

        TA.StoryLine();

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
    "Blood Fiend", // UseableMonsters[0],
	"The Hushed", // UseableMonsters[1],
	"Blood Coccoon", // UseableMonsters[2],
	"Void Fang", // UseableMonsters[3],
	"Arachnid Seeker", // UseableMonsters[4],
	"Crystalized Coccoon", // UseableMonsters[5],
	"Archfiend Casimir", // UseableMonsters[6]
};
        #endregion Useable Monsters

        // 10560 | Hemophilia
        if (!Story.QuestProgression(10560))
        {
            Core.HuntMonsterQuest(10560,
                ("fiendvoid", UseableMonsters[0], ClassType.Farm));
        }


        // 10561 | A Dash of Hope
        if (!Story.QuestProgression(10561))
        {
            Core.HuntMonsterQuest(10561,
                ("fiendvoid", UseableMonsters[1], ClassType.Farm));
        }


        // 10562 | Blood Cultivation
        if (!Story.QuestProgression(10562))
        {
            Story.MapItemQuest(10562, "fiendvoid", 15413);
            Story.MapItemQuest(10562, "fiendvoid", 15415);
        }


        // 10563 | Forever Fall
        if (!Story.QuestProgression(10563))
        {
            Core.EnsureAccept(10563);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("fiendvoid", "The Hushed", "Acrid Smoke", 3);
            Core.HuntMonster("fiendvoid", "Blood Fiend", "Ball of Veins", 6);
            Core.EnsureComplete(10563);
        }


        // 10564 | Hydrops Fetalis
        if (!Story.QuestProgression(10564))
        {
            Core.HuntMonsterQuest(10564,
                ("fiendvoid", UseableMonsters[2], ClassType.Solo));
        }


        // 10565 | Skimmed Off the Top
        if (!Story.QuestProgression(10565))
        {
            Core.HuntMonsterQuest(10565,
                ("fiendvoid", UseableMonsters[3], ClassType.Farm));
        }


        // 10566 | Sleeping Soldier
        if (!Story.QuestProgression(10566))
        {
            if (!Story.QuestProgression(10566))
            {
                Core.EnsureAccept(10566);
                Core.GetMapItem(15414, 1, "fiendvoid");
                Core.HuntMonster("fiendvoid", "Arachnid Seeker", "Crystalized Pupa", 9);
                Core.EnsureComplete(10566);
            }
        }


        // 10567 | Espadachina
        if (!Story.QuestProgression(10567))
        {
            Core.HuntMonsterQuest(10567,
                ("fiendvoid", UseableMonsters[5], ClassType.Solo));
        }


        // 10568 | Ingrained in Flesh
        if (!Story.QuestProgression(10568))
        {
            Core.EnsureAccept(10568);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("fiendvoid", "Void Fang", "Mutated Scales", 2);
            Core.HuntMonster("fiendvoid", "Arachnid Seeker", "Seeker Legs", 40);
            Core.EnsureComplete(10568);
        }


        // 10569 | I Know, Therefore I Suffer
        if (!Story.QuestProgression(10569))
        {
            Core.HuntMonsterQuest(10569,
                ("fiendvoid", UseableMonsters[6], ClassType.Solo));
        }

    }
}
