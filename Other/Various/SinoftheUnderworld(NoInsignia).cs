/*
name: Sin of the SinoftheUnderworld NoInsignia
description: Does the Sin of the SinoftheUnderworld quest upto the "Dage the Evil Insignia" item, of which skua cannot get.
tags: sin, of, the, SinoftheUnderworld, no, insignia, quest, Dage the Evil Insignia, sin of the SinoftheUnderworld, no insignia, 51, fifty one
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Legion/Revenant/CoreLR.cs
//cs_include Scripts/Legion/HeadOfTheLegionBeast.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs
//cs_include Scripts/Story/Legion/AtlasKingdom.cs
//cs_include Scripts/Story/Legion/AtlasPromenade.cs
//cs_include Scripts/Story/Legion/AtlasFalls.cs
//cs_include Scripts/Legion/InfiniteLegionDarkCaster.cs
//cs_include Scripts/Story/Legion/SeraphicWar.cs
//cs_include Scripts/Legion/YamiNoRonin/CoreYnR.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Legion/SwordMaster.cs
//cs_include Scripts/Legion/Various/VulcarsMerge.cs
//cs_include Scripts/Legion/LegionMaterials/SoulSand.cs
//cs_include Scripts/Legion/Various/LegionBonfire.cs
//cs_include Scripts/Legion/LegionMaterials/LetitBurn(SoulEssence).cs
//cs_include Scripts/Seasonal/StaffBirthdays/DageTheEvil/UnderworldTeamMerge.cs
//cs_include Scripts/Legion/MergeShops/SoulForgeMerge.cs
using Skua.Core.Interfaces;

public class SinoftheSinoftheUnderworldNoInsignia
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
    private static CoreLegion LR
    {
        get => _LR ??= new CoreLegion();
        set => _LR = value;
    }
    private static CoreLegion _LR;
    private static HeadoftheLegionBeast HotLB
    {
        get => _HotLB ??= new HeadoftheLegionBeast();
        set => _HotLB = value;
    }
    private static HeadoftheLegionBeast _HotLB;
    private static AtlasFalls AtlasFalls
    {
        get => _AtlasFalls ??= new AtlasFalls();
        set => _AtlasFalls = value;
    }
    private static AtlasFalls _AtlasFalls;
    private static CoreYnR YnR
    {
        get => _YnR ??= new CoreYnR();
        set => _YnR = value;
    }
    private static CoreYnR _YnR;
    private static VulcarsMerge VulcarsMerge
    {
        get => _VulcarsMerge ??= new VulcarsMerge();
        set => _VulcarsMerge = value;
    }
    private static VulcarsMerge _VulcarsMerge;
    private static UnderworldTeamMerge UnderworldTeamMerge
    {
        get => _UnderworldTeamMerge ??= new UnderworldTeamMerge();
        set => _UnderworldTeamMerge = value;
    }
    private static UnderworldTeamMerge _UnderworldTeamMerge;
    private static SoulForgeMerge SoulForgeMerge
    {
        get => _SoulForgeMerge ??= new SoulForgeMerge();
        set => _SoulForgeMerge = value;
    }
    private static SoulForgeMerge _SoulForgeMerge;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        Core.BankingBlackList.AddRange(
            new[]
            {
                "Undead Champion Blade",
                "Corrupted Dragon Slayer",
                "Wrath of the Legion Blade",
                "Fatal Keraunos",
                "Yami no Ronin Katana",
                "Dual Legion Soul Devourers",
                "Dage the Evil Insignia",
                "Sin Of the Underworld",
            }
        );
        GetSword();

        Core.SetOptions(false);
    }

    public void GetSword()
    {
        Core.AddDrop(
            "Undead Champion Blade",
            "Corrupted Dragon Slayer",
            "Wrath of the Legion Blade",
            "Fatal Keraunos",
            "Yami no Ronin Katana",
            "Dual Legion Soul Devourers",
            "Dage the Evil Insignia",
            "Sin Of the Underworld"
        );
        AtlasFalls.Storyline();

        Core.EnsureAccept(10147);

        //5k LTs
        LR.FarmLegionToken(25000);

        if (!Core.CheckInventory("Undead Champion Blade"))
        {
            Core.FarmingLogger("Undead Champion Blade");
            Core.EquipClass(ClassType.Farm);
            while (!Bot.ShouldExit && !Core.CheckInventory("Undead Champion Blade"))
            {
                Core.EnsureAccept(821);
                Core.HuntMonster(
                    "lair",
                    "Water Draconian",
                    "Flawless DracoHorn",
                    40,
                    isTemp: false
                );
                Core.HuntMonster("lair", "Golden Draconian", "Golden DracoHeart");
                Core.HuntMonster("lair", "Dark Draconian", "Dark DracoHeart");
                Core.HuntMonster("lair", "Bronze Draconian", "Mammoth DracoHeart");
                Core.HuntMonster("lair", "Water Draconian", "Water DracoHeart");
                Core.HuntMonster("lair", "Venom Draconian", "Venom DracoHeart");
                Core.HuntMonster("lair", "Purple Draconian", "Iron DracoHeart");
                Core.EnsureComplete(821);
            }
        }

        if (!Core.CheckInventory("Corrupted Dragon Slayer"))
        {
            Core.FarmingLogger("Corrupted Dragon Slayer");
            Core.RegisterQuests(824);
            while (!Bot.ShouldExit && !Core.CheckInventory(new[] { "Corrupted Dragon Slayer" }))
            {
                Core.EquipClass(ClassType.Farm);
                Core.KillMonster(
                    "doomhaven",
                    "r4",
                    "Down",
                    "Skeletal Ice Mage",
                    "Frostbit Skull",
                    15
                );
                Core.HuntMonster("Marsh2", "Lesser Shadow Serpent", "Potent Viper's Blood");
                Core.EquipClass(ClassType.Solo);
                Core.HuntMonster("Marsh2", "Soulseeker", "Soul Scythe", isTemp: false);
            }
        }

        if (!Core.CheckInventory("Wrath of the Legion Blade"))
        {
            UnderworldTeamMerge.BuyAllMerge("Wrath of the Legion Blade");
        }

        if (!Core.CheckInventory("Fatal Keraunos"))
        {
            SoulForgeMerge.BuyAllMerge("Fatal Keraunos");
        }

        if (!Core.CheckInventory("Yami no Ronin Katana"))
        {
            YnR.YokaiSwordScroll();
        }

        if (!Core.CheckInventory("Dual Legion Soul Devourers"))
        {
            VulcarsMerge.BuyAllMerge("Dual Legion Soul Devourers");
        }

        if (!Core.CheckInventory("Dage the Evil Insignia", 20))
        {
            Core.Logger("Bot **Cannot** get \"Dage the Evil Insignia\", please do it manually.");
        }
        else
        {
            Core.EnsureComplete(10147);
            Bot.Wait.ForDrop("Sin Of the Underworld");
            Bot.Wait.ForPickup("Sin Of the Underworld");
            if (Core.CheckInventory("Sin Of the Underworld"))
            {
                Core.Logger("You have successfully obtained the Sin Of the Underworld.");
            }
        }
    }
}
