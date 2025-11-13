/*
name: Grenwog Warrens Merge
description: This bot will farm the items belonging to the selected mode for the Grenwog Warrens Merge [2571] in /grenwogwarren
tags: grenwog, warrens, merge, grenwogwarren, lagomorph, techwear, tech, morph, ears, techmask, royal, graffiti, wings, angel, crown, punk, hammer, hammers
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/Grenwog(Easter)/GrenWogWarren.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class GrenwogWarrensMerge
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
    private static GrenWogWarren GrenWogWarren
    {
        get => _GrenWogWarren ??= new GrenWogWarren();
        set => _GrenWogWarren = value;
    }
    private static GrenWogWarren _GrenWogWarren;
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
                "Rubber Egg",
                "Sugary Egg",
                "Scaly Egg",
                "Aged Egg",
                "Liquid Egg",
                "Cabdury Egg",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        GrenWogWarren.Storyline();

        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("grenwogwarren", 2571, findIngredients, buyOnlyThis, buyMode: buyMode);

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

            string[] UseableMonsters = new[]
            {
                "Grenwog", // UseableMonsters[0],
                "Sugar Grenwog", // UseableMonsters[1],
                "Draconic Grenwog", // UseableMonsters[2],
                "Jurassic Grenwog", // UseableMonsters[3],
                "Elixir Grenwog", // UseableMonsters[4],
                "Alpha Cabdury", // UseableMonsters[5]
            };

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

                case "Rubber Egg":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "grenwogwarren",
                        UseableMonsters[0],
                        req.Name,
                        quant,
                        isTemp: false
                    );
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Sugary Egg":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "grenwogwarren",
                        UseableMonsters[1],
                        req.Name,
                        quant,
                        isTemp: false
                    );
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Scaly Egg":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "grenwogwarren",
                        UseableMonsters[2],
                        req.Name,
                        quant,
                        isTemp: false
                    );
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Aged Egg":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "grenwogwarren",
                        UseableMonsters[3],
                        req.Name,
                        quant,
                        isTemp: false
                    );
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Liquid Egg":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "grenwogwarren",
                        UseableMonsters[4],
                        req.Name,
                        quant,
                        isTemp: false
                    );
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Cabdury Egg":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "grenwogwarren",
                        UseableMonsters[5],
                        req.Name,
                        quant,
                        isTemp: false
                    );
                    Bot.Wait.ForPickup(req.Name);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "92971",
            "Lagomorph TechWear",
            "Mode: [select] only\nShould the bot buy \"Lagomorph TechWear\" ?",
            false
        ),
        new Option<bool>(
            "92972",
            "Lagomorph Tech Morph",
            "Mode: [select] only\nShould the bot buy \"Lagomorph Tech Morph\" ?",
            false
        ),
        new Option<bool>(
            "92973",
            "Lagomorph Tech Ears Visage",
            "Mode: [select] only\nShould the bot buy \"Lagomorph Tech Ears Visage\" ?",
            false
        ),
        new Option<bool>(
            "92974",
            "Lagomorph Techmask",
            "Mode: [select] only\nShould the bot buy \"Lagomorph Techmask\" ?",
            false
        ),
        new Option<bool>(
            "92975",
            "Lagomorph Tech Visage",
            "Mode: [select] only\nShould the bot buy \"Lagomorph Tech Visage\" ?",
            false
        ),
        new Option<bool>(
            "92976",
            "Lagomorph Techwear Hair",
            "Mode: [select] only\nShould the bot buy \"Lagomorph Techwear Hair\" ?",
            false
        ),
        new Option<bool>(
            "92977",
            "Lagomorph Techwear Locks",
            "Mode: [select] only\nShould the bot buy \"Lagomorph Techwear Locks\" ?",
            false
        ),
        new Option<bool>(
            "92978",
            "Royal Graffiti Wings",
            "Mode: [select] only\nShould the bot buy \"Royal Graffiti Wings\" ?",
            false
        ),
        new Option<bool>(
            "92979",
            "Graffiti Angel Wings",
            "Mode: [select] only\nShould the bot buy \"Graffiti Angel Wings\" ?",
            false
        ),
        new Option<bool>(
            "92980",
            "Graffiti Crown",
            "Mode: [select] only\nShould the bot buy \"Graffiti Crown\" ?",
            false
        ),
        new Option<bool>(
            "92983",
            "Lagomorph Punk Hammer",
            "Mode: [select] only\nShould the bot buy \"Lagomorph Punk Hammer\" ?",
            false
        ),
        new Option<bool>(
            "92984",
            "Lagomorph Punk Hammers",
            "Mode: [select] only\nShould the bot buy \"Lagomorph Punk Hammers\" ?",
            false
        ),
    };
}
