using System.Text;
using SkuaScriptsGenerator.Generators;

namespace SkuaScriptsGenerator.Writers;

/// <summary>
/// Ensures every .cs script in the repository carries a metadata header block.
/// Files without a header get a placeholder one so they keep being picked up by
/// <see cref="Generators.SkuaScriptsJsonWriter"/>.
/// </summary>
public class SkuaScriptsInfoWriter : ISkuaScriptWriter
{
    private const string PlaceholderHeader = "/*\nname: null\ndescription: null\ntags: null\n*/\n";

    public void Write()
    {
        var addedCount = 0;

        foreach (var script in EnumerateScriptFiles())
        {
            Console.WriteLine(script);

            // Read once and reuse, instead of hitting the disk three times per file.
            var content = File.ReadAllText(script);

            if (HasHeader(content))
                continue;

            // Preserve the original encoding/BOM behavior of the previous implementation
            // (headers are prepended as UTF-8 text).
            File.WriteAllText(script, PlaceholderHeader + content);
            addedCount++;
        }

        Console.Error.WriteLine($"[info] Header check finished. Headers added: {addedCount}.");
    }

    /// <summary>
    /// A file is considered to have a header when its first line starts with "/*"
    /// (after an optional UTF-8 BOM), matching what the JSON writer expects.
    /// </summary>
    internal static bool HasHeader(string content) =>
        content.StartsWith("/*", StringComparison.Ordinal)
        || (content.Length > 2 && content[0] == '\uFEFF' && content.AsSpan(1).StartsWith("/*", StringComparison.Ordinal));

    /// <summary>
    /// Shared enumeration of script files, skipping the generator project itself.
    /// Paths are normalized to '/' separators for stable output across platforms.
    /// </summary>
    internal static IEnumerable<string> EnumerateScriptFiles()
    {
        foreach (var script in Directory.EnumerateFiles("./", "*.cs", SearchOption.AllDirectories))
        {
            var normalized = script.Replace('\\', '/');
            if (normalized.Contains("SkuaScriptsGenerator", StringComparison.OrdinalIgnoreCase))
                continue;

            yield return normalized;
        }
    }
}
