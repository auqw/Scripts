/*
name: Buy All Gold Vouchers
description: as the name says
tags: gold voucher, gold, voucher
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;
using Skua.Core.Models.Shops;

public class BuyAllGoldVouchers
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
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
    private static CoreDailies Daily
    {
        get => _Daily ??= new CoreDailies();
        set => _Daily = value;
    }
    private static CoreDailies _Daily;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Getemall();

        Core.SetOptions(false);
    }

    public void Getemall(bool TestMode = false)
    {
        Core.Logger("This script is pointless... but was requested to waste gold.");

        Dictionary<string, int> vouchers = new()
        {
            { "Gold Voucher 500k", 300 },
            { "Gold Voucher 200k", 300 },
            { "Gold Voucher 100k", 300 },
            { "Gold Voucher 25k", 300 },
            { "Gold Voucher 7.5k", 300 },
        };

        foreach ((string voucher, int maxQuant) in vouchers)
            Farm.Voucher(voucher, maxQuant);
    }
}
