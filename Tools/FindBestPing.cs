/*
name: Find Best Server Ping
description: will spit out the  best server ( ping wise) into the logs ( diagnostics > logs )
tags: server, ping, ultra
*/
//cs_include Scripts/CoreBots.cs
using Skua.Core.Interfaces;

public class FindBestPing
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions(disableClassSwap: true);

        Core.FindBestServer();

        Core.SetOptions(false);
    }


}



