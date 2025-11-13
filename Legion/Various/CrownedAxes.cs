/*
name: The Castle's Crown Quest Rewards
description: This script will complete "The Castle's Crown" quest
tags: legioncastle,legion,dage,birthday,castle,crown,quest,raven's curse scythe,raven,crowned axes,crowned axes of the underworld
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Legion/Various/LegionCastle.cs
using Skua.Core.Interfaces;

public class CrownedAxes
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static GetLegionCastle LC
    {
        get => _LC ??= new GetLegionCastle();
        set => _LC = value;
    }
    private static GetLegionCastle _LC;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetCrownedAxes();

        Core.SetOptions(false);
    }

    public void GetCrownedAxes()
    {
        if (Core.CheckInventory(Core.QuestRewards(10102)))
            return;

        LC.AllYourBaseAreBelongToUs();
        if (!Core.CheckInventory("Legion Castle"))
        {
            Core.Logger(
                "You need to have Legion Castle to complete this quest. It's daily locked, run the script tomorrow."
            );
            return;
        }

        Core.AddDrop(Core.QuestRewards(10102));

        Core.EquipClass(ClassType.Solo);

        Core.EnsureAccept(10102);

        // King Klunk's Crown
        Core.HuntMonster("evilwarnul", "Laken", "King Klunk's Crown", isTemp: false);

        // Chaos King Crown
        Core.HuntMonster("swordhavenfalls", "Chaos Lord Alteon", "Chaos King Crown", isTemp: false);

        // Imbalanced Crown
        Core.HuntMonster("brightfortress", "Imbalanced Alteon", "Imbalanced Crown", isTemp: false);

        // Cecily's Crown
        Core.HuntMonster("lust", "Lascivia", "Cecily's Crown", isTemp: false);

        // Laurel Crown
        Core.HuntMonster("goldenarena", "Queen of Hope", "Laurel Crown", isTemp: false);

        // Northern Crown
        Core.HuntMonster("snowmore", "Jon S'Nooooooo", "Northern Crown", isTemp: false);

        Core.EnsureComplete(10102);
    }
}
