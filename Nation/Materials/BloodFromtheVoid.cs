/*
name: Blood From the Void
description: Farms Blood From the Void from `Obey Yourself, or be Commanded` in /tercesinvasion
tags: tercesinvasion, Jadzia, Blood From the Void, Nulgath Saga, Nulgath Merge, Nulgath Birthday
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Story/Nation/TercesInvasion.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class BloodFromTheVoid
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static TercesInvasion TI
    {
        get => _TI ??= new TercesInvasion();
        set => _TI = value;
    }
    private static TercesInvasion _TI;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        _TI.StoryLine();
        Nation.BloodFromTheVoid();

        Core.SetOptions(false);
    }


}
