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
 ===================
Safe Comp:
CAV, LR, AP, LOO

Fast Comp:
CSS,LR,AP,LOO

Fast F2P:
KE, LR, AP, LOO

Other Comp:
AI, LICH, VDK, DOT
 ===================
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
    bool isBall1Taunter;
    bool isBall2Taunter;
    bool isMustTauntTyn;
    bool isFocusTyn;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraAvatarTyndarius";
    public List<IOption> Options = new()
    {
        // Ball 1 Taunter selection
        new Option<Ball1Taunter>(
            "Ball1Taunter",
            "Ball 1 Taunter",
            "Select which class should taunt Ball 1.",
            Ball1Taunter.ChronoShadowSlayer
        ),
        // Ball 2 Taunter selection
        new Option<Ball2Taunter>(
            "Ball2Taunter",
            "Ball 2 Taunter",
            "Select which class should taunt Ball 2.",
            Ball2Taunter.LegionRevenant
        ),
        // Must Taunt Tyndarius selection
        new Option<MustTauntTyndarius>(
            "MustTauntTyndarius",
            "Must Taunt Tyndarius",
            "Select which class must taunt Tyndarius.",
            MustTauntTyndarius.ArchPaladin
        ),
        // Focus Tyndarius selection
        new Option<FocusTyndarius>(
            "FocusTyndarius",
            "Focus Tyndarius",
            "Select which class should focus Tyndarius.",
            FocusTyndarius.LordOfOrder
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
        string playerClass = Bot.Player.CurrentClass?.Name ?? string.Empty;
        bool isBall1Taunter =
            Bot.Player.CurrentClass?.Name
            == GetDescription(Bot.Config!.Get<Ball1Taunter>("Ball1Taunter"));
        bool isBall2Taunter =
            Bot.Player.CurrentClass?.Name
            == GetDescription(Bot.Config!.Get<Ball2Taunter>("Ball2Taunter"));
        bool isMustTauntTyn =
            Bot.Player.CurrentClass?.Name
            == GetDescription(Bot.Config!.Get<MustTauntTyndarius>("MustTauntTyndarius"));
        bool isFocusTyn =
            Bot.Player.CurrentClass?.Name
            == GetDescription(Bot.Config!.Get<FocusTyndarius>("FocusTyndarius"));

        Core.Boot();
        Prep();
        Fight();
        Bot.Stop();
    }

    void Prep()
    {
        if (isBall1Taunter || isBall2Taunter || isMustTauntTyn)
            Ultra.GetScrollOfEnrage();
        else
        {
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
            if (isBall1Taunter || isBall2Taunter)
            {
                Ultra.Taunt(
                    playerClass,
                    "Ultra Fire Orb",
                    "aura",
                    isBall2Taunter ? 700 : 250,
                    "Focus"
                );

                Ultra.KillWithPriority(
                    "Ultra Fire Orb",
                    isBall2Taunter ? 1 : 3,
                    "Ultra Fire Orb",
                    isBall2Taunter ? 3 : 1,
                    boss,
                    2
                );
            }

            // ======================================================
            // MUST TAUNT TYN (full tank)
            // ======================================================
            if (isMustTauntTyn)
            {
                Ultra.Taunt(playerClass, boss, "aura", 700, "Focus");

                Ultra.KillWithPriority(boss, 2, "Ultra Fire Orb", 1, "Ultra Fire Orb", 3);
            }

            // ======================================================
            // FOCUS TYN (semi-taunt)
            // ======================================================
            if (isFocusTyn)
            {
                Bot.Combat.Attack(boss);
                Bot.Sleep(500);
            }
            Bot.Skills.UseSkill(5);
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

    public enum Ball1Taunter
    {
        // In order of fast > safe > f2p fast > other
        [Description("Chrono ShadowSlayer")]
        ChronoShadowSlayer,

        [Description("Chaos Avenger")]
        ChaosAvenger,

        [Description("King's Echo")]
        KingsEcho,

        [Description("Arcana Invoker")]
        ArcanaInvoker,

        [Description("Current Class")]
        CurrentClass,
    }

    public enum Ball2Taunter
    {
        // In order of fast > safe > f2p fast > other
        [Description("Legion Revenant")]
        LegionRevenant,

        [Description("Lich")]
        Lich,

        [Description("Current Class")]
        CurrentClass,
    }

    public enum FocusTyndarius
    {
        // In order of fast > safe > f2p fast > other
        [Description("Lord of Order")]
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
