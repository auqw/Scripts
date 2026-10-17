/*
name: Kolr Solo Lich Farm
description: Repeatedly solo Kolr with post-potion-patch Elysium Lich and required Felicitous Philtre until stopped.
tags: kolr, solo, lich, ultra, farm
*/
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreEnginev3.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
using System;
using System.IO;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class KolrSoloLichFarm
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots C => CoreBots.Instance;
    private static CoreAdvanced Adv => new();
    private static CoreEnginev3 Engine => CoreEnginev3.Instance;
    private const string Map = "flameusurper";
    private const string Boss = "Kolr, Usurper of Flames";
    private const string FarmDrop = "Great Flame of Yew";
    private const string FelicitousPhiltre = "Felicitous Philtre";

    private static string _fbsMuteFile = "";
    private int _rotationStep;
    private long _finalPhaseStartedAt;
    private bool _finalRecoveryUsed;
    private string _lastPhase = "";
    private string _rotationMode = "";
    private bool _emergencyRecovery;

    public void ScriptMain(IScriptInterface bot)
    {
        C.SetOptions(true);
        _fbsMuteFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Skua", "fbs_mute.sync"
        );

        try
        {
            RefreshFbsMute();
            Engine.Boot();
            PrepareLich();
            PrepareFelicitousPhiltre();
            Run();
        }
        finally
        {
            Engine.DisableSkills();
            try { if (File.Exists(_fbsMuteFile)) File.Delete(_fbsMuteFile); } catch { }
            C.SetOptions(false);
        }
    }

    private void PrepareLich()
    {
        if (!C.CheckInventory("Lich"))
        {
            Bot.StopSync();
            return;
        }

        C.Equip("Lich");
        Bot.Sleep(1500);

        if (!string.Equals(Bot.Player.CurrentClass?.Name, "Lich", StringComparison.OrdinalIgnoreCase))
        {
            Bot.StopSync();
            return;
        }

        Adv.EnhanceEquipped(
            type: EnhancementType.Lucky,
            hSpecial: HelmSpecial.Examen,
            wSpecial: WeaponSpecial.Elysium,
            cSpecial: CapeSpecial.Penitence
        );
        Bot.Sleep(2500);
    }

    private void PrepareFelicitousPhiltre()
    {
        if (Bot.ShouldExit)
            return;

        if (!C.CheckInventory(FelicitousPhiltre, 15))
        {
            const string voucher = "Gold Voucher 100k";
            const int shopId = 2036;
            const string shopMap = "alchemyacademy";

            if (!C.CheckInventory(voucher, 2))
                C.BuyItem(shopMap, shopId, voucher, 2);

            if (C.CheckInventory(voucher, 2))
                C.BuyItem(shopMap, shopId, FelicitousPhiltre, 25);
        }

        if (!C.CheckInventory(FelicitousPhiltre, 15))
        {
            Bot.StopSync();
            return;
        }

        if (!Bot.Inventory.IsEquipped(FelicitousPhiltre))
        {
            Bot.Inventory.EquipUsableItem(FelicitousPhiltre);
            Bot.Wait.ForTrue(() => Bot.Inventory.IsEquipped(FelicitousPhiltre), 20);
        }

        if (!Bot.Inventory.IsEquipped(FelicitousPhiltre))
        {
            Bot.StopSync();
            return;
        }
    }

    private void Run()
    {
        if (Bot.ShouldExit)
            return;

        C.AddDrop(FarmDrop);
        int successfulKills = 0;

        while (!Bot.ShouldExit)
        {
            PrepareFelicitousPhiltre();
            if (Bot.ShouldExit)
                return;

            ResetEncounterState();
            if (!FightOnce())
                return;

            successfulKills++;
            C.Logger(
                $"[Kolr Farm] Kolr kills: {successfulKills}. "
                + $"{FarmDrop}: {Bot.Inventory.GetQuantity(FarmDrop)}."
            );

            // Leave the encounter so the next join creates a fresh fight.
            C.Join("Whitemap");
            Bot.Sleep(1000);
        }
    }

    private void ResetEncounterState()
    {
        _rotationStep = 0;
        _finalPhaseStartedAt = 0;
        _finalRecoveryUsed = false;
        _lastPhase = "";
        _rotationMode = "";
        _emergencyRecovery = false;
    }

    private bool FightOnce()
    {
        if (Bot.ShouldExit)
            return false;

        C.Join(Map);
        Engine.ChooseBestCellOneMonster(Boss);
        Bot.Player.SetSpawnPoint();
        Engine.DisableSkills(); // This fight uses the phase-aware rotation below.
        Bot.Sleep(1500);
        bool bossWasEngaged = false;

        while (!Bot.ShouldExit)
        {
            RefreshFbsMute();

            if (bossWasEngaged
                && (!Bot.Player.HasTarget || Bot.Player.Target?.HP <= 0))
            {
                Engine.DisableSkills();
                Bot.Sleep(1200); // Allow configured drops to be picked up.
                return true;
            }

            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                _rotationStep = 0;
                _finalPhaseStartedAt = 0;
                _finalRecoveryUsed = false;
                _rotationMode = "";
                _emergencyRecovery = false;
                bossWasEngaged = false;
                Bot.Sleep(1000);
                continue;
            }

            if (Bot.Player.Target?.Name != Boss)
            {
                Bot.Combat.Attack(Boss);
                Bot.Sleep(250);
                continue;
            }

            bossWasEngaged = true;

            MaintainFelicitousPhiltre();
            RunPhaseAwareRotation();
            Bot.Sleep(IsReflectPhase() ? 225 : 100);
        }

        return false;
    }

    private void MaintainFelicitousPhiltre()
    {
        if (SelfHasAura(FelicitousPhiltre))
            return;

        if (!C.CheckInventory(FelicitousPhiltre, 1)
            || !Bot.Inventory.IsEquipped(FelicitousPhiltre))
        {
            Bot.StopSync();
            return;
        }

        if (!Bot.Skills.CanUseSkill(5))
            return;

        Bot.Skills.UseSkill(5);
    }

    private void RunPhaseAwareRotation()
    {
        string phase = GetPhaseName();
        if (!string.Equals(phase, _lastPhase, StringComparison.Ordinal))
        {
            _lastPhase = phase;
            _rotationStep = 0;
            _rotationMode = "";
            _emergencyRecovery = false;
        }

        // At 10%, Kolr becomes invulnerable and applies a lethal max-HP DoT.
        // Let the Lich passive absorb the dangerous part, then use empowered
        // Wither as soon as Kolr can take damage again.
        if (TargetHasAura("Invulnerable") || TargetHasAura("Meltdown"))
        {
            if (_finalPhaseStartedAt == 0)
                _finalPhaseStartedAt = Environment.TickCount64;

            if (!TargetHasAura("Invulnerable") && !_finalRecoveryUsed)
            {
                UseEmpowered(2); // Skill 3: Wither (lifesteal).
                _finalRecoveryUsed = true;
            }
            return;
        }

        if (_finalPhaseStartedAt > 0 && !_finalRecoveryUsed)
        {
            UseEmpowered(2);
            _finalRecoveryUsed = true;
            return;
        }

        bool phaseOne = TargetHasAura("Flames of Maleno");
        bool phaseFour = TargetHasAura("Flames of Rubedo");
        bool putrefaction = SelfHasAura("Putrefaction");
        double hpRatio = Bot.Player.MaxHealth > 0
            ? (double)Bot.Player.Health / Bot.Player.MaxHealth
            : 0;

        if (phaseOne && (putrefaction || hpRatio < 0.80))
        {
            SetRotationMode("Recovery-P1",
                putrefaction
                    ? "Putrefaction active"
                    : $"low HP ({Bot.Player.Health}/{Bot.Player.MaxHealth})"
            );
            RunSequence(new[] { 4, 2, 4, 1, 2 });
            return;
        }

        if (phaseOne)
        {
            SetRotationMode("Damage-P1", "Putrefaction absent and HP safe");
            RunSequence(new[] { 4, 3, 4, 2, 4, 1 });
            return;
        }

        if (!_emergencyRecovery && hpRatio < 0.25)
            _emergencyRecovery = true;
        else if (_emergencyRecovery && hpRatio >= 0.50)
            _emergencyRecovery = false;

        if (_emergencyRecovery)
        {
            SetRotationMode(
                phaseFour ? "Emergency-P4" : "Emergency",
                $"critical HP ({Bot.Player.Health}/{Bot.Player.MaxHealth})"
            );
            RunSequence(phaseFour
                ? new[] { 4, 2, 4, 1, 2 }
                : new[] { 4, 2, 4, 1 });
            return;
        }

        SetRotationMode(phaseFour ? "Damage-P4" : "Damage", "fixed damage rotation");

        if (phaseFour)
            RunSequence(new[] { 4, 3, 4, 2, 4, 1, 4, 2 });
        else
            RunSequence(new[] { 4, 3, 4, 2, 4, 1 });
    }

    private void SetRotationMode(string mode, string reason)
    {
        if (string.Equals(mode, _rotationMode, StringComparison.Ordinal))
            return;

        string previousMode = _rotationMode;
        int previousStep = _rotationStep;
        _rotationMode = mode;
        _rotationStep = 0;

        bool enteringSafetyMode = mode.StartsWith("Recovery", StringComparison.Ordinal)
            || mode.StartsWith("Emergency", StringComparison.Ordinal);
        bool leavingSafetyMode = previousMode.StartsWith("Recovery", StringComparison.Ordinal)
            || previousMode.StartsWith("Emergency", StringComparison.Ordinal);

        if (enteringSafetyMode
            && previousMode.StartsWith("Damage", StringComparison.Ordinal))
        {
            if (previousStep == 5)
                _rotationStep = 3;
            else if (previousStep == 1 || previousStep == 3 || previousStep == 7)
                _rotationStep = 1;
        }
        else if (mode.StartsWith("Damage", StringComparison.Ordinal)
            && leavingSafetyMode)
        {
            if (previousStep == 3)
                _rotationStep = 5;
            else if (previousStep == 1 || previousStep == 4)
                _rotationStep = 3;
        }

    }

    private void RunSequence(int[] sequence)
    {
        if (_rotationStep >= sequence.Length)
            _rotationStep = 0;

        int skill = sequence[_rotationStep];
        if (!Bot.Skills.CanUseSkill(skill))
            return;

        Bot.Skills.UseSkill(skill);
        _rotationStep = (_rotationStep + 1) % sequence.Length;
    }

    private void UseEmpowered(int skill)
    {
        if (Bot.Skills.CanUseSkill(4))
        {
            Bot.Skills.UseSkill(4);
            Bot.Sleep(150);
        }

        if (Bot.Skills.CanUseSkill(skill))
            Bot.Skills.UseSkill(skill);
    }

    private bool IsReflectPhase() =>
        TargetHasAura("Flames of Citrinitas") || TargetHasAura("Burning Ward");

    private string GetPhaseName()
    {
        if (TargetHasAura("Invulnerable") || TargetHasAura("Meltdown"))
            return "Phase 5 (Forges)";
        if (TargetHasAura("Flames of Rubedo"))
            return "Phase 4 (Rubedo)";
        if (TargetHasAura("Flames of Citrinitas"))
            return "Phase 3 (Citrinitas/reflect)";
        if (TargetHasAura("Flames of Albedo"))
            return "Phase 2 (Albedo/purge)";
        if (TargetHasAura("Flames of Maleno"))
            return "Phase 1 (Maleno/Putrefaction)";
        return "transition/unknown phase";
    }

    private static bool TargetHasAura(string name) =>
        Bot.Target?.Auras?.Any(a => a != null &&
            a.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) == true;

    private static bool SelfHasAura(string name) =>
        Bot.Self.Auras?.Any(a => a != null &&
            a.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) == true;

    private static void RefreshFbsMute()
    {
        try
        {
            File.WriteAllText(
                _fbsMuteFile,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
            );
        }
        catch { }
    }
}
