/*
name: Fortressparty
description: Completes the quest chain in /fortressparty.
tags: fortressparty, story, quests, fortressparty
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class Fortressparty
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
        if (Core.isCompletedBefore(10892))
            return;

        Story.PreLoad(this);

        string[] UseableMonsters =
        [
            "Alteon Cosplayer", // UseableMonsters[0]
            "Athon Cosplayer", // UseableMonsters[1]
            "Doom Figment", // UseableMonsters[2]
            "Drago Cosplayer", // UseableMonsters[3]
            "Empire Reveler", // UseableMonsters[4]
            "Fiend Servant", // UseableMonsters[5]
            "Hopeful Shadow", // UseableMonsters[6]
            "Photoghoulpher", // UseableMonsters[7]
            "Shadowmuncher", // UseableMonsters[8]
        ];

        // 10883 | Party Crashout
        Story.KillQuest(10883, "fortressparty", UseableMonsters[4]); // Stolen Presents x12

        // 10884 | Cake Conquest
        Story.KillQuest(10884, "fortressparty", UseableMonsters[8]); // Cake Slice x60

        // 10885 | Spirited Photography
        Story.KillQuest(10885, "fortressparty", UseableMonsters[7]); // Broken Camera x12

        // 10886 | Pain Proxy
        Story.KillQuest(10886, "fortressparty", UseableMonsters[5]); // Fiend Servant Subdued x1

        // 10887 | Mar-Athon of Sorrow
        Story.KillQuest(10887, "fortressparty", UseableMonsters[1]); // Athon Costume x1

        // 10888 | King of the Party
        Story.KillQuest(10888, "fortressparty", UseableMonsters[0]); // King Alteon Costume x1

        // 10889 | Fate's Facilitators
        Story.KillQuest(10889, "fortressparty", UseableMonsters[2]); // Doom Residue x6

        // 10890 | Here, Kitty Kitty!
        Story.KillQuest(10890, "fortressparty", UseableMonsters[3]); // Drago Costume x1

        // 10891 | Ruined Surprise
        Story.MapItemQuest(10891, "fortressparty", 16250);

        // 10892 | Just a Dad
        Story.KillQuest(10892, "fortressparty", UseableMonsters[6]); // Hopeful Shadow Defeated x1
    }
}
