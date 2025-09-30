/*
name: Best Available Legion Token
description: uses the best pet you have for legion token farming
tags: legion, legion token
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class BestAvailableLegionTokens
{
    public IScriptInterface Bot => IScriptInterface.Instance;

    public CoreBots Core => CoreBots.Instance;
    private static CoreLegion Legion { get => _Legion ??= new CoreLegion(); set => _Legion = value; }    private static CoreLegion _Legion;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Legion.FarmLegionToken();

        Core.SetOptions(false);
    }
}
