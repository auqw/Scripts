/*
name: Celestial Pirate Commander (Polly Rogers)
description: This will farm the Celestial Pirate Commander items and (Polly Rogers) pet.
tags: farm, pet, polly-rogers, celestial-pirate-commander, pirate
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class CelestialPirateCommander
{
    public IScriptInterface Bot => IScriptInterface.Instance;

    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;

    public CoreBots Core => CoreBots.Instance;
    public bool DontPreconfigure = true;


    public string OptionsStorage = "CelestialPiriateCommander";

    public List<IOption> Options = new()
    {
        new Option<CPCReward>(
            "CPCReward",
            "Which reward do you want?",
            "Select a specific reward or Any to farm everything",
            CPCReward.All
        ),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetCPC(Reward);

        Core.SetOptions(false);
    }

    public string[] Rewards =
    {
        "Celestial Pirate Commander",
        "Celestial Commander's Hat",
        "Celestial Commander's Locks",
        "Celestial Commander's Locks + Hat",
        "Celestial Commander's Wings",
        "Celestial Commander's Back Blade",
        "Celestial Commander's Wings+ Blade",
        "Celestial Commander's Sword",
        "Celestial Commander's Hat + Morph",
        "Celestial Commander's Morph + Locks",
        "Celestial Commander's Plank",
        "Polly Roger",
    };

    // Declare `Reward` at class level using a void.. type thing
    CPCReward Reward => Bot.Config!.Get<CPCReward>("CPCReward");

    public void GetCPC(CPCReward Reward)
    {
        if (!Bot.Quests.IsAvailable(7713))
        {
            Core.Logger("Not the right season ya dummy");
            return;
        }

        if (Reward != CPCReward.All && Core.CheckInventory((int)Reward))
        {
            Core.Logger($"You already have the selected reward");
            return;
        }

        Farm.Experience(80);

        int i = 1;
        Core.AddDrop(Rewards);

        while (!Bot.ShouldExit && (Reward == CPCReward.All ? !Core.CheckInventory(Rewards, toInv: false) : !Core.CheckInventory((int)Reward)))
        {
            Core.EnsureAccept(7713);
            Core.EquipClass(ClassType.Dodge);

            Core.HuntMonster(
                "frozenlair",
                "Legion Lich Lord",
                "Sapphire Orb",
                5,
                false,
                publicRoom: true
            );

            Core.EquipClass(ClassType.Solo);

            Core.HuntMonster(
                "lostruinswar",
                "Diabolical Warlord",
                "Rumors of the Celestial Commander",
                5,
                false,
                publicRoom: true
            );

            Core.HuntMonster(
                "iceplane",
                "Animus of Ice",
                "Starlit Journal Page 1 Scraps",
                10,
                false
            );

            Core.HuntMonster(
                "ivoliss",
                "Ivoliss",
                "Starlit Journal Page 2 Scraps",
                10,
                false,
                publicRoom: true
            );

            Core.HuntMonster(
                "voidnightbane",
                "Nightbane",
                "Starlit Journal Page 3 Scraps",
                10,
                false,
                publicRoom: true
            );

            Core.HuntMonster(
                "extinction",
                "Ultra SN.O.W.",
                "Starlit Journal Page 4 Scraps",
                10,
                false,
                publicRoom: true
            );

            Core.HuntMonster(
                "starsinc",
                "Empowered Prime",
                "Map of the Celestial Seas",
                1,
                false,
                publicRoom: true
            );

            //why the fuck was the class buffed!?
            InventoryItem? usethis = Bot
                .Inventory.Items.Concat(Bot.Bank.Items)
                .FirstOrDefault(n =>
                    n.Name.Equals("Yami no Ronin") || n.Name.StartsWith("Chaos Slayer")
                );
            if (usethis != null)
            {
                Core.Equip(usethis.ID);
                Core.Equip(Core.FarmGear);
                if (usethis.Name.Equals("Yami no Ronin"))
                    Bot.Skills.StartAdvanced("1 | 4");
            }
            else
                Core.EquipClass(ClassType.Dodge);

            Core.HuntMonster(
                "underlair",
                "ArchFiend DragonLord",
                "Coffer of the Stars",
                1,
                false,
                publicRoom: true
            );

            Core.EquipClass(ClassType.Solo);

            if (Reward != CPCReward.All)
            {
                Core.EnsureComplete(7713, (int)Reward);
                return;
            }
            else
            {
                // Complete for first unowned if reward == `Any`
                Core.EnsureCompleteChoose(7713);
                Core.ToBank(Rewards);
                Core.Logger($"Completed x{i++}");
            }
        }

        Core.Logger(Reward == CPCReward.All ? "You already have all the drops" : "You already have the selected reward");
    }

    public enum CPCReward
    {
        All = 0,
        PollyRoger = 56776,
        CelestialPirateCommander = 56588,
        CommandersHat = 56589,
        CommandersLocks = 56590,
        LocksAndHat = 56591,
        Wings = 56592,
        BackBlade = 56593,
        WingsAndBlade = 56594,
        Sword = 56595,
        HatAndMorph = 56596,
        MorphAndLocks = 56597,
        Plank = 56619
    }
}
