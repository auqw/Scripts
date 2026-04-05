/*
name: Celestial Temple Forge Merge
description: This bot will farm the items belonging to the selected mode for the Celestial Temple Forge Merge [2303] in /templeshrine
tags: celestial, temple, forge, merge, templeshrine, rite, ascension, solarbrand, lunarbrand, umbrabrand, burning, sun, glowing, moon, bound, eclipse, greatblade, midnight, solstice, entwined
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Dungeons/EclipseAscent/CoreEclipse.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/Story/VictorMatsuri.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;
using SkuaScripts.Scripts.Custom.EclipseAscent;

public class CelestialTempleForgeMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    public CoreEclipse coreEclipse = new();
    private static CoreArmyLite sArmy = new();
    private static VictorMatsuri VictorMatsuri = new();


    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;
    public List<IOption> Options = new()
    {
        new Option<string>(
            "player1",
            "Account #1",
            "This character will be using Legion Revenant",
            ""
        ),
        new Option<string>(
            "player2",
            "Account #2",
            "This character will be using StoneCrusher",
            ""
        ),
        new Option<string>(
            "player3",
            "Account #3",
            "This character will be using ArchPaladin",
            ""
        ),
        new Option<string>(
            "player4",
            "Account #4",
            "This character will be using Lord Of Order",
            ""
        ),
        new Option<bool>(
            "autoclass",
            "Auto Equip Classes",
            "This will auto equip all classes, if false it will use the classes already equipped.",
            true
        ),
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        Bot.Events.ScriptStopping += OnBotStopped;
        Bot.Events.ExtensionPacketReceived += sArmy.PartyManagement;

        Core.BankingBlackList.AddRange(new[] { "Sliver of Moonlight", "Sliver of Sunlight", "Victor of the Festival", "Ecliptic Offering" });

        Core.SetOptions();
        Core.SendPackets($"%xt%zm%cmd%1%uopref%bParty%true%"); //To be able to join party

        while (!Bot.ShouldExit && sArmy.PartyMemberArray()!.Length < 4)
            coreEclipse.SetupParty();

        Core.SendPackets($"%xt%zm%cmd%1%uopref%bParty%false%");

        Adv.GearStore(EnhAfter: true);

        coreEclipse.EquipWait();
        coreEclipse.EquipClasses(true);

        BuyAllMerge();

        sArmy.PartyLeave();

        Bot.Events.ScriptStopping -= OnBotStopped;
        Bot.Events.ExtensionPacketReceived -= sArmy.PartyManagement;

        Adv.GearStore(true, EnhAfter: true);

        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("templeshrine", 2303, findIngredients, buyOnlyThis, buyMode: buyMode);

        #region Dont edit this part
        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            int currentQuant = req.Temp ? Bot.TempInv.GetQuantity(req.Name) : Bot.Inventory.GetQuantity(req.Name);
            var sliverQuant = quant + (!Core.CheckInventory("Rite of Ascension") ? 1 : 0);
            if (req == null)
            {
                Core.Logger("req is NULL");
                return;
            }

            switch (req.Name)
            {
                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger($"The bot hasn't been taught how to get {req.Name}." + (shouldStop ? " Please report the issue." : " Skipping"), messageBox: shouldStop, stopBot: shouldStop);
                    break;
        #endregion

                case "Sliver of Moonlight":
                    Core.FarmingLogger(req.Name, sliverQuant);
                    coreEclipse.GetSliverOfMoonlight(sliverQuant);
                    break;


                case "Sliver of Sunlight":
                    Core.FarmingLogger(req.Name, sliverQuant);
                    coreEclipse.GetSliverOfSunlight(sliverQuant);
                    break;


                case "Victor of the Festival":
                    VictorMatsuri.Storyline();
                    if (!Core.CheckInventory("Victor of the Festival"))
                    {
                        Core.Logger("Victor Matsuri questline didn't finish, exiting...");
                        Bot.StopSync(true);
                    }
                    break;


                case "Ecliptic Offering":
                    if (!Core.CheckInventory("Rite of Ascension"))
                        BuyAllMerge("Rite of Ascension");
                    Core.FarmingLogger("Ecliptic Offering", quant);
                    coreEclipse.GetEclipticOffering(quant);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>("78809", "Rite of Ascension", "Mode: [select] only\nShould the bot buy \"Rite of Ascension\" ?", false),
        new Option<bool>("78465", "Solarbrand", "Mode: [select] only\nShould the bot buy \"Solarbrand\" ?", false),
        new Option<bool>("78460", "Lunarbrand", "Mode: [select] only\nShould the bot buy \"Lunarbrand\" ?", false),
        new Option<bool>("78455", "Umbrabrand", "Mode: [select] only\nShould the bot buy \"Umbrabrand\" ?", false),
        new Option<bool>("78466", "Blade of the Burning Sun", "Mode: [select] only\nShould the bot buy \"Blade of the Burning Sun\" ?", false),
        new Option<bool>("78461", "Blade of the Glowing Moon", "Mode: [select] only\nShould the bot buy \"Blade of the Glowing Moon\" ?", false),
        new Option<bool>("78456", "Blade of the Bound Eclipse", "Mode: [select] only\nShould the bot buy \"Blade of the Bound Eclipse\" ?", false),
        new Option<bool>("78467", "Greatblade of the Midnight Sun", "Mode: [select] only\nShould the bot buy \"Greatblade of the Midnight Sun\" ?", false),
        new Option<bool>("78462", "Greatblade of the Solstice Moon", "Mode: [select] only\nShould the bot buy \"Greatblade of the Solstice Moon\" ?", false),
        new Option<bool>("78457", "Greatblade of the Entwined Eclipse", "Mode: [select] only\nShould the bot buy \"Greatblade of the Entwined Eclipse\" ?", false),
    };

    private bool OnBotStopped(Exception? exception)
    {
        Bot.Events.ScriptStopping -= OnBotStopped;
        Bot.Events.ExtensionPacketReceived -= sArmy.PartyManagement;

        Core.JumpWait();
        sArmy.PartyLeave();

        Adv.GearStore(true, EnhAfter: true);

        return true;
    }
}
