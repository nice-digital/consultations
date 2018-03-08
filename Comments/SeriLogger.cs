using System;
using Comments.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NICE.Logging;
using NICE.Logging.Sinks.RabbitMQ;
using Serilog;

namespace Comments
{
    public interface ISeriLogger {
        void Configure(ILoggerFactory loggerFactory, IConfiguration configuration, IApplicationLifetime appLifetime, IHostingEnvironment env);
    }

    public class SeriLogger : ISeriLogger
    {
        public void Configure(ILoggerFactory loggerFactory, IConfiguration configuration, IApplicationLifetime appLifetime, IHostingEnvironment env)
        {
            // read appsettings
            var logCfg = configuration.GetSection("Logging");

            loggerFactory.AddConsole(logCfg); // add provider to send logs to System.Console.WriteLine()
            loggerFactory.AddDebug(); // add provider to send logs to System.Diagnostics.Debug.WriteLine()

            var useRabbit = bool.Parse(logCfg["UseRabbit"]);
            var useFile = bool.Parse(logCfg["UseFile"]);

            int.TryParse(logCfg["RabbitMQPort"], out var rPort);
            var rHost = logCfg["RabbitMQHost"];
            var logFilePath = logCfg["LogFilePath"];
            var environment = AppSettings.Environment.Name;

            var formatter = new NiceSerilogFormatter(environment, "Consultations");

            var rabbit = new RabbitMQConfiguration {
                Hostname = rHost,
                Port = rPort,
                Protocol = RabbitMQ.Client.Protocols.AMQP_0_9_1,
                Exchange = "logging.application.serilog",
                ExchangeType = "topic"
            };

            var logConfig = new LoggerConfiguration()
                .MinimumLevel
                .Warning();

            if (useRabbit)
                logConfig.WriteTo.RabbitMQ(rabbit, formatter);

            if (useFile)
                logConfig.WriteTo.RollingFile(formatter, logFilePath, fileSizeLimitBytes: 5000000);

            Log.Logger = logConfig.CreateLogger();

            // add serilog provider (this is the hook)
            loggerFactory.AddSerilog();


                //var useFile = bool.Parse(logCfg["UseFile"]);

                //Log.Logger = GetFileLogger(logFilePath, formatter);
                //loggerFactory.AddSerilog();

                //var logger = loggerFactory.CreateLogger<SeriLogger>();
                //logger.LogError($"Could not connect to RabbitMQ. Hostname:{rHost} Port:{rPort}, Exception: {ex.Message}");
            

            // clean up on shutdown
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);
        }


        private static Serilog.ILogger GetFileLogger(string logFilePath, NiceSerilogFormatter formatter)
        {
            return new LoggerConfiguration()
            .MinimumLevel
            .Warning()
            .WriteTo.RollingFile(formatter, logFilePath, fileSizeLimitBytes: 5000000)
            .CreateLogger();
        }
    }
}
