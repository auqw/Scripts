/*
name: Lae Wedding Merge
description: This bot will farm the items belonging to the selected mode for the Lae Wedding Merge [2496] in /laewed
tags: lae, wedding, merge, laewed, gothic, altar, tender, dark, wings, spear, undying, love, chalice, chalices, hollowborn, witch, witchs, broom, shadow
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/LaeWedding.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class LaeWeddingMerge
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

    private static LaeWedding LW
    {
        get => _LW ??= new LaeWedding();
        set => _LW = value;
    }
    private static LaeWedding _LW;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Spectral Memento" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        LW.Storyline();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("laewed", 2496, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Spectral Memento":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(
                            9954,
                            ("laewed", LW.UseableMonsters[2], ClassType.Farm),
                            ("laewed", LW.UseableMonsters[3], ClassType.Farm),
                            ("laewed", LW.UseableMonsters[5], ClassType.Solo)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "89286",
            "Gothic Altar Tender",
            "Mode: [select] only\nShould the bot buy \"Gothic Altar Tender\" ?",
            false
        ),
        new Option<bool>(
            "89287",
            "Gothic Altar Tender Hair",
            "Mode: [select] only\nShould the bot buy \"Gothic Altar Tender Hair\" ?",
            false
        ),
        new Option<bool>(
            "89288",
            "Gothic Altar Tender Locks",
            "Mode: [select] only\nShould the bot buy \"Gothic Altar Tender Locks\" ?",
            false
        ),
        new Option<bool>(
            "89289",
            "Dark Altar Wings",
            "Mode: [select] only\nShould the bot buy \"Dark Altar Wings\" ?",
            false
        ),
        new Option<bool>(
            "89290",
            "Dark Altar Spear",
            "Mode: [select] only\nShould the bot buy \"Dark Altar Spear\" ?",
            false
        ),
        new Option<bool>(
            "87518",
            "Undying Love Chalice",
            "Mode: [select] only\nShould the bot buy \"Undying Love Chalice\" ?",
            false
        ),
        new Option<bool>(
            "87519",
            "Undying Love Chalices",
            "Mode: [select] only\nShould the bot buy \"Undying Love Chalices\" ?",
            false
        ),
        new Option<bool>(
            "89273",
            "Hollowborn Witch",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Witch\" ?",
            false
        ),
        new Option<bool>(
            "89275",
            "Hollowborn Witch Visage",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Witch Visage\" ?",
            false
        ),
        new Option<bool>(
            "89277",
            "Hollowborn Witch's Wand",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Witch's Wand\" ?",
            false
        ),
        new Option<bool>(
            "89278",
            "Hollowborn Witch's Broom",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Witch's Broom\" ?",
            false
        ),
        new Option<bool>(
            "89280",
            "Hollowborn Witch Shadow Visage",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Witch Shadow Visage\" ?",
            false
        ),
    };
}
