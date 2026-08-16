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

    private const string FileName = "LeaderButlerSyncv2.dll";
    private const string DllUrl = "https://raw.githubusercontent.com/auqw/Scripts/Skua/Tools/Butlerv4/" + FileName;
    private static readonly TimeSpan DownloadTimeout = TimeSpan.FromSeconds(30);

    public void ScriptMain(IScriptInterface bot) => Download();

    public static bool Download()
    {
        Core.Logger($"Checking for {FileName}...");

        string? appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if (string.IsNullOrEmpty(appData))
        {
            Core.Logger("Could not resolve AppData folder. Aborting.", messageBox: true);
            return false;
        }

        string skuaRoot = Path.Combine(appData, "Skua");
        string destDll = Path.Combine(skuaRoot, "plugins", FileName);
        string bundledDll = Path.Combine(skuaRoot, "Scripts", "Tools", "Butlerv4", FileName);

        if (File.Exists(destDll))
        {
            Core.Logger($"{FileName} already present in plugins folder. Nothing to do.");
            return false;
        }

        string pluginsDir = Path.GetDirectoryName(destDll)!;

        try
        {
            Core.Logger($"Ensuring plugins folder exists: {pluginsDir}");
            Directory.CreateDirectory(pluginsDir);
        }
        catch (Exception ex)
        {
            Core.Logger($"Could not create plugins folder: {ex.Message}", messageBox: true);
            return false;
        }

        string tempDll = destDll + ".tmp";

        try
        {
            if (File.Exists(bundledDll))
            {
                Core.Logger($"Found local copy at {bundledDll}. Copying instead of downloading...");
                File.Copy(bundledDll, tempDll, overwrite: true);
                Core.Logger("Copy complete. Verifying...");
            }
            else
            {
                Core.Logger($"No local copy found. Downloading from {DllUrl} ...");
                using var http = new HttpClient { Timeout = DownloadTimeout };
                byte[] data = http.GetByteArrayAsync(DllUrl).GetAwaiter().GetResult();
                Core.Logger($"Download complete ({data.Length:N0} bytes). Verifying...");

                if (!LooksLikeValidDll(data))
                    throw new IOException("Downloaded file is not a valid DLL (bad response or corrupted transfer).");

                File.WriteAllBytes(tempDll, data);
            }

            Core.Logger("Verified OK. Moving into plugins folder...");
            File.Move(tempDll, destDll, overwrite: true);
            Core.Logger($"{FileName} Downloaded successfully. Please restart your clients for Butlerv4 to work.", messageBox: true, stopBot: true);
            return true;
        }
        catch (Exception ex)
        {
            Core.Logger($"Failed to download {FileName}: {ex.Message}", messageBox: true);
            return false;
        }
        finally
        {
            try
            {
                if (File.Exists(tempDll))
                {
                    File.Delete(tempDll);
                    Core.Logger("Cleaned up temp file.");
                }
            }
            catch (Exception ex)
            {
                var cleanupException = new DownloadDLLException("Failed to clean up temp file.", ex);
                Core.Logger(cleanupException.Message, messageBox: true);
            }
        }
    }

    // Cheap sanity check: valid PE files start with the "MZ" DOS header.
    // Catches HTML error pages / empty responses / corrupted transfers
    // before they get installed as a "DLL". Not a security signature check.
    private static bool LooksLikeValidDll(byte[]? data)
        => data is { Length: > 64 } && data[0] == 'M' && data[1] == 'Z';
}

public class DownloadDLLException : System.Exception
{
    public DownloadDLLException() { }
    public DownloadDLLException(string message) : base(message) { }
    public DownloadDLLException(string message, System.Exception inner) : base(message, inner) { }
}