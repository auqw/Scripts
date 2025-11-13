/*
name: AE25s Loot Merge
description: This bot will farm the items belonging to the selected mode for the AE25s Loot Merge [2575] in /twigguhunt
tags: ae25s, loot, merge, twigguhunt, fourth, guardian, balance, morph, goggles, eyewear, consular
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/MayThe4th/MurderMoonStory.cs
//cs_include Scripts/Seasonal/MayThe4th/TwigguHunt.cs
//cs_include Scripts/Seasonal/MayThe4th/AE25Story.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class AE25sLootMerge
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

    private static AE25Quests AE25
    {
        get => _AE25 ??= new AE25Quests();
        set => _AE25 = value;
    }
    private static AE25Quests _AE25;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Obliterator Droid's Generator", "Droid Scrap" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        AE25.Storyline();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("twigguhunt", 2575, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Obliterator Droid's Generator":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        if (Core.IsMember)
                            Core.HuntMonsterQuest(10229, "twigguhunt", "Obliterator Droid");
                        Core.HuntMonsterQuest(10228, "twigguhunt", "Obliterator Droid");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Droid Scrap":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(
                        "twigguhunt",
                        "r2",
                        "Down",
                        "*",
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "93302",
            "Fourth Guardian of Balance",
            "Mode: [select] only\nShould the bot buy \"Fourth Guardian of Balance\" ?",
            false
        ),
        new Option<bool>(
            "93303",
            "Fourth Guardian Morph",
            "Mode: [select] only\nShould the bot buy \"Fourth Guardian Morph\" ?",
            false
        ),
        new Option<bool>(
            "93304",
            "Fourth Guardian Visage",
            "Mode: [select] only\nShould the bot buy \"Fourth Guardian Visage\" ?",
            false
        ),
        new Option<bool>(
            "93305",
            "Fourth Guardian Goggles",
            "Mode: [select] only\nShould the bot buy \"Fourth Guardian Goggles\" ?",
            false
        ),
        new Option<bool>(
            "93306",
            "Fourth Guardian Eyewear",
            "Mode: [select] only\nShould the bot buy \"Fourth Guardian Eyewear\" ?",
            false
        ),
        new Option<bool>(
            "93307",
            "Fourth Guardian Cape",
            "Mode: [select] only\nShould the bot buy \"Fourth Guardian Cape\" ?",
            false
        ),
        new Option<bool>(
            "93312",
            "Fourth Consular of Balance",
            "Mode: [select] only\nShould the bot buy \"Fourth Consular of Balance\" ?",
            false
        ),
        new Option<bool>(
            "93313",
            "Fourth Consular Goggles",
            "Mode: [select] only\nShould the bot buy \"Fourth Consular Goggles\" ?",
            false
        ),
        new Option<bool>(
            "93314",
            "Fourth Consular Eyewear",
            "Mode: [select] only\nShould the bot buy \"Fourth Consular Eyewear\" ?",
            false
        ),
        new Option<bool>(
            "93315",
            "Fourth Consular Cape",
            "Mode: [select] only\nShould the bot buy \"Fourth Consular Cape\" ?",
            false
        ),
    };
}
