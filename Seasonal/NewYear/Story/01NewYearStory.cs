/*
name: New Year Story
description: This will complete the New Year story.
tags: story, quest, seasonal, new-year
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/NewYear/CoreNewYear.cs
using Skua.Core.Interfaces;

public class NewYear
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreNewYear NY
    {
        get => _NY ??= new CoreNewYear();
        set => _NY = value;
    }
    private static CoreNewYear _NY;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        NY.NewYear();

        Core.SetOptions(false);
    }
}
