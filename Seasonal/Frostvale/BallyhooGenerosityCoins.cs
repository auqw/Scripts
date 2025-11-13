/*
name: BallyCool's Generosity Coins
description: This will complete the quest that gets you 2 free Generosity Coins.
tags: ballyhoo,bally-hoo,ballycool,generosity,gc,generositycoin,generosity coin,coins, seasonal, frostvale
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Seasonal/Frostvale/ChillysParticipation.cs
using Skua.Core.Interfaces;

public class BallyhooGC
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static ChillysQuest Chillys
    {
        get => _Chillys ??= new ChillysQuest();
        set => _Chillys = value;
    }
    private static ChillysQuest _Chillys;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        GetCoins();

        Core.SetOptions(false);
    }

    public void GetCoins()
    {
        if (Core.isCompletedBefore(10016) || !Core.isSeasonalMapActive("frostvale"))
            return;

        Chillys.ChillysParticipation();
        Core.AddDrop("Generosity Coin");
        Core.EnsureAccept(10016);
        Core.HuntMonsterMapID("battleontown", 1, "Stolen Bag of Coins");
        Core.EnsureComplete(10016);
    }
}
