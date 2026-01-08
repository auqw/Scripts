/*
name: VADaily
description: `wrong turn at voidbuquerque` quest with an army.
tags: wrong turn at voidbuquerque, void, aura, voidaura 
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

public class VADaily
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
        C.BankingBlackList.AddRange(new[] { "Flibbitiestgibbet's ??? Essence", "Nightbane's ??? Essence", "Xyfrag's ??? Essence" });

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

        DoVoidbuquerque();

        C.SetOptions(false);
    }

    void DoVoidbuquerque()
    {

        C.EnsureAccept(9091);

        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        if (IsTaunter())
        {
            Bot.Events.ExtensionPacketReceived += Ultra.GenericChargeListener;
            Ultra.GetScrollOfEnrage();
        }
        Xyfrag();

        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        Flib();

        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        NightBane();

        C.EnsureComplete(9091);
        C.Logger("All players finished farm.");

    }

    void Flib()
    {
        string map = "voidflibbi";
        string Boss = "Flibbitiestgibbet";
        string syncPath = Ultra.ResolveSyncPath("ArmyBool.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(25000);
        C.Logger($"Players in Curreny Army: {sArmy.Players().Length}");
        Core.Join(map);
        if (sArmy.Players().Length > 1)
            Ultra.WaitForArmy(sArmy.Players().Length - 1, "VoidFlib.sync");
        C.AddDrop("Flibbitiestgibbet's ??? Essence");
        Core.ChooseBestCell(Boss);
        Bot.Player.SetSpawnPoint();
        Bot.Sleep(1500);
        Bot.Options.AggroMonsters = true;
        while (!Bot.ShouldExit)
        {
            if (Ultra.CheckArmyProgressBool(() => Bot.Inventory.Contains("Flibbitiestgibbet's ??? Essence"), syncPath))
            {
                C.JumpWait();
                C.Logger("All players finished farming \"Flibbitiestgibbet's ??? Essence\".");
                break;
            }

            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            Bot.Combat.Attack(Boss);
            Bot.Sleep(500);
        }
    }

    void NightBane()
    {
        string map = "voidnightbane";
        string Boss = "Nightbane";
        string syncPath = Ultra.ResolveSyncPath("ArmyBool.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(25000);
        C.Logger($"Players in Curreny Army: {sArmy.Players().Length}");
        Core.Join(map);
        Core.ChooseBestCell(Boss);
        if (sArmy.Players().Length > 1)
            Ultra.WaitForArmy(sArmy.Players().Length - 1, "Nightbane.sync");
        C.AddDrop("Nightbane's ??? Essence");
        Bot.Player.SetSpawnPoint();
        Bot.Sleep(1500);
        Bot.Options.AggroMonsters = true;
        while (!Bot.ShouldExit)
        {
            if (Ultra.CheckArmyProgressBool(() => Bot.Inventory.Contains("Nightbane's ??? Essence"), syncPath))
            {
                C.JumpWait();
                C.Logger("All players finished farming \"Nightbane's ??? Essence\".");
                break;
            }

            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            Bot.Combat.Attack(Boss);
            Bot.Sleep(500);
        }
    }

    void Xyfrag()
    {
        const string map = "voidxyfrag";
        const string boss = "Xyfrag";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        // 'Wrong Turn at Voidbuquerque' && 'Doom Spikes'
        C.EnsureAcceptmultiple(9091, 9418, 8547);
        C.AddDrop("Xyfrag's ??? Essence", "Xyfrag's Slimy Tooth", "Void Energy");

        Core.Join(map);
        if (sArmy.Players().Length > 1)
            Ultra.WaitForArmy(sArmy.Players().Length - 1, "Xyfrag.sync");
        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();
        while (!Bot.ShouldExit)
        {
            if (Ultra.CheckArmyProgressBool(() => Bot.Inventory.Contains($"Xyfrag's ??? Essence"), syncPath))
            {
                Bot.Events.ExtensionPacketReceived -= Ultra.GenericChargeListener;
                C.JumpWait();
                C.Logger("All players finished farming \"Xyfrag's ??? Essence\".");
                break;
            }
            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }


            if (Core.HasClassEquipped(taunter))
                Ultra.Taunt(taunter, boss, "charge", 250, "Focus");
            Bot.Combat.Attack(boss);
            Bot.Sleep(500);
        }
        Bot.Events.ExtensionPacketReceived -= Ultra.GenericChargeListener;
    }


}
