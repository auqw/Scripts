/*
name: MoglinPunter
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class MoglinPunter
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    bool Datagood = false;
    bool Finished = false;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Badge();

        Core.SetOptions(false);
    }

    public void Badge()
    {
        Bot.Options.LagKiller = false;
        if (Core.HasWebBadge(badge) || !Core.isSeasonalMapActive("punt"))
        {
            Core.Logger($"Already have the {badge} badge, or the map is not available.");
            return;
        }
        Core.OneTimeMessage(
            "Minigame Explanation",
            "This minigame works off of a \"value\" system for ponts, so 9999 is 99, for the quest so youll need to get a value of 10000 points which may take a while.",
            forcedMessageBox: true
        );

        int Punt = 0;

        Core.Logger($"Doing quest for {badge} badge, Purely Rng based, good luck");
        Core.Join("zorbakpunt");
        Bot.Events.ExtensionPacketReceived += puntingPacketReader;
        while (!Bot.ShouldExit && !Core.HasWebBadge(badge))
        {
            Datagood = false;
            Core.Sleep();
            Core.SendPackets("%xt%zm%ia%1%rval%btnPuntting%%");
            Bot.Wait.ForCellChange("Punt");
            Bot.Wait.ForTrue(() => Datagood, 5);

            if (Finished || Core.CheckInventory(68214))
            {
                Bot.Wait.ForDrop(68214);
                Bot.Wait.ForPickup(68214);

                Core.ChainComplete(8532);
                Core.Logger($"Punts to get the badge: {Punt}");
                break;
            }
            Core.Jump("Enter", "Spawn");
        }
        Bot.Events.ExtensionPacketReceived -= puntingPacketReader;

        void puntingPacketReader(dynamic packet)
        {
            string type = packet["params"].type;
            dynamic data = packet["params"].dataObj;
            if (type is not null and "json")
            {
                string cmd = data.cmd.ToString();
                switch (cmd)
                {
                    case "ia":
                        if (
                            data.oName.ToString() == "btnPuntting"
                            && data.unm.ToString() == Core.Username()
                        )
                        {
                            Datagood = true;
                            double score = data.val;
                            double RoundedScore = Math.Round(
                                float.Parse($"{score.ToString()[..^2]}.{score.ToString()[^2..]}")
                            );

                            Core.Logger(
                                $"Punt [#{Punt++}] | Score [{score} (Rounded Score [{RoundedScore}])], \n"
                                    + $"Win? ({(RoundedScore < 100 ? "❌" : "✅")})"
                            );

                            if (RoundedScore == 100)
                            {
                                Bot.Events.ExtensionPacketReceived -= puntingPacketReader;
                                Finished = true;
                            }
                        }
                        break;
                }
            }
        }
    }

    private string badge = "Moglin Punter";
}
