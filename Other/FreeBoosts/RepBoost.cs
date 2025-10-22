/*
name: RepBoost
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class RepBoost
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "XREPUTATION Boost! (10 min)", "Fishing Dynamite" });
        Core.SetOptions();

        Farm.GetBoost("REP", 9999, true);

        Core.SetOptions(false);
    }
}
