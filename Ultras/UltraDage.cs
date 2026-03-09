/*
name: UltraDage
description: Two-taunter strategy for Ultra Dage with aura-based taunting and army synchronization.
tags: Ultra

Fight notes:
- Composition order is [slot 1-4]: BestAvailable = CAv / AP / DPS / DPS.
- Fixed taunter slots are slot 1 and slot 2.
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraDage
{
    private CoreBots C => CoreBots.Instance;
    public IScriptInterface Bot => IScriptInterface.Instance;
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
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraDage";
    public string[] MultiOptions = { "Main", "CoreSettings", "ClassOverrides" };

    public List<IOption> Main = new()
    {
        new Option<DageComp>(
            "DoEquipClasses",
            "Automatically Equip Classes",
            "Auto-equip classes across all 4 clients\n"
                + "BestAvailable: CAv / AP / Best DPS / Best DPS\n"
                + "Unselected = off (use current classes).",
            DageComp.BestAvailable
        ),
        CoreBots.Instance.SkipOptions,
    };

    public List<IOption> CoreSettings = new()
    {
        new Option<bool>("EquipBestGear", "Equip Best Gear", "Equip best gear for encounter", true),
        new Option<bool>("DoEnh", "Do Enhancements", "Auto-Enhance Gear properly for the fight", true),
        new Option<bool>("RestoreGear", "Restore Gear", "Restore original gear after the script finishes", false),
        new Option<bool>("UseLifeSteal", "Use LifeSteal", "Non-taunters equip/restock/use Scroll of Life Steal.", true),
    };

    public List<IOption> ClassOverrides = new()
    {
        new Option<string>("a", "Primary Class Override (T)", "Blank = use selected comp default for slot 1 (taunter).", ""),
        new Option<string>("b", "Secondary Class Override (T)", "Blank = use selected comp default for slot 2 (taunter).", ""),
        new Option<string>("c", "Tertiary Class Override", "Blank = use selected comp default for slot 3 (dps).", ""),
        new Option<string>("d", "Quaternary Class Override", "Blank = use selected comp default for slot 4 (dps).", ""),
    };

    private string tauntSlot1 = "chaos avenger";
    private string tauntSlot2 = "archpaladin";
    private string overrideA = string.Empty;
    private string overrideB = string.Empty;
    private string overrideC = string.Empty;
    private string overrideD = string.Empty;
    private DageComp ActiveComp = DageComp.BestAvailable;
    private bool EquipBestGear;
    private bool DoEnhancements;
    private bool RestoreGear;
    private bool UseLifeSteal;

    private string NormalizeString(string input) => (input ?? "").Trim().ToLower();

    public void ScriptMain(IScriptInterface bot)
    {
        if (!C.isCompletedBefore(793))
            C.Logger("player is not part of the legion, you may not be able to turn in.");

        C.Join("whitemap");

        if (Bot.Config != null && !Bot.Config.Get<bool>("Main", "SkipOption"))
            Bot.Config.Configure();

        ActiveComp = Bot.Config == null ? DageComp.BestAvailable : Bot.Config.Get<DageComp>("Main", "DoEquipClasses");
        overrideA = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "a") ?? string.Empty)).Trim();
        overrideB = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "b") ?? string.Empty)).Trim();
        overrideC = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "c") ?? string.Empty)).Trim();
        overrideD = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "d") ?? string.Empty)).Trim();
        EquipBestGear = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "EquipBestGear");
        DoEnhancements = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "DoEnh");
        RestoreGear = Bot.Config == null ? false : Bot.Config.Get<bool>("CoreSettings", "RestoreGear");
        UseLifeSteal = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "UseLifeSteal");

        Adv.GearStore();
        try
        {
            Run(ActiveComp, EquipBestGear, DoEnhancements, UseLifeSteal, overrideA, overrideB, overrideC, overrideD);
        }
        finally
        {
            if (RestoreGear)
                Adv.GearStore(true, true);
            C.SetOptions(false);
            Bot.StopSync();
        }
    }

    public void Run(
        DageComp comp = DageComp.BestAvailable,
        bool equipBestGear = true,
        bool doEnhancements = true,
        bool useLifeSteal = true,
        string? classAOverride = null,
        string? classBOverride = null,
        string? classCOverride = null,
        string? classDOverride = null
    )
    {
        ActiveComp = comp;
        EquipBestGear = equipBestGear;
        DoEnhancements = doEnhancements;
        UseLifeSteal = useLifeSteal;
        overrideA = classAOverride?.Trim() ?? string.Empty;
        overrideB = classBOverride?.Trim() ?? string.Empty;
        overrideC = classCOverride?.Trim() ?? string.Empty;
        overrideD = classDOverride?.Trim() ?? string.Empty;

        Core.Boot();
        Prep();
        Bot.Events.ExtensionPacketReceived += UltraDageListener;
        try
        {
            Fight();
        }
        finally
        {
            Bot.Events.ExtensionPacketReceived -= UltraDageListener;
        }
    }

    bool IsTaunter() => Core.HasClassEquipped(tauntSlot1) || Core.HasClassEquipped(tauntSlot2);

    void Prep()
    {
        Bot.Quests.UpdateQuest(793);
        ApplyCompAndEquip(ActiveComp, overrideA, overrideB, overrideC, overrideD);

        if (EquipBestGear)
            EquipBestDmgGear();

        if (DoEnhancements)
            DoEnh();

        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");

        if (IsTaunter())
            Ultra.GetScrollOfEnrage();
        else if (UseLifeSteal)
            Ultra.GetScrollOfLifeSteal();
    }

    void ApplyCompAndEquip(DageComp comp, string aOverride, string bOverride, string cOverride, string dOverride)
    {
        if (comp == DageComp.Unselected)
        {
            tauntSlot1 = NormalizeString(string.IsNullOrWhiteSpace(aOverride) ? "Chaos Avenger" : aOverride);
            tauntSlot2 = NormalizeString(string.IsNullOrWhiteSpace(bOverride) ? "ArchPaladin" : bOverride);
            return;
        }

        string[] dpsOptions =
        {
            "Lich", "Legion Revenant", "Great Thief", "Hollowborn Vindicator", "Quantum Chronomancer", "Phantom Chronomancer",
            "Verus DoomKnight", "King's Echo", "Arachnomancer", "Archfiend", "Infinity Knight", "StoneCrusher"
        };

        string[][] classes = new[]
        {
            new[] { string.IsNullOrWhiteSpace(aOverride) ? "Chaos Avenger" : aOverride },
            new[] { string.IsNullOrWhiteSpace(bOverride) ? "ArchPaladin" : bOverride },
            string.IsNullOrWhiteSpace(cOverride) ? dpsOptions : new[] { cOverride },
            string.IsNullOrWhiteSpace(dOverride) ? dpsOptions : new[] { dOverride }
        };

        tauntSlot1 = NormalizeString(classes[0][0]);
        tauntSlot2 = NormalizeString(classes[1][0]);
        Ultra.EquipClassSync(classes, 4, "dage_class.sync");
    }

    void EquipBestDmgGear()
    {
        C.EquipBestItemsForMeta(
            new Dictionary<string, string[]>
            {
                { "Weapon", new[] { "dmgAll", "dmg", "damage" } },
                { "Armor", new[] { "dmgAll", "dmg", "damage" } },
                { "Helm", new[] { "dmgAll", "dmg", "damage" } },
                { "Cape", new[] { "dmgAll", "dmg", "damage" } },
                { "Pet", new[] { "dmgAll", "dmg", "damage" } },
            }
        );
    }

    void Fight()
    {
        const string map = "ultradage";
        const string boss = "Dage the Dark Lord";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);
        C.AddDrop("Dage the Evil Insignia");
        C.EnsureAccept(8547);

        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_dage.sync");
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        while (!Bot.ShouldExit)
        {
            if (Bot.Map?.Name != map)
            {
                Core.Join(map);
                Core.ChooseBestCell(boss);
                Bot.Player.SetSpawnPoint();
            }

            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => C.CheckInventory("Dage the Dark Lord Defeated", 1), syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                if (!Bot.Quests.IsDailyComplete(8547))
                    C.EnsureComplete(8547);
                break;
            }

            if (UseLifeSteal && !IsTaunter() && Bot.Skills.CanUseSkill(5))
                Bot.Skills.UseSkill(5);

            if (Core.HasClassEquipped(tauntSlot1) || Core.HasClassEquipped(tauntSlot2) && !Bot.Target.Auras.Any(a => a.Name == "Focus"))
            {
                if (Bot.Skills.CanUseSkill(5))
                    Bot.Skills.UseSkill(5);
            }

            Bot.Sleep(500);

            if (!Bot.Player.HasTarget)
                Bot.Combat.Attack(boss);
        }
    }

    public async void UltraDageListener(dynamic packet)
    {
        if (packet?["params"]?.type?.ToString() != "json")
            return;
        if (!Bot.Player.Alive)
            return;
        dynamic data = packet["params"].dataObj;
        if (data?.cmd?.ToString() != "event")
            return;

        string? zoneSet = data?.args?.zoneSet?.ToString();
        if (string.IsNullOrEmpty(zoneSet))
            return;

        if (string.Equals(zoneSet, "A", StringComparison.OrdinalIgnoreCase))
        {
            _ = Task.Run(() => Bot.Player.WalkTo(122, 420));
            return;
        }

        if (string.Equals(zoneSet, "B", StringComparison.OrdinalIgnoreCase))
        {
            _ = Task.Run(() => Bot.Player.WalkTo(856, 420));
            return;
        }
    }

    void DoEnh()
    {
        string className = Bot.Player!.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        switch (className.ToLower())
        {
            case "chaos avenger":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Anima, wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp, cSpecial: CapeSpecial.Vainglory);
                break;
            case "archpaladin":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Forge, wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp, cSpecial: CapeSpecial.Lament);
                break;
            case "legion revenant":
                Adv.EnhanceEquipped(type: EnhancementType.Wizard, hSpecial: HelmSpecial.Pneuma, wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp, cSpecial: CapeSpecial.Vainglory);
                break;
            case "archfiend":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Forge, wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp, cSpecial: CapeSpecial.Vainglory);
                break;
            case "arachnomancer":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Anima, wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp, cSpecial: CapeSpecial.Vainglory);
                break;
            case "king's echo":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Pneuma, wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp, cSpecial: CapeSpecial.Lament);
                break;
            case "chrono shadowslayer":
            case "chrono shadowhunter":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Vim, wSpecial: WeaponSpecial.Health_Vamp, cSpecial: CapeSpecial.Lament);
                break;
            case "quantum chronomancer":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Anima, wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp, cSpecial: CapeSpecial.Vainglory);
                break;
            case "phantom chronomancer":
            case "phantasm chronomancer":
                Adv.EnhanceEquipped(type: EnhancementType.Wizard, hSpecial: HelmSpecial.Pneuma, wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp, cSpecial: CapeSpecial.Vainglory);
                break;
            case "infinity knight":
                Adv.EnhanceEquipped(type: EnhancementType.Wizard, hSpecial: HelmSpecial.Pneuma, wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp, cSpecial: CapeSpecial.Vainglory);
                break;
            case "lich":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Examen, wSpecial: WeaponSpecial.Ravenous, cSpecial: CapeSpecial.Penitence);
                break;
            case "verus doomknight":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Anima, wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp, cSpecial: CapeSpecial.Vainglory);
                break;
            case "hollowborn vindicator":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Forge, wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp, cSpecial: CapeSpecial.Penitence);
                break;
            case "great thief":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Forge, wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp, cSpecial: CapeSpecial.Vainglory);
                break;
            case "stonecrusher":
                Adv.EnhanceEquipped(type: EnhancementType.Wizard, hSpecial: HelmSpecial.Pneuma, wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp, cSpecial: CapeSpecial.Vainglory);
                break;
        }
    }

    public enum DageComp
    {
        Unselected,
        BestAvailable,
    }
}
