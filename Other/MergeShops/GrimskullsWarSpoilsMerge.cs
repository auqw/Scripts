/*
name: Grimskulls War Spoils Merge
description: This bot will farm the items belonging to the selected mode for the Grimskulls War Spoils Merge [2590] in /lichwar
tags: grimskulls, war, spoils, merge, lichwar, grimskull, psychophant, halo, morph, wings, grim, psychophants, scythe, fists, fist
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class GrimskullsWarSpoilsMerge
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
        Core.BankingBlackList.AddRange(new[] { "Grimskull's Favor" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("lichwar", 2590, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Grimskull's Favor":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger("Grimskull's Favor requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10282, 10283);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("lichwar", "Noxus Warrior", log: false);
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
            "94075",
            "Grimskull Psychophant",
            "Mode: [select] only\nShould the bot buy \"Grimskull Psychophant\" ?",
            false
        ),
        new Option<bool>(
            "94076",
            "Grimskull Psychophant Halo Morph",
            "Mode: [select] only\nShould the bot buy \"Grimskull Psychophant Halo Morph\" ?",
            false
        ),
        new Option<bool>(
            "94077",
            "Grimskull Psychophant Morph",
            "Mode: [select] only\nShould the bot buy \"Grimskull Psychophant Morph\" ?",
            false
        ),
        new Option<bool>(
            "94078",
            "Grimskull Psychophant Hair",
            "Mode: [select] only\nShould the bot buy \"Grimskull Psychophant Hair\" ?",
            false
        ),
        new Option<bool>(
            "94079",
            "Grimskull Psychophant Halo Visage",
            "Mode: [select] only\nShould the bot buy \"Grimskull Psychophant Halo Visage\" ?",
            false
        ),
        new Option<bool>(
            "94080",
            "Grimskull Psychophant Visage",
            "Mode: [select] only\nShould the bot buy \"Grimskull Psychophant Visage\" ?",
            false
        ),
        new Option<bool>(
            "94081",
            "Grimskull Psychophant Locks",
            "Mode: [select] only\nShould the bot buy \"Grimskull Psychophant Locks\" ?",
            false
        ),
        new Option<bool>(
            "94082",
            "Grimskull Psychophant Wings",
            "Mode: [select] only\nShould the bot buy \"Grimskull Psychophant Wings\" ?",
            false
        ),
        new Option<bool>(
            "94084",
            "Grim Psychophant's Scythe",
            "Mode: [select] only\nShould the bot buy \"Grim Psychophant's Scythe\" ?",
            false
        ),
        new Option<bool>(
            "94085",
            "Grim Psychophant's Fists",
            "Mode: [select] only\nShould the bot buy \"Grim Psychophant's Fists\" ?",
            false
        ),
        new Option<bool>(
            "94086",
            "Grim Psychophant's Fist",
            "Mode: [select] only\nShould the bot buy \"Grim Psychophant's Fist\" ?",
            false
        ),
    };
}
