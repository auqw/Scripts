/*
name: Pirate Class
description: This script will get the Pirate Class.
tags: merge-shop, pirate, class, tlapd, talk-like-a-pirate-day, seasonal
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/BlazeBeardStory.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/MergeShops/BlazeBeardMerge.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Models.Shops;

public class PirateClass
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static BlazebeardMerge BBM
    {
        get => _BBM ??= new BlazebeardMerge();
        set => _BBM = value;
    }
    private static BlazebeardMerge _BBM;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetPirate();

        Core.SetOptions(false);
    }

    public void GetPirate(bool rankUpClass = true)
    {
        if (Core.CheckInventory(new[] { "Classic Pirate", "Pirate" }, any: true))
        {
            if (rankUpClass)
                Adv.RankUpClass(
                    Core.CheckInventory("Classic Pirate") ? "Classic Pirate" : "Pirate"
                );
            return;
        }

        if (!Core.isSeasonalMapActive("blazebeard"))
            return;

        TinyStory();
        BBM.BuyAllMerge("Pirate");

        if (rankUpClass)
            Adv.RankUpClass("Pirate");
    }


    void TinyStory()
    {
        if (!Core.CheckInventory("Pirate"))
        {
            if (!Core.isCompletedBefore(31))
            {
                if (!Core.isCompletedBefore(28))
                {
                    Core.EnsureAccept(28);
                    Core.HuntMonster("pirates", "Shark Bait", "Pirate Key", isTemp: true);
                    Core.GetMapItem(19, 1, "pirates");
                    Core.EnsureComplete(28);
                }

                if (!Core.isCompletedBefore(29))
                {
                    Core.EnsureAccept(29);
                    Core.HuntMonster("pirates", "Fishman Soldier", "Fish Scale", 9);
                    Core.EnsureComplete(29);

                }
                if (!Core.isCompletedBefore(30))
                {
                    Core.EnsureAccept(30);
                    Core.HuntMonster("pirates", "Fishwing", "Fish Wings", 6);
                    Core.EnsureComplete(30);

                }
            }

            //Map Recovery 31
            Core.AddDrop("Pirate");
            Core.EnsureAccept(31);
            Core.KillMonster("Pirates", "Mast", "Left", "Fishwing", "Map Fragment", 5);
            Core.EnsureComplete(31);
        }
    }
}
