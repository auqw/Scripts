/*
name: Keraunomedicine
description: One clean sweep quest completion for Keraunomedicine rewards
tags: Keraunomedicine,
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/Oasis/CoreOasis.cs

using Skua.Core.Interfaces;

public class Keraunomedicine
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private CoreFarms Farm = new();
    private static CoreOasis COasis { get => _COasis ??= new CoreOasis(); set => _COasis = value; }
    private static CoreOasis _COasis;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetItems();

        Core.SetOptions(false);
    }

    public void GetItems()
    {
        COasis.CarcossaCabins();
        int questID = 10763;

        Core.AddDrop(
            "Crown of the Sky",
            "Volgritian's Tempest Helm",
            "Energized Aetheria Wings",
            "Aetheria Wings",
            "Galvanized Astrapè",

            // materials
            "Gold Voucher 100k",
            "Volgritian's Scale",
            "Volgritian's Vet Receipt",
            "EP Cell"
        );

        Core.Logger("====================================");
        Core.Logger("      KERAUNOMEDICINE Started      ");
        Core.Logger("====================================");


        Core.EnsureAccept(questID);

        // ===== MATERIAL FARM =====

        Core.EquipClass(ClassType.Solo);
        Core.HuntMonster("naoisegrave", "Volgritian", "Volgritian's Scale", 25, isTemp: false);

        while (!Bot.ShouldExit && !Core.CheckInventory("Volgritian's Vet Receipt"))
        {
            Core.EnsureAccept(10762);
            if (!Core.CheckInventory("Gold Voucher 100k", 20))
                Farm.Voucher("Gold Voucher 100k", 20);
            Core.EnsureComplete(10762);
        }

        while (!Bot.ShouldExit && !Core.CheckInventory("Gold Voucher 100k", 20))
            Farm.Voucher("Gold Voucher 100k", 20);

        Core.RegisterQuests(Core.IsMember ? 10761 : 10760);

        while (!Bot.ShouldExit && !Core.CheckInventory("EP Cell", 250))
        {
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("carcossacabins", "Clementine", "Clementine's Blood Sample");

            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("carcossacabins", "Evolved Lifeform", "Lifeform's Fingers", 9);
            Core.HuntMonster("carcossacabins", "Doom Leech", "Leech's Ganglion", 9);

            Bot.Wait.ForPickup("EP Cell");
        }

        Core.CancelRegisteredQuests();


        // ===== COMPLETE MAIN QUEST =====

        Core.EnsureComplete(questID);

        // ===== FINISH =====

        Core.Logger("====================================");
        Core.Logger("      KERAUNOMEDICINE COMPLETE      ");
        Core.Logger("====================================");
    }
}