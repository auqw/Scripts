/*
name: SecondSpeakerPrismaticSeams
description: does the 'Mega Timestream Medals' quest for prismatic seams
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class SecondSpeakerPrismaticSeams
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
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreDailies Daily
    {
        get => _Daily ??= new CoreDailies();
        set => _Daily = value;
    }
    private static CoreDailies _Daily;
    private static CoreSoW SoW
    {
        get => _SoW ??= new CoreSoW();
        set => _SoW = value;
    }
    private static CoreSoW _SoW;

    public string OptionsStorage = "SecondSpeakerPrismaticSeams";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<bool>("Use Public Boss", "Use public rooms for this boss... [RISKY]", "", false),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Example();

        Core.SetOptions(false);
    }

    public void Example(bool TestMode = false)
    {
        Core.Logger(
            Bot.Config?.Get<bool>("Use Public Boss") == true
                ? "Public Boss is enabled, THIS IS VERY RISKY!"
                : "Public Boss room Disabled",
            Bot.Config?.Get<bool>("Use Public Boss") == true ? "****WARNING***" : "****INFO***"
        );

        //Test Stuff Below here
        SoW.TimestreamWar();
        if (Bot.Config?.Get<bool>("Use Public Boss") == true)
        {
            Core.Logger(
                "Joining whitemap then back to streamwar to avoid being stuck in private room from doing the story."
            );
            Core.Join("whitemap");
            Core.PrivateRooms = false;
        }

        Core.Join("streamwar", publicRoom: Bot.Config?.Get<bool>("Use Public Boss") == true);

        Core.Logger(
            "This Script will use the class you have set as your `SoloClass` in your CBO options.(options > corebots > solo class)\n"
                + "If you want to use a different class, change your CBO options."
        );
        Core.EquipClass(ClassType.Solo);
        Core.AddDrop("Prismatic Seams");
        Core.RegisterQuests(8815);
        Core.FarmingLogger("Prismatic Seams", 2000);
        while (!Bot.ShouldExit && !Core.CheckInventory("Prismatic Seams", 2000))
        {
            if (Bot.Map.Name != "streamwar")
            {
                Core.Join(
                    "streamwar",
                    publicRoom: Bot.Config?.Get<bool>("Use Public Boss") == true
                );
                Bot.Wait.ForMapLoad("streamwar");
            }
            if (Bot.Player.Cell != "r5")
            {
                Core.Jump("r5", "Left");
                Bot.Wait.ForCellChange("r5");
            }
            Bot.Combat.Attack("Second Speaker");
            Core.Sleep(500);
            if (Core.CheckInventory("Prismatic Seams", 2000))
                break;
        }
        // Safely Exit Boss Room to the neighboring room
        Core.Sleep(1500);
        Core.Jump("r4", "Right");
        Core.CancelRegisteredQuests();
        Bot.Wait.ForPickup("Prismatic Seams");
    }
}
