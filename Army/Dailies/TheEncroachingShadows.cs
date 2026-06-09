/*
name: VADaily
description: `wrong turn at voidbuquerque` quest with an army.
tags: The Encroaching Shadows, TheEncroachingShadows, Encroaching, Shadows, army, void aura
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Army/CoreArmyLite.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class TheEncroachingShadows
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
    private static CoreBots sCore
    {
        get => _sCore ??= new CoreBots();
        set => _sCore = value;
    }

    private static CoreBots _sCore;

    private static CoreArmyLite sArmy
    {
        get => _sArmy ??= new CoreArmyLite();
        set => _sArmy = value;
    }

    private static CoreArmyLite _sArmy;
    private static CoreArmyLite Army
    {
        get => _Army ??= new CoreArmyLite();
        set => _Army = value;
    }
    private static CoreArmyLite _Army;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    string taunter;
    public string OptionsStorage = "voidbuquerque";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<string>(
            "taunter",
            "Taunter Class",
            "Insert the name of the class that will taunt",
            "ArchPaladin"
        ),
        sArmy.player1,
        sArmy.player2,
        sArmy.player3,
        sArmy.player4,
        sArmy.player5,
        sArmy.player6,
        sArmy.player7,
        sArmy.packetDelay,
        CoreBots.Instance.SkipOptions,
    };

    bool IsTaunter() => Core.HasClassEquipped(taunter);
    public void ScriptMain(IScriptInterface Bot)
    {
        C.BankingBlackList.AddRange(new[] { $"Flibbitiestgibbet's ??? Essence", $"Nightbane's ??? Essence", $"Xyfrag's ??? Essence" });

        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();

        if (sArmy.Players().Length < 4)
        {
            C.Logger(
                "Players empty or less then 4 (7 reccomended), please add players to the options ( scripts botton > edit scripts option > insert account names exactly as is)"
            );
            return;
        }

        taunter = (Bot.Config!.Get<string>("taunter") ?? "").Trim();
        Core.Boot();

        DoEnchroach();

        C.SetOptions(false);
    }

    void DoEnchroach()
    {
        C.EnsureAcceptmultiple(8653, 9091);
        C.Logger("Also accepting [9091] to get flib's ess whilst we do this incase ya wanna do the other army daily");

        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        if (IsTaunter())
        {
            Bot.Events.ExtensionPacketReceived += Ultra.GenericChargeListener;
            Ultra.GetScrollOfEnrage();
        }
        Bot.Drops.Add($"Flibbitiestgibbet's ??? Essence", "Glacial Pinion", "Hydra Eyeball", "Flibbitigiblets", "Void Essentia", "Void Energy", "Chest Plate");
        C.AddDrop(70052, 70053, 70054, 73862);

        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Flib();
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        IceWing();
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Hydra90();

        C.AbandonQuest(9091);
        C.EnsureComplete(8653);
        C.Logger("All players finished farm.");

    }

    void Flib()
    {
        string map = "voidflibbi";
        string Boss = "Flibbitiestgibbet";
        string syncPath = Ultra.ResolveSyncPath("ArmyBool.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);
        C.Logger($"Players in Curreny Army: {sArmy.Players().Length}");
        Core.Join(map);
        if (sArmy.Players().Length > 1)
            Ultra.WaitForArmy(sArmy.Players().Length - 1, "VoidFlib.sync");
        Core.ChooseBestCell(Boss);
        Bot.Player.SetSpawnPoint();
        Bot.Sleep(1500);
        Bot.Options.AggroMonsters = true;
        while (!Bot.ShouldExit)
        {
            Bot.Drops.Add("Glacial Pinion", "Hydra Eyeball", "Flibbitigiblets", "Void Energy");
            C.AddDrop(70052, 70053, 70054);


            if (Ultra.CheckArmyProgressBool(() => C.CheckInventory(70054), syncPath))
            {
                C.JumpWait();
                C.Logger("All players finished farming \"Flibbitigiblets\".");
                break;
            }

            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (!Bot.Player.HasTarget)
                Bot.Combat.Attack(Boss);
            Bot.Sleep(500);
        }
    }

    void IceWing()
    {
        string map = "icewing";
        string Boss = "Warlord Icewing";
        string syncPath = Ultra.ResolveSyncPath("ArmyBool.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);
        C.Logger($"Players in Curreny Army: {sArmy.Players().Length}");
        Core.Join(map);
        Core.ChooseBestCell(Boss);
        if (sArmy.Players().Length > 1)
            Ultra.WaitForArmy(sArmy.Players().Length - 1, "WarlordIcewing.sync");
        Bot.Player.SetSpawnPoint();
        Bot.Sleep(1500);
        Bot.Options.AggroMonsters = true;
        while (!Bot.ShouldExit)
        {
            if (Ultra.CheckArmyProgressBool(() => C.CheckInventory(70052), syncPath))
            {
                C.JumpWait();
                C.Logger("All players finished farming \"Glacial Pinion\".");
                break;
            }

            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (!Bot.Player.HasTarget)
                Bot.Combat.Attack(Boss);
            Bot.Sleep(500);
        }
    }

    void Hydra90()
    {
        const string map = "hydrachallenge";
        const string boss = $"Hydra Head 90";
        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        Core.Join(map);
        if (sArmy.Players().Length > 1)
            Ultra.WaitForArmy(sArmy.Players().Length - 1, "HydraHead90.sync");
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        while (!Bot.ShouldExit)
        {
            if (Ultra.CheckArmyProgressBool(() => C.CheckInventory(70053, 3), syncPath))
            {
                C.JumpWait();
                C.Logger("All players finished farming \"Hydra Eyeball\".");
                break;
            }
            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (!Bot.Player.HasTarget)
                Bot.Combat.Attack(boss);
            Bot.Sleep(500);
        }
    }


}
