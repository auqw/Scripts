/*
name: NightBane
description: Using 4-7 accounts to kill NightBane
tags: NightBane, ultra, Night Bane, voidNightBane
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;


#region Composition Setup

#region 4-Player Comp (for 5-6 just add a support or another VDK)

// Legion Revenant (VDK)
// ├─ Helm: Pneuma
// ├─ Class: Wizard
// ├─ Weapon: Ravenous
// └─ Cape: Vainglory

// Lord of Order (LOO)
// ├─ Helm: Examen
// ├─ Class: Lucky
// ├─ Weapon: Elysium / Lucky Aweblast
// └─ Cape: Absolution

// StoneCrusher (SC)
// ├─ Helm: Anima
// ├─ Class: Fighter
// ├─ Weapon: Valiance
// └─ Cape: Absolution

// ArchPaladin (AP)
// ├─ Helm: Forge
// ├─ Class: Lucky
// ├─ Weapon: Valiance
// └─ Cape: Lament

#endregion

#region Fast Comp

// Great Thief
// ├─ Helm: Forge / Vim
// ├─ Class: Lucky
// ├─ Weapon: Ravenous
// └─ Cape: Lament

// Great Thief (2nd instance for comp)
// ├─ Helm: Forge / Vim
// ├─ Class: Lucky
// ├─ Weapon: Ravenous
// └─ Cape: Lament

// Verus Doomknight
// ├─ Helm: Anima
// ├─ Class: Lucky
// ├─ Weapon: Ravenous
// └─ Cape: Vainglory

// Legendary Hero
// ├─ Helm: Anima
// ├─ Class: Lucky
// ├─ Weapon: Valiance
// └─ Cape: Lament

// Legion Revenant
// ├─ Helm: Pneuma
// ├─ Class: Wizard
// ├─ Weapon: Ravenous
// └─ Cape: Vainglory

// Archfiend
// ├─ Helm: Forge
// ├─ Class: Lucky
// ├─ Weapon: Ravenous
// └─ Cape: Lament

// Lord of Order
// ├─ Helm: Examen
// ├─ Class: Lucky
// ├─ Weapon: Elysium / Lucky Aweblast
// └─ Cape: Absolution

#endregion

#region F2P Comp

// Arcana Invoker
// ├─ Helm: Examen
// ├─ Class: Lucky
// ├─ Weapon: Elysium
// └─ Cape: Absolution

// Arcana Invoker (2nd instance)
// ├─ Helm: Examen
// ├─ Class: Lucky
// ├─ Weapon: Elysium
// └─ Cape: Absolution

// Verus Doomknight
// ├─ Helm: Anima
// ├─ Class: Lucky
// ├─ Weapon: Ravenous
// └─ Cape: Vainglory

// StoneCrusher
// ├─ Helm: Anima
// ├─ Class: Fighter
// ├─ Weapon: Valiance
// └─ Cape: Absolution

// Legion Revenant
// ├─ Helm: Pneuma
// ├─ Class: Wizard
// ├─ Weapon: Ravenous
// └─ Cape: Vainglory

// Archfiend
// ├─ Helm: Forge
// ├─ Class: Lucky
// ├─ Weapon: Ravenous
// └─ Cape: Lament

// Lord of Order
// ├─ Helm: Examen
// ├─ Class: Lucky
// ├─ Weapon: Elysium / Lucky Aweblast
// └─ Cape: Absolution

#endregion

#region Other DPS 

// Chaos Slayer (CSS)
// ├─ Helm: Forge
// ├─ Class: Lucky
// ├─ Weapon: Valiance
// └─ Cape: Lament

// Light Caster (LC)
// ├─ Helm: Pneuma
// ├─ Class: Lucky
// ├─ Weapon: Ravenous / Praxis
// └─ Cape: Vainglory

// King's Echo (KE)
// ├─ Helm: Examen
// ├─ Class: Lucky
// ├─ Weapon: Ravenous
// └─ Cape: Vainglory

// Shaman
// ├─ Helm: Examen
// ├─ Class: Lucky
// ├─ Weapon: Ravenous
// └─ Cape: Lament

// Dragon of Time (DoT)
// ├─ Helm: Pneuma
// ├─ Class: Wizard
// ├─ Weapon: Elysium
// └─ Cape: Vainglory

// Sentinel
// ├─ Helm: Anima
// ├─ Class: Lucky
// ├─ Weapon: Ravenous
// └─ Cape: Vainglory

// Master Ranger (MR)
// ├─ Helm: Anima
// ├─ Class: Lucky
// ├─ Weapon: Arcana / Praxis
// └─ Cape: Vainglory

#endregion

#endregion

public class NightBane
{
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private CoreBots C => CoreBots.Instance;
    private static CoreAdvanced _Adv;
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEnginev1 Core = new();
    public CoreUltrav1 Ultra = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "NightBane";
    public List<IOption> Options = new()
    {
        new Option<PlayerCount>("PlayerCount", "How many accounts", "Number of accounts (between 4-7) we'll be using", PlayerCount.Four),
        new Option<ItemToFarm>("Item", "Item to Farm", "Which item to farm (choose 'All' for all items)", ItemToFarm.Insatiable_Hunger),
        new Option<int>("Quantity", "Quantity", "Number of items to check/farm", 1),
        new Option<bool>("DoEnh", "Do Enhancements",  "Auto-Enhance Gear properly for the fight", true),
        CoreBots.Instance.SkipOptions,
    };

    // Get selected item and quantity
    int quantityToFarm => Bot.Config!.Get<int>("Quantity");
    ItemToFarm selectedItem => Bot.Config!.Get<ItemToFarm>("Item"); // Keep this as enum
    int Players => (int)Bot.Config!.Get<PlayerCount>("PlayerCount");
    int GetQuantity(ItemToFarm item) => item == ItemToFarm.Starlit_Journal_Page_3 ? 10 : quantityToFarm;

    public void ScriptMain(IScriptInterface bot)
    {
        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();

        Core.Boot();
        Fight();
        Bot.StopSync();
    }

    void Fight()
    {
        const string map = "voidNightBane";
        const string boss = "NightBane";
        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnh();
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        // 'Wrong Turn at Voidbuquerque' && 'Doom Spikes'
        C.EnsureAcceptmultiple(9418, 7713);
        C.AddDrop("Nightbane's ??? Essence", "Insatiable Hunger", "Chest Plate", "Starlit Journal Page 3 Scraps");

        Core.Join(map);
        Ultra.WaitForArmy(Players - 1, "NightBane.sync");
        var (bestCell, bestPad) = Core.ChooseBestCell(boss);

        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();
        while (!Bot.ShouldExit)
        {
            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Bot.Player.Cell != bestCell)
            {
                Bot.Sleep(200);
                C.Jump(bestCell!, bestPad!);
                Bot.Wait.ForCellChange(bestCell!);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => InventoryHasItems(), syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                C.EnsureComplete(8547);

                if (Bot.Config!.Get<bool>("DoEnh"))
                    Adv.GearStore(true, true);
                break;
            }

            if (!Bot.Player.HasTarget)
                Bot.Combat.Attack(boss);

            Bot.Sleep(500);
        }
    }

    bool InventoryHasItems()
    {
        if (selectedItem == ItemToFarm.All)
        {
            // Check all items except "All"
            return Enum.GetValues<ItemToFarm>()
                .Where(i => i != ItemToFarm.All)
                .All(i => Bot.Inventory.Contains((int)i, GetQuantity(i)));
        }
        else
        {
            // Check single item
            return Bot.Inventory.Contains((int)selectedItem, GetQuantity(selectedItem));
        }
    }

    void DoEnh()
    {
        string className = Bot.Player!.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        className = className.ToLower();

        // Helper for conditional weapon choice
        static WeaponSpecial WeaponChoice(WeaponSpecial primary, WeaponSpecial secondary)
            => Adv.uDauntless() ? primary : secondary;

        switch (className)
        {
            case "ArchPaladin":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Lament
                );
                break;
            case "great thief":
                Adv.EnhanceEquipped(
                    hSpecial: Adv.uVim() ? HelmSpecial.Vim : HelmSpecial.Forge,
                    type: EnhancementType.Lucky,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "verus doomknight":
                Adv.EnhanceEquipped(
                    hSpecial: HelmSpecial.Anima,
                    type: EnhancementType.Lucky,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "legendary hero":
                Adv.EnhanceEquipped(
                    hSpecial: HelmSpecial.Anima,
                    type: EnhancementType.Lucky,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "legion revenant":
                Adv.EnhanceEquipped(
                    hSpecial: HelmSpecial.Pneuma,
                    type: EnhancementType.Wizard,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "archfiend":
                Adv.EnhanceEquipped(
                    hSpecial: HelmSpecial.Forge,
                    type: EnhancementType.Lucky,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "lord of order":
                Adv.EnhanceEquipped(
                    hSpecial: HelmSpecial.Examen,
                    type: EnhancementType.Lucky,
                    wSpecial: WeaponSpecial.Elysium,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "arcana invoker":
                Adv.EnhanceEquipped(
                    hSpecial: HelmSpecial.Examen,
                    type: EnhancementType.Lucky,
                    wSpecial: WeaponSpecial.Elysium,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "stonecrusher":
                Adv.EnhanceEquipped(
                    hSpecial: HelmSpecial.Anima,
                    type: EnhancementType.Fighter,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            case "chrono shadowslayer":
            case "chrono shadowhunter":
                Adv.EnhanceEquipped(
                    hSpecial: HelmSpecial.Vim,
                    type: EnhancementType.Lucky,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "light caster":
                Adv.EnhanceEquipped(
                    hSpecial: HelmSpecial.Pneuma,
                    type: EnhancementType.Lucky,
                    wSpecial: WeaponChoice(WeaponSpecial.Ravenous, WeaponSpecial.Praxis),
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "king's echo":
                Adv.EnhanceEquipped(
                    hSpecial: HelmSpecial.Examen,
                    type: EnhancementType.Lucky,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "shaman":
                Adv.EnhanceEquipped(
                    hSpecial: HelmSpecial.Examen,
                    type: EnhancementType.Lucky,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            case "dragon of time":
                Adv.EnhanceEquipped(
                    hSpecial: HelmSpecial.Pneuma,
                    type: EnhancementType.Wizard,
                    wSpecial: WeaponSpecial.Elysium,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "sentinel":
                Adv.EnhanceEquipped(
                    hSpecial: HelmSpecial.Anima,
                    type: EnhancementType.Lucky,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Vainglory
                );
                break;

            case "master ranger":
                Adv.EnhanceEquipped(
                    hSpecial: HelmSpecial.Anima,
                    type: EnhancementType.Lucky,
                    wSpecial: WeaponChoice(WeaponSpecial.Arcanas_Concerto, WeaponSpecial.Praxis),
                    cSpecial: CapeSpecial.Vainglory
                );
                break;
        }
    }



    enum PlayerCount
    {
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7
    }

    public enum ItemToFarm
    {
        All = 0,                  // Special value for "all items"
        Nightbanes_Essence = 73862,
        Insatiable_Hunger = 73361,
        Chest_Plate = 40066,
        Starlit_Journal_Page_3 = 56682
    }
}
