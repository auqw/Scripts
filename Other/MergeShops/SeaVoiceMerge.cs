/*
name: SeaVoice Merge
description: This bot will farm the items belonging to the selected mode for the SeaVoice Merge [2320] in /seavoice
tags: seavoice, merge, seavoice, midnight, glaucus, sage, mystic, morph, companion, abyssal, atlanticus, trident
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using System.Dynamic;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models.Auras;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Skills;
using Skua.Core.Options;

public class SeaVoiceMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
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
    private static CoreAdvanced sAdv
    {
        get => _sAdv ??= new CoreAdvanced();
        set => _sAdv = value;
    }
    private static CoreAdvanced _sAdv;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(
            new[]
            {
                "Algal Bloom",
                "Bioluminessence",
                "Dark Elf Pearl",
                "Glaucus Mystic",
                "Water Elf Pearl",
                "Water Elf Antler",
                "Glaucus Companion",
                "Sundered Tentacle",
                "Calamity Atlanticus Trident",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("seavoice", 2320, findIngredients, buyOnlyThis, buyMode: buyMode);

        #region Dont edit this part
        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            int currentQuant = req.Temp
                ? Bot.TempInv.GetQuantity(req.Name)
                : Bot.Inventory.GetQuantity(req.Name);
            if (req == null)
            {
                Core.Logger("req is NULL");
                return;
            }

            switch (req.Name)
            {
                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger(
                        $"The bot hasn't been taught how to get {req.Name}."
                            + (shouldStop ? " Please report the issue." : " Skipping"),
                        messageBox: shouldStop,
                        stopBot: shouldStop
                    );
                    break;
        #endregion

                case "Bioluminessence":
                case "Calamity Atlanticus Trident":
                case "Glaucus Mystic":
                case "Glaucus Companion":
                    Core.FarmingLogger(req.Name, req.Quantity);
                    AttackVoiceInTheSea(req.Name, req.Quantity);
                    break;

                case "Dark Elf Pearl":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9339);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(
                            "trenchobserve",
                            "Lady Noelle",
                            "Noelle's Brooch",
                            log: false
                        );
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(
                            "trenchobserve",
                            "Sea Spirit",
                            "Green Sea Jelly",
                            2,
                            log: false
                        );
                        Core.HuntMonster("trenchobserve", "Necro Adipocere", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Water Elf Pearl":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9302);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(
                            "midnightzone",
                            "Shadow Viscera",
                            "Fleshy Shadows",
                            8,
                            log: false
                        );
                        Core.HuntMonster(
                            "midnightzone",
                            "Venerated Wraith",
                            "Wraith Memento",
                            8,
                            log: false
                        );
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("midnightzone", "Sparagmos", "Memory Card", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Water Elf Antler":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9316);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(
                            "abyssalzone",
                            "The Ashray",
                            "Ashray Artifacts",
                            log: false
                        );
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(
                            "abyssalzone",
                            "Necro Adipocere",
                            "Adipocere Antler",
                            3,
                            log: false
                        );
                        Core.HuntMonster("abyssalzone", "Foam Scavenger");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Sundered Tentacle":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9269);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(
                            "twilightzone",
                            "Leviathan",
                            "Leviathan Tentacle",
                            1,
                            true,
                            false
                        );
                        Core.HuntMonster(
                            "twilightzone",
                            "Decay Spirit",
                            "Decay Essence",
                            8,
                            true,
                            false
                        );
                        Core.HuntMonster(
                            "twilightzone",
                            "Ice Guardian",
                            "Tarnished Icicle",
                            8,
                            true,
                            false
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
            }
        }
    }

    public void AttackVoiceInTheSea(string itemName, int quant)
    {

        // Define the possible solo classes
        string[] PossibleSoloClasses = new[]
        {
            "Chrono ShadowHunter",
            "Chrono ShadowSlayer",
            "Chaos Avenger",
            "Verus DoomKnight",
            "Hollowborn Vindicator",
            "Lich",
            "ArchPaladin",
            "Lord of Order",
            "StoneCrusher",
            "Dragon of Time",
            "Unundead Goat",
        };

        if (!Core.CheckInventory(PossibleSoloClasses, any: true))
            Core.Logger(
                "No soloing classes found in inventory; stopping (go get AP at least and rerun)",
                stopBot: true
            );

        Core.RegisterQuests(9349);
        Core.EquipClass(ClassType.Solo);
        Core.AddDrop("Algal Bloom");
        Core.Unbank("Algal Bloom");
        Adv.GearStore(EnhAfter: true);

        while (!Bot.ShouldExit && !Core.CheckInventory(itemName, quant))
        {
            // First available class in inventory/bank, fallback to equipped
            string? selectedClass =
                PossibleSoloClasses.FirstOrDefault(className =>
                    Bot.Inventory.Items.Any(item => item.Name == className)
                    || Bot.Bank.Items.Any(item => item.Name == className)
                ) ?? Bot.Player.CurrentClass?.Name;

            if (string.IsNullOrWhiteSpace(selectedClass))
            {
                Core.Logger("No soloing class found and nothing equipped; aborting SeaVoice.");
                return;
            }

            Core.Logger($"Soloing \"Voice of the Sea\" with {selectedClass}");

            Adv.GearStore(EnhAfter: true);
            Adv.SmartEnhance(selectedClass);

            KillThing(
                map: "seavoice",
                mobMapID: 1,
                itemUsed: 78994,
                Class: selectedClass,
                item: itemName,
                quant: quant,
                isTemp: true
            );
        }

        Adv.GearStore(true, EnhAfter: true);
        Core.CancelRegisteredQuests();
    }


    public void KillThing(
        string map,
        int mobMapID,
        int itemUsed,
        string Class,
        string item,
        int quant = 1,
        bool isTemp = false
    )
    {
        string? classFromPlayer = Bot.Player.CurrentClass?.Name;

        var itemToEnhance = Bot
            .Inventory?.Items?.FirstOrDefault(x =>
                x?.Equipped == true && Adv.WeaponCatagories.Contains(x.Category)
            )
            ?.Name;

        // if (itemToEnhance != null)
        //     Adv.EnhanceItem(
        //         itemToEnhance,
        //         EnhancementType.Lucky,
        //         wSpecial: WeaponSpecial.Awe_Blast
        //     );

        string? classNameToUse = Class ?? classFromPlayer;
        if (string.IsNullOrWhiteSpace(classNameToUse))
        {
            Core.Logger("KillThing aborted: no class specified and player has no current class.");
            return;
        }

        // FIX: actually equip the class you selected
        Core.Equip(classNameToUse);

        Core.Join(map);
        Bot.Wait.ForMapLoad(map);
        Adv.BuyItem("seavoice", 2320, "Vigil", 1000, 12023);
        Core.Equip(itemUsed);
        Core.Logger($"{itemUsed} [Vigil] Equiped? {Bot.Inventory?.IsEquipped("Vigil")}");
        // Move to mob cell and set respawn
        if (Bot.Player.Cell != "r2")
            Core.Jump("r2", "Left");
        Bot.Player.SetSpawnPoint();

        while (
            !Bot.ShouldExit
            && (isTemp ? !Bot.TempInv.Contains(item, quant) : !Core.CheckInventory(item, quant))
        )
        {
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Bot.Player.Cell != "r2")
                Core.Jump("r2", "Left");

            // === Handle Oxidize aura (use Vigil to cleanse) ===
            while (!Bot.ShouldExit && Bot.Self.Auras.Any(a => a.Name == "Oxidize") && !Bot.Self.Auras.Any(a => a.Name == "Vigil"))
            {
                if (Bot.Skills.CanUseSkill(5))
                    Bot.Skills.UseSkill(5);
                Core.Sleep(500);
                if (Bot.Self.Auras.Any(a => a.Name == "Vigil"))
                    break;
            }

            // === Attack phase ===
            Bot.Combat.Attack("*");
            Core.Sleep(500);
        }

        Core.Logger($"KillThing completed for {item} ({quant}).");
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "79161",
            "Midnight Glaucus Sage",
            "Mode: [select] only\nShould the bot buy \"Midnight Glaucus Sage\" ?",
            false
        ),
        new Option<bool>(
            "79160",
            "Midnight Glaucus Mystic",
            "Mode: [select] only\nShould the bot buy \"Midnight Glaucus Mystic\" ?",
            false
        ),
        new Option<bool>(
            "79154",
            "Glaucus Sage",
            "Mode: [select] only\nShould the bot buy \"Glaucus Sage\" ?",
            false
        ),
        new Option<bool>(
            "79165",
            "Midnight Glaucus Locks",
            "Mode: [select] only\nShould the bot buy \"Midnight Glaucus Locks\" ?",
            false
        ),
        new Option<bool>(
            "79164",
            "Midnight Glaucus Hair",
            "Mode: [select] only\nShould the bot buy \"Midnight Glaucus Hair\" ?",
            false
        ),
        new Option<bool>(
            "79163",
            "Midnight Glaucus Visage",
            "Mode: [select] only\nShould the bot buy \"Midnight Glaucus Visage\" ?",
            false
        ),
        new Option<bool>(
            "79162",
            "Midnight Glaucus Morph",
            "Mode: [select] only\nShould the bot buy \"Midnight Glaucus Morph\" ?",
            false
        ),
        new Option<bool>(
            "79156",
            "Glaucus Visage",
            "Mode: [select] only\nShould the bot buy \"Glaucus Visage\" ?",
            false
        ),
        new Option<bool>(
            "79155",
            "Glaucus Morph",
            "Mode: [select] only\nShould the bot buy \"Glaucus Morph\" ?",
            false
        ),
        new Option<bool>(
            "79166",
            "Midnight Glaucus Companion",
            "Mode: [select] only\nShould the bot buy \"Midnight Glaucus Companion\" ?",
            false
        ),
        new Option<bool>(
            "79167",
            "Abyssal Atlanticus Trident",
            "Mode: [select] only\nShould the bot buy \"Abyssal Atlanticus Trident\" ?",
            false
        ),
    };
}
