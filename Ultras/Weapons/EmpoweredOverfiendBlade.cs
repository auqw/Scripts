/*
name: EmpoweredOverfiendBlade
description: if you have 25 nulgath insigs, itll pick the proper sword to get
tags: EmpoweredOverfiendBlade, Empowered Over fiend Blade, Empowered, Overfiend, Blade
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Nation/CoreNation.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;

public class EmpoweredOblivion
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
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange(new[] { "Oblivion Blade of Nulgath", "Oblivion Blade of Nulgath", "Oblivion Blade of Nulgath Pet (Rare)" });
        Core.SetOptions(disableClassSwap: true);

        GetSword();

        Core.SetOptions(false);
    }

    void GetSword()
    {
        if (!Core.CheckInventory("Nulgath Insignia", 25))
        {
            Core.Logger("You don't have 25 Nulgath Insignias. Cannot complete any of the quests.");
            return;
        }

        Farm.Experience(95);
        Core.AddDrop(98430, 98431); // Empowered Oblivion Blade swords

        // Determine which version to farm based on owned items
        bool hasSword = Core.CheckInventory(13376); // Oblivion Blade of Nulgath (Sword)
        bool hasNonRarePet = Core.CheckInventory(5373); // Oblivion Blade of Nulgath (Pet)
        bool hasRarePet = Core.CheckInventory(4809); // Oblivion Blade of Nulgath Pet (Rare)


        if (hasRarePet && hasSword)
        {
            Core.Logger("You own the Rare Pet and Sword - farming Empowered Charged Oblivion Blade (Rare)");
            EmpoweredChargedOblivionBladeRare();
        }
        else if (hasNonRarePet && hasSword)
        {
            Core.Logger("You own the Non-Rare Pet and Sword - farming Empowered Charged Oblivion Blade");
            EmpoweredChargedOblivionBlade();
        }
        else if (hasSword)
        {
            Core.Logger("You only own the Sword - farming Empowered Oblivion Blade");
            EmpoweredOblivionBlade();
        }
        else
        {
            Core.Logger("You don't have the required Oblivion Blade of Nulgath (Sword). Cannot proceed.");
            return;
        }

        void EmpoweredOblivionBlade()
        {
            #region Empowered Oblivion Blade (8693)
            // Must be Level 95.
            // Must have Oblivion Blade of Nulgath (Sword) in your inventory. (13376)
            // Tainted Gem x200
            Nation.FarmTaintedGem(200);
            // Dark Crystal Shard 100
            Nation.FarmDarkCrystalShard(100);
            // Diamond of Nulgath x400
            Nation.FarmDiamondofNulgath(400);
            // Voucher of Nulgath (non-mem)
            Nation.FarmVoucher(false);
            // Totem of Nulgath x30
            Nation.FarmTotemofNulgath(30);
            // Gem of Nulgath x80
            Nation.FarmGemofNulgath(80);
            // Nulgath Insignia x25
            // ---

            Core.EnsureComplete(8693);
            Bot.Wait.ForPickup(98423);
            #endregion
        }

        void EmpoweredChargedOblivionBlade()
        {
            #region Empowered Charged Oblivion Blade (10547)
            // Must be Level 95.
            // Must have the following items in your inventory:
            //     Oblivion Blade of Nulgath (Pet) // 5373
            //     Oblivion Blade of Nulgath (Sword) // 13376
            // Dark Crystal Shard x150
            Nation.FarmDarkCrystalShard(150);
            // Diamond of Nulgath x500
            Nation.FarmDiamondofNulgath(500);
            // Totem of Nulgath x30
            Nation.FarmTotemofNulgath(30);
            // Voucher of Nulgath (non-mem) x1
            Nation.FarmVoucher(false);
            // Bane of Nulgath x1
            Nation.NulgathLarvae("Bane of Nulgath");
            // Nulgath Insignia x25
            // ---


            Core.EnsureComplete(10547);
            Bot.Wait.ForPickup(98430);
            #endregion
        }

        void EmpoweredChargedOblivionBladeRare()
        {
            #region Empowered Charged Oblivion Blade (Rare) (10548)
            // Must be Level 95.
            // Must have the following items in your inventory:
            //     Oblivion Blade of Nulgath (Sword)  // 13376
            //     Oblivion Blade of Nulgath Pet (Rare) // 4809
            // Dark Crystal Shard x150
            Nation.FarmDarkCrystalShard(150);
            // Diamond of Nulgath x500
            Nation.FarmDiamondofNulgath(500);
            // Totem of Nulgath x30
            Nation.FarmTotemofNulgath(30);
            // Voucher of Nulgath (non-mem) x1
            Nation.FarmVoucher(false);
            // Bane of Nulgath x1
            Nation.NulgathLarvae("Bane of Nulgath");
            // Nulgath Insignia x25
            // ---


            Core.EnsureComplete(10548);
            Bot.Wait.ForPickup(98430);
            #endregion
        }
    }

}



