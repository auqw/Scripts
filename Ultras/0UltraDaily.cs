/*
name: UltraDaily
description: Do all daily ultras
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreUltra.cs
//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Ultras/UltraEzrajal.cs
//cs_include Scripts/Ultras/UltraWarden.cs
//cs_include Scripts/Ultras/UltraEngineer.cs
//cs_include Scripts/Ultras/UltraAvatarTyndarius.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraDaily
{
    public string OptionsStorage = "0UltraDaily";
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
        new Option<bool>("RunEzrajal", "Ultra Ezrajal", "Handle Ultra Ezrajal", true),
        new Option<bool>("RunWarden", "Ultra Warden", "Handle Ultra Warden", true),
        new Option<bool>("RunEngineer", "Ultra Engineer", "Handle Ultra Engineer", true),
        new Option<bool>("RunAvatarTyndarius", "Ultra Avatar Tyndarius", "Handle Ultra Avatar Tyndarius", true),
    };

    public List<IOption> Compositions = new()
    {
        new Option<UltraEzrajal.EzrajalComp>(
            "Ultra Ezrajal Composition",
            "Ultra Ezrajal Composition",
            "Select the preset composition for Ultra Ezrajal.\n"
                + "Safe: Arcana Invoker / Legion Revenant / ArchPaladin / Lord Of Order\n"
                + "Fast: Chrono ShadowSlayer / Verus DoomKnight / Legion Revenant / Lord Of Order\n"
                + "F2PFastest: Arcana Invoker / Verus DoomKnight / Legion Revenant / Lord Of Order",
            UltraEzrajal.EzrajalComp.Safe
        ),
        new Option<UltraWarden.WardenComp>(
            "Ultra Warden Composition",
            "Ultra Warden Composition",
            "Select the preset composition for Ultra Warden.\n"
                + "Recommended: Legion Revenant / ArchPaladin / Lord Of Order / Verus DoomKnight",
            UltraWarden.WardenComp.Recommended
        ),
        new Option<UltraEngineer.EngineerComp>(
            "Ultra Engineer Composition",
            "Ultra Engineer Composition",
            "Select the preset composition for Ultra Engineer.\n"
                + "Safe: Legion Revenant / StoneCrusher / ArchPaladin / Lord Of Order\n"
                + "Fast: Lich / Legion Revenant / ArchPaladin / Lord Of Order\n"
                + "F2PFast: Arcana Invoker / Legion Revenant / ArchPaladin / Lord Of Order",
            UltraEngineer.EngineerComp.Safe
        ),
        new Option<UltraAvatarTyndarius.TyndariusComp>(
            "Ultra Avatar Tyndarius Composition",
            "Ultra Avatar Tyndarius Composition",
            "Select the preset composition for Ultra Avatar Tyndarius.\n"
                + "Safe: Chaos Avenger / Legion Revenant / ArchPaladin / Lord Of Order\n"
                + "Fast: Chrono ShadowSlayer / Legion Revenant / ArchPaladin / Lord Of Order\n"
                + "F2PFast: King's Echo / Legion Revenant / ArchPaladin / Lord Of Order",
            UltraAvatarTyndarius.TyndariusComp.Safe
        ),
    };

    private readonly Dictionary<string, int> _questIds = new()
    {
        { "Ultra Ezrajal", 8152 },
        { "Ultra Warden", 8153 },
        { "Ultra Engineer", 8154 },
        { "Ultra Avatar Tyndarius", 8245 },
    };
    private const int DailyArmySize = 4;

    public void ScriptMain(IScriptInterface bot)
    {
        C.Logger("[0UltraDaily] Bootstrapping daily ultra handler.");
        var ignoreNeeded = bot.Config == null ? true : bot.Config.Get<bool>("CoreSettings", "IgnoreNeeded");
        var restoreGear = bot.Config == null ? false : bot.Config.Get<bool>("CoreSettings", "RestoreGear");

        var selectedBosses = new[]
        {
            (Name: "Ultra Ezrajal", Enabled: bot.Config == null ? true : bot.Config.Get<bool>("Bosses", "RunEzrajal")),
            (Name: "Ultra Warden", Enabled: bot.Config == null ? true : bot.Config.Get<bool>("Bosses", "RunWarden")),
            (Name: "Ultra Engineer", Enabled: bot.Config == null ? true : bot.Config.Get<bool>("Bosses", "RunEngineer")),
            (Name: "Ultra Avatar Tyndarius", Enabled: bot.Config == null ? true : bot.Config.Get<bool>("Bosses", "RunAvatarTyndarius")),
        };
        
        var ezrajalComp = bot.Config == null
            ? UltraEzrajal.EzrajalComp.Safe
            : bot.Config.Get<UltraEzrajal.EzrajalComp>("Compositions", "Ultra Ezrajal Composition");
        var wardenComp = bot.Config == null
            ? UltraWarden.WardenComp.Recommended
            : bot.Config.Get<UltraWarden.WardenComp>("Compositions", "Ultra Warden Composition");
        var engineerComp = bot.Config == null
            ? UltraEngineer.EngineerComp.Safe
            : bot.Config.Get<UltraEngineer.EngineerComp>("Compositions", "Ultra Engineer Composition");
        var tyndariusComp = bot.Config == null
            ? UltraAvatarTyndarius.TyndariusComp.Safe
            : bot.Config.Get<UltraAvatarTyndarius.TyndariusComp>("Compositions", "Ultra Avatar Tyndarius Composition");

        C.Logger(
            $"[0UltraDaily] Selected flow => IgnoreNeeded={ignoreNeeded}, RestoreGear={restoreGear}"
        );
        var runEzrajal = selectedBosses.First(x => x.Name == "Ultra Ezrajal").Enabled;
        var runWarden = selectedBosses.First(x => x.Name == "Ultra Warden").Enabled;
        var runEngineer = selectedBosses.First(x => x.Name == "Ultra Engineer").Enabled;
        var runAvatar = selectedBosses.First(x => x.Name == "Ultra Avatar Tyndarius").Enabled;
        C.Logger(
            $"[0UltraDaily] Boss flags => Ezrajal={runEzrajal}, Warden={runWarden}, Engineer={runEngineer}, AvatarTyndarius={runAvatar}"
        );

        CoreBots.Instance.SetOptions();
        Adv.GearStore();
        C.Logger("[0UltraDaily] Core options captured and initial gear stored.");

        try
        {
            if (selectedBosses.All(x => !x.Item2))
            {
                C.Logger("[0UltraDaily] No ultra bosses enabled; nothing to do.");
                return;
            }

            C.Logger("[0UltraDaily] Starting boss routing loop.");

            if (runEzrajal)
            {
                C.Logger("[0UltraDaily] Boss enabled: Ultra Ezrajal");
                if (!ignoreNeeded && ShouldSkip("Ultra Ezrajal"))
                    C.Logger("[0UltraDaily] Skip Ultra Ezrajal (need check says not required).");
                else
                {
                    var script = new UltraEzrajal();
                    RunUltra(
                        () => script.Run(
                            comp: ezrajalComp,
                            equipBestGear: true,
                            doEnhancements: true,
                            useLifeSteal: true
                        ),
                        "Ultra Ezrajal"
                    );
                }
                C.Logger("[0UltraDaily] Boss routing complete: Ultra Ezrajal");
            }
            else
            {
                C.Logger("[0UltraDaily] Boss skipped by config: Ultra Ezrajal");
            }

            if (runWarden)
            {
                C.Logger("[0UltraDaily] Boss enabled: Ultra Warden");
                if (!ignoreNeeded && ShouldSkip("Ultra Warden"))
                    C.Logger("[0UltraDaily] Skip Ultra Warden (need check says not required).");
                else
                {
                    var script = new UltraWarden();
                    RunUltra(
                        () => script.Run(
                            comp: wardenComp,
                            equipBestGear: true,
                            useLifeSteal: true
                        ),
                        "Ultra Warden"
                    );
                }
                C.Logger("[0UltraDaily] Boss routing complete: Ultra Warden");
            }
            else
            {
                C.Logger("[0UltraDaily] Boss skipped by config: Ultra Warden");
            }

            if (runEngineer)
            {
                C.Logger("[0UltraDaily] Boss enabled: Ultra Engineer");
                if (!ignoreNeeded && ShouldSkip("Ultra Engineer"))
                    C.Logger("[0UltraDaily] Skip Ultra Engineer (need check says not required).");
                else
                {
                    var script = new UltraEngineer();
                    RunUltra(
                        () => script.Run(
                            comp: engineerComp,
                            equipBestGear: true,
                            doEnhancements: true,
                            useLifeSteal: true
                        ),
                        "Ultra Engineer"
                    );
                }
                C.Logger("[0UltraDaily] Boss routing complete: Ultra Engineer");
            }
            else
            {
                C.Logger("[0UltraDaily] Boss skipped by config: Ultra Engineer");
            }

            if (runAvatar)
            {
                C.Logger("[0UltraDaily] Boss enabled: Ultra Avatar Tyndarius");
                if (!ignoreNeeded && ShouldSkip("Ultra Avatar Tyndarius"))
                    C.Logger("[0UltraDaily] Skip Ultra Avatar Tyndarius (need check says not required).");
                else
                {
                    var script = new UltraAvatarTyndarius();
                    RunUltra(
                        () => script.Run(
                            comp: tyndariusComp,
                            equipBestGear: true,
                            doEnhancements: true,
                            useLifeSteal: true
                        ),
                        "Ultra Avatar Tyndarius"
                    );
                }
                C.Logger("[0UltraDaily] Boss routing complete: Ultra Avatar Tyndarius");
            }
            else
            {
                C.Logger("[0UltraDaily] Boss skipped by config: Ultra Avatar Tyndarius");
            }
        }
        catch (Exception ex)
        {
            C.Logger($"[0UltraDaily] Boss routing failed: {ex.GetType().Name}: {ex.Message}");
            throw;
        }
        finally
        {
            C.Logger("[0UltraDaily] Entering finalization block.");
            if (restoreGear)
                Adv.GearStore(true, true);
            CoreBots.Instance.SetOptions(false);
            bot.StopSync();
            C.Logger("[0UltraDaily] Finalization complete.");
        }
    }

    private bool ShouldSkip(string bossName)
    {
        if (!_questIds.TryGetValue(bossName, out var questId))
        {
            C.Logger($"[0UltraDaily] No questId mapping for {bossName}. Running by default.");
            return false;
        }

        bool localShouldRun = Ultra.ShouldRunQuest(questId, bossName);
        bool armyShouldRun = ShouldRunQuestForArmy(questId, bossName, localShouldRun);
        C.Logger($"[0UltraDaily] Quest check for {bossName} [{questId}] => local={(localShouldRun ? "RUN" : "SKIP")} | army={(armyShouldRun ? "RUN" : "SKIP")}.");
        return !armyShouldRun;
    }

    private void RunUltra(Action run, string bossName)
    {
        C.Logger($"[0UltraDaily] Starting {bossName}.");
        var started = DateTime.UtcNow;
        try
        {
            run();
            C.Logger(
                $"[0UltraDaily] Completed {bossName} in {(DateTime.UtcNow - started).TotalSeconds:F1}s."
            );
        }
        catch (Exception ex)
        {
            C.Logger($"[0UltraDaily] {bossName} threw exception after {(DateTime.UtcNow - started).TotalSeconds:F1}s: {ex.GetType().Name}: {ex.Message}");
            throw;
        }
    }

    private bool ShouldRunQuestForArmy(int questId, string bossName, bool localShouldRun)
    {
        string syncPath = Ultra.ResolveSyncPath($"daily_need_{questId}.sync");
        string username = (IScriptInterface.Instance.Player?.Username ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(username))
            username = $"unknown_{Environment.TickCount}";

        UpsertNeedEntry(syncPath, username, localShouldRun);

        DateTime waitUntil = DateTime.UtcNow.AddSeconds(15);
        int seen = 0;
        while (!IScriptInterface.Instance.ShouldExit && DateTime.UtcNow < waitUntil)
        {
            var entries = ReadNeedEntries(syncPath);
            seen = entries.Count;
            if (seen >= DailyArmySize)
                break;

            UpsertNeedEntry(syncPath, username, localShouldRun);
            IScriptInterface.Instance.Sleep(300);
        }

        var finalEntries = ReadNeedEntries(syncPath);
        if (finalEntries.Count < DailyArmySize)
        {
            C.Logger($"[0UltraDaily] Army need check for {bossName} incomplete ({finalEntries.Count}/{DailyArmySize}); running for safety.");
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

    private IScriptInterface bot => IScriptInterface.Instance;

}
