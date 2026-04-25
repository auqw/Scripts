/*
name: KillWorldBoss
description: Kills the world boss to max Flame of the Magnum Opus from Flame of the Beyond
tags: world, boss, worldboss, world boss, Flame of the Magnum Opus, Flame of the Beyond
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;

public class KillWorldBoss
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface Bot)
    {
        // Core.BankingBlackList.AddRange(new[] { "item1", "Item2", "Etc" });
        Core.SetOptions(disableClassSwap: true);

        WorldBoss();

        Core.SetOptions(false);
    }

    void WorldBoss()
    {

        Core.RegisterQuests(10702);
        Core.AddDrop("Flame of the Magnum Opus");
        Core.FarmingLogger("Flame of the Magnum Opus", 1000);


        if (!string.IsNullOrEmpty(Core.BossClass))
            Core.EquipClass(ClassType.Boss);
        else
        {
            Bot.Log("BossClass is empty/unselected in CBO, we'll use the Solo class or if thats empty, the currently equipped class.");
            Core.EquipClass(ClassType.Solo);
        }
        Bot.Events.ExtensionPacketReceived += FlameoftheBeyond;
        Bot.Options.AttackWithoutTarget = true;

        while (!Bot.ShouldExit && !Core.CheckInventory("Flame of the Magnum Opus", 1000))
        {
            if (Bot.Map.Name != "magnumopus")
                Core.Join("magnumopus");

            if (Bot.Player.Cell != "r2")
                Core.Jump("r2", "bottom");


            Bot.Combat.Attack("*");

            Bot.Sleep(200);
            if (Bot.Inventory.Contains("Flame of the Magnum Opus", 1000))
                break;
        }

        Bot.Options.AttackWithoutTarget = false;
        Bot.Events.ExtensionPacketReceived -= FlameoftheBeyond;
    }


    public async void FlameoftheBeyond(dynamic packet)
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

        float px = Bot.Player.X;
        float py = Bot.Player.Y;

        // Zone A active, walk right
        if (string.Equals(zoneSet, "A", StringComparison.OrdinalIgnoreCase))
        {
            /* box of : 
            x: 550, y: 339
            x: 678, y: 465
            */
            if (px >= 550 && px <= 678 && py >= 339 && py <= 465)
                return;

            int randX = Random.Shared.Next(550, 678);
            int randY = Random.Shared.Next(339, 465);
            _ = Task.Run(() => Bot.Player.WalkTo(randX, randY));
            return;
        }

        // Zone B active, walk left
        if (string.Equals(zoneSet, "B", StringComparison.OrdinalIgnoreCase))
        {
            /* box of : 
            x: 426, y: 359
            x: 211, y: 445
            */
            if (px >= 211 && px <= 426 && py >= 359 && py <= 445)
                return;

            int randX = Random.Shared.Next(211, 426);
            int randY = Random.Shared.Next(359, 445);
            _ = Task.Run(() => Bot.Player.WalkTo(randX, randY));
            return;
        }
    }


}



