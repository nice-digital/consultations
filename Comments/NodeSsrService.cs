using System;
using System.Diagnostics;
using System.IO;
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

    public NodeSsrService(IWebHostEnvironment env, ILogger<NodeSsrService> logger)
    {
        _env = env;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("NodeSsrService starting...");

        var workingDir = Path.Combine(_env.ContentRootPath, "ClientApp");
        var scriptPath = Path.Combine(workingDir, "src", "server", "ssr-server.js");

        _logger.LogInformation($"WorkingDirectory: {workingDir}");
        _logger.LogInformation($"ScriptPath: {scriptPath}");
        _logger.LogInformation($"Script exists: {File.Exists(scriptPath)}");

        if (!File.Exists(scriptPath))
        {
            throw new FileNotFoundException("SSR script not found", scriptPath);
        }

        if (_nodeProcess != null && !_nodeProcess.HasExited)
        {
            _logger.LogWarning("Node process already running, skipping start.");
            return Task.CompletedTask;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = "node",
            Arguments = scriptPath,
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        startInfo.Environment["NODE_PATH"] = Path.Combine(workingDir, "node_modules");

        var nodeModulesPath = Path.Combine(workingDir, "node_modules");
        startInfo.Environment["NODE_PATH"] = nodeModulesPath;

        _logger.LogInformation($"NODE_PATH set to: {nodeModulesPath}");

        // Load .env
        var envFile = Path.Combine(workingDir, ".env");
        if (File.Exists(envFile))
        {
            foreach (var line in File.ReadAllLines(envFile))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                {
                    startInfo.Environment[parts[0]] = parts[1];
                }
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
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to start Node SSR process");
            throw;
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_nodeProcess != null && !_nodeProcess.HasExited)
            {
                _logger.LogInformation("Stopping Node SSR process...");
                _nodeProcess.Kill();
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
}
