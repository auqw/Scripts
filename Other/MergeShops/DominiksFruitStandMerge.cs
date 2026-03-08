/*
name: Dominiks Fruit Stand Merge
description: This bot will farm the items belonging to the selected mode for the Dominiks Fruit Stand Merge [2557] in /extinction
tags: dominiks, fruit, stand, merge, extinction, astro, seer, flame, morph, stellar, quasar, interstellar, riftbreaker, cosmic, traveler, chibi, fracture, companion, nebula, riftblade, singularity, guest
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Extinction.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class DominiksFruitStandMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private Extinction  extinction => new();


    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Lemon", "Lime" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        extinction.StoryLine();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("extinction", 2557, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Lime":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, req.Quantity))
                    {
                        Core.EnsureAccept(10585);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("Ectocave", "Ektorax", "Regurgitated Key");
                        Core.EquipClass(ClassType.Farm);
                        Core.KillMonster("ectocave", "r1", "Left", "*", "Ecto Slime", 50);
                        Core.EnsureComplete(10585);
                    }
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;


                case "Lemon":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(10054, new[] {
                        ("extinction","Lard",ClassType.Farm),
                        ("extinction","Gelatinous Slime",ClassType.Farm),
                        ("extinction","SN.O.W. Challenge",ClassType.Solo),
                    });
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("91637", "Astro Seer", "Mode: [select] only\nShould the bot buy \"Astro Seer\" ?", false),
        new Option<bool>("91638", "Astro Seer Flame Morph", "Mode: [select] only\nShould the bot buy \"Astro Seer Flame Morph\" ?", false),
        new Option<bool>("91639", "Astro Seer Visage", "Mode: [select] only\nShould the bot buy \"Astro Seer Visage\" ?", false),
        new Option<bool>("91640", "Astro Seer Morph", "Mode: [select] only\nShould the bot buy \"Astro Seer Morph\" ?", false),
        new Option<bool>("91641", "Astro Seer Flame Visage", "Mode: [select] only\nShould the bot buy \"Astro Seer Flame Visage\" ?", false),
        new Option<bool>("91642", "Stellar Quasar", "Mode: [select] only\nShould the bot buy \"Stellar Quasar\" ?", false),
        new Option<bool>("98965", "Interstellar Riftbreaker", "Mode: [select] only\nShould the bot buy \"Interstellar Riftbreaker\" ?", false),
        new Option<bool>("98966", "Interstellar Riftbreaker Hair", "Mode: [select] only\nShould the bot buy \"Interstellar Riftbreaker Hair\" ?", false),
        new Option<bool>("98967", "Interstellar Riftbreaker Locks", "Mode: [select] only\nShould the bot buy \"Interstellar Riftbreaker Locks\" ?", false),
        new Option<bool>("98968", "Interstellar Riftbreaker Morph", "Mode: [select] only\nShould the bot buy \"Interstellar Riftbreaker Morph\" ?", false),
        new Option<bool>("98969", "Interstellar Riftbreaker Visage", "Mode: [select] only\nShould the bot buy \"Interstellar Riftbreaker Visage\" ?", false),
        new Option<bool>("98970", "Cosmic Riftbreaker Morph", "Mode: [select] only\nShould the bot buy \"Cosmic Riftbreaker Morph\" ?", false),
        new Option<bool>("98971", "Cosmic Riftbreaker Visage", "Mode: [select] only\nShould the bot buy \"Cosmic Riftbreaker Visage\" ?", false),
        new Option<bool>("98975", "Interstellar Traveler Chibi", "Mode: [select] only\nShould the bot buy \"Interstellar Traveler Chibi\" ?", false),
        new Option<bool>("98976", "Astro Cosmic Fracture", "Mode: [select] only\nShould the bot buy \"Astro Cosmic Fracture\" ?", false),
        new Option<bool>("98977", "Interstellar Traveler Chibi Companion", "Mode: [select] only\nShould the bot buy \"Interstellar Traveler Chibi Companion\" ?", false),
        new Option<bool>("98978", "Astro Nebula Riftblade", "Mode: [select] only\nShould the bot buy \"Astro Nebula Riftblade\" ?", false),
        new Option<bool>("98980", "Astro Singularity Riftblade", "Mode: [select] only\nShould the bot buy \"Astro Singularity Riftblade\" ?", false),
        new Option<bool>("98988", "Interstellar Traveler Chibi Guest", "Mode: [select] only\nShould the bot buy \"Interstellar Traveler Chibi Guest\" ?", false),
   };
}
