/*
name: Fire Champion's Armor
description: This script will farm Fire Champion's Armor.
tags: fire champions armor, fire champions armor, polished dragon slayer, flame forged metal, void scale, damage dragon, heros valiance, fca
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Other/Classes/DragonslayerGeneral.cs
//cs_include Scripts/Other/WarFuryEmblem.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/Lair.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class FireChampionsArmor
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
    private static DragonslayerGeneral DSG
    {
        get => _DSG ??= new DragonslayerGeneral();
        set => _DSG = value;
    }
    private static DragonslayerGeneral _DSG;
    private static WarfuryEmblem WFE
    {
        get => _WFE ??= new WarfuryEmblem();
        set => _WFE = value;
    }
    private static WarfuryEmblem _WFE;
    private static Lair Lair
    {
        get => _Lair ??= new Lair();
        set => _Lair = value;
    }
    private static Lair _Lair;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetFireChampsArmor();

        Core.SetOptions(false);
    }

    public void GetFireChampsArmor()
    {
        if (Core.CheckInventory(62570))
            return;

        PolishedDragonSlayer();
        DSG.EnchantedScaleandClaw(125, 0);
        WFE.WarfuryEmblemFarm(60);
        FlameForgedMetal(10);
        VoidScale(13);
        Adv.BuyItem("wartraining", 2035, "Fire Champion's Armor", shopItemID: 8759);
    }

    public void PolishedDragonSlayer()
    {
        if (Core.CheckInventory(58462))
            return;

        if (!Core.isCompletedBefore(168))
            Lair.Galanoth();

        if (!Core.CheckInventory(582))
            Core.BuyItem("lair", 38, "Dragonslayer");

        Adv.RankUpClass("Dragonslayer");

        WFE.WarfuryEmblemFarm(30);
        DSG.EnchantedScaleandClaw(30, 0);
        Core.AddDrop(11475);
        Core.FarmingLogger("Dragon Scale", 30);
        Core.EquipClass(ClassType.Farm);
        while (!Bot.ShouldExit && !Core.CheckInventory(11475, 30))
            Core.KillMonster("lair", "Hole", "Center", "*", isTemp: false, log: false);
        Adv.BuyItem("wartraining", 2035, "Polished DragonSlayer");
    }

    public void FlameForgedMetal(int Metalquant = 10)
    {
        if (Core.CheckInventory("Flame-Forged Metal", Metalquant))
            return;

        Core.EquipClass(ClassType.Solo);
        Core.AddDrop("Flame-Forged Metal");

        Core.FarmingLogger("Flame-Forged Metal", Metalquant);

        Core.RegisterQuests(6975);
        while (!Bot.ShouldExit && !Core.CheckInventory("Flame-Forged Metal", Metalquant))
            Core.HuntMonster("underworld", "Frozen Pyromancer", "Stolen Flame", log: false);
        Core.CancelRegisteredQuests();
    }

    public void VoidScale(int VoidScaleQuant = 13)
    {
        if (Core.CheckInventory("Void Scale", VoidScaleQuant))
            return;

        HashSet<string> OptimalClasses = ["Yami no Ronin", "Chaos Slayer"];

        InventoryItem? usethis = Bot.Inventory.Items.Concat(Bot.Bank.Items)
            .FirstOrDefault(n => OptimalClasses.Any(o => n.Name.Equals(o) || n.Name.StartsWith(o)));

        Core.Logger(usethis != null ? $"{usethis.Name} found -- equipping optimal class."
            : !string.IsNullOrEmpty(Core.DodgeClass) ? "No optimal class found, falling back to Dodge Class."
            : "No optimal/dodge class found, using equipped class for Archfiend Dragonlord... GOOD LUCK!!");

        if (usethis != null)
        {
            Core.Equip(usethis.ID);
            Core.Equip(Core.DodgeGear);
            if (usethis.Name.Equals("Yami no Ronin"))
                Bot.Skills.StartAdvanced("1 | 4");
        }
        else if (!string.IsNullOrEmpty(Core.DodgeClass))
            Core.EquipClass(ClassType.Dodge);

        Core.HuntMonster("underlair", "Archfiend Dragonlord", "Void Scale", VoidScaleQuant, isTemp: false);
    }
}
