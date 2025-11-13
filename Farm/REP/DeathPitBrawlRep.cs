/*
name: Death Pit Brawl REP
description: This script will farm Death Pit Brawl reputation to rank 10.
tags: deathpitbrawl, deathpit, brawl, reputation, rep, rank
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs
//cs_include Scripts/CoreAdvanced.cs

using Skua.Core.Interfaces;

public class DeathPitBrawlREP
{
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreToD CoreToD
    {
        get => _CoreToD ??= new CoreToD();
        set => _CoreToD = value;
    }
    private static CoreToD _CoreToD;
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
        CoreToD.DeathPitPVP();
        Farm.DeathPitBrawlREP();
    }
}
