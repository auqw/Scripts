/*
name: Blacksmithing REP
description: This script will farm Blacksmithing reputation to rank 10.
tags: blacksmith, rep, reputation, rank, farm
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;
using System.Collections.Generic;

public class BlacksmithingREP
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;

    private static CoreFarms _Farm;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }

    private static CoreAdvanced _Adv;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }

    public bool DontPreconfigure = true;
    public string OptionsStorage = "BlackSmithRepGold";
    public List<IOption> Options = new()
    {
        new Option<bool>(
            "UseGold",
            "Use Gold To Get Rep?",
            "Will Farm the Quest \"Intrepid Investing\" which costs 500k/ turnin. If you don't have the gold the bot will farm it.",
            false
        ),
        new Option<bool>(
            "BulkFarmGold",
            "Pre-Farm Gold(100m)",
            "Bulk Turnin after farming 100m Gold.",
            false
        ),
        new Option<HydraLevels>(
            "HydraLevel",
            "Hydra Level",
            "Select the level of Hydra to fight (Only applies if NOT using Gold).",
            HydraLevels.Level_25
        ),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        bool useGold = Bot.Config!.Get<bool>("UseGold");

        if (useGold)
        {
            Farm.BlacksmithingREP(
                10,
                Bot.Config!.Get<bool>("UseGold"),
                Bot.Config!.Get<bool>("BulkFarmGold")
            );
        }
        else
        {
            CustomBlacksmithingREP(Bot.Config!.Get<HydraLevels>("HydraLevel"));
        }

        Core.SetOptions(false);
    }

    private void CustomBlacksmithingREP(HydraLevels hydraLevelSelection)
    {
        if (Farm.FactionRank("Blacksmithing") >= 10)
        {
            Core.Logger("Blacksmithing is already Rank 10.");
            return;
        }

        Core.Logger($"Farming Blacksmithing REP using {hydraLevelSelection.ToString().Replace("_", " ")} Hydras.");

        Core.AddDrop("Creature Shard", "Monster Trophy", "Hydra Scale Piece");

        // Simplified: enum name (e.g. "Level_25") already contains the level number,
        // so build the monster name directly instead of a switch or dictionary lookup.
        string hydraMonster = $"Hydra Head {hydraLevelSelection.ToString().Split('_')[1]}";

        while (!Bot.ShouldExit && Farm.FactionRank("Blacksmithing") < 10)
        {
            Core.EnsureAccept(8736);

            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("maul", "Creature Creation", "Creature Shard", 1, false);

            Core.HuntMonster("towerofdoom", "Dread Klunk", "Monster Trophy", 15, false);

            if (hydraLevelSelection == HydraLevels.Level_85 || hydraLevelSelection == HydraLevels.Level_90)
                Core.EquipClass(ClassType.Solo);
            else
                Core.EquipClass(ClassType.Farm);

            // CHANGED: Using HuntMonster so Skua automatically detects the correct cell and pad
            Core.HuntMonster("hydrachallenge", hydraMonster, "Hydra Scale Piece", 75, false);

            Core.EnsureComplete(8736);
        }
    }

    public enum HydraLevels
    {
        Level_25,
        Level_35,
        Level_45,
        Level_55,
        Level_65,
        Level_75,
        Level_85,
        Level_90
    }
}