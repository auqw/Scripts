/*
name: Portals Merge
description: This bot will farm the items belonging to the selected mode for the Portals Merge [2585] in /basecamp
tags: portals, merge, basecamp, portal, to, grand, inquisitor, darkon, beast, headed, dracolich, red, dragon, shadowfall, fortress, swordhaven, castle, hydra, challenge, kathool, mana, golem, escherion, chaos, king, alteon, azalith, doom, vault, slugbutter, nulgaths, hidden, grotto
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/BaseCamp.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class PortalsMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private CoreFarms Farm = new();
    private CoreAdvanced Adv = new();
    private static CoreAdvanced sAdv = new();
    private CoreDailies Dailies = new();
    private BaseCamp BC = new();

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Golden Shadow Breaker", "Banana", "Ingredients?", "Beast Soul", "Undead Energy", "Silver", "Iron Draconian Sword", "Water Draconian Sword", "Venom Draconian Sword", "Mammoth Crusher Blade", "Hydra Scale Piece", "Spear of the Deep One", "Mana Golem's Core", "1st Lord Of Chaos Helm", "Chaos King Crown", "Chaos Lord Alteon", "Celestial Quintessence", "Binky Companion", "Eternity Blade", "Tainted Soul" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        BC.StoryLine();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("basecamp", 2585, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Golden Shadow Breaker":
                    Core.FarmingLogger("Golden Shadow Breaker", quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(857);
                    Core.HuntMonster("citadel", "Grand Inquisitor", req.Name, quant, req.Temp, false);
                    break;


                case "Banana":
                    Core.FarmingLogger("Banana", quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(52924);
                    Core.HuntMonster("arcangrove", "Gorillaphant", req.Name, quant, req.Temp, false);
                    break;


                case "Ingredients?":
                case "Binky Companion":
                    Core.FarmingLogger("Ingredients?", quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(52925);
                    Core.HuntMonster("doomvault", "Binky", req.Name, quant, req.Temp, false, true);
                    break;


                case "Iron Draconian Sword":
                    Core.FarmingLogger("Iron Draconian Sword", quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(571);
                    Core.HuntMonster("lair", "Purple Draconian", req.Name, quant, req.Temp, false);
                    break;


                case "Water Draconian Sword":
                    Core.FarmingLogger("Water Draconian Sword", quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(568);
                    Core.HuntMonster("lair", "Water Draconian", req.Name, quant, req.Temp, false);
                    break;


                case "Venom Draconian Sword":
                    Core.FarmingLogger("Venom Draconian Sword", quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(570);
                    Core.HuntMonster("lair", "Venom Draconian", req.Name, quant, req.Temp, false);
                    break;


                case "Spear of the Deep One":
                    Core.FarmingLogger("Spear of the Deep One", quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(79498);
                    Core.HuntMonster("deepchaos", "Kathool", req.Name, quant, req.Temp, false);
                    break;


                case "1st Lord Of Chaos Helm":
                    Core.FarmingLogger("1st Lord Of Chaos Helm", quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(1173);
                    Core.KillEscherion(req.Name, quant, req.Temp, false);
                    break;


                case "Chaos King Crown":
                case "Chaos Lord Alteon":
                    Core.FarmingLogger("Chaos King Crown", quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(20671);
                    Core.HuntMonster("swordhavenfalls", "Chaos Lord Alteon", req.Name, quant, req.Temp, false);
                    break;

                case "Tainted Soul":
                    Core.FarmingLogger("Tainted Soul", quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(4960);
                    Core.HuntMonster("evilmarsh", "Tainted Soul", req.Name, quant, req.Temp, false);
                    break;
                #endregion

                #region Known items

                case "Beast Soul":
                    if (Core.CheckInventory(req.Name, quant))
                        break;

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Adv.SmartEnhance(Core.SoloClass);
                    Core.HuntMonster("sevencircleswar", "The Beast", req.Name, quant, isTemp: false, publicRoom: true);
                    break;

                case "Undead Energy":
                    Farm.BattleUnderB("Undead Energy", quant);
                    break;

                case "Silver":
                    Dailies.MineCrafting(new[] { "Silver" }, quant);
                    break;

                case "Mammoth Crusher Blade":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("lair", "Bronze Draconian", req.Name, quant, false, false);
                    break;

                case "Hydra Scale Piece":
                    Core.HuntMonster("hydrachallenge", "Hydra Head 25", req.Name, quant, isTemp: false, true);

                    break;

                case "Mana Golem's Core":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("elemental", "Mana Golem", "Mana Golem's Core", isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Celestial Quintessence":
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster("CelestialPast", "Blessed Bear", req.Name, quant, isTemp: false);
                        Core.HuntMonster("CelestialPast", "Blessed Deer", req.Name, quant, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Eternity Blade":
                    Core.EquipClass(ClassType.Solo);
                    Core.EnsureAccept(3485);
                    Core.HuntMonster("towerofdoom10", "Slugbutter", "Eternity Blade");
                    Core.EnsureComplete(3485);
                    break;
                    #endregion

            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("93840", "Portal to the Grand Inquisitor", "Mode: [select] only\nShould the bot buy \"Portal to the Grand Inquisitor\" ?", false),
        new Option<bool>("93850", "Portal to Darkon", "Mode: [select] only\nShould the bot buy \"Portal to Darkon\" ?", false),
        new Option<bool>("93865", "Portal to The Beast", "Mode: [select] only\nShould the bot buy \"Portal to The Beast\" ?", false),
        new Option<bool>("93861", "Portal to 5-Headed Dracolich", "Mode: [select] only\nShould the bot buy \"Portal to 5-Headed Dracolich\" ?", false),
        new Option<bool>("93858", "Portal to the Red Dragon", "Mode: [select] only\nShould the bot buy \"Portal to the Red Dragon\" ?", false),
        new Option<bool>("93849", "Portal to Shadowfall Fortress", "Mode: [select] only\nShould the bot buy \"Portal to Shadowfall Fortress\" ?", false),
        new Option<bool>("93852", "Portal to Swordhaven Castle", "Mode: [select] only\nShould the bot buy \"Portal to Swordhaven Castle\" ?", false),
        new Option<bool>("93855", "Portal to 3-Headed Hydra (Challenge)", "Mode: [select] only\nShould the bot buy \"Portal to 3-Headed Hydra (Challenge)\" ?", false),
        new Option<bool>("93843", "Portal to Kathool", "Mode: [select] only\nShould the bot buy \"Portal to Kathool\" ?", false),
        new Option<bool>("93846", "Portal to the Mana Golem", "Mode: [select] only\nShould the bot buy \"Portal to the Mana Golem\" ?", false),
        new Option<bool>("93848", "Portal to Escherion", "Mode: [select] only\nShould the bot buy \"Portal to Escherion\" ?", false),
        new Option<bool>("93868", "Portal to Chaos King Alteon (Challenge)", "Mode: [select] only\nShould the bot buy \"Portal to Chaos King Alteon (Challenge)\" ?", false),
        new Option<bool>("93836", "Portal to Azalith", "Mode: [select] only\nShould the bot buy \"Portal to Azalith\" ?", false),
        new Option<bool>("93845", "Portal to Doom Vault", "Mode: [select] only\nShould the bot buy \"Portal to Doom Vault\" ?", false),
        new Option<bool>("93866", "Portal to Slugbutter", "Mode: [select] only\nShould the bot buy \"Portal to Slugbutter\" ?", false),
        new Option<bool>("93847", "Portal to Nulgath's Hidden Grotto", "Mode: [select] only\nShould the bot buy \"Portal to Nulgath's Hidden Grotto\" ?", false),
   };
}
