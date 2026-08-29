/*
name: Sole Blood Blade
description: Completes the Sole Blood Blade quest to obtain Sole Bloodletter of Nulgath.
tags: sole blood blade, sole bloodletter of nulgath, nulgath, nation
*/
//cs_include Scripts/CoreBots.cs

using Skua.Core.Interfaces;

public class SoleBloodBlade
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetSoleBloodletter();

        Core.SetOptions(false);
    }

    private void GetSoleBloodletter()
    {
        if (Core.CheckInventory("Sole Bloodletter of Nulgath"))
            return;

        if (!Core.CheckInventory("Bloodletter of Nulgath"))
        {
            Core.Logger("Bloodletter of Nulgath is required to accept Sole Blood Blade.");
            return;
        }

        Core.AddDrop("The Secret 1", "Sole Bloodletter of Nulgath");
        Core.EnsureAccept(10021);
        Core.HuntMonster("willowcreek", "Hidden Spy", "The Secret 1", isTemp: false);
        Core.EnsureComplete(10021);
        Bot.Wait.ForPickup("Sole Bloodletter of Nulgath");
    }
}
