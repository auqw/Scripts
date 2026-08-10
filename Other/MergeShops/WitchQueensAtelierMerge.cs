/*
name: Witch Queen's Atelier Merge
description: This script will farm the items required to get all the items from the Witch Queen's Atelier Merge Shop.
tags: witch, queen, atelier, merge, birgittaspire, talia, galilera, starlight, chaser, visionary, astrolabe
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;
using System.Collections.Generic;

public class WitchQueensAtelierMerge
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
    public string[] MultiOptions = { "Generic", "Select", "ArmySetup" };
    public string OptionsStorage = "WitchQueensAtelierMerge";
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Gold Voucher 100k", "Witch Princess' Hat", "Starlight Chaser's Morph", "Starlight Chaser's Visage", "Crystalized Darkovian Tear", "Witch Princess' Astrolabe", "Galilera" });
        Core.SetOptions();

        Core.Logger($"[Debug] armyMode is currently: {Bot.Config.Get<bool>("ArmySetup", "armyMode")}");

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Adv.StartBuyAllMerge("birgittaspire", 2751, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Witch Princess' Hat":
                    Core.FarmingLogger(req.Name, quant);
                    if (Bot.Config.Get<bool>("ArmySetup", "armyMode"))
                        EquipArmyClass();
                    else
                        Core.EquipClass(ClassType.Solo);
                    
                    Core.RegisterQuests(10824);
                    if (Bot.Config.Get<bool>("ArmySetup", "armyMode"))
                    {
                        Core.Logger("Farming with Army");
                        Core.Join("birgittaspire", "r2", "Left");
                        Core.Sleep();
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            if (Bot.Player.Cell != "r2")
                            {
                                Core.Jump("r2", "Left");
                                Bot.Sleep(1000);
                            }
                            Bot.Combat.Attack("Witch Queen Talia");
                            Bot.Sleep(Core.ActionDelay);
                        }
                    }
                    else
                    {
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.KillMonster("birgittaspire", "r2", "Left", "Witch Queen Talia", req.Name, quant, isTemp: false, log: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Crystalized Darkovian Tear":
                case "Witch Princess' Astrolabe":
                    Core.FarmingLogger(req.Name, quant);
                    if (Bot.Config.Get<bool>("ArmySetup", "armyMode"))
                        EquipArmyClass();
                    else
                        Core.EquipClass(ClassType.Solo);
                    
                    if (Bot.Config.Get<bool>("ArmySetup", "armyMode"))
                    {
                        Core.Logger("Farming with Army");
                        Core.Join("birgittaspire", "r2", "Left");
                        Core.Sleep();
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            if (Bot.Player.Cell != "r2")
                            {
                                Core.Jump("r2", "Left");
                                Bot.Sleep(1000);
                            }
                            Bot.Combat.Attack("Witch Queen Talia");
                            Bot.Sleep(Core.ActionDelay);
                        }
                    }
                    else
                    {
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.KillMonster("birgittaspire", "r2", "Left", "Witch Queen Talia", req.Name, quant, isTemp: false, log: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    break;

                case "Gold Voucher 100k":
                    Farm.Voucher(req.Name, quant);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("Galilera", "Galilera", "Mode: [select] only\nShould the bot buy \"Galilera\" ?", false),
        new Option<bool>("Dual Galilera", "Dual Galilera", "Mode: [select] only\nShould the bot buy \"Dual Galilera\" ?", false),
        new Option<bool>("Noble Cavendish Witch", "Noble Cavendish Witch", "Mode: [select] only\nShould the bot buy \"Noble Cavendish Witch\" ?", false),
        new Option<bool>("Starlight Chaser's Hat", "Starlight Chaser's Hat", "Mode: [select] only\nShould the bot buy \"Starlight Chaser's Hat\" ?", false),
        new Option<bool>("Starlight Chaser's Morph", "Starlight Chaser's Morph", "Mode: [select] only\nShould the bot buy \"Starlight Chaser's Morph\" ?", false),
        new Option<bool>("Starlight Chaser's Visage", "Starlight Chaser's Visage", "Mode: [select] only\nShould the bot buy \"Starlight Chaser's Visage\" ?", false),
        new Option<bool>("Starlight Witch Hat", "Starlight Witch Hat", "Mode: [select] only\nShould the bot buy \"Starlight Witch Hat\" ?", false),
        new Option<bool>("Visionary Astrolabe", "Visionary Astrolabe", "Mode: [select] only\nShould the bot buy \"Visionary Astrolabe\" ?", false),
    };

    public List<IOption> ArmySetup = new()
    {
        new Option<bool>("armyMode", "Enable Army Mode", "Enable army mode for farming the boss", false),
        new Option<string>("Account1", "Account 1", "Format: Username,ClassName", ""),
        new Option<string>("Account2", "Account 2", "Format: Username,ClassName", ""),
        new Option<string>("Account3", "Account 3", "Format: Username,ClassName", ""),
        new Option<string>("Account4", "Account 4", "Format: Username,ClassName", ""),
        new Option<string>("Account5", "Account 5", "Format: Username,ClassName", ""),
        new Option<string>("Account6", "Account 6", "Format: Username,ClassName", ""),
        new Option<string>("Account7", "Account 7", "Format: Username,ClassName", "")
    };

    private void EquipArmyClass()
    {
        string username = Core.Username().Trim().ToLower();
        Core.Logger($"[ArmyClass] Finding class for '{username}'");

        for (int i = 1; i <= 7; i++)
        {
            string accOpt = Bot.Config.Get<string>("ArmySetup", $"Account{i}") ?? "";
            if (string.IsNullOrWhiteSpace(accOpt)) continue;

            string[] parts = accOpt.Split(',');
            if (parts.Length > 1)
            {
                string targetUser = parts[0].Trim().ToLower();
                string className = parts[1].Trim();

                Core.Logger($"[ArmyClass] Account{i}: Target='{targetUser}' Class='{className}'");

                if (targetUser == username)
                {
                    Core.Logger($"[ArmyClass] Match found! Equipping '{className}'");
                    Core.JumpWait();
                    if (Core.CheckInventory(className, toInv: false))
                        Core.Unbank(className);
                    Core.Equip(className);
                    Adv.SmartEnhance(className);
                    Bot.Skills.StartAdvanced(className, true);
                    return;
                }
            }
            else
            {
                Core.Logger($"[ArmyClass] Account{i} is invalid ('{accOpt}'). Format must be Username,ClassName");
            }
        }
        
        Core.Logger($"[ArmyClass] No match for '{username}'. Falling back to default Solo class.");
        Core.EquipClass(ClassType.Solo);
    }
}
