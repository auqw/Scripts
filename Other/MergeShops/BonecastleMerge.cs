/*
name: Bonecastle Merge
description: This bot will farm the items belonging to the selected mode for the Bonecastle Merge [1242] in /bonecastle
tags: bonecastle, merge, bonecastle, piercer, plate, face, accoutrements, tomb, tombstone, vadens, enchanted, deathknight, deathknights, royal, deathbringer, cloak, sabre, pile, bone, skulls, silver, golden, castle, house, lord
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class BonecastleMerge
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
        Core.BankingBlackList.AddRange(new[] { "Bonecastle Token", "Vaden Helm Token", "Shadow Skull", "DeathKnight Lord Armor" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("bonecastle", 1242, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                #region Known items

                case "Bonecastle Token":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("bonecastlec", "Undead Golden Knight", req.Name, quant, false);
                    break;

                case "Vaden Helm Token":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("bonecastlec", "Vaden", req.Name, quant, false);
                    break;

                case "Shadow Skull":
                    Core.RegisterQuests(4993);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, req.Quantity))
                    {
                        if (!Core.CheckInventory("Silver DeathKnight Lord"))
                            Core.BuyItem("towersilver", 1243, "Silver DeathKnight Lord");
                        if (!Core.CheckInventory("Golden DeathKnight Lord"))
                            Core.BuyItem("towergold", 1243, "Golden DeathKnight Lord");


                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster("bonecastle", "Green Rat", "Gamey Rat Meat", 3);
                        Core.HuntMonster("bonecastle", "Undead Waiter", "Waiter's Notepad", 1);
                        Core.HuntMonster("bonecastle", "Turtle", "Turtle's Eggs", 6);
                        Core.HuntMonster("bonecastle", "Ghoul", "Ghoul \"Vinegar\"", 6);
                        Core.HuntMonster("bonecastle", "Grateful Undead", "Spices", 2);

                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("bonecastle", "The Butcher", "Bag of Bone Flour", 1);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "DeathKnight Lord Armor":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }
                    string[] RequiredItems = new[]
                    {
"DeathKnight Lord Gauntlets",
"DeathKnight Lord Greaves",
"DeathKnight Lord Chest Plate",
"DeathKnight Lord Hauberk",
"DeathKnight Lord Boots"
};
                    foreach (string item in RequiredItems)
                        Core.HuntMonster("bonecastle", "Vaden", item, isTemp: false);


                    // BoneCastle Amulet
                    Core.RegisterQuests(4993);
                    while (!Bot.ShouldExit && !Core.CheckInventory("Bonecastle Amulet", 30))
                    {
                        if (!Core.CheckInventory("Silver DeathKnight Lord"))
                            Core.BuyItem("towersilver", 1243, "Silver DeathKnight Lord");
                        if (!Core.CheckInventory("Golden DeathKnight Lord"))
                            Core.BuyItem("towergold", 1243, "Golden DeathKnight Lord");


                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster("bonecastle", "Green Rat", "Gamey Rat Meat", 3);
                        Core.HuntMonster("bonecastle", "Undead Waiter", "Waiter's Notepad", 1);
                        Core.HuntMonster("bonecastle", "Turtle", "Turtle's Eggs", 6);
                        Core.HuntMonster("bonecastle", "Ghoul", "Ghoul \"Vinegar\"", 6);
                        Core.HuntMonster("bonecastle", "Grateful Undead", "Spices", 2);

                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("bonecastle", "The Butcher", "Bag of Bone Flour", 1);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    if (RequiredItems.All(x => Core.CheckInventory(x))
                    && Core.CheckInventory("Bonecastle Amulet", 30))
                    {
                        Core.BuyItem("bonecastle", 1242, req.Name);
                    }
                    Bot.Wait.ForItemBuy(req.ID);
                    break;
                    #endregion

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("34577", "The Piercer Plate", "Mode: [select] only\nShould the bot buy \"The Piercer Plate\" ?", false),
        new Option<bool>("34578", "Face Of The Piercer", "Mode: [select] only\nShould the bot buy \"Face Of The Piercer\" ?", false),
        new Option<bool>("34579", "Hood Of The Piercer", "Mode: [select] only\nShould the bot buy \"Hood Of The Piercer\" ?", false),
        new Option<bool>("34580", "The Piercer Accoutrements", "Mode: [select] only\nShould the bot buy \"The Piercer Accoutrements\" ?", false),
        new Option<bool>("34581", "Axe Of The Piercer", "Mode: [select] only\nShould the bot buy \"Axe Of The Piercer\" ?", false),
        new Option<bool>("34582", "Tomb of The Piercer", "Mode: [select] only\nShould the bot buy \"Tomb of The Piercer\" ?", false),
        new Option<bool>("34583", "The Piercer Tombstone", "Mode: [select] only\nShould the bot buy \"The Piercer Tombstone\" ?", false),
        new Option<bool>("34655", "Vaden's Helm", "Mode: [select] only\nShould the bot buy \"Vaden's Helm\" ?", false),
        new Option<bool>("34571", "Enchanted DeathKnight", "Mode: [select] only\nShould the bot buy \"Enchanted DeathKnight\" ?", false),
        new Option<bool>("34572", "Enchanted DeathKnight Plate", "Mode: [select] only\nShould the bot buy \"Enchanted DeathKnight Plate\" ?", false),
        new Option<bool>("34573", "Enchanted DeathKnight Cape", "Mode: [select] only\nShould the bot buy \"Enchanted DeathKnight Cape\" ?", false),
        new Option<bool>("34574", "Enchanted DeathKnight Axe", "Mode: [select] only\nShould the bot buy \"Enchanted DeathKnight Axe\" ?", false),
        new Option<bool>("34718", "DeathKnight's Hood", "Mode: [select] only\nShould the bot buy \"DeathKnight's Hood\" ?", false),
        new Option<bool>("34740", "Royal Deathbringer", "Mode: [select] only\nShould the bot buy \"Royal Deathbringer\" ?", false),
        new Option<bool>("34741", "Royal Deathbringer Cloak", "Mode: [select] only\nShould the bot buy \"Royal Deathbringer Cloak\" ?", false),
        new Option<bool>("34742", "Royal Deathbringer Helm", "Mode: [select] only\nShould the bot buy \"Royal Deathbringer Helm\" ?", false),
        new Option<bool>("34743", "Royal Deathbringer Sabre", "Mode: [select] only\nShould the bot buy \"Royal Deathbringer Sabre\" ?", false),
        new Option<bool>("34666", "Pile of Bone Skulls", "Mode: [select] only\nShould the bot buy \"Pile of Bone Skulls\" ?", false),
        new Option<bool>("34665", "Pile of Silver Skulls", "Mode: [select] only\nShould the bot buy \"Pile of Silver Skulls\" ?", false),
        new Option<bool>("34664", "Pile of Golden Skulls", "Mode: [select] only\nShould the bot buy \"Pile of Golden Skulls\" ?", false),
        new Option<bool>("34723", "Bone Castle House", "Mode: [select] only\nShould the bot buy \"Bone Castle House\" ?", false),
        new Option<bool>("34780", "DeathKnight Lord", "Mode: [select] only\nShould the bot buy \"DeathKnight Lord\" ?", false),
   };
}
