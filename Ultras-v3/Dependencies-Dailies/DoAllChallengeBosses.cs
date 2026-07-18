/*
name: DoAllChallengeBosses
description: Runs all challenge boss dailies with shared queue — Queen Iona > Kolr > Kathool > Astral Empyrean.
tags: all, challenge, bosses, dailies, queeniona, kolr, kathool, astralempyrean
*/
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreEngine2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreUltra2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraGeneral.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraQueue.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraWaitForArmy.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/PrerequisitesChecker.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreAdvanced2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Dailies/QueenIonaNoOptionv3.cs
//cs_include Scripts/Ultras-v3/Dependencies-Dailies/KolrNoOptionv3.cs
//cs_include Scripts/Ultras-v3/Dependencies-Dailies/KathoolNoOptionv3.cs
//cs_include Scripts/Ultras-v3/Dependencies-Dailies/AstralEmpyreanNoOptionv3.cs

using System;
using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class DoAllChallengeBosses
{
    private static CoreEngine2 Core => CoreEngine2.Instance;
    private static CoreUltra2 Ultra => _Ultra ??= new CoreUltra2();
    private static CoreUltra2 _Ultra;
    private CoreBots2 C => CoreBots2.Instance;
    public IScriptInterface Bot => IScriptInterface.Instance;

    private const string BossParticipantSyncFile = "challengeboss_participants.sync";
    private const string BossSyncFile = "challengeboss_bosses.sync";

    public void ScriptMain(IScriptInterface bot)
    {
        C.SetOptions(disableCoreSkills: true);
        Core.Boot();

        RunAll();

        Core.DisableSkills();
        C.SetOptions(false);
        Bot.StopSync();
    }

    public void RunAll()
    {
        if (!new PrerequisitesChecker().PrerequisiteSyncGate(4))
            return;

        var allBosses = new[] { "QueenIona", "Kolr", "Kathool", "AstralEmpyrean" };

        int pass = 1;
        while (true)
        {
            var pending = GetSharedBossQueue(allBosses).ToList();
            if (!pending.Any())
                break;

            if (pass > 1)
                C.Logger($"[DoAllChallengeBosses] Re-run pass #{pass}: {pending.Count} boss(es) still pending.", "Info");

            RunBossQueue(pending);
            pass++;
        }

        C.Logger("[DoAllChallengeBosses] All Challenge Bosses Complete.");
    }

    private void RunBossQueue(IEnumerable<string> bosses)
    {
        foreach (string boss in bosses)
        {
            switch (boss)
            {
                case "QueenIona":
                    new QueenIonav3().RunBoss();
                    break;
                case "Kolr":
                    new Kolrv3().RunBoss();
                    break;
                case "Kathool":
                    new Kathoolv3().RunBoss();
                    break;
                case "AstralEmpyrean":
                    new AstralEmpyreanv3().RunBoss();
                    break;
                default:
                    C.Logger($"Unknown challenge boss in queue: {boss}", "Error", true, true);
                    break;
            }
        }
    }

    private IEnumerable<string> GetSharedBossQueue(IEnumerable<string> bosses)
        => UltraQueue.GetSharedBossQueue(Ultra, Bot, C, bosses, BossSyncFile, BossParticipantSyncFile, IsBossComplete);

    private bool IsBossComplete(string boss)
    {
        (int id, string name) = boss switch
        {
            "QueenIona" => (9852, "Queen Iona"),
            "Kolr" => (10715, "Kolr, Usurper of Flames"),
            "Kathool" => (9350, "God of the Depths"),
            "AstralEmpyrean" => (9803, "Astral Empyrean"),
            _ => (0, string.Empty),
        };

        if (id == 0)
            return false;

        bool complete = Bot.Quests.IsDailyComplete(id);
        C.Logger($"{name} [{id}] dailyComplete={complete}", "Info");
        return complete;
    }
}
