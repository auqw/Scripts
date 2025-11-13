/*
name: Weapon Reflection
description: Farms "Weapon Reflection" From Quest: "Dual Wield Forge".
tags: weapon reflection, dual wield forge, golden 8th birthday candle
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class DualWield
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
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreDailies Daily
    {
        get => _Daily ??= new CoreDailies();
        set => _Daily = value;
    }
    private static CoreDailies _Daily;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        WeaponReflection();

        Core.SetOptions(false);
    }

    public void WeaponReflection(int quant = 200)
    {
        if (Core.CheckInventory("Weapon Reflection", quant))
            return;

        Core.Logger("Checking if Your Acc is 8 Years Old");

        Core.BuyItem(Bot.Map.Name, 1317, "Golden 8th Birthday Candle");
        if (!Core.CheckInventory("Golden 8th Birthday Candle"))
        {
            Core.Logger("your acc isn't old enough.");
            return;
        }

        Core.AddDrop("Weapon Reflection");
        while (!Bot.ShouldExit && (!Core.CheckInventory("Weapon Reflection", quant)))
        {
            Core.EnsureAccept(5518);
            Core.HuntMonster("nostalgiaquest", "Skeletal Viking", "Reflected Glory", 5);
            Core.HuntMonster("nostalgiaquest", "Skeletal Warrior", "Divided Light", 5);
            Core.EnsureComplete(5518);
            Bot.Wait.ForPickup("Weapon Reflection");
        }
    }
}
