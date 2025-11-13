/*
name: Elodea Temple Story
description: This will complete the elodea temple Story quest.
tags: story, quest, mogloween, seasonal, elodea, temple, elodeatemple
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/Mogloween/CoreMogloween.cs
using Skua.Core.Interfaces;

public class ElodeaTemple
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreMogloween CoreMogloween
    {
        get => _CoreMogloween ??= new CoreMogloween();
        set => _CoreMogloween = value;
    }
    private static CoreMogloween _CoreMogloween;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        CoreMogloween.ElodeaTemple();

        Core.SetOptions(false);
    }
}
