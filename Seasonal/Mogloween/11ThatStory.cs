/*
name: That Story
description: This will complete the That story quest.
tags: story, quest, mogloween, seasonal, that
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/Mogloween/CoreMogloween.cs
using Skua.Core.Interfaces;

public class That
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreMogloween CoreMogloween
    {
        get => _CoreMogloween ??= new CoreMogloween();
        set => _CoreMogloween = value;
    }
    private static CoreMogloween _CoreMogloween;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        CoreMogloween.That();

        Core.SetOptions(false);
    }
}
