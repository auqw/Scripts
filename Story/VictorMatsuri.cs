/*
name: Victor Matsuri
description: This script will complete the Empress Ai No Miko's questline in /victormatsuri.
tags: victor matsuri, victormatsuri, empress ai no miko,ai no miko,matsuri,victor,rite of ascension,rite,victor of the festival,eclipse
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class VictorMatsuri
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    // Flags for event-driven action
    private bool counterAttackTriggered = false;
    private DateTime lastCounterAttack = DateTime.MinValue;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Storyline();

        Core.SetOptions(false);
    }

    public void Storyline(bool merge = false)
    {
        if (merge ? Core.isCompletedBefore(10291) : Core.isCompletedBefore(10295))
            return;

        Story.PreLoad(this);

        #region Useable Monsters
        string[] UseableMonsters = new[]
        {
            "Kitsune Himawari", // UseableMonsters[0],
            "NeOni", // UseableMonsters[1],
            "Narcis Arrhythmia", // UseableMonsters[2],
            "Haruki Matsuoka", // UseableMonsters[3],
            "Lady Laidronette", // UseableMonsters[4],
            "Masakado", // UseableMonsters[5]
        };
        #endregion Useable Monsters

        // 10290 | In Kuzunoha's Image
        if (!Story.QuestProgression(10290))
        {
            Core.HuntMonsterQuest(10290, ("victormatsuri", UseableMonsters[0], ClassType.Solo));
        }

        // 10291 | NeOni Blue
        if (!Story.QuestProgression(10291))
        {
            Core.HuntMonsterQuest(10291, ("victormatsuri", UseableMonsters[1], ClassType.Solo));
        }

        if (merge)
            return;

        // 10292 | Embodiment of Scarlet
        if (!Story.QuestProgression(10292))
        {
            Core.HuntMonsterQuest(10292, ("victormatsuri", UseableMonsters[2], ClassType.Solo));
        }

        // 10293 | Onihitokuchi
        if (!Story.QuestProgression(10293))
        {
            Core.HuntMonsterQuest(10293, ("victormatsuri", UseableMonsters[3], ClassType.Solo));
        }

        // 10294 | Tsukihime
        if (!Story.QuestProgression(10294))
        {
            Core.HuntMonsterQuest(10294, ("victormatsuri", UseableMonsters[4], ClassType.Solo));
        }

        // 10295 | Kanmu Heishi
        if (!Story.QuestProgression(10295))
        {
            Core.EnsureAccept(10295);
            KillMasakado(
                map: "victormatsuri",
                cell: "r8",
                pad: "Left",
                monster: "Masakado",
                auraNames: new[] { "Masakado prepares a counter attack!" }, // Replace with actual message if different
                item: Core.EnsureLoad(10295).Requirements[0].ToString(),
                quant: 1,
                isTemp: Core.EnsureLoad(10295).Requirements[0].Temp,
                log: true
            );
            Core.EnsureComplete(10295);
        }
    }

    public void KillMasakado(
        string map,
        string cell,
        string pad,
        string monster,
        string[] auraNames,
        string? item = null,
        int quant = 1,
        bool isTemp = false,
        bool log = true,
        int ItemToUse = 0,
        int SafeItem = 0,
        CancellationToken cancellationToken = default
    )
    {
        // return if item already in inventory
        if (
            item != null
            && (isTemp ? Bot.TempInv.Contains(item, quant) : Core.CheckInventory(item, quant))
        )
        {
            return;
        }

        // if Legion Revenant class not owned, log and return
        if (!Core.CheckInventory("Legion Revenant"))
        {
            Core.Logger("You need to own the Legion Revenant class to use this script.");
            return;
        }

        // Check for praxis only first (as it's the most important and 100% required)
        if (!Adv.uPraxis())
        {
            Core.Logger(
                "You **HAVE** to have the Praxis enhancement to kill Masakado with this script."
            );
            return;
        }

        // Then check for penitence and pneuma
        if (!Adv.uPenitence() || !Adv.uPneuma())
        {
            Core.Logger(
                "You need to have the Penitence cape and Pneuma helm enhancements to kill Masakado with this script."
            );
            return;
        }

        Adv.GearStore();
        Core.Equip("Legion Revenant");
        Adv.EnhanceEquipped(
            EnhancementType.Wizard,
            CapeSpecial.Penitence,
            HelmSpecial.Pneuma,
            WeaponSpecial.Praxis
        );

        DateTime lastAuraTrigger = DateTime.MinValue;
        TimeSpan auraCooldown = TimeSpan.FromSeconds(0);
        monster = monster.Trim().FormatForCompare();

        // Reset trigger flag
        counterAttackTriggered = false;
        lastCounterAttack = DateTime.MinValue;

        Bot.Events.ExtensionPacketReceived += AuraListener;

        #region Setup Item Equip (optional)
        if (ItemToUse > 0)
        {
            int fallbackPotion = 1749;
            int equipSafe = SafeItem > 0 ? SafeItem : fallbackPotion;

            if (!Core.CheckInventory(equipSafe))
                Adv.BuyItem("embersea", 1100, fallbackPotion, 10, 1, 17966);

            EquipRetry(equipSafe);
            Core.Equip(ItemToUse);
        }
        #endregion

        if (item == null)
        {
            if (log)
                Core.Logger($"Killing {monster}");
            Bot.Kill.Monster(monster);
        }
        else
        {
            if (!isTemp)
                Core.AddDrop(item);
            if (log)
                Core.FarmingLogger(item, quant);

            while (
                !Bot.ShouldExit
                && !Core.CheckInventory(item, quant)
                && !cancellationToken.IsCancellationRequested
            )
            {
                while (
                    !Bot.ShouldExit
                    && !Bot.Player.Alive
                    && !cancellationToken.IsCancellationRequested
                )
                {
                    Bot.Sleep(500);
                }

                if (Bot.Map.Name != map)
                    Core.Join(map, cell, pad);
                if (Bot.Player.Cell != cell)
                    Core.Jump(cell, pad);

                if (!counterAttackTriggered)
                    Bot.Combat.Attack(monster);
                Bot.Sleep(500);

                // Handle counter attack outside the listener
                if (counterAttackTriggered && (DateTime.Now - lastCounterAttack).TotalSeconds < 5)
                {
                    if (Bot.Player.Target == null || Bot.Player.Target.Name != "Masakado")
                    {
                        Bot.Combat.Attack("Masakado");
                    }
                    Bot.Sleep(1000);
                    Bot.Skills.UseSkill(3);
                    Bot.Sleep(3000);
                    Bot.Skills.UseSkill(3);
                    counterAttackTriggered = false;
                }

                if (
                    isTemp
                        ? Bot.TempInv.Contains(item, quant)
                        : (Bot.Inventory.Contains(item, quant) || Bot.Bank.Contains(item, quant))
                )
                    break;
            }
        }

        Bot.Events.ExtensionPacketReceived -= AuraListener;
        Adv.GearStore(true);

        void AuraListener(dynamic packet)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if ((string?)packet["params"]?.type != "json")
                return;

            dynamic? data = packet["params"]?.dataObj;
            if (data == null || data?.cmd?.ToString() != "ct")
                return;
            dynamic? anims = data?.anims;
            if (anims == null)
                return;
            foreach (dynamic a in anims)
            {
                string? auraName = a?["msg"]?.ToString();

                if (string.IsNullOrEmpty(auraName) || !auraNames.Contains(auraName))
                    continue;

                // Throttle cooldown
                if (DateTime.Now - lastAuraTrigger < auraCooldown)
                    continue;

                lastAuraTrigger = DateTime.Now;

                // Set flag for main loop to handle
                counterAttackTriggered = true;
                lastCounterAttack = DateTime.Now;

                break; // react to only one aura per packet
            }
        }

        void EquipRetry(int id)
        {
            Core.Equip(id);
            Bot.Wait.ForTrue(() => Bot.Inventory.IsEquipped(id), 20);
            Bot.Sleep(2000);
            Core.Equip(id); // Flash refresh workaround
            Bot.Sleep(2000);
        }
    }
}
