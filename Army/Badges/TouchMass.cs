/*
name: TouchMass
description: gets u the million kills for the `Touch Mass` badge
tags: touch, mass, badge, starfield, badge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;
using Skua.Core.Options;
using Skua.Core.Scripts;

public class TouchMass
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots C => CoreBots.Instance;

    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;

    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    private static CoreDailies Daily
    {
        get => _Daily ??= new CoreDailies();
        set => _Daily = value;
    }
    private static CoreDailies _Daily;

    private static CoreArmyLite Army
    {
        get => _Army ??= new CoreArmyLite();
        set => _Army = value;
    }
    private static CoreArmyLite _Army;

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
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    public string OptionsStorage = "Starfield Badge";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        sArmy.player1,
        sArmy.player2,
        sArmy.player3,
        sArmy.player4,
        sArmy.player5,
        sArmy.packetDelay,
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        C.SetOptions(disableClassSwap: true);
        SetAdditonOptions();
        StupidQuest();
        C.SetOptions(false);
    }

    public void StupidQuest()
    {
        if (C.HasWebBadge(badge))
        {
            C.Logger($"Already have the {badge} badge");
            return;
        }

        if (sArmy.Players().Length <= 0)
        {
            C.Logger(
                "Players empty, please add players to the options ( scripts botton > edit scripts option > insert account names exactly as is)"
            );
            return;
        }
        C.EquipClass(ClassType.Farm);
        const string map = "starfield";
        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Bot.Sleep(2500);
        C.Logger($"Players in Army: {sArmy.Players().Length}");

        C.PrivateRooms = true;
        C.PrivateRoomNumber = Army.getRoomNr();
        C.Logger("Setting Additional options to try and mitigate some lag.");

        C.AddDrop(86808); // Stars Destroyed
        C.EnsureAccept(9818);

        Core.Join(map);
        C.Jump("r3", "Left");

        if (sArmy.Players().Length > 1)
            Ultra.WaitForArmy(sArmy.Players().Length - 1, "StupidQuest.sync");

        Bot.Player.SetSpawnPoint();
        Bot.Sleep(1500);
        Bot.Options.AggroMonsters = true;

        while (!Bot.ShouldExit)
        {
            if (Ultra.CheckArmyProgressBool(() => C.HasWebBadge(badge), syncPath))
            {
                Bot.Options.AggroMonsters = false;
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                break;
            }

            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            Bot.Combat.Attack("*");
            Bot.Sleep(500);
        }
    }

    private string badge = "Touch Mass";

    private void SetAdditonOptions()
    {
        // Ensure options are enabled.. map laggy af?
        C.Logger("Enabling LagKiller");
        Bot.Options.LagKiller = true;
        C.Sleep();

        C.Logger("Setting FPS to 10");
        Bot.Options.SetFPS = 10;
        C.Sleep();

        C.Logger("Setting Custom Name to 'AE made this quest for botters'");
        Bot.Options.CustomName = "AE made this quest for botters";
        Bot.Options.CustomGuild = $"🖕";
        C.Sleep();

        C.Logger("Accepting AC Drops");
        Bot.Options.AcceptACDrops = true;
        C.Sleep();

        C.Logger("Freezing monster positions");
        Bot.Lite.FreezeMonsterPosition = true;
        C.Sleep();

        C.Logger("Enabling Custom Drops UI");
        Bot.Lite.CustomDropsUI = true;
        C.Sleep();

        C.Logger("Disabling Red Warning");
        Bot.Lite.DisableRedWarning = true;
        C.Sleep();

        C.Logger("Disabling Self Animation");
        Bot.Lite.DisableSelfAnimation = true;
        C.Sleep();

        C.Logger("Disabling Skill Animation");
        Bot.Lite.DisableSkillAnimation = true;
        C.Sleep();

        C.Logger("Disabling Weapon Animation");
        Bot.Lite.DisableWeaponAnimation = true;
        C.Sleep();

        C.Logger("Disabling Monster Animation");
        Bot.Lite.DisableMonsterAnimation = true;
        C.Sleep();

        C.Logger("Disabling Damage Strobe");
        Bot.Lite.DisableDamageStrobe = true;
        C.Sleep();
    }
}
