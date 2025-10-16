/*
name: Merge Shop Bot Generator/Helper
description: Fill in the map and shop ID and this tool will generate most of the merge bot for you, then you fill in the rest
tags: merge, shop, generator, helper, developer
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;
using Skua.Core.Models;
using Skua.Core.Models.Shops;
using Skua.Core.Models.Items;
using Skua.Core.Utils;
using System.IO;
using System.Diagnostics;



public class CaseStorage
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.RunCore();
    }

    public static readonly Dictionary<string, string> Cases = new()
    {

        /*
        // Example:
                {
                    "Item Name",
                    @"
                case ""Item Name"":
                Core.FarmingLogger(req.Name, quant);
                Core.EquipClass(ClassType.Farm);
                Core.AddDrop(req.ID);
                if (Core.IsMember)
                    Core.RegisterQuests(10255);
                while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                {
                    Core.HuntMonsterQuest(10253,
                        (""map"", ""monster"", ClassType.Farm),
                        (""map"", ""drop"", ClassType.Solo)
                    );
                    Bot.Wait.ForPickup(req.Name);
                }
                Core.CancelRegisteredQuests();
                break;
               "
                },
        */

    {
    "Astravian Medal",
    @"
case ""Astravian Medal"":
                    Core.FarmingLogger(req.Name, quant);
                    Darkon.AstravianMedal(quant);
                    break;
    "
},
{
    "A Melody",
    @"
case ""A Melody"":
                    Darkon.AMelody(quant);
                    break;
    "
},
{
    "Re's Party Attire",
    @"
case ""Re's Party Attire"":
                    Core.HuntMonster(""astraviajudge"", ""La"", req.Name, quant);
                    break;
    "
},
{
    "La's Gratitude",
    @"
case ""La's Gratitude"":
                    Darkon.LasGratitude(quant);
                    break;
    "
},
{
    "The Moon's Head",
    @"
case ""The Moon's Head"":
                    Core.HuntMonster(""astravia"", ""The Moon"", req.Name, isTemp: false);
                    break;
    "
},
{
    "The Moon's Cloak",
    @"
case ""The Moon's Cloak"":
                    Core.HuntMonster(""astravia"", ""The Moon"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Astravian Sickle",
    @"
case ""Astravian Sickle"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""astravia"", ""Creature 28"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Sheathed Urban Duelist Katana",
    @"
case ""Sheathed Urban Duelist Katana"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""astravia"", ""Creature 28"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Urban Duelist Katana and Sheath",
    @"
case ""Urban Duelist Katana and Sheath"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""astravia"", ""Creature 28"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Condensed Aversion",
    @"
case ""Condensed Aversion"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""astravia"", ""Creature 28"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Astravian Urban Duelist Locks",
    @"
case ""Astravian Urban Duelist Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""astravia"", ""Creature 27"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Astravian Urban Duelist Hair",
    @"
case ""Astravian Urban Duelist Hair"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""astravia"", ""Creature 27"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Suki's Prestige",
    @"
case ""Suki's Prestige"":
                    Core.Logger($""Farming {req.Name} ({currentQuant}/{quant})"");
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Darkon.SukisPrestiege(quant);
                    break;
    "
},
{
    "Prince Drago's Attire",
    @"
case ""Prince Drago's Attire"":
                    Core.HuntMonster(""astraviapast"", ""Forsaken Husk"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Prince Drago's Hair",
    @"
case ""Prince Drago's Hair"":
                    Core.HuntMonster(""astraviapast"", ""Forsaken Husk"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Prince Drago's Dark Attire",
    @"
case ""Prince Drago's Dark Attire"":
                    Core.HuntMonster(""astraviapast"", ""Forsaken Husk"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Suki's Casual Armor",
    @"
case ""Suki's Casual Armor"":
                    Core.HuntMonster(""astraviapast"", ""Aurola"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Suki's Ponytail",
    @"
case ""Suki's Ponytail"":
                    Core.HuntMonster(""astraviapast"", ""Aurola"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Regulus' Hair",
    @"
case ""Regulus' Hair"":
                    Core.HuntMonster(""astraviapast"", ""Regulus"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Titania's Hair",
    @"
case ""Titania's Hair"":
                    Core.HuntMonster(""astraviapast"", ""Titania"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Teeth",
    @"
case ""Teeth"":
                    Darkon.Teeth(quant);
                    break;
    "
},
{
    "Bandit's Correspondence",
    @"
case ""Bandit's Correspondence"":
                    Darkon.BanditsCorrespondence(quant);
                    break;
    "
},
{
    "Suki's Sword",
    @"
case ""Suki's Sword"":
                    Core.HuntMonsterMapID(""eridanipast"", 19, req.Name, isTemp: false);
                    break;
    "
},
{
    "Ancient Remnant",
    @"
case ""Ancient Remnant"":
                    Darkon.AncientRemnant(quant);
                    break;
    "
},
{
    "Mourning Flower",
    @"
case ""Mourning Flower"":
                    Darkon.WheelofFortune(quant, 0);
                    break;
    "
},
{
    "Jus Divinum Scale",
    @"
case ""Jus Divinum Scale"":
                    Darkon.WheelofFortune(0, quant);
                    break;
    "
},
{
    "Hours Minutes Seconds",
    @"
case ""Hours Minutes Seconds"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""magician"", ""Human Clock"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Human Clock Face House Item",
    @"
case ""Human Clock Face House Item"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""magician"", ""Human Clock"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Creature 10 Tail",
    @"
case ""Creature 10 Tail"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""magician"", ""Human Clock"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Creature 10 Half Wing",
    @"
case ""Creature 10 Half Wing"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""magician"", ""Human Clock"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Creature 10 Horns",
    @"
case ""Creature 10 Horns"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""magician"", ""Human Clock"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Creature 10 Horned Locks",
    @"
case ""Creature 10 Horned Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""magician"", ""Human Clock"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Chrono Bauble",
    @"
case ""Chrono Bauble"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(10017, ""magician"", ""Human Clock"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Drakath Armor",
    @"
case ""Drakath Armor"":
                    DAB.DrakathArmorQuest();
                    break;
    "
},
{
    "Champion Drakath Insignia",
    @"
case ""Champion Drakath Insignia"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.Logger(""You have to kill Champion Drakath weekly to get his insignias, use your army to kill it easily"");
                        return;
                    }
                    break;
    "
},
{
    "Original Drakath Armor",
    @"
case ""Original Drakath Armor"":
                    DAB.DrakathOriginalArmor();
                    break;
    "
},
{
    "Blade of Chaos",
    @"
case ""Blade of Chaos"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ultradrakath"", ""Champion of Chaos"", req.Name, quant, false, false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Chaos Avenger's Greatsword",
    @"
case ""Chaos Avenger's Greatsword"":
                    Core.EquipClass(ClassType.Solo);
                    Adv.BuyItem(""championdrakath"", 2056, req.Name);
                    break;
    "
},
{
    "Legendary Sword of Dragon Control",
    @"
case ""Legendary Sword of Dragon Control"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.KillVath(req.Name, quant);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "The Supreme Arcane Staff",
    @"
case ""The Supreme Arcane Staff"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ledgermayne"", ""Ledgermayne"", ""The Supreme Arcane Staff"", 1, false, false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Discordia Rose of Chaos",
    @"
case ""Discordia Rose of Chaos"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""palooza"", ""Chaos Lord Discordia"", req.Name, quant, false, false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Chaos Rose",
    @"
case ""Chaos Rose"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""palooza"", ""Chaos Lord Discordia"", req.Name, quant, false, false);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Unfinished Musical Score",
    @"
case ""Unfinished Musical Score"":
                    Darkon.UnfinishedMusicalScore(quant);
                    break;
    "
},
{
    "Darkon's Receipt",
    @"
case ""Darkon's Receipt"":
                    Darkon.FarmReceipt(quant);
                    break;
    "
},
{
    "Debris Fragment",
    @"
case ""Debris Fragment"":
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""garden"", ""r2"", ""Left"", ""*"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Darkon's Debris 1952",
    @"
case ""Darkon's Debris 1952"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(""Darkon's Receipt"");

                    bool EnoughPeople = false;
                    Core.Join(""doomvault"", ""r5"", ""Left"", true);

                    Core.RegisterQuests(7325);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name))
                    {
                        if (Bot.Map.Name.ToLower() == ""doomvault"")
                        {
                            while (!Bot.ShouldExit && Bot.Player.Cell != ""r5"")
                            {
                                Core.Jump(""r5"", ""Left"");
                                Core.Sleep();
                            }

                            EnoughPeople = Bot.Map.CellPlayers?.Count >= 3;

                            if (!EnoughPeople && Core.IsMember)
                                Core.HuntMonster(""ultravoid"", ""Ultra Kathool"", ""Ingredients?"", 22, false, publicRoom: true);
                            else
                                Core.KillMonster(""doomvault"", ""r5"", ""Left"", ""Binky"", ""Ingredients?"", 22, false, publicRoom: true);

                            Bot.Wait.ForPickup(""Darkon's Receipt"");
                        }
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Darkon's Debris 1935.1",
    @"
case ""Darkon's Debris 1935.1"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""garden"", ""Creature 12"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Darkon's Debris 66 Angel Wing",
    @"
case ""Darkon's Debris 66 Angel Wing"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""garden"", ""Creature 12"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Darkon's Debris 66 Fallen Wing",
    @"
case ""Darkon's Debris 66 Fallen Wing"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""garden"", ""Creature 12"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Fa's Gamer Fuel",
    @"
case ""Fa's Gamer Fuel"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""garden"", ""Fa"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Darkon's Instant Noodle",
    @"
case ""Darkon's Instant Noodle"":
                    Adv.BuyItem(""garden"", 1831, req.Name, quant);
                    break;
    "
},
{
    "Astravia Castle House",
    @"
case ""Astravia Castle House"":
                    Core.HuntMonster(""astraviajudge"", ""La"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Grace Orb",
    @"
case ""Grace Orb"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9291);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""neofortress"", ""Vindicator Recruit"", ""Grace Extracted"", 20, false, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Vindicator Badge",
    @"
case ""Vindicator Badge"":
                    Core.FarmingLogger(req.Name, quant);
                    Bot.Quests.UpdateQuest(8297);
                    Core.RegisterQuests(8299);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.KillMonster(""trygve"", ""r3"", ""Left"", ""Blood Eagle"", ""Eagle Heart"", 8, log: false);
                        Core.KillMonster(""trygve"", ""r4"", ""Left"", ""Rune Boar"", ""Boar Heart"", 8, log: false);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""trygve"", ""Gramiel"", ""Vindicator Seal"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Vindicator Soldier's Hair",
    @"
case ""Vindicator Soldier's Hair"":
                    Core.HuntMonster(""neofortress"", ""Vindicator Soldier"", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Vindicator Scout's Bow",
    @"
case ""Vindicator Scout's Bow"":
                    Core.HuntMonster(""neofortress"", ""Vindicator Recruit"", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Blessed Sigil of Vindication",
    @"
case ""Blessed Sigil of Vindication"":
                    Core.HuntMonster(""neofortress"", ""Vindicator General"", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Hammer of Vindication",
    @"
case ""Hammer of Vindication"":
                    Core.HuntMonster(""neofortress"", ""Vindicator General"", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Hammers of Vindication",
    @"
case ""Hammers of Vindication"":
                    Core.HuntMonster(""neofortress"", ""Vindicator General"", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Hollow Soul",
    @"
case ""Hollow Soul"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAcceptmultiple(new[] { 7553, 7555 });
                        Core.KillMonster(""shadowrealm"", ""r2"", ""Left"", ""Gargrowl"", ""Darkseed"", 8, log: false);
                        Core.KillMonster(""shadowrealm"", ""r2"", ""Left"", ""Shadow Guardian"", ""Shadow Medallion"", 5, log: false);
                        Core.EnsureComplete(7553);
                        Core.EnsureComplete(7555);
                    }
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Bone Dust",
    @"
case ""Bone Dust"":
                    Farm.BattleUnderB(req.Name, quant);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Gilded Scout's Quiver",
    @"
case ""Gilded Scout's Quiver"":
                    Core.HuntMonster(""neofortress"", ""Vindicator Recruit"", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Blessed Rune of Vindication",
    @"
case ""Blessed Rune of Vindication"":
                    Core.HuntMonster(""neofortress"", ""Vindicator General"", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Battlegear of Vindication",
    @"
case ""Battlegear of Vindication"":
                    Core.HuntMonster(""neofortress"", ""Vindicator General"", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Aura of Awe",
    @"
case ""Aura of Awe"":
                    if (!Core.IsMember)
                    {
                        Core.Logger(""You need to be member."");
                        break;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(2939);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""crashsite"", ""Mithril Man"", ""Evolution Of Awe"", 13, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Blade of Awe",
    @"
case ""Blade of Awe"":
                    Adv.BuyItem(""museum"", 631, req.Name);
                    break;
    "
},
{
    "Spear of Awe",
    @"
case ""Spear of Awe"":
                    Adv.BuyItem(""museum"", 631, req.Name);
                    break;
    "
},
{
    "Dagger of Awe",
    @"
case ""Dagger of Awe"":
                    Adv.BuyItem(""museum"", 631, req.Name);
                    break;
    "
},
{
    "Staff of Awe",
    @"
case ""Staff of Awe"":
                    Adv.BuyItem(""museum"", 631, req.Name);
                    break;
    "
},
{
    "Guardian Dragon Pet",
    @"
case ""Guardian Dragon Pet"":
                    Adv.BuyItem(""museum"", 631, req.Name);
                    break;
    "
},
{
    "Guardian Patent",
    @"
case ""Guardian Patent"":
                    if (Bot.Flash.GetGameObject<int>(""world.myAvatar.objData.intAQ"") > 0)
                    {
                        Adv.BuyItem(""museum"", 53, ""Guardian Patent"");
                        break;
                    }
                    else Core.Logger(""Active Aq Guardian Acc Required for this Item."");
                    break;
    "
},
{
    "Baby Red Dragon",
    @"
case ""Baby Red Dragon"":
                    Adv.BuyItem(""AriaPet"", 12, req.Name);
                    break;
    "
},
{
    "Armor of Awe",
    @"
case ""Armor of Awe"":
                    AoA.GetArmor();
                    break;
    "
},
{
    "Cape of Awe",
    @"
case ""Cape of Awe"":
                    Awe.GetAweRelic(""Cape"", 4178, 1, 1, ""doomvault"", ""Binky"");
                    Adv.BuyItem(""museum"", 1129, ""Cape of Awe"");
                    break;

    "
},
{
    "Obsidian Hollowborn Dragon Statue",
    @"
case ""Obsidian Hollowborn Dragon Statue"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""hbchallenge"", ""Nameless Dragonlord"", req.Name, quant, req.Temp, false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Hollowborn Dragon Heart",
    @"
case ""Hollowborn Dragon Heart"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(9990, ""hbchallenge"", ""Nameless Dragonlord"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Hollowborn Wyvern Heart",
    @"
case ""Hollowborn Wyvern Heart"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(9989, ""hbchallenge"", ""Hollowborn Wyvern"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Hollowborn Dragonknight Armet",
    @"
case ""Hollowborn Dragonknight Armet"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""hbchallenge"", ""Nameless Dragonlord"", req.Name, quant, req.Temp, false);
                    break;

    "
},
{
    "Hollowborn DragonBerserker Helm",
    @"
case ""Hollowborn DragonBerserker Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""hbchallenge"", ""Nameless Dragonlord"", req.Name, quant, req.Temp, false);
                    break;

    "
},
{
    "Hollowborn Writ",
    @"
case ""Hollowborn Writ"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8418);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""hbchallenge"", ""r3"", ""Right"", ""Judge's Minion"", ""Judge's Minion Judged"", 12);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Hollowborn Residue",
    @"
case ""Hollowborn Residue"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8996); //Hazardous Hybrid 8996
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""hbchallenge"", ""r5"", ""Left"", ""Chaoroot Compound"", ""Inert Charoot"", 8);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "NCS Gem",
    @"
case ""NCS Gem"":
                    Daily.NCSGem(quant);
                    Core.Logger($""{req.Name} is a daily drop, you need {quant - Bot.Inventory.GetQuantity(req.Name)} more to buy this item. Run the script again tomorrow if you don't have enough."");
                    break;
    "
},
{
    "Love Potion",
    @"
case ""Love Potion"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9643);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""sewerpink"", ""Sewer1"", ""Left"", ""*"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Cursed Ring",
    @"
case ""Cursed Ring"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9794);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""greed"", ""Cursed Treasure"", ""Ring Found"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Frozen Diamond",
    @"
case ""Frozen Diamond"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9795);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""greed"", ""Ice Crystal"", ""Frozen Diamond Found"", 5, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Krenos Spirit Katana",
    @"
case ""Krenos Spirit Katana"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9804);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""shimazu"", ""Shimazu"", ""First Rune"", log: false);
                        Core.GetMapItem(13328, map: ""evilmarsh"");
                        Core.HuntMonster(""seraphicwarlaken"", ""Rayce"", ""Third Rune"", log: false);
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""pyrewatch"", ""Firestorm Major"", ""Fourth Rune"", log: false);
                        Core.GetMapItem(13329, map: ""icewindpass"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Energy Dragon Scale",
    @"
case ""Energy Dragon Scale"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""thunderfang"", ""Tonitru"", req.Name, quant, req.Temp, false);
                    break;

    "
},
{
    "Vindicator Crest",
    @"
case ""Vindicator Crest"":
                    VindicatorCrest(quant);
                    break;
    "
},
{
    "Gramiel's Emblem",
    @"
case ""Gramiel's Emblem"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dawnsanctum"", ""Celestial Gramiel"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Gramiel's Shattered Enoch",
    @"
case ""Gramiel's Shattered Enoch"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dawnsanctum"", ""Celestial Gramiel"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Gramiel's Shattered Enochs",
    @"
case ""Gramiel's Shattered Enochs"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dawnsanctum"", ""Celestial Gramiel"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Vindicator Draconian",
    @"
case ""Vindicator Draconian"":
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""dawnsanctum"", ""r7"", ""Left"", ""Vindicator Draconian"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Draconian Vindication Axe",
    @"
case ""Draconian Vindication Axe"":
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""dawnsanctum"", ""r7"", ""Left"", ""Vindicator Draconian"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Draconian Vindication Axes",
    @"
case ""Draconian Vindication Axes"":
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""dawnsanctum"", ""r7"", ""Left"", ""Vindicator Draconian"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Hollowborn Draconian Morph",
    @"
case ""Hollowborn Draconian Morph"":
                    Core.KillMonster(""dawnsanctum"", ""r8"", ""Left"", ""Hollowborn Draconian"", req.Name, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Dawn Vindicator Helm",
    @"
case ""Dawn Vindicator Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    switch (req.Name)
                    {
                        case ""Dawn Vindicator Helm"":
                        case ""Dawn Vindicator Soldier"":
                            Core.HuntMonster(""trygve"", ""Vindicator Recruit"", req.Name, isTemp: false);
                            break;

                        case ""Dawn Vindicator Sword"":
                            Core.HuntMonster(""trygve"", ""Vindicator Soldier"", req.Name, isTemp: false);
                            break;

                        case ""Dawn Vindicator Swords"":
                            Core.HuntMonster(""trygve"", ""Vindicator Recruit"", req.Name, isTemp: false);
                            break;
                    }
                    break;
    "
},
{
    "Dawn Vindicator Sword",
    @"
case ""Dawn Vindicator Sword"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    switch (req.Name)
                    {
                        case ""Dawn Vindicator Helm"":
                        case ""Dawn Vindicator Soldier"":
                            Core.HuntMonster(""trygve"", ""Vindicator Recruit"", req.Name, isTemp: false);
                            break;

                        case ""Dawn Vindicator Sword"":
                            Core.HuntMonster(""trygve"", ""Vindicator Soldier"", req.Name, isTemp: false);
                            break;

                        case ""Dawn Vindicator Swords"":
                            Core.HuntMonster(""trygve"", ""Vindicator Recruit"", req.Name, isTemp: false);
                            break;
                    }
                    break;
    "
},
{
    "Dawn Vindicator Soldier",
    @"
case ""Dawn Vindicator Soldier"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    switch (req.Name)
                    {
                        case ""Dawn Vindicator Helm"":
                        case ""Dawn Vindicator Soldier"":
                            Core.HuntMonster(""trygve"", ""Vindicator Recruit"", req.Name, isTemp: false);
                            break;

                        case ""Dawn Vindicator Sword"":
                            Core.HuntMonster(""trygve"", ""Vindicator Soldier"", req.Name, isTemp: false);
                            break;

                        case ""Dawn Vindicator Swords"":
                            Core.HuntMonster(""trygve"", ""Vindicator Recruit"", req.Name, isTemp: false);
                            break;
                    }
                    break;
    "
},
{
    "Vindicator Priest",
    @"
case ""Vindicator Priest"":
                    VindicatorCrest(20);
                    Adv.BuyItem(""neotower"", 2474, req.Name);
                    break;
    "
},
{
    "Vindicator Beast Tamer",
    @"
case ""Vindicator Beast Tamer"":
                    VindicatorCrest(20);
                    Adv.BuyItem(""neotower"", 2474, req.Name);
                    break;
    "
},
{
    "Vindicator Assassin",
    @"
case ""Vindicator Assassin"":
                    VindicatorCrest(20);
                    Adv.BuyItem(""neotower"", 2474, req.Name);
                    break;
    "
},
{
    "Vindicator Assassin Mask",
    @"
case ""Vindicator Assassin Mask"":
                    VindicatorCrest(10);
                    Adv.BuyItem(""neotower"", 2474, req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Vindicator Beast Tamer Hood",
    @"
case ""Vindicator Beast Tamer Hood"":
                    VindicatorCrest(10);
                    Adv.BuyItem(""neotower"", 2474, req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Vindicator Beast Tamer Mask",
    @"
case ""Vindicator Beast Tamer Mask"":
                    VindicatorCrest(10);
                    Adv.BuyItem(""neotower"", 2474, req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Vindicator Priest Mask",
    @"
case ""Vindicator Priest Mask"":
                    VindicatorCrest(10);
                    Adv.BuyItem(""neotower"", 2474, req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Vindicator Assassin Hood",
    @"
case ""Vindicator Assassin Hood"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""neotower"", 12, req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Vindicator Assassin Dirk",
    @"
case ""Vindicator Assassin Dirk"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""neotower"", 12, req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Vindicator Beast Tamer Claws",
    @"
case ""Vindicator Beast Tamer Claws"":
                    VindicatorCrest(15);
                    Adv.BuyItem(""neotower"", 2474, req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Vindicator Priest Staff",
    @"
case ""Vindicator Priest Staff"":
                    VindicatorCrest(15);
                    Adv.BuyItem(""neotower"", 2474, req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Vindicator Assassin Daggers",
    @"
case ""Vindicator Assassin Daggers"":
                    VindicatorCrest(15);
                    Adv.BuyItem(""neotower"", 2474, req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Dawn Vindication Grace Texts",
    @"
case ""Dawn Vindication Grace Texts"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""neotower"", 28, ""Dawn Vindication Tome"", isTemp: false);
                    Core.HuntMonsterMapID(""neotower"", 28, ""Dawn Vindication Spellbooks"", isTemp: false);
                    Core.HuntMonsterMapID(""neotower"", 28, ""Dawn Vindication Grimoires"", isTemp: false);
                    Adv.BuyItem(""neotower"", 2474, req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Dawn Vindication Tome",
    @"
case ""Dawn Vindication Tome"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""neotower"", 28, req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Dawn Vindication Spellbooks",
    @"
case ""Dawn Vindication Spellbooks"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""neotower"", 28, req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Dawn Vindication Grimoires",
    @"
case ""Dawn Vindication Grimoires"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""neotower"", 28, req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Vindicator Priest Hood",
    @"
case ""Vindicator Priest Hood"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""neotower"", 28, req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Hollowborn Adept",
    @"
case ""Hollowborn Adept"":
                    Core.HuntMonster(""hbchallenge"", ""Shadow Rider"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Hollowborn Locks",
    @"
case ""Hollowborn Locks"":
                    Core.HuntMonster(""hbchallenge"", ""Shadow Rider"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Hollowborn Shag",
    @"
case ""Hollowborn Shag"":
                    Core.HuntMonster(""hbchallenge"", ""Shadow Rider"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Hollowborn Blades",
    @"
case ""Hollowborn Blades"":
                    Core.HuntMonster(""hbchallenge"", ""Shadow Rider"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Hollowborn Cleaver",
    @"
case ""Hollowborn Cleaver"":
                    Core.HuntMonster(""hbchallenge"", ""Shadow Rider"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Hollowborn Executioner's Axe",
    @"
case ""Hollowborn Executioner's Axe"":
                    Core.HuntMonster(""hbchallenge"", ""Shadow Rider"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Hollowborn Spirit",
    @"
case ""Hollowborn Spirit"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7548);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""hbchallenge"", ""Enter"", ""Spawn"", ""Hollowborn Tamer"", ""Hollowborn Tamer Defeated"", 5);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Hollowborn Spite",
    @"
case ""Hollowborn Spite"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7548);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""hbchallenge"", ""Enter"", ""Spawn"", ""Hollowborn Tamer"", ""Hollowborn Tamer Defeated"", 5);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Druidic Soothsayer",
    @"
case ""Druidic Soothsayer"":
                    Adv.BuyItem($""dragonrune"", 689, req.Name, quant);
                    break;              
    "
},
{
    "Druidic Soothsayer's Antlered Skull",
    @"
case ""Druidic Soothsayer's Antlered Skull"":
                    Adv.BuyItem($""dragonrune"", 689, req.Name, quant);
                    break;              
    "
},
{
    "Druidic Soothsayer Hood",
    @"
case ""Druidic Soothsayer Hood"":
                    Adv.BuyItem($""dragonrune"", 689, req.Name, quant);
                    break;              
    "
},
{
    "Druidic Soothsayer's Hooded Visage",
    @"
case ""Druidic Soothsayer's Hooded Visage"":
                    Adv.BuyItem($""dragonrune"", 689, req.Name, quant);
                    break;              
    "
},
{
    "Druidic Soothsayer's Runed Cape",
    @"
case ""Druidic Soothsayer's Runed Cape"":
                    Adv.BuyItem($""dragonrune"", 689, req.Name, quant);
                    break;              
    "
},
{
    "Druidic Soothsayer's Cape",
    @"
case ""Druidic Soothsayer's Cape"":
                    Adv.BuyItem($""dragonrune"", 689, req.Name, quant);
                    break;              
    "
},
{
    "Druidic Soothsayer's Runes",
    @"
case ""Druidic Soothsayer's Runes"":
                    Adv.BuyItem($""dragonrune"", 689, req.Name, quant);
                    break;              
    "
},
{
    "Druidic Soothsayer's Burning Aura",
    @"
case ""Druidic Soothsayer's Burning Aura"":
                    Adv.BuyItem($""dragonrune"", 689, req.Name, quant);
                    break;              
    "
},
{
    "Druidic Soothsayer's Burning Cape",
    @"
case ""Druidic Soothsayer's Burning Cape"":
                    Adv.BuyItem($""dragonrune"", 689, req.Name, quant);
                    break;              
    "
},
{
    "Druidic Soothsayer Rune Gate",
    @"
case ""Druidic Soothsayer Rune Gate"":
                    Adv.BuyItem($""dragonrune"", 689, req.Name, quant);
                    break;              
    "
},
{
    "Druidic Soothsayer Staff",
    @"
case ""Druidic Soothsayer Staff"":
                    Adv.BuyItem($""dragonrune"", 689, req.Name, quant);
                    break;              
    "
},
{
    "Druidic Soothsayer Spike",
    @"
case ""Druidic Soothsayer Spike"":
                    Adv.BuyItem($""dragonrune"", 689, req.Name, quant);
                    break;              
    "
},
{
    "Druidic Soothsayer Spikes",
    @"
case ""Druidic Soothsayer Spikes"":
                    Adv.BuyItem($""dragonrune"", 689, req.Name, quant);
                    break;              
    "
},
{
    "Druidic Soothsayer Gauntlet",
    @"
case ""Druidic Soothsayer Gauntlet"":
                    Adv.BuyItem($""dragonrune"", 689, req.Name, quant);
                    break;              
    "
},
{
    "Death's Oversight",
    @"
case ""Death's Oversight"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.KillMonster(""shadowattack"", ""Boss"", ""Left"", ""Death"", req.Name, quant, false);
                    break;
    "
},
{
    "Death's Scythe",
    @"
case ""Death's Scythe"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""shadowattack"", ""Death"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Unmoulded Fiend Essence",
    @"
case ""Unmoulded Fiend Essence"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""tercessuinotlim"", 1951, ""Unmoulded Fiend Essence"", quant);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Elven Assassin's Locks + Scarf",
    @"
case ""Elven Assassin's Locks + Scarf"":
                    YUM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Elven Assassin's Locks",
    @"
case ""Elven Assassin's Locks"":
                    YUM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Elven Assassin's Hair",
    @"
case ""Elven Assassin's Hair"":
                    YUM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Elven Assassin's Scarf",
    @"
case ""Elven Assassin's Scarf"":
                    YUM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Poisonous Thorn Wreath",
    @"
case ""Poisonous Thorn Wreath"":
                    YUM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Nightshade Assassin Guardian",
    @"
case ""Nightshade Assassin Guardian"":
                    YUM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Reversed Blade of Thorns",
    @"
case ""Reversed Blade of Thorns"":
                    YUM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Reversed Daggers of Thorns",
    @"
case ""Reversed Daggers of Thorns"":
                    YUM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Envenomed Whip of Agony",
    @"
case ""Envenomed Whip of Agony"":
                    YUM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Envenomed Gauntlet",
    @"
case ""Envenomed Gauntlet"":
                    YUM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Poisonous Rogue",
    @"
case ""Poisonous Rogue"":
                    YUM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Nightshade Thorn Assasasin",
    @"
case ""Nightshade Thorn Assasasin"":
                    YUM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Gold Voucher 25k",
    @"
case ""Gold Voucher 25k"":
                    Farm.Voucher(req.Name, req.Quantity);
                    break;
    "
},
{
    "Vindicator Archer's Hat + Locks",
    @"
case ""Vindicator Archer's Hat + Locks"":
                    DFM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Dawn Vindicator Archer",
    @"
case ""Dawn Vindicator Archer"":
                    DFM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Dawn Vindicator General",
    @"
case ""Dawn Vindicator General"":
                    DFM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Bright Bow of the Dawn",
    @"
case ""Bright Bow of the Dawn"":
                    DFM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Vindicator General's Hood",
    @"
case ""Vindicator General's Hood"":
                    DFM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Vindicator General's Hood + Locks",
    @"
case ""Vindicator General's Hood + Locks"":
                    DFM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Blessed Shield of Vindication",
    @"
case ""Blessed Shield of Vindication"":
                    DFM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Blessed Hammers of the Dawn",
    @"
case ""Blessed Hammers of the Dawn"":
                    DFM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Blessed Hammer of the Dawn",
    @"
case ""Blessed Hammer of the Dawn"":
                    DFM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Vindicator Archer's Hat",
    @"
case ""Vindicator Archer's Hat"":
                    DFM.BuyAllMerge(req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Hollow Essence",
    @"
case ""Hollow Essence"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9487);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""hbchallenge"", ""Sentient Hollow"", ""Hollow Essence"", 9);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Hollowborn Vampire Fang",
    @"
case ""Hollowborn Vampire Fang"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9488);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""hbchallenge"", ""Hollowborn Vampire"", ""Shattered Fang"");
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Hollowborn Lycan Claw",
    @"
case ""Hollowborn Lycan Claw"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9489);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""hbchallenge"", ""Hollowborn Lycan"", ""Chipped Claw"");
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Hollowborn Lycan Morph",
    @"
case ""Hollowborn Lycan Morph"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""hbchallenge"", ""Hollowborn Lycan"", req.Name, quant, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Hollowborn Vampire Lord Mask",
    @"
case ""Hollowborn Vampire Lord Mask"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""hbchallenge"", ""Sentient Hollow"", req.Name, quant, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Noble Hollowborn Vampire Wings",
    @"
case ""Noble Hollowborn Vampire Wings"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""hbchallenge"", ""Hollowborn Vampire"", req.Name, quant, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Broken Chain",
    @"
case ""Broken Chain"":
                    //10115 | You and Your Chain
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(10115,
                        (""atlaspromenade"", ""Atlas Light Magus"", ClassType.Farm),
                        (""atlaspromenade"", ""Wrath Guard"", ClassType.Farm),
                        (""atlaspromenade"", ""Usurper Lord Slaine"", ClassType.Solo)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Atlas Crest",
    @"
case ""Atlas Crest"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""atlaspromenade"", ""Atlas Light Magus"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Atlas Axis Blade",
    @"
case ""Atlas Axis Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""atlaspromenade"", ""Atlas Knight"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Legion Token",
    @"
case ""Legion Token"":
                    Legion.FarmLegionToken(quant);
                    break;
    "
},
{
    "Barrensoul Psalm",
    @"
case ""Barrensoul Psalm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""atlaspromenade"", ""Usurper Lord Slaine"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Underworld Ritualist Aeterna Adornment",
    @"
case ""Underworld Ritualist Aeterna Adornment"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""atlaspromenade"", ""Usurper Lord Slaine"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Underworld Ritualist Aeterna Hood",
    @"
case ""Underworld Ritualist Aeterna Hood"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""atlaspromenade"", ""Usurper Lord Slaine"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Underworld Ritualist Aeterna Horns",
    @"
case ""Underworld Ritualist Aeterna Horns"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""atlaspromenade"", ""Usurper Lord Slaine"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Underworld Ritualist Adornments",
    @"
case ""Underworld Ritualist Adornments"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""atlaspromenade"", ""Usurper Lord Slaine"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Underworld Ritualist Horned Mask",
    @"
case ""Underworld Ritualist Horned Mask"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""atlaspromenade"", ""Usurper Lord Slaine"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Underworld Ritualist Mask",
    @"
case ""Underworld Ritualist Mask"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""atlaspromenade"", ""Usurper Lord Slaine"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Underworld Ritualist Horned Hood",
    @"
case ""Underworld Ritualist Horned Hood"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""atlaspromenade"", ""Usurper Lord Slaine"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Underworld Ritualist Adorned Hood",
    @"
case ""Underworld Ritualist Adorned Hood"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""atlaspromenade"", ""Usurper Lord Slaine"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Underworld Ritualist Horned Adornment",
    @"
case ""Underworld Ritualist Horned Adornment"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""atlaspromenade"", ""Usurper Lord Slaine"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Underworld Ritualist Adorned Mask",
    @"
case ""Underworld Ritualist Adorned Mask"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""atlaspromenade"", ""Usurper Lord Slaine"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Underworld Ritualist Hood",
    @"
case ""Underworld Ritualist Hood"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""atlaspromenade"", ""Usurper Lord Slaine"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Underworld Ritualist",
    @"
case ""Underworld Ritualist"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""atlaspromenade"", ""Usurper Lord Slaine"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Pale Corpse Wax Candelabras",
    @"
case ""Pale Corpse Wax Candelabras"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""atlaspromenade"", ""Twisted Warrior"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Pale Corpse Wax Candelabra",
    @"
case ""Pale Corpse Wax Candelabra"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""atlaspromenade"", ""Twisted Warrior"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Atlas Lion Pelt",
    @"
case ""Atlas Lion Pelt"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(10126,
                        (""atlaskingdom"", ""Atlas Leo"", ClassType.Solo),
                        (""atlaskingdom"", ""Atlas Elite"", ClassType.Solo),
                        (""atlaskingdom"", ""Executioner Ladon"", ClassType.Solo));
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Coelho's Tome",
    @"
case ""Coelho's Tome"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""atlaskingdom"", ""Coelho"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Blue Dye",
    @"
case ""Blue Dye"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5898);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //QuarterMaster’s Supplies 5898
                        Core.HuntMonster(""ashfallcamp"", ""Lava Dragoblin"", ""Supply Chest"", 8, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Shadow Orb",
    @"
case ""Shadow Orb"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""innershadows"", ""Shadowcrow"", req.Name, quant, false);
                    break;
    "
},
{
    "Dark Spirit",
    @"
case ""Dark Spirit"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""darkalliance"", ""Shadow Void"", req.Name, quant, false);
                    break;
    "
},
{
    "Mana Gem",
    @"
case ""Mana Gem"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""darkalliance"", ""Underflame Guardian"", req.Name, quant, false);
                    break;

    "
},
{
    "Ultra Shifting Plane Gem",
    @"
case ""Ultra Shifting Plane Gem"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""darkfortress"", ""r3"", ""Left"", ""*"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Cocar dos Dançarinos",
    @"
case ""Cocar dos Dançarinos"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""darkfortress"", ""Dage the Evil"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Dançarinos da Legião",
    @"
case ""Dançarinos da Legião"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""darkfortress"", ""Dage the Evil"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Dage's Favor",
    @"
case ""Dage's Favor"":
                    Core.FarmingLogger(req.Name, quant);
                    Legion.ApprovalAndFavor(0, quant);
                    break;
    "
},
{
    "Axe Of Cerberus",
    @"
case ""Axe Of Cerberus"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""dagefortress"", ""Grrrberus"", req.Name, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Shadow of Cerberus",
    @"
case ""Shadow of Cerberus"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""dagefortress"", ""Grrrberus"", req.Name, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Flail Of Cerberus",
    @"
case ""Flail Of Cerberus"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""dagefortress"", ""Grrrberus"", req.Name, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Dark Palace Token",
    @"
case ""Dark Palace Token"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""dagefortress"", ""Grrrberus"", req.Name, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Ancient Sigil",
    @"
case ""Ancient Sigil"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(4196, 4197);
                    Core.KillMonster(""legioncrypt"", ""r3"", ""Top"", ""*"", req.Name, quant, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Broken Staff",
    @"
case ""Broken Staff"":
                    Dictionary<string, (string, ClassType)> staffPieces = new()
                        {
                            {""Gravedigger"", (""1st Piece of the Staff"", ClassType.Farm)},
                            {""Undead Infantry"", (""2nd Piece of the Staff"", ClassType.Farm)},
                            {""Legion Doomknight"", (""3rd Piece of the Staff"", ClassType.Farm)},
                            {""Brutus"", (""4th Piece of the Staff"", ClassType.Solo)}
                        };

                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(9664);
                        foreach (var kvp in staffPieces)
                        {
                            if (Core.CheckInventory(req.Name, quant))
                                break;

                            Core.EquipClass(kvp.Value.Item2);
                            Core.KillMonster(""legioncrypt"", kvp.Key != ""Brutus"" ? ""r3"" : ""r9"", kvp.Key != ""Brutus"" ? ""Top"" : ""Bottom"", kvp.Key != ""Brutus"" ? ""*"" : kvp.Key, kvp.Value.Item1);
                            Bot.Wait.ForPickup(kvp.Value.Item1);
                        }
                        Core.EnsureComplete(9664);
                    }
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Emblem of Dage",
    @"
case ""Emblem of Dage"":
                    Legion.EmblemofDage(quant);
                    break;
    "
},
{
    "Diamond Token of Dage",
    @"
case ""Diamond Token of Dage"":
                    Legion.DiamondTokenofDage(quant);
                    break;
    "
},
{
    "Crystallized Blood",
    @"
case ""Crystallized Blood"":
                    Core.FarmingLogger(req.Name, quant);
                    if (!Bot.Quests.IsAvailable(793))
                        return;
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(6976);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""underworld"", ""Bloodfiend"", ""Fiend Blood"", 10);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Flaming Skull",
    @"
case ""Flaming Skull"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""underworld"", ""Frozen Pyromancer"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Darkness Shard",
    @"
case ""Darkness Shard"":
                    Core.FarmingLogger(req.Name, quant);
                    DShard.GetShard(quant);
                    if (!Core.CheckInventory(req.Name, quant))
                        Core.Logger(""Not enough Darkness Shards (Daily) try again tomarrow."", stopBot: true);
                    break;
    "
},
{
    "Flame-Forged Metal",
    @"
case ""Flame-Forged Metal"":
                    if (!Bot.Quests.IsAvailable(793))
                        return;
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(6975);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""underworld"", ""Frozen Pyromancer"", ""Stolen Flame"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Soul-Forged Metal",
    @"
case ""Soul-Forged Metal"":
                    if (!Bot.Quests.IsAvailable(793))
                        return;
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(6977);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""underworld"", ""Frozen Pyromancer"", ""Pyromancer Soul Shard"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Argus' Iris",
    @"
case ""Argus' Iris"":
                    Iris(quant);
                    break;
    "
},
{
    "Underworld Linen",
    @"
case ""Underworld Linen"":
                    Linen(quant);
                    break;
    "
},
{
    "River Glowstone",
    @"
case ""River Glowstone"":
                    Glowstone(quant);
                    break;
    "
},
{
    "Teacup Mace",
    @"
case ""Teacup Mace"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""junkhoard"", ""Junk Golem"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Ice Spike",
    @"
case ""Ice Spike"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""frozenlair"", ""Frozen Legionnaire"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Ice Splinter",
    @"
case ""Ice Splinter"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""frozenlair"", ""Frozen Legionnaire"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Sapphire Orb",
    @"
case ""Sapphire Orb"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""frozenlair"", ""Legion Lich Lord"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Necrotic Orb",
    @"
case ""Necrotic Orb"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""frozenlair"", ""Lich Lord"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Indulgence",
    @"
case ""Indulgence"":
                    HeadoftheLegionBeast.Indulgence(quant);
                    break;
    "
},
{
    "Atlas Regalia",
    @"
case ""Atlas Regalia"":
                    Core.FarmingLogger(req.Name, quant);
                    if (Core.CheckInventory(""Chaos Avenger""))
                    {
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonsterQuest(10137, ""atlasfalls"", ""King Zedek"");
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    else
                        Core.Logger($""{req.Name} requires army, please farm it manually."");
                    break;
    "
},
{
    "Arethusa's Black Steel",
    @"
case ""Arethusa's Black Steel"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""atlasfalls"", ""Princess Arethusa"", req.Name, quant, false, false, true);
                    break;
    "
},
{
    "Sundered Soul of Atlas",
    @"
case ""Sundered Soul of Atlas"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""atlasfalls"", ""Sundered Soul"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Yami",
    @"
case ""Yami"":
                    YnR.Yami(quant);
                    break;
    "
},
{
    "Folded Steel",
    @"
case ""Folded Steel"":
                    YnR.FoldedSteel();
                    break;
    "
},
{
    "Platinum Paragon Medal",
    @"
case ""Platinum Paragon Medal"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""underworld"", 238, ""Platinum Paragon Medal"", quant);
                    break;
    "
},
{
    "Dark Victory Seal",
    @"
case ""Dark Victory Seal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8576);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Story.KillQuest(8576, ""dagerecruit"", new[] { ""Dark Makai"", ""Dreadfiend"", ""Bloodfiend"", ""Infernal Fiend"" });
                        Core.HuntMonster(""dagerecruit"", ""Dark Makai"", ""Dark Makai Defeated"", 6);
                        Core.HuntMonster(""dagerecruit"", ""Dreadfiend"", ""Dreadfiend Defeated"", 6);
                        Core.HuntMonster(""dagerecruit"", ""Bloodfiend"", ""Bloodfiend Defeated"", 6);
                        Core.HuntMonster(""dagerecruit"", ""Infernal Fiend"", ""Infernal Fiend Defeated"", 6);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Underworld Asgardian Helm",
    @"
case ""Underworld Asgardian Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dagerecruit"", ""Hebimaru"", req.Name, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Underworld Asgardian Cape",
    @"
case ""Underworld Asgardian Cape"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dagerecruit"", ""Hebimaru"", req.Name, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Underworld Asgardian Sword",
    @"
case ""Underworld Asgardian Sword"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dagerecruit"", ""Hebimaru"", req.Name, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Underworld DeathSpine",
    @"
case ""Underworld DeathSpine"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dagerecruit"", ""Hebimaru"", req.Name, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Underworld Asgardian Mace",
    @"
case ""Underworld Asgardian Mace"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dagerecruit"", ""Hebimaru"", req.Name, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Underworld Oni's Naginata",
    @"
case ""Underworld Oni's Naginata"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dagerecruit"", ""Nuckelavee"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Underworld Oni's Blade",
    @"
case ""Underworld Oni's Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dagerecruit"", ""Nuckelavee"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Underworld Oni's Blades",
    @"
case ""Underworld Oni's Blades"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dagerecruit"", ""Nuckelavee"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Dage's Approval",
    @"
case ""Dage's Approval"":
                    Legion.ApprovalAndFavor(quant, 0);
                    break;

    "
},
{
    "Beast Soul",
    @"
case ""Beast Soul"":
                    if (Core.CheckInventory(req.Name, quant))
                        break;

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Adv.SmartEnhance(Core.SoloClass);
                    Core.HuntMonster(""sevencircleswar"", ""The Beast"", req.Name, quant, isTemp: false, publicRoom: true);
                    break;
    "
},
{
    "Souls of Heresy",
    @"
case ""Souls of Heresy"":
                    HeadoftheLegionBeast.SoulsHeresy(quant);
                    break;
    "
},
{
    "Penance",
    @"
case ""Penance"":
                    HeadoftheLegionBeast.Penance(quant);
                    break;
    "
},
{
    "Essence of Treachery",
    @"
case ""Essence of Treachery"":
                    HeadoftheLegionBeast.EssenceTreachery(quant);
                    break;
    "
},
{
    "Essence of Wrath",
    @"
case ""Essence of Wrath"":
                    HeadoftheLegionBeast.EssenceWrath(quant);
                    break;
    "
},
{
    "Essence of Violence",
    @"
case ""Essence of Violence"":
                    HeadoftheLegionBeast.EssenceViolence(quant);
                    break;

                //these come from circles not war:
    "
},
{
    "Stare of Greed",
    @"
case ""Stare of Greed"":
                    SevenCirclesMerge.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Gluttony's Maw",
    @"
case ""Gluttony's Maw"":
                    SevenCirclesMerge.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Aspect of Luxuria",
    @"
case ""Aspect of Luxuria"":
                    SevenCirclesMerge.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Helms of the Seven Circles",
    @"
case ""Helms of the Seven Circles"":
                    HeadoftheLegionBeast.HelmSevenCircles();
                    break;

    "
},
{
    "Underworld Laurel",
    @"
case ""Underworld Laurel"":
                    while (!Bot.ShouldExit && !Core.CheckInventory(""Underworld Laurel"", quant))
                    {
                        Core.EnsureAccept(8544);
                        Core.HuntMonster(""Dage"", ""Dage the Evil"", ""Dage Dueled"");
                        Core.EnsureComplete(8544);
                        Bot.Wait.ForPickup(""Underworld Laurel"");
                    }
                    break;
    "
},
{
    "Underworld Medal",
    @"
case ""Underworld Medal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(""Underworld Medal"", quant))
                    {
                        Core.EnsureAccept(8545);
                        Legion.ApprovalAndFavor(0, 200);
                        Legion.ObsidianRock(10);
                        HOTLB.SoulsHeresy(30);
                        Core.EnsureComplete(8545);
                        Bot.Wait.ForPickup(""Underworld Medal"");
                    }
                    break;
    "
},
{
    "Underworld Accolade",
    @"
case ""Underworld Accolade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(8546);
                        Core.HuntMonster(""legionarena"", ""legion fiend rider"", ""Fiend Rider's Approval"");
                        Core.HuntMonster(""frozenlair"", ""lich lord"", ""Lich Lord's Approval"");
                        Core.HuntMonster(""dagefortress"", ""Grrrberus"", ""Grrrberus's Grr Grrr"");
                        Core.EnsureComplete(8546);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Avarice of the Legion's Hood",
    @"
case ""Avarice of the Legion's Hood"":
                    Core.AddDrop(""Avarice of the Legion's Hood"", ""Avarice of the Legion's Skull"");
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dage"", ""Dage the Evil"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Avarice of the Legion's Skull",
    @"
case ""Avarice of the Legion's Skull"":
                    Core.AddDrop(""Avarice of the Legion's Hood"", ""Avarice of the Legion's Skull"");
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dage"", ""Dage the Evil"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Dage the Evil Insignia",
    @"
case ""Dage the Evil Insignia"":
                    if (!Core.CheckInventory(req.Name, quant))
                        Core.Logger($""Player does not have required amount of insignias [x{quant}]"", stopBot: true);
                    Core.Logger($""Insignias [x{quant}] found, continuing"");
                    break;

    "
},
{
    "Frosted Falchion",
    @"
case ""Frosted Falchion"":
                    Adv.BuyItem(""BlindingSnow"", 236, req.Name);
                    break;
    "
},
{
    "Judgement Scythe",
    @"
case ""Judgement Scythe"":
                    LegionExercise4.Exercise(new[] { ""Judgement Scythe"", ""Legion Token"" });
                    break;
    "
},
{
    "Judgement Hammer",
    @"
case ""Judgement Hammer"":
                    LegionExercise3.Exercise(new[] { ""Judgement Hammer"", ""Legion Token"" });
                    break;
    "
},
{
    "Cursed Scimitar",
    @"
case ""Cursed Scimitar"":
                    Adv.BuyItem(""SandSea"", 242, req.Name);
                    break;
    "
},
{
    "Essence of the Undead Legend",
    @"
case ""Essence of the Undead Legend"":
                    if (!Core.isSeasonalMapActive(""DarkBirthday""))
                    {
                        Core.Logger($""{req.Name} Is seasonal item from Dage's Dark Birthday Shop, failing over to next item"");
                        return;
                    }

                    Core.Logger(""\""DarkBirthday\"" is a seasonal map available during dage's birthday"");
                    Adv.BuyItem(""DarkBirthday"", 376, req.Name);
                    break;
    "
},
{
    "Shadow Shroud",
    @"
case ""Shadow Shroud"":
                    Daily.ShadowShroud();
                    if (!Core.CheckInventory(req.Name, quant))
                        Core.Logger($""Not enough \""Shadow Shroud\"", please do the daily {15 - Bot.Inventory.GetQuantity(""Shadow Shroud"")} more times (not today)"", messageBox: true);
                    break;
    "
},
{
    "DragonBlade of Nulgath",
    @"
case ""DragonBlade of Nulgath"":
                    DBoN.GetDragonBlade();
                    break;
    "
},
{
    "Fallen MonsterHunter",
    @"
case ""Fallen MonsterHunter"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""DeepForest"", ""Aberrant Horror"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Fallen MonsterHunter Helm",
    @"
case ""Fallen MonsterHunter Helm"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""DeepForest"", ""Aberrant Horror"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Fallen MonsterHunter Cape",
    @"
case ""Fallen MonsterHunter Cape"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""DeepForest"", ""Aberrant Horror"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Fallen MonsterHunter Sword",
    @"
case ""Fallen MonsterHunter Sword"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""DeepForest"", ""Aberrant Horror"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Exalted Crown",
    @"
case ""Exalted Crown"":
                    LR.ExaltedCrown();
                    break;
    "
},
{
    "Death's Requiem Staff",
    @"
case ""Death's Requiem Staff"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""cocytusbarracks"", ""Maleagant"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Staff of Inversion",
    @"
case ""Staff of Inversion"":
                    Core.KillEscherion(req.Name, isTemp: req.Temp);
                    break;
    "
},
{
    "BattleMage Armor",
    @"
case ""BattleMage Armor"":
                    Adv.BuyItem(""castleroof"", 749, req.Name, 1, shopItemID: 12773);
                    break;
    "
},
{
    "Nightlocke War Staff",
    @"
case ""Nightlocke War Staff"":
                    Core.EquipClass(ClassType.Solo);
                    Core.KillMonster(""aqw3d"", ""r13"", ""Left"", ""Nightlocke Staff"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Calamitous Warlic's Tome",
    @"
case ""Calamitous Warlic's Tome"":
                    Core.KillMonster(""ruinedcrown"", ""r10"", ""Left"", ""Calamitous Warlic"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Dishpan Cleric Costume",
    @"
case ""Dishpan Cleric Costume"":
                    Core.KillMonster(""cleric"", ""Frame3"", ""Left"", ""Chaos Dragon"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Chaotic Healer",
    @"
case ""Chaotic Healer"":
                    Core.FarmingLogger(req.Name, quant);
                    LOC.Hero();
                    Adv.BuyItem(""newfinale"", 891, req.Name);
                    break;
    "
},
{
    "Battle Cleric of the Dragon",
    @"
case ""Battle Cleric of the Dragon"":
                    TerminaTempleMerge.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Amia's Cult Secret",
    @"
case ""Amia's Cult Secret"":
                    Core.KillMonster(""fotia"", ""r6"", ""Left"", ""Amia the Cult Leader"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Queen's Sage Scythe",
    @"
case ""Queen's Sage Scythe"":
                    GooseMerge.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "ShadowFlame Empress",
    @"
case ""ShadowFlame Empress"":
                    BrightForestMerge.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Fragment of the Queen",
    @"
case ""Fragment of the Queen"":
                    Core.EquipClass(ClassType.Solo);
                    Bot.Quests.UpdateQuest(8094);
                    Core.HuntMonster(""transformation"", ""Queen of Monsters"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Empress' Finger's Ring",
    @"
case ""Empress' Finger's Ring"":
                    Core.HuntMonsterMapID(""firstobservatory"", 13, req.Name, quant, req.Temp);
                    break;
    "
},
{
    "King Klunk's Crown",
    @"
case ""King Klunk's Crown"":
                    Core.HuntMonster(""evilwarnul"", ""Laken"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Crowned Skull of Na'al",
    @"
case ""Crowned Skull of Na'al"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.DodgeClass();
                    Core.Logger($""Doing story first: {!Core.isCompletedBefore(9373)}"");
                    if (!Core.isCompletedBefore(9377))
                    {
                        Core.Logger(""Boss: [Na'al]\n this may take an hr or 2... or u may first try it so good luck (a kill has been gotten with VHL) so its confirmd able to be done...)"");
                        InfernalArena.DoStory();
                    }
                    Adv.BuyItem(Bot.Map.Name, 2336, req.Name, quant);
                    break;
    "
},
{
    "Zealous Crown",
    @"
case ""Zealous Crown"":
                    DLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Judgment Tonic",
    @"
case ""Judgment Tonic"":
                    PotionBuyer.// Call the method with specific parameters to farm Judgment Tonics with a quantity of 50
                    INeedYourStrongestPotions(new[] { ""Judgment Tonic"" }, new bool[] { true }, quant, true, true);
                    break;
    "
},
{
    "Lich Emperor's Catalyst",
    @"
case ""Lich Emperor's Catalyst"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""warundead"", ""Lich Emperor"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Blessed Abezeth",
    @"
case ""Blessed Abezeth"":
                    CCM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Inquisitor of the Light",
    @"
case ""Inquisitor of the Light"":
                    SOLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Divine Guardian Of Aegis",
    @"
case ""Divine Guardian Of Aegis"":
                    AOTM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Chaos Weaver Cleric's Doctrine",
    @"
case ""Chaos Weaver Cleric's Doctrine"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""chaosweb"", ""ChaosWeaver Cleric"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Love Token",
    @"
case ""Love Token"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""battlewedding"", ""Platinum Mech Dragon"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Time Heart",
    @"
case ""Time Heart"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""portalmazec"", ""Vorefax "", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Storm Heart",
    @"
case ""Storm Heart"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""pride"", ""Valsarian"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Mercutio's Heart",
    @"
case ""Mercutio's Heart"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""mercutio"", ""Mercutio"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Racing Trophy",
    @"
case ""Racing Trophy"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(""Racing Trophy"");
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.ChainComplete(746);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Sphinx Sentinel",
    @"
case ""Sphinx Sentinel"":
                    CM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dread Deadmoor BattleAxe",
    @"
case ""Dread Deadmoor BattleAxe"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""deadmoor"", ""Lucid Nightmare"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "SMU Brutalcorn's Horn",
    @"
case ""SMU Brutalcorn's Horn"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ultrabrutalcorn"", ""SMU BrutalCorn"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Ouroboros Scale",
    @"
case ""Ouroboros Scale"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9443);
                    Core.Logger(""Good luck with this \""ultra\""! --the maw"");
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""camlan"", ""Bellona"", ""Bellona's Edict of War"", log: false);
                        Core.HuntMonster(""camlan"", ""Sleih"", ""Sleih's Changeling Records"", log: false);
                        Core.HuntMonster(""camlan"", ""Metamorphosis Maw"", ""Alchemic Snake Scale"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Libran Scales",
    @"
case ""Libran Scales"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""lightoviacave"", ""Imbalanced Mage"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Akriloth's Scale",
    @"
case ""Akriloth's Scale"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""shadowstrike"", ""Sepulchuroth"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "ArchFiend DragonKnight's Scale",
    @"
case ""ArchFiend DragonKnight's Scale"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""underlair"", ""ArchFiend DragonKnight"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "ARTX 3090 Controller",
    @"
case ""ARTX 3090 Controller"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""mverse"", ""Major Mushroom"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Soulseeker's Grim Hood",
    @"
case ""Soulseeker's Grim Hood"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""marsh2"", ""Soulseeker"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Nothing's Solus",
    @"
case ""Nothing's Solus"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""pocketdimension"", ""Nothing"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Lucky Pet",
    @"
case ""Lucky Pet"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""pilgrimage"", ""Lucky"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Second Chance Coin",
    @"
case ""Second Chance Coin"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7781);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Adv.BuyItem(""onsen"", 1926, ""Gachapon Coin"", Log: false);
                        Core.HuntMonster(""yokaigrave"", ""Skello Kitty"", ""Skello Kitty Bone"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Treasure Chest",
    @"
case ""Treasure Chest"":
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""finalbattle"", ""r2"", ""Left"", ""*"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Divine Elixir",
    @"
case ""Divine Elixir"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""poisonforest"", ""Xavier Lionfang"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Ultra Lobthulu's Fortune",
    @"
case ""Ultra Lobthulu's Fortune"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ultralob"", ""Ultra Lobthulhu"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Enchanted Martial Artist's Gi",
    @"
case ""Enchanted Martial Artist's Gi"":
                    SSM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Strong Axe of Golmoth",
    @"
case ""Strong Axe of Golmoth"":
                    DPM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Fortitude Tonic",
    @"
case ""Fortitude Tonic"":
                    PotionBuyer.// Call the method with specific parameters to farm Fortitude Tonics with a quantity of 50
                    INeedYourStrongestPotions(new[] { ""Fortitude Tonic"" }, new bool[] { true }, quant, true, true);
                    break;
    "
},
{
    "Strong Drag's Intact Wing",
    @"
case ""Strong Drag's Intact Wing"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dracocon"", ""Strong Drag"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Chained Rune Bonebreaker",
    @"
case ""Chained Rune Bonebreaker"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""archportal"", ""High Legion Inquisitor"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Chained Rune Bonebreakers",
    @"
case ""Chained Rune Bonebreakers"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""archportal"", ""High Legion Inquisitor"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Noble Sacrifice",
    @"
case ""Noble Sacrifice"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""necrodungeon"", 48, req.Name, quant, req.Temp);
                    break;
    "
},
{
    "The Answer",
    @"
case ""The Answer"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""battlefowl"", ""Zeuster Projection"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Astero's Insight",
    @"
case ""Astero's Insight"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""fortressdelve"", ""Astero"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Death Pit Arena Medal",
    @"
case ""Death Pit Arena Medal"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""deathpit"", ""Training Dummy"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Enchanted DeathKnight",
    @"
case ""Enchanted DeathKnight"":
                    BCM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Super Death's Scythe Fragment",
    @"
case ""Super Death's Scythe Fragment"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""superdeath"", ""Super Death"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Celestial Wings of Guiding",
    @"
case ""Celestial Wings of Guiding"":
                    CRM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Blessed Coffee Cup",
    @"
case ""Blessed Coffee Cup"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5405);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""sandsea"", ""Oasis Monkey"", ""Pally Luwak Beans"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Northern Crown",
    @"
case ""Northern Crown"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""snowmore"", ""Jon S'NOOOOOOO"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Azkorath's Wing",
    @"
case ""Azkorath's Wing"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""infernalspire"", ""Azkorath"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Fiendish Outlaw",
    @"
case ""Fiendish Outlaw"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""fiendpast"", ""Dage the Lich"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Fiendish Remains",
    @"
case ""Fiendish Remains"":
                    VR.Storyline();
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9532);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""voidrefuge"", ""Paladin Ascendant"", ""Sussurating Helm"", 3, log: false);
                        Core.HuntMonster(""voidrefuge"", ""Nation Outrider"", ""Scarred Coin"", 8, log: false);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""voidrefuge"", ""Carnage"", ""Carnage's Ichor"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Glass Horns",
    @"
case ""Glass Horns"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ashfallcamp"", ""Blackrawk"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Fiend Champion's Spike",
    @"
case ""Fiend Champion's Spike"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""originul"", ""Fiend Champion"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Earth Stone",
    @"
case ""Earth Stone"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(3317);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""fableforest"", ""Earth Elemental"", ""Earth Aura"", 5, log: false);
                        Core.HuntMonster(""fableforest"", ""Undead Satyr"", ""Satyr Hoof"", 5, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Dragon Runestone",
    @"
case ""Dragon Runestone"":
                    // Core.FarmingLogger(req.Name, quant);
                    // Adv.BuyItem(""alchemyacademy"", 395, 62749, quant, 1, 8777);
                    // Core.BuyItem(""alchemyacademy"", 395, ""Dragon Runestone"", quant, 8844);
                    Farm.DragonRunestone(quant);
                    break;
    "
},
{
    "Arcangrove Tower House",
    @"
case ""Arcangrove Tower House"":
                    TLWHM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Nevanna's Revelation",
    @"
case ""Nevanna's Revelation"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""gaiazor"", ""Nevanna"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Star Scrap",
    @"
case ""Star Scrap"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""starsinc"", ""Star Sprites"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Rising Star Token",
    @"
case ""Rising Star Token"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""herolobby"", ""Training Partner"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Dark Stars",
    @"
case ""Dark Stars"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""deadlines"", ""Eternal Dragon"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Star Sapphire Fragment",
    @"
case ""Star Sapphire Fragment"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""skytower"", ""Star Sapphire"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Moon Rock Fragments",
    @"
case ""Moon Rock Fragments"":
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""lunacove"", ""r2"", ""Right"", ""*"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Blood Moon Warrior",
    @"
case ""Blood Moon Warrior"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""marchosiasfight"", ""Marchosias"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Celestial Khopesh",
    @"
case ""Celestial Khopesh"":
                    MoonlightKhopeshMerge.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "The Moon's Reflection",
    @"
case ""The Moon's Reflection"":
                    Bot.Quests.UpdateQuest(8000);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""Astravia"", ""The Moon"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Golden Sun Seal",
    @"
case ""Golden Sun Seal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titanattack"", ""Titanic Paladin"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Sun Zone Chit",
    @"
case ""Sun Zone Chit"":
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
                        Core.HuntMonster(""sunlightzone"", ""Marine Snow"", ""Marine Sample"", marineQty);

                        // Equip farming class for the rest
                        Core.EquipClass(ClassType.Farm);
                        Core.KillMonster(""sunlightzone"", ""r9"", ""left"", ""*"", ""seraphic sample"", seraphicQty);
                        Core.KillMonster(""sunlightzone"", ""r8"", ""left"", ""*"", ""infernal sample"", infernalQty);
                        Core.EnsureCompleteMulti(9252);
                        Bot.Wait.ForPickup(req.Name);

                        // Recalculate remaining quantity after each turn-in
                        remainingQuant = quant - Bot.Inventory.GetQuantity(req.Name);
                    }
                    break;
    "
},
{
    "Armor of the Sun",
    @"
case ""Armor of the Sun"":
                    ThirdspellMerge.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "The Sun's Enlightenment",
    @"
case ""The Sun's Enlightenment"":
                    Bot.Quests.UpdateQuest(8256);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""astraviacastle"", ""The Sun"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Trumpet",
    @"
case ""Trumpet"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""astraviajudge"", ""Trumpeter"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Enchanted Lance of Doom Reborn",
    @"
case ""Enchanted Lance of Doom Reborn"":
                    ShadowMerge.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Minos' Sentence",
    @"
case ""Minos' Sentence"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""judgement"", ""Minos"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Geode of Oblivion",
    @"
case ""Geode of Oblivion"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(10033);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.KillMonster(""deleuzetundra"", ""r4"", ""Left"", ""Oblivion Magus"", ""Honeycomb Flesh"", 8);
                        Core.KillMonster(""deleuzetundra"", ""Enter"", ""Spawn"", ""Empty Creature"", ""Empty Carcass"", 8);
                        Core.EquipClass(ClassType.Solo);
                        Core.KillMonster(""deleuzetundra"", ""r5"", ""Left"", ""Oblivion's Herald"", ""Obsidian Bone Shard"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Outrider's Broken Blade",
    @"
case ""Outrider's Broken Blade"":
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""deleuzetundra"", ""r2"", ""Left"", ""Nation Outrider"", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Death Badge",
    @"
case ""Death Badge"":
                    Core.EquipClass(ClassType.Solo);
                    if (Core.isCompletedBefore(793))
                    {
                        Core.RegisterQuests(6742);
                        Core.AddDrop(""Bone Sigil"");
                    }
                    Core.HuntMonster(""legionarena"", ""Legion Fiend Rider"", req.Name, quant, false);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Bone Sigil",
    @"
case ""Bone Sigil"":
                    Core.FarmingLogger(req.Name, quant);
                    if (Core.isCompletedBefore(793))
                    {
                        Core.Logger(""Legion Fiend Rider - Bone Sigil"");
                        Core.EquipClass(ClassType.Solo);
                        Core.RegisterQuests(6742);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""legionarena"", ""Legion Fiend Rider"", ""Undead Rider Defeated"");
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    else
                    {
                        Core.Logger(""Legions Finest - Bone Sigil"");
                        Core.EquipClass(ClassType.Farm);
                        Core.RegisterQuests(6741);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.KillMonster(""legionarena"", ""r4"", ""Left"", ""*"", ""Legion's Finest Defeated"", 8);
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Essence of Blade Master",
    @"
case ""Essence of Blade Master"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""underworld"", ""Blade Master"", ""Essence of Blade Master"", quant, false);
                    break;
    "
},
{
    "Primarch's Trophy",
    @"
case ""Primarch's Trophy"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(10052, ""bosschallenge"", ""Colossal Primarch"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Diamond of Nulgath",
    @"
case ""Diamond of Nulgath"":
                    Nation.FarmDiamondofNulgath(quant);
                    break;
    "
},
{
    "Archfiend's Favor",
    @"
case ""Archfiend's Favor"":
                    Nation.ApprovalAndFavor(0, quant);
                    break;
    "
},
{
    "Tainted Gem",
    @"
case ""Tainted Gem"":
                    Nation.FarmTaintedGem(quant);
                    break;
    "
},
{
    "Dark Crystal Shard",
    @"
case ""Dark Crystal Shard"":
                    Nation.FarmDarkCrystalShard(quant);
                    break;
    "
},
{
    "Totem of Nulgath",
    @"
case ""Totem of Nulgath"":
                    Nation.FarmTotemofNulgath(quant);
                    break;
    "
},
{
    "Gem of Nulgath",
    @"
case ""Gem of Nulgath"":
                    Nation.FarmGemofNulgath(quant);
                    break;
    "
},
{
    "Blood Gem of the Archfiend",
    @"
case ""Blood Gem of the Archfiend"":
                    Nation.FarmBloodGem(quant);
                    break;

    "
},
{
    "Abyssal Lore Scrap",
    @"
case ""Abyssal Lore Scrap"":
                    Core.FarmingLogger(req.Name, quant);

                    if (Core.CheckInventory(""Great Thief""))
                        Bot.Skills.StartAdvanced(""Great Thief"", true, ClassUseMode.Def);
                    else if (Core.CheckInventory(""Yami no Ronin""))
                        Bot.Skills.StartAdvanced(""Yami no Ronin)"", true, ClassUseMode.Def);
                    else Core.EquipClass(ClassType.Solo);

                    Core.RegisterQuests(8475);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //Room for Improvement 8475
                        Core.KillMonster(""Tercessuinotlim"", ""Boss2"", ""Right"", ""Nulgath"", ""Archfiend Analysis"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "ArchFiend Mage's Wand",
    @"
case ""ArchFiend Mage's Wand"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""Tercessuinotlim"", ""Evil Elemental"", req.Name, isTemp: false);
                    break;

    "
},
{
    "ArchFiend Mage's Tome",
    @"
case ""ArchFiend Mage's Tome"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""Tercessuinotlim"", ""Evil Elemental"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Shard of the Shard",
    @"
case ""Shard of the Shard"":
                    Fiendshard.Fiendshard_QuestlineP1();
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    //De-shard the Shard 7901
                    Core.RegisterQuests(7901);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    Core.KillMonster(""fiendshard"", ""r9"", ""Left"", 15, ""Nulgath's Shard Shaving"", 3);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Void General Surveillance",
    @"
case ""Void General Surveillance"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""Fiendshard"", ""Dirtlicker"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Diamond Badge of Nulgath",
    @"
case ""Diamond Badge of Nulgath"":
                    NLR.FarmQuest(new string[] { req.Name }, quant);
                    break;
    "
},
{
    "Emblem of Nulgath",
    @"
case ""Emblem of Nulgath"":
                    Nation.EmblemofNulgath(quant);
                    break;
    "
},
{
    "Soul Sand",
    @"
case ""Soul Sand"":
                    SSand.SoulSand(quant);
                    break;
    "
},
{
    "Soul Essence",
    @"
case ""Soul Essence"":
                    LetItBurn.SoulEssence(quant);
                    break;
    "
},
{
    "Legion Undead Visor",
    @"
case ""Legion Undead Visor"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7992);
                        Core.HuntMonster(""dagefortress"", ""Grrrberus"", ""Grrberus' Flame"");
                        SSand.SoulSand(3);
                        Core.EnsureCompleteChoose(7992, new[] { req.Name });
                    }
                    break;

    "
},
{
    "Empowered Voidstone",
    @"
case ""Empowered Voidstone"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7277);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""wanders"", ""r2"", ""Down"", ""Kalestri Worshiper"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Ice Diamond",
    @"
case ""Ice Diamond"":
                Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7279);
                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(""Ice Diamond"");
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7279);
                        Core.KillMonster(""kingcoal"", ""r1"", ""Left"", ""*"", ""Frozen Coal"", 10, log: false);
                        Core.EnsureComplete(7279);
                        Bot.Wait.ForPickup(""Ice Diamond"");
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Dark Bloodstone",
    @"
case ""Dark Bloodstone"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7281);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""safiria"", ""Blood Maggot"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Butterfly Sapphire",
    @"
case ""Butterfly Sapphire"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7287);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""bloodtusk"", ""Trollola Plant"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Understone",
    @"
case ""Understone"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7289);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""battleunderc"", ""Blue Crystalized Undead"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Rainbow Moonstone",
    @"
case ""Rainbow Moonstone"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7291);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""earthstorm"", ""Diamond Golem"", ""Chip of Diamond"", log: false);
                        Core.HuntMonster(""earthstorm"", ""Emerald Golem"", ""Chip of Emerald"", log: false);
                        Core.HuntMonster(""earthstorm"", ""Ruby Golem"", ""Chip of Ruby"", log: false);
                        Core.HuntMonster(""earthstorm"", ""Sapphire Golem"", ""Chip of Sapphire"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Nulgath's Approval",
    @"
case ""Nulgath's Approval"":
                    Nation.ApprovalAndFavor(quant, 0);
                    break;
    "
},
{
    "Unidentified 13",
    @"
case ""Unidentified 13"":
                    Nation.FarmUni13(quant);
                    break;
    "
},
{
    "Random Weapon of Nulgath",
    @"
case ""Random Weapon of Nulgath"":
                    Nation.Supplies(req.Name, quant);
                    break;
    "
},
{
    "Primal Dread Fang",
    @"
case ""Primal Dread Fang"":
                    Nation.Supplies(req.Name, quant);
                    break;
    "
},
{
    "Voucher of Nulgath (non-mem)",
    @"
case ""Voucher of Nulgath (non-mem)"":
                    Nation.FarmVoucher(false);
                    break;
    "
},
{
    "Unidentified 27",
    @"
case ""Unidentified 27"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(584);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Nation.Supplies(""Unidentified 26"");
                        Nation.FarmTaintedGem(5);

                        Core.ResetQuest(7551);
                        Core.DarkMakaiItem(""Dark Makai Sigil"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Taro's Manslayer",
    @"
case ""Taro's Manslayer"":
                    Taro.GuardianTaro(true);
                    break;
    "
},
{
    "Blade of Holy Might",
    @"
case ""Blade of Holy Might"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""northlands"", ""Aisha's Drake"", ""Blade of Holy Might"", isTemp: false);
                    break;
    "
},
{
    "Iron",
    @"
case ""Iron"":
                    BLOD.UnlockMineCrafting();
                    Dailies.MineCrafting(new[] { ""Iron"" }, quant);
                    break;
    "
},
{
    "Cloak of Nulgath",
    @"
case ""Cloak of Nulgath"":
                    Core.BuyItem(""tercessuinotlim"", 4667, ""Cloak of Nulgath"");
                    break;
    "
},
{
    "Staff of Imp Fire",
    @"
case ""Staff of Imp Fire"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""bludrut2"", ""Fire Elemental"", ""Staff of Imp Fire"", isTemp: false);
                    break;
    "
},
{
    "Cool Head",
    @"
case ""Cool Head"":
                    Core.BuyItem(""tercessuinotlim"", 4826, ""Cool Head"");
                    break;
    "
},
{
    "Crystal Phoenix Blade of Nulgath",
    @"
case ""Crystal Phoenix Blade of Nulgath"":
                    Core.FarmingLogger(req.Name, quant);
                    Nation.FarmDiamondofNulgath(13);
                    Nation.FarmDarkCrystalShard(50);
                    Nation.FarmTotemofNulgath(3);
                    Nation.FarmGemofNulgath(20);
                    Nation.FarmVoucher(false);
                    Nation.FarmTaintedGem(50);

                    Core.EnsureAccept(837);
                    Core.HuntMonster(""underworld"", ""Undead Bruiser"", ""Undead Bruiser Rune"");
                    Core.EnsureComplete(837, req.ID);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Gold Star of Avarice",
    @"
case ""Gold Star of Avarice"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""tercessuinotlim"", 1951, req.Name);
                    break;
    "
},
{
    "Void Remnant",
    @"
case ""Void Remnant"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.AddDrop(""Void Remnant"");
                        Core.EnsureAccept(9553);
                        Core.EquipClass(ClassType.Solo);
                        Core.KillMonster(""voidchasm"", ""r10"", ""Left"", ""Carcano"", ""Carcano's Teratoma"");
                        Core.KillMonster(""voidchasm"", ""r9"", ""Left"", ""Carnage"", ""Bloodied Chainlink"");
                        Core.EquipClass(ClassType.Farm);
                        Core.KillMonster(""voidchasm"", ""r7"", ""Left"", ""The Hushed"", ""Defunct Seal of Approval"", 6);
                        Core.EnsureComplete(9553);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Essence of Nulgath",
    @"
case ""Essence of Nulgath"":
                    Nation.EssenceofNulgath(quant);
                    break;

    "
},
{
    "Diamond Token of Gravelyn",
    @"
case ""Diamond Token of Gravelyn"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(4737);
                    Core.AddDrop(""Diamond Token of Dage"", ""Legion Token"");
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        if (!Core.CheckInventory(""Defeated Makai"", 25))
                        {
                            Core.EquipClass(ClassType.Farm);
                            Core.KillMonster(""tercessuinotlim"", ""m2"", ""Left"", ""*"", ""Defeated Makai"", 25, false);
                            Core.JumpWait();
                            Core.Join(""aqlesson"");
                        }
                        Core.EquipClass(ClassType.Solo);
                        Core.KillMonster(""aqlesson"", ""Frame9"", ""Right"", ""Carnax"", ""Carnax Eye"", publicRoom: true);
                        Core.HuntMonster(""deepchaos"", ""Kathool"", ""Kathool Tentacle"", publicRoom: true);

                        //More then one item of the same name as drop btoh temp and non-temp.
                        while (!Bot.ShouldExit && !Core.CheckInventory(33257))
                            Core.KillMonster(""dflesson"", ""r12"", ""Right"", ""Fluffy the Dracolich"", log: false, publicRoom: true);

                        Core.HuntMonster(""lair"", ""Red Dragon"", ""Red Dragon's Fang"");
                        Core.HuntMonster(""bloodtitan"", ""Blood Titan"", ""Blood Titan's Blade"", publicRoom: true);

                        Bot.Wait.ForQuestComplete(4737);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Emblem of Gravelyn",
    @"
case ""Emblem of Gravelyn"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(4750);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""shadowblast"", ""Carnage"", ""Shadow Seal"", isTemp: false);
                        Core.HuntMonster(""shadowblast"", ""Legion Fenrir"", ""Gem of Superiority"", isTemp: false);
                        Bot.Wait.ForQuestComplete(4750);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Bone Pick",
    @"
case ""Bone Pick"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(9327);
                        Core.HuntMonster(""brainmeat"", ""Brain Matter"", ""Brain Matter"", log: false);
                        Core.EnsureComplete(9327);
                    }
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Cursed Fabric",
    @"
case ""Cursed Fabric"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""skullhall"", ""Necroupie"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Rotten Meat",
    @"
case ""Rotten Meat"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""brainmeat"", ""Brain Matter"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Metanoia Shaggy Locks",
    @"
case ""Metanoia Shaggy Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""skullarena"", ""Bellum"", req.Name, quant, false, false);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Metanoia Shag",
    @"
case ""Metanoia Shag"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""skullarena"", ""Bellum"", req.Name, quant, false, false);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Null Contract",
    @"
case ""Null Contract"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(10046);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(10046, new[] {
                            (""obliviontundra"", UseableMonsters[1], ClassType.Farm),
                            (""obliviontundra"", UseableMonsters[4], ClassType.Farm),
                            (""obliviontundra"", UseableMonsters[5], ClassType.Solo)
                        });
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Withered Archfiend's Essence",
    @"
case ""Withered Archfiend's Essence"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""obliviontundra"", UseableMonsters[2], req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "ArchFiend Healer Staff",
    @"
case ""ArchFiend Healer Staff"":
                    FLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "ArchFiend Healer Hood",
    @"
case ""ArchFiend Healer Hood"":
                    FLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "ArchFiend Rogue Knives",
    @"
case ""ArchFiend Rogue Knives"":
                    FLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "ArchFiend Rogue Knife",
    @"
case ""ArchFiend Rogue Knife"":
                    FLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "ArchFiend Rogue Backwards Knives",
    @"
case ""ArchFiend Rogue Backwards Knives"":
                    FLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "ArchFiend Rogue Backwards Knife",
    @"
case ""ArchFiend Rogue Backwards Knife"":
                    FLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "ArchFiend Mage Book + Wand",
    @"
case ""ArchFiend Mage Book + Wand"":
                    FLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "ArchFiend Mage Hood",
    @"
case ""ArchFiend Mage Hood"":
                    FLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "ArchFiend Mage Hat",
    @"
case ""ArchFiend Mage Hat"":
                    FLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual ArchFiend Warrior Champion Swords",
    @"
case ""Dual ArchFiend Warrior Champion Swords"":
                    FLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "ArchFiend Warrior Champion Sword",
    @"
case ""ArchFiend Warrior Champion Sword"":
                    FLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual ArchFiend Warrior Swords",
    @"
case ""Dual ArchFiend Warrior Swords"":
                    FLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "ArchFiend Warrior Helm",
    @"
case ""ArchFiend Warrior Helm"":
                    FLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "ArchFiend Warrior Armet",
    @"
case ""ArchFiend Warrior Armet"":
                    FLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "ArchFiend Healer",
    @"
case ""ArchFiend Healer"":
                    FLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "ArchFiend Rogue",
    @"
case ""ArchFiend Rogue"":
                    FLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "ArchFiend Mage",
    @"
case ""ArchFiend Mage"":
                    FLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "ArchFiend Warrior",
    @"
case ""ArchFiend Warrior"":
                    FLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Anti-Matter Gem",
    @"
case ""Anti-Matter Gem"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5188); // Reflections of Victory 5188
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""whitehole"", ""Dimensional Crystal"", ""Crystal Shards"", 5);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Undine Base Scrip",
    @"
case ""Undine Base Scrip"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""abyssalzone"", ""Kitefin Shark Bait"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Water Elf Antler",
    @"
case ""Water Elf Antler"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9316);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""abyssalzone"", ""The Ashray"", ""Ashray Artifacts"", log: false);
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""abyssalzone"", ""Necro Adipocere"", ""Adipocere Antler"", 3, log: false);
                        Core.HuntMonster(""abyssalzone"", ""Foam Scavenger"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Waves of Tumult",
    @"
case ""Waves of Tumult"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""abyssalzone"", ""Blighted Water"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Nulgath Nation House",
    @"
case ""Nulgath Nation House"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.Name, ""Cemaros' Amethyst"", ""Aluminium"", ""NUE Necronomicon"");
                    Nation.FarmUni10(400);
                    Nation.FarmUni13(1);
                    Nation.FarmVoucher(false);
                    Nation.FarmDiamondofNulgath(300);
                    Nation.FarmDarkCrystalShard(250);
                    Nation.FarmTotemofNulgath(30);
                    Nation.FarmGemofNulgath(150);
                    Nation.FarmTaintedGem(200);
                    Nation.FarmBloodGem(100);
                    Nation.ApprovalAndFavor(1000, 0);

                    Farm.ChaosREP(2);
                    Core.BuyItem(""mountdoomskull"", 776, ""Cemaros' Amethyst"");

                    BLOD.UnlockMineCrafting();
                    Daily.MineCrafting(new[] { ""Aluminum"" });

                    Farm.DoomWoodREP();
                    Farm.Gold(999);
                    Core.BuyItem(""lightguard"", 277, ""NUE Necronomicon"");

                    Core.EnsureAccept(4779);
                    if (!Core.EnsureComplete(4779))
                    {
                        Core.Logger(""Could not complete the quest, stopping bot"", messageBox: true);
                        return;
                    }
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Pink Star Diamond of Nulgath",
    @"
case ""Pink Star Diamond of Nulgath"":
                    Adv.BuyItem(""tercessuinotlim"", 1951, ""Pink Star Diamond of Nulgath"");
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Musgravite of Nulgath",
    @"
case ""Musgravite of Nulgath"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""timelibrary"", ""Ancient Chest"", req.Name, quant, false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Voucher of Nulgath",
    @"
case ""Voucher of Nulgath"":
                    Nation.FarmVoucher(true, true);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Corpse Maker of Nulgath",
    @"
case ""Corpse Maker of Nulgath"":
                    Core.FarmingLogger(req.Name, quant);
                    TLC.QuestItems(TheLeeryContract.RewardsSelection.Corpse_Maker_of_Nulgath);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Overfiend Blade of Nulgath",
    @"
case ""Overfiend Blade of Nulgath"":
                    Core.FarmingLogger(req.Name, quant);
                    Jugger.JuggItems(JuggernautItemsofNulgath.RewardsSelection.Overfiend_Blade_of_Nulgath);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Hadean Onyx of Nulgath",
    @"
case ""Hadean Onyx of Nulgath"":
                    Core.HuntMonster(""tercessuinotlim"", ""Shadow of Nulgath"", req.Name, quant, false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Letter from Asuka and Tendou",
    @"
case ""Letter from Asuka and Tendou"":
                    Core.HuntMonster(""citadel"", ""Burning Witch"", req.Name, quant, false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Chain of Nulgath",
    @"
case ""Chain of Nulgath"":
                    Core.HuntMonster(""necrocavern"", ""Shadow Dragon"", req.Name, quant, false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Yulgath's Hut",
    @"
case ""Yulgath's Hut"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""originul"", ""Fiend Champion"", req.Name, quant, false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Unidentified 10",
    @"
case ""Unidentified 10"":
                    Nation.FarmUni10(quant);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Punadin Badge",
    @"
case ""Punadin Badge"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""pal9001"", ""Baby Sharkcaster"", ""Punadin Badge"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Angry Zombie Skull",
    @"
case ""Angry Zombie Skull"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""doomwar"", ""Angry Zombie"", ""Angry Zombie Skull"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Infernalis Penna",
    @"
case ""Infernalis Penna"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(9887,
(""champazalith"", ""Maah-na"", ClassType.Solo),
                            (""champazalith"", ""Akh-a"", ClassType.Solo)
);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Infernalis Oculus",
    @"
case ""Infernalis Oculus"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(9888,
(""champazalith"", ""Azalith"", ClassType.Solo)
);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Divina Voluntas",
    @"
case ""Divina Voluntas"":
                    Farm.Experience(80);
                    Core.Logger($""{req.Name} cannot be farmed solo, use army."");
                    break;

    "
},
{
    "Venomous Fang Blade",
    @"
case ""Venomous Fang Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.HuntMonster(""tercessuinotlim"", ""Ninja Spy"", ""Spy's Info"", isTemp: false, log: false);
                    Core.HuntMonster(""citadel"", ""Inquisitor Captain"", ""Captain's Info"", isTemp: false, log: false);
                    Core.HuntMonster(""lairattack"", ""Flame Dragon General"", ""Broken Fang Blade"", isTemp: false, log: false);
                    Core.GetMapItem(12571, map: ""museum"");
                    break;
    "
},
{
    "Unidentified 23",
    @"
case ""Unidentified 23"":
                    Core.FarmingLogger(req.Name, quant);
                    Nation.TheAssistant(""Unidentified 23"", quant);
                    break;
    "
},
{
    "Evolved Carnage of Nulgath",
    @"
case ""Evolved Carnage of Nulgath"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""voidrefuge"", ""Carnage"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Evolved Carnage Helm",
    @"
case ""Evolved Carnage Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""voidrefuge"", ""Carnage"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Evolved Carnage Crest",
    @"
case ""Evolved Carnage Crest"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""voidrefuge"", ""Carnage"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Blood Void Spines",
    @"
case ""Blood Void Spines"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""voidrefuge"", ""Carnage"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Blood Void Spikes",
    @"
case ""Blood Void Spikes"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""voidrefuge"", ""Carnage"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Bloodletter Katana",
    @"
case ""Bloodletter Katana"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""voidrefuge"", ""Carnage"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Bloodletter Katanas",
    @"
case ""Bloodletter Katanas"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""voidrefuge"", ""Carnage"", req.Name, quant, false, false);
                    break;
    "
},
{
    "1st Betrayal Blade of Nulgath",
    @"
case ""1st Betrayal Blade of Nulgath"":
                    Core.FarmingLogger(req.Name, quant);
                    Nation.KisstheVoid(0, req.Name);
                    break;
    "
},
{
    "2nd Betrayal Blade of Nulgath",
    @"
case ""2nd Betrayal Blade of Nulgath"":
                    Core.FarmingLogger(req.Name, quant);
                    Nation.KisstheVoid(0, req.Name);
                    break;
    "
},
{
    "3rd Betrayal Blade of Nulgath",
    @"
case ""3rd Betrayal Blade of Nulgath"":
                    Core.FarmingLogger(req.Name, quant);
                    Nation.KisstheVoid(0, req.Name);
                    break;
    "
},
{
    "4th Betrayal Blade of Nulgath",
    @"
case ""4th Betrayal Blade of Nulgath"":
                    Core.FarmingLogger(req.Name, quant);
                    Nation.KisstheVoid(0, req.Name);
                    break;
    "
},
{
    "5th Betrayal Blade of Nulgath",
    @"
case ""5th Betrayal Blade of Nulgath"":
                    Core.FarmingLogger(req.Name, quant);
                    Nation.KisstheVoid(0, req.Name);
                    break;
    "
},
{
    "6th Betrayal Blade of Nulgath",
    @"
case ""6th Betrayal Blade of Nulgath"":
                    Core.FarmingLogger(req.Name, quant);
                    Nation.KisstheVoid(0, req.Name);
                    break;
    "
},
{
    "7th Betrayal Blade of Nulgath",
    @"
case ""7th Betrayal Blade of Nulgath"":
                    Core.FarmingLogger(req.Name, quant);
                    Nation.KisstheVoid(0, req.Name);
                    break;
    "
},
{
    "8th Betrayal Blade of Nulgath",
    @"
case ""8th Betrayal Blade of Nulgath"":
                    Core.FarmingLogger(req.Name, quant);
                    Nation.KisstheVoid(0, req.Name);
                    break;
    "
},
{
    "Sterling Silver",
    @"
case ""Sterling Silver"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(9991, ""frozenbalemorale"", ""Kall Haxa"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Fabric",
    @"
case ""Fabric"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5898);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //QuarterMaster’s Supplies 5898
                        Core.HuntMonster(""ashfallcamp"", ""Lava Dragoblin"", ""Supply Chest"", 8, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Red Dye",
    @"
case ""Red Dye"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5898);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //QuarterMaster’s Supplies 5898
                        Core.HuntMonster(""ashfallcamp"", ""Lava Dragoblin"", ""Supply Chest"", 8, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Green Dye",
    @"
case ""Green Dye"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5898);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //QuarterMaster’s Supplies 5898
                        Core.HuntMonster(""ashfallcamp"", ""Lava Dragoblin"", ""Supply Chest"", 8, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Dragon Scales",
    @"
case ""Dragon Scales"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(5893);
                    while (!Bot.ShouldExit && !Core.CheckInventory(40375, quant))
                    {
                        //Blackrawk Magebane 5893
                        Core.HuntMonster(""ashfallcamp"", ""Blackrawk"", ""Blackrawk Defeated"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Defender Badge",
    @"
case ""Defender Badge"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ashfallcamp"", ""Blackrawk"", req.Name, quant, false);
                    break;
    "
},
{
    "Flame Claws",
    @"
case ""Flame Claws"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ashfallcamp"", ""Smoldur"", req.Name, quant, false);
                    break;
    "
},
{
    "Flame Heart",
    @"
case ""Flame Heart"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ashfallcamp"", ""Smoldur"", req.Name, quant, false);
                    break;
    "
},
{
    "Sulphur Ore",
    @"
case ""Sulphur Ore"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5899);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ashfallcamp"", ""Sulphur Dracolich"", ""Sulphur Crystal"", 10, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Iron Ore",
    @"
case ""Iron Ore"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    int i = 1;
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EnsureAccept(5900);
                        Core.HuntMonster(""ashfallcamp"", ""Draconian Guard"", ""Iron Lump"", 5, log: false);
                        Core.HuntMonster(""ashfallcamp"", ""Draconian Guard"", ""Bile Drops"", 3, log: false);
                        Core.EnsureComplete(5900, req.ID);
                        Core.Logger($""Quest completed x{i++} times: [5900] \""Ingots and Outguts\"""");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Bile Stone",
    @"
case ""Bile Stone"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    int i = 1;
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EnsureAccept(5900);
                        Core.HuntMonster(""ashfallcamp"", ""Draconian Guard"", ""Iron Lump"", 5, log: false);
                        Core.HuntMonster(""ashfallcamp"", ""Draconian Guard"", ""Bile Drops"", 3, log: false);
                        Core.EnsureComplete(5900, req.ID);
                        Core.Logger($""Quest completed x{i++} times: [5900] \""Ingots and Outguts\"""");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Molten Lava",
    @"
case ""Molten Lava"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5897);
                    while (!Bot.ShouldExit && !Core.CheckInventory(40352, quant))
                    {
                        Core.HuntMonster(""ashfallcamp"", ""Lava Rock"", ""Lava Glob"", 5, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Venom Sac",
    @"
case ""Venom Sac"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ashfallcamp"", ""Infernus"", req.Name, quant, false);
                    break;
    "
},
{
    "Venom Fangs",
    @"
case ""Venom Fangs"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ashfallcamp"", ""Infernus"", req.Name, quant, false);
                    break;
    "
},
{
    "Crystal Eye",
    @"
case ""Crystal Eye"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ashfallcamp"", ""Blackrawk"", req.Name, quant, false);
                    break;
    "
},
{
    "Melted Glass",
    @"
case ""Melted Glass"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""pride"", ""Cellar Guard"", req.Name, quant, false);
                    break;

    "
},
{
    "Copper Wire",
    @"
case ""Copper Wire"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""pride"", ""Cellar Guard"", req.Name, quant, false);
                    break;

    "
},
{
    "Lemon",
    @"
case ""Lemon"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(10054, new[] {
    (""extinction"",""Lard"",ClassType.Farm),
    (""extinction"",""Gelatinous Slime"",ClassType.Farm),
    (""extinction"",""SN.O.W."",ClassType.Solo),
});
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Building Material",
    @"
case ""Building Material"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(6915);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""farm"", ""Treeant"", ""Wooden Planks"", 5);
                        Core.HuntMonster(""bloodtusk"", ""Rhison"", ""Glue"");
                        Core.HuntMonster(""crashsite"", ""ProtoSartorium"", ""Nails"", 10);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Foundation Material",
    @"
case ""Foundation Material"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(6916);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""river"", ""Zardman Fisher"", ""River Stones"", 5);
                        Core.HuntMonster(""dwarfprison"", ""Balboa"", ""Boulder"", 3);
                        Core.HuntMonster(""dragonplane"", ""Earth Elemental"", ""Marble"");
                        Core.HuntMonster(""gilead"", ""Fire Elemental"", ""Flames"", 3);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Decor Material",
    @"
case ""Decor Material"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(6917);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""farm"", ""Scarecrow"", ""Fabric"", 5);
                        Core.HuntMonster(""undergroundlabb"", ""Window"", ""Glass"", 5);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""goose"", ""Can of Paint"", ""Paint"", 5);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Dragonrune Blueprint",
    @"
case ""Dragonrune Blueprint"":
                    Core.FarmingLogger(req.Name, quant);
                    Farm.ElementalMasterREP();
                    Core.BuyItem(""dragonrune"", 690, 48758);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Mana Golem's Core",
    @"
case ""Mana Golem's Core"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""elemental"", ""Mana Golem"", ""Mana Golem's Core"", isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Arcangrove Blueprint",
    @"
case ""Arcangrove Blueprint"":
                    Core.FarmingLogger(req.Name, quant);
                    Farm.ArcangroveREP();
                    Core.BuyItem(""arcangrove"", 214, 48759);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Falcontower Blueprint",
    @"
case ""Falcontower Blueprint"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""falconreach"", ""Dragon Drakath"", ""Falcontower Blueprint"", isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Citadel Caverns Blueprint",
    @"
case ""Citadel Caverns Blueprint"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);

                    Core.HuntMonster(""citadel"", ""Belrot the Fiend"", ""Citadel Caverns Blueprint"", isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Citadel Blueprint",
    @"
case ""Citadel Blueprint"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.BuyItem(""citadel"", 44, 48761, Log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Seraphic Blueprint",
    @"
case ""Seraphic Blueprint"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.BuyItem(""seraph"", 1133, 48762, Log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Hachiko Blueprint",
    @"
case ""Hachiko Blueprint"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.BuyItem(""dragonkoiz"", 95, 48763, Log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Clubhouse Blueprint",
    @"
case ""Clubhouse Blueprint"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""clubhouse"", ""Riddlelord's Golem"", ""Clubhouse Blueprint"", isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Rift Defense Medal",
    @"
case ""Rift Defense Medal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5825);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""charredpath"", ""Infected Hare"", ""Invader Slain"", 10);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "DeadMog LED",
    @"
case ""DeadMog LED"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(4576);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""arena"", ""Deadmoglinster"", ""DeadMoglinster Defeated"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Chaos Fuzzies",
    @"
case ""Chaos Fuzzies"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.GetMapItem(3481, 1, ""citadel"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Unidentified 36",
    @"
case ""Unidentified 36"":
                    Core.EquipClass(ClassType.Farm);
                    HB.FreshSouls(1, 0);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Fresh Soul",
    @"
case ""Fresh Soul"":
                    Core.EquipClass(ClassType.Farm);
                    HB.FreshSouls(0, quant);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Unidentified 34",
    @"
case ""Unidentified 34"":
                    Core.EquipClass(ClassType.Farm);
                    WPE.Unidentified34(quant);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Strand of Vath's Hair",
    @"
case ""Strand of Vath's Hair"":
                    Core.EquipClass(ClassType.Farm);
                    Core.KillVath(req.Name, quant);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Unidentified 25",
    @"
case ""Unidentified 25"":
                    Core.EquipClass(ClassType.Farm);
                    Adv.BuyItem(""tercessuinotlim"", 1951, ""Unidentified 25"");
                    break;

    "
},
{
    "4th Dimension Gem",
    @"
case ""4th Dimension Gem"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5163);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""blackholesun"", ""Black Light Elemental"", ""Black Light"", 5);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Ancient Vitae",
    @"
case ""Ancient Vitae"":
                    Safiria.StoryLine();
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(""Safiria's Blood Sample"");
                    Core.RegisterQuests(1947);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //Ancient Vitae 1947
                        Core.HuntMonster(""battledoom"", ""Shadow Safiria"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Sapphires",
    @"
case ""Sapphires"":
                    Core.RegisterQuests(6070, 6071, 6073);
                    Core.Logger($""Farming {req.Name} ({currentQuant}/{quant})"");
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""bloodwarlycan"", ""Blood Guardian"", ""Vampire Medal"", 5);
                        Core.HuntMonster(""bloodwarlycan"", ""Blood Guardian"", ""Mega Vampire Medal"", 3);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Rubies",
    @"
case ""Rubies"":
                    Core.RegisterQuests(6068, 6069, 6072);
                    Core.Logger($""Farming {req.Name} ({currentQuant}/{quant})"");
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""bloodwarvamp"", ""Lunar Blazebinder"", ""Lycan Medal"", 5);
                        Core.HuntMonster(""bloodwarvamp"", ""Lunar Blazebinder"", ""Mega Lycan Medal"", 3);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Blood Titan Token",
    @"
case ""Blood Titan Token"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(Core.EnsureLoad(9253).Requirements.Select(item => item.Name).Concat(Core.EnsureLoad(2908).Requirements.Select(item => item.Name)).ToArray());
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9253);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        if (!Core.CheckInventory(""Blood Titan's Tribute""))
                        {
                            Core.EnsureAccept(2908);
                            Core.HuntMonster(""bloodtitan"", ""Blood Titan"", ""Blood Titan's Phial"", 1, false, false);
                            Core.HuntMonster(""titandrakath"", ""Titan Drakath"", ""Titanic Drakath's Blood"", 1, false, false);
                            Core.HuntMonster(""desoloth"", ""Desoloth"", ""Desoloth's Blood"", 1, false, false);
                            Core.HuntMonster(""ultracarnax"", ""Ultra-Carnax"", ""Ultra Carnax's Blood"", 1, false, false);
                            Core.EnsureComplete(2908);
                            Bot.Wait.ForPickup(""Blood Titan's Tribute"");
                        }
                        Core.HuntMonster(""bloodtitan"", ""Ultra Blood Titan"", ""Ultra Blood Titan Defeated"", 1, false, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Lunate Sigil",
    @"
case ""Lunate Sigil"":
                    ssr2.LunateSigil(quant);
                    break;
    "
},
{
    "Darkovia Hunter's Cowl",
    @"
case ""Darkovia Hunter's Cowl"":
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.KillMonster(""badmoon"", ""r5"", ""left"", ""hunter"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Iron Dussack",
    @"
case ""Iron Dussack"":
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.KillMonster(""badmoon"", ""r5"", ""left"", ""hunter"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Darkovian Hunter",
    @"
case ""Darkovian Hunter"":
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""badmoon"", ""Twisted Hunter"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "ShadowSlayer's Apprentice",
    @"
case ""ShadowSlayer's Apprentice"":
                    SSKM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Antiquated Shadow Hair",
    @"
case ""Antiquated Shadow Hair"":
                    SSKM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Antiquated Shadow Locks",
    @"
case ""Antiquated Shadow Locks"":
                    SSKM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Antiquated Shadow Hat",
    @"
case ""Antiquated Shadow Hat"":
                    SSKM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Antiquated Shadow Hat + Locks",
    @"
case ""Antiquated Shadow Hat + Locks"":
                    SSKM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Zealous Claymore",
    @"
case ""Zealous Claymore"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""techdungeon"", ""Kalron the Cryptborg"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Zealous Censer",
    @"
case ""Zealous Censer"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""stonewood"", ""BioKnight"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Shadowslayer Relic Sword",
    @"
case ""Shadowslayer Relic Sword"":
                    Core.FarmingLogger(req.Name, quant);
                    SSR.GetAll(itemFarm: req.Name);
                    break;

    "
},
{
    "Diabolical Ectomancer",
    @"
case ""Diabolical Ectomancer"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7880);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""banished"", ""Desterrat Moya"", ""Infected Tentacle"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Fresh Ectoplasm",
    @"
case ""Fresh Ectoplasm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8009);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""vendorbooths"", ""Caffeine Imp"", ""Coffee Beans"", 10);
                        Core.HuntMonster(""djinn"", ""Lamia"", ""Tasty Poison"", 10);
                        Core.HuntMonster(""charredpath"", ""Toxic Wisteria"", ""Necessary Antidote"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "IOU Slip",
    @"
case ""IOU Slip"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8009);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""vendorbooths"", ""Caffeine Imp"", ""Coffee Beans"", 10);
                        Core.HuntMonster(""djinn"", ""Lamia"", ""Tasty Poison"", 10);
                        Core.HuntMonster(""charredpath"", ""Toxic Wisteria"", ""Necessary Antidote"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "EctoBlade",
    @"
case ""EctoBlade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""shadowfallwar"", ""Skeletal Fire Mage"", ""EctoBlade"", isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Ectoplasmic Chains",
    @"
case ""Ectoplasmic Chains"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8010);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""temple"", ""Doomwood Ectomancer"", ""Refined Ectoplasm"", 10);
                        Core.HuntMonster(""ectocave"", ""Ektorax"", ""Ektorax's Ectoplasm"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Bongo Cart Pet",
    @"
case ""Bongo Cart Pet"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(""Love Token"");
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(8013);
                        // Red Rose
                        if (!Core.CheckInventory(""Red Rose"", toInv: false))
                        {
                            while (!Bot.ShouldExit && !Core.CheckInventory(21567, 10))
                                Core.HuntMonster(""battlewedding"", ""Silver Knight"", isTemp: false);
                            Adv.BuyItem(""battlewedding"", 788, ""Red Rose"");
                        }
                        // Bangin' Bongo Cat Hair
                        if (!Core.CheckInventory(""Bangin' Bongo Cat Hair""))
                            Adv.BuyItem(""battleontown"", 907, ""Bangin' Bongo Cat Hair"");
                        // Pink Rose
                        if (!Core.CheckInventory(""Pink Rose""))
                        {
                            Core.RegisterQuests(1489);
                            while (!Bot.ShouldExit && !Core.CheckInventory(""Magenta Dye"", 35))
                            {
                                //Flowers for the Pink Gal 1489
                                Core.HuntMonster(""Sandsea"", ""Cactus Creeper"", ""Fandango Flower"", 5);
                                Core.KillMonster(""wanders"", ""r5"", ""Left"", ""Lotus Spider"", ""Lotus Flower"", 4);
                            }

                            Adv.BuyItem(""tower"", 347, ""Pink Rose"");
                            while (!Bot.ShouldExit && !Core.CheckInventory(21567, 10))
                                Core.HuntMonster(""battlewedding"", ""Silver Knight"", isTemp: false);
                            Adv.BuyItem(""battlewedding"", 788, ""Red Rose"");
                        }

                        // Stray Ectoplasm
                        if (!Core.CheckInventory(""Stray Ectoplasm""))
                        {
                            while (!Bot.ShouldExit && !Core.CheckInventory(""Fresh Ectoplasm"", 15))
                            {
                                Core.EnsureAccept(8009);
                                Core.HuntMonster(""vendorbooths"", ""Caffeine Imp"", ""Coffee Beans"", 10);
                                Core.HuntMonster(""djinn"", ""Lamia"", ""Tasty Poison"", 10);
                                Core.HuntMonster(""charredpath"", ""Toxic Wisteria"", ""Necessary Antidote"");
                                Core.EnsureComplete(8009);
                                Bot.Wait.ForPickup(""Fresh Ectoplasm"");
                            }
                            Adv.BuyItem(""battleunderb"", 1990, ""Stray Ectoplasm"");
                        }
                        Core.EnsureComplete(8013);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Reho's Golden Sword Hilt",
    @"
case ""Reho's Golden Sword Hilt"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8011);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""extinction"", ""Slimed Drone"", ""Iron II.0"", 4, isTemp: false);
                        Core.HuntMonster(""doomwood"", ""Doomwood Treeant"", ""Wood"", 10);
                        Core.HuntMonster(""crashsite"", ""Dwakel Blaster"", ""Big Iron Bolts"", 10);
                        Core.HuntMonster(""portalmaze"", ""Time Wraith"", ""Piece of Cake"", 5);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Awethur's Accoutrements",
    @"
case ""Awethur's Accoutrements"":
                    Adv.BuyItem(""museum"", 631, ""Awethur's Accoutrements"");
                    break;
    "
},
{
    "Bocklin Ornament",
    @"
case ""Bocklin Ornament"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    if (Core.IsMember)
                        Core.RegisterQuests(10255);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(10253,
                            (""bocklincastle"", ""Faceless Ritualist"", ClassType.Farm),
                            (""bocklincastle"", ""Headless Knight"", ClassType.Solo),
                        (""bocklincastle"", ""Warped Revenant"", ClassType.Farm)
                            );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Monster Blood",
    @"
case ""Monster Blood"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""ebondungeon"", ""Dethrix"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Vaughn Crest",
    @"
case ""Vaughn Crest"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""bocklincastle"", ""Garde Wraith"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Chiral Valley Knight",
    @"
case ""Chiral Valley Knight"":
                    BGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Chiral Valley Duke",
    @"
case ""Chiral Valley Duke"":
                    BGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Chiral Valley Noble",
    @"
case ""Chiral Valley Noble"":
                    BGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Antlered Golden Anjou Helm",
    @"
case ""Antlered Golden Anjou Helm"":
                    BGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Regal Golden Anjou Helm",
    @"
case ""Regal Golden Anjou Helm"":
                    BGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Anjou Duke's Helm",
    @"
case ""Anjou Duke's Helm"":
                    BGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Vaughn's Sash and Aquitaine",
    @"
case ""Vaughn's Sash and Aquitaine"":
                    BGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Aquitaine",
    @"
case ""Aquitaine"":
                    BGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Aquitaine",
    @"
case ""Dual Aquitaine"":
                    BGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Golden Anjou Helm",
    @"
case ""Golden Anjou Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""bocklingrove"", ""Elder Necromancer"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Vaughn's Crimson Sash",
    @"
case ""Vaughn's Crimson Sash"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""bocklingrove"", ""Elder Necromancer"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Gem of Anjou",
    @"
case ""Gem of Anjou"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""bocklingrove"", ""Elder Necromancer"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Forbidden EarthVessel Harpoon",
    @"
case ""Forbidden EarthVessel Harpoon"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""bocklincastle"", ""Headless Knight"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Forbidden EarthVessel Sword",
    @"
case ""Forbidden EarthVessel Sword"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""bocklincastle"", ""Headless Knight"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Forbidden EarthVessel Swords",
    @"
case ""Forbidden EarthVessel Swords"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""bocklincastle"", ""Headless Knight"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Forbidden EarthVessel Dagger",
    @"
case ""Forbidden EarthVessel Dagger"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""bocklincastle"", ""Headless Knight"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Forbidden EarthVessel Daggers",
    @"
case ""Forbidden EarthVessel Daggers"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""bocklincastle"", ""Headless Knight"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Forbidden EarthVessel Rapier",
    @"
case ""Forbidden EarthVessel Rapier"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""bocklincastle"", ""Headless Knight"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Forbidden EarthVessel Rapiers",
    @"
case ""Forbidden EarthVessel Rapiers"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""bocklincastle"", ""Headless Knight"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Forbidden EarthVessel Kris",
    @"
case ""Forbidden EarthVessel Kris"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""bocklincastle"", ""Headless Knight"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Forbidden Dual EarthVessel Kris",
    @"
case ""Forbidden Dual EarthVessel Kris"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""bocklincastle"", ""Headless Knight"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "BoneBreaker Medallion",
    @"
case ""BoneBreaker Medallion"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant) && Daily.CheckDailyv2(3898))
                    {
                        Core.EnsureAccept(3898);
                        Core.HuntMonster(""bonebreaker"", ""Undead Berserker"", ""Warrior Defeated"", 5);
                        Core.EnsureComplete(3898);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.ToBank(Core.QuestRewards(3898));
                    break;

    "
},
{
    "Bonecastle Token",
    @"
case ""Bonecastle Token"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""bonecastlec"", ""Undead Golden Knight"", req.Name, quant, false);
                    break;
    "
},
{
    "Vaden Helm Token",
    @"
case ""Vaden Helm Token"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""bonecastlec"", ""Vaden"", req.Name, quant, false);
                    break;
    "
},
{
    "Enthralling Gem Shard",
    @"
case ""Enthralling Gem Shard"":
                    Core.FarmingLogger(req.Name, quant);
                    if (Core.IsMember)
                        Core.RegisterQuests(10242);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(10240,
                        (""bocklingrove"", ""Elder Necromancer"", ClassType.Solo),
                        (""bocklingrove"", ""Undead Garde"", ClassType.Farm),
                        (""bocklingrove"", ""Garde Wraith"", ClassType.Farm));
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Victorious' Golden Scale",
    @"
case ""Victorious' Golden Scale"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""darkplane"", ""Victorious"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Sheathed Aquitaine",
    @"
case ""Sheathed Aquitaine"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""bocklingrove"", ""Elder Necromancer"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "ShadowFlame Armor Scrap",
    @"
case ""ShadowFlame Armor Scrap"":
                    SoC.CompleteCoreSoC();
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(7768);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        //The Shadows Recede 7768
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""BrightForest"", ""Shadowflame Scout"", ""ShadowFlame Troops \""Informed\"""", 30, log: false);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""BrightForest"", ""ShadowFlame Dragon"", ""ShadowFlame Dragon \""Informed\"""", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Bonecastle Amulet",
    @"
case ""Bonecastle Amulet"":

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(4993);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""bonecastle"", ""Green Rat"", ""Gamey Rat Meat"", 3);
                        Core.HuntMonster(""bonecastle"", ""Undead Waiter"", ""Waiter's Notepad"");
                        Core.HuntMonster(""bonecastle"", ""Turtle"", ""Turtle's Eggs"", 6);
                        Core.HuntMonster(""bonecastle"", ""Ghoul"", ""Ghoul \""Vinegar\"""", 6);
                        Core.HuntMonster(""bonecastle"", ""Grateful Undead"", ""Spices"", 2);
                        Core.HuntMonster(""bonecastle"", ""The Butcher"", ""Bag of Bone Flour"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "SilverSkull Amulet",
    @"
case ""SilverSkull Amulet"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5009);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""towersilver"", ""Fallen DeathKnight"", ""Chef Ramskull's Apron"");
                        Core.HuntMonster(""towersilver"", ""Undead Knight"", ""Chef Ramskull's Hat"");
                        Core.HuntMonster(""towersilver"", ""Undead Warrior"", ""Chef Ramskull's Cookbook"");
                        Core.HuntMonster(""towersilver"", ""Ghoul"", ""Chef Ramskull's Spatula"");
                        Core.HuntMonster(""towersilver"", ""Undead Guard"", ""Chef Ramskull's Skillet"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "GoldSkull Amulet",
    @"
case ""GoldSkull Amulet"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5023);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""towergold"", ""Book Maggot"", ""Book Pages"", 10);
                        Core.HuntMonster(""towergold"", ""Vampire Bat"", ""Batwing Leather"");
                        Core.HuntMonster(""towergold"", ""Skullspider"", ""Skullspider Silk"", 3);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "DeathKnight Lord Gauntlets",
    @"
case ""DeathKnight Lord Gauntlets"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""bonecastle"", ""Vaden"", req.Name, isTemp: false);
                    break;
    "
},
{
    "DeathKnight Lord Greaves",
    @"
case ""DeathKnight Lord Greaves"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""bonecastle"", ""Vaden"", req.Name, isTemp: false);
                    break;
    "
},
{
    "DeathKnight Lord Chest Plate",
    @"
case ""DeathKnight Lord Chest Plate"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""bonecastle"", ""Vaden"", req.Name, isTemp: false);
                    break;
    "
},
{
    "DeathKnight Lord Hauberk",
    @"
case ""DeathKnight Lord Hauberk"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""bonecastle"", ""Vaden"", req.Name, isTemp: false);
                    break;
    "
},
{
    "DeathKnight Lord Boots",
    @"
case ""DeathKnight Lord Boots"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""bonecastle"", ""Vaden"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Silver DeathKnight Lord Gauntlets",
    @"
case ""Silver DeathKnight Lord Gauntlets"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""towersilver"", ""Flester the Silver"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Silver DeathKnight Lord Greaves",
    @"
case ""Silver DeathKnight Lord Greaves"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""towersilver"", ""Flester the Silver"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Silver DeathKnight Lord Chest Plate",
    @"
case ""Silver DeathKnight Lord Chest Plate"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""towersilver"", ""Flester the Silver"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Silver DeathKnight Lord Hauberk",
    @"
case ""Silver DeathKnight Lord Hauberk"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""towersilver"", ""Flester the Silver"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Silver DeathKnight Lord Boots",
    @"
case ""Silver DeathKnight Lord Boots"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""towersilver"", ""Flester the Silver"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Golden DeathKnight Lord Gauntlets",
    @"
case ""Golden DeathKnight Lord Gauntlets"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""towergold"", ""Yurrod the Gold"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Golden DeathKnight Lord Greaves",
    @"
case ""Golden DeathKnight Lord Greaves"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""towergold"", ""Yurrod the Gold"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Golden DeathKnight Lord Chest Plate",
    @"
case ""Golden DeathKnight Lord Chest Plate"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""towergold"", ""Yurrod the Gold"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Golden DeathKnight Lord Hauberk",
    @"
case ""Golden DeathKnight Lord Hauberk"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""towergold"", ""Yurrod the Gold"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Golden DeathKnight Lord Boots",
    @"
case ""Golden DeathKnight Lord Boots"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""towergold"", ""Yurrod the Gold"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Space Jetsam",
    @"
case ""Space Jetsam"":
                    Core.EquipClass(ClassType.Solo);
                    Core.KillMonster(""byrodax"", ""r9"", ""Right"", ""Byro-Dax Monstrosity"", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Space Flotsam",
    @"
case ""Space Flotsam"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""byrodax"", ""Security Droid"", req.Name, req.Quantity, isTemp: req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Gaheris Sigil",
    @"
case ""Gaheris Sigil"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9829);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""castlegaheris"", ""Glacial Crystal"", ""Glacial Memory"", 30, log: false);
                        Core.HuntMonster(""castlegaheris"", ""Elemental Hybrid"", ""Hybrid Residue"", 9, log: false);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""castlegaheris"", ""Thundersnow Storm"", ""Thundersnow Sigh"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Courtly Mana Scholar Hair",
    @"
case ""Courtly Mana Scholar Hair"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""castlegaheris"", ""Thundersnow Storm"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Courtly Mana Scholar Locks",
    @"
case ""Courtly Mana Scholar Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""castlegaheris"", ""Thundersnow Storm"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Militis Snowflake Rapier",
    @"
case ""Militis Snowflake Rapier"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""castlegaheris"", ""Thundersnow Storm"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Militis Snowflake Rapiers",
    @"
case ""Militis Snowflake Rapiers"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""castlegaheris"", ""Thundersnow Storm"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Grimoire of Abra-Melin",
    @"
case ""Grimoire of Abra-Melin"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""castlegaheris"", ""Thundersnow Storm"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Delicate Snowflake Rapier",
    @"
case ""Delicate Snowflake Rapier"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""castlegaheris"", ""Glacial Crystal"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Delicate Snowflake Rapiers",
    @"
case ""Delicate Snowflake Rapiers"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""castlegaheris"", ""Glacial Crystal"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Lumin Badge",
    @"
case ""Lumin Badge"":
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""celestialarenad"", ""Queen of Hope"", req.Name, quant, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Celes Badge",
    @"
case ""Celes Badge"":
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""CelestialArenaC"", ""War Construct"", req.Name, quant, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Champion Sash",
    @"
case ""Champion Sash"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""celestialarenad"", ""Aranx"", ""Champion Sash"", quant, isTemp: false);
                    break;
    "
},
{
    "Celestial Quintessence",
    @"
case ""Celestial Quintessence"":
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""CelestialPast"", ""Blessed Bear"", req.Name, quant, isTemp: false);
                        Core.HuntMonster(""CelestialPast"", ""Blessed Deer"", req.Name, quant, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Infernal Token",
    @"
case ""Infernal Token"":
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""Celestialrealm"", ""Fallen Knight"", req.Name, quant, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Uncorrupt Spear Feather",
    @"
case ""Uncorrupt Spear Feather"":
                    QOM.CompleteEverything();
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(4508);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //Defeat the Diabolical Warlord! 4508
                        Core.HuntMonster(""lostruinswar"", ""Diabolical Warlord"", ""Diabolical Warlord Defeated!"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Diabolical Minion's Seed",
    @"
case ""Diabolical Minion's Seed"":
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""lostruinswar"", ""Diabolical Warlord"", req.Name, quant, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Thronekeeper's Rune",
    @"
case ""Thronekeeper's Rune"":
                    Core.FarmingLogger(""Thronekeeper's Rune"", quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(92791);
                    // Members get x2, non-members get x1 drops
                    Core.RegisterQuests(Core.IsMember ? 10266 : 10267);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, req.Quantity))
                    {
                        Core.HuntMonster(""bocklinsanctum"", ""Thronekeeper"", ""Black Armorial Fleur"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Champion Lynaria Armor",
    @"
case ""Champion Lynaria Armor"":
                    BocklinGroveM.BuyAllMerge(""Champion Lynaria Armor"");
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Valen's Knightly Armor",
    @"
case ""Valen's Knightly Armor"":
                    BocklinArmoryM.BuyAllMerge(""Valen's Knightly Armor"");
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "King Alteon's Armor Fragment",
    @"
case ""King Alteon's Armor Fragment"":
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(93763);
                    Core.HuntMonster(""alteonfight"", ""King Alteon"", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Scion's Regalia",
    @"
case ""Scion's Regalia"":
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(93762);
                    Core.HuntMonster(""bocklinsanctum"", ""Tarnished Scion"", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Chaorrupted Hamster",
    @"
case ""Chaorrupted Hamster"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""chaoslab"", ""Chaotic Server Hamster"", req.Name, isTemp: false, log: false);
                    break;
    "
},
{
    "Crystallized Chaos",
    @"
case ""Crystallized Chaos"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""chaoslab"", ""Chaorrupted Moglin"", req.Name, quant, isTemp: false, log: false);
                    break;
    "
},
{
    "Daimyo",
    @"
case ""Daimyo"":
                    Core.BuyItem(""necropolis"", 422, ""Daimyo"");
                    break;

    "
},
{
    "Shadow BeastMaster",
    @"
case ""Shadow BeastMaster"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""brightshadow"", ""Gravelyn the Good"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Shadow BeastMaster Bow",
    @"
case ""Shadow BeastMaster Bow"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""brightshadow"", ""Gravelyn the Good"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Shadow BeastMaster Hood",
    @"
case ""Shadow BeastMaster Hood"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""brightshadow"", ""Gravelyn the Good"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Shadow BeastMaster Hood + Mask",
    @"
case ""Shadow BeastMaster Hood + Mask"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""brightshadow"", ""Gravelyn the Good"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Shadow BeastMaster Knuckle",
    @"
case ""Shadow BeastMaster Knuckle"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""brightshadow"", ""Gravelyn the Good"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Shadow BeastMaster Quiver",
    @"
case ""Shadow BeastMaster Quiver"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""brightshadow"", ""Gravelyn the Good"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Shadow BeastMaster's Beard",
    @"
case ""Shadow BeastMaster's Beard"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""brightshadow"", ""Gravelyn the Good"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Shadow BeastMaster's Locks",
    @"
case ""Shadow BeastMaster's Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""brightshadow"", ""Gravelyn the Good"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Shadow BeastMaster's Shag",
    @"
case ""Shadow BeastMaster's Shag"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""brightshadow"", ""Gravelyn the Good"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Venerated Essence",
    @"
case ""Venerated Essence"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7738);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""brightshadow"", ""Brightfall Light"", ""BrightFall Light"", 5);
                        Core.HuntMonster(""brightshadow"", ""Brightfall Guard"", ""BrightFall Dark"", 10);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "ShadowFlame Glaive",
    @"
case ""ShadowFlame Glaive"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""chaosamulet"", ""Shadowflame Warrior"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "ShadowFlame Spellsword",
    @"
case ""ShadowFlame Spellsword"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""brightchaos"", ""Blight"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "ShadowFlame Spellsword's Sheathed Blade",
    @"
case ""ShadowFlame Spellsword's Sheathed Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""brightchaos"", ""Blight"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "ShadowFlame Spellsword's Hip Blade",
    @"
case ""ShadowFlame Spellsword's Hip Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""brightchaos"", ""Blight"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "ShadowFlame SpellSword's Blade",
    @"
case ""ShadowFlame SpellSword's Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""brightchaos"", ""Blight"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "ShadowFlame SpellSword's Daggers",
    @"
case ""ShadowFlame SpellSword's Daggers"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""brightchaos"", ""Blight"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "ShadowFlame SpellSword's Tome",
    @"
case ""ShadowFlame SpellSword's Tome"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""brightchaos"", ""Blight"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "SpellSword's Flame Blade",
    @"
case ""SpellSword's Flame Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""brightchaos"", ""Blight"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "SpellSword's Reversed Daggers",
    @"
case ""SpellSword's Reversed Daggers"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""brightchaos"", ""Blight"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Blight Essence",
    @"
case ""Blight Essence"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(7750);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""brightchaos"", ""Blight"", ""Blight Subdued"", 4);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Chaos Eye",
    @"
case ""Chaos Eye"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.KillMonster(""chaoswar"", ""r2"", ""Spawn"", ""*"", req.Name, quant, isTemp: false, log: false);
                    break;
    "
},
{
    "Chaos Tentacle",
    @"
case ""Chaos Tentacle"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.KillMonster(""chaoswar"", ""r2"", ""Spawn"", ""*"", req.Name, quant, isTemp: false, log: false);
                    break;

    "
},
{
    "Platinum Wings",
    @"
case ""Platinum Wings"":
                    Adv.BuyItem(""Castle"", 88, req.Name);
                    break;
    "
},
{
    "Fuchsia Dye",
    @"
case ""Fuchsia Dye"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(""Magenta Dye"");
                    if (Core.IsMember)
                    {
                        Core.RegisterQuests(1491);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            //Dyeing for Gemstones [Membership] 1491
                            Core.HuntMonster(""DarkoviaForest"", ""Lich Of The Stone"", ""Garnet Gem"", 2);
                            Core.HuntMonster(""Cornelis"", ""Gargoyle"", ""Spinel Gem"", 6);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    else
                    {
                        Core.RegisterQuests(1489);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            //Flowers for the Pink Gal 1489
                            Core.HuntMonster(""Sandsea"", ""Cactus Creeper"", ""Fandango Flower"", 5);
                            Core.KillMonster(""wanders"", ""r5"", ""Left"", ""Lotus Spider"", ""Lotus Flower"", 4);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    break;
    "
},
{
    "Plague Strike Scythe",
    @"
case ""Plague Strike Scythe"":
                    Adv.BuyItem(""ShadowFall"", 89, req.Name);
                    break;
    "
},
{
    "Zealith Reavers",
    @"
case ""Zealith Reavers"":
                    Core.FarmingLogger(req.Name, quant);
                    LegionExercise3.Exercise(new[] { ""Judgement Hammer"", ""Legion Token"" });
                    LegionExercise4.Exercise(new[] { ""Judgement Scythe"", ""Legion Token"" });
                    CoreLegion.FarmLegionToken(50);
                    Adv.BuyItem(""Underworld"", 238, req.Name);
                    break;
    "
},
{
    "Great Astral Wings",
    @"
case ""Great Astral Wings"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""Elemental"", ""Mana Falcon"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Wave Cutter",
    @"
case ""Wave Cutter"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""Pirates"", ""Shark Bait"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Star Caster Staff",
    @"
case ""Star Caster Staff"":
                    Adv.BuyItem(""Castle"", 48, req.Name);
                    break;
    "
},
{
    "Scarlet's Costume",
    @"
case ""Scarlet's Costume"":
                    Adv.BuyItem(""Sleuthhound"", 65, req.Name);
                    break;
    "
},
{
    "Infernal Dark Blade of Cruelty",
    @"
case ""Infernal Dark Blade of Cruelty"":
                    Adv.BuyItem(""Battleon"", 10, req.Name);
                    break;
    "
},
{
    "Chaos Dragonlord Helm",
    @"
case ""Chaos Dragonlord Helm"":
                    Core.EquipClass(ClassType.Solo);
                    Core.KillVath(req.Name, isTemp: false);
                    break;
    "
},
{
    "Rose Aura of the Ascended",
    @"
case ""Rose Aura of the Ascended"":
                    Core.Logger($""{req.Name} is seasonal AC item and only available Beleen's Birthday event."");
                    Adv.BuyItem(Bot.Map.Name, 1966, req.Name);
                    break;
    "
},
{
    "Prismatic Dye",
    @"
case ""Prismatic Dye"":
                    Adv.BuyItem(""Tower"", 1966, req.Name);
                    break;
    "
},
{
    "Iron Dreadsaw",
    @"
case ""Iron Dreadsaw"":
                    Core.FarmingLogger(req.Name, quant);
                    if (!Core.CheckInventory(""Raw Dreadsaw""))
                    {
                       Nation.ApprovalAndFavor(10, 0);
                       Nation.FarmDiamondofNulgath(5);
                       Nation.FarmTaintedGem(10);
                        Adv.BuyItem(""ArchPortal"", 1211, ""Raw Dreadsaw"");
                    }
                   Nation.ApprovalAndFavor(40, 20);
                   Nation.FarmGemofNulgath(10);
                    Adv.BuyItem(""ArchPortal"", 1211, req.Name);
                    break;
    "
},
{
    "Dual Manslayer of Taro",
    @"
case ""Dual Manslayer of Taro"":
                    if (Core.IsMember)
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.EquipClass(ClassType.Farm);
                        Core.RegisterQuests(625);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            TarosManslayer.GuardianTaro(ManslayerOnly: true);
                           Nation.FarmDiamondofNulgath(7);
                           Nation.FarmDarkCrystalShard(13);
                           Nation.FarmTaintedGem(13);
                           Nation.FarmUni13(1);
                           Nation.FarmVoucher(member: true);
                            Core.HuntMonster(""Underworld"", ""Undead Bruiser"", ""Undead Bruiser Rune"");
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    else
                        Core.Logger($""\""{req.Name}\"" requires Membership to obtain"");
                    break;
    "
},
{
    "Demonhuntress Horns",
    @"
case ""Demonhuntress Horns"":
                    if (!Core.CheckInventory(""Blindfolded Pink Demonhuntress Horns"") || !Core.CheckInventory(""Pink Demonhuntress Horns""))
                    {
                        Core.Logger($""{req.Name} is pseudo-Rare, you don't have the Rare item to merge this material"");
                        return;
                    }
                    Adv.BuyItem(""Curio"", 1070, req.Name);
                    break;
    "
},
{
    "Demonhunter Horns",
    @"
case ""Demonhunter Horns"":
                    if (!Core.CheckInventory(""Blindfolded Pink Demonhunter Horns"") || !Core.CheckInventory(""Pink Demonhunter Horns""))
                    {
                        Core.Logger($""{req.Name} is pseudo-Rare, you don't have the Rare item to merge this material"");
                        return;
                    }
                    if (Core.CheckInventory(""Blindfolded Pink Demonhunter Horns""))
                        Adv.BuyItem(""Curio"", 1214, req.Name);
                    else
                        Adv.BuyItem(""Curio"", 52, req.Name);
                    break;
    "
},
{
    "DOOMFire Warrior",
    @"
case ""DOOMFire Warrior"":
                    if (!Core.HasAchievement(19, ""ip6""))
                        Core.Logger($""\""{req.Name}\"" is Special Offer item, You need to have 200k Acs achievment badge"");
                    else
                        Adv.BuyItem(""Battleon"", 1306, req.Name);
                    break;
    "
},
{
    "Fire Imp Tail",
    @"
case ""Fire Imp Tail"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""Mobius"", ""Fire Imp"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Unarmed",
    @"
case ""Unarmed"":
                    Adv.BuyItem(Bot.Map.Name, 1536, req.Name);
                    break;
    "
},
{
    "Scarbucks Latte",
    @"
case ""Scarbucks Latte"":
                    Core.Logger($""\""{req.Name}\"" is Member & Seasonal item"");
                    Adv.BuyItem(""FearFeast"", 1190, req.Name);
                    break;
    "
},
{
    "Valor High Halo",
    @"
case ""Valor High Halo"":
                    if (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.Logger($""{req.Name} is acquired from 'Open Treasure Chests! quest from Twilly"");
                        return;
                    }
                    break;
    "
},
{
    "Doge the Evil",
    @"
case ""Doge the Evil"":
                    if (Core.IsMember)
                    {

                        Core.FarmingLogger(req.Name, quant);
                        Core.EquipClass(ClassType.Farm);
                        Core.EnsureAccept(2951);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            //Legion Armored Daimyo [Member] 2951
                            Core.HuntMonster(""Ruins"", ""Dark Elemental"", ""Souls of the Destroyed"", 15);
                            Core.HuntMonster(""bludrut4"", ""Shadow Serpent"", ""Shadow Essence"", 4);
                            Core.HuntMonster(""GreenguardWest"", ""Black Knight"", ""Black Metal Armor"", 4);
                        }
                        Core.EnsureCompleteChoose(2951, new[] { req.Name });
                    }
                    else
                        Core.Logger($""\""{req.Name}\"" requires Membership to obtain"");
                    break;
    "
},
{
    "Shimmering Flakes",
    @"
case ""Shimmering Flakes"":
                    Adv.BuyItem(""BlindingSnow"", 236, req.Name);
                    break;
    "
},
{
    "Red Rose",
    @"
case ""Red Rose"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""BattleWedding"", ""EbilCorp Ninja"", ""Love Token"", 10, isTemp: false);
                    Adv.BuyItem(""ArtixWedding"", 788, req.Name);
                    break;
    "
},
{
    "Scarbucks Espresso Cup",
    @"
case ""Scarbucks Espresso Cup"":
                    Core.Logger($""\""{req.Name}\"" is Member & Seasonal item"");
                    Adv.BuyItem(""FearFeast"", 1190, req.Name);
                    break;
    "
},
{
    "Shadowslayer Armor",
    @"
case ""Shadowslayer Armor"":
                    Adv.BuyItem(""DarkoviaForest "", 138, req.Name);
                    break;
    "
},
{
    "ShadowSlayer Hat",
    @"
case ""ShadowSlayer Hat"":
                    Adv.BuyItem(""DarkoviaForest "", 138, req.Name);
                    break;
    "
},
{
    "Shadow Z Hat",
    @"
case ""Shadow Z Hat"":
                    Adv.BuyItem(""DarkoviaForest "", 138, req.Name);
                    break;
    "
},
{
    "Reavers Of Good",
    @"
case ""Reavers Of Good"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""BrightFortress"", ""Dark Assassin"", ""Mirror Token"", 35, isTemp: false);
                    Adv.BuyItem(""BrightFortress"", 795, req.Name);
                    break;
    "
},
{
    "Slayer's Neophyte Broadsword",
    @"
case ""Slayer's Neophyte Broadsword"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""DarkoviaForest"", ""Lich of the Stone"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Slithering ShadowSlayer",
    @"
case ""Slithering ShadowSlayer"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""DarkoviaForest"", ""Lich of the Stone"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Slithering Hunter's Hat",
    @"
case ""Slithering Hunter's Hat"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""DarkoviaForest"", ""Lich of the Stone"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Slithering Hunter's Hat + Locks",
    @"
case ""Slithering Hunter's Hat + Locks"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""DarkoviaForest"", ""Lich of the Stone"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Slayer's Wooden Pistol",
    @"
case ""Slayer's Wooden Pistol"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""DarkoviaForest"", ""Lich of the Stone"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Slithering Hunter's Knife",
    @"
case ""Slithering Hunter's Knife"":
                    if (Core.IsMember)
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""DarkoviaForest"", ""Lich of the Stone"", req.Name, isTemp: false);
                    }
                    else
                        Core.Logger($""\""{req.Name}\"" requires Membership to obtain"");
                    break;
    "
},
{
    "Slayer's Wooden Rifle",
    @"
case ""Slayer's Wooden Rifle"":
                    if (Core.IsMember)
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""DarkoviaForest"", ""Lich of the Stone"", req.Name, isTemp: false);
                    }
                    else
                        Core.Logger($""\""{req.Name}\"" requires Membership to obtain"");
                    break;
    "
},
{
    "Time Piece",
    @"
case ""Time Piece"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8171);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.KillMonster(""shadowrealmpast"", ""r2"", ""Right"", ""Shadow Guardian"", ""Shadow Guardians Defeated"");
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Skye's Lightning",
    @"
case ""Skye's Lightning"":
                    Core.FarmingLogger(req.Name, quant);
                    // 9834 | Eilean a' Cheò
                    Core.RegisterQuests(9834);
                    CoreAOR.ColdThunderBoss(req.Name, quant, req.Temp);
                    Bot.Wait.ForPickup(""Skye's Lightning"");
                    break;
    "
},
{
    "Electrifying Zilla Tail",
    @"
case ""Electrifying Zilla Tail"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""castlegaheris"", ""Energy Elemental"", req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Electrifying Zilla Bag",
    @"
case ""Electrifying Zilla Bag"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""castlegaheris"", ""Energy Elemental"", req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Crawler Leg",
    @"
case ""Crawler Leg"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""mirrormaze"", ""Doom Crawler"", req.Name, quant, false);
                    break;
    "
},
{
    "Zombie Dragon Scale",
    @"
case ""Zombie Dragon Scale"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""mirrormaze"", ""Zombie Dragon"", req.Name, quant, false);
                    break;
    "
},
{
    "De'Sawed's Stinger",
    @"
case ""De'Sawed's Stinger"":
                    Core.EquipClass(ClassType.Solo);
                    Core.KillMonster(""catacombs"", ""Boss2"", ""Left"", ""Dr. De'Sawed"", req.Name, quant, false);
                    break;

    "
},
{
    "Celestial Coin",
    @"
case ""Celestial Coin"":
                    DjinnGuard.CompleteDjinnGuard();
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(6275);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //Celestial Spoils: Djinn Warrior 6275
                        Core.HuntMonster(""DjinnGuard"", ""Air Spirit"", ""Air Essence"", 3, log: false);
                        Core.HuntMonster(""DjinnGuard"", ""Water Spirit"", ""Water Essence"", 3, log: false);
                        Core.HuntMonster(""DjinnGuard"", ""Earth Spirit"", ""Earth Essence"", 3, log: false);
                        Core.HuntMonster(""DjinnGuard"", ""Fire Spirit"", ""Fire Essence"", 3, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Blade of the Fallen Djinn",
    @"
case ""Blade of the Fallen Djinn"":
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""DjinnGuard"", ""Image of Crulon"", req.Name, quant, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Blade of the Djinn King",
    @"
case ""Blade of the Djinn King"":
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""DjinnGuard"", ""Image of Crulon"", req.Name, quant, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Lucky Button",
    @"
case ""Lucky Button"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(9836,
(""deadmoor"", ""Toxic Souleater"", ClassType.Farm),
    (""moonlab"", ""Infected Scientist"", ClassType.Farm),
    (""deerhunt"", ""Zweinichthirsch"", ClassType.Solo)
);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Sobekemsaph's Hieroglyph",
    @"
case ""Sobekemsaph's Hieroglyph"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9538);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""dreampalace"", ""Golmoth"", ""Hieroglyph Ruby"", log: false);
                        Core.HuntMonster(""dreampalace"", ""Flaming Harpy"", ""Flame Glyph"", 6, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Sobekemsaph's Scale",
    @"
case ""Sobekemsaph's Scale"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9539);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""crocriver"", ""Sobekemsaph"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Sphinx Sentinel Helm",
    @"
case ""Sphinx Sentinel Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""crocriver"", ""Sobekemsaph"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Sheathed Black Moon Blades",
    @"
case ""Sheathed Black Moon Blades"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""crocriver"", ""Sobekemsaph"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Sphinx Sentinel Cape",
    @"
case ""Sphinx Sentinel Cape"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""crocriver"", ""Sobekemsaph"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Molten Sword",
    @"
case ""Molten Sword"":
                    Adv.BuyItem(""Fotia"", 649, req.Name);
                    break;
    "
},
{
    "Ash Priest Hood",
    @"
case ""Ash Priest Hood"":
                    Adv.BuyItem(""Fotia"", 649, req.Name);
                    break;
    "
},
{
    "Priest of the Ashes",
    @"
case ""Priest of the Ashes"":
                    Adv.BuyItem(""Fotia"", 649, req.Name);
                    break;
    "
},
{
    "Magitech Plating",
    @"
case ""Magitech Plating"":
                    Adv.BuyItem(""UnderRealm"", 660, req.Name);
                    break;
    "
},
{
    "Ancient Undead Helm",
    @"
case ""Ancient Undead Helm"":
                    Legion.JoinLegion();
                    Adv.BuyItem(""RavenScar"", 615, req.Name);
                    break;
    "
},
{
    "The Scythe of Lost Hope",
    @"
case ""The Scythe of Lost Hope"":
                    Core.FarmingLogger(req.Name, quant);
                    Legion.ApprovalAndFavor(0, 50);
                    Adv.BuyItem(""UnderWorld"", 454, req.Name);
                    break;
    "
},
{
    "Legion Beast Within",
    @"
case ""Legion Beast Within"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);

                    Legion.ObsidianRock(60);
                    Legion.SoulForgeHammer();

                    //Soul Forgery 2743
                    Core.RegisterQuests(2743);
                    while (!Core.CheckInventory(""Solidified Soul"", 50))
                    {
                        Core.HuntMonster(""ShadowFallInvasion"", ""Bone Creeper"", ""Shards of a Soul"", 10);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();

                    Legion.FarmLegionToken(50);
                    Adv.BuyItem(""underworld"", 577, req.Name);
                    break;
    "
},
{
    "Golden Bough",
    @"
case ""Golden Bough"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);

                    Core.EnsureAccept(3010);
                    Core.HuntMonster(""UnderRealm"", ""Underworld Soul"", ""Souls Released"", 8);
                    Core.EnsureComplete(3010);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Molten Staff",
    @"
case ""Molten Staff"":
                    Core.HuntMonster(""Fotia"", ""Fotia Elemental"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Psyche",
    @"
case ""Psyche"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);

                    //Judged on Allegiance to Dage 3041
                    Core.RegisterQuests(3041);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""Judgement"", ""Aeacus"", ""Aeacus' Permission"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Heroic Berserker",
    @"
case ""Heroic Berserker"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""chaosmilitia"", ""Xiang"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Heroic Berserker Shag",
    @"
case ""Heroic Berserker Shag"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""chaosmilitia"", ""Xiang"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Heroic Berserker Locks",
    @"
case ""Heroic Berserker Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""chaosmilitia"", ""Xiang"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Heroic Berserker Skullcap",
    @"
case ""Heroic Berserker Skullcap"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""chaosmilitia"", ""Xiang"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Heroic Berserker Blade",
    @"
case ""Heroic Berserker Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""chaosmilitia"", ""Xiang"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Heroic Berserker Hammer",
    @"
case ""Heroic Berserker Hammer"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""chaosmilitia"", ""Xiang"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Heroic Berserker Axe",
    @"
case ""Heroic Berserker Axe"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""chaosmilitia"", ""Xiang"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Heroic Berserker Accoutrements",
    @"
case ""Heroic Berserker Accoutrements"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""chaosmilitia"", ""Xiang"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Militia Merit",
    @"
case ""Militia Merit"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(5775, ""citadel"", ""Inquisitor Guard"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Darkblood Guards",
    @"
case ""Darkblood Guards"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""falguard"", 544, req.Name, quant);
                    break;
    "
},
{
    "Enchanted Dark Blood",
    @"
case ""Enchanted Dark Blood"":
                    Core.FarmingLogger(req.Name, quant);
                    Daily.EnchantedDarkBlood();
                    if (!Core.CheckInventory(req.Name, quant))
                    {
                        Core.Logger($""{req.Name} is a daily quest drop, you have {Bot.Inventory.GetQuantity(req.Name)} out of {quant}. Run the script again tomorrow."");
                    }
                    break;

    "
},
{
    "ShadowFlame War Medal",
    @"
case ""ShadowFlame War Medal"":
                    SWM.Medals(quant);
                    break;
    "
},
{
    "ShadowFlame Battle Spear",
    @"
case ""ShadowFlame Battle Spear"":
                    Core.FarmingLogger(req.Name, quant);
                    SWM.Medals(100);
                    Adv.BuyItem(""chaosamulet"", 1914, req.Name);
                    break;
    "
},
{
    "ShadowFlame Battle Staff",
    @"
case ""ShadowFlame Battle Staff"":
                    Core.FarmingLogger(req.Name, quant);
                    SWM.Medals(100);
                    Adv.BuyItem(""chaosamulet"", 1914, req.Name);
                    break;
    "
},
{
    "ShadowFlame Broadsword",
    @"
case ""ShadowFlame Broadsword"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""chaosamulet"", ""Goldun"", req.Name, quant);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Glowing Pumpkinseed",
    @"
case ""Glowing Pumpkinseed"":
                    if (Core.IsMember)
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.RegisterQuests(4617);

                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            //ULTRA Pumpkinseed Farming Quest 4617 [Member]
                            Core.HuntMonster(""CruxShip"", ""Apephryx"", ""Otherworld Sigil"", isTemp: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    else
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.RegisterQuests(4615);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            //Gather the Gold Debns 4615
                            Core.HuntMonster(""CruxShip"", ""Treasure Hunter"", ""Debns Gathered"", 6);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    break;

    "
},
{
    "Carnival Ticket",
    @"
case ""Carnival Ticket"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""dreamforest"", ""r3"", ""Left"", ""*"", ""Carnival Ticket"", 300);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Darkblood War Medal",
    @"
case ""Darkblood War Medal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5874, 5875); //Acolyte's Medallions 5874, Acolyte's Mega Medallions 5875
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""kolyaban"", ""r2"", ""Left"", ""*"", ""Acolyte's Medallion"", 4);
                        Core.KillMonster(""kolyaban"", ""r2"", ""Left"", ""*"", ""Acolyte's Mega Medallion"", 2);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Glowing Sock",
    @"
case ""Glowing Sock"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(2777);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""greenguardwest"", ""Slime"", ""Slimy Lost Sock"", 5, true, false);
                        Core.HuntMonster(""greenguardeast"", ""Wolf"", ""Furry Lost Sock"", 2, true, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Pure Monstrite",
    @"
case ""Pure Monstrite"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(""Queen's Follower Slain"");
                    Core.RegisterQuests(8095);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""transformation"", ""Enter"", ""Spawn"", ""*"", ""Queen's Follower Slain"", 100, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Icy Pelt",
    @"
case ""Icy Pelt"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8433);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""deerhunt"", ""Scared Wolf"", ""Wolf Warded"", 9);
                        Core.HuntMonster(""deerhunt"", ""Deer?"", ""Deer Deterred"", 3);
                        Core.HuntMonster(""deerhunt"", ""Frightened Owl"", ""Owl Ousted"", 6);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "WinterWild Axe",
    @"
case ""WinterWild Axe"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""deerhunt"", ""r8"", ""Left"", ""Zweinichthirsch"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Old Moglin Teddy Mace",
    @"
case ""Old Moglin Teddy Mace"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""deerhunt"", ""r8"", ""Left"", ""Zweinichthirsch"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Deimos' Chain",
    @"
case ""Deimos' Chain"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.Logger($""{req.Name} requires you to kill Devastator Deimos which skua can't do, use army."");
                    break;
    "
},
{
    "Cursed Grudge Nagamaki",
    @"
case ""Cursed Grudge Nagamaki"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.Logger($""{req.Name} requires you to kill Devastator Deimos which skua can't do, use army."");
                    break;
    "
},
{
    "Deimos' Fang",
    @"
case ""Deimos' Fang"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.Logger($""{req.Name} requires you to kill Devastator Deimos which skua can't do, use army."");
                    break;
    "
},
{
    "Toxic Gem",
    @"
case ""Toxic Gem"":
                    Core.FarmingLogger(req.Name, quant);
                    FarmToxicGem(quant);
                    break;
    "
},
{
    "Gamma Toxic Gem",
    @"
case ""Gamma Toxic Gem"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9751);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Adv.BuyItem(""tercessuinotlim"", 1951, ""Doomatter"", Log: false);
                        Adv.BuyItem(""ectocave"", 2449, ""Toxian Metal"", Log: false);
                        FarmToxicGem(5);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Elodie's Trinket",
    @"
case ""Elodie's Trinket"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9155);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                       Core.HuntMonster(""shogunwar"", ""Bamboo Treeant"", ""Bamboo Stalk"", 7);
                       Core.HuntMonster(""aozorahills"", ""Reishi"", ""Dried Reishi"", 7);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Unseen Essence",
    @"
case ""Unseen Essence"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(6162);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""djinngate"", ""Harpy"", ""Potent Harpy Mana"", 2, true, false);
                        Core.HuntMonster(""djinngate"", ""Lamia"", ""Potent Lamia Mana"", 2, true, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    
                    break;

    "
},
{
    "Flame Incantation",
    @"
case ""Flame Incantation"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9848);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""djinnguard"", UseableMonsters[1], ""Jaan's Flames"");

                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Silver Ward",
    @"
case ""Silver Ward"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9849);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""towerofmirrors"", UseableMonsters[2], ""Silver Tincture"", 10);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Almoravid's Bracer",
    @"
case ""Almoravid's Bracer"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9850);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""crulonwed"", UseableMonsters[0], ""Silver Tincture"", 10);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Luminous Soul Bow",
    @"
case ""Luminous Soul Bow"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""crulonwed"", UseableMonsters[0], req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Luminous Soul Spear",
    @"
case ""Luminous Soul Spear"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""crulonwed"", UseableMonsters[0], req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Luminous Emblem",
    @"
case ""Luminous Emblem"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""crulonwed"", UseableMonsters[0], req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Luminous Soul Blade",
    @"
case ""Luminous Soul Blade"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""crulonwed"", UseableMonsters[0], req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Desert Bandana",
    @"
case ""Desert Bandana"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""crulonwed"", UseableMonsters[0], req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Honored Sandsea Guest",
    @"
case ""Honored Sandsea Guest"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""crulonwed"", UseableMonsters[0], req.Name, quant, req.Temp);
                    break;

    "
},
{
    "General Gall Medal",
    @"
case ""General Gall Medal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(5147);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //Battle: You vs General Gall! 5147
                        Core.HuntMonster(""deathpit"", ""General Gall"", ""General Gall Defeated"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "General Velm Medal",
    @"
case ""General Velm Medal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(5149);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //General Velm 5149
                        Core.HuntMonster(""deathpit"", ""General Velm"", ""General Velm Defeated"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "General Hun'Gar Medal",
    @"
case ""General Hun'Gar Medal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(5155);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //Do You Even Brawl 5155
                        Core.HuntMonster(""deathpit"", ""Velm's Restorer"", ""Death Pit Token"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "General Chud Medal",
    @"
case ""General Chud Medal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(5151);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //General Chud 5151
                        Core.HuntMonster(""deathpit"", ""General Chud"", ""General Chud Defeated"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Bounty Hunter Dubloon",
    @"
case ""Bounty Hunter Dubloon"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    // Very Trobblesome 9394
                    Core.RegisterQuests(9394);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                        Core.HuntMonsterMapID(""dreadspace"", 48, ""Trobble Captured"");
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Deep Trobble Plunger",
    @"
case ""Deep Trobble Plunger"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    // Pressurized Weapon 9157
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID))
                    {
                        Core.EnsureAccept(9157);
                        Core.HuntMonster(""brightoak"", ""Bright Treeant"", ""Chunk of Rubber"", 10);
                        Core.HuntMonster(""marsh"", ""Marsh Tree"", ""Broken Sticks"", 8);
                        Adv.BuyItem(""yulgar"", 16, 16946, shopItemID: 10477);
                        Core.EnsureComplete(9157);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Phoenix Gate Token",
    @"
case ""Phoenix Gate Token"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    if (!Core.IsMember)
                    {
                        Core.RegisterQuests(4214);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""phoenixrise"", ""Firestorm Tiger"", ""Tigerskin"", 5, log: false);
                            Core.HuntMonster(""phoenixrise"", ""Infernal Goblin"", ""Strips of Goblin Leather"", 3, log: false);
                            Core.HuntMonster(""phoenixrise"", ""Lava Troll"", ""Lava Globule"", 4, log: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    else
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.EquipClass(ClassType.Solo);
                        Core.RegisterQuests(4215);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""phoenixrise"", ""Cinderclaw"", ""Minotiger Horn"", log: false);
                            Core.HuntMonster(""phoenixrise"", ""Gargrowl"", ""Stone Shard"", log: false);
                            Core.HuntMonster(""phoenixrise"", ""Pyrric Ursus"", ""Crystal Pommel"", log: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    break;

    "
},
{
    "Dragon Shinobi Token",
    @"
case ""Dragon Shinobi Token"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Yokai.Quests();
                    Core.RegisterQuests(7924);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""shadowfortress"", ""1st Head of Orochi"", ""Perfect Orochi Scales"", 10, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Shadowscythe Trooper",
    @"
case ""Shadowscythe Trooper"":
                    Core.HuntMonsterMapID(""thorngarde"", 2, req.Name, isTemp: false);
                    break;
    "
},
{
    "ShadowScythe Trooper's Helm",
    @"
case ""ShadowScythe Trooper's Helm"":
                    Core.HuntMonsterMapID(""thorngarde"", 2, req.Name, isTemp: false);
                    break;
    "
},
{
    "ShadowScythe Trooper's Cape",
    @"
case ""ShadowScythe Trooper's Cape"":
                    Core.HuntMonsterMapID(""thorngarde"", 2, req.Name, isTemp: false);
                    break;
    "
},
{
    "ShadowScythe Blade",
    @"
case ""ShadowScythe Blade"":
                    Core.HuntMonsterMapID(""thorngarde"", 2, req.Name, isTemp: false);
                    break;
    "
},
{
    "Salvaged Deadtech Node",
    @"
case ""Salvaged Deadtech Node"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(7601);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""thorngarde"", ""CryptHacker"", ""Deadtech Power Core"", 7);
                        Core.HuntMonster(""thorngarde"", ""CryptHacker"", ""CryptHacker Circuitry"", 15);
                        Core.HuntMonster(""thorngarde"", ""NecroMech"", ""NecroMech Targeting Systems"", 5);

                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""thorngarde"", ""Zyrus the BioKnight"", ""BioKnight Engine"", 3);

                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "ShadowScythe Rogue",
    @"
case ""ShadowScythe Rogue"":
                    Core.HuntMonsterMapID(""thorngarde"", 3, req.Name, isTemp: false);
                    break;
    "
},
{
    "ShadowScythe Rogue's Helm",
    @"
case ""ShadowScythe Rogue's Helm"":
                    Core.HuntMonsterMapID(""thorngarde"", 3, req.Name, isTemp: false);
                    break;
    "
},
{
    "ShadowScythe Rogue's Cape",
    @"
case ""ShadowScythe Rogue's Cape"":
                    Core.HuntMonsterMapID(""thorngarde"", 3, req.Name, isTemp: false);
                    break;
    "
},
{
    "ShadowScythe Reversed Daggers",
    @"
case ""ShadowScythe Reversed Daggers"":
                    Core.HuntMonsterMapID(""thorngarde"", 3, req.Name, isTemp: false);
                    break;
    "
},
{
    "ShadowScythe Daggers",
    @"
case ""ShadowScythe Daggers"":
                    Core.HuntMonsterMapID(""thorngarde"", 3, req.Name, isTemp: false);
                    break;
    "
},
{
    "ShadowScythe Mage",
    @"
case ""ShadowScythe Mage"":
                    Core.HuntMonsterMapID(""thorngarde"", 5, req.Name, isTemp: false);
                    break;
    "
},
{
    "ShadowScythe Mage's Hat",
    @"
case ""ShadowScythe Mage's Hat"":
                    Core.HuntMonsterMapID(""thorngarde"", 5, req.Name, isTemp: false);
                    break;
    "
},
{
    "ShadowScythe Mage's Hat + Locks",
    @"
case ""ShadowScythe Mage's Hat + Locks"":
                    Core.HuntMonsterMapID(""thorngarde"", 5, req.Name, isTemp: false);
                    break;
    "
},
{
    "ShadowScythe Mage's Rune",
    @"
case ""ShadowScythe Mage's Rune"":
                    Core.HuntMonsterMapID(""thorngarde"", 5, req.Name, isTemp: false);
                    break;
    "
},
{
    "ShadowScythe Staff",
    @"
case ""ShadowScythe Staff"":
                    Core.HuntMonsterMapID(""thorngarde"", 5, req.Name, isTemp: false);
                    break;
    "
},
{
    "Zealous Paladin",
    @"
case ""Zealous Paladin"":
                    Core.HuntMonster(""techdungeon"", ""Kalron the Cryptborg"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Zealous Veil",
    @"
case ""Zealous Veil"":
                    Core.HuntMonster(""techdungeon"", ""Kalron the Cryptborg"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Zealous Cherubs",
    @"
case ""Zealous Cherubs"":
                    Core.HuntMonster(""techdungeon"", ""Kalron the Cryptborg"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Cryptborg",
    @"
case ""Cryptborg"":
                    Core.HuntMonster(""techdungeon"", ""Kalron the Cryptborg"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Cryptborg Wrap",
    @"
case ""Cryptborg Wrap"":
                    Core.HuntMonster(""techdungeon"", ""Kalron the Cryptborg"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Cryptborg Torpedo",
    @"
case ""Cryptborg Torpedo"":
                    Core.HuntMonster(""techdungeon"", ""Kalron the Cryptborg"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Cryptborg Blade",
    @"
case ""Cryptborg Blade"":
                    Core.HuntMonster(""techdungeon"", ""Kalron the Cryptborg"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Cryptborg Helm",
    @"
case ""Cryptborg Helm"":
                    Core.HuntMonster(""techdungeon"", ""Kalron the Cryptborg"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Zealous Badge",
    @"
case ""Zealous Badge"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(7616);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""techdungeon"", ""Kalron the Cryptborg"", ""Immutable Dedication"", 7, log: false);
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""techdungeon"", ""DoomBorg Guard"", ""Paladin Armor Scraps"", 30, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Zealous Rays Of Light",
    @"
case ""Zealous Rays Of Light"":
                    Core.HuntMonster(""stonewood"", ""BioKnight"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Deadtech Booster",
    @"
case ""Deadtech Booster"":
                    Core.HuntMonster(""stonewood"", ""Doomwood Treeant"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "DoomMaster",
    @"
case ""DoomMaster"":
                    Bot.Quests.UpdateQuest(7635);
                    Core.HuntMonster(""stonewood"", ""Sir Kut"", req.Name, isTemp: false);
                    break;
    "
},
{
    "DoomMaster Horns",
    @"
case ""DoomMaster Horns"":
                    Bot.Quests.UpdateQuest(7635);
                    Core.HuntMonster(""stonewood"", ""Sir Kut"", req.Name, isTemp: false);
                    break;
    "
},
{
    "DoomMaster's Wrap",
    @"
case ""DoomMaster's Wrap"":
                    Bot.Quests.UpdateQuest(7635);
                    Core.HuntMonster(""stonewood"", ""Sir Kut"", req.Name, isTemp: false);
                    break;
    "
},
{
    "DoomMaster's Whip",
    @"
case ""DoomMaster's Whip"":
                    Bot.Quests.UpdateQuest(7635);
                    Core.HuntMonster(""stonewood"", ""Sir Kut"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Accursed Arsenic of Doom",
    @"
case ""Accursed Arsenic of Doom"":
                    SDKA.UpgradeMetal((HardCoreMetalsEnum)Enum.Parse(typeof(HardCoreMetalsEnum), req.Name.Split(' ')[1]));
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Baneful Beryllium of Doom",
    @"
case ""Baneful Beryllium of Doom"":
                    SDKA.UpgradeMetal((HardCoreMetalsEnum)Enum.Parse(typeof(HardCoreMetalsEnum), req.Name.Split(' ')[1]));
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Calamitous Chromium of Doom",
    @"
case ""Calamitous Chromium of Doom"":
                    SDKA.UpgradeMetal((HardCoreMetalsEnum)Enum.Parse(typeof(HardCoreMetalsEnum), req.Name.Split(' ')[1]));
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Pernicious Palladium of Doom",
    @"
case ""Pernicious Palladium of Doom"":
                    SDKA.UpgradeMetal((HardCoreMetalsEnum)Enum.Parse(typeof(HardCoreMetalsEnum), req.Name.Split(' ')[1]));
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Reprehensible Rhodium of Doom",
    @"
case ""Reprehensible Rhodium of Doom"":
                    SDKA.UpgradeMetal((HardCoreMetalsEnum)Enum.Parse(typeof(HardCoreMetalsEnum), req.Name.Split(' ')[1]));
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Diabolical Aura",
    @"
case ""Diabolical Aura"":
                    SDKA.PinpointBroadsword(quant);
                    break;
    "
},
{
    "Corrupt Spirit Orb",
    @"
case ""Corrupt Spirit Orb"":
                    SDKA.DoomKnightWK(req.Name, quant);
                    break;
    "
},
{
    "Ominous Aura",
    @"
case ""Ominous Aura"":
                    SDKA.DoomKnightWK(req.Name, quant);
                    break;
    "
},
{
    "Dark Spirit Orb",
    @"
case ""Dark Spirit Orb"":
                    SDKA.DSO(quant);
                    break;
    "
},
{
    "Dark Energy",
    @"
case ""Dark Energy"":
                    Core.FarmingLogger(req.Name, quant);  // Log the farming request

                    // Ensure no negative values when calculating the sell amount
                    int sellAmount = Math.Max(0, Bot.Inventory.GetQuantity(""Dark Energy"") - quant);

                    // Proceed if there’s at least one item to sell
                    if (sellAmount > 0)
                    {
                        Core.Logger($""Selling {sellAmount} Dark Energy to prevent errors... Hopefully."", ""Sell"");
                        Core.SellItem(""Dark Energy"", sellAmount);
                    }

                    // Equip the farming class and start killing monsters
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""dwarfhold"", ""r2"", ""Left"", ""Chaos Drow"", req.Name, quant, false);
                    break;
    "
},
{
    "Undead Energy",
    @"
case ""Undead Energy"":
                    Farm.BattleUnderB(""Undead Energy"", quant);
                    break;
    "
},
{
    "DoomKnight Weapon Kit",
    @"
case ""DoomKnight Weapon Kit"":
                    SDKA.DoomKnightWK(quant: quant);
                    break;
    "
},
{
    "DoomSoldier Weapon Kit",
    @"
case ""DoomSoldier Weapon Kit"":
                    SDKA.DoomSoldierWK(quant);
                    break;
    "
},
{
    "DoomSquire Weapon Kit",
    @"
case ""DoomSquire Weapon Kit"":
                    SDKA.DoomSquireWK(quant);
                    break;
    "
},
{
    "Legion Daimyo Armor",
    @"
case ""Legion Daimyo Armor"":
                    Core.AddDrop(req.Name);
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(2951);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ruins"", ""Dark Elemental"", ""Heart of Darkness"", 15);
                        Core.HuntMonster(""bludrut4"", ""Shadow Serpent"", ""Shadow Essence"", 4);
                        Core.HuntMonster(""GreenguardWest"", ""Black Knight"", ""Black Metal Armor"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Dark Daimyo Armor",
    @"
case ""Dark Daimyo Armor"":
                    Core.AddDrop(req.Name);
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(2080);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ruins"", ""Dark Elemental"", ""Heart of Darkness"", 15);
                        Core.HuntMonster(""bludrut4"", ""Shadow Serpent"", ""Shadow Essence"", 4);
                        Core.HuntMonster(""GreenguardWest"", ""Black Knight"", ""Black Metal Armor"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Druid Fabric",
    @"
case ""Druid Fabric"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(800, 801);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""arcangrove"", ""Gorillaphant"", ""Gorillaphant Tusk"", 6);
                        Core.HuntMonster(""arcangrove"", ""Seed Spitter"", ""Spool of Arcane Thread"", 10);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Dwarven Metal",
    @"
case ""Dwarven Metal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9237);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""dwarfhold"", ""Chaos Drow"", ""Broken Drow Blade"", 5, log: false);
                        Core.HuntMonster(""dwarfhold"", ""Chaotic Draconian"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Dwarven Alloy",
    @"
case ""Dwarven Alloy"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9238);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""mooncursedlair"", ""Shard of Moonlight"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Dwobo Coin",
    @"
case ""Dwobo Coin"":
                    Nation.DwoboCoin(quant);
                    break;

    "
},
{
    "Red Space Fabric",
    @"
case ""Red Space Fabric"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""dreadspace"", ""Red Trobble"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Blue Space Fabric",
    @"
case ""Blue Space Fabric"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""dreadspace"", ""Trobble"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Yellow Space Fabric",
    @"
case ""Yellow Space Fabric"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""dreadspace"", ""Troblor"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Scrap Metal",
    @"
case ""Scrap Metal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""dreadspace"", ""Undead Space Marine"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Antimatter dye",
    @"
case ""Antimatter dye"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(4289);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""dreadspace"", ""Undead Space Marine"", ""Golden Spork of Justice"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Star Scrap Metal",
    @"
case ""Star Scrap Metal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(4289);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""dreadspace"", ""Undead Space Marine"", ""Golden Spork of Justice"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Cyber Brain Core",
    @"
case ""Cyber Brain Core"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""dreadspace"", ""Dread Space"", req.Name, quant);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Unstable Isotope",
    @"
case ""Unstable Isotope"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(4294, ""dreadspace"", ""Dra'gorn"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Blinding Light of Dread Space",
    @"
case ""Blinding Light of Dread Space"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(4294, ""dreadspace"", ""Dra'gorn"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Enchanted Crystal",
    @"
case ""Enchanted Crystal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(8722);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""dreadforest"", ""Reignolds' Knight"", ""Valuable Metals"", 8);
                        Core.HuntMonster(""dreadforest"", ""Taxidermied Servant"", ""Gold Pouches"", 8);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""dreadforest"", ""Lord Reignolds"", ""Reignolds' Brooch"", 1);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Blade of Dread",
    @"
case ""Blade of Dread"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""dreadforest"", ""Noble's Knight"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Greatsword of Dread",
    @"
case ""Greatsword of Dread"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""dreadforest"", ""Treacherous Bandit"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Dagger of Dread",
    @"
case ""Dagger of Dread"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""dreadforest"", ""Treacherous Bandit"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Daggers of Dread",
    @"
case ""Daggers of Dread"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""dreadforest"", ""Treacherous Bandit"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "High Axe of Dread",
    @"
case ""High Axe of Dread"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster($""dreadforest"", ""Noble’s Servant"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Greataxe of Dread",
    @"
case ""Greataxe of Dread"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8722);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""dreadforest"", ""Taxidermied Servant"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Poleaxe of Dread",
    @"
case ""Poleaxe of Dread"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8722);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""dreadforest"", ""Lord Reignolds"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Axes of Dread",
    @"
case ""Axes of Dread"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8722);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""dreadforest"", ""Reignolds' Knight"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Handaxe of Dread",
    @"
case ""Handaxe of Dread"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""dreadforest"", ""Noble's Knight"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Handaxes of Dread",
    @"
case ""Handaxes of Dread"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""dreadforest"", ""Noble's Knight"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Flower of Renewal",
    @"
case ""Flower of Renewal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(4669);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""elfhame"", ""Blighted Deer"", ""Deer Horn"", 2, true, false);
                        Core.HuntMonster(""elfhame"", ""Wolfrider"", ""Elfhame Wolf Pelt"", 2, true, false);
                        Core.HuntMonster(""elfhame"", ""Ruin Dweller"", ""Ruin Dweller Remains"", 3, true, false);
                        Core.HuntMonster(""elfhame"", ""Ratawampus"", ""Ratawampus Tail"", 2, true, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Reality Shard",
    @"
case ""Reality Shard"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8456);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""eternalchaos"", ""Chaos Time Fairy"", ""Preserved Chaos Fairy Wing"", 5, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Dark Earth Token",
    @"
case ""Dark Earth Token"":
                    if (Core.IsMember)
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.RegisterQuests(1718, 1719);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""etherwarevil"", ""Tainted Emu"", ""Twisted Emu Feather"", 6, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Pelican"", ""Twisted Pelican Feather"", 6, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Hummingbird"", ""Twisted Hummingbird Feather"", 6, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Phoenix"", ""Twisted Phoenix Feather"", 6, true, false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    else
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.RegisterQuests(1719);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""etherwarevil"", ""Tainted Emu"", ""Twisted Emu Feather"", 3, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Pelican"", ""Twisted Pelican Feather"", 3, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Hummingbird"", ""Twisted Hummingbird Feather"", 3, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Phoenix"", ""Twisted Phoenix Feather"", 3, true, false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    break;

    "
},
{
    "Dark Water Token",
    @"
case ""Dark Water Token"":
                    if (Core.IsMember)
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.RegisterQuests(1718, 1719);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""etherwarevil"", ""Tainted Emu"", ""Twisted Emu Feather"", 6, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Pelican"", ""Twisted Pelican Feather"", 6, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Hummingbird"", ""Twisted Hummingbird Feather"", 6, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Phoenix"", ""Twisted Phoenix Feather"", 6, true, false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    else
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.RegisterQuests(1719);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""etherwarevil"", ""Tainted Emu"", ""Twisted Emu Feather"", 3, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Pelican"", ""Twisted Pelican Feather"", 3, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Hummingbird"", ""Twisted Hummingbird Feather"", 3, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Phoenix"", ""Twisted Phoenix Feather"", 3, true, false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    break;

    "
},
{
    "Dark Fire Token",
    @"
case ""Dark Fire Token"":
                    if (Core.IsMember)
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.RegisterQuests(1718, 1719);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""etherwarevil"", ""Tainted Emu"", ""Twisted Emu Feather"", 6, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Pelican"", ""Twisted Pelican Feather"", 6, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Hummingbird"", ""Twisted Hummingbird Feather"", 6, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Phoenix"", ""Twisted Phoenix Feather"", 6, true, false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    else
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.RegisterQuests(1719);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""etherwarevil"", ""Tainted Emu"", ""Twisted Emu Feather"", 3, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Pelican"", ""Twisted Pelican Feather"", 3, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Hummingbird"", ""Twisted Hummingbird Feather"", 3, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Phoenix"", ""Twisted Phoenix Feather"", 3, true, false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    break;

    "
},
{
    "Dark Air Token",
    @"
case ""Dark Air Token"":
                    if (Core.IsMember)
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.RegisterQuests(1718, 1719);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""etherwarevil"", ""Tainted Emu"", ""Twisted Emu Feather"", 6, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Pelican"", ""Twisted Pelican Feather"", 6, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Hummingbird"", ""Twisted Hummingbird Feather"", 6, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Phoenix"", ""Twisted Phoenix Feather"", 6, true, false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    else
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.RegisterQuests(1719);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""etherwarevil"", ""Tainted Emu"", ""Twisted Emu Feather"", 3, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Pelican"", ""Twisted Pelican Feather"", 3, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Hummingbird"", ""Twisted Hummingbird Feather"", 3, true, false);
                            Core.HuntMonster(""etherwarevil"", ""Tainted Phoenix"", ""Twisted Phoenix Feather"", 3, true, false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    break;

    "
},
{
    "Axe of Golmoth",
    @"
case ""Axe of Golmoth"":
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""DreamPalace"", ""Golmoth"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Scales of Golmoth",
    @"
case ""Scales of Golmoth"":
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""DreamPalace"", ""Golmoth"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Token of Air",
    @"
case ""Token of Air"":
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""DreamPalace"", ""Mote of Power"", req.Name, quant, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Token of Water",
    @"
case ""Token of Water"":
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""DreamPalace"", ""Mote of Power"", req.Name, quant, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Token of Earth",
    @"
case ""Token of Earth"":
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""DreamPalace"", ""Mote of Power"", req.Name, quant, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Token of Fire",
    @"
case ""Token of Fire"":
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""DreamPalace"", ""Mote of Power"", req.Name, quant, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Zahad's Ancient Gem",
    @"
case ""Zahad's Ancient Gem"":
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""DreamPalace"", ""Zahad"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Scythe of Gazeroth",
    @"
case ""Scythe of Gazeroth"":
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""DreamPalace"", ""Gazeroth"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Souls of Gazeroth",
    @"
case ""Souls of Gazeroth"":
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""DreamPalace"", ""Gazeroth"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Claws of Zelkur",
    @"
case ""Claws of Zelkur"":
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""DreamPalace"", ""Zelkur"", req.Name, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Bow of Zelkur",
    @"
case ""Bow of Zelkur"":
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""DreamPalace"", ""Zelkur"", req.Name, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Feathers of Zal",
    @"
case ""Feathers of Zal"":
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""DreamPalace"", ""Zal"", req.Name, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Scimitar of Zal",
    @"
case ""Scimitar of Zal"":
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""DreamPalace"", ""Zal"", req.Name, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Water Defender Token",
    @"
case ""Water Defender Token"":
                    if (Core.IsMember)
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.RegisterQuests(1716, 1717);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""etherwargood"", ""Tainted Emu"", ""Twisted Emu Feather"", 6, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Pelican"", ""Twisted Pelican Feather"", 6, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Hummingbird"", ""Twisted Hummingbird Feather"", 6, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Phoenix"", ""Twisted Phoenix Feather"", 6, true, false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    else
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.RegisterQuests(1717);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""etherwargood"", ""Tainted Emu"", ""Twisted Emu Feather"", 3, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Pelican"", ""Twisted Pelican Feather"", 3, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Hummingbird"", ""Twisted Hummingbird Feather"", 3, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Phoenix"", ""Twisted Phoenix Feather"", 3, true, false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    break;

    "
},
{
    "Fire Defender Token",
    @"
case ""Fire Defender Token"":
                    if (Core.IsMember)
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.RegisterQuests(1716, 1717);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""etherwargood"", ""Tainted Emu"", ""Twisted Emu Feather"", 6, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Pelican"", ""Twisted Pelican Feather"", 6, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Hummingbird"", ""Twisted Hummingbird Feather"", 6, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Phoenix"", ""Twisted Phoenix Feather"", 6, true, false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    else
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.RegisterQuests(1717);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""etherwargood"", ""Tainted Emu"", ""Twisted Emu Feather"", 3, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Pelican"", ""Twisted Pelican Feather"", 3, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Hummingbird"", ""Twisted Hummingbird Feather"", 3, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Phoenix"", ""Twisted Phoenix Feather"", 3, true, false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    break;

    "
},
{
    "Earth Defender Token",
    @"
case ""Earth Defender Token"":
                    if (Core.IsMember)
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.RegisterQuests(1716, 1717);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""etherwargood"", ""Tainted Emu"", ""Twisted Emu Feather"", 6, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Pelican"", ""Twisted Pelican Feather"", 6, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Hummingbird"", ""Twisted Hummingbird Feather"", 6, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Phoenix"", ""Twisted Phoenix Feather"", 6, true, false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    else
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.RegisterQuests(1717);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""etherwargood"", ""Tainted Emu"", ""Twisted Emu Feather"", 3, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Pelican"", ""Twisted Pelican Feather"", 3, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Hummingbird"", ""Twisted Hummingbird Feather"", 3, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Phoenix"", ""Twisted Phoenix Feather"", 3, true, false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    break;

    "
},
{
    "Air Defender Token",
    @"
case ""Air Defender Token"":
                    if (Core.IsMember)
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.RegisterQuests(1716, 1717);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""etherwargood"", ""Tainted Emu"", ""Twisted Emu Feather"", 6, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Pelican"", ""Twisted Pelican Feather"", 6, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Hummingbird"", ""Twisted Hummingbird Feather"", 6, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Phoenix"", ""Twisted Phoenix Feather"", 6, true, false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    else
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.RegisterQuests(1717);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""etherwargood"", ""Tainted Emu"", ""Twisted Emu Feather"", 3, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Pelican"", ""Twisted Pelican Feather"", 3, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Hummingbird"", ""Twisted Hummingbird Feather"", 3, true, false);
                            Core.HuntMonster(""etherwargood"", ""Tainted Phoenix"", ""Twisted Phoenix Feather"", 3, true, false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    break;

    "
},
{
    "Ezrajal Insignia",
    @"
case ""Ezrajal Insignia"":
                    Core.Logger($""{req.Name} needs to be farmed manually."");
                    break;
    "
},
{
    "Warden Insignia",
    @"
case ""Warden Insignia"":
                    Core.Logger($""{req.Name} needs to be farmed manually."");
                    break;
    "
},
{
    "Engineer Insignia",
    @"
case ""Engineer Insignia"":
                    Core.Logger($""{req.Name} needs to be farmed manually."");
                    break;
    "
},
{
    "Exalted Artillery Shard",
    @"
case ""Exalted Artillery Shard"":
                    Core.Logger($""{req.Name} needs to be farmed manually."");
                    break;
    "
},
{
    "Exalted Drone Pet",
    @"
case ""Exalted Drone Pet"":
                    Core.Logger($""{req.Name} needs to be farmed manually."");
                    break;
    "
},
{
    "Exalted Node",
    @"
case ""Exalted Node"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""timeinn"", ""r3"", ""Bottom"", ""*"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Exalted Relic Piece",
    @"
case ""Exalted Relic Piece"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""timeinn"", ""The Warden"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Exalted Forgemetal",
    @"
case ""Exalted Forgemetal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""timeinn"", ""Ezrajal"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Algid Token",
    @"
case ""Algid Token"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""battlegrounda"", ""r2"", ""Left"", ""*"", isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Frost Token",
    @"
case ""Frost Token"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""battlegroundb"", ""r2"", ""Left"", ""*"", isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Icy Token",
    @"
case ""Icy Token"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""battlegroundc"", ""r2"", ""Left"", ""*"", isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Rime Token",
    @"
case ""Rime Token"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""battlegroundd"", ""r2"", ""Left"", ""*"", isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Gelid Token",
    @"
case ""Gelid Token"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""battlegrounde"", ""r2"", ""Left"", ""*"", isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Glacial Token",
    @"
case ""Glacial Token"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""battlegroundf"", ""r2"", ""Left"", ""*"", isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Blighted Deer's Hide",
    @"
case ""Blighted Deer's Hide"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.HuntMonster(""snowmore"", ""Blighted Deer"", req.Name, quant, false, false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Blighted Lion's Fang",
    @"
case ""Blighted Lion's Fang"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.HuntMonster(""snowmore"", ""Blighted Lion"", req.Name, quant, false, false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Blighted Wolf's Blood",
    @"
case ""Blighted Wolf's Blood"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.HuntMonster(""snowmore"", ""Blighted Wolf"", req.Name, quant, false, false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Blighted Dragon's Bone",
    @"
case ""Blighted Dragon's Bone"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.HuntMonster(""snowmore"", ""Blighted Dragon"", req.Name, quant, false, false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Winter Throne",
    @"
case ""Winter Throne"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.HuntMonster(""snowmore"", ""Jon S'NOOOOOOO"", req.Name, quant, false, false); ;
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Salvaged Skye Armament",
    @"
case ""Salvaged Skye Armament"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.KillMonster(""castleeblana"", ""r2"", ""Left"", ""Skye Warrior"", req.Name, quant, req.Temp, false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Liquid Gold Solution",
    @"
case ""Liquid Gold Solution"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(9742);
                        Core.EquipClass(ClassType.Farm);
                        Core.KillMonster(""castleeblana"", ""r6"", ""Left"", ""*"", ""Gorta's Soul"", 12, log: false);
                        Core.KillMonster(""castleeblana"", ""r5"", ""Left"", ""*"", ""Raven's Bauble"", 12, log: false);
                        Core.EquipClass(ClassType.Solo);
                        Core.KillMonster(""castleeblana"", ""r10"", ""Left"", ""Warden Indradeep"", ""Rainfall Inscription"", log: false);
                        Core.EnsureComplete(9742);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Dungeon Token",
    @"
case ""Dungeon Token"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.KillMonster(""darkdungeon"", ""r9"", ""Left"", ""Cockatrice"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Nation Medallion",
    @"
case ""Nation Medallion"":
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8495);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""fiendpast"", ""Proto-Legion Knight"", ""Legionnaire Defeated"", 10);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Fiendish Outlaw Revolver",
    @"
case ""Fiendish Outlaw Revolver"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""fiendpast"", ""Dage the Lich"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Fiendish Outlaw Bowie Knife",
    @"
case ""Fiendish Outlaw Bowie Knife"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""fiendpast"", ""Dage the Lich"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Fiendish Outlaw Sheathed Shotgun",
    @"
case ""Fiendish Outlaw Sheathed Shotgun"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""fiendpast"", ""Dage the Lich"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Fire and Ice Token",
    @"
case ""Fire and Ice Token"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(6326);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""drakonnan"", ""Fire Dragon"", ""Dragon Scale"");
                        Core.HuntMonster(""drakonnan"", ""Living Fire"", ""Ember of a Living Flame"");
                        Core.HuntMonster(""drakonnan"", ""Fire Elemental"", ""Fire Elemental's Gauntlet"");
                        Core.HuntMonster(""drakonnan"", ""Living Lava"", ""Lava Rock"");
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Ice Katana",
    @"
case ""Ice Katana"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(6319);
                    Core.HuntMonster(""drakonnan"", ""Living Fire"", ""Inferno Heart"");
                    Core.EnsureComplete(6319);
                    break;

    "
},
{
    "Ichorus Scythe Piece",
    @"
case ""Ichorus Scythe Piece"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(3874);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ectocave"", ""Ichor Dracolich"", ""Uncut Emerald"", 9);
                        Core.HuntMonster(""ectocave"", ""Ektorax"", ""Brilliant Diamond"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Ichorus Egg",
    @"
case ""Ichorus Egg"":
                    if (!Bot.Player.IsMember)
                        break;

                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""ectocave"", ""Ektorax"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Pure Ichor Gem",
    @"
case ""Pure Ichor Gem"":
                    if (!Bot.Player.IsMember)
                        break;

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(3873);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ectocave"", ""Ichor Draconian"", ""Uncut Ichor Gem"", 50, req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Slime",
    @"
case ""Slime"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""ectocave"", ""Swamp Lurker"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Dragon Rogue Klinge",
    @"
case ""Dragon Rogue Klinge"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ectocave"", ""Ektorax"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Dragon Rogue",
    @"
case ""Dragon Rogue"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ectocave"", ""Ektorax"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Dragon Rogue Hood",
    @"
case ""Dragon Rogue Hood"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ectocave"", ""Ektorax"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Dragon Rogue Twin Klinge Cape",
    @"
case ""Dragon Rogue Twin Klinge Cape"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ectocave"", ""Ektorax"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Piece of Fabric",
    @"
case ""Piece of Fabric"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""ectocave"", ""Ichor Draconian"", req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Bone",
    @"
case ""Bone"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""ectocave"", ""Ichor Draconian"", req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Wind Stone",
    @"
case ""Wind Stone"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(3316);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""fableforest"", ""Wind Elemental"", ""Wind Aura"", 5);
                        Core.HuntMonster(""fableforest"", ""Forest Fury"", ""Forest Fury Feather"", 5);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Fire Stone",
    @"
case ""Fire Stone"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(3314);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""fableforest"", ""Fire Elemental"", ""Fire Aura"", 5);
                        Core.HuntMonster(""fableforest"", ""Bloodwolf"", ""Bloodwolf Pelt"", 5);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Water Stone",
    @"
case ""Water Stone"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(3315);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""fableforest"", ""Water Elemental"", ""Water Aura"", 5);
                        Core.HuntMonster(""fableforest"", ""Aqueevil"", ""Aqueevil Spirit"", 5);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Chaos Stone",
    @"
case ""Chaos Stone"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(3318);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""fableforest"", ""Forest Guardian"", ""Chaos Aura"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Plain Dragon Tail",
    @"
case ""Plain Dragon Tail"":
                    Adv.BuyItem(""fableforest"", 814, req.Name);
                    break;

    "
},
{
    "OakHeart Helm",
    @"
case ""OakHeart Helm"":
                    Adv.BuyItem(""fableforest"", 814, req.Name);
                    break;

    "
},
{
    "OakHeart ArmBlades",
    @"
case ""OakHeart ArmBlades"":
                    Adv.BuyItem(""fableforest"", 814, req.Name);
                    break;

    "
},
{
    "Not Quite Dread Mask",
    @"
case ""Not Quite Dread Mask"":
                    Adv.BuyItem(""fableforest"", 814, req.Name);
                    break;

    "
},
{
    "Not Quite Dread Shape",
    @"
case ""Not Quite Dread Shape"":
                    Adv.BuyItem(""fableforest"", 814, req.Name);
                    break;

    "
},
{
    "Hydra Cape",
    @"
case ""Hydra Cape"":
                    Adv.BuyItem(""fableforest"", 814, req.Name);
                    break;

    "
},
{
    "Dreadspider Cape",
    @"
case ""Dreadspider Cape"":
                    Adv.BuyItem(""fableforest"", 814, req.Name);
                    break;

    "
},
{
    "Dreadspider Abdomen",
    @"
case ""Dreadspider Abdomen"":
                    Adv.BuyItem(""fableforest"", 814, req.Name);
                    break;

    "
},
{
    "Red Dragon Morph",
    @"
case ""Red Dragon Morph"":
                    Adv.BuyItem(""fableforest"", 814, req.Name);
                    break;

    "
},
{
    "Faerie Botanis Sword",
    @"
case ""Faerie Botanis Sword"":
                    Adv.BuyItem(""fableforest"", 814, req.Name);
                    break;

    "
},
{
    "Burningjay Feather",
    @"
case ""Burningjay Feather"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""extinction"", ""Lard"", isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Coal",
    @"
case ""Coal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""extinction"", ""Cyworg"", isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Fabric Scraps",
    @"
case ""Fabric Scraps"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""extinction"", ""Lard"", isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Iron II.0",
    @"
case ""Iron II.0"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""extinction"", ""Cyworg"", isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "A Kitten?",
    @"
case ""A Kitten?"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""extinction"", ""SN.O.W."", isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Golden Egg",
    @"
case ""Golden Egg"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""battlefowl"", ""Chicken"", isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Chickenwing ArmBlade",
    @"
case ""Chickenwing ArmBlade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""battlefowl"", ""Chicken"", isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Golden Feather",
    @"
case ""Golden Feather"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""battlefowl"", ""Chicken"", isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Chicken Claw",
    @"
case ""Chicken Claw"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""battlefowl"", ""Chicken"", isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Stories of the Forest",
    @"
case ""Stories of the Forest"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(4645);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""rivensylth"", ""Rivensylth Spider"", req.Name, quant, true, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Faded Pigment",
    @"
case ""Faded Pigment"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(""Roses"", ""Strawberries"", ""Rubies"");
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9107);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""battleodium"", ""Widowing"", ""Roses"", 1, false, false);
                        Core.KillMonster(""battleodium"", ""r6"", ""Left"", ""*"", ""Strawberries"", 1, false, false);
                        while (!Bot.ShouldExit && !Core.CheckInventory(76286)) ///multiple items with name ""Rubies""
                            Core.KillMonster(""battleodium"", ""r6"", ""Left"", ""*"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Grapes",
    @"
case ""Grapes"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""battleodium"", ""r6"", ""Left"", ""*"", req.Name, quant, false, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Diamonds",
    @"
case ""Diamonds"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""battleodium"", ""r6"", ""Left"", ""*"", req.Name, quant, false, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Frozen SpiderSilk",
    @"
case ""Frozen SpiderSilk"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(10018, ""frozenqueen"", ""Frostspinner Queen"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Ice Vapor",
    @"
case ""Ice Vapor"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.KillMonster(""lair"", ""Enter"", ""Spawn"", ""*"", req.Name, quant, req.Temp, false);
                    break;

    "
},
{
    "Cysero's Cookie",
    @"
case ""Cysero's Cookie"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""goose"", ""Queen's Sage"", ""Cysero's Cookie"", quant, false, false);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Elemental Embers",
    @"
case ""Elemental Embers"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8125, 8126);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.KillMonster(""fireplanewar"", ""r5"", ""Right"", ""*"", ""War Medal"", 5, log: false);
                        Core.KillMonster(""fireplanewar"", ""r5"", ""Right"", ""*"", ""Mega War Medal"", 3, log: false);
                        Bot.Wait.ForPickup(req.ID);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Burnt Cinders",
    @"
case ""Burnt Cinders"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(8131);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(""fireplanewar"", ""ShadowClaw"", ""ShadowClaw Defeated"", log: false);
                        Bot.Wait.ForPickup(req.ID);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Seared Ashes",
    @"
case ""Seared Ashes"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(""fireplanewar"", ""ShadowFlame Phedra"", req.Name, quant, log: false);
                        Bot.Wait.ForPickup(req.ID);
                    }
                    break;
    "
},
{
    "ShadowFlame Flamberge",
    @"
case ""ShadowFlame Flamberge"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.BuyItem(""fireplanewar"", 2007, req.Name, quant);
                        Bot.Wait.ForPickup(req.ID);
                    }
                    break;
    "
},
{
    "Refulgent Flamberge",
    @"
case ""Refulgent Flamberge"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(""fireplanewar"", ""Shadowflame Soldier"", req.Name, quant, log: false);
                        Bot.Wait.ForPickup(req.ID);
                    }
                    break;
    "
},
{
    "ShadowFlame Great Harp",
    @"
case ""ShadowFlame Great Harp"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.BuyItem(""fireplanewar"", 2007, req.Name, quant);
                        Bot.Wait.ForPickup(req.ID);
                    }
                    break;
    "
},
{
    "Vulcan Great Harp",
    @"
case ""Vulcan Great Harp"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(""fireplanewar"", ""Shadefire Onslaught"", req.Name, quant, log: false);
                        Bot.Wait.ForPickup(req.ID);
                    }
                    break;
    "
},
{
    "Flame Guardian",
    @"
case ""Flame Guardian"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""firewar"", 1586, req.Name);
                    break;
    "
},
{
    "Flame Guardian Helm",
    @"
case ""Flame Guardian Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""firewar"", 1586, req.Name);
                    break;
    "
},
{
    "Flame Guardian's Wrap",
    @"
case ""Flame Guardian's Wrap"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""firewar"", 1586, req.Name);
                    break;
    "
},
{
    "Flame Guardian's Blade",
    @"
case ""Flame Guardian's Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""firewar"", 1586, req.Name);
                    break;
    "
},
{
    "Flame Guardian's Lance",
    @"
case ""Flame Guardian's Lance"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""firewar"", 1586, req.Name);
                    break;
    "
},
{
    "Flame Guardian's Blade + Shield",
    @"
case ""Flame Guardian's Blade + Shield"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""firewar"", 1586, req.Name);
                    break;
    "
},
{
    "Dragon Flame",
    @"
case ""Dragon Flame"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(6300);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""firewar"", ""Fire Dragon"", ""Fire Dragon Slain"", 3);
                        Core.KillMonster(""firewar"", ""r8"", ""Left"", ""Inferno Dragon"", ""Inferno Dragon Slain"", 2);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Dragon Eye",
    @"
case ""Dragon Eye"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""firewar"", ""Uriax"", ""Dragon Eye"", quant, false, false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Flame Guardian's Accoutrements",
    @"
case ""Flame Guardian's Accoutrements"":
                    Core.FarmingLogger(req.Name, quant);
                    if (Core.IsMember)
                        Adv.BuyItem(""firewar"", 1586, ""Flame Guardian's Accoutrements"");
                    else
                        Core.Logger(""Membership is required."");

                    break;

    "
},
{
    "Village's Grace",
    @"
case ""Village's Grace"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9601, 9602);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""hakuwar"", ""Enter"", ""Spawn"", ""*"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Sword and Scroll Badge",
    @"
case ""Sword and Scroll Badge"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7495);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //Studying the Bard 7495
                        Core.EquipClass(ClassType.Farm);
                        Core.KillMonster(""palooza"", ""Act6"", ""Left"", ""Music Pirate"", ""Lo-Fi Recording"", 4);
                        Core.EquipClass(ClassType.Solo);
                        Core.KillMonster(""Stairway"", ""r8"", ""Right"", ""*"", ""Scroll: O'Carolan's Reel"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Harley's Reinforced Steel",
    @"
case ""Harley's Reinforced Steel"":


                    if (!Core.CheckInventory(new[] { ""ArchPaladin"", ""Chaos Avenger"", ""Verus DoomKnight"", ""Void Highlord"", ""Void Highlord (IoDA)"" }, any: true))
                    {
                        Core.Logger(""These Classes are not required, but making killing the boss... possible atleast solo: AP/VDk/CAV/VHL. if you dont have *any* of them... good luck killing it"", ""**WARNING**"");
                        Core.Logger($""Bot will use {Bot.Player?.CurrentClass} to farm Harley's Reinforced Steel"", ""WARNING"");
                    }
                    else
                    {
                        foreach (string ClassName in new[] { ""Chaos Avenger"", ""Verus DoomKnight"", ""Void Highlord"", ""Void Highlord (IoDA)"", ""ArchPaladin"" })
                        {
                            if (Core.CheckInventory(ClassName))
                            {
                                Core.Unbank(ClassName);
                                Core.Equip(ClassName);
                                Core.Logger($""Using {ClassName} for farming Harley's Reinforced Steel"");
                                break;
                            }
                            continue;
                        }
                    }
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10177);
                    Core.KillMonster(""trainers"", ""r3"", ""Left"", ""Warlord Harley"", req.Name, req.Quantity);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Hydra Scale Piece",
    @"
case ""Hydra Scale Piece"":
                    Core.HuntMonster(""hydrachallenge"", ""Hydra Head 25"", req.Name, quant, isTemp: false, true);

                    break;
    "
},
{
    "Enchanted Pearl",
    @"
case ""Enchanted Pearl"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""hydrachallenge"", ""Hydra Head 90"", req.Name, quant, req.Temp, false, publicRoom: Core.PublicDifficult);
                    break;
    "
},
{
    "Hero's Hilt Fragment",
    @"
case ""Hero's Hilt Fragment"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(3002);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""doomvaultb"", ""Grim Shelleton"", ""Relic of Strength"", 3);
                        Core.HuntMonster(""doomvaultb"", ""Grim Fire Mage"", ""Relic of Heart "", 3);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Hero's Blade Fragment",
    @"
case ""Hero's Blade Fragment"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(3001);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""doomvaultb"", ""Grim Lich"", ""Relic of Courage"", 3);
                        Core.HuntMonster(""doomvaultb"", ""Grim Ectomancer"", ""Relic of Will"", 3);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Grime Token",
    @"
case ""Grime Token"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""doomvault"", ""Grim Shelleton"", req.Name, quant);
                    break;
    "
},
{
    "Binky's Uni-horn",
    @"
case ""Binky's Uni-horn"":
                    Core.EquipClass(ClassType.Solo);
                    Core.KillMonster(""doomvault"", ""r5"", ""Left"", ""Binky"", req.Name, quant, req.Temp, publicRoom: true);
                    break;
    "
},
{
    "Grimskull's Face",
    @"
case ""Grimskull's Face"":
                    Core.HuntMonsterMapID(""doomvaultb"", 48, req.Name, isTemp: req.Temp);
                    break;
    "
},
{
    "GrimBlade",
    @"
case ""GrimBlade"":
                    if (!Core.isCompletedBefore(3004))
                        DVB.StoryLine();

                    if (!Core.CheckInventory(""GrimBlade""))
                    {
                        Core.EnsureAccept(3004);
                        Core.HuntMonsterMapID(""doomvaultb"", 48, ""Raxgore Slain"");
                        Core.EnsureComplete(3004);
                        Bot.Wait.ForPickup(""GrimBlade"");
                    }
                    break;

    "
},
{
    "Eternal Scale",
    @"
case ""Eternal Scale"":
                    Core.FarmingLogger(req.Name, quant);
                    Bot.Quests.UpdateQuest(2804);
                    Core.RegisterQuests(9175);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                    Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""venomvaults"", ""Manticore"", ""Manticore Stinger"", 3);
                    Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""worldscore"", ""Elemental Attempt"", ""Attempt's Essence"", 3);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Tengu Feather",
    @"
case ""Tengu Feather"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9600);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.KillMonster(""hakuvillage"", ""r3"", ""Left"", ""*"", ""Enchanted Chime"", 8, log: false);
                        Core.KillMonster(""hakuvillage"", ""r4"", ""Left"", ""*"", ""Pale Scale"", 8, log: false);

                        Core.EquipClass(ClassType.Solo);
                        Core.KillMonster(""hakuvillage"", ""r5"", ""Left"", ""*"", ""Wind Blade"", log: false);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Mikoto's Puppet String",
    @"
case ""Mikoto's Puppet String"":
                    DOY.YokaiRealm();
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9690);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""yokairealm"", ""Mikoto Kukol'nyy"", ""Mikoto's Red String"", 3, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Yokai Realm Moss",
    @"
case ""Yokai Realm Moss"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""yokairealm"", ""Snake Shikigami"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Infernal Down",
    @"
case ""Infernal Down"":
                    InfernalDown(quant);
                    break;
    "
},
{
    "Arthelyn's Oculus",
    @"
case ""Arthelyn's Oculus"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""infernaldianoia"", ""Fallen Arthelyn"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Life Spirit",
    @"
case ""Life Spirit"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""infernaldianoia"", ""Avatar of Life"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Fishin' Chips",
    @"
case ""Fishin' Chips"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);

                    bool legendDailyDone = !Core.IsMember || Bot.Quests.IsDailyComplete(1684);
                    bool nonLegendDailyDone = Bot.Quests.IsDailyComplete(1683);

                    if (!legendDailyDone)
                        Core.EnsureAccept(1684);
                    if (!nonLegendDailyDone)
                        Core.EnsureAccept(1683);

                    Bot.Events.ExtensionPacketReceived += FishingWaiter;
                    Core.RegisterQuests(1682, 1614, 1615);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Farm.GetBaitandDynamite(0, 20);
                        Core.Join(""fishing"");
                        while (!Bot.ShouldExit && !Bot.Player.Loaded)
                        { int i = 0; Core.Logger($""Waiting for play to load {i++}""); Core.Sleep(); }
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant) && Core.CheckInventory(""Fishing Dynamite""))
                        {
                            Core.Sleep();
                            Bot.Send.Packet(""%xt%zm%FishCast%1%Dynamite%30%"");
                            Core.Logger($""CatchTimer™ Delay: {waitTimer}ms"");
                            Core.Sleep(waitTimer);
                            Bot.Send.Packet(""%xt%zm%getFish%1%false%"");
                        }
                        Bot.Events.ExtensionPacketReceived -= FishingWaiter;
                        waitTimer = 0;

                        // Hunt monsters based on temporary inventory
                        while (!Bot.ShouldExit && Bot.TempInv.Contains(""Fish Caught"", 30))
                            Core.HuntMonster(""greenguardwest"", ""Slime"", log: false);
                        while (!Bot.ShouldExit && Bot.TempInv.Contains(""Endangered Fish"", 5))
                            Core.HuntMonster(""greenguardwest"", ""Frogzard"", log: false);

                        // Complete daily quests if conditions are met
                        if (!legendDailyDone && Bot.Quests.CanCompleteFullCheck(1684))
                        {
                            Core.EnsureComplete(1684);
                            legendDailyDone = true;
                        }
                        if (!nonLegendDailyDone && Bot.Quests.CanCompleteFullCheck(1683))
                        {
                            Core.EnsureComplete(1683);
                            nonLegendDailyDone = true;
                        }

                        Bot.Wait.ForPickup(req.Name);
                    }

                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "castWait",
    @"
case ""castWait"":
                        if (data.wait is not null)
                        {
                            waitTimer = data.wait;
                            Core.Logger($""Derp Moosefish: {data.derp}, Set CatchTimer™: {waitTimer}ms"");
                        }
                        break;


                    //idt this one works
    "
},
{
    "CatchResult",
    @"
case ""CatchResult"":
                        foreach (var c in data.catchResult)
                        {
                            if (c is null || (string)c[""act""] == null || (int)c[""myRep""] == 0)
                                continue;

                            switch ((string)c[""act""])
                            {
                                case ""Miss"":
                                case ""CatchPole"":
                                    Core.Logger($""{(string)c[""act""]}"");
                                    break;
                            }

                            if ((int)c[""myRep""] != 0)
                            {
                                Core.Logger($""{(int)c[""myRep""]}"");
                            }
                        }
                        break;
    "
},
{
    "Icy Token I",
    @"
case ""Icy Token I"":
                    // Glacera.IceDungeon();
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7838);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //Basic Ingredients 7838
                        Core.HuntMonster(""icedungeon"", ""Frosted Banshee"", ""Frosted Banshee Defeated"", 10);
                        Core.HuntMonster(""icedungeon"", ""Frozen Undead"", ""Frozen Undead Defeated"", 10);
                        Core.HuntMonster(""icedungeon"", ""Ice Symbiote"", ""Ice Symbiote Defeated"", 10);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Icy Token II",
    @"
case ""Icy Token II"":
                    // Glacera.IceDungeon();
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7839);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //Cool Flavor 7839
                        Core.HuntMonster(""icedungeon"", ""Spirit of Ice"", ""Spirit of Ice Defeated"", 10);
                        Core.HuntMonster(""icedungeon"", ""Ice Crystal"", ""Ice Crystal Defeated"", 10);
                        Core.HuntMonster(""icedungeon"", ""Frigid Spirit"", ""Frigid Spirit Defeated"", 10);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Icy Token III",
    @"
case ""Icy Token III"":
                    // Glacera.IceDungeon();
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7840);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //Chilled to Perfection 7840
                        Core.HuntMonster(""icedungeon"", ""Living Ice"", ""Living Ice Defeated"", 5);
                        Core.HuntMonster(""icedungeon"", ""Crystallized Elemental"", ""Crystallized Elemental Defeated"", 5);
                        Core.HuntMonster(""icedungeon"", ""Frozen Demon"", ""Frozen Demon Defeated"", 5);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Icy Token IV",
    @"
case ""Icy Token IV"":
                    // Glacera.IceDungeon();
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(7841);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //Icing on the Cake 7841
                        Core.HuntMonster(""icedungeon"", ""Image of Glace"", ""Glace's Approval"");
                        Core.HuntMonster(""icedungeon"", ""Abel"", ""Abel's Approval"");
                        Core.HuntMonster(""icedungeon"", ""Shade of Kyanos"", ""Kyanos' Approval"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Warrior of Kyanos",
    @"
case ""Warrior of Kyanos"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""icedungeon"", ""Shade of Kyanos"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Glacial Envoy's Helm",
    @"
case ""Glacial Envoy's Helm"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""icedungeon"", ""Abel"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Glacial Envoy's Wrap",
    @"
case ""Glacial Envoy's Wrap"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""icedungeon"", ""Abel"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Warrior of Kyanos Daggers",
    @"
case ""Warrior of Kyanos Daggers"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""icedungeon"", ""Abel"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Glacial Portal",
    @"
case ""Glacial Portal"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""icedungeon"", ""Image of Glace"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Floating Glacial Shards Mace",
    @"
case ""Floating Glacial Shards Mace"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""icedungeon"", ""Image of Glace"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Glacial Envoy's Buzzcut",
    @"
case ""Glacial Envoy's Buzzcut"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""icedungeon"", ""Image of Glace"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Glacial Envoy's Locks",
    @"
case ""Glacial Envoy's Locks"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""icedungeon"", ""Image of Glace"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Jade Box Trinket",
    @"
case ""Jade Box Trinket"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(1593);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""akiba"", ""Shadow Nukemichi"", ""Jade Box"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Jade Box Jewel",
    @"
case ""Jade Box Jewel"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(1593);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""akiba"", ""Shadow Nukemichi"", ""Jade Box"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Jade Box Heirloom",
    @"
case ""Jade Box Heirloom"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(1593);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""akiba"", ""Shadow Nukemichi"", ""Jade Box"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Spectral Memento",
    @"
case ""Spectral Memento"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(9954,
                        (""laewed"", LW.UseableMonsters[2], ClassType.Farm),
                        (""laewed"", LW.UseableMonsters[3], ClassType.Farm),
                        (""laewed"", LW.UseableMonsters[5], ClassType.Solo));
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Golden Catalyst",
    @"
case ""Golden Catalyst"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9815);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""liatarahill"", ""Undead Garde"", ""Garde's Brooch"", 9, log: false);
                        Core.HuntMonster(""liatarahill"", ""Garde Wraith"", ""Ghost Blossoms"", 9, log: false);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""liatarahill"", ""Warden Illaria"", ""Illaria's Amulet"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Plasma Orb",
    @"
case ""Plasma Orb"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""liatarahill"", ""Warden Illaria"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Drained Skye Obelisk",
    @"
case ""Drained Skye Obelisk"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""liatarahill"", ""Warden Illaria"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Tainted Blade of Na'al",
    @"
case ""Tainted Blade of Na'al"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.HuntMonster(""infernalarena"", ""Na'al"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Champion's Seal",
    @"
case ""Champion's Seal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.HuntMonster(""infernalarena"", ""Na'al"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Tainted Dagger of Na'al",
    @"
case ""Tainted Dagger of Na'al"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.HuntMonster(""infernalarena"", ""Na'al"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Cervus Dente",
    @"
case ""Cervus Dente"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.HuntMonster(""infernalarena"", ""Cervus Malus"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Infernal Krampus' Claw",
    @"
case ""Infernal Krampus' Claw"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""infernalarena"", ""Infernal Krampus"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Infernal Emblem",
    @"
case ""Infernal Emblem"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""infernalarena"", ""Infernal Krampus"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Axe of the Infernal Defiler",
    @"
case ""Axe of the Infernal Defiler"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""infernalarena"", ""Destructive Defiler"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Infernal Incantation",
    @"
case ""Infernal Incantation"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.HuntMonster(""infernalarena"", ""Key of Sholemoh"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Scythe Shard",
    @"
case ""Scythe Shard"":
                    Core.DodgeClass(""Lord Of Order"");
                    Core.HuntMonster(""infernalarena"", ""Azalith's Scythe"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Duo's Dinner",
    @"
case ""Duo's Dinner"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.BossClass();
                    Core.HuntMonster(""infernalarena"", ""Deadly Duo"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Infernal Badge",
    @"
case ""Infernal Badge"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""infernalarena"", ""Infernal Mage"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Balemorale Crest",
    @"
case ""Balemorale Crest"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""balemorale"", ""r2"", ""Left"", ""*"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Lothian's Lightning",
    @"
case ""Lothian's Lightning"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.Logger($""{req.Name} requires an ultra boss, you need to farm it manually."");
                    break;
    "
},
{
    "Dark Thunder Master Locks",
    @"
case ""Dark Thunder Master Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.Logger($""{req.Name} requires an ultra boss, you need to farm it manually."");
                    break;
    "
},
{
    "Dark Lightning Gloria",
    @"
case ""Dark Lightning Gloria"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.Logger($""{req.Name} requires an ultra boss, you need to farm it manually."");
                    break;
    "
},
{
    "Skye Nobility Sash",
    @"
case ""Skye Nobility Sash"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.Logger($""{req.Name} requires an ultra boss, you need to farm it manually."");
                    break;
    "
},
{
    "Priestess Eire's Cletiné",
    @"
case ""Priestess Eire's Cletiné"":
                    Core.FarmingLogger(req.Name, quant);
                    AOR.ColdThunderBoss(req.Name, quant, false);
                    break;
    "
},
{
    "Skye Warden of the East",
    @"
case ""Skye Warden of the East"":
                    Core.FarmingLogger(req.Name, quant);
                    FGGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Skye Warden of the West",
    @"
case ""Skye Warden of the West"":
                    Core.FarmingLogger(req.Name, quant);
                    LLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Skye Warden of the South",
    @"
case ""Skye Warden of the South"":
                    Core.FarmingLogger(req.Name, quant);
                    LTHLM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Queen Iona's Royal Attire",
    @"
case ""Queen Iona's Royal Attire"":
                    Core.FarmingLogger(req.Name, quant);
                    CTM.BuyAllMerge(req.Name);
                    break;

    "
},
{
    "Speirling Dagger",
    @"
case ""Speirling Dagger"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""loughshine"", ""Skye Executor"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Speirling Daggers",
    @"
case ""Speirling Daggers"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""loughshine"", ""Skye Executor"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Skye Executor Hooded Locks",
    @"
case ""Skye Executor Hooded Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""loughshine"", ""Skye Executor"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Skye Executor's Cloak",
    @"
case ""Skye Executor's Cloak"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""loughshine"", ""Skye Executor"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Solid Gold Alloy",
    @"
case ""Solid Gold Alloy"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9765);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""loughshine"", ""Scorched Elder Yew"", ""Yew Root"", 100, log: false);
                        Core.HuntMonster(""loughshine"", ""Energy Elemental"", ""Ion Particles"", 60, log: false);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""loughshine"", ""Warden Iseul"", ""Gold Pendant"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Mehensi Fang",
    @"
case ""Mehensi Fang"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""whitehole"", ""Mehensi Serpent"", req.Name, quant, isTemp: false);
                    break;

    "
},
{
    "Water Elf Pearl",
    @"
case ""Water Elf Pearl"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9302);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""midnightzone"", ""Sparagmos"", ""Memory Card"", log: false);
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""midnightzone"", ""Shadow Viscera"", ""Fleshy Shadows"", 8, log: false);
                        Core.HuntMonster(""midnightzone"", ""Venerated Wraith"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Mirror Realm Token",
    @"
case ""Mirror Realm Token"":
                    LoC.Xiang();
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""mirrorportal"", 1, req.Name, quant, false);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Undead Paladin Token",
    @"
case ""Undead Paladin Token"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""OverWorld"", ""Undead Artix"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Chaos Shifter",
    @"
case ""Chaos Shifter"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""mountdoomskull"", 776, req.Name);
                    break;
    "
},
{
    "Purification Orb",
    @"
case ""Purification Orb"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""Doomwood"", ""Undead Paladin"", req.Name, quant, isTemp: false);
                    break;

    "
},
{
    "Molten Core",
    @"
case ""Molten Core"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""battleundere"", ""Lava Guard"", req.Name, quant, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Crystamorphosis",
    @"
case ""Crystamorphosis"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""battleundere"", ""r9"", ""Left"", ""*"", req.Name, quant, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Crystamorph Helm",
    @"
case ""Crystamorph Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""battleundere"", ""r9"", ""Left"", ""*"", req.Name, quant, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Crystamorph Eyes",
    @"
case ""Crystamorph Eyes"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""battleundere"", ""r9"", ""Left"", ""*"", req.Name, quant, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Crystamorph Visor",
    @"
case ""Crystamorph Visor"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""battleundere"", ""r9"", ""Left"", ""*"", req.Name, quant, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Crystamorph Wings",
    @"
case ""Crystamorph Wings"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""battleundere"", ""r9"", ""Left"", ""*"", req.Name, quant, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Crystamorph Stinger",
    @"
case ""Crystamorph Stinger"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""battleundere"", ""r9"", ""Left"", ""*"", req.Name, quant, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Crystamorph Venom",
    @"
case ""Crystamorph Venom"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""battleundere"", ""r9"", ""Left"", ""*"", req.Name, quant, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Crystamorph Dual Banes",
    @"
case ""Crystamorph Dual Banes"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""battleundere"", ""r9"", ""Left"", ""*"", req.Name, quant, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Apocalyptic Nihil Coin",
    @"
case ""Apocalyptic Nihil Coin"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(9929, ""monaghangorge"", ""Trickster Duartaine"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Trickster's Apprentice",
    @"
case ""Trickster's Apprentice"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""monaghangorge"", ""Trickster Duartaine"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Dark Omen Trickster Morph",
    @"
case ""Dark Omen Trickster Morph"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""monaghangorge"", ""Trickster Duartaine"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Dark Omen Trickster Hair",
    @"
case ""Dark Omen Trickster Hair"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""monaghangorge"", ""Trickster Duartaine"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Dark Omen Trickster Visage",
    @"
case ""Dark Omen Trickster Visage"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""monaghangorge"", ""Trickster Duartaine"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Dark Omen Trickster Locks",
    @"
case ""Dark Omen Trickster Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""monaghangorge"", ""Trickster Duartaine"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Dark Omen Cards",
    @"
case ""Dark Omen Cards"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""monaghangorge"", ""Trickster Duartaine"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Dark Omen Card Familiar",
    @"
case ""Dark Omen Card Familiar"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""monaghangorge"", ""Trickster Duartaine"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Trickster's Dark Omen",
    @"
case ""Trickster's Dark Omen"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""monaghangorge"", ""Trickster Duartaine"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Dark Omen Barb",
    @"
case ""Dark Omen Barb"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""monaghangorge"", ""Trickster Duartaine"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Dark Omen Barbs",
    @"
case ""Dark Omen Barbs"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""monaghangorge"", ""Trickster Duartaine"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Dark Omen Trick Card",
    @"
case ""Dark Omen Trick Card"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""monaghangorge"", ""Trickster Duartaine"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Trickster's Hidden Grin",
    @"
case ""Trickster's Hidden Grin"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""monaghangorge"", ""Trickster Duartaine"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Dragonling Bone",
    @"
case ""Dragonling Bone"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""naoisegrave"", ""Dragonling"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Volgritian's Dragon Bone",
    @"
case ""Volgritian's Dragon Bone"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9778);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""naoisegrave"", ""Bone Dragonling"", ""Dragonling Soul"", log: false);
                        Core.HuntMonster(""naoisegrave"", ""Ice Guardian"", ""Cryostone"", log: false);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""naoisegrave"", ""Volgritian"", ""Gold Chain"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Synthetic Viscera",
    @"
case ""Synthetic Viscera"":
                    DarkCarnax.SyntheticViscera(quant);
                    break;
    "
},
{
    "Carnax Essence",
    @"
case ""Carnax Essence"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""aqlesson"", ""Carnax"", req.Name, quant, false);
                    break;
    "
},
{
    "Perfect Orochi Scales",
    @"
case ""Perfect Orochi Scales"":
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""shadowfortress"", ""r12"", ""Bottom"", ""*"", req.Name, quant, false);
                    break;
    "
},
{
    "Energized Aura",
    @"
case ""Energized Aura"":
                    NSOD.EnergizedAura();
                    break;
    "
},
{
    "Abyssal Contract",
    @"
case ""Abyssal Contract"":
                    AF.AbyssalContract();
                    break;
    "
},
{
    "Purified Undead Dragon Essence",
    @"
case ""Purified Undead Dragon Essence"":
                    uBLOD.PurifiedUndeadDragonEssence();
                    break;
    "
},
{
    "Overwhelmed Axe",
    @"
case ""Overwhelmed Axe"":
                    uBLOD.OverwhelmedAxe();
                    break;

    "
},
{
    "Remnant of the Deep",
    @"
case ""Remnant of the Deep"":
                    Core.Logger($""{req.Name}"" + "" requires ultra boss, you need to prefarm it yourself."");
                    break;
    "
},
{
    "Adeptus Relic",
    @"
case ""Adeptus Relic"":
                    Core.Logger($""{req.Name}"" + "" requires ultra boss, you need to prefarm it yourself."");
                    break;
    "
},
{
    "Adeptus Kathooli Hair",
    @"
case ""Adeptus Kathooli Hair"":
                    Core.Logger($""{req.Name}"" + "" requires ultra boss, you need to prefarm it yourself."");
                    break;
    "
},
{
    "Adeptus Kathooli Locks",
    @"
case ""Adeptus Kathooli Locks"":
                    Core.Logger($""{req.Name}"" + "" requires ultra boss, you need to prefarm it yourself."");
                    break;
    "
},
{
    "MindSmasher Blade",
    @"
case ""MindSmasher Blade"":
                    Core.Logger($""{req.Name}"" + "" requires ultra boss, you need to prefarm it yourself."");
                    break;
    "
},
{
    "MindSmasher Blades",
    @"
case ""MindSmasher Blades"":
                    Core.Logger($""{req.Name}"" + "" requires ultra boss, you need to prefarm it yourself."");
                    break;
    "
},
{
    "Psychic Domination Spear",
    @"
case ""Psychic Domination Spear"":
                    Core.Logger($""{req.Name}"" + "" requires ultra boss, you need to prefarm it yourself."");
                    break;
    "
},
{
    "Psychic Domination Spears",
    @"
case ""Psychic Domination Spears"":
                    Core.Logger($""{req.Name}"" + "" requires ultra boss, you need to prefarm it yourself."");
                    break;
    "
},
{
    "Ashray Villager",
    @"
case ""Ashray Villager"":
                    SSM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Undine Defence Director",
    @"
case ""Undine Defence Director"":
                    UCM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Evacuation Protocol Suit",
    @"
case ""Evacuation Protocol Suit"":
                    TZM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Ashray Elf Warden",
    @"
case ""Ashray Elf Warden"":
                    AZM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "DeepWater Drow",
    @"
case ""DeepWater Drow"":
                    TOM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Midnight Glaucus Sage",
    @"
case ""Midnight Glaucus Sage"":
                    SVM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Kathool Acolyte",
    @"
case ""Kathool Acolyte"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""deepchaos"", ""Kathool"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Elden Ruby",
    @"
case ""Elden Ruby"":
                    Core.FarmingLogger(req.Name, quant);
                    Daily.EldenRuby(quant);
                    break;
    "
},
{
    "Compass Rose Skull",
    @"
case ""Compass Rose Skull"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(9894,
(""dracocon"", ""Treasure Pile"", ClassType.Farm),
                    (""battleundere"", ""Treasure Pile"", ClassType.Farm),
                    (""greed"", ""Treasure Pile"", ClassType.Farm)
);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Ancient Astrolabe",
    @"
case ""Ancient Astrolabe"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(8349);
                        Core.HuntMonster(""orbhunt"", ""Chamat"", ""Chamat Defeated"");
                        Core.HuntMonster(""orbhunt"", ""Horothotep"", ""Horothotep Defeated"");
                        Core.HuntMonster(""orbhunt"", ""Kolyaban"", ""Kolyaban Defeated"");
                        Core.HuntMonster(""orbhunt"", ""Quetzal"", ""Quetzal Defeated"");
                        Core.EnsureComplete(8349);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Advanced Weapon Kit",
    @"
case ""Advanced Weapon Kit"":
                    CoreBLOD.AdvancedWK(quant);
                    break;
    "
},
{
    "Ultimate Weapon Kit",
    @"
case ""Ultimate Weapon Kit"":
                    CoreBLOD.UltimateWK(quant: quant);
                    break;
    "
},
{
    "Basic Weapon Kit",
    @"
case ""Basic Weapon Kit"":
                    CoreBLOD.BasicWK(quant);
                    break;
    "
},
{
    "Golden Daimyo Armor",
    @"
case ""Golden Daimyo Armor"":
                    if (!Core.IsMember)
                    {
                        Core.Logger(""Membership Required to start the `Golden Armored Daimyo [2079] Quest."");
                        break;
                    }
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(2079);
                        Core.HuntMonster(""mafic"", ""Living Fire"", ""Heart of Flame "", 15);
                        Core.HuntMonster(""greenguardwest"", ""Black Knight"", ""Black Metal Armor"");
                        Core.EnsureComplete(2079);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Sanctified Silver of Destiny",
    @"
case ""Sanctified Silver of Destiny"":
                    CoreBLOD.UpgradeMetal(MineCraftingMetalsEnum.Silver);
                    break;
    "
},
{
    "Immortal Iron of Destiny",
    @"
case ""Immortal Iron of Destiny"":
                    CoreBLOD.UpgradeMetal(MineCraftingMetalsEnum.Iron);
                    break;
    "
},
{
    "Glorious Gold of Destiny",
    @"
case ""Glorious Gold of Destiny"":
                    CoreBLOD.UpgradeMetal(MineCraftingMetalsEnum.Gold);
                    break;
    "
},
{
    "Celestial Copper of Destiny",
    @"
case ""Celestial Copper of Destiny"":
                    CoreBLOD.UpgradeMetal(MineCraftingMetalsEnum.Copper);
                    break;
    "
},
{
    "Blessed Barium of Destiny",
    @"
case ""Blessed Barium of Destiny"":
                    CoreBLOD.UpgradeMetal(MineCraftingMetalsEnum.Barium);
                    break;
    "
},
{
    "Almighty Aluminum of Destiny",
    @"
case ""Almighty Aluminum of Destiny"":
                    CoreBLOD.UpgradeMetal(MineCraftingMetalsEnum.Aluminum);
                    break;
    "
},
{
    "Twilly Puppy Saddle",
    @"
case ""Twilly Puppy Saddle"":
                    if (!Core.IsMember) // Daimyo
                    {
                        Core.Logger(""Membership Required to buy \""daimyo (pet)\"" for this item."");
                        break;
                    }
                    else Adv.BuyItem(""necropolis"", 422, 152);
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(5132);
                        Core.KillMonster(""castleundead"", ""Enter"", ""Left"", ""Skeletal Warrior"", ""Undead Head"", 10);
                        Core.EnsureComplete(5132, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Twig Puppy Saddle",
    @"
case ""Twig Puppy Saddle"":
                    if (!Core.IsMember) // Daimyo
                    {
                        Core.Logger(""Membership Required to buy \""daimyo (pet)\"" for this item."");
                        break;
                    }
                    else Adv.BuyItem(""necropolis"", 422, 152);
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(5132);
                        Core.KillMonster(""castleundead"", ""Enter"", ""Left"", ""Skeletal Warrior"", ""Undead Head"", 10);
                        Core.EnsureComplete(5132, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Zorbak Puppy Saddle",
    @"
case ""Zorbak Puppy Saddle"":
                    if (!Core.IsMember) // Daimyo
                    {
                        Core.Logger(""Membership Required to buy \""daimyo (pet)\"" for this item."");
                        break;
                    }
                    else Adv.BuyItem(""necropolis"", 422, 152);
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(5132);
                        Core.KillMonster(""castleundead"", ""Enter"", ""Left"", ""Skeletal Warrior"", ""Undead Head"", 10);
                        Core.EnsureComplete(5132, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Fiend Emblem",
    @"
case ""Fiend Emblem"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(7890);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.KillMonster(""originul"", ""r5"", ""Right"", ""*"", ""Essence of The Citadel"", 30);

                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""originul"", ""Fiend Champion"", ""Champion's Essence"");

                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Panopticon Gear Wreckage",
    @"
case ""Panopticon Gear Wreckage"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9730); //C:\The Depths are a Harsh Mistress (9730)
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""trenchobserve"", ""Sea Spirit"", ""Squishy Organic Thingy"", 5, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Panopticon Gear Linker",
    @"
case ""Panopticon Gear Linker"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9730); //C:\The Depths are a Harsh Mistress (9730)
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""trenchobserve"", ""Sea Spirit"", ""Squishy Organic Thingy"", 5, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Paradox Core",
    @"
case ""Paradox Core"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""portalmazec"", ""Vorefax "", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Paradox Gem",
    @"
case ""Paradox Gem"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dragontown"", ""The Neverborn"", req.Name, quant, isTemp: false);
                    break;

    "
},
{
    "Dragon's Plague Scythe",
    @"
case ""Dragon's Plague Scythe"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""sloth"", ""Phlegnn"", req.Name, quant, false);
                    break;
    "
},
{
    "Sloth Gem",
    @"
case ""Sloth Gem"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""sloth"", ""Phlegnn"", req.Name, quant, false);
                    break;
    "
},
{
    "Slime Claw",
    @"
case ""Slime Claw"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""sloth"", ""Phlegnn"", req.Name, quant, false);
                    break;
    "
},
{
    "Slime Fang",
    @"
case ""Slime Fang"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""sloth"", ""Phlegnn"", req.Name, quant, false);
                    break;
    "
},
{
    "Sloth Heart",
    @"
case ""Sloth Heart"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""sloth"", ""Phlegnn"", req.Name, quant, false);
                    break;
    "
},
{
    "Plague Badge",
    @"
case ""Plague Badge"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""sloth"", ""Phlegnn"", req.Name, quant, false);
                    break;
    "
},
{
    "Bloody Claw",
    @"
case ""Bloody Claw"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""sloth"", ""Cured Phlegnn"", req.Name, quant, false);
                    break;
    "
},
{
    "Bloodless Heart",
    @"
case ""Bloodless Heart"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""sloth"", ""Cured Phlegnn"", req.Name, quant, false);
                    break;
    "
},
{
    "Bloody Fang",
    @"
case ""Bloody Fang"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""sloth"", ""Cured Phlegnn"", req.Name, quant, false);
                    break;
    "
},
{
    "Bloody Scale",
    @"
case ""Bloody Scale"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""sloth"", ""Cured Phlegnn"", req.Name, quant, false);
                    break;
    "
},
{
    "Slime Scale",
    @"
case ""Slime Scale"":
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""sloth"", ""r2"", ""Bottom"", ""*"", req.Name, quant, false);
                    break;
    "
},
{
    "Star Piece",
    @"
case ""Star Piece"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""starfield"", ""r3"", ""bottom"", ""*"", req.Name, quant, req.Temp, log: false);
                    break;
    "
},
{
    "Ascended Light of Destiny",
    @"
case ""Ascended Light of Destiny"":
                    ADG.AscendedGear(""Ascended Light of Destiny"");

                    break;
    "
},
{
    "Blackhole Light of Dread Space",
    @"
case ""Blackhole Light of Dread Space"":
                    DRM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Void Light of Destiny",
    @"
case ""Void Light of Destiny"":
                    VP.Sacrifice();
                    break;
    "
},
{
    "Polished Blinding Light of Destiny",
    @"
case ""Polished Blinding Light of Destiny"":
                    CIU.GetPolishedBLoD();
                    break;
    "
},
{
    "Hollowborn Shadow of Fate",
    @"
case ""Hollowborn Shadow of Fate"":
                    CHP.HBShadowOfFate();
                    break;
    "
},
{
    "Obsidian Light of Destiny",
    @"
case ""Obsidian Light of Destiny"":
                    ObsidianLightofDestiny.Axe();
                    break;
    "
},
{
    "Ultimate Blinding Light of Destiny",
    @"
case ""Ultimate Blinding Light of Destiny"":
                    UltimateBLoD.UltimateBlindingLightofDestiny();
                    break;
    "
},
{
    "Sanctified Light of Destiny",
    @"
case ""Sanctified Light of Destiny"":
                    SanctifiedLightofDestiny.GetSanctifiedLightofDestiny();
                    break;
    "
},
{
    "Dark Dragon Slayer's Halberd",
    @"
case ""Dark Dragon Slayer's Halberd"":
                    if (!Bot.Player.IsMember)
                        StreamwarMerge.BuyAllMerge(""Dark Dragon Slayer's Halberd"");
                    else
                        DBoN.GetDragonBlade();

                    Core.BuyItem(""novashrine"", 2458, ""Star Light of Destiny"", 1, !Bot.Player.IsMember ? 13334 : 13333);
                    break;
    "
},
{
    "Star of the Empyrean",
    @"
case ""Star of the Empyrean"":
                    Core.Logger($""Cannot Obtain {req.Name} as its from an \""Ultra\"", and Skua cannot do ultras. Please Wait until InsertCreates/adds this ultra to his Bot Collecetion (and update grim li to 1.5.2 for the newest handler)"");

                    break;
    "
},
{
    "ArchPaladin Armor",
    @"
case ""ArchPaladin Armor"":
                    Adv.BuyItem(""darkthronehub"", 1303, req.Name);
                    break;
    "
},
{
    "Blinding Aura",
    @"
case ""Blinding Aura"":
                    BLOD.BlindingAura(quant);

                    break;
    "
},
{
    "Cosmic Stardust",
    @"
case ""Cosmic Stardust"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9802);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""novashrine"", ""r2"", ""left"", ""Nova Empyrean"", req.Name, quant, req.Temp);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Nova Empyrean Tail",
    @"
case ""Nova Empyrean Tail"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9802);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""novashrine"", ""r2"", ""left"", ""Nova Empyrean"", req.Name, quant, req.Temp);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Ancient Hourglass",
    @"
case ""Ancient Hourglass"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(8326);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""queenreign"", ""Sa-Laatan"", ""Sa-Lataan Defeated"");
                        Core.HuntMonster(""queenreign"", ""Grou'luu"", ""Grou'luu Defeated"");
                        Core.HuntMonster(""queenreign"", ""Extriki"", ""Extriki Defeated"");
                        Core.HuntMonster(""queenreign"", ""Jaaku"", ""Jaaku Defeated"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "LightningLord",
    @"
case ""LightningLord"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""queenreign"", ""Extriki"", req.Name);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "LightningLord Helm",
    @"
case ""LightningLord Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""queenreign"", ""Extriki"", req.Name);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "LightningLord Locks",
    @"
case ""LightningLord Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""queenreign"", ""Extriki"", req.Name);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "LightningLord Rune",
    @"
case ""LightningLord Rune"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""queenreign"", ""Extriki"", req.Name);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Green Dancin' Feathers Merge",
    @"
case ""Green Dancin' Feathers Merge"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(1180);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""bloodtusk"", ""Horc Boar Scout"", ""Boar Bristles"", 5);
                        Core.HuntMonster(""bloodtusk"", ""Horc Boar Scout"", ""Boar Bone"");
                        Core.HuntMonster(""bloodtusk"", ""Jungle Vulture"", ""Dyed Feathers"", 10);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Samba Hair",
    @"
case ""Samba Hair"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(1180);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""bloodtusk"", ""Horc Boar Scout"", ""Boar Bristles"", 5);
                        Core.HuntMonster(""bloodtusk"", ""Horc Boar Scout"", ""Boar Bone"");
                        Core.HuntMonster(""bloodtusk"", ""Jungle Vulture"", ""Dyed Feathers"", 10);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Purple Dancin' Feathers Merge",
    @"
case ""Purple Dancin' Feathers Merge"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(1181);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""bloodtusk"", ""Horc Boar Scout"", ""Bolt of Cloth"", 3);
                        Core.HuntMonster(""bloodtusk"", ""Rhison"", ""Shiny Metal"", 3);
                        Core.GetMapItem(46408, 10, ""bloodtusk"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Samba Outfit!",
    @"
case ""Samba Outfit!"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(1181);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""bloodtusk"", ""Horc Boar Scout"", ""Bolt of Cloth"", 3);
                        Core.HuntMonster(""bloodtusk"", ""Rhison"", ""Shiny Metal"", 3);
                        Core.GetMapItem(46408, 10, ""bloodtusk"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Celestial Seal",
    @"
case ""Celestial Seal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""goldenarena"", ""Blessed Dragon"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Golden Scale",
    @"
case ""Golden Scale"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""goldenarena"", ""Blessed Dragon"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Golden Badge",
    @"
case ""Golden Badge"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""goldenarena"", ""Blessed Inquisitor"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Golden Wing",
    @"
case ""Golden Wing"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""goldenarena"", ""Blessed Gladius"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Golden Rune",
    @"
case ""Golden Rune"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""goldenarena"", ""Blessed Karok"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Laurel Crown",
    @"
case ""Laurel Crown"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""goldenarena"", ""Queen of Hope"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Divine Down",
    @"
case ""Divine Down"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(10082,
                            (""infernalparadise"", ""Akh-a"", ClassType.Solo),
                            (""infernalparadise"", ""Azalith"", ClassType.Solo),
                            (""infernalparadise"", ""Infernal Knight"", ClassType.Farm)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Infernal Mage's Incantation",
    @"
case ""Infernal Mage's Incantation"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""infernalparadise"", ""Infernal Mage"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Malxas' Shed Feather",
    @"
case ""Malxas' Shed Feather"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""infernalparadise"", ""Infernal Malxas"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Sur-gion Token",
    @"
case ""Sur-gion Token"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9235);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ashray"", ""Kitefin Shark Bait"", ""Shark Fin"", 7, log: false);
                        Core.HuntMonster(""ashray"", ""Ashray Fisherman"", ""Ashray Blood Sample"", 7, log: false);
                        Core.HuntMonster(""ashray"", ""Seafoam Elemental"", ""Seafoam Bubbles"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Severed Tentacle",
    @"
case ""Severed Tentacle"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(8362);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""queenbattle"", ""Proto Chaos Champion"", ""Proto Chaos Champion Redefeated"", log: false);
                        Core.HuntMonster(""queenbattle"", ""Queen of Monsters"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "1st Hero of Balance",
    @"
case ""1st Hero of Balance"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""queenbattle"", ""Queen of Monsters"", req.Name, quant, false, false);
                    break;
    "
},
{
    "1st Hero of Balance Hood",
    @"
case ""1st Hero of Balance Hood"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""queenbattle"", ""Queen of Monsters"", req.Name, quant, false, false);
                    break;
    "
},
{
    "1st Hero of Balance Cloak",
    @"
case ""1st Hero of Balance Cloak"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""queenbattle"", ""Queen of Monsters"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Good Hero of Balance",
    @"
case ""Good Hero of Balance"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""queenbattle"", ""Queen of Monsters"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Good Hero of Balance Morph",
    @"
case ""Good Hero of Balance Morph"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""queenbattle"", ""Queen of Monsters"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Evil Hero of Balance",
    @"
case ""Evil Hero of Balance"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""queenbattle"", ""Queen of Monsters"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Evil Hero of Balance Morph",
    @"
case ""Evil Hero of Balance Morph"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""queenbattle"", ""Queen of Monsters"", req.Name, quant, false, false);
                    break;
    "
},
{
    "3rd Hero of Balance",
    @"
case ""3rd Hero of Balance"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""queenbattle"", ""Queen of Monsters"", req.Name, quant, false, false);
                    break;
    "
},
{
    "3rd Hero of Balance Locks",
    @"
case ""3rd Hero of Balance Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""queenbattle"", ""Queen of Monsters"", req.Name, quant, false, false);
                    break;
    "
},
{
    "3rd Hero of Balance Scarf",
    @"
case ""3rd Hero of Balance Scarf"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""queenbattle"", ""Queen of Monsters"", req.Name, quant, false, false);
                    break;
    "
},
{
    "3rd Hero of Balance Daggers",
    @"
case ""3rd Hero of Balance Daggers"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""queenbattle"", ""Queen of Monsters"", req.Name, quant, false, false);
                    break;
    "
},
{
    "3rd Hero of Balance Dirk",
    @"
case ""3rd Hero of Balance Dirk"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""queenbattle"", ""Queen of Monsters"", req.Name, quant, false, false);
                    break;
    "
},
{
    "4th Hero of Balance",
    @"
case ""4th Hero of Balance"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""queenbattle"", ""Queen of Monsters"", req.Name, quant, false, false);
                    break;
    "
},
{
    "4th Hero of Balance Cloak",
    @"
case ""4th Hero of Balance Cloak"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""queenbattle"", ""Queen of Monsters"", req.Name, quant, false, false);
                    break;
    "
},
{
    "5th Hero of Balance",
    @"
case ""5th Hero of Balance"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""queenbattle"", ""Queen of Monsters"", req.Name, quant, false, false);
                    break;
    "
},
{
    "5th Hero of Balance Morph",
    @"
case ""5th Hero of Balance Morph"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""queenbattle"", ""Queen of Monsters"", req.Name, quant, false, false);
                    break;
    "
},
{
    "5th Hero of Balance Wings",
    @"
case ""5th Hero of Balance Wings"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""queenbattle"", ""Queen of Monsters"", req.Name, quant, false, false);
                    break;
    "
},
{
    "7th Hero of Balance",
    @"
case ""7th Hero of Balance"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""queenbattle"", ""Queen of Monsters"", req.Name, quant, false, false);
                    break;
    "
},
{
    "7th Hero of Balance Morph",
    @"
case ""7th Hero of Balance Morph"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""queenbattle"", ""Queen of Monsters"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Blood Token",
    @"
case ""Blood Token"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(6246, 6247);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""seraphicwarlaken"", ""Enter"", ""Spawn"", ""*"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Dark Token",
    @"
case ""Dark Token"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(6248, 6249);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""seraphicwardage"", ""Enter"", ""Spawn"", ""*"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Seraphic Paladin Shield",
    @"
case ""Seraphic Paladin Shield"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""seraphicwardage"", ""Supercharged Laken"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Seraphic Paladin Wings",
    @"
case ""Seraphic Paladin Wings"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""seraphicwardage"", ""Supercharged Laken"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Dark Heart Medal",
    @"
case ""Dark Heart Medal"":
                    ShadowGates.StoryLine();
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(3294);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //Gravelyn's Dark Rewards 3294
                        Core.HuntMonster(""Shadowfall"", ""Skeletal Knight"", ""Infected Skull"", 7);
                        Core.HuntMonster(""ShadowGates"", ""Chaos Warrior"", ""Chaorrupted Bones"", 6);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Shade Spark",
    @"
case ""Shade Spark"":
                    SoW.Tyndarius();
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8145);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //Catching Fire 8145
                        Core.HuntMonster(""shadowfireplane"", ""Living Shadowflame"", ""Shadefire Essence"", 20);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Shadow Shield",
    @"
case ""Shadow Shield"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Daily.DailyRoutine(3828, ""lightguardwar"", ""Citadel Crusader"", ""Broken Blade"");
                    if (Core.IsMember)
                        Daily.DailyRoutine(3827, ""lightguardwar"", ""Citadel Crusader"", ""Broken Blade"");
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""lightguardwar"", ""Sigrid Sunshield"", req.Name, quant);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Bioluminessence",
    @"
case ""Bioluminessence"":
                    Core.FarmingLogger(req.Name, quant);
                    AttackVoiceInTheSea(req.Name, quant);
                    break;
    "
},
{
    "Calamity Atlanticus Trident",
    @"
case ""Calamity Atlanticus Trident"":
                    Core.FarmingLogger(req.Name, quant);
                    AttackVoiceInTheSea(req.Name, quant);
                    break;
    "
},
{
    "Glaucus Mystic",
    @"
case ""Glaucus Mystic"":
                    Core.FarmingLogger(req.Name, quant);
                    AttackVoiceInTheSea(req.Name, quant);
                    break;
    "
},
{
    "Glaucus Companion",
    @"
case ""Glaucus Companion"":
                    Core.FarmingLogger(req.Name, quant);
                    AttackVoiceInTheSea(req.Name, quant);
                    break;
    "
},
{
    "Dark Elf Pearl",
    @"
case ""Dark Elf Pearl"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9339);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""trenchobserve"", ""Lady Noelle"", ""Noelle's Brooch"", log: false);
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""trenchobserve"", ""Sea Spirit"", ""Green Sea Jelly"", 2, log: false);
                        Core.HuntMonster(""trenchobserve"", ""Necro Adipocere"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Sundered Tentacle",
    @"
case ""Sundered Tentacle"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9269);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""twilightzone"", ""Leviathan"", ""Leviathan Tentacle"", 1, true, false);
                        Core.HuntMonster(""twilightzone"", ""Decay Spirit"", ""Decay Essence"", 8, true, false);
                        Core.HuntMonster(""twilightzone"", ""Ice Guardian"", ""Tarnished Icicle"", 8, true, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Oxidize",
    @"
case ""Oxidize"":
                        while (!Bot.ShouldExit && !Bot.Self.HasActiveAura(""Vigil""))
                        {
                            UsePotion();
                            Core.Sleep();

                            // Check if targetAura is not null before accessing its SecondsRemaining() method
                            // Assuming `targetAura` is the aura you're referring to
                            if (Bot.Self.HasActiveAura(""Vigil""))
                            {
                                Core.Logger($""Vigil Active!"");
                                break;
                            }
                        }
                        break;

                    case null:
                        break;
    "
},
{
    "Ninjo",
    @"
case ""Ninjo"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Shink.Storyline();
                    Core.RegisterQuests(8124);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""Shinkansen"", ""Crystallis Soldier"", ""Favor Done"", 30);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Enchanted Gauntlet Leather",
    @"
case ""Enchanted Gauntlet Leather"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(4429);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""shipwreck"", ""Gilded Water"", ""Lifeless Water"", 14);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Anti-Au Crystals",
    @"
case ""Anti-Au Crystals"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(4430);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""shipwreck"", ""Gilded Crystal Undead"", ""Crystal Crew Shards"", 8);
                        Core.HuntMonster(""shipwreck"", ""Captain Nubar"", ""Pirate Pistols"", 8);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Sidhe's Silk",
    @"
case ""Sidhe's Silk"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9746);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""castleeblana"", ""Leanan Sidhe"", ""Glassy Wings"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Leanan Sidhe's Butterflies",
    @"
case ""Leanan Sidhe's Butterflies"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""castleeblana"", ""Leanan Sidhe"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Especially Unbroken Skull",
    @"
case ""Especially Unbroken Skull"":
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(!Core.IsMember ? 8411 : 8412);
                    Core.Logger($""Farming {req.Name} ({currentQuant}/{quant})"");
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""warundead"", ""r3"", ""Left"", ""*"", ""Unbroken Skulls"", 100);
                        Core.HuntMonster(""warundead"", ""Summon Lich"", ""Summon Lich's Orb"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Silver Exalted Paladin",
    @"
case ""Silver Exalted Paladin"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(7586);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""warhorc"", ""General Drox"", ""Paladin Armor Found"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Ancient Alloy",
    @"
case ""Ancient Alloy"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.Name);
                    Core.RegisterQuests(7587);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""shadowvault"", ""Shadowstryke"", ""Alloy Materials"", quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Silver Exalted Winged Visor",
    @"
case ""Silver Exalted Winged Visor"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7582);
                        Core.HuntMonster(""frozentower"", ""FrostDeep Dweller"", ""Paladin Helmet Wings"");
                        Core.EnsureComplete(7582, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Silver Exalted Winged Helm",
    @"
case ""Silver Exalted Winged Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7582);
                        Core.HuntMonster(""frozentower"", ""FrostDeep Dweller"", ""Paladin Helmet Wings"");
                        Core.EnsureComplete(7582, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Silver Exalted Visor",
    @"
case ""Silver Exalted Visor"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7581);
                        Core.HuntMonster(""ectocave"", ""Ichor Dracolich"", ""Sticky Paladin Helm"");
                        Core.EnsureComplete(7581, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Silver Exalted Helmet",
    @"
case ""Silver Exalted Helmet"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7581);
                        Core.HuntMonster(""ectocave"", ""Ichor Dracolich"", ""Sticky Paladin Helm"");
                        Core.EnsureComplete(7581, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Silver Exalted Haloed Wings",
    @"
case ""Silver Exalted Haloed Wings"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(7583);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""thirdspell"", ""Great Solar Elemental"", ""Wings Found"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Silver Exalted Spears of Light",
    @"
case ""Silver Exalted Spears of Light"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7584);
                        Core.HuntMonster(""table"", ""Roach"", ""Paladin Polearm Found"");
                        Core.EnsureComplete(7584, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Silver Exalted Paladin Poleaxe",
    @"
case ""Silver Exalted Paladin Poleaxe"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7584);
                        Core.HuntMonster(""table"", ""Roach"", ""Paladin Polearm Found"");
                        Core.EnsureComplete(7584, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Silver Exalted Paladin Spear",
    @"
case ""Silver Exalted Paladin Spear"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7584);
                        Core.HuntMonster(""table"", ""Roach"", ""Paladin Polearm Found"");
                        Core.EnsureComplete(7584, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Silver Exalted Paladin Axe",
    @"
case ""Silver Exalted Paladin Axe"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7585);
                        Core.HuntMonster(""dracocon"", ""Singer"", ""Paladin Weapon Found"");
                        Core.EnsureComplete(7585, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Silver Exalted Paladin Blade",
    @"
case ""Silver Exalted Paladin Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7585);
                        Core.HuntMonster(""dracocon"", ""Singer"", ""Paladin Weapon Found"");
                        Core.EnsureComplete(7585, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Undine Visitor Badge",
    @"
case ""Undine Visitor Badge"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.HuntMonster(""sunlightzone"", ""Astravian Illusion"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Leviathan Scale",
    @"
case ""Leviathan Scale"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.Logger(""Better to use alts to farm it faster."");
                    Core.HuntMonster(""twilightzone"", ""Leviathan"", ""Leviathan Scale"", quant, false, false);
                    break;
    "
},
{
    "Undine Coffee Table",
    @"
case ""Undine Coffee Table"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""midnightzone"", ""Sparagmos"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Sleeping Monitor",
    @"
case ""Sleeping Monitor"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""midnightzone"", ""Sparagmos"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Water Temple Pedestal",
    @"
case ""Water Temple Pedestal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""midnightzone"", ""Sparagmos"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Scattered Bones",
    @"
case ""Scattered Bones"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""midnightzone"", ""Undead Prisoner"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Experimentation Chair",
    @"
case ""Experimentation Chair"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""midnightzone"", ""Undead Prisoner"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Ascending Kathool Tentacle",
    @"
case ""Ascending Kathool Tentacle"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""abyssalzone"", ""The Ashray"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Ashray Trench Pedestal",
    @"
case ""Ashray Trench Pedestal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""trenchobserve"", ""Lady Noelle"", req.Name, quant, false, false);
                    break;


    "
},
{
    "Apprentice of the Light",
    @"
case ""Apprentice of the Light"":
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.FarmingLogger(req.Name, req.Quantity); 
                    Core.RegisterQuests(6560, 6561);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, req.Quantity))
                        Core.KillMonster(""lightguardwar"", ""r2"", ""Left"", ""Citadel Crusader"");
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Furred Ruff of the Light",
    @"
case ""Furred Ruff of the Light"":
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.FarmingLogger(req.Name, req.Quantity); 
                    Core.RegisterQuests(6560, 6561);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, req.Quantity))
                        Core.KillMonster(""lightguardwar"", ""r2"", ""Left"", ""Citadel Crusader"");
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Apprentice of the Light Hair",
    @"
case ""Apprentice of the Light Hair"":
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.FarmingLogger(req.Name, req.Quantity); 
                    Core.RegisterQuests(6560, 6561);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, req.Quantity))
                        Core.KillMonster(""lightguardwar"", ""r2"", ""Left"", ""Citadel Crusader"");
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Apprentice of the Light Locks",
    @"
case ""Apprentice of the Light Locks"":
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.FarmingLogger(req.Name, req.Quantity); 
                    Core.RegisterQuests(6560, 6561);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, req.Quantity))
                        Core.KillMonster(""lightguardwar"", ""r2"", ""Left"", ""Citadel Crusader"");
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Citadel's Light Blade",
    @"
case ""Citadel's Light Blade"":
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.FarmingLogger(req.Name, req.Quantity); 
                    Core.RegisterQuests(6560, 6561);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, req.Quantity))
                        Core.KillMonster(""lightguardwar"", ""r2"", ""Left"", ""Citadel Crusader"");
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Medal of Light",
    @"
case ""Medal of Light"":
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.FarmingLogger(req.Name, req.Quantity); 
                    Core.RegisterQuests(6560, 6561);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, req.Quantity))
                        Core.KillMonster(""lightguardwar"", ""r2"", ""Left"", ""Citadel Crusader"");
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Medal of Honor",
    @"
case ""Medal of Honor"":
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.FarmingLogger(req.Name, req.Quantity);
                    Core.RegisterQuests(6562, 6563);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, req.Quantity))
                        Core.KillMonster(""lightguardwar"", ""r2"", ""Left"", ""Citadel Crusader"");
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Medal of Justice",
    @"
case ""Medal of Justice"":
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.FarmingLogger(req.Name, req.Quantity);
                    Core.RegisterQuests(6566);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, req.Quantity))
                        Core.KillMonster(""lightguardwar"", ""r2"", ""Left"", ""Citadel Crusader"");
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Pristine Deepsea Pearl",
    @"
case ""Pristine Deepsea Pearl"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9718);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""ashray"", ""Enter"", ""Spawn"", ""*"", ""Deepsea Pearls"", 10, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Deepdark Pearl",
    @"
case ""Deepdark Pearl"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9715);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""midnightzone"", ""Shadow Viscera"", ""Viscera Sample"", 10, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Electrojolt Scholar",
    @"
case ""Electrojolt Scholar"":
                    Adv.BuyItem(""balemorale"", 2443, req.Name, quant);
                    break;
    "
},
{
    "Royal Electrojolt Scholar",
    @"
case ""Royal Electrojolt Scholar"":
                    Adv.BuyItem(""balemorale"", 2443, req.Name, quant);
                    break;
    "
},
{
    "Tattered Court Mage Robe",
    @"
case ""Tattered Court Mage Robe"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""balemorale"", ""Chaos Crystal"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Victoria's Fletching",
    @"
case ""Victoria's Fletching"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""balemorale"", ""Queen Victoria"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Spirit Ward Sigil",
    @"
case ""Spirit Ward Sigil"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(1695); // Spirit Ward Sigil (1695)
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""bludrut4"", ""Groglurk"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Screamwave",
    @"
case ""Screamwave"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(1694); // Screamwave (1694)
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""bludrut3"", ""Siren"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Ectoamber",
    @"
case ""Ectoamber"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(1693);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""bludrut"", ""Rattlebones"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Tonitrus Gem",
    @"
case ""Tonitrus Gem"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    if (!Core.CheckInventory(Core.QuestRewards(4243)))
                    {
                        Core.AddDrop(Core.QuestRewards(4243));
                        Core.EnsureAccept(4243);
                        Core.HuntMonster(""thunderfang"", ""Energy Elemental"", ""Gem Found"");
                        Core.EnsureComplete(4243);
                    }
                    // Core.RegisterQuests(4246);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(4246);
                        Core.HuntMonster(""thunderfang"", ""Storm Draconian"", ""Storm Draconian Defeated"", 8);
                        Core.EnsureComplete(4246);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    // Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Dragon Crystal",
    @"
case ""Dragon Crystal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(4549);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //Gather Energy Beans 4549
                        Core.GetMapItem(3760, 4, ""DragonRoad"");
                        Core.HuntMonster(""DragonRoad"", ""Desert Wolf Bandit"", ""Energy Bean"", 3, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Anqa's Feather",
    @"
case ""Anqa's Feather"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    //incase u get teh otehr drops along the way vvv
                    Core.AddDrop(new[] { ""Glowing Ember"", ""Golden Firebird's Spear"", ""Golden Firebird's Blade"", ""Golden Firebird's Blades"", ""Miniature Phoenix Guest"" });
                    Core.RegisterQuests(9752);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""sunsetdunes"", ""Firebird Anqa"", req.Name, quant, req.Temp);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Glowing Ember",
    @"
case ""Glowing Ember"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    //incase u get teh otehr drops along the way vvv
                    Core.AddDrop(new[] { ""Glowing Ember"", ""Golden Firebird's Spear"", ""Golden Firebird's Blade"", ""Golden Firebird's Blades"", ""Miniature Phoenix Guest"" });
                    Core.RegisterQuests(9752);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""sunsetdunes"", ""Firebird Anqa"", req.Name, quant, req.Temp);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Golden Firebird's Spear",
    @"
case ""Golden Firebird's Spear"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    //incase u get teh otehr drops along the way vvv
                    Core.AddDrop(new[] { ""Glowing Ember"", ""Golden Firebird's Spear"", ""Golden Firebird's Blade"", ""Golden Firebird's Blades"", ""Miniature Phoenix Guest"" });
                    Core.RegisterQuests(9752);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""sunsetdunes"", ""Firebird Anqa"", req.Name, quant, req.Temp);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Golden Firebird's Blade",
    @"
case ""Golden Firebird's Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    //incase u get teh otehr drops along the way vvv
                    Core.AddDrop(new[] { ""Glowing Ember"", ""Golden Firebird's Spear"", ""Golden Firebird's Blade"", ""Golden Firebird's Blades"", ""Miniature Phoenix Guest"" });
                    Core.RegisterQuests(9752);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""sunsetdunes"", ""Firebird Anqa"", req.Name, quant, req.Temp);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Golden Firebird's Blades",
    @"
case ""Golden Firebird's Blades"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    //incase u get teh otehr drops along the way vvv
                    Core.AddDrop(new[] { ""Glowing Ember"", ""Golden Firebird's Spear"", ""Golden Firebird's Blade"", ""Golden Firebird's Blades"", ""Miniature Phoenix Guest"" });
                    Core.RegisterQuests(9752);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""sunsetdunes"", ""Firebird Anqa"", req.Name, quant, req.Temp);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Miniature Phoenix Guest",
    @"
case ""Miniature Phoenix Guest"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    //incase u get teh otehr drops along the way vvv
                    Core.AddDrop(new[] { ""Glowing Ember"", ""Golden Firebird's Spear"", ""Golden Firebird's Blade"", ""Golden Firebird's Blades"", ""Miniature Phoenix Guest"" });
                    Core.RegisterQuests(9752);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""sunsetdunes"", ""Firebird Anqa"", req.Name, quant, req.Temp);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Fame Token",
    @"
case ""Fame Token"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8033);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""superdeath"", ""Cave Yeti"", ""Normal Monsters Defeated"", 5);
                        Core.HuntMonster(""superdeath"", ""Shadow Mutant"", ""Shadow Monsters Defeated"", 5);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Yergen's HeroSmash Trophy",
    @"
case ""Yergen's HeroSmash Trophy"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""superdeath"", ""Super Death"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Martial Artist's Gi",
    @"
case ""Martial Artist's Gi"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""dragonkoi"", ""Ryoku"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Ryoku's Spikes",
    @"
case ""Ryoku's Spikes"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""dragonkoi"", ""Ryoku"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Fatal Lily",
    @"
case ""Fatal Lily"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""superslayin"", ""Newb Cybot"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Pockey Ball",
    @"
case ""Pockey Ball"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""superslayin"", ""Charidon"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Dragon Orb",
    @"
case ""Dragon Orb"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""superslayin"", ""Super Ryoku"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Master's Gi",
    @"
case ""Master's Gi"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""superslayin"", ""Super Ryoku"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Super Ryoku Morph",
    @"
case ""Super Ryoku Morph"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""superslayin"", ""Super Ryoku"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Super Ryoku Spikes",
    @"
case ""Super Ryoku Spikes"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""superslayin"", ""Super Ryoku"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Synderes' Souvenir",
    @"
case ""Synderes' Souvenir"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(4247);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //Synderes Souvenirs Shop 4247
                        Core.HuntMonster(""enemyforest"", ""Evil Elemental"", ""Forest Denizen Slain"", 5);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Meat Ration",
    @"
case ""Meat Ration"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8263);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""cellar"", ""GreenRat"", ""Green Mystery Meat"", 10, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Grain Ration",
    @"
case ""Grain Ration"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8264);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""safiria"", ""Blood Maggot"", ""Bundle of Rice"", 3, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Dairy Ration",
    @"
case ""Dairy Ration"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(8265);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""odokuro"", ""Boss"", ""Right"", ""O-dokuro"", ""Bone Hurt Juice"", 5, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Shadowslayer Apprentice Badge",
    @"
case ""Shadowslayer Apprentice Badge"":
                    Core.FarmingLogger(req.Name, quant);
                    if (!Core.CheckInventory(""Chibi Eldritch Yume""))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""chaosbeast"", ""Kathool"", ""Chibi Eldritch Yume"", isTemp: false);
                    }
                    Core.RegisterQuests(8266);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        if (!Core.CheckInventory(""Holy Wasabi""))
                        {
                            Core.AddDrop(""Holy Wasabi"");
                            Core.EnsureAccept(1075);

                            Core.EquipClass(ClassType.Farm);
                            Core.HuntMonster(""doomwood"", ""Doomwood Ectomancer"", ""Dried Wasabi Powder"", 4);
                            Core.GetMapItem(428, 1, ""lightguard"");

                            Core.EnsureComplete(1075);
                            Bot.Wait.ForPickup(""Holy Wasabi"");
                        }
                        Adv.BuyItem(""alchemyacademy"", 2036, ""Sage Tonic"", 3);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""Sloth"", ""Phlegnn"", ""Unnatural Ooze"", 8);
                        Core.HuntMonster(""beehive"", ""Killer Queen Bee"", ""Sleepy Honey"");

                        Dailies.EldersBlood();
                        if (!Core.CheckInventory(""Elders' Blood""))
                            Core.Logger(""You ran out Elders' Blood, run the bot again at a later date."", messageBox: true, stopBot: true);

                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Brimstone Scrap",
    @"
case ""Brimstone Scrap"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""starsinc"", ""Infernal Imp"", req.Name, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Star Fragment",
    @"
case ""Star Fragment"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(4413);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""starsinc"", ""Living Star"", ""Living Star Defeated"", 30, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Taker and Giver Stone",
    @"
case ""Taker and Giver Stone"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(4414);
                        Farm.BattleUnderB(""Bone Dust"", 15);
                        Farm.BludrutBrawlBoss(quant: 5);
                        Core.HuntMonster(""starsinc"", ""Living Star"", ""Living Star Essence"", 100, false);
                        Core.EnsureComplete(4414);

                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Prime's Respect",
    @"
case ""Prime's Respect"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(4415);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""starsinc"", ""Empowered Prime"", ""Empowered Primed Defeated"", 10, false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Feather of Purity",
    @"
case ""Feather of Purity"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(6287);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""guardiantree"", ""Blossoming Treeant"", ""Treeant Blossom Nectar"", 3);
                        Core.HuntMonster(""guardiantree"", ""Myconid"", ""Myconid Spore"", 3);
                        Core.HuntMonster(""guardiantree"", ""Corrupted Zard"", ""Corrupted Zard"", 3);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Bow of Semiramis",
    @"
case ""Bow of Semiramis"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""guardiantree"", ""Terrane"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Blade of Semiramis",
    @"
case ""Blade of Semiramis"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""guardiantree"", ""Terrane"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Daggers of Semiramis",
    @"
case ""Daggers of Semiramis"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""guardiantree"", ""Terrane"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Staff of Semiramis",
    @"
case ""Staff of Semiramis"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""guardiantree"", ""Terrane"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Thirdspell Token",
    @"
case ""Thirdspell Token"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""thirdspell"", ""Pure Fire Elemental"", req.Name, quant, false);
                    break;

    "
},
{
    "AntiTitan Supplies",
    @"
case ""AntiTitan Supplies"":
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""titanattack"", ""r9"", ""Left"", ""*"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Titan Paladin's Blade",
    @"
case ""Titan Paladin's Blade"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titanattack"", ""Titanic Paladin"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Vindicator Titan",
    @"
case ""Vindicator Titan"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titanattack"", ""Titanic Vindicator"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Vindicator Titan's Axe",
    @"
case ""Vindicator Titan's Axe"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titanattack"", ""Titanic Vindicator"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Titanic Fluid",
    @"
case ""Titanic Fluid"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titanattack"", ""Titanic Vindicator"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Holy Wasabi Jar",
    @"
case ""Holy Wasabi Jar"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titanattack"", ""Supply Caravan"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Holy Hand Grenade",
    @"
case ""Holy Hand Grenade"":
                    Adv.BuyItem(""castle"", 88, 1843, quant, shopItemID: 1847);
                    break;

    "
},
{
    "Blue Overdrive",
    @"
case ""Blue Overdrive"":
                    if (!Core.CheckInventory(""Orange Tachyon Blade""))
                        BuyAllMerge(""Orange Tachyon Blade"");
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5084);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""tachyon"", ""Svelgr the Devourer"", ""Svelgr the Devourer Defeated"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Blue Powercell",
    @"
case ""Blue Powercell"":
                    if (!Core.CheckInventory(""Orange Tachyon Blade""))
                        BuyAllMerge(""Orange Tachyon Blade"");
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5084);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""tachyon"", ""Svelgr the Devourer"", ""Svelgr the Devourer Defeated"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Blue Tachyon Trigger",
    @"
case ""Blue Tachyon Trigger"":
                    if (!Core.CheckInventory(""Orange Tachyon Blade""))
                        BuyAllMerge(""Orange Tachyon Blade"");
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5084);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""tachyon"", ""Svelgr the Devourer"", ""Svelgr the Devourer Defeated"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Blue Tachyon Grip",
    @"
case ""Blue Tachyon Grip"":
                    if (!Core.CheckInventory(""Orange Tachyon Blade""))
                        BuyAllMerge(""Orange Tachyon Blade"");
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5084);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""tachyon"", ""Svelgr the Devourer"", ""Svelgr the Devourer Defeated"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Tachyon Core Piece",
    @"
case ""Tachyon Core Piece"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5083);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""tachyon"", ""Svelgr the Devourer"", ""Svelgr the Devourer Defeated"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Orange Overdrive",
    @"
case ""Orange Overdrive"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5083);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""tachyon"", ""Svelgr the Devourer"", ""Svelgr the Devourer Defeated"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Orange Tachyon Grip",
    @"
case ""Orange Tachyon Grip"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5083);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""tachyon"", ""Svelgr the Devourer"", ""Svelgr the Devourer Defeated"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Orange Tachyon Trigger",
    @"
case ""Orange Tachyon Trigger"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5083);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""tachyon"", ""Svelgr the Devourer"", ""Svelgr the Devourer Defeated"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Orange Powercell",
    @"
case ""Orange Powercell"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5083);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""tachyon"", ""Svelgr the Devourer"", ""Svelgr the Devourer Defeated"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Saeculum Gem",
    @"
case ""Saeculum Gem"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5085);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""tachyon"", ""Svelgr the Devourer"", ""Svelgr Fang"", isTemp: false);
                        Core.HuntMonster(""portalwar"", ""Chronorysa"", ""Sands of Time"", 6, isTemp: false);
                        Core.HuntMonster(""portalmaze"", ""Time Wraith"", ""Wraith Wisp"", 12, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Frozen Tower Merge Token",
    @"
case ""Frozen Tower Merge Token"":
                    Glacera.FrozenTower();
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(3939);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //Clear the Wolves 3939
                        Core.HuntMonster(""frozentower"", ""Ice Wolf"", ""Ice Wolf Slain"", 7);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Bits of Cloth",
    @"
case ""Bits of Cloth"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(3955);
                    Core.HuntMonster(""frozentower"", ""Frostwyrm"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Pieces of Glass",
    @"
case ""Pieces of Glass"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(3955);
                    Core.HuntMonster(""frozentower"", ""Frostwyrm"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Metal bits",
    @"
case ""Metal bits"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(3955);
                    Core.HuntMonster(""frozentower"", ""Frostwyrm"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Bits of Hair",
    @"
case ""Bits of Hair"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(3955);
                    Core.HuntMonster(""frozentower"", ""Frostwyrm"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Pieces of Cloth",
    @"
case ""Pieces of Cloth"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(3955);
                    Core.HuntMonster(""frozentower"", ""Polar Elemental"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Ice Crystals",
    @"
case ""Ice Crystals"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(3955);
                    Core.HuntMonster(""frozentower"", ""Polar Elemental"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Metal Pieces",
    @"
case ""Metal Pieces"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(3955);
                    Core.HuntMonster(""frozentower"", ""Polar Elemental"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Flame of Courage",
    @"
case ""Flame of Courage"":
                    Glacera.FrozenTower();
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(3955);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //Flame of Courage 3955
                        Core.HuntMonster(""frozenruins"", ""Frost Invader"", ""Spark of Courage"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Karok's Glaceran Gem",
    @"
case ""Karok's Glaceran Gem"":
                    Core.EnsureAccept(3955);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""northstar"", ""Karok The Fallen"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Mercury",
    @"
case ""Mercury"":
                    Glacera.FrozenTower();
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(3956);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        ///Quick as silver 3956
                        Core.HuntMonster(""frozenruins"", ""Arctic Eel"", ""Quicker Silver"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Bright Dragon Shield",
    @"
case ""Bright Dragon Shield"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9215);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""terminatemple"", ""Termina Defender"", ""Defender Sparred With"", 8, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Battle Cleric's Draconic Spear",
    @"
case ""Battle Cleric's Draconic Spear"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9215);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""terminatemple"", ""Termina Defender"", ""Defender Sparred With"", 8, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "DragonGuard Badge",
    @"
case ""DragonGuard Badge"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9215);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""terminatemple"", ""Termina Defender"", ""Defender Sparred With"", 8, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Termina Sigil",
    @"
case ""Termina Sigil"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9215);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""terminatemple"", ""Termina Defender"", ""Defender Sparred With"", 8, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Bright Aura",
    @"
case ""Bright Aura"":
                    BLOD.BrightAura(quant);
                    break;
    "
},
{
    "Void Aura",
    @"
case ""Void Aura"":
                    NSOD.VoidAuras(quant);
                    break;
    "
},
{
    "Trace of Chaos",
    @"
case ""Trace of Chaos"":
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ultradrakath"", ""Champion of Chaos"", isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Deadtech War Medal",
    @"
case ""Deadtech War Medal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7638, 7638, 7639, 7641);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""techfortress"", ""Enter"", ""Spawn"", ""*"", log: false);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Pink Blade of Destruction",
    @"
case ""Pink Blade of Destruction"":
                    PBOD.GetPBoD();
                    break;
    "
},
{
    "Unicorn Essence",
    @"
case ""Unicorn Essence"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""undergroundlabb"", ""Ultra Brutalcorn"", ""Unicorn Essence"", quant, false);
                    break;
    "
},
{
    "Silver",
    @"
case ""Silver"":
                    Dailies.MineCrafting(new[] { ""Silver"" }, quant);
                    break;
    "
},
{
    "Spirit Orb",
    @"
case ""Spirit Orb"":
                    BLOD.SpiritOrb(quant);
                    break;
    "
},
{
    "Loyal Spirit Orb",
    @"
case ""Loyal Spirit Orb"":
                    BLOD.LoyalSpiritOrb(quant);
                    break;
    "
},
{
    "Brilliant Aura",
    @"
case ""Brilliant Aura"":
                    BLOD.BrilliantAura(quant);
                    break;
    "
},
{
    "Shard of An Orb",
    @"
case ""Shard of An Orb"":
                    Core.FarmingLogger(req.Name, quant);
                    BLOD.BlindingLightOfDestiny();
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(7654);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //More then one item of the same name as drop btoh temp and non-temp.
                        while (!Bot.ShouldExit && !Core.CheckInventory(55903, 10))
                            Core.KillMonster(""dflesson"", ""r12"", ""Right"", ""Fluffy the Dracolich"", log: false);
                        Core.KillMonster(""dflesson"", ""r3"", ""Right"", ""Fire Elemental"", ""Fire Elemental's Bracer"", 5, isTemp: false);
                        Core.KillMonster(""dflesson"", ""r6"", ""Right"", ""Tog"", ""Tog Claw"", 5, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Ashray Elf Top Knot",
    @"
case ""Ashray Elf Top Knot"":
                    AZM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Ashray Elf Locks",
    @"
case ""Ashray Elf Locks"":
                    AZM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Ashray Top Knot and Horns",
    @"
case ""Ashray Top Knot and Horns"":
                    AZM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Ashray Locks and Horns",
    @"
case ""Ashray Locks and Horns"":
                    AZM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Coastal Corona",
    @"
case ""Coastal Corona"":
                    AZM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Golden Ashray Trident",
    @"
case ""Golden Ashray Trident"":
                    AZM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dancer's Sea Streams",
    @"
case ""Dancer's Sea Streams"":
                    AZM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Silver Vindicator Sword",
    @"
case ""Silver Vindicator Sword"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""trygve"", ""Vindicator Recruit"", req.Name, quant, isTemp: req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Silver Vindicator Hood",
    @"
case ""Silver Vindicator Hood"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""trygve"", ""Vindicator Recruit"", req.Name, quant, isTemp: req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Silver Vindicator Recruit",
    @"
case ""Silver Vindicator Recruit"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""trygve"", ""Vindicator Recruit"", req.Name, quant, isTemp: req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Silver Vindicator Blade",
    @"
case ""Silver Vindicator Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""trygve"", ""Vindicator Soldier"", req.Name, quant, isTemp: req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Silver Vindicator Helm",
    @"
case ""Silver Vindicator Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""trygve"", ""Vindicator Soldier"", req.Name, quant, isTemp: req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Silver Vindicator Soldier",
    @"
case ""Silver Vindicator Soldier"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""trygve"", ""Vindicator Soldier"", req.Name, quant, isTemp: req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Dawn Vindicator Lieutenant Helm",
    @"
case ""Dawn Vindicator Lieutenant Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""trygve"", ""Gramiel"", req.Name, quant, isTemp: req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Dawn Vindicator Lieutenant",
    @"
case ""Dawn Vindicator Lieutenant"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""trygve"", ""Gramiel"", req.Name, quant, isTemp: req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Silver Vindicator Bow",
    @"
case ""Silver Vindicator Bow"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""trygve"", ""Gramiel"", req.Name, quant, isTemp: req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Ubear X Pass",
    @"
case ""Ubear X Pass"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""limft"", ""Ubear"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Unitas Fragment",
    @"
case ""Unitas Fragment"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    if (!Core.IsMember)
                    {
                        Core.RegisterQuests(3760);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""battleundera"", ""Undead Berserker"", ""Warrior Claymore Blade"", isTemp: false, log: false);
                            Core.HuntMonster(""maul"", ""SlimeSkull"", ""Dark Crown Axe"", isTemp: false, log: false);
                            Farm.BattleUnderB(""Undead Energy"", 50);
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    else
                    {
                        Core.RegisterQuests(3763);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""graveyard"", ""Big Jack Sprat"", ""Bone Axe"", isTemp: false, log: false);
                            if (Core.HeroAlignment != 2)
                                Core.ChangeAlignment(Alignment.Evil);
                            Core.BuyItem(""shadowfall"", 47, ""Helm of the Dark Lord"");
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Crypto Token",
    @"
case ""Crypto Token"":
                    Core.Logger($""{req.Name} is daily, make sure you have enough."");
                    Daily.CryptoToken();
                    break;
    "
},
{
    "Shadowed Infernal Companion",
    @"
case ""Shadowed Infernal Companion"":
                    Core.Logger($""{req.Name} is rare, it cannot be farmed."");
                    break;
    "
},
{
    "GrandPapa the Golden Sneevil Morph",
    @"
case ""GrandPapa the Golden Sneevil Morph"":
                    Core.Logger($""{req.Name} is rare, it cannot be farmed."");
                    break;
    "
},
{
    "Papa the Sneevil",
    @"
case ""Papa the Sneevil"":
                    Core.Logger($""{req.Name} is rare, it cannot be farmed."");
                    break;
    "
},
{
    "Guardian Shard",
    @"
case ""Guardian Shard"":
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""worldsoul"", ""r4"", ""Left"", ""*"", req.Name, quant, false);
                    break;

    "
},
{
    "Doomatter",
    @"
case ""Doomatter"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""tercessuinotlim"", 1951, req.Name, quant);
                    break;
    "
},
{
    "Dual Boom Went The Dynamite",
    @"
case ""Dual Boom Went The Dynamite"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual TheWicked",
    @"
case ""Dual TheWicked"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Overlord's DoomBlade",
    @"
case ""Dual Overlord's DoomBlade"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Blessed Coffee Cup",
    @"
case ""Dual Blessed Coffee Cup"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Party Slasher Birthday Sword",
    @"
case ""Dual Party Slasher Birthday Sword"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Rapier of Skulls",
    @"
case ""Dual Rapier of Skulls"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Frostbite",
    @"
case ""Dual Frostbite"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Rocks",
    @"
case ""Dual Rocks"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Phoenix Blade of Nulgath",
    @"
case ""Dual Phoenix Blade of Nulgath"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Shadow Spear of Nulgath",
    @"
case ""Dual Shadow Spear of Nulgath"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Guardian of Virtue",
    @"
case ""Dual Guardian of Virtue"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Leviasea Sword",
    @"
case ""Dual Leviasea Sword"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Iron Dreadsaw",
    @"
case ""Dual Iron Dreadsaw"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Blood Axe Of Destruction",
    @"
case ""Dual Blood Axe Of Destruction"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual PainSaw of Eidolon",
    @"
case ""Dual PainSaw of Eidolon"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Hanzamune Dragon Koi Blade",
    @"
case ""Dual Hanzamune Dragon Koi Blade"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Ugly Stick",
    @"
case ""Dual Ugly Stick"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Balrog Blade",
    @"
case ""Dual Balrog Blade"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Legendary Magma Sword",
    @"
case ""Dual Legendary Magma Sword"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Dragon Saw",
    @"
case ""Dual Dragon Saw"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Overfiend Blade of Nulgath",
    @"
case ""Dual Overfiend Blade of Nulgath"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Bone Sword",
    @"
case ""Dual Bone Sword"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Honor Guard's Blade",
    @"
case ""Dual Honor Guard's Blade"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Ceremonial Legion Blade",
    @"
case ""Dual Ceremonial Legion Blade"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Alteon's Pride",
    @"
case ""Dual Alteon's Pride"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Ddog Sea Serpent Sword",
    @"
case ""Dual Ddog Sea Serpent Sword"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Eternity Blade",
    @"
case ""Dual Eternity Blade"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Blinding Light of Destiny",
    @"
case ""Dual Blinding Light of Destiny"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Crystal Claymore",
    @"
case ""Dual Crystal Claymore"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Dark Crystal Claymore",
    @"
case ""Dual Dark Crystal Claymore"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Soulreaper of Nulgath",
    @"
case ""Dual Soulreaper of Nulgath"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Grumpy Warhammer",
    @"
case ""Dual Grumpy Warhammer"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Crystal Phoenix Blade of Nulgath",
    @"
case ""Dual Crystal Phoenix Blade of Nulgath"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Maximillian's Whip",
    @"
case ""Dual Maximillian's Whip"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual WarpForce War Shovel 20K",
    @"
case ""Dual WarpForce War Shovel 20K"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Godly Mace of the Ancients",
    @"
case ""Dual Godly Mace of the Ancients"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Mace of the Grand Inquisitor",
    @"
case ""Dual Mace of the Grand Inquisitor"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual KneeCappers",
    @"
case ""Dual KneeCappers"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Morning Stars",
    @"
case ""Dual Morning Stars"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Axe of the Black Knight",
    @"
case ""Dual Axe of the Black Knight"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Cruel Axe of Midnight",
    @"
case ""Dual Cruel Axe of Midnight"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Platinum Axe of Destiny",
    @"
case ""Dual Platinum Axe of Destiny"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Star Sword",
    @"
case ""Dual Star Sword"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Big 100K",
    @"
case ""Dual Big 100K"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Golden Phoenix Sword",
    @"
case ""Dual Golden Phoenix Sword"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Hydra Blades",
    @"
case ""Dual Hydra Blades"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Crusader Sword",
    @"
case ""Dual Crusader Sword"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Bloodrivers",
    @"
case ""Dual Bloodrivers"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Star Sword Breaker",
    @"
case ""Dual Star Sword Breaker"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual ReignBringers",
    @"
case ""Dual ReignBringers"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Balor's Cruelty",
    @"
case ""Dual Balor's Cruelty"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Default Sword",
    @"
case ""Dual Default Sword"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Iron Spears",
    @"
case ""Dual Iron Spears"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Mighty Sword Of The Dragons",
    @"
case ""Dual Mighty Sword Of The Dragons"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Butcher Knife",
    @"
case ""Butcher Knife"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Pencil of Endless Scribbles",
    @"
case ""Dual Pencil of Endless Scribbles"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Krom's Brutalities",
    @"
case ""Dual Krom's Brutalities"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Abaddon's Terrors",
    @"
case ""Dual Abaddon's Terrors"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Blades of Awe",
    @"
case ""Dual Blades of Awe"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Burning Blades Of Abezeth",
    @"
case ""Dual Burning Blades Of Abezeth"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Necrotic Swords of Doom",
    @"
case ""Dual Necrotic Swords of Doom"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Burn it Down Staves",
    @"
case ""Dual Burn it Down Staves"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Phoenix Blades",
    @"
case ""Phoenix Blades"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Shadow Terror Axes",
    @"
case ""Dual Shadow Terror Axes"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual DragonBlades of Nulgath",
    @"
case ""Dual DragonBlades of Nulgath"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual ShadowReapers Of Doom",
    @"
case ""Dual ShadowReapers Of Doom"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Cysero's Potatoes",
    @"
case ""Cysero's Potatoes"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Kuro's Wrath",
    @"
case ""Dual Kuro's Wrath"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Mammoth Crusher Blade",
    @"
case ""Dual Mammoth Crusher Blade"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Lilith Katana",
    @"
case ""Dual Lilith Katana"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Light Prismatic Katana",
    @"
case ""Dual Light Prismatic Katana"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Corpse Maker of Nulgath",
    @"
case ""Dual Corpse Maker of Nulgath"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Excavated Glaive: Sword",
    @"
case ""Dual Excavated Glaive: Sword"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Golden Blade of Fate",
    @"
case ""Dual Golden Blade of Fate"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Hex Blade of Nulgath",
    @"
case ""Dual Hex Blade of Nulgath"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Shadowworn",
    @"
case ""Dual Shadowworn"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Bane of Nulgath",
    @"
case ""Dual Bane of Nulgath"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Hollowborn Oblivion Blade",
    @"
case ""Dual Hollowborn Oblivion Blade"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Katana of Revontheus",
    @"
case ""Dual Katana of Revontheus"":
                    YDWM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Destroyer Essence",
    @"
case ""Destroyer Essence"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titanstrike"", ""Titanic Destroyer"", req.Name, quant, false);
                    break;
    "
},
{
    "Titanic Destroyer Blade",
    @"
case ""Titanic Destroyer Blade"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titanstrike"", ""Titanic Destroyer"", req.Name, quant, false);
                    break;
    "
},
{
    "Titanic Destroyer Morph",
    @"
case ""Titanic Destroyer Morph"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titanstrike"", ""Titanic Destroyer"", req.Name, quant, false);
                    break;
    "
},
{
    "Titanic Tincture",
    @"
case ""Titanic Tincture"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titandrakath"", ""Titan Drakath"", req.Name, quant, false);
                    break;
    "
},
{
    "Heroic Titan's Greatsword",
    @"
case ""Heroic Titan's Greatsword"":
                    if (Core.isCompletedBefore(8776))
                        Core.Logger($""{req.Name} is obtained from a One-Time only quest that you have already completed. Please check your BuyBack"", messageBox: true, stopBot: true);

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.EnsureAccept(8776);
                    Core.HuntMonster(""titanstrike"", ""Titanic Paladin"", ""Paladin Punished"");
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titanstrike"", ""Titanic Doomknight"", ""Doomknight Decimated"");
                    Core.HuntMonster(""titanstrike"", ""Titanic Destroyer"", ""Destroyer Destroyed"");
                    Core.EnsureComplete(8776);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Titan Paladin",
    @"
case ""Titan Paladin"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""titanattack"", ""Chaorrupted Bandit"", ""AntiTitan Supplies"", 100, false);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titanattack"", ""Titanic Vindicator"", ""Titanic Fluid"", 40, false);
                    Adv.BuyItem(""titanattack"", 2149, req.Name);
                    break;
    "
},
{
    "Vindicator Titan XL",
    @"
case ""Vindicator Titan XL"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""titanattack"", ""Chaorrupted Bandit"", ""AntiTitan Supplies"", 100, false);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titanattack"", ""Titanic Vindicator"", ""Titanic Fluid"", 40, false);
                    Core.HuntMonster(""titanattack"", ""Titanic Vindicator"", ""Vindicator Titan"", isTemp: false);
                    Adv.BuyItem(""titanattack"", 2149, req.Name);
                    break;
    "
},
{
    "Vindicator Titan's Axes",
    @"
case ""Vindicator Titan's Axes"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""titanattack"", ""Chaorrupted Bandit"", ""AntiTitan Supplies"", 50, false);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titanattack"", ""Titanic Vindicator"", ""Titanic Fluid"", 20, false);
                    Core.HuntMonster(""titanattack"", ""Titanic Vindicator"", ""Vindicator Titan's Axe"", isTemp: false);
                    Adv.BuyItem(""titanattack"", 2149, req.Name);
                    break;
    "
},
{
    "Titan Paladin's Blades",
    @"
case ""Titan Paladin's Blades"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""titanattack"", ""Chaorrupted Bandit"", ""AntiTitan Supplies"", 50, false);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titanattack"", ""Titanic Vindicator"", ""Titanic Fluid"", 20, false);
                    Core.HuntMonster(""titanattack"", ""Titanic Paladin"", ""Titan Paladin's Blade"", isTemp: false);
                    Adv.BuyItem(""titanattack"", 2149, req.Name);
                    break;
    "
},
{
    "Titan Drakath's Blade",
    @"
case ""Titan Drakath's Blade"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titandrakath"", ""Titan Drakath"", req.Name, quant, false);
                    break;
    "
},
{
    "Titan Paladin's Helm",
    @"
case ""Titan Paladin's Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""titanattack"", ""Chaorrupted Bandit"", ""AntiTitan Supplies"", 25, false);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titanattack"", ""Titanic Vindicator"", ""Titanic Fluid"", 10, false);
                    Adv.BuyItem(""titanattack"", 2149, req.Name);
                    break;
    "
},
{
    "Vindicator Titan's Helm",
    @"
case ""Vindicator Titan's Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""titanattack"", ""Chaorrupted Bandit"", ""AntiTitan Supplies"", 25, false);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titanattack"", ""Titanic Vindicator"", ""Titanic Fluid"", 10, false);
                    Adv.BuyItem(""titanattack"", 2149, req.Name);
                    break;
    "
},
{
    "Titan Drakath's Morph",
    @"
case ""Titan Drakath's Morph"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titandrakath"", ""Titan Drakath"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Titan Paladin's Cloak",
    @"
case ""Titan Paladin's Cloak"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""titanattack"", ""Chaorrupted Bandit"", ""AntiTitan Supplies"", 25, false);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titanattack"", ""Titanic Vindicator"", ""Titanic Fluid"", 10, false);
                    Adv.BuyItem(""titanattack"", 2149, req.Name);
                    break;
    "
},
{
    "Vindicator Titan's Cloak",
    @"
case ""Vindicator Titan's Cloak"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""titanattack"", ""Chaorrupted Bandit"", ""AntiTitan Supplies"", 25, false);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""titanattack"", ""Titanic Vindicator"", ""Titanic Fluid"", 10, false);
                    Adv.BuyItem(""titanattack"", 2149, req.Name);
                    break;
    "
},
{
    "Chaorrupted AntiTitan Corps",
    @"
case ""Chaorrupted AntiTitan Corps"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""titanattack"", ""AntiTitan Corps"", req.Name, quant, false);
                    break;
    "
},
{
    "Titan Hunter",
    @"
case ""Titan Hunter"":
                    Adv.BuyItem(""artistalley"", 729, req.Name);
                    break;

    "
},
{
    "Venomous Rose",
    @"
case ""Venomous Rose"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9274);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""twilightzone"", ""Leviathan"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Shikigami String",
    @"
case ""Shikigami String"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9677);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""yokaiportal"", ""Kitsune Spirits"", ""Kitsune Spirit Incense"", 15, log: false);
                        Core.HuntMonster(""yokaiportal"", ""Puppeted Dragonling"", ""Draconic Red String"", 15, log: false);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""yokaiportal"", ""Kitsune Kukol'nyy"", ""Lord Kitsune's Red String"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Zakhvatchik's Sapphire",
    @"
case ""Zakhvatchik's Sapphire"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""hakuwar"", ""Zakhvatchik"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Noble Amethyst Katana",
    @"
case ""Noble Amethyst Katana"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""yokaiportal"", ""Kitsune Kukol'nyy"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Kitsune's Ruby",
    @"
case ""Kitsune's Ruby"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""yokaiportal"", ""Kitsune Kukol'nyy"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Admiral Zheng's Jade",
    @"
case ""Admiral Zheng's Jade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""yokaitreasure"", ""Admiral Zheng"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Warfury Emblem",
    @"
case ""Warfury Emblem"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Emblem.WarfuryEmblemFarm(quant);
                    break;
    "
},
{
    "WarFury Soldier's Morph",
    @"
case ""WarFury Soldier's Morph"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""wartraining"", ""Varga"", req.Name, isTemp: false);
                    break;
    "
},
{
    "WarFury Soldier's Armor",
    @"
case ""WarFury Soldier's Armor"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""wartraining"", ""Varga"", req.Name, isTemp: false);
                    break;
    "
},
{
    "WarFury Soldier's Blade",
    @"
case ""WarFury Soldier's Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""wartraining"", ""Varga"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Enchanted Scale",
    @"
case ""Enchanted Scale"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    DSG.EnchantedScaleandClaw(quant, 0);
                    break;
    "
},
{
    "Void Scale",
    @"
case ""Void Scale"":
                    Core.FarmingLogger(req.Name, quant);
                    FCA.VoidScale(quant);
                    break;
    "
},
{
    "Dragon Scale",
    @"
case ""Dragon Scale"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""lair"", ""Bronze Draconian"", req.Name, quant);
                    break;
    "
},
{
    "Ox Medallion",
    @"
case ""Ox Medallion"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7942);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""yokaihunt"", ""r3"", ""Left"", ""Golden Ox Guard"", ""Ox Meat"", 8, log: false);
                        Core.KillMonster(""yokaihunt"", ""r6"", ""Left"", ""Ox Yokai Spirit"", ""Holographic Ox Meat"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Miko's Blessing",
    @"
case ""Miko's Blessing"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(10059,
                        (""shogunwar"", ""Shadow Samurai"", ClassType.Farm));
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Pearlescent Scale",
    @"
case ""Pearlescent Scale"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(10060,
                        (""yokaihunt"", ""Zhenzhu Shé"", ClassType.Solo));
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Urban Serpent Cap + Glasses",
    @"
case ""Urban Serpent Cap + Glasses"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""yokaihunt"", ""Zhenzhu Shé"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Urban Serpent Hat + Glasses",
    @"
case ""Urban Serpent Hat + Glasses"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""yokaihunt"", ""Zhenzhu Shé"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Urban Serpent Locks + Glasses",
    @"
case ""Urban Serpent Locks + Glasses"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""yokaihunt"", ""Zhenzhu Shé"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Urban Serpent Hair + Glasses",
    @"
case ""Urban Serpent Hair + Glasses"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""yokaihunt"", ""Zhenzhu Shé"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Lunar Fragment",
    @"
case ""Lunar Fragment"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9094);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""guardiantree"", ""Blossoming Treeant"", ""Fresh Blossoms"", 8, log: false);
                        Core.HuntMonster(""guardiantree"", ""Seed Spitter"", ""Fresh Seeds"", 8, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Etokoun Residue",
    @"
case ""Etokoun Residue"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(9097);
                        Core.KillMonster(""yokaihunt"", ""r6a"", ""Left"", ""*"", ""Etokoun Wrangled"", log: false);
                        Core.EnsureComplete(9097);
                        Bot.Wait.ForPickup(req.Name);

                    }
                    break;
    "
},
{
    "Baoyu's Red Envelope",
    @"
case ""Baoyu's Red Envelope"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9572);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""shipwreck"", ""Gilded Merdraconian"", ""Merdraconian Coins"", 15, log: false);
                        Core.HuntMonster(""shipwreck"", ""Lobthulhu"", ""Lobthulu's Gold Bar"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Baoyu's Flaming Envelope",
    @"
case ""Baoyu's Flaming Envelope"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9574);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ashfallcamp"", ""Smoldur"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Baoyu's Rainbow Envelope",
    @"
case ""Baoyu's Rainbow Envelope"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9575);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""yokaihunt"", ""Mutou Hong"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Duplication Error",
    @"
case ""Duplication Error"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ebilart"", ""Ebil AI Blender"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Meateor Shard",
    @"
case ""Meateor Shard"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""meateortown"", ""Giant ChickenCow"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Cutie Cow Pet",
    @"
case ""Cutie Cow Pet"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""meateortown"", ""Giant ChickenCow"", req.Name, quant, false, false);
                    break;
    "
},
{
    "ChickenCow Teeth",
    @"
case ""ChickenCow Teeth"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.isCompletedBefore(8612))
                    {
                        Core.EnsureAccept(8612);
                        Core.HuntMonster(""meateortown"", ""Giant ChickenCow"", ""ChickenCow Tamed"");
                        Core.EnsureComplete(8612);
                    }
                    Core.RegisterQuests(8613);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""meateortown"", ""Spicy ChickenCow"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Silver Savior of Battleon",
    @"
case ""Silver Savior of Battleon"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.isCompletedBefore(8612))
                    {
                        Core.EnsureAccept(8612);
                        Core.HuntMonster(""meateortown"", ""Giant ChickenCow"", ""ChickenCow Tamed"");
                        Core.EnsureComplete(8612);
                    }
                    Core.RegisterQuests(8613);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""meateortown"", ""Spicy ChickenCow"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Illustrious Savior of Battleon",
    @"
case ""Illustrious Savior of Battleon"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.isCompletedBefore(8612))
                    {
                        Core.EnsureAccept(8612);
                        Core.HuntMonster(""meateortown"", ""Giant ChickenCow"", ""ChickenCow Tamed"");
                        Core.EnsureComplete(8612);
                    }
                    Core.RegisterQuests(8613);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""meateortown"", ""Spicy ChickenCow"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Armored Defender of Battleon",
    @"
case ""Armored Defender of Battleon"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.isCompletedBefore(8612))
                    {
                        Core.EnsureAccept(8612);
                        Core.HuntMonster(""meateortown"", ""Giant ChickenCow"", ""ChickenCow Tamed"");
                        Core.EnsureComplete(8612);
                    }
                    Core.RegisterQuests(8613);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""meateortown"", ""Spicy ChickenCow"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Armored Victor of Battleon",
    @"
case ""Armored Victor of Battleon"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.isCompletedBefore(8612))
                    {
                        Core.EnsureAccept(8612);
                        Core.HuntMonster(""meateortown"", ""Giant ChickenCow"", ""ChickenCow Tamed"");
                        Core.EnsureComplete(8612);
                    }
                    Core.RegisterQuests(8613);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""meateortown"", ""Spicy ChickenCow"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Silver Defender's Helm",
    @"
case ""Silver Defender's Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.isCompletedBefore(8612))
                    {
                        Core.EnsureAccept(8612);
                        Core.HuntMonster(""meateortown"", ""Giant ChickenCow"", ""ChickenCow Tamed"");
                        Core.EnsureComplete(8612);
                    }
                    Core.RegisterQuests(8613);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""meateortown"", ""Spicy ChickenCow"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Silver Savior's Visor",
    @"
case ""Silver Savior's Visor"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.isCompletedBefore(8612))
                    {
                        Core.EnsureAccept(8612);
                        Core.HuntMonster(""meateortown"", ""Giant ChickenCow"", ""ChickenCow Tamed"");
                        Core.EnsureComplete(8612);
                    }
                    Core.RegisterQuests(8613);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""meateortown"", ""Spicy ChickenCow"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Silver Savior's Magical Wrap",
    @"
case ""Silver Savior's Magical Wrap"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.isCompletedBefore(8612))
                    {
                        Core.EnsureAccept(8612);
                        Core.HuntMonster(""meateortown"", ""Giant ChickenCow"", ""ChickenCow Tamed"");
                        Core.EnsureComplete(8612);
                    }
                    Core.RegisterQuests(8613);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""meateortown"", ""Spicy ChickenCow"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Illustrious Defender Pet",
    @"
case ""Illustrious Defender Pet"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.isCompletedBefore(8612))
                    {
                        Core.EnsureAccept(8612);
                        Core.HuntMonster(""meateortown"", ""Giant ChickenCow"", ""ChickenCow Tamed"");
                        Core.EnsureComplete(8612);
                    }
                    Core.RegisterQuests(8613);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""meateortown"", ""Spicy ChickenCow"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Illustrious Savior's Blade",
    @"
case ""Illustrious Savior's Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.isCompletedBefore(8612))
                    {
                        Core.EnsureAccept(8612);
                        Core.HuntMonster(""meateortown"", ""Giant ChickenCow"", ""ChickenCow Tamed"");
                        Core.EnsureComplete(8612);
                    }
                    Core.RegisterQuests(8613);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""meateortown"", ""Spicy ChickenCow"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Illustrious Savior's Blades",
    @"
case ""Illustrious Savior's Blades"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.isCompletedBefore(8612))
                    {
                        Core.EnsureAccept(8612);
                        Core.HuntMonster(""meateortown"", ""Giant ChickenCow"", ""ChickenCow Tamed"");
                        Core.EnsureComplete(8612);
                    }
                    Core.RegisterQuests(8613);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""meateortown"", ""Spicy ChickenCow"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Silver Victor's Hammer",
    @"
case ""Silver Victor's Hammer"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.isCompletedBefore(8612))
                    {
                        Core.EnsureAccept(8612);
                        Core.HuntMonster(""meateortown"", ""Giant ChickenCow"", ""ChickenCow Tamed"");
                        Core.EnsureComplete(8612);
                    }
                    Core.RegisterQuests(8613);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""meateortown"", ""Spicy ChickenCow"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Silver Victor's Hammers",
    @"
case ""Silver Victor's Hammers"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.isCompletedBefore(8612))
                    {
                        Core.EnsureAccept(8612);
                        Core.HuntMonster(""meateortown"", ""Giant ChickenCow"", ""ChickenCow Tamed"");
                        Core.EnsureComplete(8612);
                    }
                    Core.RegisterQuests(8613);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""meateortown"", ""Spicy ChickenCow"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Tidal Byakko Warrior",
    @"
case ""Tidal Byakko Warrior"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""akibacny"", ""Umitora"", req.Name, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Tidal Byakko Warrior Hair",
    @"
case ""Tidal Byakko Warrior Hair"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""akibacny"", ""Umitora"", req.Name, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Tidal Byakko Warrior Locks",
    @"
case ""Tidal Byakko Warrior Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""akibacny"", ""Umitora"", req.Name, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Tidal Tiger Blissus' Fighting Stance",
    @"
case ""Tidal Tiger Blissus' Fighting Stance"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""akibacny"", ""Umitora"", req.Name, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Tidal Byakko Wakizashi",
    @"
case ""Tidal Byakko Wakizashi"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""akibacny"", ""Umitora"", req.Name, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Tidal Byakko Wakizashis",
    @"
case ""Tidal Byakko Wakizashis"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""akibacny"", ""Umitora"", req.Name, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Tidal Byakko Fan",
    @"
case ""Tidal Byakko Fan"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""akibacny"", ""Umitora"", req.Name, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Tidal Byakko Fans",
    @"
case ""Tidal Byakko Fans"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""akibacny"", ""Umitora"", req.Name, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Tidal Byakko's Grasps",
    @"
case ""Tidal Byakko's Grasps"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""akibacny"", ""Umitora"", req.Name, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Tidal Byakko's Claws",
    @"
case ""Tidal Byakko's Claws"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""akibacny"", ""Umitora"", req.Name, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Lady Lua's Fan",
    @"
case ""Lady Lua's Fan"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""akibacny"", ""Umitora"", req.Name, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Lua's Lucky Envelope",
    @"
case ""Lua's Lucky Envelope"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(8506);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""akibacny"", ""Umitora"", req.Name, quant, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;




    "
},
{
    "Epic Item Name",
    @"
case ""Epic Item Name"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9660); // Gathering Evidence (9660)
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""ebilart"", ""Ebil AI Blender"", ""AI Learning Algorithm"", 1643631, log: false);
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""ebilart"", ""UNUNSkellingdens"", ""Blurry Teeth"", 94543, log: false);
                        Core.HuntMonster(""ebilart"", ""Fish"", ""Wet Sashimi"", 64731, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Ebil Company Sign",
    @"
case ""Ebil Company Sign"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ebilart"", ""Ebil AI Blender"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Furry Ticket",
    @"
case ""Furry Ticket"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9185);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.GetMapItem(11466, map: ""magicmeaderp"");
                        Core.GetMapItem(11467, map: ""magicmeaderp"");
                        Core.GetMapItem(11468, map: ""magicmeaderp"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Slobber Ticket",
    @"
case ""Slobber Ticket"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9185);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.GetMapItem(11466, map: ""magicmeaderp"");
                        Core.GetMapItem(11467, map: ""magicmeaderp"");
                        Core.GetMapItem(11468, map: ""magicmeaderp"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Floofy Ticket",
    @"
case ""Floofy Ticket"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9185);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.GetMapItem(11466, map: ""magicmeaderp"");
                        Core.GetMapItem(11467, map: ""magicmeaderp"");
                        Core.GetMapItem(11468, map: ""magicmeaderp"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Fancy Ticket",
    @"
case ""Fancy Ticket"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9186);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.GetMapItem(11469, map: ""magicmeaderp"");
                        Core.GetMapItem(11470, map: ""magicmeaderp"");
                        Core.GetMapItem(11471, map: ""magicmeaderp"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Hoppy Ticket",
    @"
case ""Hoppy Ticket"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9186);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.GetMapItem(11469, map: ""magicmeaderp"");
                        Core.GetMapItem(11470, map: ""magicmeaderp"");
                        Core.GetMapItem(11471, map: ""magicmeaderp"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Mushy Ticket",
    @"
case ""Mushy Ticket"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9186);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.GetMapItem(11469, map: ""magicmeaderp"");
                        Core.GetMapItem(11470, map: ""magicmeaderp"");
                        Core.GetMapItem(11471, map: ""magicmeaderp"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Pwny Ticket",
    @"
case ""Pwny Ticket"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9187);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.GetMapItem(11472, map: ""magicmeaderp"");
                        Core.GetMapItem(11473, map: ""magicmeaderp"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Bony Ticket",
    @"
case ""Bony Ticket"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9187);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.GetMapItem(11472, map: ""magicmeaderp"");
                        Core.GetMapItem(11473, map: ""magicmeaderp"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Starry Bow",
    @"
case ""Starry Bow"":
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""spacepwny"", ""r3"", ""Right"", ""*"", req.Name, quant, false);
                    break;
    "
},
{
    "DOOM Gift",
    @"
case ""DOOM Gift"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""spacepwny"", ""Mr DED"", req.Name, quant, false);
                    break;
    "
},
{
    "Scarbucks Gift Card",
    @"
case ""Scarbucks Gift Card"":
                    Core.EquipClass(ClassType.Solo);
                    Bot.Quests.UpdateQuest(8892);
                    Core.KillMonster(""mermaidsushi"", ""r7a"", ""Left"", ""*"", req.Name, quant, false);
                    break;
    "
},
{
    "Golden Anniversary Gift",
    @"
case ""Golden Anniversary Gift"":
                    Core.EquipClass(ClassType.Solo);
                    if (req.Name == ""Platinum Leaf"")
                        Core.RegisterQuests(8925);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""yulgarparty"", ""Treasure Pile"", ""Twilly's Treasure Defeated"");
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Platinum Leaf",
    @"
case ""Platinum Leaf"":
                    Core.EquipClass(ClassType.Solo);
                    if (req.Name == ""Platinum Leaf"")
                        Core.RegisterQuests(8925);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""yulgarparty"", ""Treasure Pile"", ""Twilly's Treasure Defeated"");
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Ultimate Dragonlord Cape",
    @"
case ""Ultimate Dragonlord Cape"":
                    Core.EquipClass(ClassType.Solo);
                    if (req.Name == ""Platinum Leaf"")
                        Core.RegisterQuests(8925);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""yulgarparty"", ""Treasure Pile"", ""Twilly's Treasure Defeated"");
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Ultimate Dragonlord Wings",
    @"
case ""Ultimate Dragonlord Wings"":
                    Core.EquipClass(ClassType.Solo);
                    if (req.Name == ""Platinum Leaf"")
                        Core.RegisterQuests(8925);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""yulgarparty"", ""Treasure Pile"", ""Twilly's Treasure Defeated"");
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Copper Scale",
    @"
case ""Copper Scale"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);

                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        // 14th Anniversary Gifts 6554
                        Core.EnsureAccept(6554);

                        Core.HuntMonster(""birthday"", ""Birthday Cake"", ""Top Cherry"");
                        Core.HuntMonster(""birthday"", ""Birthday Cake"", ""Copper Knife"");
                        Core.HuntMonster(""birthday"", ""Birthday Cake"", ""Platinum Fork"");
                        Core.HuntMonster(""birthday"", ""Birthday Cake"", ""Gold Spoon"");

                        Core.EnsureComplete(6554, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Gold Scale",
    @"
case ""Gold Scale"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);

                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        // 14th Anniversary Gifts 6554
                        Core.EnsureAccept(6554);

                        Core.HuntMonster(""birthday"", ""Birthday Cake"", ""Top Cherry"");
                        Core.HuntMonster(""birthday"", ""Birthday Cake"", ""Copper Knife"");
                        Core.HuntMonster(""birthday"", ""Birthday Cake"", ""Platinum Fork"");
                        Core.HuntMonster(""birthday"", ""Birthday Cake"", ""Gold Spoon"");

                        Core.EnsureComplete(6554, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Platinum Scale",
    @"
case ""Platinum Scale"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);

                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        // 14th Anniversary Gifts 6554
                        Core.EnsureAccept(6554);

                        Core.HuntMonster(""birthday"", ""Birthday Cake"", ""Top Cherry"");
                        Core.HuntMonster(""birthday"", ""Birthday Cake"", ""Copper Knife"");
                        Core.HuntMonster(""birthday"", ""Birthday Cake"", ""Platinum Fork"");
                        Core.HuntMonster(""birthday"", ""Birthday Cake"", ""Gold Spoon"");

                        Core.EnsureComplete(6554, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Onyx Scale",
    @"
case ""Onyx Scale"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);

                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        // 14th Anniversary Gifts 6554
                        Core.EnsureAccept(6554);

                        Core.HuntMonster(""birthday"", ""Birthday Cake"", ""Top Cherry"");
                        Core.HuntMonster(""birthday"", ""Birthday Cake"", ""Copper Knife"");
                        Core.HuntMonster(""birthday"", ""Birthday Cake"", ""Platinum Fork"");
                        Core.HuntMonster(""birthday"", ""Birthday Cake"", ""Gold Spoon"");

                        Core.EnsureComplete(6554, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Hero Plushie",
    @"
case ""Hero Plushie"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""birthday"", ""Birthday Cake"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Chaorrupted Button",
    @"
case ""Chaorrupted Button"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""birthday"", ""Birthday Cake"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Cursed Pinata Candy",
    @"
case ""Cursed Pinata Candy"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""birthday"", ""Twilly Pinata"", req.Name, quant, isTemp: false);
                    break;


    "
},
{
    "Tinfoil Wrapper",
    @"
case ""Tinfoil Wrapper"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""birthday"", ""Twilly Pinata"", req.Name, quant, isTemp: false);
                    break;


    "
},
{
    "Knight Armet",
    @"
case ""Knight Armet"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""birthday"", ""Twilly Pinata"", req.Name, quant, isTemp: false);
                    break;


    "
},
{
    "Knight Armor",
    @"
case ""Knight Armor"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""birthday"", ""Twilly Pinata"", req.Name, quant, isTemp: false);
                    break;


    "
},
{
    "Knight Sallet",
    @"
case ""Knight Sallet"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""birthday"", ""Twilly Pinata"", req.Name, quant, isTemp: false);
                    break;


    "
},
{
    "Knight Cloak",
    @"
case ""Knight Cloak"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""birthday"", ""Twilly Pinata"", req.Name, quant, isTemp: false);
                    break;


    "
},
{
    "Knight Mace",
    @"
case ""Knight Mace"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""birthday"", ""Twilly Pinata"", req.Name, quant, isTemp: false);
                    break;


    "
},
{
    "Knight Spear",
    @"
case ""Knight Spear"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""birthday"", ""Twilly Pinata"", req.Name, quant, isTemp: false);
                    break;


    "
},
{
    "Knight Zweihander",
    @"
case ""Knight Zweihander"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""birthday"", ""Twilly Pinata"", req.Name, quant, isTemp: false);
                    break;


    "
},
{
    "Knight Great Helm",
    @"
case ""Knight Great Helm"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""birthday"", ""Twilly Pinata"", req.Name, quant, isTemp: false);
                    break;


    "
},
{
    "Emblem of Righteousness",
    @"
case ""Emblem of Righteousness"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    if (!Core.IsMember)
                    {
                        Core.RegisterQuests(1582);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""mafic"", ""Mafic Dragon"", ""Magmas Spirit"");
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    else
                    {
                        Core.RegisterQuests(1584);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""j6"", ""Sketchy Dragon"", ""Scrawl Spirit"");
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Emblem of Good Luck",
    @"
case ""Emblem of Good Luck"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    if (!Core.IsMember)
                    {
                        Core.RegisterQuests(1582);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""mafic"", ""Mafic Dragon"", ""Magmas Spirit"");
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    else
                    {
                        Core.RegisterQuests(1584);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""j6"", ""Sketchy Dragon"", ""Scrawl Spirit"");
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Emblem of Knowledge",
    @"
case ""Emblem of Knowledge"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    if (!Core.IsMember)
                    {
                        Core.RegisterQuests(1582);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""mafic"", ""Mafic Dragon"", ""Magmas Spirit"");
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    else
                    {
                        Core.RegisterQuests(1584);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""j6"", ""Sketchy Dragon"", ""Scrawl Spirit"");
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Emblem of Longevity",
    @"
case ""Emblem of Longevity"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    if (!Core.IsMember)
                    {
                        Core.RegisterQuests(1582);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""mafic"", ""Mafic Dragon"", ""Magmas Spirit"");
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    else
                    {
                        Core.RegisterQuests(1584);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""j6"", ""Sketchy Dragon"", ""Scrawl Spirit"");
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Lucky Red Envelope",
    @"
case ""Lucky Red Envelope"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    if (!Core.IsMember)
                    {
                        if (!Core.CheckInventory(""Emblem of Longevity""))
                        {
                            Core.AddDrop(""Emblem of Longevity"");
                            Core.EnsureAccept(954);
                            Core.HuntMonster(""mountfrost"", ""Snow Golem"", ""Icy Amulet"", 5);
                            Core.HuntMonster(""mountfrost"", ""Frostwyrm Rider"", ""Water Amulet"", 5);
                            Core.EnsureComplete(954);
                        }
                        Core.RegisterQuests(955);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""creatures"", ""Black Tortoise"", req.Name, quant);
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    else
                    {
                        Core.RegisterQuests(1584);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""j6"", ""Sketchy Dragon"", req.Name, quant);
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Super Lucky Red Envelope",
    @"
case ""Super Lucky Red Envelope"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    if (!Core.IsMember)
                    {
                        if (!Core.CheckInventory(""Emblem of Longevity""))
                        {
                            Core.AddDrop(""Emblem of Longevity"");
                            Core.EnsureAccept(954);
                            Core.HuntMonster(""mountfrost"", ""Snow Golem"", ""Icy Amulet"", 5);
                            Core.HuntMonster(""mountfrost"", ""Frostwyrm Rider"", ""Water Amulet"", 5);
                            Core.EnsureComplete(954);
                        }
                        Core.RegisterQuests(955);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""creatures"", ""Black Tortoise"", req.Name, quant);
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    else
                    {
                        Core.RegisterQuests(1584);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""j6"", ""Sketchy Dragon"", req.Name, quant);
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Luckier Red Envelope",
    @"
case ""Luckier Red Envelope"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    if (!Core.IsMember)
                    {
                        if (!Core.CheckInventory(""Emblem of Longevity""))
                        {
                            Core.AddDrop(""Emblem of Longevity"");
                            Core.EnsureAccept(954);
                            Core.HuntMonster(""mountfrost"", ""Snow Golem"", ""Icy Amulet"", 5);
                            Core.HuntMonster(""mountfrost"", ""Frostwyrm Rider"", ""Water Amulet"", 5);
                            Core.EnsureComplete(954);
                        }
                        Core.RegisterQuests(955);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""creatures"", ""Black Tortoise"", req.Name, quant);
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    else
                    {
                        Core.RegisterQuests(1584);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""j6"", ""Sketchy Dragon"", req.Name, quant);
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Bunny",
    @"
case ""Bunny"":
                    Adv.BuyItem(""ariapet"", 12, ""Bunny"");
                    break;
    "
},
{
    "Gold Medallion",
    @"
case ""Gold Medallion"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""akibacny"", ""Hinezumi"", req.Name, quant);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Lunar Firecracker",
    @"
case ""Lunar Firecracker"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(7923);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""akibacny"", ""Lu Niu"", req.Name, quant);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Magenta Dye",
    @"
case ""Magenta Dye"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    if (!Core.IsMember)
                    {
                        Core.RegisterQuests(1489);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""sandsea"", ""Cactus Creeper"", ""Fandango Flower"", 5);
                            Core.KillMonster(""wanders"",""r2"", ""Down"", ""Lotus Spider"", ""Lotus Flower"", 4);
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    else
                    {
                        Core.RegisterQuests(1490);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""uppercity"", ""Rhino Beetle"", ""Carmine Pigment"", 4);
                            Core.HuntMonster(""doomwood"", ""Doomwood Treeant"", ""Cerise Flower"", 3);
                            Core.HuntMonster(""voltaire"", ""Fishbones"", ""Anthurium Flower"");
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Necromancer's Pride",
    @"
case ""Necromancer's Pride"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    //I Want to Be The Very Best Necromancer 7751
                    Core.RegisterQuests(7751);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterMapID(""battleundera"", 10, ""Skeleton Captured"", 10, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Necromancer's Joy",
    @"
case ""Necromancer's Joy"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    //Like No One Ever Was 7752
                    Core.RegisterQuests(7752);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""doomwood"", ""r8"", ""Left"", ""*"", ""Bones Collected"", 15, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Necromancer's Insanity",
    @"
case ""Necromancer's Insanity"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    //To Raise Them is my Real Quest 7753
                    Bot.Quests.UpdateQuest(2060);
                    Bot.Quests.UpdateQuest(3019);
                    Core.RegisterQuests(7753);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterMapID(""necrodungeon"", 46, ""Dracolich Head"");
                        Core.HuntMonsterMapID(""necrodungeon"", 47, ""Yet Another Dracolich Head"");
                        Core.HuntMonsterMapID(""necrodungeon"", 49, ""More Dracolich Heads"");
                        Core.HuntMonsterMapID(""underrealm"", 24, ""Fresh Agony Wraps"", 5);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Crown of Chaos",
    @"
case ""Crown of Chaos"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9965);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""hiddenduat"", ""Anubian Overseer"", ""Duanmutef Glyph"", 6, log: false);
                        Core.HuntMonster(""hiddenduat"", ""Pharaoh Neith"", ""Neith's Uraeus"", log: false);
                        Core.HuntMonster(""hiddenduat"", ""Umbral Chaos"", ""Apophis' Violet Favor"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Advent Darkness Axe",
    @"
case ""Advent Darkness Axe"":
                    Core.EquipClass(ClassType.Solo);
                    Core.FarmingLogger(req.Name, quant);
                    Core.HuntMonster(""camlan"", ""Metamorphosis Maw"", req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Advent Darkness Blade",
    @"
case ""Advent Darkness Blade"":
                    Core.EquipClass(ClassType.Solo);
                    Core.FarmingLogger(req.Name, quant);
                    Core.HuntMonster(""camlan"", ""Metamorphosis Maw"", req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Shrouded Carnage Maw Cleaver",
    @"
case ""Shrouded Carnage Maw Cleaver"":
                    Core.EquipClass(ClassType.Solo);
                    Core.FarmingLogger(req.Name, quant);
                    Core.HuntMonster(""camlan"", ""Metamorphosis Maw"", req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Dark Eons Sword",
    @"
case ""Dark Eons Sword"":
                    Core.EquipClass(ClassType.Solo);
                    Core.FarmingLogger(req.Name, quant);
                    Core.HuntMonster(""camlan"", ""Metamorphosis Maw"", req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Dark Eons Broadsword",
    @"
case ""Dark Eons Broadsword"":
                    Core.EquipClass(ClassType.Solo);
                    Core.FarmingLogger(req.Name, quant);
                    Core.HuntMonster(""camlan"", ""Metamorphosis Maw"", req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Corrupted Hieroglyph",
    @"
case ""Corrupted Hieroglyph"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(9921,
(""lotustomb"", SoD.UMLotusTomb[4], ClassType.Solo),
        (""lotustomb"", SoD.UMLotusTomb[2], ClassType.Farm),
        (""lotustomb"", SoD.UMLotusTomb[3], ClassType.Farm)
);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "A Memory",
    @"
case ""A Memory"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9938, 9939);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""shadowduat"", ""r2"", ""Left"", ""*"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Metamorphosis Maw's Knight",
    @"
case ""Metamorphosis Maw's Knight"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""shadowduat"", ""DoomKnight Dryden"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Metamorphosis Maw's Loyal Knight",
    @"
case ""Metamorphosis Maw's Loyal Knight"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""shadowduat"", ""DoomKnight Dryden"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Metamophosis Maw's Knight Hair",
    @"
case ""Metamophosis Maw's Knight Hair"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""shadowduat"", ""DoomKnight Dryden"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Metamophosis Maw's Knight Morph",
    @"
case ""Metamophosis Maw's Knight Morph"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""shadowduat"", ""DoomKnight Dryden"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Purified Energy Core",
    @"
case ""Purified Energy Core"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7236);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""technospace"", ""Technocaster Rogue"", ""Energy Core"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Seraphic Steel Plate",
    @"
case ""Seraphic Steel Plate"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7235);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""technospace"", ""Technowolf"", ""Seraphic Steel"", 5, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Pena mágica",
    @"
case ""Pena mágica"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""darkfesta"", ""Ultra Belo"", req.Name, quant, false, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Rainha da Bateria",
    @"
case ""Rainha da Bateria"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""darkfesta"", ""Dark Boitata"", req.Name, quant, false, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Rainha da Bateria Headdress",
    @"
case ""Rainha da Bateria Headdress"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""darkfesta"", ""Dark Boitata"", req.Name, quant, false, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Rainha da Bateria Headdress + Locks",
    @"
case ""Rainha da Bateria Headdress + Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""darkfesta"", ""Dark Boitata"", req.Name, quant, false, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Rainha da Bateria Staff",
    @"
case ""Rainha da Bateria Staff"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""darkfesta"", ""Dark Boitata"", req.Name, quant, false, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Rainha da Bateria Feathered Tail",
    @"
case ""Rainha da Bateria Feathered Tail"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""darkfesta"", ""Dark Boitata"", req.Name, quant, false, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Rainha da Bateria Feathers",
    @"
case ""Rainha da Bateria Feathers"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""darkfesta"", ""Dark Boitata"", req.Name, quant, false, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Ceremonial Standard",
    @"
case ""Ceremonial Standard"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9115);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""sambaflag"", ""Flag Bearer"", ""Flag Standard"");
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""sambaflag"", ""Master Of Ceremonies"", ""Ceremony Feather"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Costume Piece",
    @"
case ""Costume Piece"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9110);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""bloodtusk"", ""Jungle Vulture"", ""Vulture Feathers"", 8);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""bloodtusk"", ""Rhison"", ""Rhison Fur"", 8);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Pandeiro",
    @"
case ""Pandeiro"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""sambaflag"", ""Master Of Ceremonies"", req.Name, quant);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Tantan",
    @"
case ""Tantan"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""sambaflag"", ""Master Of Ceremonies"", req.Name, quant);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Cavaquinho",
    @"
case ""Cavaquinho"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""sambaflag"", ""Master Of Ceremonies"", req.Name, quant);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Spirit Beads",
    @"
case ""Spirit Beads"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7957);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""danceguru"", ""Crow"", ""Bead Shards"", 20, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Pássaro Rosa Cape",
    @"
case ""Pássaro Rosa Cape"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""danceguru"", ""Carnaval Harpy"", req.Name, quant, false, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Pano Azul",
    @"
case ""Pano Azul"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""danceguru"", ""Carnaval Harpy"", req.Name, quant, false, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Doom Essence",
    @"
case ""Doom Essence"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(6948);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""nursery"", ""Flesh Golem"", ""Treasure Found"", 10, true, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Golden Apple",
    @"
case ""Golden Apple"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(8793);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""extracredit"", ""Dogear"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Silver Ruler",
    @"
case ""Silver Ruler"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8790);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""extracredit"", ""Supply Locker"", ""Bookbag"", log: false);
                        Core.HuntMonster(""extracredit"", ""Supply Locker"", ""Pencil"", 3, log: false);
                        Core.HuntMonster(""extracredit"", ""Supply Locker"", ""Notebook"", 3, log: false);
                        Core.HuntMonster(""extracredit"", ""Grade A Bully"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Plastic Toy",
    @"
case ""Plastic Toy"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8791);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""extracredit"", ""Grade A Bully"", ""Bully Defeated"", 5, log: false);
                        Core.HuntMonster(""extracredit"", ""Meanest Girl"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Bronze Plaque",
    @"
case ""Bronze Plaque"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8792);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""extracredit"", ""Videogame Console"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Guava Sip",
    @"
case ""Guava Sip"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""extracredit"", ""Meanest Girl"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Purple Paddlepop",
    @"
case ""Purple Paddlepop"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""extracredit"", ""Meanest Girl"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Crystallis Trainer Locks",
    @"
case ""Crystallis Trainer Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""extracredit"", ""Meanest Girl"", req.Name, quant, false, false);
                    break;
    "
},
{
    "DogEar's Snack Serum",
    @"
case ""DogEar's Snack Serum"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""extracredit"", ""Dogear"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Raw Cookie Dough Blade",
    @"
case ""Raw Cookie Dough Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.GetMapItem(11646, quant, ""oaklore"");
                    break;
    "
},
{
    "Crystallis Trainer Hair",
    @"
case ""Crystallis Trainer Hair"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""extracredit"", ""Grade A Bully"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Bones",
    @"
case ""Bones"":
                    Core.FarmingLogger(req.Name, quant);

                    Core.HuntMonster(""nursery"", ""Skeletal Minion"", req.Name, quant, false, true);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Glue",
    @"
case ""Glue"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.HuntMonster(""nursery"", ""Flesh Golem"", req.Name, quant, false, true);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Darkness",
    @"
case ""Darkness"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.HuntMonster(""nursery"", ""Spilled Ink"", req.Name, quant, false, true);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Ragged Cloth Scrap",
    @"
case ""Ragged Cloth Scrap"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(8212);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        //Underworld Home Renovations 8212
                        Core.HuntMonster(""RotFinger"", ""Rotfinger"", ""Rotfinger Parts"", 3);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Deadfly's Armor",
    @"
case ""Deadfly's Armor"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""DeadFly"", ""Deadfly"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Deadfly Morph",
    @"
case ""Deadfly Morph"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""DeadFly"", ""Deadfly"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Rotfinger's Bow",
    @"
case ""Rotfinger's Bow"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""RotFinger"", ""Rotfinger"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Rotfinger's ArmBlades",
    @"
case ""Rotfinger's ArmBlades"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""RotFinger"", ""Rotfinger"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Rotfinger's Scythe",
    @"
case ""Rotfinger's Scythe"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""RotFinger"", ""Rotfinger"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Rotfinger's Staff",
    @"
case ""Rotfinger's Staff"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""RotFinger"", ""Rotfinger"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Spooky Fabric Scrap",
    @"
case ""Spooky Fabric Scrap"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8676);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""oddities"", ""r3"", ""Left"", ""*"", ""Cursed Cloth Roll"", 13, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Eerie Embellishment",
    @"
case ""Eerie Embellishment"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8677);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""oddities"", ""r9"", ""Left"", ""*"", ""Freaky Fripperies"", 13, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "GetchaDolla",
    @"
case ""GetchaDolla"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(6269);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""gonnagetcha"", ""Vengeful Ghost"", ""Ghost Gone"", 2, log: false);
                        Core.HuntMonster(""gonnagetcha"", ""Shrade Cultist"", ""Cultist Cleared"", 6, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "LoreTrek Token",
    @"
case ""LoreTrek Token"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""wormhole"", ""r2"", ""Left"", ""*"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Matted Dust Bunny",
    @"
case ""Matted Dust Bunny"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5067);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""wormhole"", ""r5"", ""Left"", ""Blue Trobbolier"", ""Blue Trobbolier Fluff"", 4);
                        Core.KillMonster(""wormhole"", ""r8"", ""Left"", ""Purple Trobbolier"", ""Purple Trobbolier Fluff"", 4);
                        Core.KillMonster(""wormhole"", ""r8"", ""Left"", ""Green Trobbolier"", ""Green Trobbolier Fluff"", 4);
                        Core.KillMonster(""wormhole"", ""r5"", ""Left"", ""Red Trobbolier"", ""Red Trobbolier Fluff"", 4);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "A Whisper",
    @"
case ""A Whisper"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9421, 9422);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.KillMonster(""shadowbattleon"", ""r7"", ""Left"", ""*"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Zenobia's Moglinberry Juice",
    @"
case ""Zenobia's Moglinberry Juice"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9056);
                    //9056 | Magic Dance
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""blackmaze"", ""Globlin"", ""Globlin Wings"", 7);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""blackmaze"", ""Vi'eel Dreaddacovra"", ""White Scale"");
                        Core.HuntMonster(""blackmaze"", ""Shadow Fernando"", ""Purple Flame"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Gift Ribbons",
    @"
case ""Gift Ribbons"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7829);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""goldenruins"", ""Golden Warrior"", ""Golden Warriors Trashed"", 6);
                        Core.HuntMonster(""goldenruins"", ""Maximillian Lionfang"", ""Lionfang Thrown Out"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Cosmic Dust",
    @"
case ""Cosmic Dust"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9678);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.KillMonster(""starsinc"", ""r2"", ""Left"", ""*"", ""Star Dust"", 30, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Cosmic Aura",
    @"
case ""Cosmic Aura"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9679);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""blackholesun"", ""Black Light Elemental"", ""Black Light Aura"", 7);
                        Core.KillMonster(""dreadspace"", ""r22"", ""Left"", ""Troblor"", ""Star Scrap Metal"", 10, isTemp: false);
                        // while (!Bot.ShouldExit && !Core.CheckInventory(""Star Scrap Metal"", 10))
                        // {
                        //     Core.EnsureAccept(4289);
                        //     Core.KillMonster(""dreadspace"", ""r20"", ""Right"", ""*"", ""Golden Spork of Justice"", log: false);
                        //     Core.EnsureCompleteMulti(4289);
                        //     Bot.Wait.ForPickup(""Star Scrap Metal"");
                        // }
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonsterMapID(""whitehole"", 49, ""Vortex Essence"", 12);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Astral Alignment Sword",
    @"
case ""Astral Alignment Sword"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""starfield"", ""Astral Spirit"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Astral Alignment Swords",
    @"
case ""Astral Alignment Swords"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""starfield"", ""Astral Spirit"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Fimbul's Frost",
    @"
case ""Fimbul's Frost"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9507);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""glacetomb"", ""Kriomein"", ""Valedictorian Speech"", log: false);
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""glacetomb"", ""Draugr"", ""Frozen Marrow"", 8, log: false);
                        Core.HuntMonster(""glacetomb"", ""Snow Fairy"", ""Crystalline Wings"", 8, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Fimbul's Crystal",
    @"
case ""Fimbul's Crystal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9519);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""fimbultomb"", ""Fimbulventr Witch"", ""Ice Crown"", log: false);
                        Core.HuntMonster(""fimbultomb"", ""Daselm"", ""Daselm's Thesis"", log: false);
                        Core.HuntMonster(""fimbultomb"", ""Peter"", ""Peter's Recc Letter"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Cursed Doll Tassel",
    @"
case ""Cursed Doll Tassel"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(8667);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.KillMonster(""oddities"", ""Enter"", ""Spawn"", ""*"", ""Chipped Wood"", 7, log: false);
                        Core.KillMonster(""oddities"", ""r6"", ""Left"", ""*"", ""Fuzz Tuff"", 7, log: false);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""oddities"", ""Cursed Spirit"", ""Doll Eyes"", 7, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Odd Coin",
    @"
case ""Odd Coin"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8674);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""oddities"", ""r6"", ""Left"", ""*"", ""Frankensteined Teddy"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Ectoplasmic Token",
    @"
case ""Ectoplasmic Token"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(8675);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""oddities"", ""Cursed Spirit"", ""Doll Eye"", 5, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Frostguarde Blade",
    @"
case ""Frostguarde Blade"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""glacetomb"", 7, req.Name, 1, req.Temp);
                    break;

    "
},
{
    "Frostguarde Blades",
    @"
case ""Frostguarde Blades"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""glacetomb"", 7, req.Name, 1, req.Temp);
                    break;

    "
},
{
    "Polaris Duelist Rapiers",
    @"
case ""Polaris Duelist Rapiers"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""glacetomb"", 7, req.Name, 1, req.Temp);
                    break;

    "
},
{
    "Polaris Duelist Rapier",
    @"
case ""Polaris Duelist Rapier"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""glacetomb"", 7, req.Name, 1, req.Temp);
                    break;

    "
},
{
    "Frost Shatter Spear",
    @"
case ""Frost Shatter Spear"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""glacetomb"", 7, req.Name, 1, req.Temp);
                    break;

    "
},
{
    "Aurum Wings Blade",
    @"
case ""Aurum Wings Blade"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""caroltown"", 6, req.Name, quant, false);
                    break;
    "
},
{
    "Brunswick Leo Scion",
    @"
case ""Brunswick Leo Scion"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""caroltown"", 6, req.Name, quant, false);
                    break;
    "
},
{
    "Brunswick Leo's Requiem",
    @"
case ""Brunswick Leo's Requiem"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""caroltown"", 6, req.Name, quant, false);
                    break;
    "
},
{
    "Brunswick Leo Scion Cane",
    @"
case ""Brunswick Leo Scion Cane"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""caroltown"", 6, req.Name, quant, false);
                    break;
    "
},
{
    "Spearmint Candy Cane",
    @"
case ""Spearmint Candy Cane"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""caroltown"", 6, req.Name, quant, false);
                    break;
    "
},
{
    "100 Pound Gift",
    @"
case ""100 Pound Gift"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""caroltown"", 6, req.Name, quant, false);
                    break;
    "
},
{
    "Chill Hoodie Outfit",
    @"
case ""Chill Hoodie Outfit"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""caroltown"", 6, req.Name, quant, false);
                    break;
    "
},
{
    "Chill Hat + Hair",
    @"
case ""Chill Hat + Hair"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""caroltown"", 6, req.Name, quant, false);
                    break;
    "
},
{
    "Chill Hat + Locks",
    @"
case ""Chill Hat + Locks"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""caroltown"", 6, req.Name, quant, false);
                    break;
    "
},
{
    "Chill Hat Visage",
    @"
case ""Chill Hat Visage"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""caroltown"", 6, req.Name, quant, false);
                    break;
    "
},
{
    "Chill Hat Morph",
    @"
case ""Chill Hat Morph"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""caroltown"", 6, req.Name, quant, false);
                    break;
    "
},
{
    "Red Ribbon",
    @"
case ""Red Ribbon"":
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.Join(""whitemap"");
                        Core.Join(""caroling"");

                        for (int killCount = 0; killCount < 3 && !Bot.ShouldExit; killCount++)
                        {
                            Bot.Kill.Monster(1);

                            Core.Logger($""Kill: {killCount + 1}/3, {(killCount < 2 ? ""Swapping Map at 3"" : ""Swapping map to respawn mob"")}"");
                            Bot.Wait.ForMonsterSpawn(1);
                        }

                        Core.Join(""whitemap"");
                        Core.Join(""caroling"");
                    }

                    break;
    "
},
{
    "Silver Tinsel",
    @"
case ""Silver Tinsel"":
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.Join(""whitemap"");
                        Core.Join(""caroling"");

                        for (int killCount = 0; killCount < 3 && !Bot.ShouldExit; killCount++)
                        {
                            Bot.Kill.Monster(1);

                            Core.Logger($""Kill: {killCount + 1}/3, {(killCount < 2 ? ""Swapping Map at 3"" : ""Swapping map to respawn mob"")}"");
                            Bot.Wait.ForMonsterSpawn(1);
                        }

                        Core.Join(""whitemap"");
                        Core.Join(""caroling"");
                    }

                    break;
    "
},
{
    "Jingle Bells",
    @"
case ""Jingle Bells"":
                    Core.EquipClass(ClassType.Farm);
                    // Jingle Spells - 9520
                    Core.RegisterQuests(9520);
                    Core.HuntMonster(""caroltown"", ""Frostval Deer"", req.Name, quant, false);
                    Bot.Wait.ForPickup(req.ID);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Icy Fur",
    @"
case ""Icy Fur"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""caroltown"", ""Krumpet"", req.Name, quant, false);
                    break;
    "
},
{
    "Wrapping Paper",
    @"
case ""Wrapping Paper"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""caroltown"", ""Krumpet"", req.Name, quant, false);
                    break;
    "
},
{
    "Sluagh Bell",
    @"
case ""Sluagh Bell"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8446, 8447, 8448);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.KillMonster(""otziwar"", ""r6"", ""Left"", ""Sluagh Warrior"", req.Name, quant);
                        Bot.Wait.ForPickup(req.ID);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Punk Coal Elf Stabber",
    @"
case ""Punk Coal Elf Stabber"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""otziwar"", 14, ""req.Name"", isTemp: false);
                    break;
    "
},
{
    "Festive Punk Elf Stabber",
    @"
case ""Festive Punk Elf Stabber"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""otziwar"", 14, ""req.Name"", isTemp: false);
                    break;
    "
},
{
    "Wild Huntress' Sword",
    @"
case ""Wild Huntress' Sword"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonsterMapID(""otziwar"", 14, ""req.Name"", isTemp: false);
                    break;
    "
},
{
    "Fire Starting Kit",
    @"
case ""Fire Starting Kit"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9016);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""snowview"", ""Vaderix"", ""Alien Mandible"", log: false);
                        Core.HuntMonster(""snowview"", ""Tundra Steed"", ""Horse Hair"", 7, log: false);
                        Core.HuntMonster(""snowview"", ""Mountain Owl"", ""Tinder Feathers"", 7, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Turkey Leg?",
    @"
case ""Turkey Leg?"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9027);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""snowviewrace"", ""Aurora Vaderix"", ""Aurora Wing"", log: false);
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""snowviewrace"", ""Bandit Fletcher"", ""Bandit Leader Bounty"", 3, log: false);
                        Core.HuntMonster(""snowviewrace"", ""Juvenile Vaderix"", ""Vaderix Drumstick"", 7, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Grief Medal",
    @"
case ""Grief Medal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    //Adv.BestGear(RacialGearBoost.Elemental);
                    Core.RegisterQuests(7856, 7857);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""winterhorror"", ""Chillybones"", ""Monster Gem"", 5);
                        Core.HuntMonster(""winterhorror"", ""Chillybones"", ""Mega Monster Gem"", 3);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Burnt Bow",
    @"
case ""Burnt Bow"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""icestorm"", ""Dragon Hunter"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Elegant Frostval Wrap",
    @"
case ""Elegant Frostval Wrap"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7262);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""frozensoul"", ""Frozen Minion"", ""Shard of Ice"", 10, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Hammered Ice",
    @"
case ""Hammered Ice"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7262);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""frozensoul"", ""Frozen Minion"", ""Shard of Ice"", 10, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Frosted Heart",
    @"
case ""Frosted Heart"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(7263);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""frozensoul"", ""Jack Frost"", ""Jack's Frosted Heart"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Cheery Frostvale Hat + Locks",
    @"
case ""Cheery Frostvale Hat + Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(7263);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""frozensoul"", ""Jack Frost"", ""Jack's Frosted Heart"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Cheery Frostvale Hat",
    @"
case ""Cheery Frostvale Hat"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(7263);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""frozensoul"", ""Jack Frost"", ""Jack's Frosted Heart"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Elegant Frostvale Suit",
    @"
case ""Elegant Frostvale Suit"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(7264);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""frozensoul"", ""r4"", ""Left"", ""*"", ""Queen's Frozen Soul"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Ruby Frostval Cane",
    @"
case ""Ruby Frostval Cane"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(7264);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""frozensoul"", ""r4"", ""Left"", ""*"", ""Queen's Frozen Soul"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Frozen Soul",
    @"
case ""Frozen Soul"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(7264);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""frozensoul"", ""r4"", ""Left"", ""*"", ""Queen's Frozen Soul"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Poleaxe of Kheimon",
    @"
case ""Poleaxe of Kheimon"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""frozensoul"", ""r4"", ""Left"", ""*"", req.Name, isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Frozen Rune of Kheimon",
    @"
case ""Frozen Rune of Kheimon"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""frozensoul"", ""r4"", ""Left"", ""*"", req.Name, isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Longevity Egg",
    @"
case ""Longevity Egg"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""elixirgrenwog"", ""Elixir Grenwog"", req.Name, quant, false);
                    break;

    "
},
{
    "Magical Marshmallow Cheep",
    @"
case ""Magical Marshmallow Cheep"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""deathgazer"", ""Deathgazer"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Gold-foil Chocolate Bunny",
    @"
case ""Gold-foil Chocolate Bunny"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""greendragon"", ""Greenguard Dragon"", req.Name, quant, isTemp: false);

                    break;
    "
},
{
    "Basketful of Dyed Eggs",
    @"
case ""Basketful of Dyed Eggs"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""trunk"", ""Greenguard Basilisk"", req.Name, quant, isTemp: false);

                    break;
    "
},
{
    "Berserker Bunny",
    @"
case ""Berserker Bunny"":
                    Farm.BerserkerBunny(0, sell: false);
                    break;

    "
},
{
    "Grenstory Token",
    @"
case ""Grenstory Token"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""grenstory"", ""Chinchilizard"", req.Name, quant, isTemp: false);
                    break;

                    

    "
},
{
    "Golden Branch",
    @"
case ""Golden Branch"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8420);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""helsgrove"", ""Krimpler"", ""Gilded Branch"", 3);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Hazel Switch",
    @"
case ""Hazel Switch"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8421);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""helsgrove"", ""Belsnickling"", ""Belsnickling Beaten"", 6);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Frostval Treat",
    @"
case ""Frostval Treat"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8421);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""helsgrove"", ""Belsnickling"", ""Belsnickling Beaten"", 6);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Chibi GroveRider's Locks + Bridle",
    @"
case ""Chibi GroveRider's Locks + Bridle"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""helsgrove"", ""Helsdottir"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Chibi GroveRider's Locks",
    @"
case ""Chibi GroveRider's Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""helsgrove"", ""Helsdottir"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Helsgrove Guardian Scarf",
    @"
case ""Helsgrove Guardian Scarf"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""helsgrove"", ""Helsdottir"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Fallen Leaf",
    @"
case ""Fallen Leaf"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""eventhub"", ""Leaf Painter"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Gold Flake",
    @"
case ""Gold Flake"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""blightharvest"", ""Tantalocust"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Gnarled Wood",
    @"
case ""Gnarled Wood"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""fearfeast"", ""OverGourd"", req.Name, quant, req.Temp, false);
                    break;

    "
},
{
    "Tin Can of ???",
    @"
case ""Tin Can of ???"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(8971);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ebiltakeover"", ""Smorgasbord"", req.Name, quant, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "PACVEC Mach 1.0",
    @"
case ""PACVEC Mach 1.0"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ebilmech"", ""Ebil Mech Dragon"", req.Name, quant, false);
                    break;
    "
},
{
    "PACVEC Helm",
    @"
case ""PACVEC Helm"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ebilmech"", ""Ebil Mech Dragon"", req.Name, quant, false);
                    break;
    "
},
{
    "PACVEC Visor",
    @"
case ""PACVEC Visor"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ebilmech"", ""Ebil Mech Dragon"", req.Name, quant, false);
                    break;
    "
},
{
    "PACVEC Guard",
    @"
case ""PACVEC Guard"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ebilmech"", ""Ebil Mech Dragon"", req.Name, quant, false);
                    break;
    "
},
{
    "PACVEC Battle Wings",
    @"
case ""PACVEC Battle Wings"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ebilmech"", ""Ebil Mech Dragon"", req.Name, quant, false);
                    break;
    "
},
{
    "PACVEC Battle Hammer",
    @"
case ""PACVEC Battle Hammer"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ebilmech"", ""Ebil Mech Dragon"", req.Name, quant, false);
                    break;
    "
},
{
    "PACVEC Railgun",
    @"
case ""PACVEC Railgun"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ebilmech"", ""Ebil Mech Dragon"", req.Name, quant, false);
                    break;
    "
},
{
    "PACVEC Alien",
    @"
case ""PACVEC Alien"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ebilmech"", ""Ebil Mech Dragon"", req.Name, quant, false);
                    break;
    "
},
{
    "Chocolate Eggshells",
    @"
case ""Chocolate Eggshells"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""GreenguardEast"", ""Gurushroom"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Creme Eggshells",
    @"
case ""Creme Eggshells"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""GreenShell"", ""Tsukumogami"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Caramel Eggshells",
    @"
case ""Caramel Eggshells"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""GreenguardWest"", ""Kittarian"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Rainbow Eggshells",
    @"
case ""Rainbow Eggshells"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""Greendragon"", ""Greenguard Dragon"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Shadow Eggshells",
    @"
case ""Shadow Eggshells"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""Grenwog"", ""Grenwog"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "DraGrenwog Scale",
    @"
case ""DraGrenwog Scale"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""Grenwog"", ""Grenwog"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Chaotic Eggshells",
    @"
case ""Chaotic Eggshells"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""Grenstory"", ""Imposter Egg"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Golden Eggshells",
    @"
case ""Golden Eggshells"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""Greed"", ""Treasure Pile"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Anti-Neggshells",
    @"
case ""Anti-Neggshells"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""Greymoor"", ""Spooky Treeant"", req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Stymphalian's Bronze Feather",
    @"
case ""Stymphalian's Bronze Feather"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    // Muck and Feather 9462
                    Core.RegisterQuests(9462);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterMapID(""birdswithharms"", 33, ""Bronze Feather"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Pink Balloon Scrap",
    @"
case ""Pink Balloon Scrap"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""float"", ""Beleen Balloon"", req.Name, quant);
                    break;

    "
},
{
    "Green Balloon Scrap",
    @"
case ""Green Balloon Scrap"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""float"", ""Beleen Balloon"", req.Name, quant);
                    break;

    "
},
{
    "Red Balloon Scrap",
    @"
case ""Red Balloon Scrap"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""float"", ""Beleen Balloon"", req.Name, quant);
                    break;

    "
},
{
    "Rubber Egg",
    @"
case ""Rubber Egg"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""grenwogwarren"", UseableMonsters[0], req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Sugary Egg",
    @"
case ""Sugary Egg"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""grenwogwarren"", UseableMonsters[1], req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Scaly Egg",
    @"
case ""Scaly Egg"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""grenwogwarren"", UseableMonsters[2], req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Aged Egg",
    @"
case ""Aged Egg"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""grenwogwarren"", UseableMonsters[3], req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Liquid Egg",
    @"
case ""Liquid Egg"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""grenwogwarren"", UseableMonsters[4], req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Cabdury Egg",
    @"
case ""Cabdury Egg"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""grenwogwarren"", UseableMonsters[5], req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;


    "
},
{
    "Emergency Rations",
    @"
case ""Emergency Rations"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(9975,
                        (""manaharvest"", CHD.UMManaHarvest[1], ClassType.Farm),
                        (""manaharvest"", CHD.UMManaHarvest[7], ClassType.Solo),
                        (""manaharvest"", CHD.UMManaHarvest[4], ClassType.Farm)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Glossly Chestnut Waves",
    @"
case ""Glossly Chestnut Waves"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""manaharvest"", CHD.UMManaHarvest[7], req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Glossy Chestnut Locks",
    @"
case ""Glossy Chestnut Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""manaharvest"", CHD.UMManaHarvest[7], req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Earplug",
    @"
case ""Earplug"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7808);
                    // Quiet Down, Will Ya? 7808
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""memetnightmare"", ""Fire Cyclone"", ""Cyclones Subdued"", 8);
                        Core.HuntMonster(""memetnightmare"", ""Burning Ember"", ""Embers Smothered"", 8);
                        Core.HuntMonster(""memetnightmare"", ""Cannibal Mermaid"", ""Mermaids Dispersed"", 8);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Nightmare Medal",
    @"
case ""Nightmare Medal"":
                    HarvestDay.NightmareWar();
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7809, 7810);
                    // Murderbug Medal 7809
                    // Mega Murderbug Medal 7810
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""nightmarewar"", ""Zombie Cicada"", ""Murderbug Medal"", 5);
                        Core.HuntMonster(""nightmarewar"", ""Zombie Cicada"", ""Mega Murderbug Medal"", 3);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Blight Bone",
    @"
case ""Blight Bone"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.Name);
                    Core.RegisterQuests(9482);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.KillMonster(""blightharvest"", ""r5"", ""Left"", ""Tantalocust"", ""Mealy Bug Legs"", 6);
                        Core.KillMonster(""blightharvest"", ""r7"", ""Left"", ""Fear Gorta"", ""Hunger Grass"", 6);
                        Core.EquipClass(ClassType.Solo);
                        Core.KillMonster(""blightharvest"", ""r10"", ""Left"", ""Famine"", ""Famine's Spice Flakes"");
                        Core.Logger(""This item is not setup yet"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Turdraken Carver",
    @"
case ""Turdraken Carver"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.KillMonster(""blightharvest"", ""r5"", ""Left"", ""*"", req.Name, isTemp: req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Harvest Rifle",
    @"
case ""Harvest Rifle"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.KillMonster(""blightharvest"", ""r5"", ""Left"", ""*"", req.Name, isTemp: req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Turdraken Carvers",
    @"
case ""Turdraken Carvers"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.KillMonster(""blightharvest"", ""r5"", ""Left"", ""*"", req.Name, isTemp: req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Vintage Shotgun",
    @"
case ""Vintage Shotgun"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.KillMonster(""blightharvest"", ""r5"", ""Left"", ""*"", req.Name, isTemp: req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Cranberry Shoulder Imp",
    @"
case ""Cranberry Shoulder Imp"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.KillMonster(""blightharvest"", ""r5"", ""Left"", ""*"", req.Name, isTemp: req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "EbilCoin",
    @"
case ""EbilCoin"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    //Core.RegisterQuests(8408); // Problem Exists Between Chairman and Keyboard
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(8408);
                        Core.HuntMonster(""ebilcorphq"", ""Master Chairman"", ""Master Chairman Destroyed (again)"", 10);
                        Core.EnsureCompleteMulti(8408);
                    }
                    //Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Ebil Operative",
    @"
case ""Ebil Operative"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ebilcorphq"", ""Master Chairman"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Ebil Operative Hair + Mask",
    @"
case ""Ebil Operative Hair + Mask"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ebilcorphq"", ""Master Chairman"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Ebil Operative Protective Gear",
    @"
case ""Ebil Operative Protective Gear"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ebilcorphq"", ""Master Chairman"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Ebil Operative Hair + Muffler",
    @"
case ""Ebil Operative Hair + Muffler"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ebilcorphq"", ""Master Chairman"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Ebil Operative Hair + Scarf",
    @"
case ""Ebil Operative Hair + Scarf"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ebilcorphq"", ""Master Chairman"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Ebil Operative Edgy Hair + Mask",
    @"
case ""Ebil Operative Edgy Hair + Mask"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ebilcorphq"", ""Master Chairman"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Ebil Operative Edgy Protective Gear",
    @"
case ""Ebil Operative Edgy Protective Gear"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ebilcorphq"", ""Master Chairman"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Ebil Operative Edgy Hair + Scarf",
    @"
case ""Ebil Operative Edgy Hair + Scarf"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ebilcorphq"", ""Master Chairman"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Ebil Operative Edgy Hair + Muffler",
    @"
case ""Ebil Operative Edgy Hair + Muffler"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ebilcorphq"", ""Master Chairman"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Ebil Operative Mask Helm",
    @"
case ""Ebil Operative Mask Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ebilcorphq"", ""Master Chairman"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Ebil Operative Helm + Scarf",
    @"
case ""Ebil Operative Helm + Scarf"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ebilcorphq"", ""Master Chairman"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Ebil Operative Hood + Mask",
    @"
case ""Ebil Operative Hood + Mask"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ebilcorphq"", ""Master Chairman"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Ebil Operative Crossed Baton Blades",
    @"
case ""Ebil Operative Crossed Baton Blades"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ebilcorphq"", ""Master Chairman"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Ebil Operative Cape",
    @"
case ""Ebil Operative Cape"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ebilcorphq"", ""Master Chairman"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Ebil Operative Baton Blade",
    @"
case ""Ebil Operative Baton Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ebilcorphq"", ""Master Chairman"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Ebil Operative Death Blade",
    @"
case ""Ebil Operative Death Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ebilcorphq"", ""Master Chairman"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Ebil Operative Long Baton",
    @"
case ""Ebil Operative Long Baton"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ebilcorphq"", ""Master Chairman"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Ebil Operative Tactical Rifle",
    @"
case ""Ebil Operative Tactical Rifle"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""ebilcorphq"", ""Master Chairman"", req.Name, isTemp: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Heart-Shaped Gem",
    @"
case ""Heart-Shaped Gem"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""tunneloflove"", ""Love Knight"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Lovely Laurel",
    @"
case ""Lovely Laurel"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuest(10072,
                        (""tunneloflove"", ""Love Knight"", ClassType.Farm),
                        (""tunneloflove"", ""Oubliette"", ClassType.Solo),
                        (""tunneloflove"", ""Rosey Moth"", ClassType.Farm)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Burning Flame",
    @"
case ""Burning Flame"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""tunneloflove"", ""Galanoth"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Moth-Spun Silk",
    @"
case ""Moth-Spun Silk"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""tunneloflove"", ""Rosey Moth"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Pink Diamond",
    @"
case ""Pink Diamond"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""tunneloflove"", ""Oubliette"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Silphium Love Potions",
    @"
case ""Silphium Love Potions"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""tunneloflove"", ""Oubliette"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Goredon's Zard Sauce",
    @"
case ""Goredon's Zard Sauce"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""feastboss"", ""Goredon Rampage"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Harvest Golem Parfait",
    @"
case ""Harvest Golem Parfait"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""feastwarevil"", ""Harvest Golem"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Ultra Turdrakogiblet",
    @"
case ""Ultra Turdrakogiblet"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""killerkitchen"", ""Ultra Turdrakolich"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Wretched Rider Meat",
    @"
case ""Wretched Rider Meat"":
                    HarvestDay.FoulFarm();
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dullahan"", ""Wretched Rider"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Overgourd Seed",
    @"
case ""Overgourd Seed"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""fearfeast"", ""OverGourd"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Autumnal Civilian Hair",
    @"
case ""Autumnal Civilian Hair"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""birdswithharms"", ""Rawrgobble"", req.Name, quant, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Autumnal Locks",
    @"
case ""Autumnal Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""birdswithharms"", ""Rawrgobble"", req.Name, quant, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Scarbucks Pumpkin Spice Latte",
    @"
case ""Scarbucks Pumpkin Spice Latte"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""birdswithharms"", ""Rawrgobble"", req.Name, quant, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Maple Leaf",
    @"
case ""Maple Leaf"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""birdswithharms"", ""Rawrgobble"", req.Name, quant, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Burnt Feather",
    @"
case ""Burnt Feather"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""birdswithharms"", ""Rawrgobble"", req.Name, quant, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Scarbucks Pumpkin Pie",
    @"
case ""Scarbucks Pumpkin Pie"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""birdswithharms"", ""Rawrgobble"", req.Name, quant, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Autumnal Civilian",
    @"
case ""Autumnal Civilian"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""birdswithharms"", ""Rawrgobble"", req.Name, quant, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "TurKing Claw",
    @"
case ""TurKing Claw"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                        Core.KillMonster(""birdswithharms"", ""r10"", ""Left"", ""TurKing"", log: false);
                    Bot.Wait.ForPickup(req.ID);
                    break;
    "
},
{
    "Guncraft Shadowslayer Big Irons",
    @"
case ""Guncraft Shadowslayer Big Irons"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""ebilmech"", ""Ebil Mech Dragon"", req.Name, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Guncraft Shadowslayer Big Iron",
    @"
case ""Guncraft Shadowslayer Big Iron"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""ebilmech"", ""Ebil Mech Dragon"", req.Name, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Wub Charm",
    @"
case ""Wub Charm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""wubblevania"", ""Charmed Alina"", req.Name, quant, false, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Chocolate Tail",
    @"
case ""Chocolate Tail"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""wubblevania"", ""Mr. Wubbles"", req.Name, quant, false, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Furry Heart",
    @"
case ""Furry Heart"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""wubblevania"", ""Mr. Wubbles"", req.Name, quant, false, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Beleen's Gratitude",
    @"
case ""Beleen's Gratitude"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    if (Core.IsMember)
                    {
                        Core.RegisterQuests(7355);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""canalshore"", ""Fishwing"", ""Fishwing Defeated"", 6, log: false);
                            Core.HuntMonster(""canalshore"", ""MerSiren"", ""MerSiren Defeated"", 8, log: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    else
                    {
                        Core.RegisterQuests(7351);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""canalshore"", ""Water Elemental"", ""Water Elemental Defeated"", 5, log: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Townspeople's Affection",
    @"
case ""Townspeople's Affection"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    if (Core.IsMember)
                    {
                        Core.RegisterQuests(7355);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""canalshore"", ""Fishwing"", ""Fishwing Defeated"", 6, log: false);
                            Core.HuntMonster(""canalshore"", ""MerSiren"", ""MerSiren Defeated"", 8, log: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    else
                    {
                        Core.RegisterQuests(7350);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""canalshore"", ""Trapped Snack"", ""Civilian Rescued"", 4, log: false);
                            Core.HuntMonster(""canalshore"", ""MerSiren"", ""MerSiren Defeated"", 5, log: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Astice's Claw",
    @"
case ""Astice's Claw"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    if (Core.IsMember)
                    {
                        Core.RegisterQuests(7356);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""canalshore"", ""Astice"", ""Broken Claw"", log: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    else
                    {
                        Core.RegisterQuests(7352);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.HuntMonster(""canalshore"", ""Astice"", ""Astice Defeated"", log: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Rangda's Mask",
    @"
case ""Rangda's Mask"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Bot.Quests.UpdateQuest(7622);
                    Core.HuntMonster(""rangda"", ""Rangda"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Abhorrent Remnant",
    @"
case ""Abhorrent Remnant"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""rangda"", ""Tuyul"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Wentiran Seal",
    @"
case ""Wentiran Seal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9342);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""wentira"", ""Pesugihan Boar"", ""Boar Leather"", 6, log: false);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""wentira"", ""Kabasaran Waranei"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Gold Nugget",
    @"
case ""Gold Nugget"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""wentira"", ""Pesugihan Boar"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Broken Tusk",
    @"
case ""Broken Tusk"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""wentira"", ""Pesugihan Boar"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Beloved Blessing Hair",
    @"
case ""Beloved Blessing Hair"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""wentira"", ""Pesugihan Boar"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Uncut Ruby",
    @"
case ""Uncut Ruby"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""wentira"", ""Pesugihan Boar"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Beloved Blessing Locks",
    @"
case ""Beloved Blessing Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""wentira"", ""Pesugihan Boar"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Blessed Beloved's Kris Knife",
    @"
case ""Blessed Beloved's Kris Knife"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""wentira"", ""Pesugihan Boar"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Wiracana Fan",
    @"
case ""Wiracana Fan"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""wentira"", ""Pesugihan Boar"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Ancient Bone",
    @"
case ""Ancient Bone"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""wentira"", ""Kabasaran Waranei"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Golden Ticket",
    @"
case ""Golden Ticket"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.Join(""luck"");
                        Core.SendPackets(""%xt%zm%getMapItem%10173%101%"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;

    "
},
{
    "Makai Token",
    @"
case ""Makai Token"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.Join(""luck"");
                        Core.SendPackets(""%xt%zm%getMapItem%10173%5679%"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Lovely Silk",
    @"
case ""Lovely Silk"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9588);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.KillMonster(""yguasu"", ""r3"", ""Left"", ""*"", ""Giggling Mask"", 10, log: false);
                        Core.KillMonster(""yguasu"", ""r4"", ""Left"", ""*"", ""Wolfman Talisman"", 10, log: false);

                        Core.EquipClass(ClassType.Solo);
                        Core.KillMonster(""yguasu"", ""r5"", ""Left"", ""*"", ""M'Boi's Throat"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Bolsa da Mãe D'água",
    @"
case ""Bolsa da Mãe D'água"":
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                        Core.KillMonster(""yguasu"", ""r5"", ""Left"", ""M'Boi"", log: false);
                    Bot.Wait.ForPickup(req.ID);
                    break;
    "
},
{
    "Espelho da Mãe D'água",
    @"
case ""Espelho da Mãe D'água"":
                    Core.FarmingLogger(req.Name, quant);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonsterQuestChoose(9610, 83981,
                        (""canalshore"", ""Trapped Snack"", ClassType.Farm),
                        (""cursedshop"", ""Ghost Vase"", ClassType.Farm),
                        (""terradefesta"", ""Baron Sunday"", ClassType.Solo));
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Golden Coupon",
    @"
case ""Golden Coupon"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonsterMapID(""luckdragon"", 2, req.Name, quant, isTemp: false, log: false);
                    break;

    "
},
{
    "Obliterator Droid's Generator",
    @"
case ""Obliterator Droid's Generator"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        if (Core.IsMember)
                            Core.HuntMonsterQuest(10229, ""twigguhunt"", ""Obliterator Droid"");
                        Core.HuntMonsterQuest(10228, ""twigguhunt"", ""Obliterator Droid"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Droid Scrap",
    @"
case ""Droid Scrap"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""twigguhunt"", ""r2"", ""Down"", ""*"", req.Name, quant, req.Temp, false);
                    break;

    "
},
{
    "Salvaged Droid Part",
    @"
case ""Salvaged Droid Part"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9703);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""twigguhunt"", ""r2"", ""Down"", ""*"", ""Broken Droid Part"", 300, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "GL-1ST",
    @"
case ""GL-1ST"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""twigguhunt"", ""Twiggu"", req.Name, quant, false, false);
                    break;
    "
},
{
    "GL-1ST Helm",
    @"
case ""GL-1ST Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""twigguhunt"", ""Twiggu"", req.Name, quant, false, false);
                    break;
    "
},
{
    "GL-1ST Mask",
    @"
case ""GL-1ST Mask"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""twigguhunt"", ""Twiggu"", req.Name, quant, false, false);
                    break;
    "
},
{
    "GL-1ST Visor",
    @"
case ""GL-1ST Visor"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""twigguhunt"", ""Twiggu"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Seraphic Fourth Morph",
    @"
case ""Seraphic Fourth Morph"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""twigguhunt"", ""Twiggu"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Seraphic Fourth Beard Morph",
    @"
case ""Seraphic Fourth Beard Morph"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""twigguhunt"", ""Twiggu"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Seraphic Fourth Visage",
    @"
case ""Seraphic Fourth Visage"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""twigguhunt"", ""Twiggu"", req.Name, quant, false, false);
                    break;
    "
},
{
    "GL-1ST Pronged Spear",
    @"
case ""GL-1ST Pronged Spear"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""twigguhunt"", ""Bodyguard Droid"", req.Name, quant, false, false);
                    break;
    "
},
{
    "GL-1ST Pronged Spears",
    @"
case ""GL-1ST Pronged Spears"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""twigguhunt"", ""Bodyguard Droid"", req.Name, quant, false, false);
                    break;
    "
},
{
    "GL-1ST Salvage Axe",
    @"
case ""GL-1ST Salvage Axe"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""twigguhunt"", ""Scout Droid"", req.Name, quant, false, false);
                    break;
    "
},
{
    "GL-1ST Salvage Axes",
    @"
case ""GL-1ST Salvage Axes"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""twigguhunt"", ""Scout Droid"", req.Name, quant, false, false);
                    break;
    "
},
{
    "GL-1ST Salvage Gun",
    @"
case ""GL-1ST Salvage Gun"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""twigguhunt"", ""Infantry Droid"", req.Name, quant, false, false);
                    break;
    "
},
{
    "GL-1ST Salvage Guns",
    @"
case ""GL-1ST Salvage Guns"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""twigguhunt"", ""Infantry Droid"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Furry Egg",
    @"
case ""Furry Egg"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""zorbaspalace"", ""Lem-or"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Woopee",
    @"
case ""Woopee"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""zorbaspalace"", ""Thwompcat"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Cyber Crystal",
    @"
case ""Cyber Crystal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8065);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.KillMonster(""murdermoon"", ""r2"", ""Left"", ""Tempest Soldier"", ""Tempest Soldier Badge"", 5, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Fifth Lord's Filtrinator",
    @"
case ""Fifth Lord's Filtrinator"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""murdermoon"", ""Fifth Sepulchure"", req.Name, quant, false, false);
                    break;
    "
},
{
    "S Ring",
    @"
case ""S Ring"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""murdermoon"", ""Fifth Sepulchure"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Dotty",
    @"
case ""Dotty"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""zorbaspalace"", ""Zorba the Bakk"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Dark Helmet",
    @"
case ""Dark Helmet"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""zorbaspalace"", ""Zorba the Bakk"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Dark Tempest Soldier",
    @"
case ""Dark Tempest Soldier"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""murdermoon"", ""Tempest Soldier"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Dark Tempest Soldier Helm",
    @"
case ""Dark Tempest Soldier Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""murdermoon"", ""Tempest Soldier"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Dark Tempest Soldier Jetpack",
    @"
case ""Dark Tempest Soldier Jetpack"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""murdermoon"", ""Tempest Soldier"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Dark Tempest Soldier Blaster",
    @"
case ""Dark Tempest Soldier Blaster"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""murdermoon"", ""Tempest Soldier"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Dark Tempest Soldier Laserblade",
    @"
case ""Dark Tempest Soldier Laserblade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""murdermoon"", ""Tempest Soldier"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Dark Tempest Soldier Pet",
    @"
case ""Dark Tempest Soldier Pet"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""murdermoon"", ""Tempest Soldier"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Dark Tempest Soldier Mask",
    @"
case ""Dark Tempest Soldier Mask"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""murdermoon"", ""Tempest Soldier"", req.Name, quant, false, false);
                    break;

    "
},
{
    "White Oval",
    @"
case ""White Oval"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7375);
                        Core.HuntMonster(""greed"", ""Goregold"", ""Stolen Gem Found"", log: false);
                        Core.EnsureComplete(7375, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Blush Brilliant",
    @"
case ""Blush Brilliant"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7375);
                        Core.HuntMonster(""greed"", ""Goregold"", ""Stolen Gem Found"", log: false);
                        Core.EnsureComplete(7375, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Pink Pear",
    @"
case ""Pink Pear"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7375);
                        Core.HuntMonster(""greed"", ""Goregold"", ""Stolen Gem Found"", log: false);
                        Core.EnsureComplete(7375, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Half Rose",
    @"
case ""Half Rose"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7375);
                        Core.HuntMonster(""greed"", ""Goregold"", ""Stolen Gem Found"", log: false);
                        Core.EnsureComplete(7375, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Cerise Trillian",
    @"
case ""Cerise Trillian"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7375);
                        Core.HuntMonster(""greed"", ""Goregold"", ""Stolen Gem Found"", log: false);
                        Core.EnsureComplete(7375, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Ruby Heart",
    @"
case ""Ruby Heart"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7375);
                        Core.HuntMonster(""greed"", ""Goregold"", ""Stolen Gem Found"", log: false);
                        Core.EnsureComplete(7375, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "White Box",
    @"
case ""White Box"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7369);
                        Core.HuntMonster(""pastelia"", ""Cutie Makai"", ""White Box Found"", log: false);
                        Core.EnsureComplete(7369, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Oval Setting",
    @"
case ""Oval Setting"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7369);
                        Core.HuntMonster(""pastelia"", ""Cutie Makai"", ""White Box Found"", log: false);
                        Core.EnsureComplete(7369, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Sparkles",
    @"
case ""Sparkles"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7369);
                        Core.HuntMonster(""pastelia"", ""Cutie Makai"", ""White Box Found"", log: false);
                        Core.EnsureComplete(7369, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Blush Box",
    @"
case ""Blush Box"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7370);
                        Core.HuntMonster(""dwarfhold"", ""Gemrald"", ""Blush Box Found"", log: false);
                        Core.EnsureComplete(7370, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Brilliant Setting",
    @"
case ""Brilliant Setting"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7370);
                        Core.HuntMonster(""dwarfhold"", ""Gemrald"", ""Blush Box Found"", log: false);
                        Core.EnsureComplete(7370, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Pink Box",
    @"
case ""Pink Box"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7371);
                        Core.HuntMonster(""earthstorm"", ""Amethite"", ""Pink Box Found"", log: false);
                        Core.EnsureComplete(7371, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Pear Setting",
    @"
case ""Pear Setting"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7371);
                        Core.HuntMonster(""earthstorm"", ""Amethite"", ""Pink Box Found"", log: false);
                        Core.EnsureComplete(7371, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Rose Box",
    @"
case ""Rose Box"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7372);
                        Core.HuntMonster(""stalagbite"", ""Stalagbite"", ""Rose Box Found"", log: false);
                        Core.EnsureComplete(7372, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Half Rose Setting",
    @"
case ""Half Rose Setting"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7372);
                        Core.HuntMonster(""stalagbite"", ""Stalagbite"", ""Rose Box Found"", log: false);
                        Core.EnsureComplete(7372, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Cerise Box",
    @"
case ""Cerise Box"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7373);
                        Core.HuntMonster(""castleofglass"", ""Chihuly"", ""Cerise Box Found"", log: false);
                        Core.EnsureComplete(7373, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Trillian Setting",
    @"
case ""Trillian Setting"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7373);
                        Core.HuntMonster(""castleofglass"", ""Chihuly"", ""Cerise Box Found"", log: false);
                        Core.EnsureComplete(7373, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Ruby Box",
    @"
case ""Ruby Box"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7374);
                        Core.HuntMonster(""beleensdream"", ""Heart Elemental"", ""Ruby Box Found"", log: false);
                        Core.EnsureComplete(7374, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Heart Setting",
    @"
case ""Heart Setting"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7374);
                        Core.HuntMonster(""beleensdream"", ""Heart Elemental"", ""Ruby Box Found"", log: false);
                        Core.EnsureComplete(7374, req.ID);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Lucky Clover",
    @"
case ""Lucky Clover"":

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant) && Daily.CheckDailyv2(Core.CheckInventory(971) ? 1761 : 1759))
                    {
                        Core.EnsureAccept(Core.CheckInventory(971) ? 1761 : 1759);
                        Core.HuntMonster(""rainbow"", ""Lucky Harms"", ""Clover Leaves"");
                        Core.EnsureComplete(Core.CheckInventory(971) ? 1761 : 1759);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();

                    if (!Core.CheckInventory(req.Name, quant))
                    {
                        Core.Logger($""not enough {req.Name}. Run this again tomarrow!"");
                        break;
                    }
                    break;
    "
},
{
    "Rainbow Shard",
    @"
case ""Rainbow Shard"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(1758);
                    Core.KillMonster(""rainbow"", ""Well"", ""Left"", ""Rainbow Rat"", req.Name, quant, isTemp: false, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Khonsu Seal",
    @"
case ""Khonsu Seal"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""cruxship"", ""Apephryx"", req.Name, quant, false);
                    break;

    "
},
{
    "Fwog Egg",
    @"
case ""Fwog Egg"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9223);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""murdermoon"", ""Tempest Soldier"", isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Astravian Enforcer Crescent Halo",
    @"
case ""Astravian Enforcer Crescent Halo"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""murdermoon"", ""Fifth Sepulchure"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Large Hoverpram Shard",
    @"
case ""Large Hoverpram Shard"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""zorbaspit"", ""Zorblatt"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Hoverpram Fragments",
    @"
case ""Hoverpram Fragments"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""zorbaspit"", ""Zorblatt"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Light Blade of the Rebellion",
    @"
case ""Light Blade of the Rebellion"":
                    Core.FarmingLogger(req.Name, quant);
                    Farm.GoodREP();
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(8648);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Z.AssembledSword();
                        Core.HuntMonster(""greed"", ""Goregold"", ""Goregold Resisted"", isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Zorblatt's Pizza Slice",
    @"
case ""Zorblatt's Pizza Slice"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(8651);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""zorbaspit"", ""Zorblatt"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Dark Blade of the Fifth",
    @"
case ""Dark Blade of the Fifth"":
                    Core.FarmingLogger(req.Name, quant);
                    Farm.EvilREP();
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(8649);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Z.AssembledSword();
                        Core.HuntMonster(""murdermoon"", ""Fifth Sepulchure"", ""Fifth Sepulchure Defeated"", isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Chaos Blade of the Imperium",
    @"
case ""Chaos Blade of the Imperium"":
                    Core.FarmingLogger(req.Name, quant);
                    Farm.ChaosREP();
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(8650);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Z.AssembledSword();
                        Core.HuntMonster(""ledgermayne"", ""Ledgermayne"", ""Ledgermayne Defeated"", isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Orange Dye",
    @"
case ""Orange Dye"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(6538);
                        Core.HuntMonster(""chromafection"", ""Chromafection"", ""Candy Dye"", 3);
                        Core.EnsureComplete(6538, req.ID);
                    }
                    break;

    "
},
{
    "Yellow Dye",
    @"
case ""Yellow Dye"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(6538);
                        Core.HuntMonster(""chromafection"", ""Chromafection"", ""Candy Dye"", 3);
                        Core.EnsureComplete(6538, req.ID);
                    }
                    break;

    "
},
{
    "Black Dye",
    @"
case ""Black Dye"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(6538);
                        Core.HuntMonster(""chromafection"", ""Chromafection"", ""Candy Dye"", 3);
                        Core.EnsureComplete(6538, req.ID);
                    }
                    break;

    "
},
{
    "Purple Dye",
    @"
case ""Purple Dye"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(6538);
                        Core.HuntMonster(""chromafection"", ""Chromafection"", ""Candy Dye"", 3);
                        Core.EnsureComplete(6538, req.ID);
                    }
                    break;

    "
},
{
    "Pink Dye",
    @"
case ""Pink Dye"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(6538);
                        Core.HuntMonster(""chromafection"", ""Chromafection"", ""Candy Dye"", 3);
                        Core.EnsureComplete(6538, req.ID);
                    }
                    break;

    "
},
{
    "Royce's Direclaw",
    @"
case ""Royce's Direclaw"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""crescentmoon"", ""Royce"", req.Name, quant, false);
                    break;
    "
},
{
    "Spectral Lycan",
    @"
case ""Spectral Lycan"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""crescentmoon"", ""Royce"", req.Name, quant, false);
                    break;
    "
},
{
    "Spectral Lycan's Hood",
    @"
case ""Spectral Lycan's Hood"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""crescentmoon"", ""Royce"", req.Name, quant, false);
                    break;
    "
},
{
    "Spectral Lycan's Morph",
    @"
case ""Spectral Lycan's Morph"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""crescentmoon"", ""Royce"", req.Name, quant, false);
                    break;
    "
},
{
    "Spectral Lycan's Backfur",
    @"
case ""Spectral Lycan's Backfur"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""crescentmoon"", ""Royce"", req.Name, quant, false);
                    break;
    "
},
{
    "Howling Spectral Lycan",
    @"
case ""Howling Spectral Lycan"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""crescentmoon"", ""Royce"", req.Name, quant, false);
                    break;
    "
},
{
    "Spectral Ground Flames",
    @"
case ""Spectral Ground Flames"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""crescentmoon"", ""Royce"", req.Name, quant, false);
                    break;
    "
},
{
    "Spectral Lycan's Spear",
    @"
case ""Spectral Lycan's Spear"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""crescentmoon"", ""Royce"", req.Name, quant, false);
                    break;
    "
},
{
    "Spectral Lycan's Claws",
    @"
case ""Spectral Lycan's Claws"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""crescentmoon"", ""Royce"", req.Name, quant, false);
                    break;
    "
},
{
    "Glowball",
    @"
case ""Glowball"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    //Survive Frankenwerepire! 3163
                    Core.RegisterQuests(3163);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""franken"", ""Frankenwerepire"", ""Defeat Frankenwerepire"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Candy Dragon Egg",
    @"
case ""Candy Dragon Egg"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    // Host of Honor 9455
                    Core.RegisterQuests(9455);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""cursedcastle"", ""Luminous Fungus"", ""Grilled Shroom Caps"", 6);
                        Core.HuntMonster(""cursedcastle"", ""Noble Gargoyle"", ""Decorated Gargoyle"", 6);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""cursedcastle"", ""Unborn Brood"", ""Unborn Brood Defeated"");

                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Jiangshi",
    @"
case ""Jiangshi"":
                    Core.HuntMonster(""cursedcastle"", ""Noble Ghost"", req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Jiangshi Hair",
    @"
case ""Jiangshi Hair"":
                    Core.HuntMonster(""cursedcastle"", ""Noble Ghost"", req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Jiangshi Cap",
    @"
case ""Jiangshi Cap"":
                    Core.HuntMonster(""cursedcastle"", ""Noble Ghost"", req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Jiangshi Locks",
    @"
case ""Jiangshi Locks"":
                    Core.HuntMonster(""cursedcastle"", ""Noble Ghost"", req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Jiangshi Bandages",
    @"
case ""Jiangshi Bandages"":
                    Core.HuntMonster(""cursedcastle"", ""Noble Ghost"", req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Jiangshi Talisman Locks",
    @"
case ""Jiangshi Talisman Locks"":
                    Core.HuntMonster(""cursedcastle"", ""Noble Ghost"", req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Jiangshi Talisman Hair",
    @"
case ""Jiangshi Talisman Hair"":
                    Core.HuntMonster(""cursedcastle"", ""Noble Ghost"", req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Jiangshi Hat",
    @"
case ""Jiangshi Hat"":
                    Core.HuntMonster(""cursedcastle"", ""Noble Ghost"", req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Treats",
    @"
case ""Treats"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.AddDrop(""Treats"");
                        Core.Join(""tricktown"");
                        Core.KillMonster(""trickortreat"", ""Enter"", ""Spawn"", ""Trick or Treater"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Ghastly Gummy",
    @"
case ""Ghastly Gummy"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(8936); // Ghoul Gang 8936
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""tricktown"", ""Madam Ester"", ""Crystalized Slime"", 1);
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""tricktown"", ""Decay Spirit"", ""Decay Essence"", 10);
                        Core.HuntMonster(""tricktown"", ""Rotting Mound"", ""Melty Scabs"", 10);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Blood Moon Token",
    @"
case ""Blood Moon Token"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    // Core.RegisterQuests(Core.IsMember ? 6060 : 6059); // uncomment when registerquest is fixed. if more then 1 item is found in inv it only complets once then afks/
                    while (!Bot.ShouldExit && !Core.CheckInventory(""Blood Moon Token"", quant))
                    {
                        Core.EnsureAccept(Core.IsMember ? 6060 : 6059);
                        Core.KillMonster(""bloodmoon"", ""r12a"", ""Left"", ""Black Unicorn"", ""Black Blood Vial"", isTemp: false);
                        Core.KillMonster(""bloodmoon"", ""r4a"", ""Left"", ""Lycan Guard"", ""Moon Stone"", isTemp: false);
                        Core.EnsureComplete(Core.IsMember ? 6060 : 6059);
                        Bot.Wait.ForPickup(""Blood Moon Token"");
                    }
                    break;

    "
},
{
    "MarshMeowllows",
    @"
case ""MarshMeowllows"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    //Chocolate and Caramel Cravings 7120
                    Core.RegisterQuests(7120);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""twigsarcade"", ""Clucky Moo"", ""Chocolate Candy"", 10);
                        Core.KillMonster(""pie"", ""r5"", ""Left"", ""Gourdo"", ""Pumpkin Caramel"", 10);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Horehound Bits",
    @"
case ""Horehound Bits"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    //Loads of Gummies and Lollies 7121
                    Core.RegisterQuests(7121);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""twigsarcade"", ""Baby"", ""Gummy Babies"", 10);
                        Core.HuntMonster(""pie"", ""Myst Imp"", ""Licked Lollies"", 10);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Kitty Cordials",
    @"
case ""Kitty Cordials"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    //Fizzies and Stickies and Gooies OH MY 7122
                    Core.RegisterQuests(7122);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""twigsarcade"", ""Ectoplasm"", ""GOO-dies"", 6);
                        Core.HuntMonster(""chromafection"", ""Free Samples"", ""Sour Stickies"", 6);
                        Core.HuntMonster(""candyshop"", ""Sugarrush Ghoul"", ""Fuzzy Fizzies"", 6);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Candied Jalapeno",
    @"
case ""Candied Jalapeno"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""candyshop"", ""Sugarrush Ghoul"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Spicy Sample",
    @"
case ""Spicy Sample"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""candyshop"", ""Sugarrush Ghoul"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Tactical Agent Alpha",
    @"
case ""Tactical Agent Alpha"":
                    Core.HuntMonster(""mogloweengrave"", ""Zombie Terror"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Tactical Agent Bravo",
    @"
case ""Tactical Agent Bravo"":
                    Core.HuntMonster(""mogloweengrave"", ""Zombie Terror"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Tactical Agent Bravo Beard",
    @"
case ""Tactical Agent Bravo Beard"":
                    Core.HuntMonster(""mogloweengrave"", ""Zombie Terror"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Tactical Agent Bravo Locks",
    @"
case ""Tactical Agent Bravo Locks"":
                    Core.HuntMonster(""mogloweengrave"", ""Zombie Terror"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Tactical Alpha Rifle",
    @"
case ""Tactical Alpha Rifle"":
                    Core.HuntMonster(""mogloweengrave"", ""Zombie Terror"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Tactical Alpha Rifles",
    @"
case ""Tactical Alpha Rifles"":
                    Core.HuntMonster(""mogloweengrave"", ""Zombie Terror"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Backup Ol Reliable Zombie Buster",
    @"
case ""Backup Ol Reliable Zombie Buster"":
                    Core.HuntMonster(""mogloweengrave"", ""Zombie Terror"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Gummy Brains",
    @"
case ""Gummy Brains"":
                    Core.HuntMonster(""mogloweengrave"", ""Zombie Terror"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Chrono Gem",
    @"
case ""Chrono Gem"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9536);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""chronogem"", ""Gem Forgemaster"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "ChronoSand",
    @"
case ""ChronoSand"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9033);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""timeretaliate"", $""Min’et’s Corpse"", ""Min'et Death Mask"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "StasisGlass",
    @"
case ""StasisGlass"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9034);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""timeretaliate"", ""Retrograde Maw"", ""Maw's Flesh"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Keeper of the Amazon",
    @"
case ""Keeper of the Amazon"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(8261);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""iara"", ""Iara"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Iara Insignia",
    @"
case ""Iara Insignia"":
                    Core.Logger($""{req.Name}"" + "" requires ultra boss, you need to farm it manually."");
                    break;

    "
},
{
    "Lol-E-Pop",
    @"
case ""Lol-E-Pop"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""candycorn"", ""r2"", ""Right"", ""*"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Box-o-Chocolates",
    @"
case ""Box-o-Chocolates"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""candycorn"", ""r2"", ""Right"", ""*"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Candy Corn",
    @"
case ""Candy Corn"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""candycorn"", ""r2"", ""Right"", ""*"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Ghostly Cape",
    @"
case ""Ghostly Cape"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""mogloween"", ""Ghostly Sheet"", req.Name, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Cursed Bone Club",
    @"
case ""Cursed Bone Club"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""candycorn"", ""Stalkwalker"", req.Name, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Ivy Blade",
    @"
case ""Ivy Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""mogloween"", ""Pumpkinhead Fred"", req.Name, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Blister's Chainsaw",
    @"
case ""Blister's Chainsaw"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""mogloween"", ""Blister"", req.Name, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Medusa Curse",
    @"
case ""Medusa Curse"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.BuyItem(""mogloween"", 30, req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Sinister Pumpkin Sickles",
    @"
case ""Sinister Pumpkin Sickles"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""candycorn"", ""Field Guardian"", req.Name, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Great Pumpkin King Sword",
    @"
case ""Great Pumpkin King Sword"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""mogloween"", ""Great Pumpkin King"", req.Name, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Water Drop",
    @"
case ""Water Drop"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(6814, 6816);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""WaterWar"", ""Solar Elemental"");
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Solar Badge",
    @"
case ""Solar Badge"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""WaterWar"", ""Aloe"", req.Name, quant, false);

                    break;

    "
},
{
    "Aegis Armor",
    @"
case ""Aegis Armor"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""cathedral"", ""Skeletal Warrior"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Aegis Robe",
    @"
case ""Aegis Robe"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""cathedral"", ""Infernal Knight"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Aegis Ward",
    @"
case ""Aegis Ward"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""cathedral"", ""Pactagonal Knight"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Blessed Metal",
    @"
case ""Blessed Metal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""cathedral"", ""Corrupted Sentry"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Golden Faceplate",
    @"
case ""Golden Faceplate"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""cathedral"", ""Flying Pieball"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Data Scroll",
    @"
case ""Data Scroll"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""cathedral"", ""Data Glitch"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Time Key",
    @"
case ""Time Key"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""cathedral"", ""Incarnation of Time"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Poeira do Saci",
    @"
case ""Poeira do Saci"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(7682);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""mythperception"", ""Saci"", ""Trapped Saci"");
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Ossos do Corpo-Seco",
    @"
case ""Ossos do Corpo-Seco"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7683);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""mythperception"", ""Corpo-Seco"", ""Corpo-Seco's Nails"", 5);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Escamas da Cuca",
    @"
case ""Escamas da Cuca"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7684);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""mythperception"", ""Cuca"", ""Cuca's Hat"", quant);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Pink Gem of the Sea",
    @"
case ""Pink Gem of the Sea"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""mythperception"", ""Boto"", req.Name, quant);
                    break;
    "
},
{
    "Cuca's Dye",
    @"
case ""Cuca's Dye"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""mythperception"", ""Cuca"", req.Name, quant);
                    break;

    "
},
{
    "Abyssal Medallion",
    @"
case ""Abyssal Medallion"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(7392);
                    Core.HuntMonster(""abysslair"", ""Devourer of Souls"", req.Name, quant, false, false);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Abyssal Scale",
    @"
case ""Abyssal Scale"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7391);
                    Core.HuntMonster(""abysslair"", ""Abyssal Underbeast"", req.Name, quant, false, false);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Coldfire Gem",
    @"
case ""Coldfire Gem"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7390);
                    Core.HuntMonster(""abysslair"", ""Abyssal Guard"", req.Name, quant, false, false);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Coin For the Dead",
    @"
case ""Coin For the Dead"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9633);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""cocytusbarracks"", ""Maleagant"", ""Aestiua Shard"", log: false);
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""cocytusbarracks"", ""Cerberus Pup"", ""Phlegethon Tag"", 8, log: false);
                        Core.HuntMonster(""cocytusbarracks"", ""Mourner"", ""Lethe Wreath"", 8, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Weapon Reflection",
    @"
case ""Weapon Reflection"":
                    Core.AddDrop(req.Name);
                    if (!Core.CheckInventory(req.Name))
                    {
                        Core.EnsureAccept(5518);
                        Core.HuntMonster(""nostalgiaquest"", ""Skeletal Viking"", ""Reflected Glory"", 5);
                        Core.HuntMonster(""nostalgiaquest"", ""Skeletal Warrior"", ""Divided Light"", 5);
                        Core.EnsureComplete(5518);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Boom Went The Dynamite",
    @"
case ""Boom Went The Dynamite"":
                    if (!Core.IsMember)
                        return;
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""banished"", ""Desterrat Moya"", req.Name, quant, false);
                    break;
    "
},
{
    "TheWicked",
    @"
case ""TheWicked"":
                    Core.Logger($""You don't own {req.Name} (Rare)"");
                    break;
    "
},
{
    "Oblivion of Nulgath",
    @"
case ""Oblivion of Nulgath"":
                    juggernaut.JuggItems(reward: JuggernautItemsofNulgath.RewardsSelection.Oblivion_of_Nulgath);
                    break;
    "
},
{
    "Overlord's DoomBlade",
    @"
case ""Overlord's DoomBlade"":
                    if (Core.HasAchievement(27, ""ip0""))
                        Core.BuyItem(Bot.Map.Name, 340, req.Name);
                    else
                        Core.Logger($""You don't have access to this shop for {req.Name}"");
                    break;
    "
},
{
    "Party Slasher Birthday Sword",
    @"
case ""Party Slasher Birthday Sword"":
                    Core.Logger($""You don't own {req.Name} (Rare)"");
                    break;
    "
},
{
    "Rapier of Skulls",
    @"
case ""Rapier of Skulls"":
                    Core.Logger($""You don't own {req.Name} (Rare)"");
                    break;
    "
},
{
    "Frostbite",
    @"
case ""Frostbite"":
                    if (!Core.IsMember || (!Core.isCompletedBefore(793)))
                    {
                        Core.Logger($""You require Membership for {req.Name}, or you're not part of the Legion"");
                        return;
                    }
                    if (!Core.CheckInventory(""Frosted Falchion""))
                        Adv.BuyItem(""blindingsnow"", 236, ""Frosted Falchion"");
                    Legion.FarmLegionToken(70);
                    Adv.BuyItem(""underworld"", 238, req.Name);
                    break;
    "
},
{
    "A Rock",
    @"
case ""A Rock"":
                    if (!Bot.Inventory.Contains(req.Name))
                    {
                        Core.Logger($""You don't own {req.Name} (Rare)"");
                        return;
                    }
                    break;
    "
},
{
    "Phoenix Blade of Nulgath",
    @"
case ""Phoenix Blade of Nulgath"":
                    //  5373 = Oblivion Blade of Nulgath (Pet) ---- 4809 = Oblivion Blade of Nulgath Pet (Rare)
                    if (!Core.IsMember || (!Core.CheckInventory(5373)) && (!Core.CheckInventory(4809)))
                    {
                        Core.Logger($""You don't own any of the pets/Membership to get {req.Name}"");
                        return;
                    }

                    if (Core.CheckInventory(5373))
                        Core.EnsureAccept(2558);
                    else if (Core.CheckInventory(4809))
                        Core.EnsureAccept(558);
                    Core.AddDrop(Nation.bagDrops);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""lair"", ""Red Dragon"", ""Phoenix Blade"", isTemp: false);
                    Nation.FarmDarkCrystalShard(5);
                    Nation.FarmDiamondofNulgath(10);
                    Nation.FarmTaintedGem(5);
                    Nation.FarmUni13(1);
                    Core.HuntMonster(""underworld"", ""Undead Bruiser"", ""Undead Bruiser Sigil"");
                    Core.AddDrop(req.Name);
                    if (Core.CheckInventory(5373))
                        Core.EnsureComplete(2558);
                    else if (Core.CheckInventory(4809))
                        Core.EnsureComplete(558);
                    Core.RemoveDrop(Nation.bagDrops);
                    Core.ToBank(Nation.bagDrops);
                    break;
    "
},
{
    "Shadow Spear of Nulgath",
    @"
case ""Shadow Spear of Nulgath"":
                    if (!Core.CheckInventory(req.Name))
                    {
                        Core.Logger($""You don't own {req.Name} (Rare)"");
                        return;
                    }
                    break;
    "
},
{
    "Guardian of Virtue",
    @"
case ""Guardian of Virtue"":
                    if (!Core.CheckInventory(req.Name))
                    {
                        Core.Logger($""You don't own {req.Name} (Rare)"");
                        return;
                    }
                    break;
    "
},
{
    "Leviasea Sword",
    @"
case ""Leviasea Sword"":
                    if (!Core.CheckInventory(req.Name) && !Core.IsMember)
                    {
                        Core.Logger($""You require Membership for {req.Name}"");
                        return;
                    }
                    if (!Core.CheckInventory(req.Name) && Core.IsMember)
                        Adv.BuyItem(""yulgar"", 69, req.Name);
                    break;
    "
},
{
    "Blood Axe Of Destruction",
    @"
case ""Blood Axe Of Destruction"":
                    Core.EquipClass(ClassType.Farm);
                    if (!Core.CheckInventory(req.Name))
                        Core.KillMonster(""infernalspire"", ""r2"", ""Left"", ""*"", req.Name, isTemp: false);
                    break;
    "
},
{
    "PainSaw of Eidolon",
    @"
case ""PainSaw of Eidolon"":
                    if (!Core.CheckInventory(""Undead Champion""))
                    {
                        Core.Logger($""You don't own Undead Champion - go and complete the Legion intro (requires 1200 AC)"");
                        return;
                    }
                    Core.AddDrop(""PainSaw of Eidolon"", ""Judgement Scythe"", ""Soul Eater Advanced"", ""Legion Token"");
                    Core.RegisterQuests(824);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && (!Core.CheckInventory(req.Name)))
                    {
                        Core.KillMonster(""marsh2"", ""End"", ""Left"", 72, ""Soul Scythe"", 1, false);
                        Core.KillMonster(""marsh2"", ""End"", ""Left"", ""Lesser Shadow Serpent"", ""Potent Viper's Blood"");
                        Core.HuntMonster(""battleundera"", ""Skeletal Ice Mage"", ""Frostbit Skull"", 15);
                    }
                    if (Core.CheckInventory(""Judgement Scythe""))
                        Core.ToBank(""Judgement Scythe"");
                    if (Core.CheckInventory(""Soul Eater Advanced""))
                        Core.ToBank(""Soul Eater Advanced"");
                    Core.ToBank(""Legion Token"");
                    break;
    "
},
{
    "Hanzamune Dragon Koi Blade",
    @"
case ""Hanzamune Dragon Koi Blade"":
                    Core.EquipClass(ClassType.Solo);
                    Core.KillKitsune(req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Ugly Stick",
    @"
case ""Ugly Stick"":
                    if (!Core.CheckInventory(req.Name))
                        Adv.BuyItem(""newbie"", 39, req.Name);
                    break;
    "
},
{
    "Balrog Blade",
    @"
case ""Balrog Blade"":
                    if (Core.HasAchievement(5))
                        Adv.BuyItem(Bot.Map.Name, 5, req.Name);
                    else
                        Core.Logger($""You don't have access to this shop for {req.Name}"");

                    break;
    "
},
{
    "Legendary Magma Sword",
    @"
case ""Legendary Magma Sword"":
                    if (!Core.CheckInventory(req.Name))
                    {
                        Core.Logger($""You don't own {req.Name} (Rare)"");
                        return;
                    }
                    break;
    "
},
{
    "Dragon Saw",
    @"
case ""Dragon Saw"":
                    if (!Core.CheckInventory(req.Name))
                        Adv.BuyItem(""yulgar"", 16, req.Name);
                    break;
    "
},
{
    "Bone Sword",
    @"
case ""Bone Sword"":
                    if (!Core.CheckInventory(req.Name))
                    {
                        Core.EnsureAccept(7);
                        Core.RegisterQuests(5);
                        Core.AddDrop(req.Name);
                        while (!Bot.ShouldExit && !Bot.TempInv.Contains(""Small Skull"", 8))
                            Core.HuntMonster(""graveyard"", ""Big Jack Sprat"");
                        Core.EnsureComplete(7);
                    }
                    break;
    "
},
{
    "Honor Guard's Blade",
    @"
case ""Honor Guard's Blade"":
                    if (!Core.CheckInventory(req.Name))
                    {
                        Core.Logger($""You don't own {req.Name} (Rare)"");
                        return;
                    }
                    break;
    "
},
{
    "Ceremonial Legion Blade",
    @"
case ""Ceremonial Legion Blade"":
                    if (!Core.CheckInventory(req.Name))
                    {
                        Core.Logger($""You don't own {req.Name} (Rare)"");
                        return;
                    }
                    break;
    "
},
{
    "Alteon's Pride",
    @"
case ""Alteon's Pride"":
                    if (!Core.CheckInventory(req.Name))
                    {
                        if (Farm.FactionRank(""Good"") < 7)
                            Farm.GoodREP(7);
                        Adv.BuyItem(""castle"", 88, req.Name);
                    }
                    break;
    "
},
{
    "Ddog Sea Serpent Sword",
    @"
case ""Ddog Sea Serpent Sword"":
                    if (!Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires Membership to obtain"");
                        return;
                    }
                    Core.EnsureAccept(554);
                    Nation.FarmUni13(1);
                    Core.HuntMonster(""underworld"", ""Undead Legend"", ""Undead Legend Rune"", log: false);
                    Core.EnsureCompleteChoose(554, new[] { ""Ddog Sea Serpent Sword"" });
                    break;
    "
},
{
    "Eternity Blade",
    @"
case ""Eternity Blade"":
                    Core.EquipClass(ClassType.Solo);
                    Core.EnsureAccept(3485);
                    Core.HuntMonster(""towerofdoom10"", ""Slugbutter"", ""Eternity Blade"");
                    Core.EnsureComplete(3485);
                    break;
    "
},
{
    "Blinding Light of Destiny",
    @"
case ""Blinding Light of Destiny"":
                    if (!Core.CheckInventory(req.Name))
                    {
                        Core.Logger($""Go and get {req.Name} yourself."");
                        return;
                    }
                    break;
    "
},
{
    "Crystal Claymore",
    @"
case ""Crystal Claymore"":
                    Adv.BuyItem(""castle"", 48, req.Name);
                    break;
    "
},
{
    "Dark Crystal Claymore",
    @"
case ""Dark Crystal Claymore"":
                    Adv.BuyItem(""shadowfall"", 47, req.Name);
                    break;
    "
},
{
    "Soulreaper of Nulgath",
    @"
case ""Soulreaper of Nulgath"":
                    if (!Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires Membership to obtain"");
                        return;
                    }
                    Core.AddDrop(req.Name);
                    Core.EnsureAccept(571);
                    if (!Core.CheckInventory(""Godly Golden Dragon Axe""))
                    {
                        Core.EnsureAccept(554);
                        Nation.FarmUni13(1);
                        Core.HuntMonster(""underworld"", ""Undead Legend"", ""Undead Legend Rune"", log: false);
                        Core.EnsureCompleteChoose(554, new[] { ""Godly Golden Dragon Axe"" });
                    }
                    Nation.FarmDiamondofNulgath(10);
                    Nation.FarmDarkCrystalShard(5);
                    Nation.FarmTaintedGem(5);
                    Nation.FarmUni13(1);
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.CheckInventory(""Abaddon's Terror""))
                        Core.HuntMonster(""twilight"", ""Abaddon"", ""Abaddon's Terror"", isTemp: false);
                    Core.EnsureComplete(571);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Grumpy Warhammer",
    @"
case ""Grumpy Warhammer"":
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.CheckInventory(req.Name))
                        Core.HuntMonster(""boxes"", ""Sneeviltron"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Maximillian's Whip",
    @"
case ""Maximillian's Whip"":
                    if (!Core.CheckInventory(req.Name))
                    {
                        Core.Logger($""You don't own {req.Name} (Rare)"");
                        return;
                    }
                    break;
    "
},
{
    "Mystic Pencil of Endless Scribbles",
    @"
case ""Mystic Pencil of Endless Scribbles"":
                    if (!Core.CheckInventory(req.Name))
                    {
                        Core.Logger($""You don't own {req.Name} (Rare)"");
                        return;
                    }
                    break;
    "
},
{
    "WarpForce War Shovel 20K",
    @"
case ""WarpForce War Shovel 20K"":
                    if (!Core.CheckInventory(req.Name))
                    {
                        Core.Logger($""You don't own {req.Name} (Rare)"");
                        return;
                    }
                    break;
    "
},
{
    "Godly Mace of the Ancients",
    @"
case ""Godly Mace of the Ancients"":
                    if (!Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires Membership to obtain"");
                        return;
                    }
                    if (!Core.CheckInventory(req.Name))
                        Adv.BuyItem(""citadel"", 44, req.Name);
                    break;
    "
},
{
    "Mace of the Grand Inquisitor",
    @"
case ""Mace of the Grand Inquisitor"":
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.CheckInventory(req.Name))
                        Core.HuntMonster(""citadel"", ""Grand Inquisitor"", req.Name, isTemp: false);
                    break;
    "
},
{
    "KneeCapper",
    @"
case ""KneeCapper"":
                    if (!Core.CheckInventory(req.Name))
                    {
                        Core.Logger($""You don't own {req.Name} (Rare)"");
                        return;
                    }
                    break;
    "
},
{
    "Morning Star",
    @"
case ""Morning Star"":
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.CheckInventory(req.Name))
                        Core.HuntMonster(""forest"", ""Boss Zardman"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Axe of the Black Knight",
    @"
case ""Axe of the Black Knight"":
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.CheckInventory(req.Name))
                        Core.KillMonster(""greenguardwest"", ""BKWest15"", ""Down"", ""Black Knight"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Cruel Axe of Midnight",
    @"
case ""Cruel Axe of Midnight"":
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.CheckInventory(req.Name))
                        Core.KillMonster(""greenguardwest"", ""BKWest15"", ""Down"", ""Black Knight"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Platinum Axe of Destiny",
    @"
case ""Platinum Axe of Destiny"":
                    if (!Core.CheckInventory(req.Name))
                    {
                        Core.Logger($""You don't own {req.Name} (Rare)"");
                        return;
                    }
                    break;
    "
},
{
    "Star Sword",
    @"
case ""Star Sword"":
                    if (!Core.CheckInventory(req.Name))
                    {
                        Core.Logger($""You don't own {req.Name} (Rare)"");
                        return;
                    }
                    break;
    "
},
{
    "Big 100K",
    @"
case ""Big 100K"":
                    if (!Core.CheckInventory(req.Name))
                        Adv.BuyItem(""swordhaven"", 3, req.Name);
                    break;
    "
},
{
    "Blister's Chainsaw 08",
    @"
case ""Blister's Chainsaw 08"":
                    if (!Core.CheckInventory(req.Name))
                    {
                        Core.Logger($""You don't own {req.Name} (Rare)"");
                        return;
                    }
                    break;
    "
},
{
    "Golden Phoenix Sword",
    @"
case ""Golden Phoenix Sword"":
                    if (!Core.CheckInventory(req.Name))
                    {
                        Core.Logger($""You don't own {req.Name} (Rare)"");
                        return;
                    }
                    break;
    "
},
{
    "Hydra Blade",
    @"
case ""Hydra Blade"":
                    if (!Core.CheckInventory(req.Name))
                        Adv.BuyItem(""swordhaven"", 4, req.Name);
                    break;
    "
},
{
    "Crusader Sword",
    @"
case ""Crusader Sword"":
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.CheckInventory(req.Name))
                        Core.HuntMonster(""citadel"", ""Crusader"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Bloodriver",
    @"
case ""Bloodriver"":
                    if (!Core.CheckInventory(req.Name))
                    {
                        Core.Logger($""You don't own {req.Name} (Rare)"");
                        return;
                    }
                    break;
    "
},
{
    "Star Sword Breaker",
    @"
case ""Star Sword Breaker"":
                    if (!Core.CheckInventory(req.Name))
                    {
                        Core.Logger($""You don't own {req.Name} (Rare)"");
                        return;
                    }
                    break;
    "
},
{
    "ReignBringer",
    @"
case ""ReignBringer"":
                    if (!Core.CheckInventory(req.Name))
                        Adv.BuyItem(""swordhaven"", 4, req.Name);
                    break;
    "
},
{
    "Balor's Cruelty",
    @"
case ""Balor's Cruelty"":
                    if (!Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires Membership to obtain"");
                        return;
                    }
                    Core.EquipClass(ClassType.Solo);
                    if (!Core.CheckInventory(req.Name))
                        Core.HuntMonster(""twilight"", ""Abaddon"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Default Sword",
    @"
case ""Default Sword"":
                    Adv.BuyItem(""yulgar"", 16, req.Name);
                    break;
    "
},
{
    "Iron Spear",
    @"
case ""Iron Spear"":
                    Adv.BuyItem(""yulgar"", 16, req.Name);
                    break;
    "
},
{
    "Undead Plague Spear",
    @"
case ""Undead Plague Spear"":
                    if (Core.HasAchievement(5))
                        Adv.BuyItem(Bot.Map.Name, 5, req.Name);
                    else
                        Core.HuntMonster(""graveyard"", ""Big Jack Sprat"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Mighty Sword Of The Dragons",
    @"
case ""Mighty Sword Of The Dragons"":
                    if (!Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires Membership to obtain"");
                        return;
                    }
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(""Zellare's Death Scale"", ""Moganth's Death Scale"", ""Udaroth's Death Scale"", ""Cellot's Death Scale"", ""Mighty Sword Of The Dragons"");
                    Core.RegisterQuests(3343);
                    Bot.Quests.UpdateQuest(1416);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name))
                    {
                        Core.HuntMonster(""wind"", ""Cellot"", ""Cellot's Death Scale"", isTemp: false);
                        Core.HuntMonster(""fire"", ""Zellare"", ""Zellare's Death Scale"", isTemp: false);
                        Core.HuntMonster(""water"", ""Udaroth"", ""Udaroth's Death Scale"", isTemp: false);
                        Core.HuntMonster(""dragonplane"", ""Moganth"", ""Moganth's Death Scale"", isTemp: false);
                    }
                    break;
    "
},
{
    "Necrotic Sword of Doom",
    @"
case ""Necrotic Sword of Doom"":
                    Core.Logger($""Go and get {req.Name} yourself."");
                    break;
    "
},
{
    "Burning Blade Of Abezeth",
    @"
case ""Burning Blade Of Abezeth"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""celestialarenad"", ""Aranx"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Abaddon's Terror",
    @"
case ""Abaddon's Terror"":
                    if (!Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires Membership to obtain"");
                        return;
                    }
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""twilight"", ""Abaddon"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Krom's Brutality",
    @"
case ""Krom's Brutality"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""forest"", ""Boss Zardman"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Phoenix Blade",
    @"
case ""Phoenix Blade"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""lair"", ""Red Dragon"", ""Phoenix Blade"", isTemp: false);
                    break;
    "
},
{
    "Burn it Down",
    @"
case ""Burn it Down"":
                    if (!Daily.CheckDailyv2(187, true, true, req.Name))
                    {
                        Core.Logger($""{req.Name} owned, or daily unavailable"");
                        return;
                    }
                    Core.AddDrop(req.Name);
                    Core.EnsureAccept(187);
                    Core.EquipClass(ClassType.Farm);
                    Core.KillMonster(""portalundead"", ""Enter"", ""Spawn"", ""*"", ""Fire Gem"");
                    Core.EnsureComplete(187);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Shadow Terror Axe",
    @"
case ""Shadow Terror Axe"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""battleundera"", ""Bone Terror"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Soul Terror Sword",
    @"
case ""Soul Terror Sword"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""battleundera"", ""Bone Terror"", req.Name, isTemp: false);
                    break;
    "
},
{
    "ShadowReaper Of Doom",
    @"
case ""ShadowReaper Of Doom"":
                    SRoD.ShadowReaperOfDoom();
                    break;
    "
},
{
    "Cysero's Potato",
    @"
case ""Cysero's Potato"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5528);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""nostalgiaquest"", ""Zardman Grunt"", ""Enchanted Rubber Ducky"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Kuro's Wrath",
    @"
case ""Kuro's Wrath"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""river"", ""Kuro"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Lilith Katana",
    @"
case ""Lilith Katana"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""elemental"", ""Mana Golem"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Mammoth Crusher Blade",
    @"
case ""Mammoth Crusher Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""lair"", ""Bronze Draconian"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Light Prismatic Katana",
    @"
case ""Light Prismatic Katana"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""akiba"", 131, req.Name, quant);
                    break;
    "
},
{
    "Excavated Glaive: Sword",
    @"
case ""Excavated Glaive: Sword"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""artistalley"", 753, req.Name, quant);
                    break;
    "
},
{
    "Golden Blade of Fate",
    @"
case ""Golden Blade of Fate"":
                    Core.FarmingLogger(req.Name, quant);
                    GBOF.GetGBoF();
                    break;
    "
},
{
    "Blade of Affliction",
    @"
case ""Blade of Affliction"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.BuyItem(""Tercessuinotlim"", 68, req.Name, quant);
                    break;
    "
},
{
    "Hex Blade of Nulgath",
    @"
case ""Hex Blade of Nulgath"":
                    Nation.NulgathLarvae(req.Name, quant);
                    break;
    "
},
{
    "Bane of Nulgath",
    @"
case ""Bane of Nulgath"":
                    Nation.NulgathLarvae(req.Name, quant);
                    break;
    "
},
{
    "Shadowworn",
    @"
case ""Shadowworn"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""shadowrealmpast"", ""Shadow Lord"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Hollowborn Oblivion Blade",
    @"
case ""Hollowborn Oblivion Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    HOB.GetBlade();
                    break;
    "
},
{
    "Loyalty Blade of the Nation",
    @"
case ""Loyalty Blade of the Nation"":
                    NDM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Katana of Revontheus",
    @"
case ""Katana of Revontheus"":
                    Core.Logger($""{req.Name} is rare, it can't be farmed."");
                    break;
    "
},
{
    "Risoluto",
    @"
case ""Risoluto"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""necrocavern"", ""Chaos Vordred"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Fiendish Blood Blade",
    @"
case ""Fiendish Blood Blade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""bludrut4"", ""Groglurk"", req.Name, quant, false, false);
                    break;
    "
},
{
    "SpiritHunter Katana",
    @"
case ""SpiritHunter Katana"":
                    SHM.BuyAllMerge(req.Name);
                    break;

    "
},
{
    "Cyber Skull",
    @"
case ""Cyber Skull"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""futurewardage"", ""SF3017 Paragonator"", req.Name, quant, log: false);
                    break;

    "
},
{
    "UnDeath Core",
    @"
case ""UnDeath Core"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""futurewardage"", ""SF3017 Paragonator"", req.Name, quant, log: false);
                    break;

    "
},
{
    "Astral Entity",
    @"
case ""Astral Entity"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""Ledgermayne"", ""Ledgermayne"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Pink Potion",
    @"
case ""Pink Potion"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""chateau"", ""Pinky"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Mr. Cuddles Pet",
    @"
case ""Mr. Cuddles Pet"":
                    Core.HuntMonster(""lovelockdown"", ""Ultra Cuddles"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Mr. Cuddles on your Head",
    @"
case ""Mr. Cuddles on your Head"":
                    Core.HuntMonster(""lovelockdown"", ""Ultra Cuddles"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Fists of Fire",
    @"
case ""Fists of Fire"":
                    Core.HuntMonster(""xancave"", ""Shurpu Ring Guardian"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Like a Battlemoglin",
    @"
case ""Like a Battlemoglin"":
                    Adv.BuyItem(""ariapet"", 12, req.Name);
                    break;
    "
},
{
    "Green Sockatana",
    @"
case ""Green Sockatana"":
                    CyseroMerge.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Chainsaw Katana",
    @"
case ""Chainsaw Katana"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""darkoviahorde"", ""Zombie Dragon"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "BaconCat Force Face",
    @"
case ""BaconCat Force Face"":
                    Adv.BuyItem(""baconcatlair"", 1260, req.Name);
                    break;
    "
},
{
    "Kitty SkyFighter",
    @"
case ""Kitty SkyFighter"":
                    Adv.BuyItem(""baconcatlair"", 1260, req.Name);
                    break;
    "
},
{
    "Ebil Ninja",
    @"
case ""Ebil Ninja"":
                    ArtixWeddingMerge.BuyAllMerge(req.Name);
                    break;

    "
},
{
    "Ebil Ninja Hood",
    @"
case ""Ebil Ninja Hood"":
                    ArtixWeddingMerge.BuyAllMerge(req.Name);
                    break;

    "
},
{
    "Legion Defender Medal",
    @"
case ""Legion Defender Medal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8584, 8585);
                    Core.KillMonster(""darkwarlegion"", ""r2"", ""Left"", ""Dreadfiend"", req.Name, quant, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Legion War Banner",
    @"
case ""Legion War Banner"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8587);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""darkwarlegion"", ""Manslayer Fiend"", ""ManSlayer Slain"", 5);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Legion Trophy",
    @"
case ""Legion Trophy"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8586);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""darkwarlegion"", ""Dreadfiend"", ""Nation's Dread"", 5, isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Soiled Fiend Crystal",
    @"
case ""Soiled Fiend Crystal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(8588);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""darkwarlegion"", ""Dirtlicker"", ""Dirtlicker Defeated"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Concentrated Mana",
    @"
case ""Concentrated Mana"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(6979);
                        Core.HuntMonster(""prison"", ""Piggy Drake"", ""Broken Piggy Bank"");
                        Core.EnsureComplete(6979);
                    }
                    Core.Join(""akiba"", ""r1"", ""Right"", false);
                    break;
    "
},
{
    "Green Scrap",
    @"
case ""Green Scrap"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(6978);
                        Core.HuntMonster(""newbie"", ""Slime"", ""Hidden Giftbox"", 5);
                        Core.HuntMonster(""noobshire"", ""Kittarian Mouse Eater"", ""Decorated Box"", 5);
                        Core.EnsureComplete(6978);
                    }
                    Core.Join(""akiba"", ""r1"", ""Right"", false);
                    break;
    "
},
{
    "Bido's Appreciation",
    @"
case ""Bido's Appreciation"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(6980);
                        Core.HuntMonster(""well"", ""Gell Oh No"", ""Piece of Gell Oh No Perfectly Slushied"");
                        Core.HuntMonster(""ashfallcamp"", ""Smoldur"", ""Smoldur's Shedded Scales"", 4);
                        Core.EnsureComplete(6980);
                    }
                    Core.Join(""akiba"", ""r1"", ""Right"", false);
                    break;
    "
},
{
    "Unknown Alloy",
    @"
case ""Unknown Alloy"":
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Adv.BuyItem(""alchemyacademy"", 2036, 62749, quant, 1, 8777, Log: false);
                        Adv.BuyItem(""alchemyacademy"", 2114, req.Name, quant, Log: false);
                    }
                    Core.Join(""akiba"", ""r1"", ""Right"", false);
                    break;
    "
},
{
    "Monster Trophy",
    @"
case ""Monster Trophy"":
                    Core.HuntMonster(""towerofdoom"", ""Dread Klunk"", req.Name, quant, false);
                    Core.Join(""akiba"", ""r1"", ""Right"", false);
                    break;
    "
},
{
    "Alteon the Imbalanced",
    @"
case ""Alteon the Imbalanced"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""brightfortress"", ""Imbalanced Alteon"", req.Name, quant, false);
                    Core.Join(""akiba"", ""r1"", ""Right"", false);
                    break;

    "
},
{
    "Wretched Blade of the Void",
    @"
case ""Wretched Blade of the Void"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Adv.BuyItem(""darkwarnation"", 2123, req.Name);
                        Bot.Wait.ForItemBuy();
                    }
                    break;
    "
},
{
    "Rune of Radiance",
    @"
case ""Rune of Radiance"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9170);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""fortressdelve"", ""Enlightened Shadow"", ""Shadowscythe Bone Shard"", 10, log: false);
                        Core.HuntMonster(""fortressdelve"", ""Delirious Elemental"", ""Elemental Residue"", 10, log: false);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""fortressdelve"", ""Astero"", ""Glass Wing"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "ProtoSoul Gem",
    @"
case ""ProtoSoul Gem"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""futurewar"", ""SF3017 Paragonator"", req.Name, quant, log: false);
                    break;
    "
},
{
    "Underworld Drachma",
    @"
case ""Underworld Drachma"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9620);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.KillMonster(""legionbarracks"", ""r4"", ""Left"", ""*"", ""Legion Cocytus Engraving"", 6, log: false);
                        Core.HuntMonster(""legionbarracks"", ""Overdriven paladin"", ""Paladin's Death Tag"", 6, log: false);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""legionbarracks"", ""Paladin Arondight"", ""Arondight's Starlight"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Grand Antaeus Spear",
    @"
case ""Grand Antaeus Spear"":
                    Core.Logger($""Item {req.Name} is not obtainable anymore."");
                    break;

    "
},
{
    "Shard of Armor",
    @"
case ""Shard of Armor"":
                    //3408 requires you to join the legion (1200acs) added a method for non-legions
                    if (Core.isCompletedBefore(793))
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.EquipClass(ClassType.Farm);
                        Core.RegisterQuests(3408);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.KillMonster(""underworld"", ""r8"", ""Left"", ""*"", ""Dread Head"", 20, log: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    else
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""undervoid"", ""Conquest"", req.Name, quant, false);
                    }
                    break;
    "
},
{
    "Helm Piece",
    @"
case ""Helm Piece"":
                    //3408 requires you to join the legion (1200acs) added a method for non-legions
                    if (Core.isCompletedBefore(793))
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.EquipClass(ClassType.Farm);
                        Core.RegisterQuests(3408);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.KillMonster(""underworld"", ""r8"", ""Left"", ""*"", ""Dread Head"", 20, log: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    else
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""undervoid"", ""Conquest"", req.Name, quant, false);
                    }
                    break;
    "
},
{
    "Leg Pieces",
    @"
case ""Leg Pieces"":
                    //3408 requires you to join the legion (1200acs) added a method for non-legions
                    if (Core.isCompletedBefore(793))
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.EquipClass(ClassType.Farm);
                        Core.RegisterQuests(3408);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.KillMonster(""underworld"", ""r8"", ""Left"", ""*"", ""Dread Head"", 20, log: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    else
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""undervoid"", ""Conquest"", req.Name, quant, false);
                    }
                    break;
    "
},
{
    "Arm Pieces",
    @"
case ""Arm Pieces"":
                    //3408 requires you to join the legion (1200acs) added a method for non-legions
                    if (Core.isCompletedBefore(793))
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.EquipClass(ClassType.Farm);
                        Core.RegisterQuests(3408);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        {
                            Core.KillMonster(""underworld"", ""r8"", ""Left"", ""*"", ""Dread Head"", 20, log: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    else
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""undervoid"", ""Conquest"", req.Name, quant, false);
                    }
                    break;
    "
},
{
    "Weapon Shard",
    @"
case ""Weapon Shard"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""undervoid"", ""Conquest"", req.Name, quant, false);
                    break;
    "
},
{
    "Cape Piece",
    @"
case ""Cape Piece"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""undervoid"", ""Conquest"", req.Name, quant, false);
                    break;
    "
},
{
    "Rand's Approval",
    @"
case ""Rand's Approval"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9129);
                    Core.HuntMonster(""seraph"", ""Seraphic Recruit"", req.Name, quant, false, false);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Electric Underworld Katana",
    @"
case ""Electric Underworld Katana"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""abysslair"", ""Abyssal Underbeast"", req.Name, quant, false);
                    break;

    "
},
{
    "Rune of Doom",
    @"
case ""Rune of Doom"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9144);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""siegefortress"", ""Shadow Traitor"", ""Traitorous Specimen"", 8, log: false);
                        Core.HuntMonster(""siegefortress"", ""Enslaved Elemental"", ""Elemental Rune"", 8, log: false);
                        Core.HuntMonster(""siegefortress"", ""Enslaved Astero"", ""Colossal Light Rune"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Abyssal Seer Hair",
    @"
case ""Abyssal Seer Hair"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""Siege Fortress"", ""Dage The Evil"", req.Name, isTemp: false, log: false);
                    break;

    "
},
{
    "Abyssal Frost Sedge Hat",
    @"
case ""Abyssal Frost Sedge Hat"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""Siege Fortress"", ""Dage The Evil"", req.Name, isTemp: false, log: false);
                    break;

    "
},
{
    "Abyssal Frost Samurai Spirit",
    @"
case ""Abyssal Frost Samurai Spirit"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""Siege Fortress"", ""Dage The Evil"", req.Name, isTemp: false, log: false);
                    break;

    "
},
{
    "Legion Combat Trophy",
    @"
case ""Legion Combat Trophy"":
                    Legion.DagePvP(quant, 0, 0);
                    break;
    "
},
{
    "1v1 Legion PvP Trophy",
    @"
case ""1v1 Legion PvP Trophy"":
                    Core.Logger(""Cannot Get Item, Requires Manual pvp."");
                    break;
    "
},
{
    "Doomed Extract",
    @"
case ""Doomed Extract"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9090);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""templedelve"", ""Delirious Elemental"", ""Elemental Study"", 6);
                        Core.HuntMonster(""templedelve"", ""Infested Nation"", ""Infestation Study"", 6);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""templedelve"", ""Doomed Fiend"", ""Fiend Worm"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Nation Ritualist",
    @"
case ""Nation Ritualist"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""templedelve"", ""Doomed Fiend"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Void Nation Ritualist",
    @"
case ""Void Nation Ritualist"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""templedelve"", ""Doomed Fiend"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Obsidian Rock",
    @"
case ""Obsidian Rock"":
                    Legion.ObsidianRock(quant);
                    break;
    "
},
{
    "Sworn Legion Sovereign",
    @"
case ""Sworn Legion Sovereign"":
                    Core.FarmingLogger(req.Name, quant);
                    AFGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Legion Sovereign",
    @"
case ""Legion Sovereign"":
                    Core.FarmingLogger(req.Name, quant);
                    AFGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Legion Sovereign Helm",
    @"
case ""Legion Sovereign Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    AFGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Legion Sovereign Horns",
    @"
case ""Legion Sovereign Horns"":
                    Core.FarmingLogger(req.Name, quant);
                    AFGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Legion Sovereign Locks",
    @"
case ""Legion Sovereign Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    AFGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Empowered Legion Sovereign Locks",
    @"
case ""Empowered Legion Sovereign Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    AFGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Legion Sovereign Banner",
    @"
case ""Legion Sovereign Banner"":
                    Core.FarmingLogger(req.Name, quant);
                    AFGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Legion Sovereign Cloak",
    @"
case ""Legion Sovereign Cloak"":
                    Core.FarmingLogger(req.Name, quant);
                    AFGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Legion Sovereign Crown",
    @"
case ""Legion Sovereign Crown"":
                    Core.FarmingLogger(req.Name, quant);
                    AFGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Evanescence",
    @"
case ""Evanescence"":
                    Core.FarmingLogger(req.Name, quant);
                    AFGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dual Evanescence",
    @"
case ""Dual Evanescence"":
                    Core.FarmingLogger(req.Name, quant);
                    AFGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Terror of Enyo",
    @"
case ""Terror of Enyo"":
                    Core.FarmingLogger(req.Name, quant);
                    AFGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Eclipse of Enyo",
    @"
case ""Eclipse of Enyo"":
                    Core.FarmingLogger(req.Name, quant);
                    AFGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Eclipses of Enyo",
    @"
case ""Eclipses of Enyo"":
                    Core.FarmingLogger(req.Name, quant);
                    AFGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Hand of the Legion Sovereign",
    @"
case ""Hand of the Legion Sovereign"":
                    Core.FarmingLogger(req.Name, quant);
                    AFGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Hands of the Legion Sovereign",
    @"
case ""Hands of the Legion Sovereign"":
                    Core.FarmingLogger(req.Name, quant);
                    AFGM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Yang's Favor",
    @"
case ""Yang's Favor"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9035);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""poisonforest"", ""Traitor Knight"", ""Traitor's Medal"", 15);
                        Core.HuntMonster(""poisonforest"", ""Xavier Lionfang"", ""Xavier's Medal"", 5);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Nation Defender Medal",
    @"
case ""Nation Defender Medal"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8578, 8579);
                    Core.KillMonster(""darkwarnation"", ""r2"", ""Left"", ""*"", req.Name, quant);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Nation Trophy",
    @"
case ""Nation Trophy"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8580);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""darkwarnation"", ""Legion DoomKnight"", ""Legion Doomed"", 5);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Nation War Banner",
    @"
case ""Nation War Banner"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8581);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""darkwarnation"", ""Legion Dread Knight"", ""Legion's Dread"", 5);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Spoils of War",
    @"
case ""Spoils of War"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(8582);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""darkwarnation"", ""War"", ""War Defeated"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Inscribed Skull",
    @"
case ""Inscribed Skull"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.GetMapItem(11839, map: ""deathpits"");
                    break;
    "
},
{
    "Cursed Pirate Note",
    @"
case ""Cursed Pirate Note"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""shipwreck"", ""Cursed Pirate"", req.Name, quant, req.Temp, false);
                    break;

    "
},
{
    "Volcanic Fragment",
    @"
case ""Volcanic Fragment"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.KillMonster(""lavarockbay"", ""r2"", ""Left"", ""*"", req.Name, quant, false, false);
                    break;

    "
},
{
    "Summer Sizzle Lotion",
    @"
case ""Summer Sizzle Lotion"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8794);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""summerbreak"", ""MMMirage"", ""Gum Ball"", 6);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Volleyball Captain",
    @"
case ""Volleyball Captain"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(8794);
                    Core.HuntMonster(""summerbreak"", ""MMMirage"", ""Gum Ball"", 6);
                    Core.EnsureCompleteChoose(8794, new[] { req.Name });
                    break;

    "
},
{
    "Volleyball Hero",
    @"
case ""Volleyball Hero"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(8794);
                    Core.HuntMonster(""summerbreak"", ""MMMirage"", ""Gum Ball"", 6);
                    Core.EnsureCompleteChoose(8794, new[] { req.Name });
                    break;

    "
},
{
    "Volleyball Hero's Hat + Glasses",
    @"
case ""Volleyball Hero's Hat + Glasses"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(8794);
                    Core.HuntMonster(""summerbreak"", ""MMMirage"", ""Gum Ball"", 6);
                    Core.EnsureCompleteChoose(8794, new[] { req.Name });
                    break;

    "
},
{
    "Volleyball Heroine's Hat + Glasses",
    @"
case ""Volleyball Heroine's Hat + Glasses"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(8794);
                    Core.HuntMonster(""summerbreak"", ""MMMirage"", ""Gum Ball"", 6);
                    Core.EnsureCompleteChoose(8794, new[] { req.Name });
                    break;

    "
},
{
    "Volleyball Team A Mascot",
    @"
case ""Volleyball Team A Mascot"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(8794);
                    Core.HuntMonster(""summerbreak"", ""MMMirage"", ""Gum Ball"", 6);
                    Core.EnsureCompleteChoose(8794, new[] { req.Name });
                    break;

    "
},
{
    "Volleyball Team B Mascot",
    @"
case ""Volleyball Team B Mascot"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(8794);
                    Core.HuntMonster(""summerbreak"", ""MMMirage"", ""Gum Ball"", 6);
                    Core.EnsureCompleteChoose(8794, new[] { req.Name });
                    break;

    "
},
{
    "Volleyball Team C Mascot",
    @"
case ""Volleyball Team C Mascot"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(8794);
                    Core.HuntMonster(""summerbreak"", ""MMMirage"", ""Gum Ball"", 6);
                    Core.EnsureCompleteChoose(8794, new[] { req.Name });
                    break;

    "
},
{
    "Volleyball Hero's Board Cape",
    @"
case ""Volleyball Hero's Board Cape"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(8794);
                    Core.HuntMonster(""summerbreak"", ""MMMirage"", ""Gum Ball"", 6);
                    Core.EnsureCompleteChoose(8794, new[] { req.Name });
                    break;

    "
},
{
    "Volleyball Hero's Rod",
    @"
case ""Volleyball Hero's Rod"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(8794);
                    Core.HuntMonster(""summerbreak"", ""MMMirage"", ""Gum Ball"", 6);
                    Core.EnsureCompleteChoose(8794, new[] { req.Name });
                    break;

    "
},
{
    "Volleyball Hero's Surfboard",
    @"
case ""Volleyball Hero's Surfboard"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(8794);
                    Core.HuntMonster(""summerbreak"", ""MMMirage"", ""Gum Ball"", 6);
                    Core.EnsureCompleteChoose(8794, new[] { req.Name });
                    break;

    "
},
{
    "Volleyball Hero's Foam Spear",
    @"
case ""Volleyball Hero's Foam Spear"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(8794);
                    Core.HuntMonster(""summerbreak"", ""MMMirage"", ""Gum Ball"", 6);
                    Core.EnsureCompleteChoose(8794, new[] { req.Name });
                    break;

    "
},
{
    "Volleyball Hero's Foam Gauntlets",
    @"
case ""Volleyball Hero's Foam Gauntlets"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(8794);
                    Core.HuntMonster(""summerbreak"", ""MMMirage"", ""Gum Ball"", 6);
                    Core.EnsureCompleteChoose(8794, new[] { req.Name });
                    break;

    "
},
{
    "Volleyball Hero's WaterGun",
    @"
case ""Volleyball Hero's WaterGun"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(8794);
                    Core.HuntMonster(""summerbreak"", ""MMMirage"", ""Gum Ball"", 6);
                    Core.EnsureCompleteChoose(8794, new[] { req.Name });
                    break;

    "
},
{
    "Volleyball Hero's WaterGuns",
    @"
case ""Volleyball Hero's WaterGuns"":
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(8794);
                    Core.HuntMonster(""summerbreak"", ""MMMirage"", ""Gum Ball"", 6);
                    Core.EnsureCompleteChoose(8794, new[] { req.Name });
                    break;

    "
},
{
    "Scrap of Cloth",
    @"
case ""Scrap of Cloth"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        foreach (int mon in new[] { 3, 7, 15, 10 })
                        {
                            Monster? M = Bot.Monsters.MapMonsters.FirstOrDefault(x => x != null && x.MapID == mon);
                            if (M == null)
                                continue;

                            if (Bot.Map.Name != ""tlapd"")
                                Core.Join(""tlapd"");
                            if (Bot.Player.Cell != M!.Cell)
                                Core.Jump(M.Cell);

                            if (M != null && M.HP >= 0)
                                Bot.Hunt.Monster(M.MapID);

                            if (Core.CheckInventory(req.Name, quant))
                                break;
                        }
                    }
                    break;
    "
},
{
    "Pirate Mage Token",
    @"
case ""Pirate Mage Token"":
                    BlazeBeard.TokenQuests();
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(Core.IsMember ? 4531 : 4530);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        if (Core.IsMember)
                        {
                            //Undead Pirate Hordes 4531 [Member]
                            Core.HuntMonster(""Blazebeard"", ""Pirate Crew"", ""Cursed Medallion"");
                            Bot.Wait.ForPickup(req.Name);
                        }
                        else
                        {
                            //Pirate Caster Hunting 4530
                            Core.HuntMonster(""ManaCannon"", ""Pirate Caster"", ""Pirate Caster Beaten"", 10);
                            Core.HuntMonster(""ManaCannon"", ""Pirate Caster"", ""Pirate Caster Research Clue "");
                            Bot.Wait.ForPickup(req.Name);
                        }
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Explorer Pistol",
    @"
case ""Explorer Pistol"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ManaCannon"", ""Blazebeard"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Blaze Gem",
    @"
case ""Blaze Gem"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""ManaCannon"", ""Blazebeard"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Pirate Class Token",
    @"
case ""Pirate Class Token"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    if (!Core.CheckInventory(""Classic Pirate""))
                    {
                        //Map Recovery 31
                        Core.AddDrop(""Classic Pirate"");
                        Core.EnsureAccept(31);
                        Core.HuntMonster(""Pirates"", ""Fishwing"", ""Map Fragment"", 5);
                        Core.EnsureComplete(31);
                        Adv.RankUpClass(""Classic Pirate"");
                    }
                    Core.RegisterQuests(4551);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        //New Pirate Class 4551
                        Core.HuntMonster(""Blazebeard"", ""Undead Pirate"", ""Rusty Nail"");
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Alpha Pirate Class Token",
    @"
case ""Alpha Pirate Class Token"":
                    if (!Core.CheckInventory(""Classic Alpha Pirate""))
                    {
                        Core.Logger($""\""{req.Name}\"" requires you to have \""Classic Alpha Pirate\"" which is rare. Skipping this item."");
                        return;
                    }
                    else
                    {
                        Core.FarmingLogger(req.Name, quant);
                        Core.EquipClass(ClassType.Farm);
                        Core.RegisterQuests(4552);
                        while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                            //New Alpha Pirate Class 4552
                            Core.HuntMonster(""Blazebeard"", ""Undead Pirate"", ""Rusty Nail"");
                        Core.CancelRegisteredQuests();
                    }
                    break;

    "
},
{
    "Deepest Desire",
    @"
case ""Deepest Desire"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8753);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""Tercessuinotlim"", ""Tainted Elemental"", ""Tainted Essence Collected"", 10);
                        Core.KillMonster(""tercessuinotlim"", ""m2"", ""Left"", ""*"", ""Makai Essence Collected"", 20);
                        Core.HuntMonster(""necrodungeon"", ""SlimeSkull"", ""Necropolis Soul Collected"", 15);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""necrodungeon"", ""5 Headed Dracolich"", ""Dracolich Soul Collected"", 15);
                        Core.HuntMonster(""necrodungeon"", ""Doom Overlord"", ""Doom Power Catalyst"", 2);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Hidden Hope",
    @"
case ""Hidden Hope"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(8751);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster($""battleunderb"", ""Enter"", ""Spawn"", ""*"", ""Bundle O' Bones"", 30);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonsterMapID($""Odokuro"", 1, ""Odokuro's Occipital"");
                        Core.HuntMonster($""bonecastle"", ""Vaden"", ""Vaden's Other Arm"");
                        Core.HuntMonster($""vordredboss"", ""Vordred"", ""Vordred's Skull(s)"", 3);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Simple Wish",
    @"
case ""Simple Wish"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8748);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        if (!Core.CheckInventory(""Twilly Twig""))
                        {
                            Core.AddDrop(""Twilly Twig"");
                            Core.EnsureAccept(11);
                            Core.HuntMonster(""farm"", ""Treeant"", ""Treeant Branch"");
                            Core.EnsureComplete(11);
                            Bot.Wait.ForPickup(""Twilly Twig"");
                        }
                        Core.HuntMonster(""brightoak"", ""Bright Treeant"", ""Brightest Branch"", 6);
                        Core.HuntMonster(""farm"", ""Treeant"", ""Treant Leaf"");
                        Core.HuntMonster(""guardiantree"", ""Blossoming Treeant"", ""Beautiful Blossom"", 6);
                        Core.HuntMonster(""NibbleOn"", ""Mean Old Treeant"", ""Bitter Bark"", 8);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Fallen Star Shard",
    @"
case ""Fallen Star Shard"":
                    Core.EquipClass(ClassType.Solo);
                    //Adv.BestGear(RacialGearBoost.Elemental);
                    Core.HuntMonster(""starfest"", ""Fallen Star"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Hashihime's Heart",
    @"
case ""Hashihime's Heart"":
                    Core.EquipClass(ClassType.Solo);
                    //Adv.BestGear(RacialGearBoost.Chaos);
                    Core.HuntMonster(""yokaistarriver"", ""Uji No Hashihime"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Nerites Scale",
    @"
case ""Nerites Scale"":
                    Core.Logger($""{req.Name} requires ultra boss, you need to farm it manually."", stopBot: true);
                    break;
    "
},
{
    "Glint Edge Cutlass",
    @"
case ""Glint Edge Cutlass"":
                    Core.Logger($""{req.Name} requires ultra boss, you need to farm it manually."", stopBot: true);
                    break;
    "
},
{
    "Pirate Remains",
    @"
case ""Pirate Remains"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9409);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""nerites"", ""Ghostly Eel"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Sea Salt",
    @"
case ""Sea Salt"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(8858);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""aluteanursery"", ""Last Alutian"", ""Angler Antena"");
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""aluteanursery"", ""Stagnant Water"", ""Stale Seawater"", 6);
                        Core.HuntMonster(""aluteanursery"", ""Bone Crustacean"", ""Pale Shell"", 6);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Naval Guard",
    @"
case ""Naval Guard"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(8858);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""aluteanursery"", ""Last Alutian"", ""Angler Antena"");
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""aluteanursery"", ""Stagnant Water"", ""Stale Seawater"", 6);
                        Core.HuntMonster(""aluteanursery"", ""Bone Crustacean"", ""Pale Shell"", 6);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Naval Guard's Tricorn + Hair",
    @"
case ""Naval Guard's Tricorn + Hair"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(8858);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""aluteanursery"", ""Last Alutian"", ""Angler Antena"");
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""aluteanursery"", ""Stagnant Water"", ""Stale Seawater"", 6);
                        Core.HuntMonster(""aluteanursery"", ""Bone Crustacean"", ""Pale Shell"", 6);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Naval Guard's Tricorn + Locks",
    @"
case ""Naval Guard's Tricorn + Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(8858);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""aluteanursery"", ""Last Alutian"", ""Angler Antena"");
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""aluteanursery"", ""Stagnant Water"", ""Stale Seawater"", 6);
                        Core.HuntMonster(""aluteanursery"", ""Bone Crustacean"", ""Pale Shell"", 6);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Naval Guard's Cutlass",
    @"
case ""Naval Guard's Cutlass"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(8858);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""aluteanursery"", ""Last Alutian"", ""Angler Antena"");
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""aluteanursery"", ""Stagnant Water"", ""Stale Seawater"", 6);
                        Core.HuntMonster(""aluteanursery"", ""Bone Crustacean"", ""Pale Shell"", 6);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Naval Guard's Cutlasses",
    @"
case ""Naval Guard's Cutlasses"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(8858);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""aluteanursery"", ""Last Alutian"", ""Angler Antena"");
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""aluteanursery"", ""Stagnant Water"", ""Stale Seawater"", 6);
                        Core.HuntMonster(""aluteanursery"", ""Bone Crustacean"", ""Pale Shell"", 6);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Naval Guard's Rapier",
    @"
case ""Naval Guard's Rapier"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(8858);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""aluteanursery"", ""Last Alutian"", ""Angler Antena"");
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""aluteanursery"", ""Stagnant Water"", ""Stale Seawater"", 6);
                        Core.HuntMonster(""aluteanursery"", ""Bone Crustacean"", ""Pale Shell"", 6);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Naval Guard's Rapiers",
    @"
case ""Naval Guard's Rapiers"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(8858);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""aluteanursery"", ""Last Alutian"", ""Angler Antena"");
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""aluteanursery"", ""Stagnant Water"", ""Stale Seawater"", 6);
                        Core.HuntMonster(""aluteanursery"", ""Bone Crustacean"", ""Pale Shell"", 6);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Naval Guard's ArmBlade",
    @"
case ""Naval Guard's ArmBlade"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(8858);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""aluteanursery"", ""Last Alutian"", ""Angler Antena"");
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""aluteanursery"", ""Stagnant Water"", ""Stale Seawater"", 6);
                        Core.HuntMonster(""aluteanursery"", ""Bone Crustacean"", ""Pale Shell"", 6);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "DeepSea Star Pirate",
    @"
case ""DeepSea Star Pirate"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""aluteanursery"", ""Last Alutian"", req.Name, isTemp: false);
                    break;

    "
},
{
    "DeepSea Star Pirate's Hair",
    @"
case ""DeepSea Star Pirate's Hair"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""aluteanursery"", ""Last Alutian"", req.Name, isTemp: false);
                    break;

    "
},
{
    "DeepSea Star Pirate's Locks",
    @"
case ""DeepSea Star Pirate's Locks"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""aluteanursery"", ""Last Alutian"", req.Name, isTemp: false);
                    break;

    "
},
{
    "DeepSea Star Pirate's Morph",
    @"
case ""DeepSea Star Pirate's Morph"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""aluteanursery"", ""Last Alutian"", req.Name, isTemp: false);
                    break;

    "
},
{
    "DeepSea Star Pirate's Morph + Locks",
    @"
case ""DeepSea Star Pirate's Morph + Locks"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""aluteanursery"", ""Last Alutian"", req.Name, isTemp: false);
                    break;

    "
},
{
    "DeepSea Star Pirate's Light Gun",
    @"
case ""DeepSea Star Pirate's Light Gun"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""aluteanursery"", ""Last Alutian"", req.Name, isTemp: false);
                    break;

    "
},
{
    "DeepSea Star Pirate's Light Guns",
    @"
case ""DeepSea Star Pirate's Light Guns"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""aluteanursery"", ""Last Alutian"", req.Name, isTemp: false);
                    break;

    "
},
{
    "DeepSea Smol Wave",
    @"
case ""DeepSea Smol Wave"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""aluteanursery"", ""Last Alutian"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Blood Isle Booty",
    @"
case ""Blood Isle Booty"":
                    Core.FarmingLogger(req.Name, quant);
                    // 9886 | Petty Proposition
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(9886);
                        Core.HuntMonster(""bloodisles"", UseableMonsters[6], ""Blood Captain Cap"");
                        Core.HuntMonster(""bloodisles"", UseableMonsters[7], ""Kurok's Moon Ring"");
                        Core.HuntMonster(""bloodisles"", UseableMonsters[8], ""Merpyre Scale"");
                        Core.EnsureComplete(9886);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Amira 2.0 Gear",
    @"
case ""Amira 2.0 Gear"":
                    if (!Core.IsMember)
                    {
                        Core.Logger(""Members only map"");
                        return;
                    }
                    Core.HuntMonster(""amira"", ""Amira 2.0"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Fancy Golden Scissors",
    @"
case ""Fancy Golden Scissors"":
                    Core.HuntMonster(""bloodisles"", UseableMonsters[8], req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Dual Fancy Golden Scissors",
    @"
case ""Dual Fancy Golden Scissors"":
                    Core.HuntMonster(""bloodisles"", UseableMonsters[8], req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Dragon King's Favor",
    @"
case ""Dragon King's Favor"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8288);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""dragoncapital"", ""Titan Leech"", ""Titan Leftovers Defeated"", 6);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Regal Pirate Fleet",
    @"
case ""Regal Pirate Fleet"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dragoncapital"", ""Leviathanius"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Regal Pirate Leggings",
    @"
case ""Regal Pirate Leggings"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dragoncapital"", ""Leviathanius"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Regal Pirate's Hat",
    @"
case ""Regal Pirate's Hat"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dragoncapital"", ""Leviathanius"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Regal Pirate's Accessories",
    @"
case ""Regal Pirate's Accessories"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dragoncapital"", ""Leviathanius"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Regal Pirate's Hat + Locks",
    @"
case ""Regal Pirate's Hat + Locks"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dragoncapital"", ""Leviathanius"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Regal Pirate's Accessories + Locks",
    @"
case ""Regal Pirate's Accessories + Locks"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dragoncapital"", ""Leviathanius"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Regal Pirate's Rapier",
    @"
case ""Regal Pirate's Rapier"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dragoncapital"", ""Leviathanius"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Regal Pirate's Accoutrements",
    @"
case ""Regal Pirate's Accoutrements"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dragoncapital"", ""Leviathanius"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Formal Pirate Fleet",
    @"
case ""Formal Pirate Fleet"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dragoncapital"", ""Leviathanius"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Formal Pirate Leggings",
    @"
case ""Formal Pirate Leggings"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dragoncapital"", ""Leviathanius"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Regal Pirate's Wheel",
    @"
case ""Regal Pirate's Wheel"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dragoncapital"", ""Empowered Scalebeard"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Regal Pirate's Cape + Wheel",
    @"
case ""Regal Pirate's Cape + Wheel"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dragoncapital"", ""Empowered Scalebeard"", req.Name, isTemp: false);
                    break;
    "
},
{
    "Shadow Extract",
    @"
case ""Shadow Extract"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9068);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""templesiege"", ""Doomed Oblivion"", ""Oblivion's Gem"", log: false);
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""templesiege"", ""Doomed Beast"", ""Dark Remnants"", 7, log: false);
                        Core.HuntMonster(""templesiege"", ""Overdriven Paladin"", ""Paladin Armament"", 7, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Warden of Light",
    @"
case ""Warden of Light"":
                    Core.FarmingLogger(req.Name, quant);
                    Farm.BludrutBrawlBoss(quant: 500);
                    Core.BuyItem(""battleon"", 222, req.Name);
                    break;
    "
},
{
    "Conqueror of Shadow",
    @"
case ""Conqueror of Shadow"":
                    Core.FarmingLogger(req.Name, quant);
                    Farm.BludrutBrawlBoss(quant: 350);
                    Core.BuyItem(""battleon"", 222, req.Name);
                    break;
    "
},
{
    "Crimson Plate of Nulgath",
    @"
case ""Crimson Plate of Nulgath"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(765);
                    Nation.FarmTotemofNulgath(3);
                    Core.HuntMonster(""underworld"", ""Skull Warrior"", ""Skull Warrior Rune"");
                    Core.EnsureComplete(765, 4695);
                    break;
    "
},
{
    "Behemoth Blade of Light",
    @"
case ""Behemoth Blade of Light"":
                    DB.BehemothBladeof(""Light"");
                    break;
    "
},
{
    "Behemoth Blade of Shadow",
    @"
case ""Behemoth Blade of Shadow"":
                    DB.BehemothBladeof(""Shadow"");
                    break;
    "
},
{
    "DragonFire of Nulgath",
    @"
case ""DragonFire of Nulgath"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(765);
                    Nation.FarmTotemofNulgath(3);
                    Core.HuntMonster(""underworld"", ""Skull Warrior"", ""Skull Warrior Rune"");
                    Core.EnsureComplete(765, 1316);
                    break;
    "
},
{
    "Light Warden Helm",
    @"
case ""Light Warden Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Farm.BludrutBrawlBoss(quant: 150);
                    Core.BuyItem(""battleon"", 222, req.Name);
                    break;
    "
},
{
    "Shadow Conqueror Helm",
    @"
case ""Shadow Conqueror Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Farm.BludrutBrawlBoss(quant: 100);
                    Core.BuyItem(""battleon"", 222, req.Name);
                    break;
    "
},
{
    "Crimson Face Plate of Nulgath",
    @"
case ""Crimson Face Plate of Nulgath"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.EnsureAccept(765);
                    Nation.FarmTotemofNulgath(3);
                    Core.HuntMonster(""underworld"", ""Skull Warrior"", ""Skull Warrior Rune"");
                    Core.EnsureComplete(765, 4961);
                    break;

    "
},
{
    "Gallaeon's Piece of Eight",
    @"
        case ""Gallaeon's Piece of Eight"":
            Core.FarmingLogger(req.Name, req.Quantity);
            Core.RegisterQuests(9355);
            Core.EquipClass(ClassType.Solo);
            Core.Join(""doompirate"", ""r5"", ""Left"");

            bool restartKills = false;

        RestartKills:
            while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, req.Quantity))
            {
                if (restartKills)
                {
                    restartKills = false;

                    Core.Logger(""Send player to house to reset map"");
                    Bot.Send.Packet($""%xt%zm%house%1%{Core.Username()}%"");
                    Bot.Wait.ForMapLoad(""house"");

                    Core.Logger(""Rejoin map to reset mobs"");
                    Core.Join(""doompirate"", ""r5"", ""Left"");
                    Bot.Wait.ForMapLoad(""doompirate"");
                }

                foreach (int mobId in new[] { 5, 4, 7, 6, 9, 8, 11, 10 })
                {
                    while (!Bot.ShouldExit)
                    {
                        if (!Bot.Player.Alive)
                        {
                            Core.Logger(""Death - Resetting"");
                            while (!Bot.ShouldExit && !Bot.Player.Alive) { Bot.Sleep(1000); }
                            restartKills = true;
                            goto RestartKills;
                        }

                        if (Bot.Player.Cell != ""r5"")
                        {
                            Core.Jump(""r5"", ""Left"");
                            Core.Sleep();
                        }

                        Monster? mon = Bot.Monsters.CurrentAvailableMonsters.FirstOrDefault(x => x != null && x.MapID == mobId);
                        if (mon == null)
                        {
                            Core.Logger($""Skipping mob {mobId}, not found."");
                            continue;
                        }

                        if (!Bot.Player.HasTarget)
                            Bot.Combat.Attack(mobId);

                        Bot.Sleep(1500);

                        if (Core.GetMonsterHP(mobId.ToString()) <= 0)
                        {
                            Bot.Combat.CancelTarget();
                            break;
                        }
                    }
                }

                Bot.Kill.Monster(12);
            }
            break;
    "
},
{
    "Doom Doubloon",
    @"
case ""Doom Doubloon"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(9354);
                    Core.HuntMonsterMapID(""doompirate"", 3, req.Name, quant, log: false);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Draconic Doubloon",
    @"
case ""Draconic Doubloon"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8276);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""dragonpirate"", ""Dragon Gunner"", ""Pirates Defeated"", 10);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Lightning Pirate's Machine Pistol",
    @"
case ""Lightning Pirate's Machine Pistol"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""dragonpirate"", ""Dragon Pirate"", req.Name);
                    break;
    "
},
{
    "Lightning Pirate's Tricorn",
    @"
case ""Lightning Pirate's Tricorn"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""dragonpirate"", ""Dragon Gunner"", req.Name);
                    break;
    "
},
{
    "Lightning Pirate's Tricorn + Locks",
    @"
case ""Lightning Pirate's Tricorn + Locks"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""dragonpirate"", ""Dragon Gunner"", req.Name);
                    break;
    "
},
{
    "Lightning Pirate's Tricorn + Eyepatch",
    @"
case ""Lightning Pirate's Tricorn + Eyepatch"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""dragonpirate"", ""Dragon Pirate"", req.Name);
                    break;
    "
},
{
    "Lightning Pirate's Tricorn Locks + Eyepatch",
    @"
case ""Lightning Pirate's Tricorn Locks + Eyepatch"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""dragonpirate"", ""Dragon Pirate"", req.Name);
                    break;
    "
},
{
    "Lightning Pirate",
    @"
case ""Lightning Pirate"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""dragonpirate"", ""Scalebeard"", req.Name);
                    break;
    "
},
{
    "Blood Testament Trophy",
    @"
case ""Blood Testament Trophy"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9868, 9869);
                    Core.KillMonster(""piratevampire"", ""r2"", ""Left"", ""*"", req.Name, quant, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Gilded Sheet Music",
    @"
case ""Gilded Sheet Music"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9868, 9869);
                    Core.KillMonster(""piratevampire"", ""r2"", ""Left"", ""*"", req.Name, quant, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Stardust",
    @"
case ""Stardust"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""pirates"", 724, req.Name, quant);
                    break;
    "
},
{
    "Facial Hair",
    @"
case ""Facial Hair"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""pirates"", 724, req.Name, quant);
                    break;
    "
},
{
    "Gears",
    @"
case ""Gears"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""pirates"", 724, req.Name, quant);
                    break;
    "
},
{
    "Breath of Flame",
    @"
case ""Breath of Flame"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""pirates"", 724, req.Name, quant);
                    break;
    "
},
{
    "Shard of Ice",
    @"
case ""Shard of Ice"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""pirates"", 724, req.Name, quant);
                    break;
    "
},
{
    "Nugget of Platinum",
    @"
case ""Nugget of Platinum"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""pirates"", 724, req.Name, quant);
                    break;
    "
},
{
    "Blue Skull",
    @"
case ""Blue Skull"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""pirates"", 724, req.Name, quant);
                    break;
    "
},
{
    "Red Cloth",
    @"
case ""Red Cloth"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""pirates"", 724, req.Name, quant);
                    break;
    "
},
{
    "Zombie Flesh",
    @"
case ""Zombie Flesh"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""pirates"", 724, req.Name, quant);
                    break;
    "
},
{
    "Pink Cloth",
    @"
case ""Pink Cloth"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""pirates"", 724, req.Name, quant);
                    break;
    "
},
{
    "Toxic Flame",
    @"
case ""Toxic Flame"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""pirates"", 724, req.Name, quant);
                    break;
    "
},
{
    "Toxic Gas Mask",
    @"
case ""Toxic Gas Mask"":
                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""pirates"", 724, req.Name, quant);
                    break;
    "
},
{
    "Legend Top Hat",
    @"
case ""Legend Top Hat"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""pirates"", ""Fishman Soldier"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Icy Naval Top Hat",
    @"
case ""Icy Naval Top Hat"":
                    NTHM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "ShadowChaos Mote",
    @"
case ""ShadowChaos Mote"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(7700);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""lagunabeach"", ""Flying Fisheye"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Evidence Tag",
    @"
case ""Evidence Tag"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(8846);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""lowtide"", ""Exiled General Miel"", ""Gem Encrusted Medal"", 3);
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""lowtide"", ""Spectral Jellyfish"", ""Spindley Tentacles"", 30);
                        Core.HuntMonster(""lowtide"", ""Ghostly Eel"", ""Eel Fangs"", 30);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Dark Sea Corsair",
    @"
case ""Dark Sea Corsair"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""lowtide"", ""Exiled General Miel"", req.Name, 1, false);
                    break;
    "
},
{
    "Dark Sea Corsair's Mask",
    @"
case ""Dark Sea Corsair's Mask"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""lowtide"", ""Exiled General Miel"", req.Name, 1, false);
                    break;
    "
},
{
    "Dark Sea Corsair's Mask + Locks",
    @"
case ""Dark Sea Corsair's Mask + Locks"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""lowtide"", ""Exiled General Miel"", req.Name, 1, false);
                    break;
    "
},
{
    "Dark Sea Corsair's Hat",
    @"
case ""Dark Sea Corsair's Hat"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""lowtide"", ""Exiled General Miel"", req.Name, 1, false);
                    break;
    "
},
{
    "Dark Sea Corsair's Hat + Locks",
    @"
case ""Dark Sea Corsair's Hat + Locks"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""lowtide"", ""Exiled General Miel"", req.Name, 1, false);
                    break;
    "
},
{
    "Dark Sea Corsair's Battle Mask",
    @"
case ""Dark Sea Corsair's Battle Mask"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""lowtide"", ""Exiled General Miel"", req.Name, 1, false);
                    break;
    "
},
{
    "Dark Sea Corsair's Battle Mask + Locks",
    @"
case ""Dark Sea Corsair's Battle Mask + Locks"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""lowtide"", ""Exiled General Miel"", req.Name, 1, false);
                    break;
    "
},
{
    "Enchanted Corsair's Rapier",
    @"
case ""Enchanted Corsair's Rapier"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""lowtide"", ""Spectral Jellyfish"", req.Name, 1, false);
                    break;

    "
},
{
    "Enchanted Corsair's Pistol",
    @"
case ""Enchanted Corsair's Pistol"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""lowtide"", ""Spectral Jellyfish"", req.Name, 1, false);
                    break;

    "
},
{
    "Pirate's Rag",
    @"
case ""Pirate's Rag"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9388);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""yokaipirate"", ""Lord Brentan"", ""Gold Leaf Brooch"");
                        Core.HuntMonster(""yokaipirate"", ""Neverglades  Knight"", ""Knight's Emblem"", 7);

                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""yokaipirate"", ""Disguised Pirate"", ""Yokai Pirate's Piece"", 7);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Yokai Gunpowder",
    @"
case ""Yokai Gunpowder"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""yokaipirate"", ""Serpent Warrior Monster"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Maurader's Mane",
    @"
case ""Maurader's Mane"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""yokaipirate"", ""Disguised Pirate"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Maurader's Mane + Beard",
    @"
case ""Maurader's Mane + Beard"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""yokaipirate"", ""Disguised Pirate"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Disguised Pirate's Hair",
    @"
case ""Disguised Pirate's Hair"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""yokaipirate"", ""Disguised Pirate"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Coastal Raider's Beard",
    @"
case ""Coastal Raider's Beard"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""yokaipirate"", ""Disguised Pirate"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Disguised Pirate's Tricorn",
    @"
case ""Disguised Pirate's Tricorn"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""yokaipirate"", ""Disguised Pirate"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Disguised Pirate's BattleGear",
    @"
case ""Disguised Pirate's BattleGear"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""yokaipirate"", ""Disguised Pirate"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Disguised Pirate's Cutlass",
    @"
case ""Disguised Pirate's Cutlass"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""yokaipirate"", ""Disguised Pirate"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Disguised Pirate's EyePatch",
    @"
case ""Disguised Pirate's EyePatch"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""yokaipirate"", ""Disguised Pirate"", req.Name, quant, req.Temp);
                    break;
    "
},
{
    "Swashbuckler's Rapier",
    @"
case ""Swashbuckler's Rapier"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""yokaipirate"", ""Neverglades  Knight"", req.Name, quant, req.Temp);
                    break;

    "
},
{
    "Top Hat",
    @"
case ""Top Hat"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""Pirates"", ""Fishman Soldier"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Unbound Thread",
    @"
case ""Unbound Thread"":
                    SOWM.UnboundThread(quant);
                    break;
    "
},
{
    "Pack Of Spices",
    @"
case ""Pack Of Spices"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.RegisterQuests(1550);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""pirates"", ""Capt. Beard"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Gold Ingot",
    @"
case ""Gold Ingot"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(1548);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""pirates"", ""Undead Pirate"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Bolt Of Silk",
    @"
case ""Bolt Of Silk"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(1549);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""pirates"", ""Undead Pirate"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "ShadowFire Trophy",
    @"
case ""ShadowFire Trophy"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(8192);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""fireinvasion"", ""Living Shadowflame"", ""ShadowFlame Tag"", 15, log: false);
                        Core.HuntMonster(""fireinvasion"", ""Shadefire Cavalry"", ""Corrupted Badge"", 3, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Wuji Steel",
    @"
case ""Wuji Steel"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9406);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""yokaitreasure"", ""Admiral Zheng"", ""Shapeshifting Pearl"", log: false);
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""yokaitreasure"", ""Needle Mouth"", ""Condemned Brand"", 4, log: false);
                        Core.HuntMonster(""yokaitreasure"", ""Imperial Warrior"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Mercury Phial",
    @"
case ""Mercury Phial"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""yokaitreasure"", ""Needle Mouth"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Tengu Typhoon Cutlass",
    @"
case ""Tengu Typhoon Cutlass"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""yokaitreasure"", ""Admiral Zheng"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Moonlit Steel Rapier",
    @"
case ""Moonlit Steel Rapier"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""yokaitreasure"", ""Admiral Zheng"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Stealthy Sea Hair",
    @"
case ""Stealthy Sea Hair"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""yokaitreasure"", ""Imperial Warrior"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Stealthy Sea Locks",
    @"
case ""Stealthy Sea Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""yokaitreasure"", ""Imperial Warrior"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Stealthy Sea Patch Hair",
    @"
case ""Stealthy Sea Patch Hair"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""yokaitreasure"", ""Imperial Warrior"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Stealthy Sea Patch Locks",
    @"
case ""Stealthy Sea Patch Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""yokaitreasure"", ""Imperial Warrior"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Iron Flight Cutlass",
    @"
case ""Iron Flight Cutlass"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""yokaitreasure"", ""Imperial Warrior"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Willpower",
    @"
case ""Willpower"":
                    SOWM.Willpower(quant);
                    break;
    "
},
{
    "ShadowFlame Healer",
    @"
case ""ShadowFlame Healer"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""ruinedcrown"", ""Mana-Burdened Mage"", req.Name, isTemp: false);
                    break;
    "
},
{
    "ShadowFlame Warrior",
    @"
case ""ShadowFlame Warrior"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""ruinedcrown"", ""Mana-Burdened Mage"", req.Name, isTemp: false);
                    break;
    "
},
{
    "ShadowFlame Mage",
    @"
case ""ShadowFlame Mage"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""ruinedcrown"", ""Mana-Burdened Mage"", req.Name, isTemp: false);
                    break;
    "
},
{
    "ShadowFlame Rogue",
    @"
case ""ShadowFlame Rogue"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""ruinedcrown"", ""Mana-Burdened Minion"", req.Name, isTemp: false);
                    break;

    "
},
{
    "ShadowFlame Rogue's Mask",
    @"
case ""ShadowFlame Rogue's Mask"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""ruinedcrown"", ""Mana-Burdened Minion"", req.Name, isTemp: false);
                    break;

    "
},
{
    "ShadowFlame Rogue's Locks",
    @"
case ""ShadowFlame Rogue's Locks"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""ruinedcrown"", ""Mana-Burdened Minion"", req.Name, isTemp: false);
                    break;

    "
},
{
    "ShadowFlame Rogue's Mortal Locks",
    @"
case ""ShadowFlame Rogue's Mortal Locks"":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster(""ruinedcrown"", ""Mana-Burdened Minion"", req.Name, isTemp: false);
                    break;

    "
},
{
    "Garish Remnant",
    @"
case ""Garish Remnant"":
                    SOWM.GarishRemnant(quant);
                    break;

    "
},
{
    "Avatar's Flame Bow",
    @"
case ""Avatar's Flame Bow"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""Streamwar"", ""Second Speaker"", req.Name, isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Avatar's Flame Spikes",
    @"
case ""Avatar's Flame Spikes"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""Streamwar"", ""Second Speaker"", req.Name, isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Avatar's Flame Banners",
    @"
case ""Avatar's Flame Banners"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""Streamwar"", ""Second Speaker"", req.Name, isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Avatar's Flame Sabre",
    @"
case ""Avatar's Flame Sabre"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""Streamwar"", ""Second Speaker"", req.Name, isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Avatar's Flame",
    @"
case ""Avatar's Flame"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""Streamwar"", ""Second Speaker"", req.Name, isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Avatar's Flame Guard",
    @"
case ""Avatar's Flame Guard"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""Streamwar"", ""Second Speaker"", req.Name, isTemp: false, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Prismatic Seams",
    @"
case ""Prismatic Seams"":
                    SOWM.PrismaticSeams(quant);
                    break;
    "
},
{
    "YourItemHere1",
    @"
case ""YourItemHere1"":
                    Core.RegisterQuests(0000);
                    Core.Logger($""Farming {req.Name} ({currentQuant}/{quant})"");
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""map"", ""monster"", ""item"", 99999999);
                        Core.HuntMonster(""map"", ""monster"", ""item"", 99999999);
                        Core.HuntMonster(""map"", ""monster"", ""item"", 99999999);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "YourItemHere2",
    @"
case ""YourItemHere2"":
                    Core.HuntMonster(""map"", ""monster"", req.Name, isTemp: false);
                    break;

                    // Add more cases here if needed
    "
},
{
    "Acquiescence",
    @"
case ""Acquiescence"":
                    SOWM.Acquiescence(quant);
                    break;
    "
},
{
    "Elemental Core",
    @"
case ""Elemental Core"":
                    SOWM.ElementalCore(quant);
                    break;
    "
},
{
    "Mainyu Rune",
    @"
case ""Mainyu Rune"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(SoW.MalgorDrops.Concat(SoW.MainyuDrops).ToArray());
                    Adv.GearStore();
                    Core.BossClass();
                    Core.HuntMonster(""manacradle"", ""The Mainyu"", req.Name, isTemp: false);
                    Adv.GearStore(true);
                    break;

    "
},
{
    "Mainyu Wings",
    @"
case ""Mainyu Wings"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(SoW.MalgorDrops.Concat(SoW.MainyuDrops).ToArray());
                    Adv.GearStore();
                    Core.BossClass();
                    Core.HuntMonster(""manacradle"", ""The Mainyu"", req.Name, isTemp: false);
                    Adv.GearStore(true);
                    break;

    "
},
{
    "Mainyu Tail",
    @"
case ""Mainyu Tail"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(SoW.MalgorDrops.Concat(SoW.MainyuDrops).ToArray());
                    Adv.GearStore();
                    Core.BossClass();
                    Core.HuntMonster(""manacradle"", ""The Mainyu"", req.Name, isTemp: false);
                    Adv.GearStore(true);
                    break;

    "
},
{
    "War Blade of Courage",
    @"
case ""War Blade of Courage"":
                            BLOD.BrilliantAura(50);
                            BLOD.BlindingAura(1);
                            Core.Logger(""Adding 7 to the Insignias Count"");
                            InsigniasCount += 7;
                            Core.Logger(""Adding 10 to the Acquiescence Count"");
                            AcquiescenceCount += 10;
                            break;
    "
},
{
    "War Blade of Power",
    @"
case ""War Blade of Power"":
                            //Dragon Scale(1)
                            Core.AddDrop(11475);
                            while (!Core.CheckInventory(11475, 30))
                                Core.KillMonster(""lair"", ""Hole"", ""Center"", ""*"", isTemp: false, log: false);
                            DSG.EnchantedScaleandClaw(250, 0);

                            Core.Logger(""Adding 7 to the Insignias Count"");
                            InsigniasCount += 7;
                            Core.Logger(""Adding 10 to the Acquiescence Count"");
                            AcquiescenceCount += 10;
                            break;
    "
},
{
    "War Blade of Speed",
    @"
case ""War Blade of Speed"":
                            Core.EquipClass(ClassType.Farm);
                            Core.HuntMonster(""shadowfallwar"", ""Skeletal Fire Mage"", ""Ultimate Darkness Gem"", 75, isTemp: false);
                            Core.EquipClass(ClassType.Solo);
                            Core.KillMonster(""shadowattack"", ""Boss"", ""Left"", ""Death"", ""Death's Oversight"", 5, false);

                            Core.Logger(""Adding 7 to the Insignias Count"");
                            InsigniasCount += 7;
                            Core.Logger(""Adding 10 to the Acquiescence Count"");
                            AcquiescenceCount += 10;
                            break;
    "
},
{
    "War Blade of Strength",
    @"
case ""War Blade of Strength"":
                            SoW.Tyndarius();

                            Core.AddDrop(""Fire Avatar's Favor"");
                            Core.EquipClass(ClassType.Farm);

                            Core.RegisterQuests(8244);
                            while (!Bot.ShouldExit && !Core.CheckInventory(""Fire Avatar's Favor"", 25))
                            {
                                Core.KillMonster(""fireavatar"", ""r4"", ""Right"", ""*"", ""Onslaught Defeated"", 6);
                                Core.KillMonster(""fireavatar"", ""r6"", ""Left"", ""*"", ""Elemental Defeated"", 6);

                                Bot.Wait.ForPickup(""Fire Avatar's Favor"");
                            }
                            Core.CancelRegisteredQuests();

                            Core.Logger(""Adding 7 to the Insignias Count"");
                            InsigniasCount += 7;
                            Core.Logger(""Adding 10 to the Acquiescence Count"");
                            AcquiescenceCount += 10;
                            Core.Logger(""Adding 25 to the ElementalCore Count"");
                            ElementalCoreCount += 25;
                            break;
    "
},
{
    "War Blade of Wisdom",
    @"
case ""War Blade of Wisdom"":
                            Core.AddDrop(""Fragment of the Queen"", ""ShadowChaos Mote"");
                            Core.EquipClass(ClassType.Solo);
                            Bot.Quests.UpdateQuest(8094);
                            Core.HuntMonster(""transformation"", ""Queen of Monsters"", ""Fragment of the Queen"", 13, false);

                            Core.EquipClass(ClassType.Farm);
                            Core.RegisterQuests(7700);
                            Core.HuntMonster(""lagunabeach"", ""Flying Fisheye"", ""ShadowChaos Mote"", 250, false);
                            Bot.Wait.ForPickup(""ShadowChaos Mote"");
                            Core.CancelRegisteredQuests();

                            Core.Logger(""Adding 7 to the Insignias Count"");
                            InsigniasCount += 7;
                            Core.Logger(""Adding 10 to the Acquiescence Count"");
                            AcquiescenceCount += 10;
                            break;
    "
},
{
    "Example Item",
    @"
case ""Example Item"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(5825);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""charredpath"", ""Infected Hare"", ""Invader Slain"", 10);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Golden Shadow Breaker",
    @"
case ""Golden Shadow Breaker"":
                    Core.FarmingLogger(""Golden Shadow Breaker"", quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(857);
                    Core.HuntMonster(""citadel"", ""Grand Inquisitor"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Banana",
    @"
case ""Banana"":
                    Core.FarmingLogger(""Banana"", quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(52924);
                    Core.HuntMonster(""arcangrove"", ""Gorillaphant"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Ingredients?",
    @"
case ""Ingredients?"":
                    Core.FarmingLogger(""Ingredients?"", quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(52925);
                    Core.HuntMonster(""doomvault"", ""Binky"", req.Name, quant, req.Temp, false, true);
                    break;
    "
},
{
    "Binky Companion",
    @"
case ""Binky Companion"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(52925);
                    Core.HuntMonster(""doomvault"", ""Binky"", req.Name, quant, req.Temp, false, true);
                    break;
    "
},
{
    "Iron Draconian Sword",
    @"
case ""Iron Draconian Sword"":
                    Core.FarmingLogger(""Iron Draconian Sword"", quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(571);
                    Core.HuntMonster(""lair"", ""Purple Draconian"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Water Draconian Sword",
    @"
case ""Water Draconian Sword"":
                    Core.FarmingLogger(""Water Draconian Sword"", quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(568);
                    Core.HuntMonster(""lair"", ""Water Draconian"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Venom Draconian Sword",
    @"
case ""Venom Draconian Sword"":
                    Core.FarmingLogger(""Venom Draconian Sword"", quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(570);
                    Core.HuntMonster(""lair"", ""Venom Draconian"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Spear of the Deep One",
    @"
case ""Spear of the Deep One"":
                    Core.FarmingLogger(""Spear of the Deep One"", quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(79498);
                    Core.HuntMonster(""deepchaos"", ""Kathool"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "1st Lord Of Chaos Helm",
    @"
case ""1st Lord Of Chaos Helm"":
                    Core.FarmingLogger(""1st Lord Of Chaos Helm"", quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(1173);
                    Core.KillEscherion(req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Chaos King Crown",
    @"
case ""Chaos King Crown"":
                    Core.FarmingLogger(""Chaos King Crown"", quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(20671);
                    Core.HuntMonster(""swordhavenfalls"", ""Chaos Lord Alteon"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Chaos Lord Alteon",
    @"
case ""Chaos Lord Alteon"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(20671);
                    Core.HuntMonster(""swordhavenfalls"", ""Chaos Lord Alteon"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Tainted Soul",
    @"
case ""Tainted Soul"":
                    Core.FarmingLogger(""Tainted Soul"", quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(4960);
                    Core.HuntMonster(""evilmarsh"", ""Tainted Soul"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Sea Creature Membrane",
    @"
case ""Sea Creature Membrane"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(93822);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, req.Quantity))
                    {
                        Core.HuntMonsterQuest(10275, ""sunkencity"", ""Merdrathoolian"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Oxidized Steel",
    @"
case ""Oxidized Steel"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(93823);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, req.Quantity))
                    {
                        Core.HuntMonsterQuest(10276, ""sunkencity"", ""Nereid Princess"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Riptide Helicoprion",
    @"
case ""Riptide Helicoprion"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(94014);
                    Core.HuntMonster(""sunkencity"", ""Nereid Princess"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Riptide Helicoprion Helm",
    @"
case ""Riptide Helicoprion Helm"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(94014);
                    Core.HuntMonster(""sunkencity"", ""Nereid Princess"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Grimskull's Favor",
    @"
case ""Grimskull's Favor"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger(""Grimskull's Favor requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10282, 10283);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(""lichwar"", ""Noxus Warrior"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Noxus' Favor",
    @"
case ""Noxus' Favor"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger(""Noxus' Favor requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10278, 10279);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(""lichwar"", ""Grim Soldier"", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Platinum Album Shard",
    @"
case ""Platinum Album Shard"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(4199,
                        (""boxes"", ""Sneevil Boxer"", ClassType.Farm),
                        (""greenguardwest"", ""Slime"", ClassType.Farm));
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Pristine Skull",
    @"
case ""Pristine Skull"":
                    MSWB.Setup(quant);
                    break;
    "
},
{
    "Vordred's Armor",
    @"
case ""Vordred's Armor"":
                    VA.GetVordredsArmor(true);
                    Adv.BuyItem(""stonewood"", 2063, req.Name);
                    break;
    "
},
{
    "Vordred's Helm",
    @"
case ""Vordred's Helm"":
                    VA.GetVordredsArmor(true);
                    Adv.BuyItem(""stonewood"", 2063, req.Name);
                    break;
    "
},
{
    "Vordred's Chestpiece",
    @"
case ""Vordred's Chestpiece"":
                    VA.GetVordredsArmor(true);
                    Adv.BuyItem(""stonewood"", 2063, req.Name);
                    break;
    "
},
{
    "Vordred's Cape",
    @"
case ""Vordred's Cape"":
                    VA.GetVordredsArmor(true);
                    Adv.BuyItem(""stonewood"", 2063, req.Name);
                    break;
    "
},
{
    "Jade Silk",
    @"
case ""Jade Silk"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(10292,
                (""victormatsuri"", ""Narcis Arrhythmia"", ClassType.Solo));
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Crimson Silk",
    @"
case ""Crimson Silk"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(10290,
                (""victormatsuri"", ""Kitsune Himawari"", ClassType.Solo));
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Midnight Silk",
    @"
case ""Midnight Silk"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(10290,
                (""victormatsuri"", ""Kitsune Himawari"", ClassType.Solo));
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Silver Victory Laurel",
    @"
case ""Silver Victory Laurel"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    List<int> svlQuests = new();
                    if (Core.IsMember && Bot.Quests.IsAvailable(10304))
                        svlQuests.Add(10304);
                    if (Bot.Quests.IsAvailable(10303))
                        svlQuests.Add(10303);
                    if (svlQuests.Count > 0)
                    {
                        Core.RegisterQuests(svlQuests.ToArray());
                        for (int i = 0; i < svlQuests.Count; i++)
                        {
                            Core.HuntMonster(""coliseum"", ""Nethersea Shark"", ""Level 25 Boss Defeated"", log: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    Core.HuntMonster(""coliseum"", ""Nethersea Shark"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Gold Victory Laurel",
    @"
case ""Gold Victory Laurel"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    List<int> gvlQuests = new();
                    if (Core.IsMember && Bot.Quests.IsAvailable(10306))
                        gvlQuests.Add(10306);
                    if (Bot.Quests.IsAvailable(10305))
                        gvlQuests.Add(10305);
                    if (gvlQuests.Count > 0)
                    {
                        Core.RegisterQuests(gvlQuests.ToArray());
                        for (int i = 0; i < gvlQuests.Count; i++)
                        {
                            Core.HuntMonster(""coliseum"", ""Void Dragon"", ""Level 50 Boss Defeated"", log: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    Core.HuntMonster(""coliseum"", ""Void Dragon"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Platinum Victory Laurel",
    @"
case ""Platinum Victory Laurel"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    List<int> pvlQuests = new();
                    if (Core.IsMember && Bot.Quests.IsAvailable(10308))
                        pvlQuests.Add(10308);
                    if (Bot.Quests.IsAvailable(10307))
                        pvlQuests.Add(10307);
                    if (pvlQuests.Count > 0)
                    {
                        Core.RegisterQuests(pvlQuests.ToArray());
                        for (int i = 0; i < pvlQuests.Count; i++)
                        {
                            Core.HuntMonster(""coliseum"", ""Chimera"", ""Level 75 Boss Defeated"", log: false);
                            Bot.Wait.ForPickup(req.Name);
                        }
                        Core.CancelRegisteredQuests();
                    }
                    Core.HuntMonster(""coliseum"", ""Chimera"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Trygve's Testament",
    @"
case ""Trygve's Testament"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""trygve"", ""Gramiel"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Fortress' Faith",
    @"
case ""Fortress' Faith"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""neofortress"", ""Vindicator General"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Tower's Trisagion",
    @"
case ""Tower's Trisagion"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""neotower"", ""Vindicator Assassin"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Sanctum's Salvation",
    @"
case ""Sanctum's Salvation"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""dawnsanctum"", ""Grandmaster Gramiel"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Grandmaster Gramiel",
    @"
case ""Grandmaster Gramiel"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""dawnsanctum"", ""Grandmaster Gramiel"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Grandmaster Gramiel Hair",
    @"
case ""Grandmaster Gramiel Hair"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""dawnsanctum"", ""Grandmaster Gramiel"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Celestial Gramiel Wings",
    @"
case ""Celestial Gramiel Wings"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""dawnsanctum"", ""Grandmaster Gramiel"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Gramiel the Graceful's Insignia",
    @"
case ""Gramiel the Graceful's Insignia"":
                    Core.Logger($""{req.Name} requires army, skipping..."");
                    break;
    "
},
{
    "Gramiel's Glorified Enoch",
    @"
case ""Gramiel's Glorified Enoch"":
                    DM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Dawn Vindicator Castle",
    @"
case ""Dawn Vindicator Castle"":
                    DM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Condensed Grace",
    @"
case ""Condensed Grace"":
                    if (Bot.Quests.IsAvailable(10299))
                        HBV.GetClass(false, true, quant);
                    break;
    "
},
{
    "Delta Fragment",
    @"
case ""Delta Fragment"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(10318,
                                        (""yokaiportal"", ""Kitsune Spirits"", ClassType.Farm));
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Byakko's Stardust",
    @"
case ""Byakko's Stardust"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(""whitetigerpoint"", ""Byakko"", req.Name, quant, req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "White Tiger Hair",
    @"
case ""White Tiger Hair"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(""whitetigerpoint"", ""Byakko"", req.Name, quant, req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "White Tiger Locks",
    @"
case ""White Tiger Locks"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(""whitetigerpoint"", ""Byakko"", req.Name, quant, req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "White Tiger Cape",
    @"
case ""White Tiger Cape"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(""whitetigerpoint"", ""Byakko"", req.Name, quant, req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Byakko's Aura",
    @"
case ""Byakko's Aura"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(""whitetigerpoint"", ""Byakko"", req.Name, quant, req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Komokuten's Star Wand",
    @"
case ""Komokuten's Star Wand"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(""whitetigerpoint"", ""Byakko"", req.Name, quant, req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Refined Orpheum",
    @"
case ""Refined Orpheum"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10334);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.KillMonster(""somnia"", ""r9"", ""Left"", ""*"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Somnic Extract",
    @"
case ""Somnic Extract"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10335);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(""somnia"", ""Nightwyrm"", ""Nightwyrm Head"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Dwarven Ether",
    @"
case ""Dwarven Ether"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10332);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(""dwarfhold"", ""Chaotic Draconian"", ""Draconian Chaos Ether"", 8);
                        Core.HuntMonster(""dwarfhold"", ""Chaos Drow"", ""Drow Chaos Ether"", 8);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Theliman Ore",
    @"
case ""Theliman Ore"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10333);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.KillMonster(""mountainpath"", ""Enter"", ""Spawn"", ""*"", ""Coarse Ore"", 21);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Pigment Powder",
    @"
        case ""Pigment Powder"":
            if (req.Upgrade && !Core.IsMember)
            {
                Core.Logger($""{req.Name} requires membership to farm, skipping."");
                return;
            }

            // Unlock farming quest
            if (!Core.isCompletedBefore(10337))
            {
                Core.EnsureAccept(10336);
                Core.HuntMonster(""castlewhistler"", ""King of the Dark"", ""King's Pigment"");
                Core.EnsureComplete(10336);
            }

            Core.FarmingLogger(req.Name, quant);
            Core.EquipClass(ClassType.Solo);
            Core.AddDrop(""Pigment Powder"");
            Core.RegisterQuests(10337);
            while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
            {
                Core.HuntMonster(""castlewhistler"", ""King of the Dark"", ""King's Varnish"");
                Bot.Wait.ForPickup(req.Name);
            }
            Core.CancelRegisteredQuests();
            break;
    "
},

{
    "Winter's Dirge",
    @"
case ""Winter's Dirge"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(""Armorial Crown"", ""Winter's Dirge"", ""Midsummer Rhapsody"");

                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(""castlewhistler"", ""King of the Dark"", req.Name, quant, req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }

                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Midsummer Rhapsody",
    @"
case ""Midsummer Rhapsody"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(""Armorial Crown"", ""Winter's Dirge"", ""Midsummer Rhapsody"");

                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(""castlewhistler"", ""King of the Dark"", req.Name, quant, req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }

                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Armorial Crown",
    @"
case ""Armorial Crown"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(""Armorial Crown"", ""Winter's Dirge"", ""Midsummer Rhapsody"");

                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(""castlewhistler"", ""King of the Dark"", req.Name, quant, req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }

                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "Yew Ember",
    @"
case ""Yew Ember"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""thelimacity"", ""Maleno Match"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Steel Ingot",
    @"
case ""Steel Ingot"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(Core.IsMember ? 10358 : 10357,
                        (""thelimacity"", ""Maleno Elemental"", ClassType.Solo),
                        (""thelimacity"", ""Maleno Match"", ClassType.Farm),
                        (""thelimacity"", ""Flame of Maleno"", ClassType.Solo)
                        );
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Mystic Topaz",
    @"
case ""Mystic Topaz"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""thelimacity"", ""Noelle Knight"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Dwarven Gold",
    @"
case ""Dwarven Gold"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""thelimacity"", ""Dwarven Aegis"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Dwarven Emerald",
    @"
case ""Dwarven Emerald"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""thelimacity"", ""Dwarven Aegis"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Maleno Obsidian",
    @"
case ""Maleno Obsidian"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""thelimacity"", ""Maleno Elemental"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Drow Silver",
    @"
case ""Drow Silver"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""thelimacity"", ""Drow Soldier"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Drow Amethyst",
    @"
case ""Drow Amethyst"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""thelimacity"", ""Drow Soldier"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Necrotic Darkness Gem",
    @"
case ""Necrotic Darkness Gem"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.KillMonster(""shadowfallwar"", ""Garden2"", ""Left"", ""*"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Tortured Darkness Gem",
    @"
case ""Tortured Darkness Gem"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.KillMonster(""shadowfallwar"", ""Garden2"", ""Left"", ""*"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Malignant Darkness Gem",
    @"
case ""Malignant Darkness Gem"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.KillMonster(""shadowfallwar"", ""Garden2"", ""Left"", ""*"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Ultimate Darkness Gem",
    @"
case ""Ultimate Darkness Gem"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.KillMonster(""shadowfallwar"", ""Garden2"", ""Left"", ""*"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Dawn Vindicator Swords",
    @"
case ""Dawn Vindicator Swords"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    switch (req.Name)
                    {
                        case ""Dawn Vindicator Helm"":
                        case ""Dawn Vindicator Soldier"":
                            Core.HuntMonster(""trygve"", ""Vindicator Recruit"", req.Name, isTemp: false);
                            break;

                        case ""Dawn Vindicator Sword"":
                            Core.HuntMonster(""trygve"", ""Vindicator Soldier"", req.Name, isTemp: false);
                            break;

                        case ""Dawn Vindicator Swords"":
                            Core.HuntMonster(""trygve"", ""Vindicator Recruit"", req.Name, isTemp: false);
                            break;
                    }
                    break;
    "
},
{
    "Black Flame of Maleno",
    @"
case ""Black Flame of Maleno"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(Core.IsMember ? 10370 : 10369,
                        (""mountmaleno"", ""Draconian Bandit"", ClassType.Farm),
                        (""mountmaleno"", ""Maleno Elemental"", ClassType.Solo),
                        (""mountmaleno"", ""Idalion"", ClassType.Solo));
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Shadowbrand's Edge",
    @"
case ""Shadowbrand's Edge"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""mountmaleno"", ""Idalion"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Shadowbrand's Edges",
    @"
case ""Shadowbrand's Edges"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""mountmaleno"", ""Idalion"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Blackfire Halberd",
    @"
case ""Blackfire Halberd"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""mountmaleno"", ""Idalion"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Nightcleaver",
    @"
case ""Nightcleaver"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""mountmaleno"", ""Idalion"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Nightcleavers",
    @"
case ""Nightcleavers"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""mountmaleno"", ""Idalion"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Maleno's Fang",
    @"
case ""Maleno's Fang"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""mountmaleno"", ""Idalion"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Maleno's Fangs",
    @"
case ""Maleno's Fangs"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""mountmaleno"", ""Idalion"", req.Name, quant, false, false);
                    break;
    "
},
{
    "Silver Claymore",
    @"
case ""Silver Claymore"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EnsureAccept(739);
                        while (!Bot.ShouldExit && !Core.CheckInventory(""Racing Trophy"", 20))
                            Core.ChainComplete(746);
                        Core.HuntMonster(""table"", ""Roach"", ""Gold Roach Antenna"", 10);
                        Core.EnsureCompleteChoose(739, new[] { req.Name });
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Necrotized Claymore",
    @"
case ""Necrotized Claymore"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.Logger(""Cannot Get Item, requires manual pvp."");
                    break;
    "
},
{
    "Stark's Ice",
    @"
case ""Stark's Ice"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""blindingsnow"", 236, req.Name, quant);
                    break;
    "
},
{
    "Grave Terror",
    @"
case ""Grave Terror"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    SFM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "The Unholy",
    @"
case ""The Unholy"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    SFM.BuyAllMerge(req.Name);
                    break;
    "
},
{
    "Heavy Holy Blade",
    @"
case ""Heavy Holy Blade"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""necropolis"", 408, req.Name, quant);
                    break;
    "
},
{
    "Blade of the Desert Dunes",
    @"
case ""Blade of the Desert Dunes"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Adv.BuyItem(""sandsea"", 242, req.Name, quant);
                    break;
    "
},
{
    "Silver Vindicator Swords",
    @"
case ""Silver Vindicator Swords"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""trygve"", ""Vindicator Recruit"", req.Name, quant, isTemp: req.Temp);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Gold Voucher 100K",
    @"
case ""Gold Voucher 100K"":
                    Farm.Voucher(req.Name, req.Quantity);
                    break;
    "
},
{
    "Aiwass Diamond",
    @"
case ""Aiwass Diamond"":
                    int questID = Core.IsMember ? 10387 : 10385;
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(questID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""sanctuaryaiwass"", ""r9"", ""Top"", ""*"", ""Sal Alembroth"", 1, false);
                        Core.KillMonster(""sanctuaryaiwass"", ""r9"", ""Top"", ""*"", ""Milk of Sulfur"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Aeon Dream"", 1, false);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Adamas Tenebris Drow",
    @"
case ""Adamas Tenebris Drow"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Drow"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Hair"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Locks"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Drow"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Hair"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Locks"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Farblight Fur"", 1, false);
                    }
                    break;
    "
},
{
    "Adamas Tenebris Hair",
    @"
case ""Adamas Tenebris Hair"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Drow"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Hair"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Locks"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Drow"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Hair"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Locks"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Farblight Fur"", 1, false);
                    }
                    break;
    "
},
{
    "Adamas Tenebris Locks",
    @"
case ""Adamas Tenebris Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Drow"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Hair"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Locks"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Drow"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Hair"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Locks"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Farblight Fur"", 1, false);
                    }
                    break;
    "
},
{
    "Enchanted Tenebris Drow",
    @"
case ""Enchanted Tenebris Drow"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Drow"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Hair"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Locks"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Drow"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Hair"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Locks"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Farblight Fur"", 1, false);
                    }
                    break;
    "
},
{
    "Enchanted Tenebris Hair",
    @"
case ""Enchanted Tenebris Hair"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Drow"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Hair"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Locks"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Drow"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Hair"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Locks"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Farblight Fur"", 1, false);
                    }
                    break;
    "
},
{
    "Enchanted Tenebris Locks",
    @"
case ""Enchanted Tenebris Locks"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Drow"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Hair"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Locks"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Drow"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Hair"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Locks"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Farblight Fur"", 1, false);
                    }
                    break;
    "
},
{
    "Farblight Fur",
    @"
case ""Farblight Fur"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Drow"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Hair"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Adamas Tenebris Locks"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Drow"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Hair"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Enchanted Tenebris Locks"", 1, false);
                        Core.HuntMonster(""sanctuaryaiwass"", ""Anima Animus Aiwass"", ""Farblight Fur"", 1, false);
                    }
                    break;
    "
},
{
    "Moeda Real",
    @"
case ""Moeda Real"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);

                    Core.KillMonster(""povoheroico"", ""r2"", ""Bottom"", ""Brado Retumbante"", req.Name, quant, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Brabo Retumbante",
    @"
case ""Brabo Retumbante"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);

                    Core.KillMonster(""povoheroico"", ""r2"", ""Bottom"", ""Brado Retumbante"", req.Name, quant, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Brabos Retumbantes",
    @"
case ""Brabos Retumbantes"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);

                    Core.KillMonster(""povoheroico"", ""r2"", ""Bottom"", ""Brado Retumbante"", req.Name, quant, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Brado Retumbante",
    @"
case ""Brado Retumbante"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);

                    Core.KillMonster(""povoheroico"", ""r2"", ""Bottom"", ""Brado Retumbante"", req.Name, quant, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Capuz Retumbante",
    @"
case ""Capuz Retumbante"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);

                    Core.KillMonster(""povoheroico"", ""r2"", ""Bottom"", ""Brado Retumbante"", req.Name, quant, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Face Retumbante",
    @"
case ""Face Retumbante"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);

                    Core.KillMonster(""povoheroico"", ""r2"", ""Bottom"", ""Brado Retumbante"", req.Name, quant, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Símbolo da Ordem",
    @"
case ""Símbolo da Ordem"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);

                    Core.KillMonster(""povoheroico"", ""r2"", ""Bottom"", ""Brado Retumbante"", req.Name, quant, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Símbolo do Progresso",
    @"
case ""Símbolo do Progresso"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);

                    Core.KillMonster(""povoheroico"", ""r2"", ""Bottom"", ""Brado Retumbante"", req.Name, quant, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Batik Fabric",
    @"
case ""Batik Fabric"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10371); // TODO: Replace with actual quest ID
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(""rangda"", ""Rangda"", req.Name, 1);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Benang",
    @"
case ""Benang"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.RegisterQuests(10372); // TODO: Replace with actual quest ID
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonster(""rangda"", ""Tuyul"", ""Tuyul Soul"", 9);
                        Core.HuntMonster(""rangda"", ""Leyak"", ""Leyak Jaw"", 9);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Warok's Wicked Snapper",
    @"
case ""Warok's Wicked Snapper"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""rangda"", ""Rangda"", req.Name, quant, isTemp: req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Duskwind Warok Visage",
    @"
case ""Duskwind Warok Visage"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""rangda"", ""Rangda"", req.Name, quant, isTemp: req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Elder Warok Morph",
    @"
case ""Elder Warok Morph"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""rangda"", ""Rangda"", req.Name, quant, isTemp: req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Southern King Hair",
    @"
case ""Southern King Hair"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""rangda"", ""Leyak"", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Ocean Maiden’s Locks",
    @"
case ""Ocean Maiden’s Locks"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""rangda"", ""Leyak"", req.Name, req.Quantity, req.Temp);
                    Bot.Wait.ForPickup(req.Name);
                    break;

    "
},
{
    "Belladonna's Flag",
    @"
case ""Belladonna's Flag"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10390); // Bumbling Bellamy
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.KillMonster(""piratehunt"", ""r5"", ""Left"", ""Captain Bellamy"", ""Bellamy's Greasy Beard"", 1, false);
                    Core.EnsureComplete(10390);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Bourgeois' Flag",
    @"
case ""Bourgeois' Flag"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10392); // From the Top
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.KillMonster(""piratehunt"", ""r7"", ""Left"", ""Captain Verich"", ""Captain Verich's Tricorn"", 1, false);
                    Core.EnsureComplete(10392);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Pirated Tech's Flag",
    @"
case ""Pirated Tech's Flag"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10394); // Subversive Sailor
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.KillMonster(""piratehunt"", ""r9"", ""Left"", ""Captain Chamfer"", ""Captain Chamfer's Mods"", 1, false);
                    Core.EnsureComplete(10394);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Merry Celeste's Flag",
    @"
case ""Merry Celeste's Flag"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10396); // Salty Spirit
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.KillMonster(""piratehunt"", ""r11"", ""Left"", ""Captain Haines"", ""Captain Haines' Soul"", 1, false);
                    Core.EnsureComplete(10396);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Mercurius' Flag",
    @"
case ""Mercurius' Flag"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10398); // Mercurious
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.KillMonster(""piratehunt"", ""r13"", ""Left"", ""*"", ""Larunda's Counterfeit Amulet"", 1, false);
                        Core.KillMonster(""piratehunt"", ""r13"", ""Left"", ""*"", ""Mercurius' Funny Hat"", 1, false);
                    }
                    Core.EnsureComplete(10398);
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Cutlass of Awe",
    @"
case ""Cutlass of Awe"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(""Cutlass of Awe"");

                    while (!Bot.ShouldExit && !Core.CheckInventory(""Cutlass of Awe"", quant))
                    {
                        if (!Core.isCompletedBefore(10388) && !Core.CheckInventory(""Cutlass of Awe""))
                            Core.EnsureAccept(10388);

                        if (!Core.CheckInventory(""Cutlass of Awe Handle""))
                            Core.KillMonster(""seakingkurok"", ""r2"", ""Left"", ""Sea King Gravefang"", ""Cutlass of Awe Handle"", 1, false);

                        if (!Core.CheckInventory(""Cutlass of Awe Hilt""))
                            Core.KillMonster(""dragoncapital"", ""r8"", ""Left"", ""Empowered Scalebeard"", ""Cutlass of Awe Hilt"", 1, false);

                        if (!Core.CheckInventory(""Cutlass of Awe Blade""))
                            Core.KillMonster(""kaijuwar"", ""r9"", ""Left"", ""Captain Kraylox"", ""Cutlass of Awe Blade"", 1, false);

                        if (!Core.CheckInventory(""Awe Binding Spell""))
                            Core.KillMonster(""blazingbeach"", ""r8"", ""Left"", ""Magma Blazebeard"", ""Awe Binding Spell"", 1, false);

                        if (Core.CheckInventory(""Cutlass of Awe Handle"") &&
                            Core.CheckInventory(""Cutlass of Awe Hilt"") &&
                            Core.CheckInventory(""Cutlass of Awe Blade"") &&
                            Core.CheckInventory(""Awe Binding Spell""))
                        {
                            Core.EnsureComplete(10388);
                            Bot.Wait.ForPickup(""Cutlass of Awe"");
                        }
                    }
                    break;
    "
},
{
    "Dual Cutlass of Awe",
    @"
case ""Dual Cutlass of Awe"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(""Cutlass of Awe"");

                    while (!Bot.ShouldExit && !Core.CheckInventory(""Cutlass of Awe"", quant))
                    {
                        if (!Core.isCompletedBefore(10388) && !Core.CheckInventory(""Cutlass of Awe""))
                            Core.EnsureAccept(10388);

                        if (!Core.CheckInventory(""Cutlass of Awe Handle""))
                            Core.KillMonster(""seakingkurok"", ""r2"", ""Left"", ""Sea King Gravefang"", ""Cutlass of Awe Handle"", 1, false);

                        if (!Core.CheckInventory(""Cutlass of Awe Hilt""))
                            Core.KillMonster(""dragoncapital"", ""r8"", ""Left"", ""Empowered Scalebeard"", ""Cutlass of Awe Hilt"", 1, false);

                        if (!Core.CheckInventory(""Cutlass of Awe Blade""))
                            Core.KillMonster(""kaijuwar"", ""r9"", ""Left"", ""Captain Kraylox"", ""Cutlass of Awe Blade"", 1, false);

                        if (!Core.CheckInventory(""Awe Binding Spell""))
                            Core.KillMonster(""blazingbeach"", ""r8"", ""Left"", ""Magma Blazebeard"", ""Awe Binding Spell"", 1, false);

                        if (Core.CheckInventory(""Cutlass of Awe Handle"") &&
                            Core.CheckInventory(""Cutlass of Awe Hilt"") &&
                            Core.CheckInventory(""Cutlass of Awe Blade"") &&
                            Core.CheckInventory(""Awe Binding Spell""))
                        {
                            Core.EnsureComplete(10388);
                            Bot.Wait.ForPickup(""Cutlass of Awe"");
                        }
                    }
                    break;
    "
},
{
    "Gold Voucher 100k",
    @"
case ""Gold Voucher 100k"":
                    Farm.Voucher(req.Name, req.Quantity);
                    break;
    "
},
{
    "J6's Secret Hideout Map",
    @"
case ""J6's Secret Hideout Map"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.Name);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                        Core.HuntMonster(""j6"", ""Sketchy Dragon"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Novac Sal Privateer",
    @"
case ""Novac Sal Privateer"":
                    if (!Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership. Skipping."");
                        break;
                    }
                    goto default;
    "
},
{
    "Novac Sal Pirate Morph",
    @"
case ""Novac Sal Pirate Morph"":
                    if (!Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership. Skipping."");
                        break;
                    }
                    goto default;
    "
},
{
    "Novac Sal Pirate Visage",
    @"
case ""Novac Sal Pirate Visage"":
                    if (!Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership. Skipping."");
                        break;
                    }
                    goto default;
    "
},
{
    "Bloodstained Doubloon",
    @"
case ""Bloodstained Doubloon"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }
                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(Core.IsMember ? 10412 : 10410,
                        (""piratealliance"", ""S.S. Phantom Pirate"", ClassType.Farm),
                        (""piratealliance"", ""Captain Squalus"", ClassType.Solo),
                        (""piratealliance"", ""Phantom Jaws"", ClassType.Solo));
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Mer-Unit Operator Hair",
    @"
case ""Mer-Unit Operator Hair"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""piratealliance"", ""Phantom Jaws"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Mer-Unit Operator Locks",
    @"
case ""Mer-Unit Operator Locks"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""piratealliance"", ""Phantom Jaws"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Spear of Lost Jewels",
    @"
case ""Spear of Lost Jewels"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""piratealliance"", ""Phantom Jaws"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "White Flame of Albedo",
    @"
case ""White Flame of Albedo"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.AddDrop(req.ID);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.HuntMonsterQuest(10423,
                        (""forgealbedo"", ""Defensive Turret"", ClassType.Farm),
                        (""forgealbedo"", ""Defense Droid"", ClassType.Farm),
                        (""forgealbedo"", ""Flame of Albedo"", ClassType.Solo));
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Noelle Warden Morph",
    @"
case ""Noelle Warden Morph"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""thelimacity"", ""Noelle Knight"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Noelle Warden Visage",
    @"
case ""Noelle Warden Visage"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""thelimacity"", ""Noelle Knight"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Cape of the Noelle Knight",
    @"
case ""Cape of the Noelle Knight"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""thelimacity"", ""Noelle Knight"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Noelle Warden Bob",
    @"
case ""Noelle Warden Bob"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""thelimacity"", ""Noelle Knight"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Noelle Warden Ponytail",
    @"
case ""Noelle Warden Ponytail"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""thelimacity"", ""Noelle Knight"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Stars of Capella",
    @"
case ""Stars of Capella"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""forgealbedo"", ""Flame of Albedo"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Rusted Cutlass",
    @"
case ""Rusted Cutlass"":
                    Core.HuntMonster(""piratealliance"", ""Captain Squalus"", req.Name, quant, isTemp: false);
                    break;
    "
},
{
    "Mer-Unit Operator Morph",
    @"
case ""Mer-Unit Operator Morph"":
                    if (Bot.Inventory.GetQuantity(""Mer-Unit Operator Locks"") < 1)
                        Core.HuntMonster(""piratealliance"", ""Phantom Jaws"", ""Mer-Unit Operator Locks"", 1, isTemp: false);
                    if (Bot.Inventory.GetQuantity(""Mer-Unit Operator Hair"") < 1)
                        Core.HuntMonster(""piratealliance"", ""Phantom Jaws"", ""Mer-Unit Operator Hair"", 1, isTemp: false);
                    if (Bot.Inventory.GetQuantity(""Bloodstained Doubloon"") < 25)
                        findIngredients(); // farm more Doubloons if needed
                    Adv.BuyItem(""piratealliance"", 2623, req.Name, quant);
                    break;
    "
},
{
    "Mortal Sea King Gravefang",
    @"
case ""Mortal Sea King Gravefang"":
                    Core.HuntMonster(""piratealliance"", req.Name, req.Name, quant, isTemp: false);
                    Adv.BuyItem(""piratealliance"", 2623, req.Name, quant);
                    break;
    "
},
{
    "Sea King Gravefang",
    @"
case ""Sea King Gravefang"":
                    if (Bot.Inventory.GetQuantity(""Mortal Sea King Gravefang"") < 1)
                        Adv.BuyItem(""piratealliance"", 2623, ""Mortal Sea King Gravefang"", 1);
                    Adv.BuyItem(""piratealliance"", 2623, req.Name, quant);
                    break;
    "
},
{
    "Burger Buns",
    @"
case ""Burger Buns"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.Name);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7522); // Burglinster's Revenge
                        Core.HuntMonster(""borgars"", ""Burglinster"", ""Burglinster Cured"", 1, isTemp: true);
                        Core.EnsureComplete(7522);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Sugar Cubes",
    @"
case ""Sugar Cubes"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.Name);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7512); // Sacchar-Imp
                        Core.HuntMonster(""freakitiki"", ""Sugar Imp"", ""Sugar"", 5, isTemp: true);
                        Core.EnsureComplete(7512);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Cowbell",
    @"
case ""Cowbell"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.Name);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7511); // Creamy!
                        Core.HuntMonster(""battlefowl"", ""Chickencow"", ""Heavy Cream"", 1, isTemp: true);
                        Core.EnsureComplete(7511);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Fish Oil",
    @"
case ""Fish Oil"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.Name);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EnsureAccept(7514); // Filet o' Fishwing
                        Core.HuntMonster(""pirates"", ""Fishwing"", ""Fishwing Filet"", 5, isTemp: true);
                        Core.EnsureComplete(7514);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    break;
    "
},
{
    "Shadow Skull",
    @"
case ""Shadow Skull"":
                    Core.RegisterQuests(4993);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, req.Quantity))
                    {
                        if (!Core.CheckInventory(""Silver DeathKnight Lord""))
                            Core.BuyItem(""towersilver"", 1243, ""Silver DeathKnight Lord"");
                        if (!Core.CheckInventory(""Golden DeathKnight Lord""))
                            Core.BuyItem(""towergold"", 1243, ""Golden DeathKnight Lord"");


                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""bonecastle"", ""Green Rat"", ""Gamey Rat Meat"", 3);
                        Core.HuntMonster(""bonecastle"", ""Undead Waiter"", ""Waiter's Notepad"", 1);
                        Core.HuntMonster(""bonecastle"", ""Turtle"", ""Turtle's Eggs"", 6);
                        Core.HuntMonster(""bonecastle"", ""Ghoul"", ""Ghoul \""Vinegar\"""", 6);
                        Core.HuntMonster(""bonecastle"", ""Grateful Undead"", ""Spices"", 2);

                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""bonecastle"", ""The Butcher"", ""Bag of Bone Flour"", 1);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

    "
},
{
    "DeathKnight Lord Armor",
    @"
case ""DeathKnight Lord Armor"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }
                    string[] RequiredItems = new[]
                            {
                                ""DeathKnight Lord Gauntlets"",
                                ""DeathKnight Lord Greaves"",
                                ""DeathKnight Lord Chest Plate"",
                                ""DeathKnight Lord Hauberk"",
                                ""DeathKnight Lord Boots""
                            };
                    foreach (string item in RequiredItems)
                        Core.HuntMonster(""bonecastle"", ""Vaden"", item, isTemp: false);


                    // BoneCastle Amulet
                    Core.RegisterQuests(4993);
                    while (!Bot.ShouldExit && !Core.CheckInventory(""Bonecastle Amulet"", 30))
                    {
                        if (!Core.CheckInventory(""Silver DeathKnight Lord""))
                            Core.BuyItem(""towersilver"", 1243, ""Silver DeathKnight Lord"");
                        if (!Core.CheckInventory(""Golden DeathKnight Lord""))
                            Core.BuyItem(""towergold"", 1243, ""Golden DeathKnight Lord"");


                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster(""bonecastle"", ""Green Rat"", ""Gamey Rat Meat"", 3);
                        Core.HuntMonster(""bonecastle"", ""Undead Waiter"", ""Waiter's Notepad"", 1);
                        Core.HuntMonster(""bonecastle"", ""Turtle"", ""Turtle's Eggs"", 6);
                        Core.HuntMonster(""bonecastle"", ""Ghoul"", ""Ghoul \""Vinegar\"""", 6);
                        Core.HuntMonster(""bonecastle"", ""Grateful Undead"", ""Spices"", 2);

                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster(""bonecastle"", ""The Butcher"", ""Bag of Bone Flour"", 1);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    if (RequiredItems.All(x => Core.CheckInventory(x))
                     && Core.CheckInventory(""Bonecastle Amulet"", 30))
                    {
                        Core.BuyItem(""bonecastle"", 1242, req.Name);
                    }
                    Bot.Wait.ForItemBuy(req.ID);
                    break;
    "
},
{
    "Dread Thread",
    @"
case ""Dread Thread"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10427); // Arach-kids
                    Core.EquipClass(ClassType.Farm);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""abaddoncave"", ""Cursed Dreadspider"", ""Dreadspider Carapace"", 9);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Abaddon Vertebrae",
    @"
case ""Abaddon Vertebrae"":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10428); // Arachnosapien
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster(""abaddoncave"", ""Spineback Abbadon"", ""Abaddon Carapace"");
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
    "
},
{
    "Spineback Abaddon Guard",
    @"
case ""Spineback Abaddon Guard"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""abaddoncave"", ""Spineback Abbadon"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Spineback Abaddon Guest",
    @"
case ""Spineback Abaddon Guest"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""abaddoncave"", ""Spineback Abbadon"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Spineback Abaddon Carapace",
    @"
case ""Spineback Abaddon Carapace"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""abaddoncave"", ""Spineback Abbadon"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Arachnosapien Locks",
    @"
case ""Arachnosapien Locks"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""abaddoncave"", ""Spineback Abbadon"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Arachnosapien Visage",
    @"
case ""Arachnosapien Visage"":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster(""abaddoncave"", ""Spineback Abbadon"", req.Name, quant, isTemp: false);
                    Bot.Wait.ForPickup(req.Name);
                    break;
    "
},
{
    "Mana Orb",
    @"
case ""Mana Orb"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""castleparty"", ""Legion Partycrasher"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Mana Creation Orb",
    @"
case ""Mana Creation Orb"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""castleparty"", ""Legion Partycrasher"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Gilded Gem",
    @"
case ""Gilded Gem"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""castleparty"", ""Noxus' Gift"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Noble Ether Staff",
    @"
case ""Noble Ether Staff"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""castleparty"", ""Treasure Chest"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Gleaming Ore",
    @"
case ""Gleaming Ore"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""castleparty"", ""Treasure Chest"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Royal Fortune Sword",
    @"
case ""Royal Fortune Sword"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""castleparty"", ""Treasure Chest"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Royal Fortune Swords",
    @"
case ""Royal Fortune Swords"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""castleparty"", ""Treasure Chest"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Iota of Eternity",
    @"
case ""Iota of Eternity"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""castleparty"", ""Kathool's Gift"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Flux Sigil",
    @"
case ""Flux Sigil"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""castleparty"", ""Drakath's Gift"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Darkness Rune",
    @"
case ""Darkness Rune"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""castleparty"", ""Nulgath's Gift"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Shadowbound Magus Cloak",
    @"
case ""Shadowbound Magus Cloak"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""castleparty"", ""Nulgath's Gift"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Dark Descent Rune",
    @"
case ""Dark Descent Rune"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""castleparty"", ""Nulgath's Gift"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Dominating Shadowbinder",
    @"
case ""Dominating Shadowbinder"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""castleparty"", ""Nulgath's Gift"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Dark Descent Sigil",
    @"
case ""Dark Descent Sigil"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""castleparty"", ""Nulgath's Gift"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Undying Essence",
    @"
case ""Undying Essence"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""castleparty"", ""Sally's Gift"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Giftbox Ribbon",
    @"
case ""Giftbox Ribbon"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""castleparty"", ""Lost Giftbox"", req.Name, quant, req.Temp, false);
                    break;
    "
},
{
    "Golden Euphoria Blade",
    @"
case ""Golden Euphoria Blade"":
                    if (req.Upgrade && !Core.IsMember)
                    {
                        Core.Logger($""{req.Name} requires membership to farm, skipping."");
                        return;
                    }

                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.AddDrop(req.ID);
                    Core.HuntMonster(""castleparty"", ""Lost Giftbox"", req.Name, quant, req.Temp, false);
                    break;
    "
},
};

    public static bool TryGetCase(string itemName, out string? logic)
        => Cases.TryGetValue(itemName, out logic);

    /// <summary>
    /// Returns the case logic with placeholders replaced by provided values.
    /// </summary>
    public static string GetCaseWithValues(string itemName, string reqName, int quant, string map, string monster, string drop)
    {
        if (!TryGetCase(itemName, out string? logic) || string.IsNullOrWhiteSpace(logic))
            return string.Empty;

        return logic
            .Replace("{reqName}", reqName)
            .Replace("{quant}", quant.ToString())
            .Replace("{map}", map)
            .Replace("{monster}", monster)
            .Replace("{drop}", drop);
    }
}