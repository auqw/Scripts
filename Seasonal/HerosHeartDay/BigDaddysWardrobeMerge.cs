/*
name: Big Daddys Wardrobe Merge
description: This bot will farm the items belonging to the selected mode for the Big Daddys Wardrobe Merge [2560] in /tunneloflove
tags: big, daddys, wardrobe, merge, tunneloflove, lovely, moonlighters, moonlighter, heart, hearts, arcane, rouge, cowl, morph, silphium, rapier, potion, rapiers, crystalized, alchemist, daddy, cherub
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/HerosHeartDay/TunnelOfLove.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class BigDaddysWardrobeMerge
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

    private static TunnelOfLove TOL
    {
        get => _TOL ??= new TunnelOfLove();
        set => _TOL = value;
    }
    private static TunnelOfLove _TOL;

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
                "Heart-Shaped Gem",
                "Lovely Laurel",
                "Burning Flame",
                "Moth-Spun Silk",
                "Pink Diamond",
                "Silphium Love Potions",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        if (!Core.isSeasonalMapActive("tunneloflove"))
            return;
        TOL.Storyline();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("tunneloflove", 2560, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Heart-Shaped Gem":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(
                        "tunneloflove",
                        "Love Knight",
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    break;

                case "Lovely Laurel":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(
                            10072,
                            ("tunneloflove", "Love Knight", ClassType.Farm),
                            ("tunneloflove", "Oubliette", ClassType.Solo),
                            ("tunneloflove", "Rosey Moth", ClassType.Farm)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Burning Flame":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("tunneloflove", "Galanoth", req.Name, quant, req.Temp, false);
                    break;

                case "Moth-Spun Silk":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(
                        "tunneloflove",
                        "Rosey Moth",
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    break;

                case "Pink Diamond":
                case "Silphium Love Potions":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("tunneloflove", "Oubliette", req.Name, quant, req.Temp, false);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "92011",
            "Lovely Moonlighters",
            "Mode: [select] only\nShould the bot buy \"Lovely Moonlighters\" ?",
            false
        ),
        new Option<bool>(
            "92010",
            "Lovely Moonlighter",
            "Mode: [select] only\nShould the bot buy \"Lovely Moonlighter\" ?",
            false
        ),
        new Option<bool>(
            "92006",
            "Heart of Hearts",
            "Mode: [select] only\nShould the bot buy \"Heart of Hearts\" ?",
            false
        ),
        new Option<bool>(
            "92005",
            "Arcane Rouge Cowl",
            "Mode: [select] only\nShould the bot buy \"Arcane Rouge Cowl\" ?",
            false
        ),
        new Option<bool>(
            "92004",
            "Arcane Rouge Hood",
            "Mode: [select] only\nShould the bot buy \"Arcane Rouge Hood\" ?",
            false
        ),
        new Option<bool>(
            "92003",
            "Arcane Rouge Visage",
            "Mode: [select] only\nShould the bot buy \"Arcane Rouge Visage\" ?",
            false
        ),
        new Option<bool>(
            "92002",
            "Arcane Rouge Morph",
            "Mode: [select] only\nShould the bot buy \"Arcane Rouge Morph\" ?",
            false
        ),
        new Option<bool>(
            "92001",
            "Arcane Rouge",
            "Mode: [select] only\nShould the bot buy \"Arcane Rouge\" ?",
            false
        ),
        new Option<bool>(
            "91382",
            "Silphium Rapier and Potion",
            "Mode: [select] only\nShould the bot buy \"Silphium Rapier and Potion\" ?",
            false
        ),
        new Option<bool>(
            "91379",
            "Silphium Rapiers",
            "Mode: [select] only\nShould the bot buy \"Silphium Rapiers\" ?",
            false
        ),
        new Option<bool>(
            "91378",
            "Silphium Rapier",
            "Mode: [select] only\nShould the bot buy \"Silphium Rapier\" ?",
            false
        ),
        new Option<bool>(
            "91377",
            "Crystalized Hearts",
            "Mode: [select] only\nShould the bot buy \"Crystalized Hearts\" ?",
            false
        ),
        new Option<bool>(
            "91374",
            "Silphium Alchemist",
            "Mode: [select] only\nShould the bot buy \"Silphium Alchemist\" ?",
            false
        ),
        new Option<bool>(
            "47386",
            "Big Daddy Cherub",
            "Mode: [select] only\nShould the bot buy \"Big Daddy Cherub\" ?",
            false
        ),
    };
}
