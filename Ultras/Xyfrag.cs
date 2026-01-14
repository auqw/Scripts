/*
name: null
description: null
tags: null
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class Xyfrag
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

    string taunter;
    public bool DontPreconfigure = true;
    public string OptionsStorage = "Xyfrag";
    public List<IOption> Options = new()
    {
        new Option<string>(
            "taunter",
            "Taunter Class",
            "Insert the name of the class that will taunt",
            ""
        ),
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

        Bot.Options.InfiniteRange = true;

        taunter = (Bot.Config!.Get<string>("taunter") ?? "").Trim();
        if (string.IsNullOrEmpty(taunter))
        {
            Core.Log("Setup", "Fill the taunter class in Script Options.");
            Bot.Stop();
            return;
        }

        Core.Boot();
        Prep();
        Fight();
        Bot.Events.ExtensionPacketReceived -= Ultra.GenericChargeListener;
        Bot.Stop();
    }

    bool IsTaunter() => Core.HasClassEquipped(taunter);

    void Prep()
    {
        Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
        if (IsTaunter())
        {
            Bot.Events.ExtensionPacketReceived += Ultra.GenericChargeListener;
            Ultra.GetScrollOfEnrage();
        }
    }


    void Fight()
    {
        const string map = "voidxyfrag";
        const string boss = "Xyfrag";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        Bot.Sleep(2500);

        // 'Wrong Turn at Voidbuquerque' && 'Doom Spikes'
        C.EnsureAcceptmultiple(9091, 9418);
        C.AddDrop("Xyfrag's ??? Essence", "Xyfrag's Slimy Tooth", "Void Energy");

        Core.Join(map);
        Ultra.WaitForArmy(6, "xyfrag.sync");
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
            
            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains("Xyfrag's Slimy Tooth", 5), syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                C.EnsureComplete(8547);
                Adv.GearStore(true, true);
                break;
            } 

            if (Core.HasClassEquipped(taunter))
                Ultra.Taunt(taunter, boss, "charge", 250);
            else
            {
                Core.Kill(boss);
                Bot.Skills.UseSkill(5);
            }
        }
    }
}
