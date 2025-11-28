/*
name: UltraDage
description: Two-taunter strategy for Ultra Dage with aura-based taunting and army synchronization.
tags: Ultra
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs

/*
Required classes:
==================
    Chaos Avenger
    ArchPaladin
==================
DPS classes:
==================
    Legion Revenant
    Chrono ShadowSlayer
    Lich
    Archfiend
    Quantum Chronomancer
    Hollowborn Vindicator
    Arachnomancer
    Infinity Knight
    Verus Doomknight
    King's Echo
    Phantom Chronomancer
    Great Thief
==================
*/

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class UltraDage
{
    private CoreBots C => CoreBots.Instance;
    public IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();
    string? a,
        b;
    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraDage";
    public List<IOption> Options = new()
    {
        new Option<string>(
            "a",
            "First Taunter Class",
            "Insert the name of the class that will taunt ( examples: AP, Cav, LR, KE(?))",
            ""
        ),
        new Option<string>(
            "b",
            "Second Taunter Class",
            "Insert the name of the class that will taunt ( examples: AP, Cav, LR, KE(?))",
            ""
        ),
    };

    private string NormalizeString(string input) => (input ?? "").Trim().ToLower();

    public void ScriptMain(IScriptInterface bot)
    {
        if (!Bot.Quests.IsAvailable(8547))
            C.Logger(
                @"Quest not complete: ""Power of the Undead Legion"", go run ""Story\Legion\DageChallengeStory.cs"" first",
                messageBox: true,
                stopBot: true
            );

        C.Join("whitemap");
        Bot.Config!.Configure();
        a = NormalizeString(Bot.Config!.Get<string>("a")!);
        b = NormalizeString(Bot.Config.Get<string>("b")!);
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
        {
            Core.Log("Setup", "Fill both taunter classes in Script Options.");
            Bot.Stop();
            return;
        }
        Core.Boot();
        Adv.GearStore();
        Prep();
        Fight();
        Bot.Events.ExtensionPacketReceived -= UltraDageListener;
        Adv.GearStore(true);
        Bot.Stop();
    }

    bool IsTaunter() => Core.HasClassEquipped(a!) || Core.HasClassEquipped(b!);

    void Prep()
    {
        Bot.Events.ExtensionPacketReceived += UltraDageListener;
        Bot.Quests.UpdateQuest(793);
        Core.ChooseBestEnhancement("Weapon", "Health Vamp");
        if (IsTaunter())
            Ultra.GetScrollOfEnrage();
        else
        {
            Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
            Ultra.BuyAlchemyPotion("Potent Honor Potion");
            Core.EquipConsumable("Potent Honor Potion");
        }
        Enhancements();
    }

    void Fight()
    {
        const string map = "ultradage";
        const string boss = "Dage the Dark Lord";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        C.AddDrop("Dage the Evil Insignia");
        C.EnsureAccept(8547);

        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_dage.sync");
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        while (!Bot.ShouldExit)
        {
            if (Ultra.CheckArmyProgress("Dage the Dark Lord Defeated", 1, true, syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                C.EnsureComplete(8547);
                break;
            }
            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Core.HasClassEquipped(a!))
                Ultra.Taunt(a!, boss, "aura", 250, "Focus");
            else if (Core.HasClassEquipped(b!))
                Ultra.Taunt(b!, boss, "aura", 700, "Focus");
            else
            {
                Core.Kill(boss);
            }
        }
    }

    public async void UltraDageListener(dynamic packet)
    {
        if (packet?["params"]?.type?.ToString() != "json")
            return;
        if (!Bot.Player.Alive)
            return;
        dynamic data = packet["params"].dataObj;
        if (data?.cmd?.ToString() != "event")
            return;

        if (
            !string.IsNullOrEmpty(data?.args?.zoneSet?.ToString())
            && string.Equals(
                data?.args?.zoneSet?.ToString(),
                "A",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            await Task.Run(() => Bot.Player.WalkTo(122, 420));
            return;
        }
        if (
            !string.IsNullOrEmpty(data?.args?.zoneSet?.ToString())
            && string.Equals(
                data?.args?.zoneSet?.ToString(),
                "B",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            await Task.Run(() => Bot.Player.WalkTo(856, 420));
            return;
        }
    }

    void Enhancements()
    {
        string? playerName = Bot.Player?.Username;
        if (string.IsNullOrWhiteSpace(playerName))
        {
            C.Logger("[ERROR] Unable to determine player name.", stopBot: true);
            return;
        }
        var acceptableDPSClasses = new HashSet<string>
        {
            "Legion Revenant",
            "Chrono ShadowSlayer",
            "Lich",
            "Archfiend",
            "Quantum Chronomancer",
            "Hollowborn Vindicator",
            "Arachnomancer",
            "Infinity Knight",
            "Verus Doomknight",
            "King's Echo",
            "Phantom Chronomancer",
            "Great Thief",
        };

        var taunterclasses = new HashSet<string> { "Chaos Avenger", "ArchPaladin" };

        // Only equip if current class is NOT in the list
        string? currentClass = Bot.Player!.CurrentClass?.Name;
        if (
            !IsTaunter()
            && !string.IsNullOrEmpty(currentClass)
            && !acceptableDPSClasses.Contains(currentClass)
        )
        {
            string? classToEquip = Bot
                .Inventory.Items.Concat(Bot.Bank.Items)
                .FirstOrDefault(x => acceptableDPSClasses.Contains(x.Name))
                ?.Name;

            if (!string.IsNullOrEmpty(classToEquip))
                C.Equip(classToEquip);
        }
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
        switch (className.ToLower())
        {
            case "chaos avenger":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Anima);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Vainglory);
                break;

            case "archpaladion":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Forge);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Lament);
                break;

            case "legion revenant":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Pneuma);
                Adv.EnhanceItem(className, EnhancementType.Wizard);
                Adv.EnhanceItem(weapon, EnhancementType.Wizard, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Wizard, CapeSpecial.Vainglory);
                break;

            case "archfiend":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Forge);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Vainglory);
                break;

            case "arachnomancer":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Anima);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Vainglory);
                break;

            case "king's echo":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Pneuma);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Lament);
                break;

            case "chrono shadowslayer":
            case "chrono shadowhunter":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Vim);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Health_Vamp);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Lament);
                break;

            case "quantum chronomancer":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Anima);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Vainglory);
                break;

            case "phantom chronomancer":
                Adv.EnhanceItem(helm, EnhancementType.Wizard, hSpecial: HelmSpecial.Pneuma);
                Adv.EnhanceItem(className, EnhancementType.Wizard);
                Adv.EnhanceItem(weapon, EnhancementType.Wizard, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Wizard, CapeSpecial.Vainglory); // or Lament if needed
                break;

            case "infinity knight":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Pneuma);
                Adv.EnhanceItem(className, EnhancementType.Wizard);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Wizard, CapeSpecial.Vainglory);
                break;

            case "lich":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Examen);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Ravenous);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Penitence);
                break;

            case "verus doomknight":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Anima);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Vainglory);
                break;

            case "hollowborn vindicator":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Forge);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Dauntless);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Penitence);
                break;

            case "great thief":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Forge); // or Vim if needed
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Dauntless); // or Lucky HealthVamp
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Vainglory);
                break;
        }
    }
}
