/*
name: Castle Party Story
description: This will finish the storyline of Castle Party.
tags: castle party,castleparty,seasonal,aqwbirthday,tara,story,gifts
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class CastleParty
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Storyline();

        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(10437))
            return;

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
    "Lost Giftbox", // UseableMonsters[0],
	"Treasure Chest", // UseableMonsters[1],
	"Nulgath's Gift", // UseableMonsters[2],
	"Kathool's Gift", // UseableMonsters[3],
	"Legion Partycrasher", // UseableMonsters[4],
	"Noxus' Gift", // UseableMonsters[5],
	"Sally's Gift", // UseableMonsters[6],
	"Drakath's Gift", // UseableMonsters[7]
};
        #endregion Useable Monsters

        // 10429 | Tara and Celestia's Gifts
        if (!Story.QuestProgression(10429))
        {
            Core.HuntMonsterQuest(10429,
                ("castleparty", UseableMonsters[0], ClassType.Farm));
        }


        // 10430 | Heroes' Gifts
        if (!Story.QuestProgression(10430))
        {
            Core.HuntMonsterQuest(10430,
                ("castleparty", UseableMonsters[1], ClassType.Farm));
        }


        // 10431 | Lorekeeper Gifts
        if (!Story.QuestProgression(10431))
        {
            Core.HuntMonsterQuest(10431,
                ("castleparty", UseableMonsters[0], ClassType.Farm),
                ("castleparty", UseableMonsters[1], ClassType.Farm));
        }


        // 10432 | Legion's Gift
        if (!Story.QuestProgression(10432))
        {
            Core.HuntMonsterQuest(10432,
                ("castleparty", UseableMonsters[4], ClassType.Farm));
        }


        // 10433 | Nation's Gift
        if (!Story.QuestProgression(10433))
        {
            Core.HuntMonsterQuest(10433,
                ("castleparty", UseableMonsters[2], ClassType.Solo));
        }


        // 10434 | Kathool's Gift
        if (!Story.QuestProgression(10434))
        {
            Core.HuntMonsterQuest(10434,
                ("castleparty", UseableMonsters[3], ClassType.Solo));
        }


        // 10435 | Noxus' Gift
        if (!Story.QuestProgression(10435))
        {
            Core.HuntMonsterQuest(10435,
                ("castleparty", UseableMonsters[5], ClassType.Solo));
        }


        // 10436 | Sally's Gift
        if (!Story.QuestProgression(10436))
        {
            Core.HuntMonsterQuest(10436,
                ("castleparty", UseableMonsters[6], ClassType.Solo));
        }


        // 10437 | Drakath's Gift
        if (!Story.QuestProgression(10437))
        {
            Core.HuntMonsterQuest(10437,
                ("castleparty", UseableMonsters[7], ClassType.Solo));
        }


    }
}
