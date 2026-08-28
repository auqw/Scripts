/*
name: Caladbolg
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Legion/CoreLegion.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class Caladbolg
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreLegion Legion
    {
        get => _Legion ??= new CoreLegion();
        set => _Legion = value;
    }
    private static CoreLegion _Legion;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public List<IOption> Options = new()
    {
        new Option<bool>(
            "other",
            "Other Rewards",
            "If True, the bot will also get the Dual Caladbolgs and Caladboogly",
            false
        ),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetCaladbolg(Bot.Config!.Get<bool>("other"));

        Core.SetOptions(false);
    }

    public void GetCaladbolg(bool otherRewards = false)
    {
        string[] target = otherRewards
            ? new[] { "Caladbolg", "Caladboogly", "Dual Caladbolgs" }
            : new[] { "Caladbolg" };
        if (Core.CheckInventory(target))
            return;

        if (!Core.CheckInventory("Altar Of Caladbolg"))
        {
            Core.Logger(
                "This bot requires you to have a \"Altar Of Caladbolg\". Stopping the bot.",
                messageBox: true
            );
            return;
        }

        if (!Core.CheckInventory("Legion Titan"))
        {
            if (!Core.CheckInventory("Essence of the Undead Legend"))
            {
                Core.Logger(
                    "This bot requires you to have a \"Essence of the Undead Legend\" (Seasonal - March). Stopping the bot.",
                    messageBox: true
                );
                return;
            }
            if (!Core.CheckInventory("Undead Legend"))
            {
                if (!Core.CheckInventory("Undead Legion Overlord"))
                {
                    Legion.FarmLegionToken(300);
                    Adv.BuyItem("underworld", 238, "Undead Legion Overlord");
                }

                Legion.FarmLegionToken(200);
                Adv.BuyItem("underworld", 238, "Undead Legend");
            }

            Adv.BuyItem("underworld", 238, "Legion Titan");
        }
        int QuestID = Core.CheckInventory(11953) ? 1960 : 3419;
        Core.AddDrop(Core.EnsureLoad(QuestID).Rewards.Select(x => x.Name).ToArray());

        Core.RegisterQuests(QuestID);
        while (!Bot.ShouldExit && !Core.CheckInventory(target))
        {
            if (!Core.CheckInventory("Legion Token", 5))
                Legion.FarmLegionToken(5);
            Core.KillMonster("underworld", "r9", "Left", 20, 13148, isTemp: true);
        }
        Core.CancelRegisteredQuests();
    }
}
