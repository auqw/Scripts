/*
name: Astro Seer Merge
description: This bot will farm the items belonging to the selected mode for the Astro Seer Merge [2557] in /extinction
tags: astro, seer, merge, extinction, flame, morph, stellar, quasar
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Extinction.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class AstroSeerMerge
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

    private static Extinction Ext
    {
        get => _Ext ??= new Extinction();
        set => _Ext = value;
    }
    private static Extinction _Ext;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Lemon" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Ext.StoryLine();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("extinction", 2557, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Lemon":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(
                            10054,
                            new[]
                            {
                                ("extinction", "Lard", ClassType.Farm),
                                ("extinction", "Gelatinous Slime", ClassType.Farm),
                                ("extinction", "SN.O.W.", ClassType.Solo),
                            }
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Lime":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, req.Quantity))
                    {
                        Core.EnsureAccept(10585);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("Ectocave", "Ektorax", "Regurgitated Key");
                        Core.EquipClass(ClassType.Farm);
                        Core.KillMonster("ectocave", "r1", "Left", "*", "Ecto Slime", 50);
                        Core.EnsureComplete(10585);
                    }
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "91637",
            "Astro Seer",
            "Mode: [select] only\nShould the bot buy \"Astro Seer\" ?",
            false
        ),
        new Option<bool>(
            "91638",
            "Astro Seer Flame Morph",
            "Mode: [select] only\nShould the bot buy \"Astro Seer Flame Morph\" ?",
            false
        ),
        new Option<bool>(
            "91639",
            "Astro Seer Visage",
            "Mode: [select] only\nShould the bot buy \"Astro Seer Visage\" ?",
            false
        ),
        new Option<bool>(
            "91640",
            "Astro Seer Morph",
            "Mode: [select] only\nShould the bot buy \"Astro Seer Morph\" ?",
            false
        ),
        new Option<bool>(
            "91641",
            "Astro Seer Flame Visage",
            "Mode: [select] only\nShould the bot buy \"Astro Seer Flame Visage\" ?",
            false
        ),
        new Option<bool>(
            "91642",
            "Stellar Quasar",
            "Mode: [select] only\nShould the bot buy \"Stellar Quasar\" ?",
            false
        ),
    };
}
