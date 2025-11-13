/*
name: Colossal Wares Merge
description: This bot will farm the items belonging to the selected mode for the Colossal Wares Merge [2591] in /moreskulls
tags: colossal, wares, merge, moreskulls, deathbound, knight, thorn, crown, skull, shroud, vestal, gold, awakened, vordred, plate, nictos, necronomicon
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Evil/VordredsArmor.cs
//cs_include Scripts/Prototypes/MoreSkullsBoss.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class ColossalWaresMerge
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

    private static VordredArmor VA
    {
        get => _VA ??= new VordredArmor();
        set => _VA = value;
    }
    private static VordredArmor _VA;
    private static MoreSkullsWorldBoss MSWB
    {
        get => _MSWB ??= new MoreSkullsWorldBoss();
        set => _MSWB = value;
    }
    private static MoreSkullsWorldBoss _MSWB;

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
                "Pristine Skull",
                "Vordred's Armor",
                "Vordred's Helm",
                "Vordred's Chestpiece",
                "Vordred's Cape",
                "Grimskull's Favor",
                "Noxus' Favor",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        if (!Core.isSeasonalMapActive("moreskulls"))
        {
            Core.Logger("The seasonal map 'moreskulls' is not active.");
            return;
        }

        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("moreskulls", 2591, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Pristine Skull":
                    MSWB.Setup(quant);
                    break;

                case "Vordred's Armor":
                case "Vordred's Helm":
                case "Vordred's Chestpiece":
                case "Vordred's Cape":
                    VA.GetVordredsArmor(true);
                    Adv.BuyItem("stonewood", 2063, req.Name);
                    break;
                #endregion

                #region Known items

                case "Grimskull's Favor":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger("Grimskull's Favor requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10282, 10283);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("lichwar", "Noxus Warrior", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Noxus' Favor":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger("Noxus' Favor requires membership to farm, skipping.");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10278, 10279);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster("lichwar", "Grim Soldier", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                #endregion
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "94066",
            "Deathbound Knight",
            "Mode: [select] only\nShould the bot buy \"Deathbound Knight\" ?",
            false
        ),
        new Option<bool>(
            "94067",
            "Deathbound Thorn Crown",
            "Mode: [select] only\nShould the bot buy \"Deathbound Thorn Crown\" ?",
            false
        ),
        new Option<bool>(
            "94068",
            "Deathbound Thorn Hood",
            "Mode: [select] only\nShould the bot buy \"Deathbound Thorn Hood\" ?",
            false
        ),
        new Option<bool>(
            "94069",
            "Deathbound Knight Skull",
            "Mode: [select] only\nShould the bot buy \"Deathbound Knight Skull\" ?",
            false
        ),
        new Option<bool>(
            "94070",
            "Deathbound Knight Shroud",
            "Mode: [select] only\nShould the bot buy \"Deathbound Knight Shroud\" ?",
            false
        ),
        new Option<bool>(
            "94071",
            "Vestal Gold Blade",
            "Mode: [select] only\nShould the bot buy \"Vestal Gold Blade\" ?",
            false
        ),
        new Option<bool>(
            "94072",
            "Vestal Gold Blades",
            "Mode: [select] only\nShould the bot buy \"Vestal Gold Blades\" ?",
            false
        ),
        new Option<bool>(
            "94073",
            "Vestal Gold Axe",
            "Mode: [select] only\nShould the bot buy \"Vestal Gold Axe\" ?",
            false
        ),
        new Option<bool>(
            "94074",
            "Vestal Gold Axes",
            "Mode: [select] only\nShould the bot buy \"Vestal Gold Axes\" ?",
            false
        ),
        new Option<bool>(
            "94136",
            "Awakened Vordred",
            "Mode: [select] only\nShould the bot buy \"Awakened Vordred\" ?",
            false
        ),
        new Option<bool>(
            "94137",
            "Awakened Vordred Helm",
            "Mode: [select] only\nShould the bot buy \"Awakened Vordred Helm\" ?",
            false
        ),
        new Option<bool>(
            "94138",
            "Awakened Vordred Plate",
            "Mode: [select] only\nShould the bot buy \"Awakened Vordred Plate\" ?",
            false
        ),
        new Option<bool>(
            "94139",
            "Awakened Vordred Cape",
            "Mode: [select] only\nShould the bot buy \"Awakened Vordred Cape\" ?",
            false
        ),
        new Option<bool>(
            "94052",
            "Nicto's Necronomicon",
            "Mode: [select] only\nShould the bot buy \"Nicto's Necronomicon\" ?",
            false
        ),
    };
}
