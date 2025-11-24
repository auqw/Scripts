/*
name: Ascended Drakath Gear
description: Farms the ascended Drakath Set
tags: ascended drakath, set, drakath, ascended blade of awe, ascended light of destiny, ascended face of chaos
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Chaos/DrakathsArmor.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Story/TowerOfDoom.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
using Skua.Core.Interfaces;

public class AscendedDrakathGear
{
    public IScriptInterface Bot => IScriptInterface.Instance;

    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static DrakathArmorBot DA
    {
        get => _DA ??= new DrakathArmorBot();
        set => _DA = value;
    }
    private static DrakathArmorBot _DA;
    private static TowerOfDoom TOD
    {
        get => _TOD ??= new TowerOfDoom();
        set => _TOD = value;
    }
    private static TowerOfDoom _TOD;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetAll();

        Core.SetOptions(false);
    }

    public void GetAll()
    {
        if (
            Core.CheckInventory(
                new[]
                {
                    "Ascended Blade of Awe",
                    "Ascended Light of Destiny",
                    "Ascended Face of Chaos",
                }
            )
        )
            return;

        Core.AddDrop(
            "Ascended Blade of Awe",
            "Ascended Light of Destiny",
            "Ascended Face of Chaos"
        );

        AscendedGear("Ascended Blade of Awe");
        AscendedGear("Ascended Light of Destiny");
        AscendedGear("Ascended Face of Chaos");
    }

    public void AscendedGear(string Target)
    {
        if (Core.CheckInventory(Target))
            return;

        DA.DrakathOriginalArmor();
        if (!Core.CheckInventory("Original Drakath Armor"))
        {
            Core.Logger(
                "Missing \"Original Drakath Armor\", which is required to accept and turnin this quest. returning to previous script/stopping script.\n"
                    + $"Probably Missing \"Dage's Scroll fragments\" [{Core.dynamicQuant("Dage's Scroll Fragment", false) / 13}], try again tomarrow when the dailys available."
            );
            return;
        }

        Core.AddDrop(Target);
        Core.RegisterQuests(3767);
        Core.HuntMonster("towerofdoom4", "Dread Stranglerfish", Target, isTemp: false, log: false);
        Core.CancelRegisteredQuests();
    }
}
