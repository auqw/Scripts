/*
name: Harvest Day Merges Do All
description: Runs all Harvest Day merge shop scripts in this folder.
tags: harvestday, harvest, merge, doall
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/HarvestDay/MergeShops/AutumnMerge.cs
//cs_include Scripts/Seasonal/HarvestDay/MergeShops/EbilCorpCafeMerge.cs
//cs_include Scripts/Seasonal/HarvestDay/MergeShops/EbilCorpHQMerge.cs
//cs_include Scripts/Seasonal/HarvestDay/MergeShops/FreeasaBirdMerge.cs
//cs_include Scripts/Seasonal/HarvestDay/MergeShops/HarvestDayParadeMerge.cs
//cs_include Scripts/Seasonal/HarvestDay/MergeShops/HarvestMerge.cs
//cs_include Scripts/Seasonal/HarvestDay/MergeShops/ManaHarvestMerge.cs
//cs_include Scripts/Seasonal/HarvestDay/MergeShops/MoglinFeastMerge.cs
//cs_include Scripts/Seasonal/HarvestDay/MergeShops/NightmareHarvestMerge.cs
//cs_include Scripts/Seasonal/HarvestDay/MergeShops/NightmareHarvestWarMerge.cs
//cs_include Scripts/Seasonal/HarvestDay/MergeShops/RobinasHarvestMerge.cs
//cs_include Scripts/Seasonal/HarvestDay/CoreHarvestDay.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class HarvestDayMergesDoAll
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreHarvestDay HarvestDay
    {
        get => _HarvestDay ??= new CoreHarvestDay();
        set => _HarvestDay = value;
    }
    private static CoreHarvestDay _HarvestDay;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "HarvestDayMergesDoAll";
    public string[] MultiOptions = { "Generic", "Select" };
    public List<IOption> Generic = BuildGenericOptions();
    public List<IOption> Select = BuildSelectOptions();

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(
            new[]
            {
                "Fallen Leaf",
                "Gold Flake",
                "Gnarled Wood",
                "Blight Bone",
                "Turdraken Carver",
                "Harvest Rifle",
                "Turdraken Carvers",
                "Vintage Shotgun",
                "Cranberry Shoulder Imp",
                "Nightmare Medal ",
                "Earplug ",
                "Golden Leaves",
                "Persimmon Branch",
                "Fall Fruit Basket",
                "Fae Dust",
                "Fall Fae Assassin Morph",
                "Fall Fae Assassin Visage",
                "Emergency Rations",
                "Glossly Chestnut Waves",
                "Glossy Chestnut Locks",
                "Goredon's Zard Sauce",
                "Harvest Golem Parfait",
                "Ultra Turdrakogiblet",
                "Wretched Rider Meat",
                "Overgourd Seed",
                "Autumnal Civilian",
                "Maple Leaf",
                "Burnt Feather",
                "Autumnal Civilian Hair",
                "Autumnal Locks",
                "Scarbucks Pumpkin Spice Latte",
                "Scarbucks Pumpkin Pie",
                "Iron Ore",
                "TurKing Claw",
                "Guncraft Shadowslayer Big Iron",
                "Guncraft Shadowslayer Big Irons ",
                "Pink Balloon Scrap",
                "Green Balloon Scrap",
                "Red Balloon Scrap ",
                "Stymphalian's Bronze Feather",
                "Tin Can of ???",
                "PACVEC Mach 1.0",
                "PACVEC Helm",
                "PACVEC Visor",
                "PACVEC Guard",
                "PACVEC Battle Wings",
                "PACVEC Battle Hammer",
                "PACVEC Railgun",
                "PACVEC Alien",
                "EbilCoin",
                "Ebil Operative",
                "Ebil Operative Hair + Mask",
                "Ebil Operative Protective Gear",
                "Ebil Operative Hair + Muffler",
                "Ebil Operative Hair + Scarf",
                "Ebil Operative Edgy Hair + Mask",
                "Ebil Operative Edgy Protective Gear",
                "Ebil Operative Edgy Hair + Scarf",
                "Ebil Operative Edgy Hair + Muffler",
                "Ebil Operative Mask Helm",
                "Ebil Operative Helm + Scarf",
                "Ebil Operative Hood + Mask",
                "Ebil Operative Crossed Baton Blades",
                "Ebil Operative Cape",
                "Ebil Operative Baton Blade",
                "Ebil Operative Death Blade",
                "Ebil Operative Long Baton",
                "Ebil Operative Tactical Rifle",
            }
        );

        Core.SetOptions();

        HarvestDay.DoAll();

        foreach (Action buy in GetMergeShopActions())
        {
            if (Bot.ShouldExit)
                break;

            buy();
        }

        Core.SetOptions(false);
    }

    private IEnumerable<Action> GetMergeShopActions()
    {
        mergeOptionsEnum buyMode = GetBuyMode();
        yield return () => new AutumnMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new EbilCorpCafeMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new EbilHQMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new FreeasaBirdMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new HarvestDayParadeMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new HarvestMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new ManaHarvestMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new MoglinFeastMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new NightmareHarvestMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new NightmareHarvestWarMerge().BuyAllMerge(buyMode: buyMode);
        yield return () => new RobinasHarvestMerge().BuyAllMerge(buyMode: buyMode);
    }

    private mergeOptionsEnum GetBuyMode() =>
        ToBuyMode(
            Bot.Config?.Get<HarvestDayMergesDoAllMode>("Generic", "mode")
                ?? HarvestDayMergesDoAllMode.all
        );

    private static mergeOptionsEnum ToBuyMode(HarvestDayMergesDoAllMode mode) =>
        mode switch
        {
            HarvestDayMergesDoAllMode.acOnly => mergeOptionsEnum.acOnly,
            HarvestDayMergesDoAllMode.mergeMats => mergeOptionsEnum.mergeMats,
            HarvestDayMergesDoAllMode.select => mergeOptionsEnum.select,
            _ => mergeOptionsEnum.all,
        };

    private static List<IOption> BuildGenericOptions()
    {
        return new List<IOption>
        {
            CoreBots.Instance.SkipOptions,
            new Option<HarvestDayMergesDoAllMode>(
                "mode",
                "Purchase mode for all shops",
                "Choose one purchase mode to apply to every Harvest Day merge shop.",
                HarvestDayMergesDoAllMode.all
            ),
        };
    }

    private static List<IOption> BuildSelectOptions()
    {
        List<IOption> options = new();
        AddMergeShopSelectOptions(options, "Autumn", new AutumnMerge().Select);
        AddMergeShopSelectOptions(options, "Ebil Corp Cafe", new EbilCorpCafeMerge().Select);
        AddMergeShopSelectOptions(options, "Ebil Corp HQ", new EbilHQMerge().Select);
        AddMergeShopSelectOptions(options, "Free as a Bird", new FreeasaBirdMerge().Select);
        AddMergeShopSelectOptions(options, "Harvest Day Parade", new HarvestDayParadeMerge().Select);
        AddMergeShopSelectOptions(options, "Harvest", new HarvestMerge().Select);
        AddMergeShopSelectOptions(options, "Mana Harvest", new ManaHarvestMerge().Select);
        AddMergeShopSelectOptions(options, "Moglin Feast", new MoglinFeastMerge().Select);
        AddMergeShopSelectOptions(options, "Nightmare Harvest", new NightmareHarvestMerge().Select);
        AddMergeShopSelectOptions(options, "Nightmare Harvest War", new NightmareHarvestWarMerge().Select);
        AddMergeShopSelectOptions(options, "Robina's Harvest", new RobinasHarvestMerge().Select);
        return options;
    }

    private static void AddMergeShopSelectOptions(
        List<IOption> options,
        string shopName,
        IEnumerable<IOption> selectOptions
    )
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

public enum HarvestDayMergesDoAllMode
{
    all = 0,
    acOnly = 1,
    mergeMats = 2,
    select = 3,
}
