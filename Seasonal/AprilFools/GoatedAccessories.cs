/*
name: GoatedAccessories
description: This script will optain all rewards from the `Goated Accessories` quest.
tags: GoatedAccessories, goated, accessories, goated accessories, april fools, seasonal
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

public class GoatedAccessories
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

        GetStuff();

        Core.SetOptions(false);
    }

    public void GetStuff()
    {
        string[] Rewards = Core.QuestRewards(10155);

        if (Core.CheckInventory(Rewards, toInv: false))
            return;

        Core.AddDrop(Rewards);
        Core.EquipClass(ClassType.Solo);
        Core.RegisterQuests(10155);
        while (!Bot.ShouldExit && !Core.CheckInventory(Rewards, toInv: false))
        {
            Core.KillMonster("j6cruise", "Frame3", "Left", "MEGA-GOAT");
        }
        Core.CancelRegisteredQuests();
        Core.JumpWait();
        Core.ToBank(Rewards);
    }
}
