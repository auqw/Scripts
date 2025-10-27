/*
name: Weekly Release Generator
description: Automates the process of generating quest progression checks and corresponding monster hunt commands based on specified quest ID ranges. It handles single and multiple requirements efficiently, and outputs the formatted script to a temporary file for easy integration.
tags: quest automation, monster hunting, script generation, game scripting, quest progression
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;
using Skua.Core.Options;
using System.Collections.Generic;
using System.IO;

public class WeeklyReleaseGenerator
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;

    public string OptionsStorage = "WeeklyReleaseGenerator";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<int>("QuestIDRangeStart", "QuestIDRangeStart", "First QuestID in the story"),
        new Option<int>("QuestIDRangeEnd", "QuestIDRangeEnd", "Last QuestID in the story"),
        new Option<string>("MapName", "MapName", "Default map name for the hunt"),
        CoreBots.Instance.SkipOptions
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions(disableClassSwap: true);
        Generator();
        Core.SetOptions(false);
    }

   public void Generator()
{
    if (Bot.Config == null)
    {
        Core.Logger("⚠️ Config is not set properly.");
        return;
    }

    int startID = Bot.Config.Get<int>("QuestIDRangeStart");
    int endID = Bot.Config.Get<int>("QuestIDRangeEnd");
    string mapName = Bot.Config.Get<string>("MapName") ?? string.Empty;

    if (string.IsNullOrEmpty(mapName))
    {
        Core.Logger("⚠️ MapName is not set properly.");
        return;
    }

    Core.Logger($"🔹 Joining map: {mapName.ToLower()}");
    Core.Join(mapName.ToLower());
    Bot.Wait.ForMapLoad(mapName.ToLower());
    Core.Logger("✅ Map loaded successfully.");

    List<string> generatedLines = new()
    {
        $"if (Core.isCompletedBefore({endID}))",
        "    return;",
        string.Empty,
        "Story.PreLoad(this);",
        string.Empty
    };

    // Collect monster names
    List<string> monsterNames = Bot.Monsters.MapMonsters
        .Select(m => m.Name)
        .Distinct()
        .ToList();

    // Generate UseableMonsters array
    generatedLines.Add("#region Useable Monsters");
    generatedLines.Add("string[] UseableMonsters = new[]");
    generatedLines.Add("{");
    generatedLines.AddRange(monsterNames.Select((name, index) =>
        $"\t\"{name}\", // UseableMonsters[{index}]{(index < monsterNames.Count - 1 ? "," : "")}"));
    generatedLines.Add("};");
    generatedLines.Add("#endregion Useable Monsters");

    var sortedQuests = Core.EnsureLoad(Core.FromTo(startID, endID))
                            .OrderBy(q => q.ID)
                            .ToList();

    string[] mapItemKeywords = { "click", "talk", "find", "read", "investigate" };

    foreach (Quest q in sortedQuests)
    {
        generatedLines.Add(string.Empty);
        generatedLines.Add($"// {q.ID} | {q.Name}");
        generatedLines.Add($"if (!Story.QuestProgression({q.ID}))");
        generatedLines.Add("{");

        Core.Logger($"🔹 Generating quest {q.ID}: {q.Name}");

        bool addedMapItemQuest = false;

        // Detect quest-level map item
        if ((q.Description?.IndexOfAny(mapItemKeywords.SelectMany(s => s).ToArray()) ?? -1) >= 0 ||
            (q.Name?.IndexOfAny(mapItemKeywords.SelectMany(s => s).ToArray()) ?? -1) >= 0)
        {
            generatedLines.Add($"    Story.MapItemQuest({q.ID}, \"{mapName}\", FillmeIn);");
            addedMapItemQuest = true;
            Core.Logger($"📜 MapItemQuest detected for quest {q.ID}");
        }

        // Process requirements
        if (q.Requirements.Count > 0)
        {
            // Add MapItemQuest first if needed
            foreach (var req in q.Requirements)
            {
                string reqLower = req.Name?.ToLower() ?? "";
                if (mapItemKeywords.Any(k => reqLower.Contains(k)))
                {
                    generatedLines.Add($"    Story.MapItemQuest({q.ID}, \"{mapName}\", FillmeIn);");
                    addedMapItemQuest = true;
                    Core.Logger($"📜 Requirement '{req.Name}' detected as MapItemQuest");
                }
            }

            // Add HuntMonsterQuest if there are any non-mapitem requirements
            if (q.Requirements.Any(r => !mapItemKeywords.Any(k => (r.Name?.ToLower() ?? "").Contains(k))))
            {
                generatedLines.Add($"    Core.HuntMonsterQuest({q.ID},");
                foreach (var req in q.Requirements.Select((r, i) => new { r, i }))
                {
                    string reqLower = req.r.Name?.ToLower() ?? "";
                    if (mapItemKeywords.Any(k => reqLower.Contains(k)))
                        continue;

                    // Always use UseableMonsters[0] for simplicity
                    generatedLines.Add($"        (\"{mapName}\", UseableMonsters[0], ClassType.Solo),");
                    Core.Logger($"🗡️ Requirement parsed: '{req.r.Name}' → UseableMonsters[0] → ClassType.Solo");
                }

                // Remove last comma and close
                generatedLines[^1] = generatedLines.Last().TrimEnd(',') + " );";
            }
        }

        // Fallback: if no requirements at all
        if (!addedMapItemQuest && q.Requirements.Count == 0)
        {
            generatedLines.Add($"    Core.ChainQuest({q.ID});");
            Core.Logger($"⚡ ChainQuest fallback for quest {q.ID}");
        }

        generatedLines.Add("}");
        generatedLines.Add(string.Empty);
    }

    // Write to temporary file
    string tempFilePath = Path.GetTempFileName();
    File.WriteAllLines(tempFilePath, generatedLines);
    Core.Logger($"🔹 Generation complete! File saved to {tempFilePath}");
    System.Diagnostics.Process.Start("notepad.exe", tempFilePath);
}



}
