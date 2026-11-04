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
    public List<IOption> Generic = new MogloweenSeasonalMerge().Generic;
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

        MogloweenMergeMode selectedMode =
            Bot.Config?.Get<MogloweenMergeMode>("Generic", "mode") ?? MogloweenMergeMode.all;
        mergeOptionsEnum buyMode = selectedMode switch
        {
            MogloweenMergeMode.acOnly => mergeOptionsEnum.acOnly,
            MogloweenMergeMode.acNonLegion => mergeOptionsEnum.acOnly,
            MogloweenMergeMode.mergeMats => mergeOptionsEnum.mergeMats,
            MogloweenMergeMode.select => mergeOptionsEnum.select,
            _ => mergeOptionsEnum.all,
        };

        new AlnswickCastleMerge().BuyAllMerge(buyMode: buyMode);
        new CandiedCostumesMerge().BuyAllMerge(buyMode: buyMode);
        new ChromaCandyMerge().BuyAllMerge(buyMode: buyMode);
        new CrescentMerge().BuyAllMerge(buyMode: buyMode);
        new ElodeasCandyHaulMerge().BuyAllMerge(buyMode: buyMode);
        new FeastoftheDepths().BuyAllMerge(buyMode: buyMode);
        new HuntersReward().BuyAllMerge(buyMode: buyMode);
        new MogGLOWweenMerge().BuyAllMerge(buyMode: buyMode);
        new MogloweenSeasonalMerge().BuyAllMerge(buyMode: buyMode);
        new MoonlightKhopeshMerge().BuyAllMerge(buyMode: buyMode);
        new TricktownMerge().BuyAllMerge(buyMode: buyMode);
        new UngourdlyGearMerge().BuyAllMerge(buyMode: buyMode);
        new VampireLordMerge().BuyAllMerge(buyMode: buyMode);
        new WalpurgaWitchWares().BuyAllMerge(buyMode: buyMode);
        new WitchsCharms().BuyAllMerge(buyMode: buyMode);

        Core.SetOptions(false);
    }

    private static List<IOption> BuildSelectOptions()
    {
        List<IOption> options = new();
        options.AddRange(new AlnswickCastleMerge().Select);
        options.AddRange(new CandiedCostumesMerge().Select);
        options.AddRange(new ChromaCandyMerge().Select);
        options.AddRange(new CrescentMerge().Select);
        options.AddRange(new ElodeasCandyHaulMerge().Select);
        options.AddRange(new FeastoftheDepths().Select);
        options.AddRange(new HuntersReward().Select);
        options.AddRange(new MogGLOWweenMerge().Select);
        options.AddRange(new MogloweenSeasonalMerge().Select);
        options.AddRange(new MoonlightKhopeshMerge().Select);
        options.AddRange(new TricktownMerge().Select);
        options.AddRange(new UngourdlyGearMerge().Select);
        options.AddRange(new VampireLordMerge().Select);
        options.AddRange(new WalpurgaWitchWares().Select);
        options.AddRange(new WitchsCharms().Select);
        return options;
    }
}
