/*
name: Four Harbingers LW (old, this script will not work) (it will be removed soon)
description: null
tags: null
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Skills;
using Skua.Core.Options;
using System.Reflection;

public class FourHarbingersLW
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.Logger("This script has been removed, run the ones in the folder: Prototypes/FourHarbingers instead", messageBox: true);
        Core.Sleep(2000);
    }
}