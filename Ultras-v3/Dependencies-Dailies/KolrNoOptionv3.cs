/*
name: Kolrv3 (No Option)
description: Farm Kolr, Usurper of Flames for Choronzonite. No config needed.
*/
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreEngine2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreUltra2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraEnhancements.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraPotions.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraGeneral.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraCustomClassSync.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraWaitForArmy.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreAdvanced2.cs
using System;
using System.Linq;
using Skua.Core.Interfaces;

public class Kolrv3
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots2 C => CoreBots2.Instance;
    private static CoreEngine2 Engine => CoreEngine2.Instance;
    private static CoreUltra2 Ultra => _Ultra ??= new CoreUltra2();
    private static CoreUltra2 _Ultra;
    private static UltraEnhancements Enh => _Enh ??= new UltraEnhancements();
    private static UltraEnhancements _Enh;
    private static UltraPotions Pots => _Pots ??= new UltraPotions();
    private static UltraPotions _Pots;

    private const string Group1Dps1 = "King's Echo";
    private const string Group1Dps2 = "Lord Of Order";
    private const string Group2Dps1 = "King's Echo";
    private const string Group2Dps2 = "Lord Of Order";

    private static readonly string[][] UltraClassesByRole =
    {
        new[] { Group1Dps1 },
        new[] { Group1Dps2 },
        new[] { Group2Dps1 },
        new[] { Group2Dps2 }
    };

    private static int _roomOffset = 0;

    public void ScriptMain(IScriptInterface bot)
    {
        RunBoss();
        Bot.StopSync();
    }

    public void RunBoss()
    {
        C.SetOptions(disableCoreSkills: true);
        Engine.Boot();

        try
        {
            Prep();
            Fight();
        }
        finally
        {
            Engine.DisableSkills();
            C.SetOptions(false);
        }
    }

    private void EquipPresetClasses()
    {
        int armySize = 4;
        bool allowDuplicates = true;

        C.Logger($"[Kolrv3] Equipping role-based ultra classes for army size {armySize}.");
        string[][] classSlots = new string[armySize][];

        for (int i = 0; i < armySize; i++)
        {
            classSlots[i] = i < UltraClassesByRole.Length ? UltraClassesByRole[i] : UltraClassesByRole[0];
        }

        UltraCustomClassSync.CustomClassSync(Ultra, Bot, classSlots, armySize, "kolrv3_class.sync", allowDuplicates);

        AssignGroup(armySize);
    }

    private void AssignGroup(int armySize)
    {
        string username = Bot.Player.Username;
        string groupSync = Ultra.ResolveSyncPath("kolrv3_group.sync");

        Ultra.UpdateEntry(groupSync, username, "1");
        Bot.Sleep(500);

        while (!Bot.ShouldExit)
        {
            string[] lines = Ultra.ReadLines(groupSync);
            var valid = lines
                .Select(l => l.Split(':')[0])
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (valid.Count >= armySize)
                break;
            Ultra.UpdateEntry(groupSync, username, "1");
            Bot.Sleep(500);
        }

        var accounts = Ultra.ReadLines(groupSync)
            .Select(l => l.Split(':')[0])
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToList();

        int myIndex = accounts.FindIndex(a => string.Equals(a, username, StringComparison.OrdinalIgnoreCase));
        int half = armySize / 2;
        _roomOffset = (myIndex / half) * 10;

        C.Logger($"[Kolrv3] Assigned to Group{(myIndex / half) + 1} (room offset: +{_roomOffset})");
    }

    private void Prep()
    {
        UltraGeneral.EquipWarriorClass();
        Bot.Sleep(2000);
        EquipPresetClasses();
        Bot.Sleep(2000);

        Enh.ApplyKolr();

        Bot.Sleep(2500);
    }

    private void Fight()
    {
        const string map = "flameusurper";
        const string boss = "Kolr, Usurper of Flames";
        const string bossDefeatedTemp = "Choronzonite";

        const string waitSyncFile = "kolrv3.sync";
        const string completionSyncFile = "Kolrv3Completion.sync";
        const int armySize = 4;

        const int questId = 10715;

        if (!UltraGeneral.IsQuestGreen(Bot, questId))
            UltraGeneral.EnsureAcceptOnce(Bot, questId);

        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(completionSyncFile));

        const int potionQuant = 10;
        Pots.EnsureRecommendedPotions(potionQuant, skipThird: false, context: "Kolr");

        C.Join("Whitemap");
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: false);

        Pots.UseRecommendedPotions(potionQuant, skipThird: false, context: "Kolr", ensureStock: false);

        C.Join($"flameusurper-{C.PrivateRoomNumber + _roomOffset}");
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: true);

        Bot.Map.Jump("r2", "Bottom");
        Bot.Player.SetSpawnPoint();
        Bot.Sleep(2000);

        // Pre-seed completion sync file
        string? _username = Bot.Player.Username;
        string? _className = Bot.Player.CurrentClass?.Name;
        if (!string.IsNullOrWhiteSpace(_username) && !string.IsNullOrWhiteSpace(_className))
        {
            string _myKey = $"{_username}|{_className}".Replace(":", "-");
            Ultra.UpdateEntry(Ultra.ResolveSyncPath(completionSyncFile), _myKey, "0");
        }

        while (!Bot.ShouldExit)
        {
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains(bossDefeatedTemp, 1), completionSyncFile))
            {
                C.Logger("Boss defeated. Finishing quest.");
                Engine.DisableSkills();
                C.Join($"flameusurper-{C.PrivateRoomNumber + _roomOffset}");
                Ultra.PersistentJoinHouse();
                UltraGeneral.CompleteQuest(Bot, questId);
                Bot.Sleep(3000);
                break;
            }

            // Default — attack the boss
            if (Bot.Player.Target?.Name != boss)
                Bot.Combat.Attack(boss);

            Pots.ActivateEquippedPotion();
            Bot.Sleep(500);
        }
    }
}
