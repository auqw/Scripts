//cs_include Scripts/Ultras/CoreUltras.cs

using System;
using System.Dynamic;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraWarden
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreUltras Core = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraWarden";
    public List<IOption> Options = new() { new Option<string>("taunterClass", "Taunter Class", "Insert the name of the class that will taunt", "StoneCrusher"), };


    public void ScriptMain(IScriptInterface bot)
    {
        Core.Boot();

        Kill(taunterClass: Bot.Config.Get<string>("taunterClass"));

        Bot.Stop();
    }

    void Kill(string taunterClass)
    {
        if (Core.HasClassEquipped(taunterClass))
            Core.GetScrollOfEnrage();

        Core.Join("ultrawarden");
        Core.WaitForArmy(3);
        Core.ChooseBestCell("Ultra Warden");
        Core.EnableSkills();

        while (Core.MonsterAlive("Ultra Warden") && !Bot.ShouldExit)
            if (Core.HasClassEquipped(taunterClass))
                Core.UltraWardenTaunter();
            else
                Core.Kill("Ultra Warden");
    }
}


