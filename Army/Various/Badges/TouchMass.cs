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
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;
using Skua.Core.Options;
using Skua.Core.Scripts;

public class TouchMass
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;

    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;

    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;

    private static CoreDailies Daily { get => _Daily ??= new CoreDailies(); set => _Daily = value; }
    private static CoreDailies _Daily;

    private static CoreArmyLite Army { get => _Army ??= new CoreArmyLite(); set => _Army = value; }
    private static CoreArmyLite _Army;


    private static CoreBots sCore { get => _sCore ??= new CoreBots(); set => _sCore = value; }


    private static CoreBots _sCore;

    private static CoreArmyLite sArmy { get => _sArmy ??= new CoreArmyLite(); set => _sArmy = value; }

    private static CoreArmyLite _sArmy;


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
        Core.SetOptions(disableClassSwap: true);
        StupidQuest();
        Core.SetOptions(false);
    }

    public void StupidQuest()
    {
        if (Core.HasWebBadge(badge))
        {
            Core.Logger($"Already have the {badge} badge");
            return;
        }

        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = Army.getRoomNr();

        Core.Logger("Setting Aditional options to try and mitigate some lag.");
        SetAdditonOptions();

        Core.AddDrop(86808); // Stars Destroyed
        Core.EnsureAccept(9818);

        Army.AggroMonMIDs(7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22);
        Army.AggroMonStart("starfield");
        Army.DivideOnCells("r3");
        bool ded = false;

        // Method must match the delegate signature
        void OnMonsterKilled(int b) => ded = true;

        Bot.Events.MonsterKilled += OnMonsterKilled;

        while (!Bot.ShouldExit && !Core.HasWebBadge(badge))
        {
            foreach (Monster targetMonster in Bot.Monsters.CurrentAvailableMonsters)
            {
                while (!Bot.ShouldExit && !ded)
                {
                    #region cell & alive checks
                    while (!Bot.ShouldExit && !Bot.Player.Alive) Core.Sleep();
                    if (Bot.Player.Cell != "r3")
                    {
                        Core.Jump("r3", "Left");
                        Bot.Wait.ForCellChange("r3");
                        Core.Sleep();
                    }
                    #endregion
                    if (Core.HasWebBadge(badge))
                    {
                        Core.JumpWait();
                        break;
                    }
                    Bot.Combat.Attack(targetMonster);
                    Bot.Sleep(200);
                }

                if (Core.HasWebBadge(badge))
                {
                    Bot.Options.AggroMonsters = false;
                    Core.JumpWait();
                    break;
                }
            }
        }

        // Unsubscribe properly
        Bot.Events.MonsterKilled -= OnMonsterKilled;
    }
    private string badge = "Touch Mass";


    private void SetAdditonOptions()
    {
        // Ensure options are enabled.. map laggy af?
        Core.Logger("Enabling LagKiller");
        Bot.Options.LagKiller = true;
        Core.Sleep();

        Core.Logger("Setting FPS to 10");
        Bot.Options.SetFPS = 10;
        Core.Sleep();

        Core.Logger("Setting Custom Name to 'AE made this quest for botters'");
        Bot.Options.CustomName = "AE made this quest for botters";
        Bot.Options.CustomGuild = $"🖕";
        Core.Sleep();

        Core.Logger("Accepting AC Drops");
        Bot.Options.AcceptACDrops = true;
        Core.Sleep();

        Core.Logger("Freezing monster positions");
        Bot.Lite.FreezeMonsterPosition = true;
        Core.Sleep();

        Core.Logger("Enabling Custom Drops UI");
        Bot.Lite.CustomDropsUI = true;
        Core.Sleep();

        Core.Logger("Disabling Red Warning");
        Bot.Lite.DisableRedWarning = true;
        Core.Sleep();

        Core.Logger("Disabling Self Animation");
        Bot.Lite.DisableSelfAnimation = true;
        Core.Sleep();

        Core.Logger("Disabling Skill Animation");
        Bot.Lite.DisableSkillAnimation = true;
        Core.Sleep();

        Core.Logger("Disabling Weapon Animation");
        Bot.Lite.DisableWeaponAnimation = true;
        Core.Sleep();

        Core.Logger("Disabling Monster Animation");
        Bot.Lite.DisableMonsterAnimation = true;
        Core.Sleep();

        Core.Logger("Disabling Damage Strobe");
        Bot.Lite.DisableDamageStrobe = true;
        Core.Sleep();

    }
}



