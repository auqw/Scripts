/*
name: SoulForge Merge
description: This bot will farm the items belonging to the selected mode for the SoulForge Merge [577] in /underworld
tags: soulforge, merge, underworld, classy, skullseeker, legion, beast, within, cane, death, bane, obsidian, rock, solidified, soul, pride, horrid, cleaver, spine, shanks, ancient, undead, horns, breaker, warrior, shadow, katana, fatal, keraunos
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Legion/SwordMaster.cs
//cs_include Scripts/Legion/YamiNoRonin/CoreYnR.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class SoulForgeMerge
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

    private static CoreLegion Legion
    {
        get => _Legion ??= new CoreLegion();
        set => _Legion = value;
    }
    private static CoreLegion _Legion;
    private static CoreYnR YnR
    {
        get => _YnR ??= new CoreYnR();
        set => _YnR = value;
    }
    private static CoreYnR _YnR;

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
                "Legion Token",
                "Yami",
                "Folded Steel",
                "Platinum Paragon Medal",
                "Dage's Favor",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Legion.SoulForgeHammer();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("underworld", 577, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Legion Token":
                    Legion.FarmLegionToken(quant);
                    break;

                case "Yami":
                    YnR.Yami(quant);
                    break;

                case "Folded Steel":
                    YnR.FoldedSteel();
                    break;

                case "Platinum Paragon Medal":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem("underworld", 238, "Platinum Paragon Medal", quant);
                    break;

                case "Dage's Favor":
                    Legion.ApprovalAndFavor(0, quant);
                    break;

                case "Obsidian Rock":
                    Legion.ObsidianRock(quant);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "16421",
            "Classy SkullSeeker",
            "Mode: [select] only\nShould the bot buy \"Classy SkullSeeker\" ?",
            false
        ),
        new Option<bool>(
            "16422",
            "Legion Beast Within",
            "Mode: [select] only\nShould the bot buy \"Legion Beast Within\" ?",
            false
        ),
        new Option<bool>(
            "16423",
            "SkullSeeker Cane",
            "Mode: [select] only\nShould the bot buy \"SkullSeeker Cane\" ?",
            false
        ),
        new Option<bool>(
            "16424",
            "Classy SkullSeeker Cane",
            "Mode: [select] only\nShould the bot buy \"Classy SkullSeeker Cane\" ?",
            false
        ),
        new Option<bool>(
            "16425",
            "Death Bane of the Legion",
            "Mode: [select] only\nShould the bot buy \"Death Bane of the Legion\" ?",
            false
        ),
        new Option<bool>(
            "16409",
            "Obsidian Rock",
            "Mode: [select] only\nShould the bot buy \"Obsidian Rock\" ?",
            false
        ),
        new Option<bool>(
            "16411",
            "Solidified Soul",
            "Mode: [select] only\nShould the bot buy \"Solidified Soul\" ?",
            false
        ),
        new Option<bool>(
            "16467",
            "Pride of the Legion",
            "Mode: [select] only\nShould the bot buy \"Pride of the Legion\" ?",
            false
        ),
        new Option<bool>(
            "16534",
            "Legion Horrid Cleaver",
            "Mode: [select] only\nShould the bot buy \"Legion Horrid Cleaver\" ?",
            false
        ),
        new Option<bool>(
            "16535",
            "Legion Spine Shanks",
            "Mode: [select] only\nShould the bot buy \"Legion Spine Shanks\" ?",
            false
        ),
        new Option<bool>(
            "17275",
            "Ancient Undead Horns",
            "Mode: [select] only\nShould the bot buy \"Ancient Undead Horns\" ?",
            false
        ),
        new Option<bool>(
            "17276",
            "Ancient Undead Breaker",
            "Mode: [select] only\nShould the bot buy \"Ancient Undead Breaker\" ?",
            false
        ),
        new Option<bool>(
            "17277",
            "Dual Ancient Legion Axe",
            "Mode: [select] only\nShould the bot buy \"Dual Ancient Legion Axe\" ?",
            false
        ),
        new Option<bool>(
            "17278",
            "Ancient Undead Warrior",
            "Mode: [select] only\nShould the bot buy \"Ancient Undead Warrior\" ?",
            false
        ),
        new Option<bool>(
            "17343",
            "Ancient Undead Helm",
            "Mode: [select] only\nShould the bot buy \"Ancient Undead Helm\" ?",
            false
        ),
        new Option<bool>(
            "54104",
            "Shadow Katana Blade",
            "Mode: [select] only\nShould the bot buy \"Shadow Katana Blade\" ?",
            false
        ),
        new Option<bool>(
            "84610",
            "Fatal Keraunos",
            "Mode: [select] only\nShould the bot buy \"Fatal Keraunos\" ?",
            false
        ),
        new Option<bool>(
            "84611",
            "Dual Fatal Keraunos",
            "Mode: [select] only\nShould the bot buy \"Dual Fatal Keraunos\" ?",
            false
        ),
    };
}
