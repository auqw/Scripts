/*
name: EbilMegaMall Merge
description: This bot will farm the items belonging to the selected mode for the EbilMegaMall Merge [2641] in /ebilmegamall
tags: ebilmegamall, merge, ebilmegamall, super, rare, mogugu, common, successful, ebilcorp, scalper, undercover, morph, sunglasses, shades, ebil, capital, cyber, night, prowler, mohawk, cut, tail, prowlers, catastrophe, cannons, executor, executors, denken, reserve, claw, claws, collection, , ultimate, display, mogugudra, behemoth
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/BlackFriday/EbilMegaMallStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class EbilMegaMallMerge
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
    private static EbilMegaMall EbilMegaMall
    {
        get => _EbilMegaMall ??= new EbilMegaMall();
        set => _EbilMegaMall = value;
    }
    private static EbilMegaMall _EbilMegaMall;

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
                "EbilCorp Scalper",
                "Pockeymog Card Pack",
                "EbilCorp Scalper Morph",
                "EbilCorp Scalper Locks",
                "EbilCorp Scalper Hair",
                "EbilCorp Scalper Visage",
                "Mogugu Display Case",
                "Red Mogugu Box",
                "Yellow Mogugu Box",
                "Blue Mogugu Box",
                "Blue Mogugu Critter",
                "Yellow Mogugu Critter",
                "Red Mogugu Critter",
                "Black Mogugu Critter",
                "Black Mogugu Box",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        EbilMegaMall.StoryLine();
        Adv.MergeItemisinShopExceptions.AddRange(
            new[]
            {
                "Common Mogugu",
                "Super Rare Mogugu",
                "Super Super Rare Mogugu",
                "Super Super Super Rare Mogugu",
            }
        );
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("ebilmegamall", 2641, findIngredients, buyOnlyThis, buyMode: buyMode);
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

                #region Items not setup

                case "Common Mogugu":
                    Core.AddDrop(
                        "Common Mogugu",
                        "Super Rare Mogugu",
                        "Super Super Rare Mogugu",
                        "Super Super Super Rare Mogugu"
                    );
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(10509);
                    Core.KillMonster("ebilmegamall", "r8", "Left", "*", req.Name, quant, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
                case "Super Rare Mogugu":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(10509);

                    int needed = quant;
                    const int MAX_STACK = 1000;

                    while (!Bot.ShouldExit && needed > 0)
                    {
                        if (Core.CheckInventory(req.Name, needed))
                        {
                            needed = 0;
                            continue;
                        }

                        int commonNeeded = Math.Min(needed * 10, MAX_STACK);

                        while (
                            !Bot.ShouldExit && !Core.CheckInventory("Common Mogugu", commonNeeded)
                        )
                        {
                            Core.KillMonster("ebilmegamall", "r8", "Left", "*", log: false);
                            Bot.Wait.ForPickup("Common Mogugu");
                        }

                        int batchSize = commonNeeded / 10;
                        Core.BuyItem("ebilmegamall", 2641, req.Name, batchSize);
                        Bot.Wait.ForPickup(req.Name);
                        needed -= batchSize;
                    }

                    Core.CancelRegisteredQuests();
                    break;

                case "Super Super Rare Mogugu":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(10509);

                    string[] tiers =
                    {
                        "Common Mogugu",
                        "Super Rare Mogugu",
                        "Super Super Rare Mogugu",
                    };
                    needed = quant;

                    while (!Bot.ShouldExit && needed > 0)
                    {
                        if (Core.CheckInventory(req.Name, needed))
                        {
                            needed = 0;
                            continue;
                        }

                        // Batch in chunks to avoid exceeding stack limit
                        int batchSize = Math.Min(needed, MAX_STACK / 100); // Max 10 per batch since 10 needs 1000 Common
                        if (batchSize == 0)
                            batchSize = 1;

                        // Work backwards through tiers
                        for (int tier = tiers.Length - 2; tier >= 0; tier--)
                        {
                            int tierNeeded = batchSize * (int)Math.Pow(10, tiers.Length - 2 - tier);
                            while (!Bot.ShouldExit && tierNeeded > 0)
                            {
                                if (Core.CheckInventory(tiers[tier], tierNeeded))
                                {
                                    tierNeeded = 0;
                                    continue;
                                }

                                if (tier == 0) // Common Mogugu - farm it
                                {
                                    Core.KillMonster("ebilmegamall", "r8", "Left", "*", log: false);
                                    Bot.Wait.ForPickup("Common Mogugu");
                                }
                                else // Buy from shop
                                {
                                    Core.BuyItem("ebilmegamall", 2641, tiers[tier], tierNeeded);
                                    Bot.Wait.ForPickup(tiers[tier]);
                                }
                            }
                        }

                        Core.BuyItem("ebilmegamall", 2641, req.Name, batchSize);
                        Bot.Wait.ForPickup(req.Name);
                        needed -= batchSize;
                    }

                    Core.CancelRegisteredQuests();
                    break;

                case "Super Super Super Rare Mogugu":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(10509);

                    string[] tiersSSSR =
                    {
                        "Common Mogugu",
                        "Super Rare Mogugu",
                        "Super Super Rare Mogugu",
                        "Super Super Super Rare Mogugu",
                    };
                    needed = quant;

                    while (!Bot.ShouldExit && needed > 0)
                    {
                        if (Core.CheckInventory(req.Name, needed))
                        {
                            needed = 0;
                            continue;
                        }

                        // Batch in chunks to avoid exceeding stack limit
                        int batchSize = Math.Min(needed, MAX_STACK / 1000); // Max 1 per batch since 1 needs 1000 Common
                        if (batchSize == 0)
                            batchSize = 1;

                        // Work backwards through tiers
                        for (int tier = tiersSSSR.Length - 2; tier >= 0; tier--)
                        {
                            int tierNeeded =
                                batchSize * (int)Math.Pow(10, tiersSSSR.Length - 2 - tier);
                            while (!Bot.ShouldExit && tierNeeded > 0)
                            {
                                if (Core.CheckInventory(tiersSSSR[tier], tierNeeded))
                                {
                                    tierNeeded = 0;
                                    continue;
                                }

                                if (tier == 0) // Common Mogugu - farm it
                                {
                                    Core.KillMonster("ebilmegamall", "r8", "Left", "*", log: false);
                                    Bot.Wait.ForPickup("Common Mogugu");
                                }
                                else // Buy from shop
                                {
                                    Core.BuyItem("ebilmegamall", 2641, tiersSSSR[tier], tierNeeded);
                                    Bot.Wait.ForPickup(tiersSSSR[tier]);
                                }
                            }
                        }

                        Core.BuyItem("ebilmegamall", 2641, req.Name, batchSize);
                        Bot.Wait.ForPickup(req.Name);
                        needed -= batchSize;
                    }

                    Core.CancelRegisteredQuests();
                    break;
                case "EbilCorp Scalper":
                case "Pockeymog Card Pack":
                case "EbilCorp Scalper Morph":
                case "EbilCorp Scalper Locks":
                case "EbilCorp Scalper Hair":
                case "EbilCorp Scalper Visage":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("ebilmegamall", "Scalper", req.Name, quant, req.Temp, false);
                    break;

                case "Mogugu Display Case":
                case "Red Mogugu Box":
                case "Yellow Mogugu Box":
                case "Blue Mogugu Box":
                case "Blue Mogugu Critter":
                case "Yellow Mogugu Critter":
                case "Red Mogugu Critter":
                case "Black Mogugu Critter":
                case "Black Mogugu Box":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }
                    Core.AddDrop(
                        "Mogugu Display Case",
                        "Red Mogugu Box",
                        "Yellow Mogugu Box",
                        "Blue Mogugu Box",
                        "Blue Mogugu Critter",
                        "Yellow Mogugu Critter",
                        "Red Mogugu Critter",
                        "Black Mogugu Critter",
                        "Black Mogugu Box"
                    );
                    Core.RegisterQuests(10509);
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("ebilmegamall", "Mogugudra", req.Name, quant, req.Temp, false);
                    break;

                #endregion
            }
        }
    }

    void CommonMogugu(int quant)
    {
        if (Core.CheckInventory("Common Mogugu", quant))
            return;

        Core.FarmingLogger("Common Mogugu", quant);
        Core.KillMonster(
            "ebilmegamall",
            "r8",
            "Left",
            "*",
            "Common Mogugu",
            quant,
            isTemp: false,
            false
        );
        Bot.Wait.ForPickup("Common Mogugu");
    }

    void SuperRareMogugu(int quant)
    {
        if (Core.CheckInventory("Super Rare Mogugu", quant))
            return;

        Core.FarmingLogger("Super Rare Mogugu", quant);
        CommonMogugu(10 * quant);
        Core.BuyItem("ebilmegamall", 2641, "Super Rare Mogugu", quant);
        Bot.Wait.ForPickup("Super Rare Mogugu");
    }

    void SuperSuperRareMogugu(int quant)
    {
        if (Core.CheckInventory("Super Super Rare Mogugu", quant))
            return;
        Core.FarmingLogger("Super Super Rare Mogugu", quant);
        SuperRareMogugu(10 * quant);
        Core.BuyItem("ebilmegamall", 2641, "Super Super Rare Mogugu", quant);
        Bot.Wait.ForPickup("Super Super Rare Mogugu");
    }

    void SuperSuperSuperRareMogugu(int quant)
    {
        if (Core.CheckInventory("Super Super Super Rare Mogugu", quant))
            return;
        SuperSuperRareMogugu(10 * quant);
        Core.BuyItem("ebilmegamall", 2641, "Super Super Super Rare Mogugu", quant);
        Bot.Wait.ForPickup("Super Super Super Rare Mogugu");
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "96808",
            "Successful EbilCorp Scalper",
            "Mode: [select] only\nShould the bot buy \"Successful EbilCorp Scalper\" ?",
            false
        ),
        new Option<bool>(
            "96810",
            "Undercover EbilCorp Scalper Morph",
            "Mode: [select] only\nShould the bot buy \"Undercover EbilCorp Scalper Morph\" ?",
            false
        ),
        new Option<bool>(
            "96811",
            "EbilCorp Scalper Sunglasses",
            "Mode: [select] only\nShould the bot buy \"EbilCorp Scalper Sunglasses\" ?",
            false
        ),
        new Option<bool>(
            "96814",
            "EbilCorp Scalper Shades",
            "Mode: [select] only\nShould the bot buy \"EbilCorp Scalper Shades\" ?",
            false
        ),
        new Option<bool>(
            "96815",
            "Undercover EbilCorp Scalper Visage",
            "Mode: [select] only\nShould the bot buy \"Undercover EbilCorp Scalper Visage\" ?",
            false
        ),
        new Option<bool>(
            "96820",
            "Ebil Capital Blade",
            "Mode: [select] only\nShould the bot buy \"Ebil Capital Blade\" ?",
            false
        ),
        new Option<bool>(
            "96821",
            "Ebil Capital Blades",
            "Mode: [select] only\nShould the bot buy \"Ebil Capital Blades\" ?",
            false
        ),
        new Option<bool>(
            "96933",
            "Cyber Night Prowler",
            "Mode: [select] only\nShould the bot buy \"Cyber Night Prowler\" ?",
            false
        ),
        new Option<bool>(
            "96934",
            "Cyber Night Prowler Helm",
            "Mode: [select] only\nShould the bot buy \"Cyber Night Prowler Helm\" ?",
            false
        ),
        new Option<bool>(
            "96935",
            "Cyber Night Prowler Mask",
            "Mode: [select] only\nShould the bot buy \"Cyber Night Prowler Mask\" ?",
            false
        ),
        new Option<bool>(
            "96936",
            "Cyber Night Prowler Mohawk",
            "Mode: [select] only\nShould the bot buy \"Cyber Night Prowler Mohawk\" ?",
            false
        ),
        new Option<bool>(
            "96937",
            "Cyber Night Prowler Cut",
            "Mode: [select] only\nShould the bot buy \"Cyber Night Prowler Cut\" ?",
            false
        ),
        new Option<bool>(
            "96938",
            "Cyber Night Prowler Locks",
            "Mode: [select] only\nShould the bot buy \"Cyber Night Prowler Locks\" ?",
            false
        ),
        new Option<bool>(
            "96939",
            "Cyber Night Prowler Tail",
            "Mode: [select] only\nShould the bot buy \"Cyber Night Prowler Tail\" ?",
            false
        ),
        new Option<bool>(
            "96940",
            "Prowler's Catastrophe Cannons",
            "Mode: [select] only\nShould the bot buy \"Prowler's Catastrophe Cannons\" ?",
            false
        ),
        new Option<bool>(
            "96942",
            "Cyber Prowler's Executor",
            "Mode: [select] only\nShould the bot buy \"Cyber Prowler's Executor\" ?",
            false
        ),
        new Option<bool>(
            "96943",
            "Cyber Prowler's Executors",
            "Mode: [select] only\nShould the bot buy \"Cyber Prowler's Executors\" ?",
            false
        ),
        new Option<bool>(
            "96944",
            "Prowler's Denken",
            "Mode: [select] only\nShould the bot buy \"Prowler's Denken\" ?",
            false
        ),
        new Option<bool>(
            "96945",
            "Prowler's Dual Denken",
            "Mode: [select] only\nShould the bot buy \"Prowler's Dual Denken\" ?",
            false
        ),
        new Option<bool>(
            "96946",
            "Prowler's Reserve Denken",
            "Mode: [select] only\nShould the bot buy \"Prowler's Reserve Denken\" ?",
            false
        ),
        new Option<bool>(
            "96947",
            "Prowler's Reserve Dual Denken",
            "Mode: [select] only\nShould the bot buy \"Prowler's Reserve Dual Denken\" ?",
            false
        ),
        new Option<bool>(
            "96948",
            "Cyber Prowler's Claw",
            "Mode: [select] only\nShould the bot buy \"Cyber Prowler's Claw\" ?",
            false
        ),
        new Option<bool>(
            "96949",
            "Cyber Prowler's Claws",
            "Mode: [select] only\nShould the bot buy \"Cyber Prowler's Claws\" ?",
            false
        ),
        new Option<bool>(
            "97230",
            "Mogugu Collection: 25%",
            "Mode: [select] only\nShould the bot buy \"Mogugu Collection: 25%\" ?",
            false
        ),
        new Option<bool>(
            "97231",
            "Mogugu Collection: 50%",
            "Mode: [select] only\nShould the bot buy \"Mogugu Collection: 50%\" ?",
            false
        ),
        new Option<bool>(
            "97232",
            "Mogugu Collection: 75%",
            "Mode: [select] only\nShould the bot buy \"Mogugu Collection: 75%\" ?",
            false
        ),
        new Option<bool>(
            "97234",
            "ULTIMATE Mogugu Display",
            "Mode: [select] only\nShould the bot buy \"ULTIMATE Mogugu Display\" ?",
            false
        ),
        new Option<bool>(
            "97235",
            "Mogugudra Behemoth",
            "Mode: [select] only\nShould the bot buy \"Mogugudra Behemoth\" ?",
            false
        ),
        new Option<bool>(
            "97233",
            "Mogugu Collection: 100%",
            "Mode: [select] only\nShould the bot buy \"Mogugu Collection: 100%\" ?",
            false
        ),
    };
}
