/*
name: Paragon Fiend - Time for Some Spring Cleaning Farm
description: Legion Tokens with a Paragon Fiend Quest Pet.
tags: legion, legion token, paragon fiend, spring cleaning, dage
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Legion/CoreLegion.cs

using Skua.Core.Interfaces;

public class ParagonFiendSpringCleaning
{
    public CoreBots Core => CoreBots.Instance;
    
    private static CoreLegion Legion
    {
        get => _Legion ??= new CoreLegion();
        set => _Legion = value;
    }
    private static CoreLegion _Legion;

    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Legion.LTParagonFiend(FromStandAlone: true);

        Core.SetOptions(false);
    }
}