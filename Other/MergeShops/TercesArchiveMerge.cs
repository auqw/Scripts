/*
name: Terces Archive Merge
description: This bot will farm the items belonging to the selected mode for the Terces Archive Merge [2668] in /tercesarchive
tags: terces, archive, merge, tercesarchive, void, kittarian, morph, fiendish, feline, claws, fiend, voracity, overfiend, pet, battle, crimson, scolex, bane, ascension, fury
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/Various/JuggernautItems.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class TercesArchiveMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static JuggernautItemsofNulgath Jugger
    {
        get => _Jugger ??= new JuggernautItemsofNulgath();
        set => _Jugger = value;
    }
    private static JuggernautItemsofNulgath _Jugger;


    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Abyssal Fang", "Unidentified 13", "Diamond of Nulgath", "Dark Crystal Shard", "Tainted Gem", "Totem of Nulgath", "Gem of Nulgath", "Voucher of Nulgath (non-mem)", "Nulgath Armor", "Nulgath Horns", "Battlefiend Blade of Nulgath", "Voucher of Nulgath", "Overfiend Blade of Nulgath", "Blood Gem of the Archfiend", "Blood Star of the Archfiend" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("tercesarchive", 2668, findIngredients, buyOnlyThis, buyMode: buyMode);

        #region Dont edit this part
        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            int currentQuant = req.Temp ? Bot.TempInv.GetQuantity(req.Name) : Bot.Inventory.GetQuantity(req.Name);
            if (req == null)
            {
                Core.Logger("req is NULL");
                return;
            }

            switch (req.Name)
            {
                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger($"The bot hasn't been taught how to get {req.Name}." + (shouldStop ? " Please report the issue." : " Skipping"), messageBox: shouldStop, stopBot: shouldStop);
                    break;
        #endregion

                #region Items not setup
                case "Nulgath Horns":
                case "Battlefiend Blade of Nulgath":
                case "Blood Star of the Archfiend":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("tercesarchive", "Fiend of Voracity", req.Name, req.Quantity, req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                #endregion

                case "Abyssal Fang":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EnsureAccept(Core.IsMember ? 10559 : 10558);
                        Core.KillMonster("tercesarchive", "r7", "Bottom", "Fiend of Voracity", "Voracious Appetite", 1);
                        Core.KillMonster("tercesarchive", "r6", "Center", "*", "Double Iris", 6);
                        Core.KillMonster("tercesarchive", "r5", "Left", "*", "Twisted Vision", 6);
                        Core.EnsureComplete(Core.IsMember ? 10559 : 10558);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;


                #region Known items

                case "Unidentified 13":
                    Nation.FarmUni13(quant);
                    break;

                case "Diamond of Nulgath":
                    Nation.FarmDiamondofNulgath(quant);
                    break;

                case "Dark Crystal Shard":
                    Nation.FarmDarkCrystalShard(quant);
                    break;

                case "Tainted Gem":
                    Nation.FarmTaintedGem(quant);
                    break;

                case "Totem of Nulgath":
                    Nation.FarmTotemofNulgath(quant);
                    break;

                case "Gem of Nulgath":
                    Nation.FarmGemofNulgath(quant);
                    break;

                case "Voucher of Nulgath (non-mem)":
                    Nation.FarmVoucher(false);
                    break;

                case "Voucher of Nulgath":
                    Nation.FarmVoucher(true, true);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Nulgath Armor":
                    Core.FarmingLogger(req.Name, quant);
                    Jugger.JuggItems(JuggernautItemsofNulgath.RewardsSelection.Nulgath_Armor);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Overfiend Blade of Nulgath":
                    Core.FarmingLogger(req.Name, quant);
                    Jugger.JuggItems(JuggernautItemsofNulgath.RewardsSelection.Overfiend_Blade_of_Nulgath);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Blood Gem of the Archfiend":
                    Nation.FarmBloodGem(quant);
                    break;
                #endregion

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("75932", "Void Kittarian", "Mode: [select] only\nShould the bot buy \"Void Kittarian\" ?", false),
        new Option<bool>("75933", "Void Kittarian Morph", "Mode: [select] only\nShould the bot buy \"Void Kittarian Morph\" ?", false),
        new Option<bool>("75934", "Fiendish Feline Claws", "Mode: [select] only\nShould the bot buy \"Fiendish Feline Claws\" ?", false),
        new Option<bool>("98448", "Fiend Of Voracity", "Mode: [select] only\nShould the bot buy \"Fiend Of Voracity\" ?", false),
        new Option<bool>("98449", "Fiend of Voracity Morph", "Mode: [select] only\nShould the bot buy \"Fiend of Voracity Morph\" ?", false),
        new Option<bool>("98450", "Overfiend Blade of Voracity Pet", "Mode: [select] only\nShould the bot buy \"Overfiend Blade of Voracity Pet\" ?", false),
        new Option<bool>("98451", "Overfiend Blade of Voracity Battle Pet", "Mode: [select] only\nShould the bot buy \"Overfiend Blade of Voracity Battle Pet\" ?", false),
        new Option<bool>("98452", "Overfiend Blade of Voracity", "Mode: [select] only\nShould the bot buy \"Overfiend Blade of Voracity\" ?", false),
        new Option<bool>("98453", "Overfiend Blades of Voracity", "Mode: [select] only\nShould the bot buy \"Overfiend Blades of Voracity\" ?", false),
        new Option<bool>("98488", "Crimson Scolex of the Void", "Mode: [select] only\nShould the bot buy \"Crimson Scolex of the Void\" ?", false),
        new Option<bool>("98489", "Fiendish Bane of Ascension", "Mode: [select] only\nShould the bot buy \"Fiendish Bane of Ascension\" ?", false),
        new Option<bool>("98490", "Fiendish Fury of Ascension", "Mode: [select] only\nShould the bot buy \"Fiendish Fury of Ascension\" ?", false),
   };
}
