/*
name: Isthmiacastle
description: Completes the quest chain in /isthmiacastle.
tags: isthmiacastle, story, quests, isthmiacastle
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Prototypes/DeadlySeas/Deadlyseas.cs
using Skua.Core.Interfaces;

public class Isthmiacastle
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static Deadlyseas Prerequisite { get => _Prerequisite ??= new Deadlyseas(); set => _Prerequisite = value; }
    private static Deadlyseas _Prerequisite;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();
        Storyline();
        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(10878))
            return;

        Prerequisite.Storyline();

        Story.PreLoad(this);

        string[] UseableMonsters =
        [
            "Isthmian Eel", // UseableMonsters[0]
            "Isthmian Guard", // UseableMonsters[1]
            "Mersoul", // UseableMonsters[2]
            "Reaper of the Sea", // UseableMonsters[3]
            "Reaper Sharks", // UseableMonsters[4]
        ];

        // 10869 | Elf Loser vs Pirate Beauty, Live Now!
        Story.MapItemQuest(10869, "isthmiacastle", 16208);

        // 10870 | Haddock Enough
        Story.KillQuest(10870, "isthmiacastle", UseableMonsters[1]); // Isthmian Sea Biscuit x3

        // 10871 | Rogue Carp
        Story.MapItemQuest(10871, "isthmiacastle", 16209);
        Story.KillQuest(10871, "isthmiacastle", UseableMonsters[0]); // Eel Stomach Stone x6

        // 10872 | Carpe Die
        Story.KillQuest(
            10872,
            "isthmiacastle",
            new[]
            {
                UseableMonsters[1], // Guard's Spindle Shell x4
                UseableMonsters[0] // Knotted Eel x6
            }
        );

        // 10873 | Nurse Shark
        Story.MapItemQuest(10873, "isthmiacastle", 16210);
        Story.KillQuest(10873, "isthmiacastle", UseableMonsters[4]); // Shark Scythe x12

        // 10874 | Fish are Friends
        Story.MapItemQuest(10874, "isthmiacastle", 16211);
		Story.MapItemQuest(10874, "isthmiacastle", 16214, 10);

        // 10875 | Dead Management
        Story.MapItemQuest(10875, "isthmiacastle", 16212);
        Story.KillQuest(10875, "isthmiacastle", UseableMonsters[0]); // Guard's Sand Dollar x2

        // 10876 | Dream of the Lost
        Story.MapItemQuest(10876, "isthmiacastle", 16213);

        // 10877 | Ghostly Vitreolus
        Story.KillQuest(10877, "isthmiacastle", UseableMonsters[2]); // Pearlescent Silt x6

        // 10878 | Reaper Leviathan
        Story.KillQuest(10878, "isthmiacastle", UseableMonsters[3]); // Reaper of the Sea Defeated x1
    }
}
