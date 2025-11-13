/*
name: Neo Tower
description: This script will complete the Lae's storyline in /neotower.
tags: hollowborn, saga, lae, neotower, neo tower, quest
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Hollowborn/CoreHollowbornStory.cs
using Skua.Core.Interfaces;

public class NeoTower
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreHollowbornStory HB
    {
        get => _HB ??= new CoreHollowbornStory();
        set => _HB = value;
    }
    private static CoreHollowbornStory _HB;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        HB.NeoTower();

        Core.SetOptions(false);
    }
}
