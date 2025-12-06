/*
name: Tavern
description: Quest from `Ulfgar Geirsson` in /tavern
tags: tavern, Ulfgar Geirsson
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;

public class Tavern
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static CoreDailies Daily { get => _Daily ??= new CoreDailies(); set => _Daily = value; }
    private static CoreDailies _Daily;

    public void ScriptMain(IScriptInterface Bot)
    {
        // Core.BankingBlackList.AddRange(new[] { "item1", "Item2", "Etc" });
        Core.SetOptions();

        TavernStory();

        Core.SetOptions(false);
    }

    public void TavernStory()
    {
        if (Core.isCompletedBefore(3181))
            return;

        Story.PreLoad(this);

        // 3176 | Rune Escaped
        if (!Story.QuestProgression(3176))
        { 
            Core.HuntMonsterQuest(3176,
                ("sandport", "Tomb Robber", ClassType.Farm));
        }


        // 3177 | Hone in on the Horn
        if (!Story.QuestProgression(3177))
        {
            Core.HuntMonsterQuest(3177,
                ("mythsong", "French Horned Toadragon", ClassType.Solo));
        }


        // 3178 | Barrier Carrier
        if (!Story.QuestProgression(3178))
        {
            Core.HuntMonsterQuest(3178,
                ("crashsite", "Barrier Bot", ClassType.Solo));
        }


        // 3179 | Thor's Fishing Tale
        if (!Story.QuestProgression(3179))
        { 
            Core.HuntMonsterQuest(3179,
                ("natatorium", "Anglerfish", ClassType.Farm));
        }


        // 3180 | Skulls, Bones, and Runestones
        if (!Story.QuestProgression(3180))
        {
            Core.HuntMonsterQuest(3180,
                ("battleundera", "Bone Terror", ClassType.Farm));
        }


        // 3181 | A Key Discovery: King with the Key
        if (!Story.QuestProgression(3181))
        {
            Core.HuntMonsterQuest(3181,
                ("kingcoal", "Frost King", ClassType.Solo));
        }


    }


}



