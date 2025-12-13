/*
name: PickRandomScript
description: This class provides functionality to randomly select one non-utility script from subdirectories in the Scripts folder. It excludes specific folders and file prefixes, then logs the selected script.
tags: file management, random selection, script picker
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;

public class PickRandomScript
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface Bot)
    {
        PickScript();
    }

    public void PickScript()
    {
        try
        {
            // Get all script files from subdirectories only (not directly in Scripts folder)
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string sourceDirectory = Path.Combine(appDataPath, "Skua", "Scripts");

            // Excluded folders
            string[] excludedFolders = { "obj", "plugins", "Templates", "Ultras", "Tools", "WIP", "Core", "docs", "Army", ".config", ".github", "bin" };

            // Get immediate subdirectories only
            string[] subdirectories = Directory.GetDirectories(sourceDirectory)
                .Where(d => !excludedFolders.Contains(Path.GetFileName(d)))
                .ToArray();

            // Get all files from immediate subdirectories only (not recursive)
            string[] files = subdirectories
                .SelectMany(d => Directory.GetFiles(d))
                .Where(f =>
                {
                    string filename = Path.GetFileName(f);
                    return !filename.StartsWith("Generate", StringComparison.OrdinalIgnoreCase);
                })
                .ToArray();

            // Log the total number of non-core files
            Core.Logger("");
            Core.Logger("🎰 ═══════════════════════════════════════ 🎰");
            Core.Logger("🎲 ACTIVATING SCRIPT RANDOMIZER PROTOCOL 🎲");
            Core.Logger("🃏 ═══════════════════════════════════════ 🃏");
            Core.Logger("");
            Core.Logger("💰 Spinning the wheel of fate...");
            System.Threading.Thread.Sleep(500);
            Core.Logger("🎯 Calibrating quantum randomness matrices...");
            System.Threading.Thread.Sleep(300);
            Core.Logger("✨ Harvesting cosmic entropy from the void...");
            System.Threading.Thread.Sleep(400);
            Core.Logger("🎪 Rolling the RNG dice of destiny...");
            Core.Logger("");
            Core.Logger($"🔍 Discovered {files.Length} viable scripts in the vault...");
            Core.Logger("");

            // Check if any non-core files exist
            if (files.Length == 0)
            {
                Core.Logger("⚠️  No non-core scripts found in the source directory.");
                return;
            }

            // Randomly select one file
            Core.Logger("🎡 SPINNING THE GREAT WHEEL OF FORTUNE 🎡");
            System.Threading.Thread.Sleep(600);
            Core.Logger("⚡ bzzzzzzzzzzzzzzzz ⚡");
            System.Threading.Thread.Sleep(400);
            Core.Logger("💫 CLICK! 💫");
            Core.Logger("");
            
            Random random = new();
            string selectedFile = files[random.Next(files.Length)];
            string selectedFileName = Path.GetFileName(selectedFile);

            // Log the selected script with fanfare
            Core.Logger("🎊 ═══════════════════════════════════════ 🎊");
            Core.Logger($"🏆 JACKPOT! Selected script: {selectedFileName} 🏆");
            Core.Logger("🎉 ═══════════════════════════════════════ 🎉");
        }
        catch (Exception ex)
        {
            // Log any exceptions
            Core.Logger($"An error occurred: {ex.Message}");
        }
    }
}