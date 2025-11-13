/*
name: UnundeadGoat
description: This script will do the `Extended Vacation` Quest to acess the `UnundeadGoat`, get the class and rank it.
tags: UnundeadGoat, goatfield, class, rank, farm, seasonal, aprilfools
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Darkon/CoreDarkon.cs
//cs_include Scripts/Other/Weapons/GoldenBladeOfFate.cs
//cs_include Scripts/Other/Weapons/PinkBladeofDestruction.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
//cs_include Scripts/Story/QueenofMonsters/CoreQoM.cs
//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs
//cs_include Scripts/Story/7DeadlyDragons/Core7DD.cs
//cs_include Scripts/Other/MysteriousEgg.cs
//cs_include Scripts/Story/Summer2015AdventureMap/CoreSummer.cs
//cs_include Scripts/Story/Borgars.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs
//cs_include Scripts/Other/Classes/DragonOfTime.cs

using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Models.Shops;

public class UnundeadGoat
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
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
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static DragonOfTime DoT
    {
        get => _DoT ??= new DragonOfTime();
        set => _DoT = value;
    }
    private static DragonOfTime _DoT;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Getthestuff();

        Core.SetOptions(false);
    }

    public void Getthestuff(bool rankUpClass = true)
    {
        if (Core.CheckInventory("UnUnundead Goat"))
        {
            if (rankUpClass)
            {
                Adv.RankUpClass("Unundead Goat");
            }
            return;
        }

        if (!Core.isSeasonalMapActive("goatfield"))
            return;

        if (!Core.isCompletedBefore(10139))
        {
            Core.AddDrop(92935);
            Core.EnsureAccept(10139);

            if (!Bot.House.Items.Concat(Bot.Bank.Items).Any(i => i.ID == 1286))
                Core.BuyItem("buyhouse", 71, 1286, 1, 1401);

            if (!Core.CheckInventory(17585))
                Farm.BladeofAweREP(1, true);

            if (!Core.CheckInventory(56723))
            {
                Core.FarmingLogger("Dragon of Time Armor");
                DoT.GetDoT(false, false, true);
            }

            if (!Core.CheckInventory("Gold Voucher 500k", 2))
                Farm.Voucher("Gold Voucher 500k", 2);

            Core.EquipClass(ClassType.Farm);
            Core.KillMonster("j6cruise", "Frame1", "Left", "Chaos Goat", "Goat Bone", 189, false);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("warundead", "Lich King", "Necronomnomicon", isTemp: false);

            Core.EnsureComplete(10139);
            Bot.Wait.ForPickup(92935);
        }

        Core.Join("goatfield");

        // Load shop data
        int retry = 0;
        while (!Bot.ShouldExit && Bot.Shops.ID != 2569)
        {
            Bot.Shops.Load(2569);
            Bot.Wait.ForActionCooldown(GameActions.LoadShop);
            Bot.Wait.ForTrue(() => Bot.Shops.IsLoaded && Bot.Shops.ID == 2569, 20);
            Core.Sleep(1000);
            if (Bot.Shops.ID == 2569 || retry == 20)
                break;
            else
                retry++;
        }
        retry = 0;

        foreach (ShopItem x in Bot.Shops.Items.ToArray())
        {
            if (Core.CheckInventory(x.Name, 1, false))
                continue;

            Core.BuyItem("goatfield", 2569, x.ID, 1, x.ShopItemID);
            if (x.Name != "Unundead Goat")
                Core.ToBank(x.ID);
        }

        if (rankUpClass)
            Adv.RankUpClass("Unundead Goat");
    }
}
