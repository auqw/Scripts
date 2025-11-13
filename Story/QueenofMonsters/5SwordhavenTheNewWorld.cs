/*
name: Swordhaven The New World
description: This will finish the Swordhaven The New World quest.
tags: story, quest, queen-of-monsters, swordhaven-the-new-world
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Story/QueenofMonsters/CoreQoM.cs

using Skua.Core.Interfaces;

public class CompleteTheNewWorld
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
    private static CoreQOM QOM
    {
        get => _QOM ??= new CoreQOM();
        set => _QOM = value;
    }
    private static CoreQOM _QOM;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        QOM.SwordhavenTheNewWorld();

        Core.SetOptions(false);
    }
}
