/*
name: Murder Moon Story
description: This will complete the Murder Moon story.
tags: story, quest, seasonal, murder, moon, may-the-4th
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class MurderMoon
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
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

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        MurderMoonStory();

        Core.SetOptions(false);
    }

    public void MurderMoonStory(bool StopEarly = false)
    {
        if (Core.isCompletedBefore(9224) || !Core.isSeasonalMapActive("murdermoon"))
            return;

        //That Is The Way
        if (!Story.QuestProgression(8062))
        {
            Core.EnsureAccept(8062);
            Core.KillMonster("murdermoon", "r2", "Left", "Tempest Soldier", "Soldiers Defeated", 6);
            Core.EnsureComplete(8062);
        }

        //Murder Moon Plans
        if (!Story.QuestProgression(8063))
        {
            Core.EnsureAccept(8063);
            Core.KillMonster("murdermoon", "r2", "Left", "Tempest Soldier", "Murder Moon Plans");
            Story.MapItemQuest(8063, "murdermoon", 8373, 5);
        }

        //Revenge of the Fifth
        Story.KillQuest(8064, "murdermoon", "Fifth Sepulchure");

        // Tempest Proofing (9223)
        Story.KillQuest(9223, "murdermoon", "Tempest Soldier");

        if (StopEarly)
            return;

        // Liberty's Ghost (9224)
        if (!Story.QuestProgression(9224))
        {
            Adv.GearStore();
            if (!Core.CheckInventory(new[] { "Dark Lord", "Darkside" }, any: true))
                GetDL();
            else
            {
                if (Core.CheckInventory("Dark Lord"))
                    Core.Equip("Dark Lord");
                else if (Core.CheckInventory("Darkside"))
                    Core.Equip("Darkside");
                else
                    Core.Logger(
                        "Neither `Darkside or `Dark Lord` owned... guess we'll use what u have on peasant"
                    );
            }

            if (Adv.uElysium())
            {
                InventoryItem? EquippedWeapon = Bot.Inventory.Items.Find(i =>
                    i != null && i.Equipped && Adv.WeaponCatagories.Contains(i.Category)
                );
                Adv.EnhanceItem(
                    EquippedWeapon!.Name,
                    EnhancementType.Wizard,
                    wSpecial: WeaponSpecial.Elysium
                );
            }

            Story.KillQuest(9224, "murdermoon", "Fourth Lynaria");
            Adv.GearStore(true);
        }
    }

    // keep this here, i tried referencing Darklord to loop back.. but it crashes the client
    public void GetDL()
    {
        if (Core.CheckInventory("Dark Lord"))
            return;

        Core.AddDrop(
            $"Cyber Crystal",
            "S Ring",
            "Fifth Lord's Filtrinator",
            "Dark Helmet",
            "Dotty"
        );

        //Cyber Crystal x66
        Core.EquipClass(ClassType.Farm);
        Core.RegisterQuests(8065);
        while (!Bot.ShouldExit && !Core.CheckInventory("Cyber Crystal", 66))
            Core.KillMonster(
                "murdermoon",
                "r2",
                "Left",
                "Tempest Soldier",
                "Tempest Soldier Badge",
                5,
                log: false
            );
        Core.CancelRegisteredQuests();

        //S Ring x15
        Core.EquipClass(ClassType.Solo);
        Core.HuntMonster("murdermoon", "Fifth Sepulchure", "S Ring", 15, false);

        //Fifth Lord's Filtrinator x15
        Core.HuntMonster($"murdermoon", "Fifth Sepulchure", "Fifth Lord's Filtrinator", 15, false);

        //Dark Helmet x1
        Bot.Quests.UpdateQuest(7484);
        Core.HuntMonster("zorbaspalace", "Zorba the Bakk", "Dark Helmet", 1, false);

        //Dotty x15
        Core.HuntMonster("zorbaspalace", "Zorba the Bakk", "Dotty", 15, false);

        //Gold Voucher 25k x4
        Adv.BuyItem("murdermoon", 1998, "Gold Voucher 25k", 4);

        //Buying the Dark Lord
        Core.BuyItem("murdermoon", 1998, "Dark Lord");
        Bot.Wait.ForItemBuy();

        Adv.RankUpClass("Dark Lord");
    }
}
