//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/Dungeons/EclipseAscent/CoreEclipse.cs
//cs_include Scripts/Dungeons/EclipseAscent/CelestialTempleForgeMerge.cs
//cs_include Scripts/Army/CoreArmyLite.cs

using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Options;

public class GreatbladeoftheEntwinedEclipse
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots C => CoreBots.Instance;
    private static CoreAdvanced Adv = new();
    public CoreUltra Ultra = new();
    private static CoreArmyLite sArmy = new();
    public static CoreEclipse coreEclipse
    {
        get => _coreEclipse;
        set => _coreEclipse = value;
    }
    public static CoreEclipse _coreEclipse = new();
    public CelestialTempleForgeMerge templeMerge = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "EclipseAscent";
    public List<IOption> Options = GetOptions();

    public void ScriptMain(IScriptInterface Bot)
    {
        coreEclipse.BotStart();

        templeMerge.BuyAllMerge("Greatblade of the Entwined Eclipse");
        
        coreEclipse.BotStop();
    }

    private static List<IOption> GetOptions()
    {
        var list = new List<IOption>
        {
            CoreBots.Instance.SkipOptions,
        };
        list.AddRange(coreEclipse.Options);
        return list;
    }
}
