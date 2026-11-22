/*
name: Find Top 3 Best Server Pings
description: measures AQW handshake latency and logs the three fastest servers ( diagnostics > logs )
tags: server, ping, ultra
*/

//cs_include Scripts/CoreBots.cs

using Skua.Core.Interfaces;
using Skua.Core.Models.Servers;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

public class FindBestPing
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    public async Task ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions(disableClassSwap: true);

        try
        {
            string result = await FindTopThreeServers();
            Bot.Log(result);
        }
        finally
        {
            Core.SetOptions(false);
        }
    }

    /// <summary>
    /// Finds the three fastest game servers by measuring actual AQW handshake latency.
    /// Uses three samples per server and ranks servers by their median successful latency.
    /// </summary>
    private async Task<string> FindTopThreeServers()
    {
        List<Server> servers = await Bot.Servers.GetServers(true);
        byte[] versionCheck = Encoding.UTF8.GetBytes("<msg t='sys'><body action='verChk' r='0'><ver v='165' /></body></msg>\0");
        List<(Server Server, long Ping)> pingResults = new();

        foreach (Server server in servers.Where(server =>
            server.Online && !string.IsNullOrWhiteSpace(server.IP) && server.Port > 0))
        {
            List<double> samples = new(3);

            for (int sample = 0; sample < 3; sample++)
            {
                try
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    using TcpClient client = new();
                    await client.ConnectAsync(server.IP, server.Port).WaitAsync(TimeSpan.FromMilliseconds(2500));

                    using NetworkStream stream = client.GetStream();
                    await stream.WriteAsync(versionCheck.AsMemory());
                    await stream.FlushAsync();

                    using CancellationTokenSource timeout = new(2500);
                    byte[] buffer = new byte[1];
                    StringBuilder response = new();

                    while (!timeout.IsCancellationRequested)
                    {
                        int bytesRead = await stream.ReadAsync(buffer.AsMemory(), timeout.Token);
                        if (bytesRead == 0)
                            break;

                        if (buffer[0] != 0)
                        {
                            response.Append((char)buffer[0]);
                            continue;
                        }

                        string message = response.ToString();
                        response.Clear();

                        if (message.Contains("action='apiOK'", StringComparison.OrdinalIgnoreCase) ||
                            message.Contains("action=\"apiOK\"", StringComparison.OrdinalIgnoreCase))
                        {
                            stopwatch.Stop();
                            samples.Add(stopwatch.Elapsed.TotalMilliseconds);
                            break;
                        }

                        if (message.Contains("action='apiKO'", StringComparison.OrdinalIgnoreCase) ||
                            message.Contains("action=\"apiKO\"", StringComparison.OrdinalIgnoreCase))
                            break;
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception) { }
            }

            if (samples.Count == 0)
                continue;

            samples.Sort();
            long medianPing = (long)Math.Round(samples[samples.Count / 2], MidpointRounding.AwayFromZero);
            pingResults.Add((server, medianPing));
        }

        List<(Server Server, long Ping)> bestServers = pingResults
            .OrderBy(result => result.Ping)
            .ThenBy(result => result.Server.Name, StringComparer.OrdinalIgnoreCase)
            .Take(3)
            .ToList();

        if (bestServers.Count == 0)
            return "Could not determine an optimal server, all hosts failed their AQW handshake.";

        return $"Top {bestServers.Count} servers by AQW handshake latency:{Environment.NewLine}" +
            string.Join(Environment.NewLine, bestServers.Select((result, index) =>
                $"{index + 1}. {result.Server.Name} ({result.Server.IP}) - {result.Ping}ms"));
    }
}
