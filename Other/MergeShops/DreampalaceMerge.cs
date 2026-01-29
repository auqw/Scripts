/*
name: Dreampalace Merge
description: This bot will farm the items belonging to the selected mode for the Dreampalace Merge [1961] in /dreampalace
tags: dreampalace, merge, dreampalace, strong, golmoth, vibrant, awakened, scythe, gazeroth, bow, zelkur, scimitar, zal, djinn, realm, techsuit, polycrystalline, tactical, solarcore, battering, shield, silicon, visor, backup, amplified, techguard
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/DreamPalace.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class DreampalaceMerge
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

    private static DreamPalace dreamPalace
    {
        get => _dreamPalace ??= new DreamPalace();
        set => _dreamPalace = value;
    }
    private static DreamPalace _dreamPalace;

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
                "Axe of Golmoth",
                "Scales of Golmoth",
                "Token of Fire",
                "Zahad's Ancient Gem",
                "Scythe of Gazeroth",
                "Souls of Gazeroth",
                "Token of Earth",
                "Bow of Zelkur",
                "Claws of Zelkur",
                "Token of Water",
                "Scimitar of Zal",
                "Feathers of Zal",
                "Token of Air",
            }
        );
        Core.SetOptions();

        dreamPalace.StoryLine();
        BuyAllMerge();

        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("dreampalace", 1961, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Axe of Golmoth":
                case "Scales of Golmoth":
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster("DreamPalace", "Golmoth", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Token of Air":
                case "Token of Water":
                case "Token of Earth":
                case "Token of Fire":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("DreamPalace", "Mote of Power", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Zahad's Ancient Gem":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("DreamPalace", "Zahad", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Scythe of Gazeroth":
                case "Souls of Gazeroth":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("DreamPalace", "Gazeroth", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Claws of Zelkur":
                case "Bow of Zelkur":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("DreamPalace", "Zelkur", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;

                case "Feathers of Zal":
                case "Scimitar of Zal":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("DreamPalace", "Zal", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "58693",
            "Strong Axe of Golmoth",
            "Mode: [select] only\nShould the bot buy \"Strong Axe of Golmoth\" ?",
            false
        ),
        new Option<bool>(
            "58694",
            "Vibrant Axe of Golmoth",
            "Mode: [select] only\nShould the bot buy \"Vibrant Axe of Golmoth\" ?",
            false
        ),
        new Option<bool>(
            "58695",
            "Awakened Axe of Golmoth",
            "Mode: [select] only\nShould the bot buy \"Awakened Axe of Golmoth\" ?",
            false
        ),
        new Option<bool>(
            "58701",
            "Strong Scythe of Gazeroth",
            "Mode: [select] only\nShould the bot buy \"Strong Scythe of Gazeroth\" ?",
            false
        ),
        new Option<bool>(
            "58702",
            "Vibrant Scythe of Gazeroth",
            "Mode: [select] only\nShould the bot buy \"Vibrant Scythe of Gazeroth\" ?",
            false
        ),
        new Option<bool>(
            "58703",
            "Awakened Scythe of Gazeroth",
            "Mode: [select] only\nShould the bot buy \"Awakened Scythe of Gazeroth\" ?",
            false
        ),
        new Option<bool>(
            "58705",
            "Strong Bow of Zelkur",
            "Mode: [select] only\nShould the bot buy \"Strong Bow of Zelkur\" ?",
            false
        ),
        new Option<bool>(
            "58706",
            "Vibrant Bow of Zelkur",
            "Mode: [select] only\nShould the bot buy \"Vibrant Bow of Zelkur\" ?",
            false
        ),
        new Option<bool>(
            "58707",
            "Awakened Bow of Zelkur",
            "Mode: [select] only\nShould the bot buy \"Awakened Bow of Zelkur\" ?",
            false
        ),
        new Option<bool>(
            "58697",
            "Strong Scimitar of Zal",
            "Mode: [select] only\nShould the bot buy \"Strong Scimitar of Zal\" ?",
            false
        ),
        new Option<bool>(
            "58698",
            "Vibrant Scimitar of Zal",
            "Mode: [select] only\nShould the bot buy \"Vibrant Scimitar of Zal\" ?",
            false
        ),
        new Option<bool>(
            "58699",
            "Awakened Scimitar of Zal",
            "Mode: [select] only\nShould the bot buy \"Awakened Scimitar of Zal\" ?",
            false
        ),
        new Option<bool>(
            "91066",
            "Djinn Realm TechSuit",
            "Mode: [select] only\nShould the bot buy \"Djinn Realm TechSuit\" ?",
            false
        ),
        new Option<bool>(
            "91067",
            "Polycrystalline Tactical Helm",
            "Mode: [select] only\nShould the bot buy \"Polycrystalline Tactical Helm\" ?",
            false
        ),
        new Option<bool>(
            "91071",
            "Solarcore Battering Shield",
            "Mode: [select] only\nShould the bot buy \"Solarcore Battering Shield\" ?",
            false
        ),
        new Option<bool>(
            "91068",
            "Polycrystalline Silicon Helm",
            "Mode: [select] only\nShould the bot buy \"Polycrystalline Silicon Helm\" ?",
            false
        ),
        new Option<bool>(
            "91069",
            "Polycrystalline Silicon Visor",
            "Mode: [select] only\nShould the bot buy \"Polycrystalline Silicon Visor\" ?",
            false
        ),
        new Option<bool>(
            "91070",
            "Back-Up Solarcore Shield",
            "Mode: [select] only\nShould the bot buy \"Back-Up Solarcore Shield\" ?",
            false
        ),
        new Option<bool>(
            "91073",
            "Amplified Djinn Realm TechGuard",
            "Mode: [select] only\nShould the bot buy \"Amplified Djinn Realm TechGuard\" ?",
            false
        ),
        new Option<bool>(
            "91072",
            "Djinn Realm TechGuard",
            "Mode: [select] only\nShould the bot buy \"Djinn Realm TechGuard\" ?",
            false
        ),
    };
}
