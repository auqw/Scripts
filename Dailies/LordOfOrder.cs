/*
name: Lord Of Order (Daily)
description: This bot will do the dailies for the Lord Of Order, after that is done it will get the remainder rewards
tags: daily, lord, order, LOO, mirror, realm, support, xing, xang, kitsune, alteon, vath, wolfwing, ledgermayne, khasaanda
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Story/Nation/CitadelRuins.cs
//cs_include Scripts/Story/DragonFableOrigins.cs
using Skua.Core.Interfaces;

public class LordOfOrder
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static CitadelRuins CR { get => _CR ??= new CitadelRuins(); set => _CR = value; }
    private static CitadelRuins _CR;
    private static DragonFableOrigins DFO { get => _DFO ??= new DragonFableOrigins(); set => _DFO = value; }
    private static DragonFableOrigins _DFO;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetLoO();

        Core.SetOptions(false);
    }

    public void GetLoO(bool rankUpClass = true, bool getExtras = false)
    {

        // LOO Quest ItemID: 50741
        // LOO Collector Chest ItemID: 50576

        // Check if the item is already in inventory or if extras are needed
        if ((Core.CheckInventory(new[] { 50741, 50576 }, any: true, toInv: false) && !getExtras && Core.isCompletedBefore(7165)) ||
            (getExtras && Core.CheckInventory(Core.QuestRewards(7165), toInv: false) && Core.isCompletedBefore(7165)))
        {
            if (rankUpClass)
                Adv.RankUpClass("Lord Of Order");
            if (getExtras)
                Core.Logger("All desired rewards owned for LOO.");
            return;
        }

        Story.PreLoad(this);
        Core.Logger("Daily: Lord Of Order Class");
        Core.Logger("For the idiots that keep complaining its not continuing, this is a *daily* Quest, upto the LOO Quest [7165] (10 dailies)");

        Farm.Experience(50);
        // Heart of Servitude
        if (Bot.Quests.IsAvailable(7156) && !Core.isCompletedBefore(7156))
        {
            Core.EnsureAccept(7156);
            Core.AddDrop(Core.QuestRewards(7156));
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("watchtower", "Chaorrupted Knight", "Pristine Blades of Order", isTemp: false);
            Core.BuyItem("dreadrock", 1221, "Dreadrock Donation Receipt");
            Core.HuntMonster("deadmoor", "Banshee Mallora", "Deadmoor Spirits Helped", isTemp: false);
            CR.MurrysQuests();
            CR.PolishsQuestsCitadelRuins();
            if (!Core.CheckInventory("Mage's Gratitude"))
            {
                Core.AddDrop("Mage's Gratitude");
                Core.EnsureAccept(6182);
                Core.HuntMonster("citadelruins", "Enn'tröpy", "Enn'tröpy Defeated", isTemp: true);
                Core.EnsureComplete(6182);
            }
            Core.BuyItem("ravenscar", 614, "Ravenscar's Truth");

            Core.EnsureComplete(7156);
            Core.ToBank(Core.QuestRewards(7156));
            return;
        }

        // Spirit of Justice
        if (Bot.Quests.IsAvailable(7157) && !Core.isCompletedBefore(7157))
        {
            Core.EnsureAccept(7157);
            Core.AddDrop(Core.QuestRewards(7157));

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("dwarfprison", "Warden Elfis", "Warden Elfis Detained", isTemp: false);
            Core.HuntMonster("prison", "Piggy Drake", "Piggy Drake Punished", isTemp: false);
            Core.HuntMonster("mysteriousdungeon", "Mysterious Stranger", "Mysterious Stranger Foiled", isTemp: false);
            Core.HuntMonster("dreammaster", "Calico Cobby", "Calico Cobby Crushed", isTemp: false);

            Core.EnsureComplete(7157);
            Core.ToBank(Core.QuestRewards(7157));
            return;
        }

        // Purification of Chaos
        if (Bot.Quests.IsAvailable(7158) && !Core.isCompletedBefore(7158))
        {
            Core.EnsureAccept(7158);
            Core.AddDrop(Core.QuestRewards(7158));

            Core.EquipClass(ClassType.Solo);

            Adv.BuyItem("tercessuinotlim", 1951, "Chaoroot", 15);
            Core.HuntMonster("chaosboss", "Ultra Chaos Warlord", "Chaotic War Essence", 15, false);
            Core.HuntMonster("shadowgates", "Chaorruption", "Chaorrupting Particles", 15, false);
            Bot.Quests.UpdateQuest(2814);
            Core.HuntMonster("stormtemple", "Chaos Lord Lionfang", "Purified Raindrop", 45, false);

            Core.EnsureComplete(7158);
            Core.ToBank(Core.QuestRewards(7158));
            return;
        }

        // Steadfast Will
        if (Bot.Quests.IsAvailable(7159) && !Core.isCompletedBefore(7159))
        {
            Core.EnsureAccept(7159);
            Core.AddDrop(Core.QuestRewards(7159));

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("gaiazor", "Gaiazor", "Gaiazor's Cornerstone", isTemp: false, publicRoom: Core.PublicDifficult);
            Bot.Quests.UpdateQuest(4361);
            Core.HuntMonster("treetitanbattle", "Dakka the Dire Dragon", "Dakka's Crystal", isTemp: false);
            Core.HuntMonster("andre", "Giant Necklace", "Andre's Necklace Fragment", isTemp: false);
            // Perma-Aggroed mob escape.
            Core.JumpWait();
            Core.HuntMonster("desolich", "Desolich", "Desolich's Skull", isTemp: false, publicRoom: Core.PublicDifficult);

            Core.EnsureComplete(7159);
            Core.ToBank(Core.QuestRewards(7159));
            return;
        }

        // Strike of Order
        if (Bot.Quests.IsAvailable(7160) && !Core.isCompletedBefore(7160))
        {
            Core.EnsureAccept(7160);
            Core.AddDrop(Core.QuestRewards(7160));

            Core.EquipClass(ClassType.Solo);
            Core.KillKitsune("Hanzamune Dragon Koi Blade");
            Core.HuntMonster("ledgermayne", "Ledgermayne", "The Supreme Arcane Staff", isTemp: false);
            Core.HuntMonster("mqlesson", "Dragonoid", "Dragonoid of Hours", isTemp: false);
            if (!Core.CheckInventory("Safiria's Spirit Orb"))
            {
                Core.AddDrop("Safiria's Spirit Orb");
                Core.GetMapItem(5470, 1, "maxius");
                Bot.Wait.ForPickup("Safiria's Spirit Orb");
            }
            DFO.DragonFableOriginsAll();
            if (!Core.CheckInventory("Ice Katana"))
            {
                Core.AddDrop("Ice Katana");
                Core.EnsureAccept(6319);
                Core.EquipClass(ClassType.Farm);
                Core.HuntMonster("drakonnan", "Living Fire", "Inferno Heart");
                Core.EnsureComplete(6319);
            }
            Core.EnsureComplete(7160);
            Core.ToBank(Core.QuestRewards(7160));
            return;
        }

        // Harmony
        if (Bot.Quests.IsAvailable(7161) && !Core.isCompletedBefore(7161))
        {
            Core.EnsureAccept(7161);
            Core.AddDrop(Core.QuestRewards(7161));

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("elemental", "Tree of Destiny", "Unity of Life", isTemp: false);
            Core.HuntMonster("orchestra", "Faust", "Harmony of Solace", isTemp: false);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("cathedral", "Pactagonal Knight", "Teamwork Observed", 100, isTemp: false);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("goose", "Queen's ArchSage", "Scroll of Enchantment", isTemp: false);

            Core.EnsureComplete(7161);
            Core.ToBank(Core.QuestRewards(7161));
            return;
        }

        // Ordinance
        if (Bot.Quests.IsAvailable(7162) && !Core.isCompletedBefore(7162))
        {
            Core.EnsureAccept(7162);
            Core.AddDrop(Core.QuestRewards(7162));

            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("newfinale", "Alliance Healer", "Acolyte's Braille", isTemp: false);
            Core.HuntMonster("wardwarf", "Drow Assassin", "Suppressed Drows", 50, false);
            Core.HuntMonster("warundead", "Skeletal Fire Mage", "Suppressed Undead", 50, false);
            Core.HuntMonster("warhorc", "Horc Warrior", "Suppressed Horcs", 50, false);
            Core.HuntMonster("weaverwar", "Weaver Queen's Hound", "Suppressed Weavers", 50, false);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("extriki", "Extriki", "Strength of Resilience", isTemp: false);

            Core.EnsureComplete(7162);
            Core.ToBank(Core.QuestRewards(7162));
            return;
        }

        // Axiom
        if (Bot.Quests.IsAvailable(7163) && !Core.isCompletedBefore(7163))
        {
            Core.EnsureAccept(7163);
            Core.AddDrop(Core.QuestRewards(7163));

            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("elfhame", "Guardian Spirit", "Law of Nature", isTemp: false);
            Core.HuntMonster("deepchaos", "Kathool", "Law of Time", isTemp: false);
            Core.HuntMonster("necrocavern", "ShadowStone Support", "Law of Gravity", isTemp: false);
            Core.HuntMonster("blackholesun", "Reflecteract", "Law of Relativity", isTemp: false);
            Core.HuntMonster("thunderfang", "Tonitru", "Law of Conservation of Energy", isTemp: false);
            Core.HuntMonster("lair", "Red Dragon", "Law of Low Drop Rates", 100, false);

            Core.EnsureComplete(7163);
            Core.ToBank(Core.QuestRewards(7163));
            return;
        }

        // Blessing of Order
        if (Bot.Quests.IsAvailable(7164) && !Core.isCompletedBefore(7164))
        {
            Core.EnsureAccept(7164);
            Core.AddDrop(Core.QuestRewards(7164));

            Core.EquipClass(ClassType.Solo);
            Core.KillMonster("doomvaultb", "r26", "Left", "Undead Raxgore", "Weapon Imprint", 15, false);
            Farm.FishingREP(7);
            Core.BuyItem("greenguardwest", 363, "Lure of Order");
            Adv.GearStore();
            Core.KillXiang("Quixotic Mana Essence", 10, true);
            Adv.GearStore(true);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("yasaris", "Serepthys", "Inversion Infusion", 5, false);

            Core.EnsureComplete(7164);
            Core.ToBank(Core.QuestRewards(7164));
            return;
        }

        // The Final Challenge
        if (Bot.Quests.IsAvailable(7165) && !Core.isCompletedBefore(7165))
        {
            Bot.Drops.Add(Core.QuestRewards(7165));

            Core.EnsureAccept(7165);
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("ultradrakath", "Champion of Chaos", "Champion of Chaos Confronted", isTemp: false, publicRoom: Core.PublicDifficult);
            Bot.Drops.Add(50741);
            // If quest 7165 is not completed and either missing extras or missing key items
            if (!Core.isCompletedBefore(7165))
            {
                Core.EnsureComplete(7165, 50741);
                Bot.Wait.ForPickup(50741);

                Core.EnsureCompleteChoose(7165);
                Core.ToBank(Core.QuestRewards(7165).Except("Lord Of Order"));

                if (rankUpClass)
                    Adv.RankUpClass("Lord Of Order");
            }
        }
    }
}
