/*
name: Masakado King's Echo
description: Solo or army helper for Victor Matsuri's final boss, Masakado, using King's Echo and Royal Resolve.
tags: masakado, victor matsuri, kings echo, king's echo, army, solo, royal resolve
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Ultrasv3/Entwined Eclipse/CoreUltra.cs
//cs_include Scripts/Ultrasv3/Entwined Eclipse/CoreEngine.cs
//cs_include Scripts/Army/CoreArmyLite.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Skills;
using Skua.Core.Options;
using System;
using System.IO;
using System.Linq;

public class MasakadoKingsEchoArmy
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreUltra Ultra
    {
        get => _Ultra ??= new CoreUltra();
        set => _Ultra = value;
    }
    private static CoreUltra _Ultra;
    private static CoreArmyLite sArmy
    {
        get => _sArmy ??= new CoreArmyLite();
        set => _sArmy = value;
    }
    private static CoreArmyLite _sArmy;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "MasakadoKingsEchoArmy";
    public List<IOption> Options = new()
    {
        new Option<string>("player1", "King's Echo",    "AQW account name for the King's Echo slot. Falls back to LR → Arcana Invoker → ArchMage if KE is missing.", ""),
        new Option<string>("player2", "StoneCrusher",  "AQW account name for the StoneCrusher slot. Falls back to ArchPaladin → LoO → LR.", ""),
        new Option<string>("player3", "Legion Revenant", "AQW account name for the Legion Revenant slot. Falls back to ArchPaladin → LoO → LightCaster.", ""),
        new Option<string>("player4", "Lord of Order", "AQW account name for the Lord of Order slot. Falls back to ArchPaladin → LightCaster → LR.", ""),
        sArmy.packetDelay,
        CoreBots.Instance.SkipOptions,
        new Option<bool>(
            "armyMode",
            "Army Mode",
            "OFF: run as a solo helper. ON: wait for and sync 4 configured accounts.",
            false
        ),
        new Option<bool>(
            "autoEnhance",
            "Auto-Apply Enhancements",
            "Applies recommended enhancements for the selected/fallback class.",
            true
        ),
        new Option<bool>(
            "allowFallbackClass",
            "Fallback If No King's Echo",
            "ON: use the best available fallback class if King's Echo is missing. OFF: stop if King's Echo is missing.",
            true
        ),
        new Option<bool>(
            "useRevitalize",
            "Use Potent Revitalize Elixir",
            "Equips and periodically uses Potent Revitalize Elixir if the item is present.",
            true
        ),
        new Option<bool>(
            "buyRevitalize",
            "Buy Revitalize Elixir",
            "Buys Potent Revitalize Elixir from Gebo's /alchemyacademy merge shop when missing.",
            true
        ),
    };

    const int QuestID = 10295;
    const string Map = "victormatsuri";
    const string Cell = "r8";
    const string Pad = "Left";
    const string Monster = "Masakado";
    const string KingsEcho = "King's Echo";
    const string LegionRevenant = "Legion Revenant";
    const string ArchMage = "ArchMage";
    const string ArchPaladin = "ArchPaladin";
    const string StoneCrusher = "StoneCrusher";
    const string LordOfOrder = "Lord of Order";
    const string LightCaster = "LightCaster";
    static readonly string[] FallbackClasses = new[]
    {
        LegionRevenant,
        ArchMage,
        "Chaos Avenger",
        "Void HighLord",
        "Dragon of Time",
        "Arcana Invoker",
        LightCaster,
        ArchPaladin,
        LordOfOrder,
        StoneCrusher,
        "Verus DoomKnight",
        "Verus Doomknight",
        "Cavern Celestite",
        "Elemental Warrior",
        "Shaman",
    };
    const string Revitalize = "Potent Revitalize Elixir";
    const int RevitalizeTarget = 20;
    const int RevitalizeBuyClicks = 4;

    bool armyMode;
    bool autoEnhance;
    bool allowFallbackClass;
    bool useRevitalize;
    bool buyRevitalize;

    /// <summary>
    /// Orchestration hook: when true, skip Core.SetOptions(true/false). Used when
    /// this script is invoked from a parent orchestrator that already runs its own
    /// SetOptions and shouldn't re-prompt the options panel or trigger cleanup.
    /// </summary>
    public bool SkipSetOptions;
    string activeClass = KingsEcho;
    bool usingUnknownFallback;
    bool openedWithPraxis;
    bool openedWithDefense;
    int syncCount;
    DateTime lastPotionUse = DateTime.MinValue;
    DateTime lastRoyalResolve = DateTime.MinValue;

    public void ScriptMain(IScriptInterface bot)
    {
        if (!SkipSetOptions)
            Core.SetOptions(disableClassSwap: true);

        try
        {
            armyMode = Bot.Config!.Get<bool>("armyMode");
            autoEnhance = Bot.Config!.Get<bool>("autoEnhance");
            allowFallbackClass = Bot.Config!.Get<bool>("allowFallbackClass");
            useRevitalize = Bot.Config!.Get<bool>("useRevitalize");
            buyRevitalize = Bot.Config!.Get<bool>("buyRevitalize");

            if (armyMode)
            {
                if (sArmy.Players().Length < 4)
                {
                    Core.Logger("Add 4 account names in the script options before starting Army Mode.");
                    return;
                }

                Core.PrivateRooms = true;
                if (Core.PrivateRoomNumber < 1000 || Core.PrivateRoomNumber > 99999)
                    Core.PrivateRoomNumber = sArmy.getRoomNr();

                Core.Logger($"Masakado army mode enabled: {sArmy.Players().Length} accounts, private room #{Core.PrivateRoomNumber}.");
            }
            else
            {
                Core.PrivateRooms = false;
                Core.Logger("Masakado solo mode enabled.");
            }

            EquipFightClass();
            if (autoEnhance)
                ApplyClassEnhancements();
            if (useRevitalize)
                EquipRevitalize();

            Core.EnsureAccept(QuestID);
            var quest = Core.EnsureLoad(QuestID);
            string item = quest.Requirements[0].ToString();
            bool isTemp = quest.Requirements[0].Temp;
            int quant = 1;

            if (!isTemp)
                Core.AddDrop(item);

            SyncArmy("masakado_ready.sync");
            Core.Logger($"Fighting {Monster} for quest {QuestID}: {item}.");

            while (!Bot.ShouldExit && !HasRequirement(item, isTemp, quant))
            {
                EnsureAtMasakado();

                if (!Bot.Player.Alive)
                {
                    Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                    Bot.Sleep(500);
                    EnsureAtMasakado();
                    continue;
                }

                if (Bot.Player.Target == null || !string.Equals(Bot.Player.Target.Name, Monster, StringComparison.OrdinalIgnoreCase))
                    Bot.Combat.Attack(Monster);

                OpenWithPraxis();
                OpenWithDefense();
                if (AvoidCounterAttack())
                {
                    Bot.Sleep(500);
                    continue;
                }

                UseRevitalizeIfReady();
                KeepRoyalResolveUp();
                Bot.Sleep(500);
            }

            SyncArmy("masakado_done.sync");

            if (HasRequirement(item, isTemp, quant))
                Core.EnsureComplete(QuestID);
        }
        finally
        {
            if (!SkipSetOptions)
                Core.SetOptions(false);
        }
    }

    void EquipFightClass()
    {
        activeClass = SelectFightClass();

        if (string.IsNullOrWhiteSpace(activeClass))
        {
            Core.Logger($"{Core.Username()} does not have {KingsEcho} or a known fallback class, and no class is currently equipped.", stopBot: true);
            return;
        }

        if (!string.Equals(Bot.Player.CurrentClass?.Name, activeClass, StringComparison.OrdinalIgnoreCase))
        {
            Core.Equip(activeClass);
            Bot.Wait.ForItemEquip(activeClass);
            Bot.Sleep(1000);
        }

        Bot.Skills.StartAdvanced(usingUnknownFallback ? activeClass : SkillRotation(activeClass), 250, SkillUseMode.WaitForCooldown);
    }

    string SelectFightClass()
    {
        if (armyMode)
            return SelectArmyClass();

        if (Core.CheckInventory(KingsEcho))
            return KingsEcho;

        if (!allowFallbackClass)
        {
            Core.Logger($"{Core.Username()} does not have {KingsEcho}. Enable fallback if you want LR/ArchMage used instead.", stopBot: true);
            return "";
        }

        foreach (string className in FallbackClasses)
            if (Core.CheckInventory(className))
                return className;

        string equippedClass = Bot.Player.CurrentClass?.Name ?? "";
        if (!string.IsNullOrWhiteSpace(equippedClass))
        {
            usingUnknownFallback = true;
            Core.Logger($"{Core.Username()} has no known fallback class; using currently equipped class: {equippedClass}.");
            return equippedClass;
        }

        return "";
    }

    string SelectArmyClass()
    {
        string username = Core.Username();
        string? p1 = Bot.Config!.Get<string>("player1");
        string? p2 = Bot.Config!.Get<string>("player2");
        string? p3 = Bot.Config!.Get<string>("player3");
        string? p4 = Bot.Config!.Get<string>("player4");

        if (username.Equals(p1, StringComparison.OrdinalIgnoreCase))
            return FirstOwned(KingsEcho, LegionRevenant, "Arcana Invoker", ArchMage);

        if (username.Equals(p2, StringComparison.OrdinalIgnoreCase))
            return FirstOwned(StoneCrusher, ArchPaladin, LordOfOrder, LegionRevenant);

        if (username.Equals(p3, StringComparison.OrdinalIgnoreCase))
            return FirstOwned(LegionRevenant, ArchPaladin, LordOfOrder, LightCaster);

        if (username.Equals(p4, StringComparison.OrdinalIgnoreCase))
            return FirstOwned(LightCaster, LordOfOrder, ArchPaladin, LegionRevenant);

        Core.Logger($"{username} was not matched to an army slot; using solo fallback selection.");
        return FirstOwned(KingsEcho, LegionRevenant, ArchMage, "Arcana Invoker", ArchPaladin, LightCaster, LordOfOrder);
    }

    string FirstOwned(params string[] classNames)
    {
        foreach (string className in classNames)
            if (Core.CheckInventory(className))
                return className;

        if (!allowFallbackClass)
            return "";

        foreach (string className in FallbackClasses)
            if (Core.CheckInventory(className))
                return className;

        string equippedClass = Bot.Player.CurrentClass?.Name ?? "";
        if (!string.IsNullOrWhiteSpace(equippedClass))
        {
            usingUnknownFallback = true;
            Core.Logger($"{Core.Username()} has none of the comp classes; using currently equipped class: {equippedClass}.");
            return equippedClass;
        }

        return "";
    }

    string SkillRotation(string className) =>
        className.ToLower() switch
        {
            "king's echo" => "3|4|2|5|4|3|2|4|5",
            "legion revenant" => "4|3|2|5|4|2|3|4|2",
            "archmage" => "4|2|3|5|4|2|3|4|5",
            "chaos avenger" => "2|3|5|4|2|3|4|5",
            "void highlord" => "3|2|4|5|3|2|4",
            "dragon of time" => "3|2|4|2|1|2",
            "arcana invoker" => "4|3|5|3|5|3|5",
            "lightcaster" => "4|2|3|5|2|3|4|5",
            "archpaladin" => "4|2|3|5|2|4|3",
            "Lord of Order" => "2|3|4|5|2|3|4",
            "stonecrusher" => "3|4|2|5|3|4|2",
            "verus doomknight" => "2|3|4|5|2|3|4",
            "cavern celestite" => "2|3|4|5|2|3|4",
            "elemental warrior" => "2|3|4|5|2|3|4",
            "shaman" => "2|3|4|5|2|3|4",
            _ => "4|2|3|5"
        };

    void ApplyClassEnhancements()
    {
        if (usingUnknownFallback)
        {
            Core.Logger($"{Core.Username()} using unknown fallback class '{activeClass}' - skipping auto-enhancements.");
            return;
        }

        switch (activeClass.ToLower())
        {
            case "king's echo":
                Core.Logger($"{Core.Username()} applying King's Echo enhancements: Ravenous, Examen, Absolution.");
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    cSpecial: CapeSpecial.Absolution,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous
                );
                break;

            case "legion revenant":
                Core.Logger($"{Core.Username()} applying Legion Revenant Masakado enhancements: Praxis, Pneuma.");
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Praxis
                );
                break;

            case "archmage":
                Core.Logger($"{Core.Username()} applying ArchMage Masakado enhancements: Praxis, Pneuma, Absolution.");
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    cSpecial: CapeSpecial.Absolution,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Praxis
                );
                break;

            case "arcana invoker":
                Core.Logger($"{Core.Username()} applying Arcana Invoker Masakado enhancements: Praxis.");
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    wSpecial: WeaponSpecial.Praxis
                );
                break;

            case "stonecrusher":
                Core.Logger($"{Core.Username()} applying StoneCrusher support enhancements.");
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    cSpecial: Adv.uAbsolution() ? CapeSpecial.Absolution : CapeSpecial.None,
                    hSpecial: Adv.uAnima() ? HelmSpecial.Anima : HelmSpecial.None,
                    wSpecial: Adv.uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Awe_Blast
                );
                break;

            case "archpaladin":
                Core.Logger($"{Core.Username()} applying ArchPaladin support enhancements.");
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: Adv.uAbsolution() ? CapeSpecial.Absolution : CapeSpecial.None,
                    hSpecial: Adv.uForgeHelm() ? HelmSpecial.Forge : HelmSpecial.None,
                    wSpecial: Adv.uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Awe_Blast
                );
                break;

            case "lord of order":
                Core.Logger($"{Core.Username()} applying Lord of Order support enhancements.");
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    cSpecial: Adv.uPenitence() ? CapeSpecial.Penitence : CapeSpecial.None,
                    hSpecial: Adv.uForgeHelm() ? HelmSpecial.Forge : HelmSpecial.None,
                    wSpecial: Adv.uArcanasConcerto() ? WeaponSpecial.Arcanas_Concerto : WeaponSpecial.Awe_Blast
                );
                break;

            case "lightcaster":
                Core.Logger($"{Core.Username()} applying LightCaster support enhancements.");
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    cSpecial: Adv.uAbsolution() ? CapeSpecial.Absolution : CapeSpecial.None,
                    hSpecial: Adv.uPneuma() ? HelmSpecial.Pneuma : HelmSpecial.None,
                    wSpecial: Adv.uPraxis() ? WeaponSpecial.Praxis : WeaponSpecial.Awe_Blast
                );
                break;

            default:
                Core.Logger($"{Core.Username()} applying generic fallback enhancements for {activeClass}: Lucky, Ravenous if available.");
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    wSpecial: Adv.uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Awe_Blast
                );
                break;
        }
    }

    void EquipRevitalize()
    {
        if (buyRevitalize)
            StockRevitalize();

        if (!Core.CheckInventory(Revitalize))
        {
            Core.Logger($"{Core.Username()} does not have {Revitalize}; continuing without it.");
            return;
        }

        if (!Bot.Inventory.IsEquipped(Revitalize))
            Bot.Inventory.EquipUsableItem(Revitalize);
    }

    void UseRevitalizeIfReady()
    {
        if (!useRevitalize || !Core.CheckInventory(Revitalize))
            return;

        if (!Bot.Inventory.IsEquipped(Revitalize))
            Bot.Inventory.EquipUsableItem(Revitalize);

        if ((DateTime.Now - lastPotionUse).TotalSeconds < 30)
            return;

        Core.UsePotion();
        lastPotionUse = DateTime.Now;
    }

    void StockRevitalize()
    {
        const string voucher = "Gold Voucher 500k";
        const string map = "alchemyacademy";
        const int shop = 2036;

        if (Bot.Inventory.GetQuantity(Revitalize) >= RevitalizeTarget)
            return;

        int neededVouchers = RevitalizeBuyClicks * 2;
        int missingVouchers = Math.Max(0, neededVouchers - Bot.Inventory.GetQuantity(voucher));
        if (missingVouchers > 0)
        {
            Core.Logger($"Buying {missingVouchers}x {voucher} for {Revitalize}...");
            Core.BuyItem(map, shop, voucher, missingVouchers);
        }

        Core.Logger($"Stocking {Revitalize}: buying {RevitalizeBuyClicks} times from Gebo's merge shop.");
        Core.BuyItem(map, shop, Revitalize, RevitalizeTarget);
    }

    void KeepRoyalResolveUp()
    {
        if (!string.Equals(activeClass, KingsEcho, StringComparison.OrdinalIgnoreCase))
            return;

        if ((DateTime.Now - lastRoyalResolve).TotalSeconds < 5)
            return;

        Bot.Skills.UseSkill(4);
        lastRoyalResolve = DateTime.Now;
    }

    void OpenWithPraxis()
    {
        if (openedWithPraxis || !IsPlayer1())
            return;

        Core.Logger($"{Core.Username()} opening Masakado with Praxis on AQW button 2 / Skua skill 1 using {activeClass}.");
        Bot.Skills.UseSkill(1);
        openedWithPraxis = true;
        Bot.Sleep(1000);
    }

    void OpenWithDefense()
    {
        if (openedWithDefense || !Bot.Player.HasTarget)
            return;

        // Masakado uses Unsheathe once at the start, then Yokai Slayer 3s later.
        // This gives support classes a moment to establish defensive buffs before that hit.
        Bot.Sleep(1800);

        string className = activeClass.ToLower();
        int skill = className switch
        {
            "king's echo" => 4,
            "archpaladin" => 4,
            "Lord of Order" => 4,
            "lightcaster" => 4,
            "stonecrusher" => 3,
            _ => 0
        };

        if (skill > 0)
            Bot.Skills.UseSkill(skill);

        openedWithDefense = true;
        Bot.Sleep(800);
    }

    bool AvoidCounterAttack()
    {
        if (!Bot.Player.HasTarget || Bot.Target?.Auras == null)
            return false;

        bool countering = Bot.Target.Auras.Any(a =>
            a != null && string.Equals(a.Name, "Counter Attack", StringComparison.OrdinalIgnoreCase));

        if (!countering)
            return false;

        Core.Logger($"{Core.Username()} detected Counter Attack - pausing damage.");
        Bot.Combat.CancelAutoAttack();

        long end = Environment.TickCount64 + 4500;
        while (!Bot.ShouldExit && Bot.Player.Alive && Environment.TickCount64 < end)
        {
            UseSupportDuringCounter();
            Bot.Sleep(600);
        }

        return true;
    }

    void UseSupportDuringCounter()
    {
        switch (activeClass.ToLower())
        {
            case "archpaladin":
                Bot.Skills.UseSkill(2);
                break;

            case "lord of order":
                Bot.Skills.UseSkill(1);
                Bot.Sleep(250);
                Bot.Skills.UseSkill(2);
                Bot.Sleep(250);
                Bot.Skills.UseSkill(3);
                break;

            case "lightcaster":
                Bot.Skills.UseSkill(3);
                break;

            case "stonecrusher":
                Bot.Skills.UseSkill(3);
                Bot.Sleep(250);
                Bot.Skills.UseSkill(4);
                break;

            case "king's echo":
                Bot.Skills.UseSkill(4);
                break;
        }
    }

    bool IsPlayer1()
    {
        string? p1 = Bot.Config!.Get<string>("player1");
        return !string.IsNullOrWhiteSpace(p1)
            && Core.Username().Equals(p1, StringComparison.OrdinalIgnoreCase);
    }

    void EnsureAtMasakado()
    {
        string target = armyMode ? $"{Map}-{Core.PrivateRoomNumber}" : Map;

        if (!Bot.Map.Name.Equals(Map, StringComparison.OrdinalIgnoreCase))
        {
            Core.Join(target, Cell, Pad);
            Bot.Wait.ForMapLoad(Map);
            Bot.Sleep(1000);
        }

        if (!Bot.Player.Cell.Equals(Cell, StringComparison.OrdinalIgnoreCase))
        {
            Bot.Map.Jump(Cell, Pad, autoCorrect: false);
            Bot.Wait.ForCellChange(Cell);
            Bot.Sleep(500);
        }

        Bot.Player.SetSpawnPoint();
    }

    bool HasRequirement(string item, bool isTemp, int quant) =>
        isTemp
            ? Bot.TempInv.Contains(item, quant)
            : Bot.Inventory.Contains(item, quant) || Bot.Bank.Contains(item, quant);

    void SyncArmy(string syncFile)
    {
        int partySize = sArmy.Players().Length;
        if (!armyMode || partySize <= 1)
            return;

        string path = Ultra.ResolveSyncPath($"Masakado_{Core.PrivateRoomNumber}_{++syncCount}_{syncFile}");
        string username = Core.Username().ToLower();

        while (!Bot.ShouldExit)
        {
            string[] lines = ReadSyncLines(path);
            lines = lines.Where(line => !line.StartsWith($"{username}:", StringComparison.OrdinalIgnoreCase)).ToArray();
            WriteSyncLines(path, lines.Append($"{username}:ready:{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}").ToArray());

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            int ready = ReadSyncLines(path)
                .Select(line => line.Split(':'))
                .Where(parts => parts.Length >= 3 && long.TryParse(parts[2], out long ts) && now - ts <= 120)
                .Select(parts => parts[0])
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();

            if (ready >= partySize)
                return;

            Bot.Sleep(250);
        }
    }

    string[] ReadSyncLines(string path)
    {
        try
        {
            return File.Exists(path)
                ? File.ReadAllLines(path).Where(line => !string.IsNullOrWhiteSpace(line)).ToArray()
                : Array.Empty<string>();
        }
        catch
        {
            Bot.Sleep(100);
            return Array.Empty<string>();
        }
    }

    void WriteSyncLines(string path, string[] lines)
    {
        try
        {
            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);
            File.WriteAllLines(path, lines);
        }
        catch
        {
            Bot.Sleep(100);
        }
    }
}
