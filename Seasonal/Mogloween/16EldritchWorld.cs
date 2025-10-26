/*
name: Eldritch World Story
description: This will complete the eldritch world Story quest.
tags: story, quest, mogloween, seasonal, eldritch, world, eldritchworld, elodea
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/Mogloween/CoreMogloween.cs
using Skua.Core.Interfaces;

public class EldritchWorld
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreMogloween CoreMogloween { get => _CoreMogloween ??= new CoreMogloween(); set => _CoreMogloween = value; }
    private static CoreMogloween _CoreMogloween;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        CoreMogloween.EldritchWorld();

        Core.SetOptions(false);
    }

}
