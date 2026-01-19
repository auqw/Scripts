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

5. Verus DoomKnight
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

5. Verus DoomKnight
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

    public bool DontPreconfigure = true;
    public string OptionsStorage = "AstralEmpyrean";
    public List<IOption> Options = new()
    {
        new Option<bool>("DoEnh", "Do Enhancements", "Auto-Enhance Gear properly for the fight", true),
        new Option<bool>("DoZoning", "Actualy use Zones", "To actually use the proper zoning technique, or sit outside the zone (works either way)", false),
        CoreBots.Instance.SkipOptions,
    };

    private string NormalizeString(string input) => (input ?? "").Trim().ToLower();
    bool DoZoning;
    public void ScriptMain(IScriptInterface bot)
    {
        if (!Bot.Quests.IsUnlocked(9803))
            C.Logger(
                "Quest not unlocked: Asterism's Toll,",
                messageBox: true,
                stopBot: true
            );

        Core.Boot();
        Adv.GearStore(EnhAfter: true);
        Prep();
        Fight();
        Adv.GearStore(true, EnhAfter: true);
        Bot.Stop();
    }

    void Prep()
    {
        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnhs();
        Bot.Quests.UpdateQuest(9802);
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Ultra.BuyAlchemyPotion("Potent Honor Potion");
        Core.EquipConsumable("Potent Honor Potion");
        Ultra.GetScrollOfEnrage();
    }

    void Fight()
    {
        bool DoZoning = Bot.Config!.Get<bool>("DoZoning");
        const string map = "astralshrine";
        const string boss = "Astral Empyrean";
        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);
        if (DoZoning)
            Bot.Events.ExtensionPacketReceived += AstralZoneListener;
        C.AddDrop("Star of the Empyrean");
        C.EnsureAccept(8547);

        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_dage.sync");
        Core.ChooseBestCell(boss);
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
            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Astral's Supernova", 1), syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                if (Bot.Quests.CanCompleteFullCheck(9803))
                    C.EnsureComplete(9803);
                break;
            }

            if (!DoZoning)
            {
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
            }

            if (!Bot.Player!.HasTarget)
                Bot.Combat.Attack("*");
            Bot.Sleep(200);
            if (!Bot.Target.Auras.Any(x => x != null && x.Name == "Focus")
                && Bot.Skills.CanUseSkill(5))
                Bot.Skills.UseSkill(5);
        }
        if (DoZoning)
            Bot.Events.ExtensionPacketReceived -= AstralZoneListener;
        if (Bot.Config!.Get<bool>("DoEnh"))
            Adv.GearStore(true, true);
    }


    void DoEnhs()
    {
        string className = Bot.Player!.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

        C.Logger("Starting Ultra Enhancing -- Beep Boop");

        switch (className)
        {
            // Chrono ShadowSlayer
            case "Chrono ShadowSlayer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Vim,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            // Archfiend
            case "Archfiend":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            // Arachnomancer
            case "Arachnomancer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Vim,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            // Legion Revenant
            case "Legion Revenant":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            // Verus DoomKnight
            case "Verus DoomKnight":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            // Legendary Hero
            case "Legendary Hero":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            // Lord Of Order
            case "Lord Of Order":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Valiance,
                    cSpecial: CapeSpecial.Absolution
                );
                break;

            // Arcana Invoker
            case "Arcana Invoker":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            // Lich
            case "Lich":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            // King's Echo
            case "King's Echo":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            // Sentinel
            case "Sentinel":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Anima,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            // Great Thief
            case "Great Thief":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Arcanas_Concerto,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            // Guardian
            case "Guardian":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Forge,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Penitence
                );
                break;

            // Phantom Chronomancer
            case "Phantom Chronomancer":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Wizard,
                    hSpecial: HelmSpecial.Examen,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Lament
                );
                break;

            // Light Caster
            case "Light Caster":
                Adv.EnhanceEquipped(
                    type: EnhancementType.Lucky,
                    hSpecial: HelmSpecial.Pneuma,
                    wSpecial: WeaponSpecial.Ravenous,
                    cSpecial: CapeSpecial.Lament
                );
                break;
        }
    }

    public async void AstralZoneListener(dynamic packet)
    {
        if (!DoZoning)
        {
            Bot.Events.ExtensionPacketReceived -= AstralZoneListener;
            return;
        }
        if (packet?["params"]?.type?.ToString() != "json")
            return;
        if (!Bot.Player.Alive)
            return;
        dynamic data = packet["params"].dataObj;
        if (data?.cmd?.ToString() != "event")
            return;
        string? zoneSet = data?.args?.zoneSet?.ToString();
        if (string.IsNullOrEmpty(zoneSet))
            return;

        Random rnd = new Random();
        int x = 0, y = 0;

        switch (zoneSet.ToUpper())
        {
            case "A":
                x = 122 + rnd.Next(-15, 16);
                y = 420 + rnd.Next(-15, 16);
                break;
            case "B":
                x = 856 + rnd.Next(-15, 16);
                y = 420 + rnd.Next(-15, 16);
                break;
            default:
                return;
        }

        _ = Task.Run(() => Bot.Player.WalkTo(x, y));
    }
}

