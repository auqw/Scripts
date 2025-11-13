using SkuaScriptsGenerator.Generators;

namespace SkuaScriptsGenerator.Writers;

public class SkuaScriptsInfoWriter : ISkuaScriptWriter
{
    public void Write()
    {
        foreach (var script in Directory.EnumerateFiles("./", "*.cs", SearchOption.AllDirectories))
        {
            if (script.Contains("SkuaScriptsGenerator"))
                continue;

            Console.WriteLine(script);

            if (File.ReadLines(script).First().StartsWith("/*"))
                continue;

            if (!File.ReadLines(script).First().StartsWith("/*"))
                File.WriteAllText(
                    script,
                    "/*\nname: null\ndescription: null\ntags: null\n*/\n" + File.ReadAllText(script)
                );
        }
    }
}
