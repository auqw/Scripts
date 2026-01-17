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
            // Run GenerateQuestFiles and wait for completion
            // blocking call to async UpdateAsync
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

        Core.Logger("Updating Quest.txt...");
        UpdateQuests(token); // blocks but cancelable

        token.ThrowIfCancellationRequested();

        // 1️⃣ The authoritative TXT (updated by UpdateQuests)
        string questsTxtPath = Path.Combine(ClientFileSources.SkuaDIR, "Quests.txt");

        // 2️⃣ The JSON outputs
        string clientJsonPath = Path.Combine(ClientFileSources.SkuaDIR, "QuestData.json");         // client copy
        string scriptsJsonPath = Path.Combine(ClientFileSources.SkuaScriptsDIR, "QuestData.json"); // scripts folder copy

        dynamic[] quests;

        try
        {
            Core.Logger("Reading updated QuestData.txt...");
            string jsonText = File.ReadAllText(questsTxtPath);
            quests = JsonConvert.DeserializeObject<dynamic[]>(jsonText)!;
            Core.Logger($"Loaded {quests.Length} quests."); // should show 10559
        }
        catch (Exception ex)
        {
            Core.Logger("Failed to read or parse QuestData: " + ex.Message);
            return;
        }

        token.ThrowIfCancellationRequested();

        // Build pipe-delimited data if needed
        List<string> d = new() { "ID|Name|Once|Slot|Value|Upgrade|Gold|XP" };
        foreach (dynamic q in quests)
        {
            token.ThrowIfCancellationRequested();
            d.Add($"{q.ID}|{q.Name}|{q.Once}|{q.Slot}|{q.Value}|{q.Upgrade}|{q.Gold}|{q.XP}");
        }

        // 1️⃣ Write JSON to client-side folder
        File.WriteAllText(clientJsonPath, JsonConvert.SerializeObject(quests, Formatting.Indented));

        // 2️⃣ Write JSON to scripts folder
        File.WriteAllText(scriptsJsonPath, JsonConvert.SerializeObject(quests, Formatting.Indented));

        Core.Logger("Files Updated: ");
        Core.Logger($" - Client: {clientJsonPath}");
        Core.Logger($" - Scripts: {scriptsJsonPath}");
    }

    private void UpdateQuests(CancellationToken token)
    {
        _loaderCTS = CancellationTokenSource.CreateLinkedTokenSource(token);

        IQuestDataLoaderService loader =
            service ??= Ioc.Default.GetRequiredService<IQuestDataLoaderService>();

        // Block until async update finishes, but still cancelable
        loader.UpdateAsync("Quests.txt", false, null, _loaderCTS.Token)
              .GetAwaiter().GetResult();

        _loaderCTS.Dispose();
        _loaderCTS = null;
    }



    private CancellationTokenSource? _loaderCTS;
    private IQuestDataLoaderService? service;
}

