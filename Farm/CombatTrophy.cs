/*
name: Combat Trophy
description: This script will farm Combat Trophies in /bludrutbtawl
tags: combat, trophy, pvp, bludrut, brawl
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class CombatTrophy
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

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(
            new[] { "The Secret 4", "Yoshino's Citrine", "Combat Trophy" }
        );
        Core.SetOptions();

        DoCombatTrophy();

        Core.SetOptions(false);
    }

    public void DoCombatTrophy()
    {
        Core.Logger(
            "90% of this farm is going to be transitioning rooms during pvp. There is nothing we can do to speed this up as it takes 2 return packets from the server a move and a \"mcid\" packet to actualy transition rooms. ( no you can't jump cells as itll soft-lock the map, and yes we've tried teh walking method its **Very** unrelaible as you can just get stuck at doors.)"
        );
        Farm.BludrutBrawlBoss();
    }
}
