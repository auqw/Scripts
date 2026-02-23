/*
name: Arcana Invoker Resource Merge
description: This bot will farm the items belonging to the selected mode for the Arcana Invoker Resource Merge [2434] in /arcana
tags: arcana, invoker, resource, merge, arcana, magicians, desire, high, priestess, intuition, empress, initiative, emperors, authority, hierophants, servitude, lovers, embrace, chariots, triumph, justices, righteousness, hermits, solitude, wheel, fortunes, destiny, strengths, fortitude, hanged, mans, discernment, deaths, mortality, temperances, frugality, devilish, temptation, towers, upheaval, stars, hope, moons, illusion, suns, optimism, judgements, absolution, worlds, voyage
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Other/MergeShops/TerminaTempleMerge.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Other/Classes/Necromancer.cs
//cs_include Scripts/Evil/NSoD/CoreNSOD.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Other/MergeShops/GooseMerge.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/ShadowsOfChaos/CoreSoC.cs
//cs_include Scripts/Other/MergeShops/BrightForestMerge.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/InfernalArena.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/CelestialArena.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
//cs_include Scripts/Other/MergeShops/DoomLegacyMerge.cs
//cs_include Scripts/Other/MergeShops/CelestialChallengerMerge.cs
//cs_include Scripts/Other/MergeShops/SpoilsofLightMerge.cs
//cs_include Scripts/Seasonal/NewYear/ArchiveofTimeMerge.cs
//cs_include Scripts/Other/MergeShops/CrocriverMerge.cs
//cs_include Scripts/Other/MergeShops/SuperSlayinMerge.cs
//cs_include Scripts/Story/DreamPalace.cs
//cs_include Scripts/Other/MergeShops/DreampalaceMerge.cs
//cs_include Scripts/Other/MergeShops/BonecastleMerge.cs
//cs_include Scripts/Story/QueenofMonsters/CoreQoM.cs
//cs_include Scripts/Other/MergeShops/CelestialRealmMerge.cs
//cs_include Scripts/Other/MergeShops/3LittleWolvesHousesMerge.cs
//cs_include Scripts/Other/Various/Potions.cs
//cs_include Scripts/Story/CruxShip.cs
//cs_include Scripts/Seasonal/Mogloween/MoonlightKhopeshMerge.cs
//cs_include Scripts/Other/MergeShops/ThirdspellMerge.cs
//cs_include Scripts/Seasonal/Friday13th/MergeShops/ShadowMerge.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs
//cs_include Scripts/Darkon/CoreDarkon.cs
//cs_include Scripts/Seasonal/BlackFriday/ShadowofDoom/CoreShadowofDoom.cs
//cs_include Scripts/Story/FableForest.cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
//cs_include Scripts/Story/SepulchureSaga/CoreSepulchure.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Options;

public class ArcanaInvokerResourceMerge
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
    private static CoreAdvanced sAdv
    {
        get => _sAdv ??= new CoreAdvanced();
        set => _sAdv = value;
    }
    private static CoreAdvanced _sAdv;

    private static Core13LoC LOC
    {
        get => _LOC ??= new Core13LoC();
        set => _LOC = value;
    }
    private static Core13LoC _LOC;
    private static GooseMerge GooseMerge
    {
        get => _GooseMerge ??= new GooseMerge();
        set => _GooseMerge = value;
    }
    private static GooseMerge _GooseMerge;
    private static BrightForestMerge BrightForestMerge
    {
        get => _BrightForestMerge ??= new BrightForestMerge();
        set => _BrightForestMerge = value;
    }
    private static BrightForestMerge _BrightForestMerge;
    private static TerminaTempleMerge TerminaTempleMerge
    {
        get => _TerminaTempleMerge ??= new TerminaTempleMerge();
        set => _TerminaTempleMerge = value;
    }
    private static TerminaTempleMerge _TerminaTempleMerge;
    private static InfernalArena InfernalArena
    {
        get => _InfernalArena ??= new InfernalArena();
        set => _InfernalArena = value;
    }
    private static InfernalArena _InfernalArena;
    private static DoomLegacyMerge DLM
    {
        get => _DLM ??= new DoomLegacyMerge();
        set => _DLM = value;
    }
    private static DoomLegacyMerge _DLM;
    private static CelestialChallengerMerge CCM
    {
        get => _CCM ??= new CelestialChallengerMerge();
        set => _CCM = value;
    }
    private static CelestialChallengerMerge _CCM;
    private static SpoilsofLightMerge SOLM
    {
        get => _SOLM ??= new SpoilsofLightMerge();
        set => _SOLM = value;
    }
    private static SpoilsofLightMerge _SOLM;
    private static ArchiveofTimeMerge AOTM
    {
        get => _AOTM ??= new ArchiveofTimeMerge();
        set => _AOTM = value;
    }
    private static ArchiveofTimeMerge _AOTM;
    private static CrocriverMerge CM
    {
        get => _CM ??= new CrocriverMerge();
        set => _CM = value;
    }
    private static CrocriverMerge _CM;
    private static SuperSlayinMerge SSM
    {
        get => _SSM ??= new SuperSlayinMerge();
        set => _SSM = value;
    }
    private static SuperSlayinMerge _SSM;
    private static DreampalaceMerge DPM
    {
        get => _DPM ??= new DreampalaceMerge();
        set => _DPM = value;
    }
    private static DreampalaceMerge _DPM;
    private static BonecastleMerge BCM
    {
        get => _BCM ??= new BonecastleMerge();
        set => _BCM = value;
    }
    private static BonecastleMerge _BCM;
    private static CelestialRealmMerge CRM
    {
        get => _CRM ??= new CelestialRealmMerge();
        set => _CRM = value;
    }
    private static CelestialRealmMerge _CRM;
    private static ThreeLittleWolvesHousesMerge TLWHM
    {
        get => _TLWHM ??= new ThreeLittleWolvesHousesMerge();
        set => _TLWHM = value;
    }
    private static ThreeLittleWolvesHousesMerge _TLWHM;
    private static PotionBuyer PotionBuyer
    {
        get => _PotionBuyer ??= new PotionBuyer();
        set => _PotionBuyer = value;
    }
    private static PotionBuyer _PotionBuyer;
    private static MoonlightKhopeshMerge MoonlightKhopeshMerge
    {
        get => _MoonlightKhopeshMerge ??= new MoonlightKhopeshMerge();
        set => _MoonlightKhopeshMerge = value;
    }
    private static MoonlightKhopeshMerge _MoonlightKhopeshMerge;
    private static ShadowMerge ShadowMerge
    {
        get => _ShadowMerge ??= new ShadowMerge();
        set => _ShadowMerge = value;
    }
    private static ShadowMerge _ShadowMerge;
    private static ThirdspellMerge ThirdspellMerge
    {
        get => _ThirdspellMerge ??= new ThirdspellMerge();
        set => _ThirdspellMerge = value;
    }
    private static ThirdspellMerge _ThirdspellMerge;
    private static CoreDarkon Darkon
    {
        get => _Darkon ??= new CoreDarkon();
        set => _Darkon = value;
    }
    private static CoreDarkon _Darkon;
    private static CoreShadowofDoom CoreShadowofDoom
    {
        get => _CoreShadowofDoom ??= new CoreShadowofDoom();
        set => _CoreShadowofDoom = value;
    }
    private static CoreShadowofDoom _CoreShadowofDoom;
    private static FableForest FableForest
    {
        get => _FableForest ??= new FableForest();
        set => _FableForest = value;
    }
    private static FableForest _FableForest;
    private static VoidRefuge VR
    {
        get => _VR ??= new VoidRefuge();
        set => _VR = value;
    }
    private static VoidRefuge _VR;
    private static CoreAOR AOR
    {
        get => _AOR ??= new CoreAOR();
        set => _AOR = value;
    }
    private static CoreAOR _AOR;
    private static CoreSepulchure CoreSS
    {
        get => _CoreSS ??= new CoreSepulchure();
        set => _CoreSS = value;
    }
    private static CoreSepulchure _CoreSS;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(
            new[]
            {
                "Staff of Inversion",
                "BattleMage Armor",
                "Nightlocke War Staff",
                "Calamitous Warlic's Tome",
                "Dishpan Cleric Costume",
                "Chaotic Healer",
                "Battle Cleric of the Dragon",
                "Amia's Cult Secret",
                "Queen's Sage Scythe",
                "ShadowFlame Empress",
                "Fragment of the Queen",
                "Empress' Finger's Ring",
                "King Klunk's Crown",
                "Crowned Skull of Na'al",
                "Zealous Crown",
                "Lich Emperor's Catalyst",
                "Blessed Abezeth",
                "Inquisitor of the Light",
                "Divine Guardian Of Aegis",
                "Chaos Weaver Cleric's Doctrine",
                "Love Token",
                "Time Heart",
                "Storm Heart",
                "Mercutio's Heart",
                "Racing Trophy",
                "Sphinx Sentinel",
                "Dread Deadmoor BattleAxe",
                "SMU Brutalcorn's Horn",
                "Ouroboros Scale",
                "Libran Scales",
                "Akriloth's Scale",
                "ArchFiend DragonKnight's Scale",
                "Fa's Gamer Fuel",
                "ARTX 3090 Controller",
                "Soulseeker's Grim Hood",
                "Nothing's Solus",
                "Lucky Pet",
                "Second Chance Coin",
                "Treasure Chest",
                "Ultra Lobthulu's Fortune",
                "Enchanted Martial Artist's Gi",
                "Strong Axe of Golmoth",
                "Fortitude Tonic",
                "Strong Drag's Intact Wing",
                "Chained Rune Bonebreaker",
                "Noble Sacrifice",
                "The Answer",
                "Astero's Insight",
                "Death's Oversight",
                "Death Pit Arena Medal",
                "Enchanted DeathKnight",
                "Super Death's Scythe Fragment",
                "Celestial Wings of Guiding",
                "Blessed Coffee Cup",
                "Northern Crown",
                "Azkorath's Wing",
                "Fiendish Outlaw",
                "Fiendish Remains",
                "Glass Horns",
                "Fiend Champion's Spike",
                "Earth Stone",
                "Dragon Runestone",
                "Arcangrove Tower House",
                "Nevanna's Revelation",
                "Star Scrap",
                "Rising Star Token",
                "Dark Stars",
                "Star Sapphire Fragment",
                "Moon Rock Fragments",
                "Blood Moon Warrior",
                "Celestial Khopesh",
                "The Moon's Reflection",
                "Golden Sun Seal",
                "Sun Zone Chit",
                "Armor of the Sun",
                "The Sun's Enlightenment",
                "Trumpet",
                "Judgment Tonic",
                "Enchanted Lance of Doom Reborn",
                "Minos' Sentence",
                "Darkon's Receipt",
                "Teeth",
                "La's Gratitude",
                "Astravian Medal",
                "A Melody",
                "Bandit's Correspondence",
                "Suki's Prestige",
                "Ancient Remnant",
                "Mourning Flower",
                "Unfinished Musical Score",
            }
        );
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        //prolly more storylines missing, add as found
        CoreShadowofDoom.DoAll();
        FableForest.StoryLine();
        AOR.SunlightZone();
        CoreSS.CompleteSS();
        //prolly more storylines missing, add as found

        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("arcana", 2434, findIngredients, buyOnlyThis, buyMode: buyMode);

        #region Dont edit this part
        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            int currentQuant = req.Temp
                ? Bot.TempInv.GetQuantity(req.Name)
                : Bot.Inventory.GetQuantity(req.Name);
            if (req == null)
            {
                Core.Logger("req is NULL");
                return;
            }

            switch (req.Name)
            {
                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger(
                        $"The bot hasn't been taught how to get {req.Name}."
                            + (shouldStop ? " Please report the issue." : " Skipping"),
                        messageBox: shouldStop,
                        stopBot: shouldStop
                    );
                    break;
        #endregion

                case "Staff of Inversion":
                    Core.KillEscherion(req.Name, isTemp: req.Temp);
                    break;

                case "BattleMage Armor":
                    Adv.BuyItem("castleroof", 749, req.Name, 1, shopItemID: 12773);
                    break;

                case "Nightlocke War Staff":
                    Core.EquipClass(ClassType.Solo);
                    Core.KillMonster(
                        "aqw3d",
                        "r13",
                        "Left",
                        "Nightlocke Staff",
                        req.Name,
                        quant,
                        req.Temp
                    );
                    break;

                case "Calamitous Warlic's Tome":
                    Core.KillMonster(
                        "ruinedcrown",
                        "r10",
                        "Left",
                        "Calamitous Warlic",
                        req.Name,
                        quant,
                        req.Temp
                    );
                    break;

                case "Dishpan Cleric Costume":
                    Core.KillMonster(
                        "cleric",
                        "Frame3",
                        "Left",
                        "Chaos Dragon",
                        req.Name,
                        quant,
                        req.Temp
                    );
                    break;

                case "Chaotic Healer":
                    Core.FarmingLogger(req.Name, quant);
                    LOC.Hero();
                    Adv.BuyItem("newfinale", 891, req.Name);
                    break;

                case "Battle Cleric of the Dragon":
                    TerminaTempleMerge.BuyAllMerge(req.Name);
                    break;

                case "Amia's Cult Secret":
                    Core.KillMonster(
                        "fotia",
                        "r6",
                        "Left",
                        "Amia the Cult Leader",
                        req.Name,
                        quant,
                        req.Temp
                    );
                    break;

                case "Queen's Sage Scythe":
                    GooseMerge.BuyAllMerge(req.Name);
                    break;

                case "ShadowFlame Empress":
                    BrightForestMerge.BuyAllMerge(req.Name);
                    break;

                case "Fragment of the Queen":
                    Core.EquipClass(ClassType.Solo);
                    Bot.Quests.UpdateQuest(8094);
                    Core.HuntMonster(
                        "transformation",
                        "Queen of Monsters",
                        req.Name,
                        quant,
                        req.Temp
                    );
                    break;

                case "Empress' Finger's Ring":
                    Core.HuntMonsterMapID("firstobservatory", 13, req.Name, quant, req.Temp);
                    break;

                case "King Klunk's Crown":
                    Core.HuntMonster("evilwarnul", "Laken", req.Name, quant, req.Temp);
                    break;

                case "Crowned Skull of Na'al":
                    Core.FarmingLogger(req.Name, quant);
                    Core.UseDodgeClass();
                    Core.Logger($"Doing story first: {!Core.isCompletedBefore(9373)}");
                    if (!Core.isCompletedBefore(9377))
                    {
                        Core.Logger(
                            "Boss: [Na'al]\n this may take an hr or 2... or u may first try it so good luck (a kill has been gotten with VHL) so its confirmd able to be done...)"
                        );
                        InfernalArena.DoStory();
                    }
                    Adv.BuyItem(Bot.Map.Name, 2336, req.Name, quant);
                    break;

                case "Zealous Crown":
                    DLM.BuyAllMerge(req.Name);
                    break;

                case "Judgment Tonic":
                    PotionBuyer. // Call the method with specific parameters to farm Judgment Tonics with a quantity of 50
                    INeedYourStrongestPotions(
                        new[] { "Judgment Tonic" },
                        new bool[] { true },
                        quant,
                        true,
                        true
                    );
                    break;

                case "Lich Emperor's Catalyst":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("warundead", "Lich Emperor", req.Name, quant, req.Temp, false);
                    break;

                case "Blessed Abezeth":
                    CCM.BuyAllMerge(req.Name);
                    break;

                case "Inquisitor of the Light":
                    SOLM.BuyAllMerge(req.Name);
                    break;

                case "Divine Guardian Of Aegis":
                    AOTM.BuyAllMerge(req.Name);
                    break;

                case "Chaos Weaver Cleric's Doctrine":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("chaosweb", "ChaosWeaver Cleric", req.Name, quant, req.Temp);
                    break;

                case "Love Token":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(
                        "battlewedding",
                        "Platinum Mech Dragon",
                        req.Name,
                        quant,
                        req.Temp
                    );
                    break;

                case "Time Heart":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("portalmazec", "Vorefax ", req.Name, quant, req.Temp);
                    break;

                case "Storm Heart":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("pride", "Valsarian", req.Name, quant, req.Temp);
                    break;

                case "Mercutio's Heart":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("mercutio", "Mercutio", req.Name, quant, req.Temp);
                    break;

                case "Racing Trophy":
                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop("Racing Trophy");
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.ChainComplete(746);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

                case "Sphinx Sentinel":
                    CM.BuyAllMerge(req.Name);
                    break;

                case "Dread Deadmoor BattleAxe":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("deadmoor", "Lucid Nightmare", req.Name, quant, req.Temp);
                    break;

                case "SMU Brutalcorn's Horn":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "ultrabrutalcorn",
                        "SMU BrutalCorn",
                        req.Name,
                        quant,
                        req.Temp
                    );
                    break;

                case "Ouroboros Scale":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9443);
                    bool needBellona = !Core.CheckInventory("Bellona's Edict of War");
                    bool needSleih = !Core.CheckInventory("Sleih's Changeling Records");
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, req.Quantity))
                    {
                        while (!Bot.ShouldExit && needBellona && needSleih)
                        {
                            if (Bot.Map.Name != "camlan")
                                Core.Join("camlan");

                            if (Bot.Player.Cell != "r9")
                                Core.Jump("r9", "Let");


                            Monster bellona = Bot.Monsters.CurrentAvailableMonsters?.FirstOrDefault(m => m?.Alive == true && m.MapID == 22);
                            Monster sleih = Bot.Monsters.CurrentAvailableMonsters?.FirstOrDefault(m => m?.Alive == true && m.MapID == 23);

                            // Priority: missing drop
                            if (needBellona && bellona != null)
                                Bot.Combat.Attack(22);

                            else if (needSleih && sleih != null)
                                Bot.Combat.Attack(23);

                            // If both drops owned OR priority mob dead → kill the other for respawn sync
                            else if (bellona != null)
                                Bot.Combat.Attack(22);

                            else if (sleih != null)
                                Bot.Combat.Attack(23);

                            Bot.Sleep(200);
                        }
                        Core.Logger("Good luck with this \"ultra\"! --the maw");
                        Core.HuntMonster("camlan", "Metamorphosis Maw", "Alchemic Snake Scale", log: false);
                    }
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;

                case "Libran Scales":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("lightoviacave", "Imbalanced Mage", req.Name, quant, req.Temp);
                    break;

                case "Akriloth's Scale":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("shadowstrike", "Sepulchuroth", req.Name, quant, req.Temp);
                    break;

                case "ArchFiend DragonKnight's Scale":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "underlair",
                        "ArchFiend DragonKnight",
                        req.Name,
                        quant,
                        req.Temp
                    );
                    break;

                case "Fa's Gamer Fuel":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("garden", "Fa", req.Name, quant, req.Temp);
                    break;

                case "ARTX 3090 Controller":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("mverse", "Major Mushroom", req.Name, quant, req.Temp);
                    break;

                case "Soulseeker's Grim Hood":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("marsh2", "Soulseeker", req.Name, quant, req.Temp);
                    break;

                case "Nothing's Solus":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("pocketdimension", "Nothing", req.Name, quant, req.Temp);
                    break;

                case "Lucky Pet":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("pilgrimage", "Lucky", req.Name, quant, req.Temp);
                    break;

                case "Second Chance Coin":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7781);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Adv.BuyItem("onsen", 1926, "Gachapon Coin", Log: false);
                        Core.HuntMonster(
                            "yokaigrave",
                            "Skello Kitty",
                            "Skello Kitty Bone",
                            log: false
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Treasure Chest":
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster("finalbattle", "r2", "Left", "*", req.Name, quant, req.Temp);
                    break;

                case "Divine Elixir":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("poisonforest", "Xavier Lionfang", req.Name, quant, req.Temp);
                    break;

                case "Ultra Lobthulu's Fortune":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("ultralob", "Ultra Lobthulhu", req.Name, quant, req.Temp);
                    break;

                case "Enchanted Martial Artist's Gi":
                    SSM.BuyAllMerge(req.Name);
                    break;

                case "Strong Axe of Golmoth":
                    DPM.BuyAllMerge(req.Name);
                    break;

                case "Fortitude Tonic":
                    PotionBuyer. // Call the method with specific parameters to farm Fortitude Tonics with a quantity of 50
                    INeedYourStrongestPotions(
                        new[] { "Fortitude Tonic" },
                        new bool[] { true },
                        quant,
                        true,
                        true
                    );
                    break;

                case "Strong Drag's Intact Wing":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("dracocon", "Strong Drag", req.Name, quant, req.Temp);
                    break;

                case "Chained Rune Bonebreaker":
                case "Chained Rune Bonebreakers":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(
                        "archportal",
                        "High Legion Inquisitor",
                        req.Name,
                        quant,
                        req.Temp
                    );
                    break;

                case "Noble Sacrifice":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID("necrodungeon", 48, req.Name, quant, req.Temp);
                    break;

                case "The Answer":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("battlefowl", "Zeuster Projection", req.Name, quant, req.Temp);
                    break;

                case "Astero's Insight":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("fortressdelve", "Astero", req.Name, quant, req.Temp);
                    break;

                case "Death's Oversight":
                    Core.EquipClass(ClassType.Solo);
                    Core.KillMonster(
                        "shadowattack",
                        "Boss",
                        "Left",
                        "Death",
                        req.Name,
                        quant,
                        req.Temp
                    );
                    break;

                case "Death Pit Arena Medal":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("deathpit", "Training Dummy", req.Name, quant, req.Temp);
                    break;

                case "Enchanted DeathKnight":
                    BCM.BuyAllMerge(req.Name);
                    break;

                case "Super Death's Scythe Fragment":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("superdeath", "Super Death", req.Name, quant, req.Temp);
                    break;

                case "Celestial Wings of Guiding":
                    CRM.BuyAllMerge(req.Name);
                    break;

                case "Blessed Coffee Cup":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5405);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(
                            "sandsea",
                            "Oasis Monkey",
                            "Pally Luwak Beans",
                            log: false
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Northern Crown":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("snowmore", "Jon S'NOOOOOOO", req.Name, quant, req.Temp);
                    break;

                case "Azkorath's Wing":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("infernalspire", "Azkorath", req.Name, quant, req.Temp);
                    break;

                case "Fiendish Outlaw":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("fiendpast", "Dage the Lich", req.Name, quant, req.Temp);
                    break;

                case "Fiendish Remains":
                    VR.Storyline();
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9532);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(
                            "voidrefuge",
                            "Paladin Ascendant",
                            "Sussurating Helm",
                            3,
                            log: false
                        );
                        Core.HuntMonster(
                            "voidrefuge",
                            "Nation Outrider",
                            "Scarred Coin",
                            8,
                            log: false
                        );
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("voidrefuge", "Carnage", "Carnage's Ichor", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Glass Horns":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("ashfallcamp", "Blackrawk", req.Name, quant, req.Temp);
                    break;

                case "Fiend Champion's Spike":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("originul", "Fiend Champion", req.Name, quant, req.Temp);
                    break;

                case "Earth Stone":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(3317);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(
                            "fableforest",
                            "Earth Elemental",
                            "Earth Aura",
                            5,
                            log: false
                        );
                        Core.HuntMonster(
                            "fableforest",
                            "Undead Satyr",
                            "Satyr Hoof",
                            5,
                            log: false
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Dragon Runestone":
                    // Core.FarmingLogger(req.Name, quant);
                    // Adv.BuyItem("alchemyacademy", 395, 62749, quant, 1, 8777);
                    // Core.BuyItem("alchemyacademy", 395, "Dragon Runestone", quant, 8844);
                    Farm.DragonRunestone(quant);
                    break;

                case "Arcangrove Tower House":
                    TLWHM.BuyAllMerge(req.Name);
                    break;

                case "Nevanna's Revelation":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("gaiazor", "Nevanna", req.Name, quant, req.Temp);
                    break;

                case "Star Scrap":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("starsinc", "Star Sprites", req.Name, quant, req.Temp);
                    break;

                case "Rising Star Token":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("herolobby", "Training Partner", req.Name, quant, req.Temp);
                    break;

                case "Dark Stars":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("deadlines", "Eternal Dragon", req.Name, quant, req.Temp);
                    break;

                case "Star Sapphire Fragment":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("skytower", "Star Sapphire", req.Name, quant, req.Temp);
                    break;

                case "Moon Rock Fragments":
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster("lunacove", "r2", "Right", "*", req.Name, quant, req.Temp);
                    break;

                case "Blood Moon Warrior":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("marchosiasfight", "Marchosias", req.Name, quant, req.Temp);
                    break;

                case "Celestial Khopesh":
                    MoonlightKhopeshMerge.BuyAllMerge(req.Name);
                    break;

                case "The Moon's Reflection":
                    Bot.Quests.UpdateQuest(8000);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("Astravia", "The Moon", req.Name, quant, req.Temp);
                    break;

                case "Golden Sun Seal":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("titanattack", "Titanic Paladin", req.Name, quant, req.Temp);
                    break;

                case "Sun Zone Chit":
                    int remainingQuant = quant - Bot.Inventory.GetQuantity(req.Name);

                    // Calculate the maximum items per quest turn-in (5 * max reward = 35)
                    int maxRewardPerTurnIn = 35;

                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.EnsureAccept(9252);

                        // Adjust quantities based on remaining required items
                        int infernalQty = remainingQuant < maxRewardPerTurnIn ? 10 : 50;
                        int seraphicQty = remainingQuant < maxRewardPerTurnIn ? 10 : 50;
                        int marineQty = remainingQuant < maxRewardPerTurnIn ? 1 : 5;

                        // Equip solo class for Marine Snow
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("sunlightzone", "Marine Snow", "Marine Sample", marineQty);

                        // Equip farming class for the rest
                        Core.EquipClass(ClassType.Farm);
                        Core.KillMonster(
                            "sunlightzone",
                            "r9",
                            "left",
                            "*",
                            "seraphic sample",
                            seraphicQty
                        );
                        Core.KillMonster(
                            "sunlightzone",
                            "r8",
                            "left",
                            "*",
                            "infernal sample",
                            infernalQty
                        );
                        Core.EnsureCompleteMulti(9252);
                        Bot.Wait.ForPickup(req.Name);

                        // Recalculate remaining quantity after each turn-in
                        remainingQuant = quant - Bot.Inventory.GetQuantity(req.Name);
                    }
                    break;

                case "Armor of the Sun":
                    ThirdspellMerge.BuyAllMerge(req.Name);
                    break;

                case "The Sun's Enlightenment":
                    Bot.Quests.UpdateQuest(8256);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("astraviacastle", "The Sun", req.Name, quant, req.Temp);
                    break;

                case "Trumpet":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("astraviajudge", "Trumpeter", req.Name, quant, req.Temp);
                    break;

                case "Enchanted Lance of Doom Reborn":
                    ShadowMerge.BuyAllMerge(req.Name);
                    break;

                case "Minos' Sentence":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("judgement", "Minos", req.Name, quant, req.Temp);
                    break;

                case "Darkon's Receipt":
                    Darkon.FarmReceipt(quant);
                    break;

                case "Teeth":
                    Darkon.Teeth(quant);
                    break;

                case "La's Gratitude":
                    Darkon.LasGratitude(quant);
                    break;

                case "Astravian Medal":
                    Darkon.AstravianMedal(quant);
                    break;

                case "A Melody":
                    Darkon.AMelody(quant);
                    break;

                case "Bandit's Correspondence":
                    Darkon.BanditsCorrespondence(quant);
                    break;

                case "Suki's Prestige":
                    Darkon.SukisPrestiege(quant);
                    break;

                case "Ancient Remnant":
                    Darkon.AncientRemnant(quant);
                    break;

                case "Mourning Flower":
                    Darkon.WheelofFortune(quant, 0);
                    break;

                case "Unfinished Musical Score":
                    Darkon.UnfinishedMusicalScore(quant);
                    break;
            }
        }
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "85452",
            "1 - The Magician's Desire",
            "Mode: [select] only\nShould the bot buy \"1 - The Magician's Desire\" ?",
            false
        ),
        new Option<bool>(
            "85453",
            "2 - The High Priestess' Intuition",
            "Mode: [select] only\nShould the bot buy \"2 - The High Priestess' Intuition\" ?",
            false
        ),
        new Option<bool>(
            "85455",
            "3 - The Empress' Initiative",
            "Mode: [select] only\nShould the bot buy \"3 - The Empress' Initiative\" ?",
            false
        ),
        new Option<bool>(
            "85457",
            "4 - The Emperor's Authority",
            "Mode: [select] only\nShould the bot buy \"4 - The Emperor's Authority\" ?",
            false
        ),
        new Option<bool>(
            "85459",
            "5 - The Hierophant's Servitude",
            "Mode: [select] only\nShould the bot buy \"5 - The Hierophant's Servitude\" ?",
            false
        ),
        new Option<bool>(
            "85461",
            "6 - The Lovers' Embrace",
            "Mode: [select] only\nShould the bot buy \"6 - The Lovers' Embrace\" ?",
            false
        ),
        new Option<bool>(
            "85463",
            "7 - The Chariot's Triumph",
            "Mode: [select] only\nShould the bot buy \"7 - The Chariot's Triumph\" ?",
            false
        ),
        new Option<bool>(
            "85465",
            "8 - Justice's Righteousness",
            "Mode: [select] only\nShould the bot buy \"8 - Justice's Righteousness\" ?",
            false
        ),
        new Option<bool>(
            "85467",
            "9 - The Hermit's Solitude",
            "Mode: [select] only\nShould the bot buy \"9 - The Hermit's Solitude\" ?",
            false
        ),
        new Option<bool>(
            "85469",
            "10 - Wheel of Fortune's Destiny",
            "Mode: [select] only\nShould the bot buy \"10 - Wheel of Fortune's Destiny\" ?",
            false
        ),
        new Option<bool>(
            "85471",
            "11 - Strength's Fortitude",
            "Mode: [select] only\nShould the bot buy \"11 - Strength's Fortitude\" ?",
            false
        ),
        new Option<bool>(
            "85473",
            "12 - The Hanged Man's Discernment",
            "Mode: [select] only\nShould the bot buy \"12 - The Hanged Man's Discernment\" ?",
            false
        ),
        new Option<bool>(
            "85475",
            "13 - Death's Mortality",
            "Mode: [select] only\nShould the bot buy \"13 - Death's Mortality\" ?",
            false
        ),
        new Option<bool>(
            "85477",
            "14 - Temperance's Frugality",
            "Mode: [select] only\nShould the bot buy \"14 - Temperance's Frugality\" ?",
            false
        ),
        new Option<bool>(
            "85479",
            "15 - Devilish Temptation",
            "Mode: [select] only\nShould the bot buy \"15 - Devilish Temptation\" ?",
            false
        ),
        new Option<bool>(
            "85481",
            "16 - The Tower's Upheaval",
            "Mode: [select] only\nShould the bot buy \"16 - The Tower's Upheaval\" ?",
            false
        ),
        new Option<bool>(
            "85483",
            "17 - The Star's Hope",
            "Mode: [select] only\nShould the bot buy \"17 - The Star's Hope\" ?",
            false
        ),
        new Option<bool>(
            "85485",
            "18 - The Moon's Illusion",
            "Mode: [select] only\nShould the bot buy \"18 - The Moon's Illusion\" ?",
            false
        ),
        new Option<bool>(
            "85487",
            "19 - The Sun's Optimism",
            "Mode: [select] only\nShould the bot buy \"19 - The Sun's Optimism\" ?",
            false
        ),
        new Option<bool>(
            "85489",
            "20 - Judgement's Absolution",
            "Mode: [select] only\nShould the bot buy \"20 - Judgement's Absolution\" ?",
            false
        ),
        new Option<bool>(
            "85491",
            "21 - The World's Voyage",
            "Mode: [select] only\nShould the bot buy \"21 - The World's Voyage\" ?",
            false
        ),
    };
}
