/*
name: null
description: null
tags: null
*/

//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreAdvanced2.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Skua.Core.Interfaces;

public class UltraWaitForArmy
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots2 C => CoreBots2.Instance;

    private static UltraWaitForArmy? _instance;
    public static UltraWaitForArmy Instance => _instance ??= new UltraWaitForArmy();

    /// <summary>
    /// Waits for <paramref name="quantity"/> other army members (total = quantity + 1) to signal readiness
    /// via a shared sync file. Once all are ready (or timeout is reached), breaks and does a brief
    /// warmup skill spam to keep the client responsive.
    /// </summary>
    public void WaitForArmySkills(
        int quantity,
        string syncFilePath = "army_sync.sync",
        int bufferTimeMs = 3000,
        int tickMs = 500,
        int timeoutMs = 0
    )
    {
        if (Bot?.Map == null)
            return;

        // --- Resolve safe writable sync path ---
        string FindHome(string path)
        {
            try
            {
                path = Environment.ExpandEnvironmentVariables(path);

                // Allow absolute path if directory exists
                string? dir = Path.GetDirectoryName(path);
                if (Path.IsPathRooted(path) && !string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                    return path;

                // Default to %AppData%\Skua
                string baseDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Skua"
                );
                string full = Path.Combine(baseDir, Path.GetFileName(path));
                Directory.CreateDirectory(baseDir);

                for (int attempt = 0; attempt < 5; attempt++)
                {
                    try
                    {
                        using FileStream fs = new(
                            full,
                            FileMode.OpenOrCreate,
                            FileAccess.Write,
                            FileShare.ReadWrite
                        );
                        return full;
                    }
                    catch (IOException)
                    {
                        Bot?.Sleep(50);
                    }
                }
                return full;
            }
            catch (Exception ex)
            {
                Bot?.Log($"[WaitForArmySkills] Path resolution failed: {ex.Message}");
                return Path.GetFullPath(path);
            }
        }

        // --- File I/O Helpers ---
        string[] Slurp(string path)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    using FileStream fs = new(
                        path,
                        FileMode.OpenOrCreate,
                        FileAccess.Read,
                        FileShare.ReadWrite
                    );
                    using StreamReader sr = new(fs);
                    return sr.ReadToEnd()
                        .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                }
                catch (IOException)
                {
                    Bot?.Sleep(50);
                }
                catch
                {
                    break;
                }
            }
            return Array.Empty<string>();
        }

        void Yeet(string path, string[] lines)
        {
            for (int i = 0; i < 15; i++)
            {
                try
                {
                    File.WriteAllLines(path, lines);
                    return;
                }
                catch (IOException)
                {
                    Bot?.Sleep(50);
                }
                catch
                {
                    return;
                }
            }
        }

        // Each line: username|class:ready:timestamp
        void Poke(string path, string key, bool ready)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            string[] keyParts = key.Split('|');
            if (keyParts.Length < 2 || string.IsNullOrWhiteSpace(keyParts[0]))
                return;

            string username = keyParts[0];
            string className = keyParts[1];

            List<string> lines = Slurp(path).ToList();

            // purge broken historical entries (|Peasant etc.)
            lines.RemoveAll(l => l.StartsWith("|", StringComparison.Ordinal));

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string entry = $"{username}|{className}:{(ready ? 1 : 0)}:{now}";

            int idx = lines.FindIndex(l =>
            {
                string[] parts = l.Split(':');
                if (parts.Length < 1)
                    return false;

                string[] existingKey = parts[0].Split('|');
                return existingKey.Length >= 1 && existingKey[0] == username;
            });

            if (idx >= 0)
                lines[idx] = entry;
            else
                lines.Add(entry);

            Yeet(path, lines.ToArray());
        }


        int HowMany(string path)
        {
            string[] lines = Slurp(path);
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            const int staleThreshold = 600; // 10 minutes
            List<string> valid = new();

            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length < 3)
                    continue;

                string key = parts[0];
                string status = parts[1];
                if (!long.TryParse(parts[2], out long ts))
                    continue;

                if (now - ts <= staleThreshold)
                    valid.Add(line);
            }

            // Rewrite file only if we cleaned something out
            if (valid.Count != lines.Length)
                Yeet(path, valid.ToArray());

            return valid.Count(l => l.Split(':')[1] == "1");
        }

        // --- Initialize sync file ---
        string syncFile = FindHome(syncFilePath);
        try
        {
            string? dir = Path.GetDirectoryName(syncFile);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            if (
                !File.Exists(syncFile)
                || (DateTime.UtcNow - File.GetLastWriteTimeUtc(syncFile)).TotalMinutes > 15
                || Slurp(syncFile).All(l => l.EndsWith(":1"))
            )
                File.WriteAllText(syncFile, "");
        }
        catch (Exception ex)
        {
            Bot?.Log($"[WaitForArmySkills] Sync file setup failed: {ex.Message}");
        }

        string me =
            $"{Bot?.Player?.Username ?? "Nobody"}|{Bot?.Player?.CurrentClass?.Name ?? "Peasant"}".Replace(
                ":",
                "-"
            );
        int need = quantity == 0 ? 1 : Math.Max(1, quantity) + 1;

        Poke(syncFile, me, false);

        Stopwatch clock = Stopwatch.StartNew();
        int lastReady = -1;

        // --- Wait for army readiness ---
        while (!Bot!.ShouldExit)
        {
            int ready = HowMany(syncFile);
            if (ready != lastReady)
            {
                lastReady = ready;
                Bot?.Log($"[WaitForArmySkills] Ready: {ready}/{need}");
            }

            if (ready >= need)
            {
                Bot?.Log("[WaitForArmySkills] All members ready!");
                break;
            }

            Poke(syncFile, me, true);

            if (timeoutMs > 0 && clock.ElapsedMilliseconds >= timeoutMs)
            {
                Bot?.Log("[WaitForArmySkills] Timeout reached — continuing anyway.");
                break;
            }

            Bot?.Sleep(tickMs);
        }

        if (Bot?.ShouldExit == true)
        {
            try
            {
                File.WriteAllText(syncFile, "");
            }
            catch { }
            return;
        }

        // --- Warmup spam to keep clients responsive ---
        DateTime spam = DateTime.UtcNow.AddMilliseconds(2000);
        while (!Bot!.ShouldExit && DateTime.UtcNow < spam)
        {
            Bot.Skills.UseSkill(3);
            Bot?.Sleep(300);
            Bot.Skills.UseSkill(2);
            Bot?.Sleep(300);
            Bot.Skills.UseSkill(1);
            Bot?.Sleep(300);
        }
    }

    /// <summary>
    /// Clears the sync file at start (fresh slate), then waits for the whole army
    /// to signal readiness before proceeding. No timeout — waits for all members indefinitely.
    /// </summary>
    /// <param name="quantity">Number of OTHER army members to wait for (total = quantity + 1).</param>
    /// <param name="syncFilePath">Shared sync file path.</param>
    /// <param name="tickMs">Milliseconds between readiness checks.</param>
    /// <param name="staleThresholdSec">
    /// Entries older than this many seconds are ignored.
    /// 0 or negative = no age check (all entries counted).
    /// </param>
    public void NewWaitForArmy(
        int quantity,
        string syncFilePath = "army_sync.sync",
        int tickMs = 500,
        int staleThresholdSec = 0,
        bool useSkill = false
    )
    {
        if (Bot?.Map == null)
            return;

        // --- Resolve safe writable sync path ---
        string FindHome(string path)
        {
            try
            {
                path = Environment.ExpandEnvironmentVariables(path);

                string? dir = Path.GetDirectoryName(path);
                if (Path.IsPathRooted(path) && !string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                    return path;

                string baseDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Skua"
                );
                string full = Path.Combine(baseDir, Path.GetFileName(path));
                Directory.CreateDirectory(baseDir);

                for (int attempt = 0; attempt < 5; attempt++)
                {
                    try
                    {
                        using FileStream fs = new(
                            full,
                            FileMode.OpenOrCreate,
                            FileAccess.Write,
                            FileShare.ReadWrite
                        );
                        return full;
                    }
                    catch (IOException)
                    {
                        Bot?.Sleep(50);
                    }
                }
                return full;
            }
            catch (Exception ex)
            {
                Bot?.Log($"[NewWaitForArmy] Path resolution failed: {ex.Message}");
                return Path.GetFullPath(path);
            }
        }

        // --- File I/O Helpers ---
        string[] Slurp(string path)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    using FileStream fs = new(
                        path,
                        FileMode.OpenOrCreate,
                        FileAccess.Read,
                        FileShare.ReadWrite
                    );
                    using StreamReader sr = new(fs);
                    return sr.ReadToEnd()
                        .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                }
                catch (IOException)
                {
                    Bot?.Sleep(50);
                }
                catch
                {
                    break;
                }
            }
            return Array.Empty<string>();
        }

        void Yeet(string path, string[] lines)
        {
            for (int i = 0; i < 15; i++)
            {
                try
                {
                    File.WriteAllLines(path, lines);
                    return;
                }
                catch (IOException)
                {
                    Bot?.Sleep(50);
                }
                catch
                {
                    return;
                }
            }
        }

        void Poke(string path, string key, bool ready)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            string[] keyParts = key.Split('|');
            if (keyParts.Length < 2 || string.IsNullOrWhiteSpace(keyParts[0]))
                return;

            string username = keyParts[0];
            string className = keyParts[1];

            List<string> lines = Slurp(path).ToList();
            lines.RemoveAll(l => l.StartsWith("|", StringComparison.Ordinal));

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string entry = $"{username}|{className}:{(ready ? 1 : 0)}:{now}";

            int idx = lines.FindIndex(l =>
            {
                string[] parts = l.Split(':');
                if (parts.Length < 1)
                    return false;

                string[] existingKey = parts[0].Split('|');
                return existingKey.Length >= 1 && existingKey[0] == username;
            });

            if (idx >= 0)
                lines[idx] = entry;
            else
                lines.Add(entry);

            Yeet(path, lines.ToArray());
        }

        int HowMany(string path, int staleSec)
        {
            string[] lines = Slurp(path);
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            List<string> valid = new();

            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length < 3)
                    continue;

                string key = parts[0];
                string status = parts[1];
                if (!long.TryParse(parts[2], out long ts))
                    continue;

                if (staleSec > 0 && now - ts > staleSec)
                    continue;

                valid.Add(line);
            }

            if (valid.Count != lines.Length)
                Yeet(path, valid.ToArray());

            return valid.Count(l => l.Split(':')[1] == "1");
        }

        // --- Fresh slate: clear the file ---
        string syncFile = FindHome(syncFilePath);
        try
        {
            string? dir = Path.GetDirectoryName(syncFile);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(syncFile, "");
        }
        catch (Exception ex)
        {
            Bot?.Log($"[NewWaitForArmy] Sync file setup failed: {ex.Message}");
        }

        string me =
            $"{Bot?.Player?.Username ?? "Nobody"}|{Bot?.Player?.CurrentClass?.Name ?? "Peasant"}".Replace(
                ":",
                "-"
            );
        int need = quantity == 0 ? 1 : Math.Max(1, quantity) + 1;

        // Write self as ready immediately
        Poke(syncFile, me, true);

        int lastReady = -1;

        // --- Wait for army readiness ---
        while (!Bot!.ShouldExit)
        {
            int ready = HowMany(syncFile, staleThresholdSec);
            if (ready != lastReady)
            {
                lastReady = ready;
                Bot?.Log($"[NewWaitForArmy] Ready: {ready}/{need}");
            }

            if (ready >= need)
            {
                Bot?.Log("[NewWaitForArmy] All members ready!");
                break;
            }

            // Re-poke to keep our entry fresh
            Poke(syncFile, me, true);

            Bot?.Sleep(tickMs);
        }

        if (Bot?.ShouldExit == true)
        {
            try
            {
                File.WriteAllText(syncFile, "");
            }
            catch { }
            return;
        }

        // --- Brief pause to let army members breathe ---
        DateTime waitTill = DateTime.UtcNow.AddMilliseconds(2000);
        while (!Bot!.ShouldExit && DateTime.UtcNow < waitTill)
        {
            if (useSkill)
            {
                Bot.Skills.UseSkill(3);
                Bot?.Sleep(300);
                Bot.Skills.UseSkill(2);
                Bot?.Sleep(300);
                Bot.Skills.UseSkill(1);
                Bot?.Sleep(300);
            }
            else
                Bot?.Sleep(500);
        }
    }
}
