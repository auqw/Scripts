/*
name: Mogloween Merges Do All
description: Runs all Mogloween merge shop scripts in this folder.
tags: mogloween, merge, doall
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/Mogloween/MergeShops/AlnswickCastleMerge.cs
//cs_include Scripts/Seasonal/Mogloween/MergeShops/CandiedCostumesMerge.cs
//cs_include Scripts/Seasonal/Mogloween/MergeShops/ChromaCandyMerge.cs
//cs_include Scripts/Seasonal/Mogloween/MergeShops/CrescentMerge.cs
//cs_include Scripts/Seasonal/Mogloween/MergeShops/ElodeasCandyHaulMerge.cs
//cs_include Scripts/Seasonal/Mogloween/MergeShops/FeastoftheDepthsMerge.cs
//cs_include Scripts/Seasonal/Mogloween/MergeShops/HuntersReward.cs
//cs_include Scripts/Seasonal/Mogloween/MergeShops/Mog-GLOW-weenMerge.cs
//cs_include Scripts/Seasonal/Mogloween/MergeShops/MogloweenSeasonalMerge.cs
//cs_include Scripts/Seasonal/Mogloween/MoonlightKhopeshMerge.cs
//cs_include Scripts/Seasonal/Mogloween/MergeShops/TricktownMerge.cs
//cs_include Scripts/Seasonal/Mogloween/MergeShops/UngourdlyGearMerge.cs
//cs_include Scripts/Seasonal/Mogloween/MergeShops/VampireLordMerge.cs
//cs_include Scripts/Seasonal/Mogloween/MergeShops/WalpurgaWitchWares.cs
//cs_include Scripts/Seasonal/Mogloween/MergeShops/WitchsCharms.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class MogloweenMergesDoAll
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "MogloweenMergesDoAll";
    public string[] MultiOptions = { "Generic", "Select" };
    public List<IOption> Generic = BuildGenericOptions();
    public List<IOption> Select = BuildSelectOptions();

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(
            new[]
            {
                "Candy Dragon Egg",
                "Jiangshi",
                "Jiangshi Hair",
                "Jiangshi Locks",
                "Jiangshi Hat",
                "Jiangshi Cap",
                "Jiangshi Talisman Hair",
                "Jiangshi Talisman Locks",
                "Jiangshi Bandages",
                "MarshMeowllows",
                "Horehound Bits",
                "Kitty Cordials",
                "Gummy Brains",
                "Gold Voucher 25k",
                "Tactical Agent Alpha",
                "Tactical Agent Bravo",
                "Tactical Agent Bravo Beard",
                "Tactical Agent Bravo Locks",
                "Tactical Alpha Rifle",
                "Tactical Alpha Rifles",
                "Backup Ol Reliable Zombie Buster",
                "Orange Dye",
                "Green Dye",
                "Yellow Dye",
                "Black Dye",
                "Purple Dye",
                "Pink Dye ",
                "Royce's Direclaw",
                "Spectral Lycan",
                "Spectral Lycan's Hood",
                "Spectral Lycan's Morph",
                "Spectral Lycan's Backfur",
                "Howling Spectral Lycan",
                "Spectral Ground Flames",
                "Spectral Lycan's Spear",
                "Spectral Lycan's Claws ",
                "Nudibranch Roe",
                "Barnacle Rash",
                "Shrimp Noodle Pack",
                "Dzeza Welcome Pack",
                "Dzeza Coconuts",
                "Desiccated Bulbs",
                "Head of Hair",
                "Bucket of Molars",
                "Thok's War Armor",
                "Arcana's Vesture",
                "Metrea's Garb",
                "Juvania's Robes",
                "Great Calabaza Blade",
                "Great Calabaza Blades",
                "Pumpkin King's Revenge",
                "Dual Pumpkin King's Revenge",
                "Pumpkin Fever Staff",
                "Kabocha King Naginata",
                "Pumpkin King's Wrath",
                "Dual Pumpkin King's Wrath",
                "Pumpkin Fever Axe",
                "Pumpkin Fever Axes",
                "Thok's Hair",
                "Arcana's Locks",
                "Metrea's Locks",
                "Juvania's Locks",
                "Acromegalia Seed",
                "Depths Scale",
                "Darkovian Nobility Hair",
                "Darkovian Nobility Locks",
                "Darkovian Pup Hair",
                "Darkovian Pup Locks",
                "Silver Stake",
                "Glowball ",
                "Candy Corn",
                "Box-o-Chocolates",
                "Lol-E-Pop",
                "Ghostly Cape",
                "Cursed Bone Club",
                "Ivy Blade",
                "Blister's Chainsaw",
                "Medusa Curse",
                "Sinister Pumpkin Sickles",
                "Great Pumpkin King Sword",
                "Legion Token",
                "Diamond of Nulgath ",
                "Treats",
                "Ghastly Gummy ",
                "Blood Moon Token ",
                "Gothic Guest Hair",
                "Gothic Guest Locks",
                "Hollyhock Brew",
                "Owl's Dark Guise",
                "Poltergeistcorn Snack",
                "Poltergeistcorn Snacks",
                "Roosting Owl",
                "Rufous Wisdom Staff",
                "Sinema Snack",
                "Sinema Snacks",
                "Stikini Witch Visage",
                "Strikini Witch Morph",
                "Tiny Pink Princess Cupcake",
                "Tiny Pink Princess Cupcakes",
                "Upsizer",
                "Luna Dial",
                "Silverblood Blade",
                "Sventura",
                "Witch Princess' Hat",
            }
        );

        Core.SetOptions();

        (string Name, Action Run)[] merges =
        {
            (
                "Alnswick Castle",
                () => new AlnswickCastleMerge().BuyAllMerge(buyMode: GetMode("alnswickMode"))
            ),
            (
                "Candied Costumes",
                () => new CandiedCostumesMerge().BuyAllMerge(buyMode: GetMode("candiedCostumesMode"))
            ),
            (
                "Chroma Candy",
                () => new ChromaCandyMerge().BuyAllMerge(buyMode: GetMode("chromaCandyMode"))
            ),
            (
                "Crescent",
                () => new CrescentMerge().BuyAllMerge(buyMode: GetMode("crescentMode"))
            ),
            (
                "Elodea's Candy Haul",
                () => new ElodeasCandyHaulMerge().BuyAllMerge(buyMode: GetMode("elodeasCandyHaulMode"))
            ),
            (
                "Feast of the Depths",
                () => new FeastoftheDepths().BuyAllMerge(buyMode: GetMode("feastOfTheDepthsMode"))
            ),
            (
                "Hunter's Reward",
                () => new HuntersReward().BuyAllMerge(buyMode: GetMode("huntersRewardMode"))
            ),
            (
                "Mog-GLOW-ween",
                () => new MogGLOWweenMerge().BuyAllMerge(buyMode: GetMode("mogGlowWeenMode"))
            ),
            (
                "Mogloween Seasonal",
                () => new MogloweenSeasonalMerge().BuyAllMerge(buyMode: GetSeasonalMode())
            ),
            (
                "Moonlight Khopesh",
                () => new MoonlightKhopeshMerge().BuyAllMerge(buyMode: GetMode("moonlightKhopeshMode"))
            ),
            (
                "Tricktown",
                () => new TricktownMerge().BuyAllMerge(buyMode: GetMode("tricktownMode"))
            ),
            (
                "Ungourdly Gear",
                () => new UngourdlyGearMerge().BuyAllMerge(buyMode: GetMode("ungourdlyGearMode"))
            ),
            (
                "Vampire Lord",
                () => new VampireLordMerge().BuyAllMerge(buyMode: GetMode("vampireLordMode"))
            ),
            (
                "Walpurga Witch Wares",
                () => new WalpurgaWitchWares().BuyAllMerge(buyMode: GetMode("walpurgaWitchWaresMode"))
            ),
            (
                "Witch's Charms",
                () => new WitchsCharms().BuyAllMerge(buyMode: GetMode("witchsCharmsMode"))
            ),
        };

        foreach (var (name, run) in merges)
        {
            Core.Logger($"Starting {name} Merge Shop");
            run();
            Core.Logger($"Finished {name} Merge Shop");
        }

        Core.SetOptions(false);
    }

    private mergeOptionsEnum GetMode(string optionName) =>
        Bot.Config?.Get<mergeOptionsEnum>("Generic", optionName) ?? mergeOptionsEnum.all;

    private mergeOptionsEnum GetSeasonalMode() =>
        Bot.Config?.Get<MogloweenMergeMode>("Generic", "mode")
            switch
            {
                MogloweenMergeMode.acOnly => mergeOptionsEnum.acOnly,
                MogloweenMergeMode.acNonLegion => mergeOptionsEnum.acOnly,
                MogloweenMergeMode.mergeMats => mergeOptionsEnum.mergeMats,
                MogloweenMergeMode.select => mergeOptionsEnum.select,
                _ => mergeOptionsEnum.all,
            };

    private static List<IOption> BuildGenericOptions() =>
    [
        CoreBots.Instance.SkipOptions,
        new Option<string>(
            " ",
            "Mode Explanation [all]",
            "Mode [all]: Get every item available in each shop.",
            "click here"
        ),
        new Option<string>(
            " ",
            "Mode Explanation [acOnly]",
            "Mode [acOnly]: Get AC-tagged items from each shop.",
            "click here"
        ),
        new Option<string>(
            " ",
            "Mode Explanation [acNonLegion]",
            "Mode [acNonLegion]: For Mogloween Seasonal only, get AC-tagged items except Legion items.",
            "click here"
        ),
        new Option<string>(
            " ",
            "Mode Explanation [mergeMats]",
            "Mode [mergeMats]: Get the materials for each shop without buying the items.",
            "click here"
        ),
        new Option<string>(
            " ",
            "Mode Explanation [select]",
            "Mode [select]: Choose items in the Select category below.",
            "click here"
        ),
        new Option<mergeOptionsEnum>(
            "alnswickMode",
            "Alnswick Castle mode",
            "Select what to get from Alnswick Castle.",
            mergeOptionsEnum.all
        ),
        new Option<mergeOptionsEnum>(
            "candiedCostumesMode",
            "Candied Costumes mode",
            "Select what to get from Candied Costumes.",
            mergeOptionsEnum.all
        ),
        new Option<mergeOptionsEnum>(
            "chromaCandyMode",
            "Chroma Candy mode",
            "Select what to get from Chroma Candy.",
            mergeOptionsEnum.all
        ),
        new Option<mergeOptionsEnum>(
            "crescentMode",
            "Crescent mode",
            "Select what to get from Crescent.",
            mergeOptionsEnum.all
        ),
        new Option<mergeOptionsEnum>(
            "elodeasCandyHaulMode",
            "Elodea's Candy Haul mode",
            "Select what to get from Elodea's Candy Haul.",
            mergeOptionsEnum.all
        ),
        new Option<mergeOptionsEnum>(
            "feastOfTheDepthsMode",
            "Feast of the Depths mode",
            "Select what to get from Feast of the Depths.",
            mergeOptionsEnum.all
        ),
        new Option<mergeOptionsEnum>(
            "huntersRewardMode",
            "Hunter's Reward mode",
            "Select what to get from Hunter's Reward.",
            mergeOptionsEnum.all
        ),
        new Option<mergeOptionsEnum>(
            "mogGlowWeenMode",
            "Mog-GLOW-ween mode",
            "Select what to get from Mog-GLOW-ween.",
            mergeOptionsEnum.all
        ),
        new Option<MogloweenMergeMode>(
            "mode",
            "Mogloween Seasonal mode",
            "Select what to get from Mogloween Seasonal.",
            MogloweenMergeMode.all
        ),
        new Option<mergeOptionsEnum>(
            "moonlightKhopeshMode",
            "Moonlight Khopesh mode",
            "Select what to get from Moonlight Khopesh.",
            mergeOptionsEnum.all
        ),
        new Option<mergeOptionsEnum>(
            "tricktownMode",
            "Tricktown mode",
            "Select what to get from Tricktown.",
            mergeOptionsEnum.all
        ),
        new Option<mergeOptionsEnum>(
            "ungourdlyGearMode",
            "Ungourdly Gear mode",
            "Select what to get from Ungourdly Gear.",
            mergeOptionsEnum.all
        ),
        new Option<mergeOptionsEnum>(
            "vampireLordMode",
            "Vampire Lord mode",
            "Select what to get from Vampire Lord.",
            mergeOptionsEnum.all
        ),
        new Option<mergeOptionsEnum>(
            "walpurgaWitchWaresMode",
            "Walpurga Witch Wares mode",
            "Select what to get from Walpurga Witch Wares.",
            mergeOptionsEnum.all
        ),
        new Option<mergeOptionsEnum>(
            "witchsCharmsMode",
            "Witch's Charms mode",
            "Select what to get from Witch's Charms.",
            mergeOptionsEnum.all
        ),
    ];

    private static List<IOption> BuildSelectOptions()
    {
        List<IOption> options = new();
        AddShopOptions(options, "Alnswick Castle", new AlnswickCastleMerge().Select);
        AddShopOptions(options, "Candied Costumes", new CandiedCostumesMerge().Select);
        AddShopOptions(options, "Chroma Candy", new ChromaCandyMerge().Select);
        AddShopOptions(options, "Crescent", new CrescentMerge().Select);
        AddShopOptions(options, "Elodea's Candy Haul", new ElodeasCandyHaulMerge().Select);
        AddShopOptions(options, "Feast of the Depths", new FeastoftheDepths().Select);
        AddShopOptions(options, "Hunter's Reward", new HuntersReward().Select);
        AddShopOptions(options, "Mog-GLOW-ween", new MogGLOWweenMerge().Select);
        AddShopOptions(options, "Mogloween Seasonal", new MogloweenSeasonalMerge().Select);
        AddShopOptions(options, "Moonlight Khopesh", new MoonlightKhopeshMerge().Select);
        AddShopOptions(options, "Tricktown", new TricktownMerge().Select);
        AddShopOptions(options, "Ungourdly Gear", new UngourdlyGearMerge().Select);
        AddShopOptions(options, "Vampire Lord", new VampireLordMerge().Select);
        AddShopOptions(options, "Walpurga Witch Wares", new WalpurgaWitchWares().Select);
        AddShopOptions(options, "Witch's Charms", new WitchsCharms().Select);
        return options;
    }

    private static void AddShopOptions(List<IOption> options, string category, IEnumerable<IOption> shopOptions)
    {
        Option<string> section = new(
            $"shopSection{options.Count}",
            $"===== {category.ToUpperInvariant()} MERGE =====",
            $"Items from {category}.",
            "click here",
            transient: true
        )
        {
            Category = category,
        };
        options.Add(section);

        foreach (IOption option in shopOptions)
        {
            option.Category = category;
            options.Add(option);
        }
    }
}
