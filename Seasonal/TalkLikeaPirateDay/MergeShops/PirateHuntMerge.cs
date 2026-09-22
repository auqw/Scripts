/*
name: Pirate Plunder Merge
description: Farms the Pirate Plunder Merge [2621] in /piratehunt.
tags: piratehunt, merge, pirate, plunder, merge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class PiratePlunder
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static PirateClass Pirate
    {
        get => _Pirate ??= new PirateClass();
        set => _Pirate = value;
    }
    private static PirateClass _Pirate;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange([
            "Belladonna's Flag",
            "Bourgeois' Flag",
            "Cutlass of Awe",
            "Eyepatch",
            "First Gold Rapier",
            "J6's Secret Hideout Map",
            "Mercurius' Flag",
            "Merry Celeste's Flag",
            "Novac Sal Eyepatch Morph",
            "Novac Sal Eyepatch Visage",
            "Novac Sal Morph",
            "Novac Sal Visage",
            "Pirated Tech's Flag",
        ]);
        Core.SetOptions();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Adv.StartBuyAllMerge("piratehunt", 2621, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
                return;

            switch (req.Name)
            {
                case "Belladonna's Flag":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10390);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("piratehunt", "Captain Bellamy", "Bellamy's Greasy Beard", 1);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Bourgeois' Flag":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10392);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("piratehunt", "Captain Verich", "Captain Verich's Tricorn", 1);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Cutlass of Awe": // #FROM CASE STORAGE
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop("Cutlass of Awe");

                    while (!Bot.ShouldExit && !Core.CheckInventory("Cutlass of Awe", quant))
                    {
                        if (!Core.isCompletedBefore(10388) && !Core.CheckInventory("Cutlass of Awe"))
                            Core.EnsureAccept(10388);

                        if (!Core.CheckInventory("Cutlass of Awe Handle"))
                            Core.KillMonster("seakingkurok", "r2", "Left", "Sea King Gravefang", "Cutlass of Awe Handle", 1, false);

                        if (!Core.CheckInventory("Cutlass of Awe Hilt"))
                            Core.KillMonster("dragoncapital", "r8", "Left", "Empowered Scalebeard", "Cutlass of Awe Hilt", 1, false);

                        if (!Core.CheckInventory("Cutlass of Awe Blade"))
                            Core.KillMonster("kaijuwar", "r9", "Left", "Captain Kraylox", "Cutlass of Awe Blade", 1, false);

                        if (!Core.CheckInventory("Awe Binding Spell"))
                            Core.KillMonster("blazingbeach", "r8", "Left", "Magma Blazebeard", "Awe Binding Spell", 1, false);

                        if (Core.CheckInventory("Cutlass of Awe Handle") &&
                            Core.CheckInventory("Cutlass of Awe Hilt") &&
                            Core.CheckInventory("Cutlass of Awe Blade") &&
                            Core.CheckInventory("Awe Binding Spell"))
                        {
                            Core.EnsureComplete(10388);
                            Bot.Wait.ForPickup("Cutlass of Awe");
                        }
                    }
                    break;
                case "Eyepatch": // #FROM CASE STORAGE
                    if (!Core.CheckInventory(new[] { 22, 252, 31176 }, any: true))
                    {
                        Core.Logger($"Pirate class required to buy {req.Name}, we'll aquire it for you.");
                        Pirate.GetPirate();
                    }

                    Adv.BuyItem("pirates", 9, req.Name);
                    Bot.Wait.ForPickup(req.ID);
                    break;
                case "First Gold Rapier":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("piratehunt", "Captain Haines", req.Name, quant, req.Temp);
                    break;
                case "J6's Secret Hideout Map": // #FROM CASE STORAGE
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.Name);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster("j6", "Sketchy Dragon", req.Name, quant, isTemp: false);
                    break;
                case "Mercurius' Flag":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10398);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("piratehunt", "Captain Mercurius", "Mercurius' Funny Hat", 1);
                        Core.HuntMonster("piratehunt", "Dragonsworn Larunda", "Larunda's Counterfeit Amulet", 1);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Merry Celeste's Flag":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10396);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("piratehunt", "Captain Haines", "Captain Haines' Soul", 1);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Novac Sal Eyepatch Morph":
                case "Novac Sal Eyepatch Visage":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("piratehunt", "Captain Bellamy", req.Name, quant, req.Temp);
                    break;
                case "Novac Sal Morph":
                case "Novac Sal Visage":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("piratehunt", "Captain Verich", req.Name, quant, req.Temp);
                    break;
                case "Pirated Tech's Flag":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10394);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("piratehunt", "Captain Chamfer", "Captain Chamfer's Mods", 1);
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
        new Option<bool>("94712", "Novac Sal Pirate", "Mode: [select] only\nShould the bot buy \"Novac Sal Pirate\" ?", false),
        new Option<bool>("94713", "Novac Sal Privateer", "Mode: [select] only\nShould the bot buy \"Novac Sal Privateer\" ?", false),
        new Option<bool>("95375", "First Mate o' Awe", "Mode: [select] only\nShould the bot buy \"First Mate o' Awe\" ?", false),
        new Option<bool>("95382", "Sheathed Rapier of Awe", "Mode: [select] only\nShould the bot buy \"Sheathed Rapier of Awe\" ?", false),
        new Option<bool>("95386", "Dual Cutlass of Awe", "Mode: [select] only\nShould the bot buy \"Dual Cutlass of Awe\" ?", false),
        new Option<bool>("94724", "First Gold Cutlasses", "Mode: [select] only\nShould the bot buy \"First Gold Cutlasses\" ?", false),
        new Option<bool>("94727", "Pirate's First Gold", "Mode: [select] only\nShould the bot buy \"Pirate's First Gold\" ?", false),
        new Option<bool>("95388", "Rapiers of Awe", "Mode: [select] only\nShould the bot buy \"Rapiers of Awe\" ?", false),
        new Option<bool>("94716", "Novac Sal Crew Morph", "Mode: [select] only\nShould the bot buy \"Novac Sal Crew Morph\" ?", false),
        new Option<bool>("94717", "Novac Sal Crew Visage", "Mode: [select] only\nShould the bot buy \"Novac Sal Crew Visage\" ?", false),
        new Option<bool>("95376", "Captain o' Awe Cap", "Mode: [select] only\nShould the bot buy \"Captain o' Awe Cap\" ?", false),
        new Option<bool>("95377", "Captain o' Awe Locks", "Mode: [select] only\nShould the bot buy \"Captain o' Awe Locks\" ?", false),
        new Option<bool>("95378", "Captain o' Awe Patch", "Mode: [select] only\nShould the bot buy \"Captain o' Awe Patch\" ?", false),
        new Option<bool>("95379", "Captain o' Awe Eyepatch", "Mode: [select] only\nShould the bot buy \"Captain o' Awe Eyepatch\" ?", false),
        new Option<bool>("94714", "Novac Sal Pirate Morph", "Mode: [select] only\nShould the bot buy \"Novac Sal Pirate Morph\" ?", false),
        new Option<bool>("94715", "Novac Sal Pirate Visage", "Mode: [select] only\nShould the bot buy \"Novac Sal Pirate Visage\" ?", false),
        new Option<bool>("94723", "First Gold Cutlass", "Mode: [select] only\nShould the bot buy \"First Gold Cutlass\" ?", false),
        new Option<bool>("95387", "Rapier of Awe", "Mode: [select] only\nShould the bot buy \"Rapier of Awe\" ?", false),
    ];
}
