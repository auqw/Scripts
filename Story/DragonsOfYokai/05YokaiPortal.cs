/*
name: Yokai Portal
description: This script will complete "Yokai Portal" storyline.
tags: story, quest, saga, dragons, dragon, yokai, portal, yokaiportal, complete, all
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/DragonsOfYokai/CoreDOY.cs
using Skua.Core.Interfaces;

public class YokaiPortal
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

        DOY.YokaiPortal();
        Core.SetOptions(false);
    }
}
