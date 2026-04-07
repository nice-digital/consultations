using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class NodeSsrService : IHostedService, IDisposable
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger _logger;
    private Process _nodeProcess;

    private const int Port = 4000;

    public NodeSsrService(IWebHostEnvironment env, ILogger<NodeSsrService> logger)
    {
        _env = env;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("NodeSsrService starting...");

        KillProcessesOnPort(Port);

        var workingDir = Path.Combine(_env.ContentRootPath, "ClientApp");
        var scriptPath = Path.Combine(workingDir, "src", "server", "ssr-server.js");

        _logger.LogInformation($"WorkingDirectory: {workingDir}");
        _logger.LogInformation($"ScriptPath: {scriptPath}");

        if (!File.Exists(scriptPath))
            throw new FileNotFoundException("SSR script not found", scriptPath);

        if (_nodeProcess != null && !_nodeProcess.HasExited)
        {
            _logger.LogWarning("Node process already running, skipping start.");
            return;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = "node",
            Arguments = scriptPath,
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var nodeModulesPath = Path.Combine(workingDir, "node_modules");
        startInfo.Environment["NODE_PATH"] = nodeModulesPath;

        _logger.LogInformation($"NODE_PATH set to: {nodeModulesPath}");

        var envFile = Path.Combine(workingDir, ".env");
        if (File.Exists(envFile))
        {
            foreach (var line in File.ReadAllLines(envFile))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                    startInfo.Environment[parts[0]] = parts[1];
            }
        }

        _nodeProcess = new Process { StartInfo = startInfo };

        _nodeProcess.OutputDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                _logger.LogInformation("[SSR OUTPUT] " + e.Data);
        };

        _nodeProcess.ErrorDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                _logger.LogError("[SSR ERROR] " + e.Data);
        };

        _nodeProcess.EnableRaisingEvents = true;
        _nodeProcess.Exited += (_, __) =>
        {
            _logger.LogError("Node SSR process EXITED unexpectedly!");
        };

        try
        {
            _logger.LogInformation("Starting Node process...");
            _nodeProcess.Start();

            _logger.LogInformation($"Node process started. PID: {_nodeProcess.Id}");

            _nodeProcess.BeginOutputReadLine();
            _nodeProcess.BeginErrorReadLine();

            await WaitForPortAsync(Port, 15000);

            _logger.LogInformation("Node SSR server is ready.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to start Node SSR process");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_nodeProcess != null && !_nodeProcess.HasExited)
            {
                _logger.LogInformation("Stopping Node SSR process...");
                _nodeProcess.Kill(true);
                _nodeProcess.WaitForExit(5000);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping Node process");
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _nodeProcess?.Dispose();
    }

    private void KillProcessesOnPort(int port)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = $"/c netstat -ano -p tcp | findstr :{port}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var pids = new HashSet<int>();

            foreach (var line in output.Split(Environment.NewLine))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (!line.Contains("LISTENING")) continue;

                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (int.TryParse(parts[^1], out var pid))
                    pids.Add(pid);
            }

            foreach (var pid in pids)
            {
                try
                {
                    var proc = Process.GetProcessById(pid);

                    _logger.LogWarning($"Killing PID {pid} on port {port} ({proc.ProcessName})");

                    proc.Kill(true);
                    proc.WaitForExit(5000);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to kill PID {pid}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to inspect port {port}");
        }
    }
    private async Task WaitForPortAsync(int port, int timeoutMs)
    {
        var start = DateTime.UtcNow;

        while ((DateTime.UtcNow - start).TotalMilliseconds < timeoutMs)
        {
            if (_nodeProcess.HasExited)
            {
                throw new Exception("Node process exited while waiting for port.");
            }

            try
            {
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync("127.0.0.1", port);

                var completed = await Task.WhenAny(connectTask, Task.Delay(500));

                if (completed == connectTask && client.Connected)
                    return;
            }
            catch { }

            await Task.Delay(200);
        }

        throw new TimeoutException($"Timed out waiting for port {port}");
    }
}
