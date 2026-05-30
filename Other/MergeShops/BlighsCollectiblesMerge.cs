/*
name: Blighs Collectibles Merge
description: This bot will farm the items belonging to the selected mode for the Blighs Collectibles Merge [2724] in /forgecitrinitas
tags: blighs, collectibles, merge, forgecitrinitas, syszeros, uniform, mech, guest, ebilgames, arcade, game, assault, mecha, server, ping, syszero, morph, astronomical, viewport
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

public class BlighsCollectiblesMerge
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
        Core.BankingBlackList.AddRange(new[] { "Gold Voucher 100k", "Nova Gemstone"});
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        AOR.ForgeCitrinitas();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("forgecitrinitas", 2724, findIngredients, buyOnlyThis, buyMode: buyMode);

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

#region Known items

case "Gold Voucher 100k":
                    Farm.Voucher(req.Name, req.Quantity);
                    break;
#endregion

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("98455", "Sys-Zero's Uniform", "Mode: [select] only\nShould the bot buy \"Sys-Zero's Uniform\" ?", false),
        new Option<bool>("101068", "Mech Guest", "Mode: [select] only\nShould the bot buy \"Mech Guest\" ?", false),
        new Option<bool>("101060", "EbilGames Arcade Game", "Mode: [select] only\nShould the bot buy \"EbilGames Arcade Game\" ?", false),
        new Option<bool>("101059", "Assault Mecha Arcade Game", "Mode: [select] only\nShould the bot buy \"Assault Mecha Arcade Game\" ?", false),
        new Option<bool>("101063", "Server Ping...", "Mode: [select] only\nShould the bot buy \"Server Ping...\" ?", false),
        new Option<bool>("98456", "Sys-Zero Morph", "Mode: [select] only\nShould the bot buy \"Sys-Zero Morph\" ?", false),
        new Option<bool>("98457", "Sys-Zero Hair", "Mode: [select] only\nShould the bot buy \"Sys-Zero Hair\" ?", false),
        new Option<bool>("101067", "Astronomical Viewport", "Mode: [select] only\nShould the bot buy \"Astronomical Viewport\" ?", false),
   };
}
