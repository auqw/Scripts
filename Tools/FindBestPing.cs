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

    public async Task ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions(disableClassSwap: true);

        try
        {
            string result = await Core.FindBestServer();
            Bot.Log(result);
        }
        finally
        {
            Core.SetOptions(false);
        }
    }


}



