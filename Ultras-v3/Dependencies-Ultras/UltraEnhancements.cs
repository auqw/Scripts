/*
name: UltraEnhancements
description: Centralised enhancement presets for all Ultra boss scripts.
tags: ultra, enhancements
*/

//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreFarms2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreAdvanced2.cs

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class UltraEnhancements
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots2 C => CoreBots2.Instance;

    private static CoreAdvanced2 Adv
    {
        get => _Adv ??= new CoreAdvanced2();
        set => _Adv = value;
    }
    private static CoreAdvanced2 _Adv;

    /// <summary>
    /// Applies the recommended enhancement preset for the currently equipped class.
    /// Falls back to SmartEnhance if the class has no explicit preset.
    /// </summary>
    public void Apply()
    {
        string className = Bot.Player?.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        C.Logger($"[UltraEnhancements] Enhancing for: {className}");

        switch (className)
        {
            // ── Taunters / Support ────────────────────────────────────────────

            case "ArchPaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "Lord Of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: Adv.uArcanasConcerto() ? WeaponSpecial.Arcanas_Concerto : WeaponSpecial.Awe_Blast,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "StoneCrusher":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Absolution
                );
                break;


            case "Verus DoomKnight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Lacerate,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "King's Echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: Adv.uValiance() ? WeaponSpecial.Valiance : WeaponSpecial.Spiral_Carve,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "Dragon of Time":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    wSpecial: WeaponSpecial.Elysium,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "Arachnomancer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Health_Vamp
                );
                break;

            case "Void Highlord":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "ArchFiend":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "Scion of Flames":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "Chaos Avenger":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Dauntless,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            default:
                C.Logger($"[UltraEnhancements] No preset for '{className}', using SmartEnhance.");
                Adv.SmartEnhance(className);
                break;
        }
    }

    public void ApplyTyndarius()
    {
        string className = Bot.Player?.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        C.Logger($"[UltraEnhancements] Tyndarius enhancing for: {className}");

        switch (className)
        {
            case "ArchPaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Lacerate
                );
                break;

            case "Lord Of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Awe_Blast,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "Shaman":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    wSpecial: Adv.uElysium() ? WeaponSpecial.Elysium : WeaponSpecial.Mana_Vamp,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "StoneCrusher":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "Arachnomancer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Health_Vamp
                );
                break;

            case "Scion of Flames":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    wSpecial: WeaponSpecial.Valiance
                );
                break;

            case "Dragon of Time":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    wSpecial: Adv.uElysium() ? WeaponSpecial.Elysium : WeaponSpecial.Mana_Vamp
                );
                break;

            case "King's Echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: Adv.uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Spiral_Carve,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "Verus DoomKnight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Lacerate,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "ArchFiend":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Valiance
                );
                break;

            default:
                C.Logger($"[UltraEnhancements] No preset for '{className}', using SmartEnhance.");
                Adv.SmartEnhance(className);
                break;
        }
    }
    public void ApplyDage()
    {
        string className = Bot.Player?.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        C.Logger($"[UltraEnhancements] Dage enhancing for: {className} using fixed Lucky preset");

        switch (className)
        {
            case "ArchPaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "Lord Of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "StoneCrusher":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    wSpecial: WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "Verus DoomKnight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "King's Echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Lament
                );
                break;


            default:
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    wSpecial: WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;
        }
    }

    public void ApplyDrakath()
    {
        string className = Bot.Player?.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        C.Logger($"[UltraEnhancements] Dage enhancing for: {className} using fixed Lucky preset");

        switch (className)
        {

            case "Lord Of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Absolution
                    
                );
                break;

            case "StoneCrusher":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Awe_Blast,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "Shaman":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: Adv.uElysium() ? WeaponSpecial.Elysium : WeaponSpecial.Mana_Vamp,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "ArchPaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Praxis,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            default:
                C.Logger($"[UltraEnhancements] No preset for '{className}', using SmartEnhance.");
                Adv.SmartEnhance(className);
                break;
        }
    }
    public void ApplyDarkon()
    {
        string className = Bot.Player?.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        C.Logger($"[UltraEnhancements] Dage enhancing for: {className} using fixed Lucky preset");

        switch (className)
        {

            case "Lord Of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Awe_Blast,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "StoneCrusher":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Absolution
                );
                break;


            case "Verus DoomKnight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Lacerate,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "King's Echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: Adv.uElysium() ? WeaponSpecial.Elysium : WeaponSpecial.Mana_Vamp,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "ArchPaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Lacerate,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            default:
                C.Logger($"[UltraEnhancements] No preset for '{className}', using SmartEnhance.");
                Adv.SmartEnhance(className);
                break;
        }
    }
    public void ApplySpeaker()
    {
        string className = Bot.Player?.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        C.Logger($"[UltraEnhancements] Dage enhancing for: {className} using fixed Lucky preset");

        switch (className)
        {
            case "ArchPaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Lacerate
                );
                break;

            case "Lord Of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Valiance
                );
                break;

            case "StoneCrusher":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Absolution
                );
                break;


            case "Verus DoomKnight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Praxis,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "Void Highlord":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Health_Vamp
                );
                break;

            case "King's Echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: Adv.uElysium() ? WeaponSpecial.Elysium : WeaponSpecial.Mana_Vamp,
                    cSpecial: CapeSpecial.Lament
                );

                break;


            default:
                C.Logger($"[UltraEnhancements] No preset for '{className}', using SmartEnhance.");
                Adv.SmartEnhance(className);
                break;
        }
    }


    public void ApplyGramiel()
    {
        string className = Bot.Player?.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        C.Logger($"[UltraEnhancements] Dage enhancing for: {className} using fixed Lucky preset");

        switch (className)
        {
            case "ArchPaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Awe_Blast,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "Lord Of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Health_Vamp
                );
                break;

            case "StoneCrusher":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: Adv.uValiance() ? WeaponSpecial.Valiance : WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "ArchFiend":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    wSpecial: WeaponSpecial.Health_Vamp
                );
                break;

            case "Shaman":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    wSpecial: WeaponSpecial.Mana_Vamp
                );
                break;

            case "Verus DoomKnight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            default:
                C.Logger($"[UltraEnhancements] No preset for '{className}', using SmartEnhance.");
                Adv.SmartEnhance(className);
                break;
        }
    }

    public void ApplyNulgath()
    {
        string className = Bot.Player?.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        C.Logger($"[UltraEnhancements] Nulgath enhancing for: {className}");

        switch (className)
        {
            case "ArchPaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Lacerate,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "Lord Of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: Adv.uArcanasConcerto() ? WeaponSpecial.Arcanas_Concerto : WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "StoneCrusher":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "Dragon of Time":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    wSpecial: Adv.uArcanasConcerto() ? WeaponSpecial.Elysium : WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "Void Highlord":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "Scion of Flames":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    wSpecial: Adv.uArcanasConcerto() ? WeaponSpecial.Arcanas_Concerto : WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Lament
                );
                break;


            case "Arachnomancer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Health_Vamp
                );
                break;

            case "Verus DoomKnight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Lacerate,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "King's Echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: Adv.uValiance() ? WeaponSpecial.Valiance : WeaponSpecial.Spiral_Carve,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            default:
                C.Logger($"[UltraEnhancements] No preset for '{className}', using SmartEnhance.");
                Adv.SmartEnhance(className);
                break;
        }
    }

    public void ApplyAstralEmpyrean()
    {
        string className = Bot.Player?.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        C.Logger($"[UltraEnhancements] AstralEmpyrean enhancing for: {className}");

        switch (className)
        {
            case "King's Echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Healer,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Elysium
                );
                break;

            case "ArchPaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Lacerate,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "Lord Of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: Adv.uArcanasConcerto() ? WeaponSpecial.Arcanas_Concerto : WeaponSpecial.Awe_Blast,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "StoneCrusher":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Absolution
                );
                break;


            case "Verus DoomKnight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Praxis,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "Scion of Flames":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Elysium,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            default:
                C.Logger($"[UltraEnhancements] No preset for '{className}', using SmartEnhance.");
                Adv.SmartEnhance(className);
                break;
        }
    }

    public void ApplyKolr()
    {
        string className = Bot.Player?.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        C.Logger($"[UltraEnhancements] Kolr enhancing for: {className}");

        switch (className)
        {
            case "Verus DoomKnight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "Void Highlord":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Health_Vamp
                );
                break;

            case "Lord Of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Health_Vamp,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            case "ArchPaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Fighter,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: Adv.uPraxis() ? WeaponSpecial.Praxis : WeaponSpecial.Lacerate,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "King's Echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: Adv.uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Spiral_Carve,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            default:
                C.Logger($"[UltraEnhancements] No preset for '{className}', using SmartEnhance.");
                Adv.SmartEnhance(className);
                break;
        }
    }
}

