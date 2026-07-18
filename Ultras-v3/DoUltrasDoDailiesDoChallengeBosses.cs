/*
name: Do Ultras + Dailies + Challenge Bosses
description: Runs all ultras, dailies, and challenge bosses.
tags: ultras,dailies,challenge bosses,all
*/
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreEngine2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraWaitForArmy.cs

//cs_include Scripts/Ultras-v3/DoAllUltras.cs
//cs_include Scripts/Ultras-v3/Dependencies-Dailies/0AllDailiesv3.cs
//cs_include Scripts/Ultras-v3/Dependencies-Dailies/DoAllChallengeBosses.cs

using Skua.Core.Interfaces;

public class DoUltrasDoDailiesDoChallengeBosses
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots2 C => CoreBots2.Instance;
    private static CoreEngine2 Core => CoreEngine2.Instance;
    private static CoreUltra2 Ultra => _Ultra ??= new CoreUltra2();
    private static CoreUltra2 _Ultra;

    public void ScriptMain(IScriptInterface Bot)
    {
        C.SetOptions(disableCoreSkills: true);
        Core.Boot();

        new DoAllUltras().RunAll();
        UltraWaitForArmy.Instance.NewWaitForArmy(3, "doall_sync.sync", useSkill: false);

        new FarmAllDailies().RunAll();
        UltraWaitForArmy.Instance.NewWaitForArmy(3, "doall_sync.sync", useSkill: false);

        new DoAllChallengeBosses().RunAll();
        UltraWaitForArmy.Instance.NewWaitForArmy(3, "doall_sync.sync", useSkill: false);

        Core.DisableSkills();
        C.SetOptions(false);
    }
}
