/*
name: GrenWogWarren
description: This script will complete the questline in grenwogwarren.
tags: grenstory, seasonal, grenwogwarren, easter
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class GrenWogWarren
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Storyline();
        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(10152))
            return;

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Grenwog", // UseableMonsters[0],
            "Sugar Grenwog", // UseableMonsters[1],
            "Draconic Grenwog", // UseableMonsters[2],
            "Jurassic Grenwog", // UseableMonsters[3],
            "Elixir Grenwog", // UseableMonsters[4],
            "Alpha Cabdury", // UseableMonsters[5]
        };
        #endregion Useable Monsters

        // 10148 | Classic Original
        if (!Story.QuestProgression(10148))
        {
            Core.HuntMonsterQuest(10148, ("grenwogwarren", UseableMonsters[0], ClassType.Solo));
        }

        // 10149 | Sweet Buckteeth
        if (!Story.QuestProgression(10149))
        {
            Core.HuntMonsterQuest(10149, ("grenwogwarren", UseableMonsters[1], ClassType.Solo));
        }

        // 10150 | Tastes Like Chicken
        if (!Story.QuestProgression(10150))
        {
            Core.HuntMonsterQuest(10150, ("grenwogwarren", UseableMonsters[2], ClassType.Solo));
        }

        // 10151 | Jurassic Instinct
        if (!Story.QuestProgression(10151))
        {
            Core.HuntMonsterQuest(10151, ("grenwogwarren", UseableMonsters[3], ClassType.Solo));
        }

        // 10152 | Rapid Cycling
        if (!Story.QuestProgression(10152))
        {
            Core.HuntMonsterQuest(10152, ("grenwogwarren", UseableMonsters[4], ClassType.Solo));
        }
    }
}
