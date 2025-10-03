/*
name: Bonecastle Tower Merge
description: This bot will farm the items belonging to the selected mode for the Bonecastle Tower Merge [1243] in /towersilver
tags: bonecastle, tower, merge, towersilver, deathknight, lord, deathknights, silver, golden
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class BonecastleTowerMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static CoreToD TOD { get => _TOD ??= new CoreToD(); set => _TOD = value; }
    private static CoreToD _TOD;


    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "DeathKnight Lord Gauntlets", "DeathKnight Lord Greaves", "DeathKnight Lord Chest Plate", "DeathKnight Lord Hauberk", "DeathKnight Lord Boots", "Bonecastle Amulet", "Silver DeathKnight Lord Gauntlets", "Silver DeathKnight Lord Greaves", "Silver DeathKnight Lord Chest Plate", "Silver DeathKnight Lord Hauberk", "Silver DeathKnight Lord Boots", "SilverSkull Amulet", "Golden DeathKnight Lord Gauntlets", "Golden DeathKnight Lord Greaves", "Golden DeathKnight Lord Chest Plate", "Golden DeathKnight Lord Hauberk", "Golden DeathKnight Lord Boots", "GoldSkull Amulet", "Shadow Skull" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        TOD.BoneTowerAll();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("towersilver", 1243, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Bonecastle Amulet":
                case "Shadow Skull":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(4993);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster("bonecastle", "Green Rat", "Gamey Rat Meat", 3);
                        Core.HuntMonster("bonecastle", "Undead Waiter", "Waiter's Notepad");
                        Core.HuntMonster("bonecastle", "Turtle", "Turtle's Eggs", 6);
                        Core.HuntMonster("bonecastle", "Ghoul", "Ghoul \"Vinegar\"", 6);
                        Core.HuntMonster("bonecastle", "Grateful Undead", "Spices", 2);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("bonecastle", "The Butcher", "Bag of Bone Flour");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "GoldSkull Amulet":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5023);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster("towergold", "Book Maggot", "Book Pages", 10);
                        Core.HuntMonster("towergold", "Vampire Bat", "Batwing Leather");
                        Core.HuntMonster("towergold", "Skullspider", "Skullspider Silk", 3);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "SilverSkull Amulet":
                    Core.RegisterQuests(5010);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("towersilver", "Bloody Scary", req.Name, req.Quantity, isTemp: req.Temp);
                    break;

                case "DeathKnight Lord Gauntlets":
                case "DeathKnight Lord Greaves":
                case "DeathKnight Lord Chest Plate":
                case "DeathKnight Lord Hauberk":
                case "DeathKnight Lord Boots":
                    Core.AddDrop(
                        "DeathKnight Lord Gauntlets",
                        "DeathKnight Lord Greaves",
                        "DeathKnight Lord Chest Plate",
                        "DeathKnight Lord Hauberk",
                        "DeathKnight Lord Boots"
                    );
                    Core.HuntMonster("bonecastle", "Vaden", req.Name, isTemp: false);
                    break;

                case "Silver DeathKnight Lord Gauntlets":
                case "Silver DeathKnight Lord Greaves":
                case "Silver DeathKnight Lord Chest Plate":
                case "Silver DeathKnight Lord Hauberk":
                case "Silver DeathKnight Lord Boots":
                    Core.AddDrop(
                          "Silver DeathKnight Lord Gauntlets",
                          "Silver DeathKnight Lord Greaves",
                          "Silver DeathKnight Lord Chest Plate",
                          "Silver DeathKnight Lord Hauberk",
                          "Silver DeathKnight Lord Boots",
                          "SilverSkull Amulet"
                      );
                    Core.HuntMonster("towersilver", "Flester the Silver", req.Name, isTemp: false);
                    break;

                case "Golden DeathKnight Lord Gauntlets":
                case "Golden DeathKnight Lord Greaves":
                case "Golden DeathKnight Lord Chest Plate":
                case "Golden DeathKnight Lord Hauberk":
                case "Golden DeathKnight Lord Boots":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }
                    Core.AddDrop(
                        "Golden DeathKnight Lord Gauntlets",
                        "Golden DeathKnight Lord Greaves",
                        "Golden DeathKnight Lord Chest Plate",
                        "Golden DeathKnight Lord Hauberk",
                        "Golden DeathKnight Lord Boots"
                    );
                    Core.HuntMonster("towergold", "Yurrod the Gold", req.Name, isTemp: false);
                    break;

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("34717", "DeathKnight Lord Armor", "Mode: [select] only\nShould the bot buy \"DeathKnight Lord Armor\" ?", false),
        new Option<bool>("34726", "DeathKnight's Blade", "Mode: [select] only\nShould the bot buy \"DeathKnight's Blade\" ?", false),
        new Option<bool>("34729", "DeathKnight Helm", "Mode: [select] only\nShould the bot buy \"DeathKnight Helm\" ?", false),
        new Option<bool>("34724", "Silver DeathKnight Lord", "Mode: [select] only\nShould the bot buy \"Silver DeathKnight Lord\" ?", false),
        new Option<bool>("34727", "Silver DeathKnight's Blade", "Mode: [select] only\nShould the bot buy \"Silver DeathKnight's Blade\" ?", false),
        new Option<bool>("34730", "Silver DeathKnight Helm", "Mode: [select] only\nShould the bot buy \"Silver DeathKnight Helm\" ?", false),
        new Option<bool>("34725", "Golden DeathKnight Lord", "Mode: [select] only\nShould the bot buy \"Golden DeathKnight Lord\" ?", false),
        new Option<bool>("34728", "Golden DeathKnight's Blade", "Mode: [select] only\nShould the bot buy \"Golden DeathKnight's Blade\" ?", false),
        new Option<bool>("34731", "Golden DeathKnight Helm", "Mode: [select] only\nShould the bot buy \"Golden DeathKnight Helm\" ?", false),
        new Option<bool>("34744", "DeathKnight Lord Cape", "Mode: [select] only\nShould the bot buy \"DeathKnight Lord Cape\" ?", false),
        new Option<bool>("34780", "DeathKnight Lord", "Mode: [select] only\nShould the bot buy \"DeathKnight Lord\" ?", false),
   };
}
