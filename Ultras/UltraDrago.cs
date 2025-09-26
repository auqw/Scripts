//cs_include Scripts/WIP/CoreUltras.cs

using System;
using System.Dynamic;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraDrago
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreUltras Core = new();

    public void ScriptMain(IScriptInterface bot)
    {
        Core.Boot();

        Kill(primaryTaunter: "ArchPaladin", secondaryTaunter: "Lord Of Order");

        Bot.Stop();
    }

    void Kill(string primaryTaunter, string secondaryTaunter)
    {
        if (Core.HasClassEquipped(primaryTaunter) || Core.HasClassEquipped(secondaryTaunter))
            Core.GetScrollOfEnrage();

        Core.Join("ultradrago");
        Core.WaitForArmy(3);
        Core.ChooseBestCell("King Drago");
        Core.EnableSkills();

        while (Core.MonsterAlive("King Drago") && !Bot.ShouldExit)
            if (Core.HasClassEquipped(primaryTaunter) || Core.HasClassEquipped(secondaryTaunter))
            {
                while (Core.MonsterAlive("Executioner Dene") && !Bot.ShouldExit)
                {
                    if (Core.HasClassEquipped(primaryTaunter))
                        Core.TauntCycle(primaryTaunter, "Executioner Dene", "Focus", 250);
                    else if (Core.HasClassEquipped(secondaryTaunter))
                        Core.TauntCycle(secondaryTaunter, "Executioner Dene", "Focus", 700);
                }
            }
            else
                Core.KillWithPriority("King Drago", "Bowmaster Algie", "Executioner Dene");
    }
}
