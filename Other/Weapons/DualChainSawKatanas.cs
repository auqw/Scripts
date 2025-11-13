/*
name: DualChainSawKatanas
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class DualChainSawKatanas
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

        GetWep();

        Core.SetOptions(false);
    }

    public void GetWep()
    {
        if (Core.CheckInventory("Dual Chainsaw Katanas", toInv: false))
            return;

        Core.EnsureAccept(8670);
        Core.EquipClass(ClassType.Farm);
        Core.KillMonster("darkoviahorde", "r8", "Right", "Zombie", "Zombie Defeated", 100);
        Core.EnsureComplete(8670);
        Core.JumpWait();
        Core.Sleep();
        Core.SetAchievement(10);
        Core.Sleep();
        Core.BuyItem("Darkoviahorde", 1171, "Dual Chainsaw Katanas");
    }
}
