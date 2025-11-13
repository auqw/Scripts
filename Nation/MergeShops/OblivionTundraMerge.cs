/*
name: Oblivion Tundra Merge
description: This bot will farm the items belonging to the selected mode for the Oblivion Tundra Merge [2477] in /obliviontundra
tags: oblivion, tundra, merge, obliviontundra, envenomed, spear, nulgath, fiend, voids, desire, hunger, healing, euclid, nation, healer, rogues, knives, knife, reverse, rogue, thaumaturgy, tools, tome, fiends, catalyst, thaumaturge, hunters, trust, bounty, dealbreaker, warrior, armet
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/MergeShops/FiendishLoreMasterMerge.cs
//cs_include Scripts/Story/Nation/OblivionTundra.cs
//cs_include Scripts/Story/Nation/DeleuzeTundra.cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
//cs_include Scripts/Story/Nation/VoidChasm.cs
//cs_include Scripts/Story/Nation/Tercessuinotlim.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class OblivionTundraMerge
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

    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static FiendishLoreMasterMerge FLM
    {
        get => _FLM ??= new FiendishLoreMasterMerge();
        set => _FLM = value;
    }
    private static FiendishLoreMasterMerge _FLM;
    private static OblivionTundra OT
    {
        get => _OT ??= new OblivionTundra();
        set => _OT = value;
    }
    private static OblivionTundra _OT;

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
                "Unidentified 13",
                "Tainted Gem",
                "Dark Crystal Shard",
                "Diamond of Nulgath",
                "Null Contract",
                "Withered Archfiend's Essence",
                "ArchFiend Healer Staff",
                "ArchFiend Healer Hood",
                "ArchFiend Healer",
                "ArchFiend Rogue Knives",
                "ArchFiend Rogue Knife",
                "ArchFiend Rogue Backwards Knives",
                "ArchFiend Rogue Backwards Knife",
                "ArchFiend Rogue",
                "ArchFiend Mage Book + Wand",
                "ArchFiend Mage's Tome",
                "ArchFiend Mage's Wand",
                "ArchFiend Mage Hood",
                "ArchFiend Mage Hat",
                "ArchFiend Mage",
                "Dual ArchFiend Warrior Champion Swords",
                "ArchFiend Warrior Champion Sword",
                "Dual ArchFiend Warrior Swords",
                "ArchFiend Warrior Sword",
                "ArchFiend Warrior",
                "ArchFiend Warrior Helm",
                "ArchFiend Warrior Armet",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        OT.Storyline();
        Adv.StartBuyAllMerge(
            "obliviontundra",
            2477,
            findIngredients,
            buyOnlyThis,
            buyMode: buyMode
        );

        #region Dont edit this part
        void findIngredients()
        {
            #region Useable Monsters
            string[] UseableMonsters = new[]
            {
                "Empty Creature", // UseableMonsters[0],
                "Hushed", // UseableMonsters[1],
                "Withered Archfiend", // UseableMonsters[2],
                "Oblivion Magus ", // UseableMonsters[3],
                "Ettin Fiend", // UseableMonsters[4],
                "Archfiend Oblivion", // UseableMonsters[5],
                "Infinity Pool", // UseableMonsters[6]
            };
            #endregion Useable Monsters
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

                case "Unidentified 13":
                    Nation.FarmUni13(quant);
                    break;

                case "Tainted Gem":
                    Nation.FarmTaintedGem(quant);
                    break;

                case "Dark Crystal Shard":
                    Nation.FarmDarkCrystalShard(quant);
                    break;

                case "Diamond of Nulgath":
                    Nation.FarmDiamondofNulgath(quant);
                    break;

                case "Null Contract":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(10046);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(
                            10046,
                            new[]
                            {
                                ("obliviontundra", UseableMonsters[1], ClassType.Farm),
                                ("obliviontundra", UseableMonsters[4], ClassType.Farm),
                                ("obliviontundra", UseableMonsters[5], ClassType.Solo),
                            }
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Withered Archfiend's Essence":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "obliviontundra",
                        UseableMonsters[2],
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    break;

                case "ArchFiend Healer Staff":
                case "ArchFiend Healer Hood":
                case "ArchFiend Rogue Knives":
                case "ArchFiend Rogue Knife":
                case "ArchFiend Rogue Backwards Knives":
                case "ArchFiend Rogue Backwards Knife":
                case "ArchFiend Mage Book + Wand":
                case "ArchFiend Mage Hood":
                case "ArchFiend Mage Hat":
                case "Dual ArchFiend Warrior Champion Swords":
                case "ArchFiend Warrior Champion Sword":
                case "Dual ArchFiend Warrior Swords":
                case "ArchFiend Warrior Helm":
                case "ArchFiend Warrior Armet":
                case "ArchFiend Healer":
                case "ArchFiend Rogue":
                case "ArchFiend Mage":
                case "ArchFiend Warrior":
                    FLM.BuyAllMerge(req.Name);
                    break;

                case "ArchFiend Mage's Tome":
                case "ArchFiend Mage's Wand":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(
                        "enemyforest",
                        "Evil Elemental",
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "79257",
            "Envenomed Spear of Nulgath",
            "Mode: [select] only\nShould the bot buy \"Envenomed Spear of Nulgath\" ?",
            false
        ),
        new Option<bool>(
            "79255",
            "Fiend Void's Desire",
            "Mode: [select] only\nShould the bot buy \"Fiend Void's Desire\" ?",
            false
        ),
        new Option<bool>(
            "91497",
            "Dual Fiend Void's Hunger",
            "Mode: [select] only\nShould the bot buy \"Dual Fiend Void's Hunger\" ?",
            false
        ),
        new Option<bool>(
            "79253",
            "Fiend Void's Hunger",
            "Mode: [select] only\nShould the bot buy \"Fiend Void's Hunger\" ?",
            false
        ),
        new Option<bool>(
            "66980",
            "Healing Euclid Staff",
            "Mode: [select] only\nShould the bot buy \"Healing Euclid Staff\" ?",
            false
        ),
        new Option<bool>(
            "66979",
            "Nation Healer Hood",
            "Mode: [select] only\nShould the bot buy \"Nation Healer Hood\" ?",
            false
        ),
        new Option<bool>(
            "66978",
            "Nation Healer",
            "Mode: [select] only\nShould the bot buy \"Nation Healer\" ?",
            false
        ),
        new Option<bool>(
            "66974",
            "Nation Rogue's Knives",
            "Mode: [select] only\nShould the bot buy \"Nation Rogue's Knives\" ?",
            false
        ),
        new Option<bool>(
            "66973",
            "Nation Rogue's Knife",
            "Mode: [select] only\nShould the bot buy \"Nation Rogue's Knife\" ?",
            false
        ),
        new Option<bool>(
            "66972",
            "Reverse Nation Rogue's Knives",
            "Mode: [select] only\nShould the bot buy \"Reverse Nation Rogue's Knives\" ?",
            false
        ),
        new Option<bool>(
            "66971",
            "Reverse Nation Rogue's Knife",
            "Mode: [select] only\nShould the bot buy \"Reverse Nation Rogue's Knife\" ?",
            false
        ),
        new Option<bool>(
            "66970",
            "Nation Rogue",
            "Mode: [select] only\nShould the bot buy \"Nation Rogue\" ?",
            false
        ),
        new Option<bool>(
            "66961",
            "Nation Thaumaturgy Tools",
            "Mode: [select] only\nShould the bot buy \"Nation Thaumaturgy Tools\" ?",
            false
        ),
        new Option<bool>(
            "66960",
            "Nation Thaumaturgy Tome",
            "Mode: [select] only\nShould the bot buy \"Nation Thaumaturgy Tome\" ?",
            false
        ),
        new Option<bool>(
            "66959",
            "Fiend's Euclid Catalyst",
            "Mode: [select] only\nShould the bot buy \"Fiend's Euclid Catalyst\" ?",
            false
        ),
        new Option<bool>(
            "66958",
            "Nation Thaumaturge Hood",
            "Mode: [select] only\nShould the bot buy \"Nation Thaumaturge Hood\" ?",
            false
        ),
        new Option<bool>(
            "66957",
            "Nation Thaumaturge Hat",
            "Mode: [select] only\nShould the bot buy \"Nation Thaumaturge Hat\" ?",
            false
        ),
        new Option<bool>(
            "66956",
            "Nation Thaumaturge",
            "Mode: [select] only\nShould the bot buy \"Nation Thaumaturge\" ?",
            false
        ),
        new Option<bool>(
            "66949",
            "Dual Fiend Hunter's Trust",
            "Mode: [select] only\nShould the bot buy \"Dual Fiend Hunter's Trust\" ?",
            false
        ),
        new Option<bool>(
            "66948",
            "Fiend Hunter's Trust",
            "Mode: [select] only\nShould the bot buy \"Fiend Hunter's Trust\" ?",
            false
        ),
        new Option<bool>(
            "66947",
            "Dual Bounty Hunter's Dealbreaker",
            "Mode: [select] only\nShould the bot buy \"Dual Bounty Hunter's Dealbreaker\" ?",
            false
        ),
        new Option<bool>(
            "66946",
            "Bounty Hunter's Dealbreaker",
            "Mode: [select] only\nShould the bot buy \"Bounty Hunter's Dealbreaker\" ?",
            false
        ),
        new Option<bool>(
            "66943",
            "Nation Warrior",
            "Mode: [select] only\nShould the bot buy \"Nation Warrior\" ?",
            false
        ),
        new Option<bool>(
            "66945",
            "Nation Warrior Helm",
            "Mode: [select] only\nShould the bot buy \"Nation Warrior Helm\" ?",
            false
        ),
        new Option<bool>(
            "66944",
            "Nation Warrior Armet",
            "Mode: [select] only\nShould the bot buy \"Nation Warrior Armet\" ?",
            false
        ),
    };
}
