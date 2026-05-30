/*
name: Ouroboros Lost and Found Merge
description: This bot will farm the items belonging to the selected mode for the Ouroboros Lost and Found Merge [2716] in /forgecitrinitas
tags: ouroboros, lost, and, found, merge, forgecitrinitas, wolfblade, leader, eye, dn, doomed, obsession, gram, prototype, eckesachs, spear, discord
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class OuroborosLostandFoundMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static CoreAOR AOR
    {
        get => _AOR ??= new CoreAOR();
        set => _AOR = value;
    }
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
        Core.BankingBlackList.AddRange(new[] { "Nova Gemstone" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        AOR.ForgeCitrinitas();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("forgecitrinitas", 2716, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Nova Gemstone":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10735); // TODO: Replace with actual quest ID
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("forgecitrinitas", "Luma Lifeform", "Lifeform's Hope", 12, true, false);
                        Core.HuntMonster("forgecitrinitas", "Defense Droid", "Droid's Battery Acid", 9, true, false);
                        Core.HuntMonster("forgecitrinitas", "Clematis", "Clematis' Blood Sample", 1, true, false);
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
        new Option<bool>("101014", "WolfBlade Leader", "Mode: [select] only\nShould the bot buy \"WolfBlade Leader\" ?", false),
        new Option<bool>("84983", "Eye of 0D1N", "Mode: [select] only\nShould the bot buy \"Eye of 0D1N\" ?", false),
        new Option<bool>("88470", "Doomed Eye of Obsession", "Mode: [select] only\nShould the bot buy \"Doomed Eye of Obsession\" ?", false),
        new Option<bool>("101018", "WolfBlade Leader Cape", "Mode: [select] only\nShould the bot buy \"WolfBlade Leader Cape\" ?", false),
        new Option<bool>("84978", "Dual Gram Prototype", "Mode: [select] only\nShould the bot buy \"Dual Gram Prototype\" ?", false),
        new Option<bool>("84980", "Dual Eckesachs Prototype", "Mode: [select] only\nShould the bot buy \"Dual Eckesachs Prototype\" ?", false),
        new Option<bool>("101015", "WolfBlade Leader Hair", "Mode: [select] only\nShould the bot buy \"WolfBlade Leader Hair\" ?", false),
        new Option<bool>("101016", "WolfBlade Leader Locks", "Mode: [select] only\nShould the bot buy \"WolfBlade Leader Locks\" ?", false),
        new Option<bool>("101017", "WolfBlade Leader Visage", "Mode: [select] only\nShould the bot buy \"WolfBlade Leader Visage\" ?", false),
        new Option<bool>("88469", "Doomed Spear of Discord", "Mode: [select] only\nShould the bot buy \"Doomed Spear of Discord\" ?", false),
        new Option<bool>("84977", "Gram Prototype", "Mode: [select] only\nShould the bot buy \"Gram Prototype\" ?", false),
        new Option<bool>("84979", "Eckesachs Prototype", "Mode: [select] only\nShould the bot buy \"Eckesachs Prototype\" ?", false),
   };
}
