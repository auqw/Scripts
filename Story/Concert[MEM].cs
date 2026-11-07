/*
name: Concert Story (Member)
description: This will finish the Concert quest.
tags: story, quest, concert, member
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class Concert
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
        DoAll();
        Core.SetOptions(false);
    }

    public void DoAll()
    {
        if (!Core.IsMember)
        {
            Core.Logger("Concert Event Is Member Only. Skipping this Script");
            return;
        }

        Story.PreLoad(this);

        Vertigo();
        Darkness();
        Feardeath();
    }

    public void Vertigo()
    {
        if (Core.isCompletedBefore(1024))
            return;

        //Up to the Highest Heights 1020
        Story.MapItemQuest(1020, "vertigo", 390);

        //High-sterical 1021
        if (!Story.QuestProgression(1021))
        {
            Core.EnsureAccept(1021);
            Core.HuntMonster("vertigo", "Fear Muncher", "Fear Muncher Teeth", 6);
            Core.HuntMonster("vertigo", "Banished Banshee", "Banshee Wail", 4);
            Core.EnsureComplete(1021);
        }

        //Elevation Sensation 1022
        if (!Story.QuestProgression(1022))
        {
            Core.EnsureAccept(1022);
            Core.HuntMonster("vertigo", "Cloaked Fiend", "Elevated Fiend", 5);
            Core.HuntMonster("vertigo", "Banished Banshee", "Elevated Banshee", 5);
            Core.EnsureComplete(1022);
        }

        //Altitude Adjustment 1023
        if (!Story.QuestProgression(1023))
        {
            Core.EnsureAccept(1023);
            Core.HuntMonster("vertigo", "Cloaked Fiend", "Re-adjusted Fiend", 3);
            Core.HuntMonster("vertigo", "Abandoned Dolly", "Re-adjusted Dolly", 5);
            Core.EnsureComplete(1023);
        }

        //Conquer Vertigo 1024
        Story.KillQuest(1024, "vertigo", "Vertigo");
    }

    public void Darkness()
    {
        if (Core.isCompletedBefore(1029))
            return;

        //No Matter How Dark the Night 1025
        if (!Story.QuestProgression(1025))
        {
            Core.EnsureAccept(1025);
            Core.HuntMonster("darkness", "Fear Muncher", "Beam of Sunlight", 4);
            Core.HuntMonster("darkness", "Banished Banshee", "Beam of Sunshine", 5);
            Core.EnsureComplete(1025);
        }

        //The Dark (K)Nights 1026
        Story.KillQuest(1026, "darkness", "Cloaked Fiend");

        //Something, Something Dark Side 1027
        if (!Story.QuestProgression(1027))
        {
            Core.EnsureAccept(1027);
            Core.HuntMonster("darkness", "Banished Banshee", "Force-d Light", 6);
            Core.HuntMonster("darkness", "Abandoned Dolly", "Force-d Beam", 5);
            Core.EnsureComplete(1027);
        }

        //One Little Candle 1028
        Story.MapItemQuest(1028, "darkness", 392, 10);

        //Conquer Nyctox 1029
        Story.KillQuest(1029, "darkness", "Nyctox");
    }

    public void Feardeath()
    {
        if (Core.isCompletedBefore(1035))
            return;

        //Death Be Not Pround 1030
        Story.MapItemQuest(1030, "feardeath", 391);

        //What Fear Makes 1031
        Story.KillQuest(1031, "feardeath", "Abandoned Dolly");

        //The Little Death 1032
        Story.KillQuest(1032, "feardeath", "Fear Muncher");

        //Terrify the Terrorkind 1033
        if (!Story.QuestProgression(1033))
        {
            Core.EnsureAccept(1033);
            Core.HuntMonster("feardeath", "Fear Muncher", "Muncher Terrified", 2);
            Core.HuntMonster("feardeath", "Abandoned Dolly", "Dolly Terrified", 3);
            Core.HuntMonster("feardeath", "Banished Banshee", "Banshee Terrified", 3);
            Core.HuntMonster("feardeath", "Cloaked Fiend", "Fiend Terrified", 2);
            Core.EnsureComplete(1033);
        }

        //Conquer Thanatops 1034
        Story.KillQuest(1034, "feardeath", "Thanotops");

        //Face Fear 1034
        Story.KillQuest(1035, "fear", "Fear");
    }
}
