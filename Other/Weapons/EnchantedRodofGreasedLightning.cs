/*
name: EnchantedRodofGreasedLightning
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class EnchantedRodofGreasedLightning
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Core.HuntMonster(
            "crashruins",
            "CluckMoo Idol",
            "Enchanted Rod of Greased Lightning",
            isTemp: false
        );

        Core.SetOptions(false);
    }
}
