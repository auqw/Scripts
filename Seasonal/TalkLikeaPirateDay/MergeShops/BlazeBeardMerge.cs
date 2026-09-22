/*
name: Blazebeard Merge
description: Farms the Blazebeard Merge [108] in /blazebeard.
tags: blazebeard, merge, blazebeard, merge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class BlazebeardMerge
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
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange([
            "Alpha Pirate Class Token",
            "Blaze Gem",
            "Explorer Pistol",
            "Pirate Class Token",
            "Pirate Mage Token",
        ]);
        Core.SetOptions();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Adv.StartBuyAllMerge("blazebeard", 108, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
                return;

            switch (req.Name)
            {
                case "Alpha Pirate Class Token":
                    Core.FarmingLogger(req.Name, quant);
                    Core.Logger("Cannot automate Classic Alpha Pirate [22] for New Alpha Pirate Class [4552] from this map.", messageBox: true, stopBot: true);
                    Core.RegisterQuests(4552);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster("blazebeard", "Undead Pirate", "Rusty Nail", 1);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Blaze Gem": // #FROM CASE STORAGE
                case "Explorer Pistol": // #FROM CASE STORAGE
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("ManaCannon", "Blazebeard", req.Name, isTemp: false);
                    break;
                case "Pirate Class Token":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(252, 1)) // Classic Pirate
                    {
                        while (!Bot.ShouldExit && !Core.CheckInventory(252, 1)) // Classic Pirate
                        {
                            Core.Logger("Recursive quest dependency detected at New Pirate Class [4551].", messageBox: true, stopBot: true);
                            Bot.Wait.ForPickup(252);
                        }
                        Core.EnsureAccept(4551);
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster("blazebeard", "Undead Pirate", "Rusty Nail", 1);
                        Core.EnsureComplete(4551);
                        Bot.Wait.ForPickup(252);
                    }
                    Core.RegisterQuests(4551);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster("blazebeard", "Undead Pirate", "Rusty Nail", 1);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Pirate Mage Token":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(Core.IsMember ? 4531 : 4528);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster("blazebeard", "Pirate Crew", "Crew Member Beaten", 10);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger($"The bot hasn't been taught how to get {req.Name}.", messageBox: shouldStop, stopBot: shouldStop);
                    break;
            }
        }
    }

    public List<IOption> Select =
    [
        new Option<bool>("31158", "Crimson Pirate Mage", "Mode: [select] only\nShould the bot buy \"Crimson Pirate Mage\" ?", false),
        new Option<bool>("35079", "Platinum Pirate Mage", "Mode: [select] only\nShould the bot buy \"Platinum Pirate Mage\" ?", false),
        new Option<bool>("31165", "Crimson Pirate Mage Weapons", "Mode: [select] only\nShould the bot buy \"Crimson Pirate Mage Weapons\" ?", false),
        new Option<bool>("31166", "Pirate Mage Side Cutlass", "Mode: [select] only\nShould the bot buy \"Pirate Mage Side Cutlass\" ?", false),
        new Option<bool>("31281", "Alpha Pirate", "Mode: [select] only\nShould the bot buy \"Alpha Pirate\" ?", false),
        new Option<bool>("31176", "Pirate", "Mode: [select] only\nShould the bot buy \"Pirate\" ?", false),
        new Option<bool>("31227", "Dual Great Golden Pistols", "Mode: [select] only\nShould the bot buy \"Dual Great Golden Pistols\" ?", false),
        new Option<bool>("31178", "Dual Pirate Mage Flintlock", "Mode: [select] only\nShould the bot buy \"Dual Pirate Mage Flintlock\" ?", false),
        new Option<bool>("31179", "Dual Pirate Mage Cutlasses", "Mode: [select] only\nShould the bot buy \"Dual Pirate Mage Cutlasses\" ?", false),
        new Option<bool>("31180", "Pirate Mage Flintlock and Cutlass", "Mode: [select] only\nShould the bot buy \"Pirate Mage Flintlock and Cutlass\" ?", false),
        new Option<bool>("31170", "Pirate Mage Flintlock", "Mode: [select] only\nShould the bot buy \"Pirate Mage Flintlock\" ?", false),
        new Option<bool>("31224", "Golden Explorer Pistol", "Mode: [select] only\nShould the bot buy \"Golden Explorer Pistol\" ?", false),
        new Option<bool>("31225", "Great Golden Explorer Pistol", "Mode: [select] only\nShould the bot buy \"Great Golden Explorer Pistol\" ?", false),
        new Option<bool>("31159", "Crimson Pirate Mage Hood", "Mode: [select] only\nShould the bot buy \"Crimson Pirate Mage Hood\" ?", false),
        new Option<bool>("31160", "Crimson Pirate Mage Tricorn", "Mode: [select] only\nShould the bot buy \"Crimson Pirate Mage Tricorn\" ?", false),
        new Option<bool>("31161", "Crimson Pirate Mage Tricorn Locks", "Mode: [select] only\nShould the bot buy \"Crimson Pirate Mage Tricorn Locks\" ?", false),
        new Option<bool>("31162", "Crimson Pirate Mage Hat", "Mode: [select] only\nShould the bot buy \"Crimson Pirate Mage Hat\" ?", false),
        new Option<bool>("31164", "Crimson Pirate Mage Locks", "Mode: [select] only\nShould the bot buy \"Crimson Pirate Mage Locks\" ?", false),
        new Option<bool>("35081", "Platinum Pirate Mage Tricorn Locks", "Mode: [select] only\nShould the bot buy \"Platinum Pirate Mage Tricorn Locks\" ?", false),
        new Option<bool>("35080", "Platinum Pirate Mage Tricorn", "Mode: [select] only\nShould the bot buy \"Platinum Pirate Mage Tricorn\" ?", false),
        new Option<bool>("31172", "Pirate Mage SpellBook", "Mode: [select] only\nShould the bot buy \"Pirate Mage SpellBook\" ?", false),
        new Option<bool>("31177", "Alpha Ferret Pirate", "Mode: [select] only\nShould the bot buy \"Alpha Ferret Pirate\" ?", false),
        new Option<bool>("31167", "Pirate Mage Skull Staff", "Mode: [select] only\nShould the bot buy \"Pirate Mage Skull Staff\" ?", false),
        new Option<bool>("31168", "Pirate Mage Harpoon", "Mode: [select] only\nShould the bot buy \"Pirate Mage Harpoon\" ?", false),
        new Option<bool>("31169", "Pirate Mage Flintlock Staff", "Mode: [select] only\nShould the bot buy \"Pirate Mage Flintlock Staff\" ?", false),
        new Option<bool>("31171", "Pirate Mage Cutlass", "Mode: [select] only\nShould the bot buy \"Pirate Mage Cutlass\" ?", false),
    ];
}