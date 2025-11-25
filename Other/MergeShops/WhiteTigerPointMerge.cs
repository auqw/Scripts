/*
name: WhiteTigerPoint Merge
description: This bot will farm the items belonging to the selected mode for the WhiteTigerPoint Merge [2600] in /whitetigerpoint
tags: whitetigerpoint, merge, whitetigerpoint, aureate, white, tiger, starbound, morph, cowl, starstruck, komokutens, star, spear, byakko, house
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Story/WhiteTigerPoint.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class WhiteTigerPointMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static Whitetigerpoint whitetigerpoint
    {
        get => _whitetigerpoint ??= new Whitetigerpoint();
        set => _whitetigerpoint = value;
    }
    private static Whitetigerpoint _whitetigerpoint;
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
            new[]
            {
                "Byakko's Stardust",
                "White Tiger Hair",
                "White Tiger Locks",
                "White Tiger Cape",
                "Byakko's Aura",
                "Komokuten's Star Wand",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        whitetigerpoint.DoStory();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge(
            "whitetigerpoint",
            2600,
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

                case "Byakko's Stardust":
                case "White Tiger Hair":
                case "White Tiger Locks":
                case "White Tiger Cape":
                case "Byakko's Aura":
                case "Komokuten's Star Wand":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("whitetigerpoint", "Byakko", req.Name, quant, req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
                #endregion
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "94631",
            "Aureate White Tiger",
            "Mode: [select] only\nShould the bot buy \"Aureate White Tiger\" ?",
            false
        ),
        new Option<bool>(
            "94632",
            "Starbound White Tiger",
            "Mode: [select] only\nShould the bot buy \"Starbound White Tiger\" ?",
            false
        ),
        new Option<bool>(
            "94633",
            "White Tiger Mask Morph",
            "Mode: [select] only\nShould the bot buy \"White Tiger Mask Morph\" ?",
            false
        ),
        new Option<bool>(
            "94634",
            "White Tiger Cowl Visage",
            "Mode: [select] only\nShould the bot buy \"White Tiger Cowl Visage\" ?",
            false
        ),
        new Option<bool>(
            "94635",
            "White Tiger Mask",
            "Mode: [select] only\nShould the bot buy \"White Tiger Mask\" ?",
            false
        ),
        new Option<bool>(
            "94636",
            "White Tiger Cowl",
            "Mode: [select] only\nShould the bot buy \"White Tiger Cowl\" ?",
            false
        ),
        new Option<bool>(
            "94638",
            "Starbound Tiger Cape",
            "Mode: [select] only\nShould the bot buy \"Starbound Tiger Cape\" ?",
            false
        ),
        new Option<bool>(
            "94639",
            "Starstruck Tiger Cape",
            "Mode: [select] only\nShould the bot buy \"Starstruck Tiger Cape\" ?",
            false
        ),
        new Option<bool>(
            "94640",
            "Komokuten's Star Spear",
            "Mode: [select] only\nShould the bot buy \"Komokuten's Star Spear\" ?",
            false
        ),
        new Option<bool>(
            "94641",
            "Byakko Morph",
            "Mode: [select] only\nShould the bot buy \"Byakko Morph\" ?",
            false
        ),
        new Option<bool>(
            "94645",
            "House Tiger Byakko",
            "Mode: [select] only\nShould the bot buy \"House Tiger Byakko\" ?",
            false
        ),
    };
}
