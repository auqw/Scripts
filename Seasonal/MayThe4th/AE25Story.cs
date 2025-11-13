/*
name: AE-25's Quests
description: This script will complete AE-25's Quests in /twigguhunt.
tags: ae25,ae-25,twigguhunt,twiggu hunt,may the 4th,seasonal, story, quests, may4th
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/MayThe4th/MurderMoonStory.cs
//cs_include Scripts/Seasonal/MayThe4th/TwigguHunt.cs
using Skua.Core.Interfaces;

public class AE25Quests
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static TwigguHunt TH
    {
        get => _TH ??= new TwigguHunt();
        set => _TH = value;
    }
    private static TwigguHunt _TH;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Storyline();
        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(10227) || !Core.isSeasonalMapActive("twigguhunt"))
            return;

        TH.Storyline();

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Scout Droid", // UseableMonsters[0],
            "Infantry Droid", // UseableMonsters[1],
            "Bodyguard Droid", // UseableMonsters[2],
            "Twiggu's Bodyguard", // UseableMonsters[3],
            "Twiggu", // UseableMonsters[4],
            "Obliterator Droid", // UseableMonsters[5]
        };
        #endregion Useable Monsters

        // 10224 | Swab the Deck
        if (!Story.QuestProgression(10224))
        {
            Core.HuntMonsterQuest(
                10224,
                ("twigguhunt", UseableMonsters[1], ClassType.Farm),
                ("twigguhunt", UseableMonsters[0], ClassType.Farm)
            );
        }

        // 10225 | Raise the Sails
        if (!Story.QuestProgression(10225))
        {
            Core.HuntMonsterQuest(10225, ("twigguhunt", UseableMonsters[2], ClassType.Farm));
        }

        // 10226 | Onward!
        Story.MapItemQuest(10226, "twigguhunt", 14416);
        Story.KillQuest(10226, "twigguhunt", UseableMonsters[2]);

        // 10227 | Batten Down the Hatches
        if (!Story.QuestProgression(10227))
        {
            Core.HuntMonsterQuest(10227, ("twigguhunt", UseableMonsters[5], ClassType.Solo));
        }
    }
}
