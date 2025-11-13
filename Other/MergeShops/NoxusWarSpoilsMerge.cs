/*
name: Noxus War Spoils Merge
description: This bot will farm the items belonging to the selected mode for the Noxus War Spoils Merge [2589] in /lichwar
tags: noxus, war, spoils, merge, lichwar, psychophant, horned, morph, wings, psychophants, spear, noxious, beast
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class NoxusWarSpoilsMerge
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
        Core.BankingBlackList.AddRange(new[] { "Noxus' Favor" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("lichwar", 2589, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Noxus' Favor":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger("Noxus' Favor requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10278, 10279);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("lichwar", "Grim Soldier", log: false);
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
            "94087",
            "Noxus Psychophant",
            "Mode: [select] only\nShould the bot buy \"Noxus Psychophant\" ?",
            false
        ),
        new Option<bool>(
            "94088",
            "Horned Noxus Psychophant Morph",
            "Mode: [select] only\nShould the bot buy \"Horned Noxus Psychophant Morph\" ?",
            false
        ),
        new Option<bool>(
            "94089",
            "Noxus Psychophant Morph",
            "Mode: [select] only\nShould the bot buy \"Noxus Psychophant Morph\" ?",
            false
        ),
        new Option<bool>(
            "94090",
            "Noxus Psychophant Hair",
            "Mode: [select] only\nShould the bot buy \"Noxus Psychophant Hair\" ?",
            false
        ),
        new Option<bool>(
            "94091",
            "Noxus Psychophant Locks",
            "Mode: [select] only\nShould the bot buy \"Noxus Psychophant Locks\" ?",
            false
        ),
        new Option<bool>(
            "94092",
            "Noxus Psychophant Visage",
            "Mode: [select] only\nShould the bot buy \"Noxus Psychophant Visage\" ?",
            false
        ),
        new Option<bool>(
            "94093",
            "Noxus Psychophant Wings",
            "Mode: [select] only\nShould the bot buy \"Noxus Psychophant Wings\" ?",
            false
        ),
        new Option<bool>(
            "94095",
            "Noxus Psychophant's Spear",
            "Mode: [select] only\nShould the bot buy \"Noxus Psychophant's Spear\" ?",
            false
        ),
        new Option<bool>(
            "94097",
            "Noxious Beast Gauntlet",
            "Mode: [select] only\nShould the bot buy \"Noxious Beast Gauntlet\" ?",
            false
        ),
        new Option<bool>(
            "94096",
            "Noxious Beast Gauntlets",
            "Mode: [select] only\nShould the bot buy \"Noxious Beast Gauntlets\" ?",
            false
        ),
    };
}
