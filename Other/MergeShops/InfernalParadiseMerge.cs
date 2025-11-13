/*
name: Infernal Paradise Merge
description: This bot will farm the items belonging to the selected mode for the Infernal Paradise Merge [2561] in /infernalparadise
tags: infernal, paradise, merge, infernalparadise, golden, crown, astral, celestial, wings, halo, fallen, aranx, evolved, wreathed, furled, archangel, mysteries
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/CelestialPast.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/InfernalParadise.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class InfernalParadiseMerge
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

    private static InfernalParadise IP
    {
        get => _IP ??= new InfernalParadise();
        set => _IP = value;
    }
    private static InfernalParadise _IP;

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
                "Celestial Seal",
                "Golden Scale",
                "Golden Badge",
                "Golden Wing",
                "Golden Rune",
                "Laurel Crown",
                "Divine Down",
                "Infernal Mage's Incantation",
                "Malxas' Shed Feather",
                "Infernal Token",
                "Champion Sash",
                "Celestial Quintessence",
                "Infernalis Penna",
                "Infernalis Oculus",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        IP.Storyline();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge(
            "infernalparadise",
            2561,
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

                case "Celestial Seal":
                case "Golden Scale":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "goldenarena",
                        "Blessed Dragon",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;

                case "Golden Badge":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "goldenarena",
                        "Blessed Inquisitor",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;

                case "Golden Wing":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "goldenarena",
                        "Blessed Gladius",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;

                case "Golden Rune":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("goldenarena", "Blessed Karok", req.Name, quant, false, false);
                    break;

                case "Laurel Crown":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("goldenarena", "Queen of Hope", req.Name, quant, false, false);
                    break;

                case "Divine Down":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(
                            10082,
                            ("infernalparadise", "Akh-a", ClassType.Solo),
                            ("infernalparadise", "Azalith", ClassType.Solo),
                            ("infernalparadise", "Infernal Knight", ClassType.Farm)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Infernal Mage's Incantation":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(
                        "infernalparadise",
                        "Infernal Mage",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;

                case "Malxas' Shed Feather":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "infernalparadise",
                        "Infernal Malxas",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;

                case "Infernal Token":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(
                        "Celestialrealm",
                        "Fallen Knight",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;

                case "Champion Sash":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("celestialarenad", "Aranx", req.Name, quant, false, false);
                    break;

                case "Celestial Quintessence":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(
                        "celestialpast",
                        "r2",
                        "Left",
                        "*",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;

                case "Infernalis Penna":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(
                            9887,
                            ("champazalith", "Maah-na", ClassType.Solo),
                            ("champazalith", "Akh-a", ClassType.Solo)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Infernalis Oculus":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(9888, ("champazalith", "Azalith", ClassType.Solo));
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "92183",
            "Golden Crown",
            "Mode: [select] only\nShould the bot buy \"Golden Crown\" ?",
            false
        ),
        new Option<bool>(
            "92113",
            "Astral Celestial Wings",
            "Mode: [select] only\nShould the bot buy \"Astral Celestial Wings\" ?",
            false
        ),
        new Option<bool>(
            "92112",
            "Astral Celestial Halo",
            "Mode: [select] only\nShould the bot buy \"Astral Celestial Halo\" ?",
            false
        ),
        new Option<bool>(
            "92111",
            "Astral Celestial Hood",
            "Mode: [select] only\nShould the bot buy \"Astral Celestial Hood\" ?",
            false
        ),
        new Option<bool>(
            "92110",
            "Astral Celestial",
            "Mode: [select] only\nShould the bot buy \"Astral Celestial\" ?",
            false
        ),
        new Option<bool>(
            "92061",
            "Fallen Aranx Wings",
            "Mode: [select] only\nShould the bot buy \"Fallen Aranx Wings\" ?",
            false
        ),
        new Option<bool>(
            "92060",
            "Evolved Fallen Aranx Wings",
            "Mode: [select] only\nShould the bot buy \"Evolved Fallen Aranx Wings\" ?",
            false
        ),
        new Option<bool>(
            "92059",
            "Wreathed Fallen Aranx Wings",
            "Mode: [select] only\nShould the bot buy \"Wreathed Fallen Aranx Wings\" ?",
            false
        ),
        new Option<bool>(
            "92058",
            "Furled Fallen Aranx Wings",
            "Mode: [select] only\nShould the bot buy \"Furled Fallen Aranx Wings\" ?",
            false
        ),
        new Option<bool>(
            "92063",
            "Archangel of Mysteries",
            "Mode: [select] only\nShould the bot buy \"Archangel of Mysteries\" ?",
            false
        ),
        new Option<bool>(
            "92064",
            "Archangel of Mysteries Helm",
            "Mode: [select] only\nShould the bot buy \"Archangel of Mysteries Helm\" ?",
            false
        ),
        new Option<bool>(
            "92065",
            "Archangel of Mysteries Wings",
            "Mode: [select] only\nShould the bot buy \"Archangel of Mysteries Wings\" ?",
            false
        ),
    };
}
