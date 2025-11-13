/*
name: Thelima Jewelers Merge
description: This bot will farm the items belonging to the selected mode for the Thelima Jewelers Merge [2616] in /thelimacity
tags: thelima, jewelers, merge, thelimacity, custom, gem, vert, vitriol, emerald, emile, melano, amethyst, catalyst, blue, smaragdine, enchanted, golden, claymore, gilded, doomwood, starks, brilliant, ice, engraved, unholy, heavy, holy, desert, dunes, toothbrush, toothpaste, amp
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
//cs_include Scripts/Evil/ShadowFallMerge.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class ThelimaJewelersMerge
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

    private static CoreAOR AOR
    {
        get => _AOR ??= new CoreAOR();
        set => _AOR = value;
    }
    private static CoreAOR _AOR;
    private static ShadowFallMerge SFM
    {
        get => _SFM ??= new ShadowFallMerge();
        set => _SFM = value;
    }
    private static ShadowFallMerge _SFM;

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
                "Mystic Topaz",
                "Dwarven Gold",
                "Maleno Obsidian",
                "Drow Silver",
                "Dwarven Emerald",
                "Drow Amethyst",
                "Silver Claymore",
                "Necrotized Claymore",
                "Stark's Ice",
                "Grave Terror",
                "The Unholy",
                "Heavy Holy Blade",
                "Blade of the Desert Dunes",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        AOR.ThelimaCity();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("thelimacity", 2616, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                #region Items not setup

                case "Mystic Topaz":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("thelimacity", "Noelle Knight", req.Name, quant, false, false);
                    break;

                case "Dwarven Gold":
                case "Dwarven Emerald":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("thelimacity", "Dwarven Aegis", req.Name, quant, false, false);
                    break;

                case "Maleno Obsidian":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(
                        "thelimacity",
                        "Maleno Elemental",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;

                case "Drow Silver":
                case "Drow Amethyst":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("thelimacity", "Drow Soldier", req.Name, quant, false, false);
                    break;

                case "Silver Claymore":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EnsureAccept(739);
                        while (!Bot.ShouldExit && !Core.CheckInventory("Racing Trophy", 20))
                            Core.ChainComplete(746);
                        Core.HuntMonster("table", "Roach", "Gold Roach Antenna", 10);
                        Core.EnsureCompleteChoose(739, new[] { req.Name });
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Necrotized Claymore":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.Logger("Cannot Get Item, requires manual pvp.");
                    break;

                case "Stark's Ice":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem("blindingsnow", 236, req.Name, quant);
                    break;

                case "Grave Terror":
                case "The Unholy":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    SFM.BuyAllMerge(req.Name);
                    break;

                case "Heavy Holy Blade":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem("necropolis", 408, req.Name, quant);
                    break;

                case "Blade of the Desert Dunes":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem("sandsea", 242, req.Name, quant);
                    break;
                #endregion
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "93124",
            "Custom Gem Dagger",
            "Mode: [select] only\nShould the bot buy \"Custom Gem Dagger\" ?",
            false
        ),
        new Option<bool>(
            "93125",
            "Custom Gem Daggers",
            "Mode: [select] only\nShould the bot buy \"Custom Gem Daggers\" ?",
            false
        ),
        new Option<bool>(
            "93380",
            "Vert Vitriol Dagger",
            "Mode: [select] only\nShould the bot buy \"Vert Vitriol Dagger\" ?",
            false
        ),
        new Option<bool>(
            "93381",
            "Vert Vitriol Daggers",
            "Mode: [select] only\nShould the bot buy \"Vert Vitriol Daggers\" ?",
            false
        ),
        new Option<bool>(
            "93385",
            "Emerald Emile",
            "Mode: [select] only\nShould the bot buy \"Emerald Emile\" ?",
            false
        ),
        new Option<bool>(
            "93386",
            "Melano Amethyst Catalyst",
            "Mode: [select] only\nShould the bot buy \"Melano Amethyst Catalyst\" ?",
            false
        ),
        new Option<bool>(
            "93118",
            "Blue Vitriol Dagger",
            "Mode: [select] only\nShould the bot buy \"Blue Vitriol Dagger\" ?",
            false
        ),
        new Option<bool>(
            "93119",
            "Melano Amethyst Dagger",
            "Mode: [select] only\nShould the bot buy \"Melano Amethyst Dagger\" ?",
            false
        ),
        new Option<bool>(
            "93120",
            "Smaragdine Dagger",
            "Mode: [select] only\nShould the bot buy \"Smaragdine Dagger\" ?",
            false
        ),
        new Option<bool>(
            "93121",
            "Smaragdine Daggers",
            "Mode: [select] only\nShould the bot buy \"Smaragdine Daggers\" ?",
            false
        ),
        new Option<bool>(
            "93382",
            "Enchanted Gem Dagger",
            "Mode: [select] only\nShould the bot buy \"Enchanted Gem Dagger\" ?",
            false
        ),
        new Option<bool>(
            "93383",
            "Enchanted Gem Daggers",
            "Mode: [select] only\nShould the bot buy \"Enchanted Gem Daggers\" ?",
            false
        ),
        new Option<bool>(
            "95099",
            "Golden Claymore",
            "Mode: [select] only\nShould the bot buy \"Golden Claymore\" ?",
            false
        ),
        new Option<bool>(
            "95100",
            "Gilded Claymore of Doomwood",
            "Mode: [select] only\nShould the bot buy \"Gilded Claymore of Doomwood\" ?",
            false
        ),
        new Option<bool>(
            "95101",
            "Stark's Brilliant Ice Blade",
            "Mode: [select] only\nShould the bot buy \"Stark's Brilliant Ice Blade\" ?",
            false
        ),
        new Option<bool>(
            "95102",
            "Gilded Engraved Blade",
            "Mode: [select] only\nShould the bot buy \"Gilded Engraved Blade\" ?",
            false
        ),
        new Option<bool>(
            "95103",
            "Golden Unholy Blade",
            "Mode: [select] only\nShould the bot buy \"Golden Unholy Blade\" ?",
            false
        ),
        new Option<bool>(
            "95104",
            "Brilliant Heavy Holy Blade",
            "Mode: [select] only\nShould the bot buy \"Brilliant Heavy Holy Blade\" ?",
            false
        ),
        new Option<bool>(
            "95105",
            "Gilded Blade of the Desert Dunes",
            "Mode: [select] only\nShould the bot buy \"Gilded Blade of the Desert Dunes\" ?",
            false
        ),
        new Option<bool>(
            "95106",
            "Golden Toothbrush",
            "Mode: [select] only\nShould the bot buy \"Golden Toothbrush\" ?",
            false
        ),
        new Option<bool>(
            "95107",
            "Golden Toothpaste",
            "Mode: [select] only\nShould the bot buy \"Golden Toothpaste\" ?",
            false
        ),
        new Option<bool>(
            "95108",
            "Golden Toothbrush &amp; Toothpaste",
            "Mode: [select] only\nShould the bot buy \"Golden Toothbrush &amp; Toothpaste\" ?",
            false
        ),
    };
}
