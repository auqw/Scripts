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
    public CoreStory Story = new();

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
            Core.HuntMonsterQuest(10270,
                ("cornelis", "Gargoyle", ClassType.Farm),
                ("cornelis", "Stone Golem", ClassType.Farm),
                ("faerie", "Cyclops Warlord", ClassType.Farm));
        }


        // 10271 | Room with a View
        if (!Story.QuestProgression(10271))
        {
            Core.HuntMonsterQuest(10271,
                ("dwarfhold", "Gemrald", ClassType.Farm),
                ("crashsite", "Mithril Man", ClassType.Farm),
                ("crashsite", "ProtoSartorium", ClassType.Farm),
                ("digitalmaintown", "8-Bit Lionfang", ClassType.Farm));
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
            Core.HuntMonsterQuest(10273,
                ("swordhavenbridge", "Slime", ClassType.Farm),
                ("farm", "Scarecrow", ClassType.Farm),
                ("boxes", "Sneeviltron", ClassType.Farm));
        }


    }
}