/*
name: Verus DoomKnight Class
description: This script will farm Verus DoomKnight Class.
tags: versus, verus, doomknight, vdk, class
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreDailies.cs

//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs

//cs_include Scripts/Evil/SepulchuresOriginalHelm.cs
//cs_include Scripts/Evil/ADK.cs
//cs_include Scripts/Other/Weapons/ShadowReaperOfDoom.cs
//cs_include Scripts/Good/BLOD/CoreBLOD.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Other/Classes/Necromancer.cs

//cs_include Scripts/Evil/NSoD/CoreNSOD.cs

//cs_include Scripts/Other/MergeShops/TerminaTempleMerge.cs

//cs_include Scripts/Seasonal/TalkLikeaPirateDay/DoomPirateStory.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/MergeShops/DoomPirateHaulMerge.cs

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Skills;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Shops;
using Newtonsoft.Json;
using System.Dynamic;
using Skua.Core.Models.Auras;

public class VerusDoomKnightClass
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static SepulchuresOriginalHelm SOH { get => _SOH ??= new SepulchuresOriginalHelm(); set => _SOH = value; }
    private static SepulchuresOriginalHelm _SOH;
    private static ArchDoomKnight ADK { get => _ADK ??= new ArchDoomKnight(); set => _ADK = value; }
    private static ArchDoomKnight _ADK;
    private static SRoD SRoD { get => _SRoD ??= new SRoD(); set => _SRoD = value; }
    private static SRoD _SRoD;
    private static TerminaTempleMerge TTMerge { get => _TTMerge ??= new TerminaTempleMerge(); set => _TTMerge = value; }
    private static TerminaTempleMerge _TTMerge;
    private static DoomPirateHaulMerge DPHM { get => _DPHM ??= new DoomPirateHaulMerge(); set => _DPHM = value; }
    private static DoomPirateHaulMerge _DPHM;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        GetClass();
        Core.SetOptions(false);
    }

    public void GetClass(bool rankup = true)
    {
        if (Core.CheckInventory("Verus DoomKnight"))
            return;

        Story.PreLoad(this);

        Farm.Experience(80);

        Core.EquipClass(ClassType.Solo);
        // Body, Soul and, Domination (9411)
        if (!Story.QuestProgression(9411))
        {
            Core.EnsureAccept(9411);
            Core.HuntMonster("underrealm", "Fear", "Fear's Bones", 13, false);
            Core.HuntMonster("brainmeat", "Brain Matter", "Gray Matter", 13, false);
            Bot.Quests.UpdateQuest(8777);
            Core.HuntMonster("titanattack", "Titanic DoomKnight", "Titanic Spine", 13, false);
            Core.HuntMonsterMapID("valleyofdoom", 25, "Doom Knight Plating", 13, false);
            Core.EnsureComplete(9411);
        }

        // Of the Same Cloak (9412)
        if (!Story.QuestProgression(9412))
        {
            Farm.Experience(50);
            Core.EnsureAccept(9412);
            Core.HuntMonsterMapID("necrodungeon", 47, "The Mask of the Skulls", isTemp: false);
            Core.HuntMonster("lumafortress", "Corrupted Luma", "Doom Worshipper's Blade Of Doom", isTemp: false);
            Core.Logger(" \"Empress' ShadowCloak\" 's Droprate is fairly low... so dont complain if it takes \"hours\" according to reddit... ");
            Core.HuntMonster("innershadows", "Krahen", "Empress' ShadowCloak", isTemp: false);
            Bot.Quests.UpdateQuest(7646);
            Core.HuntMonster("techfortress", "MechaVortrix", "Cybernetic Doom Blade", isTemp: false);
            Core.GhostItem(55823, "Kyger", 1, false, ItemCategory.Pet, "Time for training!", 1);
            Bot.Quests.UpdateQuest(7650);
            Core.HuntMonsterMapID("stonewooddeep", 16, "Asherion Armor", isTemp: false);
            Core.EnsureComplete(9412);
        }

        // Refracted Light (9413)
        if (!Story.QuestProgression(9413))
        {
            Farm.Experience(60);
            Core.EnsureAccept(9413);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("brightshadow", "Shadowflame Paladin", "Shadowflame Spike", 150, false);
            Core.HuntMonster("fiendshard", "Paladin Fiend", "Light Fiend Horn", 150, false);
            Core.HuntMonster("legionarena", "Dark Legion Paladin", "Underworld Soul Glow", 50, false);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("noxustower", "General Goldhammer", "Gold Hammer Chip", isTemp: false);
            Core.HuntMonster("chaoslab", "Chaos Artix", "Shimmering Tentacle", 20, false);
            Core.EnsureComplete(9413);
        }

        // Life Carve (9417)
        if (!Story.QuestProgression(9417))
        {
            Farm.Experience(70);
            Farm.EvilREP();
            Bot.Quests.UpdateQuest(3773);
            Core.EnsureAccept(9417);
            Core.Logger("The map \"Wanders\", is a bit broke,\n" +
            "it will take a minute to hunt the mosnter");
            Core.HuntMonsterMapID("wanders", 46, "Trace of Light", 8, false); //i hate this map
            Core.HuntMonster("lightguardwar", "Extreme Noxus", "Trace of Dark", 8, false);
            Core.HuntMonster("eternalchaos", "Bandit Drakath", "Trace of Wind", 8, false);
            Core.HuntMonster("quibblehunt", "Entropy Dragon", "Trace of Earth", 8, false);
            Core.HuntMonster("crashsite", "ProtoSartorium", "Trace of Energy", 8, false);
            Core.HuntMonster("northlands", "Aisha's Drake", "Trace of Ice", 8, false);
            Core.HuntMonster("deepchaos", "Kathool", "Trace of Water", 8, false);
            Core.HuntMonster("drakonnan", "Ultra Drakonnan", "Trace of Fire", 8, false);
            Core.HuntMonster("battlefowl", "Zeuster Projection", "Trace of Bacon", isTemp: false);
            Core.EnsureComplete(9417);
        }

        // Soul Fracture (9416)
        if (!Story.QuestProgression(9416))
        {
            Core.HuntMonsterQuest(9416,
            ("ultraalteon", "Ultra Chaos Alteon", ClassType.Solo),
            ("ebondungeon", "Dethrix", ClassType.Solo),
            ("shadowstrike", "Sepulchuroth", ClassType.Solo),
            ("ultradrakath", "Champion of Chaos", ClassType.Solo),
            ("ebilcorphq", "Gravelyn", ClassType.Solo),
            ("shadowvoid", "Fragment of Doom", ClassType.Solo));
        }

        // Doom Spikes (9418)
        if (!Story.QuestProgression(9418))
        {
            Farm.Experience(80);
            Core.EnsureAccept(9418);
            Adv.GearStore();
            Core.KillDoomKitten("Doomkitten's Molar", 20, false);
            Adv.GearStore(true, true);
            if (!Core.CheckInventory("Deadly Duo's Decayed Denture", 10))
            {
                Core.Logger("InfernalArena is a **SOLO ONLY** map!");
                Adv.GearStore();
                Core.BossClass(Core.CheckInventory(new[] { "Void Highlord", "Void Highlord (IoDA)" }, any: true)
                ? (Core.CheckInventory("Void Highlord (IoDA)")
                ? "Void Highlord (IoDA)" : "Void Highlord")
                : "ArchPaladin");
                Core.JumpWait();
                Core.Sleep();
                Core.HuntMonster("infernalarena", "Deadly Duo", "Deadly Duo's Decayed Denture", 10, false);
                Core.JumpWait();
                Adv.GearStore(true, true);
            }

            if (!Core.CheckInventory("Maw of the Sea", 10))
            {
                VoTSSolo();
            }

            int energyNeeded = 0;

            // Calculate energy based on missing items
            if (!Core.CheckInventory("Xyfrag's Slimy Tooth", 5))
                energyNeeded += 250;

            if (!Core.CheckInventory("Nerfkitten's Fang", 3))
                energyNeeded += 150;

            // Hunt for Void Energy if required
            if (energyNeeded > 0)
                Core.HuntMonster("thevoid", "Ninja", "Void Energy", energyNeeded, isTemp: false);

            // Purchase required items.
            Adv.BuyItem("thevoid", 1406, "Xyfrag's Slimy Tooth", 5);
            Adv.BuyItem("thevoid", 1406, "Nerfkitten's Fang", 3);

            Core.EnsureComplete(9418);
        }

        // Necrotic Blade (9414)
        if (!Story.QuestProgression(9414))
        {
            Core.EnsureAccept(9414);
            SRoD.ShadowReaperOfDoom();
            SOH.DoAll();
            ADK.AMeansToAnEnd(HelmOnly: true);
            TTMerge.BuyAllMerge("Dragonlord of Evil");
            DPHM.BuyAllMerge("DoomTech DoomKnight");

            //Ensure Everything is unbanked.
            Core.Unbank("Dragonlord of Evil", "DoomTech DoomKnight", "Arch DoomKnight Helm", "Sepulchure's Original Helm", "ShadowReaper Of Doom");
            Core.EnsureComplete(9414);
        }

        // Unleash Doom (9419)
        if (!Story.QuestProgression(9419))
        {
            Core.EquipClass(ClassType.Farm);
            Core.EnsureAccept(9419);
            Core.HuntMonster("citadelruins", "Inquisitor Hobo", "Inquisitor Bones", 1000000, false);
            Core.HuntMonster("deltavlab", "Pistol Guard", "Refined Metal", 1000000, false);
            Core.HuntMonster("etherwardes", "Earth Dragon Warrior", "Dragon Skin", 1000000, false);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("necrocavern", "Chaos Vordred", "PaladinSlayer's Skull", 100000, false);
            Core.EnsureComplete(9419);
        }

        Core.BuyItem("terminatemple", 2343, "Verus DoomKnight");

        if (rankup)
            Adv.RankUpClass("Verus DoomKnight");
    }



    void VoTSSolo()
    {
        // Define the possible solo classes
        string[] PossibleSoloClasses = new[] { "Chaos Avenger", "Verus Doomknight", "Void Highlord", "ArchPaladin" };

        if (!Core.CheckInventory(PossibleSoloClasses, any: true))
            Core.Logger("no Soloing classes found stopping (go get AP atleast and rerun)", stopBot: true);

        // Find the first available class in inventory or bank
        string? selectedClass = PossibleSoloClasses.FirstOrDefault(className =>
            Bot.Inventory.Items.Any(item => item.Name == className) ||
            Bot.Bank.Items.Any(item => item.Name == className)
        );

        Core.Logger($"Soloing \"Voice of the Sea\" with {selectedClass}");

        Adv.GearStore();

        // Adv.SmartEnhance(selectedClass);

        // Call the KillThing method with the specified parameters
        KillThing(
            map: "seavoice",
            mobMapID: 1,
            targetAuraName: "Oxidize",
            ItemUsed: 78994,
            Class: selectedClass,
            item: "Maw of the Sea",
            quant: 10,
            isTemp: false
        );
        Adv.GearStore(true, true);
    }

    public void KillThing(string? map = null, int mobMapID = 1, string? targetAuraName = null, int ItemUsed = 1, string? Class = null, string? item = null, int quant = 1, bool isTemp = false)
    {
        Adv.BuyItem("seavoice", 2320, "Vigil", 1000, 12023);

        Core.Join(map);
        Bot.Wait.ForMapLoad(map!);
        Bot.Wait.ForTrue(() => Bot.Player.Loaded, 20);

        Core.Logger($"map: {map}");
        Core.Logger($"mobMapID: {mobMapID}");
        Core.Logger($"targetAuraName: {targetAuraName}");
        Core.Logger($"ItemUsed: {ItemUsed} [Vigil]");
        Core.Logger($"Class: {Class}");
        Core.Logger($"item: {item}");
        Core.Logger($"quant: {quant}");
        Core.Logger($"isTemp: {isTemp}");

        Core.Equip(Class!);
        if (Class == "Void Highlord")
            Bot.Skills.StartAdvanced("Void HighLord", true, ClassUseMode.Def);
        Core.Equip(ItemUsed);
        Core.Logger($"{ItemUsed} [Vigil] Equiped? {Bot.Inventory.IsEquipped("Vigil")}");

        Monster? mob = Bot.Monsters.MapMonsters.FirstOrDefault(m => m.MapID == mobMapID);
        if (targetAuraName != null)
        {
            Aura? targetAura = Bot.Target.Auras.Concat(Bot.Self.Auras).FirstOrDefault(a => a.Name == targetAuraName);
        }

        if (Bot.Player.Cell != mob!.Cell)
            Core.Jump(mob.Cell);
        Bot.Player.SetSpawnPoint();

        while (!Bot.ShouldExit && item != null && isTemp ? !Bot.TempInv.Contains(item!, quant) : !Core.CheckInventory(item!, quant))
        {
            //Check if/move to /in mob cell && Bot.Player.Alive)
            if (Bot.Player.Cell != mob.Cell)
                Core.Jump(mob.Cell);

            #region  UltraSpeaker
            if (map == "ultraspeaker")
            {
                while (!Bot.ShouldExit && !Bot.Player.Alive)
                    Core.Sleep();
            }
            #endregion

            if (targetAuraName != null)
                AuraHandling(targetAuraName);

            if (Bot.Player.Alive && !Bot.Self.HasActiveAura(targetAuraName!) && !Bot.Target.HasActiveAura(targetAuraName!))
                Bot.Combat.Attack(mob);
            Core.Sleep();

            if (isTemp ? Bot.TempInv.Contains(item!, quant) : Core.CheckInventory(item, quant))
            {
                break;
            }
        }

        void AuraHandling(string? targetAuraName)
        {
            foreach (Aura A in Bot.Target.Auras.Concat(Bot.Self.Auras))
            {
                if (targetAuraName == null)
                    continue;

                switch (A.Name)
                {
                    case "Oxidize":
                        while (!Bot.ShouldExit && !Bot.Self.HasActiveAura("Vigil"))
                        {
                            UsePotion();
                            Core.Sleep();

                            // Check if targetAura is not null before accessing its SecondsRemaining() method
                            // Assuming `targetAura` is the aura you're referring to
                            if (Bot.Self.HasActiveAura("Vigil"))
                            {
                                Core.Logger($"Vigil Active!");
                                break;
                            }
                        }
                        break;

                    case null:
                        break;
                }
            }
        }

        void UsePotion()
        {
            var skill = Bot.Flash.GetArrayObject<dynamic>("world.actions.active", 5);
            if (skill == null) return;
            Bot.Flash.CallGameFunction("world.testAction", JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(skill))!);
        }
    }

}
