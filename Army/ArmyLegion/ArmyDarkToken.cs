/*
name: Army Dark Token
description: uses an army to farm dark tokens
tags: dark token, army, seraphicwardage
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Options;
using System.Linq;

public class ArmyDarkToken
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreArmyLite Army { get => _Army ??= new CoreArmyLite(); set => _Army = value; }
    private static CoreArmyLite _Army;
    private static CoreLegion Legion { get => _Legion ??= new CoreLegion(); set => _Legion = value; }
    private static CoreLegion _Legion;

    private static CoreBots sCore { get => _sCore ??= new CoreBots(); set => _sCore = value; }

    private static CoreBots _sCore;

    private static CoreArmyLite sArmy { get => _sArmy ??= new CoreArmyLite(); set => _sArmy = value; }

    private static CoreArmyLite _sArmy;


    public string OptionsStorage = "ArmyDarkToken";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        sArmy.player1,
        sArmy.player2,
        sArmy.player3,
        sArmy.player4,
        sArmy.player5,
        sArmy.player6,
        sArmy.packetDelay,
        CoreBots.Instance.SkipOptions
    };


    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Core.Logger("~\"All\"~ Army Scripts have been disabled by the author.", "**READ ME!!**", stopBot: true);
        // Setup();

        Core.SetOptions(false);
    }

    public void Setup(int quant = 10000)
    {
        if (Core.CheckInventory("Dark Token", quant))
            return;

        Core.OneTimeMessage("Only for army", "This is intended for use with an army, not for solo players.");

        Core.FarmingLogger("Dark Token", quant);
        Core.AddDrop("Dark Token");

        Core.EquipClass(ClassType.Farm);


        Core.RegisterQuests(6248, 6249, 6251);
        Army.SmartAggroMonStart("seraphicwardage", new[] { "Seraphic Commander, Seraphic Soldier" });



        while (!Bot.ShouldExit && !Core.CheckInventory("Dark Token", quant))
            Bot.Combat.Attack("*");
        Core.CancelRegisteredQuests();
        Army.AggroMonStop(true);
    }
}
