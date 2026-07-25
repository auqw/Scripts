/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/Oasis/CoreOasis.cs

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;

public class TheLastSunSetCore
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    private static CoreOasis COA
    {
        get => _COA ??= new CoreOasis();
        set => _COA = value;
    }
    private static CoreOasis _COA;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Core.RunCore();

        Core.SetOptions(false);
    }

    public void DoALl()
    {
        TempleofDoom();
    }

    public void TempleofDoom()
    {
        if (Core.isCompletedBefore(10813))
            return;

        COA.CarcossaCourt();
     
        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Emptiness", // UseableMonsters[0],
            "Doom Leech", // UseableMonsters[1],
            "Tainted Paladin", // UseableMonsters[2],
            "Downfall of Empires", // UseableMonsters[3]
        };
        #endregion Useable Monsters

        // 10810 | Light and Lemmings
        if (!Story.QuestProgression(10810))
        {
            Core.HuntMonsterQuest(10810,
                ("templeofdoom", UseableMonsters[2], ClassType.Farm));
        }

        // 10811 | Logged for Destruction
        if (!Story.QuestProgression(10811))
        {
            Core.HuntMonsterQuest(10811,
                ("templeofdoom", UseableMonsters[0], ClassType.Farm));
        }

        // 10812 | Amplectinol
        if (!Story.QuestProgression(10812))
        {
            Story.MapItemQuest(10812, "templeofdoom", /* Quantity */ 1);
            Core.HuntMonsterQuest(10812,
                ("templeofdoom", UseableMonsters[1], ClassType.Farm));
        }

        // 10813 | Downfall of Empires
        if (!Story.QuestProgression(10813))
        {
            Core.HuntMonsterQuest(10813,
                ("templeofdoom", UseableMonsters[3], ClassType.Boss));
        }


    }
}