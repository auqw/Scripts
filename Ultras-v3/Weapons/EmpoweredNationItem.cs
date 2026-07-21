/*
name: Empowered Weapons of Nulgath
description: pick an empowered weapon, if you own teh requirements and 25 insignias, itll farm the empowered ver.
tags: empowered, nulgath, reavers, bloodletter, overfiend blade, shadow spear, prismatic manslayers, legacy of nulgath, worshipper of nulgath, evovled void armor
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Nation/CoreNation.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class EmpoweredWeaponsofNulgath
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public string OptionsStorage = "EmpoweredWeaponofN";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        CoreBots.Instance.SkipOptions,
        new Option<EmpoweredItems>(
            "EmpoweredWep",
            "Choose Weapon",
            "Choose, and the bot will Farm the Appropriate item.",
            EmpoweredItems.Empowered_Overfiend_Blade
        ),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(Nation.bagDrops);
        Core.SetOptions();

        GetEmpoweredItem(Bot.Config!.Get<EmpoweredItems>("EmpoweredWep"));

        Core.SetOptions(false);
    }

    public void GetEmpoweredItem(EmpoweredItems Item)
    {
        if (!Core.CheckInventory("Nulgath Insignia", 25))
            Core.Logger(
                "Could not find 25x Nulgath Insignia, stopping.",
                messageBox: true,
                stopBot: true
            );

        #region  nullcheck
        // Retry mechanism to get the selected item from config
        EmpoweredItems? selectedItem = null;
        for (int i = 0; i < 5; i++)
        {
            selectedItem = Bot.Config?.Get<EmpoweredItems>("EmpoweredWep");
            if (selectedItem != null)
                break;
            Core.Logger($"Attempt {i + 1}: EmpoweredWep not found in config. Retrying...");
            Core.Sleep(1000); // Wait for 1 second before retrying
        }

        // Ensure we have a valid item selection from the config
        if (selectedItem == null)
        {
            Core.Logger("EmpoweredWep not found in config after 5 attempts.");
            return;
        }

        // Convert the enum value to a string for checking in the inventory
        string? itemName = selectedItem?.ToString()?.Replace('_', ' ');

        if (string.IsNullOrEmpty(itemName))
        {
            Core.Logger("Item name is null or empty after conversion.");
            return;
        }
        #endregion  nullcheck

        Farm.Experience(80);
        Core.AddDrop(Nation.bagDrops);
        foreach (int Quest in Core.FromTo(8694, 8701))
        {
            Quest? q = Core.InitializeWithRetries(() => Core.EnsureLoad(Quest));
            if (q == null)
            {
                Core.Logger($"Failed to load quest with ID {Quest}");
                continue;
            }
            Core.AddDrop(q.Rewards.Select(x => x.ID).ToArray());
        }

        if (Core.CheckInventory(itemName, toInv: false))
            return;

        Core.AddDrop(itemName);

        switch (Bot.Config?.Get<EmpoweredItems>("EmpoweredWep"))
        {
            //Empowered Bloodletter 8696
            case EmpoweredItems.Empowered_Bloodletter:
                if (!Core.CheckInventory("Bloodletter of Nulgath"))
                    Core.Logger(
                        $"Missing required items. Bot cannot continue",
                        messageBox: true,
                        stopBot: true
                    );

                Core.EnsureAccept(8696);
                Nation.FarmTaintedGem(350);
                Nation.FarmDarkCrystalShard(200);
                Nation.FarmDiamondofNulgath(500);
                Nation.FarmVoucher(false);
                Core.EnsureComplete(8696);
                Bot.Wait.ForPickup((int)Bot.Config.Get<EmpoweredItems>("EmpoweredWep"));
                break;

            //Empowered Evolved Void Armors 1 8700
            case EmpoweredItems.Empowered_Evolved_Fiend:
            case EmpoweredItems.Empowered_Evolved_Void:
                if (
                    !Core.CheckInventory(
                        Bot.Config?.Get<EmpoweredItems>("EmpoweredWep") switch
                        {
                            EmpoweredItems.Empowered_Evolved_Fiend => "Evolved Fiend Of Nulgath",
                            EmpoweredItems.Empowered_Evolved_Void => "Evolved Void Of Nulgath",
                            _ => null,
                        }
                    )
                )
                {
                    Core.Logger(
                        "Missing required item. Bot cannot continue",
                        messageBox: true,
                        stopBot: true
                    );
                    break;
                }
                Core.EnsureAccept(8700);
                Core.EquipClass(ClassType.Solo);
                Core.HuntMonster("lair", "Red Dragon", "Phoenix Blade", isTemp: false);
                Core.HuntMonster("Marsh2", "Soulseeker", "Soul Scythe", isTemp: false);
                Core.HuntMonster("superdeath", "Super Death", "Chaos Tentacles", isTemp: false);
                Nation.FarmDiamondofNulgath(1000);
                Nation.FarmTotemofNulgath(30);
                Farm.ChronoSpanREP(4);
                Adv.BuyItem("thespan", 435, "Shadow Warrior");
                Adv.BuyItem("tercessuinotlim", 1951, "Unmoulded Fiend Essence");
                Core.EnsureComplete(8700, (int)Bot.Config!.Get<EmpoweredItems>("EmpoweredWep"));
                break;

            //Empowered Evolved Void Armors 2 8701
            case EmpoweredItems.Empowered_Evolved_Blood:
            case EmpoweredItems.Empowered_Evolved_Shadow:
            case EmpoweredItems.Empowered_Evolved_Hex:
                if (
                    !Core.CheckInventory(
                        Bot.Config?.Get<EmpoweredItems>("EmpoweredWep") switch
                        {
                            EmpoweredItems.Empowered_Evolved_Blood => "Evolved Blood of Nulgath",
                            EmpoweredItems.Empowered_Evolved_Shadow => "Evolved Shadow of Nulgath",
                            EmpoweredItems.Empowered_Evolved_Hex => "Evolved Hex of Nulgath",
                            _ => null,
                        }
                    )
                )
                {
                    Core.Logger(
                        "Missing required item. Bot cannot continue",
                        messageBox: true,
                        stopBot: true
                    );
                    break;
                }
                Core.EnsureAccept(8701);
                Core.EquipClass(ClassType.Solo);
                Core.HuntMonster("lair", "Red Dragon", "Phoenix Blade", isTemp: false);
                Core.HuntMonster("Marsh2", "Soulseeker", "Soul Scythe", isTemp: false);
                Core.HuntMonster("superdeath", "Super Death", "Chaos Tentacles", isTemp: false);
                Nation.FarmDiamondofNulgath(1000);
                Nation.FarmTotemofNulgath(30);
                Farm.ChronoSpanREP(4);
                Adv.BuyItem("thespan", 435, "Shadow Warrior");
                Adv.BuyItem("tercessuinotlim", 1951, "Unmoulded Fiend Essence");
                Core.EnsureComplete(8701, (int)Bot.Config!.Get<EmpoweredItems>("EmpoweredWep"));
                break;

            //Empowered Legacy of Nulgath 8698
            case EmpoweredItems.Empowered_Legacy_of_Nulgath:
                if (!Core.CheckInventory("Legacy of Nulgath"))
                    Core.Logger(
                        $"Missing required items. Bot cannot continue",
                        messageBox: true,
                        stopBot: true
                    );

                Core.EnsureAccept(8698);
                Nation.FarmDiamondofNulgath();
                Core.EnsureComplete(8698);
                break;

            //Empowered Overfiend Blade 8693
            case EmpoweredItems.Empowered_Overfiend_Blade:
                if (!Core.CheckInventory("Overfiend Blade of Nulgath"))
                    Core.Logger(
                        $"Missing required items. Bot cannot continue",
                        messageBox: true,
                        stopBot: true
                    );

                Core.EnsureAccept(8693);
                Nation.FarmTaintedGem(200);
                Nation.FarmDarkCrystalShard(100);
                Nation.FarmDiamondofNulgath(400);
                Nation.FarmVoucher(false);
                Nation.FarmTotemofNulgath(30);
                Nation.FarmGemofNulgath(80);
                Core.EnsureComplete(8693);
                break;

            //Empowered Prismatic Manslayers 8697
            case EmpoweredItems.Empowered_Prismatic_Manslayer:
            case EmpoweredItems.Empowered_Prismatic_Manslayers:
                if (
                    !Core.CheckInventory(
                        Bot.Config?.Get<EmpoweredItems>("EmpoweredWep")
                        == EmpoweredItems.Empowered_Prismatic_Manslayer
                            ? "Taro's Prismatic Manslayer"
                            : "Taro's Dual Prismatic Manslayers"
                    ) || !Core.IsMember
                )
                {
                    Core.Logger(
                        "Required item not found or you're not a member. Bot cannot continue",
                        messageBox: true,
                        stopBot: true
                    );
                    break;
                }

                Core.EnsureAccept(8697);
                Nation.FarmTaintedGem(400);
                Nation.FarmDarkCrystalShard(250);
                Nation.FarmDiamondofNulgath(600);
                Nation.FarmGemofNulgath(150);
                Nation.FarmBloodGem(70);
                Core.EnsureComplete(8697, (int)Bot.Config!.Get<EmpoweredItems>("EmpoweredWep"));
                break;

            //Empowered Shadow Spear 8695
            case EmpoweredItems.Empowered_Shadow_Spear:
                if (!Core.CheckInventory("Shadow Spear of Nulgath"))
                    Core.Logger(
                        $"Missing required items. Bot cannot continue",
                        messageBox: true,
                        stopBot: true
                    );

                Core.EnsureAccept(8695);
                Nation.FarmTaintedGem(350);
                Nation.FarmDarkCrystalShard(200);
                Nation.FarmDiamondofNulgath(500);
                Nation.FarmVoucher(false);
                break;

            //Empowered Ungodly Reavers 8694
            case EmpoweredItems.Empowered_Ungodly_Reavers:
                if (!Core.CheckInventory("Ungodly Reavers of Nulgath"))
                    Core.Logger(
                        $"Missing required items. Bot cannot continue",
                        messageBox: true,
                        stopBot: true
                    );

                Core.EnsureAccept(8694);
                Nation.FarmTaintedGem(200);
                Nation.FarmDarkCrystalShard(100);
                Nation.FarmDiamondofNulgath(400);
                Nation.FarmVoucher(false);
                Nation.FarmTotemofNulgath(30);
                Nation.FarmGemofNulgath(80);
                Core.EnsureComplete(8694);
                break;

            //Empowered Worshipper of Nulgath 8699
            case EmpoweredItems.Empowered_Worshipper_of_Nulgath:
                if (!Core.CheckInventory("Worshipper of Nulgath"))
                    Core.Logger(
                        $"Missing required items. Bot cannot continue",
                        messageBox: true,
                        stopBot: true
                    );

                Core.EnsureAccept(8699);
                Nation.FarmDiamondofNulgath();
                Core.EnsureComplete(8699);
                break;
        }
    }
}

public enum EmpoweredItems
{
    Empowered_Overfiend_Blade = 70441,
    Empowered_Ungodly_Reavers = 70442,
    Empowered_Shadow_Spear = 70443,
    Empowered_Bloodletter = 70444,
    Empowered_Prismatic_Manslayer = 70445,
    Empowered_Prismatic_Manslayers = 70446,
    Empowered_Legacy_of_Nulgath = 70447,
    Empowered_Worshipper_of_Nulgath = 70448,
    Empowered_Evolved_Void = 70449,
    Empowered_Evolved_Fiend = 70450,
    Empowered_Evolved_Blood = 70451,
    Empowered_Evolved_Hex = 70452,
    Empowered_Evolved_Shadow = 70453,
};
