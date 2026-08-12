/*
name: Do All Ultras Story Prerequisites
description: Completes all story prerequisites needed to unlock every Ultra boss. Covers: Ultra Ezrajal, Ultra Warden, Ultra Engineer (ExaltiaTower), Ultra Avatar Tyndarius + Ultra Speaker (Shadows of War), Ultra Dage (Isle of Fotia), Champion Drakath (13 Lords of Chaos), Ultra Drago + Ultra Darkon (Elegy of Madness / Astravia). Skipped: Ultra Nulgath and Ultra Gramiel (Level 80 only, no story required).
tags: story, quest, ultra, ezrajal, warden, engineer, tyndarius, speaker, dage, drakath, darkon, drago, all, complete
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ExaltiaTower.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Story/IsleOfFotia/CoreIsleOfFotia.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs

//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs
//cs_include Scripts/Story/Hollowborn/CoreHollowbornStory.cs
//cs_include Scripts/Story/Legion/DageChallengeStory.cs
using Skua.Core.Interfaces;

public class DoAllUltrasStory
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static ExaltiaTower Exaltia => _Exaltia ??= new ExaltiaTower();
    private static ExaltiaTower _Exaltia;
    private static CoreSoW SoW => _SoW ??= new CoreSoW();
    private static CoreSoW _SoW;
    private static CoreIsleOfFotia Fotia => _Fotia ??= new CoreIsleOfFotia();
    private static CoreIsleOfFotia _Fotia;
    private static Core13LoC LoC => _LoC ??= new Core13LoC();
    private static Core13LoC _LoC;
    private static CoreAstravia Astravia => _Astravia ??= new CoreAstravia();
    private static CoreAstravia _Astravia;
    private static CoreHollowbornStory Hollowborn => _Hollowborn ??= new CoreHollowbornStory();
    private static CoreHollowbornStory _Hollowborn;
    private static DageChallengeStory DageChallenge => _DageChallenge ??= new DageChallengeStory();
    private static DageChallengeStory _DageChallenge;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        Core.EquipClass(ClassType.Solo);

        DoAllUltrasPrereqs();

        Core.SetOptions(false);
    }

    public void DoAllUltrasPrereqs()
    {
        // Ultra Nulgath  → Level 80 only, no story required
        Core.Logger("Leveling to 80 for Ultra Nulgath");
        Farm.Experience(80);

        // Unlocks: Ultra Ezrajal, Ultra Warden, Ultra Engineer
        Core.Logger("Exaltia Tower story → Ultra Ezrajal / Warden / Engineer");
        Exaltia.StoryLine();

        // Unlocks: Ultra Avatar Tyndarius
        Core.Logger("Shadows of War story → Ultra Avatar Tyndarius");
        SoW.Tyndarius();

        // Unlocks: Champion Drakath
        Core.Logger("13 Lords of Chaos story → Champion Drakath");
        LoC.Complete13LOC(true);

        // Unlocks: Ultra Dage
        Core.Logger("Dage Challenge story → Ultra Dage");
        DageChallenge.DageChallengeQuests();

        // Unlocks: Ultra Drago (AstraviaJudgement / Mahapadma) + Ultra Darkon (TheWorld)
        Core.Logger("Elegy of Madness / Astravia story → Ultra Drago + Ultra Darkon");
        Astravia.CompleteCoreAstravia();

        // Unlocks: Ultra Gramiel
        Core.Logger("Hollowborn story → Ultra Gramiel");
        Hollowborn.DoAll();
        
        // Unlocks: Ultra Avatar Tyndarius (Tyndarius step) + Ultra Speaker (ManaCradle step)
        Core.Logger("Shadows of War story → Ultra Avatar Tyndarius + Ultra Speaker");
        SoW.CompleteCoreSoW();

    }
}
