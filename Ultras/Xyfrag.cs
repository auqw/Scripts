//cs_include Scripts/Ultras/CoreUltras.cs

using System;
using System.Dynamic;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class Xyfrag
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreUltras Core = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "Xyfrag";
    public List<IOption> Options = new() { new Option<string>("taunterClass", "Taunter Class", "Insert the name of the class that will taunt", "Legion Revenant"), };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.Boot();

        Kill(taunterClass: Bot.Config.Get<string>("taunterClass"));

        Bot.Stop();
    }

    void Kill(string taunterClass)
    {
        if (Core.HasClassEquipped(taunterClass))
        {
            Core.GetScrollOfEnrage();
            Bot.Events.ExtensionPacketReceived += Core.ChargeListener;
        }

        Core.Join("voidxyfrag");
        Core.WaitForArmy(6);
        Core.ChooseBestCell("Xyfrag");
        Core.EnableSkills();

        while (Core.MonsterAlive("Xyfrag") && !Bot.ShouldExit)
        {
            if (Core.HasClassEquipped(taunterClass))
                Core.TauntCharge(taunterClass, "Xyfrag", "Focus", 250);
            else
                Core.Kill("Xyfrag");
        }
    }
}

