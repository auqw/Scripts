/*
name: UltraEzrajal
description: Ultra Ezrajal helper handling Counter Attack windows with army sync.
tags: Ultra
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs

using System.ComponentModel;
using System.Reflection;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

// Classes and Enhancements (safe-mode):
// ==============================
//  Arcana Invoker ( Helm: Healer , Class: Lucky, Weapon: Elysium, Cape: Absolution)
//  Legion Revenant ( Helm: Wizard, Class: Wizard, Weapon: Ravenous/Valiance/Arcana Concerto, Cape: Vainglory)
//  ArchPaladin ( Helm: Forge, Class: Lucky, Weapon: Valiance, Cape: Lament)
//  Lord of Order ( Helm: Forge, Class: Lucky, Weapon: Lucky AweBlast/Valiance, Cape: Absolution)
// ==============================

public class UltraEzrajal
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

    public string OptionsStorage = "UltraEzrajal";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<RoleSelection>(
            "Class Selection",
            "Choose Your Role",
            "Select the class you want to use for Ultra Ezrajal run (only one of each role is needed in the army).\n\n"
                + " Arcana Invoker: Healer Helm, Lucky Class, Elysium Weapon, Absolution Cape\n"
                + " Legion Revenant: Wizard Helm, Wizard Class, Ravenous/Valiance/Arc Concerto Weapon, Vainglory Cape\n"
                + " ArchPaladin: Forge Helm, Lucky Class, Valiance Weapon, Lament Cape\n"
                + " Lord of Order: Forge Helm, Lucky Class, Lucky AweBlast/Valiance Weapon, Absolution Cape",
            RoleSelection.ArcanaInvoker
        ),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.Boot();
        Adv.GearStore();
        Enhancements(Bot.Config!.Get<RoleSelection>("Class Selection"));
        Fight();
        Adv.GearStore(true);
        Bot.Stop();
    }

    void Fight()
    {
        const string map = "ultraezrajal";
        const string boss = "Ultra Ezrajal";

        // Apply enhancements based on selected role.
        Enhancements(Bot.Config!.Get<RoleSelection>("Class Selection"));

        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");

        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_ezrajal.sync");
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        C.EnsureAccept(8152);

        while (!Bot.ShouldExit && !Bot.TempInv.Contains("Ultra Ezrajal Defeated"))
        {
            if (!Bot.Player.Alive)
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

            // If Counter Attack aura is up, stop hitting for a moment
            if (Bot.Player.HasTarget && Bot.Target.Auras.Any(a => a.Name == "Counter Attack"))
            {
                Bot.Combat.CancelAutoAttack();
                Bot.Sleep(6300);
            }

            Bot.Combat.Attack(boss);
            Bot.Sleep(200);
        }

        C.EnsureComplete(8152);
    }

    // void Enhancements()
    // {

    //     //  Arcana Invoker ( Helm: Healer , Class: Lucky, Weapon: Elysium, Cape: Absolution)
    //     if (Bot.Player?.CurrentClass?.Name == "Arcana Invoker")
    //     {
    //         // Enhance Weapon with Elysium
    //         Adv.EnhanceItem(
    //             Bot.Inventory.Items.FirstOrDefault(x =>
    //                 x != null && x.Equipped && Adv.WeaponCatagories.ToList().Contains(x.Category)
    //             )?.Name
    //             ?? "",
    //             EnhancementType.Lucky,
    //             wSpecial: WeaponSpecial.Elysium
    //         );
    //         // Enhance Class with Lucky
    //         Adv.EnhanceItem(Bot.Player.CurrentClass?.Name ?? "", EnhancementType.Lucky);
    //         // Enhance Helm with Healer
    //         Adv.EnhanceItem(
    //             Bot.Inventory.Items.FirstOrDefault(x =>
    //                 x != null
    //                 && x.Equipped
    //                 && x.Category == Skua.Core.Models.Items.ItemCategory.Helm
    //             )?.Name
    //             ?? "",
    //             EnhancementType.Healer
    //         );
    //         // Enhance Cape with Absolution
    //         Adv.EnhanceItem(
    //             Bot.Inventory.Items.FirstOrDefault(x =>
    //                 x != null
    //                 && x.Equipped
    //                 && x.Category == Skua.Core.Models.Items.ItemCategory.Cape
    //             )?.Name
    //             ?? "",
    //             EnhancementType.Lucky,
    //             CapeSpecial.Absolution
    //         );
    //     }
    //     //  Legion Revenant ( Helm: Wizard, Class: Wizard, Weapon: Ravenous/Valiance, Cape: Vainglory)
    //     else if (Bot.Player?.CurrentClass?.Name == "Legion Revenant")
    //     {
    //         // Enhance Weapon with Ravenous/Valiance/Arcana Concerto
    //         Adv.EnhanceItem(
    //             Bot.Inventory.Items.FirstOrDefault(x =>
    //                 x != null && x.Equipped && Adv.WeaponCatagories.ToList().Contains(x.Category)
    //             )?.Name
    //             ?? "",
    //             EnhancementType.Wizard,
    //             wSpecial: Adv.uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Valiance
    //         );
    //         // Enhance Class with Wizard
    //         Adv.EnhanceItem(Bot.Player.CurrentClass?.Name ?? "", EnhancementType.Wizard);
    //         // Enhance Helm with Wizard
    //         Adv.EnhanceItem(
    //             Bot.Inventory.Items.FirstOrDefault(x =>
    //                 x != null
    //                 && x.Equipped
    //                 && x.Category == Skua.Core.Models.Items.ItemCategory.Helm
    //             )?.Name
    //             ?? "",
    //             EnhancementType.Wizard
    //         );
    //         // Enhance Cape with Vainglory
    //         Adv.EnhanceItem(
    //             Bot.Inventory.Items.FirstOrDefault(x =>
    //                 x != null
    //                 && x.Equipped
    //                 && x.Category == Skua.Core.Models.Items.ItemCategory.Cape
    //             )?.Name
    //             ?? "",
    //             EnhancementType.Wizard,
    //             CapeSpecial.Vainglory
    //         );
    //     }
    //     //  ArchPaladin ( Helm: Forge, Class: Lucky, Weapon: Valiance, Cape: Lament)
    //     else if (Bot.Player?.CurrentClass?.Name == "ArchPaladin")
    //     {
    //         // Enhance Weapon with Valiance
    //         Adv.EnhanceItem(
    //             Bot.Inventory.Items.FirstOrDefault(x =>
    //                 x != null && x.Equipped && Adv.WeaponCatagories.ToList().Contains(x.Category)
    //             )?.Name
    //             ?? "",
    //             EnhancementType.Lucky,
    //             wSpecial: WeaponSpecial.Valiance
    //         );
    //         // Enhance Class with Lucky
    //         Adv.EnhanceItem(Bot.Player.CurrentClass?.Name ?? "", EnhancementType.Lucky);
    //         // Enhance Helm with Forge
    //         Adv.EnhanceItem(
    //             Bot.Inventory.Items.FirstOrDefault(x =>
    //                 x != null
    //                 && x.Equipped
    //                 && x.Category == Skua.Core.Models.Items.ItemCategory.Helm
    //             )?.Name
    //             ?? "",
    //             EnhancementType.Lucky,
    //             hSpecial: HelmSpecial.Forge
    //         );
    //         // Enhance Cape with Lament
    //         Adv.EnhanceItem(
    //             Bot.Inventory.Items.FirstOrDefault(x =>
    //                 x != null
    //                 && x.Equipped
    //                 && x.Category == Skua.Core.Models.Items.ItemCategory.Cape
    //             )?.Name
    //             ?? "",
    //             EnhancementType.Lucky,
    //             CapeSpecial.Lament
    //         );
    //     }
    //     //  Lord of Order ( Helm: Forge, Class: Lucky, Weapon: Lucky AweBlast/Valiance, Cape: Absolution)
    //     else if (Bot.Player?.CurrentClass?.Name == "Lord of Order")
    //     {
    //         // Enhance Weapon with Lucky AweBlast/Valiance
    //         Adv.EnhanceItem(
    //             Bot.Inventory.Items.FirstOrDefault(x =>
    //                 x != null && x.Equipped && Adv.WeaponCatagories.ToList().Contains(x.Category)
    //             )?.Name
    //             ?? "",
    //             EnhancementType.Lucky,
    //             wSpecial: WeaponSpecial.Awe_Blast
    //         );
    //         // Enhance Class with Lucky
    //         Adv.EnhanceItem(Bot.Player.CurrentClass?.Name ?? "", EnhancementType.Lucky);
    //         // Enhance Helm with Forge
    //         Adv.EnhanceItem(
    //             Bot.Inventory.Items.FirstOrDefault(x =>
    //                 x != null
    //                 && x.Equipped
    //                 && x.Category == Skua.Core.Models.Items.ItemCategory.Helm
    //             )?.Name
    //             ?? "",
    //             EnhancementType.Lucky,
    //             hSpecial: HelmSpecial.Forge
    //         );
    //         // Enhance Cape with Absolution
    //         Adv.EnhanceItem(
    //             Bot.Inventory.Items.FirstOrDefault(x =>
    //                 x != null
    //                 && x.Equipped
    //                 && x.Category == Skua.Core.Models.Items.ItemCategory.Cape
    //             )?.Name
    //             ?? "",
    //             EnhancementType.Lucky,
    //             CapeSpecial.Absolution
    //         );
    //     }
    // }
    void Enhancements(RoleSelection role)
    {
        // Determine the required class name for this role.
        string? requiredClass = role switch
        {
            RoleSelection.ArcanaInvoker => "Arcana Invoker",
            RoleSelection.LegionRevenant => "Legion Revenant",
            RoleSelection.ArchPaladin => "ArchPaladin",
            RoleSelection.LordOfOrder => "Lord of Order",
            _ => null,
        };

        // If the user does not own the required class → stop everything.
        if (!C.CheckInventory(requiredClass))
        {
            C.Logger(
                $"[ERROR] You do not own the required class: {requiredClass}. Stopping.",
                stopBot: true
            );
        }
        else
        {
            // Equip the required class.
            C.Equip(requiredClass!);
        }

        // Cache equipped items safely once.
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
        string className = Bot.Player?.CurrentClass?.Name ?? "";

        // Always log what we found for debugging readability.
        C.Logger(
            $"Class: {className}\n" + $"Weapon: {weapon}\n" + $"Helm: {helm}\n" + $"Cape: {cape}",
            "info"
        );

        switch (role)
        {
            case RoleSelection.ArcanaInvoker:
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Elysium);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(helm, EnhancementType.Healer);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Absolution);
                break;

            case RoleSelection.LegionRevenant:
                Adv.EnhanceItem(
                    weapon,
                    EnhancementType.Wizard,
                    wSpecial: Adv.uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Valiance
                );
                Adv.EnhanceItem(className, EnhancementType.Wizard);
                Adv.EnhanceItem(helm, EnhancementType.Wizard);
                Adv.EnhanceItem(cape, EnhancementType.Wizard, CapeSpecial.Vainglory);
                break;

            case RoleSelection.ArchPaladin:
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Valiance);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Forge);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Lament);
                break;

            case RoleSelection.LordOfOrder:
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Awe_Blast);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Forge);
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
        [Description("Arcana Invoker")]
        ArcanaInvoker,

        [Description("Legion Revenant")]
        LegionRevenant,

        [Description("ArchPaladin")]
        ArchPaladin,

        [Description("Lord Of Order")]
        LordOfOrder,
    }
}
