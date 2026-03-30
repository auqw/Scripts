/*
name: Good REP 2 (Loyalty Rewarded)
description: Faster Good reputation farm using quest 1952 (Loyalty Rewarded, Wounds Salved) by killing Burning Loyalist in PoisonForest. Handles Manor and PoisonForest prerequisites automatically if not done yet.
tags: good, rep, rank, reputation, loyalty rewarded, wounds salved, burning loyalist, poisonforest
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Manor.cs
//cs_include Scripts/Story/PoisonForest.cs

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class GoodRep2
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
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
    private static PoisonForest PForest
    {
        get => _PForest ??= new PoisonForest();
        set => _PForest = value;
    }
    private static PoisonForest _PForest;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        DoGoodRep2();

        Core.SetOptions(false);
    }

    public void DoGoodRep2(int rank = 10)
    {
        if (Farm.FactionRank("Good") >= rank)
            return;

        // Run story prerequisites if not yet done.
        // This covers Manor (quests 1058–1062) and PoisonForest (1948–1955),
        // all of which give Good rep. Skips automatically if already completed.
        if (!Core.isCompletedBefore(1955))
        {
            Core.Logger("Running prerequisites: Manor → PoisonForest story (gives Good rep along the way)...");
            PForest.StoryLine();
        }

        if (Farm.FactionRank("Good") >= rank)
            return;

        Core.ChangeAlignment(Alignment.Good);
        Core.EquipClass(ClassType.Farm);

        // Pin to a private room so the bot never switches rooms mid-farm
        Core.PrivateRooms = true;
        Core.SavedState(true, "PoisonForest");
        Farm.ToggleBoost(BoostType.Reputation);
        Core.Logger($"Farming Good rank {rank} with Loyalty Rewarded (quest 1952) in PoisonForest");

        // RegisterQuests handles accept/complete in the background automatically
        Core.RegisterQuests(1952);
        while (!Bot.ShouldExit && Farm.FactionRank("Good") < rank)
        {
            if (Core.CheckSaveState())
                Core.ExecuteSaveState();

            // HuntMonster navigates to Burning Loyalist's cell once, then stays there
            // since the mob respawns in place — no room-hopping
            Core.HuntMonster("PoisonForest", "Burning Loyalist");
        }
        Core.CancelRegisteredQuests();

        Farm.ToggleBoost(BoostType.Reputation, false);
        Core.SavedState(false);
        Core.PrivateRooms = false;
    }
}
