/*
name: Paladin (Class)
description: This bot farms the Paladin class for you.
tags: warrior, healer, paladin, class
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class Paladin
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

        GetPaladin();

        Core.SetOptions(false);
    }

    public void GetPaladin(bool rankUpClass = true)
    {
        if (Core.CheckInventory("Paladin") || !Core.IsMember)
            return;

        Core.BuyItem("necropolis", 26, "Warrior");
        Core.BuyItem("necropolis", 26, "Healer");

        Adv.RankUpClass("Warrior");
        Adv.RankUpClass("Healer");

        Farm.GoodREP(5);

        Core.BuyItem("necropolis", 26, "Paladin");

        if (rankUpClass)
            Adv.RankUpClass("Paladin");
    }
}
//why isnt this pushing to  people
