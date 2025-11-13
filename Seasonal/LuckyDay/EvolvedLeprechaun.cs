/*
name: Evolved Leprechaun (Seasonal) Class
description: This script farms the Evolved Leprechaun class.
tags: seasonal, class, leprechaun, lucky day, shamrock
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/LuckyDay/LuckyDayShamrockFairMerge.cs
//cs_include Scripts/CoreDailies.cs
using Skua.Core.Interfaces;

public class EvolvedLeprechaun
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static LuckyDayShamrockFairMerge LDSFM
    {
        get => _LDSFM ??= new LuckyDayShamrockFairMerge();
        set => _LDSFM = value;
    }
    private static LuckyDayShamrockFairMerge _LDSFM;
    private static CoreDailies Daily
    {
        get => _Daily ??= new CoreDailies();
        set => _Daily = value;
    }
    private static CoreDailies _Daily;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        GetClass();
        Core.SetOptions(false);
    }

    public void GetClass(bool rankUpClass = true)
    {
        if (
            Core.CheckInventory("Evolved Leprechaun")
            || Core.CheckInventory("Evolved Leprechaun")
                && !Daily.CheckDailyv2(Core.CheckInventory(971) ? 1761 : 1759)
            || !Core.isSeasonalMapActive("luck")
        )
        {
            Core.Logger(
                Core.CheckInventory("Evolved Leprechaun")
                    ? "You already own Evolved Leprechaun class."
                    : "This class is only available during the Good Luck Day event."
            );
            return;
        }

        LDSFM.BuyAllMerge("Evolved Leprechaun");

        if (rankUpClass)
            Adv.RankUpClass("Evolved Leprechaun");
    }
}
