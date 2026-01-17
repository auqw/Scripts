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

        Core.Logger("Updating Quest.txt");
        UpdateQuests(token); // blocks but can be cancelled

        token.ThrowIfCancellationRequested();

        Core.Logger("Reading Quest.txt");
        dynamic[] quests = JsonConvert.DeserializeObject<dynamic[]>(
            File.ReadAllText(ClientFileSources.SkuaQuestsFile)
        )!;

        List<string> d = new() { "ID|Name|Once|Slot|Value|Upgrade|Gold|XP" };

        foreach (dynamic q in quests)
        {
            token.ThrowIfCancellationRequested();
            d.Add($"{q.ID}|{q.Name}|{q.Once}|{q.Slot}|{q.Value}|{q.Upgrade}|{q.Gold}|{q.XP}");
        }

        token.ThrowIfCancellationRequested();

        File.Copy(
            ClientFileSources.SkuaQuestsFile,
            Path.Combine(ClientFileSources.SkuaScriptsDIR, "QuestData.json"),
            true
        );

        Core.Logger("Files Updated: Scripts/QuestData.json");
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

