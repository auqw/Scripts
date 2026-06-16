/*
name: Generate Quest Files
description: This will generate the the files needed for the QuestData sheet and the #quest-ids channel in our discord
tags: quests, developer, lists, files, spreadsheet, excel, data
*/
//cs_include Scripts/CoreBots.cs
using System.Collections.Generic;
using System.IO;
using CommunityToolkit.Mvvm.DependencyInjection;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Quests;
using Skua.Core.Scripts;

public class GetQuests
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private CancellationTokenSource? _cts;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.Logger("Starting quest generation...");
        _cts = new();

        try
        {
            GenerateQuestFiles(_cts.Token);
        }
        catch (OperationCanceledException)
        {
            Core.Logger("Script stopped by user.");
        }
        finally
        {
            _cts.Dispose();
            _cts = null;
        }

        Core.Logger("Update Complete.");
    }

    private void GenerateQuestFiles(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        Core.Logger("Updating Quests.json (client)...");
        UpdateQuests(token); // blocks but cancelable

        token.ThrowIfCancellationRequested();

        // Read the authoritative client JSON
        string clientJsonPath = Path.Combine(ClientFileSources.SkuaDIR, "QuestData.json");
        dynamic[] quests;

        try
        {
            Core.Logger("Reading updated client QuestData.json...");
            string jsonText = File.ReadAllText(clientJsonPath);
            quests = JsonConvert.DeserializeObject<dynamic[]>(jsonText)!;
            Core.Logger($"Loaded {quests.Length} quests."); // should show 10559
        }
        catch (Exception ex)
        {
            Core.Logger("Failed to read or parse client QuestData.json: " + ex.Message);
            return;
        }

        token.ThrowIfCancellationRequested();

        // Build pipe-delimited data if needed (optional)
        List<string> d = new() { "ID|Name|Once|Slot|Value|Upgrade|Gold|XP" };
        foreach (dynamic q in quests)
        {
            token.ThrowIfCancellationRequested();
            d.Add($"{q.ID}|{q.Name}|{q.Once}|{q.Slot}|{q.Value}|{q.Upgrade}|{q.Gold}|{q.XP}");
        }

        // Write the fully updated JSON to scripts folder (overwrite)
        string scriptsJsonPath = Path.Combine(ClientFileSources.SkuaScriptsDIR, "QuestData.json");
        File.WriteAllText(scriptsJsonPath, JsonConvert.SerializeObject(quests, Formatting.Indented));

        Core.Logger("Scripts JSON fully updated:");
        Core.Logger($" - {scriptsJsonPath}");
    }

    private void UpdateQuests(CancellationToken token)
    {
        _loaderCTS = CancellationTokenSource.CreateLinkedTokenSource(token);

        IQuestDataLoaderService loader =
            service ??= Ioc.Default.GetRequiredService<IQuestDataLoaderService>();

        // Block until async update finishes, but still cancelable
        loader.UpdateAsync("QuestData.json", false, null, token)
       .GetAwaiter().GetResult();

        File.Copy(
            Path.Combine(ClientFileSources.SkuaDIR, "QuestData.json"),
            Path.Combine(ClientFileSources.SkuaScriptsDIR, "QuestData.json"),
            true
        );


        _loaderCTS.Dispose();
        _loaderCTS = null;
    }

    private CancellationTokenSource? _loaderCTS;
    private IQuestDataLoaderService? service;

}

