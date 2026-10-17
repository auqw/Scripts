// See https://aka.ms/new-console-template for more information
// DO NOT REMOVE THIS FILE: GitHub Actions runs this program to regenerate scripts.json.
using SkuaScriptsGenerator.Factories;

// Usage: dotnet run --project SkuaScriptsGenerator [json|info]   (defaults to "json")
var writerType = args.Length > 0 ? args[0] : "json";

try
{
    ISkuaScriptFactory factory = new ScriptSkuaFactory();
    factory.Generate(writerType).Write();
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine(ex.Message);
    Environment.ExitCode = 1;
}
