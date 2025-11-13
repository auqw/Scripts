/*
name: Atlas War Trophies Merge
description: This bot will farm the items belonging to the selected mode for the Atlas War Trophies Merge [2564] in /atlaspromenade
tags: atlas, war, trophies, merge, atlaspromenade, knight, cloak, axis, enchanted, barrensoul, psalm, ritualist, aeterna, adornment, cowl, horns, underworld, vestments, , horned, corpse, wax, candelabras, candelabra
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
//cs_include Scripts/Story/Legion/AtlasPromenade.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class AtlasWarTrophiesMerge
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

    private static AtlasPromenade AP
    {
        get => _AP ??= new AtlasPromenade();
        set => _AP = value;
    }
    private static AtlasPromenade _AP;
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
            new[]
            {
                "Broken Chain",
                "Atlas Crest",
                "Atlas Axis Blade",
                "Legion Token",
                "Barrensoul Psalm",
                "Underworld Ritualist Aeterna Adornment",
                "Underworld Ritualist Aeterna Hood",
                "Underworld Ritualist Aeterna Horns",
                "Underworld Ritualist Adornments",
                "Underworld Ritualist Horned Mask",
                "Underworld Ritualist Mask",
                "Underworld Ritualist Horned Hood",
                "Underworld Ritualist Adorned Hood",
                "Underworld Ritualist Horned Adornment",
                "Underworld Ritualist Adorned Mask",
                "Underworld Ritualist Hood",
                "Underworld Ritualist",
                "Pale Corpse Wax Candelabras",
                "Pale Corpse Wax Candelabra",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        AP.Storyline();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge(
            "atlaspromenade",
            2564,
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

                case "Broken Chain":
                    //10115 | You and Your Chain
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(
                            10115,
                            ("atlaspromenade", "Atlas Light Magus", ClassType.Farm),
                            ("atlaspromenade", "Wrath Guard", ClassType.Farm),
                            ("atlaspromenade", "Usurper Lord Slaine", ClassType.Solo)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Atlas Crest":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(
                        "atlaspromenade",
                        "Atlas Light Magus",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;

                case "Atlas Axis Blade":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(
                        "atlaspromenade",
                        "Atlas Knight",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;

                case "Legion Token":
                    Legion.FarmLegionToken(quant);
                    break;

                case "Barrensoul Psalm":
                case "Underworld Ritualist Aeterna Adornment":
                case "Underworld Ritualist Aeterna Hood":
                case "Underworld Ritualist Aeterna Horns":
                case "Underworld Ritualist Adornments":
                case "Underworld Ritualist Horned Mask":
                case "Underworld Ritualist Mask":
                case "Underworld Ritualist Horned Hood":
                case "Underworld Ritualist Adorned Hood":
                case "Underworld Ritualist Horned Adornment":
                case "Underworld Ritualist Adorned Mask":
                case "Underworld Ritualist Hood":
                case "Underworld Ritualist":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "atlaspromenade",
                        "Usurper Lord Slaine",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;

                case "Pale Corpse Wax Candelabras":
                case "Pale Corpse Wax Candelabra":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(
                        "atlaspromenade",
                        "Twisted Warrior",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "92306",
            "Atlas Knight",
            "Mode: [select] only\nShould the bot buy \"Atlas Knight\" ?",
            false
        ),
        new Option<bool>(
            "92307",
            "Atlas Knight Helm",
            "Mode: [select] only\nShould the bot buy \"Atlas Knight Helm\" ?",
            false
        ),
        new Option<bool>(
            "92308",
            "Atlas Knight Cloak",
            "Mode: [select] only\nShould the bot buy \"Atlas Knight Cloak\" ?",
            false
        ),
        new Option<bool>(
            "92311",
            "Atlas Axis Blades",
            "Mode: [select] only\nShould the bot buy \"Atlas Axis Blades\" ?",
            false
        ),
        new Option<bool>(
            "84308",
            "Enchanted Barrensoul Psalm",
            "Mode: [select] only\nShould the bot buy \"Enchanted Barrensoul Psalm\" ?",
            false
        ),
        new Option<bool>(
            "84307",
            "Enchanted Ritualist Aeterna Adornment",
            "Mode: [select] only\nShould the bot buy \"Enchanted Ritualist Aeterna Adornment\" ?",
            false
        ),
        new Option<bool>(
            "84306",
            "Enchanted Ritualist Aeterna Cowl",
            "Mode: [select] only\nShould the bot buy \"Enchanted Ritualist Aeterna Cowl\" ?",
            false
        ),
        new Option<bool>(
            "84305",
            "Enchanted Ritualist Aeterna Horns",
            "Mode: [select] only\nShould the bot buy \"Enchanted Ritualist Aeterna Horns\" ?",
            false
        ),
        new Option<bool>(
            "84304",
            "Enchanted Ritualist Aeterna Adornment",
            "Mode: [select] only\nShould the bot buy \"Enchanted Ritualist Aeterna Adornment\" ?",
            false
        ),
        new Option<bool>(
            "84303",
            "Enchanted Underworld Ritualist Vestments",
            "Mode: [select] only\nShould the bot buy \"Enchanted Underworld Ritualist Vestments\" ?",
            false
        ),
        new Option<bool>(
            "84302",
            "Enchanted Underworld Ritualist Mask + Horns",
            "Mode: [select] only\nShould the bot buy \"Enchanted Underworld Ritualist Mask + Horns\" ?",
            false
        ),
        new Option<bool>(
            "84301",
            "Enchanted Underworld Ritualist Hood",
            "Mode: [select] only\nShould the bot buy \"Enchanted Underworld Ritualist Hood\" ?",
            false
        ),
        new Option<bool>(
            "84300",
            "Underworld Ritualist Horned Cowl",
            "Mode: [select] only\nShould the bot buy \"Underworld Ritualist Horned Cowl\" ?",
            false
        ),
        new Option<bool>(
            "84299",
            "Enchanted Underworld Ritualist Adornment",
            "Mode: [select] only\nShould the bot buy \"Enchanted Underworld Ritualist Adornment\" ?",
            false
        ),
        new Option<bool>(
            "84298",
            "Enchanted Underworld Ritualist Horns",
            "Mode: [select] only\nShould the bot buy \"Enchanted Underworld Ritualist Horns\" ?",
            false
        ),
        new Option<bool>(
            "84297",
            "Enchanted Underworld Ritualist Mask",
            "Mode: [select] only\nShould the bot buy \"Enchanted Underworld Ritualist Mask\" ?",
            false
        ),
        new Option<bool>(
            "84296",
            "Underworld Ritualist Cowl",
            "Mode: [select] only\nShould the bot buy \"Underworld Ritualist Cowl\" ?",
            false
        ),
        new Option<bool>(
            "84295",
            "Enchanted Underworld Ritualist",
            "Mode: [select] only\nShould the bot buy \"Enchanted Underworld Ritualist\" ?",
            false
        ),
        new Option<bool>(
            "84648",
            "Corpse Wax Candelabras",
            "Mode: [select] only\nShould the bot buy \"Corpse Wax Candelabras\" ?",
            false
        ),
        new Option<bool>(
            "84647",
            "Corpse Wax Candelabra",
            "Mode: [select] only\nShould the bot buy \"Corpse Wax Candelabra\" ?",
            false
        ),
    };
}
