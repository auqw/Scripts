/*
name: null
description: null
tags: null
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

public class DefaultTemplate
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static CoreDailies Daily { get => _Daily ??= new CoreDailies(); set => _Daily = value; }
    private static CoreDailies _Daily;

    public void ScriptMain(IScriptInterface Bot)
    {
        // Core.BankingBlackList.AddRange(new[] { "item1", "Item2", "Etc" });
        Core.SetOptions(disableClassSwap: true);

        VoTSSolo();

        Core.SetOptions(false);
    }


    void VoTSSolo()
    {
        // Define the possible solo classes
        string[] PossibleSoloClasses = new[]
        {
            "Chrono ShadowHunter",
            "Chrono ShadowSlayer",
            "Chaos Avenger",
            "Verus DoomKnight",
            "Hollowborn Vindicator",
            "Lich",
            "ArchPaladin",
            "Lord Of Order",
            "StoneCrusher",
            "Dragon of Time",
            "Unundead Goat",
        };

        // Find the first available class in inventory or bank
        string? selectedClass = PossibleSoloClasses.FirstOrDefault(className =>
            Bot.Inventory.Items.Any(item => item.Name == className)
            || Bot.Bank.Items.Any(item => item.Name == className)
        );

        if (string.IsNullOrWhiteSpace(selectedClass))
        {
            // Warn the user but fallback to currently equipped class
            string? equippedClass = Bot.Player.CurrentClass.Name;
            Core.Logger(
                $"No preferred solo class found in inventory or bank.\n"
                    + $"Preferred options: ({string.Join(", ", PossibleSoloClasses)})\n"
                    + $"Using currently equipped class: {equippedClass}. This may not be optimal.\n"
            );

            if (string.IsNullOrWhiteSpace(equippedClass))
            {
                Core.Logger("No class is currently equipped; aborting SeaVoice.");
                return;
            }

            selectedClass = equippedClass;
        }
        else
        {
            Core.Logger($"Soloing \"Voice of the Sea\" with {selectedClass}");
        }

        Adv.GearStore(EnhAfter: true);
        Adv.SmartEnhance(selectedClass);

        // Call the KillThing method with the specified parameters
        KillThing(
            map: "seavoice",
            mobMapID: 1,
            itemUsed: 78994,
            Class: selectedClass,
            item: "Maw of the Sea",
            quant: 10,
            isTemp: false
        );

        Adv.GearStore(true, true);
    }

    public void KillThing(
        string map,
        int mobMapID,
        int itemUsed,
        string Class,
        string item,
        int quant = 1,
        bool isTemp = false
    )
    {
        string? classFromPlayer = Bot.Player.CurrentClass?.Name;

        var itemToEnhance = Bot
            .Inventory?.Items?.FirstOrDefault(x =>
                x?.Equipped == true && Adv.WeaponCatagories.Contains(x.Category)
            )
            ?.Name;

        // if (itemToEnhance != null)
        //     Adv.EnhanceItem(
        //         itemToEnhance,
        //         EnhancementType.Lucky,
        //         wSpecial: WeaponSpecial.Awe_Blast
        //     );

        string? classNameToUse = Class ?? classFromPlayer;
        if (string.IsNullOrWhiteSpace(classNameToUse))
        {
            Core.Logger("KillThing aborted: no class specified and player has no current class.");
            return;
        }

        Core.Join(map);
        if(!Bot.Inventory.IsEquipped(classNameToUse))
        Core.Equip(classNameToUse);
        Adv.BuyItem("seavoice", 2320, "Vigil", 1000, 12023);
        Core.Equip(itemUsed);
        Core.Logger($"{itemUsed} [Vigil] Equiped? {Bot.Inventory?.IsEquipped("Vigil")}");
        Bot.Wait.ForMapLoad(map);
        Bot.Wait.ForTrue(() => Bot.Player.Loaded, 20);

        // Locate mob by MapID
        Monster? mob = Bot.Monsters.MapMonsters.FirstOrDefault(m => m?.MapID == mobMapID);
        if (mob == null)
        {
            Core.Logger($"KillThing aborted: No mob found with MapID {mobMapID} in {map}.");
            return;
        }

        // Move to mob cell and set respawn
        if (Bot.Player.Cell != mob.Cell)
            Core.Jump(mob.Cell);
        Bot.Player.SetSpawnPoint();

        while (
            !Bot.ShouldExit
            && (isTemp ? !Bot.TempInv.Contains(item, quant) : !Core.CheckInventory(item, quant))
        )
        {
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Bot.Player.Cell != mob.Cell)
                Core.Jump(mob.Cell, "Left");

            // === Handle Oxidize aura (use potion to cleanse) ===
            if (Bot.Self.Auras.Any(a => a != null && a.Name == "Oxidize") && !Bot.Self.Auras.Any(a => a != null && a.Name == "Vigil"))
            {
                Bot.Skills.Pause();
                while (!Bot.ShouldExit)
                {
                    Bot.Skills.UseSkill(5);
                    Core.Sleep(500);
                    if (Bot.Self.Auras.Any(a => a != null && a.Name == "Vigil"))
                    {
                        Bot.Skills.Resume();
                        break;
                    }
                }
            }

            // === Attack phase ===
            Bot.Combat.Attack(mob);
            Core.Sleep(250);
        }

        Core.Logger($"KillThing completed for {item} ({quant}).");
    }


}



