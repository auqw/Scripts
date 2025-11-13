/*
name: Bacon Cat REP
description: This script will farm Bacon Cat reputation to rank 10.
tags: baconcat, bacon cat, reputation, rep, farm, rank
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class BaconCatREP
{
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreToD TOD
    {
        get => _TOD ??= new CoreToD();
        set => _TOD = value;
    }
    private static CoreToD _TOD;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        DoRep();

        Core.SetOptions(false);
    }

    public void DoRep()
    {
        TOD.BaconCatFortress();
        TOD.LaserSharkInvasion();

        Farm.BaconCatREP();
    }
}
