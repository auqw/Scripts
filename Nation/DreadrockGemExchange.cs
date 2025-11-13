/*
name: Dreadrock Gem Exchange (Tainted Gem Quest)
description: This script will farm Tainted Gems using "Dreadrock Gem Exhange" quest.
tags: tainted gem, dreadrock, dread rock, tainted, exchange, enntropy, shattered, polished, crystal
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
using Skua.Core.Interfaces;

public class DreadrockGemExchange
{
    public CoreBots Core => CoreBots.Instance;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Nation.DreadrockGemExchange();

        Core.SetOptions(false);
    }
}
