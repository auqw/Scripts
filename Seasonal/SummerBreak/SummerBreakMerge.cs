/*
name: Summer Break Merge
description: This bot will farm the items belonging to the selected mode for the Summer Break Merge [2155] in /summerbreak
tags: summer, break, merge, summerbreak, enchanted, volleyball, captain, hero, angelica, wetsuit, covenant, dark, halo, wings, buoyant, tail, , team, a, mascot, b, c, volleyballers, board, waterguns, foam, watergun, glasses, female, horns, hairband, morph, spear, pure, beach, rod, surfboard, nulgath, nation, undead, legion
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Legion/CoreLegion.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class SummerBreakMerge
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    public static CoreAdvanced sAdv
    {
        get => _sAdv ??= new CoreAdvanced();
        set => _sAdv = value;
    }
    public static CoreAdvanced _sAdv;

    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static CoreLegion Legion
    {
        get => _Legion ??= new CoreLegion();
        set => _Legion = value;
    }
    private static CoreLegion _Legion;


    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Summer Sizzle Lotion", "Volleyball Captain", "Volleyball Hero", "Dark Angelica Wetsuit", "Solar Orb", "Dark Covenant Wetsuit", "Volcanic Fragment", "Volleyball Team A Mascot", "Volleyball Team B Mascot", "Volleyball Team C Mascot", "Volleyball Hero's Board Cape", "Volleyball Hero's WaterGuns", "Volleyball Hero's Foam Gauntlet", "Volleyball Hero's WaterGun", "Volleyball Hero's Hat + Glasses", "Volleyball Heroine's Hat + Glasses", "Volleyball Hero's Glasses", "Volleyball Heroine's Locks", "Model Hero's Cut", "Model Hero's Locks", "Model Hero's Morph", "Model Hero's Visage", "Volleyball Hero's Foam Spear", "Charcoal Beach Ball", "Volleyball Hero's Rod", "Volleyball Hero's Surfboard", "Diamond of Nulgath", "Legion Token", "Blue Beach Ball" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("summerbreak", 2155, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Volleyball Captain":
                case "Volleyball Hero":
                case "Volleyball Hero's Hat":
                case "Volleyball Heroine's Hat":
                case "Volleyball Hero's Hat + Glasses":
                case "Volleyball Heroine's Hat + Glasses":
                case "Volleyball Hero's Glasses":
                case "Volleyball Team A Mascot":
                case "Volleyball Hero's Board Cape":
                case "Volleyball Team A Mascot Pet":
                case "Volleyball Hero's Rod":
                case "Volleyball Hero's Surfboard":
                case "Volleyball Hero's Foam Spear":
                case "Volleyball Hero's Foam Gauntlet":
                case "Volleyball Hero's WaterGun":
                case "Volleyball Hero's WaterGuns":
                case "Volleyball Hero's Hair":
                case "Volleyball Heroine's Locks":
                case "Volleyball Team B Mascot":
                case "Volleyball Team C Mascot":
                case "Volleyball Team B Mascot Pet":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);

                    Core.EnsureAccept(8794);
                    Core.HuntMonster("summerbreak", "MMMirage", "Gum Ball", 6);
                    Core.EnsureComplete(8794, req.ID);

                    Core.CancelRegisteredQuests();
                    break;

                case "Model Hero's Cut":
                case "Model Hero's Locks":
                case "Model Hero's Morph":
                case "Model Hero's Visage":
                    Core.BuyItem(Bot.Map.Name, 299, "Barber");
                    break;

                case "Summer Sizzle Lotion":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8794);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster("summerbreak", "MMMirage", "Gum Ball", 6);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Volcanic Fragment":
                case "Dark Angelica Wetsuit":
                case "Dark Covenant Wetsuit":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.KillMonster("lavarockbay", "r2", "Left", "*", req.Name, quant, false, false);
                    break;

                case "Solar Orb":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("eventhub", "Solar Elemental", req.Name, quant, req.Temp);
                    break;

                case "Diamond of Nulgath":
                    Nation.FarmDiamondofNulgath(quant);
                    break;

                case "Legion Token":
                    Legion.FarmLegionToken(quant);
                    break;

                #region fill these
                case "Charcoal Beach Ball":
                case "Blue Beach Ball":
                    Core.HuntMonster("summerbreak", "Cyborg Shark", req.Name, quant, req.Temp);
                    break;
                    #endregion

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("71099", "Enchanted Volleyball Captain", "Mode: [select] only\nShould the bot buy \"Enchanted Volleyball Captain\" ?", false),
        new Option<bool>("71100", "Enchanted Volleyball Hero", "Mode: [select] only\nShould the bot buy \"Enchanted Volleyball Hero\" ?", false),
        new Option<bool>("93938", "Angelica Wetsuit", "Mode: [select] only\nShould the bot buy \"Angelica Wetsuit\" ?", false),
        new Option<bool>("93954", "Covenant Wetsuit", "Mode: [select] only\nShould the bot buy \"Covenant Wetsuit\" ?", false),
        new Option<bool>("93949", "Dark Angelica Halo", "Mode: [select] only\nShould the bot buy \"Dark Angelica Halo\" ?", false),
        new Option<bool>("93950", "Dark Angelica Wings", "Mode: [select] only\nShould the bot buy \"Dark Angelica Wings\" ?", false),
        new Option<bool>("93963", "Buoyant Covenant Tail", "Mode: [select] only\nShould the bot buy \"Buoyant Covenant Tail\" ?", false),
        new Option<bool>("93964", "Buoyant Covenant Wings", "Mode: [select] only\nShould the bot buy \"Buoyant Covenant Wings\" ?", false),
        new Option<bool>("93965", "Buoyant Covenant Wings + Tail", "Mode: [select] only\nShould the bot buy \"Buoyant Covenant Wings + Tail\" ?", false),
        new Option<bool>("93973", "Dark Covenant Tail", "Mode: [select] only\nShould the bot buy \"Dark Covenant Tail\" ?", false),
        new Option<bool>("93974", "Dark Covenant Wings", "Mode: [select] only\nShould the bot buy \"Dark Covenant Wings\" ?", false),
        new Option<bool>("93975", "Dark Covenant Wings + Tail", "Mode: [select] only\nShould the bot buy \"Dark Covenant Wings + Tail\" ?", false),
        new Option<bool>("93944", "Buoyant Angelica Halo", "Mode: [select] only\nShould the bot buy \"Buoyant Angelica Halo\" ?", false),
        new Option<bool>("93945", "Buoyant Angelica Wings", "Mode: [select] only\nShould the bot buy \"Buoyant Angelica Wings\" ?", false),
        new Option<bool>("71105", "Enchanted Team A Mascot", "Mode: [select] only\nShould the bot buy \"Enchanted Team A Mascot\" ?", false),
        new Option<bool>("71106", "Enchanted Team B Mascot", "Mode: [select] only\nShould the bot buy \"Enchanted Team B Mascot\" ?", false),
        new Option<bool>("71107", "Enchanted Team C Mascot", "Mode: [select] only\nShould the bot buy \"Enchanted Team C Mascot\" ?", false),
        new Option<bool>("71108", "Enchanted Volleyballer's Board Cape", "Mode: [select] only\nShould the bot buy \"Enchanted Volleyballer's Board Cape\" ?", false),
        new Option<bool>("71114", "Enchanted Volleyballer's WaterGuns", "Mode: [select] only\nShould the bot buy \"Enchanted Volleyballer's WaterGuns\" ?", false),
        new Option<bool>("71112", "Enchanted Volleyballer's Foam Gauntlet", "Mode: [select] only\nShould the bot buy \"Enchanted Volleyballer's Foam Gauntlet\" ?", false),
        new Option<bool>("71113", "Enchanted Volleyballer's WaterGun", "Mode: [select] only\nShould the bot buy \"Enchanted Volleyballer's WaterGun\" ?", false),
        new Option<bool>("71103", "Enchanted Volleyballer's Hat + Glasses", "Mode: [select] only\nShould the bot buy \"Enchanted Volleyballer's Hat + Glasses\" ?", false),
        new Option<bool>("71104", "Enchanted Volleyballer's Female Hat + Glasses", "Mode: [select] only\nShould the bot buy \"Enchanted Volleyballer's Female Hat + Glasses\" ?", false),
        new Option<bool>("71101", "Enchanted Volleyballer's Hair", "Mode: [select] only\nShould the bot buy \"Enchanted Volleyballer's Hair\" ?", false),
        new Option<bool>("71102", "Enchanted Volleyballer's Locks", "Mode: [select] only\nShould the bot buy \"Enchanted Volleyballer's Locks\" ?", false),
        new Option<bool>("93957", "Buoyant Covenant Horns", "Mode: [select] only\nShould the bot buy \"Buoyant Covenant Horns\" ?", false),
        new Option<bool>("93958", "Buoyant Covenant Hairband", "Mode: [select] only\nShould the bot buy \"Buoyant Covenant Hairband\" ?", false),
        new Option<bool>("93961", "Buoyant Covenant Morph", "Mode: [select] only\nShould the bot buy \"Buoyant Covenant Morph\" ?", false),
        new Option<bool>("93962", "Buoyant Covenant Visage", "Mode: [select] only\nShould the bot buy \"Buoyant Covenant Visage\" ?", false),
        new Option<bool>("93969", "Dark Covenant Horns", "Mode: [select] only\nShould the bot buy \"Dark Covenant Horns\" ?", false),
        new Option<bool>("93970", "Dark Covenant Hairband", "Mode: [select] only\nShould the bot buy \"Dark Covenant Hairband\" ?", false),
        new Option<bool>("93971", "Dark Covenant Morph", "Mode: [select] only\nShould the bot buy \"Dark Covenant Morph\" ?", false),
        new Option<bool>("93972", "Dark Covenant Visage", "Mode: [select] only\nShould the bot buy \"Dark Covenant Visage\" ?", false),
        new Option<bool>("71111", "Enchanted Volleyballer's Foam Spear", "Mode: [select] only\nShould the bot buy \"Enchanted Volleyballer's Foam Spear\" ?", false),
        new Option<bool>("93953", "Pure Dark Beach Staff", "Mode: [select] only\nShould the bot buy \"Pure Dark Beach Staff\" ?", false),
        new Option<bool>("71109", "Enchanted Volleyballer's Rod", "Mode: [select] only\nShould the bot buy \"Enchanted Volleyballer's Rod\" ?", false),
        new Option<bool>("71110", "Enchanted Volleyballer's Surfboard", "Mode: [select] only\nShould the bot buy \"Enchanted Volleyballer's Surfboard\" ?", false),
        new Option<bool>("44211", "Surfboard of Nulgath Nation", "Mode: [select] only\nShould the bot buy \"Surfboard of Nulgath Nation\" ?", false),
        new Option<bool>("44210", "Surfboard of the Undead Legion", "Mode: [select] only\nShould the bot buy \"Surfboard of the Undead Legion\" ?", false),
        new Option<bool>("93948", "Pure Beach Staff", "Mode: [select] only\nShould the bot buy \"Pure Beach Staff\" ?", false),
   };
}
