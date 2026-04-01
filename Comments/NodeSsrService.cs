using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting; // important
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class NodeSsrService : IHostedService, IDisposable
{
    private readonly IWebHostEnvironment _env;
    private Process _nodeProcess;
    private ILogger _logger;

    public NodeSsrService(IWebHostEnvironment env, ILogger<NodeSsrService>logger)
    {
        _env = env;
        _logger = logger;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("NodeSsrService starting...");

        var scriptPath = Path.Combine(
            _env.ContentRootPath,
            "ClientApp",
            "src",
            "server",
            "ssr-server.js"
        );

        var workingDir = Path.Combine(_env.ContentRootPath, "ClientApp");

        _logger.LogInformation($"ContentRootPath: {_env.ContentRootPath}");
        _logger.LogInformation($"WorkingDirectory: {workingDir}");
        _logger.LogInformation($"ScriptPath: {scriptPath}");

        _logger.LogInformation($"Script exists: {File.Exists(scriptPath)}");
        _logger.LogInformation($"Working dir exists: {Directory.Exists(workingDir)}");

        var startInfo = new ProcessStartInfo
        {
            FileName = "node",
            Arguments = scriptPath,
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        _logger.LogInformation("ProcessStartInfo configured");
        _logger.LogInformation($"FileName: {startInfo.FileName}");
        _logger.LogInformation($"Arguments: {startInfo.Arguments}");

        var envFile = Path.Combine(".env");
        _logger.LogInformation($"Looking for .env at: {Path.GetFullPath(envFile)}");

        if (File.Exists(envFile))
        {
            _logger.LogInformation(".env file found, loading variables...");

            foreach (var line in File.ReadAllLines(envFile))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                {
                    startInfo.Environment[parts[0]] = parts[1];
                    _logger.LogInformation($"Loaded env var: {parts[0]}");
                }
            }
        }
        else
        {
            _logger.LogWarning(".env file NOT found");
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

        try
        {
            _logger.LogInformation("Starting Node process...");
            _nodeProcess.Start();

            _logger.LogInformation($"Node process started. PID: {_nodeProcess.Id}");

            if (_nodeProcess.HasExited)
            {
                _logger.LogError("Node process exited immediately after start!");
            }

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
                _nodeProcess.Kill();
        }
        catch { }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _nodeProcess?.Dispose();
    }
}
