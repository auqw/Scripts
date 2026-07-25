/*
name: CarcossaCourt REP
description: This script will farm CarcossaCourt REP to rank 10.
tags: CarcossaCourt, rep, reputation, farm
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/Oasis/CoreOasis.cs
using Skua.Core.Interfaces;

public class CarcossaCourRep
{
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static TheLastSunSetCore TLSSC
    {
        get => __TLSSC ??= new TheLastSunSetCore();
        set => __TLSSC = value;
    }
    private static TheLastSunSetCore __TLSSC;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        TLSSC.DoAll();
        Farm.CarcossaCourtRep();

        Core.SetOptions(false);
    }
}
