/*
name: test potion
description: Tests UltraPotions functionality
tags: test,potion
*/

//cs_include Scripts/Ultras-v2/Dependencies/CoreEngine.cs
//cs_include Scripts/Ultras-v2/Dependencies/CoreUltra.cs
//cs_include Scripts/Ultras-v2/Dependencies/UltraPotions.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs

using Skua.Core.Interfaces;

public class TestPotion
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;

    private static UltraPotions Pots
    {
        get => _Pots ??= new UltraPotions();
        set => _Pots = value;
    }
    private static UltraPotions _Pots;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        // Test potion buying + usage
        Pots.UseRecommendedPotions();

        Core.Logger("UltraPotions test complete.");

        Core.SetOptions(false);
    }
}