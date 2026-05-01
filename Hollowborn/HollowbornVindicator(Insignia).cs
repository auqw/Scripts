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
        // Already own the class
        if (Core.CheckInventory(94357))
        {
            if (rankUpClass)
                Adv.RankUpClass("Hollowborn Vindicator");
            return;
        }

        // Prereqs
        Farm.Experience(80);
        Farm.HollowbornREP();
        HBS.DawnSanctum();

        Core.AddDrop("Condensed Grace");

        int currentQty = Bot.Inventory.GetQuantity("Condensed Grace");
        int missingQty = 4 - currentQty;
        int multiplier = missingQty;

        // Still need weekly turn-ins
        if (missingQty > 0)
        {
            // Hard gate: need insignias before wasting time farming mats
            if (!Core.CheckInventory("Gramiel the Graceful's Insignia", 5))
            {
                Core.Logger("Requires 5x Gramiel the Graceful's Insignia.");
                return;
            }

            // Weekly lock check
            if (Bot.Quests.IsDailyComplete(10300))
            {
                Core.Logger("Weekly already completed.");
                Core.Logger($"Next run: {DateTime.Now.AddDays(7):yyyy-MM-dd HH:mm:ss}");
                return;
            }

            Core.EnsureAccept(10300);

            // Farm all remaining weeks worth of materials in one run

            DP.GetDP(multiplier);                   // Death's Power      (1 per weekly)
            HS.GetYaSoulsHeeeere(75 * multiplier);  // Hollow Soul        (75 per weekly)
            VB.GetVindicatorBadge(10 * multiplier); // Vindicator Badge   (10 per weekly)
            GO.GetGraceOrb(20 * multiplier);        // Grace Orb          (20 per weekly)
            GE.GetGramielsEmblem(15 * multiplier);  // Gramiel's Emblem   (15 per weekly)
            VC.GetVindicatorCrest(5 * multiplier);  // Vindicator Crest   (5 per weekly)

            Core.EnsureComplete(10300);
            Bot.Wait.ForPickup("Condensed Grace");
        }

        // Not enough weeklies yet → wait for reset
        if (!Core.CheckInventory("Condensed Grace", 4))
        {
            Core.Logger($"Progress: {Bot.Inventory.GetQuantity("Condensed Grace")}/4 {"Condensed Grace"}");
            Core.Logger($"Run again next week: {DateTime.Now.AddDays(7):yyyy-MM-dd HH:mm:ss}");
            return;
        }

        // Buy + optionally rank up class
        Adv.BuyItem("ultragramielhub", 2593, "Hollowborn Vindicator");

        if (rankUpClass)
            Adv.RankUpClass("Hollowborn Vindicator");
    }

}
