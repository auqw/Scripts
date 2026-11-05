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

        foreach (Action buy in GetMergeShopActions())
            buy();

        Core.SetOptions(false);
    }

    private IEnumerable<Action> GetMergeShopActions()
    {
        yield return () => new AlnswickCastleMerge().BuyAllMerge(buyMode: GetBuyMode("alnswickCastleMode"));
        yield return () => new CandiedCostumesMerge().BuyAllMerge(buyMode: GetBuyMode("candiedCostumesMode"));
        yield return () => new ChromaCandyMerge().BuyAllMerge(buyMode: GetBuyMode("chromaCandyMode"));
        yield return () => new CrescentMerge().BuyAllMerge(buyMode: GetBuyMode("crescentMode"));
        yield return () => new ElodeasCandyHaulMerge().BuyAllMerge(buyMode: GetBuyMode("elodeasCandyHaulMode"));
        yield return () => new FeastoftheDepths().BuyAllMerge(buyMode: GetBuyMode("feastOfTheDepthsMode"));
        yield return () => new HuntersReward().BuyAllMerge(buyMode: GetBuyMode("huntersRewardMode"));
        yield return () => new MogGLOWweenMerge().BuyAllMerge(buyMode: GetBuyMode("mogGlowweenMode"));
        yield return () =>
        {
            MogloweenMergeMode mode = GetMode("mogloweenSeasonalMode");
            new MogloweenSeasonalMerge().BuyAllMerge(
                buyMode: ToBuyMode(mode),
                skipLegionItems: mode == MogloweenMergeMode.acNonLegion
            );
        };
        yield return () => new MoonlightKhopeshMerge().BuyAllMerge(buyMode: GetBuyMode("moonlightKhopeshMode"));
        yield return () => new TricktownMerge().BuyAllMerge(buyMode: GetBuyMode("tricktownMode"));
        yield return () => new UngourdlyGearMerge().BuyAllMerge(buyMode: GetBuyMode("ungourdlyGearMode"));
        yield return () => new VampireLordMerge().BuyAllMerge(buyMode: GetBuyMode("vampireLordMode"));
        yield return () => new WalpurgaWitchWares().BuyAllMerge(buyMode: GetBuyMode("walpurgaWitchWaresMode"));
        yield return () => new WitchsCharms().BuyAllMerge(buyMode: GetBuyMode("witchsCharmsMode"));
    }

    private mergeOptionsEnum GetBuyMode(string optionKey)
        => ToBuyMode(GetMode(optionKey));

    private MogloweenMergeMode GetMode(string optionKey)
    {
        MogloweenMergeShopMode shopMode =
            Bot.Config?.Get<MogloweenMergeShopMode>("Generic", optionKey) ?? MogloweenMergeShopMode.inherit;
        return shopMode == MogloweenMergeShopMode.inherit
            ? GetLegacyMode()
            : Enum.Parse<MogloweenMergeMode>(shopMode.ToString());
    }

    private MogloweenMergeMode GetLegacyMode() =>
        Bot.Config?.Get<MogloweenMergeMode>("Generic", "mode") ?? MogloweenMergeMode.all;

    private static mergeOptionsEnum ToBuyMode(MogloweenMergeMode mode) =>
        mode switch
        {
            MogloweenMergeMode.acOnly or MogloweenMergeMode.acNonLegion => mergeOptionsEnum.acOnly,
            MogloweenMergeMode.mergeMats => mergeOptionsEnum.mergeMats,
            MogloweenMergeMode.select => mergeOptionsEnum.select,
            _ => mergeOptionsEnum.all,
        };

    private static List<IOption> BuildGenericOptions()
    {
        List<IOption> options = new() { CoreBots.Instance.SkipOptions };
        options.Add(
            new Option<MogloweenMergeMode>(
                "mode",
                "Default purchase mode (legacy fallback)",
                "Used by shops until they have an individual purchase mode saved.",
                MogloweenMergeMode.all
            )
        );
        AddMergeShopOptions(options, "Alnswick Castle", "alnswickCastleMode");
        AddMergeShopOptions(options, "Candied Costumes", "candiedCostumesMode");
        AddMergeShopOptions(options, "Chroma Candy", "chromaCandyMode");
        AddMergeShopOptions(options, "Crescent", "crescentMode");
        AddMergeShopOptions(options, "Elodea's Candy Haul", "elodeasCandyHaulMode");
        AddMergeShopOptions(options, "Feast of the Depths", "feastOfTheDepthsMode");
        AddMergeShopOptions(options, "Hunter's Reward", "huntersRewardMode");
        AddMergeShopOptions(options, "Mog-GLOW-ween", "mogGlowweenMode");
        options.Add(
            new Option<MogloweenMergeShopMode>(
                "mogloweenSeasonalMode",
                "Mogloween Seasonal - Purchase mode",
                "Choose what to buy from the Mogloween Seasonal merge shop. Inherit uses the legacy default mode.",
                MogloweenMergeShopMode.inherit
            )
        );
        AddMergeShopOptions(options, "Moonlight Khopesh", "moonlightKhopeshMode");
        AddMergeShopOptions(options, "Tricktown", "tricktownMode");
        AddMergeShopOptions(options, "Ungourdly Gear", "ungourdlyGearMode");
        AddMergeShopOptions(options, "Vampire Lord", "vampireLordMode");
        AddMergeShopOptions(options, "Walpurga Witch Wares", "walpurgaWitchWaresMode");
        AddMergeShopOptions(options, "Witch's Charms", "witchsCharmsMode");
        return options;
    }

    private static void AddMergeShopOptions(List<IOption> options, string shopName, string optionKey)
    {
        options.Add(
            new Option<MogloweenMergeShopMode>(
                optionKey,
                $"{shopName} - Purchase mode",
                $"Choose what to buy from the {shopName} merge shop. Inherit uses the legacy default mode.",
                MogloweenMergeShopMode.inherit
            )
        );
    }

    private static List<IOption> BuildSelectOptions()
    {
        List<IOption> options = new();
        AddMergeShopSelectOptions(options, "Alnswick Castle", new AlnswickCastleMerge().Select);
        AddMergeShopSelectOptions(options, "Candied Costumes", new CandiedCostumesMerge().Select);
        AddMergeShopSelectOptions(options, "Chroma Candy", new ChromaCandyMerge().Select);
        AddMergeShopSelectOptions(options, "Crescent", new CrescentMerge().Select);
        AddMergeShopSelectOptions(options, "Elodea's Candy Haul", new ElodeasCandyHaulMerge().Select);
        AddMergeShopSelectOptions(options, "Feast of the Depths", new FeastoftheDepths().Select);
        AddMergeShopSelectOptions(options, "Hunter's Reward", new HuntersReward().Select);
        AddMergeShopSelectOptions(options, "Mog-GLOW-ween", new MogGLOWweenMerge().Select);
        AddMergeShopSelectOptions(options, "Mogloween Seasonal", new MogloweenSeasonalMerge().Select);
        AddMergeShopSelectOptions(options, "Moonlight Khopesh", new MoonlightKhopeshMerge().Select);
        AddMergeShopSelectOptions(options, "Tricktown", new TricktownMerge().Select);
        AddMergeShopSelectOptions(options, "Ungourdly Gear", new UngourdlyGearMerge().Select);
        AddMergeShopSelectOptions(options, "Vampire Lord", new VampireLordMerge().Select);
        AddMergeShopSelectOptions(options, "Walpurga Witch Wares", new WalpurgaWitchWares().Select);
        AddMergeShopSelectOptions(options, "Witch's Charms", new WitchsCharms().Select);
        return options;
    }

    private static void AddMergeShopSelectOptions(List<IOption> options, string shopName, IEnumerable<IOption> selectOptions)
    {
        options.Add(
            new Option<string>(
                $"header_{string.Concat(shopName.Where(char.IsLetterOrDigit))}",
                $"========== {shopName.ToUpperInvariant()} MERGE SHOP ==========",
                $"Select items from the {shopName} merge shop.",
                " ",
                transient: true
            )
        );
        options.AddRange(selectOptions);
    }
}

public enum MogloweenMergeShopMode
{
    inherit,
    all,
    acOnly,
    mergeMats,
    select,
    acNonLegion,
}
