/*
name: RGRoW Badge
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
using Skua.Core.Interfaces;

public class RGRoWBadge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        GetRGRoWBadge();
        Core.SetOptions(false);
    }

    public void GetRGRoWBadge()
    {
        if (Core.HasWebBadge(badge))
        {
            Core.Logger($"Already have the {badge} badge");
            return;
        }
        if (!Core.CheckInventory("Radiant Goddess of War"))
            Core.Logger(
                "Missing \"Radiant Goddess of War\", Cannot get badge",
                stopBot: true,
                messageBox: true
            );

        Core.EnsureAccept(9352);
        // 9352 | Radiant Goddess of War Badge
        Core.EquipClass(ClassType.Solo);
        Core.KillMonster("manacradle", "r10", "Left", "The Mainyu", "Licorice Scale");
        Core.EnsureComplete(9352);
    }

    private readonly string badge = "Radiant Goddess Of War";
}
