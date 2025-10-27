/*
name: Elodeas Candy Haul Merge
description: This bot will farm the items belonging to the selected mode for the Elodeas Candy Haul Merge [2632] in /eldritchworld
tags: elodeas, candy, haul, merge, eldritchworld, gold, voucher, k, horc, warrior, thok, magical, mariner, arcana, cat, burglar, metrea, thunder, flurry, juvania, aegean, calabaza, pumpkin, kings, twilight, peril, kabocha, killer, naginata, nocturne, penumbra
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/Mogloween/CoreMogloween.cs
//cs_include Scripts/Seasonal/Mogloween/MergeShops/UngourdlyGearMerge.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class ElodeasCandyHaulMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static CoreMogloween Mog { get => _CoreMogloween ??= new CoreMogloween(); set => _CoreMogloween = value; }
    private static CoreMogloween _CoreMogloween;
    private static UngourdlyGearMerge UGM { get => _Ungourdly ??= new UngourdlyGearMerge(); set => _Ungourdly = value; }
    private static UngourdlyGearMerge _Ungourdly;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Nudibranch Roe", "Barnacle Rash", "Shrimp Noodle Pack", "Dzeza Welcome Pack", "Dzeza Coconuts", "Desiccated Bulbs", "Head of Hair", "Bucket of Molars", "Thok's War Armor", "Arcana's Vesture", "Metrea's Garb", "Juvania's Robes", "Great Calabaza Blade", "Great Calabaza Blades", "Pumpkin King's Revenge", "Dual Pumpkin King's Revenge", "Pumpkin Fever Staff", "Kabocha King Naginata", "Pumpkin King's Wrath", "Dual Pumpkin King's Wrath", "Pumpkin Fever Axe", "Pumpkin Fever Axes", "Thok's Hair", "Arcana's Locks", "Metrea's Locks", "Juvania's Locks" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Mog.EldritchWorld();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("eldritchworld", 2632, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Nudibranch Roe":
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
                        Core.HuntMonsterQuest(10458, "eldritchworld", "Nudibranch");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;


                case "Barnacle Rash":
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
                        Core.HuntMonsterQuest(10459, "eldritchworld", "Infested Fisherman");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;


                case "Shrimp Noodle Pack":
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
                        Core.HuntMonsterQuest(10460, "eldritchworld", "Kathool Cultist");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;


                case "Dzeza Welcome Pack":
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
                        Core.HuntMonsterQuest(10461, "eldritchworld", "Dzeza Cultist");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;


                case "Dzeza Coconuts":
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
                        Core.HuntMonsterQuest(10462, "eldritchworld", "Dzeza Sapling");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;


                case "Desiccated Bulbs":
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
                        Core.HuntMonsterQuest(10463, "eldritchworld", "Infested Mummy");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;


                case "Head of Hair":
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
                        Core.HuntMonsterQuest(10468, "eldritchworld", "Mass of Hair");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;


                case "Bucket of Molars":
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
                        Core.HuntMonsterQuest(10464, "eldritchworld", "Dzeza Cultist");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;


                case "Thok's War Armor":
                case "Thok's Hair":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }
                    Adv.BuyItem("classhalla", 170, req.Name, quant);
                    break;


                case "Arcana's Vesture":
                case "Arcana's Locks":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Adv.BuyItem("classhalla", 174, req.Name, quant);
                    break;


                case "Metrea's Garb":
                case "Metrea's Locks":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Adv.BuyItem("classhalla", 172, req.Name, quant);
                    break;


                case "Juvania's Robes":
                case "Juvania's Locks":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Adv.BuyItem("classhalla", 176, req.Name, quant);
                    break;


                case "Great Calabaza Blade":
                case "Great Calabaza Blades":
                case "Pumpkin King's Revenge":
                case "Dual Pumpkin King's Revenge":
                case "Pumpkin Fever Staff":
                case "Kabocha King Naginata":
                case "Pumpkin King's Wrath":
                case "Dual Pumpkin King's Wrath":
                case "Pumpkin Fever Axe":
                case "Pumpkin Fever Axes":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    UGM.BuyAllMerge(req.Name);
                    break;

                    #endregion
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("96437", "Horc Warrior Thok", "Mode: [select] only\nShould the bot buy \"Horc Warrior Thok\" ?", false),
        new Option<bool>("96438", "Magical Mariner Arcana", "Mode: [select] only\nShould the bot buy \"Magical Mariner Arcana\" ?", false),
        new Option<bool>("96439", "Cat Burglar Metrea", "Mode: [select] only\nShould the bot buy \"Cat Burglar Metrea\" ?", false),
        new Option<bool>("96440", "Thunder Flurry Juvania", "Mode: [select] only\nShould the bot buy \"Thunder Flurry Juvania\" ?", false),
        new Option<bool>("89583", "Aegean Calabaza Blade", "Mode: [select] only\nShould the bot buy \"Aegean Calabaza Blade\" ?", false),
        new Option<bool>("89584", "Aegean Calabaza Blades", "Mode: [select] only\nShould the bot buy \"Aegean Calabaza Blades\" ?", false),
        new Option<bool>("89585", "Pumpkin King's Twilight", "Mode: [select] only\nShould the bot buy \"Pumpkin King's Twilight\" ?", false),
        new Option<bool>("89586", "Dual Pumpkin King's Twilight", "Mode: [select] only\nShould the bot buy \"Dual Pumpkin King's Twilight\" ?", false),
        new Option<bool>("89587", "Pumpkin Peril Staff", "Mode: [select] only\nShould the bot buy \"Pumpkin Peril Staff\" ?", false),
        new Option<bool>("89588", "Kabocha Killer Naginata", "Mode: [select] only\nShould the bot buy \"Kabocha Killer Naginata\" ?", false),
        new Option<bool>("89589", "Pumpkin King's Nocturne", "Mode: [select] only\nShould the bot buy \"Pumpkin King's Nocturne\" ?", false),
        new Option<bool>("89590", "Dual Pumpkin King's Nocturne", "Mode: [select] only\nShould the bot buy \"Dual Pumpkin King's Nocturne\" ?", false),
        new Option<bool>("89591", "Penumbra Pumpkin Axe", "Mode: [select] only\nShould the bot buy \"Penumbra Pumpkin Axe\" ?", false),
        new Option<bool>("89592", "Penumbra Pumpkin Axes", "Mode: [select] only\nShould the bot buy \"Penumbra Pumpkin Axes\" ?", false),
        new Option<bool>("96536", "Horc Warrior Thok Mask", "Mode: [select] only\nShould the bot buy \"Horc Warrior Thok Mask\" ?", false),
        new Option<bool>("96537", "Magical Mariner Arcana Mask", "Mode: [select] only\nShould the bot buy \"Magical Mariner Arcana Mask\" ?", false),
        new Option<bool>("96538", "Cat Burglar Metrea Mask", "Mode: [select] only\nShould the bot buy \"Cat Burglar Metrea Mask\" ?", false),
        new Option<bool>("96539", "Thunder Flurry Juvania Mask", "Mode: [select] only\nShould the bot buy \"Thunder Flurry Juvania Mask\" ?", false),
   };
}
