/*
name: Ultra Moglin Pets Quests
description: This script will do all the quests of Moglin Pets you own.
tags: moglin pets, seasonal, easter, quests, ultra moglin, pets, grenwog, twig, twilly,zorbak,houseguest
*/
//cs_include Scripts/CoreBots.cs
using Skua.Core.Interfaces;

public class UltraMoglinPets
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        DoAll();

        Core.SetOptions(false);
    }

    public void DoAll()
    {
        TwigPet();
        TwillyPet();
        ZorbakPet();
    }

    public void TwigPet()
    {
        if (
            Core.CheckInventory(Core.QuestRewards(10200))
            || !Core.CheckInventory("Ultra Twig's Gift Pet")
        )
            return;

        Core.AddDrop(Core.QuestRewards(10200));

        // Ultra Twig's Gift Quest [10200]
        Core.FarmingLogger(Core.QuestRewards(10200)[0], 1);
        Core.HuntMonsterQuest(
            10200,
            ("river", "Zardman Fisher", ClassType.Farm),
            ("river", "Kuro", ClassType.Farm)
        );
    }

    public void TwillyPet()
    {
        if (
            Core.CheckInventory(Core.QuestRewards(10199))
            || !Core.CheckInventory("Ultra Twilly's Gift Pet")
        )
            return;

        Core.AddDrop(Core.QuestRewards(10199));

        // Ultra Twilly's Gift Quest [10199]
        Core.HuntMonsterQuest(10199, ("farm", "Treeant", ClassType.Farm));
    }

    public void ZorbakPet()
    {
        if (
            Core.CheckInventory(Core.QuestRewards(10201))
            || !Core.CheckInventory("Ultra Zorbak's Gift Pet")
        )
            return;

        Core.AddDrop(Core.QuestRewards(10201));

        // Ultra Zorbak's Gift Quest [10201]
        Core.HuntMonsterQuest(10201, ("graveyard", "Skeletal Warrior", ClassType.Farm));
    }
}
