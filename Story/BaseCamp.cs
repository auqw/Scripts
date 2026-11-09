/*
name: Base Camp Storyline
description: This will complete the storyline in /basecamp.
tags: story, quest, base camp,basecamp,puff
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class BaseCamp
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
        if (Core.isCompletedBefore(10273))
            return;

        Story.PreLoad(this);

        // 10270 | One Brick at a Time
        if (!Story.QuestProgression(10270))
        {
            Core.EnsureAccept(10270);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("cornelis", "Gargoyle", "Portal Bricks", 25);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("cornelis", "Stone Golem", "Support Pillars", 4);
            Core.HuntMonster("faerie", "Cyclops Warlord", "Magelight", 4);
            Core.EnsureComplete(10270);
        }

        // 10271 | Room with a View
        if (!Story.QuestProgression(10271))
        {
            Core.EnsureAccept(10271);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("dwarfhold", "Gemrald", "Magic Rocks", 8);
            Core.HuntMonster("crashsite", "Mithril Man", "Power Cable");
            Core.HuntMonster("crashsite", "ProtoSartorium", "75-inch Ultra-thin 144hz 1ms LED Display");
            Core.HuntMonster("digitalmaintown", "8-bit Lionfang", "1080x1920 JPEG of Battleon");
            Core.EnsureComplete(10271);
        }

        // 10272 | Transportalation
        if (!Story.QuestProgression(10272))
        {
            Core.EnsureAccept(10272);
            Core.HuntMonster("orctown", "General Porkon", "Burlap Sack");
            Core.HuntMonster("ubear", "Honey Glob", "Lifetime Subscription to Ubear");
            Core.HuntMonster("chaoslab", "Chaorrupted Moglin", "Chauffeur Moglin");
            Story.BuyQuest(10272, "arcangrove", 211, "Potion of Sleeping", 15);
        }

        // 10273 | Your First Portal
        if (!Story.QuestProgression(10273))
        {
            Core.EnsureAccept(10273);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("swordhavenbridge", "Slime", "Nontoxic Children's Glue");
            Core.HuntMonster("farm", "Scarecrow", "Unused Popsicle Sticks", 10);
            Core.HuntMonster("boxes", "Sneeviltron", "Clear Tape");
            Core.EnsureComplete(10273);
        }
    }
}
