/*
name: First Observatory
description: This will finish the First Observatory quest.
tags: story, quest, elegy-of-madness, darkon, first-observatory
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs
using Skua.Core.Interfaces;

public class CompleteFirstObservatory
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreAstravia Astravia
    {
        get => _Astravia ??= new CoreAstravia();
        set => _Astravia = value;
    }
    private static CoreAstravia _Astravia;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Astravia.FirstObservatory();

        Core.SetOptions(false);
    }
}
