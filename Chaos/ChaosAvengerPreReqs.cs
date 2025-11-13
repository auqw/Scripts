/*
name: Chaos Avenger Class
description: Gets the prerequisites for the Chaos avengers class besides the insignias.
tags: chaos avenger, class, prerequisites
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;

public class ChaosAvengerClass
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static Core13LoC LOC
    {
        get => _LOC ??= new Core13LoC();
        set => _LOC = value;
    }
    private static Core13LoC _LOC;

    string[] CavItems = new[]
    {
        "Fragment of the Dragon",
        "Fragment of the Queen",
        "Fragments of the Lords B",
        "Fragments of the Lords A",
        "Fragment of Mount Doomskull",
        "Parallel Chaos Amulet",
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(CavItems);

        Core.SetOptions();

        GetClass();

        Core.SetOptions(false);
    }

    public void GetClass(bool rankup = true)
    {
        //Progress Check
        if (
            Core.CheckInventory("Empowered Chaos Avenger's GreatSword")
            || Core.CheckInventory("Chaos Avenger")
        )
        {
            if (rankup)
                Adv.RankUpClass("Chaos Avenger");
            return;
        }
        //Preload Quests
        if (!Core.isCompletedBefore(3881))
            Story.PreLoad(this);

        //Complete 13LoC Saga
        LOC.Hero();

        Core.EnsureAccept(8301);
        FragmentoftheDragon();
        FragmentofMountDoomskull();
        FragmentsoftheLordsA();
        FragmentsoftheLordsB();
        FragmentoftheQueen();
        ParallelChaosAmulet();
        CompleteandBuy();

        if (rankup)
            Adv.RankUpClass("Chaos Avenger");
    }

    public void ParallelChaosAmulet()
    {
        if (Core.CheckInventory("Parallel Chaos Amulet"))
            return;

        if (Core.CheckInventory("Champion Drakath Insignia", 20))
        {
            //ensure your in the map
            Core.Join("championdrakath");
            Core.BuyItem("championdrakath", 2055, "Parallel Chaos Amulet");
            return;
        }
        else
        {
            Core.Logger("not enough insigs");
            Bot.Stop();
        }
    }

    public void FragmentoftheDragon()
    {
        if (Core.CheckInventory("Fragment of the Dragon"))
            return;

        Core.EquipClass(ClassType.Solo);
        Core.Join("chaoslord");
        Bot.Wait.ForMapLoad("chaoslord");
        Core.Jump("r2", "Left");
        Bot.Wait.ForCellChange("r2");

        Monster? kys = Bot.Monsters.MapMonsters.FirstOrDefault(x => x != null && (x.MapID == 1));
        if (kys != null)
            Core.KillMonster(
                "chaoslord",
                "r2",
                "Left",
                kys.MapID == 0 ? 1 : kys.MapID,
                "Fragment of the Dragon",
                isTemp: false
            );
        else
        {
            Core.Logger("No monster found");
            return;
        }
    }

    public void FragmentofMountDoomskull()
    {
        if (Core.CheckInventory("Fragment of Mount Doomskull", 1300))
            return;

        Core.EquipClass(ClassType.Farm);
        Core.HuntMonster(
            "mountdoomskull",
            "Chaorrupted Rogue",
            "Fragment of Mount Doomskull",
            1300,
            isTemp: false
        );
    }

    public void FragmentsoftheLordsA()
    {
        if (Core.CheckInventory("Fragments of the Lords A"))
            return;

        Core.EquipClass(ClassType.Solo);
        bool Ledgermayne = true;

        //Escherion's Robe
        Core.KillEscherion("Escherion's Robe");

        //Vath's Chaotic Dragonlord Armor
        Core.KillVath("Vath's Chaotic Dragonlord Armor");

        //Chaos Shogun Armor
        Core.KillKitsune("Chaos Shogun Armor");

        //Wolfwing Armor
        Core.HuntMonster("wolfwing", "Wolfwing", "Wolfwing Armor", isTemp: false);

        //Discordia Armor
        Core.HuntMonster("palooza", "Discordia", "Discordia Armor", isTemp: false);

        //Ledgermayne (Armor)
        Core.HuntMonster(
            "Ledgermayne",
            "Ledgermayne",
            "Ledgermayne",
            isTemp: !Ledgermayne,
            publicRoom: Ledgermayne
        );

        Core.BuyItem("championdrakath", 2055, "Fragments of the Lords A");
    }

    public void FragmentsoftheLordsB()
    {
        if (Core.CheckInventory("Fragments of the Lords B"))
            return;

        Core.EquipClass(ClassType.Solo);

        //Tibicenas (Armor)
        Core.HuntMonster("djinn", "Tibicenas", "Tibicenas", isTemp: false);

        //Soul of Chaos Armor
        Core.HuntMonster("dreamnexus", "Khasaanda", "Soul of Chaos Armor", isTemp: false);

        //Iadoa (Armor)
        Core.HuntMonster("mqlesson", "Dragonoid", "Dragonoid of Hours", isTemp: false);
        Core.HuntMonster("timespace", "Chaos Lord Iadoa", "Iadoa", isTemp: false);

        //Chaos Lionfang Armor
        Bot.Quests.UpdateQuest(2814);
        Core.HuntMonster(
            "stormtemple",
            "Chaos Lord Lionfang",
            "Chaos Lionfang Armor",
            isTemp: false
        );

        //Chaos Lord Alteon (Armor)
        Core.HuntMonster(
            "swordhavenfalls",
            "Chaos Lord Alteon",
            "Chaos Lord Alteon",
            isTemp: false
        );

        //Xiang Chaos
        Adv.GearStore();
        Core.KillXiang("Xiang Chaos");
        Adv.GearStore(true);

        Core.BuyItem("championdrakath", 2055, "Fragments of the Lords B");
    }

    public void FragmentoftheQueen()
    {
        if (Core.CheckInventory("Fragment of the Queen", 13))
            return;

        Bot.Quests.UpdateQuest(8094);
        Core.EquipClass(ClassType.Solo);
        Core.HuntMonster("transformation", "Queen of Monsters", "Fragment of the Queen", 13, false);
    }

    public void CompleteandBuy()
    {
        if (!Core.CheckInventory(CavItems))
        {
            foreach (string item in CavItems.Where(x => !Core.CheckInventory(x)))
                Core.Logger($"Missing: {item}");
            return;
        }

        //insurance:
        Core.Unbank(CavItems);

        Core.EnsureComplete(8301);
        Bot.Wait.ForQuestComplete(8301);
        Adv.BuyItem("championdrakath", 2056, "Chaos Avenger");
    }
}
