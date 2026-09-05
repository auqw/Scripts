/*
name: Weekly Release Generator v2
description: Reads every quest chain from a location SWF and matches requirements to live monsterDrops packets.
tags: developer, generator, story, quests, drops, packets, v2
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Tools/ForDevelopers/GeneratorHelpers/DropPacketCollector.cs
//cs_include Scripts/Tools/ForDevelopers/GeneratorHelpers/LocationSwfQuestReader.cs
//cs_include Scripts/Tools/ForDevelopers/GeneratorHelpers/QuestPacketCollector.cs
//cs_include Scripts/Tools/ForDevelopers/GeneratorHelpers/GenerateSupportUtils.cs

using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class WeeklyReleaseGeneratorV2
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static GeneratorSupportUtils GSU => new();


    public string OptionsStorage = "WeeklyReleaseGeneratorV2";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<string>("MapName", "Map", "Map to join and generate from.", ""),
    };

    private sealed record Dependency(string IncludePath, string ClassName, string MethodName);
    private sealed record MapItemSource(int ID, string Map, bool CurrentMapVerified);
    private static readonly string[] MapItemKeywords = ["click", "talk", "find", "read", "investigate"];

    public void ScriptMain(IScriptInterface bot)
    {
        Bot.Config?.Configure();
        Core.SetOptions(disableClassSwap: true);
        try
        {
            Generate();
        }
        catch (Exception ex)
        {
            Core.Logger($"Generation failed: {ex.Message}", messageBox: true);
        }
        finally
        {
            Core.SetOptions(false);
        }
    }

    private void Generate()
    {
        string map = (Bot.Config?.Get<string>("MapName") ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(map))
            throw new InvalidOperationException("Enter the release map name.");

        Core.Join(map);
        if (!Bot.Wait.ForMapLoad(map))
            throw new InvalidOperationException($"Failed to join /{map}.");

        LocationSwfQuestReader.LocationQuestData location = new LocationSwfQuestReader().ReadCurrentMap();
        IReadOnlyList<int> questIDs = location.QuestIDs;
        if (questIDs.Count == 0)
            throw new InvalidOperationException($"No placed quest references were found in {Bot.Map.FileName}.");

        List<Quest> allQuests = LoadLocalQuestCatalog();
        List<Quest> mapQuests = LoadMapQuests(questIDs);
        if (mapQuests.Count == 0)
            throw new InvalidOperationException("The map referenced quests, but none could be loaded.");

        // Live-loaded release quests may not exist in QuestData.json yet. Merge
        // them into the working catalog so the rest of generation sees their
        // slots, values, requirements, and rewards exactly like cached quests.
        HashSet<int> loadedMapQuestIDs = mapQuests.Select(quest => quest.ID).ToHashSet();
        allQuests.RemoveAll(quest => loadedMapQuestIDs.Contains(quest.ID));
        allQuests.AddRange(mapQuests);

        IReadOnlyList<DropPacketCollector.MonsterDrops> monsters = new DropPacketCollector().Collect();
        IReadOnlyDictionary<int, IReadOnlyList<MapItemSource>> existingMapItems = FindExistingMapItems(mapQuests);
        List<Quest> repeatableQuests = mapQuests
            .DistinctBy(quest => quest.ID)
            .Where(quest => quest.Slot == -1 && !quest.Once)
            .OrderBy(quest => quest.Value)
            .ThenBy(quest => quest.ID)
            .ToList();
        List<List<Quest>> chains = mapQuests
            // Quest references from every placed NPC are combined before any
            // chain is formed. NPC boundaries never split a progression slot.
            .DistinctBy(quest => quest.ID)
            // Slot -1 quests are freely invokable side/repeatable quests, not
            // gated story progressions, so they do not get story files.
            .Where(quest => quest.Slot != -1)
            // Every remaining identical slot is exactly one output chain.
            .GroupBy(quest => quest.Slot)
            .Select(RemovePostFinalFarmQuests)
            .Where(chain => chain.Count > 0)
            // Story file numbering follows progression metadata, never the
            // arbitrary order of quest IDs found in the SWF or quest cache.
            .OrderBy(chain => chain[0].Slot)
            .ThenBy(chain => chain[0].Value)
            .ThenBy(chain => chain[0].ID)
            .ToList();
        if (chains.Count == 0 && repeatableQuests.Count == 0)
            throw new InvalidOperationException("No quest chains were found in the map.");

        string outputDirectory = Path.Combine(ClientFileSources.SkuaScriptsDIR, "WIP");
        Directory.CreateDirectory(outputDirectory);
        string mapIdentifier = MapIdentifier(map);
        string stalePattern = $@"^{Regex.Escape(mapIdentifier)}(\d+)?(Repeatables)?\.cs$";
        foreach (string stale in Directory.GetFiles(outputDirectory, $"{mapIdentifier}*.cs")
            .Where(file => Regex.IsMatch(Path.GetFileName(file), stalePattern, RegexOptions.IgnoreCase)))
            File.Delete(stale);
        List<string> outputs = new();
        HashSet<int> currentMapQuestIDs = mapQuests.Select(quest => quest.ID).ToHashSet();
        for (int index = 0; index < chains.Count; index++)
        {
            List<Quest> chain = chains[index];
            Quest? predecessor = allQuests
                .Where(quest => quest.Slot == chain[0].Slot && quest.Value < chain[0].Value)
                .OrderByDescending(quest => quest.Value)
                .ThenByDescending(quest => quest.Once)
                .ThenByDescending(quest => quest.ID)
                .FirstOrDefault();
            Dependency? dependency = predecessor == null
                ? null
                : FindDependency(predecessor, currentMapQuestIDs, allQuests);
            string className = chains.Count > 1 ? $"{mapIdentifier}{index + 1}" : mapIdentifier;
            string path = Path.Combine(outputDirectory, className + ".cs");
            Core.WriteFile(path, BuildStory(
                className,
                map,
                chain,
                monsters,
                location.MapObjectsByQuest,
                existingMapItems,
                predecessor,
                dependency,
                part: chains.Count > 1 ? index + 1 : null
            ));
            outputs.Add(path);
        }

        if (repeatableQuests.Count > 0)
        {
            string repeatablesClassName = chains.Count > 0 ? $"{mapIdentifier}Repeatables" : mapIdentifier;
            string repeatablesPath = Path.Combine(outputDirectory, repeatablesClassName + ".cs");
            Core.WriteFile(repeatablesPath, BuildStory(
                repeatablesClassName,
                map,
                repeatableQuests,
                monsters,
                location.MapObjectsByQuest,
                existingMapItems,
                null,
                null,
                repeatables: true
            ));
            outputs.Add(repeatablesPath);
        }

        Core.Logger($"Generated {outputs.Count} quest file(s):\n{string.Join("\n", outputs)}");
        Process.Start(new ProcessStartInfo("explorer.exe", outputDirectory) { UseShellExecute = true });
    }

    private List<Quest> LoadLocalQuestCatalog()
    {
        if (!File.Exists(ClientFileSources.SkuaQuestsFile))
        {
            Core.Logger("QuestData.json was not found; map quests will be loaded from the game one at a time.");
            return new();
        }

        try
        {
            return (JsonConvert.DeserializeObject<List<QuestData>>(
                File.ReadAllText(ClientFileSources.SkuaQuestsFile)
            ) ?? new()).Select(QuestPacketCollector.FromData).ToList();
        }
        catch (Exception ex)
        {
            Core.Logger($"QuestData.json could not be read ({ex.Message}); map quests will be loaded from the game one at a time.");
            return new();
        }
    }

    private List<Quest> LoadMapQuests(IReadOnlyList<int> questIDs)
    {
        List<Quest> quests = new();
        List<int> failed = new();

        foreach (int questID in questIDs.Where(id => id > 0).Distinct())
        {
            Core.Logger($"Capturing quest {questID} from its getQuests packet.");
            try
            {
                quests.Add(new QuestPacketCollector().Load(questID));
            }
            catch (Exception ex)
            {
                Core.Logger($"Quest {questID} packet load failed: {ex.Message}");
                failed.Add(questID);
            }
        }

        if (failed.Count > 0)
            throw new InvalidOperationException(
                $"Could not capture map quest packet(s): {string.Join(", ", failed)}."
            );

        return quests;
    }

    private IReadOnlyDictionary<int, IReadOnlyList<MapItemSource>> FindExistingMapItems(
        IReadOnlyList<Quest> quests
    )
    {
        Dictionary<int, IReadOnlyList<MapItemSource>> result = new();
        string scripts = ClientFileSources.SkuaScriptsDIR;
        List<string> sources = [.. Directory.GetFiles(scripts, "*.cs", SearchOption.AllDirectories)
            .Where(file =>
            {
                string relative = Path.GetRelativePath(scripts, file).Replace('\\', '/');
                return !relative.StartsWith("WIP/", StringComparison.OrdinalIgnoreCase)
                    && !relative.StartsWith("Tools/", StringComparison.OrdinalIgnoreCase);
            })
            .Select(File.ReadAllText)];

        foreach (Quest quest in quests)
        {
            List<MapItemSource> found = new();
            foreach (string source in sources)
            {
                foreach (Match questReference in Regex.Matches(source, $@"\b{quest.ID}\b"))
                {
                    int end = Math.Min(source.Length, questReference.Index + 2500);
                    foreach (Match nextQuest in Regex.Matches(
                        source[(questReference.Index + questReference.Length)..end],
                        @"\b(?:Test|EnsureAccept|HuntMonsterQuest|MapItemQuest|ChainQuest)\s*\(\s*(?<id>\d+)"
                    ))
                    {
                        if (int.TryParse(nextQuest.Groups["id"].Value, out int nextID)
                            && nextID != quest.ID)
                        {
                            end = questReference.Index + questReference.Length + nextQuest.Index;
                            break;
                        }
                    }

                    string region = source[questReference.Index..end];
                    foreach (Match call in Regex.Matches(
                        region,
                        $@"\bMapItemQuest\s*\(\s*{quest.ID}\s*,\s*""(?<map>[^""]+)""\s*,\s*(?:(?:new\s*\[\s*\]\s*)?\{{(?<ids>[\d,\s]+)\}}|(?<id>\d+))"
                    ))
                    {
                        IEnumerable<int> ids = call.Groups["ids"].Success
                            ? call.Groups["ids"].Value.Split(',')
                                .Select(value => int.TryParse(value.Trim(), out int id) ? id : 0)
                            : new[] { int.TryParse(call.Groups["id"].Value, out int id) ? id : 0 };
                        foreach (int mapItemID in ids.Where(id => id > 0))
                        {
                            if (found.Any(item => item.ID == mapItemID))
                                continue;
                            found.Add(new MapItemSource(
                                mapItemID,
                                call.Groups["map"].Value,
                                CurrentMapVerified: false
                            ));
                        }
                    }
                    foreach (Match call in Regex.Matches(region, @"\bGetMapItem\s*\(\s*(?<id>\d+)(?<args>[^)]*)\)"))
                    {
                        if (!int.TryParse(call.Groups["id"].Value, out int mapItemID)
                            || mapItemID <= 0
                            || found.Any(item => item.ID == mapItemID))
                            continue;
                        MatchCollection strings = Regex.Matches(call.Groups["args"].Value, "\"(?<value>[^\"]+)\"");
                        string knownMap = strings.Count == 0
                            ? string.Empty
                            : strings[^1].Groups["value"].Value;
                        // Existing scripts are only an ID source. They do not
                        // prove that the object belongs to the currently joined
                        // SWF, so their location must remain explicitly fillable.
                        found.Add(new MapItemSource(mapItemID, knownMap, CurrentMapVerified: false));
                    }
                }
            }
            if (found.Count > 0)
                result[quest.ID] = found;
        }

        return result;
    }

    private string[] BuildStory(
        string className,
        string map,
        List<Quest> quests,
        IReadOnlyList<DropPacketCollector.MonsterDrops> monsters,
        IReadOnlyDictionary<int, IReadOnlyList<int>> mapObjectsByQuest,
        IReadOnlyDictionary<int, IReadOnlyList<MapItemSource>> existingMapItemsByQuest,
        Quest? predecessor,
        Dependency? dependency,
        bool repeatables = false,
        int? part = null
    )
    {
        Quest gate = quests
            .Where(quest => quest.Once)
            .OrderByDescending(quest => quest.Value)
            .ThenByDescending(quest => quest.ID)
            .FirstOrDefault()
            ?? quests.OrderByDescending(quest => quest.Value).ThenByDescending(quest => quest.ID).First();
        List<DropPacketCollector.MonsterDrops> usedMonsters = quests
            .SelectMany(quest => (quest.Requirements ?? new List<ItemBase>())
                .Select(requirement => FindDropMonster(requirement, quest.Name, monsters)))
            .Where(monster => monster != null)
            .Select(monster => monster!)
            .DistinctBy(monster => monster.MonsterID)
            .OrderBy(monster => monster.MonsterName)
            .ThenBy(monster => monster.MonsterID)
            .ToList();
        List<DropPacketCollector.MonsterDrops> namedMonsters = [.. usedMonsters
            .Where(monster => !IsMonsterNameAmbiguous(monster, usedMonsters))
            .DistinctBy(monster => monster.MonsterName, StringComparer.OrdinalIgnoreCase)];
        Dictionary<int, int> monsterIndexes = usedMonsters
            .Where(monster => !IsMonsterNameAmbiguous(monster, usedMonsters))
            .ToDictionary(
                monster => monster.MonsterID,
                monster => namedMonsters.FindIndex(named => named.MonsterName.Equals(
                    monster.MonsterName,
                    StringComparison.OrdinalIgnoreCase
                ))
            );
        string displayMap = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(map);
        string label = repeatables
            ? $"{displayMap} Repeatable Quests"
            : part.HasValue
                ? $"{displayMap} Part {part}"
                : displayMap;
        List<string> lines = new()
        {
            "/*",
            $"name: {label}",
            repeatables
                ? $"description: Completes repeatable non-slotted quests found in /{map}."
                : $"description: Completes the quest chain in /{map}.",
            $"tags: {map}, story, quests, {className.ToLowerInvariant()}",
            "*/",
            "//cs_include Scripts/CoreBots.cs",
            "//cs_include Scripts/CoreStory.cs",
        };
        if (dependency != null)
            lines.Add($"//cs_include Scripts/{dependency.IncludePath}");
        lines.AddRange(new[]
        {
            "using Skua.Core.Interfaces;",
            "",
            $"public class {className}",
            "{",
            "    private IScriptInterface Bot => IScriptInterface.Instance;",
            "    private CoreBots Core => CoreBots.Instance;",
            "    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }",
            "    private static CoreStory _Story;",
        });
        if (dependency != null)
        {
            lines.Add($"    private static {dependency.ClassName} Prerequisite {{ get => _Prerequisite ??= new {dependency.ClassName}(); set => _Prerequisite = value; }}");
            lines.Add($"    private static {dependency.ClassName} _Prerequisite;");
        }
        lines.AddRange(new[]
        {
            "",
            "    public void ScriptMain(IScriptInterface Bot)",
            "    {",
            "        Core.SetOptions();",
            "        Storyline();",
            "        Core.SetOptions(false);",
            "    }",
            "",
            "    public void Storyline()",
            "    {",
        });
        if (!repeatables)
        {
            lines.Add($"        if (Core.isCompletedBefore({gate.ID}))");
            lines.Add("            return;");
            lines.Add("");
        }
        if (dependency != null)
        {
            lines.Add($"        Prerequisite.{dependency.MethodName}();");
            lines.Add("");
        }
        else if (predecessor != null)
        {
            lines.Add($"        if (!Core.isCompletedBefore({predecessor.ID}))");
            lines.Add("        {");
            lines.Add($"            Core.Logger(\"Complete prerequisite quest {Escape(predecessor.Name)} [{predecessor.ID}] first.\", messageBox: true, stopBot: true);");
            lines.Add("            return;");
            lines.Add("        }");
            lines.Add("");
        }
        lines.Add("        Story.PreLoad(this);");
        if (!repeatables && namedMonsters.Count > 0)
        {
            lines.Add("");
            lines.Add("        string[] UseableMonsters =");
            lines.Add("        [");
            lines.AddRange(namedMonsters.Select((monster, index) =>
                $"            \"{Escape(monster.MonsterName)}\", // UseableMonsters[{index}]"
            ));
            lines.Add("        ];");
        }

        // Sort again at the point of emission so execution cannot accidentally
        // inherit SWF, JSON, dictionary, or quest-ID ordering.
        foreach (Quest quest in quests
            .OrderBy(quest => quest.Value)
            .ThenByDescending(quest => quest.Once)
            .ThenBy(quest => quest.ID))
            AddQuest(
                lines,
                quest,
                map,
                monsters,
                mapObjectsByQuest,
                existingMapItemsByQuest,
                monsterIndexes,
                usedMonsters,
                inlineMonsterNames: repeatables
            );

        lines.AddRange(new[] { "    }", "}" });
        return [.. lines];
    }

    private static List<Quest> RemovePostFinalFarmQuests(IGrouping<int, Quest> group)
    {
        List<Quest> quests = group
            .OrderBy(quest => quest.Value)
            .ThenByDescending(quest => quest.Once)
            .ThenBy(quest => quest.ID)
            .ToList();
        Quest? finale = quests
            .Where(quest => quest.Once)
            .OrderByDescending(quest => quest.Value)
            .ThenByDescending(quest => quest.ID)
            .FirstOrDefault();
        if (finale == null)
            return quests;

        return quests
            .Where(quest => quest.Once || quest.Value < finale.Value)
            .ToList();
    }

    private static void AddQuest(
        List<string> lines,
        Quest quest,
        string map,
        IReadOnlyList<DropPacketCollector.MonsterDrops> monsters,
        IReadOnlyDictionary<int, IReadOnlyList<int>> mapObjectsByQuest,
        IReadOnlyDictionary<int, IReadOnlyList<MapItemSource>> existingMapItemsByQuest,
        Dictionary<int, int> monsterIndexes,
        IReadOnlyList<DropPacketCollector.MonsterDrops> usedMonsters,
        bool inlineMonsterNames
    )
    {
        List<ItemBase> requirements = quest.Requirements ?? new();
        List<ItemBase> acceptRequirements = quest.AcceptRequirements ?? new();
        DropPacketCollector.MonsterDrops?[] localDrops = requirements
            .Select(requirement => FindDropMonster(requirement, quest.Name, monsters))
            .ToArray();
        IReadOnlyList<int> mapObjects = mapObjectsByQuest.TryGetValue(quest.ID, out IReadOnlyList<int>? objects)
            ? objects
            : [];
        List<MapItemSource> currentMapItems = [.. mapObjects.Select(id => new MapItemSource(id, map, CurrentMapVerified: true))];
        List<MapItemSource> knownMapItems = [.. currentMapItems];
        if (existingMapItemsByQuest.TryGetValue(quest.ID, out IReadOnlyList<MapItemSource>? existing) && existing != null)
            knownMapItems.AddRange(existing.Where(item => knownMapItems.All(known => known.ID != item.ID)));
        List<int> nonlocalIndexes = [.. Enumerable.Range(0, requirements.Count).Where(index => localDrops[index] == null)];
        List<int> likelyMapItemIndexes = nonlocalIndexes
            .Where(index => IsMapItem(requirements[index]))
            .ToList();
        List<int> mappedIndexes = MatchMapItemIndexes(currentMapItems.Count, likelyMapItemIndexes, nonlocalIndexes);
        List<MapItemSource> selectedMapItems = currentMapItems;
        if (mappedIndexes.Count == 0)
        {
            mappedIndexes = MatchMapItemIndexes(knownMapItems.Count, likelyMapItemIndexes, nonlocalIndexes);
            selectedMapItems = knownMapItems;
        }

        lines.Add("");
        lines.Add($"        // {quest.ID} | {EscapeComment(quest.Name)}");
        // ChainQuest already checks and unbanks both normal and accept
        // requirements. Other quest helpers do not handle accept requirements
        // consistently, so only those paths need this explicit guard.
        if (requirements.Count > 0 && acceptRequirements.Count > 0)
        {
            lines.Add("        if (");
            for (int index = 0; index < acceptRequirements.Count; index++)
            {
                ItemBase requirement = acceptRequirements[index];
                int quantity = Math.Max(1, requirement.Quantity);
                string check = $"!Core.CheckInventory({requirement.ID}, {quantity})";
                string delimiter = index == acceptRequirements.Count - 1 ? string.Empty : " ||";
                lines.Add($"            {check}{delimiter} // {EscapeComment(requirement.Name)}");
            }
            lines.Add("        )");
            lines.Add("        {");
            lines.Add($"            Core.Logger(\"Missing accept requirements for {Escape(quest.Name)} [{quest.ID}].\", messageBox: true, stopBot: true);");
            lines.Add("            return;");
            lines.Add("        }");
        }
        if (requirements.Count == 0)
        {
            lines.Add($"        Story.ChainQuest({quest.ID});");
        }
        else
        {
            bool accepted = false;
            bool currentMapHelperCompletesQuest = false;
            if (mappedIndexes.Count > 0)
            {
                List<(MapItemSource Source, ItemBase Requirement)> acquisitions = mappedIndexes
                    .Select((requirementIndex, index) => (
                        selectedMapItems[index],
                        requirements[requirementIndex]
                    ))
                    .ToList();
                List<(MapItemSource Source, ItemBase Requirement)> currentMapAcquisitions = acquisitions
                    .Where(acquisition => acquisition.Source.CurrentMapVerified)
                    .ToList();
                if (currentMapAcquisitions.Count > 0)
                {
                    int[] quantities = currentMapAcquisitions
                        .Select(acquisition => Math.Max(1, acquisition.Requirement.Quantity))
                        .ToArray();
                    if (quantities.Distinct().Count() == 1)
                    {
                        string ids = currentMapAcquisitions.Count == 1
                            ? currentMapAcquisitions[0].Source.ID.ToString(CultureInfo.InvariantCulture)
                            : $"[{string.Join(", ", currentMapAcquisitions.Select(acquisition => acquisition.Source.ID))}]";
                        string amount = quantities[0] == 1 ? string.Empty : $", {quantities[0]}";
                        lines.Add($"        Story.MapItemQuest({quest.ID}, \"{map}\", {ids}{amount});");
                    }
                    else
                    {
                        string items = string.Join(", ", currentMapAcquisitions.Select(acquisition =>
                            $"({acquisition.Source.ID}, {Math.Max(1, acquisition.Requirement.Quantity)}, \"{map}\")"));
                        lines.Add($"        Story.MapItemQuest({quest.ID}, [{items}]);");
                    }
                    accepted = true;
                }
                List<(MapItemSource Source, ItemBase Requirement)> offMapAcquisitions = acquisitions
                    .Where(acquisition => !acquisition.Source.CurrentMapVerified)
                    .ToList();
                if (offMapAcquisitions.Count > 0 && !accepted)
                {
                    lines.Add($"        Core.EnsureAccept({quest.ID});");
                    accepted = true;
                }
                foreach (var acquisition in offMapAcquisitions)
                {
                    lines.Add($"        Core.GetMapItem({acquisition.Source.ID}, {Math.Max(1, acquisition.Requirement.Quantity)}, \"FILL_LOCATION\"); // {EscapeComment(acquisition.Requirement.Name)}");
                }
                currentMapHelperCompletesQuest = currentMapAcquisitions.Count > 0
                    && offMapAcquisitions.Count == 0
                    && mappedIndexes.Count == requirements.Count;
            }

            HashSet<int> unresolvedIndexes = [.. nonlocalIndexes.Except(mappedIndexes)];
            List<int> unresolvedMapItemIndexes = [.. unresolvedIndexes.Where(likelyMapItemIndexes.Contains)];
            if (unresolvedMapItemIndexes.Count > 0)
            {
                string missing = string.Join(", ", unresolvedMapItemIndexes.Select(index =>
                    $"{requirements[index].Name} [{requirements[index].ID}]"));
                throw new InvalidOperationException(
                    $"Map-item object ID was not found for quest {quest.Name} [{quest.ID}]: {missing}."
                );
            }

            List<int> huntIndexes = [.. Enumerable.Range(0, requirements.Count)
                .Where(index => !mappedIndexes.Contains(index) && !unresolvedMapItemIndexes.Contains(index))
                .OrderBy(index => localDrops[index] == null
                    ? 2
                    : ClassType(localDrops[index]!) == "Solo" ? 0 : 1)];
            if (huntIndexes.Count == 0)
            {
                if (!currentMapHelperCompletesQuest)
                    lines.Add($"        Core.EnsureComplete({quest.ID});");
                return;
            }

            bool hasAmbiguousTarget = huntIndexes.Any(index => localDrops[index] != null
                && IsMonsterNameAmbiguous(localDrops[index]!, usedMonsters));
            // KillQuest can't take a per-item map, so unresolved monsters
            // (local == null) still need the old explicit-hunt path.
            bool needsExplicitHunt = hasAmbiguousTarget
                || huntIndexes.Any(index => localDrops[index] == null);
            if (needsExplicitHunt)
            {
                if (!accepted)
                {
                    lines.Add($"        Core.EnsureAccept({quest.ID});");
                    accepted = true;
                }
                EmitExplicitHunts(lines, requirements, localDrops, huntIndexes, map, usedMonsters);
                lines.Add($"        Core.EnsureComplete({quest.ID});");
                return;
            }

            // Every remaining requirement is resolved here, so KillQuest's array
            // overload maps items to monsters purely by list position at runtime
            // (matching live QuestData.Requirements order after it filters out
            // already-owned items) — NOT the Solo/Farm sort order huntIndexes uses
            // above. Re-sort back to original requirement order before emitting so
            // positions line up correctly.
            List<int> orderedIndexes = [.. huntIndexes.OrderBy(index => index)];
            List<string> monsterNames = orderedIndexes
                .Select(index =>
                {
                    DropPacketCollector.MonsterDrops local = localDrops[index]!;
                    return inlineMonsterNames
                        ? $"\"{Escape(local.MonsterName)}\""
                        : $"UseableMonsters[{monsterIndexes[local.MonsterID]}]";
                })
                .ToList();

            bool sameMonster = orderedIndexes
                .Select(index => localDrops[index]!.MonsterID)
                .Distinct()
                .Count() == 1;

            if (sameMonster)
            {
                string itemSummary = string.Join(", ", orderedIndexes.Select(index =>
                    $"{EscapeComment(requirements[index].Name)} x{Math.Max(1, requirements[index].Quantity)}"));
                lines.Add($"        Story.KillQuest({quest.ID}, \"{map}\", {monsterNames[0]}); // {itemSummary}");
            }
            else
            {
                lines.Add("        Story.KillQuest(");
                lines.Add($"            {quest.ID},");
                lines.Add($"            \"{map}\",");
                lines.Add("            new[]");
                lines.Add("            {");
                for (int position = 0; position < orderedIndexes.Count; position++)
                {
                    int index = orderedIndexes[position];
                    ItemBase requirement = requirements[index];
                    string delimiter = position == orderedIndexes.Count - 1 ? string.Empty : ",";
                    lines.Add($"                {monsterNames[position]}{delimiter} // {EscapeComment(requirement.Name)} x{Math.Max(1, requirement.Quantity)}");
                }
                lines.Add("            }");
                lines.Add("        );");
            }
        }
    }

    private static List<int> MatchMapItemIndexes(
        int mapItemCount,
        IReadOnlyList<int> likelyMapItemIndexes,
        IReadOnlyList<int> nonlocalIndexes
    ) => mapItemCount switch
    {
        > 0 when likelyMapItemIndexes.Count > 0 && mapItemCount >= likelyMapItemIndexes.Count
            => [.. likelyMapItemIndexes],
        > 0 when nonlocalIndexes.Count > 0 && mapItemCount >= nonlocalIndexes.Count
            => [.. nonlocalIndexes],
        _ => new List<int>(),
    };

    private static void EmitExplicitHunts(
        List<string> lines,
        IReadOnlyList<ItemBase> requirements,
        IReadOnlyList<DropPacketCollector.MonsterDrops?> localDrops,
        IReadOnlyList<int> huntIndexes,
        string map,
        IReadOnlyList<DropPacketCollector.MonsterDrops> usedMonsters
    )
    {
        foreach (string classType in new[] { "Solo", "Farm" })
        {
            List<int> group = [.. huntIndexes.Where(index => localDrops[index] != null && ClassType(localDrops[index]!) == classType)];
            if (group.Count == 0)
                continue;
            lines.Add($"        Core.EquipClass(ClassType.{classType});");
            foreach (int index in group)
            {
                ItemBase requirement = requirements[index];
                DropPacketCollector.MonsterDrops local = localDrops[index]!;
                string quantity = Math.Max(1, requirement.Quantity).ToString(CultureInfo.InvariantCulture);
                string permanence = requirement.Temp ? string.Empty : ", isTemp: false";
                if (IsMonsterNameAmbiguous(local, usedMonsters))
                    lines.Add($"        Core.HuntMonsterMapID(\"{map}\", Bot.Monsters.MapMonsters.First(monster => monster.ID == {local.MonsterID}).MapID, \"{Escape(requirement.Name)}\", {quantity}{permanence});");
                else
                    lines.Add($"        Core.HuntMonster(\"{map}\", \"{Escape(local.MonsterName)}\", \"{Escape(requirement.Name)}\", {quantity}{permanence});");
            }
        }

        foreach (int index in huntIndexes.Where(index => localDrops[index] == null))
        {
            ItemBase requirement = requirements[index];
            string quantity = Math.Max(1, requirement.Quantity).ToString(CultureInfo.InvariantCulture);
            string permanence = requirement.Temp ? string.Empty : ", isTemp: false";
            lines.Add($"        Core.HuntMonster(\"FILL_LOCATION\", \"FILL_MONSTER\", \"{Escape(requirement.Name)}\", {quantity}{permanence});");
        }
    }

    private static bool IsMapItem(ItemBase requirement) => MapItemKeywords.Any(keyword =>
        (requirement.Name ?? string.Empty).Contains(keyword, StringComparison.OrdinalIgnoreCase)
    );

    private static bool HasOnlyOptionalArguments(string arguments)
    {
        string args = arguments.Trim();
        return args.Length == 0
            || args.Split(',').All(arg => arg.Contains('=') || arg.TrimStart().StartsWith("params "));
    }

    private Dependency? FindDependency(
        Quest predecessor,
        IReadOnlySet<int> currentMapQuestIDs,
        IReadOnlyList<Quest> allQuests
    )
    {
        string scripts = ClientFileSources.SkuaScriptsDIR;
        List<(Dependency Dependency, int Score)> candidates = new();
        List<Quest> sameSlotQuests = allQuests
            .Where(quest => quest.Slot == predecessor.Slot)
            .ToList();
        foreach (string file in Directory.GetFiles(scripts, "*.cs", SearchOption.AllDirectories))
        {
            string relative = Path.GetRelativePath(scripts, file).Replace('\\', '/');
            if (relative.StartsWith("WIP/", StringComparison.OrdinalIgnoreCase)
                || relative.StartsWith("Tools/", StringComparison.OrdinalIgnoreCase))
                continue;
            string source = File.ReadAllText(file);
            if (!Regex.IsMatch(source, $@"\b{predecessor.ID}\b"))
                continue;
            Match classMatch = Regex.Match(source, @"public\s+class\s+(?<name>\w+)");
            if (!classMatch.Success)
                continue;

            foreach (Match method in Regex.Matches(source, @"public\s+void\s+(?<name>\w+)\s*\((?<args>[^)]*)\)\s*\{"))
            {
                if (!HasOnlyOptionalArguments(method.Groups["args"].Value))
                    continue;
                int end = GeneratorSupportUtils.FindClosingBrace(source, method.Index + method.Length - 1);
                if (end < 0)
                    continue;
                string body = source[method.Index..(end + 1)];
                if (!Regex.IsMatch(body, $@"\b{predecessor.ID}\b"))
                    continue;
                if (!Regex.IsMatch(body, $@"isCompletedBefore\s*\(\s*{predecessor.ID}\s*\)"))
                    continue;
                if (currentMapQuestIDs.Any(id => Regex.IsMatch(body, $@"\b{id}\b")))
                    continue;

                List<Quest> referencedSlotQuests = sameSlotQuests
                    .Where(quest => Regex.IsMatch(body, $@"\b{quest.ID}\b"))
                    .ToList();
                if (referencedSlotQuests.Count == 0
                    || referencedSlotQuests.Max(quest => quest.Value) != predecessor.Value
                    || referencedSlotQuests.Any(quest => quest.Value > predecessor.Value))
                    continue;

                int score = 100;
                if (Path.GetFileName(file).StartsWith("Core", StringComparison.OrdinalIgnoreCase))
                    score += 20;
                candidates.Add((new Dependency(relative, classMatch.Groups["name"].Value, method.Groups["name"].Value), score));
            }
        }
        return candidates.OrderByDescending(candidate => candidate.Score).Select(candidate => candidate.Dependency).FirstOrDefault();
    }

    private static DropPacketCollector.MonsterDrops? FindDropMonster(
        ItemBase requirement,
        IEnumerable<Quest> quests,
        IReadOnlyList<DropPacketCollector.MonsterDrops> monsters
    ) => quests
        .Select(quest => FindDropMonster(requirement, quest.Name, monsters))
        .FirstOrDefault(monster => monster != null);

    private static DropPacketCollector.MonsterDrops? FindDropMonster(
        ItemBase requirement,
        string questName,
        IReadOnlyList<DropPacketCollector.MonsterDrops> monsters
    ) => monsters
        .Where(monster => monster.Items.Any(item => item.ID == requirement.ID))
        // ItemID establishes locality. questObjective/questGated confirm that
        // the returned item is bound to this quest when the server supplies
        // those annotations; ordinary ungated drops do not have them.
        .OrderByDescending(monster => monster.Items
            .Where(item => item.ID == requirement.ID)
            .Max(item => QuestAssociation(item, questName)))
        // Prefer a projected mob pack over a single spawn, then the weaker
        // candidate when more than one monster can supply the same item.
        .ThenByDescending(monster => monster.MonMapIDs.Count)
        .ThenBy(monster => monster.MaxHP)
        .ThenBy(monster => monster.MonsterID)
        .FirstOrDefault();

    private static int QuestAssociation(DropPacketCollector.DropItem item, string questName)
    {
        if (item.QuestObjectives.Contains(questName, StringComparer.OrdinalIgnoreCase))
            return 2;
        if (item.QuestGated.Contains(questName, StringComparer.OrdinalIgnoreCase))
            return 1;
        return 0;
    }

    private static bool IsMonsterNameAmbiguous(
        DropPacketCollector.MonsterDrops monster,
        IReadOnlyList<DropPacketCollector.MonsterDrops> usedMonsters
    )
    {
        List<DropPacketCollector.MonsterDrops> sameName = [.. usedMonsters
            .Where(candidate => candidate.MonsterName.Equals(monster.MonsterName, StringComparison.OrdinalIgnoreCase))
            .DistinctBy(candidate => candidate.MonsterID)];
        if (sameName.Count <= 1)
            return false;

        HashSet<int> firstDrops = [.. sameName[0].Items.Select(item => item.ID)];
        return sameName.Skip(1).Any(candidate =>
            !firstDrops.SetEquals(candidate.Items.Select(item => item.ID))
        );
    }

    private static string ClassType(DropPacketCollector.MonsterDrops monster) =>
        monster.MonMapIDs.Count == 1 ? "Solo" : "Farm";

    private static string MapIdentifier(string map)
    {
        string result = new(map.Where(char.IsLetterOrDigit).ToArray());
        if (result.Length == 0)
            result = "Map";
        result = char.ToUpperInvariant(result[0]) + result[1..];
        return char.IsDigit(result[0]) ? "Map" + result : result;
    }

    private static string Escape(string value) => value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    private static string EscapeComment(string value) => value.Replace("\r", " ").Replace("\n", " ");
}