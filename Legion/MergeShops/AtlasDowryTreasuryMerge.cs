/*
name: Atlas Dowry Treasury Merge
description: This bot will farm the items belonging to the selected mode for the Atlas Dowry Treasury Merge [2565] in /atlaskingdom
tags: atlas, dowry, treasury, merge, atlaskingdom, spell, book, light, mage, morph, wedding, attire, bell, rose, bush, arch, matrimony, pact, dias, souledge, armblades, armblade, shuriken, shadow, soulblades, soulblade, sheath, sheathed, master, shadows, veil
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
//cs_include Scripts/Story/Legion/AtlasPromenade.cs
//cs_include Scripts/Story/Legion/AtlasKingdom.cs
//cs_include Scripts/Legion/CoreLegion.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class AtlasDowryTreasuryMerge
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

    private static AtlasKingdom AK
    {
        get => _AK ??= new AtlasKingdom();
        set => _AK = value;
    }
    private static AtlasKingdom _AK;
    private static CoreLegion Legion
    {
        get => _Legion ??= new CoreLegion();
        set => _Legion = value;
    }
    private static CoreLegion _Legion;

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
            new[] { "Atlas Lion Pelt", "Atlas Crest", "Coelho's Tome", "Blue Dye", "Legion Token" }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        AK.Storyline();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("atlaskingdom", 2565, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Atlas Lion Pelt":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(
                            10126,
                            ("atlaskingdom", "Atlas Leo", ClassType.Solo),
                            ("atlaskingdom", "Atlas Elite", ClassType.Solo),
                            ("atlaskingdom", "Executioner Ladon", ClassType.Solo)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Atlas Crest":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(
                        "atlaskingdom",
                        "Atlas Light Magus",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;

                case "Coelho's Tome":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("atlaskingdom", "Coelho", req.Name, quant, false, false);
                    break;

                case "Blue Dye":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5898);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //QuarterMaster’s Supplies 5898
                        Core.HuntMonster(
                            "ashfallcamp",
                            "Lava Dragoblin",
                            "Supply Chest",
                            8,
                            log: false
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Legion Token":
                    Legion.FarmLegionToken(quant);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "92539",
            "Atlas Spell Book",
            "Mode: [select] only\nShould the bot buy \"Atlas Spell Book\" ?",
            false
        ),
        new Option<bool>(
            "92538",
            "Atlas Light Mage Visage",
            "Mode: [select] only\nShould the bot buy \"Atlas Light Mage Visage\" ?",
            false
        ),
        new Option<bool>(
            "92537",
            "Atlas Light Mage Morph",
            "Mode: [select] only\nShould the bot buy \"Atlas Light Mage Morph\" ?",
            false
        ),
        new Option<bool>(
            "92536",
            "Atlas Light Mage",
            "Mode: [select] only\nShould the bot buy \"Atlas Light Mage\" ?",
            false
        ),
        new Option<bool>(
            "92493",
            "Atlas Wedding Attire",
            "Mode: [select] only\nShould the bot buy \"Atlas Wedding Attire\" ?",
            false
        ),
        new Option<bool>(
            "92492",
            "Atlas Wedding Bell",
            "Mode: [select] only\nShould the bot buy \"Atlas Wedding Bell\" ?",
            false
        ),
        new Option<bool>(
            "92491",
            "Atlas Rose Bush",
            "Mode: [select] only\nShould the bot buy \"Atlas Rose Bush\" ?",
            false
        ),
        new Option<bool>(
            "92490",
            "Atlas Wedding Arch",
            "Mode: [select] only\nShould the bot buy \"Atlas Wedding Arch\" ?",
            false
        ),
        new Option<bool>(
            "92489",
            "Atlas Matrimony Pact",
            "Mode: [select] only\nShould the bot buy \"Atlas Matrimony Pact\" ?",
            false
        ),
        new Option<bool>(
            "92488",
            "Atlas Wedding Dias",
            "Mode: [select] only\nShould the bot buy \"Atlas Wedding Dias\" ?",
            false
        ),
        new Option<bool>(
            "92460",
            "Souledge Armblades",
            "Mode: [select] only\nShould the bot buy \"Souledge Armblades\" ?",
            false
        ),
        new Option<bool>(
            "92459",
            "Souledge Armblade",
            "Mode: [select] only\nShould the bot buy \"Souledge Armblade\" ?",
            false
        ),
        new Option<bool>(
            "92458",
            "Dual Souledge Shuriken",
            "Mode: [select] only\nShould the bot buy \"Dual Souledge Shuriken\" ?",
            false
        ),
        new Option<bool>(
            "92457",
            "Souledge Shuriken",
            "Mode: [select] only\nShould the bot buy \"Souledge Shuriken\" ?",
            false
        ),
        new Option<bool>(
            "92456",
            "Shadow Soulblades",
            "Mode: [select] only\nShould the bot buy \"Shadow Soulblades\" ?",
            false
        ),
        new Option<bool>(
            "92455",
            "Shadow Soulblade",
            "Mode: [select] only\nShould the bot buy \"Shadow Soulblade\" ?",
            false
        ),
        new Option<bool>(
            "92454",
            "Soulblade Sheath",
            "Mode: [select] only\nShould the bot buy \"Soulblade Sheath\" ?",
            false
        ),
        new Option<bool>(
            "92453",
            "Sheathed Soulblade",
            "Mode: [select] only\nShould the bot buy \"Sheathed Soulblade\" ?",
            false
        ),
        new Option<bool>(
            "92452",
            "Soulblade Master Mask",
            "Mode: [select] only\nShould the bot buy \"Soulblade Master Mask\" ?",
            false
        ),
        new Option<bool>(
            "92451",
            "Soulblade Master of Shadows",
            "Mode: [select] only\nShould the bot buy \"Soulblade Master of Shadows\" ?",
            false
        ),
        new Option<bool>(
            "92775",
            "Atlas Wedding Veil",
            "Mode: [select] only\nShould the bot buy \"Atlas Wedding Veil\" ?",
            false
        ),
    };
}
