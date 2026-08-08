/*
name: UltraWarden
description: Ultra Warden helper with HP-band taunt trigger and army synchronization.
tags: Ultra
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraWarden
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

    string a,
        b;
    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraWarden";
    public List<IOption> Options = new()
    {
        new Option<WardenComp>(
            "DoEquipClasses",
            "Automatically Equip Classes",
            "Auto-equip classes across all 4 clients\n"
                + "Recommended: LR / AP / LOO / VDK\n"
                + "Unselected = off (use whatever classes you already have equipped).",
            WardenComp.Unselected
        ),
        new Option<string>("a", "Taunter Class (Primary)", "Class name that will taunt first", ""),
        new Option<string>("b", "Taunter Class (Backup)", "Backup taunter class", ""),
        new Option<bool>("DoEnh", "Do Enhancements",  "Auto-Enhance Gear properly for the fight", true),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();
        a = (Bot.Config!.Get<string>("a") ?? "").Trim();
        b = (Bot.Config.Get<string>("b") ?? "").Trim();

        if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
        {
            Core.Log(
                "Setup",
                "Fill at least one taunter class (Primary or Backup) in Script Options."
            );
            Bot.StopSync();
            return;
        }

        Core.Boot();
        Prep();
        Fight();
        Bot.StopSync();
    }

    bool IsTaunter() => Core.HasClassEquipped(a) || Core.HasClassEquipped(b);

    void Prep()
    {
        // Sync-equip classes if a comp is selected
        WardenComp comp = Bot.Config!.Get<WardenComp>("DoEquipClasses");
        if (comp != WardenComp.Unselected)
        {
            string[] classes = comp switch
            {
                WardenComp.Recommended => new[] { "Legion Revenant", "ArchPaladin", "Lord of Order", "Verus DoomKnight" },
                _ => throw new InvalidOperationException($"Unhandled WardenComp value: {comp}")
            };

            Ultra.EquipClassSync(classes, 4, "warden_class.sync");
        }

        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        if (IsTaunter())
            Ultra.GetScrollOfEnrage();

    }

    void Fight()
    {
        const string map = "ultrawarden";
        const string boss = "Ultra Warden";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);
        C.EnsureAccept(8153);
        C.AddDrop("Warden Insignia");
        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_warden.sync");
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

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Ultra Warden Defeated", 1), syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                C.EnsureComplete(8153);
                if (Bot.Config!.Get<bool>("DoEnh"))
                    Adv.GearStore(true, true);
                break;
            }

            if (Core.HasClassEquipped(a) || Core.HasClassEquipped(b))
                Ultra.UltraWardenTaunter();

            Bot.Combat.Attack("*");
            Bot.Sleep(250);
        }
    }

    public enum WardenComp
    {
        Unselected,
        Recommended,
    }
}
