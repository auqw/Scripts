/*
name: Berserker Bunny Armor
description: This will finish the quest to obtain the Berserker Bunny Armor.
tags: berserker-bunny-armor, seasonal, easter, BBA
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class BerserkerBunnyArmorEaster
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

        GetBBA();

        Core.SetOptions(false);
    }

    public void GetBBA()
    {
        if (Core.CheckInventory("Berserker Bunny Armor"))
            return;

        if (!Core.CheckInventory("Berserker Bunny Armor"))
        {
            Core.EnsureAccept(236);
            Core.KillMonster(
                "greenguardwest",
                "West12",
                "Up",
                "Big Bad Boar",
                "Were Egg",
                log: false
            );
            Core.EnsureComplete(236);
            Bot.Wait.ForPickup("Berserker Bunny Armor");
        }
    }
}
