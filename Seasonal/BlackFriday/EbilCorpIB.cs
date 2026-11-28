/*
name: EbilCorpIB
description: Completes the quest chain in /ebilcorpib.
tags: ebilcorpib, story, quests, ebilcorpib
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class EbilCorpIB
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
        if (Core.isCompletedBefore(10968)||!Core.isSeasonalMapActive("ebilcorpib"))
            return;

        Story.PreLoad(this);

        string[] UseableMonsters =
        [
            "Ebilcorp IB Drone", // UseableMonsters[0]
            "IB Printer", // UseableMonsters[1]
        ];

        // 10965 | Build a Roadmap
        if (!Story.QuestProgression(10965))
        {
            Core.EnsureAccept(10965);
            Story.MapItemQuest(10965, "ebilcorpib", 16451);
            Story.MapItemQuest(10965, "ebilcorpib", 16452);
            Core.EnsureComplete(10965);
        }

        // 10966 | Hard Stop
        if (!Story.QuestProgression(10966))
        {
            Story.KillQuest(10966, "ebilcorpib", UseableMonsters[0]); // Worker Drones Cleared x15, Worker Drone's Headset x5
        }

        // 10967 | Next Steps
        if (!Story.QuestProgression(10967))
        {
            Story.KillQuest(10967, "ebilcorpib", UseableMonsters[0]); // Server Room Key x1
        }

        // 10968 | End of Day
        if (!Story.QuestProgression(10968))
        {
            Story.KillQuest(10968, "ebilcorpib", UseableMonsters[1]); // IB Defeated x1
        }
    }
}
