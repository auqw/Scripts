/*
name: Lotus Tomb
description: Does the /lotustomb storyline
tags: shadow of doom, lotustomb, lotus, tomb, story, saga, zhoom, tomb of the blue lotus
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/BlackFriday/ShadowofDoom/CoreShadowofDoom.cs
using Skua.Core.Interfaces;

public class LotusTomb
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreShadowofDoom CoreSoD
    {
        get => _CoreSoD ??= new CoreShadowofDoom();
        set => _CoreSoD = value;
    }
    private static CoreShadowofDoom _CoreSoD;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        CoreSoD.LotusTomb();

        Core.SetOptions(false);
    }
}
