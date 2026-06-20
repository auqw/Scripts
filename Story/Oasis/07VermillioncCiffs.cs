/*
name: VermillioncCiffs Story
description: This will finish the VermillioncCiffs Story.
tags: story, quest, Crulon Wed, VermillioncCiffs
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/Oasis/CoreOasis.cs
using Skua.Core.Interfaces;

public class VermillioncCiffs
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;

    private static CoreOasis COA
    {
        get => _COA ??= new CoreOasis();
        set => _COA = value;
    }
    private static CoreOasis _COA;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        COA.VermillionCliffs();

        Core.SetOptions(false);
    }


}
