/*
name: UltraDrago
description: Ultra King Drago helper with taunter classes and priority adds.
tags: Ultra

Fight notes:
- Composition order is [slot 1-4]: Fast = CSS / LR / AP / LOO, Safe = CAv / LR / AP / LOO, F2PFast = KE / LR / AP / LOO.
- Fixed taunter slots are slot 3 and slot 4.
- Slot 3 handles Group 1 taunt timing, slot 4 handles Group 2 taunt timing.
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs

using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraDrago
{
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private CoreBots C => CoreBots.Instance;
    private static CoreAstravia Astravia
    {
        get => _Astravia ??= new CoreAstravia();
        set => _Astravia = value;
    }
    private static CoreAstravia _Astravia;
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraDrago";
    public string[] MultiOptions = { "Main", "CoreSettings", "ClassOverrides" };

    public List<IOption> Main = new()
    {
        new Option<DragoComp>(
            "DoEquipClasses",
            "Automatically Equip Classes",
            "Auto-equip classes across all 4 clients\n"
                + "Fast: CSS / LR / AP / LOO\n"
                + "Safe: CAv / LR / AP / LOO\n"
                + "F2PFast: KE / LR / AP / LOO\n"
                + "Unselected = off (use whatever classes you already have equipped).",
            DragoComp.Safe
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
        new Option<string>("c", "Tertiary Class Override (T)", "Blank = use selected comp default for slot 3 (taunter).", ""),
        new Option<string>("d", "Quaternary Class Override (T)", "Blank = use selected comp default for slot 4 (taunter).", ""),
    };

    string overrideA = string.Empty;
    string overrideB = string.Empty;
    string overrideC = string.Empty;
    string overrideD = string.Empty;
    bool UseLifeSteal;
    bool EquipBestGear;
    bool DoEnhancements;
    bool RestoreGear;
    DragoComp ActiveComp = DragoComp.Safe;

    string tauntSlot3 = "ArchPaladin";
    string tauntSlot4 = "Lord Of Order";
    bool isTaunterGroup1;
    bool isTaunterGroup2;

    public void ScriptMain(IScriptInterface bot)
    {
        if (Bot.Config != null && !Bot.Config.Get<bool>("Main", "SkipOption"))
            Bot.Config.Configure();

        ActiveComp = Bot.Config == null ? DragoComp.Safe : Bot.Config.Get<DragoComp>("Main", "DoEquipClasses");
        overrideA = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "a") ?? string.Empty)).Trim();
        overrideB = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "b") ?? string.Empty)).Trim();
        overrideC = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "c") ?? string.Empty)).Trim();
        overrideD = (Bot.Config == null ? string.Empty : (Bot.Config.Get<string>("ClassOverrides", "d") ?? string.Empty)).Trim();
        EquipBestGear = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "EquipBestGear");
        DoEnhancements = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "DoEnh");
        UseLifeSteal = Bot.Config == null ? true : Bot.Config.Get<bool>("CoreSettings", "UseLifeSteal");
        RestoreGear = Bot.Config == null ? false : Bot.Config.Get<bool>("CoreSettings", "RestoreGear");

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
        DragoComp comp = DragoComp.Safe,
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

        isTaunterGroup1 = Core.HasClassEquipped(tauntSlot3);
        isTaunterGroup2 = Core.HasClassEquipped(tauntSlot4);

        C.EnsureComplete(8397);
        Fight();
    }

    void Prep()
    {
        C.Join("whitemap");
        Astravia.AstraviaJudgement();

        ApplyCompAndEquip(ActiveComp, overrideA, overrideB, overrideC, overrideD);

        if (EquipBestGear)
            EquipBestDmgGear();

        if (DoEnhancements)
            DoEnhs();

        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");

        if (Core.HasClassEquipped(tauntSlot3) || Core.HasClassEquipped(tauntSlot4))
            Ultra.GetScrollOfEnrage();
        else if (UseLifeSteal)
            Ultra.GetScrollOfLifeSteal();
    }

    void ApplyCompAndEquip(DragoComp comp, string aOverride, string bOverride, string cOverride, string dOverride)
    {
        if (comp == DragoComp.Unselected)
            return;

        string[] classes = comp switch
        {
            DragoComp.Fast => new[]
            {
                string.IsNullOrWhiteSpace(aOverride) ? "Chrono ShadowSlayer" : aOverride,
                string.IsNullOrWhiteSpace(bOverride) ? "Legion Revenant" : bOverride,
                string.IsNullOrWhiteSpace(cOverride) ? "ArchPaladin" : cOverride,
                string.IsNullOrWhiteSpace(dOverride) ? "Lord Of Order" : dOverride,
            },
            DragoComp.Safe => new[]
            {
                string.IsNullOrWhiteSpace(aOverride) ? "Chaos Avenger" : aOverride,
                string.IsNullOrWhiteSpace(bOverride) ? "Legion Revenant" : bOverride,
                string.IsNullOrWhiteSpace(cOverride) ? "ArchPaladin" : cOverride,
                string.IsNullOrWhiteSpace(dOverride) ? "Lord Of Order" : dOverride,
            },
            DragoComp.F2PFast => new[]
            {
                string.IsNullOrWhiteSpace(aOverride) ? "King's Echo" : aOverride,
                string.IsNullOrWhiteSpace(bOverride) ? "Legion Revenant" : bOverride,
                string.IsNullOrWhiteSpace(cOverride) ? "ArchPaladin" : cOverride,
                string.IsNullOrWhiteSpace(dOverride) ? "Lord Of Order" : dOverride,
            },
            _ => throw new System.InvalidOperationException($"Unhandled DragoComp value: {comp}")
        };

        tauntSlot3 = classes[2];
        tauntSlot4 = classes[3];
        Ultra.EquipClassSync(classes, 4, "drago_class.sync");
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
        const string map = "ultradrago";
        const string boss = "King Drago";
        const string leftSummon = "Bowmaster Algie";
        const string rightSummon = "Executioner Dene";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);
        if (!Bot.Quests.IsUnlocked(8397))
            Bot.Quests.UpdateQuest(8395);
        C.AddDrop("King Drago Insignia");

        Core.Join(map);
        C.EnsureAccept(8397);
        Ultra.WaitForArmy(3, "ultra_drago.sync");
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

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Drago Dethroned", 1), syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                if (Bot.Quests.IsUnlocked(8397))
                    C.EnsureComplete(8397);
                Bot.Wait.ForPickup("King Drago Insignia");
                break;
            }

            if (UseLifeSteal && !isTaunterGroup1 && !isTaunterGroup2 && Bot.Skills.CanUseSkill(5))
                Bot.Skills.UseSkill(5);

            if (isTaunterGroup1 && Ultra.MonsterAlive(rightSummon))
            {
                while (!Bot.ShouldExit)
                {
                    Ultra.Taunt(Bot.Player?.CurrentClass?.Name!, rightSummon, "aura", 250, "Focus");
                    if (!Ultra.MonsterAlive(rightSummon))
                        break;
                }
                continue;
            }

            if (isTaunterGroup2 && Ultra.MonsterAlive(rightSummon))
            {
                while (!Bot.ShouldExit)
                {
                    Ultra.Taunt(Bot.Player?.CurrentClass?.Name!, rightSummon, "aura", 700, "Focus");
                    if (!Ultra.MonsterAlive(rightSummon))
                        break;
                }
                continue;
            }

            Core.KillWithPriority(boss, leftSummon, rightSummon);
            Bot.Skills.UseSkill(5);
        }
    }

    void DoEnhs()
    {
        string className = Bot.Player!.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        switch (className)
        {
            case "Chrono ShadowSlayer":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Vim, wSpecial: WeaponSpecial.Valiance, cSpecial: CapeSpecial.Vainglory);
                break;
            case "Legion Revenant":
                Adv.EnhanceEquipped(type: EnhancementType.Wizard, hSpecial: HelmSpecial.Pneuma, wSpecial: WeaponSpecial.Valiance, cSpecial: CapeSpecial.Vainglory);
                break;
            case "ArchPaladin":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Forge, wSpecial: WeaponSpecial.Valiance, cSpecial: CapeSpecial.Lament);
                break;
            case "Lord Of Order":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Forge, wSpecial: WeaponSpecial.Awe_Blast, cSpecial: CapeSpecial.Absolution);
                break;
            case "Chaos Avenger":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Anima, wSpecial: WeaponSpecial.Valiance, cSpecial: CapeSpecial.Vainglory);
                break;
            case "King's Echo":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Examen, wSpecial: WeaponSpecial.Ravenous, cSpecial: CapeSpecial.Vainglory);
                break;
            case "Arcana Invoker":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Examen, wSpecial: WeaponSpecial.Ravenous, cSpecial: CapeSpecial.Vainglory);
                break;
            case "Archfiend":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Forge, wSpecial: WeaponSpecial.Ravenous, cSpecial: CapeSpecial.Vainglory);
                break;
            case "Lich":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Examen, wSpecial: WeaponSpecial.Ravenous, cSpecial: CapeSpecial.Penitence);
                break;
            case "Sentinel":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Anima, wSpecial: WeaponSpecial.Ravenous, cSpecial: CapeSpecial.Vainglory);
                break;
        }
    }

    public enum DragoComp
    {
        Unselected,
        Fast,
        Safe,
        F2PFast,
    }
}
