/*
name: Lavarun
description: This will finish the Lavarun quest.
tags: story, quest, fire-island, lavarun
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/FireIsland/CoreFireIsland.cs

using Skua.Core.Interfaces;

public class Lavarun
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFireIsland FI
    {
        get => _FI ??= new CoreFireIsland();
        set => _FI = value;
    }
    private static CoreFireIsland _FI;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        FI.Lavarun();

        Core.SetOptions(false);
    }
}
