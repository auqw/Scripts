/*
name: Prince Darkon's Poleaxe Pre Reqs
description: This script will farm Prince Darkon's Poleaxe Pre-Reqs.
tags: darkon, poleaxe, prereqs, teeth, melody, receipt, gratitude, astravian medal, prince
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Darkon/CoreDarkon.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/DarkonGarden.cs
using Skua.Core.Interfaces;

public class PrinceDarkonsPoleaxePreReqs
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreDarkon Darkon
    {
        get => _Darkon ??= new CoreDarkon();
        set => _Darkon = value;
    }
    private static CoreDarkon _Darkon;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        FarmPreReqs();

        Core.SetOptions(false);
    }

    public void FarmPreReqs(bool buyPoleaxe = false)
    {
        if (Core.CheckInventory("Prince Darkon's Poleaxe"))
            return;

        Darkon.AMelody(22);
        Darkon.FarmReceipt(22);
        Darkon.Teeth(22);
        Darkon.LasGratitude(22);
        Darkon.AstravianMedal(22);

        if (!Core.CheckInventory("Darkon's Instant Noodle", 22))
        {
            Farm.Gold(48888884);
            Core.BuyItem("garden", 1831, "Darkon's Instant Noodle", 22);
        } //BuyPoleaxe();

        if (buyPoleaxe)
            BuyPoleaxe();
    }

    private void BuyPoleaxe()
    {
        if (!Core.CheckInventory("Prince Darkon's Poleaxe"))
        {
            if (
                !Core.CheckInventory("Algie's Bow")
                && Core.CheckInventory("King Drago Insignia", 5)
            )
            {
                Core.BuyItem("ultradrago", 2066, "Algie's Bow");
                Bot.Wait.ForItemBuy();
            }

            if (
                !Core.CheckInventory("Dene's Axe") && Core.CheckInventory("King Drago Insignia", 5)
            )
            {
                Core.BuyItem("ultradrago", 2066, "Dene's Axe");
                Bot.Wait.ForItemBuy();
            }

            if (Core.CheckInventory("King Drago Insignia", 10))
            {
                Core.BuyItem("ultradrago", 2066, "Prince Darkon's Poleaxe");
                Bot.Wait.ForItemBuy();
            }
        }
    }
}
