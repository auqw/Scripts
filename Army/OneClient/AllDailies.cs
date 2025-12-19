/*
name: Army All Dailies
description: One-client version of All Dailies
tags: all, dailies, army
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Story/Nation/CitadelRuins.cs
//cs_include Scripts/Story/DragonFableOrigins.cs
//cs_include Scripts/Dailies/LordOfOrder.cs
//cs_include Scripts/Story/Glacera.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Dailies/0AllDailies.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/Story/Friendship.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Dailies/MineCrafting.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class ArmyAllDailies
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static FarmAllDailies FAD
    {
        get => _FAD ??= new FarmAllDailies();
        set => _FAD = value;
    }
    private static FarmAllDailies _FAD;
    private static CoreArmyLite Army
    {
        get => _Army ??= new CoreArmyLite();
        set => _Army = value;
    }
    private static CoreArmyLite _Army;

    public bool DontPreconfigure = true;
    public string OptionsStorage = "ArmyAllDailies";
    public List<IOption> Options = new()
    {
        new Option<FarmAllDailies.DailySet>(
            "Select Dailies Set",
            "Dailies set: Recommended or All?",
            "only do the few that we recommend to make it a bit quicker?",
            FarmAllDailies.DailySet.All
        ),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        CheckACs();

        Core.SetOptions(false);
    }

    public void CheckACs()
    {
        while (!Bot.ShouldExit && Army.doForAll())
        {
            if (Bot.Inventory.FreeSlots <= 0)
                continue;
            FAD.DoAllDailies(Bot.Config!.Get<FarmAllDailies.DailySet>("Select Dailies Set"));
        }
    }
}
