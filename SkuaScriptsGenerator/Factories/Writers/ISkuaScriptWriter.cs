namespace SkuaScriptsGenerator.Generators;

/// <summary>
/// Produces the output of a generator (JSON index, console info, etc.).
/// </summary>
public interface ISkuaScriptWriter
{
    /// <summary>
    /// Runs the generation and writes the result to its destination.
    /// </summary>
    void Write();
}
