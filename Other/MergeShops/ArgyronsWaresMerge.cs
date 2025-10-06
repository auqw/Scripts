/*
name: Argyrons Wares Merge
description: This bot will farm the items belonging to the selected mode for the Argyrons Wares Merge [2626] in /abaddoncave
tags: argyrons, wares, merge, abaddoncave, arachnosapien, abaddon, hybrid, spineback, horned, tarsus, abdomen, broodmaster, venom, plague, xl, guest, abbadon, guard
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class ArgyronsWaresMerge
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
        Core.BankingBlackList.AddRange(new[] { "Dread Thread", "Abaddon Vertebrae", "Arachnosapien Visage", "Arachnosapien Locks", "Spineback Abaddon Carapace", "Spineback Abaddon Guest", "Spineback Abaddon Guard" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        // Always ensure drops are added
        Core.AddDrop("Dread Thread", "Abaddon Vertebrae", "Arachnosapien Visage", "Arachnosapien Locks", "Spineback Abaddon Carapace");

        Adv.StartBuyAllMerge("abaddoncave", 2626, findIngredients, buyOnlyThis, buyMode: buyMode);

        #region findIngredients
        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null) return;

            switch (req.Name)
            {
                case "Dread Thread":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10427); // Arach-kids
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster("abaddoncave", "Cursed Dreadspider", "Dreadspider Carapace", 9);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Abaddon Vertebrae":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10428); // Arachnosapien
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster("abaddoncave", "Spineback Abbadon", "Abaddon Carapace");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Spineback Abaddon Guard":
                case "Spineback Abaddon Guest":
                case "Spineback Abaddon Carapace":
                case "Arachnosapien Locks":
                case "Arachnosapien Visage":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("abaddoncave", "Spineback Abbadon", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;


                case "Gold Voucher 100k":
                    Farm.Voucher(req.Name, req.Quantity);
                    break;

                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger($"The bot hasn't been taught how to get {req.Name}." + (shouldStop ? " Please report the issue." : " Skipping"), messageBox: shouldStop, stopBot: shouldStop);
                    break;
            }
        }
        #endregion
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("91172", "Arachnosapien", "Mode: [select] only\nShould the bot buy \"Arachnosapien\" ?", false),
        new Option<bool>("91173", "Abaddon Hybrid", "Mode: [select] only\nShould the bot buy \"Abaddon Hybrid\" ?", false),
        new Option<bool>("91175", "Spineback Abaddon Helm", "Mode: [select] only\nShould the bot buy \"Spineback Abaddon Helm\" ?", false),
        new Option<bool>("91176", "Arachnosapien Helm", "Mode: [select] only\nShould the bot buy \"Arachnosapien Helm\" ?", false),
        new Option<bool>("91177", "Spineback Abaddon Visage", "Mode: [select] only\nShould the bot buy \"Spineback Abaddon Visage\" ?", false),
        new Option<bool>("91178", "Horned Arachnosapien Visage", "Mode: [select] only\nShould the bot buy \"Horned Arachnosapien Visage\" ?", false),
        new Option<bool>("91181", "Horned Arachnosapien Locks", "Mode: [select] only\nShould the bot buy \"Horned Arachnosapien Locks\" ?", false),
        new Option<bool>("91183", "Spineback Abaddon Tarsus", "Mode: [select] only\nShould the bot buy \"Spineback Abaddon Tarsus\" ?", false),
        new Option<bool>("91185", "Spineback Abaddon Abdomen", "Mode: [select] only\nShould the bot buy \"Spineback Abaddon Abdomen\" ?", false),
        new Option<bool>("91186", "Broodmaster Venom", "Mode: [select] only\nShould the bot buy \"Broodmaster Venom\" ?", false),
        new Option<bool>("91187", "Dual Broodmaster Venom", "Mode: [select] only\nShould the bot buy \"Dual Broodmaster Venom\" ?", false),
        new Option<bool>("91188", "Broodmaster Plague", "Mode: [select] only\nShould the bot buy \"Broodmaster Plague\" ?", false),
        new Option<bool>("91189", "Dual Broodmaster Plague", "Mode: [select] only\nShould the bot buy \"Dual Broodmaster Plague\" ?", false),
        new Option<bool>("91195", "XL Spineback Abaddon Guest", "Mode: [select] only\nShould the bot buy \"XL Spineback Abaddon Guest\" ?", false),
        new Option<bool>("91196", "XL Spineback Abbadon Guard", "Mode: [select] only\nShould the bot buy \"XL Spineback Abbadon Guard\" ?", false),
   };
}