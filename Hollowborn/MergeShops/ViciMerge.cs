/*
name: Vici Merge
description: This bot will farm the items belonging to the selected mode for the Vici Merge [2635] in /hbchallenge
tags: vici, merge, hbchallenge, hollowborn, darkblood, morph, face, backblades, darkdwellers, backhand, spear, chains, fallen, warden, keeper, venis, vicis, crown, vidis, armet
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class ViciMerge
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
            new[] { "Hollow Horn", "Hollow Hoof", "Fallen Darkblood Skull", "Vici's Hood" }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("hbchallenge", 2635, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Hollow Horn":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(10488, "hbchallenge", "The Darkdweller");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Hollow Hoof":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(10487, "hbchallenge", "Fallen Darkblood");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Fallen Darkblood Skull":
                case "Vici's Hood":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($"{req.Name} requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(
                        "hbchallenge",
                        "Fallen Darkblood",
                        req.Name,
                        quant,
                        req.Temp,
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
            "96713",
            "Hollowborn Darkblood",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Darkblood\" ?",
            false
        ),
        new Option<bool>(
            "96714",
            "Hollowborn Darkblood Hair",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Darkblood Hair\" ?",
            false
        ),
        new Option<bool>(
            "96715",
            "Hollowborn Darkblood Locks",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Darkblood Locks\" ?",
            false
        ),
        new Option<bool>(
            "96716",
            "Hollowborn Darkblood Morph",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Darkblood Morph\" ?",
            false
        ),
        new Option<bool>(
            "96717",
            "Hollowborn Darkblood Face",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Darkblood Face\" ?",
            false
        ),
        new Option<bool>(
            "96719",
            "Hollowborn Darkblood Backblades",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Darkblood Backblades\" ?",
            false
        ),
        new Option<bool>(
            "96721",
            "Darkdweller's Backhand Spear",
            "Mode: [select] only\nShould the bot buy \"Darkdweller's Backhand Spear\" ?",
            false
        ),
        new Option<bool>(
            "96722",
            "Hollowborn Darkblood Blades",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Darkblood Blades\" ?",
            false
        ),
        new Option<bool>(
            "96723",
            "Hollowborn Darkblood Blade",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Darkblood Blade\" ?",
            false
        ),
        new Option<bool>(
            "96724",
            "Hollowborn Darkblood Axe",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Darkblood Axe\" ?",
            false
        ),
        new Option<bool>(
            "96725",
            "Hollowborn Darkblood Axes",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Darkblood Axes\" ?",
            false
        ),
        new Option<bool>(
            "96726",
            "Hollowborn Darkblood Chains",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Darkblood Chains\" ?",
            false
        ),
        new Option<bool>(
            "96727",
            "Fallen Darkblood",
            "Mode: [select] only\nShould the bot buy \"Fallen Darkblood\" ?",
            false
        ),
        new Option<bool>(
            "96728",
            "Fallen Darkblood Morph",
            "Mode: [select] only\nShould the bot buy \"Fallen Darkblood Morph\" ?",
            false
        ),
        new Option<bool>(
            "96720",
            "Darkdweller's Spear",
            "Mode: [select] only\nShould the bot buy \"Darkdweller's Spear\" ?",
            false
        ),
        new Option<bool>(
            "96703",
            "Hollowborn Warden",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Warden\" ?",
            false
        ),
        new Option<bool>(
            "96704",
            "Hollowborn Keeper",
            "Mode: [select] only\nShould the bot buy \"Hollowborn Keeper\" ?",
            false
        ),
        new Option<bool>(
            "96705",
            "Veni's Helmet",
            "Mode: [select] only\nShould the bot buy \"Veni's Helmet\" ?",
            false
        ),
        new Option<bool>(
            "96706",
            "Vici's Crown",
            "Mode: [select] only\nShould the bot buy \"Vici's Crown\" ?",
            false
        ),
        new Option<bool>(
            "96708",
            "Vidi's Armet",
            "Mode: [select] only\nShould the bot buy \"Vidi's Armet\" ?",
            false
        ),
        new Option<bool>(
            "96736",
            "Darkdweller's Backhand Spear",
            "Mode: [select] only\nShould the bot buy \"Darkdweller's Backhand Spear\" ?",
            false
        ),
    };
}
