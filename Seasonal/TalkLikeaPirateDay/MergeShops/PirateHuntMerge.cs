/*
name: PirateHunt Merge
description: Farms items for the PirateHunt Merge [2621] in /piratehunt
tags: piratehunt, merge, novac, sal, pirate, privateer, morph, first, gold, cutlass, crew, awe
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/PirateHunt.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class PirateHuntMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static PirateHuntStory PirateHuntStory { get => _PirateHuntStory ??= new PirateHuntStory(); set => _PirateHuntStory = value; }
    private static PirateHuntStory _PirateHuntStory;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(new[]
        {
            "Belladonna's Flag", "Bourgeois' Flag", "Pirated Tech's Flag",
            "Merry Celeste's Flag", "Mercurius' Flag", "J6's Secret Hideout Map",
            "Novac Sal Pirate", "Novac Sal Privateer", "Novac Sal Pirate Morph",
            "Novac Sal Pirate Visage", "First Gold Cutlass", "Eyepatch", "Cutlass of Awe"
        });

        Core.SetOptions();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        if (!Core.isSeasonalMapActive("piratehunt"))
            return;

        PirateHuntStory.PirateHuntSaga();
        Adv.StartBuyAllMerge("piratehunt", 2621, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
            {
                Core.Logger("req is NULL");
                return;
            }


            switch (req.Name)
            {
                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger($"The bot hasn't been taught how to get {req.Name}." +
                                (shouldStop ? " Please report the issue." : " Skipping"),
                                messageBox: shouldStop, stopBot: shouldStop);
                    break;

                #region Flag Quests
                case "Belladonna's Flag":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10390); // Bumbling Bellamy
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.KillMonster("piratehunt", "r5", "Left", "Captain Bellamy", "Bellamy's Greasy Beard", 1, false);
                    Core.EnsureComplete(10390);
                    Core.CancelRegisteredQuests();
                    break;

                case "Bourgeois' Flag":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10392); // From the Top
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.KillMonster("piratehunt", "r7", "Left", "Captain Verich", "Captain Verich's Tricorn", 1, false);
                    Core.EnsureComplete(10392);
                    Core.CancelRegisteredQuests();
                    break;

                case "Pirated Tech's Flag":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10394); // Subversive Sailor
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.KillMonster("piratehunt", "r9", "Left", "Captain Chamfer", "Captain Chamfer's Mods", 1, false);
                    Core.EnsureComplete(10394);
                    Core.CancelRegisteredQuests();
                    break;

                case "Merry Celeste's Flag":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10396); // Salty Spirit
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.KillMonster("piratehunt", "r11", "Left", "Captain Haines", "Captain Haines' Soul", 1, false);
                    Core.EnsureComplete(10396);
                    Core.CancelRegisteredQuests();
                    break;

                case "Mercurius' Flag":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10398); // Mercurious
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster("piratehunt", "r13", "Left", "*", "Larunda's Counterfeit Amulet", 1, false);
                        Core.KillMonster("piratehunt", "r13", "Left", "*", "Mercurius' Funny Hat", 1, false);
                    }
                    Core.EnsureComplete(10398);
                    Core.CancelRegisteredQuests();
                    break;

                #endregion

                #region Cutlass of Awe Quest
                case "Cutlass of Awe":
                case "Dual Cutlass of Awe":
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
                #endregion

                #region Direct Purchases
                case "Gold Voucher 100k":
                    Farm.Voucher(req.Name, req.Quantity);
                    break;

                case "J6's Secret Hideout Map":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.Name);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster("j6", "Sketchy Dragon", req.Name, quant, isTemp: false);
                    break;
                #endregion

                #region Member-only check
                case "Novac Sal Privateer":
                case "Novac Sal Pirate Morph":
                case "Novac Sal Pirate Visage":
                    if (!Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership. Skipping.");
                        break;
                    }
                    goto default;
                    #endregion
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("94712", "Novac Sal Pirate", "Mode: [select] only\nShould the bot buy \"Novac Sal Pirate\" ?", false),
        new Option<bool>("94713", "Novac Sal Privateer", "Mode: [select] only\nShould the bot buy \"Novac Sal Privateer\" ?", false),
        new Option<bool>("94714", "Novac Sal Pirate Morph", "Mode: [select] only\nShould the bot buy \"Novac Sal Pirate Morph\" ?", false),
        new Option<bool>("94715", "Novac Sal Pirate Visage", "Mode: [select] only\nShould the bot buy \"Novac Sal Pirate Visage\" ?", false),
        new Option<bool>("94723", "First Gold Cutlass", "Mode: [select] only\nShould the bot buy \"First Gold Cutlass\" ?", false),
        new Option<bool>("94724", "First Gold Cutlasses", "Mode: [select] only\nShould the bot buy \"First Gold Cutlasses\" ?", false),
        new Option<bool>("94716", "Novac Sal Crew Morph", "Mode: [select] only\nShould the bot buy \"Novac Sal Crew Morph\" ?", false),
        new Option<bool>("94717", "Novac Sal Crew Visage", "Mode: [select] only\nShould the bot buy \"Novac Sal Crew Visage\" ?", false),
        new Option<bool>("94727", "Pirate's First Gold", "Mode: [select] only\nShould the bot buy \"Pirate's First Gold\" ?", false),
        new Option<bool>("95375", "First Mate o' Awe", "Mode: [select] only\nShould the bot buy \"First Mate o' Awe\" ?", false),
        new Option<bool>("95376", "Captain o' Awe Cap", "Mode: [select] only\nShould the bot buy \"Captain o' Awe Cap\" ?", false),
        new Option<bool>("95377", "Captain o' Awe Locks", "Mode: [select] only\nShould the bot buy \"Captain o' Awe Locks\" ?", false),
        new Option<bool>("95378", "Captain o' Awe Patch", "Mode: [select] only\nShould the bot buy \"Captain o' Awe Patch\" ?", false),
        new Option<bool>("95379", "Captain o' Awe Eyepatch", "Mode: [select] only\nShould the bot buy \"Captain o' Awe Eyepatch\" ?", false),
        new Option<bool>("95382", "Sheathed Rapier of Awe", "Mode: [select] only\nShould the bot buy \"Sheathed Rapier of Awe\" ?", false),
        new Option<bool>("95386", "Dual Cutlass of Awe", "Mode: [select] only\nShould the bot buy \"Dual Cutlass of Awe\" ?", false),
        new Option<bool>("95387", "Rapier of Awe", "Mode: [select] only\nShould the bot buy \"Rapier of Awe\" ?", false),
        new Option<bool>("95388", "Rapiers of Awe", "Mode: [select] only\nShould the bot buy \"Rapiers of Awe\" ?", false),
    };
}
