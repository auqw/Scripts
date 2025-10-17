/*
name: (War) Seven Circles
description: This will finish the Seven Circles quest.
tags: story, quest, legion, seven-circles, war
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class SevenCircles
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        CirclesWar();

        Core.SetOptions(false);
    }

    public void Circles()
    {
        if (Core.isCompletedBefore(7977))
            return;

        Story.PreLoad(this);

        Core.AddDrop("Indulgence");

        //Canto IV
        Story.KillQuest(7968, "sevencircles", "Limbo Guard");

        //Canto V
        Story.KillQuest(7969, "sevencircles", "Luxuria Guard");

        //Gone With the Wind
        if (!Story.QuestProgression(7970))
        {
            Core.EnsureAccept(7970);
            Core.KillMonster("sevencircles", "r2", "Left", "Limbo Guard", "Aura of Power");
            Core.KillMonster("sevencircles", "r2", "Left", "Limbo Guard", "Aura of Happiness");
            Core.KillMonster("sevencircles", "r3", "Left", "Luxuria Guard", "Aura of Pleasure");
            Core.EnsureComplete(7970);
        }

        //Lest Ye Be Destroyed    
        Story.KillQuest(7971, "sevencircles", "Luxuria");

        //Canto VI
        Story.MapItemQuest(7972, "sevencircles", 8206, 3);

        //HeckHound
        Story.KillQuest(7973, "sevencircles", "Gluttony Guard");

        //Glutton  for Punishment
        Story.KillQuest(7974, "sevencircles", "Gluttony");

        //Canto VII
        Story.KillQuest(7975, "sevencircles", "Avarice Guard");

        //Greed the Room
        if (!Story.QuestProgression(7976))
        {
            Core.EnsureAccept(7976);
            Core.HuntMonster("sevencircles", "Limbo Guard", "Limbo Guards Defeated", 2);
            Core.HuntMonster("sevencircles", "Luxuria Guard", "Lust Guards Defeated", 2);
            Core.HuntMonster("sevencircles", "Gluttony Guard", "Gluttony Guard Defeated");
            Core.HuntMonster("sevencircles", "Avarice Guard", "Avarice Guards Defeated", 2);
            Core.EnsureComplete(7976);
        }

        //Ava-risky Business
        Story.KillQuest(7977, "sevencircles", "Avarice");
    }

    public void CirclesWar(bool excludeBoss = false, bool StopForGoldFarm = false)
    {
        if (excludeBoss ? Core.isCompletedBefore(7989) : Core.isCompletedBefore(7990))
            return;

        Circles();

        Core.AddDrop("Essence of Treachery", "Essence of Violence", "Souls of Heresy", "Essence of Wrath");

        //Guards of Wrath
        Core.EquipClass(ClassType.Farm);
        Story.KillQuest(7979, "sevencircleswar", "Wrath Guard");

        //War Medals
        Story.KillQuest(7980, "sevencircleswar", "Wrath Guard");

        //Mega War Medals
        Story.KillQuest(7981, "sevencircleswar", "Wrath Guard");

        if (StopForGoldFarm)
            return;

        //Wrath Against the Machine  
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(7982, "sevencircleswar", "Wrath");

        //Blasphemy? Blasphe-you!
        Core.EquipClass(ClassType.Farm);
        Story.KillQuest(7983, "sevencircleswar", "Heresy Guard");

        //Violence's Gatekeeper
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(7984, "sevencircleswar", "Violence's Gatekeeper");

        //Meaningless Violence
        Core.EquipClass(ClassType.Farm);
        Story.KillQuest(7985, "sevencircleswar", "Violence Guard");

        //Geryon, Not Gary On!
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(7986, "sevencircleswar", "Geryon");

        //Violence
        Story.KillQuest(7987, "sevencircleswar", "Violence");

        //Where the Trea-sun Don't Shine
        Core.EquipClass(ClassType.Farm);
        Story.KillQuest(7988, "sevencircleswar", "Treachery Guard");

        //Hanged for Treason
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(7989, "sevencircleswar", "Treachery");

        if (excludeBoss)
            return;

        //The Beast
        if (!Story.QuestProgression(7990))
        {
            Core.EnsureAccept(7990);
            Core.KillMonster("sevencircleswar", "r17", "Left", "The Beast", "The Beast Defeated");
            Core.EnsureComplete(7990);
        }
    }


}
