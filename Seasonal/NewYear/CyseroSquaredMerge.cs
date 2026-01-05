/*
name: Cysero Squared Merge
description: This bot will farm the items belonging to the selected mode for the Cysero Squared Merge [2653] in /cyseroparadox
tags: cysero, squared, merge, cyseroparadox, ducky, earmuffs, morph, mad, chronosmith, chronosmiths, tophat, paradox, enigma, distorqueo, hammer, pieradox
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class CyseroSquaredMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
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
        Core.BankingBlackList.AddRange(new[] { "Temporal Sock Fiber", "Slice of Time Pie-radox", "Pair of Pie-radox Slices" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("cyseroparadox", 2653, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Temporal Sock Fiber":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }
                    Core.EquipClass(ClassType.Solo);
                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("cyseroparadox", "Sys-Zero's Mech", log: false);
                        Core.HuntMonsterQuest(Core.CheckInventory("Star Captain") ? 10543 : Core.IsMember ? 10542 : 10541, "cyseroparadox", "Sys-Zero");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;


                case "Slice of Time Pie-radox":
                case "Pair of Pie-radox Slices":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);

                    Core.KillMonster("cyseroparadox", "r2", "Bottom", 2, 1, req.Name, quant, req.Temp, false);
                    break;
                #endregion

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("97874", "Ducky Earmuffs Morph", "Mode: [select] only\nShould the bot buy \"Ducky Earmuffs Morph\" ?", false),
        new Option<bool>("97875", "Ducky Earmuffs Visage", "Mode: [select] only\nShould the bot buy \"Ducky Earmuffs Visage\" ?", false),
        new Option<bool>("98233", "Mad Chronosmith", "Mode: [select] only\nShould the bot buy \"Mad Chronosmith\" ?", false),
        new Option<bool>("98234", "Chronosmith's Tophat Morph", "Mode: [select] only\nShould the bot buy \"Chronosmith's Tophat Morph\" ?", false),
        new Option<bool>("98235", "Chronosmith's Tophat Visage", "Mode: [select] only\nShould the bot buy \"Chronosmith's Tophat Visage\" ?", false),
        new Option<bool>("98236", "Chronosmith's Morph", "Mode: [select] only\nShould the bot buy \"Chronosmith's Morph\" ?", false),
        new Option<bool>("98237", "Chronosmith's Visage", "Mode: [select] only\nShould the bot buy \"Chronosmith's Visage\" ?", false),
        new Option<bool>("98238", "Chronosmith's Hair", "Mode: [select] only\nShould the bot buy \"Chronosmith's Hair\" ?", false),
        new Option<bool>("98239", "Chronosmith's Locks", "Mode: [select] only\nShould the bot buy \"Chronosmith's Locks\" ?", false),
        new Option<bool>("98240", "Chronosmith's Paradox", "Mode: [select] only\nShould the bot buy \"Chronosmith's Paradox\" ?", false),
        new Option<bool>("98241", "Chronosmith's Enigma", "Mode: [select] only\nShould the bot buy \"Chronosmith's Enigma\" ?", false),
        new Option<bool>("98242", "Distorqueo Hammer", "Mode: [select] only\nShould the bot buy \"Distorqueo Hammer\" ?", false),
        new Option<bool>("98316", "Pie-radox", "Mode: [select] only\nShould the bot buy \"Pie-radox\" ?", false),
   };
}
