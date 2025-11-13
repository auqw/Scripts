/*
name: Magicians Merge
description: This bot will farm the items belonging to the selected mode for the Magicians Merge [2519] in /magician
tags: magicians, merge, magician, human, clock, oscillator, decay, erosion, nostalgia, face, countdown, scalpula, debris, scythe, creature, , wings, amp, tail, morph
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class MagiciansMerge
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
                "Hours Minutes Seconds",
                "Chrono Bauble",
                "Human Clock Face House Item",
                "Creature 10 Tail",
                "Creature 10 Half Wing",
                "Creature 10 Horns",
                "Creature 10 Horned Locks",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("magician", 2519, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Hours Minutes Seconds":
                case "Human Clock Face House Item":
                case "Creature 10 Tail":
                case "Creature 10 Half Wing":
                case "Creature 10 Horns":
                case "Creature 10 Horned Locks":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("magician", "Human Clock", req.Name, quant, req.Temp, false);
                    break;

                case "Chrono Bauble":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(10017, "magician", "Human Clock");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "90893",
            "Human Clock Oscillator",
            "Mode: [select] only\nShould the bot buy \"Human Clock Oscillator\" ?",
            false
        ),
        new Option<bool>(
            "90892",
            "Human Clock Decay",
            "Mode: [select] only\nShould the bot buy \"Human Clock Decay\" ?",
            false
        ),
        new Option<bool>(
            "90891",
            "Human Clock Erosion",
            "Mode: [select] only\nShould the bot buy \"Human Clock Erosion\" ?",
            false
        ),
        new Option<bool>(
            "90890",
            "Human Clock Nostalgia",
            "Mode: [select] only\nShould the bot buy \"Human Clock Nostalgia\" ?",
            false
        ),
        new Option<bool>(
            "90889",
            "Human Clock",
            "Mode: [select] only\nShould the bot buy \"Human Clock\" ?",
            false
        ),
        new Option<bool>(
            "90887",
            "Human Clock Face",
            "Mode: [select] only\nShould the bot buy \"Human Clock Face\" ?",
            false
        ),
        new Option<bool>(
            "90885",
            "Human Clock Countdown",
            "Mode: [select] only\nShould the bot buy \"Human Clock Countdown\" ?",
            false
        ),
        new Option<bool>(
            "85529",
            "Scalpula Debris Scythe",
            "Mode: [select] only\nShould the bot buy \"Scalpula Debris Scythe\" ?",
            false
        ),
        new Option<bool>(
            "85526",
            "Creature 10 Wings &amp; Tail",
            "Mode: [select] only\nShould the bot buy \"Creature 10 Wings &amp; Tail\" ?",
            false
        ),
        new Option<bool>(
            "85525",
            "Creature 10 Wings",
            "Mode: [select] only\nShould the bot buy \"Creature 10 Wings\" ?",
            false
        ),
        new Option<bool>(
            "85521",
            "Creature 10 Mask",
            "Mode: [select] only\nShould the bot buy \"Creature 10 Mask\" ?",
            false
        ),
        new Option<bool>(
            "85520",
            "Creature 10 Visage",
            "Mode: [select] only\nShould the bot buy \"Creature 10 Visage\" ?",
            false
        ),
        new Option<bool>(
            "85519",
            "Creature 10 Morph",
            "Mode: [select] only\nShould the bot buy \"Creature 10 Morph\" ?",
            false
        ),
        new Option<bool>(
            "85516",
            "Creature 10",
            "Mode: [select] only\nShould the bot buy \"Creature 10\" ?",
            false
        ),
    };
}
