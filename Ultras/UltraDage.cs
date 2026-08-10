/*
name: UltraDage
description: Two-taunter strategy for Ultra Dage with aura-based taunting and army synchronization.
tags: Ultra
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs

#region Required Taunters
// Chaos Avenger: Lucky | Anima | Dauntless/HealthVamp | Vainglory
// ArchPaladin: Lucky | Forge | Dauntless/HealthVamp | Lament
#endregion

#region DPS (No Deaths)
// Lich: Lucky | Examen | Ravenous | Penitence
// Legion Revenant: Wizard | Pneuma | Dauntless/HealthVamp | Vainglory
// Great Thief: Lucky | Forge | Dauntless/HealthVamp | Vainglory
// Hollowborn Vindicator: Lucky | Forge | Dauntless/HealthVamp | Penitence
// Quantum Chronomancer: Lucky | Anima | Dauntless/HealthVamp | Vainglory
// Phantom Chronomancer: Wizard | Pneuma | Dauntless/HealthVamp | Vainglory
// Verus DoomKnight: Lucky | Anima | Dauntless/HealthVamp | Vainglory
// King's Echo: Lucky | Pneuma | Dauntless/HealthVamp | Lament
#endregion

#region DPS (Works with Deaths)
// Arachnomancer: Lucky | Anima | Dauntless/HealthVamp | Vainglory
// Archfiend: Lucky | Forge | Dauntless/HealthVamp | Vainglory
// Infinity Knight: Wizard | Pneuma | Dauntless/HealthVamp | Vainglory
// StoneCrusher: Wizard | Pneuma | Dauntless/HealthVamp | Vainglory
#endregion

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
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
    public CoreEnginev1 Core = new();
    public CoreUltrav1 Ultra = new();
    string a,
        b;
    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraDage";
    public List<IOption> Options = new()
    {
        new Option<DageComp>(
            "DoEquipClasses",
            "Automatically Equip Classes",
            "Auto-equip classes across all 4 clients\n"
                + "BestAvailable: CAv / AP / Best DPS / Best DPS\n"
                + "Unselected = off (use whatever classes you already have equipped).",
            DageComp.Unselected
        ),
        new Option<string>(
            "a",
            "First Taunter Class",
            "Insert the name of the class that will taunt ( examples: AP, Cav, LR, KE(?))",
            "Chaos Avenger"
        ),
        new Option<string>(
            "b",
            "Second Taunter Class",
            "Insert the name of the class that will taunt ( examples: AP, Cav, LR, KE(?))",
            "ArchPaladin"
        ),
        new Option<bool>("DoEnh", "Do Enhancements",  "Auto-Enhance Gear properly for the fight", true),
        CoreBots.Instance.SkipOptions,
    };

    private string NormalizeString(string input) => (input ?? "").Trim().ToLower();

    public void ScriptMain(IScriptInterface bot)
    {
        if (!C.isCompletedBefore(793))
            C.Logger(
                @"player is not part of the legion, you will not be able to turn the quest in. though u cna prolly do the kill."
            );

        C.Join("whitemap");

        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();
        a = NormalizeString(Bot.Config!.Get<string>("a")!);
        b = NormalizeString(Bot.Config.Get<string>("b")!);
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
        {
            Core.Log("Setup", "Fill both taunter classes in Script Options.");
            Bot.StopSync();
            return;
        }
        Core.Boot();

        Adv.GearStore(EnhAfter: true);
        Prep();
        Bot.Events.ExtensionPacketReceived += UltraDageListener;
        Fight();
        Bot.Events.ExtensionPacketReceived -= UltraDageListener;

        if (Bot.Config!.Get<bool>("DoEnh"))
            Adv.GearStore(true, true);
        Bot.StopSync();
    }

    bool IsTaunter() => Core.HasClassEquipped(a) || Core.HasClassEquipped(b);

    void Prep()
    {
        if (!Bot.Quests.IsUnlocked(793))
            Bot.Log("Quest Not unlocked (we'll still do the ultra just you wont be able to complete it)\n" +
            " -- you must go run \"\\Story\\Legion\\DageChallengeStory.cs\" first");
        // UpdateQuest to `Fail to the king` to unlock ultra dage
        Bot.Quests.UpdateQuest(793);

        // Sync-equip classes if a comp is selected
        DageComp comp = Bot.Config!.Get<DageComp>("DoEquipClasses");
        if (comp != DageComp.Unselected)
        {
            // DPS priority: no-death classes first, then death-tolerant as fallback
            string[] dpsOptions = new[] {
                "Lich",
                "Legion Revenant",
                "Great Thief",
                "Hollowborn Vindicator",
                "Quantum Chronomancer",
                "Phantom Chronomancer",
                "Verus DoomKnight",
                "King's Echo",
                "Arachnomancer",
                "Archfiend",
                "Infinity Knight",
                "StoneCrusher"
            };

            string[][] classes = new[] {
                new[] { "Chaos Avenger" },
                new[] { "ArchPaladin" },
                dpsOptions,
                dpsOptions
            };

            Ultra.EquipClassSync(classes, 4, "dage_class.sync");
        }

        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnh();
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        if (IsTaunter())
            Ultra.GetScrollOfEnrage();
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
        var (bestCell, bestPad) = Core.ChooseBestCell(boss);
         
        Bot.Player.SetSpawnPoint();
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
                Bot.Map.Jump(bestCell, bestPad);
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
            if (Core.HasClassEquipped(a) || Core.HasClassEquipped(b) && !Bot.Target.Auras.Any(a => a.Name == "Focus"))
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
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "archpaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "legion revenant":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "archfiend":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "arachnomancer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "king's echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "chrono shadowslayer":
            case "chrono shadowhunter":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Vim,
                    wSpecial: WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "quantum chronomancer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "phantom chronomancer":
            case "phantasm chronomancer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "infinity knight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp,
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
                    wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "hollowborn vindicator":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "great thief":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "stonecrusher":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: Adv.uDauntless() ? WeaponSpecial.Dauntless : WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;
        }
    }

    public enum DageComp
    {
        Unselected,
        BestAvailable,
    }
}
