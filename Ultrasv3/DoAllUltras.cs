/*
name: AllUltras
description: null
tags: null
*/
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreEnginev3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreUltrav3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraGeneral.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraQueue.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraWaitForArmy.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/PrerequisitesChecker.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/1UltraEzrajalv3.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/2UltraWardenv3.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/3UltraEngineerv3.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/4UltraAvatarTyndariusv3.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/5ChampionDrakathv3.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/6UltraNulgathv3.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/7UltraDragov3.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/8UltraDarkonv3.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/9UltraDagev3.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/10UltraSpeakerv3.cs
//cs_include Scripts/Ultrasv3/IndividualUltras/11UltraGramielv3.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class DoAllUltras
{
    private static CoreEnginev3 Core => CoreEnginev3.Instance;
    private static CoreUltrav3 Ultra => _Ultra ??= new CoreUltrav3();
    private static CoreUltrav3 _Ultra;
    private CoreBots C => CoreBots.Instance;
    public IScriptInterface Bot => IScriptInterface.Instance;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "DoAllUltras";
    public List<IOption> Options = new()
    {
        new Option<bool>("UsePrerequisitesChecker", "Use Prerequisites Checker", "Enable to run the prerequisites checker before starting ultras. Disable to skip.", true),
        CoreBots.Instance.SkipOptions,
    };

    private const string BossParticipantSyncFile = "ultrasv3_participants.sync";
    private const string BossSyncFile = "ultrasv3_bosses.sync";

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
        if (Bot.Config!.Get<bool>("UsePrerequisitesChecker"))
        {
            if (!new PrerequisitesChecker().PrerequisiteSyncGate(4))
                return;
        }

        var allBosses = new[] { "UltraEzrajal", "UltraWarden", "UltraEngineer", "UltraAvatarTyndarius", "ChampionDrakath", "UltraNulgath", "UltraDrago", "UltraDarkon", "UltraDage", "UltraSpeaker", "UltraGramiel" };

        int pass = 1;
        while (true)
        {
            var pending = GetSharedBossQueue(allBosses).ToList();
            if (!pending.Any())
                break;

            if (pass > 1)
                C.Logger($"[AllUltras-v3] Re-run pass #{pass}: {pending.Count} boss(es) still pending for some accounts.", "Info");

            RunBossQueue(pending);
            pass++;
        }

        C.Logger("[AllUltras-v3] All Bosses Complete.");
    }

    private void RunBossQueue(IEnumerable<string> bosses)
    {
        foreach (string boss in bosses)
        {
            switch (boss)
            {
                case "UltraEzrajal":
                    new UltraEzrajalv3().RunBoss();
                    break;
                case "UltraWarden":
                    new UltraWardenv3().RunBoss();
                    break;
                case "UltraEngineer":
                    new UltraEngineerv3().RunBoss();
                    break;
                case "UltraAvatarTyndarius":
                    new UltraAvatarTyndariusv3().RunBoss();
                    break;
                case "ChampionDrakath":
                    new ChampionDrakathv3().RunBoss();
                    break;
                case "UltraNulgath":
                    new UltraNulgathv3().RunBoss();
                    break;
                case "UltraDrago":
                    new UltraDragov3().RunBoss();
                    break;
                case "UltraDarkon":
                    new UltraDarkonv3().RunBoss();
                    break;
                case "UltraDage":
                    new UltraDagev3().RunBoss();
                    break;
                case "UltraSpeaker":
                    new UltraSpeakerv3().RunBoss();
                    break;
                case "UltraGramiel":
                    new UltraGramielv3().RunBoss();
                    break;
                default:
                    C.Logger($"Unknown Ultra boss in queue: {boss}", "Error", true, true);
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
            "UltraEzrajal" => (8152, "Ultra Ezrajal"),
            "UltraWarden" => (8153, "Ultra Warden"),
            "UltraEngineer" => (8154, "Ultra Engineer"),
            "UltraAvatarTyndarius" => (8245, "Ultra Avatar Tyndarius"),
            "ChampionDrakath" => (8300, "Champion Drakath"),
            "UltraNulgath" => (8692, "Nulgath the Archfiend"),
            "UltraDrago" => (8397, "King Drago"),
            "UltraDarkon" => (8746, "Darkon the Conductor"),
            "UltraDage" => (8547, "Dage the Dark Lord"),
            "UltraSpeaker" => (9173, "The First Speaker"),
            "UltraGramiel" => (10301, "Gramiel the Graceful"),
            _ => (0, string.Empty),
        };

        if (id == 0)
            return false;

        bool complete = UltraGeneral.IsQuestComplete(Bot, id);
        LogBossQuestStatus(name, id, complete);
        return complete;
    }

    private void LogBossQuestStatus(string name, int questId, bool complete)
    {
        Quest? quest = Bot.Quests.EnsureLoad(questId);
        int slot = quest?.Slot ?? 0;
        int value = quest?.Value ?? 0;
        bool active = quest?.Active ?? false;
        int questValue = slot > 0 ? Bot.Flash.CallGameFunction<int>("world.getQuestValue", slot) : 0;

        C.Logger(
            $"{name} [{questId}] complete={complete} " +
            $"daily={Bot.Quests.IsDailyComplete(questId)} " +
            $"progress={Bot.Quests.IsInProgress(questId)} " +
            $"active={active} " +
            $"everCompleted={Bot.Quests.HasBeenCompleted(questId)} " +
            $"slot={slot} value={value} questValue={questValue}",
            "Info"
        );
    }

    private bool ShouldClearParticipantSync(string path)
    {
        try
        {
            if (!System.IO.File.Exists(path))
                return true;

            return (DateTime.UtcNow - System.IO.File.GetLastWriteTimeUtc(path)).TotalMinutes > 10;
        }
        catch
        {
            return false;
        }
    }

    private void CleanupStaleParticipants(string path)
    {
        try
        {
            string[] lines = Ultra.ReadLines(path);
            if (lines.Length == 0)
                return;

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            const int staleThreshold = 600;
            var freshLines = new List<string>();

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(':');
                if (parts.Length < 3)
                    continue;

                if (!long.TryParse(parts[^1], out long ts))
                    continue;

                if (now - ts <= staleThreshold)
                    freshLines.Add(line);
            }

            if (freshLines.Count == 0)
            {
                Ultra.ClearSyncFile(path);
                return;
            }

            if (freshLines.Count != lines.Length)
            {
                System.IO.File.WriteAllText(path, string.Join(Environment.NewLine, freshLines));
                C.Logger($"[AllUltras-v3] Cleaned stale participant entries; {freshLines.Count} remain.", "Info");
            }
        }
        catch (Exception ex)
        {
            C.Logger($"[AllUltras-v3] Failed to cleanup participant sync: {ex.Message}", "Warning");
        }
    }
}
