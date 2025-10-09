/*
name: Army Free 500 accs
description: the 500 free acs quest
tags: acs, free, thefamily, army.
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/Other/FreeAcGifts[Yearly]/202xFreeAcs[TBD].cs

using Skua.Core.Interfaces;

public class ArmyFreeAcs
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreArmyLite Army { get => _Army ??= new CoreArmyLite(); set => _Army = value; }
    private static CoreArmyLite _Army;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;
    private static FreeAcs ACs { get => _ACs ??= new FreeAcs(); set => _ACs = value; }
    private static FreeAcs _ACs;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Doit();

        Core.SetOptions(false);
    }

    private void Doit()
    {
        Core.OneTimeMessage("Only for army", "This is intended for use with an army, not for solo players.");

        while (!Bot.ShouldExit && Army.doForAll())
        {
            ACs.GetYourAcsHere();
        }
    }
}