/*
name: Get All Badges
description: This will get all badges in the game.
tags: badge, complete, all
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/Asylum.cs
//cs_include Scripts/Story/Artixpointe.cs
//cs_include Scripts/Story/ArtixWedding.cs
//cs_include Scripts/Story/Borgars.cs
//cs_include Scripts/Story/CruxShip.cs
//cs_include Scripts/Story/EtherstormWastes.cs
//cs_include Scripts/Story/RavenlossSaga.cs
//cs_include Scripts/Story/ShadowVault.cs
//cs_include Scripts/Story/SkyGuardSaga.cs
//cs_include Scripts/Story/UnderGroundLab.cs
//cs_include Scripts/Story/DragonsOfYokai/CoreDOY.cs
//cs_include Scripts/Story/Doomwood/CoreDoomwood.cs
//cs_include Scripts/Story/QueenofMonsters/CoreQOM.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/CelestialArena.cs
//cs_include Scripts/Story/QueenofMonsters/Extra/GoldenArena.cs
//cs_include Scripts/Story/Cornelis[mem].cs
//cs_include Scripts/Other/Badges/6thBirthdaySavior.cs
//cs_include Scripts/Other/Badges/WorldCup.cs
//cs_include Scripts/Other/Badges/BattleBabysitter.cs
//cs_include Scripts/Other/Badges/BattleConVIP.cs
//cs_include Scripts/Other/Badges/CelestialChampion.cs
//cs_include Scripts/Other/Badges/ChaosPuppetMaster.cs
//cs_include Scripts/Other/Badges/Committed.cs
//cs_include Scripts/Other/Badges/ConZombieSlayer.cs
//cs_include Scripts/Other/Badges/CornelisReborn.cs
//cs_include Scripts/Other/Badges/CtrlAltDelMemberBadge.cs
//cs_include Scripts/Other/Badges/DerpMoosefishBadge.cs
//cs_include Scripts/Other/Badges/DesolichFreed.cs
//cs_include Scripts/Other/Badges/GoldenLaurel.cs
//cs_include Scripts/Other/Badges/GravelynsWarrior.cs
//cs_include Scripts/Other/Badges/HordeZombieSLAYER.cs
//cs_include Scripts/Other/Badges/LordOfTheWeddingRing.cs
//cs_include Scripts/Other/Badges/MoglinPunter.cs
//cs_include Scripts/Other/Badges/ZorbakPunter.cs
//cs_include Scripts/Other/Badges/MummySlayerAndCruxShadowsDefender.cs
//cs_include Scripts/Other/Badges/RavenlossWarAndChampion.cs
//cs_include Scripts/Other/Badges/ShadowVaultChampion.cs
//cs_include Scripts/Other/Badges/SkyPirateSlayerBadge.cs
//cs_include Scripts/Other/Badges/StoneCold.cs
//cs_include Scripts/Other/Badges/TableFlipper.cs
//cs_include Scripts/Other/Badges/YouMadBroBadge.cs
//cs_include Scripts/Other/Badges/VoidHighlordBadge.cs
//cs_include Scripts/Other/Badges/StoryARC.cs
//cs_include Scripts/Other/Badges/JusticeSquad.cs
//cs_include Scripts/Other/Badges/ThiefofChaos.cs
//cs_include Scripts/Other/Badges/Goal.cs
//cs_include Scripts/Other/Badges/UltraCarnax.cs
//cs_include Scripts/Other/Badges/YokaiAscension.cs
//cs_include Scripts/Story/MagicThief.cs
//cs_include Scripts/Story/Glacera.cs
//cs_include Scripts/Seasonal/Frostvale/Story/CoreFrostvale.cs
//cs_include Scripts/Seasonal/Frostvale/FrostvaleBadges.cs
using Skua.Core.Interfaces;

public class AllBadges
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;

    private static CornelisRebornbadge CRB
    {
        get => _CRB ??= new CornelisRebornbadge();
        set => _CRB = value;
    }
    private static CornelisRebornbadge _CRB;
    private static DerpMoosefishBadge DMF
    {
        get => _DMF ??= new DerpMoosefishBadge();
        set => _DMF = value;
    }
    private static DerpMoosefishBadge _DMF;
    private static SkyPirateBadge SPB
    {
        get => _SPB ??= new SkyPirateBadge();
        set => _SPB = value;
    }
    private static SkyPirateBadge _SPB;
    private static YouMadBroBadge YMBB
    {
        get => _YMBB ??= new YouMadBroBadge();
        set => _YMBB = value;
    }
    private static YouMadBroBadge _YMBB;
    private static MoglinPunt MPB
    {
        get => _MPB ??= new MoglinPunt();
        set => _MPB = value;
    }
    private static MoglinPunt _MPB;

    private static ZorbakPunt ZPB
    {
        get => _ZPB ??= new ZorbakPunt();
        set => _ZPB = value;
    }
    private static ZorbakPunt _ZPB;

    private static CtrlAltDelMemberBadge CAD
    {
        get => _CAD ??= new CtrlAltDelMemberBadge();
        set => _CAD = value;
    }
    private static CtrlAltDelMemberBadge _CAD;
    private static BirthdaySavior BS
    {
        get => _BS ??= new BirthdaySavior();
        set => _BS = value;
    }
    private static BirthdaySavior _BS;
    private static BattleBabysitter BB
    {
        get => _BB ??= new BattleBabysitter();
        set => _BB = value;
    }
    private static BattleBabysitter _BB;
    private static BattleConVIP BCV
    {
        get => _BCV ??= new BattleConVIP();
        set => _BCV = value;
    }
    private static BattleConVIP _BCV;
    private static CelestialArenaChampion CAC
    {
        get => _CAC ??= new CelestialArenaChampion();
        set => _CAC = value;
    }
    private static CelestialArenaChampion _CAC;
    private static ChaosPuppetMaster CPM
    {
        get => _CPM ??= new ChaosPuppetMaster();
        set => _CPM = value;
    }
    private static ChaosPuppetMaster _CPM;
    private static Committed C
    {
        get => _C ??= new Committed();
        set => _C = value;
    }
    private static Committed _C;
    private static ConZombieSlayer CZS
    {
        get => _CZS ??= new ConZombieSlayer();
        set => _CZS = value;
    }
    private static ConZombieSlayer _CZS;
    private static DesolichFreed DF
    {
        get => _DF ??= new DesolichFreed();
        set => _DF = value;
    }
    private static DesolichFreed _DF;
    private static GoldenLaurel GL
    {
        get => _GL ??= new GoldenLaurel();
        set => _GL = value;
    }
    private static GoldenLaurel _GL;
    private static GravelynsWarrior GW
    {
        get => _GW ??= new GravelynsWarrior();
        set => _GW = value;
    }
    private static GravelynsWarrior _GW;
    private static HordeZombieSLAYER HZS
    {
        get => _HZS ??= new HordeZombieSLAYER();
        set => _HZS = value;
    }
    private static HordeZombieSLAYER _HZS;
    private static LordOfTheWeddingRing LOTWR
    {
        get => _LOTWR ??= new LordOfTheWeddingRing();
        set => _LOTWR = value;
    }
    private static LordOfTheWeddingRing _LOTWR;
    private static MummySlayerAndCruxShadowsDefender MSACSD
    {
        get => _MSACSD ??= new MummySlayerAndCruxShadowsDefender();
        set => _MSACSD = value;
    }
    private static MummySlayerAndCruxShadowsDefender _MSACSD;
    private static RavenlossWarAndChampion RWAC
    {
        get => _RWAC ??= new RavenlossWarAndChampion();
        set => _RWAC = value;
    }
    private static RavenlossWarAndChampion _RWAC;
    private static ShadowVaultChampion SVC
    {
        get => _SVC ??= new ShadowVaultChampion();
        set => _SVC = value;
    }
    private static ShadowVaultChampion _SVC;
    private static StoneCold SC
    {
        get => _SC ??= new StoneCold();
        set => _SC = value;
    }
    private static StoneCold _SC;
    private static TableFlipper TF
    {
        get => _TF ??= new TableFlipper();
        set => _TF = value;
    }
    private static TableFlipper _TF;
    private static VoidHighlordBadge VHL
    {
        get => _VHL ??= new VoidHighlordBadge();
        set => _VHL = value;
    }
    private static VoidHighlordBadge _VHL;
    private static StoryArcBadge SA
    {
        get => _SA ??= new StoryArcBadge();
        set => _SA = value;
    }
    private static StoryArcBadge _SA;
    private static JusticeSquadBadge JS
    {
        get => _JS ??= new JusticeSquadBadge();
        set => _JS = value;
    }
    private static JusticeSquadBadge _JS;
    private static ThiefofChaosBadge ToC
    {
        get => _ToC ??= new ThiefofChaosBadge();
        set => _ToC = value;
    }
    private static ThiefofChaosBadge _ToC;
    private static UltraCarnaxBadge UC
    {
        get => _UC ??= new UltraCarnaxBadge();
        set => _UC = value;
    }
    private static UltraCarnaxBadge _UC;
    private static GoalBadge G
    {
        get => _G ??= new GoalBadge();
        set => _G = value;
    }
    private static GoalBadge _G;
    private static FrostvaleBadges FV
    {
        get => _FV ??= new FrostvaleBadges();
        set => _FV = value;
    }
    private static FrostvaleBadges _FV;
    private static YokaiAscension YA
    {
        get => _YA ??= new YokaiAscension();
        set => _YA = value;
    }
    private static YokaiAscension _YA;
    private static WorldCup WC
    {
        get => _WC ??= new WorldCup();
        set => _WC = value;
    }
    private static WorldCup _WC;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        DoAll();

        Core.SetOptions(false);
    }

    public void DoAll()
    {
        CRB.Badge();
        SPB.Badge();
        MPB.Badge();
        WC.Badge();
        ZPB.Badge();
        CAD.Badge();
        BS.Badge();
        BB.Badge();
        BCV.Badge();
        CAC.Badge();
        CPM.Badge();
        C.Badge();
        CZS.Badge();
        DF.Badge();
        GL.Badge();
        GW.Badge();
        HZS.Badge();
        LOTWR.Badge();
        MSACSD.Badge();
        RWAC.Badge();
        SVC.Badge();
        SC.Badge();
        TF.Badge();
        DMF.Badge();
        SA.Badge();
        JS.Badge();
        ToC.Badge();
        UC.Badge();
        G.Badge();
        FV.Badges();
        YMBB.Badge(true, true);
        VHL.Badge();
        YA.Badge();
        //add more as they are made.
    }
}
