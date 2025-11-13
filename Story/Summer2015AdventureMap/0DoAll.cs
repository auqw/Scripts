/*
name: (Summer) Complete Summer 2015 Adventure Map Story
description: This will finish the Summer 2015 Adventure Map story.
tags: summer, 2015, adventure, map, farm, story, complete, summer, 2015, adventure, map, all
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/Summer2015AdventureMap/CoreSummer.cs
using Skua.Core.Interfaces;

public class DoAllSummer
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreSummer Summer
    {
        get => _Summer ??= new CoreSummer();
        set => _Summer = value;
    }
    private static CoreSummer _Summer;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Summer.DoAll();

        Core.SetOptions(false);
    }
}
