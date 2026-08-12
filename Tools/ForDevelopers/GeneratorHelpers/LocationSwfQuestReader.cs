/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/Tools\ForDevelopers/GeneratorHelpers/Generatesupportutils.cs

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Skua.Core.Interfaces;

/// <summary>Finds quest IDs referenced anywhere in the current location SWF.</summary>
public sealed class LocationSwfQuestReader
{
    private IScriptInterface Bot => IScriptInterface.Instance;

    public sealed record LocationQuestData(
        IReadOnlyList<int> QuestIDs,
        IReadOnlyDictionary<int, IReadOnlyList<int>> MapObjectsByQuest,
        IReadOnlyList<int> ShopIDs
    );

    public IReadOnlyList<int> ReadCurrentMapQuestIDs() => ReadCurrentMap().QuestIDs;

    public LocationQuestData ReadCurrentMap()
    {
        string work = Path.Combine(Path.GetTempPath(), "skua-location-" + Guid.NewGuid().ToString("N"));
        string export = Path.Combine(work, "source");
        Directory.CreateDirectory(work);
        try
        {
            string swf = AcquireSwf(work);
            ExportScripts(swf, export);
            return ExtractQuestData(export);
        }
        finally
        {
            try { Directory.Delete(work, recursive: true); }
            catch { }
        }
    }

    private string AcquireSwf(string work)
    {
        string? cached = Bot.Map.FilePath;
        if (!string.IsNullOrWhiteSpace(cached))
        {
            string local = cached.Split('?')[0];
            if (File.Exists(local))
                return local;
        }

        string file = Bot.Flash.GetGameObject<string>("world.strMapFileName")
            ?? Bot.Map.FileName
            ?? string.Empty;
        file = file.Trim().Trim('"');
        if (string.IsNullOrWhiteSpace(file))
            throw new InvalidOperationException("The current map did not expose strMapFileName or Map.FilePath.");

        List<string> urls = new();
        AddUrl(urls, cached);
        AddUrl(urls, Bot.Flash.GetGameObject<string>("world.ldr_map.contentLoaderInfo.url"));

        // Game3098's getFilePath() returns the directory containing the main game
        // SWF. The game itself loads a location as getFilePath() + "maps/" + file.
        string? gameFilePath = Bot.Flash.GetGameObject<string>("sFilePath");
        if (!string.IsNullOrWhiteSpace(gameFilePath))
            AddUrl(urls, CombineMapUrl(gameFilePath, file));

        AddUrl(urls, file);
        AddUrl(urls, CombineMapUrl("https://game.aq.com/game/gamefiles/", file));
        AddUrl(urls, CombineMapUrl("http://game.aq.com/game/gamefiles/", file));

        string output = Path.Combine(work, "location.swf");
        using HttpClientHandler handler = new()
        {
            AutomaticDecompression = DecompressionMethods.All,
            AllowAutoRedirect = true,
        };
        using HttpClient client = new(handler) { Timeout = TimeSpan.FromSeconds(30) };
        client.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
            "(KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-shockwave-flash"));
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream", 0.9));
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8));
        client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");

        List<string> failures = new();
        foreach (string url in urls)
        {
            try
            {
                using HttpRequestMessage request = new(HttpMethod.Get, url);
                request.Headers.Referrer = GetReferrer(url);
                using HttpResponseMessage response = client.Send(request);
                if (!response.IsSuccessStatusCode)
                {
                    failures.Add($"{url} -> {(int)response.StatusCode} {response.ReasonPhrase}");
                    continue;
                }

                byte[] data = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                if (data.Length < 8 || !IsSwf(data))
                {
                    failures.Add($"{url} -> response was not a SWF ({data.Length} bytes)");
                    continue;
                }

                File.WriteAllBytes(output, data);
                return output;
            }
            catch (Exception ex)
            {
                failures.Add($"{url} -> {ex.Message}");
            }
        }

        throw new InvalidOperationException(
            "Could not acquire the current location SWF. Tried:\n" + string.Join("\n", failures));
    }

    private static void AddUrl(List<string> urls, string? value)
    {
        value = value?.Trim().Trim('"');
        if (string.IsNullOrWhiteSpace(value)
            || !Uri.TryCreate(value, UriKind.Absolute, out Uri? uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            return;
        if (!urls.Contains(value, StringComparer.OrdinalIgnoreCase))
            urls.Add(value);
    }

    private static string CombineMapUrl(string basePath, string file)
    {
        file = file.Trim().Trim('"').Replace('\\', '/');
        if (Uri.TryCreate(file, UriKind.Absolute, out Uri? absolute)
            && (absolute.Scheme == Uri.UriSchemeHttp || absolute.Scheme == Uri.UriSchemeHttps))
            return file;
        int maps = file.IndexOf("maps/", StringComparison.OrdinalIgnoreCase);
        if (maps >= 0)
            file = file[(maps + 5)..];
        return basePath.TrimEnd('/') + "/maps/" + file.TrimStart('/');
    }

    private static Uri? GetReferrer(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
            return null;
        return new Uri(uri.GetLeftPart(UriPartial.Authority) + "/game/");
    }

    private static bool IsSwf(byte[] data) =>
        (data[0] == (byte)'F' || data[0] == (byte)'C' || data[0] == (byte)'Z')
        && data[1] == (byte)'W'
        && data[2] == (byte)'S';

    private static void ExportScripts(string swf, string output)
    {
        string ffdec = Environment.GetEnvironmentVariable("FFDEC_PATH")
            ?? @"C:\Program Files (x86)\FFDec\ffdec-cli.exe";
        if (!File.Exists(ffdec))
            throw new FileNotFoundException("FFDec was not found. Install it or set FFDEC_PATH.", ffdec);

        ProcessStartInfo start = new(ffdec)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        start.ArgumentList.Add("-onerror");
        start.ArgumentList.Add("ignore");
        start.ArgumentList.Add("-export");
        start.ArgumentList.Add("script");
        start.ArgumentList.Add(output);
        start.ArgumentList.Add(swf);
        using Process process = Process.Start(start)
            ?? throw new InvalidOperationException("FFDec could not be started.");
        if (!process.WaitForExit(120000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("FFDec did not finish exporting the location SWF within two minutes.");
        }
        if (process.ExitCode != 0 || !Directory.Exists(output))
            throw new InvalidOperationException($"FFDec script export failed with exit code {process.ExitCode}.");
    }

    private static LocationQuestData ExtractQuestData(string directory)
    {
        var sources = Directory.GetFiles(directory, "*.as", SearchOption.AllDirectories)
            .Select(file => (File: file, Source: File.ReadAllText(file)))
            .ToList();
        HashSet<int> ids = new();
        HashSet<int> shopIDs = new();
        // Quest popups can be instantiated dynamically without an apopLinkage on
        // MainTimeline. Targeted quest API patterns are safe to scan across every
        // exported class and prevent embedded chains from silently disappearing.
        foreach (var entry in sources)
        {
            AddLists(entry.Source, "(?:strQuests|strTurnIns)\\s*=\\s*\"(?<ids>[\\d,\\s]+)\"", ids);
            AddLists(entry.Source, "showQuestList\\s*\\([^,]+,\\s*\"(?<ids>[\\d,\\s]+)\"", ids);
            AddLists(entry.Source, @"showQuests?\s*\(\s*(?<ids>\d+)", ids);
            if (entry.Source.Contains("showQuestList", StringComparison.Ordinal))
                AddLists(entry.Source, "strString\\s*=\\s*\"(?<ids>[\\d,\\s]+)\"", ids);
            foreach (Match shop in Regex.Matches(
                entry.Source,
                @"(?:sendLoadShopRequest|loadShop|showShop|openShop|loadMergeShop)\s*\(\s*(?<id>\d+)|\.(?:intShop|shopID)\s*=\s*(?<id>\d+)",
                RegexOptions.IgnoreCase
            ))
                if (int.TryParse(shop.Groups["id"].Value, out int shopID) && shopID > 0)
                    shopIDs.Add(shopID);

            HashSet<string> shopComponents = Regex.Matches(
                    entry.Source,
                    @"(?<instance>(?:this\.)?\w+)\.strAction\s*=\s*""(?:Item|Merge) Shop""",
                    RegexOptions.IgnoreCase
                )
                .Select(match => match.Groups["instance"].Value)
                .ToHashSet(StringComparer.Ordinal);
            foreach (Match componentID in Regex.Matches(
                entry.Source,
                @"(?<instance>(?:this\.)?\w+)\.intID\s*=\s*(?<id>\d+)",
                RegexOptions.IgnoreCase
            ))
                if (shopComponents.Contains(componentID.Groups["instance"].Value)
                    && int.TryParse(componentID.Groups["id"].Value, out int shopID)
                    // intID 1 is the generated "open the Shops frame" button,
                    // not a loadable shop. Real shop buttons carry their server ID.
                    && shopID > 1)
                    shopIDs.Add(shopID);
        }

        Dictionary<int, HashSet<int>> mapObjects = new();
        Regex questGuard = new(
            @"isQuestInProgress\s*\(\s*(?:int\s*\(\s*)?['""]?(?<quest>\d+)['""]?\s*\)?\s*\)",
            RegexOptions.IgnoreCase
        );
        Regex getMapObject = new(
            @"getMapItem\s*\(\s*(?:int\s*\(\s*)?['""]?(?<object>\d+)['""]?",
            RegexOptions.IgnoreCase
        );
        // Scan every exported class in the location SWF. Some map props are
        // instantiated dynamically and therefore have no static apopLinkage,
        // but their guarded getMapItem code still belongs to this SWF.
        foreach (var entry in sources)
        {
            foreach (Match guard in questGuard.Matches(entry.Source))
            {
                if (!int.TryParse(guard.Groups["quest"].Value, out int questID)
                    || !ids.Contains(questID))
                    continue;
                int open = entry.Source.IndexOf('{', guard.Index + guard.Length);
                if (open < 0 || open - (guard.Index + guard.Length) > 500)
                    continue;
                int close = GeneratorSupportUtils.FindClosingBrace(entry.Source, open);
                if (close < 0)
                    continue;
                string body = entry.Source[open..(close + 1)];
                foreach (Match call in getMapObject.Matches(body))
                {
                    if (!int.TryParse(call.Groups["object"].Value, out int objectID)
                        || objectID <= 0)
                        continue;
                    if (!mapObjects.TryGetValue(questID, out HashSet<int>? objects))
                        mapObjects[questID] = objects = new();
                    objects.Add(objectID);
                }
            }

            // Some location classes keep the quest visibility guard and the
            // clickable getMapItem handlers in separate frame/mouse methods.
            // Pair them only when the entire class references one map quest,
            // which preserves a verifiable, unambiguous association.
            int[] fileQuestIDs = questGuard.Matches(entry.Source)
                .Select(match => int.TryParse(match.Groups["quest"].Value, out int id) ? id : 0)
                .Where(id => id > 0 && ids.Contains(id))
                .Distinct()
                .ToArray();
            if (fileQuestIDs.Length == 1)
            {
                foreach (int objectID in getMapObject.Matches(entry.Source)
                    .Select(match => int.TryParse(match.Groups["object"].Value, out int id) ? id : 0)
                    .Where(id => id > 0))
                {
                    if (!mapObjects.TryGetValue(fileQuestIDs[0], out HashSet<int>? objects))
                        mapObjects[fileQuestIDs[0]] = objects = new();
                    objects.Add(objectID);
                }
            }

            // Timeline clickables declare their map item and quest as component
            // properties instead of calling getMapItem in ActionScript. Pair
            // assignments on the same component instance.
            Dictionary<string, HashSet<int>> componentMapItems = new(StringComparer.Ordinal);
            Dictionary<string, HashSet<int>> componentQuests = new(StringComparer.Ordinal);
            HashSet<string> mapItemComponents = Regex.Matches(
                    entry.Source,
                    @"(?<instance>(?:this\.)?\w+)\.strAction\s*=\s*['""](?:get\s*map\s*item|map\s*item|quest\s*item)['""]",
                    RegexOptions.IgnoreCase
                )
                .Select(match => match.Groups["instance"].Value)
                .ToHashSet(StringComparer.Ordinal);
            foreach (Match assignment in Regex.Matches(
                entry.Source,
                @"(?<instance>(?:this\.)?\w+)\.(?<property>mapItem|intID|intQuest)\s*=\s*(?:int\s*\(\s*)?['""]?(?<value>\d+)['""]?",
                RegexOptions.IgnoreCase
            ))
            {
                if (!int.TryParse(assignment.Groups["value"].Value, out int value) || value <= 0)
                    continue;
                string instance = assignment.Groups["instance"].Value;
                string property = assignment.Groups["property"].Value;
                Dictionary<string, HashSet<int>>? target = property.Equals("intQuest", StringComparison.OrdinalIgnoreCase)
                    ? componentQuests
                    : property.Equals("mapItem", StringComparison.OrdinalIgnoreCase)
                        || (property.Equals("intID", StringComparison.OrdinalIgnoreCase) && mapItemComponents.Contains(instance))
                        ? componentMapItems
                        : null;
                if (target == null)
                    continue;
                // Generated timeline components use mapItem = 1 as an unset
                // placeholder. It is not a server map-item object and must not
                // be counted alongside real getMapItem IDs.
                if (ReferenceEquals(target, componentMapItems) && value <= 1)
                    continue;
                if (!target.TryGetValue(instance, out HashSet<int>? values))
                    target[instance] = values = new();
                values.Add(value);
            }

            foreach ((string instance, HashSet<int> objectIDs) in componentMapItems)
            {
                if (!componentQuests.TryGetValue(instance, out HashSet<int>? questIDs))
                    continue;
                foreach (int questID in questIDs.Where(ids.Contains))
                {
                    foreach (int objectID in objectIDs)
                    {
                        if (!mapObjects.TryGetValue(questID, out HashSet<int>? objects))
                            mapObjects[questID] = objects = new();
                        objects.Add(objectID);
                    }
                }
            }
        }

        return new LocationQuestData(
            ids.Where(id => id > 0).OrderBy(id => id).ToArray(),
            mapObjects.ToDictionary(
                entry => entry.Key,
                entry => (IReadOnlyList<int>)entry.Value.OrderBy(id => id).ToArray()
            ),
            shopIDs.OrderBy(id => id).ToArray()
        );
    }

    private static void AddLists(string source, string pattern, HashSet<int> ids)
    {
        foreach (Match match in Regex.Matches(source, pattern, RegexOptions.IgnoreCase))
            foreach (string value in match.Groups["ids"].Value.Split(','))
                if (int.TryParse(value.Trim(), out int id))
                    ids.Add(id);
    }
}