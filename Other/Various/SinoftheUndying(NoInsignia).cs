/*
name: Sin of the SinoftheUndying NoInsignia
description: Does the Sin of the SinoftheUndying quest upto the "Atlas Regalia" item, of which skua cannot get.
tags: sin, of, the, SinoftheUndying, no, insignia, quest, atlas, regalia, sin of the SinoftheUndying, no insignia, 51, fifty one
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

public class SinoftheSinoftheUndyingNoInsignia
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
                "Sin of the Undying",
                "Atlas Lion Pelt",
                "Beast Soul",
                "Broken Chain",
                "Essence of Wrath",
                "Essence of Violence",
                "Essence of Treachery",
                "Atlas Regalia",
            }
        );
        GetSword();

        Core.SetOptions(false);
    }

    public void GetSword()
    {
        Core.AddDrop(
            "Sin of the Undying",
            "Atlas Lion Pelt",
            "Beast Soul",
            "Broken Chain",
            "Essence of Wrath",
            "Essence of Violence",
            "Essence of Treachery",
            "Atlas Regalia"
        );
        AtlasFalls.Storyline();

        Core.EnsureAccept(10146);

        // Atlas Lion Pelt x 20
        if (!Core.CheckInventory("Atlas Lion Pelt", 20))
        {
            Core.FarmingLogger("Atlas Lion Pelt", 20);
            while (!Bot.ShouldExit && !Core.CheckInventory("Atlas Lion Pelt", 20))
            {
                Core.HuntMonsterQuest(
                    10126,
                    ("atlaskingdom", "Atlas Leo", ClassType.Solo),
                    ("atlaskingdom", "Atlas Elite", ClassType.Solo),
                    ("atlaskingdom", "Executioner Ladon", ClassType.Solo)
                );
                Bot.Wait.ForPickup("Atlas Lion Pelt");
            }
        }

        // 20 Beast's Soul
        if (!Core.CheckInventory("Beast Soul", 20))
        {
            Core.EquipClass(ClassType.Solo);
            Core.FarmingLogger("Beast Soul", 20);
            Core.HuntMonster(
                "sevencircleswar",
                "The Beast",
                "Beast Soul",
                20,
                isTemp: false,
                publicRoom: true
            );
        }

        // Broke Chain x 20
        if (!Core.CheckInventory("Broken Chain", 150))
        {
            Core.FarmingLogger("Broken Chain", 150);
            while (!Bot.ShouldExit && !Core.CheckInventory("Broken Chain", 150))
            {
                Core.HuntMonsterQuest(
                    10115,
                    ("atlaspromenade", "Atlas Light Magus", ClassType.Farm),
                    ("atlaspromenade", "Wrath Guard", ClassType.Farm),
                    ("atlaspromenade", "Usurper Lord Slaine", ClassType.Solo)
                );
                Bot.Wait.ForPickup("Broken Chain");
            }
        }

        //5k LTs
        LR.FarmLegionToken(5000);

        // 20 of each Essence
        var requiredItems = new Dictionary<string, Action<int>>
        {
            { "Essence of Wrath", HotLB.EssenceWrath },
            { "Essence of Violence", HotLB.EssenceViolence },
            { "Essence of Treachery", HotLB.EssenceTreachery },
        };

        foreach (var (item, farmAction) in requiredItems)
        {
            if (!Core.CheckInventory(item, 20))
            {
                farmAction(20);
            }
        }

        if (!Core.CheckInventory("Atlas Regalia", 20))
        {
            if (Core.CheckInventory("Chaos Avenger"))
            {
                Core.UseBossClass();
                while (!Bot.ShouldExit && !Core.CheckInventory("Atlas Regalia", 20))
                {
                    Core.HuntMonsterQuest(10137, "atlasfalls", "King Zedek");
                    Bot.Wait.ForPickup("Atlas Regalia");
                }
            }
            else
                Core.Logger("Atlas Regalia requires Chaos Avenger class.");
        }
        else
        {
            Core.EnsureComplete(10146);
            Bot.Wait.ForDrop("Sin of the Undying");
            Bot.Wait.ForPickup("Sin of the Undying");
            if (Core.CheckInventory("Sin of the Undying"))
            {
                Core.Logger("You have successfully obtained the Sin of the Undying.");
            }
        }
    }
}
