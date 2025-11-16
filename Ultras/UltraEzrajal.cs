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

using Skua.Core.Interfaces;
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

    public void ScriptMain(IScriptInterface bot)
    {
        Core.Boot();
        Adv.GearStore();
        Enhancements();
        Fight();
        Adv.GearStore(true);
        Bot.Stop();
    }

    void Fight()
    {
        const string map = "ultraezrajal";
        const string boss = "Ultra Ezrajal";

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

    void Enhancements()
    {
        //  Arcana Invoker ( Helm: Healer , Class: Lucky, Weapon: Elysium, Cape: Absolution)
        if (Bot.Player?.CurrentClass?.Name == "Arcana Invoker")
        {
            // Enhance Weapon with Elysium
            Adv.EnhanceItem(
                Bot.Inventory.Items.FirstOrDefault(x =>
                    x != null && x.Equipped && Adv.WeaponCatagories.ToList().Contains(x.Category)
                )?.Name
                ?? "",
                EnhancementType.Lucky,
                wSpecial: WeaponSpecial.Elysium
            );
            // Enhance Class with Lucky
            Adv.EnhanceItem(Bot.Player.CurrentClass?.Name ?? "", EnhancementType.Lucky);
            // Enhance Helm with Healer
            Adv.EnhanceItem(
                Bot.Inventory.Items.FirstOrDefault(x =>
                    x != null
                    && x.Equipped
                    && x.Category == Skua.Core.Models.Items.ItemCategory.Helm
                )?.Name
                ?? "",
                EnhancementType.Healer
            );
            // Enhance Cape with Absolution
            Adv.EnhanceItem(
                Bot.Inventory.Items.FirstOrDefault(x =>
                    x != null
                    && x.Equipped
                    && x.Category == Skua.Core.Models.Items.ItemCategory.Cape
                )?.Name
                ?? "",
                EnhancementType.Lucky,
                CapeSpecial.Absolution
            );
        }
        //  Legion Revenant ( Helm: Wizard, Class: Wizard, Weapon: Ravenous/Valiance, Cape: Vainglory)
        else if (Bot.Player?.CurrentClass?.Name == "Legion Revenant")
        {
            // Enhance Weapon with Ravenous/Valiance/Arcana Concerto
            Adv.EnhanceItem(
                Bot.Inventory.Items.FirstOrDefault(x =>
                    x != null && x.Equipped && Adv.WeaponCatagories.ToList().Contains(x.Category)
                )?.Name
                ?? "",
                EnhancementType.Wizard,
                wSpecial: Adv.uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Valiance
            );
            // Enhance Class with Wizard
            Adv.EnhanceItem(Bot.Player.CurrentClass?.Name ?? "", EnhancementType.Wizard);
            // Enhance Helm with Wizard
            Adv.EnhanceItem(
                Bot.Inventory.Items.FirstOrDefault(x =>
                    x != null
                    && x.Equipped
                    && x.Category == Skua.Core.Models.Items.ItemCategory.Helm
                )?.Name
                ?? "",
                EnhancementType.Wizard
            );
            // Enhance Cape with Vainglory
            Adv.EnhanceItem(
                Bot.Inventory.Items.FirstOrDefault(x =>
                    x != null
                    && x.Equipped
                    && x.Category == Skua.Core.Models.Items.ItemCategory.Cape
                )?.Name
                ?? "",
                EnhancementType.Wizard,
                CapeSpecial.Vainglory
            );
        }
        //  ArchPaladin ( Helm: Forge, Class: Lucky, Weapon: Valiance, Cape: Lament)
        else if (Bot.Player?.CurrentClass?.Name == "ArchPaladin")
        {
            // Enhance Weapon with Valiance
            Adv.EnhanceItem(
                Bot.Inventory.Items.FirstOrDefault(x =>
                    x != null && x.Equipped && Adv.WeaponCatagories.ToList().Contains(x.Category)
                )?.Name
                ?? "",
                EnhancementType.Lucky,
                wSpecial: WeaponSpecial.Valiance
            );
            // Enhance Class with Lucky
            Adv.EnhanceItem(Bot.Player.CurrentClass?.Name ?? "", EnhancementType.Lucky);
            // Enhance Helm with Forge
            Adv.EnhanceItem(
                Bot.Inventory.Items.FirstOrDefault(x =>
                    x != null
                    && x.Equipped
                    && x.Category == Skua.Core.Models.Items.ItemCategory.Helm
                )?.Name
                ?? "",
                EnhancementType.Lucky,
                hSpecial: HelmSpecial.Forge
            );
            // Enhance Cape with Lament
            Adv.EnhanceItem(
                Bot.Inventory.Items.FirstOrDefault(x =>
                    x != null
                    && x.Equipped
                    && x.Category == Skua.Core.Models.Items.ItemCategory.Cape
                )?.Name
                ?? "",
                EnhancementType.Lucky,
                CapeSpecial.Lament
            );
        }
        //  Lord of Order ( Helm: Forge, Class: Lucky, Weapon: Lucky AweBlast/Valiance, Cape: Absolution)
        else if (Bot.Player?.CurrentClass?.Name == "Lord of Order")
        {
            // Enhance Weapon with Lucky AweBlast/Valiance
            Adv.EnhanceItem(
                Bot.Inventory.Items.FirstOrDefault(x =>
                    x != null && x.Equipped && Adv.WeaponCatagories.ToList().Contains(x.Category)
                )?.Name
                ?? "",
                EnhancementType.Lucky,
                wSpecial: WeaponSpecial.Awe_Blast
            );
            // Enhance Class with Lucky
            Adv.EnhanceItem(Bot.Player.CurrentClass?.Name ?? "", EnhancementType.Lucky);
            // Enhance Helm with Forge
            Adv.EnhanceItem(
                Bot.Inventory.Items.FirstOrDefault(x =>
                    x != null
                    && x.Equipped
                    && x.Category == Skua.Core.Models.Items.ItemCategory.Helm
                )?.Name
                ?? "",
                EnhancementType.Lucky,
                hSpecial: HelmSpecial.Forge
            );
            // Enhance Cape with Absolution
            Adv.EnhanceItem(
                Bot.Inventory.Items.FirstOrDefault(x =>
                    x != null
                    && x.Equipped
                    && x.Category == Skua.Core.Models.Items.ItemCategory.Cape
                )?.Name
                ?? "",
                EnhancementType.Lucky,
                CapeSpecial.Absolution
            );
        }
    }
}
