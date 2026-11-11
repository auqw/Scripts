/*
name: Kathool Adept
description: Farms Kathool Adept through Bottom Feeder or the owned Adeptus Kathooli shortcut. Requires level 80 and completes the Voice in the Sea saga.
tags: kathool, depths, adept, class, callofkathool, somnia, fishing
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
//cs_include Scripts/Other/MergeShops/UndineCommissaryMerge.cs
//cs_include Scripts/Other/MergeShops/SeaviewSouvenirsMerge.cs
//cs_include Scripts/Other/MergeShops/TwilightZoneMerge.cs
//cs_include Scripts/Other/MergeShops/AbyssalZoneMerge.cs
//cs_include Scripts/Other/MergeShops/TrenchObserveMerge.cs
//cs_include Scripts/Other/MergeShops/SeaVoiceMerge.cs
//cs_include Scripts/Other/MergeShops/MidnightZoneMerge.cs
using Skua.Core.Interfaces;

#nullable enable

public class KathoolAdept
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private readonly CoreFarms Farm = new();
    private readonly CoreAOR AOR = new();
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    private readonly string[] RequiredArmors =
    {
        "Kathool Acolyte",
        "Ashray Elf Warden",
        "Midnight Glaucus Sage",
        "DeepWater Drow",
        "Ashray Villager",
        "Evacuation Protocol Suit",
        "Undine Defence Director",
        "Elven Heritage Guardian"
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(RequiredArmors);
        Core.BankingBlackList.AddRange(new[]
        {
            "Adeptus Kathooli", "Johnnyfish", "Kultist's Morsel", "Kathool Adept"
        });
        Core.SetOptions();

        try
        {
            GetClass();
        }
        finally
        {
            Core.SetOptions(false);
        }
    }

    public void GetClass(bool rankUpClass = true)
    {
        if (Core.CheckInventory(104429, toInv: false))
        {
            Core.Logger("Kathool Adept is already owned.");
            if (rankUpClass)
                Adv.RankUpClass("Kathool Adept");
            return;
        }

        Farm.Experience(80);

        if (Bot.ShouldExit)
            return;

        AOR.SeaVoice();
        if (Bot.ShouldExit)
            return;

        Bot.Wait.ForTrue(() => Core.isCompletedBefore(9348, log: false), 20);
        if (Bot.ShouldExit)
            return;
        if (!Core.isCompletedBefore(9348, log: false))
        {
            Core.Logger("Voice in the Sea saga completion was not confirmed. Class quests will not start.");
            return;
        }

        Core.AddDrop(104429, 104532, 104533);
        Core.Join("callofkathool");

        // Both unlock quests advance slot 657 to the same value.
        if (!Bot.Quests.IsUnlocked(10920))
        {
            if (Core.CheckInventory(78153))
            {
                Core.Logger("Using the owned Adeptus Kathooli shortcut.");
                if (!AcceptQuest(10919))
                    return;
                Core.EnsureComplete(10919);
            }
            else
            {
                if (!BottomFeeder())
                    return;
            }

            Bot.Wait.ForTrue(() => Bot.Quests.IsUnlocked(10920), 20);
            if (Bot.ShouldExit)
                return;
            if (!Bot.Quests.IsUnlocked(10920))
            {
                Core.Logger("Call of Kathool is still locked. Unlock quest completion was not confirmed.");
                return;
            }
        }

        Core.Join("callofkathool");
        if (!AcceptQuest(10920))
            return;

        Core.EquipClass(ClassType.Solo);
        Core.HuntMonster("callofkathool", "Mad Morsel", "Kathool's Morsel Consumed", log: false);
        if (Bot.ShouldExit)
            return;
        if (!Bot.TempInv.Contains(104534))
        {
            Core.Logger("The Mad Morsel quest drop was not obtained.");
            return;
        }

        Core.EnsureComplete(10920, 104429);
        Bot.Wait.ForTrue(() => Bot.Inventory.Contains(104429), 20);
        if (Bot.Inventory.Contains(104429))
        {
            Core.Logger("Kathool Adept obtained.");
            if (rankUpClass)
                Adv.RankUpClass("Kathool Adept");
        }
        else
            Core.Logger("The Kathool Adept reward was not received. Check the quest and inventory space.");
    }

    private bool BottomFeeder()
    {
        FarmRequiredArmors();
        if (Bot.ShouldExit)
            return false;

        foreach (string armor in RequiredArmors)
        {
            if (!Core.CheckInventory(armor) || !Bot.Inventory.Contains(armor))
            {
                Core.Logger($"Required armor is missing from inventory: {armor}.");
                return false;
            }
        }

        Farm.SomniaREP();
        if (Bot.ShouldExit)
            return false;
        if (Farm.FactionRank("Somnia") < 10)
        {
            Core.Logger("Bottom Feeder requires rank 10 Somnia.");
            return false;
        }

        Farm.FishingREP();
        if (Bot.ShouldExit)
            return false;
        if (Farm.FactionRank("Fishing") < 10)
        {
            Core.Logger("Johnnyfish requires rank 10 Fishing.");
            return false;
        }

        Core.BuyItem("greenguardwest", 363, 104533, shopItemID: 15252);
        if (Bot.ShouldExit)
            return false;
        if (!Core.CheckInventory(104533) || !Bot.Inventory.Contains(104533))
        {
            Core.Logger("Johnnyfish is missing from inventory.");
            return false;
        }

        Core.Join("callofkathool");
        if (!AcceptQuest(10918))
            return false;

        Core.EquipClass(ClassType.Farm);
        Core.HuntMonster("callofkathool", "Kathool Kultist", "Kultist's Morsel", 75, isTemp: false, log: false);
        if (Bot.ShouldExit)
            return false;
        if (!Bot.Inventory.Contains(104532, 75))
        {
            Core.Logger("Bottom Feeder requires 75 Kultist morsels in inventory.");
            return false;
        }

        Core.EnsureComplete(10918);
        return !Bot.ShouldExit;
    }

    private void FarmRequiredArmors()
    {
        if (!Bot.ShouldExit && !Core.CheckInventory("Ashray Villager"))
            new SeaviewSouvenirsMerge().BuyAllMerge("Ashray Villager");

        if (!Bot.ShouldExit && !Core.CheckInventory("Undine Defence Director"))
            new UndineCommissaryMerge().BuyAllMerge("Undine Defence Director");

        if (!Bot.ShouldExit && !Core.CheckInventory("Evacuation Protocol Suit"))
            new TwilightZoneMerge().BuyAllMerge("Evacuation Protocol Suit");

        if (!Bot.ShouldExit && !Core.CheckInventory("Elven Heritage Guardian"))
            new MidnightZoneMerge().BuyAllMerge("Elven Heritage Guardian");

        // DeepWater Drow consumes one Warden. Obtain the retained Warden afterward.
        if (!Bot.ShouldExit && !Core.CheckInventory("DeepWater Drow"))
            new TrenchObserveMerge().BuyAllMerge("DeepWater Drow");

        if (!Bot.ShouldExit && !Core.CheckInventory("Ashray Elf Warden"))
            new AbyssalZoneMerge().BuyAllMerge("Ashray Elf Warden");

        if (!Bot.ShouldExit && !Core.CheckInventory("Midnight Glaucus Sage"))
        {
            SeaVoiceMerge seaVoice = new();
            // Buy prerequisites first so their merges do not consume materials reserved for the Sage.
            if (!Core.CheckInventory("Midnight Glaucus Mystic"))
            {
                seaVoice.BuyAllMerge("Glaucus Sage");
                seaVoice.BuyAllMerge("Midnight Glaucus Mystic");
            }
            seaVoice.BuyAllMerge("Midnight Glaucus Sage");
        }

        if (!Bot.ShouldExit && !Core.CheckInventory("Kathool Acolyte"))
        {
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("deepchaos", "Kathool", "Kathool Acolyte", isTemp: false, log: false);
        }
    }

    private bool AcceptQuest(int questID)
    {
        if (Bot.ShouldExit)
            return false;

        Core.EnsureAccept(questID);
        if (Bot.ShouldExit)
            return false;
        if (Bot.Quests.IsInProgress(questID))
            return true;

        Core.Logger($"Quest {questID} was not accepted. Check its requirements.");
        return false;
    }
}
