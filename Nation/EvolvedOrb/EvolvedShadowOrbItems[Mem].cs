/*
name: Evolved Shadow Orb Items (Member)
description: Requires active membership + Evolved Shadow Orb.Will do all quests to get all items from the orb
tags: evolved, shadow, orb, member, nulgath, helm, spear, reborn, dark, side, void, emotions, shape, nothingness
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Other/Classes/REP-based/Bard.cs
//cs_include Scripts/Nation/EvolvedOrb/EvolvedShadowOrb[Mem].cs
//cs_include Scripts/Other/MergeShops/BattleConGearMerge.cs
//cs_include Scripts/Other/Various/Potions.cs
using Skua.Core.Interfaces;

public class EvolvedShadowOrbItems
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;
    private static EvolvedShadowOrb ESO
    {
        get => _ESO ??= new EvolvedShadowOrb();
        set => _ESO = value;
    }
    private static EvolvedShadowOrb _ESO;
    private static Bard Bard
    {
        get => _Bard ??= new Bard();
        set => _Bard = value;
    }
    private static Bard _Bard;
    private static BattleConGearMerge BCon
    {
        get => _BCon ??= new BattleConGearMerge();
        set => _BCon = value;
    }
    private static BattleConGearMerge _BCon;
    private static PotionBuyer Potion
    {
        get => _Potion ??= new PotionBuyer();
        set => _Potion = value;
    }
    private static PotionBuyer _Potion;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        GetItems();

        Core.SetOptions(false);
    }

    public void GetItems()
    {
        if (!Core.IsMember)
        {
            Core.Logger("This bot requiers membership.");
            return;
        }
        if (Core.CheckInventory(Rewards, toInv: false))
        {
            Core.Logger("You already own all Evolved Shadow Orb quest items.");
            return;
        }

        ESO.GetEvolvedShadowOrb();

        Core.Unbank("Evolved Shadow Orb");
        RebornDarkSide();
        VoidEmotions();
        ShapeNothingness();
    }

    public void RebornDarkSide()
    {
        if (
            !Core.CheckInventory(
                33198 /*Evolved Shadow Orb */
                ,
                33360 /* Platinum Coin of Nulgath: 2500 */
            )
        )
        {
            Core.Logger(
                "Missing Required Quest Accept Items. CANNOT PROCEED, STOPING THE BOT",
                stopBot: true
            );
        }

        foreach (int Q in new[] { 4771, 4772, 4773 })
        {
            switch (Q)
            {
                // Reborn in the Dark Side (Rare) 4772
                case 4773:
                // Reborn in the Dark Side (Shadow) 4773
                case 4772:
                    if (
                        !Core.CheckInventory(
                            Q == 4772
                                ? 4813 /* Shadow of Nulgath (Rare) */
                                : 5430 /* Shadow of Nulgath */
                        ) || !Core.IsMember
                    )
                        continue;

                    // Lightguardian Spirit Blade
                    Core.HuntMonster(
                        "lightguard",
                        "Mysterious Spirit",
                        "Lightguardian Spirit Blade",
                        isTemp: false
                    );
                    // Mana Mallet
                    Adv.BuyItem("citadel", 44, 843);
                    // Scrolls
                    if (!Core.CheckInventory("Scroll of Dark Energy", 30))
                    {
                        Core.JoinSWF(
                            "mobius",
                            "ChiralValley/town-Mobius-21Feb14.swf",
                            "Slugfit",
                            "Bottom"
                        );
                        Core.HuntMonster("mobius", "Slugfit", "Mystic Quills", 3, false);
                        Core.BuyItem("dragonrune", 549, "Ember Ink", 3);
                        while (
                            !Bot.ShouldExit
                            && Core.CheckInventory("Ember Ink")
                            && !Core.CheckInventory("Scroll of Dark Energy", 30)
                        )
                            Core.ChainComplete(2298);
                        Bot.Wait.ForPickup("Scroll of Dark Energy");
                    }

                    if (!Core.CheckInventory("Scroll of Dark Grip", 30))
                    {
                        Core.JoinSWF(
                            "mobius",
                            "ChiralValley/town-Mobius-21Feb14.swf",
                            "Slugfit",
                            "Bottom"
                        );
                        Core.HuntMonster("mobius", "Slugfit", "Mystic Quills", 3, false);
                        Core.BuyItem("dragonrune", 549, "Runik Ink", 3);
                        while (!Bot.ShouldExit && Core.CheckInventory("Runik Ink"))
                            Core.ChainComplete(2349);
                    }
                    Nation.EssenceofNulgath(80);
                    Nation.FarmTotemofNulgath(10);
                    Core.EnsureComplete(Q);
                    Bot.Wait.ForPickup(33182);
                    break;

                // Reborn in the Dark Side 4771 (f2p - bard)
                case 4771:
                    Core.AddDrop("Evolved Shadow of Nulgath");
                    Bard.GetBard(true);
                    Nation.FarmUni13(3);
                    Adv.BuyItem("tercessuinotlim", 1951, "Unidentified 25");
                    Nation.FarmVoucher(true);

                    if (!Core.CheckInventory("Behemoth Blade of Shadow"))
                    {
                        Core.EquipClass(ClassType.Solo);
                        if (!Core.CheckInventory("Basic War Sword"))
                        {
                            Farm.BludrutBrawlBoss(quant: 100);
                            Core.BuyItem("battleon", 222, "Basic War Sword");
                        }
                        if (!Core.CheckInventory("Steel Afterlife"))
                        {
                            Farm.BludrutBrawlBoss(quant: 100);
                            Core.BuyItem("battleon", 222, "Steel Afterlife");
                        }
                        if (!Core.CheckInventory("Behemoth Blade of Shadow"))
                        {
                            Farm.BludrutBrawlBoss(quant: 500);
                            Core.BuyItem("battleon", 222, "Behemoth Blade of Shadow");
                        }
                    }

                    Core.EquipClass(ClassType.Farm);
                    Nation.ApprovalAndFavor(1, 0);
                    Nation.FarmFiendToken(30);
                    BCon.BuyAllMerge("Azure Starblade");
                    Bot.Wait.ForPickup("Evolved Shadow of Nulgath");
                    break;
            }
        }
    }

    public void VoidEmotions()
    {
        //Void Emotion 4774
        if (!Core.CheckInventory("Platinum Coin of Nulgath: 300"))
        {
            Core.Logger("Platinum Coin of Nulgath 300 not found");
            return;
        }

        Core.AddDrop("Evolved Shadow Helm");
        Core.EnsureAccept(4774);
        Core.EquipClass(ClassType.Farm);
        Nation.FarmDarkCrystalShard(10);
        Nation.FarmDiamondofNulgath(50);
        Nation.FarmVoucher(false);
        Nation.FarmBloodGem(5);
        Core.EquipClass(ClassType.Solo);
        Core.KillMonster("chaoslord", "r2", "Left", "*", "There is no Myself", isTemp: false);
        Core.EnsureComplete(4774);
        Bot.Wait.ForPickup("Evolved Shadow Helm");
    }

    public void ShapeNothingness()
    {
        if (!Core.CheckInventory("Platinum Coin of Nulgath: 2500"))
        {
            Core.Logger("Platinum Coin of Nulgath 2500 not found");
            return;
        }

        // Shape your Nothingness 4775
        Core.AddDrop(
            "Unidentified 29",
            "Random Weapon of Nulgath",
            "Evolved Shadow Spear of Nulgath"
        );
        Core.EnsureAccept(4775);
        Core.EquipClass(ClassType.Farm);
        Nation.FarmUni10(30);
        Nation.FarmTaintedGem(30);
        Nation.FarmDarkCrystalShard(30);
        Nation.Supplies("Unidentified 29");
        Nation.Supplies("Random Weapon of Nulgath");
        Nation.FarmVoucher(false);
        Nation.FarmTotemofNulgath(10);
        Adv.BuyItem("alchemyacademy", 2036, 12056, 15, shopItemID: 9827);
        Core.EnsureComplete(4775);
        Bot.Wait.ForPickup("Evolved Shadow Spear of Nulgath");
    }

    private readonly string[] Rewards =
    {
        "Evolved Shadow of Nulgath",
        "Evolved Shadow Helm",
        "Evolved Shadow Spear of Nulgath",
    };
}
