using SkuaScriptsGenerator.Generators;
using SkuaScriptsGenerator.Writers;

namespace SkuaScriptsGenerator.Factories;

/// <summary>
/// Default <see cref="ISkuaScriptFactory"/> implementation.
/// </summary>
public class ScriptSkuaFactory : ISkuaScriptFactory
{
    /// <inheritdoc/>
    public ISkuaScriptWriter Generate(string type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.Trim().ToLowerInvariant() switch
        {
            "json" => new SkuaScriptsJsonWriter(),
            "info" => new SkuaScriptsInfoWriter(),
            _ => throw new ArgumentException(
                $"Invalid writer type '{type}'. Valid types: 'json', 'info'.",
                nameof(type)
            ),
        };
    }
}
