/*
name: Lycan Shaman Set
description: farms the "Lycan Shaman" set from Quest: "Blood Rage".
tags: blood rage, lycan shaman, set
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/BloodMoon.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class LycanShamanSet
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static BloodMoon BloodMoon
    {
        get => _BloodMoon ??= new BloodMoon();
        set => _BloodMoon = value;
    }
    private static BloodMoon _BloodMoon;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetAll();

        Core.SetOptions(false);
    }

    public void GetAll()
    {
        string[] rewards =
        {
            "Lycan Shaman",
            "Lycan Shaman Helm",
            "Lycan Shaman's Familiar",
            "Lycan Shaman Staff",
        };
        if (Core.CheckInventory(rewards))
            return;

        int count = 0;

        Core.CheckSpaces(ref count, rewards);
        Core.AddDrop(rewards);

        BloodMoon.BloodMoonSaga();

        Core.RegisterQuests(6073);
        Bot.Events.ItemDropped += ItemDropped;
        Core.Logger(
            $"Farm for the Lycan Shaman set started. Farming to get {rewards.Length - count} more item"
                + ((rewards.Length - count) > 1 ? "s" : "")
        );

        while (!Bot.ShouldExit && !Core.CheckInventory(rewards))
        {
            Core.HuntMonster("bloodwarlycan", "Blood Guardian", "Ruby Crest", 5);
            Bot.Wait.ForPickup("*");
        }

        Bot.Events.ItemDropped -= ItemDropped;
        Core.CancelRegisteredQuests();

        void ItemDropped(ItemBase item, bool addedToInv, int quantityNow)
        {
            if (rewards.Contains(item.Name))
            {
                count++;
                Core.Logger($"Got {item.Name}, {rewards.Length - count} items to go");
            }
        }
    }
}
