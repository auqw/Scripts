/*
name: (Badge) Celestial Champion
description: This will get the Celestial Champion badge.
tags: badge, celestial, arena, champion
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/CelestialArena.cs
using Skua.Core.Interfaces;

public class CelestialArenaChampion
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CelestialArenaQuests CA
    {
        get => _CA ??= new CelestialArenaQuests();
        set => _CA = value;
    }
    private static CelestialArenaQuests _CA;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Badge();

        Core.SetOptions(false);
    }

    public void Badge()
    {
        if (Core.HasWebBadge(badge))
        {
            Core.Logger($"Already have the {badge} badge");
            return;
        }

        Core.Logger($"Doing Celestial Arena Challenge for {badge} badge");
        CA.Arena1to10();
        CA.Arena11to20();
        CA.Arena21to29();
    }

    private string badge = "Celestial Champion";
}
