/*
name: UltraSpeaker
description: Ultra First Speaker helper with zoning, taunt timing, and custom rotation.
tags: Ultra

Fight notes:
- Composition order is [slot 1-4]: Fast = AP / LR / QCM / LOO, Safe = LR / AP / LOO / VDK.
- All slots are taunt-capable in this encounter flow; script equips Enrage for all roles.
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs

using System;
using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraSpeaker
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
    public string OptionsStorage = "UltraSpeaker";
    public string[] MultiOptions = { "Main", "CoreSettings", "ClassOverrides" };

    public List<IOption> Main = new()
    {
        new Option<SpeakerComp>(
            "DoEquipClasses",
            "Automatically Equip Classes",
            "Auto-equip classes across all 4 clients\n"
                + "Fast: AP / LR / QCM / LOO\n"
                + "Safe: LR / AP / LOO / VDK\n"
                + "Unselected = off (use current classes).",
            SpeakerComp.Safe
        ),
        CoreBots.Instance.SkipOptions,
    };

    public List<IOption> CoreSettings = new()
    {
        new Option<bool>("EquipBestGear", "Equip Best Gear", "Equip best gear for encounter", true),
        new Option<bool>("DoEnh", "Do Enhancements", "Auto-Enhance Gear properly for the fight", true),
        new Option<bool>("RestoreGear", "Restore Gear", "Restore original gear after the script finishes", false),
        new Option<bool>("UseLifeSteal", "Use LifeSteal", "Unused here because all slots use Enrage by design.", true),
    };

    public List<IOption> ClassOverrides = new()
    {
        new Option<string>("a", "Primary Class Override", "Blank = use selected comp default for slot 1.", ""),
        new Option<string>("b", "Secondary Class Override", "Blank = use selected comp default for slot 2.", ""),
        new Option<string>("c", "Tertiary Class Override", "Blank = use selected comp default for slot 3.", ""),
        new Option<string>("d", "Quaternary Class Override", "Blank = use selected comp default for slot 4.", ""),
    };

    private SpeakerComp ActiveComp = SpeakerComp.Safe;
    private bool EquipBestGear;
    private bool DoEnhancements;
    private bool RestoreGear;
    private string overrideA = string.Empty;
    private string overrideB = string.Empty;
    private string overrideC = string.Empty;
    private string overrideD = string.Empty;

    public void ScriptMain(IScriptInterface bot)
    {
        if (Bot.Config != null && !Bot.Config.Get<bool>("Main", "SkipOption"))
            Bot.Config.Configure();

        C.Logger("This script uses the corner spam taunt method.");

        ActiveComp = Bot.Config == null ? SpeakerComp.Safe : Bot.Config.Get<SpeakerComp>("Main", "DoEquipClasses");
        overrideA = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "a") ?? string.Empty)).Trim();
        overrideB = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "b") ?? string.Empty)).Trim();
        overrideC = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "c") ?? string.Empty)).Trim();
        overrideD = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "d") ?? string.Empty)).Trim();
        EquipBestGear = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "EquipBestGear");
        DoEnhancements = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "DoEnh");
        RestoreGear = Bot.Config == null ? false : Bot.Config.Get<bool>("CoreSettings", "RestoreGear");

        Adv.GearStore();
        try
        {
            Run(ActiveComp, EquipBestGear, DoEnhancements, overrideA, overrideB, overrideC, overrideD);
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
        SpeakerComp comp = SpeakerComp.Safe,
        bool equipBestGear = true,
        bool doEnhancements = true,
        string? classAOverride = null,
        string? classBOverride = null,
        string? classCOverride = null,
        string? classDOverride = null
    )
    {
        ActiveComp = comp;
        EquipBestGear = equipBestGear;
        DoEnhancements = doEnhancements;
        overrideA = classAOverride?.Trim() ?? string.Empty;
        overrideB = classBOverride?.Trim() ?? string.Empty;
        overrideC = classCOverride?.Trim() ?? string.Empty;
        overrideD = classDOverride?.Trim() ?? string.Empty;

        Core.Boot();
        Prep();
        Kill();
    }

    void Prep()
    {
        ApplyCompAndEquip(ActiveComp, overrideA, overrideB, overrideC, overrideD);

        if (EquipBestGear)
            EquipBestDmgGear();

        if (DoEnhancements)
            DoEnh();

        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");
        Ultra.GetScrollOfEnrage();
    }

    void ApplyCompAndEquip(SpeakerComp comp, string aOverride, string bOverride, string cOverride, string dOverride)
    {
        if (comp == SpeakerComp.Unselected)
            return;

        string[] classes = comp switch
        {
            SpeakerComp.Fast => new[]
            {
                string.IsNullOrWhiteSpace(aOverride) ? "ArchPaladin" : aOverride,
                string.IsNullOrWhiteSpace(bOverride) ? "Legion Revenant" : bOverride,
                string.IsNullOrWhiteSpace(cOverride) ? "Quantum Chronomancer" : cOverride,
                string.IsNullOrWhiteSpace(dOverride) ? "Lord Of Order" : dOverride,
            },
            SpeakerComp.Safe => new[]
            {
                string.IsNullOrWhiteSpace(aOverride) ? "Legion Revenant" : aOverride,
                string.IsNullOrWhiteSpace(bOverride) ? "ArchPaladin" : bOverride,
                string.IsNullOrWhiteSpace(cOverride) ? "Lord Of Order" : cOverride,
                string.IsNullOrWhiteSpace(dOverride) ? "Verus DoomKnight" : dOverride,
            },
            _ => throw new InvalidOperationException($"Unhandled SpeakerComp value: {comp}")
        };

        Ultra.EquipClassSync(classes, 4, "speaker_class.sync");
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

    void Kill()
    {
        const string map = "ultraspeaker";
        const string boss = "The First Speaker";

        if (!Bot.Quests.IsUnlocked(9173))
            Bot.Log("Ultra Quest is not unlocked, fake unlocking for drop support.");

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);
        Bot.Options.DisableCollisions = true;
        C.EnsureAccept(9173);
        C.AddDrop("The First Speaker Silenced");
        if (!Bot.Quests.IsUnlocked(9173))
            Bot.Quests.UpdateQuest(9125);

        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_speaker.sync");
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

            bool allComplete = Ultra.CheckArmyProgressBool(() => Bot.Inventory.Contains("The First Speaker Silenced", 1), syncPath);
            if (allComplete)
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                if (Bot.Quests.IsDailyComplete(9173))
                    C.Logger("Weekly already complete, try again Friday morning");
                else
                    C.EnsureComplete(9173);
                break;
            }

            if (Bot.Self.Auras.Any(a => a.Name == "Stasis"))
            {
                Core.DisableSkills();
                Bot.Wait.ForTrue(() => Bot.Self.Auras.Any(a => a.Name == "Stasis"), 20);
                Core.EnableSkills();
                continue;
            }

            if (Bot.Player?.Cell == "Boss")
            {
                int minX = 0, maxX = 100;
                int minY = 485, maxY = 500;
                bool isInBox = Bot.Player.Position.X >= minX && Bot.Player.Position.X <= maxX && Bot.Player.Position.Y >= minY && Bot.Player.Position.Y <= maxY;
                if (!isInBox)
                {
                    Random rand = new();
                    Bot.Player.WalkTo(rand.Next(minX, maxX + 1), rand.Next(minY, maxY + 1));
                    Bot.Sleep(500);
                }
            }

            if (Bot.Monsters.CurrentMonsters.Any(m => m.Name == boss && m.Alive))
            {
                Bot.Combat.Attack(boss);
                if (!Bot.Target.Auras.Any(a => a.Name == "Focus") && Bot.Skills.CanUseSkill(5))
                    Bot.Skills.UseSkill(5);
            }
            Bot.Sleep(500);
        }
    }

    void DoEnh()
    {
        string className = Bot.Player!.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        switch (className.ToLower())
        {
            case "chrono shadowslayer":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Forge, wSpecial: WeaponSpecial.Valiance, cSpecial: CapeSpecial.Penitence);
                break;
            case "legion revenant":
                Adv.EnhanceEquipped(type: EnhancementType.Wizard, wSpecial: WeaponSpecial.Arcanas_Concerto, cSpecial: CapeSpecial.Vainglory);
                break;
            case "archpaladin":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Forge, wSpecial: WeaponSpecial.Valiance, cSpecial: CapeSpecial.Lament);
                break;
            case "lord of order":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Forge, wSpecial: WeaponSpecial.Valiance, cSpecial: CapeSpecial.Penitence);
                break;
            case "quantum chronomancer":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Forge, wSpecial: WeaponSpecial.Praxis, cSpecial: CapeSpecial.Penitence);
                break;
            case "verus doomknight":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Anima, wSpecial: WeaponSpecial.Ravenous, cSpecial: CapeSpecial.Vainglory);
                break;
            case "sentinel":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Anima, wSpecial: WeaponSpecial.Ravenous, cSpecial: CapeSpecial.Penitence);
                break;
            case "archfiend":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Forge, wSpecial: WeaponSpecial.Ravenous, cSpecial: CapeSpecial.Penitence);
                break;
            case "king's echo":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Examen, wSpecial: WeaponSpecial.Ravenous, cSpecial: CapeSpecial.Vainglory);
                break;
            case "void highlord":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Anima, wSpecial: WeaponSpecial.Ravenous, cSpecial: CapeSpecial.Vainglory);
                break;
        }
    }

    public enum SpeakerComp
    {
        Unselected,
        Fast,
        Safe,
    }
}
