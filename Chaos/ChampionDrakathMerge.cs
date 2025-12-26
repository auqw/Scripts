/*
name: Champion Drakath Merge
description: This bot will farm the items belonging to the selected mode for the Champion Drakath Merge [2055] in /championdrakath
tags: champion, drakath, merge, championdrakath, parallel, chaos, amulet, fragments, lords, a, b, empowered, original, avengers, greatsword, avenger, polished, dragon, control, supreme, arcane, colorful, rose, royal, trace, kitsunes, chaotic, guilt, chaorrupter, locked
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Chaos/DrakathsArmor.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Story/BattleUnder.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class ChampionDrakathMerge
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

    private static Core13LoC LOC
    {
        get => _LOC ??= new Core13LoC();
        set => _LOC = value;
    }
    private static Core13LoC _LOC;
    private static DrakathArmorBot DAB
    {
        get => _DAB ??= new DrakathArmorBot();
        set => _DAB = value;
    }
    private static DrakathArmorBot _DAB;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    // If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Champion Drakath Insignia", "Escherion's Robe", "Vath's Chaotic Dragonlord Armor", "Chaos Shogun Armor", "Wolfwing Armor", "Discordia Armor", "Ledgermayne", "Tibicenas", "Soul of Chaos Armor", "Iadoa", "Chaos Lionfang Armor", "Chaos Lord Alteon", "Xiang Chaos", "Drakath Armor", "Original Drakath Armor", "Blade of Chaos", "Chaos Avenger's Greatsword", "Chaos Avenger Armor", "Legendary Sword of Dragon Control", "The Supreme Arcane Staff", "Discordia Rose of Chaos", "Chaos Rose", "Kitsune's Chaos Mask" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        if (!Core.isCompletedBefore(8301))
        {
            Core.Logger("Quest \"Chaos Avenger Class\" required, gl.. it requires an army");
            return;
        }
        LOC.Hero();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge(
            "championdrakath",
            2055,
            findIngredients,
            buyOnlyThis,
            buyMode: buyMode
        );

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

                case "Drakath Armor":
                    DAB.DrakathArmorQuest();
                    break;

                case "Chaos Avenger Armor":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem("championdrakath", 2056, req.Name, quant, Log: false);
                    break;

                case "Kitsune's Chaos Mask":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }
                    FarmKitsunesMask(req.Name);
                    break;

                case "Tibicenas":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("djinn", "Tibicenas", req.Name, quant, req.Temp, false);
                    break;

                case "Soul of Chaos Armor":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("dreamnexus", "Khasaanda", req.Name, quant, req.Temp, false);
                    break;

                case "Iadoa":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("mqlesson", "Dragonoid", "Dragonoid of Hours", isTemp: false);
                    Core.HuntMonster("timespace", "Chaos Lord Iadoa", req.Name, quant, req.Temp, false);
                    break;

                case "Chaos Lionfang Armor":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Bot.Quests.UpdateQuest(2814);
                    Core.HuntMonster(
                        "stormtemple",
                        "Chaos Lord Lionfang",
                        req.Name, quant,
                        req.Temp, false
                    );
                    break;

                case "Chaos Shogun Armor":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.KillKitsune(req.Name, quant, log: false);
                    break;

                case "Wolfwing Armor":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("wolfwing", "Wolfwing", req.Name, quant, req.Temp, false);
                    break;

                case "Escherion's Robe":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.KillEscherion(req.Name, quant, log: false);
                    break;

                case "Champion Drakath Insignia":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.Logger(
                            "You have to kill Champion Drakath weekly to get his insignias, use your army to kill it easily"
                        );
                        return;
                    }
                    break;

                case "Original Drakath Armor":
                    DAB.DrakathOriginalArmor();
                    break;

                case "Blade of Chaos":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "ultradrakath",
                        "Champion of Chaos",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Xiang Chaos":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Adv.GearStore();
                    Core.KillXiang("Xiang Chaos");
                    Adv.GearStore(true);
                    break;

                case "Chaos Avenger's Greatsword":
                    Core.EquipClass(ClassType.Solo);
                    Adv.BuyItem("championdrakath", 2056, req.Name);
                    break;

                case "Vath's Chaotic Dragonlord Armor":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.KillVath(req.Name, quant, log: false);
                    break;

                case "Legendary Sword of Dragon Control":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.KillVath(req.Name, quant);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "The Supreme Arcane Staff":
                case "Ledgermayne":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "ledgermayne",
                        "Ledgermayne",
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Discordia Rose of Chaos":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "palooza",
                        "Chaos Lord Discordia",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Discordia Armor":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("palooza", "Discordia", req.Name, quant, req.Temp, false);
                    break;

                case "Chaos Rose":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "palooza",
                        "Chaos Lord Discordia",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    Bot.Wait.ForPickup(req.Name);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("63575", "Parallel Chaos Amulet", "Mode: [select] only\nShould the bot buy \"Parallel Chaos Amulet\" ?", false),
        new Option<bool>("64105", "Fragments of the Lords A", "Mode: [select] only\nShould the bot buy \"Fragments of the Lords A\" ?", false),
        new Option<bool>("64106", "Fragments of the Lords B", "Mode: [select] only\nShould the bot buy \"Fragments of the Lords B\" ?", false),
        new Option<bool>("64151", "Empowered Drakath Armor", "Mode: [select] only\nShould the bot buy \"Empowered Drakath Armor\" ?", false),
        new Option<bool>("64152", "Empowered Original Drakath Armor", "Mode: [select] only\nShould the bot buy \"Empowered Original Drakath Armor\" ?", false),
        new Option<bool>("64153", "Empowered Blade of Chaos", "Mode: [select] only\nShould the bot buy \"Empowered Blade of Chaos\" ?", false),
        new Option<bool>("64154", "Empowered Chaos Avenger's Greatsword", "Mode: [select] only\nShould the bot buy \"Empowered Chaos Avenger's Greatsword\" ?", false),
        new Option<bool>("64155", "Empowered Chaos Avenger Armor", "Mode: [select] only\nShould the bot buy \"Empowered Chaos Avenger Armor\" ?", false),
        new Option<bool>("60988", "Polished Sword of Dragon Control", "Mode: [select] only\nShould the bot buy \"Polished Sword of Dragon Control\" ?", false),
        new Option<bool>("60990", "Polished Supreme Arcane Staff", "Mode: [select] only\nShould the bot buy \"Polished Supreme Arcane Staff\" ?", false),
        new Option<bool>("60991", "Colorful Chaos Rose", "Mode: [select] only\nShould the bot buy \"Colorful Chaos Rose\" ?", false),
        new Option<bool>("60992", "Royal Chaos Rose", "Mode: [select] only\nShould the bot buy \"Royal Chaos Rose\" ?", false),
        new Option<bool>("74259", "Trace of Chaos", "Mode: [select] only\nShould the bot buy \"Trace of Chaos\" ?", false),
        new Option<bool>("94406", "Empowered Kitsune's Chaos Mask", "Mode: [select] only\nShould the bot buy \"Empowered Kitsune's Chaos Mask\" ?", false),
        new Option<bool>("94407", "Chaotic Guilt Blade", "Mode: [select] only\nShould the bot buy \"Chaotic Guilt Blade\" ?", false),
        new Option<bool>("98032", "Chaorrupter Locked", "Mode: [select] only\nShould the bot buy \"Chaorrupter Locked\" ?", false),
        new Option<bool>("98043", "Dual Chaorrupter Locked", "Mode: [select] only\nShould the bot buy \"Dual Chaorrupter Locked\" ?", false),
   };


    public void FarmKitsunesMask(string? item = null)
    {
        // Farm both if null, otherwise farm specific item
        string[] masks = item == null
            ? new[] { "Kitsune's Chaos Mask", "Kitsune's Mask" }
            : new[] { item };

        if ((item == null && Core.CheckInventory(masks)) || (item != null && Core.CheckInventory(item)))
            return;

        Core.BankingBlackList.AddRange(new[]
        {
        "Lock Spell 1", "Lock Spell 2", "Mystical Chaos Runes",
        "Spy's Disclosure", "Bandit's Map", "Unusual Rocks",
        "Chaotic Talking Gem", "Kitsune's Mask", "Kitsune's Chaos Mask"
    });

        Core.AddDrop(new[]
        {
        "Lock Spell 1", "Lock Spell 2", "Mystical Chaos Runes",
        "Spy's Disclosure", "Bandit's Map", "Unusual Rocks",
        "Chaotic Talking Gem", "Kitsune's Mask", "Kitsune's Chaos Mask"
    });

        Core.EquipClass(ClassType.Farm);

        foreach (string mask in masks)
        {
            if (Core.CheckInventory(mask))
                continue;

            Core.FarmingLogger(mask, 1);
            EnsureLockSpell2();
            EnsureLockSpell1();

            Adv.BuyItem("chaosmarsh", 2365, mask);
            Bot.Wait.ForPickup(mask);
        }
    }

    void EnsureLockSpell1()
    {
        if (Core.CheckInventory("Lock Spell 1"))
            return;

        BuyMysticalChaosRunes();
        Core.GetMapItem(12330, 1, "warundead");
    }

    void EnsureLockSpell2()
    {
        if (Core.CheckInventory("Lock Spell 2"))
            return;

        EnsureLockSpell1();
        Core.GetMapItem(12331, 1, "ravenloss");
    }

    void BuyMysticalChaosRunes()
    {
        if (Core.CheckInventory("Mystical Chaos Runes"))
            return;

        EnsureChaoticTalkingGem();
        EnsureUnusualRocks();
        EnsureBanditsMap();
        EnsureSpysDisclosure();

        Adv.BuyItem("relativity", 2344, "Mystical Chaos Runes");
        Bot.Wait.ForPickup("Mystical Chaos Runes");
    }

    void EnsureSpysDisclosure()
    {
        if (Core.CheckInventory("Spy's Disclosure"))
            return;

        Core.GetMapItem(12221, 1, "museum");
    }

    void EnsureBanditsMap()
    {
        if (Core.CheckInventory("Bandit's Map"))
            return;

        Core.HuntMonster("poisonforest", "Bandit", "Bandit's Map", isTemp: false);
    }

    void EnsureUnusualRocks()
    {
        if (Core.CheckInventory("Unusual Rocks"))
            return;

        Core.GetMapItem(12222, 1, "wargrounds");
    }

    void EnsureChaoticTalkingGem()
    {
        if (Core.CheckInventory("Chaotic Talking Gem"))
            return;

        Core.GetMapItem(12223, 1, "mountainpath");
    }
}
