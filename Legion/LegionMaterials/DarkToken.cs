/*
name: Dark Token
description: This script will farm Dark Tokens.
tags: darktoken, lf3, legion fealty 3, lr, seraphic medals, seraphic, dark token
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Story/Legion/WorldSoul.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class DarkToken
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreLegion Legion
    {
        get => _Legion ??= new CoreLegion();
        set => _Legion = value;
    }
    private static CoreLegion _Legion;
    private static WorldSoul WS
    {
        get => _WS ??= new WorldSoul();
        set => _WS = value;
    }
    private static WorldSoul _WS;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        FarmDarkToken();

        Core.SetOptions(false);
    }

    public void FarmDarkToken()
    {
        WS.WorldSoulQuests();
        Legion.DarkToken();
    }
}
