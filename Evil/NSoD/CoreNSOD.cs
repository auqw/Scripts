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
    #region Constants

    private const int ShadowfallShopId = 793;      // Shadowfall shop item ID
    private const int DoomKnightArmorId = 14474;   // Sepulchure's DoomKnight Armor
    private const int MaxFarmIterations = 1000;    // Safety limit before relogging in RetrieveVoidAuras

    #endregion

    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    // Lazily created helpers. Target-typed new() and ??= per preferences.
    private static CoreFarms Farm => _Farm ??= new();
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv => _Adv ??= new();
    private static CoreAdvanced _Adv;
    private static CoreDailies Daily => _Daily ??= new();
    private static CoreDailies _Daily;
    private static CoreBLOD BLOD => _BLOD ??= new();
    private static CoreBLOD _BLOD;
    private static CoreSDKA SDKA => _SDKA ??= new();
    private static CoreSDKA _SDKA;
    private static Necromancer Necro => _Necro ??= new();
    private static Necromancer _Necro;

    public string OptionsStorage = "NecroticSwordOfDoomOptions";
    public bool DontPreconfigure = true;
    public Option<bool> MaxStack = new("MaxStack", "Max Stack", "Max Stack Monster Essences in \"Retreive Void Auras\"\nRecommended setting: True", true);
    public Option<bool> PreFarm = new("PreFarm", "Pre Farm Materials", "Farm all requiered items before merging everything. Not recommended if you already did a merge yourself.\nRecommended setting: False", false);
    public Option<bool> GetSDKA = new("getSDKA", "Get SDKA first [Mem]", "If true, the bot will attempt to get SDKA first, so that it can use the fastest Void Aura farm available\nMember-Only\nRecommended setting: True", true);

    public string[] Essences =
    [
        "Astral Ephemerite Essence", "Belrot the Fiend Essence", "Black Knight Essence",
        "Tiger Leech Essence", "Carnax Essence", "Chaos Vordred Essence",
        "Dai Tengu Essence", "Unending Avatar Essence", "Void Dragon Essence",
        "Creature Creation Essence",
    ];

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

            // Fixed: each branch now logs its real reason for running or skipping
            bool preFarmEnabled = Bot.Config?.Get<bool>("PreFarm") == true;
            if (!preFarmEnabled)
                Core.Logger("NSoD: PreFarm disabled, continuing with the farm.");
            else if (Core.CheckInventory(new[] { "Necrotic Sword's Blade", "Necrotic Sword's Hilt", "Necrotic Sword's Aura" }, any: true))
                Core.Logger("NSoD: PreFarm skipped, a blade, hilt, or aura is already owned.");
            else
            {
                Core.Logger("NSoD: PreFarm Steps:");
                PreFarmSteps();
            }

            Core.Logger("NSoD: Step #10/16: NSBlade.");
            NSBlade();
            Core.Logger("NSoD: Step #11/16: NSHilt.");
            NSHilt();
            Core.Logger("NSoD: Step #12/16: NSAura\n (To the retards; yes this requires an extra `Unenhanced Blade` & `Unenhanced Hilt`).");
            NSAura();
            Core.Logger("NSoD: Step #13/16: ULTRA Sepulchure for \"Doom Heart\"");
            GetDoomHeart();
            Core.Logger("NSoD: Step #14/16: Void Auras x800 to complete the merge");
            VoidAuras(800);
            Core.Logger("NSoD: Step #15/16: NSoD purchase");
            BuyAndWait("Necrotic Sword of Doom");
            Core.Logger("NSoD: Step #16/16: NSoD Enhancement");
        }

        if (!Core.isCompletedBefore(7652))
        {
            Core.Logger("Getting the NSOD character page badge");
            Core.EnsureAccept(7652);
            Core.HuntMonster("graveyard", "Skeletal Warrior", "Arcane Parchment");
            Core.EnsureComplete(7652);
        }

        // Unchanged logic, but please verify: this checks the DoomKnight Armor, not the NSoD
        if (!Core.CheckInventory(DoomKnightArmorId) && !Core.IsMember)
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
        BuyAndWait("Necrotic Blade of Doom");
        Core.Logger("NBOD: Step #5: Reminder - Use AE's Buy-Back system to retrieve your NSoD", messageBox: true);
    }

    #region Void Auras

    public void VoidAuras(int quant = 7500)
    {
        if (Core.CheckInventory("Void Aura", quant))
            return;

        // Fixed: ?. replaces the `!` so a missing config no longer throws
        if (Bot.Config?.Get<bool>("getSDKA") == true && Bot.Player.IsMember && !Core.CheckInventory(DoomKnightArmorId))
            SDKA.DoAll();

        Core.AddDrop("Void Aura");
        Core.FarmingLogger("Void Aura", quant);

        if (Core.CheckInventory(DoomKnightArmorId))
            CommandingShadowEssences(quant);

        if (Bot.Player.IsMember)
            GatheringUnstableEssences(quant);
        else
            RetrieveVoidAuras(quant);
    }

    public void CommandingShadowEssences(int quant = 7500)
    {
        if (Core.CheckInventory("Void Aura", quant) || !Core.CheckInventory(DoomKnightArmorId))
            return;


        try
        {
            while (!Bot.ShouldExit && !Core.CheckInventory("Void Aura", quant))
            {
                Core.CanWeAggro();
                Core.EnsureAccept(4439);
                Core.EquipClass(ClassType.Solo);
                Core.HuntMonsterMapID("shadowrealmpast", 11, "Malignant Essence", 3, false);
                Core.EquipClass(ClassType.Farm);
                Core.KillMonster("shadowrealmpast", "Enter", "Spawn", "*", "Empowered Essence", 50, false);
                Core.EnsureComplete(4439);
                Core.FarmingLogger("Void Aura", quant);
            }
        }
        finally
        {
            Bot.Options.AggroAllMonsters = false;
            Bot.Options.AggroMonsters = false;
            Core.AbandonQuest(4439);
        }
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
        // Fixed: the goto is now an outer loop. Each pass restarts the farm from the top.
        while (!Bot.ShouldExit && !Core.CheckInventory("Void Aura", quant))
        {
            // Max Stack: 100 when enabled, 20 otherwise (also the fallback if config is missing)
            int essenceQuant = Bot.Config?.Get<bool>("MaxStack") == true ? 100 : 20;

            Farm.EvilREP();
            Core.AddDrop(Essences);
            if (!Core.CheckInventory("Necromancer", toInv: false))
                Bot.Drops.Add("Creature Shard");
            Core.RegisterQuests(4432);

            int safetyCounter = 0;
            while (!Bot.ShouldExit && !Core.CheckInventory("Void Aura", quant) && safetyCounter < MaxFarmIterations)
            {
                safetyCounter++;
                Core.FarmingLogger("Void Aura", quant);
                Core.EquipClass(ClassType.Farm);
                Core.HuntMonster("timespace", "Astral Ephemerite", "Astral Ephemerite Essence", essenceQuant, false);

                HuntMonsterBatch(essenceQuant, false, false, true,
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

                Bot.Sleep(500);
            }

            Core.CancelRegisteredQuests();

            // Fixed: check the inventory first, so finishing on the last iteration doesn't trigger a relog
            if (Core.CheckInventory("Void Aura", quant) || Bot.ShouldExit)
                return;

            Core.Relogin("RetrieveVoidAuras: Reached max iterations without collecting enough Void Auras. Attempting to relog to prevent an infinite loop.");
        }
    }

    private void HuntMonsterBatch(int quant, bool isTemp, bool publicRoom, bool log, params (string map, int monster, string essence)[] monsters)
    {
        Core.AddDrop(monsters.Select(x => x.essence).ToArray());
        Core.EquipClass(ClassType.Solo);

        // Removed the null and monster > 0 checks: the entries are hardcoded and always valid
        foreach ((string map, int monster, string essence) in monsters.Where(x => !Core.CheckInventory(x.essence, quant)))
            Core.HuntMonsterMapID(map, monster, essence, quant, isTemp, log, publicRoom);
    }

    #endregion

    #region Blades, Hilts & Auras

    public void NSBlade()
    {
        if (Core.CheckInventory("Necrotic Sword's Blade"))
        {
            Core.FarmingLogger("Necrotic Sword's Blade");
            return;
        }

        EnergizedBlade();
        BariumOfDoom(1);
        VoidAuras(200);
        BuyAndWait("Necrotic Sword's Blade");
    }

    public void NSHilt()
    {
        if (Core.CheckInventory("Necrotic Sword's Hilt"))
        {
            Core.FarmingLogger("Necrotic Sword's Hilt");
            return;
        }

        EnergizedHilt();
        BonesVoidRealm(1);
        VoidAuras(200);
        BuyAndWait("Necrotic Sword's Hilt");
    }

    public void NSAura()
    {
        if (Core.CheckInventory("Necrotic Sword's Aura"))
        {
            Core.FarmingLogger("Necrotic Sword's Aura");
            return;
        }

        Core.AddDrop("Necrotic Sword's Aura");
        EnergizedAura();
        TimeLordNecro(1);
        VoidAuras(300);
        BuyAndWait("Necrotic Sword's Aura");
    }

    public void EnergizedBlade()
    {
        if (Core.CheckInventory("Energized Blade"))
        {
            Core.FarmingLogger("Energized Blade");
            return;
        }

        FindBlade();
        BariumOfDoom(1);
        VoidAuras(100);
        BuyAndWait("Energized Blade");
    }

    public void EnergizedHilt()
    {
        if (Core.CheckInventory("Energized Hilt"))
        {
            Core.FarmingLogger("Energized Hilt");
            return;
        }

        Core.AddDrop("Energized Hilt");
        FindHilt();
        BonesVoidRealm(1);
        VoidAuras(100);
        BuyAndWait("Energized Hilt");
    }

    public void EnergizedAura()
    {
        if (Core.CheckInventory("Energized Aura"))
        {
            Core.FarmingLogger("Energized Aura");
            return;
        }

        Core.AddDrop("Energized Aura");
        FindAura();
        TimeLordNecro(1);
        VoidAuras(150);
        BuyAndWait("Energized Aura");
    }

    public void FindBlade()
    {
        if (Core.CheckInventory("Unenhanced Doom Blade"))
        {
            Core.FarmingLogger("Unenhanced Doom Blade");
            return;
        }

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
        {
            Core.FarmingLogger("Unenhanced Hilt");
            return;
        }

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
        {
            Core.FarmingLogger("Unenhanced Aura");
            return;
        }

        Core.AddDrop("Unenhanced Aura");
        Adv.GearStore(EnhAfter: true);
        Necro.GetNecromancer(true);
        Adv.GearStore(true, EnhAfter: true);

        int questId = Core.CheckInventory(8012) ? 4435 : 4436;
        Core.EnsureAccept(questId);
        FindBlade();
        FindHilt();
        TimeLordNecro(1);
        CHourglass(2);
        ScrollDarkArts(1);
        VoidAuras(10);
        Core.EnsureComplete(questId);
        Bot.Wait.ForPickup("Unenhanced Aura");
    }

    #endregion

    #region Crafting Materials

    public void BariumOfDoom(int quant)
    {
        if (Core.CheckInventory("Barium of Doom", quant))
            return;

        // Removed: a stray Core.CheckInventory("Barium", quant) call whose result was discarded
        VoidAuras(quant * 50);
        if (!Core.CheckInventory("Barium"))
            Daily.MineCrafting(new[] { "Barium" }, 1);
        BuyAndWait("Barium of Doom");
    }

    private void Barium()
    {
        Core.Unbank("Barium", "Barium of Doom");

        // Progress score: 1 point if any Aura is owned, plus the blade's list position (1 to 3) for each blade owned.
        // Necrotic Sword's Blade (3) counts for more than Unenhanced Doom Blade (1) on purpose.
        int progress = Core.CheckInventory(new[] { "Unenhanced Aura", "Energized Aura", "Necrotic Sword's Aura" }, any: true) ? 1 : 0;
        string[] blades = { "Unenhanced Doom Blade", "Energized Blade", "Necrotic Sword's Blade" };
        foreach (string blade in blades)
        {
            if (Core.CheckInventory(blade))
                progress += Array.IndexOf(blades, blade) + 1;
        }

        progress += Bot.Inventory.GetQuantity("Barium") + Bot.Inventory.GetQuantity("Barium of Doom");
        if (progress >= 4)
            return;

        BLOD.UnlockMineCrafting();
        Daily.MineCrafting(new[] { "Barium" }, 4 - progress);
    }

    public void BonesVoidRealm(int quant)
    {
        if (Core.CheckInventory("Bones from the Void Realm", quant))
            return;

        Core.FarmingLogger("Bones from the Void Realm", quant);
        Core.AddDrop("Undead Energy");
        Farm.BattleUnderB("Bone Dust", quant * 50);
        VoidAuras(quant * 50);
        BuyAndWait("Bones from the Void Realm", quant);
    }

    public void TimeLordNecro(int quant)
    {
        if (Core.CheckInventory("Time Lord's Necronomicon", quant))
            return;

        Core.FarmingLogger("Time Lord's Necronomicon", quant);
        CHourglass(quant * 10);
        ScrollDarkArts(quant);
        VoidAuras(quant * 100);
        BuyAndWait("Time Lord's Necronomicon", quant);
    }

    public void CavernCelestite(int quant) => BLOD.SoulSearching("Cavern Celestite", quant, false);

    public void PrimarchHilt(int quant)
    {
        if (Core.CheckInventory("Primarch's Hilt", quant))
            return;

        Core.EquipClass(ClassType.Dodge);
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

        const int mergeRequirement = 800;
        int hiltVA = 2910;
        int bladeVA = 460;
        int auraVA = 760 + 2500 + 60 + 10;

        Core.Logger("NSoD: PreFarm Step #1/9: Hilt, Blade, and Aura VA Calculation");

        if (Core.CheckInventory("Unenhanced Hilt"))
            hiltVA -= 2510;
        else if (Core.CheckInventory("Energized Hilt"))
            hiltVA -= 2760;
        else if (Core.CheckInventory("Necrotic Sword's Hilt"))
            hiltVA = 0;
        Core.Logger($"Total added to count for Hilt: {hiltVA}");

        if (Core.CheckInventory("Unenhanced Aura"))
            auraVA -= 2670 + 250 + 400;
        else if (Core.CheckInventory("Energized Aura"))
            auraVA -= 400;
        else if (Core.CheckInventory("Necrotic Sword's Aura"))
            auraVA = 0;
        Core.Logger($"Total added to count for Aura: {auraVA}");

        if (Core.CheckInventory("Unenhanced Blade"))
            bladeVA -= 460;
        else if (Core.CheckInventory("Energized Blade"))
            bladeVA -= 250;
        else if (Core.CheckInventory("Necrotic Sword's Blade"))
            bladeVA = 0;
        Core.Logger($"Total added to count for Blade: {bladeVA}");

        int totalVAs = hiltVA + auraVA + bladeVA + mergeRequirement;
        Core.Logger($"NSoD: PreFarm Step #1.4/9: Total VoidAuras to farm for Hilt, Blade, Aura, and Merge (subtracting for what is already owned): {totalVAs}");
        VoidAuras(totalVAs);

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
        GetDoomHeart();
    }

    #endregion

    #region Helpers

    // Buys the item from Shadowfall and waits until it lands in the inventory
    private void BuyAndWait(string item, int quant = 1)
    {
        Core.BuyItem("shadowfall", ShadowfallShopId, item, quant);
        Bot.Wait.ForPickup(item);
    }

    // Used by GetNSOD and PreFarmSteps
    private void GetDoomHeart() =>
        Core.HuntMonster("sepulchurebattle", "ULTRA Sepulchure", "Doom Heart", isTemp: false, publicRoom: true);

    #endregion
}