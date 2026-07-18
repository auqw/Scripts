//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreAdvanced2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreEngine2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreUltra2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraWaitForArmy.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class PrerequisitesChecker
{
    private CoreBots2 C => CoreBots2.Instance;
    private CoreAdvanced2 Adv => _Adv ??= new();
    private CoreAdvanced2? _Adv;
    private CoreEngine2 Core => CoreEngine2.Instance;
    private static CoreUltra2 Ultra => _Ultra ??= new CoreUltra2();
    private static CoreUltra2? _Ultra;
    public IScriptInterface Bot => IScriptInterface.Instance;

    public const string PrereqResultFile = "PrerequisiteArmyChecker.sync";
    public const string PrereqGateFile = "PrereqGate.sync";

    private static readonly string[] RequiredClasses =
    [
        "Verus DoomKnight",
        "Shaman",
        "StoneCrusher",
        "King's Echo",
        "ArchPaladin",
        "Dragon of Time",
        "ArchFiend"
    ];

    private static readonly string[] RaceKeys = ["Human", "Undead", "Dragonkin", "Chaos", "Elemental"];

    public void ScriptMain(IScriptInterface bot)
    {
        C.SetOptions(disableCoreSkills: true);
        Core.Boot();

        C.Logger("[PrerequisitesChecker] === Checking prerequisites for Ultras-v3 ===", "Info");
        C.Logger("", "Info");

        CheckClasses();
        CheckLevelAndGold();
        CheckForgeEnhancements();
        CheckDamageBoosts();
        CheckReputation();

        int total = _classFails + _levelGoldFails + _forgeFails + _boostFails + _repFails;
        if (total == 0)
            C.Logger("[PrerequisitesChecker] ✅ All prerequisites met! You're ready for Ultras-v3.", "Info");
        else
            C.Logger($"[PrerequisitesChecker] ❌ {total} prerequisite(s) failed. Fix the issues above before running ultras.", "Error");

        // Build and show message box only on failure
        if (total > 0)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Player: {C.Username()}");
            sb.AppendLine();
            foreach (var line in _messageLines)
                sb.AppendLine(line);
            sb.AppendLine();
            sb.AppendLine($"❌ {total} prerequisite(s) failed.");
            Bot.ShowMessageBox(sb.ToString(), "Ultras-v3 Prerequisites");
        }

        Core.DisableSkills();
        C.SetOptions(false);
        Bot.StopSync();
    }

    public void Enforce()
    {
        var failures = RunChecks();

        if (failures.Count == 0)
        {
            C.Logger("[PrerequisitesChecker] All prerequisites met.", "Info");
            return;
        }

        C.Logger(
            "[PrerequisitesChecker] FAILED — missing prerequisites:" + Environment.NewLine
            + string.Join(Environment.NewLine, failures.Select(f => $"  ✗ {f}")),
            "Error",
            messageBox: true,
            stopBot: true
        );
    }

    public bool Check()
    {
        _messageLines.Clear();
        var failures = RunChecks();

        if (failures.Count > 0)
        {
            C.Logger("[PrerequisitesChecker] Missing prerequisites:", "Warning");
            foreach (var f in failures)
                C.Logger($"  ✗ {f}", "Warning");

            var sb = new StringBuilder();
            sb.AppendLine($"Player: {C.Username()}");
            sb.AppendLine();
            foreach (var line in _messageLines)
                sb.AppendLine(line);
            sb.AppendLine();
            sb.AppendLine($"❌ {failures.Count} prerequisite(s) failed.");
            Bot.ShowMessageBox(sb.ToString(), "Ultras-v3 Prerequisites");
            return false;
        }

        C.Logger("[PrerequisitesChecker] All prerequisites met.", "Info");
        return true;
    }

    private List<string> RunChecks()
    {
        var failures = new List<string>();
        failures.AddRange(CheckClasses());
        failures.AddRange(CheckLevelAndGold());
        failures.AddRange(CheckForgeEnhancements());
        failures.AddRange(CheckDamageBoosts());
        failures.AddRange(CheckReputation());
        return failures;
    }

    private int _classFails, _levelGoldFails, _forgeFails, _boostFails, _repFails;
    private readonly List<string> _messageLines = new();

    #region Class Checks

    private List<string> CheckClasses()
    {
        _classFails = 0;
        var failures = new List<string>();

        C.Logger("[PrerequisitesChecker] --- Classes ---", "Info");
        _messageLines.Add("--- Classes ---");

        foreach (var cls in RequiredClasses)
        {
            if (!C.CheckInventory(cls))
            {
                C.Logger($"  ✗ {cls}: NOT OWNED", "Error");
                _messageLines.Add($"  ✗ {cls}: NOT OWNED — You must own this class to use and it must be rank 10");
                failures.Add($"Missing class: {cls}");
                continue;
            }

            int rank = C.CheckClassRank(ClassName: cls);
            if (rank < 9)
            {
                C.Logger($"  ✗ {cls}: rank {rank} (need 10)", "Error");
                _messageLines.Add($"  ✗ {cls}: rank {rank} — You need this class to be rank 10");
                failures.Add($"{cls} is rank {rank}, need rank 10");
            }
            else
            {
                C.Logger($"  ✓ {cls}: rank 10", "Info");
                _messageLines.Add($"  ✓ {cls}: rank 10");
            }
        }

        // Warrior — must be owned, any rank is fine
        if (!C.CheckInventory("Warrior"))
        {
            C.Logger("  ✗ Warrior: NOT OWNED", "Error");
            _messageLines.Add("  ✗ Warrior: NOT OWNED — You must own the Warrior class (any rank)");
            failures.Add("Missing class: Warrior");
        }
        else
        {
            int rank = C.CheckClassRank(ClassName: "Warrior");
            C.Logger($"  ✓ Warrior: rank {rank} (owned)", "Info");
            _messageLines.Add($"  ✓ Warrior: rank {rank} (owned)");
        }

        _classFails = failures.Count;
        return failures;
    }

    #endregion

    #region Level & Gold

    private List<string> CheckLevelAndGold()
    {
        _levelGoldFails = 0;
        var failures = new List<string>();

        C.Logger("[PrerequisitesChecker] --- Level & Gold ---", "Info");
        _messageLines.Add("--- Level & Gold ---");

        // Level >= 100
        int level = Bot.Player.Level;
        if (level >= 100)
        {
            C.Logger($"  ✓ Level: {level}", "Info");
            _messageLines.Add($"  ✓ Level: {level}");
        }
        else
        {
            C.Logger($"  ✗ Level: {level} (need ≥ 100)", "Error");
            _messageLines.Add($"  ✗ Level: {level} — Must be at least level 100");
            failures.Add($"Level is {level}, need ≥ 100");
        }

        // Gold >= 10 million
        long gold = Bot.Player.Gold;
        if (gold >= 10_000_000)
        {
            C.Logger($"  ✓ Gold: {gold:N0}", "Info");
            _messageLines.Add($"  ✓ Gold: {gold:N0}");
        }
        else
        {
            C.Logger($"  ✗ Gold: {gold:N0} (need ≥ 10,000,000)", "Error");
            _messageLines.Add($"  ✗ Gold: {gold:N0} — You need at least 30m gold");
            failures.Add($"Gold is {gold:N0}, need ≥ 10,000,000");
        }

        _levelGoldFails = failures.Count;
        return failures;
    }

    #endregion

    #region Forge Enhancements

    private List<string> CheckForgeEnhancements()
    {
        _forgeFails = 0;
        var failures = new List<string>();

        C.Logger("[PrerequisitesChecker] --- Forge Enhancements ---", "Info");
        _messageLines.Add("--- Forge Enhancements ---");

        // Weapon: Lacerate and Praxis unlocked
        bool hasLacerate = Adv.uLacerate();
        bool hasPraxis = Adv.uPraxis();
        if (hasLacerate && hasPraxis)
        {
            C.Logger("  ✓ Forge Weapon: Lacerate, Praxis", "Info");
            _messageLines.Add("  ✓ Forge Weapon: Lacerate, Praxis");
        }
        else
        {
            var missing = new List<string>();
            if (!hasLacerate) missing.Add("Lacerate");
            if (!hasPraxis) missing.Add("Praxis");
            C.Logger($"  ✗ Forge Weapon: missing {string.Join(", ", missing)}", "Error");
            _messageLines.Add($"  ✗ Forge Weapon: missing {string.Join(", ", missing)} — Complete the forge enhancement quests for Lacerate and Praxis");
            failures.Add($"Forge Weapon: missing {string.Join(", ", missing)}");
        }

        // Helm: all helm forges unlocked
        if (Adv.uForgeHelm())
        {
            C.Logger("  ✓ Forge Helm: all unlocked", "Info");
            _messageLines.Add("  ✓ Forge Helm: all unlocked");
        }
        else
        {
            C.Logger("  ✗ Forge Helm: not all helm forges unlocked", "Error");
            _messageLines.Add("  ✗ Forge Helm: not unlocked — Complete all of the Forge Helm quest");
            failures.Add("Forge Helm: not all helm forges unlocked");
        }

        // Cape: all cape forges unlocked
        if (Adv.uForgeCape())
        {
            C.Logger("  ✓ Forge Cape: all unlocked", "Info");
            _messageLines.Add("  ✓ Forge Cape: all unlocked");
        }
        else
        {
            C.Logger("  ✗ Forge Cape: not all cape forges unlocked", "Error");
            _messageLines.Add("  ✗ Forge Cape: not unlocked — Complete all of the Forge Cape quest");
            failures.Add("Forge Cape: not all cape forges unlocked");
        }

        _forgeFails = failures.Count;
        return failures;
    }

    #endregion

    #region Damage Boosts

    private List<string> CheckDamageBoosts()
    {
        _boostFails = 0;
        var failures = new List<string>();

        C.Logger("[PrerequisitesChecker] --- Damage Boosts ---", "Info");
        _messageLines.Add("--- Damage Boosts ---");

        // --- Weapon boost (dmgAll >= 1.4) ---
        var weapon = FindEquippedWeapon();
        if (weapon == null)
        {
            C.Logger("  ✗ Weapon: none equipped", "Error");
            _messageLines.Add("  ✗ Weapon: none equipped");
            failures.Add("Weapon boost: no weapon equipped");
        }
        else
        {
            float dmgAll = C.GetBoostFloat(weapon, "dmgAll");
            if (dmgAll >= 1.4f)
            {
                C.Logger($"  ✓ Weapon: {weapon.Name} ({dmgAll:P0} all)", "Info");
                _messageLines.Add($"  ✓ Weapon: {weapon.Name} ({dmgAll:P0} all)");
            }
            else
            {
                C.Logger($"  ✗ Weapon: {weapon.Name} has {dmgAll:P0} all (need ≥ 1.40)", "Error");
                _messageLines.Add($"  ✗ Weapon: {weapon.Name} has {dmgAll:P0} all — Needs at least a 40% all damage boost");
                failures.Add($"Weapon boost: {weapon.Name} has {dmgAll:P0} dmgAll, need ≥ 1.40");
            }
        }

        // --- Armor / Pet race boosts (all 5 races >= 1.3) ---
        var armor = Bot.Inventory.Items.FirstOrDefault(i => i.Equipped && i.Category == ItemCategory.Armor);
        var pet = FindEquippedPet();

        bool armorPass = false;
        if (armor != null)
        {
            var missing = RaceKeys.Where(r => C.GetBoostFloat(armor, r) < 1.3f).ToList();
            armorPass = missing.Count == 0;
            if (armorPass)
            {
                var vals = RaceKeys.Select(r => $"{r}={C.GetBoostFloat(armor, r):P0}");
                C.Logger($"  ✓ Armor: {armor.Name} ({string.Join(", ", vals)})", "Info");
                _messageLines.Add($"  ✓ Armor: {armor.Name} ({string.Join(", ", vals)})");
            }
            else
            {
                C.Logger($"  ✗ Armor: {armor.Name} missing {string.Join(", ", missing)} ≥ 1.30", "Error");
                _messageLines.Add($"  ✗ Armor: {armor.Name} — Needs at least 30% boost for each race ({string.Join(", ", missing)} below 30%)");
            }
        }
        else
        {
            C.Logger("  ✗ Armor: none equipped", "Error");
            _messageLines.Add("  ✗ Armor: none equipped");
        }

        bool petPass = false;
        if (pet != null)
        {
            var missing = RaceKeys.Where(r => C.GetBoostFloat(pet, r) < 1.3f).ToList();
            petPass = missing.Count == 0;
            if (petPass)
            {
                var vals = RaceKeys.Select(r => $"{r}={C.GetBoostFloat(pet, r):P0}");
                C.Logger($"  ✓ Pet: {pet.Name} ({string.Join(", ", vals)})", "Info");
                _messageLines.Add($"  ✓ Pet: {pet.Name} ({string.Join(", ", vals)})");
            }
            else
            {
                C.Logger($"  ✗ Pet: {pet.Name} missing {string.Join(", ", missing)} ≥ 1.30", "Error");
                _messageLines.Add($"  ✗ Pet: {pet.Name} — Needs at least 30% boost for each race ({string.Join(", ", missing)} below 30%)");
            }
        }

        if (armorPass || petPass)
        {
            // Pass — one of them covers all 5 races
        }
        else
        {
            string details = "";
            if (armor != null)
            {
                var missing = RaceKeys.Where(r => C.GetBoostFloat(armor, r) < 1.3f).ToList();
                details = $"armor missing: {string.Join(", ", missing)}";
            }
            if (pet != null)
            {
                var missing = RaceKeys.Where(r => C.GetBoostFloat(pet, r) < 1.3f).ToList();
                if (details.Length > 0) details += "; ";
                details += $"pet missing: {string.Join(", ", missing)}";
            }
            if (string.IsNullOrEmpty(details))
                details = "no armor or pet with race boosts found";
            C.Logger($"  ✗ Race Boosts: {details}", "Error");
            _messageLines.Add($"  ✗ Race Boosts: {details} — Needs at least 30% boost for each race");
            failures.Add($"Race boosts: {details}");
        }

        _boostFails = failures.Count;
        return failures;
    }

    private InventoryItem? FindEquippedWeapon()
    {
        var weaponCats = new[]
        {
            ItemCategory.Sword, ItemCategory.Axe, ItemCategory.Dagger,
            ItemCategory.Gun, ItemCategory.HandGun, ItemCategory.Rifle,
            ItemCategory.Bow, ItemCategory.Mace, ItemCategory.Gauntlet,
            ItemCategory.Polearm, ItemCategory.Staff, ItemCategory.Wand,
            ItemCategory.Whip
        };

        return Bot.Inventory.Items.FirstOrDefault(i =>
            i.Equipped && weaponCats.Contains(i.Category));
    }

    private InventoryItem? FindEquippedPet()
    {
        // Try inventory first
        var pet = Bot.Inventory.Items.FirstOrDefault(i =>
            i.Equipped && i.CategoryString?.Equals("Pet", StringComparison.OrdinalIgnoreCase) == true);

        if (pet != null)
            return pet;

        // Try Flash game object
        try
        {
            int petId = Bot.Flash.CallGameFunction<int>("world.myAvatar.intPetID");
            if (petId > 0)
                pet = Bot.Inventory.Items.FirstOrDefault(i => i.ID == petId);
        }
        catch { }

        return pet;
    }

    #endregion

    #region Reputation

    private List<string> CheckReputation()
    {
        _repFails = 0;
        var failures = new List<string>();

        C.Logger("[PrerequisitesChecker] --- Reputation ---", "Info");
        _messageLines.Add("--- Reputation ---");

        int alchRank = Bot.Reputation.GetRank("Alchemy");
        if (alchRank >= 8)
        {
            C.Logger($"  ✓ Alchemy: rank {alchRank}", "Info");
            _messageLines.Add($"  ✓ Alchemy: rank {alchRank}");
        }
        else
        {
            C.Logger($"  ✗ Alchemy: rank {alchRank} (need 8)", "Error");
            _messageLines.Add($"  ✗ Alchemy: rank {alchRank} — Must be rank 8 Alchemy");
            failures.Add($"Reputation: Alchemy rank {alchRank}, need 8");
        }

        int goodRank = Bot.Reputation.GetRank("Good");
        if (goodRank >= 10)
        {
            C.Logger($"  ✓ Good: rank {goodRank}", "Info");
            _messageLines.Add($"  ✓ Good: rank {goodRank}");
        }
        else
        {
            C.Logger($"  ✗ Good: rank {goodRank} (need 10)", "Error");
            _messageLines.Add($"  ✗ Good: rank {goodRank} — Must be rank 10 Good");
            failures.Add($"Reputation: Good rank {goodRank}, need 10");
        }

        _repFails = failures.Count;
        return failures;
    }

    #endregion

    #region Sync Gate

    /// <summary>
    /// Two-phase prereq gate:
    ///   Phase 1 — run check, write pass/fail to result file
    ///   Phase 2 — NewWaitForArmy to ensure ALL accounts registered
    ///   Phase 3 — read result file; any "0" → stop ALL
    /// </summary>
    public bool PrerequisiteSyncGate(int armySize)
    {
        // --- Phase 1: run prereq check, write result ---
        string resultPath = Ultra.ResolveSyncPath(PrereqResultFile);
        Ultra.ClearSyncFile(resultPath);

        bool passed = Check();
        string myKey = $"{Bot.Player.Username}|{Bot.Player.CurrentClass?.Name}".Replace(":", "-");
        Ultra.UpdateEntry(resultPath, myKey, passed ? "1" : "0");

        if (!passed)
            C.Logger($"[PrereqSync] {C.Username()} failed prerequisites — waiting for all accounts.", "Error");
        else
            C.Logger($"[PrereqSync] {C.Username()} passed — waiting for all accounts.", "Info");

        // --- Phase 2: wait for all accounts to arrive ---
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, PrereqGateFile, useSkill: false);

        // --- Phase 3: check results ---
        string[] lines = Ultra.ReadLines(resultPath);
        var failedAccounts = new List<string>();
        foreach (string line in lines)
        {
            string[] parts = line.Split(':');
            if (parts.Length >= 2 && parts[1] == "0")
            {
                string keyPart = parts[0];
                if (string.IsNullOrWhiteSpace(keyPart) || !keyPart.Contains('|'))
                    continue; // skip malformed / stale lines
                string accountName = keyPart.Split('|')[0];
                if (!string.IsNullOrWhiteSpace(accountName))
                    failedAccounts.Add(accountName);
            }
        }

        if (failedAccounts.Count > 0)
        {
            string who = string.Join(", ", failedAccounts);
            C.Logger($"[PrereqSync] {failedAccounts.Count} account(s) failed prerequisites: {who}. Stopping ALL.", "Error", stopBot: true);
            return false;
        }

        C.Logger("[PrereqSync] All accounts passed prerequisites! Proceeding to ultras.", "Info");
        return true;
    }

    #endregion
}
