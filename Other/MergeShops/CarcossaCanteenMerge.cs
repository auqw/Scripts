/*
name: Carcossa Canteen Merge
description: This bot will farm the items belonging to the selected mode for the Carcossa Canteen Merge [2624] in /forgealbedo
tags: carcossa, canteen, merge, forgealbedo, knight, noelle, squire, morph, winged, capella, shrine, scorched, warlic, warlics, ponytail, bob
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class CarcossaCanteenMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static CoreAOR AOR { get => _AOR ??= new CoreAOR(); set => _AOR = value; }
    private static CoreAOR _AOR;


    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Yew Ember", "Mystic Topaz", "White Flame of Albedo", "Noelle Warden Morph", "Noelle Warden Visage", "Cape of the Noelle Knight", "Stars of Capella", "Noelle Warden Bob", "Noelle Warden Ponytail" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        AOR.ForgeAlbedo();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("forgealbedo", 2624, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "White Flame of Albedo":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(10423,
                        ("forgealbedo", "Defensive Turret", ClassType.Farm),
                        ("forgealbedo", "Defense Droid", ClassType.Farm),
                        ("forgealbedo", "Flame of Albedo", ClassType.Solo));
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;


                case "Noelle Warden Morph":
                case "Noelle Warden Visage":
                case "Cape of the Noelle Knight":
                case "Noelle Warden Bob":
                case "Noelle Warden Ponytail":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("thelimacity", "Noelle Knight", req.Name, quant, req.Temp, false);
                    break;


                case "Stars of Capella":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("forgealbedo", "Flame of Albedo", req.Name, quant, req.Temp, false);
                    break;

                #endregion
                #region Known items

                case "Yew Ember":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("thelimacity", "Maleno Match", req.Name, quant, false, false);
                    break;

                case "Mystic Topaz":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("thelimacity", "Noelle Knight", req.Name, quant, false, false);
                    break;
                    #endregion

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("94434", "Knight of Noelle", "Mode: [select] only\nShould the bot buy \"Knight of Noelle\" ?", false),
        new Option<bool>("94435", "Squire of Noelle", "Mode: [select] only\nShould the bot buy \"Squire of Noelle\" ?", false),
        new Option<bool>("94436", "Knight of Noelle Morph", "Mode: [select] only\nShould the bot buy \"Knight of Noelle Morph\" ?", false),
        new Option<bool>("94437", "Knight of Noelle Visage", "Mode: [select] only\nShould the bot buy \"Knight of Noelle Visage\" ?", false),
        new Option<bool>("94440", "Winged Noelle Morph", "Mode: [select] only\nShould the bot buy \"Winged Noelle Morph\" ?", false),
        new Option<bool>("94441", "Winged Noelle Visage", "Mode: [select] only\nShould the bot buy \"Winged Noelle Visage\" ?", false),
        new Option<bool>("94445", "Cape of Capella", "Mode: [select] only\nShould the bot buy \"Cape of Capella\" ?", false),
        new Option<bool>("94448", "Shrine of Capella", "Mode: [select] only\nShould the bot buy \"Shrine of Capella\" ?", false),
        new Option<bool>("95134", "Scorched Warlic", "Mode: [select] only\nShould the bot buy \"Scorched Warlic\" ?", false),
        new Option<bool>("95135", "Scorched Warlic Morph", "Mode: [select] only\nShould the bot buy \"Scorched Warlic Morph\" ?", false),
        new Option<bool>("95136", "Scorched Warlic Hair", "Mode: [select] only\nShould the bot buy \"Scorched Warlic Hair\" ?", false),
        new Option<bool>("95138", "Warlic's Scorched Staff", "Mode: [select] only\nShould the bot buy \"Warlic's Scorched Staff\" ?", false),
        new Option<bool>("95980", "Knight of Noelle Ponytail", "Mode: [select] only\nShould the bot buy \"Knight of Noelle Ponytail\" ?", false),
        new Option<bool>("95981", "Knight of Noelle Bob", "Mode: [select] only\nShould the bot buy \"Knight of Noelle Bob\" ?", false),
        new Option<bool>("95982", "Winged Noelle Ponytail", "Mode: [select] only\nShould the bot buy \"Winged Noelle Ponytail\" ?", false),
        new Option<bool>("95983", "Winged Noelle Bob", "Mode: [select] only\nShould the bot buy \"Winged Noelle Bob\" ?", false),
   };
}
