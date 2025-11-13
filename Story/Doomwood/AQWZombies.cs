/*
name: AQW Zombies
description: This will finish the AQW Zombies quest.
tags: story, quest, doomwood, aqw-zombies
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
using Skua.Core.Interfaces;

public class AQWZombies
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreDoomwood DW
    {
        get => _DW ??= new CoreDoomwood();
        set => _DW = value;
    }
    private static CoreDoomwood _DW;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        DW.AQWZombies();

        Core.SetOptions(false);
    }
}
