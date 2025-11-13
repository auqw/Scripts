/*
name: Frostvale (All)
description: This script completes the full Frostvale saga.
tags: saga, story, quest, seasonal, frostval,frostvale,frost,icecave,snowglobe,alpine,snowyvale,icerise,coldwindvalley,battlefield,darkwinter,frozensoul,howardshill,icerisepast,winterhorror,cryostorm,icewindpass,icepike,frostvalpastpresentandfuture,snowview,snowviewrace,deerhunt,bowjangles,glacetomb,fimbultomb,mountotzi,otziwar,holidayhotel,holidayhorror,doall
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Glacera.cs
//cs_include Scripts/Seasonal/Frostvale/Story/CoreFrostvale.cs
using Skua.Core.Interfaces;

public class FrostvaleAll
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFrostvale Frost
    {
        get => _Frost ??= new CoreFrostvale();
        set => _Frost = value;
    }
    private static CoreFrostvale _Frost;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Frost.DoAll();
        Core.SetOptions(false);
    }
}
