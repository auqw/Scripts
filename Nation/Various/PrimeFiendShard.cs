/*
name: Prime Fiend Shard Quests
description: This script will complete the storyline from the prime fiend shard
tags: prime fiend shard, nation, ravenous, fiend shard, nulgath, voidchasm, void chasm
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/VHL/CoreVHL.cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
//cs_include Scripts/Nation/MergeShops/VoidRefugeMerge.cs
//cs_include Scripts/Nation/AssistingCragAndBamboozle[Mem].cs
//cs_include Scripts/Story/Nation/Originul.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleDelve.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleSiege.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleDelveMerge.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Good/BLOD/CoreBLOD.cs
//cs_include Scripts/Nation/Various/TheLeeryContract[Member].cs
//cs_include Scripts/Nation/Various/JuggernautItems.cs
//cs_include Scripts/Nation/Various/VoidPaladin.cs
//cs_include Scripts/Evil/NSoD/CoreNSOD.cs
//cs_include Scripts/Evil/SDKA/CoreSDKA.cs
//cs_include Scripts/Other/Classes/Necromancer.cs
//cs_include Scripts/Nation/MergeShops/NulgathDiamondMerge.cs
//cs_include Scripts/Nation/Various/DragonBlade[mem].cs
//cs_include Scripts/Nation/Various/TarosManslayer.cs
//cs_include Scripts/Nation/Various/PurifiedClaymoreOfDestiny.cs
//cs_include Scripts/Nation/Various/VoidSpartan.cs
//cs_include Scripts/Story/Nation/Fiendshard.cs
//cs_include Scripts/Nation/Various/SwirlingTheAbyss.cs
//cs_include Scripts/Hollowborn/TradingandStuff(single).cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
//cs_include Scripts/Nation/EmpoweringItems.cs
//cs_include Scripts/Other/Weapons/VoidAvengerScythe.cs
//cs_include Scripts/Nation/AFDL/NulgathDemandsWork.cs
//cs_include Scripts/Nation/AFDL/WillpowerExtraction.cs
//cs_include Scripts/Nation/Various/GoldenHanzoVoid.cs
//cs_include Scripts/Nation/MergeShops/DilligasMerge.cs
//cs_include Scripts/Nation/MergeShops/DirtlickersMerge.cs
//cs_include Scripts/Other/Weapons/WrathofNulgath.cs
//cs_include Scripts/Nation/Various/ArchfiendDeathLord.cs
//cs_include Scripts/Nation/MergeShops/VoidChasmMerge.cs
//cs_include Scripts/Story/Nation/VoidChasm.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Nation/MergeShops/NationMerge.cs
//cs_include Scripts/Nation/NationLoyaltyRewarded.cs
//cs_include Scripts/Story/Legion/DarkWarLegionandNation.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Quests;
using Skua.Core.Models.Items;

public class PrimeFiendShard
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static Originul_Story Originul { get => _Originul ??= new Originul_Story(); set => _Originul = value; }
    private static Originul_Story _Originul;
    private static CoreVHL VHL { get => _VHL ??= new CoreVHL(); set => _VHL = value; }
    private static CoreVHL _VHL;
    private static CoreNation Nation { get => _Nation ??= new CoreNation(); set => _Nation = value; }
    private static CoreNation _Nation;
    private static VoidRefugeMerge VoidRefugeMerge { get => _VoidRefugeMerge ??= new VoidRefugeMerge(); set => _VoidRefugeMerge = value; }
    private static VoidRefugeMerge _VoidRefugeMerge;
    private static TempleDelveMerge TempleDelveMerge { get => _TempleDelveMerge ??= new TempleDelveMerge(); set => _TempleDelveMerge = value; }
    private static TempleDelveMerge _TempleDelveMerge;
    private static DirtlickersMerge DirtlickersMerge { get => _DirtlickersMerge ??= new DirtlickersMerge(); set => _DirtlickersMerge = value; }
    private static DirtlickersMerge _DirtlickersMerge;
    private static VoidPaladin VoidPaladin { get => _VoidPaladin ??= new VoidPaladin(); set => _VoidPaladin = value; }
    private static VoidPaladin _VoidPaladin;
    private static NulgathDiamondMerge NulgathDiamondMerge { get => _NulgathDiamondMerge ??= new NulgathDiamondMerge(); set => _NulgathDiamondMerge = value; }
    private static NulgathDiamondMerge _NulgathDiamondMerge;
    private static VoidSpartan VoidSpartan { get => _VoidSpartan ??= new VoidSpartan(); set => _VoidSpartan = value; }
    private static VoidSpartan _VoidSpartan;
    private static SwirlingTheAbyss SwirlingTheAbyss { get => _SwirlingTheAbyss ??= new SwirlingTheAbyss(); set => _SwirlingTheAbyss = value; }
    private static SwirlingTheAbyss _SwirlingTheAbyss;
    private static TradingandStuffSingle TradingandStuffSingle { get => _TradingandStuffSingle ??= new TradingandStuffSingle(); set => _TradingandStuffSingle = value; }
    private static TradingandStuffSingle _TradingandStuffSingle;
    private static VoidAvengerScythe VoidAvengerScythe { get => _VoidAvengerScythe ??= new VoidAvengerScythe(); set => _VoidAvengerScythe = value; }
    private static VoidAvengerScythe _VoidAvengerScythe;
    private static NulgathDemandsWork NulgathDemandsWork { get => _NulgathDemandsWork ??= new NulgathDemandsWork(); set => _NulgathDemandsWork = value; }
    private static NulgathDemandsWork _NulgathDemandsWork;
    private static ArchfiendDeathLord ArchfienddDeathLord { get => _ArchfienddDeathLord ??= new ArchfiendDeathLord(); set => _ArchfienddDeathLord = value; }
    private static ArchfiendDeathLord _ArchfienddDeathLord;
    private static WrathofNulgath WrathofNulgath { get => _WrathofNulgath ??= new WrathofNulgath(); set => _WrathofNulgath = value; }
    private static WrathofNulgath _WrathofNulgath;
    private static DilligasMerge DilligasMerge { get => _DilligasMerge ??= new DilligasMerge(); set => _DilligasMerge = value; }
    private static DilligasMerge _DilligasMerge;
    private static VoidChasmMerge VoidChasmMerge { get => _VoidChasmMerge ??= new VoidChasmMerge(); set => _VoidChasmMerge = value; }
    private static VoidChasmMerge _VoidChasmMerge;
    private static NationMerge NationMerge { get => _NationMerge ??= new NationMerge(); set => _NationMerge = value; }
    private static NationMerge _NationMerge;

    public void ScriptMain(IScriptInterface Bot)
    {
        for (int questId = 9555; questId <= 9559; questId++)
        {
            Quest? quest = Core.InitializeWithRetries(() => Core.EnsureLoad(questId));

            if (quest != null)
            {
                foreach (ItemBase item in quest.Requirements)
                {
                    Core.BankingBlackList.Add(item.Name);
                }
            }
        }

        Core.SetOptions();

        Storyline();

        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(9559))
            return;

        Core.Logger("If you're going to use insingias for the roents, please do so before starting the script, as it will stop otherwise if dont have them.");

        // Required Class
        VHL.GetVHL();

        // Check for 10 roents
        VHL.VHLChallenge(10);

        // Prime Fiend Shard [required to accept]
        VoidChasmMerge.BuyAllMerge("Prime Fiend Shard");

        // Ensure requirements are unbanked
        Core.Unbank("Prime Fiend Shard");

        // Feed the Fiend Shard 9555
        if (!Story.QuestProgression(9555))
        {
            Core.EnsureAccept(9555);
            if (!Core.CheckInventory("Dual Dragonbone Axe of Nulgath"))
            {
                Core.AddDrop("Dual Dragonbone Axe of Nulgath");
                Core.Logger("Farming Dual Dragonbone Axe of Nulgath.");
                // Combat Style: Dragonbone Axe 629 - requires 1x uni 13
                Core.EnsureAccept(629);
                Nation.FarmUni13(1);
                Nation.FarmTaintedGem(13);
                Nation.FarmDarkCrystalShard(13);
                Nation.FarmDiamondofNulgath(13);
                Nation.Supplies(Nation.Uni(21));
                Core.HuntMonster("evilmarsh", "Tainted Elemental", "Tainted Rune of Evil", log: false);
                Core.EnsureComplete(629);
            }
            // 9559 requires x13 uni 13
            Nation.FarmUni13(13);
            Nation.EssenceofNulgath(20);
            Nation.FarmDiamondofNulgath(20);
            Adv.BuyItem("shadowblast", 1206, "Nation Soulstealer", shopItemID: 4175);
            TempleDelveMerge.BuyAllMerge("Void Nation Caster");
            DirtlickersMerge.BuyAllMerge("Iron Dreadsaw");

            // Ensure requirements are unbanked
            Quest? quest = Core.InitializeWithRetries(() => Core.EnsureLoad(9555));
            foreach (ItemBase Item in quest!.Requirements)
                while (!Bot.ShouldExit && !Core.CheckInventory(Item.ID))
                {
                    Core.Unbank(Item.ID);
                    Core.Sleep();
                }
            Core.EnsureComplete(9555);
        }

        // Grow the Fiend Shard 9556
        if (!Story.QuestProgression(9556))
        {
            Core.EnsureAccept(9556);
            Nation.EssenceofNulgath(40);
            NulgathDiamondMerge.BuyAllMerge("Abyssal Priest of Nulgath");
            NulgathDiamondMerge.BuyAllMerge("Storm Knight");
            VoidPaladin.DeeperandDeeperintoDarkness();
            VoidSpartan.GetSpartan("Void Spartan");

            // Ensure requirements are unbanked
            Quest? quest = Core.InitializeWithRetries(() => Core.EnsureLoad(9556));
            foreach (ItemBase Item in quest!.Requirements)
                while (!Bot.ShouldExit && !Core.CheckInventory(Item.ID))
                {
                    Core.Unbank(Item.ID);
                    Core.Sleep();
                }
            Core.EnsureComplete(9556);
        }

        // Evolve the Fiend Shard 9557
        if (!Story.QuestProgression(9557))
        {
            Core.EnsureAccept(9557);
            SwirlingTheAbyss.STA("Void Soul of Nulgath");
            TradingandStuffSingle.ArchFiendEnchantedOrbs();
            NulgathDiamondMerge.BuyAllMerge("Blood Ranger");
            VoidRefugeMerge.BuyAllMerge("Envenomed Edge of Nulgath");
            Nation.EssenceofNulgath(60);

            // Ensure requirements are unbanked
            Quest? quest = Core.InitializeWithRetries(() => Core.EnsureLoad(9557));
            foreach (ItemBase Item in quest!.Requirements)
                while (!Bot.ShouldExit && !Core.CheckInventory(Item.ID))
                {
                    Core.Unbank(Item.ID);
                    Core.Sleep();
                }
            Core.EnsureComplete(9557);
        }

        // The Fiend Shard's Demand 9558
        if (!Story.QuestProgression(9558))
        {
            Core.EnsureAccept(9558);
            VoidAvengerScythe.SkewsQuests();
            NulgathDemandsWork.NDWQuest(new[] { "Doomblade of Destruction" });
            ArchfienddDeathLord.GetArm(true, ArchfiendDeathLord.RewardChoice.Archfiend_DeathLord);
            WrathofNulgath.GetSword();
            DilligasMerge.BuyAllMerge("Ancient Shogun Armor");
            //ooga booga it wont complete

            // Ensure requirements are unbanked
            Quest? quest = Core.InitializeWithRetries(() => Core.EnsureLoad(9558));
            foreach (ItemBase Item in quest!.Requirements)
                while (!Bot.ShouldExit && !Core.CheckInventory(Item.ID))
                {
                    Core.Unbank(Item.ID);
                    Core.Sleep();
                }
            Core.EnsureComplete(9558);
        }

        // The Fiend Shard's Finale 9559
        if (!Story.QuestProgression(9559))
        {
            Core.EnsureAccept(9559);

            Nation.FarmUni13(13);
            Nation.FarmTaintedGem(750);
            Nation.FarmDarkCrystalShard(400);
            Nation.FarmDiamondofNulgath(1000);
            Nation.FarmTotemofNulgath(60);
            Nation.FarmGemofNulgath(300);
            Nation.FarmBloodGem(100);

            if (Core.CheckInventory("Roentgenium of Nulgath", 10))
            { // Ensure requirements are unbanked
                Quest? quest = Core.InitializeWithRetries(() => Core.EnsureLoad(9559));
                Core.Unbank(Core.EnsureLoad(8916).Requirements.Select(x => x.ID).ToArray());
                Core.EnsureComplete(9559);
            }
            else Core.Logger($"Not enough Roents {Bot.Inventory.GetQuantity("Roentgenium of Nulgath")} / 10");
        }

        var filteredReceipts = Nation.Receipt.Where(item => !item.StartsWith("Unidentified")).ToArray();
        Core.ToBank(filteredReceipts);
        Core.ToBank(Nation.tercessBags);
        Core.ToBank(Nation.bagDrops);
        Core.ToBank(Nation.SuppliesRewards);
        Core.ToBank(Nation.SwindlesReturnRewards);
    }
}
