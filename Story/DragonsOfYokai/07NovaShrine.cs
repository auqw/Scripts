/*
name: Nova Shrine
description: This script will complete "Nova Shrine" storyline.
tags: story, quest, saga, dragons, dragon, nova, shrine, yokairealm, complete, all
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/DragonsOfYokai/CoreDOY.cs
using Skua.Core.Interfaces;

public class NovaShrine
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreDOY DOY
    {
        get => _DOY ??= new CoreDOY();
        set => _DOY = value;
    }
    private static CoreDOY _DOY;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        DOY.NovaShrine();
        Core.SetOptions(false);
    }
}
