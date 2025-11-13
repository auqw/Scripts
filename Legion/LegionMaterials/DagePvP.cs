/*
name: Legion Combat Trophy
description: This script will farm Legion Combat Trophies in /dagepvp.
tags: legion, combat, trophy, dage, pvp, dage pvp, dagepvp
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Legion/CoreLegion.cs
using Skua.Core.Interfaces;

public class LegionCombatTrophy
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
    private static CoreLegion Legion
    {
        get => _Legion ??= new CoreLegion();
        set => _Legion = value;
    }
    private static CoreLegion _Legion;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        Core.BankingBlackList.AddRange(
            new[] { "Legion Trophy", "Technique Observed", "Sword Scroll Fragment" }
        );
        DoLegionCombatTrophy();

        Core.SetOptions(false);
    }

    public void DoLegionCombatTrophy()
    {
        Bot.Options.LagKiller = false;
        //order of quants: Trophy - Technique - Scroll
        Legion.DagePvP();
    }
}
