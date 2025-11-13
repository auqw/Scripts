/*
name: ShadowFall Merge
description: This bot will farm the items belonging to the selected mode for the ShadowFall Merge [291] in /shadowfallwar
tags: shadowfall, merge, shadowfallwar, unholy, tormenter, great, grave, terror, terrors, lich, bane, creature, royal, crown, torch, darkness, bounded, ectoplasmic, skull, mace, doublesided, painblade, accoutrements, skelecommander, skelecommanders, necrotic, bonefang, doom
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class ShadowFallMerge
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
            new[]
            {
                "Necrotic Darkness Gem",
                "Tortured Darkness Gem",
                "Malignant Darkness Gem",
                "Ultimate Darkness Gem",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("shadowfallwar", 291, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Necrotic Darkness Gem":
                case "Tortured Darkness Gem":
                case "Malignant Darkness Gem":
                case "Ultimate Darkness Gem":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.KillMonster(
                        "shadowfallwar",
                        "Garden2",
                        "Left",
                        "*",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;
                #endregion
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "8547",
            "The Unholy",
            "Mode: [select] only\nShould the bot buy \"The Unholy\" ?",
            false
        ),
        new Option<bool>(
            "8548",
            "The Tormenter",
            "Mode: [select] only\nShould the bot buy \"The Tormenter\" ?",
            false
        ),
        new Option<bool>(
            "8549",
            "The Great Grave Blade",
            "Mode: [select] only\nShould the bot buy \"The Great Grave Blade\" ?",
            false
        ),
        new Option<bool>(
            "8551",
            "Grave Terror",
            "Mode: [select] only\nShould the bot buy \"Grave Terror\" ?",
            false
        ),
        new Option<bool>(
            "8552",
            "Dual Grave Terrors",
            "Mode: [select] only\nShould the bot buy \"Dual Grave Terrors\" ?",
            false
        ),
        new Option<bool>(
            "8553",
            "Lich Bane",
            "Mode: [select] only\nShould the bot buy \"Lich Bane\" ?",
            false
        ),
        new Option<bool>(
            "8556",
            "The Creature",
            "Mode: [select] only\nShould the bot buy \"The Creature\" ?",
            false
        ),
        new Option<bool>(
            "8557",
            "Hooded Creature",
            "Mode: [select] only\nShould the bot buy \"Hooded Creature\" ?",
            false
        ),
        new Option<bool>(
            "8561",
            "Royal Lich Crown",
            "Mode: [select] only\nShould the bot buy \"Royal Lich Crown\" ?",
            false
        ),
        new Option<bool>(
            "8583",
            "Torch of Darkness",
            "Mode: [select] only\nShould the bot buy \"Torch of Darkness\" ?",
            false
        ),
        new Option<bool>(
            "8571",
            "Bounded Blade",
            "Mode: [select] only\nShould the bot buy \"Bounded Blade\" ?",
            false
        ),
        new Option<bool>(
            "8588",
            "Ectoplasmic Skull Mace",
            "Mode: [select] only\nShould the bot buy \"Ectoplasmic Skull Mace\" ?",
            false
        ),
        new Option<bool>(
            "8596",
            "DoubleSided PainBlade",
            "Mode: [select] only\nShould the bot buy \"DoubleSided PainBlade\" ?",
            false
        ),
        new Option<bool>(
            "8599",
            "PainBlade Accoutrements",
            "Mode: [select] only\nShould the bot buy \"PainBlade Accoutrements\" ?",
            false
        ),
        new Option<bool>(
            "8600",
            "SkeleCommander",
            "Mode: [select] only\nShould the bot buy \"SkeleCommander\" ?",
            false
        ),
        new Option<bool>(
            "8601",
            "SkeleCommander's Cape",
            "Mode: [select] only\nShould the bot buy \"SkeleCommander's Cape\" ?",
            false
        ),
        new Option<bool>(
            "8602",
            "SkeleCommander's Helm",
            "Mode: [select] only\nShould the bot buy \"SkeleCommander's Helm\" ?",
            false
        ),
    };
}
