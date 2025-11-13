/*
name: Brightoak REP
description: This script will farm Brightoak REP to rank 10.
tags: bright, oak, reputation, rep, rank, farm
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/BrightOak.cs

using Skua.Core.Interfaces;

public class BrightoakREP
{
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static BrightOak BrightOak
    {
        get => _BrightOak ??= new BrightOak();
        set => _BrightOak = value;
    }
    private static BrightOak _BrightOak;
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
        BrightOak.doall(true);
        Farm.BrightoakREP();
    }
}
