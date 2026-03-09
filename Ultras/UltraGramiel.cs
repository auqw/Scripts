/*
name: UltraGramiel
description: Ultra Gramiel helper with fixed taunt-role assignments and crystal-phase recovery.
tags: Ultra

Fight notes:
- Composition order is [slot 1-4]: Recommended = SC/IT / AP / LOO / VHL, Alternate = SC/IT / LC / LOO / VDK.
- Fixed taunter slots are slot 1 (Left T1), slot 2 (Left T2), slot 3 (Right T1), slot 4 (Right T2).
- If you run off-comp classes, set your role manually with Custom Role.
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

public class UltraGramiel
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
    public string OptionsStorage = "UltraGramiel";
    public string[] MultiOptions = { "Main", "CoreSettings", "ClassOverrides" };

    public List<IOption> Main = new()
    {
        new Option<GramielComp>(
            "DoEquipClasses",
            "Automatically Equip Classes",
            "Auto-equip classes across all 4 clients\n"
                + "Recommended: SC / IT, AP, LOO, VHL\n"
                + "Alternate: SC / IT, LC, LOO, VDK\n"
                + "Unselected = off (use current classes).",
            GramielComp.Recommended
        ),
        new Option<CustomRole>(
            "CustomRole",
            "Custom Role",
            "Used only when your class is off-comp and cannot be auto-mapped.",
            CustomRole.Unselected
        ),
        CoreBots.Instance.SkipOptions,
    };

    public List<IOption> CoreSettings = new()
    {
        new Option<bool>("EquipBestGear", "Equip Best Gear", "Equip best gear for encounter", true),
        new Option<bool>("DoEnh", "Do Enhancements", "Auto-Enhance Gear properly for the fight", true),
        new Option<bool>("RestoreGear", "Restore Gear", "Restore original gear after the script finishes", false),
        new Option<bool>("UseLifeSteal", "Use LifeSteal", "Unused here because every fixed role taunts with Enrage.", true),
    };

    public List<IOption> ClassOverrides = new()
    {
        new Option<string>("a", "Slot 1 Class Override (Left T1)", "Blank = use selected comp default for slot 1.", ""),
        new Option<string>("b", "Slot 2 Class Override (Left T2)", "Blank = use selected comp default for slot 2.", ""),
        new Option<string>("c", "Slot 3 Class Override (Right T1)", "Blank = use selected comp default for slot 3.", ""),
        new Option<string>("d", "Slot 4 Class Override (Right T2)", "Blank = use selected comp default for slot 4.", ""),
    };

    private int tauntCounter;
    private DateTime lastTauntWarningTime = DateTime.MinValue;
    private bool shouldExecuteTaunt;
    private DateTime gramielFightStartTime = DateTime.MinValue;
    private double tauntOffsetSeconds;
    private const double TauntIntervalSeconds = 20.0;
    private const double TauntWindowSeconds = 4.0;

    private int crystalMapId = 2;
    private bool isT1Taunter;
    private int crystalDeathCount;

    private GramielComp ActiveComp = GramielComp.Recommended;
    private CustomRole ActiveCustomRole = CustomRole.Unselected;
    private bool EquipBestGear;
    private bool DoEnhancements;
    private bool UseLifeSteal;
    private bool RestoreGear;
    private string overrideA = string.Empty;
    private string overrideB = string.Empty;
    private string overrideC = string.Empty;
    private string overrideD = string.Empty;

    public void ScriptMain(IScriptInterface bot)
    {
        if (Bot.Config != null && !Bot.Config.Get<bool>("Main", "SkipOption"))
            Bot.Config.Configure();

        C.OneTimeMessage(
            "Ultra Gramiel",
            "This encounter requires synchronized taunts and role fidelity.\n"
                + "Recommended comp: SC/IT, AP, LOO, VHL.\n"
                + "Alternate comp: SC/IT, LC, LOO, VDK.",
            true,
            true
        );

        ActiveComp = Bot.Config == null ? GramielComp.Recommended : Bot.Config.Get<GramielComp>("Main", "DoEquipClasses");
        ActiveCustomRole = Bot.Config == null ? CustomRole.Unselected : Bot.Config.Get<CustomRole>("Main", "CustomRole");
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
            Run(
                ActiveComp,
                EquipBestGear,
                DoEnhancements,
                UseLifeSteal,
                ActiveCustomRole,
                overrideA,
                overrideB,
                overrideC,
                overrideD
            );
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
        GramielComp comp = GramielComp.Recommended,
        bool equipBestGear = true,
        bool doEnhancements = true,
        bool useLifeSteal = true,
        CustomRole customRole = CustomRole.Unselected,
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
        ActiveCustomRole = customRole;
        overrideA = classAOverride?.Trim() ?? string.Empty;
        overrideB = classBOverride?.Trim() ?? string.Empty;
        overrideC = classCOverride?.Trim() ?? string.Empty;
        overrideD = classDOverride?.Trim() ?? string.Empty;

        Core.Boot();
        Bot.Events.ExtensionPacketReceived += GramielMessageListener;
        try
        {
            Prep();
            Fight();
        }
        finally
        {
            Bot.Events.ExtensionPacketReceived -= GramielMessageListener;
        }
    }

    void Prep(bool skipEnhancements = false)
    {
        ApplyCompAndEquip(ActiveComp, overrideA, overrideB, overrideC, overrideD);

        if (EquipBestGear)
            EquipBestDmgGear();

        if (DoEnhancements && !skipEnhancements)
            DoEnhs();

        AssignRoleFromClassOrCustom();

        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");

        if (UseLifeSteal)
            C.Logger("UseLifeSteal is enabled, but Gramiel uses Enrage on all fixed taunt roles.");
        Ultra.GetScrollOfEnrage();

        Bot.Sleep(2500);
    }

    void ApplyCompAndEquip(GramielComp comp, string aOverride, string bOverride, string cOverride, string dOverride)
    {
        if (comp == GramielComp.Unselected)
            return;

        string[] classes = comp switch
        {
            GramielComp.Recommended => new[]
            {
                string.IsNullOrWhiteSpace(aOverride) ? "StoneCrusher" : aOverride,
                string.IsNullOrWhiteSpace(bOverride) ? "ArchPaladin" : bOverride,
                string.IsNullOrWhiteSpace(cOverride) ? "Lord Of Order" : cOverride,
                string.IsNullOrWhiteSpace(dOverride) ? "Void Highlord" : dOverride,
            },
            GramielComp.Alternate => new[]
            {
                string.IsNullOrWhiteSpace(aOverride) ? "StoneCrusher" : aOverride,
                string.IsNullOrWhiteSpace(bOverride) ? "LightCaster" : bOverride,
                string.IsNullOrWhiteSpace(cOverride) ? "Lord Of Order" : cOverride,
                string.IsNullOrWhiteSpace(dOverride) ? "Verus DoomKnight" : dOverride,
            },
            _ => throw new InvalidOperationException($"Unhandled GramielComp value: {comp}")
        };

        Ultra.EquipClassSync(classes, 4, "gramiel_class.sync");
    }

    void AssignRoleFromClassOrCustom()
    {
        string className = Bot.Player.CurrentClass?.Name ?? string.Empty;
        switch (className)
        {
            case "StoneCrusher":
            case "Infinity Titan":
                crystalMapId = 2;
                isT1Taunter = true;
                break;
            case "LightCaster":
            case "ArchPaladin":
                crystalMapId = 2;
                isT1Taunter = false;
                break;
            case "Lord Of Order":
                crystalMapId = 3;
                isT1Taunter = true;
                break;
            case "Verus DoomKnight":
            case "Void Highlord":
                crystalMapId = 3;
                isT1Taunter = false;
                break;
            default:
                if (ActiveCustomRole == CustomRole.Unselected)
                {
                    C.Logger($"Your class '{className}' is not auto-mapped. Set Main > Custom Role.", "Fix This", true, true);
                    return;
                }

                switch (ActiveCustomRole)
                {
                    case CustomRole.LeftCrystalT1:
                        crystalMapId = 2;
                        isT1Taunter = true;
                        break;
                    case CustomRole.LeftCrystalT2:
                        crystalMapId = 2;
                        isT1Taunter = false;
                        break;
                    case CustomRole.RightCrystalT1:
                        crystalMapId = 3;
                        isT1Taunter = true;
                        break;
                    case CustomRole.RightCrystalT2:
                        crystalMapId = 3;
                        isT1Taunter = false;
                        break;
                }
                C.Logger($"Off-comp class '{className}' using Custom Role: {ActiveCustomRole}");
                break;
        }

        if (crystalMapId == 2 && isT1Taunter)
            tauntOffsetSeconds = 0;
        else if (crystalMapId == 2 && !isT1Taunter)
            tauntOffsetSeconds = 5;
        else if (crystalMapId == 3 && isT1Taunter)
            tauntOffsetSeconds = 10;
        else
            tauntOffsetSeconds = 15;

        C.Logger($"Assigned crystal role: mapId={crystalMapId}, slot={(isT1Taunter ? "T1" : "T2")}, offset={tauntOffsetSeconds}s.");
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
        const string map = "ultragramiel";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        Core.Join("whitemap");
        Bot.Wait.ForMapLoad("whitemap");
        Ultra.WaitForArmy(3, "UltraItemCheck.sync");
        Bot.Sleep(1500);

        C.EnsureAccept(10301);
        C.AddDrop("Gramiel the Graceful Vanquished");

        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_gramiel.sync");
        Core.ChooseBestCell("*");
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        while (!Bot.ShouldExit)
        {
            bool anyCrystalAlive = Bot.Monsters.CurrentAvailableMonsters.Any(x => x != null && x.Alive && (x.MapID == 2 || x.MapID == 3));

            if (!Bot.Player.Alive && anyCrystalAlive)
            {
                crystalDeathCount++;
                C.Logger($"Death during crystal phase ({crystalDeathCount}/2)");
                while (!Bot.Player.Alive && !Bot.ShouldExit)
                    Bot.Sleep(500);
                Bot.Sleep(250);

                if (crystalDeathCount < 2)
                    continue;

                Core.DisableSkills();
                C.Logger("2nd crystal phase death: restarting room to avoid desync.");
                tauntCounter = 0;
                crystalDeathCount = 0;
                gramielFightStartTime = DateTime.MinValue;

                Core.Join("whitemap");
                Bot.Wait.ForMapLoad("whitemap");
                Ultra.ClearSyncFile(syncPath);
                Bot.Sleep(2500);
                Prep(skipEnhancements: true);
                Ultra.WaitForArmy(3, "UltraItemCheck.sync");

                Core.Join(map);
                Bot.Wait.ForMapLoad(map);
                Ultra.WaitForArmy(3, "ultra_gramiel.sync");
                Core.ChooseBestCell("*");
                Bot.Player.SetSpawnPoint();
                Core.EnableSkills();
                continue;
            }

            if (Bot.Map.PlayerCount < 3)
            {
                Core.DisableSkills();
                C.Logger("Army member missing; restarting room.");
                tauntCounter = 0;
                crystalDeathCount = 0;
                gramielFightStartTime = DateTime.MinValue;

                Core.Join("whitemap");
                Bot.Wait.ForMapLoad("whitemap");
                Prep(skipEnhancements: true);
                Ultra.WaitForArmy(3, "ultra_gramiel.sync");

                Core.Join(map);
                Bot.Wait.ForMapLoad(map);
                Core.ChooseBestCell("*");
                Bot.Player.SetSpawnPoint();
                Core.EnableSkills();
                continue;
            }

            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                Bot.Sleep(1000);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => C.CheckInventory("Gramiel the Graceful Vanquished", 1), syncPath))
            {
                Core.DisableSkills();
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                if (!Bot.Quests.IsDailyComplete(10301))
                    C.EnsureComplete(10301);
                break;
            }

            AttackWithPriority();
            Bot.Sleep(250);
        }
    }

    void DoEnhs()
    {
        string className = Bot.Player.CurrentClass?.Name.ToLower() ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        switch (className)
        {
            case "stonecrusher":
            case "infinity titan":
                Adv.EnhanceEquipped(type: EnhancementType.Fighter, hSpecial: HelmSpecial.Anima, wSpecial: WeaponSpecial.Valiance, cSpecial: CapeSpecial.Absolution);
                break;
            case "lord of order":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Forge, wSpecial: WeaponSpecial.Arcanas_Concerto, cSpecial: CapeSpecial.Penitence);
                break;
            case "lightcaster":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Pneuma, wSpecial: WeaponSpecial.Ravenous, cSpecial: CapeSpecial.Penitence);
                break;
            case "verus doomknight":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, hSpecial: HelmSpecial.Anima, wSpecial: WeaponSpecial.Ravenous, cSpecial: CapeSpecial.Vainglory);
                break;
            case "archpaladin":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, wSpecial: WeaponSpecial.Awe_Blast, cSpecial: CapeSpecial.Penitence);
                break;
            case "void highlord":
                Adv.EnhanceEquipped(type: EnhancementType.Lucky, wSpecial: WeaponSpecial.Valiance, cSpecial: CapeSpecial.Lament);
                break;
            default:
                Adv.SmartEnhance(Bot.Player.CurrentClass!.Name);
                break;
        }
    }

    void AttackWithPriority()
    {
        string className = Bot.Player.CurrentClass?.Name ?? string.Empty;
        const int gramielMapId = 1;

        if (shouldExecuteTaunt)
        {
            shouldExecuteTaunt = false;
            Core.DisableSkills();
            Bot.Sleep(500);

            C.Logger($"{className} executing taunt #{tauntCounter}.");

            int attempts = 0;
            bool tauntLanded = false;
            while (!Bot.ShouldExit && attempts < 15)
            {
                if (!Bot.Player.Alive)
                    break;

                if (!Bot.Player.HasTarget)
                    Bot.Combat.Attack(crystalMapId);

                if (Bot.Skills.CanUseSkill(5))
                    Bot.Skills.UseSkill(5);

                Bot.Sleep(500);
                attempts++;

                if (Bot.Player.HasTarget && Bot.Target?.Auras?.Any(a => a?.Name == "Focus") == true)
                {
                    tauntLanded = true;
                    Bot.Sleep(500);
                    break;
                }
            }

            if (!tauntLanded)
                C.Logger($"Taunt #{tauntCounter} did not land after {attempts} attempts.");

            Core.EnableSkills();
            Bot.Sleep(300);
            return;
        }

        bool primaryCrystalAlive = Bot.Monsters.CurrentAvailableMonsters.Any(x => x != null && x.Alive && x.MapID == crystalMapId);

        int targetCrystalMapId = crystalMapId;
        if (!primaryCrystalAlive)
        {
            int otherCrystalMapId = crystalMapId == 2 ? 3 : 2;
            bool otherCrystalAlive = Bot.Monsters.CurrentAvailableMonsters.Any(x => x != null && x.Alive && x.MapID == otherCrystalMapId);
            if (otherCrystalAlive)
                targetCrystalMapId = otherCrystalMapId;
        }

        bool anyCrystalAlive = Bot.Monsters.CurrentAvailableMonsters.Any(x => x != null && x.Alive && (x.MapID == 2 || x.MapID == 3));

        if (anyCrystalAlive)
        {
            Bot.Combat.Attack(targetCrystalMapId);
        }
        else
        {
            if (gramielFightStartTime == DateTime.MinValue)
            {
                gramielFightStartTime = DateTime.Now;
                C.Logger("Both crystals are down; starting Gramiel taunt timer.");
            }

            Bot.Combat.Attack(gramielMapId);

            TimeSpan timeSinceFightStart = DateTime.Now - gramielFightStartTime;
            double currentTime = timeSinceFightStart.TotalSeconds;
            double timeInCycle = (currentTime - tauntOffsetSeconds) % TauntIntervalSeconds;

            bool inTauntWindow = timeInCycle >= 0 && timeInCycle < TauntWindowSeconds;
            bool noFocusAura = Bot.Player.HasTarget && (Bot.Target?.Auras?.Any(a => a?.Name == "Focus") != true);

            if (inTauntWindow && noFocusAura)
            {
                Core.DisableSkills();
                Bot.Sleep(500);

                int attempts = 0;
                while (!Bot.ShouldExit && attempts < 15)
                {
                    if (!Bot.Player.Alive)
                        break;

                    if (!Bot.Player.HasTarget)
                        Bot.Combat.Attack(gramielMapId);

                    if (Bot.Skills.CanUseSkill(5))
                        Bot.Skills.UseSkill(5);

                    Bot.Sleep(500);
                    attempts++;

                    if (Bot.Player.HasTarget && Bot.Target?.Auras?.Any(a => a?.Name == "Focus") == true)
                    {
                        Bot.Sleep(500);
                        break;
                    }
                }

                Core.EnableSkills();
                Bot.Sleep(300);
            }
        }
    }

    private void GramielMessageListener(dynamic packet)
    {
        try
        {
            string type = packet["params"].type;
            if (type is not "json")
                return;

            if (!Bot.Player.Alive)
                return;

            dynamic data = packet["params"].dataObj;
            string cmd = data.cmd.ToString();
            if (cmd != "ct")
                return;

            if (data.anims is null)
                return;

            foreach (dynamic anim in data.anims)
            {
                if (anim is null || anim.msg is null)
                    continue;

                string message = (string)anim.msg;
                if (!message.Contains("The Grace Crystal prepares a defense shattering attack!", StringComparison.OrdinalIgnoreCase))
                    continue;

                TimeSpan timeSinceLastWarning = DateTime.Now - lastTauntWarningTime;
                if (timeSinceLastWarning.TotalSeconds < 2)
                    return;

                lastTauntWarningTime = DateTime.Now;
                tauntCounter++;

                bool shouldTaunt = (isT1Taunter && tauntCounter % 2 == 1) || (!isT1Taunter && tauntCounter % 2 == 0);
                if (shouldTaunt)
                    shouldExecuteTaunt = true;
            }
        }
        catch { }
    }

    public enum GramielComp
    {
        Unselected,
        Recommended,
        Alternate
    }

    public enum CustomRole
    {
        Unselected,
        LeftCrystalT1,
        LeftCrystalT2,
        RightCrystalT1,
        RightCrystalT2,
    }
}
