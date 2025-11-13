/*
name: Castle Whistler Spoils Merge
description: This bot will farm the items belonging to the selected mode for the Castle Whistler Spoils Merge [2607] in /castlewhistler
tags: castle, whistler, spoils, merge, castlewhistler, king, dark, antlered, armorial, crown, darkness, unknown, winters, dirges, midsummer, rhapsodies, pavane, fantasia, dirge, rhapsody
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class CastleWhistlerSpoilsMerge
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
            new[] { "Pigment Powder", "Armorial Crown", "Winter's Dirge", "Midsummer Rhapsody" }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Core.Logger("Good luck killing \"King of the Dark\"... he hurts after awhile :D.");

        //Only edit the map and shopID here
        Adv.StartBuyAllMerge(
            "castlewhistler",
            2607,
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


                // Pigment Powder (handled separately due to different quest IDs and requirements)
                case "Pigment Powder":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    // Unlock farming quest
                    if (!Bot.Quests.IsUnlocked(10337))
                    {
                        Core.Logger("unlocking farming quest for \"Pigment Powder\"");
                        Core.EnsureAccept(10336);
                        Core.HuntMonster("castlewhistler", "King of the Dark", "King's Pigment");
                        Core.EnsureComplete(10336);
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop("Pigment Powder");
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EnsureAccept(10337);
                        Core.HuntMonster("castlewhistler", "King of the Dark", "King's Varnish");
                        Core.EnsureComplete(10337);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                // Shared case for other castlewhistler drops
                case "Winter's Dirge":
                case "Midsummer Rhapsody":
                case "Armorial Crown":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop("Armorial Crown", "Winter's Dirge", "Midsummer Rhapsody");

                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(
                            "castlewhistler",
                            "King of the Dark",
                            req.Name,
                            quant,
                            req.Temp
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }

                    Core.CancelRegisteredQuests();
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "94864",
            "King of the Dark",
            "Mode: [select] only\nShould the bot buy \"King of the Dark\" ?",
            false
        ),
        new Option<bool>(
            "94866",
            "Antlered Armorial Crown",
            "Mode: [select] only\nShould the bot buy \"Antlered Armorial Crown\" ?",
            false
        ),
        new Option<bool>(
            "94867",
            "Darkness of the Unknown",
            "Mode: [select] only\nShould the bot buy \"Darkness of the Unknown\" ?",
            false
        ),
        new Option<bool>(
            "94869",
            "Winter's Dirges",
            "Mode: [select] only\nShould the bot buy \"Winter's Dirges\" ?",
            false
        ),
        new Option<bool>(
            "94871",
            "Midsummer Rhapsodies",
            "Mode: [select] only\nShould the bot buy \"Midsummer Rhapsodies\" ?",
            false
        ),
        new Option<bool>(
            "94872",
            "Pavane and Fantasia",
            "Mode: [select] only\nShould the bot buy \"Pavane and Fantasia\" ?",
            false
        ),
        new Option<bool>(
            "94873",
            "Dirge and Rhapsody",
            "Mode: [select] only\nShould the bot buy \"Dirge and Rhapsody\" ?",
            false
        ),
    };
}
