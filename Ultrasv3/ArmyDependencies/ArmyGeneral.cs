//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreUltrav3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraGeneral.cs
//cs_include Scripts/CoreBots.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Skua.Core.Interfaces;

public class ArmyGeneral
{
    public ArmyGeneral() { }

    public static void PrepareSyncFiles(
        CoreUltrav3 ultra,
        string auraSyncFile,
        IEnumerable<string> essenceNames,
        string essenceSyncPrefix
    )
    {
        if (ultra == null || string.IsNullOrWhiteSpace(auraSyncFile) || essenceNames == null)
            return;

        try
        {
            ultra.ClearSyncFile(ultra.ResolveSyncPath(auraSyncFile));
        }
        catch { }

        foreach (var essence in essenceNames.Where(e => !string.IsNullOrWhiteSpace(e)))
        {
            try
            {
                ultra.ClearSyncFile(ultra.ResolveSyncPath(GetSyncFileForItem(essenceSyncPrefix, essence)));
            }
            catch { }
        }
    }

    public static string GetSyncFileForItem(string prefix, string itemName)
    {
        if (string.IsNullOrWhiteSpace(prefix) || string.IsNullOrWhiteSpace(itemName))
            return string.Empty;

        string safeName = NormalizeToFileName(itemName);
        return prefix.EndsWith(".sync", StringComparison.OrdinalIgnoreCase)
            ? prefix.Replace(".sync", $"_{safeName}.sync")
            : $"{prefix}_{safeName}.sync";
    }

    public static void PrepareClassSync(CoreUltrav3 ultra, IScriptInterface bot, int armySize, string syncFilePath)
    {
        if (ultra == null || bot == null || armySize < 1 || string.IsNullOrWhiteSpace(syncFilePath))
            return;

        UltraGeneral.EquipUltraDailyPresetClassesSafe(ultra, bot, syncFilePath, armySize);
    }

    public static void PrepareStageSync(CoreUltrav3 ultra, string syncFilePath)
    {
        if (ultra == null || string.IsNullOrWhiteSpace(syncFilePath))
            return;

        try
        {
            ultra.ClearSyncFile(ultra.ResolveSyncPath(syncFilePath));
        }
        catch { }
    }

    public static bool IsFullArmyActive(CoreUltrav3 ultra, string syncFilePath, int armySize)
    {
        if (ultra == null || string.IsNullOrWhiteSpace(syncFilePath) || armySize < 1)
            return false;

        string syncFile = ultra.ResolveSyncPath(syncFilePath);
        string[] lines = ultra.ReadLines(syncFile);
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        const int staleThreshold = 600;
        int activeMembers = 0;

        foreach (string line in lines)
        {
            string[] parts = line.Split(':');
            if (parts.Length < 5)
                continue;

            if (!long.TryParse(parts[^1], out long ts))
                continue;

            if (now - ts > staleThreshold)
                continue;

            activeMembers++;
        }

        return activeMembers >= armySize;
    }

    public static int PublishAndGetArmyStage(CoreUltrav3 ultra, IScriptInterface bot, int ownStage, string syncFilePath, int armySize)
    {
        if (ultra == null || bot == null || string.IsNullOrWhiteSpace(syncFilePath) || armySize < 1)
            return ownStage;

        string syncFile = ultra.ResolveSyncPath(syncFilePath);
        string username = bot.Player.Username;
        if (string.IsNullOrWhiteSpace(username))
            return ownStage;

        string key = username.Replace(":", "-");
        ultra.UpdateEntry(syncFile, key, ownStage.ToString());

        string[] lines = ultra.ReadLines(syncFile);
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        const int staleThreshold = 600;
        int minStage = ownStage;
        int activeMembers = 0;

        foreach (string line in lines)
        {
            string[] parts = line.Split(':');
            if (parts.Length < 3)
                continue;

            if (!int.TryParse(parts[1], out int stage))
                continue;
            if (!long.TryParse(parts[^1], out long ts))
                continue;

            if (now - ts > staleThreshold)
                continue;

            activeMembers++;
            minStage = Math.Min(minStage, stage);
        }

        if (activeMembers < armySize)
        {
            // Not all army members have published their current stage yet.
            // Stay on the earliest possible stage until the full army is registered.
            minStage = 0;
        }

        return minStage;
    }

    private static string NormalizeToFileName(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var cleaned = new List<char>(input.Length);
        foreach (char c in input)
        {
            if (char.IsLetterOrDigit(c) || c == '_' || c == '-')
                cleaned.Add(c);
            else if (char.IsWhiteSpace(c) || c == '.' || c == ',' || c == '\'' || c == '"')
                cleaned.Add('_');
        }

        string result = new string(cleaned.ToArray()).Trim('_');
        return string.IsNullOrEmpty(result) ? "item" : result;
    }
}
