/*
name: TwillyPunt
description: null
tags: TwillyPunt
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class MoglinPunt
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

    // 100 for /punt
    // 13 for /zorbakpunt
    private double RequiredPuntScore = 100; // Default; can be changed per year or event

    public void Badge()
    {
        // Bot.Options.LagKiller = false;
        if (Core.HasWebBadge(badge) || !Core.isSeasonalMapActive("punt"))
        {
            Core.Logger($"Already have the {badge} badge, or the map is not available.");
            return;
        }

        // Core.OneTimeMessage(
        //     "Minigame Explanation",
        //     "This minigame works off of a \"value\" system for ponts, so 9999 is 99, for the quest so youll need to get a value of 10000 points which may take a while.",
        //     forcedMessageBox: true
        // );

        // int Punt = 0;

        // Core.Logger($"Doing quest for {badge} badge, Purely Rng based, good luck");
        // Always private
        Core.EnsureAccept(8532);
        Core.GetMapItem(9714, 1, "punt-100000");
        Core.EnsureComplete(8532);
        // Bot.Events.ExtensionPacketReceived += puntingPacketReader;
        // while (!Bot.ShouldExit && !Core.HasWebBadge(badge))
        // {
        //     Datagood = false;
        //     Core.Sleep();
        // Core.SendPackets("%xt%zm%ia%1%rval%btnPuntting%%");
        // Bot.Wait.ForCellChange("Punt");
        // Bot.Sleep(1500);
        // Bot.Wait.ForTrue(() => Datagood, 20);
        // if (Finished || Bot.TempInv.Contains(68214))
        // {
        //     Bot.Wait.ForTrue(() => Bot.TempInv.Contains(68214), 20);
        //     Bot.Wait.ForPickup(68214);

        //     Core.ChainComplete(8532);
        //     Core.Logger($"Punts to get the badge: {Punt}");
        //     break;
        // }
        // Core.Jump("Enter", "Spawn");
        // }
        // Bot.Events.ExtensionPacketReceived -= puntingPacketReader;
    }

    void puntingPacketReader(dynamic packet)
    {
        const double EPSILON = 0.0001; // Tolerance for floating-point comparisons

        string type = packet["params"].type;
        dynamic data = packet["params"].dataObj;
        if (type is not null and "json")
        {
            string cmd = data.cmd.ToString();
            if (cmd == "ia"
                && data.oName.ToString() == "btnPuntting"
                && data.unm.ToString() == Core.Username())
            {
                Datagood = true;

                // Score comes as integer representing hundredths (e.g., 3489 = 34.89)
                double score = (double)data.val / 100.0;

                bool win = Math.Abs(score - RequiredPuntScore) < EPSILON; // Compare to configurable score

                Core.Logger(
                    $"Punt [#{Punt++}] | Score [{score:F2}], "
                    + $"Win: [{(win ? "Yes" : "No")}]"
                );

                if (win)
                {
                    Bot.Events.ExtensionPacketReceived -= puntingPacketReader;
                    Finished = true;
                }
            }
        }
    }

    private string badge = "Moglin Punter";
}
