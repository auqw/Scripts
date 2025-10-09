/*
name: 17th Birthday Hero Character Page Badge
description: This script will get the "17th Birthday Hero" Character Page Badge.
tags: birthday, badge, anniversary, gifts, quest, seasonal
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class BirthdayHeroBadge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface Bot)
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

        Core.EquipClass(ClassType.Solo);

        //16th Anniversary Gifts
        Story.KillQuest(6554, "birthday", new[] { "Birthday Cake", "Birthday Cake", "Birthday Cake", "Birthday Cake" });
    }

    private string badge = "17th Birthday Hero";
}
