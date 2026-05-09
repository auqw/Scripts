/*
name: Fort Luma Forge Merge
description: This bot will farm the items belonging to the selected mode for the Fort Luma Forge Merge [2684] in /fortluma
tags: fort, luma, forge, merge, fortluma, sol, gloria, proxima, imperator, nova, fomalhaut, luminary, white, knight, armet, lumiel, spinsaw, spinsaws, torch, celerity, gold, celeritas, statue, brasier, golden, geopetal, rubble
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs 
//cs_include Scripts/Other/MergeShops/CarcossaCanteenMerge.cs 
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class FortLumaForgeMerge
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
    private static CarcossaCanteenMerge CCM
    {
        get => _CCM ??= new CarcossaCanteenMerge();
        set => _CCM = value;
    }
    private static CarcossaCanteenMerge _CCM;


    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Yew Ember", "Yellow Flame of Citrinitas", "Luma Torch of Clarity", "Silver Celeritas Statue", "Geopetal Repair Glue" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        AOR.FortLuma();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("fortluma", 2684, findIngredients, buyOnlyThis, buyMode: buyMode);

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


                case "Silver Celeritas Statue":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonsterMapID("fortluma", 6, req.Name, req.Quantity, req.Temp);
                    Core.CancelRegisteredQuests();
                    break;

                case "Luma Torch of Clarity":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonsterMapID("fortluma", 5, req.Name, req.Quantity, req.Temp);
                    break;

                case "Yellow Flame of Citrinitas":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(Core.IsMember ? 10620 : 10619); // TODO: Replace with actual quest ID
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("fortluma", "Flame of Citrinitas", "Citrinitas Flicker");
                        Core.HuntMonster("fortluma", "Luma Dragon Twins", "Draconic Contrasoul");
                        Core.HuntMonsterMapID("fortluma", 10, "King's Yellow");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Geopetal Repair Glue":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("fortluma", "Luma Lifeform", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);

                    break;

                case "Yew Ember":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("warwickforest", "Rubedo Elemental", req.Name, quant, false);
                    break;
                case "Blade of Albedo":
                    CCM.BuyAllMerge(req.Name);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("97345", "Luminary White Knight", "Mode: [select] only\nShould the bot buy \"Luminary White Knight\" ?", false),
        new Option<bool>("100881", "Flame of Citrinitas", "Mode: [select] only\nShould the bot buy \"Flame of Citrinitas\" ?", false),
        new Option<bool>("97350", "Lumiel Spinsaw", "Mode: [select] only\nShould the bot buy \"Lumiel Spinsaw\" ?", false),
        new Option<bool>("93396", "Sol Imperator", "Mode: [select] only\nShould the bot buy \"Sol Imperator\" ?", false),
        new Option<bool>("100883", "Citrinitas Crown", "Mode: [select] only\nShould the bot buy \"Citrinitas Crown\" ?", false),
        new Option<bool>("100884", "Splendor Solace Cape", "Mode: [select] only\nShould the bot buy \"Splendor Solace Cape\" ?", false),
        new Option<bool>("100885", "Dawn's Splendor Cape", "Mode: [select] only\nShould the bot buy \"Dawn's Splendor Cape\" ?", false),
        new Option<bool>("94452", "Dual Nova Fomalhaut", "Mode: [select] only\nShould the bot buy \"Dual Nova Fomalhaut\" ?", false),
        new Option<bool>("93397", "Dual Sol Imperator", "Mode: [select] only\nShould the bot buy \"Dual Sol Imperator\" ?", false),
        new Option<bool>("93389", "Dual Sol Gloria", "Mode: [select] only\nShould the bot buy \"Dual Sol Gloria\" ?", false),
        new Option<bool>("97351", "Lumiel Spinsaws", "Mode: [select] only\nShould the bot buy \"Lumiel Spinsaws\" ?", false),
        new Option<bool>("99506", "Gold Celeritas Statue", "Mode: [select] only\nShould the bot buy \"Gold Celeritas Statue\" ?", false),
        new Option<bool>("99507", "Fort Luma Brasier", "Mode: [select] only\nShould the bot buy \"Fort Luma Brasier\" ?", false),
        new Option<bool>("99508", "Golden Geopetal Rubble", "Mode: [select] only\nShould the bot buy \"Golden Geopetal Rubble\" ?", false),
        new Option<bool>("99509", "Golden Geopetal", "Mode: [select] only\nShould the bot buy \"Golden Geopetal\" ?", false),
        new Option<bool>("100882", "Flame of Citrinitas Hood", "Mode: [select] only\nShould the bot buy \"Flame of Citrinitas Hood\" ?", false),
        new Option<bool>("97346", "Luminary White Armet", "Mode: [select] only\nShould the bot buy \"Luminary White Armet\" ?", false),
        new Option<bool>("99503", "Luma Torch of Celerity", "Mode: [select] only\nShould the bot buy \"Luma Torch of Celerity\" ?", false),
        new Option<bool>("93392", "Sol Proxima", "Mode: [select] only\nShould the bot buy \"Sol Proxima\" ?", false),
        new Option<bool>("93388", "Sol Gloria", "Mode: [select] only\nShould the bot buy \"Sol Gloria\" ?", false),
        new Option<bool>("94451", "Nova Fomalhaut", "Mode: [select] only\nShould the bot buy \"Nova Fomalhaut\" ?", false),
        new Option<bool>("100886", "Blade of Citrinitas", "Mode: [select] only\nShould the bot buy \"Blade of Citrinitas\" ?", false),
   };
}
