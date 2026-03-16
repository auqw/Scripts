/*
name: Celestial Arena (Extra)
description: This will finish the Celestial Arena quest.
tags: story, quest, queen-of-monsters, extra, celestial-arena
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs

using Skua.Core.Interfaces;

public class CelestialArenaQuests
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
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

        DoAll();

        Core.SetOptions(false);
    }

    public void DoAll()
    {
        Adv.GearStore(EnhAfter: true);
        Arena1to10();
        Arena11to20();
        Arena21to29();
        Adv.GearStore(true, EnhAfter: true);
    }

    public void Arena1to10()
    {
        if (Core.isCompletedBefore(6022))
            return;

        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(6013, "celestialarenab", "Slork Construct");
        Core.JumpWait();
        Story.KillQuest(6014, "celestialarenab", "Azkorath Construct");
        Core.JumpWait();
        Story.KillQuest(6015, "celestialarenab", "Blessed Inquisitor");
        Core.JumpWait();
        Story.KillQuest(6016, "celestialarenab", "Lich Ravager Construct");
        Core.JumpWait();
        Story.KillQuest(6017, "celestialarenab", "Ring Guardian Construct");
        Core.JumpWait();
        Story.KillQuest(6018, "celestialarenab", "Serepthys Construct");
        Core.JumpWait();
        Story.KillQuest(6019, "celestialarenab", "Yaomo Construct");
        Core.JumpWait();
        Story.KillQuest(6020, "celestialarenab", "Cerberus Construct");
        Core.JumpWait();
        Story.KillQuest(6021, "celestialarenab", "Infernal Warrior Construct");
        Core.JumpWait();
        Story.KillQuest(6022, "celestialarenab", "Infernal Warlord Construct");
        Core.JumpWait();
    }

    public void Arena11to20()
    {
        if (Core.isCompletedBefore(6032))
            return;

        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(6023, "celestialarenac", "Conquest Construct");
        Core.JumpWait();
        Story.KillQuest(6024, "celestialarenac", "War Construct");
        Core.JumpWait();
        Story.KillQuest(6025, "celestialarenac", "Death Construct");
        Core.JumpWait();
        Story.KillQuest(6026, "celestialarenac", "Famine Construct");
        Core.JumpWait();
        Story.KillQuest(6027, "celestialarenac", "Diabolical Warlord Construct");
        Core.JumpWait();
        Story.KillQuest(6028, "celestialarenac", "Undead Raxgore Construct");
        Core.JumpWait();
        Story.KillQuest(6029, "celestialarenac", "Blessed Karok");
        Core.JumpWait();
        Story.KillQuest(6030, "celestialarenac", "Kezeroth Construct");
        Core.JumpWait();
        Story.KillQuest(6031, "celestialarenac", "Shadow Lord Construct");
        Core.JumpWait();
        Story.KillQuest(6032, "celestialarenac", "Desolich Construct");
        Core.JumpWait();
    }

    public void Arena21to29()
    {
        if (Core.isCompletedBefore(6042))
            return;

        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(6033, "celestialarenad", "Queen of Hope");
        Core.JumpWait();
        Story.KillQuest(6034, "celestialarenad", "Malxas Construct");
        Core.JumpWait();
        Story.KillQuest(6035, "celestialarenad", "Blessed Gladius");
        Core.JumpWait();
        Story.KillQuest(6036, "celestialarenad", "High Celestial Priest");
        Core.JumpWait();
        Story.KillQuest(6037, "celestialarenad", "Blessed Enfield");
        Core.JumpWait();
        Story.KillQuest(6038, "celestialarenad", "Avatar of Spirits");
        Core.JumpWait();
        Story.KillQuest(6039, "celestialarenad", "Avatar of Time");
        Core.JumpWait();
        Story.KillQuest(6040, "celestialarenad", "Avatar of Life");
        Core.JumpWait();
        Story.KillQuest(6041, "celestialarenad", "Fallen Abezeth");
        Core.JumpWait();
        Story.KillQuest(6042, "celestialarenad", "Aranx");
        Core.JumpWait();
    }
}
