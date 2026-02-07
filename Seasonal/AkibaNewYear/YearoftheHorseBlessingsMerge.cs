/*
name: Year of the Horse Blessings Merge
description: This bot will farm the items belonging to the selected mode for the Year of the Horse Blessings Merge [2679] in /yokaihunt
tags: year, of, the, horse, blessings, merge, yokaihunt, passionate, qipao, royal, equine, vanguard, ancestral, dao, stallions, passion, fan, propserity, bamboo, ink, painting, lady, suzhens, bridge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Seasonal/AkibaNewYear/YokaiHunt.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class YearoftheHorseBlessingsMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static YokaiHunt YH { get => _YH ??= new YokaiHunt(); set => _YH = value; }
    private static YokaiHunt _YH;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Shi Wa Lam's Blessing", "Flaming Horseshoe", "Stallion's Warmth Fan", "Stallion's Vitality Fan" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        YH.ShiWaLam();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("yokaihunt", 2679, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Shi Wa Lam's Blessing":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(10589, "shadowbattleon", "Ouro Spawn");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;


                case "Flaming Horseshoe":
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
                        Core.HuntMonsterQuest(10590, "yokaihunt", "Red Hare");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;


                case "Stallion's Warmth Fan":
                case "Stallion's Vitality Fan":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("yokaihunt", "Red Hare", req.Name, quant, req.Temp, false);
                    break;
                #endregion

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("99140", "Passionate Qipao", "Mode: [select] only\nShould the bot buy \"Passionate Qipao\" ?", false),
        new Option<bool>("99141", "Royal Equine Vanguard", "Mode: [select] only\nShould the bot buy \"Royal Equine Vanguard\" ?", false),
        new Option<bool>("99142", "Royal Equine Vanguard Helm", "Mode: [select] only\nShould the bot buy \"Royal Equine Vanguard Helm\" ?", false),
        new Option<bool>("99143", "Ancestral Dao", "Mode: [select] only\nShould the bot buy \"Ancestral Dao\" ?", false),
        new Option<bool>("99144", "Dual Ancestral Dao", "Mode: [select] only\nShould the bot buy \"Dual Ancestral Dao\" ?", false),
        new Option<bool>("99176", "Stallion's Passion Fan", "Mode: [select] only\nShould the bot buy \"Stallion's Passion Fan\" ?", false),
        new Option<bool>("99178", "Stallion's Propserity Fan", "Mode: [select] only\nShould the bot buy \"Stallion's Propserity Fan\" ?", false),
        new Option<bool>("99185", "Bamboo Ink Painting", "Mode: [select] only\nShould the bot buy \"Bamboo Ink Painting\" ?", false),
        new Option<bool>("99186", "Lady Suzhen's Bridge", "Mode: [select] only\nShould the bot buy \"Lady Suzhen's Bridge\" ?", false),
   };
}
