/*
name: Hollowborn Reaper's Scythe
description: This script will farm Hollowborn Reaper's Scythe
tags: hollowborn, reaper, scythe, shadowrealm
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Hollowborn/MergeShops/ShadowrealmMerge.cs
//cs_include Scripts/Story/AgeofRuin/CoreAOR.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Other/MergeShops/YulgarsUndineMerge.cs
//cs_include Scripts/Hollowborn/MergeShops/DawnFortressMerge.cs
//cs_include Scripts/Story/Hollowborn/CoreHollowbornStory.cs
using Skua.Core.Interfaces;

public class HollowbornScythe
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static ShadowrealmMerge SRM
    {
        get => _SRM ??= new ShadowrealmMerge();
        set => _SRM = value;
    }
    private static ShadowrealmMerge _SRM;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        SRM.BuyAllMerge("Hollowborn Reaper's Scythe");

        Core.SetOptions(false);
    }
}
