/*
name: TheDarkDeal
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Nation/OblivionBlade(RareandNot)/CoreOblivionBladeofNulgath.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class TheDarkDeal
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreOblivionBladeofNulgath COBoN
    {
        get => _COBoN ??= new CoreOblivionBladeofNulgath();
        set => _COBoN = value;
    }
    private static CoreOblivionBladeofNulgath _COBoN;

    public string OptionsStorage = "TheDarkDeal";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        CoreBots.Instance.SkipOptions,
        new Option<int>("RelicQuant", "Relic Quant", "Relic of Chaos quant", 00),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        COBoN.TheDarkDeal(Bot.Config?.Get<int>("RelicQuant") ?? default(int));

        Core.SetOptions(false);
    }
}
