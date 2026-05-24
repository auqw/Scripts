/*
name: Aura Logger
description: Continuously logs current player auras
tags: aura,test,logger
*/

//cs_include Scripts/Ultras-v2/Dependencies/CoreEngine.cs
//cs_include Scripts/Ultras-v2/Dependencies/CoreUltra.cs
//cs_include Scripts/Ultras-v2/Dependencies/UltraPotions.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs

using System;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models;

public class AuraLogger
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;

    #region Auras

    public Aura? GetAuraByName(string auraName, bool self)
        => string.IsNullOrWhiteSpace(auraName)
           ? null
           : (self ? Bot?.Self?.Auras : Bot?.Target?.Auras)
             ?.FirstOrDefault(a => a?.Name != null && auraName.Equals(a.Name, StringComparison.OrdinalIgnoreCase));

    public float GetAuraStacksFloat(string auraName, bool self = false)
        => (self
            ? Bot.Self.Auras.FirstOrDefault(a => a?.Name == auraName)?.Value
            : Bot.Target.Auras.FirstOrDefault(a => a?.Name == auraName)?.Value) ?? 0f;

    public int GetAuraSecondsRemaining(string auraName, bool self = false)
    {
        var aura = GetAuraByName(auraName, self);

        return aura != null && aura.UnixTimeStamp > 0 && aura.Duration > 0
            ? Math.Max(
                0,
                (int)(
                    DateTimeOffset.FromUnixTimeMilliseconds(aura.UnixTimeStamp)
                    .AddSeconds(aura.Duration)
                    - DateTimeOffset.UtcNow
                ).TotalSeconds
            )
            : 0;
    }

    #endregion

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        while (!Bot.ShouldExit)
        {
            LogSelfAuras();

            Bot.Sleep(2000);
        }

        Core.SetOptions(false);
    }

    public void LogSelfAuras()
    {
        if (Bot.Self?.Auras == null || !Bot.Self.Auras.Any())
        {
            Core.Logger("No active self auras found.");
            return;
        }

        Core.Logger("=== ACTIVE SELF AURAS ===");

        foreach (Aura aura in Bot.Self.Auras.Where(a => a != null))
        {
            string auraName = aura.Name;

            Core.Logger(
                $"Aura: {auraName} | " +
                $"Stacks: {GetAuraStacksFloat(auraName, true)} | " +
                $"Remaining: {GetAuraSecondsRemaining(auraName, true)}s"
            );
        }
    }
}