/*
name: ArmyLR
description: use an army strategy to aquire Legion Revenant
tags: legion, legion rev, legion revenant, revenant, army
*/

//cs_include Scripts/Ultrasv3/Entwined Eclipse/CoreEngine.cs
//cs_include Scripts/Ultrasv3/Entwined Eclipse/CoreUltra.cs
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
            if (Ultra.CheckArmyProgressBool(() => C.CheckInventory("Revenant's Spellscroll", quant), "Revenants_Spellscroll.Sync"))
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
                QuestIDs: [],
                WaitForArmysyncPath: "judgement_items",
                AggroCell: "r10a",
                checkType: CheckType.Item,
                Itemname: "Aeacus Empowered",
                quant: 50
            );

            ArmyHandler(
                map: "revenant",
                QuestIDs: [],
                WaitForArmysyncPath: "LF1_revenant",
                AggroCell: "r2",
                checkType: CheckType.Item,
                Itemname: "Tethered Soul",
                quant: 300
            );

            ArmyHandler(
                map: "shadowrealmpast",
                QuestIDs: [],
                WaitForArmysyncPath: "LF1_shadowrealmpast",
                AggroCell: "Enter",
                checkType: CheckType.Item,
                Itemname: "Darkened Essence",
                quant: 500
            );

            ArmyHandler(
                map: "necrodungeon",
                QuestIDs: [],
                WaitForArmysyncPath: "LF1_necrodungeon",
                AggroCell: "r22",
                checkType: CheckType.Item,
                Itemname: "Dracolich Contract",
                quant: 1000
            );
            C.EnsureComplete(6897);
            Bot.Wait.ForPickup("Revenant's Spellscroll");
        }
    }

    void LF2Items(int quant = 0)
    {
        while (!Bot.ShouldExit)
        {
            if (Ultra.CheckArmyProgressBool(() => C.CheckInventory("Conquest Wreath", quant), "Conquest_Wreath.Sync"))
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
                QuestIDs: [],
                WaitForArmysyncPath: "LF2_doomvault",
                AggroCell: "r1",
                checkType: CheckType.Item,
                Itemname: "Grim Cohort Conquered",
                quant: 400
            );

            ArmyHandler(
                map: "mummies",
                QuestIDs: [],
                WaitForArmysyncPath: "LF2_mummies",
                AggroCell: "Enter",
                checkType: CheckType.Item,
                Itemname: "Ancient Cohort Conquered",
                quant: 400
            );

            ArmyHandler(
                map: "wrath",
                QuestIDs: [],
                WaitForArmysyncPath: "LF2_wrath",
                AggroCell: "r2",
                checkType: CheckType.Item,
                Itemname: "Pirate Cohort Conquered",
                quant: 400
            );

            ArmyHandler(
                map: "doomwar",
                QuestIDs: [],
                WaitForArmysyncPath: "LF2_doomwar",
                AggroCell: "r6",
                checkType: CheckType.Item,
                Itemname: "Battleon Cohort Conquered",
                quant: 400
            );

            ArmyHandler(
                map: "overworld",
                QuestIDs: [],
                WaitForArmysyncPath: "LF2_overworld",
                AggroCell: "Enter",
                checkType: CheckType.Item,
                Itemname: "Mirror Cohort Conquered",
                quant: 400
            );

            ArmyHandler(
                map: "deathpits",
                QuestIDs: [],
                WaitForArmysyncPath: "LF2_deathpits",
                AggroCell: "r1",
                checkType: CheckType.Item,
                Itemname: "Darkblood Cohort Conquered",
                quant: 400
            );

            ArmyHandler(
                map: "maxius",
                QuestIDs: [],
                WaitForArmysyncPath: "LF2_maxius",
                AggroCell: "r4",
                checkType: CheckType.Item,
                Itemname: "Vampire Cohort Conquered",
                quant: 400
            );

            ArmyHandler(
                map: "curseshore",
                QuestIDs: [],
                WaitForArmysyncPath: "LF2_curseshore",
                AggroCell: "Enter",
                checkType: CheckType.Item,
                Itemname: "Spirit Cohort Conquered",
                quant: 400
            );

            ArmyHandler(
                map: "dragonbone",
                QuestIDs: [],
                WaitForArmysyncPath: "LF2_dragonbone",
                AggroCell: "r2",
                checkType: CheckType.Item,
                Itemname: "Dragon Cohort Conquered",
                quant: 400
            );

            ArmyHandler(
                map: "doomwood",
                QuestIDs: [],
                WaitForArmysyncPath: "LF2_doomwood",
                AggroCell: "r3",
                checkType: CheckType.Item,
                Itemname: "Doomwood Cohort Conquered",
                quant: 400
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
            if (Ultra.CheckArmyProgressBool(() => C.CheckInventory("Exalted Crown", quant), "Exalted_Crown.Sync"))
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
            DiamondTokenofDage(30);
            ArmyEmblemOfDage(1);
            if (Bot.Quests.CanComplete(6899))
            {
                Bot.Quests.Complete(6899);
                Bot.Wait.ForPickup("Exalted Crown");
            }
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
            QuestIDs: [],
            WaitForArmysyncPath: "LF4_dagefavor",
            AggroCell: "r2",
            checkType: CheckType.Item,
            Itemname: "Dage's Favor",
            quant: quant

        );
    }

    void DiamondTokenofDage(int quant = 300)
    {
        if (C.CheckInventory("Diamond Token of Dage", quant))
            return;

        if (!C.CheckInventory("Legion Round 4 Medal"))
            CLR.LegionRound4Medal();
        if (!C.CheckInventory("Legion Token", 50))
            // farm via CLR due to it just being a requirement dont wait for others.. 
            CLR.FarmLegionToken(50);

        C.FarmingLogger("Diamond Token of Dage", quant);
        C.AddDrop("Diamond Token of Dage", "Legion Token");
        C.RegisterQuests(4743);

        while (!Bot.ShouldExit && !C.CheckInventory("Diamond Token of Dage", quant))
        {
            ArmyHandler(
                map: "tercessuinotlim",
                QuestIDs: [],
                WaitForArmysyncPath: "Makai",
                AggroCell: "m2",
                checkType: CheckType.Item,
                Itemname: "Defeated Makai",
                quant: 25
            );

            ArmyHandler(
                map: "aqlesson",
                QuestIDs: [],
                WaitForArmysyncPath: "Carnax",
                AggroCell: "Frame9",
                checkType: CheckType.Item,
                Itemname: "Carnax Eye",
                quant: 1
            );

            ArmyHandler(
                map: "deepchaos",
                QuestIDs: [],
                WaitForArmysyncPath: "Kathool",
                AggroCell: "Frame4",
                checkType: CheckType.Item,
                Itemname: "Kathool Tentacle",
                quant: 1
            );

            ArmyHandler(
                map: "lair",
                QuestIDs: [],
                WaitForArmysyncPath: "Red_Dragon",
                AggroCell: "End",
                checkType: CheckType.Item,
                Itemname: "Red Dragon's Fang",
                quant: 1
            );

            ArmyHandler(
                map: "bloodtitan",
                QuestIDs: [],
                WaitForArmysyncPath: "Blood_Titan",
                AggroCell: "Enter",
                checkType: CheckType.Item,
                Itemname: "Blood Titan's Blade",
                quant: 1
            );

            ArmyHandler(
                map: "dflesson",
                QuestIDs: [],
                WaitForArmysyncPath: "r12",
                AggroCell: "Right",
                checkType: CheckType.ItemID,
                Itemname: null,
                // Fluffy's Bones
                ItemID: 33257,
                quant: 1
            );
        }
        C.CancelRegisteredQuests();
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
            quant: quant

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
            quant: quant

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
            quant: quant

        );
    }

    private void ArmyHandler(
        string map,
        int[] QuestIDs,
        string WaitForArmysyncPath,
        string AggroCell,
        CheckType checkType,
        string? Itemname = null,
        int? ItemID = null,
        int quant = 0,
        Func<bool>? condition = null
    )
    {
        // Sync file used to keep track of what accs are done.
        string syncPath = Ultra.ResolveSyncPath(WaitForArmysyncPath);
        Ultra.ClearSyncFile(WaitForArmysyncPath);
        Bot.Sleep(2500);

        C.Logger($"Players in Curreny Army: {sArmy.Players().Length}");

        if (QuestIDs.Length > 0)
            C.RegisterQuests(QuestIDs);

        if (Itemname != null)
            C.AddDrop(Itemname);
        else if (ItemID != null)
            C.AddDrop((int)ItemID);

        if (map == "revenant")
        {
            C.GhostItem(47465, "Revenant Map Bypass", 1, false, category: Skua.Core.Models.Items.ItemCategory.Class, "Used to bypass the dark caster class requirement for the map \"Revenant\"");
            RevenantMapHandler();
        }
        if (map == "mummies")
            Bot.Quests.UpdateQuest(4614);

        Core.Join(map);
        C.Jump(AggroCell, "Left");

        if (sArmy.Players().Length > 1)
            Ultra.WaitForArmy(sArmy.Players().Length - 1, WaitForArmysyncPath);
        Bot.Player.SetSpawnPoint();
        Bot.Sleep(1500);
        Bot.Options.AggroMonsters = true;

        string syncKey = checkType switch
        {
            CheckType.Item => $"{Itemname}.Sync",
            CheckType.ItemID => $"{ItemID}.Sync",
            CheckType.Bool => $"{WaitForArmysyncPath}.Sync",
            _ => $"{WaitForArmysyncPath}.Sync"
        };

        Func<bool> progressCheck = checkType switch
        {
            CheckType.Item when Itemname != null => () => C.CheckInventory(Itemname, quant),
            CheckType.ItemID when ItemID != null => () => C.CheckInventory((int)ItemID, quant),
            CheckType.Bool when condition != null => condition,
            _ => throw new ArgumentException($"ArmyHandler: checkType {checkType} requires a matching Itemname/ItemID/condition argument.")
        };

        while (!Bot.ShouldExit)
        {
            if (Ultra.CheckArmyProgressBool(progressCheck, syncKey))
            {
                Bot.Options.AggroMonsters = false;
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                break;
            }

            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            Bot.Combat.Attack("*");
            Bot.Sleep(500);
        }
    }


    int _revenantBaseRoom = -1;
    const int RevenantBaseRoomDefault = 1; // pin explicitly instead of trusting live C.PrivateRoomNumber

    bool _rosterCheckedThisSession = false;

    void RevenantMapHandler()
    {
        string[] players = Army.Players();
        if (players == null || players.Length == 0)
            return;

        if (_revenantBaseRoom == -1)
            _revenantBaseRoom = RevenantBaseRoomDefault;

        if (string.IsNullOrEmpty(Bot.Player.Username))
            return; // not logged in yet, nothing to do

        // Must match the normalization Players() applies to its entries
        // (ToLower + Trim) or IndexOf silently fails and the room never updates.
        string currentPlayer = Bot.Player.Username.ToLower().Trim();

        // One-time startup sanity check: catch typos/mismatches in the shared
        // config immediately and loudly, instead of discovering it later as
        // "this bot never left the base room."
        if (!_rosterCheckedThisSession)
        {
            _rosterCheckedThisSession = true;

            if (!players.Contains(currentPlayer))
            {
                Bot.Log($"[RevenantMap] WARNING: '{currentPlayer}' not found in configured roster " +
                        $"[{string.Join(", ", players)}]. Check the playerN entries in the shared cfg " +
                        $"for typos, extra whitespace, or a missing entry for this account. " +
                        $"This bot will NOT be assigned a room until this is fixed.");
            }
            else
            {
                Bot.Log($"[RevenantMap] Roster check OK: '{currentPlayer}' found at index " +
                        $"{Array.IndexOf(players, currentPlayer)} of {players.Length}.");
            }
        }

        int playerIndex = Array.IndexOf(players, currentPlayer);
        if (playerIndex < 0)
            return; // already logged above; nothing more to do until config is fixed

        int roomOffset = playerIndex / 3; // 3 players per room
        int targetRoom = _revenantBaseRoom + roomOffset;

        if (C.PrivateRoomNumber != targetRoom)
            C.PrivateRoomNumber = targetRoom;
    }



    enum CheckType
    {
        Bool = 1,
        Item = 2,
        ItemID = 3
    }
}
