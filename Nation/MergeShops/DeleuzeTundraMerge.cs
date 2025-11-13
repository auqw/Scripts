/*
name: Deleuze Tundra Merge
description: This bot will farm the items belonging to the selected mode for the Deleuze Tundra Merge [2520] in /deleuzetundra
tags: deleuze, tundra, merge, deleuzetundra, fiendish, bloodhunter, bow, oblivions, final, call, abyss, boiling, blood, awakened, fiend, naginata, bloodhunt, nul, contract
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Nation/DeleuzeTundra.cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
//cs_include Scripts/Story/Nation/VoidChasm.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class DeleuzeTundraMerge
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
    private static DeleuzeTundraStory DT
    {
        get => _DT ??= new DeleuzeTundraStory();
        set => _DT = value;
    }
    private static DeleuzeTundraStory _DT;
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
        Core.BankingBlackList.AddRange(new[] { "Geode of Oblivion", "Outrider's Broken Blade" });
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        DT.DeleuzeTundra();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("deleuzetundra", 2520, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Geode of Oblivion":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(10033);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.KillMonster(
                            "deleuzetundra",
                            "r4",
                            "Left",
                            "Oblivion Magus",
                            "Honeycomb Flesh",
                            8
                        );
                        Core.KillMonster(
                            "deleuzetundra",
                            "Enter",
                            "Spawn",
                            "Empty Creature",
                            "Empty Carcass",
                            8
                        );
                        Core.EquipClass(ClassType.Solo);
                        Core.KillMonster(
                            "deleuzetundra",
                            "r5",
                            "Left",
                            "Oblivion's Herald",
                            "Obsidian Bone Shard"
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Outrider's Broken Blade":
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(
                        "deleuzetundra",
                        "r2",
                        "Left",
                        "Nation Outrider",
                        req.Name,
                        req.Quantity,
                        req.Temp
                    );
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "83950",
            "Fiendish Bloodhunter Bow",
            "Mode: [select] only\nShould the bot buy \"Fiendish Bloodhunter Bow\" ?",
            false
        ),
        new Option<bool>(
            "83948",
            "Oblivion's Final Call",
            "Mode: [select] only\nShould the bot buy \"Oblivion's Final Call\" ?",
            false
        ),
        new Option<bool>(
            "83947",
            "Oblivion's Abyss Staff",
            "Mode: [select] only\nShould the bot buy \"Oblivion's Abyss Staff\" ?",
            false
        ),
        new Option<bool>(
            "79254",
            "Boiling Blood Staff",
            "Mode: [select] only\nShould the bot buy \"Boiling Blood Staff\" ?",
            false
        ),
        new Option<bool>(
            "75319",
            "Awakened Fiend Naginata",
            "Mode: [select] only\nShould the bot buy \"Awakened Fiend Naginata\" ?",
            false
        ),
        new Option<bool>(
            "83939",
            "Fiendish Bloodhunt Dagger",
            "Mode: [select] only\nShould the bot buy \"Fiendish Bloodhunt Dagger\" ?",
            false
        ),
        new Option<bool>(
            "83940",
            "Fiendish Bloodhunt Daggers",
            "Mode: [select] only\nShould the bot buy \"Fiendish Bloodhunt Daggers\" ?",
            false
        ),
        new Option<bool>(
            "91088",
            "Nul Contract Dagger",
            "Mode: [select] only\nShould the bot buy \"Nul Contract Dagger\" ?",
            false
        ),
        new Option<bool>(
            "91089",
            "Nul Contract Daggers",
            "Mode: [select] only\nShould the bot buy \"Nul Contract Daggers\" ?",
            false
        ),
    };
}
