/*
name: Complete Isle Of Fotia Story
description: This will complete the Isle Of Fotia story.
tags: story, quest, isle-of-fotia, complete, all
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/IsleOfFotia/CoreIsleOfFotia.cs
using Skua.Core.Interfaces;

public class DoAllIsleOfFotia
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreIsleOfFotia CoreIsleOfFotia
    {
        get => _CoreIsleOfFotia ??= new CoreIsleOfFotia();
        set => _CoreIsleOfFotia = value;
    }
    private static CoreIsleOfFotia _CoreIsleOfFotia;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        CoreIsleOfFotia.CompleteALL();

        Core.SetOptions(false);
    }
}
