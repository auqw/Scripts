/*
name: Hollowborn REP
description: This script will farm Hollowborn reputation to rank 10.
tags: hollow, born, hollowborn, hollow born, lae, rep, rank, reputation
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
public class HollowbornREP
{
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();


        //Rep boost type here crahes bestgear uncomment when fixed.

        Farm.HollowbornREP();

        Core.SetOptions(false);
    }
}
