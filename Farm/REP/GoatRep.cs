/*
name: Goat REP
description: This script will farm Goat reputation to rank 10.
tags: Goat, rank, rep, reputation
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

public class GoatREP
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

        GoatRep();

        Core.SetOptions(false);
    }

    public void GoatRep()
    {
        if (Farm.FactionRank("Goat") >= 10)
        {
            Core.Logger("You already have rank 10 Goat reputation.");
            return;
        }
        string[] drops = Core.QuestRewards(10143).Concat(Core.QuestRewards(10139)).ToArray();
        Core.AddDrop(drops);

        Farm.Experience(50);
        if (!Story.QuestProgression(10139))
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
                DoT.DotArmor();
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

        // Eat Grass | 10143
        Story.MapItemQuest(10143, "goatfield", 14325);

        // Run Around | 10144
        Story.MapItemQuest(10144, "goatfield", 14326);
        Core.ToBank(drops);
    }
}
