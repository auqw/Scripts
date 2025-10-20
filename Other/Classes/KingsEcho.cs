/*
name: KingsEchoClassPrerequisites
description: King's Echo Class Prerequisites
tags: Prerequisites, King's, King, Echo, class
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
//cs_include Scripts/Other/MergeShops/BocklinTreasuryMerge.cs
//cs_include Scripts/Story/Lynaria/CoreLynaria.cs
//cs_include Scripts/Other/MergeShops/BocklinGroveMerge.cs
//cs_include Scripts/Other/MergeShops/BocklinArmoryMerge.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;

public class KingsEchoClassPrerequisites
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static CoreDailies Daily { get => _Daily ??= new CoreDailies(); set => _Daily = value; }
    private static CoreDailies _Daily;
    private static CoreAOR AOR { get => _AOR ??= new CoreAOR(); set => _AOR = value; }
    private static CoreAOR _AOR;
    private static BocklinTreasuryMerge BTM { get => _BTM ??= new BocklinTreasuryMerge(); set => _BTM = value; }
    private static BocklinTreasuryMerge _BTM;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions(disableClassSwap: true);

        Prerequisites();

        Core.SetOptions(false);
    }

    public void Prerequisites(bool rankup = true)
    {
        if (Core.CheckInventory("King's Echo"))
        {
            if (rankup)
                Adv.RankUpClass("King's Echo");
            return;
        }

        // Level 80
        Farm.Experience(80);


        // Rank 10 in the Good and Swordhaven factions
        Farm.GoodREP();
        Farm.SwordhavenREP();

        // Alden's Liberation Armor
        BTM.BuyAllMerge("Alden's Liberation Armor");

        if (!Core.isCompletedBefore(10439))
        {
            // Completion of the Rumbling of Cold Thunder saga
            AOR.TerminaTemple(true, true);

            Quest q = Core.InitializeWithRetries(() => Core.EnsureLoad(10439));
            foreach (ItemBase item in q.AcceptRequirements)
            {
                if (Core.CheckInventory(item.ID))
                    continue;

                switch (item.Name)
                {
                    case "King Alteon's Crown": // 10440 | Purify The Crown
                        // Updatequest to see queen of monsters
                        if (!Core.isCompletedBefore(8361))
                            Bot.Quests.UpdateQuest(8361);

                        Core.EnsureAccept(10440);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("queenbattle", "Queen of Monsters", "Calcified Tear", 60, isTemp: false);
                        Core.HuntMonster("dreadhaven", "Slugwrath", "Whispers of Chaos", 40, isTemp: false);
                        Core.HuntMonster("ultradrakath", "Champion of Chaos", "Trace of Chaos", 13, isTemp: false);
                        Core.EnsureComplete(10440);
                        break;

                    case "Royal Dragon Sword": // 10442 | The Path of the King
                        Core.EnsureAccept(10442);
                        Core.EquipClass(ClassType.Farm);
                        Core.KillMonster("chaoswar", "r9", "Left", "*", "Endured Against Chaos", 113, isTemp: false);
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("falcontower", "Sepulchure", "Endured Against a Fallen Friend", 100, isTemp: false);
                        Core.HuntMonster("naoisegrave", "Volgritian", "Endured Against the Great Dragon", 70, isTemp: false);
                        Core.HuntMonster(Core.IsMember ? "shattersword" : "infernalarena", Core.IsMember ? "Graveclaw the Defiler" : "Destructive Defiler", "Endured Against the Defiler", 100, isTemp: false);
                        Core.HuntMonster("ebondungeon", "Dethrix", "Endured Against the Monster King", 90, isTemp: false);
                        Core.EnsureComplete(10442);
                        break;

                    case "Alteon's Reforged Armor":
                        Core.EnsureAccept(10441);
                        Farm.Voucher("Gold Voucher 500k", 100);
                        Core.EquipClass(ClassType.Farm);
                        Core.KillMonster("thelimacity", "r5", "Center", "*", "Dwarven Gold", 30, isTemp: false);
                        Core.KillMonster("liatarahill", "r9", "Left", "*", "Skye Gold", 50, isTemp: false);
                        Core.KillMonster("atlaskingdom", "r2", "Left", "*", "Atlas Gold", 75, isTemp: false); // Temporary solution, needs to be replaced by a farmable alternative rather than a boss.
                        Core.EquipClass(ClassType.Solo);
                        Core.KillMonster("camlan", "r9", "Left", "*", "Camlan Gold", 120, isTemp: false);
                        Core.EnsureComplete(10441);
                        break;

                    case "Alden's Mace":
                        Core.BuyItem("castle", 2631, 96387);
                        break;
                }
            }

            // Echo of the King
            Core.ChainComplete(10439);
            Core.BuyItem("TerminaTemple", 2630, 95742);

            if (rankup)
                Adv.RankUpClass("King's Echo");
        }

    }
}



