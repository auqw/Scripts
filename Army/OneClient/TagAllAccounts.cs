/*
name: Tag All
description: use the AutoTag to tag all accounds in the manager.
tags: tags, autotag, auto tag
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Army/CoreArmyLite.cs
using Skua.Core.Interfaces;

public class TagAll
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreArmyLite Army
    {
        get => _Army ??= new CoreArmyLite();
        set => _Army = value;
    }
    private static CoreArmyLite _Army;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        DoTheThing();

        Core.SetOptions(false);
    }

    public void DoTheThing()
    {
        Bot.Options.ReloginServer = "Twilly";

        List<ManagedAccount> accounts = Bot.Accounts.GetAllAccounts();
        int AccCount = accounts.Count;
        int i = 1;

        while (!Bot.ShouldExit && Army.doForAll())
        {
            while (!Bot.ShouldExit && !Bot.Player.Loaded)
            {
                Core.Sleep(1000);
                if (Bot.Wait.ForMapLoad("battleon"))
                    break;
            }
            Core.Logger($"Adding Tags to {Bot.Player.Username} [Account {i}/{AccCount}], please be patient", "Tag All");
            Core.AutoAddTags();
            Core.Logger($"Tags for {Bot.Player.Username}, finished!", "Tag All");
            i++;
        }
    }
}
