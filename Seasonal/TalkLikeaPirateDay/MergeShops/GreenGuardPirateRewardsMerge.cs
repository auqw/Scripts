/*
name: Green Guard Pirate Rewards Merge
description: This bot will farm the items belonging to the selected mode for the Green Guard Pirate Rewards Merge [2623] in /piratealliance
tags: green, guard, pirate, rewards, merge, piratealliance, merunit, operator, morph, glorious, spear, stolen, treasure, smugglers, raiment, bandana, bearded, rusted, cutlass, cutlasses, seastrand, gold, sea, king, gravefang, abyssal, horde, sailors, beard, captains, wolfs, golden, bloodbiter, bloodbiters, mortal
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/PirateHunt.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/PirateAlliance.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class GreenGuardPirateRewardsMerge
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
    private static PirateAlliance PA
    {
        get => _PA ??= new PirateAlliance();
        set => _PA = value;
    }
    private static PirateAlliance _PA;

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
                "Bloodstained Doubloon",
                "Mer-Unit Operator Hair",
                "Mer-Unit Operator Locks",
                "Spear of Lost Jewels",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        PA.Storyline();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge(
            "piratealliance",
            2623,
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

                case "Bloodstained Doubloon":
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
                            Core.IsMember ? 10412 : 10410,
                            ("piratealliance", "S.S. Phantom Pirate", ClassType.Farm),
                            ("piratealliance", "Captain Squalus", ClassType.Solo),
                            ("piratealliance", "Phantom Jaws", ClassType.Solo)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Mer-Unit Operator Hair":
                case "Mer-Unit Operator Locks":
                case "Spear of Lost Jewels":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(
                        "piratealliance",
                        "Phantom Jaws",
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    break;
                #endregion
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "89389",
            "Mer-Unit Operator Morph",
            "Mode: [select] only\nShould the bot buy \"Mer-Unit Operator Morph\" ?",
            false
        ),
        new Option<bool>(
            "89390",
            "Mer-Unit Operator Locks",
            "Mode: [select] only\nShould the bot buy \"Mer-Unit Operator Locks\" ?",
            false
        ),
        new Option<bool>(
            "95356",
            "Glorious Spear of Stolen Treasure",
            "Mode: [select] only\nShould the bot buy \"Glorious Spear of Stolen Treasure\" ?",
            false
        ),
        new Option<bool>(
            "95615",
            "Smuggler's Raiment",
            "Mode: [select] only\nShould the bot buy \"Smuggler's Raiment\" ?",
            false
        ),
        new Option<bool>(
            "95616",
            "Smuggler's Bandana",
            "Mode: [select] only\nShould the bot buy \"Smuggler's Bandana\" ?",
            false
        ),
        new Option<bool>(
            "95617",
            "Smugglers Locks",
            "Mode: [select] only\nShould the bot buy \"Smugglers Locks\" ?",
            false
        ),
        new Option<bool>(
            "95618",
            "Smuggler's Bearded Bandana",
            "Mode: [select] only\nShould the bot buy \"Smuggler's Bearded Bandana\" ?",
            false
        ),
        new Option<bool>(
            "95620",
            "Rusted Cutlass",
            "Mode: [select] only\nShould the bot buy \"Rusted Cutlass\" ?",
            false
        ),
        new Option<bool>(
            "95621",
            "Rusted Cutlasses",
            "Mode: [select] only\nShould the bot buy \"Rusted Cutlasses\" ?",
            false
        ),
        new Option<bool>(
            "95622",
            "Seastrand Gold Cutlass",
            "Mode: [select] only\nShould the bot buy \"Seastrand Gold Cutlass\" ?",
            false
        ),
        new Option<bool>(
            "95623",
            "Seastrand Gold Cutlasses",
            "Mode: [select] only\nShould the bot buy \"Seastrand Gold Cutlasses\" ?",
            false
        ),
        new Option<bool>(
            "95626",
            "Sea King Gravefang",
            "Mode: [select] only\nShould the bot buy \"Sea King Gravefang\" ?",
            false
        ),
        new Option<bool>(
            "95627",
            "Abyssal Horde Sailor's Beard",
            "Mode: [select] only\nShould the bot buy \"Abyssal Horde Sailor's Beard\" ?",
            false
        ),
        new Option<bool>(
            "95628",
            "Abyssal Horde Captain's Beard",
            "Mode: [select] only\nShould the bot buy \"Abyssal Horde Captain's Beard\" ?",
            false
        ),
        new Option<bool>(
            "95629",
            "Abyssal Horde Sailor's Locks",
            "Mode: [select] only\nShould the bot buy \"Abyssal Horde Sailor's Locks\" ?",
            false
        ),
        new Option<bool>(
            "95630",
            "Abyssal Horde Captain's Locks",
            "Mode: [select] only\nShould the bot buy \"Abyssal Horde Captain's Locks\" ?",
            false
        ),
        new Option<bool>(
            "95631",
            "Abyssal Horde Captain's Morph",
            "Mode: [select] only\nShould the bot buy \"Abyssal Horde Captain's Morph\" ?",
            false
        ),
        new Option<bool>(
            "95632",
            "Abyssal Horde Wolf's Morph",
            "Mode: [select] only\nShould the bot buy \"Abyssal Horde Wolf's Morph\" ?",
            false
        ),
        new Option<bool>(
            "95634",
            "Golden Bloodbiter",
            "Mode: [select] only\nShould the bot buy \"Golden Bloodbiter\" ?",
            false
        ),
        new Option<bool>(
            "95635",
            "Golden Bloodbiters",
            "Mode: [select] only\nShould the bot buy \"Golden Bloodbiters\" ?",
            false
        ),
        new Option<bool>(
            "95900",
            "Mortal Sea King Gravefang",
            "Mode: [select] only\nShould the bot buy \"Mortal Sea King Gravefang\" ?",
            false
        ),
    };
}
