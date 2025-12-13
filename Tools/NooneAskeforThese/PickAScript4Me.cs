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
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string sourceDirectory = Path.Combine(appDataPath, "Skua", "Scripts");

            // Folders to exclude anywhere in the tree
            HashSet<string> excludedFolders = new(StringComparer.OrdinalIgnoreCase)
        {
            "obj", "plugins", "Templates", "Ultras", "Tools", "WIP",
            "Core", "docs", "Army", ".config", ".github", "bin"
        };

            EnumerationOptions options = new()
            {
                RecurseSubdirectories = true,
                IgnoreInaccessible = true,
                AttributesToSkip = FileAttributes.Hidden | FileAttributes.System
            };

            string[] files = Directory.EnumerateFiles(sourceDirectory, "*.cs", options)
                .Where(f =>
                {
                    string directory = Path.GetDirectoryName(f)!;

                    // Exclude any file inside banned folders (at any depth)
                    foreach (string part in directory.Split(Path.DirectorySeparatorChar))
                        if (excludedFolders.Contains(part))
                            return false;

                    string filename = Path.GetFileName(f);

                    // Exclude utility / generator scripts
                    return !filename.StartsWith("Generate", StringComparison.OrdinalIgnoreCase)
                        && !filename.StartsWith("Core", StringComparison.OrdinalIgnoreCase);
                })
                .ToArray();

            Core.Logger("");
            Core.Logger("🎰 ═══════════════════════════════════════ 🎰");
            Core.Logger("🎲 ACTIVATING SCRIPT RANDOMIZER PROTOCOL 🎲");
            Core.Logger("🃏 ═══════════════════════════════════════ 🃏");
            Core.Logger("");

            Core.Logger($"🔍 Discovered {files.Length} viable scripts in the vault...");
            Core.Logger("");

            if (files.Length == 0)
            {
                Core.Logger("⚠️  No valid scripts found after filtering.");
                return;
            }

            Random random = new();

            Core.Logger("🎡 SPINNING THE GREAT WHEEL OF FORTUNE 🎡");
            System.Threading.Thread.Sleep(400);

            for (int i = 0; i < 8; i++)
            {
                string tempFile = files[random.Next(files.Length)];
                Core.Logger($"🎰 {Path.GetFileName(tempFile)}");
                System.Threading.Thread.Sleep(120);
            }

            Core.Logger("");
            Core.Logger("💫 CLICK! 💫");
            Core.Logger("");

            string selectedFile = files[random.Next(files.Length)];

            Core.Logger("🎊 ═══════════════════════════════════════ 🎊");
            Core.Logger($"🏆 JACKPOT! Selected script: {Path.GetFileName(selectedFile)} 🏆");
            Core.Logger("🎉 ═══════════════════════════════════════ 🎉");
        }
        catch (Exception ex)
        {
            Core.Logger($"❌ Error: {ex}");
        }
    }


}