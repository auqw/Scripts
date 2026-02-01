/*
name: Terces Invasion Merge
description: This bot will farm the items belonging to the selected mode for the Terces Invasion Merge [2670] in /tercesinvasion
tags: terces, invasion, merge, tercesinvasion, nulgaths, oathbreaker, wretched, scolex, void, archfiend, vigneron, morph, apex, bloodfiend, wings, scythe, recollection, solstice, blood, rodeleros, fiendish, wind
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Nation/DeleuzeTundra.cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
//cs_include Scripts/Story/Nation/VoidChasm.cs
//cs_include Scripts/Story/Nation/OblivionTundra.cs
//cs_include Scripts/Story/Nation/tercesarchive.cs
//cs_include Scripts/Story/Nation/TercesInvasion.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/NationLoyaltyRewarded.cs
//cs_include Scripts/Good/BLOD/CoreBLOD.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Other/Classes/Necromancer.cs
//cs_include Scripts/Evil/NSoD/CoreNSOD.cs
//cs_include Scripts/Nation/Various/JuggernautItems.cs
//cs_include Scripts/Nation/MergeShops/NationMerge.cs
//cs_include Scripts/Other/MergeShops/TercesArchiveMerge.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleSiege.cs
//cs_include Scripts/Nation/Various/DragonBlade[mem].cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleSiegeMerge.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class TercesInvasionMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static CoreNation Nation { get => _Nation ??= new CoreNation(); set => _Nation = value; }
    private static CoreNation _Nation;
    private static TercesArchiveMerge TAM { get => _TAM ??= new TercesArchiveMerge(); set => _TAM = value; }
    private static TercesArchiveMerge _TAM;
    private static TempleSiegeMerge TSM { get => _TSM ??= new TempleSiegeMerge(); set => _TSM = value; }
    private static TempleSiegeMerge _TSM;
    private static JuggernautItemsofNulgath Jugg { get => _Jugg ??= new JuggernautItemsofNulgath(); set => _Jugg = value; }
    private static JuggernautItemsofNulgath _Jugg;
    private static TercesInvasion TI { get => _TI ??= new TercesInvasion(); set => _TI = value; }
    private static TercesInvasion _TI;


    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Wrath of Nulgath E", "Fiendish Fury of Ascension", "Evolved DragonFire of Nulgath", "Purified Claw of Nulgath", "Blade of Affliction", "Polish Hussar Spear", "Makai Bloodtaker", "Unidentified 13", "Blood Gem of the Archfiend", "Blood From the Void", "Solstice Blood Axe" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        TI.StoryLine();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("tercesinvasion", 2670, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Wrath of Nulgath E":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }
                    Farm.BludrutBrawlBoss(quant: quant * 350);
                    Adv.BuyItem("battleon", 222, req.Name, quant);
                    break;


                case "Fiendish Fury of Ascension":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }
                    TAM.BuyAllMerge(req.Name);
                    break;


                case "Evolved DragonFire of Nulgath":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }
                    TSM.BuyAllMerge(req.Name);
                    break;


                case "Purified Claw of Nulgath":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.EnsureAccept(668);
                    Nation.FarmTaintedGem(7);
                    Nation.Supplies("Claw of Nulgath");
                    Core.ResetQuest(7551);
                    Core.DarkMakaiItem("Dark Makai Sigil");
                    Core.EnsureComplete(668);
                    Bot.Wait.ForPickup(req.Name);
                    break;


                case "Polish Hussar Spear":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Jugg.JuggItems(JuggernautItemsofNulgath.RewardsSelection.Polish_Hussar_Spear);
                    break;


                case "Makai Bloodtaker":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(4008);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("tercessuinotlim", "Dark Makai");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;


                case "Blood From the Void":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(Core.IsMember ? 10583 : 10582, new[]
                        {
                            ("tercesinvasion","Archfiend Rodeleros", ClassType.Solo),
                            ("tercesinvasion","Archfiend Vigneron", ClassType.Solo),
                            ("tercesinvasion","Archfiend Casimir", ClassType.Solo)
                        });
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;


                case "Solstice Blood Axe":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("tercesinvasion", "Archfiend Vigneron", req.Name, quant, req.Temp, false);
                    break;
                #endregion

                #region Known items

                case "Blade of Affliction":
                    Core.FarmingLogger(req.Name, quant);
                    Core.BuyItem("Tercessuinotlim", 68, req.Name, quant);
                    break;

                case "Unidentified 13":
                    Nation.FarmUni13(quant);
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
        new Option<bool>("90517", "Nulgath's Oathbreaker", "Mode: [select] only\nShould the bot buy \"Nulgath's Oathbreaker\" ?", false),
        new Option<bool>("98493", "Wretched Scolex of the Void", "Mode: [select] only\nShould the bot buy \"Wretched Scolex of the Void\" ?", false),
        new Option<bool>("98907", "Archfiend Vigneron", "Mode: [select] only\nShould the bot buy \"Archfiend Vigneron\" ?", false),
        new Option<bool>("98908", "Archfiend Vigneron Morph", "Mode: [select] only\nShould the bot buy \"Archfiend Vigneron Morph\" ?", false),
        new Option<bool>("98909", "Apex Bloodfiend Wings", "Mode: [select] only\nShould the bot buy \"Apex Bloodfiend Wings\" ?", false),
        new Option<bool>("98999", "Scythe of Recollection", "Mode: [select] only\nShould the bot buy \"Scythe of Recollection\" ?", false),
        new Option<bool>("99002", "Solstice Blood Axes", "Mode: [select] only\nShould the bot buy \"Solstice Blood Axes\" ?", false),
        new Option<bool>("99003", "Archfiend Rodeleros", "Mode: [select] only\nShould the bot buy \"Archfiend Rodeleros\" ?", false),
        new Option<bool>("99004", "Archfiend Rodeleros Visage", "Mode: [select] only\nShould the bot buy \"Archfiend Rodeleros Visage\" ?", false),
        new Option<bool>("99005", "Fiendish Wind", "Mode: [select] only\nShould the bot buy \"Fiendish Wind\" ?", false),
   };
}
