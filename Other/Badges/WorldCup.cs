/*
name: WorldCup
description: aquires the `world cup` badge from /goal
tags: WorldCup, badge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class WorldCup
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Badge();

        Core.SetOptions(false);
    }

    public void Badge()
    {
        // Bot.Options.LagKiller = false;
        if (Core.HasWebBadge(badge) || !Core.isSeasonalMapActive("goal"))
        {
            Core.Logger($"Already have the {badge} badge, or the map is not available.");
            return;
        }

        Core.EnsureAccept(10774);
        Core.Join("goal-100000");
        Bot.Map.GetMapItem(15884);
        Bot.Wait.ForTrue(() => Bot.TempInv.Contains(45254), 20);
        Core.EnsureComplete(10774);
    }


    private string badge = "World Cup";
}
