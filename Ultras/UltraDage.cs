//cs_include Scripts/Ultras/CoreUltras.cs

using System;
using System.Dynamic;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraDage
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreUltras Core = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraDage";
    public List<IOption> Options = new()
    {
        new Option<string>("primaryTaunter", "First Taunter Class", "Insert the name of the class that will taunt", "Chaos Avenger"),
        new Option<string>("secondaryTaunter", "Second Taunter Class", "Insert the name of the class that will taunt", "Legion DoomKnight"),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.Boot();
        Bot.Events.ExtensionPacketReceived += UltraDageListener;

        Kill(primaryTaunter: Bot.Config.Get<string>("primaryTaunter"), secondaryTaunter: Bot.Config.Get<string>("secondaryTaunter"));

        Bot.Events.ExtensionPacketReceived -= UltraDageListener;
        Bot.Stop();
    }

    void Kill(string primaryTaunter, string secondaryTaunter)
    {
        if (Core.HasClassEquipped(primaryTaunter) || Core.HasClassEquipped(secondaryTaunter))
            Core.GetScrollOfEnrage();


        Core.Join("ultradage");
        Core.WaitForArmy(3);
        Core.ChooseBestCell("Dage the Dark Lord");
        Core.EnableSkills();

        while (Core.MonsterAlive("Dage the Dark Lord") && !Bot.ShouldExit)
        {
            if (Core.HasClassEquipped(primaryTaunter))
                Core.TauntCycle(primaryTaunter, "Dage the Dark Lord", "Focus", 250);
            else if (Core.HasClassEquipped(secondaryTaunter))
                Core.TauntCycle(secondaryTaunter, "Dage the Dark Lord", "Focus", 700);
            else
                Core.Kill("Dage the Dark Lord");
        }
    }

    async void UltraDageListener(dynamic packet)
    {
        if (packet?["params"]?.type?.ToString() != "json") return;

        dynamic data = packet["params"].dataObj;
        if (data?.cmd?.ToString() != "event") return;

        string zone = data?.args?.zoneSet?.ToString();

        if (string.Equals(zone, "A", System.StringComparison.OrdinalIgnoreCase))
        {
            Bot.Send.Packet($"%xt%zm%mv%{Bot.Map.RoomID}%122%411%8%");
            return;
        }
        if (string.Equals(zone, "B", System.StringComparison.OrdinalIgnoreCase))
        {
            Bot.Send.Packet($"%xt%zm%mv%{Bot.Map.RoomID}%856%422%8%");

            return;
        }
        if (string.IsNullOrEmpty(zone))
        {
            Bot.Send.Packet($"%xt%zm%mv%{Bot.Map.RoomID}%491%421%8%");

            return;
        }
    }
}


