/*
name: Chaos Militia Merge
description: This bot will farm the items belonging to the selected mode for the Chaos Militia Merge [1890] in /crownsreach
tags: chaos, militia, merge, crownsreach, chaotic, savage, morph, , skullcap, hammer, accoutrements, mage, robe, iaste
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreDailies.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class ChaosMilitiaMerge
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

    private static CoreDailies Daily
    {
        get => _Daily ??= new CoreDailies();
        set => _Daily = value;
    }
    private static CoreDailies _Daily;

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
                "Heroic Berserker",
                "Militia Merit",
                "Heroic Berserker Shag",
                "Heroic Berserker Locks",
                "Heroic Berserker Skullcap",
                "Heroic Berserker Blade",
                "Heroic Berserker Hammer",
                "Heroic Berserker Axe",
                "Heroic Berserker Accoutrements",
                "Darkblood Guards",
                "Enchanted Dark Blood",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Farm.ChaosREP();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("crownsreach", 1890, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Heroic Berserker":
                case "Heroic Berserker Shag":
                case "Heroic Berserker Locks":
                case "Heroic Berserker Skullcap":
                case "Heroic Berserker Blade":
                case "Heroic Berserker Hammer":
                case "Heroic Berserker Axe":
                case "Heroic Berserker Accoutrements":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("chaosmilitia", "Xiang", req.Name, quant, false, false);
                    break;

                case "Militia Merit":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(5775, "citadel", "Inquisitor Guard");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Darkblood Guards":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem("falguard", 544, req.Name, quant);
                    break;

                case "Enchanted Dark Blood":
                    Core.FarmingLogger(req.Name, quant);
                    Daily.EnchantedDarkBlood();
                    if (!Core.CheckInventory(req.Name, quant))
                    {
                        Core.Logger(
                            $"{req.Name} is a daily quest drop, you have {Bot.Inventory.GetQuantity(req.Name)} out of {quant}. Run the script again tomorrow."
                        );
                    }
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "55291",
            "Chaotic Savage",
            "Mode: [select] only\nShould the bot buy \"Chaotic Savage\" ?",
            false
        ),
        new Option<bool>(
            "55292",
            "Chaotic Savage Morph",
            "Mode: [select] only\nShould the bot buy \"Chaotic Savage Morph\" ?",
            false
        ),
        new Option<bool>(
            "55293",
            "Chaotic Savage Morph + Locks",
            "Mode: [select] only\nShould the bot buy \"Chaotic Savage Morph + Locks\" ?",
            false
        ),
        new Option<bool>(
            "55294",
            "Chaotic Savage Skullcap",
            "Mode: [select] only\nShould the bot buy \"Chaotic Savage Skullcap\" ?",
            false
        ),
        new Option<bool>(
            "55299",
            "Chaotic Savage Blade",
            "Mode: [select] only\nShould the bot buy \"Chaotic Savage Blade\" ?",
            false
        ),
        new Option<bool>(
            "55298",
            "Chaotic Savage Hammer",
            "Mode: [select] only\nShould the bot buy \"Chaotic Savage Hammer\" ?",
            false
        ),
        new Option<bool>(
            "55297",
            "Chaotic Savage Axe",
            "Mode: [select] only\nShould the bot buy \"Chaotic Savage Axe\" ?",
            false
        ),
        new Option<bool>(
            "55296",
            "Chaotic Savage Accoutrements",
            "Mode: [select] only\nShould the bot buy \"Chaotic Savage Accoutrements\" ?",
            false
        ),
        new Option<bool>(
            "55279",
            "Chaos Mage Robe",
            "Mode: [select] only\nShould the bot buy \"Chaos Mage Robe\" ?",
            false
        ),
        new Option<bool>(
            "55281",
            "Chaos Mage Cape",
            "Mode: [select] only\nShould the bot buy \"Chaos Mage Cape\" ?",
            false
        ),
        new Option<bool>(
            "55282",
            "Chaos Mage Staff",
            "Mode: [select] only\nShould the bot buy \"Chaos Mage Staff\" ?",
            false
        ),
        new Option<bool>(
            "55280",
            "Chaos Mage Helm",
            "Mode: [select] only\nShould the bot buy \"Chaos Mage Helm\" ?",
            false
        ),
        new Option<bool>(
            "73216",
            "Iaste Armor",
            "Mode: [select] only\nShould the bot buy \"Iaste Armor\" ?",
            false
        ),
    };
}
