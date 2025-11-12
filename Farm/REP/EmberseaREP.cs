/*
name: Embersea REP
description: This script will farm Embersea reputation to rank 10.
tags: embersea, ember sea, ember, sea, reputation, rank, rep
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/FireIsland/CoreFireIsland.cs
using Skua.Core.Interfaces;
public class EmberseaREP
{
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreFireIsland FI { get => _FI ??= new CoreFireIsland(); set => _FI = value; }
    private static CoreFireIsland _FI;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        FI.Feverfew();
        Farm.EmberseaREP();

        Core.SetOptions(false);
    }
}
