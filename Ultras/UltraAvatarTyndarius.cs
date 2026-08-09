/*
name: UltraAvatarTyndarius
description: Ultra Avatar Tyndarius helper with taunter rotation and orb priority.
tags: Ultra
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
using System.ComponentModel;
using System.Reflection;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Options;
using Skua.Core.Threading;

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
    public CoreEnginev1 Core = new();
    public CoreUltrav1 Ultra = new();

    private string NormalizeString(string input) => (input ?? "").Trim().ToLower();

    bool isBall2killer;
    bool isBall1Taunter;
    bool isMustTauntTyn;
    bool isFocusTyn;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraAvatarTyndarius3";
    public List<IOption> Options = new()
    {
        new Option<TyndariusComp>(
            "DoEquipClasses",
            "Automatically Equip Classes",
            "Auto-equip classes across all 4 clients\n"
                + "Safe: CAv / LR / AP / LOO\n"
                + "Fast: CSS / LR / AP / LOO\n"
                + "F2PFast: KE / LR / AP / LOO\n"
                + "Unselected = off (use whatever classes you already have equipped).",
            TyndariusComp.Unselected
        ),
        // Ball 1 Taunter selection
        new Option<Ball1Taunter>(
            "Ball1Taunter",
            "Ball 1 Taunter",
            "Select which class should taunt Ball 1.",
            Ball1Taunter.LegionRevenant
        ),
        // Ball 2 Taunter selection
        new Option<Ball2killer>(
            "Ball2killer",
            "Ball 2 killer",
            "Select which class should kill Ball 2.",
            Ball2killer.ChaosAvenger
        ),
        // Must Taunt Tyndarius selection
        new Option<MustTauntTyndarius>(
            "MustTauntTyndarius",
            "Must Taunt Tyndarius",
            "Select which class must taunt Tyndarius.",
            MustTauntTyndarius.ArchPaladin
        ),
        // Focus Tyndarius selection
        new Option<DebuffTyndarius>(
            "DebuffTyndarius",
            "Focus Tyndarius",
            "Select which class should focus Tyndarius.",
            DebuffTyndarius.LordOfOrder
        ),
        new Option<bool>("DoEnh", "Do Enhancements",  "Auto-Enhance Gear properly for the fight", true),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        C.Join("whitemap");
        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();

        if (
            NormalizeString(GetDescription(Bot.Config!.Get<Ball1Taunter>("Ball1Taunter"))) == "Dragon of Time"
            && NormalizeString(GetDescription(Bot.Config!.Get<Ball2killer>("Ball2killer"))) == "Dragon of Time"
        )
            C.Logger("Ball1Taunter & Ball2Killer are set to Dragon of Time, choose something else", "Fix This", true, true);

        Core.Boot();
        Prep();
        Fight();
        Bot.StopSync();
    }

    void Prep()
    {
        // Sync-equip classes if a comp is selected
        TyndariusComp comp = Bot.Config!.Get<TyndariusComp>("DoEquipClasses");
        if (comp != TyndariusComp.Unselected)
        {
            string[] classes = comp switch
            {
                TyndariusComp.Safe => new[] { "Chaos Avenger", "Legion Revenant", "ArchPaladin", "Lord of Order" },
                TyndariusComp.Fast => new[] { C.CheckInventory("Chrono ShadowSlayer") ? "Chrono ShadowSlayer" : "Chrono ShadowHunter", "Legion Revenant", "ArchPaladin", "Lord of Order" },
                TyndariusComp.F2PFast => new[] { "King's Echo", "Legion Revenant", "ArchPaladin", "Lord of Order" },
                _ => throw new InvalidOperationException($"Unhandled TyndariusComp value: {comp}"),
            };

            Ultra.EquipClassSync(classes, 4, "tyndarius_class.sync");
        }

        isBall1Taunter =
     NormalizeString(Bot.Player.CurrentClass!.Name)
     == NormalizeString(GetDescription(Bot.Config!.Get<Ball1Taunter>("Ball1Taunter")));
        isBall2killer =
            NormalizeString(Bot.Player.CurrentClass.Name)
            == NormalizeString(GetDescription(Bot.Config!.Get<Ball2killer>("Ball2killer")));
        isMustTauntTyn =
            NormalizeString(Bot.Player.CurrentClass.Name)
            == NormalizeString(GetDescription(Bot.Config!.Get<MustTauntTyndarius>("MustTauntTyndarius")));
        isFocusTyn =
            NormalizeString(Bot.Player.CurrentClass.Name)
            == NormalizeString(GetDescription(Bot.Config!.Get<DebuffTyndarius>("DebuffTyndarius")));

        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnh();
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        if (isBall1Taunter || isMustTauntTyn || isFocusTyn)
        {
            Ultra.GetScrollOfEnrage();
            Core.EquipEnrage();
        }
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
        var (bestCell, _) = Core.ChooseBestCell(boss);
         
        Core.EnableSkills();

        while (!Bot.ShouldExit)
        {
            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Bot.Player.Cell != bestCell)
            {
                Bot.Sleep(200);
                Bot.Map.Jump(bestCell);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Ultra Avatar Tyndarius Defeated", 1), syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");

                if (Bot.Config!.Get<bool>("DoEnh"))
                    Adv.GearStore(true, true);
                break;
            }
            if (Bot.Map.Name != map)
                Core.Join(map);

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
                    Bot.Wait.ForTrue(() => Bot.Player.Target?.MapID == 1, 20); // Wait for Ball 1 to be targeted
                }
                if (Bot.Skills.CanUseSkill(5))
                    Bot.Skills.UseSkill(5);
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
            }
            // ======================================================
            // MUST TAUNT TYN (full tank)
            // ======================================================
            if (isMustTauntTyn)
            {
                Bot.Combat.Attack(2);
                if (Bot.Skills.CanUseSkill(5))
                    Bot.Skills.UseSkill(5);
            }
            // ======================================================
            // FOCUS TYN (semi-taunt)
            // ======================================================
            if (isFocusTyn)
            {
                Bot.Combat.Attack(2);
            }
            if (Bot.ShouldExit)
                C.Jump("Enter", "Spawn");
        }
        Bot.Sleep(500);
    }

    public static string GetDescription(Enum value)
    {
        FieldInfo? field = value.GetType().GetField(value.ToString());
        DescriptionAttribute? attribute =
            field?.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault()
            as DescriptionAttribute;

        return attribute?.Description ?? value.ToString();
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

    public enum Ball2killer
    {
        // In order of fast > safe > f2p fast > other
        [Description("Chrono ShadowSlayer")]
        ChronoShadowSlayer,

        [Description("Chrono ShadowHunter")]
        ChronoShadowHunter,

        [Description("Chaos Avenger")]
        ChaosAvenger,

        [Description("King's Echo")]
        KingsEcho,

        [Description("StoneCrusher")]
        StoneCrusher,

        [Description("Arcana Invoker")]
        ArcanaInvoker,

        [Description("Dragon of Time")]
        DragonofTime,

        [Description("Current Class | Ball2killer")]
        Ball2killer_CurrentClass,
    }

    public enum Ball1Taunter
    {
        // In order of fast > safe > f2p fast > other
        [Description("Legion Revenant")]
        LegionRevenant,

        [Description("Lich")]
        Lich,

        [Description("Current Class | Ball1Taunter")]
        Ball1Taunter_Current,
    }

    public enum DebuffTyndarius
    {
        // In order of fast > safe > f2p fast > other
        [Description("Lord of Order")]
        LordOfOrder,

        [Description("Dragon of Time")]
        DragonofTime,

        [Description("Current Class | DebuffTyndarius")]
        DebuffTyndarius_Current,
    }

    public enum MustTauntTyndarius
    {
        // In order of fast > safe > f2p fast > other
        [Description("ArchPaladin")]
        ArchPaladin,

        [Description("Verus DoomKnight")]
        VerusDoomknight,

        [Description("Current Class | MustTauntTyndarius")]
        MustTauntTyndarius_Current,
    }


}
