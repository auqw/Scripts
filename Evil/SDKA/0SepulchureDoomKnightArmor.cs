/*
name: Sepulchure's DoomKnight Armor (SDKA)
description: This script will get Sepulchure's DoomKnight Armor (SDKA)
tags: sepulchure, sdka, doomknight, armor, evil, member
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class SepulchureDoomKnightArmor
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreSDKA SDKA
    {
        get => _SDKA ??= new CoreSDKA();
        set => _SDKA = value;
    }
    private static CoreSDKA _SDKA;
    public static CoreSDKA sSDKA
    {
        get => _sSDKA ??= new CoreSDKA();
        set => _sSDKA = value;
    }
    public static CoreSDKA _sSDKA;

    public string OptionsStorage = sSDKA.OptionsStorage;
    public bool DontPreconfigure = true;
    public List<IOption> Options = sSDKA.Options;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        SDKA.DoAll(Bot.Config!.Get<SDKAQuest>("SelectedQuest"));

        Core.SetOptions(false);
    }
}
