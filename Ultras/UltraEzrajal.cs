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
#nullable enable
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

    public string OptionsStorage = "UltraEzrajal2";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<string>(
            "ArcanaInvokerPlayer",
            "Arcana Invoker Player",
            "Player name assigned to Arcana Invoker role."
        ),
        new Option<string>(
            "LegionRevenantPlayer",
            "Legion Revenant Player",
            "Player name assigned to Legion Revenant role."
        ),
        new Option<string>(
            "ArchPaladinPlayer",
            "ArchPaladin Player",
            "Player name assigned to ArchPaladin role."
        ),
        new Option<string>(
            "LordOfOrderPlayer",
            "Lord Of Order Player",
            "Player name assigned to Lord Of Order role."
        ),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Bot.Config!.Configure();
        Core.Boot();
        Bot.UltraBossHelper.EnableCounterAttack();
        Adv.GearStore();
        Fight();
        Adv.GearStore(true);
        Bot.Stop();
    }

    void Fight()
    {
        const string map = "ultraezrajal";
        const string boss = "Ultra Ezrajal";
        const string syncFile = "ArmyEzrajalItemCheck.sync";
        string syncPath = Ultra.ResolveSyncPath("ArmyEzrajalItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);

        // ---------------------------
        // ENHANCEMENTS
        // ---------------------------
        Enhancements(); // new version with auto-role detection

        // ---------------------------
        // POTIONS
        // ---------------------------
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");

        // ---------------------------
        // MAP SETUP
        // ---------------------------
        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_ezrajal.sync");
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        C.EnsureAccept(8152);

        // ---------------------------
        // MAIN COMBAT LOOP
        // ---------------------------
        while (!Bot.ShouldExit)
        {
            // Check if the whole army has finished
            if (Ultra.CheckArmyProgress("Ultra Ezrajal Defeated", 1, true, syncFile))
            {
                C.Logger("All players finished farm.");
                C.EnsureComplete(8152);
                break;
            }
            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            // ---------------------------
            // COUNTER ATTACK HANDLER
            // ---------------------------
            if (
                Bot.Player.HasTarget
                && Bot.Target?.Auras?.Any(a => a != null && a?.Name == "Counter Attack") == true
            )
            {
                Bot.Combat.CancelAutoAttack();

                Bot.Sleep(6300);
            }
            else
            {
                Bot.Combat.Attack(boss);
            }

            Bot.Sleep(180); // slightly lower, smoother attacks
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

    string GetRoleForPlayer(string playerName)
    {
        string ai = Bot.Config?.Get<string>("ArcanaInvokerPlayer") ?? "";
        if (playerName.Equals(ai, StringComparison.OrdinalIgnoreCase))
            return "ArcanaInvoker";

        string lr = Bot.Config?.Get<string>("LegionRevenantPlayer") ?? "";
        if (playerName.Equals(lr, StringComparison.OrdinalIgnoreCase))
            return "LegionRevenant";

        string ap = Bot.Config?.Get<string>("ArchPaladinPlayer") ?? "";
        if (playerName.Equals(ap, StringComparison.OrdinalIgnoreCase))
            return "ArchPaladin";

        string loo = Bot.Config?.Get<string>("LordOfOrderPlayer") ?? "";
        if (playerName.Equals(loo, StringComparison.OrdinalIgnoreCase))
            return "LordOfOrder";

        return "";
    }

    void Enhancements()
    {
        string? playerName = Bot.Player?.Username;
        if (string.IsNullOrWhiteSpace(playerName))
        {
            C.Logger("[ERROR] Unable to determine player name.", stopBot: true);
            return;
        }

        string role = GetRoleForPlayer(playerName);
        if (string.IsNullOrWhiteSpace(role))
        {
            C.Logger($"[ERROR] No role assigned for player '{playerName}'", stopBot: true);
            return;
        }

        // Required class based on role
        string requiredClass = role switch
        {
            "ArcanaInvoker" => "Arcana Invoker",
            "LegionRevenant" => "Legion Revenant",
            "ArchPaladin" => "ArchPaladin",
            "LordOfOrder" => "Lord of Order",
            _ => "",
        };

        if (!C.CheckInventory(requiredClass))
        {
            C.Logger(
                $"[ERROR] You do not own the class required for your role: {requiredClass}",
                stopBot: true
            );
            return;
        }

        // Equip correct class
        C.Equip(requiredClass);

        // Cache currently equipped items
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

        C.Logger(
            $"[Enhancement]\nClass: {className}\nWeapon: {weapon}\nHelm: {helm}\nCape: {cape}",
            "info"
        );

        // Apply enhancement rules per role
        switch (role)
        {
            case "ArcanaInvoker":
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Elysium);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(helm, EnhancementType.Healer);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Absolution);
                break;

            case "LegionRevenant":
                Adv.EnhanceItem(
                    weapon,
                    EnhancementType.Wizard,
                    wSpecial: Adv.uRavenous() ? WeaponSpecial.Ravenous : WeaponSpecial.Valiance
                );
                Adv.EnhanceItem(className, EnhancementType.Wizard);
                Adv.EnhanceItem(helm, EnhancementType.Wizard);
                Adv.EnhanceItem(cape, EnhancementType.Wizard, CapeSpecial.Vainglory);
                break;

            case "ArchPaladin":
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Valiance);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Forge);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Lament);
                break;

            case "LordOfOrder":
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Awe_Blast);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Forge);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Absolution);
                break;
        }
    }
}
