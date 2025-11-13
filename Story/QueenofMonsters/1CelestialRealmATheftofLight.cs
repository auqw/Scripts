/*
name: Complete Celestial Realm
description: This will complete the Celestial Realm story arc.
tags: story, quest, queen-of-monsters, celestial-realm-at-theft-of-light
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Story/QueenofMonsters/CoreQoM.cs
using Skua.Core.Interfaces;

public class CompleteCelestialRealm
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

        QOM.aTheftofLight();

        Core.SetOptions(false);
    }
}
