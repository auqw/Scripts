/*
name: Dark Spirit Orbs
description: This script will farm 10500 Dark Spirit Orbs.
tags: dso, a penny for your foughts, sdka, evil, quest, farm
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class DarkSpiritOrbs
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreSDKA SDKA
    {
        get => _SDKA ??= new CoreSDKA();
        set => _SDKA = value;
    }
    private static CoreSDKA _SDKA;
    public string OptionsStorage = SDKA.OptionsStorage;
    public bool DontPreconfigure = true;
    public List<IOption> Options = SDKA.Options;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        SDKA.FarmDSO(10500);

        Core.SetOptions(false);
    }
}
