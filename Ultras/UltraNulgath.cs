/*
name: UltraNulgath
description: Nulgath the Archfiend helper with taunter rotation and blade priority.
tags: Ultra
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

#region Fast Comp
// Chrono ShadowSlayer: Lucky | Vim | Valiance | Vainglory
// Verus DoomKnight: Lucky | Anima | Ravenous | Vainglory
// Legion Revenant: Wizard | Pneuma | Valiance/Ravenous/Arcana | Vainglory
// Lord of Order: Lucky | Forge | Awe Blast/Valiance | Absolution
#endregion

#region F2P Fast
// Dragon of Time (x2): Wizard | Pneuma | Elysium | Vainglory
// Legion Revenant: Wizard | Pneuma | Valiance/Ravenous/Arcana | Vainglory
// Lord of Order: Lucky | Forge | Awe Blast/Valiance | Absolution
#endregion

#region Common Comp
// King's Echo: Lucky | Examen | Ravenous | Vainglory
// Legion Revenant: Wizard | Pneuma | Valiance/Ravenous/Arcana | Vainglory
// ArchPaladin: Lucky | Forge | Valiance | Lament
// Lord of Order: Lucky | Forge | Awe Blast/Valiance | Absolution
#endregion

#region Balanced Comp
// Legion Revenant: Wizard | Pneuma | Valiance/Ravenous/Arcana | Vainglory
// ArchPaladin: Lucky | Forge | Valiance | Lament
// StoneCrusher: Wizard | Pneuma | Valiance | Vainglory
// Lord of Order: Lucky | Forge | Awe Blast/Valiance | Absolution
#endregion

#region Other DPS Options
// Arcana Invoker: Lucky | Examen | Ravenous | Vainglory
// Archfiend: Lucky | Forge | Ravenous | Vainglory
// Lich: Lucky | Examen | Ravenous | Vainglory
#endregion

public class UltraNulgath
{
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private CoreBots C => CoreBots.Instance;
    private static CoreAdvanced _Adv;
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraNulgath";
    public List<IOption> Options = new()
    {
        new Option<NulgathComp>(
            "DoEquipClasses",
            "Automatically Equip Classes",
            "Auto-equip classes across all 4 clients\n"
                + "Fast: CSS / VDK / LR / LOO\n"
                + "F2PFast: DoT / DoT / LR / LOO\n"
                + "Common: KE / LR / AP / LOO\n"
                + "Balanced: LR / AP / SC / LOO\n"
                + "Unselected = off (use whatever classes you already have equipped).",
            NulgathComp.Unselected
        ),
        new Option<string>( "a", "Taunter 1 ClassName", "Names must be exact including punctuation, spelling, and captitalization", "ArchPaladin"),
        new Option<string>( "b", "Taunter 2 ClassName", "Names must be exact including punctuation, spelling, and captitalization", "Lord of Order"),
        new Option<bool>("DoEnh", "Do Enhancements",  "Auto-Enhance Gear properly for the fight", true),
        CoreBots.Instance.SkipOptions,
    };

    string? CurrentClass;
    public void ScriptMain(IScriptInterface bot)
    {
        C.OneTimeMessage(
            "Ultra Nulgath",
            "Deaths more then likely will happen, Suggested class and thier enhs are in the script at the top"
        );

        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();

        a = Bot.Config!.Get<string>("a") ?? "";
        b = Bot.Config!.Get<string>("b") ?? "";

        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
        {
            C.Logger("Setup", "Fill both taunter classes in Script Options.");
            C.SetOptions(false);
        }
        CurrentClass = Bot.Player?.CurrentClass?.Name;
        if (!string.IsNullOrEmpty(CurrentClass))
        {
            if (CurrentClass == a || CurrentClass == b)
                C.Logger("Taunter [Yes]");
            else
                C.Logger("Taunter [No]");

        }

        Core.Boot();
        Prep();
        Fight();
        C.SetOptions(false);
    }

    string a, b;
    // Overfiend Blade = 1
    // Nulgath = 2
    void Fight()
    {
        #region ignore this
        const string map = "ultranulgath";
        const string boss = "Nulgath the Archfiend";
        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);
        C.EnsureAccept(8692);
        C.AddDrop("Nulgath Insignia");
        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_nulgath.sync");

        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();
        #endregion
        while (!Bot.ShouldExit)
        {
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Nulgath the Archfiend Defeated?", 1), syncPath))
            {
                C.Logger("All players finished farm.");
                Core.DisableSkills();
                Bot.Sleep(200);
                C.Jump("Enter", "Spawn");
                C.Join("whitemap");
                if (!Bot.Quests.IsDailyComplete(8692))
                    C.EnsureComplete(8692);
                if (Bot.Config!.Get<bool>("DoEnh"))
                    Adv.GearStore(true, true);
                break;
            }

            // Taunters focus nulgath
            if (Bot.Player.CurrentClass?.Name == a || Bot.Player.CurrentClass?.Name == b)
            {
                //taunters focus Nulgath (MID 2)
                Bot.Combat.Attack(2);
                Bot.Sleep(200);
            }
            else
            {
                //DPSers attack the Overfiend Blade(1) and when it dies, swap to nulgath(2), (refocusing "Overfiend Blade" when it respawns)
                if (Bot.Monsters.MapMonsters.Any(x => x != null && x.MapID == 1 && x.HP > 0))
                    Bot.Combat.Attack(1); // Overfiend Blade
                else
                    Bot.Combat.Attack(2); // Nulgath
                Bot.Sleep(200);
            }

            // --- Advanced Time-Buffered Taunter Logic ---
            if (Bot.Player.Alive && Bot.Monsters.MapMonsters.Any(x => (x?.MapID == 2 || x?.MapID == 1) && x.HP > 0))
            {
                // Find the Focus aura object on Nulgath if it exists
                var focusAura = Bot.Target.Auras.FirstOrDefault(x => x?.Name == "Focus");

                // Get the remaining duration. If the aura doesn't exist, duration is 0.
                float remainingTime = focusAura != null ? focusAura.Duration : 0f;

                // Taunter A (The Lead): Casts if the boss is completely untaunted (0 seconds left)
                if (Bot.Player.CurrentClass?.Name == a && remainingTime <= 0f)
                {
                    Bot.Skills.UseSkill(5);
                    Bot.Sleep(200);
                }

                // Taunter B (The Interceptor): Uses your 2-second buffer rule. 
                // Steals focus early to account for server ping and animation locks.
                else if (Bot.Player.CurrentClass?.Name == b && remainingTime <= 2.0f)
                {
                    Bot.Skills.UseSkill(5);
                    Bot.Sleep(200);
                }
            }

        }
    }


    void Prep()
    {
        // Sync-equip classes if a comp is selected
        NulgathComp comp = Bot.Config!.Get<NulgathComp>("DoEquipClasses");
        if (comp != NulgathComp.Unselected)
        {
            string[][] classes = comp switch
            {
                NulgathComp.Fast => new[] {
                    new[] { C.CheckInventory("Chrono ShadowSlayer") ? "Chrono ShadowSlayer" : "Chrono ShadowHunter" },
                    new[] { "Verus DoomKnight" },
                    new[] { "Legion Revenant" },
                    new[] { "Lord of Order" }
                },
                NulgathComp.F2PFast => new[] {
                    new[] { "Dragon of Time" },
                    new[] { "Dragon of Time" },
                    new[] { "Legion Revenant" },
                    new[] { "Lord of Order" }
                },
                NulgathComp.Common => new[] {
                    new[] { "King's Echo" },
                    new[] { "Legion Revenant" },
                    new[] { "ArchPaladin" },
                    new[] { "Lord of Order" }
                },
                NulgathComp.Balanced => new[] {
                    new[] { "Legion Revenant" },
                    new[] { "ArchPaladin" },
                    new[] {  C.CheckInventory("Infinity Titan") ? "Infinity Titan" : "StoneCrusher" },
                    new[] { "Lord of Order" }
                },
                _ => throw new NotImplementedException(),
            };

            Ultra.EquipClassSync(classes, 4, "nulgath_class.sync", allowDuplicates: true);
        }

        if (Bot.Config!.Get<bool>("DoEnh"))
        {
            Adv.GearStore(false, true);
            DoEnhs();
        }
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        if (Bot.Inventory.Items.Any(x => x != null && x.Equipped && (x.Name == a || x.Name == b)))
        {
            Ultra.GetScrollOfEnrage();
            Core.EquipEnrage();
        }
    }
    void DoEnhs()
    {
        string className = Bot.Player!.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        switch (className)
        {
            // Chrono ShadowSlayer
            case "Chrono ShadowSlayer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Vim,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Verus DoomKnight
            case "Verus DoomKnight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Legion Revenant
            case "Legion Revenant":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Lord of Order
            case "Lord of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Awe_Blast,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            // Dragon of Time
            case "Dragon of Time":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Elysium,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // King's Echo
            case "King's Echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Arcana Invoker
            case "Arcana Invoker":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Archfiend
            case "Archfiend":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // Lich
            case "Lich":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            // ArchPaladin
            case "ArchPaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            // StoneCrusher
            case "StoneCrusher":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;
        }
    }

    public enum NulgathComp
    {
        Unselected,
        Fast,
        F2PFast,
        Common,
        Balanced,
    }
}
