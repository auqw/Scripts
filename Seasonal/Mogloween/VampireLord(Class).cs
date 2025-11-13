/*
name: (Class) Vampire Lord
description: This will farm the Blood Moon Token for the Vampire Lord class.
tags: class, mogloween, seasonal, vampire, vampire lord
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class VampireLord
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
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

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.Add("Blood Moon Token");
        Core.SetOptions();

        GetClass(true);

        Core.SetOptions(false);
    }

    public void GetClass(bool rankUpClass = false)
    {
        if (!Core.isSeasonalMapActive("mogloween"))
            return;
        if (Core.CheckInventory(41575, toInv: false))
            return;

        Core.FarmingLogger("Blood Moon Token", 300);
        Core.AddDrop("Blood Moon Token");
        Core.EquipClass(ClassType.Solo);

        while (!Bot.ShouldExit && !Core.CheckInventory("Blood Moon Token", 300))
        {
            Core.EnsureAccept(Core.IsMember ? 6060 : 6059);

            //farm 33x turn-in quants
            Core.KillMonster(
                "bloodmoon",
                "r12a",
                "Left",
                "Black Unicorn",
                "Black Blood Vial",
                99,
                isTemp: false
            );
            Core.KillMonster(
                "bloodmoon",
                "r4a",
                "Left",
                "Lycan Guard",
                "Moon Stone",
                33,
                isTemp: false
            );

            //turning x33
            Core.EnsureCompleteMulti(Core.IsMember ? 6060 : 6059);
            Bot.Wait.ForPickup("Blood Moon Token");
            if (Bot.Inventory.Contains("Blood Moon Token", 300))
                break;
        }
        Core.BuyItem("mogloween", 1477, "Vampire Lord", shopItemID: 5459);

        if (rankUpClass)
            Adv.RankUpClass("Vampire Lord");
    }
}
