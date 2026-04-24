/*
name: Hollowborn Vindicator Class (Non Insignia)
description: Farms Hollowborn Vindicator Class.
tags: hollowborn, class, hbv,hollowborn vindicator, vindicator, gramiel, non insignia,ultragramielhub
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

using Skua.Core.Interfaces;
using Skua.Core.Options;

public class HBVNonInsig
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

    public string OptionsStorage = "FarmerJoePet";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<bool>("Farm4Weeks", "Farm ALL 4 weeeks mats", "Save yourself some time, prefarm 4x the mats for all 4 weeks", true),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        GetClass();
        Core.SetOptions(false);
    }

    public void GetClass(bool rankUpClass = true, bool merge = false, int quant = 4, bool FarmNextWeeks = false)
    {
        // Already own the class OR already have enough Condensed Grace for merge
        if ((!merge && Core.CheckInventory(94357)) || (merge && Core.CheckInventory("Condensed Grace", quant)))
        {
            if (!merge && rankUpClass)
                Adv.RankUpClass("Hollowborn Vindicator");
            return;
        }

        // Prereqs
        Farm.Experience(80);
        Farm.HollowbornREP();
        HBS.DawnSanctum();

        string reqName = Core.QuestRewards(10299)[0];
        Core.AddDrop(reqName);

        bool preFarmNextWeeks = Bot.Config!.Get<bool>("Farm4Weeks");
        int currentQty = Bot.Inventory.GetQuantity(reqName);
        int missingQty = 4 - currentQty;

        // Prefarm ON  → farm ALL remaining weeks worth of materials
        // Prefarm OFF → farm ONLY one week's materials
        int multiplier = preFarmNextWeeks ? missingQty : 1;

        // Run weekly whenever we still need any Condensed Grace
        if (missingQty > 0)
        {
            Core.EnsureAccept(10299);

            // Farm weekly materials
            DP.GetDP(multiplier);                    // Death's Power      (1 per weekly)
            HS.GetYaSoulsHeeeere(1500 * multiplier); // Hollow Soul        (1500 per weekly)
            VB.GetVindicatorBadge(200 * multiplier); // Vindicator Badge   (200 per weekly)
            GO.GetGraceOrb(400 * multiplier);        // Grace Orb          (400 per weekly)
            GE.GetGramielsEmblem(300 * multiplier);  // Gramiel's Emblem   (300 per weekly)
            VC.GetVindicatorCrest(100 * multiplier); // Vindicator Crest   (100 per weekly)

            // Weekly lock check AFTER farming mats (so prefarming works safely)
            if (!Bot.Quests.IsAvailable(10299))
            {
                Core.Logger("Weekly quest completed. Come back next week.");
                Core.Logger($"Next run: {DateTime.Now.AddDays(7):yyyy-MM-dd HH:mm:ss}");
                return;
            }

            Core.EnsureComplete(10299);
            Bot.Wait.ForPickup(reqName);
        }

        // Not enough weekly turn-ins yet → exit and wait for reset
        if (!Core.CheckInventory(reqName, 4))
        {
            Core.Logger($"Progress: {Bot.Inventory.GetQuantity(reqName)}/4 {reqName}");
            Core.Logger($"Run again next week: {DateTime.Now.AddDays(7):yyyy-MM-dd HH:mm:ss}");
            return;
        }

        // Buy + optionally rank up class
        Adv.BuyItem("ultragramielhub", 2593, "Hollowborn Vindicator");

        if (rankUpClass)
            Adv.RankUpClass("Hollowborn Vindicator");
    }

}
