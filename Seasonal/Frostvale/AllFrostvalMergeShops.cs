/*
name: All Frostval Merge Shops
description: This script will get all items from all Frostval merge shops.
tags: frostval, frostvale, merge, all, shops, shop, seasonal
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/Glacera.cs
//cs_include Scripts/Seasonal/Frostvale/Story/CoreFrostvale.cs
//cs_include Scripts/Seasonal/Frostvale/BowJanglesMerge.cs
//cs_include Scripts/Seasonal/Frostvale/CarolingMerge.cs
//cs_include Scripts/Seasonal/Frostvale/FimbulTombMerge.cs
//cs_include Scripts/Seasonal/Frostvale/WinterHorrorWarRewardsMerge.cs
//cs_include Scripts/Seasonal/Frostvale/UnsungNecropolisMerge.cs
//cs_include Scripts/Seasonal/Frostvale/SnowviewMerge.cs
//cs_include Scripts/Seasonal/Frostvale/SnowviewRaceMerge.cs
//cs_include Scripts/Seasonal/Frostvale/SkadePassMerge.cs
//cs_include Scripts/Seasonal/Frostvale/HuntressMerge.cs
//cs_include Scripts/Seasonal/Frostvale/HelsgroveMerge.cs
//cs_include Scripts/Seasonal/Frostvale/GundaharsStashMerge.cs
//cs_include Scripts/Seasonal/Frostvale/GlacialTombMerge.cs
//cs_include Scripts/Seasonal/Frostvale/FrozenSoulMerge.cs
//cs_include Scripts/Seasonal/Frostvale/FrostvalgalaMerge.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class AllFrostvalMergeShops
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public string OptionsStorage = "AllFrostvalMergeShops";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        CoreBots.Instance.SkipOptions,
        new Option<bool>("acOnly", "AC Only Mode", "True = AC only, False = All items", false),
    };

    private readonly (string Name, Action<mergeOptionsEnum> Run)[] _merges =
    {
        ("Bow Jangles", mode => new BowJanglesMerge().BuyAllMerge(buyMode: mode)),
        ("Caroling", mode => new CarolingMerge().BuyAllMerge(buyMode: mode)),
        ("Fimbul Tomb", mode => new FimbulTombMerge().BuyAllMerge(buyMode: mode)),
        ("Winter Horror War", mode => new WinterHorrorWarRewardsMerge().BuyAllMerge(buyMode: mode)),
        ("Unsung Necropolis", mode => new UnsungNecropolisMerge().BuyAllMerge(buyMode: mode)),
        ("Snowview", mode => new SnowviewMerge().BuyAllMerge(buyMode: mode)),
        ("Snowview Race", mode => new SnowviewRaceMerge().BuyAllMerge(buyMode: mode)),
        ("SkadePass", mode => new SkadePassMerge().BuyAllMerge(buyMode: mode)),
        ("Huntress", mode => new HuntressMerge().BuyAllMerge(buyMode: mode)),
        ("Helsgrove", mode => new HelsgroveMerge().BuyAllMerge(buyMode: mode)),
        ("Gundahar's Stash", mode => new GundaharsStashMerge().BuyAllMerge(buyMode: mode)),
        ("Glacial Tomb", mode => new GlacialTombMerge().BuyAllMerge(buyMode: mode)),
        ("Frozen Soul", mode => new FrozenSoulMerge().BuyAllMerge(buyMode: mode)),
        ("Frostvalgala", mode => new FrostvalgalaMerge().BuyAllMerge(buyMode: mode)),
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        bool acOnly = Bot.Config!.Get<bool>("acOnly");
        mergeOptionsEnum mode = acOnly ? mergeOptionsEnum.acOnly : mergeOptionsEnum.all;

        DoAllMerges(mode);

        Core.SetOptions(false);
    }

    public void DoAllMerges(mergeOptionsEnum option)
    {
        foreach (var (name, run) in _merges)
        {
            Core.Logger($"Starting {name} Merge Shop");
            run(option);
            Core.Logger($"Finished {name} Merge Shop");
        }
    }
}
