/*
name: Dragons Of Yokai
description: This script will complete "Dragons Of Yokai" storyline.
tags: story, quest, saga, dragons, dragon, yokai, haku, village, pirate, treasure, war, portal, complete, all
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/DragonsOfYokai/CoreDOY.cs
using Skua.Core.Interfaces;

public class DragonsOfYokai
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

        DOY.DoAll();
        Core.SetOptions(false);
    }
}
