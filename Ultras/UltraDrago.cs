/*
name: UltraDrago
description: Ultra King Drago helper with taunter classes and priority adds.
tags: Ultra
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs

using System.ComponentModel;
using System.Reflection;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
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

    // User options
    public List<IOption> Options = new()
    {
        new Option<RoleSelection>(
            "role",
            "Role Selection",
            "Select your role for Ultra King Drago fight. Make sure to equip the corresponding class before running the script.",
            RoleSelection.ChaosAvenger
        ),
        CoreBots.Instance.SkipOptions,
    };

    // Filled at runtime
    private RoleSelection role;

    public void ScriptMain(IScriptInterface bot)
    {
        C.OneTimeMessage(
            "WARNING",
            "Please use the classes in the options to ensure proper role functionality.\n"
                + "We've allowed you to choose 'Current Class', but it's recommended to select a specific role for optimal (safe) performance.\n"
                + "Curent CLass\" Will Focus Boss -> Left Summon -> Right Summon",
            true,
            true
        );

        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();

        // Determine role from user selection or equipped class
        role = Bot.Config?.Get<RoleSelection>("role") ?? GetEquippedRole();

        C.Join("whitemap");

        Astravia.AstraviaJudgement();

        // All conditions satisfied → START script
        Core.Boot();
        C.AddDrop("King Drago Insignia");
        Adv.GearStore();
        Bot.Quests.UpdateQuest(8395);
        Prep();
        C.EnsureAccept(8397);
        Fight();
        C.EnsureComplete(8397);
        Bot.Wait.ForPickup("King Drago Insignia");
        C.JumpWait();
        Adv.GearStore(true);
    }

    RoleSelection GetEquippedRole()
    {
        string currentClass = Bot.Player?.CurrentClass?.Name ?? "";

        foreach (var roleEnum in Enum.GetValues(typeof(RoleSelection)).Cast<RoleSelection>())
        {
            string roleDesc = GetDescription(roleEnum);

            // Match CurrentClass role
            if (roleEnum == RoleSelection.CurrentClass)
                continue;

            if (roleDesc == currentClass)
            {
                return roleEnum;
            }
        }

        C.Logger("Setup", "No valid role class equipped.");
        Bot.Stop();
        return RoleSelection.CurrentClass; // Fallback
    }

    bool IsTaunter() => role == RoleSelection.ArchPaladin || role == RoleSelection.LordOfOrder;

    bool IsCurrentClass() => role == RoleSelection.CurrentClass;

    void Prep()
    {
        // CurrentClass and non-taunters don't need special prep
        if (IsCurrentClass())
        {
            return;
        }

        C.Equip(GetDescription(role));

        // Taunters (ArchPaladin & LordOfOrder) prep with Scroll of Enrage
        if (IsTaunter())
        {
            Ultra.GetScrollOfEnrage();
            return;
        }

        // Non-taunters prep with alchemy boosts
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");
    }

    void Fight()
    {
        const string map = "ultradrago";
        const string boss = "King Drago";
        const string leftSummon = "Bowmaster Algie"; // Right summon (Bow)
        const string rightSummon = "Executioner Dene"; // Left summon (Axe)

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);

        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_drago.sync");
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        // ===== MAIN LOOP =====
        while (!Bot.ShouldExit)
        {
            // ======================================================
            //              ARCHPALADIN TAUNTER LOGIC
            // ======================================================
            if (Ultra.CheckArmyProgress("Drago Dethroned", 1, true, syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                C.EnsureComplete(8547);
                break;
            }
            if (role == RoleSelection.ArchPaladin)
            {
                // ArchPaladin taunts the left summon (Axe)
                while (!Bot.ShouldExit && Ultra.MonsterAlive(rightSummon))
                {
                    Ultra.Taunt(
                        GetDescription(RoleSelection.ArchPaladin),
                        rightSummon,
                        "aura",
                        250,
                        "Focus"
                    );
                }
                continue;
            }

            // ======================================================
            //              LORD OF ORDER TAUNTER LOGIC
            // ======================================================

            if (role == RoleSelection.LordOfOrder)
            {
                // LordOfOrder loops taunt with ArchPaladin (left summon)
                while (!Bot.ShouldExit && Ultra.MonsterAlive(rightSummon))
                {
                    Ultra.Taunt(
                        GetDescription(RoleSelection.LordOfOrder),
                        rightSummon,
                        "aura",
                        700,
                        "Focus"
                    );
                }
                continue;
            }

            // ======================================================
            //              NON-TAUNTER BEHAVIOR AREA (LR / DPS / CurrentClass)
            // ======================================================

            // LegionRevenant and other DPS focus on right summon (Bow)
            Core.KillWithPriority(boss, leftSummon, rightSummon);
            Bot.Skills.UseSkill(5);
        }
    }

#nullable enable

    void Enhancements(RoleSelection selectedRole)
    {
        // CurrentClass doesn't need enhancements
        if (selectedRole == RoleSelection.CurrentClass)
        {
            return;
        }

        // Resolve class name via enum description.
        string className = GetDescription(selectedRole);

        // Ensure user owns the class.
        if (!C.CheckInventory(className))
        {
            C.Logger($"[ERROR] Missing required class: {className}", stopBot: true);
            return;
        }

        // Equip class before scanning gear.
        C.Equip(className);

        // Cache equipped gear once (avoids allocations & repeated scans).
        InventoryItem? weaponItem = Bot.Inventory?.Items?.FirstOrDefault(x =>
            x?.Equipped == true && Adv.WeaponCatagories.Contains(x.Category)
        );

        InventoryItem? helmItem = Bot.Inventory?.Items?.FirstOrDefault(x =>
            x?.Equipped == true && x.Category == ItemCategory.Helm
        );

        InventoryItem? capeItem = Bot.Inventory?.Items?.FirstOrDefault(x =>
            x?.Equipped == true && x.Category == ItemCategory.Cape
        );

        string weapon = weaponItem?.Name ?? "";
        string helm = helmItem?.Name ?? "";
        string cape = capeItem?.Name ?? "";

        // Debug snapshot
        C.Logger(
            $"[Enhancements]\n"
                + $"  Class: {className}\n"
                + $"  Weapon: {weapon}\n"
                + $"  Helm:   {helm}\n"
                + $"  Cape:   {cape}",
            "info"
        );

        // ===============================
        //  Enhancement Table
        // ===============================
        switch (selectedRole)
        {
            // -----------------------------------------------------------
            // Chaos Avenger: anima, lucky, valiance, vainglory
            // -----------------------------------------------------------
            case RoleSelection.ChaosAvenger:
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Anima);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Valiance);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Vainglory);
                break;

            // -----------------------------------------------------------
            // Legion Revenant: pneuma, wizard, ravenous/valiance, vainglory
            //
            // uRavenous() determines priority:
            // if (uRavenous()) → Ravenous
            // else → Valiance
            // -----------------------------------------------------------
            case RoleSelection.LegionRevenant:
                Adv.EnhanceItem(helm, EnhancementType.Wizard, hSpecial: HelmSpecial.Pneuma);
                Adv.EnhanceItem(className, EnhancementType.Wizard);
                Adv.EnhanceItem(
                    weapon,
                    EnhancementType.Wizard,
                    wSpecial: Adv.uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Valiance
                );
                Adv.EnhanceItem(cape, EnhancementType.Wizard, CapeSpecial.Vainglory);
                break;

            // -----------------------------------------------------------
            // ArchPaladin: forge, lucky, valiance, lament
            // -----------------------------------------------------------
            case RoleSelection.ArchPaladin:
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Forge);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Valiance);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Lament);
                break;

            // -----------------------------------------------------------
            // Lord of Order: forge, lucky, valiance / AweBlast, absolution
            //
            // Uses same dual-weapon logic as LR:
            // if (uRavenous()) → Awe_Blast
            // else → Valiance
            // -----------------------------------------------------------
            case RoleSelection.LordOfOrder:
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Forge);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(
                    weapon,
                    EnhancementType.Lucky,
                    wSpecial: Adv.uRavenous() ? WeaponSpecial.Awe_Blast : WeaponSpecial.Valiance
                );
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Absolution);
                break;
        }
    }

    public static string GetDescription(Enum value)
    {
        FieldInfo? field = value.GetType().GetField(value.ToString());
        DescriptionAttribute? attribute =
            field?.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault()
            as DescriptionAttribute;

        return attribute?.Description ?? value.ToString();
    }

    public enum RoleSelection
    {
        [Description("Current Class")]
        CurrentClass,

        [Description("Chaos Avenger")]
        ChaosAvenger,

        [Description("Legion Revenant")]
        LegionRevenant,

        [Description("ArchPaladin")]
        ArchPaladin,

        [Description("Lord of Order")]
        LordOfOrder,
    }
}
