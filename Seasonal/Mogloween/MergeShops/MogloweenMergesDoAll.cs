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
    private static CoreMogloween Mog
    {
        get => _CoreMogloween ??= new CoreMogloween();
        set => _CoreMogloween = value;
    }
    private static CoreMogloween _CoreMogloween;

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

        if (Bot.Config != null && !Bot.Config.Get<bool>(Core.SkipOptions))
            Bot.Config.Configure();

        Core.SetOptions();

        Mog.DoAll();

        foreach (Action buy in GetMergeShopActions())
            buy();

        Core.SetOptions(false);
    }

    private IEnumerable<Action> GetMergeShopActions()
    {
        mergeOptionsEnum buyMode = GetBuyMode();
        yield return () => new AlnswickCastleMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new CandiedCostumesMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new ChromaCandyMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new CrescentMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new ElodeasCandyHaulMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new FeastoftheDepths().BuyAllMerge(buyMode: buyMode);
        yield return () => new HuntersReward().BuyAllMerge(buyMode: buyMode);
        yield return () => new MogGLOWweenMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new MogloweenSeasonalMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new MoonlightKhopeshMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new TricktownMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new UngourdlyGearMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new VampireLordMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new WalpurgaWitchWares().BuyAllMerge(buyMode: buyMode);
        yield return () => new WitchsCharms().BuyAllMerge(buyMode: buyMode);
    }

    private mergeOptionsEnum GetBuyMode() =>
        ToBuyMode(
            Bot.Config?.Get<MogloweenMergesDoAllMode>("Generic", "mode") ?? MogloweenMergesDoAllMode.all
        );

    private static mergeOptionsEnum ToBuyMode(MogloweenMergesDoAllMode mode) =>
        mode switch
        {
            MogloweenMergesDoAllMode.acOnly => mergeOptionsEnum.acOnly,
            MogloweenMergesDoAllMode.mergeMats => mergeOptionsEnum.mergeMats,
            MogloweenMergesDoAllMode.select => mergeOptionsEnum.select,
            _ => mergeOptionsEnum.all,
        };

    private static List<IOption> BuildGenericOptions()
    {
        List<IOption> options = new() { CoreBots.Instance.SkipOptions };
        options.Add(
            new Option<MogloweenMergesDoAllMode>(
                "mode",
                "Purchase mode for all shops",
                "Choose one purchase mode to apply to every Mogloween merge shop.",
                MogloweenMergesDoAllMode.all
            )
        );
        options.Add(
            new Option<bool>(
                "mogloweenBuyUndeadWarrior",
                "Buy Undead Warrior and join the Legion",
                "When enabled, the script may buy Undead Warrior, join the Legion, and farm Legion Tokens. When disabled, it skips all Legion items and actions.",
                false
            )
        );
        return options;
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

public enum MogloweenMergesDoAllMode
{
    all = 0,
    acOnly = 1,
    mergeMats = 2,
    select = 3,
}
