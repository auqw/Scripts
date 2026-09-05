/*
name: DragonOfTime
description: This bot farms the Dragon of Time class for you.
tags: class, dot, damage over time, dragon of time, timeinn
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
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
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class DragonOfTime
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
    private static CoreDarkon Darkon
    {
        get => _Darkon ??= new CoreDarkon();
        set => _Darkon = value;
    }
    private static CoreDarkon _Darkon;
    private static GoldenBladeOfFate GBoF
    {
        get => _GBoF ??= new GoldenBladeOfFate();
        set => _GBoF = value;
    }
    private static GoldenBladeOfFate _GBoF;
    private static PinkBladeOfDestruciton PBoD
    {
        get => _PBoD ??= new PinkBladeOfDestruciton();
        set => _PBoD = value;
    }
    private static PinkBladeOfDestruciton _PBoD;
    private static CoreQOM QOM
    {
        get => _QOM ??= new CoreQOM();
        set => _QOM = value;
    }
    private static CoreQOM _QOM;
    private static CoreToD TOD
    {
        get => _TOD ??= new CoreToD();
        set => _TOD = value;
    }
    private static CoreToD _TOD;
    private static MysteriousEgg Egg
    {
        get => _Egg ??= new MysteriousEgg();
        set => _Egg = value;
    }
    private static MysteriousEgg _Egg;
    private static CoreSummer Coll
    {
        get => _Coll ??= new CoreSummer();
        set => _Coll = value;
    }
    private static CoreSummer _Coll;
    private static Borgars Borgars
    {
        get => _Borgars ??= new Borgars();
        set => _Borgars = value;
    }
    private static Borgars _Borgars;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetDoT();

        Core.SetOptions(false);
    }

    private string[] Extras =
    {
        "Dragon of Time Horns",
        "Dragon of Time Horns + Ponytail",
        "Dragon of Time Wings + Tail",
    };

    public void GetDoT(bool rankUpClass = true, bool doExtra = true, bool DotArmorOnly = false)
    {
        if (doExtra ? Core.CheckInventory(Extras) : Core.CheckInventory("Dragon of Time"))
            return;

        Story.PreLoad(this);

        // Acquiring Ancient Secrets 7716
        if (!Story.QuestProgression(7716))
        {
            Core.EnsureAccept(7716);

            Core.EquipClass(ClassType.Farm);
            Bot.Quests.UpdateQuest(4614);
            Core.KillMonster("mummies", "Enter", "Spawn", "Mummy", "Lost Hieroglyphic", 30, false);

            Farm.LoremasterREP(4);

            Core.BuyItem("librarium", 651, "Myths of Lore");

            Core.KillMonster("timelibrary", "FrameAQ", "Left", "*", "Historia Page", 100, false);

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("kingcoal", "Frost King", "Frost King's Story", isTemp: false);

            Core.KillMonster("baconcatyou", "Enter", "Spawn", "*", "Your Own Memories", isTemp: false, publicRoom: true);
            Story.ChainQuest(7716);
            Bot.Wait.ForPickup("*");
            Core.ToBank("Dragon of Time Helm", "Dragon of Time Ponytail");
        }

        // Time to Train Yourself 7717
        if (!Story.QuestProgression(7717))
        {
            Core.EnsureAccept(7717);

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster(
                "dragonchallenge",
                "Desoloth the Final",
                "Desoloth's Destructive Aura",
                isTemp: false,
                publicRoom: true
            );

            Bot.Quests.UpdateQuest(899);
            Core.HuntMonster("blindingsnow", "Nythera", "Nythera's Patience", isTemp: false);

            Core.AddDrop("Key of Greed");
            Core.HuntMonster("greed", "Goregold", "Goregold's Luck", isTemp: false, publicRoom: true);
            Core.HuntMonster("darkplane", "Victorious", "Victorious's Dignity", isTemp: false);

            Core.KillTrigoras("Trigoras's Tenacity", 3, Phase: 2);

            Story.ChainQuest(7717);
            Bot.Wait.ForPickup("*");
            Core.ToBank("Dragon of Time Runes", "Dragon of Time Wings", "Dragon of Time Tail");
        }

        // Do You Have the Time? 7718
        if (!Story.QuestProgression(7718))
        {
            Farm.Experience(31);
            Core.EnsureAccept(7718);

            GBoF.GetGBoF();

            PBoD.GetPBoD();

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("underworld", "Laken", "Cross-Era Stabilizer", isTemp: false);

            if (!Core.CheckInventory("Chronomancer's Codex"))
            {
                Core.HuntMonster("mqlesson", "Dragonoid", "Dragonoid of Hours", isTemp: false);
                Core.HuntMonster("timespace", "Chaos Lord Iadoa", "Chronomancer's Codex", isTemp: false);
            }

            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("arena", "Timestream Rider", "Timestream String", 100, false);

            Story.ChainQuest(7718);
            Bot.Wait.ForPickup("*");
            Core.ToBank("Dragon of Time FangBlade", "Dual Dragon of Time FangBlades");
        }

        // Through the Wormhole 7719
        if (!Story.QuestProgression(7719))
        {
            Farm.Experience(40);
            Core.EnsureAccept(7719);

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster(
                "cathedral",
                "Incarnation of Time",
                "Time Loop Broken",
                isTemp: false,
                publicRoom: true
            );

            Core.EquipClass(ClassType.Farm);
            Core.KillMonster("portalwar", "r4", "Right", "*", "Anomaly Silenced", 100, false);

            Core.HuntMonster("portalmaze", "ChronoLord", "Chronolord Stopped", 50, false);

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("ubear", "Cornholio", "Is This a Wormhole?", isTemp: false);

            Core.EnsureComplete(7719);
        }

        // If we are only getting the armor, we can return here.
        if (DotArmorOnly)
            return;

        // Rend 7720
        if (!Story.QuestProgression(7720))
        {
            Farm.Experience(60);
            Core.EnsureAccept(7720);

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("lairdefend", "Dragon Summoner", "Dimensional Dragon Portal", 2, false);
            Core.HuntMonster("bosschallenge", "Grievous Inbunche", "Brutal Slash Studied", 10, isTemp: false, publicRoom: true);
            Core.KillMonster("hydrachallenge", "h90", "Left", "*", "Epic Hydra Fang", 125, false);

            Story.ChainQuest(7720);
            Bot.Wait.ForPickup("*");
            Core.ToBank("Dragon of Time Daggers", "Dragon of Time Cleaver");
        }

        // Confluence of Fates 7721
        if (!Story.QuestProgression(7721))
        {
            Farm.Experience(60);
            Core.EnsureAccept(7721);

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("ivoliss", "Ivoliss", "Sword of Voids", isTemp: false);
            Bot.Wait.ForPickup("Sword of Voids");

            Darkon.FarmReceipt(100);

            QOM.TheReshaper();
            if (!Core.CheckInventory("Semiramis Feather"))
            {
                Core.AddDrop("Semiramis Feather");
                // Take Down Terrane 6286
                Core.EquipClass(ClassType.Solo);
                Core.EnsureAccept(6286);
                Core.KillMonster("guardiantree", "r12", "Left", "Terrane");
                Core.EnsureComplete(6286);
                Bot.Wait.ForPickup("Semiramis Feather");
            }

            Core.EquipClass(ClassType.Farm);
            Core.KillMonster("aqw3d", "r13", "Bottom", "*", "Cross-Dimensional Weapons", 300, isTemp: false, publicRoom: true);
            TOD.ShiftingPyramid();
            if (!Core.CheckInventory("Starlight Singularity"))
            {
                Core.AddDrop("Starlight Singularity");
                // Serpent of the Stars 5186
                Core.EnsureAccept(5186);
                Core.EquipClass(ClassType.Solo);
                Core.HuntMonster("whitehole", "Mehensi Serpent", "Mehen Slain");
                Core.EnsureComplete(5186);
                Bot.Wait.ForPickup("Starlight Singularity");
            }

            Coll.Collector();
            Adv.BuyItem("collection", 325, 56815, shopItemID: 6448);
            Story.ChainQuest(7721);

            Bot.Wait.ForPickup("*");
            Core.ToBank("Ascended Dragon of Time Runes", "Runes Of Time");
        }

        // Dragon's Will 7722
        if (!Story.QuestProgression(7722))
        {
            Farm.Experience(65);
            Core.EnsureAccept(7722);

            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("moonlab", "Slime Mold", "Unyielding Slime", 300, false);

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("bosschallenge", "Mutated Void Dragon", "Omnipotent Cells", 20, false, publicRoom: true);
            if (!Core.CheckInventory("Dragon's Plasma", 20))
            {
                //why the fuck was the class buffed!?
                InventoryItem? usethis = Bot.Inventory.Items.Concat(Bot.Bank.Items).FirstOrDefault(n => n.Name.Equals("Yami no Ronin") || n.Name.StartsWith("Chaos Slayer"));
                if (usethis != null)
                {
                    Core.Equip(usethis.ID);
                    Core.Equip(Core.FarmGear);
                    if (usethis.Name.Equals("Yami no Ronin"))
                        Bot.Skills.StartAdvanced("1 | 4");
                }
                else
                    Core.EquipClass(ClassType.Dodge);
                Core.HuntMonster("underlair", "ArchFiend Dragonlord", "Dragon's Plasma", 20, false);
            }
            Core.JumpWait();
            Core.EquipClass(ClassType.Solo);

            Core.HuntMonster("chaoskraken", "Chaos Kraken", "Chaotic Invertebrae", 20, false, publicRoom: true);

            Bot.Quests.UpdateQuest(9, 159);
            Core.HuntMonster("towerofdoom9", "Dread Fang", "Cryostatic Essence", 20, false, publicRoom: true);

            Core.HuntMonster("castleroof", "Ultra Chaos Dragon", "Salvaged Chaos Dragon Biomass", 20, false, publicRoom: true);
            Story.ChainQuest(7722);
            Bot.Wait.ForPickup("*");
            Core.ToBank("Dragon of Time Reaper", "Dragon of Time WingBlade");
        }

        // Burning Fates 7723
        if (!Story.QuestProgression(7723))
        {
            Farm.Experience(70);
            Core.EnsureAccept(7723);

            Core.EquipClass(ClassType.Farm);
            Core.KillMonster("volcano", "r10", "Left", "Fire Imp", "Fire Essence", 3000, false);

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("charredplains", "Akriloth", "Akriloth's Flametongue", 100, false, publicRoom: true);
            Core.HuntMonster("ultraphedra", "Ultra Phedra", "Immortal Embers", 50, false, publicRoom: true);
            Core.HuntMonster("thevoid", "Reaper", "Ashes from the Void Realm", 50, false, publicRoom: true);
            Story.ChainQuest(7723);
            Bot.Wait.ForPickup("*");
            Core.ToBank("Ascended Dragon of Time");
        }

        // Hero's Heartbeat 7724
        if (!Story.QuestProgression(7724) || !Core.CheckInventory("Dragon of Time"))
        {
            Farm.Experience(75);
            if (!Core.CheckInventory("Blade of Awe"))
                Farm.BladeofAweREP(6, true);
            Core.EnsureAccept(7724);

            Egg.GetMysteriousEgg();

            Core.EquipClass(ClassType.Solo);
            Bot.Quests.UpdateQuest(3880);
            Core.KillMonster("chaoslord", "r2", "Left", "*", "Conquered Past", isTemp: false, publicRoom: true);
            Bot.Quests.UpdateQuest(10, 159);
            Core.HuntMonster("towerofdoom10", "Slugbutter", "Slugbutter Trophy", 100, false, publicRoom: true);
            Core.EquipClass(ClassType.Dodge);
            Core.HuntMonster("icewing", "Warlord Icewing", "Icewing's Laurel", 30, false, publicRoom: true);
            Story.ChainQuest(7724);
            Bot.Wait.ForPickup("Dragon of Time");
            Bot.Wait.ForPickup("*");
            Core.ToBank("Ascended Dragon of Time Morph", "Ascended Dragon of Time Wings");
        }

        if (rankUpClass)
            Adv.RankUpClass("Dragon of Time");

        // I'm Loving It My Way 7725
        if (doExtra)
        {
            Borgars.StoryLine();
            Farm.Experience(75);
            Core.AddDrop("Burger Buns");
            Core.EquipClass(ClassType.Solo);
            Bot.Drops.Add(
                Extras.Where(x => Core.CheckInventory(x, toInv: false)).Select(x => x).ToArray()
            );
            foreach (
                string Item in Core.QuestRewards(7725)
                    .Where(x => !Core.CheckInventory(x, toInv: false))
                    .Select(x => x)
                    .ToArray()
            )
            {
                Core.EnsureAccept(7725);
                if (!Core.CheckInventory("Borgar"))
                {
                    while (!Bot.ShouldExit && !Core.CheckInventory("Burger Buns", 5))
                    {
                        // Burglinster's Revenge 7522
                        Core.EnsureAccept(7522);
                        Core.HuntMonster("borgars", "Burglinster", "Burglinster Cured");
                        Core.EnsureComplete(7522);
                        Bot.Wait.ForPickup("Burger Buns");
                    }
                    Core.BuyItem("borgars", 1884, 54650, shopItemID: 7387);
                }

                Core.EnsureCompleteChoose(7725, Extras);
                Bot.Wait.ForPickup("*");
            }
            Core.ToBank(
                "Dragon of Time Horns",
                "Dragon of Time Horns + Ponytail",
                "Dragon of Time Wings + Tail"
            );
        }
    }

    public void DotArmor()
    {
        if (Core.CheckInventory("Dragon of Time Armor"))
            return;

        if (!Core.isCompletedBefore(7719))
            GetDoT(false, false, true);

        Farm.Experience(40);
        Core.EnsureAccept(7719);

        Core.EquipClass(ClassType.Solo);
        Core.HuntMonster(
            "cathedral",
            "Incarnation of Time",
            "Time Loop Broken",
            isTemp: false,
            publicRoom: true
        );

        Core.EquipClass(ClassType.Farm);
        Core.KillMonster("portalwar", "r4", "Right", "*", "Anomaly Silenced", 100, false);

        Core.HuntMonster("portalmaze", "ChronoLord", "Chronolord Stopped", 50, false);

        Core.EquipClass(ClassType.Solo);
        Core.HuntMonster("ubear", "Cornholio", "Is This a Wormhole?", isTemp: false);

        Core.EnsureComplete(7719);
        Bot.Wait.ForDrop("Dragon of Time Armor");
        Bot.Wait.ForPickup("Dragon of Time Armor");
    }
}
