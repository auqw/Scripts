/*
name: Deadlyseas
description: Completes the quest chain in /deadlyseas.
tags: deadlyseas, story, quests, deadlyseas
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class Deadlyseas
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
        if (Core.isCompletedBefore(10866))
            return;

        Story.PreLoad(this);

        string[] UseableMonsters =
        [
            "Isthmian Eel", // UseableMonsters[0]
            "Isthmian Guard", // UseableMonsters[1]
            "Isthmian Leviathan", // UseableMonsters[2]
            "Mermaid Hunter", // UseableMonsters[3]
            "PunkStream Crew", // UseableMonsters[4]
        ];

        // 10857 | Drama Alert
        Story.KillQuest(10857, "deadlyseas", UseableMonsters[4]); // Punkstream Doubloon x12

        // 10858 | Poaching Karma
        Story.KillQuest(10858, "deadlyseas", UseableMonsters[3]); // Desiccated Mermaid Fin x6

        // 10859 | Interested Sponsors
        Story.KillQuest(
            10859,
            "deadlyseas",
            new[]
            {
                UseableMonsters[4], // Punkstream Token x3
                UseableMonsters[3] // OMERga 3 Oil x3
            }
        );

        // 10860 | Hoist the Anchor
        Story.MapItemQuest(10860, "deadlyseas", [16189, 16190, 16191]);

        // 10861 | A Bad Eeling
        Story.MapItemQuest(10861, "deadlyseas", 16192);
        Story.KillQuest(10861, "deadlyseas", UseableMonsters[0]); // Eel Skin x12

        // 10862 | Manatees and Mermaids
        if (!Story.QuestProgression(10862))
        {
            Core.EnsureAccept(10862);
            Core.Join("deadlyseas");
            Bot.Map.Jump("r6", "Left", autoCorrect: false);
            Bot.Wait.ForCellChange("r6");
            Core.Sleep();
            Core.GetMapItem(16193, 1, "deadlyseas");
            Core.EnsureComplete(10862);
        }

        // 10863 | R-eel-ed In
        Story.KillQuest(10863, "deadlyseas", UseableMonsters[0]); // Jagged Teeth x50

        // 10864 | Angler's Game
        Story.MapItemQuest(10864, "deadlyseas", [16195, 16196]);

        // 10865 | Sons of Isthmia
        Story.KillQuest(10865, "deadlyseas", UseableMonsters[1]); // Isthmian Pearl x6

        // 10866 | The rEELper
        Story.KillQuest(10866, "deadlyseas", UseableMonsters[2]); // Isthmian Leviathan Defeated x1
    }
}
