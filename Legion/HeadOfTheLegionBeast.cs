/*
name: Head of the Legion Beast
description: This bot will farm the entire Head of the Legion Beast, including stories and bosses.
tags: head, legion, beast, LOTLB, seven, circles, war, penance, essence, wrath, violence, treachery, soul, heresy, indulgence, beast, helm, violence, wrath, greed, gluttony, luxuria
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class HeadoftheLegionBeast
{
    public string OptionsStorage = "hotlb"; //<--rename this
    public bool DontPreconfigure = true; //<- Leave this alone.
    public List<IOption> Options = new()
    {
        CoreBots.Instance.SkipOptions,
        new Option<bool>(
            "badge",
            "Farm Badge",
            "Set to true to farm the Head of the Legion Beast char page badge",
            false
        ),
    };
    public string[] HeadLegionBeast =
    {
        "Penance",
        "Essence of Wrath",
        "Essence of Violence",
        "Essence of Treachery",
        "Souls of Heresy",
        "Indulgence",
        "Beast Soul",
        "Helms of the Seven Circles",
        "Faces of Violence",
        "Crown of Wrath",
        "Stare of Greed",
        "Gluttony's Maw",
        "Aspect of Luxuria",
        "Face of Treachery",
    };

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
    private static CoreLegion Legion
    {
        get => _Legion ??= new CoreLegion();
        set => _Legion = value;
    }
    private static CoreLegion _Legion;
    private static SevenCircles Circles
    {
        get => _Circles ??= new SevenCircles();
        set => _Circles = value;
    }
    private static SevenCircles _Circles;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(HeadLegionBeast);
        Core.SetOptions();

        LegionBeastHead();

        Core.SetOptions(false);
    }

    public void LegionBeastHead(bool badge = false)
    {
        const string HeadName = "Head of the Legion Beast";
        bool hasHead = Core.CheckInventory(HeadName);
        bool wantBadge = (Bot.Config?.Get<bool>("badge") ?? false) || badge;
        bool hasBadge = Core.HasWebBadge(HeadName);

        if (hasHead && (!wantBadge || hasBadge))
            return;

        if (hasHead && wantBadge && !hasBadge)
        {
            DoBadgeQuest();
            return;
        }

        Circles.CirclesWar();
        Core.AddDrop(HeadLegionBeast);

        HelmSevenCircles();
        Penance(30);
        Indulgence(30);
        Legion.FarmLegionToken(15000);

        Core.EquipClass(ClassType.Dodge);
        Core.KillMonster(
            "sevencircleswar",
            "r17",
            "Left",
            "The Beast",
            "Beast Soul",
            15,
            isTemp: false,
            publicRoom: true,
            log: false
        );

        Adv.BuyItem("sevencircleswar", 1984, HeadName);

        if (wantBadge && !hasBadge)
        {
            DoBadgeQuest();
        }
    }

    private void DoBadgeQuest()
    {
        // Head of the Legion Beast (8082)
        Core.Unbank("Head of the Legion Beast");
        Core.EnsureAccept(8082);
        Core.EquipClass(ClassType.Solo);
        Core.HuntMonster("sevencircleswar", "The Beast", "Beast Slain");
        Core.EnsureComplete(8082);
    }

    public void HelmSevenCircles()
    {
        if (Core.CheckInventory(60137))
            return;

        Core.AddDrop(HeadLegionBeast);

        CircleHelm("Aspect of Luxuria");
        CircleHelm("Gluttony's Maw");
        CircleHelm("Stare of Greed");
        CircleHelm("Crown of Wrath", true);
        CircleHelm("Face of Treachery", true);
        CircleHelm("Faces of Violence", true);

        Adv.BuyItem("sevencircleswar", 1984, "Helms of the Seven Circles");
    }

    /// <summary>
    /// Farms the specified quantity of "Essence of Wrath" items.
    /// </summary>
    /// <param name="quant">The target quantity of "Essence of Wrath" items to collect. Default is 300.</param>
    public void EssenceWrath(int quant = 300)
    {
        if (Core.CheckInventory("Essence of Wrath", quant))
        {
            Core.FarmingLogger("Essence of Wrath", quant);
            Core.Logger($"Already have {quant}. Skipping.", "EssenceWrath");
            return;
        }

        Core.Logger($"Starting farm for {quant}...", "EssenceWrath");
        Core.AddDrop(HeadLegionBeast);
        Core.EquipClass(ClassType.Farm);
        Core.FarmingLogger("Essence of Wrath", quant);
        Core.RegisterQuests(7979);

        while (!Bot.ShouldExit && !Core.CheckInventory("Essence of Wrath", quant))
        {
            Core.KillMonster("sevencircleswar", "Enter", "Spawn", "*", log: false);
            Bot.Wait.ForDrop("Essence of Wrath");
        }

        Core.CancelRegisteredQuests();
        Core.Logger($"Farm complete!", "EssenceWrath");
    }

    /// <summary>
    /// Farms the specified quantity of "Essence of Violence" items.
    /// </summary>
    /// <param name="quant">The target quantity of "Essence of Violence" items to collect. Default is 300.</param>
    public void EssenceViolence(int quant = 300)
    {
        if (Core.CheckInventory("Essence of Violence", quant))
        {
            Core.FarmingLogger("Essence of Violence", quant);
            Core.Logger($"Already have {quant}. Skipping.", "EssenceViolence");
            return;
        }

        Core.Logger($"Starting farm for {quant}...", "EssenceViolence");
        Core.AddDrop(HeadLegionBeast);
        Core.EquipClass(ClassType.Farm);
        Core.FarmingLogger("Essence of Violence", quant);
        Core.RegisterQuests(7985);

        while (!Bot.ShouldExit && !Core.CheckInventory("Essence of Violence", quant))
        {
            Core.KillMonster("sevencircleswar", "r9", "Left", "Violence Guard", log: false);
            Bot.Wait.ForDrop("Essence of Violence");
        }

        Core.CancelRegisteredQuests();
        Core.Logger($"Farm complete!", "EssenceViolence");
    }

    /// <summary>
    /// Farms the specified quantity of "Essence of Treachery" items.
    /// </summary>
    /// <param name="quant">The target quantity of "Essence of Treachery" items to collect. Default is 300.</param>
    public void EssenceTreachery(int quant = 300)
    {
        if (Core.CheckInventory("Essence of Treachery", quant))
        {
            Core.FarmingLogger("Essence of Treachery", quant);
            Core.Logger($"Already have {quant}. Skipping.", "EssenceTreachery");
            return;
        }

        Core.Logger($"Starting farm for {quant}...", "EssenceTreachery");
        Core.AddDrop(HeadLegionBeast);
        Core.EquipClass(ClassType.Farm);
        Core.FarmingLogger("Essence of Treachery", quant);
        Core.RegisterQuests(7988);

        while (!Bot.ShouldExit && !Core.CheckInventory("Essence of Treachery", quant))
        {
            Core.KillMonster("sevencircleswar", "r13", "Left", "Treachery Guard", log: false);
            Bot.Wait.ForDrop("Essence of Treachery");
        }

        Core.CancelRegisteredQuests();
        Core.Logger($"Farm complete!", "EssenceTreachery");
    }

    /// <summary>
    /// Farms the specified quantity of "Souls of Heresy" items.
    /// Max stack is 300, so will farm up to that limit per call.
    /// </summary>
    /// <param name="quant">The target quantity of "Souls of Heresy" items to collect. Default is 300.</param>
    public void SoulsHeresy(int quant = 300)
    {
        const int SOULS_MAX_STACK = 300;
        quant = Math.Min(quant, SOULS_MAX_STACK); // Cap at max stack

        if (Core.CheckInventory("Souls of Heresy", quant))
        {
            Core.FarmingLogger("Souls of Heresy", quant);
            Core.Logger($"Already have {quant}. Skipping.", "SoulsHeresy");
            return;
        }

        Core.Logger($"Starting farm for {quant} (capped at max stack {SOULS_MAX_STACK})...", "SoulsHeresy");
        Core.AddDrop(HeadLegionBeast);

        if (!Bot.Quests.IsUnlocked(7983))
        {
            Core.Logger($"Quest not unlocked. Running Circles.CirclesWar()...", "SoulsHeresy");
            Circles.CirclesWar(true);
        }

        Core.FarmingLogger("Souls of Heresy", quant);
        Core.RegisterQuests(7983, 7980, 7981); // Blasphemy? Blasphe-you! | War Medals | Mega War Medals

        Core.EquipClass(ClassType.Farm);
        Core.Logger($"Farming {quant} souls...", "SoulsHeresy");

        while (!Bot.ShouldExit && !Core.CheckInventory("Souls of Heresy", quant))
        {
            Core.KillMonster("sevencircleswar", "r7", "Left", "*", log: false);
            Bot.Wait.ForDrop("Souls of Heresy");
        }

        Core.CancelRegisteredQuests();
        Core.Logger($"Farm complete!", "SoulsHeresy");
    }

    /// <summary>
    /// Farms the specified quantity of "Penance" items.
    /// </summary>
    /// <param name="quant">The target quantity of "Penance" items to collect. Default is 300.</param>
    public void Penance(int quant = 300)
    {
        if (Core.CheckInventory("Penance", quant))
        {
            Core.FarmingLogger("Penance", quant);
            Core.Logger($"Already have {quant} Penance. Skipping.", "Penance");
            return;
        }

        Core.Logger($"Starting farm for {quant} Penance", "Penance");
        Core.AddDrop(HeadLegionBeast);
        Core.FarmingLogger("Penance", quant);
        Core.EquipClass(ClassType.Farm);

        // Farm essences once (they don't have stack limit issues)
        Core.Logger($"Farming essences...", "Penance");
        EssenceWrath(quant);
        EssenceViolence(quant);
        EssenceTreachery(quant);
        Core.Logger($"Essence farming complete", "Penance");

        const int SOULS_MAX_STACK = 300;
        const int SOULS_PER_PENANCE = 15;
        int totalSoulsNeeded = quant * SOULS_PER_PENANCE;
        int penance_bought = 0;

        Core.Logger($"Total souls needed: {totalSoulsNeeded} (farming in batches of max {SOULS_MAX_STACK})", "Penance");

        // Buy penance in batches accounting for souls max stack of 300
        while (!Bot.ShouldExit && penance_bought < quant)
        {
            int soulsCurrently = Bot.Inventory.GetQuantity("Souls of Heresy");
            int penanceRemaining = quant - penance_bought;

            Core.Logger($"Batch Progress: {penance_bought}/{quant} Penance bought | Souls on hand: {soulsCurrently}", "Penance");

            if (soulsCurrently < SOULS_PER_PENANCE)
            {
                // Farm souls up to max stack
                int soulsToFarm = Math.Min(SOULS_MAX_STACK, totalSoulsNeeded - (penance_bought * SOULS_PER_PENANCE));
                Core.Logger($"Souls depleted. Farming {soulsToFarm} more souls...", "Penance");
                SoulsHeresy(soulsToFarm);
                soulsCurrently = Bot.Inventory.GetQuantity("Souls of Heresy");
                Core.Logger($"Souls farmed. Current count: {soulsCurrently}", "Penance");
            }

            // Buy as much penance as we can with current souls
            int penanceCanBuy = soulsCurrently / SOULS_PER_PENANCE;
            int penanceToBuy = Math.Min(penanceCanBuy, penanceRemaining);

            if (penanceToBuy > 0)
            {
                Core.Logger($"Buying {penanceToBuy} Penance (requires {penanceToBuy * SOULS_PER_PENANCE} souls)...", "Penance");
                Core.BuyItem("sevencircleswar", 1984, "Penance", penanceToBuy);
                Bot.Wait.ForPickup("Penance");
                penance_bought += penanceToBuy;
                Core.Logger($"Purchase complete. Total bought: {penance_bought}/{quant}", "Penance");
            }
            else
            {
                Core.Logger($"Not enough souls to buy more penance. Need {SOULS_PER_PENANCE}, have {soulsCurrently}", "Penance");
            }

            Core.Sleep(500);
        }

        Core.Logger($"Farm complete! Got {penance_bought} Penance", "Penance");
    }

    /// <summary>
    /// Farms the specified quantity of "Indulgence" items.
    /// </summary>
    /// <param name="quant">The target quantity of "Indulgence" items to collect. Default is 100.</param>
    public void Indulgence(int quant = 100)
    {
        if (Core.CheckInventory("Indulgence", quant))
        {
            Core.Logger($"Already have {quant} Indulgence. Skipping.", "Indulgence");
            return;
        }

        Core.Logger($"Starting farm for {quant} Indulgence", "Indulgence");
        Core.AddDrop(HeadLegionBeast);
        Core.EquipClass(ClassType.Farm);
        Core.FarmingLogger("Indulgence", quant);

        int currentQuantity = Bot.Inventory.GetQuantity("Indulgence");
        int deficit = quant - currentQuantity;

        Core.Logger($"Current: {currentQuantity} | Deficit: {deficit} | Target: {quant}", "Indulgence");

        // Calculate targets based on deficit
        int soulsTarget =
            deficit >= 3 ? 75
            : deficit == 2 ? 50
            : 25;
        int essenceTarget = deficit >= 3 ? 3 : deficit;

        Core.Logger($"Soul target: {soulsTarget} | Essence target: {essenceTarget} per essence", "Indulgence");

        while (!Bot.ShouldExit && !Core.CheckInventory("Indulgence", quant))
        {
            Core.Logger($"Starting quest cycle...", "Indulgence");

            Core.EnsureAccept(7978);
            Core.EquipClass(ClassType.Farm);

            Core.Logger($"Farming {soulsTarget} Souls of Limbo...", "Indulgence");
            Core.KillMonster("sevencircles", "r2", "Left", "Limbo Guard", "Souls of Limbo", soulsTarget, log: false);

            Core.EquipClass(ClassType.Solo);

            Core.Logger($"Farming essences...", "Indulgence");
            Core.KillMonster("sevencircles", "r4", "Left", "Luxuria", "Essence of Luxuria", essenceTarget, log: false);
            Core.KillMonster("sevencircles", "r6", "Left", "Gluttony", "Essence of Gluttony", essenceTarget, log: false);
            Core.KillMonster(
                "sevencircles",
                "r8",
                "Left",
                "Avarice",
                "Essence of Avarice",
                essenceTarget,
                log: false
            );

            Core.Logger($"Completing quest...", "Indulgence");
            Core.EnsureCompleteMulti(7978);
            Bot.Wait.ForPickup("Indulgence");

            currentQuantity = Bot.Inventory.GetQuantity("Indulgence");
            Core.Logger($"Progress: {currentQuantity}/{quant}", "Indulgence");
        }

        Core.CancelRegisteredQuests();
        Core.Logger($"Farm complete! Got {Bot.Inventory.GetQuantity("Indulgence")} Indulgence", "Indulgence");
    }

    /// <summary>
    /// Farms the specified quantity of "Circle Helm" items.
    /// </summary>
    /// <param name="helm">The name of the helm to be farmed.</param>
    /// <param name="war">Whether to farm in the "sevencircleswar" zone. Default is false.</param>
    public void CircleHelm(string helm, bool war = false)
    {
        if (Core.CheckInventory(helm))
        {
            Core.Logger($"Already have {helm}. Skipping.", "CircleHelm");
            return;
        }

        Core.Logger($"Starting farm for {helm}", "CircleHelm");
        Core.FarmingLogger(helm, 1);
        Legion.FarmLegionToken(1500);

        if (war)
        {
            Penance(10);
            Adv.BuyItem("sevencircleswar", 1984, helm);
        }
        else
        {
            Indulgence(10);
            Adv.BuyItem("sevencircles", 1980, helm);
        }

        Core.Logger($"Farm complete! Got {helm}", "CircleHelm");
    }

}
