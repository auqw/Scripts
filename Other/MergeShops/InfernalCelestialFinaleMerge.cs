/*
name: Infernal Celestial Finale Merge
description: This bot will farm the items belonging to the selected mode for the Infernal Celestial Finale Merge [2562] in /infernaldianoia
tags: infernal, celestial, finale, merge, infernaldianoia, nightstar, crown, fallen, arthelyn, wings, dominion
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/CelestialPast.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/InfernalParadise.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/InfernalDianoia.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class InfernalCelestialFinaleMerge
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

    private static InfernalDianoia ID
    {
        get => _ID ??= new InfernalDianoia();
        set => _ID = value;
    }
    private static InfernalDianoia _ID;

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
            new[] { "Infernal Down", "Arthelyn's Oculus", "Life Spirit" }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        ID.Storyline();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge(
            "infernaldianoia",
            2562,
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

                case "Infernal Down":
                    InfernalDown(quant);
                    break;

                case "Arthelyn's Oculus":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "infernaldianoia",
                        "Fallen Arthelyn",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;

                case "Life Spirit":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "infernaldianoia",
                        "Avatar of Life",
                        req.Name,
                        quant,
                        false,
                        false
                    );
                    break;
            }
        }
    }

    public void InfernalDown(int quant = 100)
    {
        if (Core.CheckInventory("Infernal Down", quant))
            return;
        Core.FarmingLogger("Infernal Down", quant);
        Core.AddDrop("Infernal Down");
        while (!Bot.ShouldExit && !Core.CheckInventory("Infernal Down", quant))
        {
            Core.HuntMonsterQuest(
                10095,
                ("infernaldianoia", "Eudae", ClassType.Solo),
                ("infernaldianoia", "Avatar of Time", ClassType.Solo),
                ("infernaldianoia", "Aranx, Nightstar", ClassType.Solo)
            );
            Bot.Wait.ForPickup("Infernal Down");
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "92057",
            "Nightstar Crown",
            "Mode: [select] only\nShould the bot buy \"Nightstar Crown\" ?",
            false
        ),
        new Option<bool>(
            "92056",
            "Fallen Arthelyn Wings",
            "Mode: [select] only\nShould the bot buy \"Fallen Arthelyn Wings\" ?",
            false
        ),
        new Option<bool>(
            "92055",
            "Fallen Arthelyn Helm",
            "Mode: [select] only\nShould the bot buy \"Fallen Arthelyn Helm\" ?",
            false
        ),
        new Option<bool>(
            "92054",
            "Fallen Arthelyn",
            "Mode: [select] only\nShould the bot buy \"Fallen Arthelyn\" ?",
            false
        ),
        new Option<bool>(
            "92188",
            "Celestial Dominion Gauntlet",
            "Mode: [select] only\nShould the bot buy \"Celestial Dominion Gauntlet\" ?",
            false
        ),
        new Option<bool>(
            "92187",
            "Celestial Dominion Wings",
            "Mode: [select] only\nShould the bot buy \"Celestial Dominion Wings\" ?",
            false
        ),
        new Option<bool>(
            "92186",
            "Celestial Dominion Helm",
            "Mode: [select] only\nShould the bot buy \"Celestial Dominion Helm\" ?",
            false
        ),
        new Option<bool>(
            "92185",
            "Celestial Dominion",
            "Mode: [select] only\nShould the bot buy \"Celestial Dominion\" ?",
            false
        ),
    };
}
