/*
name: Ebil Mega Mall Story
description: This will complete the Ebil Mega Mall story.
tags: story, quest, ebil mega mall, black friday, seasonal, yes ma'am, yes maam,ebilmegamall
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class EbilMegaMall
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        StoryLine();

        Core.SetOptions(false);
    }

    public void StoryLine()
    {
        if (Core.isCompletedBefore(10508) || !Core.isSeasonalMapActive("ebilmegamall"))
            return;

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Vending Machine", // UseableMonsters[0],
            "Mogugu", // UseableMonsters[1],
            "Deal Hunter", // UseableMonsters[2],
            "Scalper", // UseableMonsters[3],
            "Customer Support", // UseableMonsters[4],
            "Mogugudra", // UseableMonsters[5],
            "Black BOGOdrone Prime", // UseableMonsters[6]
        };
        #endregion Useable Monsters

        // 10500 | Vendebils
        if (!Story.QuestProgression(10500))
        {
            Core.HuntMonsterQuest(10500, ("ebilmegamall", UseableMonsters[0], ClassType.Farm));
        }

        // 10501 | Bad Batch
        if (!Story.QuestProgression(10501))
        {
            Core.HuntMonsterQuest(10501, ("ebilmegamall", UseableMonsters[1], ClassType.Farm));
        }

        // 10502 | Deal Camper
        if (!Story.QuestProgression(10502))
        {
            Core.HuntMonsterQuest(10502, ("ebilmegamall", UseableMonsters[2], ClassType.Farm));
        }

        // 10503 | Questing Quencher
        if (!Story.QuestProgression(10503))
        {
            Story.MapItemQuest(10503, "ebilmegamall", 15186, 6);
        }

        // 10504 | Free PR
        if (!Story.QuestProgression(10504))
        {
            Core.HuntMonsterQuest(
                10504,
                ("ebilmegamall", UseableMonsters[1], ClassType.Farm),
                ("ebilmegamall", UseableMonsters[2], ClassType.Farm)
            );
        }

        // 10505 | Artificial Demand
        if (!Story.QuestProgression(10505))
        {
            Core.HuntMonsterQuest(10505, ("ebilmegamall", UseableMonsters[3], ClassType.Farm));
        }

        // 10506 | Instant Euphoria
        if (!Story.QuestProgression(10506))
        {
            Core.EnsureAccept(10506);
            Core.KillMonster(
                "ebilmegamall",
                "r6",
                "Left",
                "*",
                Core.QuestRequirements<string>(10506)[0]
            );
            Core.EnsureComplete(10506);
        }

        // 10507 | Please Hold
        if (!Story.QuestProgression(10507))
        {
            Core.HuntMonsterQuest(10507, ("ebilmegamall", UseableMonsters[4], ClassType.Farm));
        }

        // 10508 | De-unioned
        if (!Story.QuestProgression(10508))
        {
            Core.HuntMonsterQuest(10508, ("ebilmegamall", UseableMonsters[5], ClassType.Solo));
        }
    }
}
