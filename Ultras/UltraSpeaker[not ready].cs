//cs_include Scripts/Ultras/CoreUltras.cs

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Dynamic;
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraSpeaker
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreUltras Core = new();

    public void ScriptMain(IScriptInterface bot)
    {
        Core.Boot();
        Bot.Events.ExtensionPacketReceived += UltraSpeakerListener;

        Kill();

        Bot.Events.ExtensionPacketReceived -= UltraSpeakerListener;
        Bot.Stop();
    }

    // --- roles ---
    enum Role { MagiaSoaker, StasisSoaker, EqualizeA, EqualizeB, None }
    Role MyRole = Role.None; // auto-set from class on first run

    // --- class → role mapping ---
    // Change these if you prefer a different assignment.
    // Default suggestions:
    //  - StoneCrusher  -> Magia Soaker (perma-out, keeps auras up)
    //  - Legion Revenant -> Stasis Soaker (can eat periodic 6s stuns, great sustain)
    //  - ArchPaladin   -> Equalize A (durable spike soaker)
    //  - Chaos Avenger -> Equalize B (durable spike soaker)
    void AssignRoleFromClass()
    {
        if (Core.HasClassEquipped("StoneCrusher")) MyRole = Role.MagiaSoaker;
        else if (Core.HasClassEquipped("Legion Revenant")) MyRole = Role.StasisSoaker;
        else if (Core.HasClassEquipped("ArchPaladin")) MyRole = Role.EqualizeA;
        else if (Core.HasClassEquipped("Chaos Avenger")) MyRole = Role.EqualizeB;
        else MyRole = Role.EqualizeA; // fallback
    }

    // --- detectors (you provide these flags elsewhere) ---
    bool listenDetected = false; // Energy Draw charge ("You shall listen.")
    bool truthDetected = false; // Magia Draw charge ("I will make you see the truth.")
    bool equalDetected = false; // Power Split charge ("All stand equal...")

    bool prevListen = false, prevTruth = false, prevEqual = false;

    // --- state ---
    int equalCastIndex = -1; // increments on Power Split charge start

    // --- helpers ---
    bool CanEnterRedZone()
    {
        var debuffs = new List<string> { "Magia Burn", "Stasis", "Sanctity" };
        return !Core.HasAnyAura(debuffs, true);
    }

    bool IsMyTurnToSoakEqualize()
    {
        // Rotation: EqualizeA -> EqualizeB -> StasisSoaker -> (repeat)
        int slot = ((equalCastIndex % 3) + 3) % 3; // 0..2
        return (MyRole == Role.EqualizeA && slot == 0)
            || (MyRole == Role.EqualizeB && slot == 1)
            || (MyRole == Role.StasisSoaker && slot == 2);
    }

    void Taunt() => Core.UsePotion();

    void MoveOut() => Bot.Send.Packet($"%xt%zm%mv%{Bot.Map.RoomID}%881%363%10%");
    void MoveIn() => Bot.Send.Packet($"%xt%zm%mv%{Bot.Map.RoomID}%506%351%10%");

    // --- main ---
    void Kill()
    {
        Core.GetScrollOfEnrage();
        Bot.Quests.UpdateQuest(9125);
        Core.Join("ultraspeaker");
        Core.WaitForArmy(3);
        Core.ChooseBestCell("The First Speaker");
        Core.EnableSkills();

        while (Core.MonsterAlive("The First Speaker") && !Bot.ShouldExit)
        {
            if (equalDetected && CanEnterRedZone())
                MoveIn();
            else
                MoveOut();
        }
    }

    async void UltraSpeakerListener(dynamic packet)
    {
        if (packet?["params"]?.type?.ToString() != "json") return;
        dynamic data = packet["params"].dataObj;
        if (data?.cmd?.ToString() != "ct") return;

        var anims = data.anims as System.Collections.IEnumerable;
        if (anims == null) return;

        foreach (var a in anims)
        {
            string animStr = (a as dynamic)?.animStr?.ToString();
            if (!string.IsNullOrEmpty(animStr))
            {
                if (animStr.Equals("ChargeA", StringComparison.OrdinalIgnoreCase))
                {
                    listenDetected = true;
                    await Task.Delay(2000);
                    listenDetected = false;
                    return;
                }

                if (animStr.Equals("ChargeB", StringComparison.OrdinalIgnoreCase))
                {
                    truthDetected = true;
                    await Task.Delay(2000);
                    truthDetected = false;
                    return;
                }

                if (animStr.Equals("ChargeC", StringComparison.OrdinalIgnoreCase))
                {
                    equalDetected = true;
                    await Task.Delay(3000);
                    equalDetected = false;
                    return;
                }
            }
        }
    }
}
