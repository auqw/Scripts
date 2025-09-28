//cs_include Scripts/Ultras/CoreUltras.cs

using System;
using System.Dynamic;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class TestZone
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreUltras Core = new();

    public void ScriptMain(IScriptInterface bot)
    {
        Core.Boot();

        Test();

        Bot.Stop();
    }

    void Test()
    {
        Core.Join("j6");
        Core.WaitForArmy(3);
        Core.Join("battleunderb");
    }
}
