public class UltraAvatarTyndarius
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreUltras Core = new();

    public bool DontPreconfigure = true;
    public string OptionsStorage = "UltraAvatarTyndarius";
    public List<IOption> Options = new()
    {
        new Option<string>("primaryTaunter", "First Taunter Class", "Insert the name of the class that will taunt", "ArchPaladin"),
        new Option<string>("secondaryTaunter", "Second Taunter Class", "Insert the name of the class that will taunt", "Lord Of Order"),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.Boot();

        Kill(primaryTaunter: Bot.Config.Get<string>("primaryTaunter"), secondaryTaunter: Bot.Config.Get<string>("secondaryTaunter"));

        Bot.Stop();
    }

    void Kill(string primaryTaunter, string secondaryTaunter)
    {
        if (Core.HasClassEquipped(primaryTaunter) || Core.HasClassEquipped(secondaryTaunter))
            Core.GetScrollOfEnrage();

        Core.Join("ultratyndarius");
        Core.WaitForArmy(3);
        Core.ChooseBestCell("Ultra Avatar Tyndarius");
        Core.EnableSkills();

        while (Core.MonsterAlive("Ultra Avatar Tyndarius") && !Bot.ShouldExit)
        {
            if (Core.HasClassEquipped(primaryTaunter))
                Core.TauntCycle(primaryTaunter, "Ultra Avatar Tyndarius", "Focus", 250);
            else if (Core.HasClassEquipped(secondaryTaunter))
                Core.TauntCycle(secondaryTaunter, "Ultra Avatar Tyndarius", "Focus", 700);
            else
                Core.KillWithPriority("Ultra Avatar Tyndarius", 2, "Ultra Fire Orb", 3, "Ultra Fire Orb", 1);
        }
    }
}
