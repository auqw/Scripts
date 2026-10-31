/*
name: Merge Template Helper v2
description: Generates a merge bot and teaches local ingredients from live monsterDrops packets.
tags: developer, merge, generator, drops, packets, v2
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Tools/ForDevelopers/CaseStorage.cs
//cs_include Scripts/Tools/ForDevelopers/GeneratorHelpers/DropPacketCollector.cs
//cs_include Scripts/Tools/ForDevelopers/GeneratorHelpers/LocationSwfQuestReader.cs
//cs_include Scripts/Tools/ForDevelopers/GeneratorHelpers/QuestPacketCollector.cs

using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Models.Shops;
using Skua.Core.Options;
using Skua.Core.Scripts;
using Skua.Core.Utils;
using Skua.Core.ViewModels;

public class MergeTemplateHelperV2
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private sealed record GeneratedCase(string Label, string Body, bool FromCaseStorage);
    private sealed record GenerationContext(
        string Map,
        int ShopID,
        IReadOnlyList<DropPacketCollector.MonsterDrops> Monsters,
        IReadOnlyList<Quest> MapQuests,
        IReadOnlyDictionary<int, IReadOnlyList<int>> MapObjectsByQuest,
        IReadOnlySet<int> ShopItemIDs,
        bool IgnoreCaseStorage
    );

    public string OptionsStorage = "MergeTemplateHelperV2";
    public bool DontPreconfigure = true;
    public List<IOption> Options =
    [
        new Option<string>("location", "Location", "Location containing the merge shop and its monsters.", ""),
        new Option<int>("shopID", "Shop ID", "Optional. Leave 0 to generate every shop found in the location SWF.", 0),
        new Option<bool>("ignoreCaseStorage", "Ignore CaseStorage", "Generate without using stored acquisition cases.", false),
        new Option<bool>("regenerateExisting", "Regenerate Existing Merge", "Select an existing merge script from your Skua Scripts directory and refresh its generated sections.", false),
        CoreBots.Instance.SkipOptions,
    ];

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions(disableClassSwap: true);
        try
        {
            Generate();
        }
        catch (Exception ex)
        {
            Core.Logger($"Generation failed: {ex}", messageBox: true);
        }
        finally
        {
            Core.SetOptions(false);
        }
    }

    private void Generate()
    {
        string map = (Bot.Config?.Get<string>("location") ?? "").Trim().ToLowerInvariant();
        int requestedShopID = Bot.Config?.Get<int>("shopID") ?? 0;
        bool ignoreCaseStorage = Bot.Config?.Get<bool>("ignoreCaseStorage") ?? false;
        bool regenerateExisting = Bot.Config?.Get<bool>("regenerateExisting") ?? false;
        if (regenerateExisting)
        {
            RegenerateExisting(ignoreCaseStorage);
            return;
        }
        if (string.IsNullOrWhiteSpace(map))
            throw new InvalidOperationException("Enter a valid location.");

        Core.Join(map);
        if (!Bot.Wait.ForMapLoad(map))
            throw new InvalidOperationException($"Failed to join /{map}.");

        LocationSwfQuestReader.LocationQuestData location = new LocationSwfQuestReader().ReadCurrentMap();
        IReadOnlyList<int> shopIDs = requestedShopID > 0
            ? [requestedShopID]
            : location.ShopIDs;
        if (shopIDs.Count == 0)
            throw new InvalidOperationException($"No shops were found in the /{map} location SWF.");

        IReadOnlyList<DropPacketCollector.MonsterDrops> monsters = new DropPacketCollector().Collect();
        List<Quest> mapQuests = LoadMapQuests(location.QuestIDs);
        List<string> outputs = [];
        foreach (int shopID in shopIDs.Distinct().OrderBy(id => id))
        {
            string? output = GenerateShop(
                map,
                shopID,
                monsters,
                mapQuests,
                location.MapObjectsByQuest,
                ignoreCaseStorage
            );
            if (output != null)
                outputs.Add(output);
        }
        if (outputs.Count == 0)
            throw new InvalidOperationException($"No merge shops were found in /{map}.");

        Core.Logger($"Generated {outputs.Count} merge template(s):\n{string.Join("\n", outputs)}");

        if (outputs.Count == 1 && File.Exists(outputs[0]))
        {
            Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{outputs[0]}\"")
            {
                UseShellExecute = true
            });
        }
        else
        {
            string outputDirectory = Path.Combine(ClientFileSources.SkuaScriptsDIR, "WIP");

            Process.Start(new ProcessStartInfo("explorer.exe", outputDirectory)
            {
                UseShellExecute = true
            });
        }
    }

    private void RegenerateExisting(bool ignoreCaseStorage)
    {
        IFileDialogService fileDialog = Ioc.Default.GetRequiredService<IFileDialogService>();
        string? path = fileDialog.OpenFile(ClientFileSources.SkuaScriptsDIR, "Skua Merge Script (*.cs)|*.cs");
        if (string.IsNullOrWhiteSpace(path))
            return;

        string scriptsDirectory = Path.GetFullPath(ClientFileSources.SkuaScriptsDIR);
        string selectedPath = Path.GetFullPath(path);
        string relativePath = Path.GetRelativePath(scriptsDirectory, selectedPath);
        if (relativePath == ".." || relativePath.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal)
            || Path.IsPathRooted(relativePath))
            throw new InvalidOperationException("Select a .cs file inside your Skua Scripts directory.");

        string existingContent = File.ReadAllText(selectedPath);
        Match mergeCall = Regex.Match(
            existingContent,
            @"Adv\.StartBuyAllMerge\s*\(\s*""(?<map>(?:\\.|[^""])*)""\s*,\s*(?<shopID>\d+)",
            RegexOptions.CultureInvariant
        );
        if (!mergeCall.Success
            || !int.TryParse(mergeCall.Groups["shopID"].Value, out int shopID)
            || shopID <= 0)
            throw new InvalidOperationException("The selected file does not contain a valid Adv.StartBuyAllMerge call with a quoted map and numeric shop ID.");

        string map = mergeCall.Groups["map"].Value.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(map))
            throw new InvalidOperationException("The selected merge call contains an empty map name.");

        Core.Logger($"Regenerating {Path.GetFileName(selectedPath)} from /{map}, shop {shopID}.");
        Core.Join(map);
        if (!Bot.Wait.ForMapLoad(map))
            throw new InvalidOperationException($"Failed to join /{map}.");

        LocationSwfQuestReader.LocationQuestData location = new LocationSwfQuestReader().ReadCurrentMap();
        if (!location.ShopIDs.Contains(shopID))
            Core.Logger($"Shop {shopID} was not listed in the current location SWF; attempting to load it directly.");

        IReadOnlyList<DropPacketCollector.MonsterDrops> monsters = new DropPacketCollector().Collect();
        List<Quest> mapQuests = LoadMapQuests(location.QuestIDs);
        string? output = GenerateShop(
            map,
            shopID,
            monsters,
            mapQuests,
            location.MapObjectsByQuest,
            ignoreCaseStorage,
            selectedPath
        );
        if (output == null)
            throw new InvalidOperationException($"Shop {shopID} in /{map} is not a merge shop or has no requirements.");

        Core.Logger($"Successfully regenerated {Path.GetFileName(selectedPath)}. Existing custom code outside generated sections was retained.");
        Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{selectedPath}\"")
        {
            UseShellExecute = true
        });
    }

    private string? GenerateShop(
        string map,
        int shopID,
        IReadOnlyList<DropPacketCollector.MonsterDrops> monsters,
        IReadOnlyList<Quest> mapQuests,
        IReadOnlyDictionary<int, IReadOnlyList<int>> mapObjectsByQuest,
        bool ignoreCaseStorage,
        string? existingFilePath = null
    )
    {
        List<ShopItem> allShopItems = Core.GetShopItems(map, shopID)
            .GroupBy(item => item.ID)
            .Select(group => group.First())
            .ToList();
        List<ShopItem> shopItems = allShopItems
            .Where(item => item.Requirements is { Count: > 0 })
            .ToList();
        if (shopItems.Count == 0)
        {
            Core.Logger($"Skipping non-merge shop {Bot.Shops.Name ?? shopID.ToString()} [{shopID}].");
            return null;
        }

        HashSet<int> shopItemIDs = allShopItems.Select(item => item.ID).ToHashSet();
        List<ItemBase> ingredients = shopItems
            .SelectMany(item => item.Requirements!)
            .Where(item => !shopItemIDs.Contains(item.ID))
            .GroupBy(item => item.ID)
            .Select(group => group.First())
            .OrderBy(item => item.Name)
            .ToList();

        string shopName = Bot.Shops.Name ?? $"Shop {shopID}";
        string displayName = CleanShopName(shopName) + " Merge";
        string className = Identifier(shopName);
        GenerationContext context = new(
            map,
            shopID,
            monsters,
            mapQuests,
            mapObjectsByQuest,
            shopItemIDs,
            ignoreCaseStorage
        );
        List<GeneratedCase> cases = [.. ingredients.Select(ingredient => GenerateCase(ingredient, context))];
        List<(Quest Target, Quest Predecessor)> unlocks = [];
        HashSet<int> inspectedUnlockQuests = [];
        foreach (ItemBase ingredient in ingredients)
        {
            Quest? rewardQuest = FindRewardQuest(ingredient, mapQuests);
            if (rewardQuest != null)
                CollectQuestUnlocks(rewardQuest, context, inspectedUnlockQuests, unlocks);
        }
        List<string> lines =
        [
            "/*",
            $"name: {displayName}",
            $"description: Farms the {displayName} [{shopID}] in /{map}.",
            $"tags: {map}, merge, {string.Join(", ", displayName.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries))}",
            "*/",
            "//cs_include Scripts/CoreBots.cs",
            "//cs_include Scripts/CoreFarms.cs",
            "//cs_include Scripts/CoreAdvanced.cs",
            "using Skua.Core.Interfaces;",
            "using Skua.Core.Models.Items;",
            "using Skua.Core.Options;",
            "",
            $"public class {className}",
            "{",
            "    private IScriptInterface Bot => IScriptInterface.Instance;",
            "    private CoreBots Core => CoreBots.Instance;",
            "    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }",
            "    private static CoreFarms _Farm;",
            "    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }",
            "    private static CoreAdvanced _Adv;",
            "    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }",
            "    private static CoreAdvanced _sAdv;",
            "",
            "    public bool DontPreconfigure = true;",
            "    public List<IOption> Generic = sAdv.MergeOptions;",
            "    public string[] MultiOptions = { \"Generic\", \"Select\" };",
            "    public string OptionsStorage = sAdv.OptionsStorage;",
            "    private bool dontStopMissingIng = false;",
            "",
            "    public void ScriptMain(IScriptInterface Bot)",
            "    {",
            "        Core.BankingBlackList.AddRange([",
            .. ingredients.Select(item => $"            \"{Escape(item.Name)}\","),
            .. new[]
            {
                "        ]);",
                "        Core.SetOptions();",
                "        BuyAllMerge();",
                "        Core.SetOptions(false);",
                "    }",
                "",
                "    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)",
                "    {",
            },
        ];
        foreach ((Quest target, Quest predecessor) in unlocks
            .DistinctBy(unlock => unlock.Target.ID)
            .OrderBy(unlock => unlock.Target.ID))
            lines.Add($"        // FILL_QUEST_UNLOCK: Add the story call that completes \"{Escape(predecessor.Name)}\" [{predecessor.ID}] to unlock \"{Escape(target.Name)}\" [{target.ID}].");
        lines.AddRange(new[]
        {
            $"        Adv.StartBuyAllMerge(\"{map}\", {shopID}, findIngredients, buyOnlyThis, buyMode: buyMode);",
            "",
            "        void findIngredients()",
            "        {",
            "            ItemBase req = Adv.externalItem;",
            "            int quant = Adv.externalQuant;",
            "            if (req == null)",
            "                return;",
            "",
            "            switch (req.Name)",
            "            {",
        });
        foreach (IGrouping<string, GeneratedCase> group in cases.GroupBy(value => value.Body))
        {
            foreach (GeneratedCase generatedCase in group.OrderBy(value => value.Label))
            {
                string provenance = generatedCase.FromCaseStorage ? " // #FROM CASE STORAGE" : string.Empty;
                lines.Add($"                case \"{Escape(generatedCase.Label)}\":{provenance}");
            }
            lines.AddRange(group.Key.Replace("\r", "").Split('\n'));
        }
        lines.AddRange(new[]
        {
            "                default:",
            "                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;",
            "                    Core.Logger($\"The bot hasn't been taught how to get {req.Name}.\", messageBox: shouldStop, stopBot: shouldStop);",
            "                    break;",
            "            }",
            "        }",
            "    }",
            "",
            "    public List<IOption> Select =",
            "    [",
        });
        lines.AddRange(shopItems.Select(item =>
            $"        new Option<bool>(\"{item.ID}\", \"{Escape(item.Name)}\", \"Mode: [select] only\\nShould the bot buy \\\"{Escape(item.Name)}\\\" ?\", false),"
        ));
        lines.AddRange(new[]
        {
            "    ];",
            "}",
        });

        string outputDirectory = Path.Combine(ClientFileSources.SkuaScriptsDIR, "WIP");
        Directory.CreateDirectory(outputDirectory);
        string path = existingFilePath ?? Path.Combine(outputDirectory, className + ".cs");
        string generatedContent = string.Join(Environment.NewLine, lines);
        if (existingFilePath != null)
        {
            string existingContent = File.ReadAllText(existingFilePath);
            string backupPath = existingFilePath + ".bak";
            if (!File.Exists(backupPath))
                File.Copy(existingFilePath, backupPath);
            generatedContent = MergeExistingFile(existingContent, generatedContent);
            File.WriteAllText(existingFilePath, generatedContent);
        }
        else
            Core.WriteFile(path, [.. lines]);
        return path;
    }


    /// <summary>
    /// Updates generated metadata, class name, ingredient cases, and Select options while retaining
    /// the selected script's custom code outside those generated sections.
    /// </summary>
    private static string MergeExistingFile(string existingContent, string generatedContent)
    {
        string result = existingContent;
        string[] metadataKeys = ["name", "description", "tags"];
        foreach (string key in metadataKeys)
        {
            Match generatedMetadata = Regex.Match(
                generatedContent,
                $@"(?m)^\s*{key}:\s*.*$",
                RegexOptions.CultureInvariant
            );
            if (!generatedMetadata.Success)
                continue;

            Match existingMetadata = Regex.Match(
                result,
                $@"(?m)^\s*{key}:\s*.*$",
                RegexOptions.CultureInvariant
            );
            if (existingMetadata.Success)
                result = result.Remove(existingMetadata.Index, existingMetadata.Length)
                    .Insert(existingMetadata.Index, generatedMetadata.Value);
        }

        Match generatedClass = Regex.Match(generatedContent, @"(?m)^\s*public\s+class\s+\w+");
        Match existingClass = Regex.Match(result, @"(?m)^\s*public\s+class\s+\w+");
        if (generatedClass.Success && existingClass.Success)
            result = result.Remove(existingClass.Index, existingClass.Length)
                .Insert(existingClass.Index, generatedClass.Value);

        result = ReplaceGeneratedSwitch(result, generatedContent);
        result = ReplaceSelectOptions(result, generatedContent);
        return result;
    }

    /// <summary>
    /// Replaces generated ingredient cases, reusing existing case bodies for ingredients that remain.
    /// </summary>
    private static string ReplaceGeneratedSwitch(string existingContent, string generatedContent)
    {
        (int ExistingOpen, int ExistingClose) = FindSwitchBounds(existingContent);
        (int GeneratedOpen, int GeneratedClose) = FindSwitchBounds(generatedContent);
        if (ExistingOpen < 0 || ExistingClose < 0 || GeneratedOpen < 0 || GeneratedClose < 0)
            return existingContent;

        string existingBody = existingContent[(ExistingOpen + 1)..ExistingClose];
        string generatedBody = generatedContent[(GeneratedOpen + 1)..GeneratedClose];
        Dictionary<string, string> oldCaseBodies = ExtractCaseBodies(existingBody);
        MatchCollection generatedLabels = Regex.Matches(
            generatedBody,
            @"(?m)^[ \t]*case\s+""(?<label>(?:\\.|[^""])*)""\s*:",
            RegexOptions.CultureInvariant
        );
        if (generatedLabels.Count == 0)
            return existingContent;

        string prefix = generatedBody[..generatedLabels[0].Index];
        List<string> refreshedCases = [];
        List<(string Label, string Line)> pendingLabels = [];
        for (int i = 0; i < generatedLabels.Count; i++)
        {
            Match labelMatch = generatedLabels[i];
            int lineEnd = generatedBody.IndexOf('\n', labelMatch.Index + labelMatch.Length);
            int segmentStart = lineEnd < 0 ? labelMatch.Index + labelMatch.Length : lineEnd + 1;
            int segmentEnd = i + 1 < generatedLabels.Count ? generatedLabels[i + 1].Index : generatedBody.Length;
            string segment = generatedBody[segmentStart..segmentEnd];
            string label = labelMatch.Groups["label"].Value.Replace("\\\"", "\"").Replace("\\\\", "\\");
            string labelLine = generatedBody[labelMatch.Index..segmentStart];
            pendingLabels.Add((label, labelLine));

            if (!ContainsCaseCode(segment) && i + 1 < generatedLabels.Count)
                continue;

            foreach ((string pendingLabel, string pendingLine) in pendingLabels)
            {
                string body = oldCaseBodies.TryGetValue(pendingLabel, out string? oldBody) ? oldBody : segment;
                refreshedCases.Add(pendingLine + body);
            }
            pendingLabels.Clear();
        }

        string mergedBody = prefix + string.Join("", refreshedCases);
        return existingContent.Remove(ExistingOpen + 1, ExistingClose - ExistingOpen - 1)
            .Insert(ExistingOpen + 1, mergedBody);
    }

    private static Dictionary<string, string> ExtractCaseBodies(string switchBody)
    {
        Dictionary<string, string> result = [];
        MatchCollection labels = Regex.Matches(
            switchBody,
            @"(?m)^[ \t]*case\s+""(?<label>(?:\\.|[^""])*)""\s*:",
            RegexOptions.CultureInvariant
        );
        List<string> pendingLabels = [];
        for (int i = 0; i < labels.Count; i++)
        {
            Match label = labels[i];
            int lineEnd = switchBody.IndexOf('\n', label.Index + label.Length);
            int start = lineEnd < 0 ? label.Index + label.Length : lineEnd + 1;
            int end = i + 1 < labels.Count ? labels[i + 1].Index : switchBody.Length;
            string body = switchBody[start..end];
            pendingLabels.Add(label.Groups["label"].Value.Replace("\\\"", "\"").Replace("\\\\", "\\"));
            if (!ContainsCaseCode(body) && i + 1 < labels.Count)
                continue;
            foreach (string pending in pendingLabels)
                result[pending] = body;
            pendingLabels.Clear();
        }
        return result;
    }

    private static bool ContainsCaseCode(string value)
    {
        string withoutComments = Regex.Replace(value, @"(?m)//.*$", "");
        return !string.IsNullOrWhiteSpace(withoutComments);
    }

    private static (int Open, int Close) FindSwitchBounds(string content)
    {
        Match match = Regex.Match(content, @"\bswitch\s*\(\s*req\.Name\s*\)", RegexOptions.CultureInvariant);
        if (!match.Success)
            return (-1, -1);
        int open = content.IndexOf('{', match.Index + match.Length);
        if (open < 0)
            return (-1, -1);
        int close = FindMatchingDelimiter(content, open, '{', '}');
        return (open, close);
    }

    private static string ReplaceSelectOptions(string existingContent, string generatedContent)
    {
        int existingStart = existingContent.IndexOf("public List<IOption> Select", StringComparison.Ordinal);
        int generatedStart = generatedContent.IndexOf("public List<IOption> Select", StringComparison.Ordinal);
        if (existingStart < 0 || generatedStart < 0)
            return existingContent;

        int existingEquals = existingContent.IndexOf('=', existingStart);
        int generatedEquals = generatedContent.IndexOf('=', generatedStart);
        int existingOpen = FindInitializerOpen(existingContent, existingEquals + 1);
        int generatedOpen = FindInitializerOpen(generatedContent, generatedEquals + 1);
        if (existingOpen < 0 || generatedOpen < 0)
            return existingContent;

        char existingOpenChar = existingContent[existingOpen];
        char generatedOpenChar = generatedContent[generatedOpen];
        char existingCloseChar = existingOpenChar == '[' ? ']' : '}';
        char generatedCloseChar = generatedOpenChar == '[' ? ']' : '}';
        int existingClose = FindMatchingDelimiter(existingContent, existingOpen, existingOpenChar, existingCloseChar);
        int generatedClose = FindMatchingDelimiter(generatedContent, generatedOpen, generatedOpenChar, generatedCloseChar);
        if (existingClose < 0 || generatedClose < 0)
            return existingContent;

        int existingEnd = existingClose + 1;
        while (existingEnd < existingContent.Length && char.IsWhiteSpace(existingContent[existingEnd]))
            existingEnd++;
        if (existingEnd < existingContent.Length && existingContent[existingEnd] == ';')
            existingEnd++;
        int generatedEnd = generatedClose + 1;
        while (generatedEnd < generatedContent.Length && char.IsWhiteSpace(generatedContent[generatedEnd]))
            generatedEnd++;
        if (generatedEnd < generatedContent.Length && generatedContent[generatedEnd] == ';')
            generatedEnd++;

        string generatedSection = generatedContent[generatedStart..generatedEnd];
        return existingContent.Remove(existingStart, existingEnd - existingStart)
            .Insert(existingStart, generatedSection);
    }

    private static int FindInitializerOpen(string content, int start)
    {
        for (int i = Math.Max(0, start); i < content.Length; i++)
            if (content[i] is '[' or '{')
                return i;
        return -1;
    }

    private static int FindMatchingDelimiter(string content, int start, char open, char close)
    {
        int depth = 0;
        bool inString = false;
        bool escaped = false;
        for (int i = start; i < content.Length; i++)
        {
            char current = content[i];
            if (inString)
            {
                if (escaped)
                    escaped = false;
                else if (current == '\\')
                    escaped = true;
                else if (current == '"')
                    inString = false;
                continue;
            }
            if (current == '"')
                inString = true;
            else if (current == open)
                depth++;
            else if (current == close && --depth == 0)
                return i;
        }
        return -1;
    }

    private List<Quest> LoadMapQuests(IReadOnlyList<int> questIDs)
    {
        List<Quest> result = [];
        foreach (int questID in questIDs.Where(id => id > 0).Distinct())
        {
            Core.Logger($"Capturing quest {questID} from its getQuests packet.");
            result.Add(new QuestPacketCollector().Load(questID));
            Core.Sleep();
        }
        return result;
    }

    private static GeneratedCase GenerateCase(ItemBase ingredient, GenerationContext context)
    {
        if (IsGoldVoucher(ingredient))
            return new GeneratedCase(
                ingredient.Name,
                @"                    Farm.Voucher(req.Name, quant);
                    break;",
                FromCaseStorage: false
            );

        DropPacketCollector.MonsterDrops? monster = FindMonster(ingredient, context.Monsters);
        if (monster != null)
            return new GeneratedCase(
                ingredient.Name,
                $@"                    Core.EquipClass(ClassType.{MonsterClass(monster)});
                    Core.HuntMonster(""{context.Map}"", ""{Escape(monster.MonsterName)}"", req.Name, quant, req.Temp);
                    break;",
                FromCaseStorage: false
            );

        Quest? rewardQuest = FindRewardQuest(ingredient, context.MapQuests);
        if (rewardQuest != null)
            return new GeneratedCase(
                ingredient.Name,
                GenerateQuestRewardBody(rewardQuest, context),
                FromCaseStorage: false
            );

        if (!context.IgnoreCaseStorage
            && CaseStorage.Cases.TryGetValue(ingredient.Name, out string? knownCase)
            && knownCase != null)
            return new GeneratedCase(
                ingredient.Name,
                ExtractStoredCaseBody(knownCase),
                FromCaseStorage: true
            );

        return new GeneratedCase(
            ingredient.Name,
            $@"                    // FILL_ACQUISITION: Add acquisition logic for {EscapeComment(ingredient.Name)} [{ingredient.ID}].
                    break;",
            FromCaseStorage: false
        );
    }

    private static Quest? FindRewardQuest(ItemBase ingredient, IReadOnlyList<Quest> mapQuests) => mapQuests
        .Where(quest => (quest.SimpleRewards?.Any(reward =>
                reward != null
                && (reward.ID == ingredient.ID
                    || string.Equals(reward.Name, ingredient.Name, StringComparison.OrdinalIgnoreCase))
            ) == true)
            || (quest.Rewards?.Any(reward =>
                reward != null
                && (reward.ID == ingredient.ID
                    || string.Equals(reward.Name, ingredient.Name, StringComparison.OrdinalIgnoreCase))
            ) == true))
        .OrderBy(quest => quest.Slot == -1 ? 1 : 0)
        .ThenBy(quest => quest.Value)
        .ThenBy(quest => quest.ID)
        .FirstOrDefault();

    private static string GenerateQuestRewardBody(Quest target, GenerationContext context)
    {
        List<string> lines =
        [
            "                    Core.FarmingLogger(req.Name, quant);",
        ];
        HashSet<int> registeredQuestIDs = [];
        CollectRegisteredQuests(target, context, registeredQuestIDs, []);
        if (registeredQuestIDs.Count > 0)
        {
            foreach (Quest quest in context.MapQuests.Where(quest => registeredQuestIDs.Contains(quest.ID)))
                foreach (ItemBase requirement in quest.AcceptRequirements ?? [])
                    EmitRequirement(lines, requirement, quest, context, [], "                    ", null);
            lines.Add($"                    Core.RegisterQuests({string.Join(", ", BuildRegisteredQuestArguments(registeredQuestIDs, context.MapQuests))});");
        }

        lines.Add("                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))");
        lines.Add("                    {");
        if (registeredQuestIDs.Contains(target.ID))
            EmitQuestRequirements(lines, target, context, [target.ID], "                        ", registeredQuestIDs);
        else
            EmitQuestAttempt(lines, target, context, [], "                        ", "req.ID", registeredQuestIDs);
        lines.Add("                        Bot.Wait.ForPickup(req.Name);");
        lines.Add("                    }");
        if (registeredQuestIDs.Count > 0)
            lines.Add("                    Core.CancelRegisteredQuests();");
        lines.Add("                    break;");
        return string.Join("\n", lines);
    }

    private static IReadOnlyList<string> BuildRegisteredQuestArguments(
        IReadOnlySet<int> registeredQuestIDs,
        IReadOnlyList<Quest> mapQuests
    )
    {
        List<string> arguments = [];
        HashSet<int> handled = [];
        foreach (Quest quest in mapQuests
            .Where(quest => registeredQuestIDs.Contains(quest.ID))
            .OrderBy(quest => quest.ID))
        {
            if (!handled.Add(quest.ID))
                continue;
            Quest? counterpart = mapQuests.FirstOrDefault(candidate =>
                candidate.ID != quest.ID
                && !quest.Once
                && !candidate.Once
                && candidate.Slot == quest.Slot
                && candidate.Value == quest.Value
                && candidate.Upgrade != quest.Upgrade
                && RewardsOverlap(quest, candidate));
            if (counterpart == null)
            {
                arguments.Add(quest.ID.ToString());
                continue;
            }

            handled.Add(counterpart.ID);
            Quest member = quest.Upgrade ? quest : counterpart;
            Quest regular = quest.Upgrade ? counterpart : quest;
            arguments.Add($"Core.IsMember ? {member.ID} : {regular.ID}");
        }
        return arguments;
    }

    private static bool RewardsOverlap(Quest left, Quest right)
    {
        HashSet<int> leftIDs = (left.SimpleRewards ?? [])
            .Where(reward => reward != null && reward.ID > 0)
            .Select(reward => reward.ID)
            .Concat((left.Rewards ?? [])
                .Where(reward => reward != null && reward.ID > 0)
                .Select(reward => reward.ID))
            .ToHashSet();
        if ((right.SimpleRewards ?? [])
            .Any(reward => reward != null && leftIDs.Contains(reward.ID)))
            return true;
        return (right.Rewards ?? [])
            .Any(reward => reward != null && leftIDs.Contains(reward.ID));
    }

    private static void CollectRegisteredQuests(
        Quest quest,
        GenerationContext context,
        HashSet<int> registered,
        HashSet<int> visited
    )
    {
        if (!visited.Add(quest.ID))
            return;
        if (!QuestPacketCollector.HasChoiceRewards(quest))
            registered.Add(quest.ID);
        foreach (ItemBase requirement in (quest.AcceptRequirements ?? new()).Concat(quest.Requirements ?? new()))
        {
            Quest? dependency = FindRewardQuest(requirement, context.MapQuests);
            if (dependency != null)
                CollectRegisteredQuests(dependency, context, registered, visited);
        }
    }

    private static void CollectQuestUnlocks(
        Quest quest,
        GenerationContext context,
        HashSet<int> visited,
        List<(Quest Target, Quest Predecessor)> unlocks
    )
    {
        if (!visited.Add(quest.ID))
            return;
        Quest? predecessor = context.MapQuests
            .Where(candidate => quest.Slot != -1
                && candidate.Slot == quest.Slot
                && candidate.Once
                && candidate.ID != quest.ID
                && (candidate.Value < quest.Value
                    || (!quest.Once && candidate.Value == quest.Value)))
            // A repeatable farm quest at the terminal value is unlocked by
            // the one-time finale at that same value, not value - 1.
            .OrderByDescending(candidate => candidate.Value)
            .ThenByDescending(candidate => candidate.ID)
            .FirstOrDefault();
        if (predecessor != null)
            unlocks.Add((quest, predecessor));
        foreach (ItemBase requirement in (quest.AcceptRequirements ?? new()).Concat(quest.Requirements ?? new()))
        {
            Quest? dependency = FindRewardQuest(requirement, context.MapQuests);
            if (dependency != null)
                CollectQuestUnlocks(dependency, context, visited, unlocks);
        }
    }

    private static void EmitQuestAttempt(
        List<string> lines,
        Quest quest,
        GenerationContext context,
        HashSet<int> questStack,
        string indent,
        string? rewardExpression = null,
        IReadOnlySet<int>? registeredQuestIDs = null
    )
    {
        if (!questStack.Add(quest.ID))
        {
            lines.Add($"{indent}Core.Logger(\"Recursive quest dependency detected at {Escape(quest.Name)} [{quest.ID}].\", messageBox: true, stopBot: true);");
            return;
        }

        foreach (ItemBase requirement in quest.AcceptRequirements ?? [])
            EmitRequirement(lines, requirement, quest, context, questStack, indent, registeredQuestIDs);

        lines.Add($"{indent}Core.EnsureAccept({quest.ID});");
        EmitQuestRequirements(lines, quest, context, questStack, indent, registeredQuestIDs);

        string reward = rewardExpression != null && QuestPacketCollector.HasChoiceRewards(quest)
            ? $", {rewardExpression}"
            : string.Empty;
        lines.Add($"{indent}Core.EnsureComplete({quest.ID}{reward});");
        questStack.Remove(quest.ID);
    }

    private static void EmitQuestRequirements(
        List<string> lines,
        Quest quest,
        GenerationContext context,
        HashSet<int> questStack,
        string indent,
        IReadOnlySet<int>? registeredQuestIDs
    )
    {
        List<ItemBase> requirements = quest.Requirements ?? new();
        IReadOnlyList<int> mapObjects = context.MapObjectsByQuest.TryGetValue(quest.ID, out IReadOnlyList<int>? ids)
            ? ids
            : [];
        List<ItemBase> unresolved = [.. requirements.Where(requirement => !CanResolveWithoutMapObject(requirement, context))];
        Dictionary<int, int> mapObjectByItem = mapObjects.Count >= unresolved.Count
            ? unresolved.Select((requirement, index) => (requirement.ID, mapObjects[index]))
                .GroupBy(pair => pair.ID)
                .ToDictionary(group => group.Key, group => group.First().Item2)
            : new();

        List<(ItemBase Requirement, DropPacketCollector.MonsterDrops Monster)> hunts = requirements
            .Where(requirement => !mapObjectByItem.ContainsKey(requirement.ID))
            .Select(requirement => (Requirement: requirement, Monster: FindMonster(requirement, context.Monsters)))
            .Where(entry => entry.Monster != null)
            .Select(entry => (entry.Requirement, entry.Monster!))
            .ToList();

        foreach (ItemBase requirement in requirements)
        {
            if (mapObjectByItem.TryGetValue(requirement.ID, out int mapObjectID))
            {
                lines.Add($"{indent}Core.GetMapItem({mapObjectID}, {Math.Max(1, requirement.Quantity)}, \"{context.Map}\"); // {EscapeComment(requirement.Name)}");
                continue;
            }
            if (hunts.Any(hunt => hunt.Requirement.ID == requirement.ID))
                continue;
            EmitRequirement(lines, requirement, quest, context, questStack, indent, registeredQuestIDs);
        }

        foreach (string classType in new[] { "Solo", "Farm" })
        {
            List<(ItemBase Requirement, DropPacketCollector.MonsterDrops Monster)> group = hunts
                .Where(hunt => MonsterClass(hunt.Monster) == classType)
                .ToList();
            if (group.Count == 0)
                continue;
            lines.Add($"{indent}Core.EquipClass(ClassType.{classType});");
            foreach ((ItemBase requirement, DropPacketCollector.MonsterDrops monster) in group)
            {
                int quantity = Math.Max(1, requirement.Quantity);
                string temp = requirement.Temp ? string.Empty : ", isTemp: false";
                lines.Add($"{indent}Core.HuntMonster(\"{context.Map}\", \"{Escape(monster.MonsterName)}\", \"{Escape(requirement.Name)}\", {quantity}{temp});");
            }
        }
    }

    private static bool CanResolveWithoutMapObject(ItemBase requirement, GenerationContext context) =>
        context.ShopItemIDs.Contains(requirement.ID)
        || FindMonster(requirement, context.Monsters) != null
        || FindRewardQuest(requirement, context.MapQuests) != null;

    private static void EmitRequirement(
        List<string> lines,
        ItemBase requirement,
        Quest owner,
        GenerationContext context,
        HashSet<int> questStack,
        string indent,
        IReadOnlySet<int>? registeredQuestIDs = null
    )
    {
        int quantity = Math.Max(1, requirement.Quantity);
        if (IsGoldVoucher(requirement))
        {
            lines.Add($"{indent}Farm.Voucher(\"{Escape(requirement.Name)}\", {quantity});");
            return;
        }

        if (context.ShopItemIDs.Contains(requirement.ID))
        {
            lines.Add($"{indent}Adv.BuyItem(\"{context.Map}\", {context.ShopID}, {requirement.ID}, {quantity}); // {EscapeComment(requirement.Name)}");
            return;
        }

        DropPacketCollector.MonsterDrops? monster = FindMonster(requirement, context.Monsters);
        if (monster != null)
        {
            string temp = requirement.Temp ? string.Empty : ", isTemp: false";
            lines.Add($"{indent}Core.HuntMonster(\"{context.Map}\", \"{Escape(monster.MonsterName)}\", \"{Escape(requirement.Name)}\", {quantity}{temp});");
            return;
        }

        Quest? dependency = FindRewardQuest(requirement, context.MapQuests);
        if (dependency != null)
        {
            lines.Add($"{indent}while (!Bot.ShouldExit && !Core.CheckInventory({requirement.ID}, {quantity})) // {EscapeComment(requirement.Name)}");
            lines.Add($"{indent}{{");
            if (registeredQuestIDs?.Contains(dependency.ID) == true)
            {
                if (!questStack.Add(dependency.ID))
                {
                    lines.Add($"{indent}    Core.Logger(\"Recursive quest dependency detected at {Escape(dependency.Name)} [{dependency.ID}].\", messageBox: true, stopBot: true);");
                }
                else
                {
                    EmitQuestRequirements(lines, dependency, context, questStack, indent + "    ", registeredQuestIDs);
                    questStack.Remove(dependency.ID);
                }
            }
            else
            {
                EmitQuestAttempt(lines, dependency, context, questStack, indent + "    ", requirement.ID.ToString(), registeredQuestIDs);
            }
            lines.Add($"{indent}    Bot.Wait.ForPickup({requirement.ID});");
            lines.Add($"{indent}}}");
            return;
        }

        lines.Add($"{indent}Core.Logger(\"Cannot automate {Escape(requirement.Name)} [{requirement.ID}] for {Escape(owner.Name)} [{owner.ID}] from this map.\", messageBox: true, stopBot: true);");
    }

    private static DropPacketCollector.MonsterDrops? FindMonster(
        ItemBase ingredient,
        IReadOnlyList<DropPacketCollector.MonsterDrops> monsters
    ) => monsters
        .Where(monster => monster.Items.Any(item =>
            item != null
            && (item.ID == ingredient.ID
                || string.Equals(item.Name, ingredient.Name, StringComparison.OrdinalIgnoreCase))
        ))
        .OrderBy(monster => monster.MaxHP)
        .FirstOrDefault();

    private static string MonsterClass(DropPacketCollector.MonsterDrops monster) =>
        monster.MonMapIDs.Count == 1 ? "Solo" : "Farm";

    private static string ExtractStoredCaseBody(string value)
    {
        string[] lines = [.. value.Replace("\r", "")
            .Split('\n')
            .SkipWhile(string.IsNullOrWhiteSpace)
            .Reverse()
            .SkipWhile(string.IsNullOrWhiteSpace)
            .Reverse()
            .Select(line => line.TrimEnd())];
        int bodyStart = 0;
        while (bodyStart < lines.Length
            && lines[bodyStart].TrimStart().StartsWith("case ", StringComparison.Ordinal))
            bodyStart++;
        return string.Join("\n", lines[bodyStart..]);
    }

    private static string CleanShopName(string value)
    {
        foreach (string remove in new[] { "Merge", "merge", "Shop", "shop", ",", "'", "’", "-", "_" })
            value = value.Replace(remove, "");
        return string.Join(" ", value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    private static string Identifier(string value)
    {
        string result = new([.. value.Where(char.IsLetterOrDigit)]);
        if (string.IsNullOrEmpty(result))
            result = "GeneratedMerge";
        return char.IsDigit(result[0]) ? "Merge" + result : result;
    }

    private static string Escape(string value) => value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    private static string EscapeComment(string value) => value.Replace("\r", " ").Replace("\n", " ");
    private static bool IsGoldVoucher(ItemBase item) =>
        item.Name?.Contains("Gold Voucher", StringComparison.OrdinalIgnoreCase) == true;
}