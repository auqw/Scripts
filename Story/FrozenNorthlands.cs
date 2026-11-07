/*
name: Frozen Northlands Story
description: This will finish the Frozen Northlands Story.
tags: story, quest, frozen-northlands
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class FrozenNorthlands
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

        Storyline();

        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (!Core.IsMember)
        {
            Core.Logger("This is a member-only storyline.");
            return;
        }

        if (Core.isCompletedBefore(3638))
            return;

        Story.PreLoad(this);

        // Cleanse the Chaorruption 3634
        if (!Story.QuestProgression(3634))
        {
            Core.EnsureAccept(3634);
            Core.HuntMonster("chaosnorth", "Chaotic Symbiote", "Symbiote Slain", 10);
            Core.HuntMonster("chaosnorth", "Chaorruption", "Chaorruption Cleansed", 10);
            Core.EnsureComplete(3634);
        }

        // Book of Chaos and Flames 3635
        if (!Story.QuestProgression(3635))
        {
            Core.EnsureAccept(3635);
            Core.HuntMonster("chaosnorth", "Chaorrupted Mage", "Chaos Magic Nullified", 13);
            Core.HuntMonster("chaosnorth", "Chaorrupted Mage", "Tempus Chaos Amulet");
            Core.HuntMonster("chaosnorth", "Chaorrupted Mage", "Book of Chaos and Flames");
            Core.EnsureComplete(3635);
        }

        // Facets of Chaos 3636
        if (!Story.QuestProgression(3636))
        {
            Core.EnsureAccept(3636);
            Core.HuntMonster("chaosnorth", "Chaos Gemrald", "Shard of Chaos", 13);
            Core.EnsureComplete(3636);
        }

        // Chaos Eye Spy 3637
        if (!Story.QuestProgression(3637))
        {
            Core.EnsureAccept(3637);
            Core.HuntMonster("chaosnorth", "Chaorruption", "Chaorrupted Tongue", 3);
            Core.HuntMonster("chaosnorth", "Chaos Sp-eye", "Chaos Eyes", 3);
            Core.HuntMonster("chaosnorth", "Chaorrupted Imp", "Imp Ears", 3);
            Core.HuntMonster("chaosnorth", "Chaorrupted Mage", "Mage Nose", 3);
            Core.EnsureComplete(3637);
        }

        // Defeat Chaorrupted Xan Illusion 3638
        Story.KillQuest(3638, "chaosnorth", "Chaorrupted Xan");
    }
}
