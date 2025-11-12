/*
name: Get All Ranks
description: This script will get all reputations to rank 10.
tags: all reps, reputation, rank, all ranks, farm, rep, reps
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs    
//cs_include Scripts/Story/ThroneofDarkness/CoreToD.cs
//cs_include Scripts/Story/Glacera.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/BrightOak.cs

using Skua.Core.Interfaces;
using Skua.Core.Options;

public class GetAllRanks
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreToD TOD { get => _TOD ??= new CoreToD(); set => _TOD = value; }
    private static CoreToD _TOD;
    private static Core13LoC LOC { get => _LOC ??= new Core13LoC(); set => _LOC = value; }
    private static Core13LoC _LOC;
    private static GlaceraStory Glac { get => _Glac ??= new GlaceraStory(); set => _Glac = value; }
    private static GlaceraStory _Glac;
    private static BrightOak BrightOak { get => _BrightOak ??= new BrightOak(); set => _BrightOak = value; }
    private static BrightOak _BrightOak;


    public bool DontPreconfigure = true;
    public string OptionsStorage = "GetAllRanks";
    public List<IOption> Options = new()
    {
        new Option<bool>("doDeathPit", "Do Death Pit", "Should the bot farm Death Pit Brawl and Death Pit Arena reputations?", true),
        CoreBots.Instance.SkipOptions // Skip options when set
    };


    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        DoGetAllRanks();

        Core.SetOptions(false);
    }


    public void DoGetAllRanks()
    {
        //Required Stories add when needed.
        Core.Logger("Doing Required Stories for the reps, let tato know if another is required.");
        bool doDeathPit = Bot.Config!.Get<bool>("doDeathPit");
        //if !doDeathPit > skip story for the DP rep
        TOD.CompleteToD(!doDeathPit);
        LOC.Complete13LOC();
        BrightOak.doall(true);
        Core.Logger("when doing the `Glacera` storyline, you may have to restart it in the middle of the quests due to ae's bullshit how it keeps another quest locked even though the preivosu is completed till you relog");
        Glac.DoAll();
        Core.Logger($"Doing all Reputations to Rank 10, Death Pit: {doDeathPit}");
        if (doDeathPit)
            TOD.DeathPitPVP();

        Farm.GetAllRanks(doDeathPit);

    }
}
