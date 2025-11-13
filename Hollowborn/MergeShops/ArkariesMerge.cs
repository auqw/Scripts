/*
name: Arkaries Merge
description: This bot will farm the items belonging to the selected mode for the Arkaries Merge [2510] in /hbchallenge
tags: arkaries, merge, hbchallenge, hollowborn, wyvernslayer, noble, dragonknight, dragonlord, dragonberserker, dragonbulwark, plumed, armet, fury, plated, rage, dragons, curse, scythe, drake, halberd, masterblade, sapphire, dragon, statue, emerald, topaz, tourmaline, amethyst, ruby, turquoise, diamond, tiger, eye
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class ArkariesMerge
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
                "Hollowborn Dragon Heart",
                "Hollowborn Wyvern Heart",
                "Hollowborn Dragonknight Armet",
                "Hollowborn DragonBerserker Helm",
                "Hollow Soul",
                "Obsidian Hollowborn Dragon Statue",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("hbchallenge", 2510, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Hollow Soul":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAcceptmultiple(new[] { 7553, 7555 });
                        Core.KillMonster(
                            "shadowrealm",
                            "r2",
                            "Left",
                            "Gargrowl",
                            "Darkseed",
                            8,
                            log: false
                        );
                        Core.KillMonster(
                            "shadowrealm",
                            "r2",
                            "Left",
                            "Shadow Guardian",
                            "Shadow Medallion",
                            5,
                            log: false
                        );
                        Core.EnsureComplete(7553);
                        Core.EnsureComplete(7555);
                    }
                    Bot.Wait.ForPickup(req.Name);
                    break;
                    ;

                case "Obsidian Hollowborn Dragon Statue":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "hbchallenge",
                        "Nameless Dragonlord",
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Hollowborn Dragon Heart":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(9990, "hbchallenge", "Nameless Dragonlord");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Hollowborn Wyvern Heart":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(9989, "hbchallenge", "Hollowborn Wyvern");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Hollowborn Dragonknight Armet":
                case "Hollowborn DragonBerserker Helm":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "hbchallenge",
                        "Nameless Dragonlord",
                        req.Name,
                        quant,
                        req.Temp,
                        false
                    );
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "55362",
            "Hollowborn WyvernSlayer",
            "Mode: [select] only\nShould the bot buy \"Hollowborn WyvernSlayer\" ?",
            false
        ),
        new Option<bool>(
            "90105",
            "Noble Hollowborn DragonKnight",
            "Mode: [select] only\nShould the bot buy \"Noble Hollowborn DragonKnight\" ?",
            false
        ),
        new Option<bool>(
            "90106",
            "Noble Hollowborn Dragonlord",
            "Mode: [select] only\nShould the bot buy \"Noble Hollowborn Dragonlord\" ?",
            false
        ),
        new Option<bool>(
            "90107",
            "Hollowborn DragonBerserker",
            "Mode: [select] only\nShould the bot buy \"Hollowborn DragonBerserker\" ?",
            false
        ),
        new Option<bool>(
            "90109",
            "Hollowborn DragonBulwark",
            "Mode: [select] only\nShould the bot buy \"Hollowborn DragonBulwark\" ?",
            false
        ),
        new Option<bool>(
            "90112",
            "Plumed Hollowborn Dragonknight Armet",
            "Mode: [select] only\nShould the bot buy \"Plumed Hollowborn Dragonknight Armet\" ?",
            false
        ),
        new Option<bool>(
            "90115",
            "Hollowborn DragonBerserker Fury Helm",
            "Mode: [select] only\nShould the bot buy \"Hollowborn DragonBerserker Fury Helm\" ?",
            false
        ),
        new Option<bool>(
            "90116",
            "Hollowborn DragonBerserker Plated Helm",
            "Mode: [select] only\nShould the bot buy \"Hollowborn DragonBerserker Plated Helm\" ?",
            false
        ),
        new Option<bool>(
            "90117",
            "Plumed Hollowborn DragonBerserker Helm",
            "Mode: [select] only\nShould the bot buy \"Plumed Hollowborn DragonBerserker Helm\" ?",
            false
        ),
        new Option<bool>(
            "90111",
            "Noble Hollowborn Dragonknight Armet",
            "Mode: [select] only\nShould the bot buy \"Noble Hollowborn Dragonknight Armet\" ?",
            false
        ),
        new Option<bool>(
            "90114",
            "Noble Hollowborn DragonBerserker Helm",
            "Mode: [select] only\nShould the bot buy \"Noble Hollowborn DragonBerserker Helm\" ?",
            false
        ),
        new Option<bool>(
            "90118",
            "Hollowborn DragonBerserker Rage Helm",
            "Mode: [select] only\nShould the bot buy \"Hollowborn DragonBerserker Rage Helm\" ?",
            false
        ),
        new Option<bool>(
            "90120",
            "Hollowborn Dragon's Curse",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Dragon's Curse\" ?",
            false
        ),
        new Option<bool>(
            "90126",
            "Hollowborn Dragonknight Scythe",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Dragonknight Scythe\" ?",
            false
        ),
        new Option<bool>(
            "90127",
            "Hollowborn Drake Halberd",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Drake Halberd\" ?",
            false
        ),
        new Option<bool>(
            "90128",
            "Hollowborn Dragonlord MasterBlade",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Dragonlord MasterBlade\" ?",
            false
        ),
        new Option<bool>(
            "90297",
            "Sapphire Hollowborn Dragon Statue",
            "Mode: [select] only\nShould the bot buy \"Sapphire Hollowborn Dragon Statue\" ?",
            false
        ),
        new Option<bool>(
            "90298",
            "Emerald Hollowborn Dragon Statue",
            "Mode: [select] only\nShould the bot buy \"Emerald Hollowborn Dragon Statue\" ?",
            false
        ),
        new Option<bool>(
            "90299",
            "Topaz Hollowborn Dragon Statue",
            "Mode: [select] only\nShould the bot buy \"Topaz Hollowborn Dragon Statue\" ?",
            false
        ),
        new Option<bool>(
            "90300",
            "Tourmaline Hollowborn Dragon Statue",
            "Mode: [select] only\nShould the bot buy \"Tourmaline Hollowborn Dragon Statue\" ?",
            false
        ),
        new Option<bool>(
            "90301",
            "Amethyst Hollowborn Dragon Statue",
            "Mode: [select] only\nShould the bot buy \"Amethyst Hollowborn Dragon Statue\" ?",
            false
        ),
        new Option<bool>(
            "90302",
            "Ruby Hollowborn Dragon Statue",
            "Mode: [select] only\nShould the bot buy \"Ruby Hollowborn Dragon Statue\" ?",
            false
        ),
        new Option<bool>(
            "90303",
            "Turquoise Hollowborn Dragon Statue",
            "Mode: [select] only\nShould the bot buy \"Turquoise Hollowborn Dragon Statue\" ?",
            false
        ),
        new Option<bool>(
            "90304",
            "Diamond Hollowborn Dragon",
            "Mode: [select] only\nShould the bot buy \"Diamond Hollowborn Dragon\" ?",
            false
        ),
        new Option<bool>(
            "90305",
            "Tiger Eye Hollowborn Dragon",
            "Mode: [select] only\nShould the bot buy \"Tiger Eye Hollowborn Dragon\" ?",
            false
        ),
    };
}
