/*
name: UltraAvatarTyndarius
description: Ultra Avatar Tyndarius helper with taunter rotation and orb priority.
tags: Ultra

Fight notes:
- Composition order is [slot 1-4]: Safe = CAv / LR / AP / LOO, Fast = CSS / LR / AP / LOO, F2PFast = KE / LR / AP / LOO.
- Default composition is set to Safe when available.
- Fixed taunt-role assignment is based on the resolved comp slots:
  - Slot 2: Ball 1 Taunt
  - Slot 3: Must-Taunt Tyndarius
  - Slot 4: Focus Tyndarius
- Slot 1 remains Ball 2 support/killer.
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

/*
============================================================================

Enhancements are in order of (Helm - Class - Weapon - Cape)

Safe Comp (HIGHLY RECOMMENDED):
// CAV (anima / lucky / valiance / vainglory)
// LR  (pneuma / wizard / valiance-ravenous-arcana / vainglory)
// AP  (forge / lucky / valiance / lament)
// LOO (forge / lucky / valiance / absolution)


Fast Comp:
// CSS (vim / lucky / valiance / vainglory-lament)
// LR  (pneuma / wizard / valiance-ravenous-arcana / vainglory)
// AP  (forge / lucky / valiance / lament)
// LOO (forge / lucky / lucky-aweblast-valiance / absolution)


Fast F2P:
// KE  (examen / lucky / ravenous / vainglory)
// LR  (pneuma / wizard / valiance-ravenous-arcana / vainglory)
// AP  (forge / lucky / valiance / lament)
// LOO (forge / lucky / lucky-aweblast-valiance / absolution)


Other Dps Options:
// AI   (examen / lucky / ravenous-valiance / vainglory)
// LICH (examen / lucky / ravenous / penitence)
// VDK  (anima / lucky / ravenous-valiance / vainglory)
// DOT  (pneuma / wizard / elysium / vainglory)

============================================================================
*/

public class UltraAvatarTyndarius
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

    bool isBall2killer;
    bool isBall1Taunter;
    bool isMustTauntTyn;
    bool isFocusTyn;
    string tauntSlot1 = "Chaos Avenger";
    string tauntSlot2 = "Legion Revenant";
    string tauntSlot3 = "ArchPaladin";
    string tauntSlot4 = "Lord Of Order";

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraAvatarTyndarius3";

    string a,
        b,
        c,
        d,
        overrideA,
        overrideB,
        overrideC,
        overrideD;
    bool UseLifeSteal;
    bool EquipBestGear;
    bool DoEnhancements;
    bool RestoreGear;
    TyndariusComp ActiveComp = TyndariusComp.Safe;
    public string[] MultiOptions = { "Main", "CoreSettings", "ClassOverrides" };

    public List<IOption> Main = new()
    {
        new Option<TyndariusComp>(
            "DoEquipClasses",
            "Automatically Equip Classes",
            "Auto-equip classes across all 4 clients\n"
                + "Safe: CAv / LR / AP / LOO\n"
                + "Fast: CSS / LR / AP / LOO\n"
                + "F2PFast: KE / LR / AP / LOO\n"
                + "Unselected = off (use whatever classes you already have equipped).",
            TyndariusComp.Safe
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
        new Option<string>("a", "Primary Class Override", "Blank = use selected comp default for slot 1.", ""),
        new Option<string>("b", "Secondary Class Override", "Blank = use selected comp default for slot 2.", ""),
        new Option<string>("c", "Tertiary Class Override", "Blank = use selected comp default for slot 3.", ""),
        new Option<string>("d", "Quaternary Class Override", "Blank = use selected comp default for slot 4.", ""),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        C.Join("whitemap");
        if (Bot.Config != null && !Bot.Config.Get<bool>("Main", "SkipOption"))
            Bot.Config.Configure();

        UseLifeSteal = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "UseLifeSteal");
        EquipBestGear = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "EquipBestGear");
        DoEnhancements = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "DoEnh");
        RestoreGear = Bot.Config == null ? false : Bot.Config.Get<bool>("CoreSettings", "RestoreGear");
        ActiveComp = Bot.Config == null ? TyndariusComp.Safe : Bot.Config.Get<TyndariusComp>("Main", "DoEquipClasses");

        overrideA = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "a") ?? string.Empty)).Trim();
        overrideB = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "b") ?? string.Empty)).Trim();
        overrideC = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "c") ?? string.Empty)).Trim();
        overrideD = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "d") ?? string.Empty)).Trim();

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
        TyndariusComp comp = TyndariusComp.Safe,
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
        a = overrideA;
        b = overrideB;
        c = overrideC;
        d = overrideD;

        Core.Boot();
        Prep();
        Fight();
    }

    void Prep()
    {
        // Sync-equip classes if a comp is selected
        ApplyCompAndEquip(ActiveComp, overrideA, overrideB, overrideC, overrideD);

        if (EquipBestGear)
            EquipBestDmgGear();

        if (DoEnhancements)
            DoEnh();
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");
        if (isBall1Taunter || isMustTauntTyn || isFocusTyn)
            Ultra.GetScrollOfEnrage();
        else if (UseLifeSteal)
            Ultra.GetScrollOfLifeSteal();
    }

    void EquipBestDmgGear()
    {
        C.EquipBestItemsForMeta(
            new Dictionary<string, string[]>
            {
                { "Weapon", new[] { "dmgAll", "dmg", "gold", "cp", "rep", "xp" } },
                { "Armor", new[] { "dmgAll", "dmg", "gold", "cp", "rep", "xp" } },
                { "Helm", new[] { "dmgAll", "dmg", "gold", "cp", "rep", "xp" } },
                { "Cape", new[] { "dmgAll", "dmg", "gold", "cp", "rep", "xp" } },
                { "Pet", new[] { "dmgAll", "dmg", "gold", "cp", "rep", "xp" } },
            }
        );
    }

    void ApplyCompAndEquip(TyndariusComp comp, string aOverride, string bOverride, string cOverride, string dOverride)
    {
        if (comp == TyndariusComp.Unselected)
            return;

        string[] classes = comp switch
        {
            TyndariusComp.Safe => new[]
            {
                string.IsNullOrWhiteSpace(aOverride) ? "Chaos Avenger" : aOverride,
                string.IsNullOrWhiteSpace(bOverride) ? "Legion Revenant" : bOverride,
                string.IsNullOrWhiteSpace(cOverride) ? "ArchPaladin" : cOverride,
                string.IsNullOrWhiteSpace(dOverride) ? "Lord Of Order" : dOverride,
            },
            TyndariusComp.Fast => new[]
            {
                string.IsNullOrWhiteSpace(aOverride) ? "Chrono ShadowSlayer" : aOverride,
                string.IsNullOrWhiteSpace(bOverride) ? "Legion Revenant" : bOverride,
                string.IsNullOrWhiteSpace(cOverride) ? "ArchPaladin" : cOverride,
                string.IsNullOrWhiteSpace(dOverride) ? "Lord Of Order" : dOverride,
            },
            TyndariusComp.F2PFast => new[]
            {
                string.IsNullOrWhiteSpace(aOverride) ? "King's Echo" : aOverride,
                string.IsNullOrWhiteSpace(bOverride) ? "Legion Revenant" : bOverride,
                string.IsNullOrWhiteSpace(cOverride) ? "ArchPaladin" : cOverride,
                string.IsNullOrWhiteSpace(dOverride) ? "Lord Of Order" : dOverride,
            },
            _ => throw new InvalidOperationException($"Unhandled TyndariusComp value: {comp}"),
        };

        tauntSlot1 = classes[0];
        tauntSlot2 = classes[1];
        tauntSlot3 = classes[2];
        tauntSlot4 = classes[3];

        Ultra.EquipClassSync(classes, 4, "tyndarius_class.sync");
        SetRoleAllocations();
    }

    void Fight()
    {
        const string map = "ultratyndarius";
        const string boss = "Ultra Avatar Tyndarius";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        C.AddDrop("Avatar Tyndarius Insignia");
        C.EnsureAccept(8245);
        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_tyndarius.sync");
        Core.ChooseBestCell(boss);
        Core.EnableSkills();

        while (!Bot.ShouldExit)
        {
            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (UseLifeSteal && Bot.Skills.CanUseSkill(5))
                Bot.Skills.UseSkill(5);

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Ultra Avatar Tyndarius Defeated", 1), syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                C.EnsureComplete(8245);

                break;
            }
            if (Bot.Map.Name != map)
            {
                Core.Join(map);
            }

            if (Bot.Player.Cell != "Boss")
            {
                Bot.Map.Jump("Boss", "Left", autoCorrect: false);
                Bot.Wait.ForCellChange("Boss");
            }
            // ======================================================
            // BALL 1 TAUNTER
            // ======================================================
            // Ball 1 = MID 1 | Left Ball
            // Ball 2 = MID 3 | Right Ball
            // Tynd   = MID 2
            bool Ball1alive = Bot.Monsters.CurrentAvailableMonsters.Any(x => x != null && x.Alive && x.MapID == 1);
            bool Ball2alive = Bot.Monsters.CurrentAvailableMonsters.Any(x => x != null && x.Alive && x.MapID == 3);
            bool BothDead = !Ball1alive && !Ball2alive;

            C.Logger($"Ball1: {Ball1alive} | Ball2: {Ball2alive} | BothDead: {BothDead}");

            if (isBall1Taunter)
            {
                if (BothDead)
                {
                    Bot.Combat.Attack(2);
                }
                else if (Ball1alive)
                {
                    Bot.Combat.Attack(1);
                }
                Bot.Sleep(500);
            }
            if (isBall2killer)
            {
                if (Ball2alive)
                {
                    Bot.Combat.Attack(3);
                }
                else if (Ball1alive)
                {
                    Bot.Combat.Attack(1);
                }
                else
                {
                    Bot.Combat.Attack(2);
                }
                Bot.Sleep(500);
            }
            // ======================================================
            // MUST TAUNT TYN (full tank)
            // ======================================================
            if (isMustTauntTyn)
            {
                Bot.Combat.Attack(2);
                Bot.Sleep(500);
            }
            // ======================================================
            // FOCUS TYN (semi-taunt)
            // ======================================================
            if (isFocusTyn)
            {
                Bot.Combat.Attack(2);
                Bot.Sleep(500);
            }
            if (Bot.ShouldExit)
                C.Jump("Enter", "Spawn");
        }
    }

    void SetRoleAllocations()
    {
        isBall1Taunter = Core.HasClassEquipped(tauntSlot2);
        isBall2killer = Core.HasClassEquipped(tauntSlot1);
        isMustTauntTyn = Core.HasClassEquipped(tauntSlot3);
        isFocusTyn = Core.HasClassEquipped(tauntSlot4);
    }

    void DoEnh()
    {
        string className = Bot.Player!.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;
        switch (className.ToLower())
        {
            case "chrono shadowslayer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Vim,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;
            case "legion revenant":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;
            case "archpaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Lament
                );
                break;
            case "lord of order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Absolution
                );
                break;
            case "chaos avenger":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;
            case "king's echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;
            case "arcana invoker":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;
            case "lich":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Penitence
                );
                break;
            case "verus doomknight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;
            case "dragon of time":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Elysium,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            default:
                Adv.SmartEnhance(Bot.Player.CurrentClass!.Name);
                break;
        }
    }

    public enum TyndariusComp
    {
        Unselected,
        Safe,
        Fast,
        F2PFast,
    }

}
