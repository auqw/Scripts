/*
name: Quest Data Sync Generator
description: Automatically synchronizes QuestData.json by performing incremental delta updates or full rebuilds when required. Supports configurable target quest ID limits, safe bootstrap for empty datasets, and dual-file mirroring between SkuaDIR and ScriptsDIR. Includes optional full rebuild mode with automatic backups and change tracking for reliable recovery and data integrity.
tags: quest sync, data generation, json sync, incremental update, full rebuild, backup system, skua automation, game data management
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.DependencyInjection;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Quests;
using Skua.Core.Options;
using Skua.Core.Scripts;

public class QuestFileUpdater
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    private IQuestDataLoaderService? service;
    private CancellationTokenSource? _loaderCTS;

    // =========================
    // ⚙️ OPTIONS
    // =========================
    public string OptionsStorage = "QuestFileUpdater";
    public bool DontPreconfigure = true;

    public List<IOption> Options =
    [
        new Option<int>(
            "TargetQuestID",
            "Target Quest ID",
            "Stop syncing when this quest ID is reached",
            10787
        ),
       new Option<bool>(
            "ForceFullRebuild",
            "Force Full Rebuild",
            "Deletes both QuestData files and rebuilds from scratch (creates backup first)",
            false
        ),
        new Option<string>(
            "QuestRange",
            "Quest ID Range (start,end)",
            "Example: 1000,2000. Empty = auto",
            ""
        ),

        CoreBots.Instance.SkipOptions,
    ];

    // =========================
    // 🚀 ENTRY
    // =========================
    public void ScriptMain(IScriptInterface bot)
    {
        try
        {

            Core.SetOptions();

            _loaderCTS = new();

            Core.Logger("Starting quest generation...");
            UpdateQuests(_loaderCTS.Token);
        }
        catch (Exception ex)
        {
            Core.Logger("FATAL ERROR: " + ex);
        }
        finally
        {
            _loaderCTS?.Dispose();
            _loaderCTS = null;

            Core.Logger("Update Complete.");
        }
        Core.SetOptions(false);
    }

    // =========================
    // 🔄 MAIN UPDATE
    // =========================
    private void UpdateQuests(CancellationToken token)
    {
        bool forceRebuild = Bot.Config!.Get<bool>("ForceFullRebuild");

        string clientPath = Path.Combine(ClientFileSources.SkuaDIR, "QuestData.json");
        string scriptsPath = Path.Combine(ClientFileSources.SkuaScriptsDIR, "QuestData.json");

        _loaderCTS = CancellationTokenSource.CreateLinkedTokenSource(token);

        IQuestDataLoaderService loader =
            service ??= Ioc.Default.GetRequiredService<IQuestDataLoaderService>();

        // =========================
        // ⚠ FULL REBUILD MODE
        // =========================
        if (forceRebuild)
        {
            Core.Logger("⚠ ForceFullRebuild enabled - creating backup and resetting data");

            string backupDir = Path.Combine(ClientFileSources.SkuaDIR, "QuestBackups");
            Directory.CreateDirectory(backupDir);

            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");

            // Backup and delete client file
            if (File.Exists(clientPath))
            {
                string backupClient = Path.Combine(backupDir, $"QuestData_client_{timestamp}.json");
                File.Copy(clientPath, backupClient, true);
                File.Delete(clientPath);
                Core.Logger($"Backed up client file → {backupClient}");
            }

            // Backup and delete scripts file
            if (File.Exists(scriptsPath))
            {
                string backupScripts = Path.Combine(backupDir, $"QuestData_scripts_{timestamp}.json");
                File.Copy(scriptsPath, backupScripts, true);
                File.Delete(scriptsPath);
                Core.Logger($"Backed up scripts file → {backupScripts}");
            }

            Core.Logger("Reset complete → rebuilding from scratch");

            // Fetch fresh quest data without file dependency
            int targetMaxId = Bot.Config.Get<int>("TargetQuestID");
            List<QuestData> rebuilt = BootstrapFresh(loader, targetMaxId, token);

            // Save the rebuilt data to both locations
            SaveAndMirror(rebuilt, clientPath, scriptsPath);
            Core.Logger($"Rebuild complete: {rebuilt.Count} quests saved");
            Cleanup();
            return; // 🔴 CRITICAL: exit here - don't continue to delta mode
        }

        // =========================
        // RANGE PARSE (DELTA MODE)
        // =========================
        int targetMaxId_delta =
            Bot.Config?.Get<int>("TargetQuestID") ?? 0;

        string range =
            Bot.Config?.Get<string>("QuestRange") ?? "";

        bool hasRange = !string.IsNullOrWhiteSpace(range);

        int startId;
        int endId;

        if (hasRange)
        {
            string[] split = range.Split(',');

            startId = (split.Length > 0 && int.TryParse(split[0], out int s)) ? s : 0;
            endId = (split.Length > 1 && int.TryParse(split[1], out int e)) ? e : targetMaxId_delta;

            Core.Logger($"⚠ FORCE RANGE REGEN: {startId} → {endId}");
        }
        else
        {
            List<QuestData> existing =
                File.Exists(clientPath)
                    ? loader.GetFromFileAsync(clientPath).GetAwaiter().GetResult()
                    : new List<QuestData>();

            startId = existing.Count > 0
                ? existing.Max(x => x.ID) + 1
                : 1;

            endId = targetMaxId_delta;

            Core.Logger($"Delta mode: {startId} → {endId}");
        }

        // =========================
        // LOAD EXISTING
        // =========================
        List<QuestData> existingData =
            File.Exists(clientPath)
                ? loader.GetFromFileAsync(clientPath).GetAwaiter().GetResult()
                : new List<QuestData>();

        Dictionary<int, QuestData> map = new(existingData.Count);

        foreach (QuestData q in existingData)
            map[q.ID] = q;

        // =========================
        // FORCE REGEN MODE CLEANUP
        // =========================
        if (hasRange)
        {
            existingData.RemoveAll(q => q.ID >= startId && q.ID <= endId);
            Core.Logger($"Cleared existing range data ({startId}-{endId})");
        }

        List<QuestData> incoming = new();
        int window = 200;

        // =========================
        // FETCH LOOP (SAFE)
        // =========================
        List<(int Start, int End)> segments = BuildSegments(startId, endId, window);

        foreach ((int start, int end) in segments)
        {
            if (Bot.ShouldExit)
                break;

            token.ThrowIfCancellationRequested();

            Core.Logger($"Fetching segment: {start} → {end}");

            List<QuestData> batch =
                loader.UpdateRangeAsync(
                        clientPath,
                        start,
                        end,
                        null,
                        token
                    )
                    .GetAwaiter()
                    .GetResult();

            if (batch.Count == 0)
                continue;

            incoming.AddRange(batch);
        }

        // =========================
        // MERGE
        // =========================
        int added = 0;
        int updated = 0;

        foreach (QuestData quest in incoming)
        {
            if (!map.TryGetValue(quest.ID, out QuestData? old))
            {
                existingData.Add(quest);
                map[quest.ID] = quest;
                added++;
                continue;
            }

            if (!QuestChanged(old, quest))
                continue;

            int index = existingData.FindIndex(x => x.ID == quest.ID);

            if (index >= 0)
                existingData[index] = quest;

            map[quest.ID] = quest;
            updated++;
        }

        SaveAndMirror(existingData, clientPath, scriptsPath);

        int lastQuestID = existingData.Count > 0 ? existingData.Max(x => x.ID) : 0;
        Core.Logger($"Done | Total: {existingData.Count} | Added: {added} | Updated: {updated} | EndFile QuestID: {lastQuestID}");
        Cleanup();
    }

    // =========================
    // 🔰 BOOTSTRAP FRESH (for full rebuild)
    // =========================
    private List<QuestData> BootstrapFresh(
        IQuestDataLoaderService loader,
        int targetMaxId,
        CancellationToken token)
    {
        int window = 500;
        int start = 1;

        List<QuestData> all = new();

        // Use a temporary path for the loader (file won't be created, just used internally)
        string tempPath = Path.Combine(ClientFileSources.SkuaDIR, "QuestData_temp.json");

        Core.Logger($"BootstrapFresh: fetching quests 1 → {targetMaxId}");

        while (start <= targetMaxId && !Bot.ShouldExit)
        {
            token.ThrowIfCancellationRequested();

            if (Bot.ShouldExit)
                break;

            int end = Math.Min(start + window - 1, targetMaxId);

            Core.Logger($"  Fetching: {start} → {end}");

            // Pass tempPath instead of null - the service needs a valid path parameter
            List<QuestData> batch =
                loader.UpdateRangeAsync(
                        tempPath,
                        start,
                        end,
                        null,
                        token
                    )
                    .GetAwaiter()
                    .GetResult();

            if (batch.Count == 0)
            {
                Core.Logger($"  No quests found in range {start}-{end}, stopping");
                break;
            }

            all.AddRange(batch);
            int highestID = all.Max(q => q.ID);
            Core.Logger($"  Fetched {batch.Count} quests (total: {all.Count} quests, highest ID: {highestID})");

            start += window;
        }

        // =========================
        // DEDUPLICATE BY QUEST ID
        // =========================
        var deduplicated = all
            .GroupBy(q => q.ID)
            .Select(g => g.Last()) // Keep the last (most recent) version of each quest ID
            .OrderBy(q => q.ID)
            .ToList();

        Core.Logger($"BootstrapFresh complete: {deduplicated.Count} unique quests (removed {all.Count - deduplicated.Count} duplicates)");

        return deduplicated;
    }

    // =========================
    // 💾 SAVE + MIRROR (ALWAYS)
    // =========================
    private void SaveAndMirror(List<QuestData> data, string questPath, string scriptsPath)
    {
        try
        {
            File.WriteAllText(
                questPath,
                JsonConvert.SerializeObject(data, Formatting.Indented)
            );

            Core.Logger($"✓ Saved {data.Count} quests to SkuaDIR");
        }
        catch (Exception ex)
        {
            Core.Logger($"✗ Failed to save SkuaDIR: {ex.Message}");
            throw;
        }

        try
        {
            File.Copy(questPath, scriptsPath, true);
            Core.Logger($"✓ Mirrored to ScriptsDIR");
        }
        catch (Exception ex)
        {
            Core.Logger($"✗ Mirror copy failed: {ex.Message}");
        }
    }

    // =========================
    // 🧹 CLEANUP
    // =========================
    private void Cleanup()
    {
        _loaderCTS?.Dispose();
        _loaderCTS = null;
    }

    // =========================
    // 🔧 CHANGE DETECTION
    // =========================
    private static bool QuestChanged(QuestData oldQuest, QuestData newQuest) =>
        JsonConvert.SerializeObject(oldQuest) != JsonConvert.SerializeObject(newQuest);

    private List<(int Start, int End)> BuildSegments(int startId, int endId, int window)
    {
        List<(int Start, int End)> segments = [];

        for (int i = startId; i <= endId && !Bot.ShouldExit; i += window)
        {
            int end = Math.Min(i + window - 1, endId);
            segments.Add((i, end));
        }

        return segments;
    }
}