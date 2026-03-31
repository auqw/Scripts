/*
name: Good REP
description: This script will farm Good reputation to rank 10.
tags: good, rep, rank, reputation
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Manor.cs
//cs_include Scripts/Story/PoisonForest.cs
using Skua.Core.Interfaces;

public class GoodREP
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
    private static PoisonForest PForest
    {
        get => _PForest ??= new PoisonForest();
        set => _PForest = value;
    }
    private static PoisonForest _PForest;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Core.Logger("Running PoisonForest storyline to be able to do the better rep method.");
        PForest.StoryLine();
        Farm.GoodREP();

        Core.SetOptions(false);
    }
}
