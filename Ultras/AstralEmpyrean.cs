/*
name: AstralEmpyrean
description: Two-taunter strategy for Astral Empyrean with aura-based taunting and army synchronization.
tags: Ultra, AstralEmpyrean, Astral Empyrean
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs


/* FAST COMP
============================================
============================================
1. Chrono ShadowSlayer
   - Helm: Vim (Lucky)
   - Class: Lucky
   - Weapon: Valiance (Lucky)
   - Cape: Lament (Lucky)

2. Archfiend
   - Helm: Forge (Lucky)
   - Class: Lucky
   - Weapon: Ravenous (Lucky)
   - Cape: Lament (Lucky)

3. Arachnomancer
   - Helm: Vim (Lucky)
   - Class: Lucky
   - Weapon: Ravenous (Lucky)
   - Cape: Lament (Lucky)

4. Legion Revenant
   - Helm: Pneuma (Lucky)
   - Class: Wizard
   - Weapon: Ravenous (Wizard)
   - Cape: Lament (Wizard)

5. Verus Doomknight
   - Helm: Anima (Lucky)
   - Class: Lucky
   - Weapon: Ravenous (Lucky)
   - Cape: Penitence (Lucky)

6. Legendary Hero
   - Helm: Anima (Lucky)
   - Class: Lucky
   - Weapon: Valiance (Lucky)
   - Cape: Lament (Lucky)

7. Lord of Order
   - Helm: Examen (Lucky)
   - Class: Lucky
   - Weapon: Valiance (Lucky)
   - Cape: Absolution (Lucky)
============================================
*/

/* F2P COMP
============================================
============================================
1. Arcana Invoker
   - Helm: Examen (Lucky)
   - Class: Lucky
   - Weapon: Ravenous (Lucky)
   - Cape: Penitence (Lucky)

2. Archfiend
   - Helm: Forge (Lucky)
   - Class: Lucky
   - Weapon: Ravenous (Lucky)
   - Cape: Lament (Lucky)

3. Arachnomancer
   - Helm: Vim (Lucky)
   - Class: Lucky
   - Weapon: Ravenous (Lucky)
   - Cape: Lament (Lucky)

4. Legion Revenant
   - Helm: Pneuma (Lucky)
   - Class: Wizard
   - Weapon: Ravenous (Wizard)
   - Cape: Lament (Wizard)

5. Verus Doomknight
   - Helm: Anima (Lucky)
   - Class: Lucky
   - Weapon: Ravenous (Lucky)
   - Cape: Penitence (Lucky)

6. Legendary Hero
   - Helm: Anima (Lucky)
   - Class: Lucky
   - Weapon: Valiance (Lucky)
   - Cape: Lament (Lucky)

7. Lord of Order
   - Helm: Examen (Lucky)
   - Class: Lucky
   - Weapon: Valiance (Lucky)
   - Cape: Absolution (Lucky)
============================================
*/

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class AstralEmpyrean
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

    private string NormalizeString(string input) => (input ?? "").Trim().ToLower();

    public void ScriptMain(IScriptInterface bot)
    {
        if (!Bot.Quests.IsUnlocked(9803))
            C.Logger(
                "Quest not unlocked: Asterism's Toll,",
                messageBox: true,
                stopBot: true
            );

        C.Join("whitemap");
        Core.Boot();
        Adv.GearStore();
        Prep();
        Fight();
        Adv.GearStore(true);
        Bot.Stop();
    }

    void Prep()
    {
        Bot.Quests.UpdateQuest(9802);
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");
        Ultra.GetScrollOfEnrage();
        Enhancements();
    }

    void Fight()
    {
        const string map = "astralshrine";
        const string boss = "Astral Empyrean";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        C.AddDrop("Star of the Empyrean");
        C.EnsureAccept(8547);

        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_dage.sync");
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        while (!Bot.ShouldExit)
        {
            if (Ultra.CheckArmyProgress("Astral's Supernova", 1, true, syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                if (Bot.Quests.CanCompleteFullCheck(9803))
                    C.EnsureComplete(9803);
                break;
            }
            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            // Define box boundaries (0,0 to 101,101)
            int minX = 374;
            int maxX = 454;
            int minY = 398;
            int maxY = 419;

            // Check if player is within the box
            bool isInBox =
                Bot.Player.Position.X >= minX
                && Bot.Player.Position.X <= maxX
                && Bot.Player.Position.Y >= minY
                && Bot.Player.Position.Y <= maxY;

            // If not in box, move to random location within box
            if (!isInBox)
            {
                Random rand = new();
                int randomX = rand.Next(minX, maxX + 1);
                int randomY = rand.Next(minY, maxY + 1);
                Bot.Player.WalkTo(randomX, randomY);
            }

            if (!Bot.Player!.HasTarget)
                Bot.Combat.Attack("*");
            Bot.Sleep(200);
            if (
                !Bot.Self.Auras.Any(x => x != null && x.Name == "Focus")
                && Bot.Skills.CanUseSkill(5)
            )
                Bot.Skills.UseSkill(5);
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

        C.Logger("Starting Ultra Enhancing -- Beep Boop");

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

        // Apply enhancement rules per role
        switch (Bot.Player?.CurrentClass?.Name.ToLower())
        {
            case "chrono shadowslayer":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Vim);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Valiance);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Lament);
                break;

            case "archfiend":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Forge);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Ravenous);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Lament);
                break;

            case "arachnomancer":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Vim);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Ravenous);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Lament);
                break;

            case "legion revenant":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Pneuma);
                Adv.EnhanceItem(className, EnhancementType.Wizard);
                Adv.EnhanceItem(weapon, EnhancementType.Wizard, wSpecial: WeaponSpecial.Ravenous);
                Adv.EnhanceItem(cape, EnhancementType.Wizard, CapeSpecial.Lament);
                break;

            case "verus doomknight":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Anima);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Ravenous);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Penitence);
                break;

            case "legendary hero":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Anima);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Valiance);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Lament);
                break;

            case "lord of order":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Examen);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Valiance);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Absolution);
                break;

            case "arcana invoker":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Examen);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Ravenous);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Penitence);
                break;

            case "lich":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Examen);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Ravenous);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Penitence);
                break;

            case "king's echo":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Examen);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Ravenous);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Lament);
                break;

            case "sentinel":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Anima);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Ravenous);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Lament);
                break;

            case "great thief":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Forge);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Arcanas_Concerto);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Lament);
                break;

            case "guardian":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Forge);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Ravenous);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Penitence);
                break;

            case "phantom chronomancer":
                Adv.EnhanceItem(helm, EnhancementType.Wizard, hSpecial: HelmSpecial.Examen);
                Adv.EnhanceItem(className, EnhancementType.Wizard);
                Adv.EnhanceItem(weapon, EnhancementType.Wizard, wSpecial: WeaponSpecial.Ravenous);
                Adv.EnhanceItem(cape, EnhancementType.Wizard, CapeSpecial.Lament);
                break;

            case "light caster":
                Adv.EnhanceItem(helm, EnhancementType.Lucky, hSpecial: HelmSpecial.Pneuma);
                Adv.EnhanceItem(className, EnhancementType.Lucky);
                Adv.EnhanceItem(weapon, EnhancementType.Lucky, wSpecial: WeaponSpecial.Ravenous);
                Adv.EnhanceItem(cape, EnhancementType.Lucky, CapeSpecial.Lament);
                break;
        }
    }

}
