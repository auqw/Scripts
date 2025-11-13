/*
name: Light Merge
description: This bot will farm the items belonging to the selected mode for the Light Merge [422] in /necropolis
tags: light, merge, necropolis, armored, daimyo, battlepet, spirit, orb, destiny, broadsword, scythe, mace, bow, bright, blinding, twilly, moglinrider, twig, zorbak
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class LightMerge
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
    private static CoreBLOD CoreBLOD
    {
        get => _CoreBLOD ??= new CoreBLOD();
        set => _CoreBLOD = value;
    }
    private static CoreBLOD _CoreBLOD;
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
                "Golden Daimyo Armor",
                "Undead Energy",
                "Basic Weapon Kit",
                "Almighty Aluminum of Destiny",
                "Blessed Barium of Destiny",
                "Glorious Gold of Destiny",
                "Immortal Iron of Destiny",
                "Celestial Copper of Destiny",
                "Sanctified Silver of Destiny",
                "Advanced Weapon Kit",
                "Ultimate Weapon Kit",
                "Twilly Puppy Saddle",
                "Twig Puppy Saddle",
                "Zorbak Puppy Saddle",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("necropolis", 422, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Advanced Weapon Kit":
                    CoreBLOD.AdvancedWK(quant);
                    break;

                case "Ultimate Weapon Kit":
                    CoreBLOD.UltimateWK(quant: quant);
                    break;

                case "Basic Weapon Kit":
                    CoreBLOD.BasicWK(quant);
                    break;

                case "Golden Daimyo Armor":
                    if (!Core.IsMember)
                    {
                        Core.Logger(
                            "Membership Required to start the `Golden Armored Daimyo [2079] Quest."
                        );
                        break;
                    }
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(2079);
                        Core.HuntMonster("mafic", "Living Fire", "Heart of Flame ", 15);
                        Core.HuntMonster("greenguardwest", "Black Knight", "Black Metal Armor");
                        Core.EnsureComplete(2079);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Undead Energy":
                    Farm.BattleUnderB("Undead Energy", quant);
                    break;

                #region metals
                case "Sanctified Silver of Destiny":
                    CoreBLOD.UpgradeMetal(MineCraftingMetalsEnum.Silver);
                    break;
                case "Immortal Iron of Destiny":
                    CoreBLOD.UpgradeMetal(MineCraftingMetalsEnum.Iron);
                    break;
                case "Glorious Gold of Destiny":
                    CoreBLOD.UpgradeMetal(MineCraftingMetalsEnum.Gold);
                    break;
                case "Celestial Copper of Destiny":
                    CoreBLOD.UpgradeMetal(MineCraftingMetalsEnum.Copper);
                    break;
                case "Blessed Barium of Destiny":
                    CoreBLOD.UpgradeMetal(MineCraftingMetalsEnum.Barium);
                    break;
                case "Almighty Aluminum of Destiny":
                    CoreBLOD.UpgradeMetal(MineCraftingMetalsEnum.Aluminum);
                    break;
                #endregion metals

                case "Twilly Puppy Saddle":
                case "Twig Puppy Saddle":
                case "Zorbak Puppy Saddle":
                    if (!Core.IsMember) // Daimyo
                    {
                        Core.Logger("Membership Required to buy \"daimyo (pet)\" for this item.");
                        break;
                    }
                    else
                        Adv.BuyItem("necropolis", 422, 152);
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(5132);
                        Core.KillMonster(
                            "castleundead",
                            "Enter",
                            "Left",
                            "Skeletal Warrior",
                            "Undead Head",
                            10
                        );
                        Core.EnsureComplete(5132, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "6673",
            "Armored Daimyo Battlepet",
            "Mode: [select] only\nShould the bot buy \"Armored Daimyo Battlepet\" ?",
            false
        ),
        new Option<bool>(
            "12184",
            "Spirit Orb",
            "Mode: [select] only\nShould the bot buy \"Spirit Orb\" ?",
            false
        ),
        new Option<bool>(
            "12513",
            "Daggers of Destiny",
            "Mode: [select] only\nShould the bot buy \"Daggers of Destiny\" ?",
            false
        ),
        new Option<bool>(
            "12516",
            "Blade of Destiny",
            "Mode: [select] only\nShould the bot buy \"Blade of Destiny\" ?",
            false
        ),
        new Option<bool>(
            "12517",
            "Broadsword of Destiny",
            "Mode: [select] only\nShould the bot buy \"Broadsword of Destiny\" ?",
            false
        ),
        new Option<bool>(
            "12512",
            "Scythe of Destiny",
            "Mode: [select] only\nShould the bot buy \"Scythe of Destiny\" ?",
            false
        ),
        new Option<bool>(
            "12520",
            "Mace of Destiny",
            "Mode: [select] only\nShould the bot buy \"Mace of Destiny\" ?",
            false
        ),
        new Option<bool>(
            "12510",
            "Bow of Destiny",
            "Mode: [select] only\nShould the bot buy \"Bow of Destiny\" ?",
            false
        ),
        new Option<bool>(
            "12689",
            "Bright Bow of Destiny",
            "Mode: [select] only\nShould the bot buy \"Bright Bow of Destiny\" ?",
            false
        ),
        new Option<bool>(
            "12691",
            "Bright Daggers of Destiny",
            "Mode: [select] only\nShould the bot buy \"Bright Daggers of Destiny\" ?",
            false
        ),
        new Option<bool>(
            "12693",
            "Bright Mace of Destiny",
            "Mode: [select] only\nShould the bot buy \"Bright Mace of Destiny\" ?",
            false
        ),
        new Option<bool>(
            "12695",
            "Bright Scythe of Destiny",
            "Mode: [select] only\nShould the bot buy \"Bright Scythe of Destiny\" ?",
            false
        ),
        new Option<bool>(
            "12697",
            "Bright Broadsword of Destiny",
            "Mode: [select] only\nShould the bot buy \"Bright Broadsword of Destiny\" ?",
            false
        ),
        new Option<bool>(
            "12742",
            "Bright Blade of Destiny",
            "Mode: [select] only\nShould the bot buy \"Bright Blade of Destiny\" ?",
            false
        ),
        new Option<bool>(
            "14468",
            "Blinding Bow of Destiny",
            "Mode: [select] only\nShould the bot buy \"Blinding Bow of Destiny\" ?",
            false
        ),
        new Option<bool>(
            "14469",
            "Blinding Daggers of Destiny",
            "Mode: [select] only\nShould the bot buy \"Blinding Daggers of Destiny\" ?",
            false
        ),
        new Option<bool>(
            "14470",
            "Blinding Mace of Destiny",
            "Mode: [select] only\nShould the bot buy \"Blinding Mace of Destiny\" ?",
            false
        ),
        new Option<bool>(
            "14471",
            "Blinding Scythe of Destiny",
            "Mode: [select] only\nShould the bot buy \"Blinding Scythe of Destiny\" ?",
            false
        ),
        new Option<bool>(
            "14472",
            "Blinding Broadsword of Destiny",
            "Mode: [select] only\nShould the bot buy \"Blinding Broadsword of Destiny\" ?",
            false
        ),
        new Option<bool>(
            "14473",
            "Blinding Blade of Destiny",
            "Mode: [select] only\nShould the bot buy \"Blinding Blade of Destiny\" ?",
            false
        ),
        new Option<bool>(
            "35248",
            "Twilly MoglinRider Daimyo",
            "Mode: [select] only\nShould the bot buy \"Twilly MoglinRider Daimyo\" ?",
            false
        ),
        new Option<bool>(
            "35249",
            "Twig MoglinRider Daimyo",
            "Mode: [select] only\nShould the bot buy \"Twig MoglinRider Daimyo\" ?",
            false
        ),
        new Option<bool>(
            "35250",
            "Zorbak MoglinRider Daimyo",
            "Mode: [select] only\nShould the bot buy \"Zorbak MoglinRider Daimyo\" ?",
            false
        ),
        new Option<bool>(
            "152",
            "Daimyo",
            "Mode: [select] only\nShould the bot buy \"Daimyo\" ?",
            false
        ),
    };
}
