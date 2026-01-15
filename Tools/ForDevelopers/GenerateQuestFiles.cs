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

    public void ScriptMain(IScriptInterface bot)
    {
        GenerateQuestFiles();
    }
    private async void GenerateQuestFiles()
    {
        Core.Logger("Starting quest update process...");

        // Step 1: Update the source Quest.txt file using the loader service
        Core.Logger("Step 1: Updating Quests.txt via loader service...");
        try
        {
            await UpdateQuests();
            Core.Logger("Step 1: Quests.txt update completed successfully.");
        }
        catch (Exception ex)
        {
            Core.Logger($"❌ Step 1: Failed to update Quests.txt: {ex.Message}");
            return;
        }

        // Step 2: Verify the updated file exists
        if (!File.Exists(ClientFileSources.SkuaQuestsFile))
        {
            Core.Logger($"❌ Step 2: Quests.txt not found at {ClientFileSources.SkuaQuestsFile}");
            return;
        }
        Core.Logger($"Step 2: Quests.txt verified at {ClientFileSources.SkuaQuestsFile}");

        // Step 3: Copy the updated Quest.txt to QuestData.json
        string questDataJsonPath = Path.Combine(ClientFileSources.SkuaScriptsDIR, "QuestData.json");
        try
        {
            Core.Logger($"Step 3: Copying Quests.txt to {questDataJsonPath}...");
            File.Copy(ClientFileSources.SkuaQuestsFile, questDataJsonPath, true);
            Core.Logger($"Step 3: QuestData.json updated successfully at {questDataJsonPath}");
        }
        catch (Exception ex)
        {
            Core.Logger($"❌ Step 3: Failed to copy QuestData.json: {ex.Message}");
            return;
        }

        // Step 4: Optional sanity check by counting quests in the JSON
        try
        {
            var quests = JsonConvert.DeserializeObject<dynamic[]>(
                File.ReadAllText(questDataJsonPath)
            );
            Core.Logger($"Step 4: Loaded {quests?.Length ?? 0} quests from QuestData.json");
        }
        catch (Exception ex)
        {
            Core.Logger($"❌ Step 4: Failed to read QuestData.json: {ex.Message}");
        }

        Core.Logger("Quest update process completed successfully.");
    }

    private async Task UpdateQuests()
    {
        Core.Logger("UpdateQuests: Initializing loader service...");
        _loaderCTS = new CancellationTokenSource();
        service ??= Ioc.Default.GetRequiredService<IQuestDataLoaderService>();

        try
        {
            Core.Logger("UpdateQuests: Starting async update of Quests.txt...");
            // Force full overwrite to ensure no partial or cached data
            List<QuestData> questData = await service.UpdateAsync(
                "Quests.txt",
                true,  // force overwrite
                null,
                _loaderCTS.Token
            );

            Core.Logger($"UpdateQuests: Loader returned {questData?.Count ?? 0} quests.");
        }
        catch (Exception ex)
        {
            Core.Logger($"❌ UpdateQuests: Exception during update: {ex.Message}");
            throw;
        }
        finally
        {
            Core.Logger("UpdateQuests: Cleaning up loader token...");
            _loaderCTS.Dispose();
            _loaderCTS = null;
        }
    }

    private CancellationTokenSource? _loaderCTS;
    private IQuestDataLoaderService? service;

}
