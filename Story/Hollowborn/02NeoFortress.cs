/*
name: Neo Fortress
description: This script will complete the storyline in /neofortress.
tags: hollowborn, saga, trygve, neofortress, lae
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Hollowborn/CoreHollowbornStory.cs
using Skua.Core.Interfaces;

public class NeoFortress
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

        HB.NeoFortress();

        Core.SetOptions(false);
    }
}
