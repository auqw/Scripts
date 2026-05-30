/*
name: Greatblade of the Entwined Eclipse (Full Chain)
description: End-to-end farm for Greatblade of the Entwined Eclipse. Runs Victor Matsuri storyline, kills Masakado via the King's Echo army flow for Victor of the Festival, farms Slivers of Sunlight/Moonlight, merges Rite of Ascension, farms Ecliptic Offering in Ascension of the Eclipse, then merges the full blade chain up to the final Greatblade.
tags: greatblade, entwined, eclipse, full, chain, victor, matsuri, masakado, rite, ascension, midnight, solstice, ascend, army
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/Ultras/Entwined Eclipse/VictorMatsuriStory.cs
//cs_include Scripts/Ultras/Entwined Eclipse/Masakado.cs
//cs_include Scripts/Ultras/Entwined Eclipse/MidnightSun.cs
//cs_include Scripts/Ultras/Entwined Eclipse/SolsticeMoon.cs
//cs_include Scripts/Ultras/Entwined Eclipse/AscensionoftheEclipse.cs 
using Skua.Core.Interfaces;
using Skua.Core.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class GreatbladeOfTheEntwinedEclipse
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    public CoreUltra Ultra = new();
    private static CoreArmyLite sArmy
    {
        get => _sArmy ??= new CoreArmyLite();
        set => _sArmy = value;
    }
    private static CoreArmyLite _sArmy;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "GreatbladeEntwinedEclipse";
    // ─────────────────────────────────────────────────────────────────────────
    // CLASS REQUIREMENTS PER ACCOUNT
    //
    // Each slot below is an AQW account name. The same account is always that
    // slot across every phase, but the class it equips changes per phase.
    // Make sure each account actually OWNS every class listed for its slot —
    // missing classes will either fall back to a weaker pick or stop the bot.
    //
    //   Slot 1 (player1) needs:  King's Echo  AND  Legion Revenant
    //                            └ King's Echo is used for Masakado.
    //                            └ Legion Revenant is used in all 3 dungeons.
    //
    //   Slot 2 (player2) needs:  StoneCrusher
    //                            └ Used for Masakado AND all 3 dungeons.
    //
    //   Slot 3 (player3) needs:  ArchPaladin
    //                            └ Used for Masakado (via fallback) AND all 3 dungeons.
    //
    //   Slot 4 (player4) needs:  Lord of Order
    //                            └ Used for Masakado AND all 3 dungeons.
    //
    // ─────────────────────────────────────────────────────────────────────────
    public List<IOption> Options = new()
    {
        new Option<string>("player1", "King's Echo + Legion Revenant", "AQW account name for the King's Echo + Legion Revenant slot. This account MUST own BOTH classes: King's Echo is used for Masakado, Legion Revenant is used for the dungeons.", ""),
        new Option<string>("player2", "StoneCrusher", "AQW account name for the StoneCrusher slot. This account MUST own StoneCrusher for Masakado and all dungeon phases.", ""),
        new Option<string>("player3", "ArchPaladin", "AQW account name for the ArchPaladin slot. This account MUST own ArchPaladin for the dungeon phases; Masakado can also fall back to it.", ""),
        new Option<string>("player4", "Lord Of Order", "AQW account name for the Lord Of Order slot. This account MUST own Lord Of Order for Masakado and all dungeon phases.", ""),
        sArmy.packetDelay,
        CoreBots.Instance.SkipOptions,
        new Option<bool>("autoEnhance", "Auto-Apply Enhancements", "ON: each sub-script auto-applies the correct enhancements for its assigned class.", true),
        new Option<bool>("autoGetEnrage", "Auto-Craft Scroll of Enrage", "ON: each sub-script auto-crafts Scroll of Enrage before farming. Requires SpellCrafting rank 5.", true),
        new Option<bool>("tauntSanityCheck", "Check Usable Slot Before Boss Rooms", "Forwarded to AscensionoftheEclipse. ON: tests usable/potion slot before r3.", true),
        new Option<bool>("armyMode", "Army Mode (4 accounts)", "Forwarded to Masakado. ON: run Masakado as a 4-account army.", true),
        new Option<bool>("allowFallbackClass", "Masakado: Fallback If No King's Echo", "Forwarded to Masakado.", true),
        new Option<bool>("useRevitalize", "Masakado: Use Potent Revitalize Elixir", "Forwarded to Masakado.", true),
        new Option<bool>("buyRevitalize", "Masakado: Buy Revitalize Elixir", "Forwarded to Masakado.", true),
        new Option<bool>("skipVictorMatsuri", "Skip Victor Matsuri Storyline", "ON: skip the Victor Matsuri storyline (use this if all 4 accounts already have Victor of the Festival).", false),
    };

    // ── Items / shop ─────────────────────────────────────────────────────────
    const string VictorOfTheFestival = "Victor of the Festival";
    const string SliverSunlight      = "Sliver of Sunlight";
    const string SliverMoonlight     = "Sliver of Moonlight";
    const string EclipticOffering    = "Ecliptic Offering";
    const string RiteOfAscension     = "Rite of Ascension";

    const string Solarbrand          = "Solarbrand";
    const string Lunarbrand          = "Lunarbrand";
    const string Umbrabrand          = "Umbrabrand";
    const string BladeBurningSun     = "Blade of the Burning Sun";
    const string BladeGlowingMoon    = "Blade of the Glowing Moon";
    const string BladeBoundEclipse   = "Blade of the Bound Eclipse";
    const string GreatMidnightSun    = "Greatblade of the Midnight Sun";
    const string GreatSolsticeMoon   = "Greatblade of the Solstice Moon";
    const string GreatEntwinedEclipse = "Greatblade of the Entwined Eclipse";

    const string MergeMap  = "templeshrine";
    const int    MergeShop = 2303;

    const int ID_RiteOfAscension      = 78809;
    const int ID_Solarbrand           = 78465;
    const int ID_Lunarbrand           = 78460;
    const int ID_Umbrabrand           = 78455;
    const int ID_BladeBurningSun     = 78466;
    const int ID_BladeGlowingMoon    = 78461;
    const int ID_BladeBoundEclipse   = 78456;
    const int ID_GreatMidnightSun    = 78467;
    const int ID_GreatSolsticeMoon   = 78462;
    const int ID_GreatEntwinedEclipse = 78457;

    // Full-chain material totals from a clean inventory:
    // Sun/Moon: 3× basic brand (5 ea) + 2× middle blade (50 ea) + 1× greatblade (100) + 1× Rite = 216.
    // Ecliptic Offering: Umbrabrand (5) + Blade Bound Eclipse (50) + final Greatblade (100) = 155.
    // The script now calculates the remaining subset dynamically from current inventory.

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        try
        {
            if (Core.CheckInventory(GreatEntwinedEclipse))
            {
                Core.Logger($"{Core.Username()} already owns {GreatEntwinedEclipse} — nothing to do.");
                return;
            }

            Core.AddDrop(VictorOfTheFestival, SliverSunlight, SliverMoonlight, EclipticOffering);

            Phase1_VictorOfTheFestival();
            if (Bot.ShouldExit) return;

            Phase2_FarmSlivers();
            if (Bot.ShouldExit) return;

            Phase3_MergeRiteOfAscension();
            if (Bot.ShouldExit) return;

            Phase4_FarmEclipticOfferings();
            if (Bot.ShouldExit) return;

            Phase5_MergeFinalBlade();

            Core.Logger($"Done — {Core.Username()} now owns {GreatEntwinedEclipse}.");
        }
        finally
        {
            Core.SetOptions(false);
        }
    }

    // ── Phase 1: Victor of the Festival ───────────────────────────────────────
    //
    //   1a. VictorMatsuriStory.Storyline() runs quests 10290–10294 only. It self-skips
    //       per account if the relevant quests are already complete.
    //   1b. Army-level needs check: every account publishes whether it owns Victor.
    //       If *any* account is missing it, ALL four run Masakado together (quest
    //       10295 is repeatable, so the haves can re-fight without breaking).
    //       If every account already has Victor, ALL four skip the fight.
    //   1c. MasakadoKingsEchoArmy.ScriptMain handles quest 10295 (accept → kill → complete).
    // The original VictorMatsuri.cs is intentionally bypassed because its built-in
    // Masakado kill is unreliable.
    void Phase1_VictorOfTheFestival()
    {
        if (Bot.Config!.Get<bool>("skipVictorMatsuri"))
        {
            Core.Logger("[Phase 1] Skipping Victor Matsuri storyline by user option.");
            return;
        }

        Core.Logger("[Phase 1a] Running Victor Matsuri storyline (quests 10290–10294)...");
        new VictorMatsuriStory().Storyline();

        bool anyoneNeeds = ArmyAnyoneMissing(VictorOfTheFestival, "phase1_victor_needs.sync");
        if (!anyoneNeeds)
        {
            Core.Logger($"[Phase 1] Every account already owns {VictorOfTheFestival}; skipping Masakado for the whole army.");
            return;
        }

        Core.Logger($"[Phase 1b] At least one account is missing {VictorOfTheFestival}; whole army fights Masakado (quest 10295 is repeatable).");
        new MasakadoKingsEchoArmy { SkipSetOptions = true }.ScriptMain(Bot);

        if (!Core.CheckInventory(VictorOfTheFestival))
        {
            Core.Logger("[Phase 1] Masakado did not produce Victor of the Festival; stopping.", stopBot: true);
        }
    }

    /// <summary>
    /// Army-level "anyone missing this item?" check. Every account writes its own
    /// has/needs status to a shared sync file; once all party members have written,
    /// every account returns the same answer based on the aggregate.
    /// Returns true if at least one account is missing the item.
    /// </summary>
    bool ArmyAnyoneMissing(string item, string syncFile)
    {
        int partySize = sArmy.Players().Length;
        if (partySize <= 1)
            return !Core.CheckInventory(item);

        string path = Ultra.ResolveSyncPath($"GreatbladeEntwined_{Core.PrivateRoomNumber}_{syncFile}");
        string username = Core.Username().ToLower();
        string status = Core.CheckInventory(item) ? "has" : "needs";
        long writeNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long pruneCutoff = writeNow - 120;

        string[] lines = ReadSyncLines(path)
            .Where(l => !l.StartsWith($"{username}:", StringComparison.OrdinalIgnoreCase))
            .Where(l =>
            {
                var p = l.Split(':');
                return p.Length < 3 || !long.TryParse(p[2], out long ts) || ts >= pruneCutoff;
            })
            .ToArray();

        WriteSyncLines(path, lines.Append($"{username}:{status}:{writeNow}").ToArray());

        while (!Bot.ShouldExit)
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var fresh = ReadSyncLines(path)
                .Select(l => l.Split(':'))
                .Where(p => p.Length >= 3 && long.TryParse(p[2], out long ts) && now - ts <= 120)
                .GroupBy(p => p[0].ToLower())
                .Select(g => g.Last())
                .ToList();

            if (fresh.Count >= partySize)
                return fresh.Any(p => string.Equals(p[1], "needs", StringComparison.OrdinalIgnoreCase));

            Bot.Sleep(250);
        }

        return false;
    }

    string[] ReadSyncLines(string path)
    {
        try
        {
            return File.Exists(path)
                ? File.ReadAllLines(path).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray()
                : Array.Empty<string>();
        }
        catch { Bot.Sleep(100); return Array.Empty<string>(); }
    }

    void WriteSyncLines(string path, string[] lines)
    {
        try
        {
            string? dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(dir)) Directory.CreateDirectory(dir);
            File.WriteAllLines(path, lines);
        }
        catch { Bot.Sleep(100); }
    }

    // ── Phase 2: Farm Slivers (Sun + Moon) ────────────────────────────────────
    void Phase2_FarmSlivers()
    {
        CraftPlan plan = BuildRemainingCraftPlan();
        int sunlightNeeded = plan.Sunlight;
        int moonlightNeeded = plan.Moonlight;

        if (sunlightNeeded <= 0)
        {
            Core.Logger($"[Phase 2] No additional {SliverSunlight} needed for the remaining chain.");
        }
        else if (Core.CheckInventory(SliverSunlight, sunlightNeeded))
        {
            Core.Logger($"[Phase 2] Already have ≥{sunlightNeeded} {SliverSunlight}; skipping MidnightSun farm.");
        }
        else
        {
            Core.Logger($"[Phase 2] Farming {SliverSunlight} in /midnightsun until inventory ≥ {sunlightNeeded}.");
            var ms = new MidnightSunTest { TargetSliverCount = sunlightNeeded, SkipSetOptions = true };
            ms.ScriptMain(Bot);
        }

        if (Bot.ShouldExit) return;

        if (moonlightNeeded <= 0)
        {
            Core.Logger($"[Phase 2] No additional {SliverMoonlight} needed for the remaining chain.");
        }
        else if (Core.CheckInventory(SliverMoonlight, moonlightNeeded))
        {
            Core.Logger($"[Phase 2] Already have ≥{moonlightNeeded} {SliverMoonlight}; skipping SolsticeMoon farm.");
        }
        else
        {
            Core.Logger($"[Phase 2] Farming {SliverMoonlight} in /solsticemoon until inventory ≥ {moonlightNeeded}.");
            var sm = new SolsticeMoonTest { TargetSliverCount = moonlightNeeded, SkipSetOptions = true };
            sm.ScriptMain(Bot);
        }
    }

    // ── Phase 3: Merge Rite of Ascension ──────────────────────────────────────
    // Recipe (templeshrine #2303, item 78809): 1× Victor of the Festival + 1× Sliver of Sunlight + 1× Sliver of Moonlight
    void Phase3_MergeRiteOfAscension()
    {
        if (Core.CheckInventory(RiteOfAscension))
        {
            Core.Logger($"[Phase 3] {RiteOfAscension} already owned; skipping merge.");
            return;
        }

        Core.Logger($"[Phase 3] Merging {RiteOfAscension} at /{MergeMap} shop {MergeShop}.");
        Core.BuyItem(MergeMap, MergeShop, RiteOfAscension, 1, ID_RiteOfAscension);

        if (!Core.CheckInventory(RiteOfAscension))
        {
            Core.Logger($"[Phase 3] Failed to obtain {RiteOfAscension}; stopping.", stopBot: true);
        }
    }

    // ── Phase 4: Farm Ecliptic Offerings ──────────────────────────────────────
    void Phase4_FarmEclipticOfferings()
    {
        int eclipticOfferingNeeded = BuildRemainingCraftPlan().EclipticOffering;

        if (eclipticOfferingNeeded <= 0)
        {
            Core.Logger($"[Phase 4] No additional {EclipticOffering} needed for the remaining chain.");
            return;
        }

        if (Core.CheckInventory(EclipticOffering, eclipticOfferingNeeded))
        {
            Core.Logger($"[Phase 4] Already have ≥{eclipticOfferingNeeded} {EclipticOffering}; skipping AscensionEclipse farm.");
            return;
        }

        Core.Logger($"[Phase 4] Farming {EclipticOffering} in /ascendeclipse until inventory ≥ {eclipticOfferingNeeded}.");
        var ae = new AscendEclipseTest { TargetEclipticOfferingCount = eclipticOfferingNeeded, SkipSetOptions = true };
        ae.ScriptMain(Bot);
    }

    // ── Phase 5: Merge full blade chain ───────────────────────────────────────
    //
    // Chain is dependency-planned from current inventory. Existing higher-tier
    // weapons count as already-consuming their lower-tier components, so we only
    // craft the missing copies still needed by later merges.
    void Phase5_MergeFinalBlade()
    {
        CraftPlan plan = BuildRemainingCraftPlan();
        Core.Logger($"[Phase 5] Merging remaining blade chain at /{MergeMap} shop {MergeShop}.");

        MergeCrafts(plan, Solarbrand,        ID_Solarbrand);
        MergeCrafts(plan, Lunarbrand,        ID_Lunarbrand);
        MergeCrafts(plan, BladeBurningSun,   ID_BladeBurningSun);
        MergeCrafts(plan, BladeGlowingMoon,  ID_BladeGlowingMoon);
        MergeCrafts(plan, GreatMidnightSun,  ID_GreatMidnightSun);
        MergeCrafts(plan, GreatSolsticeMoon, ID_GreatSolsticeMoon);

        MergeCrafts(plan, Umbrabrand,           ID_Umbrabrand);
        MergeCrafts(plan, BladeBoundEclipse,    ID_BladeBoundEclipse);
        MergeCrafts(plan, GreatEntwinedEclipse, ID_GreatEntwinedEclipse);
    }

    void MergeCrafts(CraftPlan plan, string itemName, int shopItemID)
    {
        int toCraft = plan.Crafts.TryGetValue(itemName, out int count) ? count : 0;
        if (toCraft <= 0)
        {
            Core.Logger($"[Phase 5] {itemName}: no missing craft needed.");
            return;
        }

        int before = Bot.Inventory.GetQuantity(itemName);
        Core.Logger($"[Phase 5] Merging {toCraft}× {itemName} ({before} currently owned)...");
        Core.BuyItem(MergeMap, MergeShop, itemName, toCraft, shopItemID);

        int after = Bot.Inventory.GetQuantity(itemName);
        if (after < before + toCraft)
        {
            Core.Logger($"[Phase 5] Failed to merge {toCraft}× {itemName} (had {before}, now have {after}); stopping.", stopBot: true);
        }
    }

    CraftPlan BuildRemainingCraftPlan()
    {
        var plan = new CraftPlan();

        RequireItem(GreatEntwinedEclipse, 1, plan);

        if (!Core.CheckInventory(RiteOfAscension))
        {
            plan.Sunlight += 1;
            plan.Moonlight += 1;
        }

        Core.Logger(
            $"[Planner] Remaining materials: {plan.Sunlight} {SliverSunlight}, " +
            $"{plan.Moonlight} {SliverMoonlight}, {plan.EclipticOffering} {EclipticOffering}."
        );

        return plan;
    }

    void RequireItem(string itemName, int quantityNeeded, CraftPlan plan)
    {
        if (quantityNeeded <= 0)
            return;

        int available = plan.Available.TryGetValue(itemName, out int cached)
            ? cached
            : Bot.Inventory.GetQuantity(itemName);

        int usedFromInventory = Math.Min(available, quantityNeeded);
        plan.Available[itemName] = available - usedFromInventory;

        int toCraft = quantityNeeded - usedFromInventory;
        if (toCraft <= 0)
            return;

        AddCraft(plan, itemName, toCraft);
        AddRecipeRequirements(itemName, toCraft, plan);
    }

    void AddCraft(CraftPlan plan, string itemName, int count)
    {
        if (!plan.Crafts.ContainsKey(itemName))
            plan.Crafts[itemName] = 0;

        plan.Crafts[itemName] += count;
    }

    void AddRecipeRequirements(string itemName, int count, CraftPlan plan)
    {
        switch (itemName)
        {
            case Solarbrand:
                plan.Sunlight += 5 * count;
                break;

            case Lunarbrand:
                plan.Moonlight += 5 * count;
                break;

            case BladeBurningSun:
                RequireItem(Solarbrand, count, plan);
                plan.Sunlight += 50 * count;
                break;

            case BladeGlowingMoon:
                RequireItem(Lunarbrand, count, plan);
                plan.Moonlight += 50 * count;
                break;

            case GreatMidnightSun:
                RequireItem(BladeBurningSun, count, plan);
                plan.Sunlight += 100 * count;
                break;

            case GreatSolsticeMoon:
                RequireItem(BladeGlowingMoon, count, plan);
                plan.Moonlight += 100 * count;
                break;

            case Umbrabrand:
                RequireItem(Solarbrand, count, plan);
                RequireItem(Lunarbrand, count, plan);
                plan.EclipticOffering += 5 * count;
                break;

            case BladeBoundEclipse:
                RequireItem(BladeBurningSun, count, plan);
                RequireItem(BladeGlowingMoon, count, plan);
                RequireItem(Umbrabrand, count, plan);
                plan.EclipticOffering += 50 * count;
                break;

            case GreatEntwinedEclipse:
                RequireItem(GreatMidnightSun, count, plan);
                RequireItem(GreatSolsticeMoon, count, plan);
                RequireItem(BladeBoundEclipse, count, plan);
                plan.EclipticOffering += 100 * count;
                break;
        }
    }

    class CraftPlan
    {
        public Dictionary<string, int> Crafts { get; } = new();
        public Dictionary<string, int> Available { get; } = new(StringComparer.OrdinalIgnoreCase);
        public int Sunlight;
        public int Moonlight;
        public int EclipticOffering;
    }
}
