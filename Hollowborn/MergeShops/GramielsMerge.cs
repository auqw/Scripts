/*
name: Gramiels Merge
description: This bot will farm the items belonging to the selected mode for the Gramiels Merge [2593] in /ultragramielhub
tags: gramiels, merge, ultragramielhub, celestial, gramiel, robes, gaze, halo, ascended, wings, grace, orb, orbs, divine, enoch, pet, battle, hollowborn, vindicator, spear, empowered, grandmaster, dawn, fortress, vindicators, chosen
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Hollowborn/Materials/HollowSoul.cs
//cs_include Scripts/Hollowborn/Materials/VindicatorBadge.cs
//cs_include Scripts/Hollowborn/Materials/DeathsPower.cs
//cs_include Scripts/Hollowborn/Materials/GraceOrb.cs
//cs_include Scripts/Hollowborn/Materials/GramielsEmblem.cs
//cs_include Scripts/Hollowborn/Materials/VindicatorCrest.cs
//cs_include Scripts/Story/Hollowborn/CoreHollowbornStory.cs
//cs_include Scripts/Hollowborn/MergeShops/DawnsanctumMerge.cs
//cs_include Scripts/Hollowborn/HollowbornVindicator(NonInsignia).cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class GramielsMerge
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

    private static DawnsanctumMerge DM
    {
        get => _DM ??= new DawnsanctumMerge();
        set => _DM = value;
    }
    private static DawnsanctumMerge _DM;
    private static HBVNonInsig HBV
    {
        get => _HBV ??= new HBVNonInsig();
        set => _HBV = value;
    }
    private static HBVNonInsig _HBV;

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
                "Trygve's Testament",
                "Fortress' Faith",
                "Tower's Trisagion",
                "Sanctum's Salvation",
                "Grandmaster Gramiel",
                "Grandmaster Gramiel Hair",
                "Celestial Gramiel Wings",
                "Gramiel the Graceful's Insignia",
                "Gramiel's Glorified Enoch",
                "Gramiel's Emblem",
                "Dawn Vindicator Castle",
                "Condensed Grace",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge(
            "ultragramielhub",
            2593,
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

                #region Items not setup

                case "Trygve's Testament":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster("trygve", "Gramiel", req.Name, quant, false, false);
                    break;

                case "Fortress' Faith":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(
                        "neofortress",
                        "Vindicator General",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;

                case "Tower's Trisagion":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(
                        "neotower",
                        "Vindicator Assassin",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;

                case "Sanctum's Salvation":
                case "Grandmaster Gramiel":
                case "Grandmaster Gramiel Hair":
                case "Celestial Gramiel Wings":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(
                        "dawnsanctum",
                        "Grandmaster Gramiel",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;

                case "Gramiel the Graceful's Insignia":
                    Core.Logger($"{req.Name} requires army, skipping...");
                    break;

                case "Gramiel's Glorified Enoch":
                case "Dawn Vindicator Castle":
                    DM.BuyAllMerge(req.Name);
                    break;

                case "Condensed Grace":
                    if (Bot.Quests.IsAvailable(10299))
                        HBV.GetClass(false, true, quant);
                    break;
                #endregion

                #region Known items

                case "Gramiel's Emblem":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "dawnsanctum",
                        "Celestial Gramiel",
                        req.Name,
                        quant,
                        isTemp: false
                    );
                    break;
                #endregion
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "94028",
            "Celestial Gramiel Robes",
            "Mode: [select] only\nShould the bot buy \"Celestial Gramiel Robes\" ?",
            false
        ),
        new Option<bool>(
            "94029",
            "Celestial Gramiel Armor",
            "Mode: [select] only\nShould the bot buy \"Celestial Gramiel Armor\" ?",
            false
        ),
        new Option<bool>(
            "94030",
            "Celestial Gramiel Gaze",
            "Mode: [select] only\nShould the bot buy \"Celestial Gramiel Gaze\" ?",
            false
        ),
        new Option<bool>(
            "94031",
            "Celestial Gramiel Halo",
            "Mode: [select] only\nShould the bot buy \"Celestial Gramiel Halo\" ?",
            false
        ),
        new Option<bool>(
            "94032",
            "Ascended Gramiel Wings",
            "Mode: [select] only\nShould the bot buy \"Ascended Gramiel Wings\" ?",
            false
        ),
        new Option<bool>(
            "94034",
            "Celestial Grace Orb",
            "Mode: [select] only\nShould the bot buy \"Celestial Grace Orb\" ?",
            false
        ),
        new Option<bool>(
            "94035",
            "Celestial Grace Orbs",
            "Mode: [select] only\nShould the bot buy \"Celestial Grace Orbs\" ?",
            false
        ),
        new Option<bool>(
            "94033",
            "Gramiel's Divine Enoch",
            "Mode: [select] only\nShould the bot buy \"Gramiel's Divine Enoch\" ?",
            false
        ),
        new Option<bool>(
            "94351",
            "Gramiel's Divine Enoch Pet",
            "Mode: [select] only\nShould the bot buy \"Gramiel's Divine Enoch Pet\" ?",
            false
        ),
        new Option<bool>(
            "94352",
            "Gramiel's Divine Enoch Battle Pet",
            "Mode: [select] only\nShould the bot buy \"Gramiel's Divine Enoch Battle Pet\" ?",
            false
        ),
        new Option<bool>(
            "94358",
            "Hollowborn Vindicator Armor",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Vindicator Armor\" ?",
            false
        ),
        new Option<bool>(
            "94359",
            "Hollowborn Vindicator Helm",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Vindicator Helm\" ?",
            false
        ),
        new Option<bool>(
            "94360",
            "Hollowborn Vindicator Wings",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Vindicator Wings\" ?",
            false
        ),
        new Option<bool>(
            "94361",
            "Hollowborn Vindicator Spear",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Vindicator Spear\" ?",
            false
        ),
        new Option<bool>(
            "94371",
            "Empowered Grandmaster Gramiel",
            "Mode: [select] only\nShould the bot buy \"Empowered Grandmaster Gramiel\" ?",
            false
        ),
        new Option<bool>(
            "94155",
            "Dawn Vindicator Fortress",
            "Mode: [select] only\nShould the bot buy \"Dawn Vindicator Fortress\" ?",
            false
        ),
        new Option<bool>(
            "94353",
            "Dawn Vindicator's Chosen",
            "Mode: [select] only\nShould the bot buy \"Dawn Vindicator's Chosen\" ?",
            false
        ),
        new Option<bool>(
            "94354",
            "Dawn Vindicator's Chosen Helm",
            "Mode: [select] only\nShould the bot buy \"Dawn Vindicator's Chosen Helm\" ?",
            false
        ),
        new Option<bool>(
            "94355",
            "Dawn Vindicator's Chosen Wings",
            "Mode: [select] only\nShould the bot buy \"Dawn Vindicator's Chosen Wings\" ?",
            false
        ),
        new Option<bool>(
            "94356",
            "Dawn Vindicator's Chosen Spear",
            "Mode: [select] only\nShould the bot buy \"Dawn Vindicator's Chosen Spear\" ?",
            false
        ),
        new Option<bool>(
            "94357",
            "Hollowborn Vindicator",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Vindicator\" ?",
            false
        ),
    };
}
