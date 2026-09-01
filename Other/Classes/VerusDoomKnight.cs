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
//cs_include Scripts/Prototypes/InfernalArenaLW.cs

using System.Dynamic;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models.Auras;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Shops;
using Skua.Core.Models.Skills;
using System.Linq;

public class VerusDoomKnightClass
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
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
    private static SepulchuresOriginalHelm SOH
    {
        get => _SOH ??= new SepulchuresOriginalHelm();
        set => _SOH = value;
    }
    private static SepulchuresOriginalHelm _SOH;
    private static ArchDoomKnight ADK
    {
        get => _ADK ??= new ArchDoomKnight();
        set => _ADK = value;
    }
    private static ArchDoomKnight _ADK;
    private static SRoD SRoD
    {
        get => _SRoD ??= new SRoD();
        set => _SRoD = value;
    }
    private static SRoD _SRoD;
    private static TerminaTempleMerge TTMerge
    {
        get => _TTMerge ??= new TerminaTempleMerge();
        set => _TTMerge = value;
    }
    private static TerminaTempleMerge _TTMerge;
    private static DoomPirateHaulMerge DPHM
    {
        get => _DPHM ??= new DoomPirateHaulMerge();
        set => _DPHM = value;
    }
    private static DoomPirateHaulMerge _DPHM;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static InfernalArenaLW Infernal { get => _Infernal ??= new(); set => _Infernal = value; }
    private static InfernalArenaLW _Infernal;

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
            Core.HuntMonster(
                "lumafortress",
                "Corrupted Luma",
                "Doom Worshipper's Blade Of Doom",
                isTemp: false
            );
            Core.Logger(
                " \"Empress' ShadowCloak\" 's Droprate is fairly low... so dont complain if it takes \"hours\" according to reddit... "
            );
            Core.HuntMonster("innershadows", "Krahen", "Empress' ShadowCloak", isTemp: false);
            Bot.Quests.UpdateQuest(7646);
            Core.HuntMonster(
                "techfortress",
                "MechaVortrix",
                "Cybernetic Doom Blade",
                isTemp: false
            );
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
            Core.HuntMonster(
                "brightshadow",
                "Shadowflame Paladin",
                "Shadowflame Spike",
                150,
                false
            );
            Core.HuntMonster("fiendshard", "Paladin Fiend", "Light Fiend Horn", 150, false);
            Core.HuntMonster(
                "legionarena",
                "Dark Legion Paladin",
                "Underworld Soul Glow",
                50,
                false
            );
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
            Core.Logger(
                "The map \"Wanders\", is a bit broke,\n"
                    + "it will take a minute to hunt the mosnter"
            );
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
            Core.HuntMonsterQuest(
                9416,
                ("ultraalteon", "Ultra Chaos Alteon", ClassType.Solo),
                ("ebondungeon", "Dethrix", ClassType.Solo),
                ("shadowstrike", "Sepulchuroth", ClassType.Solo),
                ("ultradrakath", "Champion of Chaos", ClassType.Solo),
                ("ebilcorphq", "Gravelyn", ClassType.Solo),
                ("shadowvoid", "Fragment of Doom", ClassType.Solo)
            );
        }

        // Doom Spikes (9418)
        if (!Story.QuestProgression(9418))
        {
            Farm.Experience(80);
            Core.EnsureAccept(9418);
            Adv.GearStore(EnhAfter: true);
            Core.KillDoomKitten("Doomkitten's Molar", 20, false);
            Adv.GearStore(true, true);
            Infernal.FarmDeadlyDuo("Deadly Duo's Decayed Denture", 10);

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
        // FIX: previously checked 9419 which caused this to rerun whenever Unleash Doom wasn't done.
        if (!Story.QuestProgression(9414))
        {
            Core.EnsureAccept(9414);
            SRoD.ShadowReaperOfDoom();
            SOH.DoAll();
            ADK.AMeansToAnEnd(HelmOnly: true);
            TTMerge.BuyAllMerge("Dragonlord of Evil");
            DPHM.BuyAllMerge("DoomTech DoomKnight");

            //Ensure Everything is unbanked.
            Core.Unbank(
                "Dragonlord of Evil",
                "DoomTech DoomKnight",
                "Arch DoomKnight Helm",
                "Sepulchure's Original Helm",
                "ShadowReaper Of Doom"
            );
            Core.EnsureComplete(9414);
        }

        // Unleash Doom (9419)
        if (!Story.QuestProgression(9419))
        {
            Core.EquipClass(ClassType.Farm);
            Core.EnsureAccept(9419);
            Core.HuntMonster("citadelruins", "Inquisitor Hobo", "Inquisitor Bones", 1000000, false);

            // FIX: deltavlab -> deltavcore (Refined Metal)
            Core.HuntMonster("deltavcore", "Pistol Guard", "Refined Metal", 1000000, false);

            Core.HuntMonster("etherwardes", "Earth Dragon Warrior", "Dragon Skin", 1000000, false);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster(
                "necrocavern",
                "Chaos Vordred",
                "PaladinSlayer's Skull",
                100000,
                false
            );
            Core.EnsureComplete(9419);
        }

        Core.BuyItem("terminatemple", 2343, "Verus DoomKnight");

        if (rankup)
            Adv.RankUpClass("Verus DoomKnight");
    }

    void VoTSSolo()
    {

        // Define the possible solo classes
        string[] PossibleSoloClasses = new[]
        {
            "Chrono ShadowHunter",
            "Chrono ShadowSlayer",
            "Chaos Avenger",
            "Void Highlord (IoDA)",
            "Void Highlord",
            "Verus DoomKnight",
            "Hollowborn Vindicator",
            "Lich",
            "ArchPaladin",
            "Lord of Order",
            "StoneCrusher",
            "Dragon of Time",
            "Unundead Goat",
        };

        // Find the first available class in inventory or bank
        string? selectedClass = PossibleSoloClasses.FirstOrDefault(className =>
            Bot.Inventory.Items.Any(item => item.Name == className)
            || Bot.Bank.Items.Any(item => item.Name == className)
        );

        if (string.IsNullOrWhiteSpace(selectedClass))
        {
            // Warn the user but fallback to currently equipped class
            string? equippedClass = Bot.Player.CurrentClass?.Name;
            Core.Logger(
                $"No preferred solo class found in inventory or bank.\n"
                    + $"Preferred options: ({string.Join(", ", PossibleSoloClasses)})\n"
                    + $"Using currently equipped class: {equippedClass}. This may not be optimal.\n"
            );

            selectedClass = equippedClass;
            if (string.IsNullOrWhiteSpace(selectedClass))
            {
                Core.Logger("No class is currently equipped; aborting SeaVoice.");
                return;
            }
        }
        else
        {
            Core.Logger($"Soloing \"Voice of the Sea\" with {selectedClass}");
        }

        // FIX: if class is in bank, unbank it before trying to use it
        Core.Unbank(selectedClass);

        Adv.GearStore(EnhAfter: true);
        Adv.SmartEnhance(selectedClass);

        // Call the KillThing method with the specified parameters
        KillThing(
            map: "seavoice",
            mobMapID: 1,
            itemUsed: 78994,
            Class: selectedClass,
            item: "Maw of the Sea",
            quant: 10,
            isTemp: false
        );

        Adv.GearStore(true, true);
    }

  
    public void KillThing(
        string map,
        int mobMapID,
        int itemUsed,
        string Class,
        string item,
        int quant = 1,
        bool isTemp = false
    )
    {
        string? classFromPlayer = Bot.Player.CurrentClass?.Name;

        var itemToEnhance = Bot
            .Inventory?.Items?.FirstOrDefault(x =>
                x?.Equipped == true && Adv.WeaponCatagories.Contains(x.Category)
            )
            ?.Name;

        // if (itemToEnhance != null)
        //     Adv.EnhanceItem(
        //         itemToEnhance,
        //         EnhancementType.Lucky,
        //         wSpecial: WeaponSpecial.Awe_Blast
        //     );

        string? classNameToUse = Class ?? classFromPlayer;
        if (string.IsNullOrWhiteSpace(classNameToUse))
        {
            Core.Logger("KillThing aborted: no class specified and player has no current class.");
            return;
        }

        // FIX: actually equip the class you selected
        Core.Equip(classNameToUse);

        Core.Join(map);
        Bot.Wait.ForMapLoad(map);
        Adv.BuyItem("seavoice", 2320, "Vigil", 1000, 12023);
        Core.Equip(itemUsed);
        Core.Logger($"{itemUsed} [Vigil] Equiped? {Bot.Inventory?.IsEquipped("Vigil")}");
        // Move to mob cell and set respawn
        if (Bot.Player.Cell != "r2")
            Core.Jump("r2", "Left");
        Bot.Player.SetSpawnPoint();

        while (
            !Bot.ShouldExit
            && (isTemp ? !Bot.TempInv.Contains(item, quant) : !Core.CheckInventory(item, quant))
        )
        {
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Bot.Player.Cell != "r2")
                Core.Jump("r2", "Left");

            // === Handle Oxidize aura (use Vigil to cleanse) ===
            while (!Bot.ShouldExit && Bot.Self.Auras.Any(a => a.Name == "Oxidize") && !Bot.Self.Auras.Any(a => a.Name == "Vigil"))
            {
                if (Bot.Skills.CanUseSkill(5))
                    Bot.Skills.UseSkill(5);
                Core.Sleep(500);
                if (Bot.Self.Auras.Any(a => a.Name == "Vigil"))
                    break;
            }

            // === Attack phase ===
            Bot.Combat.Attack("*");
            Core.Sleep(500);
        }

        Core.Logger($"KillThing completed for {item} ({quant}).");
    }

}
