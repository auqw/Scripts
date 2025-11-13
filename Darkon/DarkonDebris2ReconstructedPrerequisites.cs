/*
name: Darkon Debris 2 [reconstructed] Prerequisites
description: Farms the non-boss items to get the Darkon debris 2 [reconstructed], may require some manualing
tags: darkon, darkon debris 2, prerequisites
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ElegyofMadness(Darkon)/CoreAstravia.cs
//cs_include Scripts/Darkon/CoreDarkon.cs

using Skua.Core.Interfaces;

public class DarkonDebris2ReconstructedPrerequisites
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreDarkon Darkon
    {
        get => _Darkon ??= new CoreDarkon();
        set => _Darkon = value;
    }
    private static CoreDarkon _Darkon;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        FarmAll();

        Core.SetOptions(false);
    }

    public void FarmAll()
    {
        bool UseInsigs = false;
        if (Core.CheckInventory("Darkon Insignia", 20))
        {
            if (
                Core.OneTimeMessage(
                    "Insignia Warning",
                    "20 Darkon Insignia will be used to buy the \"Darkon Debris 2 (Reconstructed)\" item, is this ok?",
                    true,
                    true,
                    true
                ) == true
            )
            {
                Core.Logger(
                    "20 Darkon Insignia will be used to buy the \"Darkon Debris 2 (Reconstructed)\" item, is this ok? (y/n)",
                    messageBox: true
                );
                UseInsigs = true;
            }
        }
        if (!Core.CheckInventory("Darkon's Debris 2 (Reconstructed)"))
        {
            if (!Core.CheckInventory("Darkon's Debris 2 (Recovered)"))
            {
                Darkon.UnfinishedMusicalScore(22);
                Core.BuyItem("theworld", 2141, "Darkon's Debris 2 (Recovered)");
            }
            Darkon.BanditsCorrespondence(22);
            Darkon.SukisPrestiege(22);
            Darkon.AncientRemnant(22);
            Darkon.WheelofFortune(22, 0);
            if (!Core.CheckInventory("Darkon Insignia", 20))
            {
                Core.Logger(
                    " x20 \"Darkon Insignia\" is Required to continue the quest, our Bots cannot *currently* kill this mob, use Grim (different client) & @InsertNameHere's ultra bot",
                    messageBox: true
                );
                return;
            }
            if (UseInsigs)
                Core.BuyItem("ultradarkon", 2147, "Darkon's Debris 2 (Reconstructed)");
            else
            {
                Core.Logger(
                    "You have 20 Darkon Insignia, but you didn't want to use them, so we will not buy the \"Darkon Debris 2 (Reconstructed)\" item, do so yourself.",
                    "No Insig Use",
                    messageBox: true
                );
                return;
            }
        }
    }
}
