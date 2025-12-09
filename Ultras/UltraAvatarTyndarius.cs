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
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    private string NormalizeString(string input) => (input ?? "").Trim().ToLower();

    string playerClass;
    bool isBall2killer;
    bool isBall1Taunter;
    bool isMustTauntTyn;
    bool isFocusTyn;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraAvatarTyndarius2";
    public List<IOption> Options = new()
    {
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
            Ball2killer.ChronoShadowHunter
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

        Bot.Options.InfiniteRange = true;
        playerClass = Bot.Player.CurrentClass?.Name ?? string.Empty;
        isBall1Taunter =
            Bot.Player.CurrentClass?.Name
            == GetDescription(Bot.Config!.Get<Ball1Taunter>("Ball1Taunter"));
        isBall2killer =
            Bot.Player.CurrentClass?.Name
            == GetDescription(Bot.Config!.Get<Ball2killer>("Ball2killer"));
        isMustTauntTyn =
            Bot.Player.CurrentClass?.Name
            == GetDescription(Bot.Config!.Get<MustTauntTyndarius>("MustTauntTyndarius"));
        isFocusTyn =
            Bot.Player.CurrentClass?.Name
            == GetDescription(Bot.Config!.Get<DebuffTyndarius>("DebuffTyndarius"));

        Core.Boot();
        Prep();
        Fight();
        Bot.Stop();
    }

    void Prep()
    {
        if (isBall1Taunter || isMustTauntTyn)
        {
            Bot.Log("isTaunter = true");
            Ultra.GetScrollOfEnrage();
        }
        else
        {
            Bot.Log("isTaunter = false");
            Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
            Ultra.BuyAlchemyPotion("Potent Honor Potion");
            Core.EquipConsumable("Potent Honor Potion");
        }
        // Ultra.Enhancements();
    }

    void Fight()
    {
        const string map = "ultratyndarius";
        const string boss = "Ultra Avatar Tyndarius";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(1500);

        C.AddDrop("Avatar Tyndarius Insignia");
        C.EnsureAccept(8245);
        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_tyndarius.sync");
        Core.EnableSkills();
        Core.ChooseBestCell(boss);

        while (!Bot.ShouldExit)
        {
            if (Ultra.CheckArmyProgress("Ultra Avatar Tyndarius Defeated", 1, true, syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                C.EnsureComplete(8245);
                Ultra.ClearSyncFile(syncPath);
                break;
            }

            if (!Bot.Player.Alive)
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

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
            if (isBall1Taunter)
            {
                Bot.Combat.Attack(Bot.Monsters.MapMonsters.Any(x => x != null && x.MapID == 1 && x.HP > 0) ? 1 : Bot.Monsters.MapMonsters.Any(x => x != null && x.MapID == 3 && x.HP > 0) ? 3 : 2);
                Bot.Sleep(500);
                Bot.Skills.UseSkill(5);
            }

            if (isBall2killer)
            {
                if (Bot.Monsters.MapMonsters.Any(x => x != null && x.MapID == 3 && x.HP > 0))
                {
                    Bot.Combat.Attack(3);
                    Bot.Sleep(500);
                    Bot.Skills.UseSkill(5);
                    Bot.Sleep(500);
                }
                else if (Bot.Monsters.MapMonsters.Any(x => x != null && x.MapID == 1 && x.HP > 0))
                {
                    Bot.Combat.Attack(1);
                    Bot.Sleep(500);
                    Bot.Skills.UseSkill(5);
                    Bot.Sleep(500);
                }
                else
                {
                    Bot.Combat.Attack(2);
                    Bot.Sleep(500);
                }
            }

            // ======================================================
            // MUST TAUNT TYN (full tank)
            // ======================================================
            if (isMustTauntTyn)
            {
                Bot.Combat.Attack(2);
                Bot.Skills.UseSkill(5);
                Bot.Sleep(500);
            }

            // ======================================================
            // FOCUS TYN (semi-taunt)
            // ======================================================
            if (isFocusTyn)
            {
                Bot.Combat.Attack(2);
                Bot.Skills.UseSkill(5);
                Bot.Sleep(500);
            }
        }
    }

    public static string GetDescription(Enum value)
    {
        FieldInfo? field = value.GetType().GetField(value.ToString());
        DescriptionAttribute? attribute =
            field?.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault()
            as DescriptionAttribute;

        return attribute?.Description ?? value.ToString();
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

        [Description("Arcana Invoker")]
        ArcanaInvoker,

        [Description("Current Class")]
        CurrentClass,
    }

    public enum Ball1Taunter
    {
        // In order of fast > safe > f2p fast > other
        [Description("Legion Revenant")]
        LegionRevenant,

        [Description("Lich")]
        Lich,

        [Description("Current Class")]
        CurrentClass,
    }

    public enum DebuffTyndarius
    {
        // In order of fast > safe > f2p fast > other
        [Description("Lord Of Order")]
        LordOfOrder,

        [Description("Dragon of Time")]
        DragonofTime,

        [Description("Current Class")]
        CurrentClass,
    }

    public enum MustTauntTyndarius
    {
        // In order of fast > safe > f2p fast > other
        [Description("ArchPaladin")]
        ArchPaladin,

        [Description("Verus Doomknight")]
        VerusDoomknight,

        [Description("Current Class")]
        CurrentClass,
    }
}
