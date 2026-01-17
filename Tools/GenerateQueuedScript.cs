/*
name: Generate Queued Script
description: Pick multiple scripts that you want to run in sequence of one another, and the script will generate a new file that does exactly those bots for you
tags: generator, queud, script, follow-up, choose
*/
//cs_include Scripts/CoreBots.cs
using System.IO;
using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.ViewModels;

public class GenQueueScript
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface Bot)
    {
        GenerateQueuedScript();
    }



    public void GenerateQueuedScript()
    {
        IFileDialogService fileDialog = Ioc.Default.GetRequiredService<IFileDialogService>();

        List<string[]> scripts = new();
        List<string> scriptNames = new();
        HashSet<string> csIncludes = new();
        Dictionary<string, List<string>> sharedHelpers = new();
        List<(string ClassName, bool UsesBotParam)> scriptCalls = new();

        // Collect scripts
        while (!Bot.ShouldExit)
        {
            string? path = fileDialog.OpenFile(ClientFileSources.SkuaScriptsDIR, "Skua Script (*.cs)|*.cs");
            if (path == null) break;

            // Normalize include path to Scripts/... structure
            string includePath = Path.GetRelativePath(ClientFileSources.SkuaScriptsDIR, path)
                                     .Replace('\\', '/');
            if (!includePath.StartsWith("Scripts/"))
                includePath = "Scripts/" + includePath;


            if (includePath.Contains("Core", StringComparison.OrdinalIgnoreCase))
            {
                Bot.ShowMessageBox($"[{includePath}] is a Core file and cannot be queued.", "Invalid Script");
                continue;
            }

            string[] file = File.ReadAllLines(path);

            if (file.Any(l => l.Contains("public List<IOption>")))
            {
                Bot.ShowMessageBox($"[{includePath}] contains Script Options and cannot be queued.", "Unsupported Script");
                continue;
            }

            scripts.Add(file);
            scriptNames.Add(includePath);
            csIncludes.Add("//cs_include " + includePath);

            if (Bot.ShowMessageBox("Add another script?", "Script added", true) != true)
                break;
        }

        // Process each script + included scripts
        foreach (string[] script in scripts)
        {
            // Pull all cs_includes in the script
            foreach (string line in script
                .SkipWhile(l => !l.StartsWith("//cs_include"))
                .TakeWhile(l => l.StartsWith("//cs_include")))
            {
                csIncludes.Add(line);
            }

            // Extract class name
            string className = script.First(l => l.Contains("public class"))
                                     .Split("public class ").Last().Trim();

            bool usesBotParam = script.Any(l => l.Contains("public void ScriptMain(") &&
                                                l.Contains("IScriptInterface"));

            scriptCalls.Add((className, usesBotParam));

            // Extract static helpers
            ExtractHelpersFromScript(script, sharedHelpers);
        }


        string botName = GetBotName();

        List<string> output = new();
        output.AddRange(csIncludes.OrderBy(s => s));
        output.Add("using Skua.Core.Interfaces;");
        output.Add("using System.Threading.Tasks;");
        output.Add("");
        output.Add($"public class Generated_{botName}");
        output.Add("{");
        output.Add("    private IScriptInterface Bot => IScriptInterface.Instance;");
        output.Add("    public CoreBots Core => CoreBots.Instance;");

        // emit shared helpers
        foreach (List<string> helper in sharedHelpers.Values)
        {
            output.Add("");
            output.AddRange(helper);
        }

        // emit script instances
        foreach ((string ClassName, _) in scriptCalls)
            output.Add($"    private {ClassName} {ClassName} = new();");

        output.Add("");
        output.Add("    public void ScriptMain(IScriptInterface Bot)");
        output.Add("    {");
        output.Add("        Core.SetOptions();");
        output.Add("");

        foreach ((string ClassName, bool UsesBotParam) call in scriptCalls)
        {
            if (call.UsesBotParam)
                output.Add($"        if (!Bot.ShouldExit) {call.ClassName}.ScriptMain(Bot);");
            else
                output.Add($"        if (!Bot.ShouldExit) {call.ClassName}.ScriptMain();");
        }

        output.Add("");
        output.Add("        Core.SetOptions(false);");
        output.Add("    }");
        output.Add("}");

        string outDir = Path.Combine(ClientFileSources.SkuaScriptsDIR, "Generated");
        if (!Directory.Exists(outDir))
            Directory.CreateDirectory(outDir);

        Core.WriteFile(Path.Combine(outDir, $"{botName}.cs"), output);

        Bot.ShowMessageBox(
            $"Generated:\n- Scripts/Generated/{botName}.cs\n\nOrder:\n- {string.Join("\n- ", scriptNames)}",
            "Queue Generated"
        );
    }

    /// <summary>
    /// Extracts static helpers from a script and adds to sharedHelpers dictionary.
    /// </summary>
    private void ExtractHelpersFromScript(string[] script, Dictionary<string, List<string>> sharedHelpers)
    {
        for (int i = 0; i < script.Length; i++)
        {
            string line = script[i].Trim();

            // Stop at ScriptMain
            if (line.StartsWith("public void ScriptMain")) break;

            if (!line.StartsWith("private static") || !line.EndsWith("{")) continue;
            if (line.Contains(" _")) continue; // skip backing fields

            List<string> helper = new() { "    " + line };

            int braceDepth = 0;
            bool entered = false;

            for (int j = i + 1; j < script.Length; j++)
            {
                string l = script[j].Trim();

                if (l.Contains("{"))
                {
                    braceDepth++;
                    entered = true;
                }

                helper.Add("    " + l);

                if (l.Contains("}"))
                {
                    braceDepth--;
                    if (entered && braceDepth <= 0)
                    {
                        // optional backing field after
                        if (j + 1 < script.Length)
                        {
                            string next = script[j + 1].Trim();
                            if (next.StartsWith("private static") && next.Contains(" _"))
                                helper.Add("    " + next);
                        }
                        i = j;
                        break;
                    }
                }
            }

            string helperType = line.Split("static")[1].Trim().Split(' ')[0];
            if (!sharedHelpers.ContainsKey(helperType))
                sharedHelpers[helperType] = helper;
        }
    }

    private string GetBotName()
    {
        InputDialogViewModel diag = new(
            "Name the bot",
            "What is the name you wish to give the bot. (case-sensitive)",
            false
        );

        if (Ioc.Default.GetRequiredService<IDialogService>().ShowDialog(diag) != true)
            throw new OperationCanceledException();

        return RemoveInvalidChar(diag.DialogTextInput);
    }

    private string RemoveInvalidChar(string input)
    {
        string result = "";
        foreach (char c in input)
            if (char.IsLetterOrDigit(c))
                result += c;
        return result;
    }


}
