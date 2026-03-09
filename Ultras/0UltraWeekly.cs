/*
name: UltraWeekly
description: Do all weekly ultras
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreUltra.cs
//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Ultras/ChampionDrakath.cs
//cs_include Scripts/Ultras/UltraDrago.cs
//cs_include Scripts/Ultras/UltraDage.cs
//cs_include Scripts/Ultras/UltraDarkon.cs
//cs_include Scripts/Ultras/UltraNulgath.cs
//cs_include Scripts/Ultras/UltraGramiel.cs
//cs_include Scripts/Ultras/UltraSpeaker.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraWeekly
{
    public string OptionsStorage = "0UltraWeekly";
    public bool DontPreconfigure = true;

    private CoreAdvanced Adv => _Adv ??= new CoreAdvanced();
    private static CoreAdvanced? _Adv;
    private CoreUltra Ultra = new();
    private CoreBots C => CoreBots.Instance;

    public string[] MultiOptions =
    {
        "Main",
        "CoreSettings",
        "Bosses",
        "Compositions",
    };

    public List<IOption> CoreSettings = new()
    {
        new Option<bool>("IgnoreNeeded", "Ignore Needed Check", "Run selected ultras even if quest/state check says account may not need it.", false),
        new Option<bool>("RestoreGear", "Restore Gear", "Restore gear and consumable state at the end of this handler.", false),
    };

    public List<IOption> Bosses = new()
    {
        new Option<bool>("RunChampionDrakath", "Champion Drakath", "Handle Champion Drakath", true),
        new Option<bool>("RunUltraDrago", "Ultra Drago", "Handle Ultra Drago", true),
        new Option<bool>("RunUltraDage", "Ultra Dage", "Handle Ultra Dage", true),
        new Option<bool>("RunUltraDarkon", "Ultra Darkon", "Handle Ultra Darkon", true),
        new Option<bool>("RunUltraNulgath", "Ultra Nulgath", "Handle Ultra Nulgath", true),
        new Option<bool>("RunUltraGramiel", "Ultra Gramiel", "Handle Ultra Gramiel", true),
        new Option<bool>("RunUltraSpeaker", "Ultra Speaker", "Handle Ultra Speaker", true),
    };

    public List<IOption> Compositions = new()
    {
        new Option<ChampionDrakath.DrakathComp>(
            "Champion Drakath Composition",
            "Champion Drakath Composition",
            "Safe: AP / LR / SC / LOO\n"
                + "Fast: CSS/CSH / LR / PCM/OPCM / LOO\n"
                + "Cheapest: CS / AP / SC / LOO",
            ChampionDrakath.DrakathComp.Safe
        ),
        new Option<UltraDarkon.DarkonComp>(
            "Ultra Darkon Composition",
            "Ultra Darkon Composition",
            "Recommended: LC / LR / SC(or IT) / LOO",
            UltraDarkon.DarkonComp.Recommended
        ),
        new Option<UltraDrago.DragoComp>(
            "Ultra Drago Composition",
            "Ultra Drago Composition",
            "Fast: CSS / LR / AP / LOO\n"
                + "Safe: CAv / LR / AP / LOO\n"
                + "F2PFast: KE / LR / AP / LOO",
            UltraDrago.DragoComp.Safe
        ),
        new Option<UltraDage.DageComp>(
            "Ultra Dage Composition",
            "Ultra Dage Composition",
            "BestAvailable: CAv / AP / Best DPS / Best DPS",
            UltraDage.DageComp.BestAvailable
        ),
        new Option<UltraNulgath.NulgathComp>(
            "Ultra Nulgath Composition",
            "Ultra Nulgath Composition",
            "Fast: CSS / VDK / LR / LOO\n"
                + "F2PFast: DoT / DoT / LR / LOO\n"
                + "Common: KE / LR / AP / LOO\n"
                + "Balanced: LR / AP / SC / LOO",
            UltraNulgath.NulgathComp.Fast
        ),
        new Option<UltraGramiel.GramielComp>(
            "Ultra Gramiel Composition",
            "Ultra Gramiel Composition",
            "Recommended: SC/IT / AP / LOO / VHL\n"
                + "Alternate: SC/IT / LC / LOO / VDK",
            UltraGramiel.GramielComp.Recommended
        ),
        new Option<UltraSpeaker.SpeakerComp>(
            "Ultra Speaker Composition",
            "Ultra Speaker Composition",
            "Fast: AP / LR / QCM / LOO\n"
                + "Safe: LR / AP / LOO / VDK",
            UltraSpeaker.SpeakerComp.Safe
        ),
    };

    private readonly Dictionary<string, int> _questIds = new()
    {
        { "Champion Drakath", 8300 },
        { "Ultra Drago", 8397 },
        { "Ultra Dage", 8547 },
        { "Ultra Darkon", 8746 },
        { "Ultra Nulgath", 8692 },
        { "Ultra Gramiel", 10301 },
        { "Ultra Speaker", 9173 },
    };
    private const int WeeklyArmySize = 4;

    public void ScriptMain(IScriptInterface bot)
    {
        C.Logger("[0UltraWeekly] Bootstrapping weekly ultra handler.");
        var ignoreNeeded = bot.Config == null ? false : bot.Config.Get<bool>("CoreSettings", "IgnoreNeeded");
        var restoreGear = bot.Config == null ? false : bot.Config.Get<bool>("CoreSettings", "RestoreGear");

        var selectedBosses = new[]
        {
            (Name: "Champion Drakath", Enabled: bot.Config == null ? true : bot.Config.Get<bool>("Bosses", "RunChampionDrakath")),
            (Name: "Ultra Drago", Enabled: bot.Config == null ? true : bot.Config.Get<bool>("Bosses", "RunUltraDrago")),
            (Name: "Ultra Dage", Enabled: bot.Config == null ? true : bot.Config.Get<bool>("Bosses", "RunUltraDage")),
            (Name: "Ultra Darkon", Enabled: bot.Config == null ? true : bot.Config.Get<bool>("Bosses", "RunUltraDarkon")),
            (Name: "Ultra Nulgath", Enabled: bot.Config == null ? true : bot.Config.Get<bool>("Bosses", "RunUltraNulgath")),
            (Name: "Ultra Gramiel", Enabled: bot.Config == null ? true : bot.Config.Get<bool>("Bosses", "RunUltraGramiel")),
            (Name: "Ultra Speaker", Enabled: bot.Config == null ? true : bot.Config.Get<bool>("Bosses", "RunUltraSpeaker")),
        };
        var drakathComp = bot.Config == null
            ? ChampionDrakath.DrakathComp.Safe
            : bot.Config.Get<ChampionDrakath.DrakathComp>("Compositions", "Champion Drakath Composition");
        var darkonComp = bot.Config == null
            ? UltraDarkon.DarkonComp.Recommended
            : bot.Config.Get<UltraDarkon.DarkonComp>("Compositions", "Ultra Darkon Composition");
        var dragoComp = bot.Config == null
            ? UltraDrago.DragoComp.Safe
            : bot.Config.Get<UltraDrago.DragoComp>("Compositions", "Ultra Drago Composition");
        var dageComp = bot.Config == null
            ? UltraDage.DageComp.BestAvailable
            : bot.Config.Get<UltraDage.DageComp>("Compositions", "Ultra Dage Composition");
        var nulgathComp = bot.Config == null
            ? UltraNulgath.NulgathComp.Fast
            : bot.Config.Get<UltraNulgath.NulgathComp>("Compositions", "Ultra Nulgath Composition");
        var gramielComp = bot.Config == null
            ? UltraGramiel.GramielComp.Recommended
            : bot.Config.Get<UltraGramiel.GramielComp>("Compositions", "Ultra Gramiel Composition");
        var speakerComp = bot.Config == null
            ? UltraSpeaker.SpeakerComp.Safe
            : bot.Config.Get<UltraSpeaker.SpeakerComp>("Compositions", "Ultra Speaker Composition");

        var runChampionDrakath = selectedBosses.First(x => x.Name == "Champion Drakath").Enabled;
        var runDrago = selectedBosses.First(x => x.Name == "Ultra Drago").Enabled;
        var runDage = selectedBosses.First(x => x.Name == "Ultra Dage").Enabled;
        var runDarkon = selectedBosses.First(x => x.Name == "Ultra Darkon").Enabled;
        var runNulgath = selectedBosses.First(x => x.Name == "Ultra Nulgath").Enabled;
        var runGramiel = selectedBosses.First(x => x.Name == "Ultra Gramiel").Enabled;
        var runSpeaker = selectedBosses.First(x => x.Name == "Ultra Speaker").Enabled;

        C.Logger(
            $"[0UltraWeekly] Selected flow => IgnoreNeeded={ignoreNeeded}, RestoreGear={restoreGear}"
        );
        C.Logger(
            $"[0UltraWeekly] Boss flags => ChampionDrakath={runChampionDrakath}, Drago={runDrago}, Dage={runDage}, Nulgath={runNulgath}, Darkon={runDarkon}, Gramiel={runGramiel}, Speaker={runSpeaker}"
        );

        CoreBots.Instance.SetOptions();
        Adv.GearStore();
        C.Logger("[0UltraWeekly] Core options captured and initial gear stored.");

        try
        {
            if (selectedBosses.All(x => !x.Item2))
            {
                C.Logger("[0UltraWeekly] No ultra bosses enabled; nothing to do.");
                return;
            }

            C.Logger("[0UltraWeekly] Starting boss routing loop.");

            if (runChampionDrakath)
            {
                C.Logger("[0UltraWeekly] Boss enabled: Champion Drakath");
                if (!ignoreNeeded && ShouldSkip("Champion Drakath"))
                    C.Logger("[0UltraWeekly] Skip Champion Drakath (need check says not required).");
                else
                    RunUltra(() => new ChampionDrakath().Run(comp: drakathComp, equipBestGear: true, doEnhancements: true, useLifeSteal: true), "Champion Drakath");

                C.Logger("[0UltraWeekly] Boss routing complete: Champion Drakath");
            }
            else
            {
                C.Logger("[0UltraWeekly] Boss skipped by config: Champion Drakath");
            }

            if (runDrago)
            {
                C.Logger("[0UltraWeekly] Boss enabled: Ultra Drago");
                if (!ignoreNeeded && ShouldSkip("Ultra Drago"))
                    C.Logger("[0UltraWeekly] Skip Ultra Drago (need check says not required).");
                else
                    RunUltra(() => new UltraDrago().Run(comp: dragoComp, doEnhancements: true), "Ultra Drago");

                C.Logger("[0UltraWeekly] Boss routing complete: Ultra Drago");
            }
            else
            {
                C.Logger("[0UltraWeekly] Boss skipped by config: Ultra Drago");
            }

            if (runDage)
            {
                C.Logger("[0UltraWeekly] Boss enabled: Ultra Dage");
                if (!ignoreNeeded && ShouldSkip("Ultra Dage"))
                    C.Logger("[0UltraWeekly] Skip Ultra Dage (need check says not required).");
                else
                    RunUltra(() => new UltraDage().Run(comp: dageComp, doEnhancements: true), "Ultra Dage");

                C.Logger("[0UltraWeekly] Boss routing complete: Ultra Dage");
            }
            else
            {
                C.Logger("[0UltraWeekly] Boss skipped by config: Ultra Dage");
            }

            if (runNulgath)
            {
                C.Logger("[0UltraWeekly] Boss enabled: Ultra Nulgath");
                if (!ignoreNeeded && ShouldSkip("Ultra Nulgath"))
                    C.Logger("[0UltraWeekly] Skip Ultra Nulgath (need check says not required).");
                else
                    RunUltra(() => new UltraNulgath().Run(comp: nulgathComp, equipBestGear: true, doEnhancements: true, useLifeSteal: true), "Ultra Nulgath");

                C.Logger("[0UltraWeekly] Boss routing complete: Ultra Nulgath");
            }
            else
            {
                C.Logger("[0UltraWeekly] Boss skipped by config: Ultra Nulgath");
            }

            if (runDarkon)
            {
                C.Logger("[0UltraWeekly] Boss enabled: Ultra Darkon");
                if (!ignoreNeeded && ShouldSkip("Ultra Darkon"))
                    C.Logger("[0UltraWeekly] Skip Ultra Darkon (need check says not required).");
                else
                    RunUltra(() => new UltraDarkon().Run(comp: darkonComp, equipBestGear: true, doEnhancements: true, useLifeSteal: true), "Ultra Darkon");

                C.Logger("[0UltraWeekly] Boss routing complete: Ultra Darkon");
            }
            else
            {
                C.Logger("[0UltraWeekly] Boss skipped by config: Ultra Darkon");
            }

            if (runGramiel)
            {
                C.Logger("[0UltraWeekly] Boss enabled: Ultra Gramiel");
                if (!ignoreNeeded && ShouldSkip("Ultra Gramiel"))
                    C.Logger("[0UltraWeekly] Skip Ultra Gramiel (need check says not required).");
                else
                    RunUltra(() => new UltraGramiel().Run(comp: gramielComp, doEnhancements: true), "Ultra Gramiel");

                C.Logger("[0UltraWeekly] Boss routing complete: Ultra Gramiel");
            }
            else
            {
                C.Logger("[0UltraWeekly] Boss skipped by config: Ultra Gramiel");
            }

            if (runSpeaker)
            {
                C.Logger("[0UltraWeekly] Boss enabled: Ultra Speaker");
                if (!ignoreNeeded && ShouldSkip("Ultra Speaker"))
                    C.Logger("[0UltraWeekly] Skip Ultra Speaker (need check says not required).");
                else
                    RunUltra(() => new UltraSpeaker().Run(comp: speakerComp, doEnhancements: true), "Ultra Speaker");

                C.Logger("[0UltraWeekly] Boss routing complete: Ultra Speaker");
            }
            else
            {
                C.Logger("[0UltraWeekly] Boss skipped by config: Ultra Speaker");
            }
        }
        catch (Exception ex)
        {
            C.Logger($"[0UltraWeekly] Boss routing failed: {ex.GetType().Name}: {ex.Message}");
            throw;
        }
        finally
        {
            C.Logger("[0UltraWeekly] Entering finalization block.");
            if (restoreGear)
                Adv.GearStore(true, true);
            CoreBots.Instance.SetOptions(false);
            bot.StopSync();
            C.Logger("[0UltraWeekly] Finalization complete.");
        }
    }

    private bool ShouldSkip(string bossName)
    {
        if (!_questIds.TryGetValue(bossName, out var questId))
        {
            C.Logger($"[0UltraWeekly] No questId mapping for {bossName}. Running by default.");
            return false;
        }

        bool localShouldRun = Ultra.ShouldRunQuest(questId, bossName);
        bool armyShouldRun = ShouldRunQuestForArmy(questId, bossName, localShouldRun);
        C.Logger($"[0UltraWeekly] Quest check for {bossName} [{questId}] => local={(localShouldRun ? "RUN" : "SKIP")} | army={(armyShouldRun ? "RUN" : "SKIP")}.");
        return !armyShouldRun;
    }

    private void RunUltra(Action run, string bossName)
    {
        C.Logger($"[0UltraWeekly] Starting {bossName}.");
        var started = DateTime.UtcNow;
        try
        {
            run();
            C.Logger(
                $"[0UltraWeekly] Completed {bossName} in {(DateTime.UtcNow - started).TotalSeconds:F1}s."
            );
        }
        catch (Exception ex)
        {
            C.Logger(
                $"[0UltraWeekly] {bossName} threw exception after {(DateTime.UtcNow - started).TotalSeconds:F1}s: {ex.GetType().Name}: {ex.Message}"
            );
            throw;
        }
    }

    private bool ShouldRunQuestForArmy(int questId, string bossName, bool localShouldRun)
    {
        string syncPath = Ultra.ResolveSyncPath($"weekly_need_{questId}.sync");
        string username = (IScriptInterface.Instance.Player?.Username ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(username))
            username = $"unknown_{Environment.TickCount}";

        UpsertNeedEntry(syncPath, username, localShouldRun);

        DateTime waitUntil = DateTime.UtcNow.AddSeconds(15);
        while (!IScriptInterface.Instance.ShouldExit && DateTime.UtcNow < waitUntil)
        {
            var entries = ReadNeedEntries(syncPath);
            if (entries.Count >= WeeklyArmySize)
                break;

            UpsertNeedEntry(syncPath, username, localShouldRun);
            IScriptInterface.Instance.Sleep(300);
        }

        var finalEntries = ReadNeedEntries(syncPath);
        if (finalEntries.Count < WeeklyArmySize)
        {
            C.Logger($"[0UltraWeekly] Army need check for {bossName} incomplete ({finalEntries.Count}/{WeeklyArmySize}); running for safety.");
            return true;
        }

        return finalEntries.Values.Any(v => v);
    }

    private Dictionary<string, bool> ReadNeedEntries(string path)
    {
        Dictionary<string, (bool need, long ts)> latest = new(StringComparer.OrdinalIgnoreCase);
        string[] lines = Array.Empty<string>();
        try
        {
            if (File.Exists(path))
                lines = File.ReadAllLines(path);
        }
        catch
        {
            return new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        }

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        foreach (string line in lines)
        {
            string[] parts = line.Split(':');
            if (parts.Length < 3)
                continue;
            string user = parts[0];
            if (string.IsNullOrWhiteSpace(user))
                continue;
            if (!int.TryParse(parts[1], out int needInt))
                continue;
            if (!long.TryParse(parts[2], out long ts))
                continue;
            if (now - ts > 180)
                continue;

            bool need = needInt == 1;
            if (!latest.TryGetValue(user, out var current) || ts >= current.ts)
                latest[user] = (need, ts);
        }

        return latest.ToDictionary(k => k.Key, v => v.Value.need, StringComparer.OrdinalIgnoreCase);
    }

    private void UpsertNeedEntry(string path, string username, bool need)
    {
        for (int attempt = 0; attempt < 12; attempt++)
        {
            try
            {
                var existing = ReadNeedEntries(path);
                existing[username] = need;
                long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var outLines = existing.Select(kv => $"{kv.Key}:{(kv.Value ? 1 : 0)}:{now}").ToArray();
                string? dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(dir))
                    Directory.CreateDirectory(dir);
                File.WriteAllLines(path, outLines);
                return;
            }
            catch (IOException)
            {
                IScriptInterface.Instance.Sleep(120);
            }
            catch
            {
                return;
            }
        }
    }
}
