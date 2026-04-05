//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/Dungeons/EclipseAscent/CoreEclipse.cs
//cs_include Scripts/Dungeons/EclipseAscent/CelestialTempleForgeMerge.cs
//cs_include Scripts/Army/CoreArmyLite.cs

using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Options;

namespace SkuaScripts.Scripts.Custom.EclipseAscent;

public class GreatbladeoftheEntwinedEclipse
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots C => CoreBots.Instance;
    private static CoreAdvanced Adv = new();
    public CoreUltra Ultra = new();
    private static CoreArmyLite sArmy = new();
    public CoreEclipse coreEclipse = new();
    public CelestialTempleForgeMerge templeMerge = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "EclipseAscent";

    public List<IOption> Options = new()
    {
        new Option<string>(
            "player1",
            "Account #1",
            "This character will be using Legion Revenant",
            ""
        ),
        new Option<string>(
            "player2",
            "Account #2",
            "This character will be using StoneCrusher",
            ""
        ),
        new Option<string>(
            "player3",
            "Account #3",
            "This character will be using ArchPaladin",
            ""
        ),
        new Option<string>(
            "player4",
            "Account #4",
            "This character will be using Lord Of Order",
            ""
        ),
        new Option<bool>(
            "autoclass",
            "Auto Equip Classes",
            "This will auto equip all classes, if false it will use the classes already equipped.",
            true
        ),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        Bot.Events.ScriptStopping += OnBotStopped;
        Bot.Events.ExtensionPacketReceived += sArmy.PartyManagement;

        C.BankingBlackList.AddRange(new[]
            { "Sliver of Moonlight", "Sliver of Sunlight", "Victor of the Festival", "Ecliptic Offering" });

        C.SetOptions();
        C.SendPackets($"%xt%zm%cmd%1%uopref%bParty%true%"); //To be able to join party

        while (!Bot.ShouldExit && sArmy.PartyMemberArray()!.Length < 4)
            coreEclipse.SetupParty();

        C.SendPackets($"%xt%zm%cmd%1%uopref%bParty%false%");

        Adv.GearStore(EnhAfter: true);

        coreEclipse.EquipWait();
        coreEclipse.EquipClasses(true);

        templeMerge.BuyAllMerge("");

        sArmy.PartyLeave();

        Bot.Events.ScriptStopping -= OnBotStopped;
        Bot.Events.ExtensionPacketReceived -= sArmy.PartyManagement;

        Adv.GearStore(true, EnhAfter: true);

        C.SetOptions(false);
    }

    private bool OnBotStopped(Exception? exception)
    {
        Bot.Events.ScriptStopping -= OnBotStopped;
        Bot.Events.ExtensionPacketReceived -= sArmy.PartyManagement;

        C.JumpWait();
        sArmy.PartyLeave();

        Adv.GearStore(true, EnhAfter: true);

        return true;
    }
}
