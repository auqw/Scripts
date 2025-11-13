/*
name: 0MaxDarkonMats
description: Max all of the Darkon materials
tags: darkon, max, all, new class
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Darkon/CoreDarkon.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs
using Skua.Core.Interfaces;

public class MaxAllDarkon
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreDarkon Darkon
    {
        get => _Darkon ??= new CoreDarkon();
        set => _Darkon = value;
    }
    private static CoreDarkon _Darkon;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
        Core.BankingBlackList.AddRange(
            new[]
            {
                "A Melody",
                "Ancient Remnant",
                "Bandit's Correspondence",
                "Darkon's Receipt",
                "La's Gratitude",
                "Mourning Flower",
                "Jus Divinum Scale",
                "Unfinished Musical Score",
                "Teeth",
            }
        );
        GetAllMats();

        Core.SetOptions(false);
    }

    public void GetAllMats()
    {
        Darkon.AMelody(MaxStack: true);
        Darkon.AncientRemnant(MaxStack: true);
        Darkon.BanditsCorrespondence(MaxStack: true);
        Darkon.FarmReceipt(MaxStack: true);
        Darkon.LasGratitude(MaxStack: true);
        Darkon.WheelofFortune(MaxStack: true);
        Darkon.SukisPrestiege(MaxStack: true);
        Darkon.Teeth(MaxStack: true);
        Darkon.AstravianMedal(MaxStack: true);
        Darkon.UnfinishedMusicalScore(300);
        Core.Logger(
            $"This will require {(22 - Core.dynamicQuant("Darkon's Instant Noodle", false)) * 2222222} gold"
        );
        Adv.BuyItem("garden", 1831, "Darkon's Instant Noodle", 22);
    }
}
