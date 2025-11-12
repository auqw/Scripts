/*
name: Glacera REP
description: This script will farm Glacera reputation to rank 10.
tags: glacera, rank, rep, reputation
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/Glacera.cs
//cs_include Scripts/CoreStory.cs

using Skua.Core.Interfaces;
public class GlaceraREP
{
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static GlaceraStory GlaceraStory { get => _GlaceraStory ??= new GlaceraStory(); set => _GlaceraStory = value; }
    private static GlaceraStory _GlaceraStory;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();


        GlaceraStory.DoAll();
        Farm.GlaceraREP();

        Core.SetOptions(false);
    }
}
