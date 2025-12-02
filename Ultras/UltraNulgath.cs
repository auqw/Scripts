/*
name: UltraNulgath
description: Nulgath the Archfiend helper with taunter rotation and blade priority.
tags: Ultra
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

/* "Safe" Comp: (Enhancements go in order of `Helm | Class | Weapon | Cape`)
====================
            ===Taunters===

ArchPaladin
- Forge | Lucky | Valiance | Lament -

Lord Of Order:
- Forge | Lucky | Lucky aweblast / Valiance | Absolution -

            ===Taunters===
====================

====================
            ===DPSers===

King's Echo
- Examen | Lucky | Ravenous | Vainglory -

Legion Revenant
- Pneuma | Wizard | Valiance / Ravenous / Arcana | Vainglory -

            ===DPSers===
====================
*/
public class UltraNulgath
{
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private CoreBots C => CoreBots.Instance;
    private static CoreAdvanced _Adv;
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();

    string a,
        b;
    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraNulgath";
    public List<IOption> Options = new()
    {
        new Option<string>(
            "a",
            "First Taunter Class",
            "Insert the name of the class that will taunt",
            ""
        ),
        new Option<string>(
            "b",
            "Second Taunter Class",
            "Insert the name of the class that will taunt",
            ""
        ),
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        C.OneTimeMessage(
            "Ultra Nulgath",
            "Deaths more then likely will happen, Suggested class and thier enhs are in the script at the top"
        );
        
        if (
            Bot.Config != null
            && Bot.Config.Options.Contains(C.SkipOptions)
            && !Bot.Config.Get<bool>(C.SkipOptions)
        )
            Bot.Config.Configure();

        a = (Bot.Config!.Get<string>("a") ?? "").Trim();
        b = (Bot.Config!.Get<string>("b") ?? "").Trim();
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
        {
            C.Logger("Setup", "Fill both taunter classes in Script Options.");
            C.SetOptions(false);
        }

        Core.Boot();
        Prep();
        Fight();
        C.SetOptions(false);
    }

    void Prep()
    {
        if (Bot.Inventory.Items.Any(x => x != null && x.Equipped && (x.Name == a || x.Name == b)))
            Ultra.GetScrollOfEnrage();
        else
        {
            Ultra.UseAlchemyPotions(Ultra.GetBestTonicPotion(), Ultra.GetBestElixirPotion());
            Ultra.BuyAlchemyPotion("Potent Honor Potion");
            Core.EquipConsumable("Potent Honor Potion");
        }
    }

    void Fight()
    {
        const string map = "ultranulgath";
        const string boss = "Nulgath the Archfiend";
        const string blade = "Overfiend Blade";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        C.EnsureAccept(8692);
        C.AddDrop("Nulgath Insignia");
        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_nulgath.sync");

        Core.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Core.EnableSkills();

        while (!Bot.ShouldExit)
        {
            // Dead → wait for respawn
            if (!Bot.Player.Alive)
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);

            // Check if the whole army has finished
            if (Ultra.CheckArmyProgress("Nulgath the Archfiend Defeated?", 1, true, syncPath))
            {
                C.Logger("All players finished farm.");
                C.Join("whitemap");
                C.EnsureComplete(8692);
                break;
            }

            if (
                Bot.Inventory.Items.Any(x =>
                    x != null && x.Equipped && (x.Name == a || x.Name == b)
                )
            )
            {
                Bot.Combat.Attack(2);
                if (Bot.Skills.CanUseSkill(5) && !Bot.Self.Auras.Any(x => x.Name == "Focus"))
                    Bot.Skills.UseSkill(5);
            }
            else
            {
                Bot.Combat.Attack(
                    Bot.Monsters.MapMonsters.Any(x => x != null && x.MapID == 1 && x.HP > 0) ? 1 : 2
                );
                if (Bot.Skills.CanUseSkill(5))
                    Bot.Skills.UseSkill(5);
            }
            Bot.Sleep(500);
        }
    }
}
