/*
name: Glacial Tomb Merge
description: This bot will farm the items belonging to the selected mode for the Glacial Tomb Merge [2377] in /glacetomb
tags: glacial, tomb, merge, glacetomb, arctic, necrodraugr, scholar, nether, morph, blessed, frostguarde, resonating, polaris, rapier, rapiers, awakening, frost, shatter, spear, lumen, ice, bright, astromancer, shillelagh
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/Glacera.cs
//cs_include Scripts/Seasonal/Frostvale/Story/CoreFrostvale.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class GlacialTombMerge
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
    private static CoreFrostvale Frostvale
    {
        get => _Frostvale ??= new CoreFrostvale();
        set => _Frostvale = value;
    }
    private static CoreFrostvale _Frostvale;
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
            new[]
            {
                "Fimbul's Frost",
                "Frostguarde Blade",
                "Frostguarde Blades",
                "Polaris Duelist Rapier",
                "Polaris Duelist Rapiers",
                "Frost Shatter Spear",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Frostvale.GlaceTomb();

        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("glacetomb", 2377, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Fimbul's Frost":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9507);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster("glacetomb", "Draugr", "Frozen Marrow", 8);
                        Core.HuntMonster("glacetomb", "Snow Fairy", "Crystalline Wings", 8);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonsterMapID("glacetomb", 7, "Valedictorian Speech");
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Frostguarde Blade":
                case "Frostguarde Blades":
                case "Polaris Duelist Rapiers":
                case "Polaris Duelist Rapier":
                case "Frost Shatter Spear":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID("glacetomb", 7, req.Name, 1, req.Temp);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "82181",
            "Arctic Necro-Draugr",
            "Mode: [select] only\nShould the bot buy \"Arctic Necro-Draugr\" ?",
            false
        ),
        new Option<bool>(
            "82182",
            "Scholar of the Nether",
            "Mode: [select] only\nShould the bot buy \"Scholar of the Nether\" ?",
            false
        ),
        new Option<bool>(
            "82183",
            "Scholar of the Nether Morph",
            "Mode: [select] only\nShould the bot buy \"Scholar of the Nether Morph\" ?",
            false
        ),
        new Option<bool>(
            "67403",
            "Blessed Frostguarde Blade",
            "Mode: [select] only\nShould the bot buy \"Blessed Frostguarde Blade\" ?",
            false
        ),
        new Option<bool>(
            "67404",
            "Blessed Frostguarde Blades",
            "Mode: [select] only\nShould the bot buy \"Blessed Frostguarde Blades\" ?",
            false
        ),
        new Option<bool>(
            "67405",
            "Resonating Polaris Rapier",
            "Mode: [select] only\nShould the bot buy \"Resonating Polaris Rapier\" ?",
            false
        ),
        new Option<bool>(
            "67406",
            "Resonating Polaris Rapiers",
            "Mode: [select] only\nShould the bot buy \"Resonating Polaris Rapiers\" ?",
            false
        ),
        new Option<bool>(
            "67410",
            "Awakening Frost Shatter Spear",
            "Mode: [select] only\nShould the bot buy \"Awakening Frost Shatter Spear\" ?",
            false
        ),
        new Option<bool>(
            "42494",
            "Lumen Ice Staff",
            "Mode: [select] only\nShould the bot buy \"Lumen Ice Staff\" ?",
            false
        ),
        new Option<bool>(
            "27476",
            "Bright Astromancer Polearm",
            "Mode: [select] only\nShould the bot buy \"Bright Astromancer Polearm\" ?",
            false
        ),
        new Option<bool>(
            "82479",
            "Lumen Ice Shillelagh",
            "Mode: [select] only\nShould the bot buy \"Lumen Ice Shillelagh\" ?",
            false
        ),
    };
}
