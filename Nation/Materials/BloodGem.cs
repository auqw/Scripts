/*
name: BloodGem
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class BloodGem
{
    public CoreBots Core => CoreBots.Instance;
    private static CoreNation Nation { get => _Nation ??= new CoreNation(); set => _Nation = value; }
    private static CoreNation _Nation;


    public string OptionsStorage = "BloodGems";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<HydraLevel>("Hydra Lvl to kill", "HydraLevel", "", HydraLevel.Head_85),
         CoreBots.Instance.SkipOptions
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Nation.FarmBloodGem(100, (int)bot.Config!.Get<HydraLevel>("HydraLevel"));

        Core.SetOptions(false);
    }

    private enum HydraLevel
    {
        Head_85 = 85,
        Head_90 = 90
    }

}
