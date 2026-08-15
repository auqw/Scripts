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

        Adv.GearStore(false, true);
        if (Core.CheckInventory("ArchPaladin"))
        {
            Core.Equip("ArchPaladin");
        }
        else
            Core.EquipClass(ClassType.Solo);
        Core.KillMonster("sevencircleswar", "r17", "Left", "The Beast", "Beast Soul", 15, isTemp: false, publicRoom: true, log: false);
        Adv.GearStore(true, true);

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
        Adv.GearStore(false);
        if (Core.CheckInventory("ArchPaladin"))
        {
            Core.Equip("ArchPaladin");
        }
        else
            Core.EquipClass(ClassType.Solo);
        Core.HuntMonster("sevencircleswar", "The Beast", "Beast Slain");
        Adv.GearStore(true);
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
            Core.Logger($"Already have {quant}. Skipping.", "EssenceWrath");
            return;
        }

        Core.Logger($"Starting farm for {quant}...", "EssenceWrath");
        Core.AddDrop(HeadLegionBeast);
        Core.EquipClass(ClassType.Farm);
        Core.FarmingLogger("Essence of Wrath", quant);
        Core.RegisterQuests(7979);

        Core.KillMonster("sevencircleswar", "Enter", "Spawn", "*", "Essence of Wrath", quant, isTemp: false, log: false);
        Bot.Wait.ForDrop("Essence of Wrath");

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
            Core.Logger($"Already have {quant}. Skipping.", "EssenceViolence");
            return;
        }

        Core.Logger($"Starting farm for {quant}...", "EssenceViolence");
        Core.AddDrop(HeadLegionBeast);
        Core.EquipClass(ClassType.Farm);
        Core.FarmingLogger("Essence of Violence", quant);
        Core.RegisterQuests(7985);

        Core.KillMonster("sevencircleswar", "r9", "Left", "Violence Guard", "Essence of Violence", quant, isTemp: false, log: false);
        Bot.Wait.ForDrop("Essence of Violence");

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
            Core.Logger($"Already have {quant}. Skipping.", "EssenceTreachery");
            return;
        }

        Core.Logger($"Starting farm for {quant}...", "EssenceTreachery");
        Core.AddDrop(HeadLegionBeast);
        Core.EquipClass(ClassType.Farm);
        Core.FarmingLogger("Essence of Treachery", quant);
        Core.RegisterQuests(7988);

        Core.KillMonster("sevencircleswar", "r13", "Left", "Treachery Guard", "Essence of Treachery", quant, isTemp: false, log: false);
        Bot.Wait.ForDrop("Essence of Treachery");

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

        int soulsOnHandStart = Bot.Inventory.GetQuantity("Souls of Heresy");

        if (Core.CheckInventory("Souls of Heresy", quant))
        {
            Core.Logger($"Already have {quant}. Skipping.", "SoulsHeresy");
            return;
        }

        Core.Logger($"Starting farm for {quant} souls (capped at max stack {SOULS_MAX_STACK})", "SoulsHeresy");
        Core.Logger($"  Starting inventory: {soulsOnHandStart}/{SOULS_MAX_STACK}", "SoulsHeresy");
        Core.AddDrop(HeadLegionBeast);

        if (!Bot.Quests.IsUnlocked(7983))
        {
            Core.Logger($"Quest not unlocked. Running Circles.CirclesWar()...", "SoulsHeresy");
            Circles.CirclesWar(true);
        }

        Core.FarmingLogger("Souls of Heresy", quant);
        Core.RegisterQuests(7983, 7980, 7981); // Blasphemy? Blasphe-you! | War Medals | Mega War Medals

        Core.EquipClass(ClassType.Farm);
        Core.Logger($"Target this session: {quant} souls | Collecting souls from heretics...", "SoulsHeresy");

        int killCount = 0;
        int lastLoggedProgress = soulsOnHandStart;

        while (!Bot.ShouldExit && !Core.CheckInventory("Souls of Heresy", quant))
        {
            Core.KillMonster("sevencircleswar", "r7", "Left", "*", log: false);
            Bot.Wait.ForDrop("Souls of Heresy");

            killCount++;
            int currentSouls = Bot.Inventory.GetQuantity("Souls of Heresy");

            // Log progress every 25 kills or when we hit major milestones
            if (killCount % 25 == 0 || currentSouls >= lastLoggedProgress + 50)
            {
                int soulsInThisSession = currentSouls - soulsOnHandStart;
                Core.Logger($"Collected: {soulsInThisSession}/{quant} | Inventory: {currentSouls}/{SOULS_MAX_STACK} | Kills: {killCount}", "SoulsHeresy");
                lastLoggedProgress = currentSouls;
            }
        }

        int soulsAtEnd = Bot.Inventory.GetQuantity("Souls of Heresy");
        int totalCollected = soulsAtEnd - soulsOnHandStart;

        Core.CancelRegisteredQuests();
        Core.Logger($"✓ Collected {totalCollected} souls | Final: {soulsAtEnd}/{SOULS_MAX_STACK} | Kills: {killCount}", "SoulsHeresy");
    }

   /// <summary>
/// Farms the specified quantity of "Penance" items.
/// Each Penance requires 1 of each essence and 15 Souls of Heresy.
/// Souls of Heresy have a maximum stack size of 300.
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

    Core.AddDrop(HeadLegionBeast);
    Core.FarmingLogger("Penance", quant);
    Core.EquipClass(ClassType.Farm);

    const int essencePerPenance = 1;
    const int soulsPerPenance = 15;
    const int soulsMaxStack = 300;
    const int maxPenancePerSoulStack = soulsMaxStack / soulsPerPenance;

    int currentPenance = Bot.Inventory.GetQuantity("Penance");
    int penanceBought = currentPenance;
    int totalSoulsUsed = 0;

    Core.Logger("", "Penance");
    Core.Logger("┌─ PHASE 1: Penance Material Farming", "Penance");
    Core.Logger(
        $"│ Target: {quant} Penance | Existing: {currentPenance} | Remaining: {quant - currentPenance}",
        "Penance");
    Core.Logger(
        $"│ Cost: {essencePerPenance} of each essence + {soulsPerPenance} Souls per Penance",
        "Penance");
    Core.Logger(
        $"│ Souls max stack: {soulsMaxStack} ({maxPenancePerSoulStack} Penance)",
        "Penance");
    Core.Logger("├────────────────────────────────────────", "Penance");

    while (!Bot.ShouldExit && penanceBought < quant)
    {
        currentPenance = Bot.Inventory.GetQuantity("Penance");
        int penanceRemaining = quant - currentPenance;

        if (penanceRemaining <= 0)
            break;

        int wrath = Bot.Inventory.GetQuantity("Essence of Wrath");
        int violence = Bot.Inventory.GetQuantity("Essence of Violence");
        int treachery = Bot.Inventory.GetQuantity("Essence of Treachery");
        int souls = Bot.Inventory.GetQuantity("Souls of Heresy");

        // Merge anything we can immediately make from existing materials.
        int craftableFromEssences = Math.Min(
            wrath / essencePerPenance,
            Math.Min(
                violence / essencePerPenance,
                treachery / essencePerPenance));

        int craftableFromSouls = souls / soulsPerPenance;

        int immediatelyCraftable = Math.Min(
            penanceRemaining,
            Math.Min(craftableFromEssences, craftableFromSouls));

        if (immediatelyCraftable > 0)
        {
            int soulsCost = immediatelyCraftable * soulsPerPenance;

            Core.Logger(
                $"│ → Merging {immediatelyCraftable} Penance " +
                $"({immediatelyCraftable * essencePerPenance} each essence + {soulsCost} souls)...",
                "Penance");

            Core.BuyItem(
                "sevencircleswar",
                1984,
                "Penance",
                immediatelyCraftable);

            Bot.Wait.ForPickup("Penance");

            currentPenance = Bot.Inventory.GetQuantity("Penance");
            penanceBought = currentPenance;
            totalSoulsUsed += soulsCost;

            Core.Logger(
                $"│ ✓ Penance: {penanceBought}/{quant} | " +
                $"Souls remaining: {Bot.Inventory.GetQuantity("Souls of Heresy")}/{soulsMaxStack}",
                "Penance");

            Core.Sleep(500);
            continue;
        }

        // Prepare only enough materials for the next batch.
        // A full 300 Souls stack supports exactly 20 Penance.
        int batchSize = Math.Min(penanceRemaining, maxPenancePerSoulStack);
        int essenceTarget = batchSize * essencePerPenance;
        int soulsTarget = batchSize * soulsPerPenance;

        // Farm only the missing Essence of Wrath.
        wrath = Bot.Inventory.GetQuantity("Essence of Wrath");

        if (wrath < essenceTarget)
        {
            Core.Logger(
                $"│ ↓ Farming {essenceTarget - wrath} Essence of Wrath " +
                $"({wrath}/{essenceTarget})...",
                "Penance");

            EssenceWrath(essenceTarget);

            if (Bot.ShouldExit)
                break;
        }

        // Farm only the missing Essence of Violence.
        violence = Bot.Inventory.GetQuantity("Essence of Violence");

        if (violence < essenceTarget)
        {
            Core.Logger(
                $"│ ↓ Farming {essenceTarget - violence} Essence of Violence " +
                $"({violence}/{essenceTarget})...",
                "Penance");

            EssenceViolence(essenceTarget);

            if (Bot.ShouldExit)
                break;
        }

        // Farm only the missing Essence of Treachery.
        treachery = Bot.Inventory.GetQuantity("Essence of Treachery");

        if (treachery < essenceTarget)
        {
            Core.Logger(
                $"│ ↓ Farming {essenceTarget - treachery} Essence of Treachery " +
                $"({treachery}/{essenceTarget})...",
                "Penance");

            EssenceTreachery(essenceTarget);

            if (Bot.ShouldExit)
                break;
        }

        // Recheck essence quantities after farming.
        wrath = Bot.Inventory.GetQuantity("Essence of Wrath");
        violence = Bot.Inventory.GetQuantity("Essence of Violence");
        treachery = Bot.Inventory.GetQuantity("Essence of Treachery");
        souls = Bot.Inventory.GetQuantity("Souls of Heresy");

        craftableFromEssences = Math.Min(
            wrath / essencePerPenance,
            Math.Min(
                violence / essencePerPenance,
                treachery / essencePerPenance));

        craftableFromSouls = souls / soulsPerPenance;

        immediatelyCraftable = Math.Min(
            penanceRemaining,
            Math.Min(craftableFromEssences, craftableFromSouls));

        // If the essences are ready but Souls are not, farm only the
        // missing Souls required for this batch.
        if (immediatelyCraftable <= 0 && souls < soulsTarget)
        {
            int soulsNeeded = Math.Min(
                soulsTarget - souls,
                soulsMaxStack - souls);

            if (soulsNeeded > 0)
            {
                Core.Logger(
                    $"│ ↓ Farming {soulsNeeded} Souls of Heresy " +
                    $"({souls}/{soulsMaxStack})...",
                    "Penance");

                // SoulsHeresy() takes the amount to farm.
                SoulsHeresy(soulsNeeded);

                souls = Bot.Inventory.GetQuantity("Souls of Heresy");

                Core.Logger(
                    $"│ ↑ Souls complete: {souls}/{soulsMaxStack}",
                    "Penance");
            }
        }

        Core.Sleep(500);
    }

    penanceBought = Bot.Inventory.GetQuantity("Penance");

    Core.Logger("Penance: ├────────────────────────────────────────");
    Core.Logger($"Penance: └─ ✓ Phase Complete! {penanceBought}/{quant} Penance");
    Core.Logger($"Penance: ✓ Souls used: {totalSoulsUsed}");
    Core.Logger("Penance: ");
    Core.Logger($"╔{new string('═', boxWidth)}╗");
    Core.Logger($"║{CenterWithFill("✦ PENANCE FARMING SESSION COMPLETE ✦")}║");
    Core.Logger($"╚{new string('═', boxWidth)}╝");
}
   
    const int boxWidth = 44;

    string CenterWithFill(string text)
    {
        string decorated = $" {text} ";

        if (decorated.Length >= boxWidth)
            return decorated;

        int fillTotal = boxWidth - decorated.Length;
        int left = fillTotal / 2;
        int right = fillTotal - left;

        return new string('═', left) + decorated + new string('═', right);
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
            Core.KillMonster("sevencircles", "r4", "Left", "Luxuria", "Essence of Luxuria", essenceTarget, isTemp: false, log: false);
            Core.KillMonster("sevencircles", "r6", "Left", "Gluttony", "Essence of Gluttony", essenceTarget, isTemp: false, log: false);
            Core.KillMonster("sevencircles", "r8", "Left", "Avarice", "Essence of Avarice", essenceTarget, isTemp: false, log: false);

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