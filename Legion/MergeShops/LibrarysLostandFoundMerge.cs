/*
name: Librarys Lost and Found Merge
description: This bot will farm the items belonging to the selected mode for the Librarys Lost and Found Merge [2563] in /legionlibrary
tags: librarys, lost, and, found, merge, legionlibrary, argus, panoptes, phlegethon, magma, lava, legion, loremaster, tome, lorekeeper, veil, strategy, table, study, underworld
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class LibrarysLostandFoundMerge
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
            new[] { "Argus' Iris", "Underworld Linen", "River Glowstone", "Teacup Mace" }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("legionlibrary", 2563, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Argus' Iris":
                    Iris(quant);
                    break;

                case "Underworld Linen":
                    Linen(quant);
                    break;

                case "River Glowstone":
                    Glowstone(quant);
                    break;

                case "Teacup Mace":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("junkhoard", "Junk Golem", req.Name, quant, false, false);
                    break;
            }
        }
    }

    public void Iris(int quant = 1000)
    {
        Core.FarmingLogger("Argus' Iris", quant);
        while (!Bot.ShouldExit && !Core.CheckInventory("Argus' Iris", quant))
        {
            Core.HuntMonsterQuest(10097, ("legionlibrary", "Argus Panoptes", ClassType.Solo));
            Bot.Wait.ForPickup("Argus' Iris");
        }
        Core.CancelRegisteredQuests();
    }

    public void Linen(int quant = 1000)
    {
        Core.FarmingLogger("Underworld Linen", quant);
        if (!Core.isCompletedBefore(10098))
        {
            Core.HuntMonsterQuest(10098, ("dagefortress", "Scorned Knight", ClassType.Farm));
        }
        while (!Bot.ShouldExit && !Core.CheckInventory("Underworld Linen", quant))
        {
            Core.HuntMonsterQuest(
                10099,
                ("judgement", "Minos", ClassType.Solo),
                ("judgement", "Rhadamanthys", ClassType.Solo),
                ("judgement", "Aeacus", ClassType.Solo)
            );
            Bot.Wait.ForPickup("Underworld Linen");
        }
        Core.CancelRegisteredQuests();
    }

    public void Glowstone(int quant = 1000)
    {
        Core.FarmingLogger("River Glowstone", quant);
        if (!Core.isCompletedBefore(10098))
            Glowstone(2);
        if (!Core.isCompletedBefore(10100))
        {
            Core.HuntMonsterQuest(10100, ("legionlibrary", "Cerberus Pup", ClassType.Farm));
        }
        while (!Bot.ShouldExit && !Core.CheckInventory("River Glowstone", quant))
        {
            Core.HuntMonsterQuest(10101, ("styx", "Styx Hydra", ClassType.Solo));
            Bot.Wait.ForPickup("River Glowstone");
        }
        Core.CancelRegisteredQuests();
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "92402",
            "Argus Panoptes Gauntlets",
            "Mode: [select] only\nShould the bot buy \"Argus Panoptes Gauntlets\" ?",
            false
        ),
        new Option<bool>(
            "92401",
            "Argus Panoptes Gauntlet",
            "Mode: [select] only\nShould the bot buy \"Argus Panoptes Gauntlet\" ?",
            false
        ),
        new Option<bool>(
            "92400",
            "Phlegethon Magma",
            "Mode: [select] only\nShould the bot buy \"Phlegethon Magma\" ?",
            false
        ),
        new Option<bool>(
            "92399",
            "Phlegethon Lava",
            "Mode: [select] only\nShould the bot buy \"Phlegethon Lava\" ?",
            false
        ),
        new Option<bool>(
            "92305",
            "Legion Loremaster Tome",
            "Mode: [select] only\nShould the bot buy \"Legion Loremaster Tome\" ?",
            false
        ),
        new Option<bool>(
            "92304",
            "Legion Lorekeeper Veil",
            "Mode: [select] only\nShould the bot buy \"Legion Lorekeeper Veil\" ?",
            false
        ),
        new Option<bool>(
            "92303",
            "Legion Loremaster Veil",
            "Mode: [select] only\nShould the bot buy \"Legion Loremaster Veil\" ?",
            false
        ),
        new Option<bool>(
            "92302",
            "Legion Loremaster",
            "Mode: [select] only\nShould the bot buy \"Legion Loremaster\" ?",
            false
        ),
        new Option<bool>(
            "92408",
            "Legion Strategy Table",
            "Mode: [select] only\nShould the bot buy \"Legion Strategy Table\" ?",
            false
        ),
        new Option<bool>(
            "92407",
            "Legion Study Table",
            "Mode: [select] only\nShould the bot buy \"Legion Study Table\" ?",
            false
        ),
        new Option<bool>(
            "92470",
            "Underworld Lorekeeper",
            "Mode: [select] only\nShould the bot buy \"Underworld Lorekeeper\" ?",
            false
        ),
        new Option<bool>(
            "92471",
            "Underworld Lorekeeper Veil",
            "Mode: [select] only\nShould the bot buy \"Underworld Lorekeeper Veil\" ?",
            false
        ),
    };
}
