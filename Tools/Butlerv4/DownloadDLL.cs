/*
name: Download LeaderButlerSyncv2 DLL
description: Downloads LeaderButlerSyncv2.dll to the Skua plugins folder if missing.
tags: dll, download, butler, plugin
*/

//cs_include Scripts/CoreBots.cs

using System;
using System.IO;
using System.Net.Http;
using Skua.Core.Interfaces;

public class DownloadDLL
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        Download();
        Core.SetOptions(false);
    }

    public static bool Download()
    {
        string destDll = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Skua", "plugins", "LeaderButlerSyncv2.dll"
        );

        if (File.Exists(destDll))
        {
            Core.Logger("LeaderButlerSyncv2.dll already exists in plugins folder.");
            return false;
        }

        const string dllUrl = "https://raw.githubusercontent.com/auqw/Scripts/Skua/Tools/Butlerv4/LeaderButlerSyncv2.dll";

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destDll)!);
            Core.Logger("Downloading LeaderButlerSyncv2.dll...");
            using var http = new HttpClient();
            byte[] data = http.GetByteArrayAsync(dllUrl).GetAwaiter().GetResult();
            File.WriteAllBytes(destDll, data);
            Core.Logger("Please restart your clients for Butlerv4 to work. Because this is your first time downloading the plugin.", messageBox: true, stopBot: true);
            return true;
        }
        catch (Exception ex)
        {
            Core.Logger($"Failed to download LeaderButlerSyncv2.dll: {ex.Message}");
            return false;
        }
    }
}
