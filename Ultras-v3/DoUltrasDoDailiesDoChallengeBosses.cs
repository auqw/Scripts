/*
name: Do Ultras + Dailies + Challenge Bosses
description: Runs all ultras, dailies, and challenge bosses.
tags: ultras,dailies,challenge bosses,all
*/
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreEnginev3.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/CoreUltrav3.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/Ultras-v3/Dependencies-Ultras/UltraWaitForArmy.cs

//cs_include Scripts/Ultras-v3/DoAllUltras.cs
//cs_include Scripts/Ultras-v3/Dependencies-Dailies/DoAllChallengeBosses.cs
//cs_include Scripts/Dailies/0AllDailies.cs

using Skua.Core.Interfaces;

public class DoUltrasDoDailiesDoChallengeBosses
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots C => CoreBots.Instance;
    private static CoreEnginev3 Core => CoreEnginev3.Instance;
    private static CoreUltrav3 Ultra => _Ultra ??= new CoreUltrav3();
    private static CoreUltrav3 _Ultra;

    public void ScriptMain(IScriptInterface Bot)
    {
        C.SetOptions(true);
        Core.Boot();

        new DoAllUltras().RunAll();
        UltraWaitForArmy.Instance.NewWaitForArmy(3, "doall_sync.sync", useSkill: false);

        new DoAllChallengeBosses().RunAll();
        UltraWaitForArmy.Instance.NewWaitForArmy(3, "doall_sync.sync", useSkill: false);

        new FarmAllDailies().DoAllDailies();
        UltraWaitForArmy.Instance.NewWaitForArmy(3, "doall_sync.sync", useSkill: false);

        Core.DisableSkills();
        C.SetOptions(false);
    }
}
