/*
name: Streamwar Merge
description: This bot will farm the items belonging to the selected mode for the Streamwar Merge [2163] in /streamwar
tags: streamwar, merge, streamwar, bright, mana, flame, guard, banner, sabre, bow, green, dragon, slayer, slayers, sallet, winged, cloak, halberd, dark, a, fragment, beginning
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/ShadowsOfWar/CoreSoWMats.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class StreamwarMerge
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
    private static CoreSoW SoW
    {
        get => _SoW ??= new CoreSoW();
        set => _SoW = value;
    }
    private static CoreSoW _SoW;
    private static CoreSoWMats SOWM
    {
        get => _SOWM ??= new CoreSoWMats();
        set => _SOWM = value;
    }
    private static CoreSoWMats _SOWM;
    public static CoreAdvanced sAdv
    {
        get => _sAdv ??= new CoreAdvanced();
        set => _sAdv = value;
    }
    public static CoreAdvanced _sAdv;

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
                "Avatar's Flame",
                "Prismatic Seams",
                "Avatar's Flame Guard",
                "Avatar's Flame Spikes",
                "Avatar's Flame Banners",
                "Avatar's Flame Sabre",
                "Willpower",
                "Garish Remnant",
                "Avatar's Flame Bow",
                "Unbound Thread",
            }
        );
        Core.SetOptions();

        BuyAllMerge();

        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        SoW.CompleteCoreSoW();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("streamwar", 2163, findIngredients, buyOnlyThis, buyMode: buyMode);

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

                case "Avatar's Flame Bow":
                case "Avatar's Flame Spikes":
                case "Avatar's Flame Banners":
                case "Avatar's Flame Sabre":
                case "Avatar's Flame":
                case "Avatar's Flame Guard":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(
                            "Streamwar",
                            "Second Speaker",
                            req.Name,
                            isTemp: false,
                            log: false
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Willpower":
                    SOWM.Willpower(quant);
                    break;

                case "Garish Remnant":
                    SOWM.GarishRemnant(quant);
                    break;

                case "Prismatic Seams":
                    SOWM.PrismaticSeams(quant);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "71904",
            "Bright Mana Flame",
            "Mode: [select] only\nShould the bot buy \"Bright Mana Flame\" ?",
            false
        ),
        new Option<bool>(
            "71899",
            "Bright Mana Flame Guard",
            "Mode: [select] only\nShould the bot buy \"Bright Mana Flame Guard\" ?",
            false
        ),
        new Option<bool>(
            "71900",
            "Bright Mana Flame Spikes",
            "Mode: [select] only\nShould the bot buy \"Bright Mana Flame Spikes\" ?",
            false
        ),
        new Option<bool>(
            "71901",
            "Bright Mana Flame Banner",
            "Mode: [select] only\nShould the bot buy \"Bright Mana Flame Banner\" ?",
            false
        ),
        new Option<bool>(
            "71902",
            "Bright Mana Flame Sabre",
            "Mode: [select] only\nShould the bot buy \"Bright Mana Flame Sabre\" ?",
            false
        ),
        new Option<bool>(
            "71903",
            "Bright Mana Flame Bow",
            "Mode: [select] only\nShould the bot buy \"Bright Mana Flame Bow\" ?",
            false
        ),
        new Option<bool>(
            "71827",
            "Green Dragon Slayer",
            "Mode: [select] only\nShould the bot buy \"Green Dragon Slayer\" ?",
            false
        ),
        new Option<bool>(
            "71828",
            "Green Dragon Slayer's Sallet",
            "Mode: [select] only\nShould the bot buy \"Green Dragon Slayer's Sallet\" ?",
            false
        ),
        new Option<bool>(
            "71829",
            "Green Dragon Slayer's Winged Sallet",
            "Mode: [select] only\nShould the bot buy \"Green Dragon Slayer's Winged Sallet\" ?",
            false
        ),
        new Option<bool>(
            "71830",
            "Green Dragon Slayer's Cloak",
            "Mode: [select] only\nShould the bot buy \"Green Dragon Slayer's Cloak\" ?",
            false
        ),
        new Option<bool>(
            "71831",
            "Green Dragon Slayer's Halberd",
            "Mode: [select] only\nShould the bot buy \"Green Dragon Slayer's Halberd\" ?",
            false
        ),
        new Option<bool>(
            "71905",
            "Dark Dragon Slayer",
            "Mode: [select] only\nShould the bot buy \"Dark Dragon Slayer\" ?",
            false
        ),
        new Option<bool>(
            "71906",
            "Dark Dragon Slayer's Sallet",
            "Mode: [select] only\nShould the bot buy \"Dark Dragon Slayer's Sallet\" ?",
            false
        ),
        new Option<bool>(
            "71907",
            "Dark Dragon Slayer's Winged Sallet",
            "Mode: [select] only\nShould the bot buy \"Dark Dragon Slayer's Winged Sallet\" ?",
            false
        ),
        new Option<bool>(
            "71908",
            "Dark Dragon Slayer's Cloak",
            "Mode: [select] only\nShould the bot buy \"Dark Dragon Slayer's Cloak\" ?",
            false
        ),
        new Option<bool>(
            "71909",
            "Dark Dragon Slayer's Halberd",
            "Mode: [select] only\nShould the bot buy \"Dark Dragon Slayer's Halberd\" ?",
            false
        ),
        new Option<bool>(
            "73356",
            "A Fragment of the Beginning",
            "Mode: [select] only\nShould the bot buy \"A Fragment of the Beginning\" ?",
            false
        ),
    };
}
