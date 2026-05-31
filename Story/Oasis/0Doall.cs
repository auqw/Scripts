/*
name: OasisDoaAll
description: This will finish the Duat Palace Story.
tags: story, quest, duat palace, Oasis, Do, all, 0doall, do all, oasis do all, DuatPalace, CrocRiver, CrulonWed, MeresankhChambers
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
//cs_include Scripts/Story/Oasis/CoreOasis.cs
using Skua.Core.Interfaces;

public class OasisDoaAll
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;

    private static CoreOasis COA
    {
        get => _COA ??= new CoreOasis();
        set => _COA = value;
    }
    private static CoreOasis _COA;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        COA.DoAll();

        Core.SetOptions(false);
    }


}
