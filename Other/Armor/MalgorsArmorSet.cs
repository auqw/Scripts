/*
name: Malgors Armor Set
description: completes the quest "build malgor's armor Set" for the set.
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/ShadowsOfWar/CoreSoWMats.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Legion/CoreLegion.cs

//cs_include Scripts/Legion/YamiNoRonin/CoreYnR.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/DeadLinesMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/ShadowflameFinaleMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/TimekeepMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/StreamwarMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/WorldsCoreMerge.cs
//cs_include Scripts/ShadowsOfWar/MergeShops/ManaCradleMerge.cs

//cs_include Scripts/Legion/SwordMaster.cs

using Skua.Core.Interfaces;

public class MalgorsArmorSet
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreSoW SoW
    {
        get => _SoW ??= new CoreSoW();
        set => _SoW = value;
    }
    private static CoreSoW _SoW;
    private static CoreYnR YNR
    {
        get => _YNR ??= new CoreYnR();
        set => _YNR = value;
    }
    private static CoreYnR _YNR;
    private static DeadLinesMerge DeadLinesMerge
    {
        get => _DeadLinesMerge ??= new DeadLinesMerge();
        set => _DeadLinesMerge = value;
    }
    private static DeadLinesMerge _DeadLinesMerge;
    private static ShadowflameFinaleMerge ShadowflameFinaleMerge
    {
        get => _ShadowflameFinaleMerge ??= new ShadowflameFinaleMerge();
        set => _ShadowflameFinaleMerge = value;
    }
    private static ShadowflameFinaleMerge _ShadowflameFinaleMerge;
    private static TimekeepMerge TimekeepMerge
    {
        get => _TimekeepMerge ??= new TimekeepMerge();
        set => _TimekeepMerge = value;
    }
    private static TimekeepMerge _TimekeepMerge;
    private static StreamwarMerge StreamwarMerge
    {
        get => _StreamwarMerge ??= new StreamwarMerge();
        set => _StreamwarMerge = value;
    }
    private static StreamwarMerge _StreamwarMerge;
    private static WorldsCoreMerge WorldsCoreMerge
    {
        get => _WorldsCoreMerge ??= new WorldsCoreMerge();
        set => _WorldsCoreMerge = value;
    }
    private static WorldsCoreMerge _WorldsCoreMerge;
    private static ManaCradleMerge ManaCradleMerge
    {
        get => _ManaCradleMerge ??= new ManaCradleMerge();
        set => _ManaCradleMerge = value;
    }
    private static ManaCradleMerge _ManaCradleMerge;

    string[] ArmorSet = { "Malgor the ShadowLord", "ShadowLord's Helm" };

    string[] QuestItems =
    {
        "Timestream Ravager",
        "ShadowFlame Defender",
        "Mana Guardian",
        "Dark Dragon Slayer",
        "Mystical Devotee of Mana",
        "Dragon's Tear",
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(ArmorSet.Concat(QuestItems));
        Core.SetOptions();

        GetSet(true, ArmorSet);

        Core.SetOptions(false);
    }

    public void GetSet(bool useSet = true, string[]? item = null)
    {
        // Determine which items to check: default ArmorSet or provided array
        string[]? items = useSet ? ArmorSet : item;

        if (items == null || items.Length == 0)
            return; // Nothing to do if no items specified

        // If we already have all items, bank quest drops and exit
        if (Core.CheckInventory(items))
        {
            Core.ToBank(SoW.MalgorDrops.Concat(SoW.MainyuDrops).ToArray());
            return;
        }

        // Prepare the store for buying/upgrading gear
        Adv.GearStore();

        // Keep attempting to get items until Bot exits or inventory is complete
        while (!Bot.ShouldExit && !Core.CheckInventory(items))
        {
            DeadLinesMerge.BuyAllMerge(buyOnlyThis: "Timestream Ravager");
            ShadowflameFinaleMerge.BuyAllMerge(buyOnlyThis: "ShadowFlame Defender");
            TimekeepMerge.BuyAllMerge(buyOnlyThis: "Mana Guardian");
            StreamwarMerge.BuyAllMerge(buyOnlyThis: "Dark Dragon Slayer");
            WorldsCoreMerge.BuyAllMerge(buyOnlyThis: "Mystical Devotee of Mana");
            ManaCradleMerge.BuyAllMerge(buyOnlyThis: "Dragon's Tear");

            Adv.BuyItem("alchemyacademy", 395, "Gold Voucher 500k", 30);

            // Ensure quest items are in inventory before completing quest
            Core.Unbank(QuestItems);
            Core.ChainComplete(9127);
        }

        // Return to GearStore and bank quest drops
        Adv.GearStore(true);
        Core.ToBank(SoW.MalgorDrops.Concat(SoW.MainyuDrops).ToArray());
    }
}
