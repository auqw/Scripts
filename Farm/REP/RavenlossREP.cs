/*
name: Ravenloss REP
description: This script will farm Ravenloss reputation to rank 10.
tags: raven loss, ravenloss, raven, loss, rep, rank, reputation
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/RavenlossSaga.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class RavenlossREP
{
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
    private static RavenlossSaga RLS
    {
        get => _RLS ??= new RavenlossSaga();
        set => _RLS = value;
    }
    private static RavenlossSaga _RLS;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        RLS.DoAll();
        Farm.RavenlossREP();

        Core.SetOptions(false);
    }
}
