/*
name: Kathoolv3 (No Option)
description: Kill God of the Depths for Sacrosanct Morsel (Frenzy Feast daily). No config needed.
*/
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreEngine2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreUltra2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraEnhancements.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraPotions.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraGeneral.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraCustomClassSync.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraWaitForArmy.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/GetScrolls.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreAdvanced2.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Skua.Core.Interfaces;

public class Kathoolv3
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
    private static GetScrolls Scrolls => _Scrolls ??= new GetScrolls();
    private static GetScrolls _Scrolls;

    private const string Dps1 = "Verus DoomKnight";
    private const string Dps2 = "King's Echo";
    private const string Dps3 = "Lord Of Order";
    private const string Dps4 = "StoneCrusher";

    private static readonly string[][] UltraClassesByRole =
    {
        new[] { Dps1 },
        new[] { Dps2 },
        new[] { Dps3 },
        new[] { Dps4 }
    };

    public void ScriptMain(IScriptInterface bot)
    {
        RunBoss();
        Bot.StopSync();
    }

    public void RunBoss()
    {
        C.SetOptions(disableCoreSkills: true);
        Engine.Boot();

        Bot.Flash.FlashCall += KathoolFlashListener;
        try
        {
            Prep();
            Fight();
        }
        finally
        {
            Bot.Flash.FlashCall -= KathoolFlashListener;
            Engine.DisableSkills();
            C.SetOptions(false);
        }
    }

    private void EquipPresetClasses()
    {
        int armySize = 4;
        bool allowDuplicates = armySize > UltraClassesByRole.Length;

        C.Logger($"[Kathoolv3] Equipping DPS classes for army size {armySize}.");
        string[][] classSlots = new string[armySize][];

        for (int i = 0; i < armySize; i++)
        {
            classSlots[i] = i < UltraClassesByRole.Length ? UltraClassesByRole[i] : UltraClassesByRole[0];
        }

        UltraCustomClassSync.CustomClassSync(Ultra, Bot, classSlots, armySize, "kathoolv3_class.sync", allowDuplicates);
    }

    private void Prep()
    {
        UltraGeneral.EquipWarriorClass();
        Bot.Sleep(2000);
        EquipPresetClasses();
        Bot.Sleep(2000);

        Enh.Apply();

        Scrolls.BuyVigil();
        Bot.Sleep(2500);
    }

    private async Task UseVigilAsync()
    {
        for (int i = 0; i < 60; i++)
        {
            Engine.Cast(5);
            await Task.Delay(50);
        }
    }

    private void KathoolFlashListener(string name, object[] args)
    {
        try
        {
            if (name != "packetFromServer")
                return;

            dynamic? data = null;
            var packet = JsonConvert.DeserializeObject<dynamic>((string)args[0])!;
            data = packet?["b"]?["o"];

            if (data == null || data["cmd"]?.ToString() != "ct")
                return;

            if (data["anims"] != null)
            {
                foreach (var anim in data["anims"])
                {
                    if (anim?.msg != null)
                    {
                        string msg = (string)anim.msg;
                        if (msg.Contains("You cannot resist.", StringComparison.OrdinalIgnoreCase))
                        {
                            C.Logger("[Kathool] Detected 'You cannot resist.' — using Vigil.");
                            _ = UseVigilAsync();
                        }
                    }
                }
            }
        }
        catch { }
    }

    private void Fight()
    {
        const string map = "kathooldepths";
        const string boss = "God of the Depths";
        const string bossDefeatedTemp = "Sacrosanct Morsel";

        const string waitSyncFile = "kathoolv3.sync";
        const string completionSyncFile = "Kathoolv3Completion.sync";
        int armySize = 4;

        const int questId = 9350;

        if (!UltraGeneral.IsQuestGreen(Bot, questId))
            UltraGeneral.EnsureAcceptOnce(Bot, questId);

        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(completionSyncFile));

        const int potionQuant = 10;
        Pots.EnsureRecommendedPotions(potionQuant, skipThird: true);

        Scrolls.BuyVigil();

        C.Join("Whitemap");
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: false);

        Pots.UseRecommendedPotions(potionQuant, skipThird: true, ensureStock: false);
        Scrolls.EquipVigil();

        Engine.Join(map);
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: true);

        Engine.ChooseBestCell(boss);
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
                Engine.Join(map);
                Ultra.PersistentJoinHouse();
                UltraGeneral.CompleteQuest(Bot, questId);
                Bot.Sleep(3000);
                break;
            }

            // Target priority: Tendril (MapID 3) > Tendril (MapID 1) > God of the Depths (MapID 2)
            int[] priority = { 3, 1, 2 };
            int targetMapId = 0;
            foreach (int mapId in priority)
            {
                var mon = Bot.Monsters.MapMonsters.FirstOrDefault(m => m != null && m.MapID == mapId && m.Alive);
                if (mon != null)
                {
                    targetMapId = mapId;
                    break;
                }
            }
            if (targetMapId > 0 && (Bot.Player.Target?.MapID != targetMapId || !Bot.Player.HasTarget))
                Bot.Combat.Attack(targetMapId);

            Pots.ActivateEquippedPotion();
            Bot.Sleep(500);
        }
    }
}
