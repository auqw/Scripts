//cs_include Scripts/Ultras/CoreUltras.cs

using System;
using System.Dynamic;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraNulgath
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreUltras Core = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraNulgath";
    public List<IOption> Options = new()
    {
        new Option<string>("primaryTaunter", "First Taunter Class", "Insert the name of the class that will taunt", "ArchPaladin"),
        new Option<string>("secondaryTaunter", "Second Taunter Class", "Insert the name of the class that will taunt", "Lord Of Order"),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.Boot();

        Kill(primaryTaunter: Bot.Config.Get<string>("primaryTaunter"), secondaryTaunter: Bot.Config.Get<string>("secondaryTaunter"));

        Bot.Stop();
    }

    void Kill(string primaryTaunter, string secondaryTaunter)
    {
        if (Core.HasClassEquipped(primaryTaunter) || Core.HasClassEquipped(secondaryTaunter))
            Core.GetScrollOfEnrage();

        Core.Join("ultranulgath");
        Core.WaitForArmy(3);
        Core.ChooseBestCell("Nulgath the Archfiend");
        Core.EnableSkills();

        while (Core.MonsterAlive("Nulgath the Archfiend") && !Bot.ShouldExit)
            if (Core.HasClassEquipped(primaryTaunter))
                Core.TauntCycle(primaryTaunter, "Nulgath the Archfiend", "Focus", 250);
            else if (Core.HasClassEquipped(secondaryTaunter))
                Core.TauntCycle(secondaryTaunter, "Nulgath the Archfiend", "Focus", 700);
            else
                Core.KillWithPriority("Nulgath the Archfiend", "Overfiend Blade");

    }
}


