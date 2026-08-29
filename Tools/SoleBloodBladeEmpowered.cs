/*
name: Sole Blood Blade + Empowered Version
description: Completes Sole Blood Blade and optionally obtains the empowered version.
tags: sole blood blade, sole bloodletter of nulgath, empowered sole bloodletter, nulgath, nation
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/Nation/CoreNation.cs

using Skua.Core.Interfaces;
using Skua.Core.Options;

public class SoleBloodBladeEmpowered
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots Core => CoreBots.Instance;
    private static CoreNation Nation => _Nation ??= new CoreNation();
    private static CoreNation _Nation;

    public string OptionsStorage = "SoleBloodBladeEmpowered";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<bool>(
            "EmpoweredVersion",
            "Get Empowered Version?",
            "Also complete Sole Empowered Blood Blade. Requires level 95 and 25 Nulgath Insignias.",
            false
        ),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetSoleBloodletter();

        if (Bot.Config!.Get<bool>("EmpoweredVersion"))
            GetEmpoweredSoleBloodletter();

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

    private void GetEmpoweredSoleBloodletter()
    {
        if (Core.CheckInventory("Empowered Sole Bloodletter"))
            return;

        if (!Core.CheckInventory("Sole Bloodletter of Nulgath"))
        {
            Core.Logger("Sole Bloodletter of Nulgath is required to accept Sole Empowered Blood Blade.");
            return;
        }

        if (Bot.Player.Level < 95)
        {
            Core.Logger("Level 95 is required to accept Sole Empowered Blood Blade.");
            return;
        }

        if (!Core.CheckInventory("Nulgath Insignia", 25))
        {
            Core.Logger("25 Nulgath Insignias are required to complete Sole Empowered Blood Blade.");
            return;
        }

        Core.AddDrop("The Secret 1", "Empowered Sole Bloodletter");
        Core.EnsureAccept(10022);
        Nation.FarmTaintedGem(350);
        Nation.FarmDarkCrystalShard(200);
        Nation.FarmDiamondofNulgath(500);
        Nation.FarmVoucher(false, true);
        Core.HuntMonster("willowcreek", "Hidden Spy", "The Secret 1", isTemp: false);
        Core.EnsureComplete(10022);
        Bot.Wait.ForPickup("Empowered Sole Bloodletter");
    }
}
