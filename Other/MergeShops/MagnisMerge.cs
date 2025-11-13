/*
name: Magnis Merge
description: This bot will farm the items belonging to the selected mode for the Magnis Merge [2602] in /thelimacity
tags: magnis, merge, thelimacity, mindbreaker, paladin, w, shield, mindbreakers, winged, morph, paladins, faceplate, heroic, ground, spear, , mana, solace
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class MagnisMerge
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
            new[] { "Refined Orpheum", "Somnic Extract", "Dwarven Ether", "Theliman Ore" }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("thelimacity", 2602, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Refined Orpheum":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10334);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.KillMonster("somnia", "r9", "Left", "*");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Somnic Extract":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10335);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("somnia", "Nightwyrm", "Nightwyrm Head");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Dwarven Ether":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10332);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(
                            "dwarfhold",
                            "Chaotic Draconian",
                            "Draconian Chaos Ether",
                            8
                        );
                        Core.HuntMonster("dwarfhold", "Chaos Drow", "Drow Chaos Ether", 8);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Theliman Ore":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10333);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.KillMonster("mountainpath", "Enter", "Spawn", "*", "Coarse Ore", 21);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                #endregion
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "82109",
            "MindBreaker Paladin",
            "Mode: [select] only\nShould the bot buy \"MindBreaker Paladin\" ?",
            false
        ),
        new Option<bool>(
            "82110",
            "MindBreaker Paladin w/ Shield",
            "Mode: [select] only\nShould the bot buy \"MindBreaker Paladin w/ Shield\" ?",
            false
        ),
        new Option<bool>(
            "82115",
            "MindBreaker's Winged Hair",
            "Mode: [select] only\nShould the bot buy \"MindBreaker's Winged Hair\" ?",
            false
        ),
        new Option<bool>(
            "82116",
            "MindBreaker's Winged Locks",
            "Mode: [select] only\nShould the bot buy \"MindBreaker's Winged Locks\" ?",
            false
        ),
        new Option<bool>(
            "82117",
            "MindBreaker's Winged Morph",
            "Mode: [select] only\nShould the bot buy \"MindBreaker's Winged Morph\" ?",
            false
        ),
        new Option<bool>(
            "82118",
            "MindBreaker's Winged Visage",
            "Mode: [select] only\nShould the bot buy \"MindBreaker's Winged Visage\" ?",
            false
        ),
        new Option<bool>(
            "82121",
            "MindBreaker Paladin's Faceplate",
            "Mode: [select] only\nShould the bot buy \"MindBreaker Paladin's Faceplate\" ?",
            false
        ),
        new Option<bool>(
            "82124",
            "MindBreaker Paladin's Winged Helm",
            "Mode: [select] only\nShould the bot buy \"MindBreaker Paladin's Winged Helm\" ?",
            false
        ),
        new Option<bool>(
            "82125",
            "MindBreaker Paladin's Heroic Helm",
            "Mode: [select] only\nShould the bot buy \"MindBreaker Paladin's Heroic Helm\" ?",
            false
        ),
        new Option<bool>(
            "82126",
            "MindBreaker Paladin's Cape",
            "Mode: [select] only\nShould the bot buy \"MindBreaker Paladin's Cape\" ?",
            false
        ),
        new Option<bool>(
            "82128",
            "MindBreaker Paladin's Rune",
            "Mode: [select] only\nShould the bot buy \"MindBreaker Paladin's Rune\" ?",
            false
        ),
        new Option<bool>(
            "82127",
            "MindBreaker Paladin's Rune Cape",
            "Mode: [select] only\nShould the bot buy \"MindBreaker Paladin's Rune Cape\" ?",
            false
        ),
        new Option<bool>(
            "82129",
            "MindBreaker Paladin's Ground",
            "Mode: [select] only\nShould the bot buy \"MindBreaker Paladin's Ground\" ?",
            false
        ),
        new Option<bool>(
            "82131",
            "MindBreaker Paladin's Sword",
            "Mode: [select] only\nShould the bot buy \"MindBreaker Paladin's Sword\" ?",
            false
        ),
        new Option<bool>(
            "82132",
            "MindBreaker Paladin's Swords",
            "Mode: [select] only\nShould the bot buy \"MindBreaker Paladin's Swords\" ?",
            false
        ),
        new Option<bool>(
            "82133",
            "MindBreaker Paladin's Spear",
            "Mode: [select] only\nShould the bot buy \"MindBreaker Paladin's Spear\" ?",
            false
        ),
        new Option<bool>(
            "82134",
            "MindBreaker Paladin's Shield",
            "Mode: [select] only\nShould the bot buy \"MindBreaker Paladin's Shield\" ?",
            false
        ),
        new Option<bool>(
            "82136",
            "MindBreaker Paladin's Spear + Shield",
            "Mode: [select] only\nShould the bot buy \"MindBreaker Paladin's Spear + Shield\" ?",
            false
        ),
        new Option<bool>(
            "82137",
            "MindBreaker Paladin's Axe",
            "Mode: [select] only\nShould the bot buy \"MindBreaker Paladin's Axe\" ?",
            false
        ),
        new Option<bool>(
            "82138",
            "MindBreaker Paladin's Axes",
            "Mode: [select] only\nShould the bot buy \"MindBreaker Paladin's Axes\" ?",
            false
        ),
        new Option<bool>(
            "94776",
            "Mana Solace Paladin",
            "Mode: [select] only\nShould the bot buy \"Mana Solace Paladin\" ?",
            false
        ),
        new Option<bool>(
            "94777",
            "Mana Solace Paladin w/ Shield",
            "Mode: [select] only\nShould the bot buy \"Mana Solace Paladin w/ Shield\" ?",
            false
        ),
        new Option<bool>(
            "94782",
            "Mana Solace Winged Hair",
            "Mode: [select] only\nShould the bot buy \"Mana Solace Winged Hair\" ?",
            false
        ),
        new Option<bool>(
            "94783",
            "Mana Solace Winged Locks",
            "Mode: [select] only\nShould the bot buy \"Mana Solace Winged Locks\" ?",
            false
        ),
        new Option<bool>(
            "94784",
            "Mana Solace Winged Morph",
            "Mode: [select] only\nShould the bot buy \"Mana Solace Winged Morph\" ?",
            false
        ),
        new Option<bool>(
            "94785",
            "Mana Solace Winged Visage",
            "Mode: [select] only\nShould the bot buy \"Mana Solace Winged Visage\" ?",
            false
        ),
        new Option<bool>(
            "94788",
            "Mana Solace Paladin's Faceplate",
            "Mode: [select] only\nShould the bot buy \"Mana Solace Paladin's Faceplate\" ?",
            false
        ),
        new Option<bool>(
            "94791",
            "Mana Solace Paladin's Winged Helm",
            "Mode: [select] only\nShould the bot buy \"Mana Solace Paladin's Winged Helm\" ?",
            false
        ),
        new Option<bool>(
            "94792",
            "Mana Solace Heroic Helm",
            "Mode: [select] only\nShould the bot buy \"Mana Solace Heroic Helm\" ?",
            false
        ),
        new Option<bool>(
            "94793",
            "Mana Solace Paladin's Cape",
            "Mode: [select] only\nShould the bot buy \"Mana Solace Paladin's Cape\" ?",
            false
        ),
        new Option<bool>(
            "94794",
            "Mana Solace Paladin's Rune Cape",
            "Mode: [select] only\nShould the bot buy \"Mana Solace Paladin's Rune Cape\" ?",
            false
        ),
        new Option<bool>(
            "94795",
            "Mana Solace Paladin's Rune",
            "Mode: [select] only\nShould the bot buy \"Mana Solace Paladin's Rune\" ?",
            false
        ),
        new Option<bool>(
            "94796",
            "Mana Solace Paladin's Ground",
            "Mode: [select] only\nShould the bot buy \"Mana Solace Paladin's Ground\" ?",
            false
        ),
        new Option<bool>(
            "94798",
            "Mana Solace Paladin's Sword",
            "Mode: [select] only\nShould the bot buy \"Mana Solace Paladin's Sword\" ?",
            false
        ),
        new Option<bool>(
            "94799",
            "Mana Solace Paladin's Swords",
            "Mode: [select] only\nShould the bot buy \"Mana Solace Paladin's Swords\" ?",
            false
        ),
        new Option<bool>(
            "94800",
            "Mana Solace Paladin's Spear",
            "Mode: [select] only\nShould the bot buy \"Mana Solace Paladin's Spear\" ?",
            false
        ),
        new Option<bool>(
            "94801",
            "Mana Solace Paladin's Shield",
            "Mode: [select] only\nShould the bot buy \"Mana Solace Paladin's Shield\" ?",
            false
        ),
        new Option<bool>(
            "94803",
            "Mana Solace Paladin's Spear + Shield",
            "Mode: [select] only\nShould the bot buy \"Mana Solace Paladin's Spear + Shield\" ?",
            false
        ),
        new Option<bool>(
            "94804",
            "Mana Solace Paladin's Axe",
            "Mode: [select] only\nShould the bot buy \"Mana Solace Paladin's Axe\" ?",
            false
        ),
        new Option<bool>(
            "94805",
            "Mana Solace Paladin's Axes",
            "Mode: [select] only\nShould the bot buy \"Mana Solace Paladin's Axes\" ?",
            false
        ),
    };
}
