/*
name: ArmyLR
description: use an army strategy to aquire Legion Revenant
tags: legion, legion rev, legion revenant, revenant, army
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/Legion/CoreLegion.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class ArmyLR
{
    #region  IgnoreME
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
    public CoreLegion CLR = new();
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
    #endregion

    public string OptionsStorage = "Fill me in";
    public bool DontPreconfigure = true;

    public string[] LRMaterials =
    {
        "Exalted Crown",
        "Revenant's Spellscroll",
        "Conquest Wreath",
        "Legion Revenant",
    };
    public string[] LF1 =
    {
        "Aeacus Empowered",
        "Tethered Soul",
        "Darkened Essence",
        "Dracolich Contract",
    };
    public string[] LF2 =
    {
        "Grim Cohort Conquered",
        "Ancient Cohort Conquered",
        "Pirate Cohort Conquered",
        "Battleon Cohort Conquered",
        "Mirror Cohort Conquered",
        "Darkblood Cohort Conquered",
        "Vampire Cohort Conquered",
        "Spirit Cohort Conquered",
        "Dragon Cohort Conquered",
        "Doomwood Cohort Conquered",
    };
    public string[] LF3 =
    {
        "Hooded Legion Cowl",
        "Legion Token",
        "Dage's Favor",
        "Emblem of Dage",
        "Diamond Token of Dage",
        "Dark Token",
    };

    // Add / remove players below, to get to how ever many is the map's cap... or leave it alone, doesn't matter.
    public List<IOption> Options = new()
    {
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

    public void ScriptMain(IScriptInterface Bot)
    {
        C.BankingBlackList.AddRange(new[] { "add", "any", "drops", "needed" });
        C.BankingBlackList.AddRange(LRMaterials);
        C.BankingBlackList.AddRange(LF1);
        C.BankingBlackList.AddRange(LF2);
        C.BankingBlackList.AddRange(LF3);
        C.SetOptions(disableClassSwap: true);

        C.PrivateRooms = true;
        C.PrivateRoomNumber = Army.getRoomNr();

        CLR.JoinLegion();
        GetLR();

        C.SetOptions(false);
    }

    void LF1Items(int quant = 0)
    {
        while (!Bot.ShouldExit)
        {
            if (
                Ultra.CheckArmyProgress(
                    "Revenant's Spellscroll",
                    quant,
                    false,
                    "Revenants_Spellscroll.Sync"
                )
            )
            {
                Bot.Wait.ForPickup("Revenant's Spellscroll");
                Bot.Options.AggroMonsters = false;
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                break;
            }

            C.EnsureAccept(6897);
            Bot.Quests.UpdateQuest(2060);
            ArmyHandler(
                map: "judgement",
                QuestIDs: Array.Empty<int>(),
                WaitForArmysyncPath: "judgement_items",
                AggroCell: "r10a",
                checkType: CheckType.Item,
                Itemname: "Aeacus Empowered",
                quant: 50,
                UseBool: false
            );

            ArmyHandler(
                map: "revenant",
                QuestIDs: Array.Empty<int>(),
                WaitForArmysyncPath: "LF1_revenant",
                AggroCell: "r2",
                checkType: CheckType.Item,
                Itemname: "Tethered Soul",
                quant: 300,
                UseBool: false
            );

            ArmyHandler(
                map: "shadowrealmpast",
                QuestIDs: Array.Empty<int>(),
                WaitForArmysyncPath: "LF1_shadowrealmpast",
                AggroCell: "Enter",
                checkType: CheckType.Item,
                Itemname: "Darkened Essence",
                quant: 500,
                UseBool: false
            );

            ArmyHandler(
                map: "necrodungeon",
                QuestIDs: Array.Empty<int>(),
                WaitForArmysyncPath: "LF1_necrodungeon",
                AggroCell: "r22",
                checkType: CheckType.Item,
                Itemname: "Dracolich Contract",
                quant: 1000,
                UseBool: false
            );
            C.EnsureComplete(6897);
            Bot.Wait.ForPickup("Revenant's Spellscroll");
        }
    }

    void LF2Items(int quant = 0)
    {
        while (!Bot.ShouldExit)
        {
            if (Ultra.CheckArmyProgress("Conquest Wreath", quant, false, "Conquest_Wreath.Sync"))
            {
                Bot.Wait.ForPickup("Conquest Wreath");
                Bot.Options.AggroMonsters = false;
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                break;
            }
            C.EnsureAccept(6898);
            ArmyHandler(
                map: "doomvault",
                QuestIDs: Array.Empty<int>(),
                WaitForArmysyncPath: "LF2_doomvault",
                AggroCell: "r1",
                checkType: CheckType.Item,
                Itemname: "Grim Cohort Conquered",
                quant: 400,
                UseBool: false
            );

            ArmyHandler(
                map: "mummies",
                QuestIDs: Array.Empty<int>(),
                WaitForArmysyncPath: "LF2_mummies",
                AggroCell: "Enter",
                checkType: CheckType.Item,
                Itemname: "Ancient Cohort Conquered",
                quant: 400,
                UseBool: false
            );

            ArmyHandler(
                map: "wrath",
                QuestIDs: Array.Empty<int>(),
                WaitForArmysyncPath: "LF2_wrath",
                AggroCell: "r2",
                checkType: CheckType.Item,
                Itemname: "Pirate Cohort Conquered",
                quant: 400,
                UseBool: false
            );

            ArmyHandler(
                map: "doomwar",
                QuestIDs: Array.Empty<int>(),
                WaitForArmysyncPath: "LF2_doomwar",
                AggroCell: "r6",
                checkType: CheckType.Item,
                Itemname: "Battleon Cohort Conquered",
                quant: 400,
                UseBool: false
            );

            ArmyHandler(
                map: "overworld",
                QuestIDs: Array.Empty<int>(),
                WaitForArmysyncPath: "LF2_overworld",
                AggroCell: "Enter",
                checkType: CheckType.Item,
                Itemname: "Mirror Cohort Conquered",
                quant: 400,
                UseBool: false
            );

            ArmyHandler(
                map: "deathpits",
                QuestIDs: Array.Empty<int>(),
                WaitForArmysyncPath: "LF2_deathpits",
                AggroCell: "r1",
                checkType: CheckType.Item,
                Itemname: "Darkblood Cohort Conquered",
                quant: 400,
                UseBool: false
            );

            ArmyHandler(
                map: "maxius",
                QuestIDs: Array.Empty<int>(),
                WaitForArmysyncPath: "LF2_maxius",
                AggroCell: "r4",
                checkType: CheckType.Item,
                Itemname: "Vampire Cohort Conquered",
                quant: 400,
                UseBool: false
            );

            ArmyHandler(
                map: "curseshore",
                QuestIDs: Array.Empty<int>(),
                WaitForArmysyncPath: "LF2_curseshore",
                AggroCell: "Enter",
                checkType: CheckType.Item,
                Itemname: "Spirit Cohort Conquered",
                quant: 400,
                UseBool: false
            );

            ArmyHandler(
                map: "dragonbone",
                QuestIDs: Array.Empty<int>(),
                WaitForArmysyncPath: "LF2_dragonbone",
                AggroCell: "r2",
                checkType: CheckType.Item,
                Itemname: "Dragon Cohort Conquered",
                quant: 400,
                UseBool: false
            );

            ArmyHandler(
                map: "doomwood",
                QuestIDs: Array.Empty<int>(),
                WaitForArmysyncPath: "LF2_doomwood",
                AggroCell: "r3",
                checkType: CheckType.Item,
                Itemname: "Doomwood Cohort Conquered",
                quant: 400,
                UseBool: false
            );

            Bot.Wait.ForPickup("Conquest Wreath");
            C.EnsureComplete(6898);
        }
    }

    void LF3Items(int quant = 0)
    {
        Core.Join("whitemap");
        C.FarmingLogger("Exalted Crown", quant);
        // Core.RegisterQuests(6899);
        C.AddDrop(LF3);
        while (!Bot.ShouldExit)
        {
            C.EnsureAccept(6899);
            if (Ultra.CheckArmyProgress("Exalted Crown", quant, false, "ExalteCrown.Sync"))
            {
                Bot.Wait.ForPickup("Exalted Crown");
                Bot.Options.AggroMonsters = false;
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                break;
            }

            Adv.BuyItem("underworld", 216, "Hooded Legion Cowl");
            ArmyDarkToken(100);
            ArmyLegionTokens(4000);
            if (Bot.Quests.CanComplete(6899))
                Bot.Quests.Complete(6899);
        }
        C.CancelRegisteredQuests();
    }

    void GetLR()
    {
        C.AddDrop("Legion Revenant");
        C.EnsureAccept(6900);
        LF1Items(20);
        LF2Items(6);
        LF3Items(10);
        C.EnsureComplete(6900);
        Bot.Wait.ForPickup("Legion Revenant");
    }

    void DagesFavor(int quant)
    {
        ArmyHandler(
            map: "evilwarnul",
            QuestIDs: Array.Empty<int>(),
            WaitForArmysyncPath: "LF4_dagefavor",
            AggroCell: "r2",
            checkType: CheckType.Item,
            Itemname: "Dage's Favor",
            quant: quant,
            UseBool: false
        );
    }

    void ArmyEmblemOfDage(int quant)
    {
        ArmyHandler(
            map: "shadowblast",
            QuestIDs: new int[] { 4742 },
            WaitForArmysyncPath: "emblem_dage",
            AggroCell: "r12",
            checkType: CheckType.Item,
            Itemname: "Emblem of Dage",
            quant: quant,
            UseBool: false
        );
    }

    void ArmyLegionTokens(int quant)
    {
        ArmyHandler(
            map: "dreadrock",
            QuestIDs: new int[] { 4849 },
            WaitForArmysyncPath: "legion_tokens",
            AggroCell: "r3",
            checkType: CheckType.Item,
            Itemname: "Legion Token",
            quant: quant,
            UseBool: false
        );
    }

    void ArmyDarkToken(int quant)
    {
        ArmyHandler(
            map: "seraphicwardage",
            QuestIDs: new int[] { 6248, 6249, 6251 },
            WaitForArmysyncPath: "dark_token",
            AggroCell: "r3",
            checkType: CheckType.Item,
            Itemname: "Dark Token",
            quant: quant,
            UseBool: false
        );
    }

    private void ArmyHandler(
        string map,
        int[] QuestIDs,
        string WaitForArmysyncPath,
        string AggroCell,
        CheckType checkType,
        string? Itemname = null,
        int quant = 0,
        bool UseBool = false,
        Func<bool>? condition = null
    )
    {
        // Sync file used to keep track of what accs are done.
        string syncPath = Ultra.ResolveSyncPath(WaitForArmysyncPath);
        Ultra.ClearSyncFile(WaitForArmysyncPath);
        Bot.Sleep(2500);

        // Log Players in current army.
        C.Logger($"Players in Curreny Army: {sArmy.Players().Length}");

        if (QuestIDs.Length > 0)
            C.RegisterQuests(QuestIDs);

        if (Itemname != null)
            C.AddDrop(Itemname);

        if (map == "revenant")
        {
            C.GhostItem(47465, "Revenant Map Bypass", 1, false, category: Skua.Core.Models.Items.ItemCategory.Class, "Used to bypass the dark caster class requirement for the map \"Revenant\"");
            RevenantMapHandler();
        }
        Core.Join(map);

        C.Jump(AggroCell, "Left");

        // Don't Touch vv
        if (sArmy.Players().Length > 1)
            // Dont make this the same as the syncPath
            Ultra.WaitForArmy(sArmy.Players().Length - 1, WaitForArmysyncPath);
        Bot.Player.SetSpawnPoint();
        Bot.Sleep(1500);
        Bot.Options.AggroMonsters = true;
        // Pick a variant below ( multiple can be used as long as the sync files are different.)

        if (UseBool && condition != null)
        {
            // Bool variant
            while (!Bot.ShouldExit)
            {
                // Replace the `Bot.Player.Level >= 100` below with the bool
                // you want want all accs to have true, leave the rest of this alone.
                if (Ultra.CheckArmyProgressBool(condition, syncPath))
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
            return;
        }

        if (Itemname != null)
        {
            //Int variant
            while (!Bot.ShouldExit)
            {
                // Replace `Itemname` with the wanted item
                // Replace the 500 with the quantity you desire
                // Replace `false` if the item is a temp item with `true` or leave as `false` for non-temp items.
                if (Ultra.CheckArmyProgress(Itemname, quant, false, syncPath))
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
            return;
        }
    }

    int _revenantBaseRoom = -1;

    void RevenantMapHandler()
    {
        string[] players = Army.Players();
        if (players == null || players.Length == 0)
            return;

        if (_revenantBaseRoom == -1)
            _revenantBaseRoom = C.PrivateRoomNumber; // lock base room once

        string currentPlayer = Bot.Player.Username;
        int playerIndex = Array.IndexOf(players, currentPlayer);
        if (playerIndex < 0)
            return;

        int roomOffset = playerIndex / 3; // 3 players per room

        C.PrivateRoomNumber = _revenantBaseRoom + roomOffset;
    }



    enum CheckType
    {
        Bool = 1,
        Item = 2,
    }
}
