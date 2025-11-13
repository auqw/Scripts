/*
name: Battle Bunny Plushies
description: This will finish the Cheep Chase quest to obtain the Battle Bunny Plushies.
tags: battle-bunny-plushies, seasonal, easter,battle bunny,plushie,plushies,battle bunny plushie,valencia,grenwog
*/
//cs_include Scripts/CoreBots.cs
using Skua.Core.Interfaces;

public class BattleBunnyPlushies
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetBBP();

        Core.SetOptions(false);
    }

    public void GetBBP()
    {
        if (Core.CheckInventory(Core.QuestRewards(10202)) || !Core.isSeasonalMapActive("grenwog"))
            return;

        Core.AddDrop(Core.QuestRewards(10202));

        // Cheep Chase [10202]
        Core.EnsureAccept(10202);
        Core.GetMapItem(14388, map: "swordhavenbridge");
        Core.GetMapItem(14389, map: "deathgazer");
        Core.GetMapItem(14390, map: "somnia");
        Core.GetMapItem(14391, map: "siegefortress");
        Core.EnsureComplete(10202);
    }
}
