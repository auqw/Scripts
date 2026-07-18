/*
name: UltraPulse
description: 5-second wall-clock pulse generator for Ultra taunt systems.
tags: ultra, pulse, taunt
*/

//cs_include Scripts/Ultras-v3/Dependencies-Core/CoreBots2.cs

using System;

public class UltraPulse
{
    public static int CurrentPulse(DateTime fightStart, int pulseIntervalSec = 5)
        => (int)((DateTime.UtcNow - fightStart).TotalSeconds / pulseIntervalSec);
}
