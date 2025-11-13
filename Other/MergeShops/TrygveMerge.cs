/*
name: TrygveMerge
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/Hollowborn/CoreHollowbornStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class TrygveMerge
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    public static CoreAdvanced sAdv
    {
        get => _sAdv ??= new CoreAdvanced();
        set => _sAdv = value;
    }
    public static CoreAdvanced _sAdv;

    private static CoreHollowbornStory HB
    {
        get => _HB ??= new CoreHollowbornStory();
        set => _HB = value;
    }
    private static CoreHollowbornStory _HB;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(
            new[]
            {
                "Vindicator Badge",
                "Silver Vindicator Recruit",
                "Silver Vindicator Hood",
                "Silver Vindicator Sword",
                "Silver Vindicator Soldier",
                "Silver Vindicator Helm",
                "Silver Vindicator Blade",
                "Silver Vindicator Bow",
                "Dawn Vindicator Lieutenant",
                "Dawn Vindicator Lieutenant Helm ",
            }
        );
        Core.SetOptions();

        BuyAllMerge();

        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        HB.Trygve();
        Adv.StartBuyAllMerge("trygve", 2054, findIngredients, buyOnlyThis, buyMode: buyMode);

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


                case "Vindicator Badge":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8299);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.KillMonster("trygve", "r3", "Left", "Blood Eagle", "Eagle Heart", 8);
                        Core.KillMonster("trygve", "r4", "Left", "Rune Boar", "Boar Heart", 8);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("trygve", "Gramiel", "Vindicator Seal");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Silver Vindicator Sword":
                case "Silver Vindicator Swords":
                case "Silver Vindicator Hood":
                case "Silver Vindicator Recruit":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(
                            "trygve",
                            "Vindicator Recruit",
                            req.Name,
                            quant,
                            isTemp: req.Temp
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Silver Vindicator Blade":
                case "Silver Vindicator Helm":
                case "Silver Vindicator Soldier":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(
                            "trygve",
                            "Vindicator Soldier",
                            req.Name,
                            quant,
                            isTemp: req.Temp
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Dawn Vindicator Lieutenant Helm":
                case "Dawn Vindicator Lieutenant":
                case "Silver Vindicator Bow":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster("trygve", "Gramiel", req.Name, quant, isTemp: req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "63936",
            "Dawn Vindicator Recruit",
            "Mode: [select] only\nShould the bot buy \"Dawn Vindicator Recruit\" ?",
            false
        ),
        new Option<bool>(
            "63995",
            "Dawn Vindicator Hood",
            "Mode: [select] only\nShould the bot buy \"Dawn Vindicator Hood\" ?",
            false
        ),
        new Option<bool>(
            "63980",
            "Dawn Vindicator Sword",
            "Mode: [select] only\nShould the bot buy \"Dawn Vindicator Sword\" ?",
            false
        ),
        new Option<bool>(
            "63939",
            "Dawn Vindicator Soldier",
            "Mode: [select] only\nShould the bot buy \"Dawn Vindicator Soldier\" ?",
            false
        ),
        new Option<bool>(
            "63940",
            "Dawn Vindicator Helm",
            "Mode: [select] only\nShould the bot buy \"Dawn Vindicator Helm\" ?",
            false
        ),
        new Option<bool>(
            "63979",
            "Dawn Vindicator Blade",
            "Mode: [select] only\nShould the bot buy \"Dawn Vindicator Blade\" ?",
            false
        ),
        new Option<bool>(
            "63981",
            "Dawn Vindicator Bow",
            "Mode: [select] only\nShould the bot buy \"Dawn Vindicator Bow\" ?",
            false
        ),
        new Option<bool>(
            "63989",
            "Dawn Vindicator Captain",
            "Mode: [select] only\nShould the bot buy \"Dawn Vindicator Captain\" ?",
            false
        ),
        new Option<bool>(
            "63990",
            "Dawn Vindicator Captain Helm",
            "Mode: [select] only\nShould the bot buy \"Dawn Vindicator Captain Helm\" ?",
            false
        ),
        new Option<bool>(
            "63985",
            "Dawn Vindicator Inquisitor",
            "Mode: [select] only\nShould the bot buy \"Dawn Vindicator Inquisitor\" ?",
            false
        ),
        new Option<bool>(
            "63986",
            "Dawn Vindicator Inquisitor Helm",
            "Mode: [select] only\nShould the bot buy \"Dawn Vindicator Inquisitor Helm\" ?",
            false
        ),
        new Option<bool>(
            "90204",
            "Dawn Vindicator Swords",
            "Mode: [select] only\nShould the bot buy \"Dawn Vindicator Swords\" ?",
            false
        ),
    };
}
