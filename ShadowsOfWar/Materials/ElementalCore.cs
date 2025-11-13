/*
name: Farms Elemental Core
description: Farms the Elemental Core from Shadows of War
tags: elemental core, shadows of war
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/ShadowsOfWar/CoreSoWMats.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
using Skua.Core.Interfaces;

public class ElementalCore
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreSoWMats SOWM
    {
        get => _SOWM ??= new CoreSoWMats();
        set => _SOWM = value;
    }
    private static CoreSoWMats _SOWM;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.Add("Elemental Core");

        Core.SetOptions();

        SOWM.ElementalCore();

        Core.SetOptions(false);
    }
}
