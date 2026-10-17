using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using SkuaScriptsGenerator.Writers;

namespace SkuaScriptsGenerator.Generators;

/// <summary>
/// Scans the repository for .cs scripts that carry a metadata header block and emits
/// the scripts.json index consumed by the Skua client (printed to stdout so CI can
/// redirect it into the file).
/// </summary>
public partial class SkuaScriptsJsonWriter : ISkuaScriptWriter
{
    /// <summary>
    /// Base URL of the raw GitHub content for the branch this repository publishes from.
    /// Override with the SCRIPTS_RAW_URL environment variable if the repo/branch changes.
    /// </summary>
    private static readonly string RawScriptsUrl =
        Environment.GetEnvironmentVariable("SCRIPTS_RAW_URL")
        ?? "https://raw.githubusercontent.com/auqw/Scripts/Skua/";

    public void Write()
    {
        var scripts = new List<ScriptInfo>();

        foreach (var script in SkuaScriptsInfoWriter.EnumerateScriptFiles())
        {
            try
            {
                // Read the file once and reuse the bytes/content for both metadata and hashing.
                var rawBytes = File.ReadAllBytes(script);
                var content = RemoveZeroWidthChars(Encoding.UTF8.GetString(rawBytes));

                if (!content.StartsWith("/*", StringComparison.Ordinal))
                    continue;

                var info = ParseHeader(content, out var headerLength);
                info.Path = script.Replace("./", "");
                info.FileName = GetFileName(script);
                info.DownloadUrl = RawScriptsUrl + info.Path;

                // Size/hash must reflect exactly what the client will download (header included,
                // zero-width chars stripped), so hash the cleaned full content.
                var cleanBytes = Encoding.UTF8.GetBytes(content);
                info.Size = cleanBytes.Length;
                info.Sha256 = ComputeSha256(cleanBytes);

                scripts.Add(info);
            }
            catch (Exception ex)
            {
                // Never let one bad file break the whole index; report on stderr so stdout
                // (the JSON itself) stays clean for the CI redirection.
                Console.Error.WriteLine($"[json] Failed to process '{script}': {ex.Message}");
            }
        }

        // Deterministic ordering keeps the generated file stable across runs (smaller diffs).
        scripts.Sort(static (a, b) => string.CompareOrdinal(a.Path, b.Path));

        var json = JsonConvert.SerializeObject(scripts, Formatting.Indented);
        Console.WriteLine(json);
    }

    /// <summary>
    /// Parses the leading /* ... */ comment block into a <see cref="ScriptInfo"/>.
    /// </summary>
    private static ScriptInfo ParseHeader(string content, out int headerLength)
    {
        var info = new ScriptInfo();
        var endIdx = content.IndexOf("*/", StringComparison.Ordinal);
        headerLength = endIdx < 0 ? content.Length : endIdx + 2;

        var lines = content[..headerLength].Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("/*") || trimmed.StartsWith("*/"))
                continue;

            // Split only on the first ':' so values may contain colons (e.g. URLs/times).
            var sep = trimmed.IndexOf(':');
            if (sep <= 0)
                continue;

            var key = trimmed[..sep].Trim();
            var value = trimmed[(sep + 1)..].Trim();

            switch (key)
            {
                case "name":
                    info.Name = NormalizeNull(value);
                    break;
                case "description":
                    info.Description = NormalizeNull(value);
                    break;
                case "tags":
                    info.Tags = ParseTags(value);
                    break;
                case "version":
                    info.Version = NormalizeNull(value);
                    break;
            }
        }

        return info;
    }

    /// <summary>
    /// The literal string "null" in a header means "not set" - keep the JSON honest.
    /// </summary>
    private static string? NormalizeNull(string value) =>
        string.Equals(value, "null", StringComparison.OrdinalIgnoreCase) ? null : value;

    private static string[]? ParseTags(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || string.Equals(value, "null", StringComparison.OrdinalIgnoreCase))
            return null;

        var tags = value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(NormalizeTag)
            .Where(t => !string.IsNullOrEmpty(t))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        return tags.Length > 0 ? tags : null;
    }

    /// <summary>
    /// Keeps an all-uppercase tag as-is (acronyms like "AQW"), lowercases everything else,
    /// as the original generator did.
    /// </summary>
    private static string NormalizeTag(string tag) =>
        tag.All(char.IsUpper) ? tag : tag.ToLowerInvariant();

    private static string GetFileName(string path)
    {
        var idx = path.LastIndexOf('/');
        return idx < 0 ? path : path[(idx + 1)..];
    }

    [GeneratedRegex("\u200B|\uFEFF")]
    private static partial Regex ZeroWidthRegex();

    private static string RemoveZeroWidthChars(string text) => ZeroWidthRegex().Replace(text, "");

    private static string ComputeSha256(byte[] data)
    {
        var hash = SHA256.HashData(data);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private class ScriptInfo
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("tags")]
        public string[]? Tags { get; set; }

        [JsonProperty("path")]
        public string? Path { get; set; }

        [JsonProperty("size")]
        public int? Size { get; set; }

        [JsonProperty("sha256")]
        public string? Sha256 { get; set; }

        [JsonProperty("fileName")]
        public string? FileName { get; set; }

        [JsonProperty("downloadUrl")]
        public string? DownloadUrl { get; set; }

        // Optional extra: preserved when present, omitted otherwise (NullValueHandling).
        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public string? Version { get; set; }
    }
}
