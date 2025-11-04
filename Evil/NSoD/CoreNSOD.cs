/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Good/BLOD/CoreBLOD.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Other/Classes/Necromancer.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Monsters;
using Skua.Core.Options;

public class CoreNSOD
{
    // private bool OptimizeInv = true;

    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreDailies Daily { get => _Daily ??= new CoreDailies(); set => _Daily = value; }
    private static CoreDailies _Daily;

    private static CoreBLOD BLOD { get => _BLOD ??= new CoreBLOD(); set => _BLOD = value; }
    private static CoreBLOD _BLOD;
    private static CoreSDKA SDKA { get => _SDKA ??= new CoreSDKA(); set => _SDKA = value; }
    private static CoreSDKA _SDKA;
    private static Necromancer Necro { get => _Necro ??= new Necromancer(); set => _Necro = value; }
    private static Necromancer _Necro;
    private static BattleUnder BattleUnder { get => _BattleUnder ??= new BattleUnder(); set => _BattleUnder = value; }
    private static BattleUnder _BattleUnder;

    public string OptionsStorage = "NecroticSwordOfDoomOptions";
    public bool DontPreconfigure = true;
    public Option<bool> MaxStack = new("MaxStack", "Max Stack", "Max Stack Monster Essences in \"Retreive Void Auras\"\nRecommended setting: True", true);
    public Option<bool> PreFarm = new("PreFarm", "Pre Farm Materials", "Farm all requiered items before merging everything. Not recommended if you already did a merge yourself.\nRecommended setting: False", false);
    public Option<bool> GetSDKA = new("getSDKA", "Get SDKA first [Mem]", "If true, the bot will attempt to get SDKA first, so that it can use the fastest Void Aura farm available\nMember-Only\nRecommended setting: True", true);

    public string[] Essences =
    {
        "Astral Ephemerite Essence",
        "Belrot the Fiend Essence",
        "Black Knight Essence",
        "Tiger Leech Essence",
        "Carnax Essence",
        "Chaos Vordred Essence",
        "Dai Tengu Essence",
        "Unending Avatar Essence",
        "Void Dragon Essence",
        "Creature Creation Essence"
    };

    public void ScriptMain(IScriptInterface bot) => Core.RunCore();

    public void GetNSOD()
    {
        if (Core.CheckInventory("Necrotic Sword of Doom") && Core.HasWebBadge("Necrotic Sword of Doom"))
            return;

        if (!Core.CheckInventory("Necrotic Sword of Doom"))
        {
            if (!Core.CheckInventory("Necrotic Sword's Blade"))
            {
                Core.Logger("NSoD: Step #1/16: Barium (MineCrafting Daily)");
                Barium();
            }

            bool preFarmEnabled = Bot.Config?.Get<bool>("PreFarm") ?? false;

            if (preFarmEnabled && !Core.CheckInventory(new[] { "Necrotic Sword's Blade", "Necrotic Sword's Hilt", "Necrotic Sword's Aura" }, any: true))
            {
                Core.Logger("NSoD: PreFarm Steps:");
                PreFarmSteps();
            }
            else
                Core.Logger("NSoD: PreFarm Steps skipped, continuing with the farm.");

            Core.Logger("NSoD: Step #10/16: NSAura.");
            NSAura();
            Core.Logger("NSoD: Step #11/16: NSBlade.");
            NSBlade();
            Core.Logger("NSoD: Step #12/16: NSHilt.");
            NSHilt();
            Core.Logger("NSoD: Step #13/16: ULTRA Sepulchure for \"Doom Heart\"");
            Core.HuntMonster("sepulchurebattle", "ULTRA Sepulchure", "Doom Heart", isTemp: false, publicRoom: true);
            Core.Logger("NSoD: Step #14/16: Void Auras x800 to complete the merge");
            VoidAuras(800);

            Core.Logger("NSoD: Step #15/16: NSoD purchase");
            Core.BuyItem("shadowfall", 793, "Necrotic Sword of Doom");
            Bot.Wait.ForPickup("Necrotic Sword of Doom");
            Core.Logger("NSoD: Step #16/16: NSoD Enhancement");
        }

        if (!Core.isCompletedBefore(7652))
        {
            Core.Logger("Getting the NSOD character page badge");
            Core.EnsureAccept(7652);
            Core.HuntMonster("graveyard", "Skeletal Warrior", "Arcane Parchment");
            Core.EnsureComplete(7652);
        }

        if (!Core.CheckInventory(14474) && !Core.IsMember)
            Core.Logger("Congratulations on completing the longest farm in the game!!!", messageBox: true);
    }


    public void GetNBOD()
    {
        if (Core.CheckInventory("Necrotic Blade of Doom"))
            return;

        Core.Logger("NBOD: Step #1: Get NSoD");
        GetNSOD();

        Core.Logger("NBOD: Step #2: Void Auras x750");
        VoidAuras(750);

        if (!Core.CheckInventory("Void Essentia"))
        {
            Core.Logger("NBOD: Step #3: Kill Flibbitiestgibbet for \"Void Essentia\"");
            Core.Logger("Flibbitiestgibbet is a very tough monster, I hope you brought your army/butler/friends!");
            Core.KillMonster("voidflibbi", "Enter", "Spawn", "Flibbitiestgibbet", "Void Essentia", isTemp: false);
        }

        Core.Logger("NBOD: Step #4: Buy NBoD");
        Core.BuyItem("shadowfall", 793, "Necrotic Blade of Doom");

        Core.Logger("NBOD: Step #5: Reminder - Use AE's Buy-Back system to retrieve your NSoD", messageBox: true);
    }


    #region Void Auras

    public void VoidAuras(int quant = 7500)
    {
        if (Core.CheckInventory("Void Aura", quant))
            return;

        if (Bot.Config!.Get<bool>("GetSDKA") && Bot.Player.IsMember && !Core.CheckInventory(14474 /* Sepulchure's DoomKnight Armor */))
            SDKA.DoAll();

        Core.AddDrop("Void Aura");
        Core.FarmingLogger("Void Aura", quant);

        if (Core.CheckInventory(14474 /* Sepulchure's DoomKnight Armor */))
            CommandingShadowEssences(quant);

        if (Bot.Player.IsMember)
            GatheringUnstableEssences(quant);

        RetrieveVoidAuras(quant);
    }

    public void CommandingShadowEssences(int quant = 7500)
    {
        if (Core.CheckInventory("Void Aura", quant) || !Core.CheckInventory(14474))
            return;

        Bot.Options.AggroAllMonsters = false;
        Bot.Options.AggroMonsters = false;
        while (!Bot.ShouldExit && !Core.CheckInventory("Void Aura", quant))
        {
            Core.EnsureAccept(4439);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonsterMapID("shadowrealmpast", 11, "Malignant Essence", 3, false);

            Core.EquipClass(ClassType.Farm);
            Core.KillMonster("shadowrealmpast", "Enter", "Spawn", "*", "Empowered Essence", 50, false);
            Core.EnsureComplete(4439);
            Core.FarmingLogger("Void Aura", quant);
        }
        Core.AbandonQuest(4439);
    }

    public void GatheringUnstableEssences(int quant = 7500)
    {
        if (Core.CheckInventory("Void Aura", quant) || !Core.IsMember)
            return;

        Farm.EvilREP();
        Core.EquipClass(ClassType.Farm);

        Core.RegisterQuests(4438);
        while (!Core.CheckInventory("Void Aura", quant))
        {
            Core.KillMonster("reddeath", "r2", "Left", "*", "Mirror Essence", 175, false);
            Core.KillMonster("neverworldb", "r2", "Left", "*", "Twisted Essence", 25, false);
            Core.HuntMonster("doomwar", "Zombie King Alteon", "Transposed Essence", 1, false);

            Bot.Wait.ForPickup("Void Aura");
            Core.FarmingLogger("Void Aura", quant);
        }
        Core.AbandonQuest(4438);
    }

    public void RetrieveVoidAuras(int quant = 7500)
    {
        if (Core.CheckInventory("Void Aura", quant))
            return;

        int Essencequant = 20;
        if (Bot.Config != null)
        {
            Essencequant = Bot.Config.Get<bool>("MaxStack") ? 100 : 20;
        }
        else
        {
            Core.Logger("Bot.Config is null, defaulting Essencequant to 20.");
        }
        Farm.EvilREP();
        Core.AddDrop(Essences);
        if (!Core.CheckInventory("Necromancer", toInv: false))
            Bot.Drops.Add("Creature Shard");

        while (!Bot.ShouldExit && !Core.CheckInventory("Void Aura", quant))
        {
            Core.FarmingLogger("Void Aura", quant);
            Core.EnsureAccept(4432);

            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("timespace", "Astral Ephemerite", "Astral Ephemerite Essence", Essencequant, false);

            HuntMonsterBatch(Essencequant, false, false, true,
                   ("necrocavern", 5, "Chaos Vordred Essence"),
                   ("citadel", 21, "Belrot the Fiend Essence"),
                   ("greenguardwest", 22, "Black Knight Essence"),
                   ("mudluk", 18, "Tiger Leech Essence"),
                   ("aqlesson", 17, "Carnax Essence"),
                   ("hachiko", 10, "Dai Tengu Essence"),
                   ("timevoid", 12, "Unending Avatar Essence"),
                   ("dragonchallenge", 4, "Void Dragon Essence"),
                   ("maul", 17, "Creature Creation Essence")
           );

            Core.EnsureCompleteMulti(4432);
            Bot.Wait.ForPickup("Void Aura");
            Core.FarmingLogger("Void Aura", quant);
        }
    }

    private void HuntMonsterBatch(int quant, bool isTemp, bool publicRoom, bool log, params (string map, int monster, string essence)[] monsters)
    {
        Core.AddDrop(monsters.Select(x => x.essence).ToArray());
        Core.EquipClass(ClassType.Solo);
        foreach (var monster in monsters.Where(x => x.essence != null && x.monster > 0 && !Core.CheckInventory(x.essence, quant)))
            Core.HuntMonsterMapID(monster.map, monster.monster, monster.essence, quant, isTemp, log, publicRoom);
    }


    #endregion

    #region Blades, Hilts & Auras

    public void NSBlade()
    {
        if (Core.CheckInventory("Necrotic Sword's Blade"))
            return;

        Core.Logger("Necrotic Sword's Blade");
        EnergizedBlade();
        BariumOfDoom(1);
        VoidAuras(200);
        Core.BuyItem("shadowfall", 793, "Necrotic Sword's Blade");
        Bot.Wait.ForPickup("Necrotic Sword's Blade");
    }

    public void NSHilt()
    {
        if (Core.CheckInventory("Necrotic Sword's Hilt"))
            return;

        Core.Logger("Necrotic Sword's Hilt");
        EnergizedHilt();
        BonesVoidRealm(1);
        VoidAuras(200);
        Core.BuyItem("shadowfall", 793, "Necrotic Sword's Hilt");
        Bot.Wait.ForPickup("Necrotic Sword's Hilt");
    }

    public void NSAura()
    {
        if (Core.CheckInventory("Necrotic Sword's Aura"))
            return;

        Core.Logger("Necrotic Sword's Aura");
        EnergizedAura();
        TimeLordNecro(1);
        VoidAuras(300);
        Core.BuyItem("shadowfall", 793, "Necrotic Sword's Aura");
        Bot.Wait.ForPickup("Necrotic Sword's Aura");
    }

    public void EnergizedBlade()
    {
        if (Core.CheckInventory("Energized Blade"))
            return;

        Core.Logger("Energized Blade");
        FindBlade();
        BariumOfDoom(1);
        VoidAuras(100);
        Core.BuyItem("shadowfall", 793, "Energized Blade");
        Bot.Wait.ForPickup("Energized Blade");
    }

    public void EnergizedHilt()
    {
        if (Core.CheckInventory("Energized Hilt"))
            return;

        Core.Logger("Energized Hilt");
        FindHilt();
        BonesVoidRealm(1);
        VoidAuras(100);
        Core.BuyItem("shadowfall", 793, "Energized Hilt");
        Bot.Wait.ForPickup("Energized Hilt");
    }

    public void EnergizedAura()
    {
        if (Core.CheckInventory("Energized Aura"))
            return;

        Core.Logger("Energized Aura");
        FindAura();
        TimeLordNecro(1);
        VoidAuras(150);
        Core.BuyItem("shadowfall", 793, "Energized Aura");
        Bot.Wait.ForPickup("Energized Aura");
    }

    public void FindBlade()
    {
        if (Core.CheckInventory("Unenhanced Doom Blade"))
        {
            Core.Logger("Unenhanced Doom Blade Owned!");
            return;
        }

        Core.Logger("Unenhanced Doom Blade");
        Core.AddDrop("Unenhanced Doom Blade");
        Core.EnsureAccept(4433);
        BladeEssence(1);
        BariumOfDoom(1);
        VoidAuras(10);
        Core.EnsureComplete(4433);
        Bot.Wait.ForPickup("Unenhanced Doom Blade");
    }

    public void FindHilt()
    {
        if (Core.CheckInventory("Unenhanced Hilt"))
            return;

        Core.Logger("Unenhanced Hilt");
        Core.AddDrop("Unenhanced Hilt", "Bone Dust");
        Core.EnsureAccept(4434);
        CavernCelestite(800);
        Farm.BattleUnderB("Undead Energy", 5000);
        PrimarchHilt(1);
        BonesVoidRealm(50);
        VoidAuras(10);
        Core.EnsureComplete(4434);
        Bot.Wait.ForPickup("Unenhanced Hilt");
    }

    public void FindAura()
    {
        if (Core.CheckInventory("Unenhanced Aura"))
            return;

        Core.Logger("Unenhanced Aura");
        Adv.GearStore();
        Necro.GetNecromancer(true);
        Adv.GearStore(true);

        Core.AddDrop("Unenhanced Aura");
        Core.EnsureAccept(Core.CheckInventory(8012) ? 4435 : 4436);
        FindBlade();
        FindHilt();
        TimeLordNecro(1);
        CHourglass(2);
        ScrollDarkArts(1);
        VoidAuras(10);
        Core.EnsureComplete(Core.CheckInventory(8012) ? 4435 : 4436);
        Bot.Wait.ForPickup("Unenhanced Aura");
    }

    #endregion

    #region Crafting Materials

    public void BariumOfDoom(int quant)
    {
        if (Core.CheckInventory("Barium of Doom", quant))
            return;

        Core.CheckInventory("Barium", quant);
        VoidAuras(quant * 50);
        Core.BuyItem("shadowfall", 793, "Barium of Doom");
        Bot.Wait.ForPickup("Barium of Doom");

    }

    private void Barium()
    {
        Core.Unbank("Barium", "Barium of Doom");
        int i = 0;

        string[] Blades = { "Unenhanced Doom Blade", "Energized Blade", "Necrotic Sword's Blade" };
        if (Core.CheckInventory(new[] { "Unenhanced Aura", "Energized Aura", "Necrotic Sword's Aura" }, any: true))
            i++;

        foreach (string Item in Blades)
            if (Core.CheckInventory(Item))
                i = i + Array.IndexOf(Blades, Item) + 1;

        i = i + Bot.Inventory.GetQuantity("Barium") + Bot.Inventory.GetQuantity("Barium of Doom");
        if (i >= 4)
            return;

        BLOD.UnlockMineCrafting();
        Daily.MineCrafting(new[] { "Barium" }, 4 - i);
    }

    public void BonesVoidRealm(int quant)
    {
        if (Core.CheckInventory("Bones from the Void Realm", quant))
            return;
        Core.FarmingLogger("Bones from the Void Realm", quant);
        Core.AddDrop("Undead Energy");
        Farm.BattleUnderB("Bone Dust", quant * 50);
        VoidAuras(quant * 50);
        Core.BuyItem("shadowfall", 793, "Bones from the Void Realm", quant);
        Bot.Wait.ForPickup("Bones from the Void Realm");
    }

    public void TimeLordNecro(int quant)
    {
        if (Core.CheckInventory("Time Lord's Necronomicon", quant))
            return;

        int CurrentCHQuant = Bot.Inventory.Items.Concat(Bot.Bank.Items)
            .FirstOrDefault(x => x.Name == "Chaorrupted Hourglass")?.Quantity ?? 0;
        Core.FarmingLogger("Time Lord's Necronomicon", quant);
        CHourglass(quant * 10);
        ScrollDarkArts(quant);
        VoidAuras(quant * 100);
        Core.BuyItem("shadowfall", 793, "Time Lord's Necronomicon", quant);
        Bot.Wait.ForPickup("Time Lord's Necronomicon");


    }

    public void CavernCelestite(int quant)
    {
        BLOD.SoulSearching("Cavern Celestite", quant, false);
    }

    public void PrimarchHilt(int quant)
    {
        if (Core.CheckInventory("Primarch's Hilt", quant))
            return;

        Core.EquipClass(ClassType.Solo);
        Core.HuntMonster("bosschallenge", "Colossal Primarch", "Primarch's Hilt", quant, false, publicRoom: true);
        Bot.Wait.ForPickup("Primarch's Hilt");
    }

    public void BladeEssence(int quant)
    {
        if (Core.CheckInventory("Blade Essence", quant))
            return;

        Core.EquipClass(ClassType.Solo);
        Core.HuntMonster("chaoscrypt", "Chaorrupted Armor", "Blade Essence", quant, false);
        Bot.Wait.ForPickup("Blade Essence");
    }

    public void CHourglass(int quant)
    {
        if (Core.CheckInventory("Chaorrupted Hourglass", quant))
            return;

        Core.EquipClass(ClassType.Solo);
        Core.HuntMonster("mqlesson", "Dragonoid", "Dragonoid of Hours", isTemp: false, publicRoom: true);
        Core.HuntMonster("timespace", "Chaos Lord Iadoa", "Chaorrupted Hourglass", quant, false, publicRoom: true);
        Bot.Wait.ForPickup("Chaorrupted Hourglass");
    }

    public void ScrollDarkArts(int quant)
    {
        if (Core.CheckInventory("(Necro) Scroll of Dark Arts", quant))
            return;

        Core.EquipClass(ClassType.Solo);
        Core.HuntMonster("epicvordred", "Ultra Vordred", "(Necro) Scroll of Dark Arts", quant, false, publicRoom: true);
        Bot.Wait.ForPickup("(Necro) Scroll of Dark Arts");
    }

    private void PreFarmSteps()
    {
        if (Core.CheckInventory("Necrotic Sword of Doom"))
            return;

        int TotalVAs = 0; // Requirements will add VAs to this amount for a final calculation.

        int MergeRequirement = 800; // Always needed to merge NSoD

        int HiltVA = 2910;  // 250 (200 + 50 (bone from the void realm x1)) for >> (NS)
                            // + 150 (100 + 50 (bone from the void realm x1))for >> (E) 
                            // + 2510 (bone from the void realm x50 + 10 for quest) (UE)

        int BladeVA = 460;  // 250 (NS) + 150 (E) + 10(quest) + 50(Barium of doom) for >> (UE)
        int AuraVA = 760    // NS + E Aura
                    + 2500  // Extra UE Hilt bones
                    + 60    // Extra UE Blade
                    + 10;   // Misc quest

        Core.Logger("NSoD: PreFarm Step #1/9: Hilt, Blade, and Aura VA Calculation");

        // HILT
        Core.Logger("NSoD: PreFarm Step #1.1/9: NSOD Hilt VoidAura Calculation");
        if (Core.CheckInventory("Unenhanced Hilt"))
            HiltVA -= 2510;
        else if (Core.CheckInventory("Energized Hilt"))
            HiltVA -= 2760;
        else if (Core.CheckInventory("Necrotic Sword's Hilt"))
            HiltVA = 0;

        TotalVAs += HiltVA;
        Core.Logger($"Total added to count for Hilt: {HiltVA}");

        // AURA
        Core.Logger("NSoD: PreFarm Step #1.2/9: NSOD Aura VoidAura Calculation");
        if (Core.CheckInventory("Unenhanced Aura"))
            AuraVA -= 2670 + 250 + 400;
        else if (Core.CheckInventory("Energized Aura"))
            AuraVA -= 400;
        else if (Core.CheckInventory("Necrotic Sword's Aura"))
            AuraVA = 0;

        TotalVAs += AuraVA;
        Core.Logger($"Total added to count for Aura: {AuraVA}");

        // BLADE
        Core.Logger("NSoD: PreFarm Step #1.3/9: NSOD Blade VoidAura Calculation");
        if (Core.CheckInventory("Unenhanced Blade"))
            BladeVA -= 460;
        else if (Core.CheckInventory("Energized Blade"))
            BladeVA -= 250;
        else if (Core.CheckInventory("Necrotic Sword's Blade"))
            BladeVA = 0;

        TotalVAs += BladeVA;
        Core.Logger($"Total added to count for Blade: {BladeVA}");

        TotalVAs += MergeRequirement;

        Core.Logger($"NSoD: PreFarm Step #1.4/9: Total VoidAuras to farm for Hilt, Blade, Aura, and Merge (subtracting for what is already owned): {TotalVAs}");
        VoidAuras(TotalVAs);

        Core.Logger("NSoD: PreFarm Step #2/9: CavernCelestite (Quantity: 1600)");
        CavernCelestite(1600);

        Core.Logger("NSoD: PreFarm Step #3/9: BattleUnderB - Bone Dust (Quantity: 5100)");
        Farm.BattleUnderB("Bone Dust", 5100);

        Core.Logger("NSoD: PreFarm Step #4/9: BattleUnderB - Undead Energy (Quantity: 10000)");
        Farm.BattleUnderB("Undead Energy", 10000);

        Core.Logger("NSoD: PreFarm Step #5/9: PrimarchHilt (Quantity: 2)");
        PrimarchHilt(2);

        Core.Logger("NSoD: PreFarm Step #6/9: BladeEssence (Quantity: 2)");
        BladeEssence(2);

        Core.Logger("NSoD: PreFarm Step #7/9: CHourglass (Quantity: 31)");
        CHourglass(31);

        Core.Logger("NSoD: PreFarm Step #8/9: ScrollDarkArts (Quantity: 4)");
        ScrollDarkArts(4);

        Core.Logger("NSoD: PreFarm Step #9/9: ULTRA Sepulchure");
        Core.HuntMonster("sepulchurebattle", "ULTRA Sepulchure", "Doom Heart", isTemp: false, publicRoom: true);
    }


    #endregion
}
