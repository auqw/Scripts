/*
name: Merge Shop Bot Generator/Helper
description: Fill in the map and shop ID and this tool will generate most of the merge bot for you, then you fill in the rest
tags: merge, shop, generator, helper, developer
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Tools/ForDevelopers/CaseStorage.cs

using Skua.Core.Interfaces;
using Skua.Core.Options;
using Skua.Core.Models;
using Skua.Core.Models.Shops;
using Skua.Core.Models.Items;
using Skua.Core.Utils;
using System.IO;
using System.Diagnostics;

public class MergeTemplateHelper
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }    private static CoreAdvanced _Adv;
public static CoreAdvanced sAdv
{
    get => _sAdv ??= new CoreAdvanced();
    set => _sAdv = value;
}
public static CoreAdvanced _sAdv;

    public string OptionsStorage = "MergeTemplateHelper";

    public List<IOption> Options = new()
    {
        new Option<string>("", "Dev-Only", "This bot is to help us make merge bots, the average user won't find any use in this bot", ""),
        new Option<string>("", " ", "", ""),
        new Option<string>("mapName", "Map", "Map of the Merge Shop, please capitalize it properly", ""),
        new Option<int>("shopID", "Shop ID", "ID of the Merge Shop", 0),
        new Option<bool>("genFile", "Generate File", "Generate a MergeTemplate based bot, output will be in \\Scripts\\WIP\\", true)
    };

    // Blacklist tags to filter out common irrelevant words from item names
    private readonly string[] tagsBlacklist =
    {
        "the", "and", "of", "or", "dual", "&amp;", "&",
        "hair", "helm", "hat", "locks", "visage", "helmet", "spike", "spikes", "hood", "hooded", "mask",
        "armor", "cape", "rune", "aura",
        "staff", "staves", "dagger", "sword", "gauntlet", "gun", "revolver", "blade", "wand", "polearm", "axe",
        "gate"
    };

    private readonly string caseStoragePath = Path.Combine(ClientFileSources.SkuaScriptsDIR, "Tools", "ForDevelopers", "CaseStorage.cs");

    // Static stored cases dictionary loaded once
    public static Dictionary<string, string> StoredCases { get; private set; } = CaseStorage.Cases.ToDictionary(x => x.Key, x => string.Join("\n", x.Value));

    // Mutable local copy if needed
    private Dictionary<string, string> storedCases = StoredCases;

    public void ScriptMain(IScriptInterface bot) => Helper();

    /// <summary>
    /// Main helper to generate merge template script based on shop data.
    /// </summary>
    public void Helper()
    {
        string map = Bot.Config?.Get<string>("mapName")?.ToLower() ?? string.Empty;
        int shopID = Bot.Config?.Get<int>("shopID") ?? 0;
        bool genFile = Bot.Config?.Get<bool>("genFile") ?? false;

        if (shopID == 0 || string.IsNullOrWhiteSpace(map))
        {
            Core.Logger("Please fill in the starting form: valid Map and ShopID are required.");
            return;
        }

        LoadStoredCases();

        // Get unique shop items by ID
        List<ShopItem> shopItems = Core.GetShopItems(map, shopID)
            .GroupBy(item => item.ID)
            .Select(g => g.First())
            .ToList();


        string output = string.Empty;
        HashSet<string> processedRequirements = new();
        HashSet<string> itemsToLearn = new();
        string scriptName = Bot.Shops.Name.Replace("Merge", "").Replace("merge", "").Replace(",", "").Replace("’", "").Replace("shop", "").Replace("-", "").Replace("_", "").Replace("Shop", "").Replace("'", "").Trim() + " Merge";
        string className = scriptName.Replace(" ", "");
        string[] multipliedTagsBlacklist = tagsBlacklist.Select(x => x + 's').ToArray();

        string scriptInfo =
           "/*\n" +
           $"name: {scriptName}\n" +
           $"description: This bot will farm the items belonging to the selected mode for the {scriptName} [{shopID}] in /{map}\n" +
           $"tags: ";
        List<string> tags = scriptName.ToLower().Split(' ').ToList();
        tags.Add(map);

        List<string> shopItemNames = new();
        if (genFile)
        {
            shopItemNames.Add("");
            shopItemNames.Add("    public List<IOption> Select = new()");
            shopItemNames.Add("    {");
        }

        List<string> globalFallbackCases = new();
        List<string> globalKnownCases = new();

        foreach (ShopItem item in shopItems)
        {
            if (item.Requirements == null)
                continue;

            shopItemNames.Add($"        new Option<bool>(\"{item.ID}\", \"{item.Name}\", \"Mode: [select] only\\nShould the bot buy \\\"{item.Name}\\\" ?\", false),");

            tags.AddRange(item.Name.ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => new string(x.Where(char.IsLetter).ToArray()))
                .Except(tags)
                .Except(tagsBlacklist)
                .Except(multipliedTagsBlacklist));

            foreach (ItemBase req in item.Requirements)
            {
                if (processedRequirements.Contains(req.Name) || shopItems.Exists(_item => _item.ID == req.ID))
                    continue;

                processedRequirements.Add(req.Name);
                itemsToLearn.Add(req.Name);
                string template = "{req.Name}";
                if (storedCases.TryGetValue(req.Name, out string? caseCode))
                    globalKnownCases.Add(caseCode.TrimEnd());
                else
                {
                    globalFallbackCases.Add($@"
                case ""{req.Name}"":
                    if (req.Upgrade && !Core.IsMember)
                    {{
                        Core.Logger($""{template} requires membership to farm, skipping."");
                        return;
                    }}

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(0000); // TODO: Replace with actual quest ID
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {{
                        Core.HuntMonster(""map"", ""MonsterName"", ""item"", 1, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }}
            Core.CancelRegisteredQuests();
            break;
            ");

                }

            }
        }

        // Output fallback region first
        if (globalFallbackCases.Count > 0)
        {
            output += "\n#region Items not setup\n";
            output += string.Join('\n', globalFallbackCases);
            output += "#endregion\n";
        }

        // Output known cases region next
        if (globalKnownCases.Count > 0)
        {
            output += "\n#region Known items\n";
            output += string.Join('\n', globalKnownCases);
            output += "\n#endregion\n";
        }
        shopItemNames.Add("   };");

        if (!genFile)
        {
            Bot.ShowMessageBox("Please add the following cases to the merge bots:\n" + output, "Merge Template Helper");
            return;
        }

        string[] MergeTemplate = File.ReadAllLines(Path.Combine(ClientFileSources.SkuaScriptsDIR, "Templates", "MergeTemplate.cs"));

        int itemsIndex = Array.IndexOf(MergeTemplate, "                // Add how to get items here") - 1;
        if (itemsIndex < 0)
        {
            Core.Logger("Failed to find index");
            return;
        }
        int classIndex = Array.IndexOf(MergeTemplate, "public class MergeTemplate");
        if (classIndex < 0)
        {
            Core.Logger("Failed to find classIndex");
            return;
        }
        MergeTemplate[classIndex] = $"public class {className}";

        int blackListIndex = Array.IndexOf(MergeTemplate, "        Core.BankingBlackList.AddRange(new[] { \"\" });");
        if (blackListIndex < 0)
        {
            Core.Logger("Failed to find blackListIndex");
            return;
        }
        MergeTemplate[blackListIndex] = "        Core.BankingBlackList.AddRange(new[] { \"" + string.Join("\", \"", itemsToLearn) + "\"});";

        int startIndex = Array.IndexOf(MergeTemplate, "        Adv.StartBuyAllMerge(\"map\", 1234, findIngredients, buyOnlyThis, buyMode: buyMode);");
        if (startIndex < 0)
        {
            Core.Logger("Failed to find startIndex");
            return;
        }
        MergeTemplate[startIndex] = $"        Adv.StartBuyAllMerge(\"{map.ToLower()}\", {shopID}, findIngredients, buyOnlyThis, buyMode: buyMode);";


        scriptInfo += tags.Join(", ") + "\n*/";

        string[] content = new[] { scriptInfo }
                            .Concat(MergeTemplate[5..itemsIndex])
                            .Concat(new[] { output })
                            .Concat(MergeTemplate[(MergeTemplate.Length - 4)..(MergeTemplate.Length - 1)])
                            .Concat(shopItemNames.ToArray())
                            .Concat(new[] { "}" })
                            .ToArray();


        string path = Path.Combine(ClientFileSources.SkuaScriptsDIR, "WIP", className + ".cs");
        Directory.CreateDirectory(Path.Combine(ClientFileSources.SkuaScriptsDIR, "WIP"));
        Core.WriteFile(path, content);
        if (Bot.ShowMessageBox($"File has been generated. Path is {path}\n\nPress OK to open the file",
                                                "File Generated", "OK").Text == "OK")
            Process.Start("explorer", path);
    }

    /// <summary>
    /// Loads stored cases from the specified file path.
    /// </summary>
    // private void LoadStoredCases()
    // {
    //     if (!File.Exists(caseStoragePath))
    //     {
    //         Core.Logger($"Case storage file not found at {caseStoragePath}.");
    //         StoredCases = new Dictionary<string, string>();
    //         return;
    //     }

    //     try
    //     {
    //         string[] lines = File.ReadAllLines(caseStoragePath);
    //         string? currentItem = null;
    //         List<string> currentCaseLines = new();

    //         foreach (string line in lines)
    //         {
    //             if (line.TrimStart().StartsWith("case \""))
    //             {
    //                 if (currentItem != null && currentCaseLines.Count > 0)
    //                     StoredCases[currentItem] = string.Join("\n", currentCaseLines);

    //                 currentCaseLines.Clear();

    //                 int start = line.IndexOf('"') + 1;
    //                 int end = line.IndexOf('"', start);
    //                 currentItem = line[start..end];
    //                 currentCaseLines.Add(line);
    //             }
    //             else if (currentItem != null)
    //             {
    //                 currentCaseLines.Add(line);
    //                 if (line.Trim() == "break;")
    //                 {
    //                     StoredCases[currentItem] = string.Join("\n", currentCaseLines);
    //                     currentItem = null;
    //                     currentCaseLines.Clear();
    //                 }
    //             }
    //         }

    //         if (currentItem != null && currentCaseLines.Count > 0)
    //             StoredCases[currentItem] = string.Join("\n", currentCaseLines);

    //         Core.Logger($"Loaded {StoredCases.Count} stored cases from CaseStorage.");
    //     }
    //     catch (System.Exception ex)
    //     {
    //         Core.Logger($"Failed to load stored cases: {ex.Message}");
    //         StoredCases = new Dictionary<string, string>();
    //     }
    // }

    private static readonly object _caseLock = new();

    private void LoadStoredCases()
    {
        if (!File.Exists(caseStoragePath))
        {
            Core.Logger($"Case storage file not found at {caseStoragePath}.");
            StoredCases = new Dictionary<string, string>();
            return;
        }

        try
        {
            Dictionary<string, string> tempCases = new();
            string[] lines = File.ReadAllLines(caseStoragePath);
            string? currentItem = null;
            List<string> currentCaseLines = new();

            foreach (string line in lines)
            {
                if (line.TrimStart().StartsWith("case \""))
                {
                    if (currentItem != null && currentCaseLines.Count > 0)
                        tempCases[currentItem] = string.Join("\n", currentCaseLines);

                    currentCaseLines.Clear();

                    int start = line.IndexOf('"') + 1;
                    int end = line.IndexOf('"', start);
                    currentItem = line[start..end];
                    currentCaseLines.Add(line);
                }
                else if (currentItem != null)
                {
                    currentCaseLines.Add(line);
                    if (line.Trim() == "break;")
                    {
                        tempCases[currentItem] = string.Join("\n", currentCaseLines);
                        currentItem = null;
                        currentCaseLines.Clear();
                    }
                }
            }

            if (currentItem != null && currentCaseLines.Count > 0)
                tempCases[currentItem] = string.Join("\n", currentCaseLines);

            lock (_caseLock)
                StoredCases = tempCases;

            Core.Logger($"Loaded {StoredCases.Count} stored cases from CaseStorage.");
        }
        catch (Exception ex)
        {
            Core.Logger($"Failed to load stored cases: {ex.Message}");
            StoredCases = new Dictionary<string, string>();
        }
    }

}
