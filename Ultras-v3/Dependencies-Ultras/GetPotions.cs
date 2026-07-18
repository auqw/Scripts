/*
name: Potions
description: null
tags: null
*/
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreFarms2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreAdvanced2.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class PotionBuyer
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots2 Core => CoreBots2.Instance;
    private static CoreFarms2 Farm
    {
        get => _Farm ??= new CoreFarms2();
        set => _Farm = value;
    }
    private static CoreFarms2 _Farm;
    private static CoreAdvanced2 Adv
    {
        get => _Adv ??= new CoreAdvanced2();
        set => _Adv = value;
    }
    private static CoreAdvanced2 _Adv;
    public string OptionsStorage = "Potionv2";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<bool>("BuyReagents", "Buy Reagents?", "Use gold to buy the reagents for the Potions [this takes ALOT of gold.].", false),
        new Option<int>("PotionQuant", "Potion Quantity", "Desired stack amount [max - 300]", 0),
        new Option<bool>("MaxAll", "Max all Potions within the script.", "as the name says", false),

        CoreBots2.Instance.SkipOptions,

        // Tonic
        new Option<bool>("FarmFate", "Fate", "Should the bot Farm Fate Tonics?", false),
        new Option<bool>("FarmSage", "Sage", "Should the bot Farm Sage Tonics?", false),
        new Option<bool>("FarmMight", "Might", "Should the bot Farm Might Tonics?", false),
        new Option<bool>("FarmFortitude", "Fortitude", "Should the bot Farm Fortitude Tonics?", false),
        new Option<bool>("FarmJudgment", "Judgment", "Should the bot Farm Judgment Tonics?", false),

        // Elixir
        new Option<bool>("FarmBattle", "Battle", "Should the bot Farm Battle Elixirs?", false),
        new Option<bool>("FarmMalevolence", "Malevolence", "Should the bot Farm Malevolence Elixirs?", false),
        new Option<bool>("FarmUnstableDivine", "Unstable Divine", "Should the bot Farm Unstable Divine Elixirs?", false),
        new Option<bool>("FarmDivine", "Divine Elixir", "Should the bot Farm Divine Elixirs?", false),
        new Option<bool>("FarmRevitalize", "Revitalize", "Should the bot Farm Potent Revitalize Elixirs", false),
        new Option<bool>("FarmDestruction", "Destruction", "Should the bot Farm Potent Destruction Elixir?", false),
        new Option<bool>("FarmFelicitousPhiltre", "Felicitous Philtre", "Should the bot get Felicitous Philtre?", false),
        new Option<bool>("FarmEnduranceDraught", "Endurance Draught", "Should the bot get Endurance Draught?", false),

        // Potion
        new Option<bool>("FarmMalice", "Malice", "Should the bot Farm Malice Potions?", false), // FIXED NAME
        new Option<bool>("FarmHonor", "Honor", "Should the bot Farm Honor Potions?", false),
        new Option<bool>("FarmLife", "Life", "Should the bot Farm Potent Life Potion?", false),
        new Option<bool>("FarmBody", "Body", "Should the bot Farm Body Tonics?", false),
        new Option<bool>("FarmSoul", "Soul", "Should the bot Farm Soul Potions?", false),
    };


    //2ndary potions that are obtained alongside the normal versions, to be banked and added as a drop.
    string[] SecondaryPotions = new[] { "Potent Malice Potion", "Potent Soul Potion" };

    public void ScriptMain(IScriptInterface bot)
    {
        // Add items that should never be banked
        Core.BankingBlackList.Add("Dragon Runestone");
        Core.BankingBlackList.AddRange(SecondaryPotions);

        Core.SetOptions();

        // Determine if MaxAll is enabled
        bool maxAll = Bot.Config!.Get<bool>("MaxAll");

        // Prepare Potions and PotionsFarm arrays if MaxAll is true
        string[]? potions = maxAll
            ? new[]
            {
            "Judgment Tonic",
            "Fortitude Tonic",
            "Fate Tonic",
            "Sage Tonic",
            "Potent Battle Elixir",
            "Potent Malevolence Elixir",
            "Potent Honor Potion",
            "Unstable Divine Elixir",
            "Divine Elixir",
            "Potent Revitalize Elixir",
            "Endurance Draught",
            "Felicitous Philtre",
            "Potent Destruction Elixir",
            "Body Tonic",
            "Soul Potion",
            "Unstable Battle Elixir",
            "Unstable Body Tonic",
            "Unstable Fate Tonic",
            "Unstable Keen Elixir",
            "Unstable Mastery Tonic",
            "Unstable Might Tonic",
            "Unstable Wise Tonic",
            "Might Tonic",
            "Malice Potion",
            "Potent Life Potion",
            }
            : null;

        bool[]? potionsFarm = maxAll
            ? [.. Enumerable.Repeat(true, potions!.Length)]
            : null;

        string raw = Bot.Config!.Get<string>("PotionQuant");

        int potionQuant;

        if (!int.TryParse(raw, out potionQuant))
        {
            Core.Logger($"Invalid PotionQuant '{raw}', defaulting to 300");
            potionQuant = 300;
        }

        potionQuant = maxAll ? 300 : potionQuant;
        // Call the main potion farming method
        INeedYourStrongestPotions(
        Potions: potions,
        PotionsFarm: potionsFarm,
        PotionQuant: potionQuant,
        BuyReagents: Bot.Config!.Get<bool>("BuyReagents")
    );


        Core.SetOptions(false);
    }


    public void INeedYourStrongestPotions(string[]? Potions = null, bool[]? PotionsFarm = null, int PotionQuant = 300, bool BuyReagents = false, bool Seperate = false)
    {
        // Clean boolean logic
        BuyReagents = Bot.Config!.Get<bool>("BuyReagents") || BuyReagents;

        Farm.AlchemyREP(8);
        Farm.GoodREP();

        Core.Logger(BuyReagents ? "Method Choose: Buy Reagents" : "Farm Reagents");

        bool maxAll = Bot.Config!.Get<bool>("MaxAll");

        // Default potion list
        Potions ??= new[]
        {
        "Judgment Tonic",
        "Fortitude Tonic",
        "Fate Tonic",
        "Sage Tonic",
        "Potent Battle Elixir",
        "Potent Malevolence Elixir",
        "Potent Honor Potion",
        "Unstable Divine Elixir",
        "Divine Elixir",
        "Potent Revitalize Elixir",
        "Endurance Draught",
        "Felicitous Philtre",
        "Potent Destruction Elixir",
        "Body Tonic",
        "Soul Potion",
        "Unstable Battle Elixir",
        "Unstable Body Tonic",
        "Unstable Fate Tonic",
        "Unstable Keen Elixir",
        "Unstable Mastery Tonic",
        "Unstable Might Tonic",
        "Unstable Wise Tonic",
        "Might Tonic",
        "Malice Potion",
        "Potent Life Potion",
    };

        // Selection mapping (NO MORE INDEX DEPENDENCY)
        Dictionary<string, Func<bool>> potionSelection = new()
        {
            ["Judgment Tonic"] = () => Bot.Config!.Get<bool>("FarmJudgment"),
            ["Fortitude Tonic"] = () => Bot.Config!.Get<bool>("FarmFortitude"),
            ["Fate Tonic"] = () => Bot.Config!.Get<bool>("FarmFate"),
            ["Sage Tonic"] = () => Bot.Config!.Get<bool>("FarmSage"),            
            ["Potent Battle Elixir"] = () => Bot.Config!.Get<bool>("FarmBattle"),
            ["Potent Malevolence Elixir"] = () => Bot.Config!.Get<bool>("FarmMalevolence"),
            ["Potent Honor Potion"] = () => Bot.Config!.Get<bool>("FarmHonor"),
            ["Unstable Divine Elixir"] = () => Bot.Config!.Get<bool>("FarmUnstableDivine"),
            ["Divine Elixir"] = () => Bot.Config!.Get<bool>("FarmDivine"),
            ["Potent Revitalize Elixir"] = () => Bot.Config!.Get<bool>("FarmRevitalize"),

            ["Endurance Draught"] = () => Bot.Config!.Get<bool>("FarmEnduranceDraught"),
            ["Felicitous Philtre"] = () => Bot.Config!.Get<bool>("FarmFelicitousPhiltre"),

            ["Potent Destruction Elixir"] = () => Bot.Config!.Get<bool>("FarmDestruction"),
            ["Body Tonic"] = () => Bot.Config!.Get<bool>("FarmBody"),
            ["Soul Potion"] = () => Bot.Config!.Get<bool>("FarmSoul"),

            // Unstable potions only farmed if MaxAll is enabled
            ["Unstable Battle Elixir"] = () => maxAll,
            ["Unstable Body Tonic"] = () => maxAll,
            ["Unstable Fate Tonic"] = () => maxAll,
            ["Unstable Keen Elixir"] = () => maxAll,
            ["Unstable Mastery Tonic"] = () => maxAll,
            ["Unstable Might Tonic"] = () => maxAll,
            ["Unstable Wise Tonic"] = () => maxAll,


            ["Might Tonic"] = () => Bot.Config!.Get<bool>("FarmMight"),
            ["Malice Potion"] = () => Bot.Config!.Get<bool>("FarmMalice"),
            ["Potent Life Potion"] = () => Bot.Config!.Get<bool>("FarmLife")
        };

        // Validation
        if ((!Seperate && !maxAll && !potionSelection.Any(p => p.Value())) || PotionQuant is < 1 or > 300)
        {
            Core.Logger("No Potions selected or invalid quantity. Stopping.", messageBox: true, stopBot: true);
            return;
        }

        Core.AddDrop(Potions);
        Core.ToBank(SecondaryPotions);
        Core.AddDrop(SecondaryPotions);

        // Debug (super useful)
        Core.Logger($"Selected: {string.Join(", ", Potions.Where(p => maxAll || Seperate || potionSelection[p]()))}");

        foreach (string Potion in Potions)
        {
            bool shouldFarm = maxAll || Seperate || potionSelection[Potion]();

            Core.Logger($"{Potion}: {shouldFarm}");

            if (!shouldFarm)
                continue;

            if (Core.CheckInventory(Potion, PotionQuant))
                continue;

            Core.FarmingLogger(Potion, PotionQuant);

            CoreFarms2.AlchemyTraits currTrait = CoreFarms2.AlchemyTraits.Int;

            switch (Potion)
            {
                case "Fate Tonic":
                case "Sage Tonic":
                    currTrait = Potion == "Sage Tonic"
                        ? CoreFarms2.AlchemyTraits.Int
                        : CoreFarms2.AlchemyTraits.Luc;
                    BulkGrind("Arashtite Ore", "Dried Slime");
                    break;

                case "Potent Battle Elixir":
                case "Potent Malevolence Elixir":
                    currTrait = Potion == "Potent Malevolence Elixir"
                        ? CoreFarms2.AlchemyTraits.SPw
                        : CoreFarms2.AlchemyTraits.APw;
                    BulkGrind("Doomatter", "Chaoroot");
                    break;

                case "Potent Honor Potion":
                case "Malice Potion":
                    currTrait = CoreFarms2.AlchemyTraits.Dam;
                    BulkGrind("Chaoroot", "Chaos Entity");
                    break;

                case "Potent Life Potion":
                    currTrait = CoreFarms2.AlchemyTraits.Hea;
                    BulkGrind("Dragon Scale", "Searbush");
                    break;

                case "Might Tonic":
                    currTrait = CoreFarms2.AlchemyTraits.Dam;
                    BulkGrind("Chaos Entity", "Rhison Blood");
                    break;

                case "Unstable Divine Elixir":
                    if (PotionQuant > 99)
                    {
                        Core.Logger($"Max quant for [{Potion}] is [99] - Adjusting");
                        PotionQuant = 99;
                    }
                    currTrait = CoreFarms2.AlchemyTraits.hOu;
                    BulkGrind("Dragon Scale", "Lemurphant Tears");
                    break;

                case "Divine Elixir":
                    if (PotionQuant > 99)
                    {
                        Core.Logger($"Max quant for [{Potion}] is [99] - Adjusting");
                        PotionQuant = 99;
                    }
                    currTrait = CoreFarms2.AlchemyTraits.hOu;
                    BulkGrind("Dragon Scale", "Ice Vapor");
                    break;

                case "Potent Revitalize Elixir":
                    currTrait = CoreFarms2.AlchemyTraits.hRe;
                    BulkGrind("Chaoroot", "Lemurphant Tears");
                    break;

                case "Felicitous Philtre":
                case "Endurance Draught":
                    Core.Logger($"[{Potion}] has no farm method, buying.");
                    Adv.BuyItem("alchemyacademy", 2036, Potion, PotionQuant);
                    break;

                case "Potent Destruction Elixir":
                    currTrait = CoreFarms2.AlchemyTraits.mRe;
                    BulkGrind("Dried Slime", "Arashtite Ore");
                    break;

                case "Body Tonic":
                    currTrait = CoreFarms2.AlchemyTraits.End;
                    BulkGrind("Roc Tongue", "Chaoroot");
                    break;

                case "Soul Potion":
                    currTrait = CoreFarms2.AlchemyTraits.Dam;
                    BulkGrind("Necrot", "Nimblestem");
                    break;

                case "Unstable Battle Elixir":
                    currTrait = CoreFarms2.AlchemyTraits.APw;
                    BulkGrind("Doomatter", "Nimblestem");
                    break;

                case "Unstable Body Tonic":
                    currTrait = CoreFarms2.AlchemyTraits.End;
                    BulkGrind("Nimblestem", "Roc Tongue");
                    break;

                case "Unstable Fate Tonic":
                    currTrait = CoreFarms2.AlchemyTraits.Luc;
                    BulkGrind("Dried Slime", "Trollola Nectar");
                    break;

                case "Unstable Keen Elixir":
                    currTrait = CoreFarms2.AlchemyTraits.Cri;
                    BulkGrind("Trollola Nectar", "Doomatter");
                    break;

                case "Unstable Mastery Tonic":
                    currTrait = CoreFarms2.AlchemyTraits.Dex;
                    BulkGrind("Chaos Entity", "Dried Slime");
                    break;

                case "Unstable Might Tonic":
                    currTrait = CoreFarms2.AlchemyTraits.Str;
                    BulkGrind("Chaos Entity", "Fish Oil");
                    break;

                case "Unstable Wise Tonic":
                    currTrait = (CoreFarms2.AlchemyTraits)13;
                    BulkGrind("Moglin Tears", "Rhison Blood");
                    break;

                case "Judgment Tonic":
                    currTrait = (CoreFarms2.AlchemyTraits)13;
                    BulkGrind("Dragon Scale", "Moglin Tears", AlchemyRunes.Jera);
                    break;

                case "Fortitude Tonic":
                    currTrait = CoreFarms2.AlchemyTraits.End;
                    BulkGrind("Necrot", "Roc Tongue", AlchemyRunes.Fehu);
                    break;

                default:
                    Core.Logger($"Unknown potion: {Potion}");
                    break;
            }

            void BulkGrind(string reagent1, string reagent2, AlchemyRunes rune = AlchemyRunes.Gebo)
            {
                if (Bot.ShouldExit || Core.CheckInventory(Potion, PotionQuant))
                    return;

                GetIngredient(reagent1);
                GetIngredient(reagent2);

                Core.Join("alchemy");
                Farm.AlchemyPacket(reagent1, reagent2, rune, trait: currTrait, item: Potion, quant: PotionQuant);

                Core.Logger($"Finished one crafting attempt for {Potion}. Not retrying.");
            }

            void GetIngredient(string ingredient, int ingreQuant = 30)
            {
                if (Core.CheckInventory(ingredient, ingreQuant))
                    return;

                Core.ToggleAggro(false);

                switch (ingredient)
                {
                    case "Ice Vapor":
                        if (!BuyReagents)
                            Core.KillMonster("lair", "Enter", "Spawn", "*", "Ice Vapor", 2, isTemp: false, log: false);
                        else
                            Adv.BuyItem("alchemyacademy", 397, 11478, ingreQuant, 2, 1235);
                        break;

                    case "Moglin Tears":
                        if (!Core.IsMember && !BuyReagents)
                            Core.Logger("Farming map is members only, buying the materials");

                        if (!BuyReagents && Core.IsMember)
                            Core.HuntMonster("twig", "Sweetish Fish", ingredient, ingreQuant, isTemp: false);
                        else
                            Adv.BuyItem("alchemyacademy", 397, 11472, ingreQuant, 2, 1229);
                        break;

                    case "Lemurphant Tears":
                        if (!BuyReagents)
                            Core.HuntMonster("ravinetemple", "Lemurphant", ingredient, ingreQuant, isTemp: false);
                        else
                            Adv.BuyItem("alchemyacademy", 397, 11479, ingreQuant, 2, 1236);
                        break;

                    case "Dried Slime":
                        if (!BuyReagents)
                            Core.HuntMonster("orecavern", "Crashroom", ingredient, ingreQuant, isTemp: false);
                        else
                            Adv.BuyItem("alchemyacademy", 397, 11474, ingreQuant, 2, 1231);
                        break;

                    case "Arashtite Ore":
                        if (!BuyReagents)
                            Core.HuntMonster("orecavern", "Deathmole", ingredient, ingreQuant, isTemp: false);
                        else
                            Adv.BuyItem("alchemyacademy", 397, 11473, ingreQuant, 2, 1230);
                        break;

                    case "Chaos Entity":
                        Adv.BuyItem("alchemyacademy", 2114, 11482, ingreQuant, 1, 9740);
                        break;

                    case "Fish Oil":
                        Adv.BuyItem("alchemyacademy", 397, 11467, ingreQuant, 3, 1224);
                        break;

                    case "Doomatter":
                        if (!BuyReagents)
                        {
                            if (Bot.Player.IsMember)
                                Core.HuntMonster("Creepy", "Fear Feeder", ingredient, ingreQuant, isTemp: false);
                            else
                                Core.HuntMonster("maul", "Creature Creation", ingredient, ingreQuant, isTemp: false);
                        }
                        else
                            Adv.BuyItem("alchemyacademy", 397, 11477, ingreQuant, 2, 1234);
                        break;

                    case "Chaoroot":
                        if (!BuyReagents)
                            Core.HuntMonster("orecavern", "Naga Baas", ingredient, ingreQuant, isTemp: false);
                        else
                            Adv.BuyItem("tercessuinotlim", 1951, 11481, ingreQuant, 10, 7911);
                        break;

                    case "Nimblestem":
                        if (!BuyReagents)
                            Core.HuntMonster("mudluk", "Swamp Frogdrake", "Nimblestem", ingreQuant, isTemp: false);
                        else
                            Adv.BuyItem("alchemyacademy", 397, 11469, ingreQuant, 2, 1226);
                        break;

                    case "Trollola Nectar":
                        if (!BuyReagents)
                            Core.HuntMonster("bloodtusk", "Trollola Plant", ingredient, ingreQuant, isTemp: false);
                        else
                            Adv.BuyItem("alchemyacademy", 397, 11476, ingreQuant, 2, 1233);
                        break;

                    case "Searbush":
                        if (!BuyReagents)
                            Core.HuntMonster("mafic", "Living Fire", ingredient, ingreQuant, isTemp: false);
                        else
                            Adv.BuyItem("alchemyacademy", 397, 11468, ingreQuant, 2, 1225);
                        break;

                    case "Dragon Scale":
                        Core.AddDrop(11475);
                        if (!BuyReagents)
                        {
                            while (!Bot.ShouldExit && !Core.CheckInventory(11475, ingreQuant))
                                Core.KillMonster("lair", "Hole", "Center", "*", isTemp: false, log: false);
                        }
                        else if (!Core.CheckInventory(11475, ingreQuant))
                            Adv.BuyItem("alchemyacademy", 397, 11475, ingreQuant, 2, 1232);
                        break;

                    case "Roc Tongue":
                        if (!BuyReagents)
                            Core.HuntMonster("roc", "Rock Roc", ingredient, ingreQuant, isTemp: false, log: false);
                        else
                            Adv.BuyItem("alchemyacademy", 397, 11471, ingreQuant, 2, 1228);
                        break;

                    case "Necrot":
                        if (!BuyReagents)
                            Core.HuntMonster("deathsrealm", "Skeleton Fighter", ingredient, ingreQuant, isTemp: false, log: false);
                        else
                            Adv.BuyItem("alchemyacademy", 397, 11480, ingreQuant, 2, 1237);
                        break;

                    case "Rhison Blood":
                        if (!BuyReagents)
                            Core.HuntMonster("bloodtusk", "Rhison", ingredient, ingreQuant, isTemp: false, log: false);
                        else
                            Adv.BuyItem("alchemyacademy", 397, 11470, ingreQuant, 2, 1227);
                        break;

                    default:
                        Core.Logger($"Unknown ingredient: {ingredient}");
                        break;
                }
            }

        }
    }

}

