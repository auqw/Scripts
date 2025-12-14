/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class CoreHollowborn
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.RunCore();
    }

    public void HardcoreContract()
    {
        if (Core.CheckInventory(55157))
            return;

        Core.AddDrop("Human Soul", "Fallen Soul", "Lae\'s Hardcore Contract");
        Farm.Experience(65);

        Core.Logger("Getting Lae's Hardcore Contract");
        Core.EnsureAccept(7556);

        if (!Core.CheckInventory("Soul Potion"))
        {
            Farm.Gold(2500000);
            Core.BuyItem("alchemyacademy", 2036, "Gold Voucher 500k", 5);
            Core.BuyItem("alchemyacademy", 2036, "Soul Potion");
            Bot.Wait.ForItemBuy();
        }

        HumanSoul(50);

        Core.EquipClass(ClassType.Solo);
        Core.KillMonster("doomwood", "r10", "right", "Undead Paladin", "Fallen Soul", 13, false);

        Core.EnsureComplete(7556);
    }

    public void HumanSoul(int quant = 300)
    {
        if (Core.CheckInventory("Human Soul", quant))
            return;

        Core.AddDrop("Human Soul");

        Core.EquipClass(ClassType.Farm);
        Core.KillMonster("noxustower", "r13", "Left", "*", "Human Soul", quant, false);
    }

    public void FreshSouls(int Uni36Quant = 3, int FSQuant = 1000)
    {
        if (
            Core.CheckInventory("Unidentified 36", Uni36Quant)
            && Core.CheckInventory("Fresh Soul", FSQuant)
        )
            return;

        Farm.Experience(50);

        Core.AddDrop("Unidentified 36");
        // "Fresh Soul"
        Core.AddDrop(52588);
        Core.RegisterQuests(7293);

        if (FSQuant > 0)
            Core.FarmingLogger("Fresh Soul", FSQuant);
        if (Uni36Quant > 0)
            Core.FarmingLogger("Unidentified 36", Uni36Quant);

        while (
            !Bot.ShouldExit
            && (
                !Core.CheckInventory("Unidentified 36", Uni36Quant)
                || !Core.CheckInventory(52588, FSQuant)
            )
        )
        {
            Core.HuntMonster("citadel", "Inquisitor Guard", "Fresh Soul?", 10, log: false);
            Bot.Wait.ForPickup(52588);
        }
        Core.CancelRegisteredQuests();
    }

    public void HBLycanClaw(int quant = 1000)
    {
        if (Core.CheckInventory("Hollowborn Lycan Claw", quant))
            return;

        Core.FarmingLogger("Hollowborn Lycan Claw", quant);
        Core.EquipClass(ClassType.Solo);
        Core.RegisterQuests(9489);
        Core.KillMonster(
            "hbchallenge",
            "r9",
            "Left",
            "*",
            "Hollowborn Lycan Claw",
            quant,
            isTemp: false
        );
        Bot.Wait.ForPickup("Hollowborn Lycan Claw");
        Core.CancelRegisteredQuests();
    }

    public void HBVampireFang(int quant = 1000)
    {
        Core.FarmingLogger("Hollowborn Vampire Fang", quant);
        Core.EquipClass(ClassType.Solo);
        Core.RegisterQuests(9488);
        while (!Bot.ShouldExit && !Core.CheckInventory("Hollowborn Vampire Fang", quant))
            Core.KillMonster(
                "hbchallenge",
                "r8",
                "Left",
                "Hollowborn Vampire",
                "Hollowborn Vampire Fang",
                quant,
                isTemp: false
            );
        Bot.Wait.ForPickup("Hollowborn Vampire Fang");
        Core.CancelRegisteredQuests();
    }

    public void HBHollowbornResidue(int quant = 1000)
    {
        Core.FarmingLogger("Hollowborn Residue", quant);
        Core.EquipClass(ClassType.Farm);
        Core.RegisterQuests(8996); //Hazardous Hybrid 8996
        Core.KillMonster(
            "hbchallenge",
            "r5",
            "Left",
            "*",
            "Hollowborn Residue",
            quant,
            isTemp: false
        );
        Bot.Wait.ForPickup("Hollowborn Residue");
        Core.CancelRegisteredQuests();
    }

    public void HBWrit(int quant = 1000)
    {
        Core.FarmingLogger("Hollowborn Writ", quant);
        Core.EquipClass(ClassType.Farm);
        Core.RegisterQuests(8418);
        Core.KillMonster(
            "hbchallenge",
            "r3",
            "Right",
            "Judge's Minion",
            "Hollowborn Writ",
            quant,
            isTemp: false
        );
        Bot.Wait.ForPickup("Hollowborn Writ");
        Core.CancelRegisteredQuests();
    }
}
