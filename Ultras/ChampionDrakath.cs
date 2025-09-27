//cs_include Scripts/Ultras/CoreUltras.cs

using System;
using System.Dynamic;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class ChampionDrakath
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreUltras Core = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "ChampionDrakath";
    public List<IOption> Options = new() { new Option<string>("taunterClass", "Taunter Class", "Insert the name of the class that will taunt", "ArchPaladin"), };

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

        Core.Join("championdrakath");
        Core.WaitForArmy(3);
        Core.ChooseBestCell("Champion Drakath");
        Core.EnableSkills();

        while (Core.MonsterAlive("Champion Drakath") && !Bot.ShouldExit)
        {
            if (Core.HasClassEquipped(taunterClass))
                Core.DrakathTaunter();
            else
                Core.Kill("Champion Drakath");
        }
    }
}

