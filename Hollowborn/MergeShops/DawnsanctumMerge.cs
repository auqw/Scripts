/*
name: Dawnsanctum Merge
description: This bot will farm the items belonging to the selected mode for the Dawnsanctum Merge [2505] in /dawnsanctum
tags: dawnsanctum, merge, dawnsanctum, gramiels, celestial, enoch, enochs, glorified, hollowborn, draconian, hollow, beast, tamer, claws, vindicated, assassin, dirk, soldier, dawn, priest, tome, grace, texts, spellbooks, grimoires
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
//cs_include Scripts/Story/Hollowborn/CoreHollowbornStory.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Hollowborn/Materials/HollowSoul.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Shops;
using Skua.Core.Options;

public class DawnsanctumMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv
    {
        get => _sAdv ??= new CoreAdvanced();
        set => _sAdv = value;
    }
    private static CoreAdvanced _sAdv;

    private static CoreHollowbornStory HBS
    {
        get => _HBS ??= new CoreHollowbornStory();
        set => _HBS = value;
    }
    private static CoreHollowbornStory _HBS;
    private static HollowSoul HS
    {
        get => _HS ??= new HollowSoul();
        set => _HS = value;
    }
    private static HollowSoul _HS;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(
            new[]
            {
                "Vindicator Badge",
                "Grace Orb",
                "Vindicator Crest",
                "Gramiel's Emblem",
                "Gramiel's Shattered Enoch",
                "Gramiel's Shattered Enochs",
                "Hollow Soul",
                "Vindicator Draconian",
                "Hollowborn Draconian Morph",
                "Draconian Vindication Axe",
                "Draconian Vindication Axes",
                "Vindicator Beast Tamer",
                "Vindicator Beast Tamer Mask",
                "Vindicator Beast Tamer Hood",
                "Vindicator Beast Tamer Claws",
                "Vindicator Assassin",
                "Vindicator Assassin Mask",
                "Vindicator Assassin Hood",
                "Vindicator Assassin Dirk",
                "Vindicator Assassin Daggers",
                "Dawn Vindicator Soldier",
                "Dawn Vindicator Helm",
                "Dawn Vindicator Sword",
                "Dawn Vindicator Swords",
                "Vindicator Priest",
                "Vindicator Priest Mask",
                "Vindicator Priest Hood",
                "Dawn Vindication Tome",
                "Dawn Vindication Grace Texts",
                "Dawn Vindication Spellbooks",
                "Dawn Vindication Grimoires",
                "Vindicator Priest Staff",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        HBS.DawnSanctum();

        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("dawnsanctum", 2505, findIngredients, buyOnlyThis, buyMode: buyMode);

        #region Dont edit this part
        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            int currentQuant = req.Temp
                ? Bot.TempInv.GetQuantity(req.Name)
                : Bot.Inventory.GetQuantity(req.Name);
            if (req == null)
            {
                Core.Logger("req is NULL");
                return;
            }

            switch (req.Name)
            {
                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger(
                        $"The bot hasn't been taught how to get {req.Name}."
                            + (shouldStop ? " Please report the issue." : " Skipping"),
                        messageBox: shouldStop,
                        stopBot: shouldStop
                    );
                    break;
        #endregion

                case "Vindicator Badge":
                    VindicatorBadge(quant);
                    break;

                case "Grace Orb":
                    GraceOrb(quant);
                    break;

                case "Vindicator Crest":
                    VindicatorCrest(quant);
                    break;

                case "Gramiel's Emblem":
                case "Gramiel's Shattered Enoch":
                case "Gramiel's Shattered Enochs":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "dawnsanctum",
                        "Celestial Gramiel",
                        req.Name,
                        quant,
                        isTemp: false
                    );
                    break;

                case "Hollow Soul":
                    HS.GetYaSoulsHeeeere(quant);
                    break;

                case "Vindicator Draconian":
                case "Draconian Vindication Axe":
                case "Draconian Vindication Axes":
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(
                        "dawnsanctum",
                        "r7",
                        "Left",
                        "Vindicator Draconian",
                        req.Name,
                        quant,
                        isTemp: false
                    );
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Hollowborn Draconian Morph":
                    Core.KillMonster(
                        "dawnsanctum",
                        "r8",
                        "Left",
                        "Hollowborn Draconian",
                        req.Name,
                        isTemp: false
                    );
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Dawn Vindicator Helm":
                case "Dawn Vindicator Sword":
                case "Dawn Vindicator Swords":
                case "Dawn Vindicator Soldier":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    switch (req.Name)
                    {
                        case "Dawn Vindicator Helm":
                        case "Dawn Vindicator Soldier":
                            Core.HuntMonster(
                                "trygve",
                                "Vindicator Recruit",
                                req.Name,
                                isTemp: false
                            );
                            break;

                        case "Dawn Vindicator Sword":
                            Core.HuntMonster(
                                "trygve",
                                "Vindicator Soldier",
                                req.Name,
                                isTemp: false
                            );
                            break;

                        case "Dawn Vindicator Swords":
                            Core.HuntMonster(
                                "trygve",
                                "Vindicator Recruit",
                                req.Name,
                                isTemp: false
                            );
                            break;
                    }
                    break;

                case "Vindicator Priest":
                case "Vindicator Beast Tamer":
                case "Vindicator Assassin":
                    VindicatorCrest(20);
                    Adv.BuyItem("neotower", 2474, req.Name);
                    break;

                case "Vindicator Assassin Mask":
                case "Vindicator Beast Tamer Hood":
                case "Vindicator Beast Tamer Mask":
                case "Vindicator Priest Mask":
                    VindicatorCrest(10);
                    Adv.BuyItem("neotower", 2474, req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Vindicator Assassin Hood":
                case "Vindicator Assassin Dirk":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID("neotower", 12, req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Vindicator Beast Tamer Claws":
                case "Vindicator Priest Staff":
                case "Vindicator Assassin Daggers":
                    VindicatorCrest(15);
                    Adv.BuyItem("neotower", 2474, req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Dawn Vindication Grace Texts":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID("neotower", 28, "Dawn Vindication Tome", isTemp: false);
                    Core.HuntMonsterMapID(
                        "neotower",
                        28,
                        "Dawn Vindication Spellbooks",
                        isTemp: false
                    );
                    Core.HuntMonsterMapID(
                        "neotower",
                        28,
                        "Dawn Vindication Grimoires",
                        isTemp: false
                    );
                    Adv.BuyItem("neotower", 2474, req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Dawn Vindication Tome":
                case "Dawn Vindication Spellbooks":
                case "Dawn Vindication Grimoires":
                case "Vindicator Priest Hood":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID("neotower", 28, req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
            }
        }

        #region Farm Area
        void VindicatorCrest(int quant)
        {
            if (Core.CheckInventory("Vindicator Crest", quant))
                return;

            Core.FarmingLogger("Vindicator Crest", quant);
            Core.AddDrop("Vindicator Crest");
            Core.EquipClass(ClassType.Farm);
            Core.RegisterQuests(9865);
            while (!Bot.ShouldExit && !Core.CheckInventory("Vindicator Crest", quant))
            {
                Core.HuntMonsterMapID("neotower", 12, "Vindicated Blades");
                Core.HuntMonsterMapID("neotower", 17, "Vindicated Chain");
                Core.HuntMonsterMapID("neotower", 28, "Vindicated Scripture");
                Bot.Wait.ForPickup("Vindicator Crest");
            }
            Core.CancelRegisteredQuests();
        }

        void VindicatorBadge(int quant)
        {
            if (Core.CheckInventory("Vindicator Badge", quant))
                return;

            Core.FarmingLogger("Vindicator Badge", quant);
            Bot.Quests.UpdateQuest(8297);
            Core.RegisterQuests(8299);
            while (!Bot.ShouldExit && !Core.CheckInventory("Vindicator Badge", quant))
            {
                Core.EquipClass(ClassType.Farm);
                Core.KillMonster("trygve", "r3", "Left", "Blood Eagle", "Eagle Heart", 8);
                Core.KillMonster("trygve", "r4", "Left", "Rune Boar", "Boar Heart", 8);
                Core.EquipClass(ClassType.Solo);
                Core.HuntMonster("trygve", "Gramiel", "Vindicator Seal");
                Bot.Wait.ForPickup("Vindicator Badge");
            }
            Core.CancelRegisteredQuests();
        }

        // void GramielsEmblem(int quant)
        // {
        //     if (Core.CheckInventory("Gramiel's Emblem", quant))
        //         return;

        //     Core.FarmingLogger("Gramiel's Emblem", quant);
        //     Core.AddDrop("Gramiel's Emblem");
        //     Core.EquipClass(ClassType.Solo);

        //     Core.HuntMonster("dawnsanctum", "Celestial Gramiel", "Gramiel's Emblem", quant, isTemp: false);
        //     Bot.Wait.ForPickup("Gramiel's Emblem");
        // }

        void GraceOrb(int quant)
        {
            Core.FarmingLogger("Grace Orb", quant);
            Core.EquipClass(ClassType.Farm);
            Core.RegisterQuests(9291);
            while (!Bot.ShouldExit && !Core.CheckInventory("Grace Orb", quant))
            {
                Core.HuntMonster(
                    "neofortress",
                    "Vindicator Recruit",
                    "Grace Extracted",
                    20,
                    false,
                    false
                );
                Bot.Wait.ForPickup("Grace Orb");
            }
            Core.CancelRegisteredQuests();
        }

        // bool IsShopLoaded(string? mapName, string? shopName)
        // {
        //     return Bot.Map.Name == mapName && Bot.Shops.IsLoaded && Bot.Shops.Name == shopName;
        // }

        #endregion Farm Area
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "89889",
            "Gramiel's Celestial Enoch",
            "Mode: [select] only\nShould the bot buy \"Gramiel's Celestial Enoch\" ?",
            false
        ),
        new Option<bool>(
            "89890",
            "Gramiel's Celestial Enochs",
            "Mode: [select] only\nShould the bot buy \"Gramiel's Celestial Enochs\" ?",
            false
        ),
        new Option<bool>(
            "89887",
            "Gramiel's Glorified Enoch",
            "Mode: [select] only\nShould the bot buy \"Gramiel's Glorified Enoch\" ?",
            false
        ),
        new Option<bool>(
            "89888",
            "Gramiel's Glorified Enochs",
            "Mode: [select] only\nShould the bot buy \"Gramiel's Glorified Enochs\" ?",
            false
        ),
        new Option<bool>(
            "89898",
            "Hollowborn Draconian",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Draconian\" ?",
            false
        ),
        new Option<bool>(
            "89900",
            "Hollowborn Draconian Mask",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Draconian Mask\" ?",
            false
        ),
        new Option<bool>(
            "89901",
            "Hollowborn Draconian Helm",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Draconian Helm\" ?",
            false
        ),
        new Option<bool>(
            "89902",
            "Hollow Draconian Axe",
            "Mode: [select] only\nShould the bot buy \"Hollow Draconian Axe\" ?",
            false
        ),
        new Option<bool>(
            "89903",
            "Hollow Draconian Axes",
            "Mode: [select] only\nShould the bot buy \"Hollow Draconian Axes\" ?",
            false
        ),
        new Option<bool>(
            "89904",
            "Hollowborn Beast Tamer",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Beast Tamer\" ?",
            false
        ),
        new Option<bool>(
            "89905",
            "Hollowborn Beast Tamer Mask",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Beast Tamer Mask\" ?",
            false
        ),
        new Option<bool>(
            "89906",
            "Hollowborn Beast Tamer Hood",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Beast Tamer Hood\" ?",
            false
        ),
        new Option<bool>(
            "89908",
            "Hollowborn Beast Tamer Claws",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Beast Tamer Claws\" ?",
            false
        ),
        new Option<bool>(
            "89909",
            "Vindicated Hollowborn Assassin",
            "Mode: [select] only\nShould the bot buy \"Vindicated Hollowborn Assassin\" ?",
            false
        ),
        new Option<bool>(
            "89910",
            "Hollowborn Assassin Mask",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Assassin Mask\" ?",
            false
        ),
        new Option<bool>(
            "89911",
            "Hollowborn Assassin Hood",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Assassin Hood\" ?",
            false
        ),
        new Option<bool>(
            "89912",
            "Hollowborn Assassin Dirk",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Assassin Dirk\" ?",
            false
        ),
        new Option<bool>(
            "89913",
            "Hollowborn Assassin Daggers",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Assassin Daggers\" ?",
            false
        ),
        new Option<bool>(
            "89914",
            "Vindicated Hollowborn Soldier",
            "Mode: [select] only\nShould the bot buy \"Vindicated Hollowborn Soldier\" ?",
            false
        ),
        new Option<bool>(
            "89915",
            "Hollowborn Soldier Helm",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Soldier Helm\" ?",
            false
        ),
        new Option<bool>(
            "89916",
            "Vindicated Dawn Blade",
            "Mode: [select] only\nShould the bot buy \"Vindicated Dawn Blade\" ?",
            false
        ),
        new Option<bool>(
            "89917",
            "Vindicated Dawn Blades",
            "Mode: [select] only\nShould the bot buy \"Vindicated Dawn Blades\" ?",
            false
        ),
        new Option<bool>(
            "89918",
            "Vindicated Hollowborn Priest",
            "Mode: [select] only\nShould the bot buy \"Vindicated Hollowborn Priest\" ?",
            false
        ),
        new Option<bool>(
            "89919",
            "Vindicated Hollowborn Priest Mask",
            "Mode: [select] only\nShould the bot buy \"Vindicated Hollowborn Priest Mask\" ?",
            false
        ),
        new Option<bool>(
            "89920",
            "Vindicated Hollowborn Priest Hood",
            "Mode: [select] only\nShould the bot buy \"Vindicated Hollowborn Priest Hood\" ?",
            false
        ),
        new Option<bool>(
            "89921",
            "Hollow Dawn Tome",
            "Mode: [select] only\nShould the bot buy \"Hollow Dawn Tome\" ?",
            false
        ),
        new Option<bool>(
            "89922",
            "Hollow Grace Texts",
            "Mode: [select] only\nShould the bot buy \"Hollow Grace Texts\" ?",
            false
        ),
        new Option<bool>(
            "89923",
            "Hollow Dawn Spellbooks",
            "Mode: [select] only\nShould the bot buy \"Hollow Dawn Spellbooks\" ?",
            false
        ),
        new Option<bool>(
            "89924",
            "Hollow Dawn Grimoires",
            "Mode: [select] only\nShould the bot buy \"Hollow Dawn Grimoires\" ?",
            false
        ),
        new Option<bool>(
            "89925",
            "Hollowborn Priest Staff",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Priest Staff\" ?",
            false
        ),
        new Option<bool>(
            "90518",
            "Dawn Vindicator Castle",
            "Mode: [select] only\nShould the bot buy \"Dawn Vindicator Castle\" ?",
            false
        ),
    };
}
