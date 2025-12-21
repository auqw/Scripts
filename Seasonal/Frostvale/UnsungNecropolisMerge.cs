/*
name: Unsung Necropolis Merge
description: This bot will farm the items belonging to the selected mode for the Unsung Necropolis Merge [2652] in /unsungnecropolis
tags: unsung, necropolis, merge, unsungnecropolis, angel, azaveyr, wardens, angels, warden, morph, saviors, shadow, crown, royal, convalescence, ivoryfall, dragonblood, crest, snow, gale, cloak, arctic, vestige, tail, wings, ivoryblood, battleaxe, battleaxes, great, shining, dragons, frostvale
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Glacera.cs
//cs_include Scripts/Seasonal/Frostvale/Story/CoreFrostvale.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class UnsungNecropolisMerge
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
    private static CoreFrostvale Frost
    {
        get => _Frost ??= new CoreFrostvale();
        set => _Frost = value;
    }
    private static CoreFrostvale _Frost;

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
                "Tattered Page",
                "Unsung Warden Hair",
                "Unsung Angel Locks",
                "Non-Melt Ice",
                "Azurefall Dragonblood",
                "Azurefall Helm",
                "Azurefall Crest",
                "Ice Gale Cape",
                "Ice Gale Cloak",
                "Glacial Vestige Tail",
                "Glacial Vestige Wings",
                "Glacial Vestige Wings and Tail",
                "Azureblood Blade",
                "Azureblood Battleaxe",
                "Azureblood Battleaxes",
                "Azureblood Great Axe",
                "Azureblood Great Axes",
                "Whistler Bullion",
                "Uncut Ruby",
                "Gleaming Ore",
            }
        );

        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Frost.UnsungNecropolis();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge(
            "unsungnecropolis",
            2652,
            findIngredients,
            buyOnlyThis,
            buyMode: buyMode
        );

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

                case "Tattered Page":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(
                        "unsungnecropolis",
                        "Unsung Knight",
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    break;

                case "Unsung Warden Hair":
                case "Unsung Angel Locks":
                case "Azurefall Dragonblood":
                case "Azurefall Helm":
                case "Azurefall Crest":
                case "Ice Gale Cape":
                case "Ice Gale Cloak":
                case "Glacial Vestige Tail":
                case "Glacial Vestige Wings":
                case "Glacial Vestige Wings and Tail":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(
                        "unsungnecropolis",
                        "The Unsung",
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    break;

                case "Non-Melt Ice":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        if (Core.CheckInventory(57565))
                            Core.HuntMonsterQuestChoose(
                                10539,
                                null,
                                ("unsungnecropolis", "The Unsung", ClassType.Solo),
                                ("unsungnecropolis", "Unsung Warrior", ClassType.Farm),
                                ("unsungnecropolis", "Bone Dragonling", ClassType.Farm)
                            );
                        else
                            Core.HuntMonsterQuest(
                                Core.IsMember ? 10538 : 10537,
                                ("unsungnecropolis", "The Unsung", ClassType.Solo),
                                ("unsungnecropolis", "Unsung Warrior", ClassType.Farm),
                                ("unsungnecropolis", "Bone Dragonling", ClassType.Farm)
                            );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Azureblood Blade":
                case "Azureblood Great Axe":
                case "Azureblood Great Axes":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(
                        "unsungnecropolis",
                        "Dracolich Sole",
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    break;

                case "Azureblood Battleaxe":
                case "Azureblood Battleaxes":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(
                        "unsungnecropolis",
                        "Dracolich Blain",
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    break;

                case "Whistler Bullion":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }
                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(
                            Core.IsMember ? 10525 : 10524,
                            ("frostvalgala", "Unsung Queen", ClassType.Solo),
                            ("frostvalgala", "Unsung Knight", ClassType.Farm),
                            ("frostvalgala", "Unsung Beast", ClassType.Farm)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Uncut Ruby":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    if (Core.isSeasonalMapActive("wentira"))
                        Core.HuntMonster(
                            "wentira",
                            "Pesugihan Boar",
                            req.Name,
                            quant,
                            false,
                            false
                        );
                    else
                        Core.HuntMonster(
                            "frostvalgala",
                            "Vaughn Knight",
                            req.Name,
                            quant,
                            false,
                            false
                        );
                    break;

                case "Gleaming Ore":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("castleparty", "Treasure Chest", req.Name, quant, req.Temp);
                    break;
                #endregion
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "97857",
            "Angel of Azaveyr",
            "Mode: [select] only\nShould the bot buy \"Angel of Azaveyr\" ?",
            false
        ),
        new Option<bool>(
            "97858",
            "Unsung Warden's Mask",
            "Mode: [select] only\nShould the bot buy \"Unsung Warden's Mask\" ?",
            false
        ),
        new Option<bool>(
            "97859",
            "Unsung Angel's Mask",
            "Mode: [select] only\nShould the bot buy \"Unsung Angel's Mask\" ?",
            false
        ),
        new Option<bool>(
            "97860",
            "Unsung Warden Morph",
            "Mode: [select] only\nShould the bot buy \"Unsung Warden Morph\" ?",
            false
        ),
        new Option<bool>(
            "97861",
            "Unsung Angel Visage",
            "Mode: [select] only\nShould the bot buy \"Unsung Angel Visage\" ?",
            false
        ),
        new Option<bool>(
            "97866",
            "Savior's Shadow Cape",
            "Mode: [select] only\nShould the bot buy \"Savior's Shadow Cape\" ?",
            false
        ),
        new Option<bool>(
            "97867",
            "Crown of the Unsung",
            "Mode: [select] only\nShould the bot buy \"Crown of the Unsung\" ?",
            false
        ),
        new Option<bool>(
            "97868",
            "Royal Convalescence",
            "Mode: [select] only\nShould the bot buy \"Royal Convalescence\" ?",
            false
        ),
        new Option<bool>(
            "97869",
            "Dual Royal Convalescence",
            "Mode: [select] only\nShould the bot buy \"Dual Royal Convalescence\" ?",
            false
        ),
        new Option<bool>(
            "98034",
            "Ivoryfall Dragonblood",
            "Mode: [select] only\nShould the bot buy \"Ivoryfall Dragonblood\" ?",
            false
        ),
        new Option<bool>(
            "98035",
            "Ivoryfall Helm",
            "Mode: [select] only\nShould the bot buy \"Ivoryfall Helm\" ?",
            false
        ),
        new Option<bool>(
            "98036",
            "Ivoryfall Crest",
            "Mode: [select] only\nShould the bot buy \"Ivoryfall Crest\" ?",
            false
        ),
        new Option<bool>(
            "98037",
            "Snow Gale Cape",
            "Mode: [select] only\nShould the bot buy \"Snow Gale Cape\" ?",
            false
        ),
        new Option<bool>(
            "98038",
            "Snow Gale Cloak",
            "Mode: [select] only\nShould the bot buy \"Snow Gale Cloak\" ?",
            false
        ),
        new Option<bool>(
            "98039",
            "Arctic Vestige Tail",
            "Mode: [select] only\nShould the bot buy \"Arctic Vestige Tail\" ?",
            false
        ),
        new Option<bool>(
            "98040",
            "Arctic Vestige Wings",
            "Mode: [select] only\nShould the bot buy \"Arctic Vestige Wings\" ?",
            false
        ),
        new Option<bool>(
            "98041",
            "Arctic Vestige Wings and Tail",
            "Mode: [select] only\nShould the bot buy \"Arctic Vestige Wings and Tail\" ?",
            false
        ),
        new Option<bool>(
            "98042",
            "Ivoryblood Blade",
            "Mode: [select] only\nShould the bot buy \"Ivoryblood Blade\" ?",
            false
        ),
        new Option<bool>(
            "98044",
            "Ivoryblood Battleaxe",
            "Mode: [select] only\nShould the bot buy \"Ivoryblood Battleaxe\" ?",
            false
        ),
        new Option<bool>(
            "98045",
            "Ivoryblood Battleaxes",
            "Mode: [select] only\nShould the bot buy \"Ivoryblood Battleaxes\" ?",
            false
        ),
        new Option<bool>(
            "98046",
            "Ivoryblood Great Axe",
            "Mode: [select] only\nShould the bot buy \"Ivoryblood Great Axe\" ?",
            false
        ),
        new Option<bool>(
            "98048",
            "Ivoryblood Great Axes",
            "Mode: [select] only\nShould the bot buy \"Ivoryblood Great Axes\" ?",
            false
        ),
        new Option<bool>(
            "98058",
            "Shining Dragon's Blade of Frostvale",
            "Mode: [select] only\nShould the bot buy \"Shining Dragon's Blade of Frostvale\" ?",
            false
        ),
    };
}
