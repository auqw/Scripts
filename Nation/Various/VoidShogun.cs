/*
name: VoidShogun
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class VoidShogun
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;

    public readonly string[] Rewards =
    {
        "Void Shogun",
        "Void Shogun Mask",
        "Void Shogun Helm",
        "Void Shogun Masked Helm",
        "Void Shogun Banner",
        "Void Shogun Runes",
        "Void Shogun Katana",
        "Void Shogun Naginata",
        "Mini Void Shogun",
        "Mini Void Shogun Battlepet",
        "Void Shogun Katanas on your Hip",
        "Dual Void Shogun Katanas",
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(Nation.bagDrops);
        Core.BankingBlackList.AddRange(Rewards);
        Core.SetOptions();

        GetShogun();

        Core.SetOptions(false);
    }

    public void GetShogun()
    {
        Core.AddDrop(Nation.bagDrops);
        Core.AddDrop(Rewards);

        Core.AddDrop("Void Voucher", "Dai Tengu Blade of Wind", "Orochi's Shadow");

        if (!Core.CheckInventory("Void Monk of Nulgath"))
        {
            Core.EquipClass(ClassType.Farm);
            Core.KillMonster("quibblehunt", "r2", "Left", "*", "Void Voucher", 500, false);
            Core.JumpWait();
            Core.BuyItem("quibblehunt", 1421, "Void Monk of Nulgath");
        }

        Farm.YokaiREP();
        Nation.FarmVoucher(false);

        foreach (
            ItemBase item in Core.EnsureLoad(6484)
                .Rewards.Where(x => x != null && Rewards.Contains(x.Name))
                .Select(x => x)
        )
        {
            if (Core.CheckInventory(item.ID))
            {
                Core.Logger($"Already have {item}");
                continue;
            }

            while (!Bot.ShouldExit && !Core.CheckInventory(item.ID, toInv: false))
            {
                if (Core.CheckInventory(item.ID))
                {
                    Core.Logger($"Already have {item}");
                    continue;
                }
                Core.EnsureAccept(6484);

                Nation.FarmUni13(1);
                Nation.FarmBloodGem(7);
                Nation.TheAssistant("Unidentified 24");

                Core.EquipClass(ClassType.Solo);
                Core.HuntMonster("hachiko", "Dai Tengu", "Dai Tengu Blade of Wind", isTemp: false);
                Core.HuntMonster("shogunwar", "Orochi", "Orochi's Shadow", isTemp: false);
                Core.HuntMonster("necrocavern", "Shadowstone Support", "ShadowStone Rune");

                Core.EnsureComplete(6484, item.ID);
                Bot.Wait.ForPickup(item.ID);
            }
        }
    }
}
