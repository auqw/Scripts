/*
name: PinkBladeofDestruction
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
using Skua.Core.Interfaces;

public class PinkBladeOfDestruciton
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreBLOD BLoD
    {
        get => _BLoD ??= new CoreBLOD();
        set => _BLoD = value;
    }
    private static CoreBLOD _BLoD;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreDoomwood P3
    {
        get => _P3 ??= new CoreDoomwood();
        set => _P3 = value;
    }
    private static CoreDoomwood _P3;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetPBoD();

        Core.SetOptions(false);
    }

    public void GetPBoD()
    {
        if (Core.CheckInventory("Pink Blade of Destruction"))
            return;

        P3.DoomwoodPart3();

        Core.AddDrop("Fuchsia Dye", "Zealous Badge", "Pink Blade of Destruction");

        Core.EnsureAccept(7650);

        while (!Bot.ShouldExit && !Core.CheckInventory("Fuchsia Dye", 50))
        {
            Core.EnsureAccept(1487);
            Core.HuntMonster("natatorium", "Anglerfish", "Pink Coral", 3, log: false);
            Core.HuntMonster("bloodtuskwar", "Chaotic Vulture", "Amaranth Flower", 5, log: false);
            Core.EnsureComplete(1487);
        }

        BLoD.UnlockMineCrafting();
        BLoD.SpiritOrb(500);

        while (!Bot.ShouldExit && !Core.CheckInventory("Zealous Badge", 5))
        {
            Core.EnsureAccept(7616);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster(
                "techdungeon",
                "Kalron the Cryptborg",
                "Immutable Dedication",
                7,
                log: false
            );
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster(
                "techdungeon",
                "DoomBorg Guard",
                "Paladin Armor Scraps",
                30,
                log: false
            );
            Core.EnsureComplete(7616);
        }

        Core.EquipClass(ClassType.Solo);
        Core.HuntMonster("undergroundlabb", "Ultra BrutalCorn", "Unicorn Essence", 5, false, false);

        Core.HuntMonster("undergroundlabb", "Ultra Battle Gem", "Gem Power", 5, false, false);

        Core.EnsureComplete(7650, 55884);
        Bot.Wait.ForPickup("Pink Blade of Destruction");
    }
}
