/*
name: UltraWarden
description: Ultra Warden helper with HP-band taunt trigger and army synchronization.
tags: Ultra
*/

//cs_include Scripts/Ultras/CoreEngine.cs
//cs_include Scripts/Ultras/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class UltraWarden
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
    public string OptionsStorage = "UltraWarden";
    public List<IOption> Options = new()
    {
        new Option<string>("a", "Taunter Class (Primary)", "Class name that will taunt first", ""),
        new Option<string>("b", "Taunter Class (Backup)", "Backup taunter class", ""),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        a = (Bot.Config!.Get<string>("a") ?? "").Trim();
        b = (Bot.Config.Get<string>("b") ?? "").Trim();

        if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
        {
            Core.Log(
                "Setup",
                "Fill at least one taunter class (Primary or Backup) in Script Options."
            );
            Bot.Stop();
            return;
        }

        Core.Boot();
        Prep();
        Fight();
        Bot.Stop();
    }

    bool IsTaunter() => Core.HasClassEquipped(a) || Core.HasClassEquipped(b);

    void Prep()
    {
        if (IsTaunter())
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
        const string map = "ultrawarden";
        const string boss = "Ultra Warden";

        string syncPath = Ultra.ResolveSyncPath("UltraItemCheck.sync");
        Ultra.ClearSyncFile(syncPath);
        C.EnsureAccept(8153);
        C.AddDrop("Warden Insignia");
        Core.Join(map);
        Ultra.WaitForArmy(3, "ultra_warden.sync");
        Core.ChooseBestCell(boss);
        Core.EnableSkills();

        while (!Bot.ShouldExit)
        {
            if (Ultra.CheckArmyProgress("Ultra Warden Defeated", 1, true, syncPath))
            {
                C.Jump("Enter", "Spawn");
                C.Logger("All players finished farm.");
                C.EnsureComplete(8153);
                break;
            }

            // Dead → wait for respawn
            if (!Bot.Player.Alive)
            {
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Core.HasClassEquipped(a))
            {
                Ultra.UltraWardenTaunter();
            }
            else if (Core.HasClassEquipped(b))
            {
                Ultra.UltraWardenTaunter();
            }
            else
            {
                Core.Kill(boss);
            }
        }
    }
}
