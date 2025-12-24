/*
name: Dark Hand Merge
description: This bot will farm the items belonging to the selected mode for the Dark Hand Merge [1693] in /legionarena
tags: dark, hand, merge, legionarena, assassin, hands, shag, masked, chains, cyclone, cyclones, claws, claw, double, edge, kusarigama, axeros, moult, mandibles, torn, shroud, moon, airstrike, pet, behemoth, horns, morph, sheathed, slaughter, battle, cloak, slaughters, darkblood, ashura, armaments
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Legion/Various/WorthyOfTheBlade.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class DarkHandMerge
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
    private static CoreLegion Legion
    {
        get => _Legion ??= new CoreLegion();
        set => _Legion = value;
    }
    private static CoreLegion _Legion;
    public static CoreAdvanced sAdv
    {
        get => _sAdv ??= new CoreAdvanced();
        set => _sAdv = value;
    }
    public static CoreAdvanced _sAdv;

    private static WorthyBlade WB
    {
        get => _WB ??= new WorthyBlade();
        set => _WB = value;
    }
    private static WorthyBlade _WB;

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
                "Death Badge",
                "Bone Sigil",
                "Legion Token",
                "Essence of Blade Master",
                "Primarch's Trophy",
            }
        );
        Core.SetOptions();

        BuyAllMerge();

        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        WB.WorthyOfTheBlade();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("legionarena", 1693, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Death Badge":
                    Core.EquipClass(ClassType.Solo);
                    if (Core.isCompletedBefore(793))
                    {
                        Core.RegisterQuests(6742);
                        Core.AddDrop("Bone Sigil");
                    }
                    Core.HuntMonster("legionarena", "Legion Fiend Rider", req.Name, quant, false);
                    Core.CancelRegisteredQuests();
                    break;

                case "Bone Sigil":
                    Core.FarmingLogger(req.Name, quant);
                    if (Core.isCompletedBefore(793))
                    {
                        Core.Logger("Legion Fiend Rider - Bone Sigil");
                        Core.EquipClass(ClassType.Solo);
                        Core.RegisterQuests(6742);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(
                                "legionarena",
                                "Legion Fiend Rider",
                                "Undead Rider Defeated"
                            );
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    else
                    {
                        Core.Logger("Legions Finest - Bone Sigil");
                        Core.EquipClass(ClassType.Farm);
                        Core.RegisterQuests(6741);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.KillMonster(
                                "legionarena",
                                "r4",
                                "Left",
                                "*",
                                "Legion's Finest Defeated",
                                8
                            );
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Legion Token":
                    Legion.FarmLegionToken(quant);
                    break;

                case "Essence of Blade Master":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "underworld",
                        "Blade Master",
                        "Essence of Blade Master",
                        quant,
                        false
                    );
                    break;

                case "Primarch's Trophy":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Dodge);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(10052, "bosschallenge", "Colossal Primarch", EquipBestClassType: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "47315",
            "Dark Hand Assassin",
            "Mode: [select] only\nShould the bot buy \"Dark Hand Assassin\" ?",
            false
        ),
        new Option<bool>(
            "47316",
            "Dark Hand's Shag",
            "Mode: [select] only\nShould the bot buy \"Dark Hand's Shag\" ?",
            false
        ),
        new Option<bool>(
            "47317",
            "Dark Hand's Mask",
            "Mode: [select] only\nShould the bot buy \"Dark Hand's Mask\" ?",
            false
        ),
        new Option<bool>(
            "47318",
            "Dark Hand's Locks",
            "Mode: [select] only\nShould the bot buy \"Dark Hand's Locks\" ?",
            false
        ),
        new Option<bool>(
            "47319",
            "Dark Hand's Masked Locks",
            "Mode: [select] only\nShould the bot buy \"Dark Hand's Masked Locks\" ?",
            false
        ),
        new Option<bool>(
            "47320",
            "Dark Hand's Chains",
            "Mode: [select] only\nShould the bot buy \"Dark Hand's Chains\" ?",
            false
        ),
        new Option<bool>(
            "47321",
            "Dark Hand's Cyclone Cape",
            "Mode: [select] only\nShould the bot buy \"Dark Hand's Cyclone Cape\" ?",
            false
        ),
        new Option<bool>(
            "47322",
            "Dark Hand's Dual Cyclones",
            "Mode: [select] only\nShould the bot buy \"Dark Hand's Dual Cyclones\" ?",
            false
        ),
        new Option<bool>(
            "47323",
            "Dark Hand's Claws",
            "Mode: [select] only\nShould the bot buy \"Dark Hand's Claws\" ?",
            false
        ),
        new Option<bool>(
            "47324",
            "Dark Hand's Claw",
            "Mode: [select] only\nShould the bot buy \"Dark Hand's Claw\" ?",
            false
        ),
        new Option<bool>(
            "47325",
            "Dark Hand's Cyclone",
            "Mode: [select] only\nShould the bot buy \"Dark Hand's Cyclone\" ?",
            false
        ),
        new Option<bool>(
            "47326",
            "Dark Hand's Double Edge",
            "Mode: [select] only\nShould the bot buy \"Dark Hand's Double Edge\" ?",
            false
        ),
        new Option<bool>(
            "47327",
            "Dark Hand's Kusari-Gama",
            "Mode: [select] only\nShould the bot buy \"Dark Hand's Kusari-Gama\" ?",
            false
        ),
        new Option<bool>(
            "74003",
            "Axeros Moult",
            "Mode: [select] only\nShould the bot buy \"Axeros Moult\" ?",
            false
        ),
        new Option<bool>(
            "74004",
            "Axeros Mandibles",
            "Mode: [select] only\nShould the bot buy \"Axeros Mandibles\" ?",
            false
        ),
        new Option<bool>(
            "74005",
            "Axeros Torn Shroud",
            "Mode: [select] only\nShould the bot buy \"Axeros Torn Shroud\" ?",
            false
        ),
        new Option<bool>(
            "74006",
            "Moon Airstrike Pet",
            "Mode: [select] only\nShould the bot buy \"Moon Airstrike Pet\" ?",
            false
        ),
        new Option<bool>(
            "91607",
            "Dark Hand Behemoth",
            "Mode: [select] only\nShould the bot buy \"Dark Hand Behemoth\" ?",
            false
        ),
        new Option<bool>(
            "91608",
            "Dark Hand Behemoth Horns",
            "Mode: [select] only\nShould the bot buy \"Dark Hand Behemoth Horns\" ?",
            false
        ),
        new Option<bool>(
            "91609",
            "Dark Hand Behemoth Morph",
            "Mode: [select] only\nShould the bot buy \"Dark Hand Behemoth Morph\" ?",
            false
        ),
        new Option<bool>(
            "91610",
            "Dark Hand Behemoth Visage",
            "Mode: [select] only\nShould the bot buy \"Dark Hand Behemoth Visage\" ?",
            false
        ),
        new Option<bool>(
            "91611",
            "Dark Hand Behemoth Hood",
            "Mode: [select] only\nShould the bot buy \"Dark Hand Behemoth Hood\" ?",
            false
        ),
        new Option<bool>(
            "91612",
            "Sheathed Dark Hand's Slaughter",
            "Mode: [select] only\nShould the bot buy \"Sheathed Dark Hand's Slaughter\" ?",
            false
        ),
        new Option<bool>(
            "91613",
            "Dark Hand Behemoth Battle Cloak",
            "Mode: [select] only\nShould the bot buy \"Dark Hand Behemoth Battle Cloak\" ?",
            false
        ),
        new Option<bool>(
            "91614",
            "Dark Hand Behemoth Cloak",
            "Mode: [select] only\nShould the bot buy \"Dark Hand Behemoth Cloak\" ?",
            false
        ),
        new Option<bool>(
            "91615",
            "Dark Hand's Slaughter",
            "Mode: [select] only\nShould the bot buy \"Dark Hand's Slaughter\" ?",
            false
        ),
        new Option<bool>(
            "91616",
            "Dark Hand's Slaughters",
            "Mode: [select] only\nShould the bot buy \"Dark Hand's Slaughters\" ?",
            false
        ),
        new Option<bool>(
            "91617",
            "Darkblood Ashura",
            "Mode: [select] only\nShould the bot buy \"Darkblood Ashura\" ?",
            false
        ),
        new Option<bool>(
            "91618",
            "Dual Darkblood Ashura",
            "Mode: [select] only\nShould the bot buy \"Dual Darkblood Ashura\" ?",
            false
        ),
        new Option<bool>(
            "91619",
            "Dual Dark Hand's Armaments",
            "Mode: [select] only\nShould the bot buy \"Dual Dark Hand's Armaments\" ?",
            false
        ),
    };
}
