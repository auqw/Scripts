/*
name: Celestial Of Mysteries
description: This script will complete "Luctus in Perpetuum" [10096] quest.
tags: celestial, aranx, azalith, luctus in perpetuum, celestial realm, infernaldianoia,extra,qom,celestial of mysteries,luctus,perpetuum
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/CelestialPast.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/InfernalParadise.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/InfernalDianoia.cs
//cs_include Scripts/Other/MergeShops/InfernalParadiseMerge.cs
//cs_include Scripts/Other/MergeShops/InfernalCelestialFinaleMerge.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;

public class CelestialOfMysteries
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
    private static InfernalDianoia ID
    {
        get => _ID ??= new InfernalDianoia();
        set => _ID = value;
    }
    private static InfernalDianoia _ID;
    private static InfernalParadiseMerge IPM
    {
        get => _IPM ??= new InfernalParadiseMerge();
        set => _IPM = value;
    }
    private static InfernalParadiseMerge _IPM;
    private static InfernalCelestialFinaleMerge ICFM
    {
        get => _ICFM ??= new InfernalCelestialFinaleMerge();
        set => _ICFM = value;
    }
    private static InfernalCelestialFinaleMerge _ICFM;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        DoQuest();
        Core.SetOptions(false);
    }

    public void DoQuest()
    {
        if (Core.CheckInventory(Core.QuestRewards(10096)))
            return;

        // Prereqs
        ID.Storyline();
        Farm.Experience(90);
        Quest? q = Core.InitializeWithRetries(() => Core.EnsureLoad(10096));
        if (q == null)
        {
            Core.Logger("Failed to load quest 10096.");
            return;
        }
        foreach (ItemBase req in q.AcceptRequirements)
        {
            if (!Core.CheckInventory(req.ID))
            {
                IPM.BuyAllMerge(req.Name); // Archangel of Mysteries Armor, Helm and Wings
            }
            Core.Unbank(req.ID);
        }

        // Gold Voucher 500k
        Farm.Voucher("Gold Voucher 500k", 35);
        Adv.GearStore(true);
        // Duo's Dinner
        if (!Core.CheckInventory("Duo's Dinner", 35))
        {
            Core.UseBossClass();
            Core.HuntMonster("infernalarena", "Deadly Duo", "Duo's Dinner", 35, false);
        }
        // Cervus Dente
        if (!Core.CheckInventory("Cervus Dente", 35))
        {
            Core.UseBossClass();
            Core.HuntMonster("infernalarena", "Cervus Malus", "Cervus Dente", 35, false);
        }
        // Infernal Incantation
        if (!Core.CheckInventory("Infernal Incantation", 35))
        {
            Core.UseBossClass("Dragon of Time");
            Adv.EnhanceEquipped(EnhancementType.Healer, wSpecial: WeaponSpecial.Elysium);
            Core.HuntMonster("infernalarena", "Key of Sholemoh", "Infernal Incantation", 35, false);
        }
        // Scythe Shard
        if (!Core.CheckInventory("Scythe Shard", 35))
        {
            Adv.EnhanceEquipped(EnhancementType.Lucky, wSpecial: WeaponSpecial.Health_Vamp);
            Core.UseDodgeClass("Lord of Order");
            Core.HuntMonster("infernalarena", "Azalith's Scythe", "Scythe Shard", 35, false);
        }
        // Champion's Seal
        if (!Core.CheckInventory("Champion's Seal", 20))
        {
            Core.UseBossClass(Core.CheckInventory("Void HighLord (IoDA)") ? "Void HighLord (IoDA)" : "Void Highlord");
            Adv.EnhanceEquipped(EnhancementType.Lucky, cSpecial: CapeSpecial.Penitence, hSpecial: HelmSpecial.Vim, wSpecial: WeaponSpecial.Valiance);
            Core.HuntMonster("infernalarena", "Na'al", "Champion's Seal", 20, false, false);
        }
        // Infernal Down
        ICFM.InfernalDown(50);

        if (!Core.CheckInventory("The Divine Will"))
        {
            Core.Logger("Farming Azalith for The Divine Will. Azalith is tough, consider using an army to speed it up.");
            Core.EquipClass(ClassType.Dodge);
            Core.HuntMonster("celestialpast", "Azalith", "The Divine Will", 1, false);
        }
        Adv.GearStore(true, true);

        Core.AddDrop(Core.QuestRewards(10096));
        Core.ChainComplete(10096);
    }
}
