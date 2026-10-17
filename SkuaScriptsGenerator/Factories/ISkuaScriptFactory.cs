using SkuaScriptsGenerator.Generators;

namespace SkuaScriptsGenerator.Factories;

/// <summary>
/// Creates the <see cref="ISkuaScriptWriter"/> that matches the requested output type.
/// </summary>
public interface ISkuaScriptFactory
{
    /// <summary>
    /// Creates a writer for the given type ("json" or "info").
    /// </summary>
    /// <param name="type">The writer type identifier.</param>
    /// <returns>A writer instance ready to run.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> is not a known writer type.</exception>
    ISkuaScriptWriter Generate(string type);
}
