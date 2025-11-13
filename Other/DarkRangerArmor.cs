/*
name: Dark Ranger Armor
description: farms the "Dark Ranger" set from Quest: "Dark Ranger Corps".
tags: dark ranger, armor, set
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class DarkRanger
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
        Core.SetOptions();

        GetArmor();

        Core.SetOptions(false);
    }

    public void GetArmor()
    {
        if (Core.CheckInventory("Dark Ranger"))
            return;

        if (!Core.HasAchievement(24, "ip9"))
        {
            Core.Logger("You need to have \"8 years played\" badge in order to use this bot.");
            return;
        }

        Adv.BuyItem("whitemap", 1317, "Golden 8th Birthday Candle");
        Farm.SandseaREP();
        Core.AddDrop("Dark Ranger");

        Core.RegisterQuests(5517);
        while (!Bot.ShouldExit && !Core.CheckInventory("Dark Ranger"))
        {
            Core.HuntMonster("nostalgiaquest", "Zardman Grunt", "Grunt Leather", 6);
            Core.HuntMonster("nostalgiaquest", "Big Jack Sprat", "Black Dye", 2);
            Bot.Wait.ForPickup("Dark Ranger");
        }
        Core.CancelRegisteredQuests();
    }
}
