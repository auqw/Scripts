/*
name: Lothian Treasury Merge
description: This bot will farm the items belonging to the selected mode for the Lothian Treasury Merge [2475] in /queeniona
tags: lothian, treasury, merge, queeniona, dark, thunder, master, long, lightning, gloria, scarf, crownslayer, bananach, galvanic, tyrant, tyrants, stormkings, storm, bolt, sovereign, storms
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
//cs_include Scripts/Other/MergeShops/FelixsGildedGearMerge.cs
//cs_include Scripts/Other/MergeShops/LoughshineLootMerge.cs
//cs_include Scripts/Other/MergeShops/LiaTaraHillLootMerge.cs
//cs_include Scripts/Other/MergeShops/ColdThunderMerge.cs
//cs_include Scripts/Ultras/QueenIona.cs
//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class LothianTreasuryMerge
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

    private static CoreAOR AOR
    {
        get => _AOR ??= new CoreAOR();
        set => _AOR = value;
    }
    private static CoreAOR _AOR;
    private static FelixsGildedGearMerge FGGM
    {
        get => _FGGM ??= new FelixsGildedGearMerge();
        set => _FGGM = value;
    }
    private static FelixsGildedGearMerge _FGGM;
    private static LoughshineLootMerge LLM
    {
        get => _LLM ??= new LoughshineLootMerge();
        set => _LLM = value;
    }
    private static LoughshineLootMerge _LLM;
    private static LiaTaraHillLootMerge LTHLM
    {
        get => _LTHLM ??= new LiaTaraHillLootMerge();
        set => _LTHLM = value;
    }
    private static LiaTaraHillLootMerge _LTHLM;

    private static ColdThunderMerge CTM
    {
        get => _CTM ??= new ColdThunderMerge();
        set => _CTM = value;
    }
    private static ColdThunderMerge _CTM;

    private static QueenIona QI
    {
        get => _QI ??= new QueenIona();
        set => _QI = value;
    }
    private static QueenIona _QI;


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
                "Lothian's Lightning",
                "Skye's Lightning",
                "Dark Thunder Master Locks",
                "Dark Lightning Gloria",
                "Skye Nobility Sash",
                "Priestess Eire's Cletiné",
                "Skye Warden of the East",
                "Skye Warden of the West",
                "Skye Warden of the South",
                "Queen Iona's Royal Attire",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        AOR.TerminaTemple(coldThunder: true);
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("queeniona", 2475, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Lothian's Lightning":
                case "Dark Thunder Master Locks":
                case "Dark Lightning Gloria":
                case "Skye Nobility Sash":
                    Core.FarmingLogger(req.Name, quant);
                    QI.Fight(req.Name, req.Quantity, true);
                    break;

                case "Skye's Lightning":
                case "Priestess Eire's Cletiné":
                    Core.FarmingLogger(req.Name, quant);
                    AOR.ColdThunderBoss(req.Name, quant, false);
                    break;

                case "Skye Warden of the East":
                    Core.FarmingLogger(req.Name, quant);
                    FGGM.BuyAllMerge(req.Name);
                    break;

                case "Skye Warden of the West":
                    Core.FarmingLogger(req.Name, quant);
                    LLM.BuyAllMerge(req.Name);
                    break;

                case "Skye Warden of the South":
                    Core.FarmingLogger(req.Name, quant);
                    LTHLM.BuyAllMerge(req.Name);
                    break;

                case "Queen Iona's Royal Attire":
                    Core.FarmingLogger(req.Name, quant);
                    CTM.BuyAllMerge(req.Name);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "86834",
            "Dark Thunder Master",
            "Mode: [select] only\nShould the bot buy \"Dark Thunder Master\" ?",
            false
        ),
        new Option<bool>(
            "86837",
            "Dark Thunder Master Long Locks",
            "Mode: [select] only\nShould the bot buy \"Dark Thunder Master Long Locks\" ?",
            false
        ),
        new Option<bool>(
            "86841",
            "Dark Lightning Gloria and Scarf",
            "Mode: [select] only\nShould the bot buy \"Dark Lightning Gloria and Scarf\" ?",
            false
        ),
        new Option<bool>(
            "87926",
            "Crownslayer Bananach",
            "Mode: [select] only\nShould the bot buy \"Crownslayer Bananach\" ?",
            false
        ),
        new Option<bool>(
            "86844",
            "Galvanic Tyrant",
            "Mode: [select] only\nShould the bot buy \"Galvanic Tyrant\" ?",
            false
        ),
        new Option<bool>(
            "86845",
            "Galvanic Tyrants",
            "Mode: [select] only\nShould the bot buy \"Galvanic Tyrants\" ?",
            false
        ),
        new Option<bool>(
            "87950",
            "StormKing's Storm Bolt",
            "Mode: [select] only\nShould the bot buy \"StormKing's Storm Bolt\" ?",
            false
        ),
        new Option<bool>(
            "87713",
            "Sovereign of Storms",
            "Mode: [select] only\nShould the bot buy \"Sovereign of Storms\" ?",
            false
        ),
    };
}
