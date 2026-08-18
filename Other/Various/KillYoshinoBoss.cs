/*
name: KillYoshinoBoss
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class KillYoshinoBoss
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Yoshino();
        Core.SetOptions(false);
    }

    public void Yoshino()
    {
        if (!Bot.Quests.IsAvailable(5720))
            return;

        Core.EquipClass(ClassType.Solo);


        Core.AddDrop("Limited Event Coin");
        Core.RegisterQuests(5720);
        Core.KillMonster("yoshino", "r2", "Right", "*", "Limited Event Monster Proof");
        Core.JumpWait();
        Farm.ToggleBoost(BoostType.Gold);
        Core.Sleep();
        Core.EnsureComplete(5720);
        Bot.Wait.ForPickup("Limited Event Coin");
        Farm.ToggleBoost(BoostType.Gold, false);
    }
}
