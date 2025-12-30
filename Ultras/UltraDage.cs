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
    Verus DoomKnight
    King's Echo
    Phantom Chronomancer / Phantasm Chronomancer
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
    string a,
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
        new Option<bool>("DoEnh", "Do Enhancements",  "Auto-Enhance Gear properly for the fight", true),
        CoreBots.Instance.SkipOptions,
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

        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();
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

    bool IsTaunter() => Core.HasClassEquipped(a) || Core.HasClassEquipped(b);

    void Prep()
    {
        Bot.Events.ExtensionPacketReceived += UltraDageListener;
        Bot.Quests.UpdateQuest(793);
        if (Bot.Config!.Get<bool>("DoEnh"))
            DoEnh();
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        if (IsTaunter())
            Ultra.GetScrollOfEnrage();
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
            if (Ultra.CheckArmyProgressBool(() => C.CheckInventory("Dage the Dark Lord Defeated", 1), syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                C.EnsureComplete(8547);
                break;
            }
            // Dead → wait for respawn
            if (!Bot.Player.Alive)
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

            if (Core.HasClassEquipped(a) || Core.HasClassEquipped(b))
            {
                if (Bot.Skills.CanUseSkill(5))
                    Bot.Skills.UseSkill(5);
            }

            if (!Bot.Player.HasTarget)
                Bot.Combat.Attack(boss);
        }
    }

    // public async void UltraDageListener(dynamic packet)
    // {
    //     if (packet?["params"]?.type?.ToString() != "json")
    //         return;
    //     if (!Bot.Player.Alive)
    //         return;
    //     dynamic data = packet["params"].dataObj;
    //     if (data?.cmd?.ToString() != "event")
    //         return;

    //     if (
    //         !string.IsNullOrEmpty(data?.args?.zoneSet?.ToString())
    //         && string.Equals(
    //             data?.args?.zoneSet?.ToString(),
    //             "A",
    //             StringComparison.OrdinalIgnoreCase
    //         )
    //     )
    //     {
    //         await Task.Run(() => Bot.Player.WalkTo(122, 420));
    //         return;
    //     }
    //     if (
    //         !string.IsNullOrEmpty(data?.args?.zoneSet?.ToString())
    //         && string.Equals(
    //             data?.args?.zoneSet?.ToString(),
    //             "B",
    //             StringComparison.OrdinalIgnoreCase
    //         )
    //     )
    //     {
    //         await Task.Run(() => Bot.Player.WalkTo(856, 420));
    //         return;
    //     }
    // }

    public async void UltraDageListener(dynamic packet)
    {
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

        if (string.Equals(zoneSet, "A", StringComparison.OrdinalIgnoreCase))
        {
            _ = Task.Run(() => Bot.Player.WalkTo(122, 420));
            return;
        }

        if (string.Equals(zoneSet, "B", StringComparison.OrdinalIgnoreCase))
        {
            _ = Task.Run(() => Bot.Player.WalkTo(856, 420));
            return;
        }
    }

    void DoEnh()
    {
        string className = Bot.Player!.CurrentClass?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(className))
            return;

   switch (className.ToLower())
{
    case "chaos avenger":
        Adv.EnhanceEquipped(
            type: EnhancementType.Lucky,
            hSpecial: HelmSpecial.Anima,
            wSpecial: WeaponSpecial.Dauntless,
            cSpecial: CapeSpecial.Vainglory
        );
        break;

    case "archpaladin":
        Adv.EnhanceEquipped(
            type: EnhancementType.Lucky,
            hSpecial: HelmSpecial.Forge,
            wSpecial: WeaponSpecial.Dauntless,
            cSpecial: CapeSpecial.Lament
        );
        break;

    case "legion revenant":
        Adv.EnhanceEquipped(
            type: EnhancementType.Wizard,
            hSpecial: HelmSpecial.Pneuma,
            wSpecial: WeaponSpecial.Dauntless,
            cSpecial: CapeSpecial.Vainglory
        );
        break;

    case "archfiend":
        Adv.EnhanceEquipped(
            type: EnhancementType.Lucky,
            hSpecial: HelmSpecial.Forge,
            wSpecial: WeaponSpecial.Dauntless,
            cSpecial: CapeSpecial.Vainglory
        );
        break;

    case "arachnomancer":
        Adv.EnhanceEquipped(
            type: EnhancementType.Lucky,
            hSpecial: HelmSpecial.Anima,
            wSpecial: WeaponSpecial.Dauntless,
            cSpecial: CapeSpecial.Vainglory
        );
        break;

    case "king's echo":
        Adv.EnhanceEquipped(
            type: EnhancementType.Lucky,
            hSpecial: HelmSpecial.Pneuma,
            wSpecial: WeaponSpecial.Dauntless,
            cSpecial: CapeSpecial.Lament
        );
        break;

    case "chrono shadowslayer":
    case "chrono shadowhunter":
        Adv.EnhanceEquipped(
            type: EnhancementType.Lucky,
            hSpecial: HelmSpecial.Vim,
            wSpecial: WeaponSpecial.Health_Vamp,
            cSpecial: CapeSpecial.Lament
        );
        break;

    case "quantum chronomancer":
        Adv.EnhanceEquipped(
            type: EnhancementType.Lucky,
            hSpecial: HelmSpecial.Anima,
            wSpecial: WeaponSpecial.Dauntless,
            cSpecial: CapeSpecial.Vainglory
        );
        break;

    case "phantom chronomancer":
    case "phantasm chronomancer":
        Adv.EnhanceEquipped(
            type: EnhancementType.Wizard,
            hSpecial: HelmSpecial.Pneuma,
            wSpecial: WeaponSpecial.Dauntless,
            cSpecial: CapeSpecial.Vainglory
        );
        break;

    case "infinity knight":
        Adv.EnhanceEquipped(
            type: EnhancementType.Wizard,
            hSpecial: HelmSpecial.Pneuma,
            wSpecial: WeaponSpecial.Dauntless,
            cSpecial: CapeSpecial.Vainglory
        );
        break;

    case "lich":
        Adv.EnhanceEquipped(
            type: EnhancementType.Lucky,
            hSpecial: HelmSpecial.Examen,
            wSpecial: WeaponSpecial.Ravenous,
            cSpecial: CapeSpecial.Penitence
        );
        break;

    case "verus doomknight":
        Adv.EnhanceEquipped(
            type: EnhancementType.Lucky,
            hSpecial: HelmSpecial.Anima,
            wSpecial: WeaponSpecial.Dauntless,
            cSpecial: CapeSpecial.Vainglory
        );
        break;

    case "hollowborn vindicator":
        Adv.EnhanceEquipped(
            type: EnhancementType.Lucky,
            hSpecial: HelmSpecial.Forge,
            wSpecial: WeaponSpecial.Dauntless,
            cSpecial: CapeSpecial.Penitence
        );
        break;

    case "great thief":
        Adv.EnhanceEquipped(
            type: EnhancementType.Lucky,
            hSpecial: HelmSpecial.Forge,
            wSpecial: WeaponSpecial.Dauntless,
            cSpecial: CapeSpecial.Vainglory
        );
        break;
}
}


}
