/*
name: GenerateQuestFilesv3
description: Lean quest data generator — no fluff, just fetch quests in batches and save.
tags: debug, quest, data, generation, v3
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

public class QuestFileUpdaterV3
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private IQuestDataLoaderService? service;

    public string OptionsStorage = "QuestFileUpdaterV3";
    public bool DontPreconfigure = true;

    public List<IOption> Options =
    [
        new Option<int>("TargetQuestID", "Target Quest ID", "Stop syncing when this quest ID is reached.", 10799),
        new Option<string>("QuestRange", "Quest ID Range (start,end)", "Force-regenerate a specific range. Empty = continue from last ID + 1.", ""),
        new Option<int>("BatchSize", "Batch Size", "Quest IDs per request.", 30),
        new Option<string>("SkipRange", "Skip Quest Range (start,end)", "Skip this range entirely.", ""),
        CoreBots.Instance.SkipOptions,
    ];

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        try
        {
            string clientPath = Path.Combine(ClientFileSources.SkuaDIR, "QuestData.json");
            string scriptsPath = Path.Combine(ClientFileSources.SkuaScriptsDIR, "QuestData.json");

            service ??= Ioc.Default.GetRequiredService<IQuestDataLoaderService>();

            int targetMaxId = Bot.Config!.Get<int>("TargetQuestID");
            string range = Bot.Config!.Get<string>("QuestRange") ?? "";
            int batchSize = Bot.Config!.Get<int>("BatchSize");
            string skipRaw = Bot.Config!.Get<string>("SkipRange") ?? "";

            if (batchSize < 1) batchSize = 1;
            if (batchSize > 500) batchSize = 500;

            // Parse skip range
            int skipStart = 0, skipEnd = 0;
            if (!string.IsNullOrWhiteSpace(skipRaw))
            {
                var parts = skipRaw.Split(',');
                if (parts.Length >= 2 && int.TryParse(parts[0], out int ss) && int.TryParse(parts[1], out int se))
                {
                    skipStart = ss;
                    skipEnd = se;
                    Core.Logger($"Skipping quest range {skipStart} to {skipEnd}.");
                }
            }

            // Load existing data
            List<QuestData> existingData = File.Exists(clientPath)
                ? service.GetFromFileAsync(clientPath).GetAwaiter().GetResult()
                : new List<QuestData>();

            var map = new Dictionary<int, QuestData>(existingData.Count);
            foreach (var q in existingData)
                map[q.ID] = q;

            // Determine range to fetch
            int fetchStart, fetchEnd;

            if (!string.IsNullOrWhiteSpace(range))
            {
                var parts = range.Split(',');
                fetchStart = parts.Length > 0 && int.TryParse(parts[0], out int s) ? s : 1;
                fetchEnd = parts.Length > 1 && int.TryParse(parts[1], out int e) ? e : targetMaxId;
                Core.Logger($"Range mode: {fetchStart} to {fetchEnd}");

                // Remove existing entries in range
                existingData.RemoveAll(q => q.ID >= fetchStart && q.ID <= fetchEnd);
                foreach (int id in Enumerable.Range(fetchStart, fetchEnd - fetchStart + 1))
                    map.Remove(id);
            }
            else
            {
                fetchStart = existingData.Count > 0 ? existingData.Max(q => q.ID) + 1 : 1;
                fetchEnd = targetMaxId;
                Core.Logger($"Delta mode: {fetchStart} to {fetchEnd}");
            }

            if (fetchStart > fetchEnd)
            {
                Core.Logger("Nothing to fetch.");
                return;
            }

            // Fetch in batches
            int added = 0, updated = 0, emptyInARow = 0;
            var seenThisRun = new HashSet<int>();

            for (int s = fetchStart; s <= fetchEnd && !Bot.ShouldExit; s += batchSize)
            {
                if (Bot.ShouldExit) break;

                int e = Math.Min(s + batchSize - 1, fetchEnd);

                // Check skip range
                if (skipEnd > 0 && s >= skipStart && e <= skipEnd)
                {
                    Core.Logger($"Skipping quests {s} to {e} (in skip range).");
                    s = skipEnd + 1 - batchSize; // loop will add batchSize, landing at skipEnd + 1
                    if (s < skipStart) s = skipStart;
                    continue;
                }

                // Skip if all IDs in this batch are already in the map
                if (existingData.Count > 0)
                {
                    bool allKnown = true;
                    for (int id = s; id <= e; id++)
                    {
                        if (!map.ContainsKey(id))
                        {
                            allKnown = false;
                            break;
                        }
                    }
                    if (allKnown)
                    {
                        continue;
                    }
                }

                Core.Logger($"Fetching quests {s} to {e}...");
                var batch = service.UpdateRangeAsync(clientPath, s, e, null, CancellationToken.None).GetAwaiter().GetResult();

                if (batch == null || batch.Count == 0)
                {
                    emptyInARow++;
                    if (emptyInARow >= 50)
                    {
                        Core.Logger($"50 consecutive empty batches, stopping at quest {s}.");
                        break;
                    }
                    continue;
                }

                emptyInARow = 0;

                foreach (var quest in batch)
                {
                    if (!seenThisRun.Add(quest.ID))
                        continue;

                    if (!map.ContainsKey(quest.ID))
                    {
                        existingData.Add(quest);
                        map[quest.ID] = quest;
                        added++;
                    }
                    else if (QuestChanged(map[quest.ID], quest))
                    {
                        int idx = existingData.FindIndex(x => x.ID == quest.ID);
                        if (idx >= 0) existingData[idx] = quest;
                        map[quest.ID] = quest;
                        updated++;
                    }
                }

                Core.Logger($"Done with quests {s} to {e} ({batch.Count} returned, {added + updated} total new/changed so far)");

                // Save every 20 batches so progress isn't lost
                if (added + updated > 0 && (added + updated) % 20 == 0)
                    SaveFiles(existingData, clientPath, scriptsPath);
            }

            SaveFiles(existingData, clientPath, scriptsPath);
            Core.Logger($"Done. Total: {existingData.Count} | Added: {added} | Updated: {updated}");
        }
        catch (System.Exception ex)
        {
            Core.Logger("Error: " + ex.Message);
        }
        finally
        {
            Core.SetOptions(false);
        }
    }

    private void SaveFiles(List<QuestData> data, string clientPath, string scriptsPath)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(clientPath, json);
        try { File.Copy(clientPath, scriptsPath, true); } catch { }
        Core.Logger($"Saved {data.Count} quests.");
    }

    private static bool QuestChanged(QuestData a, QuestData b) =>
        JsonConvert.SerializeObject(a) != JsonConvert.SerializeObject(b);
}
