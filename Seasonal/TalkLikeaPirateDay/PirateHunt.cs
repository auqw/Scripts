/*
name: Pirate Hunt Story
description: Completes the storyline in piratehunt
tags: piratehunt, story, saga
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class PirateHuntStory
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        PirateHuntSaga();
        
        Core.SetOptions(false);
    }

    public void PirateHuntSaga()
    {
        if (Core.isCompletedBefore(10399))
            return;

        Story.PreLoad(this);

        // Ensure bot keeps required flags for final quest
        Core.AddDrop("Belladonna's Flag", "Bourgeois' Flag", "Pirated Tech's Flag", "Merry Celeste's Flag", "Mercurius' Flag");

        // 10389 - My Unfair Lady
        Core.EquipClass(ClassType.Farm);
        Story.KillQuest(10389, "piratehunt", "Belladonna Pirate");

        // 10390 - Bumbling Bellamy
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(10390, "piratehunt", "Captain Bellamy");

        // 10391 - Verich City
        Core.EquipClass(ClassType.Farm);
        Story.KillQuest(10391, "piratehunt", "Bourgeois Pirate");

        // 10392 - From the Top
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(10392, "piratehunt", "Captain Verich");

        // 10393 - Technical Piracy
        Core.EquipClass(ClassType.Farm);
        Story.KillQuest(10393, "piratehunt", "Pirated Tech");

        // 10394 - Subversive Sailor
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(10394, "piratehunt", "Captain Chamfer");

        // 10395 - Plundered Souls
        Core.EquipClass(ClassType.Farm);
        Story.KillQuest(10395, "piratehunt", "Merry Celeste Crew");

        // 10396 - Salty Spirit
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(10396, "piratehunt", "Captain Haines");

        // 10397 - Nautical Narcissism
        Core.EquipClass(ClassType.Farm);
        Story.KillQuest(10397, "piratehunt", "Mercurius Pirate");

        // 10398 - Mercurious
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(10398, "piratehunt", new[] { "Dragonsworn Larunda", "Captain Mercurius" });

        // 10399 - Scurvy Most Wanted
        Story.ChainQuest(10399);
    }
}
