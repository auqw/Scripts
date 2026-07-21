/*
name: DoAllChallengeBosses
description: Runs all challenge boss dailies with shared queue — Queen Iona > Kolr > Kathool > Astral Empyrean.
tags: all, challenge, bosses, dailies, queeniona, kolr, kathool, astralempyrean
*/
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreEnginev3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreUltrav3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraGeneral.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraQueue.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraWaitForArmy.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/PrerequisitesChecker.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Ultrasv3/DependenciesDailies/QueenIonaNoOptionv3.cs
//cs_include Scripts/Ultrasv3/DependenciesDailies/KolrNoOptionv3.cs
//cs_include Scripts/Ultrasv3/DependenciesDailies/KathoolNoOptionv3.cs
//cs_include Scripts/Ultrasv3/DependenciesDailies/AstralEmpyreanNoOptionv3.cs

using System;
using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class DoAllChallengeBosses
{
    private static CoreEnginev3 Core => CoreEnginev3.Instance;
    private static CoreUltrav3 Ultra => _Ultra ??= new CoreUltrav3();
    private static CoreUltrav3 _Ultra;
    private CoreBots C => CoreBots.Instance;
    public IScriptInterface Bot => IScriptInterface.Instance;

    private const string BossParticipantSyncFile = "challengeboss_participants.sync";
    private const string BossSyncFile = "challengeboss_bosses.sync";

    public void ScriptMain(IScriptInterface bot)
    {
        C.SetOptions(true);
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

        var allBosses = new[] { "QueenIona", "Kolr", "AstralEmpyrean", "Kathool" };

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
                    new QueenIonaNoOpv3().RunBoss();
                    break;
                case "Kolr":
                    new KolrNoOpv3().RunBoss();
                    break;
                case "Kathool":
                    new KathoolNoOpv3().RunBoss();
                    break;
                case "AstralEmpyrean":
                    new AstralEmpyreanNoOpv3().RunBoss();
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
