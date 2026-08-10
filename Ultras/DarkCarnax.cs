/*
name: DarkCarnax
description: Farming script for Nightmare Dark Carnax with movement zone automation.
tags: Ultra, carnax, dark carnax, synthetic viscera
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs

/*
Recommended classes:
==================
Solo with Dragon of Time or Healer classes
==================
DPS classes:
==================
    Legion Revenant
    Chrono ShadowSlayer
    Lich
    Archfiend
    Quantum Chronomancer
    Hollowborn Vindicator
    Arachnomancer
    Infinity Knight
    Verus DoomKnight
    King's Echo
    Phantom Chronomancer / Phantasm Chronomancer
    Great Thief
==================
*/

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Skills;
using Skua.Core.Options;

public class UltraDarkCarnax
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

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraDarkCarnax";
    public List<IOption> Options = new()
    {
        new Option<bool>(
            "AttemptSolo",
            "Attempt Solo",
            "If you have the Dragon of Time or Healer class, this will attempt to solo Nightmare Carnax.",
            false
        ),
        new Option<bool>("DoEnh", "Do Enhancements", "Auto-Enhance Gear properly for the fight", true),
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

        Core.Boot();
        Adv.GearStore(EnhAfter: true);
        Prep();
        Fight();
        Bot.Events.ExtensionPacketReceived -= DarkCarnaxListener;
        if (Bot.Config!.Get<bool>("DoEnh"))
            Adv.GearStore(true, true);
        Bot.StopSync();
    }

    void Prep()
    {
        Bot.Events.ExtensionPacketReceived += DarkCarnaxListener;
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnh();
    }

    void Fight()
    {
        const string map = "darkcarnax";
        const string boss = "Nightmare Carnax";
        bool attemptSolo = Bot.Config!.Get<bool>("AttemptSolo");
        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(1500);

        C.AddDrop("Synthetic Viscera");
        C.RegisterQuests(8872);
        Ultra.WaitForArmy(attemptSolo ? 0 : 3, "darkcarnax.sync");
        var (bestCell, bestPad) = Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();

        // Setup for solo attempt if configured
        if (attemptSolo)
        {
            if (C.CheckInventory("Dragon of Time"))
            {
                C.Equip("Dragon of Time");
                Bot.Skills.StartAdvanced("3|2|4|2|1|2", 250, SkillUseMode.WaitForCooldown);
            }
            else if (C.CheckInventory("Healer (Rare)"))
                Bot.Skills.StartAdvanced("Healer (Rare)", true, ClassUseMode.Base);
            else if (C.CheckInventory("Healer"))
                Bot.Skills.StartAdvanced("Healer", true, ClassUseMode.Base);
            else
                C.EquipClass(ClassType.Solo);

            if (Bot.Config!.Get<bool>("DoEnh"))
                Adv.EnhanceEquipped(EnhancementType.Healer, wSpecial: WeaponSpecial.Elysium);
        }
        else
        {
            // Check if Dragon of Time is equipped and set advanced skills
            if (Bot.Player.CurrentClass?.Name == "Dragon of Time")
                Bot.Skills.StartAdvanced("3|2|4|2|1|2", 250, SkillUseMode.WaitForCooldown);
            else
                Core.EnableSkills();
        }

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
                C.Jump(bestCell!, bestPad!);
                Bot.Wait.ForCellChange(bestCell);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.Inventory.Contains("Synthetic Viscera", 1000), syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                if (Bot.Config!.Get<bool>("DoEnh"))
                    Adv.GearStore(true, true);
                break;
            }

            // Rejoin if needed
            if (Bot.Map.Name != map)
                Core.Join(map, "Boss", "Left");
            if (Bot.Player!.Cell != "Boss")
                C.Jump("Boss", "Left");

            if (!Bot.Player.HasTarget)
                Bot.Combat.Attack(boss);
            Bot.Sleep(250);
        }

        C.CancelRegisteredQuests();
        Bot.Options.AttackWithoutTarget = false;
    }

    public async void DarkCarnaxListener(dynamic packet)
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

        int y = Bot.Random.Next(380, 475);
        int x = 0;

        switch (zoneSet.ToUpper())
        {
            case "A":
                x = Bot.Random.Next(600, 931);
                break;
            case "B":
                x = Bot.Random.Next(25, 326);
                break;
            default:
                x = Bot.Random.Next(325, 601);
                break;
        }

        _ = Task.Run(() => Bot.Player.WalkTo(x, y));
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
                    wSpecial: WeaponSpecial.Dauntless,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "archpaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Dauntless,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "legion revenant":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Dauntless,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "lord of order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Awe_Blast,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "archfiend":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Dauntless,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "arachnomancer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Dauntless,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "king's echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Dauntless,
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
                    wSpecial: WeaponSpecial.Dauntless,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "phantom chronomancer":
            case "phantasm chronomancer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Dauntless,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "infinity knight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Dauntless,
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
                    wSpecial: WeaponSpecial.Dauntless,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "hollowborn vindicator":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Dauntless,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "great thief":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Dauntless,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "healer":
            case "healer (rare)":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    wSpecial: WeaponSpecial.Elysium
                );
                break;

            case "dragon of time":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    wSpecial: WeaponSpecial.Elysium
                );
                break;
        }
    }
}
