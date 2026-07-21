/*
name: QueenIonav3 (No Option)
description: Queen Iona v3 — no config needed, automatically uses enhancements and potions.
*/
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreEnginev3.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreUltrav3.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraEnhancements.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraPotions.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraGeneral.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraWaitForArmy.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Skua.Core.Interfaces;

public class QueenIonaNoOpv3
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots C => CoreBots.Instance;
    private static CoreEnginev3 Engine => CoreEnginev3.Instance;
    private static CoreUltrav3 Ultra => _Ultra ??= new CoreUltrav3();
    private static CoreUltrav3 _Ultra;
    private static UltraEnhancements Enh => _Enh ??= new UltraEnhancements();
    private static UltraEnhancements _Enh;
    private static UltraPotions Pots => _Pots ??= new UltraPotions();
    private static UltraPotions _Pots;
    private static string _fbsMuteFile = "";

    public void ScriptMain(IScriptInterface bot)
    {
        RunBoss();
        Bot.StopSync();
    }

    public void RunBoss()
    {
        C.SetOptions();
        _fbsMuteFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Skua", "fbs_mute.sync"
        );
        try { File.WriteAllText(_fbsMuteFile, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()); } catch { }

        try
        {
            Prep();
            Fight();
        }
        finally
        {
            try { if (File.Exists(_fbsMuteFile)) File.Delete(_fbsMuteFile); } catch { }
            C.SetOptions(false);
        }
    }

    private void Prep()
    {
        UltraGeneral.EquipWarriorClass();
        Bot.Sleep(2000);
        C.Equip("King's Echo");
        Bot.Sleep(2000);

        Enh.Apply();

        Bot.Sleep(2500);
    }

    private void Fight()
    {
        const string map = "queeniona";
        const string boss = "Queen Iona";
        const string item = "Lightning Diadem";
        const int armySize = 4;
        const string waitSyncFile = "queeniona.sync";

        const int questId = 9852;

        if (!Bot.Quests.IsDailyComplete(questId))
            C.EnsureAccept(questId);

        const int potionQuant = 10;
        Pots.EnsureRecommendedPotions(potionQuant, skipThird: false);

        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: false);

        Pots.UseRecommendedPotions(potionQuant, skipThird: false, ensureStock: false);

        C.Join("Whitemap");
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: false);

        C.Join(map);
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: true);

        Engine.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();

        Bot.Events.ExtensionPacketReceived += QueenIonaListener;

        while (!Bot.ShouldExit)
        {
            // Refresh mute file so FBS plugin stays muted during the fight
            try { File.WriteAllText(_fbsMuteFile, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()); } catch { }

            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Bot.TempInv.Contains(item, 1))
            {
                C.Logger("Queen Iona defeated. Finishing quest.");
                Bot.Events.ExtensionPacketReceived -= QueenIonaListener;
                C.Join(map);
                Ultra.PersistentJoinHouse();
                UltraGeneral.CompleteQuest(Bot, questId);
                Bot.Sleep(3000);
                break;
            }

            if (!Bot.Player.HasTarget)
                Bot.Combat.Attack("*");

            Pots.ActivateEquippedPotion();
            Bot.Sleep(500);
        }

        Bot.Events.ExtensionPacketReceived -= QueenIonaListener;
    }

    private async void QueenIonaListener(dynamic packet)
    {
        if (packet?["params"]?.type?.ToString() != "json")
            return;

        if (!Bot.Player.Alive)
            return;

        dynamic data = packet["params"].dataObj;
        if (data?.cmd?.ToString() != "event")
            return;

        string? zoneSet = data?.args?.zoneSet?.ToString();
        if (string.IsNullOrEmpty(zoneSet))
            return;

        // Wait until a charge aura appears
        string? chargeAura = null;
        while (!Bot.ShouldExit)
        {
            // Refresh mute file so FBS plugin stays muted during the fight
            try { File.WriteAllText(_fbsMuteFile, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()); } catch { }

            if (!Bot.Player.Alive)
                return;

            chargeAura = Bot.Self?.Auras?.FirstOrDefault(a => a != null &&
                          (a?.Name == "Positive Charge" || a?.Name == "Negative Charge" ||
                           a?.Name == "Positive Charge?" || a?.Name == "Negative Charge?"))?.Name;

            if (!string.IsNullOrEmpty(chargeAura))
                break;

            await Task.Delay(100);
        }

        if (string.IsNullOrEmpty(chargeAura))
            return;

        (int x, int y) zoneA = (373, 447);
        (int x, int y) zoneB = (569, 442);

        bool inverted = chargeAura.EndsWith("?");
        (int x, int y) target;

        if (!inverted)
        {
            target = chargeAura == "Positive Charge"
                ? (zoneSet.Equals("A", StringComparison.OrdinalIgnoreCase) ? zoneB : zoneA)
                : (zoneSet.Equals("A", StringComparison.OrdinalIgnoreCase) ? zoneA : zoneB);
        }
        else
        {
            target = chargeAura == "Positive Charge?"
                ? (zoneSet.Equals("A", StringComparison.OrdinalIgnoreCase) ? zoneA : zoneB)
                : (zoneSet.Equals("A", StringComparison.OrdinalIgnoreCase) ? zoneB : zoneA);
        }

        try
        {
            await Task.Run(() => Bot.Player.WalkTo(target.x, target.y));
        }
        catch (Exception ex)
        {
            C.Logger($"[QueenIona] WalkTo failed: {ex.Message}");
        }
    }
}
