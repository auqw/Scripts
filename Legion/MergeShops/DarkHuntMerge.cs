/*
name: Dark Hunt Merge
description: This bot will farm the items belonging to the selected mode for the Dark Hunt Merge [1102] in /darkfortress
tags: dark, hunt, merge, darkfortress, bone, crusher, knight, skull, centurion, cocar, dos, mortosvivos, dançarinos
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Legion/CoreLegion.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class DarkHuntMerge
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
                "Ultra Shifting Plane Gem",
                "Cocar dos Dançarinos",
                "Dage's Favor",
                "Dançarinos da Legião",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("darkfortress", 1102, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Ultra Shifting Plane Gem":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(
                        "darkfortress",
                        "r3",
                        "Left",
                        "*",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;

                case "Cocar dos Dançarinos":
                case "Dançarinos da Legião":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "darkfortress",
                        "Dage the Evil",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;

                case "Dage's Favor":
                    Core.FarmingLogger(req.Name, quant);
                    Legion.ApprovalAndFavor(0, quant);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "28667",
            "Bone Crusher Knight",
            "Mode: [select] only\nShould the bot buy \"Bone Crusher Knight\" ?",
            false
        ),
        new Option<bool>(
            "28668",
            "Skull Crusher Helm",
            "Mode: [select] only\nShould the bot buy \"Skull Crusher Helm\" ?",
            false
        ),
        new Option<bool>(
            "28669",
            "Centurion Bone Crusher",
            "Mode: [select] only\nShould the bot buy \"Centurion Bone Crusher\" ?",
            false
        ),
        new Option<bool>(
            "28670",
            "Centurion Bone Crusher Helm",
            "Mode: [select] only\nShould the bot buy \"Centurion Bone Crusher Helm\" ?",
            false
        ),
        new Option<bool>(
            "92242",
            "Cocar dos Mortos-Vivos",
            "Mode: [select] only\nShould the bot buy \"Cocar dos Mortos-Vivos\" ?",
            false
        ),
        new Option<bool>(
            "92241",
            "Dançarinos Mortos-Vivos",
            "Mode: [select] only\nShould the bot buy \"Dançarinos Mortos-Vivos\" ?",
            false
        ),
    };
}
