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

            // Ensure target directories exist
            Directory.CreateDirectory(Path.GetDirectoryName(clientPath)!);
            Directory.CreateDirectory(Path.GetDirectoryName(scriptsPath)!);

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
                fetchStart = parts.Length > 0 && int.TryParse(parts[0], out int ps) ? ps : 1;
                fetchEnd = parts.Length > 1 && int.TryParse(parts[1], out int pe) ? pe : targetMaxId;
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
            int batchCount = 0;

            int s = fetchStart;
            while (s <= fetchEnd && !Bot.ShouldExit)
            {
                if (Bot.ShouldExit) break;

                int e = Math.Min(s + batchSize - 1, fetchEnd);

                // Check skip range — if any ID in [s, e] falls within [skipStart, skipEnd]
                if (skipEnd > 0 && e >= skipStart && s <= skipEnd)
                {
                    // Process the portion before the skip range first
                    if (s < skipStart)
                    {
                        int preEnd = skipStart - 1;
                        Core.Logger($"Fetching quests {s} to {preEnd} (before skip range)...");
                        FetchProbe(service, clientPath, s, preEnd, existingData, map, seenThisRun, ref added, ref updated);
                    }

                    Core.Logger($"Skipping quests {skipStart} to {skipEnd} (in skip range).");
                    s = skipEnd + 1;
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
                        s += batchSize;
                        continue;
                    }
                }

                Core.Logger($"Fetching quests {s} to {e}...");
                int foundInBatch = FetchProbe(service, clientPath, s, e, existingData, map, seenThisRun, ref added, ref updated);
                if (foundInBatch == 0)
                {
                    emptyInARow++;
                    if (emptyInARow >= 50)
                    {
                        Core.Logger($"50 consecutive empty batches, stopping at quest {s}.");
                        break;
                    }
                    s += batchSize;
                    continue;
                }

                emptyInARow = 0;

                batchCount++;
                Core.Logger($"Done with quests {s} to {e} ({foundInBatch} returned, {added + updated} total new/changed so far)");

                // Auto-save every 10 batches so progress isn't lost
                if (batchCount % 10 == 0)
                    SaveFiles(existingData, clientPath, scriptsPath);

                s += batchSize;
            }

            SaveFiles(existingData, clientPath, scriptsPath);
            Core.Logger($"Done. Total: {existingData.Count} | Added: {added} | Updated: {updated}");
        }
        catch (System.Exception ex)
        {
            Core.Logger("Error: " + ex.ToString());
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
        try
        {
            File.Copy(clientPath, scriptsPath, true);
        }
        catch (System.Exception ex)
        {
            Core.Logger($"Warning: Failed to copy quest data to scripts path: {ex.Message}");
        }
        Core.Logger($"Saved {data.Count} quests.");
    }

    private static bool QuestChanged(QuestData a, QuestData b)
    {
        if (a.ID != b.ID) return true;
        if (a.Slot != b.Slot) return true;
        if (a.Value != b.Value) return true;
        if (a.Name != b.Name) return true;
        if (a.Once != b.Once) return true;
        if (a.Field != b.Field) return true;
        if (a.Index != b.Index) return true;
        if (a.Upgrade != b.Upgrade) return true;
        if (a.Level != b.Level) return true;
        if (a.RequiredClassID != b.RequiredClassID) return true;
        if (a.RequiredClassPoints != b.RequiredClassPoints) return true;
        if (a.RequiredFactionId != b.RequiredFactionId) return true;
        if (a.RequiredFactionRep != b.RequiredFactionRep) return true;
        if (a.Gold != b.Gold) return true;
        if (a.XP != b.XP) return true;

        if ((a.AcceptRequirements == null) != (b.AcceptRequirements == null)) return true;
        if ((a.Requirements == null) != (b.Requirements == null)) return true;
        if ((a.Rewards == null) != (b.Rewards == null)) return true;
        if ((a.SimpleRewards == null) != (b.SimpleRewards == null)) return true;

        if (!ItemBaseListsEqual(a.AcceptRequirements, b.AcceptRequirements)) return true;
        if (!ItemBaseListsEqual(a.Requirements, b.Requirements)) return true;
        if (!ItemBaseListsEqual(a.Rewards, b.Rewards)) return true;
        if (!SimpleRewardListsEqual(a.SimpleRewards, b.SimpleRewards)) return true;

        return false;
    }

    private static bool ItemBaseListsEqual(List<ItemBase>? a, List<ItemBase>? b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        return JsonConvert.SerializeObject(a) == JsonConvert.SerializeObject(b);
    }

    private static bool SimpleRewardListsEqual(List<SimpleReward>? a, List<SimpleReward>? b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        return JsonConvert.SerializeObject(a) == JsonConvert.SerializeObject(b);
    }

    private void ProcessBatch(
        List<QuestData> batch,
        List<QuestData> existingData,
        Dictionary<int, QuestData> map,
        HashSet<int> seenThisRun,
        ref int added,
        ref int updated)
    {
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
    }

    /// <summary>
    /// Tries to fetch a range of quests. If the batch returns empty (possible undefined IDs poisoning the request),
    /// falls back to probing each ID individually.
    /// Returns the number of quests found.
    /// </summary>
    private int FetchProbe(
        IQuestDataLoaderService loader,
        string filePath,
        int start, int end,
        List<QuestData> existingData,
        Dictionary<int, QuestData> map,
        HashSet<int> seenThisRun,
        ref int added,
        ref int updated)
    {
        var batch = loader.UpdateRangeAsync(filePath, start, end, null, CancellationToken.None).GetAwaiter().GetResult();
        if (batch != null && batch.Count > 0)
        {
            ProcessBatch(batch, existingData, map, seenThisRun, ref added, ref updated);
            return batch.Count;
        }

        // Full batch returned empty — probe individual IDs
        int found = 0;
        for (int probe = start; probe <= end; probe++)
        {
            if (Bot.ShouldExit) break;
            var single = loader.UpdateRangeAsync(filePath, probe, probe, null, CancellationToken.None).GetAwaiter().GetResult();
            if (single != null && single.Count > 0)
            {
                ProcessBatch(single, existingData, map, seenThisRun, ref added, ref updated);
                found += single.Count;
            }
        }
        if (found > 0)
            Core.Logger($"Found {found} quest(s) in range {start}-{end} via individual probe (some IDs undefined).");
        return found;
    }
}
