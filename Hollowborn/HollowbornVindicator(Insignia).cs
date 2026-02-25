/*
name: Hollowborn Vindicator Class (Insignia)
description: Farms Hollowborn Vindicator Class prereqs for Insignia quest.
tags: hollowborn, class, hbv,hollowborn vindicator, vindicator, gramiel,insignia,ultragramielhub, ultragramiel
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Hollowborn/Materials/HollowSoul.cs
//cs_include Scripts/Hollowborn/Materials/VindicatorBadge.cs
//cs_include Scripts/Hollowborn/Materials/DeathsPower.cs
//cs_include Scripts/Hollowborn/Materials/GraceOrb.cs
//cs_include Scripts/Hollowborn/Materials/GramielsEmblem.cs
//cs_include Scripts/Hollowborn/Materials/VindicatorCrest.cs
//cs_include Scripts/Story/Hollowborn/CoreHollowbornStory.cs
//cs_include Scripts/Hollowborn/HollowbornVindicator(NonInsignia).cs

using Skua.Core.Interfaces;

public class HBVInsig
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static HollowSoul HS
    {
        get => _HS ??= new HollowSoul();
        set => _HS = value;
    }
    private static HollowSoul _HS;
    private static HBVNonInsig HBV
    {
        get => _HBV ??= new HBVNonInsig();
        set => _HBV = value;
    }
    private static HBVNonInsig _HBV;
    private static CoreHollowbornStory HBS
    {
        get => _HBS ??= new CoreHollowbornStory();
        set => _HBS = value;
    }
    private static CoreHollowbornStory _HBS;
    private static VindicatorBadge VB
    {
        get => _VB ??= new VindicatorBadge();
        set => _VB = value;
    }
    private static VindicatorBadge _VB;
    private static DeathsPower DP
    {
        get => _DP ??= new DeathsPower();
        set => _DP = value;
    }
    private static DeathsPower _DP;
    private static GraceOrb GO
    {
        get => _GO ??= new GraceOrb();
        set => _GO = value;
    }
    private static GraceOrb _GO;
    private static GramielsEmblem GE
    {
        get => _GE ??= new GramielsEmblem();
        set => _GE = value;
    }
    private static GramielsEmblem _GE;
    private static VindicatorCrest VC
    {
        get => _VC ??= new VindicatorCrest();
        set => _VC = value;
    }
    private static VindicatorCrest _VC;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        GetClass();
        Core.SetOptions(false);
    }

    public void GetClass(bool rankUpClass = true)
    {
        if (Core.CheckInventory(94357))
        {
            if (rankUpClass)
                Adv.RankUpClass("Hollowborn Vindicator");
            return;
        }

        Farm.Experience(80);
        Farm.HollowbornREP();

        HBS.DawnSanctum();
        string reqName = Core.QuestRewards(10300)[0];
        Core.AddDrop(reqName);

        int Owned = 4 - Bot.Inventory.GetQuantity(reqName);

        if (!Core.CheckInventory(reqName, 4 / Owned))
        {
            Core.EnsureAccept(10300);

            // Vindicator Crest
            VC.GetVindicatorCrest(20 / Owned);

            // Gramiel's Emblem
            GE.GetGramielsEmblem(60 / Owned);

            // Grace Orb
            GO.GetGraceOrb(80 / Owned);

            // Vindicator Badge
            VB.GetVindicatorBadge(40 / Owned);

            // Hollow Soul
            HS.GetYaSoulsHeeeere(300 / Owned);

            // Death's Power
            DP.GetDP(4 / Owned);

            if (!Core.CheckInventory("Gramiel the Graceful's Insignia", 5))
            {
                Core.Logger($"You need 5x Gramiel the Graceful's Insignia to complete the quest.)");
                return;
            }

            if (!Bot.Quests.IsAvailable(10300))
            {
                Core.Logger("This is a weekly quest, you need to wait until next week to get the class.");
                return;
            }
            else
            {
                Core.EnsureComplete(10300);
                Bot.Wait.ForPickup(reqName);
            }
        }

        if (!Core.CheckInventory(reqName, 4))
        {
            Core.Logger($"You need 4x {reqName} to get the class. Run the script next week.");
            return;
        }
        else
            Adv.BuyItem("ultragramielhub", 2593, "Hollowborn Vindicator");

        if (rankUpClass)
            Adv.RankUpClass("Hollowborn Vindicator");
    }
}
