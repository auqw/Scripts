/*
name: KiyametGrove
description: Completes the quest chain in /kiyametgrove.
tags: kiyametgrove, story, quests, kiyametgrove
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class KiyametGrove
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
        if (Core.isCompletedBefore(10936))
            return;

        Story.PreLoad(this);

        string[] UseableMonsters =
        [
            "Apocalyptic Colossus", // UseableMonsters[0]
            "Apocalyptic Golem", // UseableMonsters[1]
            "Overcharged Colossus", // UseableMonsters[2]
        ];

        // 10935 | Arockalyptic
        Story.KillQuest(10935, "kiyametgrove", UseableMonsters[1]); // Apocalypse Shard x12

        // 10936 | A Colossal Task
        Story.KillQuest(10936, "kiyametgrove", UseableMonsters[0]); // Colossus Stone x1
    }
}
