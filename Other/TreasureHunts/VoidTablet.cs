/*
name: Void Tablet Treasure Hunt
description: This script will complete the Void Tablet Treasure Hunt, which rewards Void Medulla Makila.
tags: void tablet, treasure hunt,museum,valencia,void gem,void-walking,void-walking potion,void,void shop,void medulla makilla
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class VoidTablet
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        DoHunt();

        Core.SetOptions(false);
    }

    public void DoHunt()
    {
        if (Core.CheckInventory("Void Medulla Makila"))
            return;

        Core.AddDrop("Void Medulla Makila");

        // Void Tablet Hunt Started
        if (!Core.CheckInventory(93827))
        {
            Core.AddDrop(93827);
            Core.GetMapItem(14493, map: "museum");
        }

        // Void Gem
        if (!Core.CheckInventory(93828))
        {
            Core.AddDrop(93828);
            Core.GetMapItem(14494, map: "voidcave");
        }
        // Potion of Void-Walking
        if (!Core.CheckInventory(Core.QuestRewards(10269)))
        {
            Core.AddDrop(Core.QuestRewards(10269));
            Core.EnsureAccept(10269);
            Adv.BuyItem("alchemyacademy", 397, 11474, 2, 2, 1231);
            Core.HuntMonster("lumafortress", "Light Treeant", "Light Apples", 4);
            Core.HuntMonster("rivensylth", "Rivensylth Spider", "Spider Venom", 8);
            if (!Core.CheckInventory("Void Energy Crystal"))
                Core.GetMapItem(14495, map: "laken");
            Core.EnsureComplete(10269);
            Bot.Wait.ForPickup(Core.QuestRewards(10269)[0]);
        }

        // Golden Void Star
        if (!Core.CheckInventory("Golden Void Star"))
        {
            Core.AddDrop("Golden Void Star");
            Core.GetMapItem(14497, map: "voidtempext");
        }

        // Void Medulla Makila
        Core.BuyItem("void", 2583, 75303);
    }
}
