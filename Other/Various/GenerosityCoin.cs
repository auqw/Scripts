/*
name: Generosity Coin
description: Donates 250 Acs to get the Generosity Coin.
tags: Generosity, Coin, Generosity Coin, GenerosityCoin, acs, ac
*/
//cs_include Scripts/CoreBots.cs
using Skua.Core.Interfaces;

public class GenerosityCoinDonation
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        GenerosityCoin();

        Core.SetOptions(false);
    }

    public void GenerosityCoin(bool TestMode = false)
    {
        Bot.Wait.ForTrue(() => Bot.Player.Loaded, 20);
        int currentAcs = Bot.Flash.GetGameObject<int>("world.myAvatar.objData.intCoins");
        int StaticACs = currentAcs;
        Core.FarmingLogger("Generosity Coin", 300);

        if (
            !Core.CheckInventory("Generosity Coin", 300)
            && currentAcs >= 250
            && Bot.ShowMessageBox(
                "This Script will do a 1-time Donation of *250* Acs, click \"Yes\" to continue.",
                "Warning!",
                true
            ) == true
        )
        {
            Core.AddDrop("Generosity Coin");
            Core.Logger($"Current Acs: {currentAcs}");
            if (currentAcs - 250 < 250)
                Core.Logger(
                    $"Current Acs: {currentAcs}. You need {250 - currentAcs} more acs to be elegable to donate for the coin.",
                    stopBot: true
                );
            else
            {
                Core.SendPackets($"%xt%zm%cmd%1%giftcoins%250%");
                Core.Sleep();
                Bot.Wait.ForPickup("Generosity Coin");
                currentAcs = Bot.Flash.GetGameObject<int>("world.myAvatar.objData.intCoins");
                Core.Sleep();
                if (currentAcs == StaticACs - 250)
                    Core.Logger(
                        "Donation Successful! Remaining Acs: "
                            + (currentAcs)
                            + ". Remember to come back tomarrow!!"
                    );
                else
                    Core.Logger("Donation Failed!");
            }
        }
    }
}
