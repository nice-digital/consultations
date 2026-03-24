using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Comments
{
    public class NodeSsrService : IHostedService, IDisposable
    {
        private Process _nodeProcess;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var scriptPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "ClientApp",
                "src",
                "server",
                "ssr-server.js"
            );

            var startInfo = new ProcessStartInfo
            {
                FileName = "node",
                Arguments = scriptPath,
                WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "ClientApp"),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var envFile = Path.Combine(
                startInfo.WorkingDirectory,
                env == "Development" ? ".env" : ".env.production"
            );

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
                    Console.WriteLine("[SSR] " + e.Data);
            };

            _nodeProcess.ErrorDataReceived += (_, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Console.WriteLine("[SSR ERROR] " + e.Data);
            };

            _nodeProcess.Start();
            _nodeProcess.BeginOutputReadLine();
            _nodeProcess.BeginErrorReadLine();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!_nodeProcess.HasExited)
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
}
