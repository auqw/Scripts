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
    private IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;

    public string OptionsStorage = "BloodGems";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<bool>("BloodyChaos", "Do Bloody Chaos", "Thiss will require either an army, or if you're retarded a public group.", false);
        new Option<HydraLevel>("HydraLevel", "Hydra Lvl to kill", "", HydraLevel.Head_85),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        SetupHydras();

        Core.SetOptions(false);
    }

    void SetupHydras()
    {
        int HydraLevel = (int)Bot.Config!.Get<HydraLevel>("HydraLevel");
        if (Bot.Config!.Get<bool>("BloodyChaos"))
            Nation.BloodyChaos(100, false, HydraLevel);
        else
            Nation.FarmBloodGem(100, HydraLevel);
    }

    private enum HydraLevel
    {
        Head_85 = 85,
        Head_90 = 90,
    }
}
