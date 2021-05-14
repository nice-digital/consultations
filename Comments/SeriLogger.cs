using Microsoft.Extensions.Configuration;
using NICE.Logging;
using NICE.Logging.Sinks.RabbitMQ;
using Serilog;
using System;
using Serilog.Events;

namespace Comments
{
    /// <summary>
    /// This has been refactored for .NET Core 3.1 
    /// 
    /// It now just sets up a LoggerConfiguration object based off appsettings.json (and secrets.json on dev machines)
    /// </summary>
    public static class SeriLogger
    {
        public static LoggerConfiguration GetLoggerConfiguration()
        {
            // Read Logging configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddUserSecrets<Startup>()
                .Build();
            var logCfg = configuration.GetSection("Logging");

            var application = logCfg["Application"];
            var environment = logCfg["Environment"];
            var rabbitMQHost = logCfg["RabbitMQHost"];
            var rabbitMQVHost = logCfg["RabbitMQVHost"];
            var rabbitPortIsSet = int.TryParse(logCfg["RabbitMQPort"], out var rabbitMQPort);
            var rabbitMQUsername = logCfg["RabbitMQUsername"];
            var rabbitMQPassword = logCfg["RabbitMQPassword"];
            var rabbitMQExchangeName = logCfg["RabbitMQExchangeName"];
            var rabbitMQExchangeType = logCfg["RabbitMQExchangeType"];
            var serilogFilePath = logCfg["SerilogFilePath"] ?? logCfg["LogFilePath"];
            Enum.TryParse(logCfg["SerilogMinLevel"], out LogEventLevel serilogMinLevel);
            bool.TryParse(logCfg["UseRabbit"], out var useRabbit);
            bool.TryParse(logCfg["UseFile"], out var useFile);

            var serilogFormatter = new NiceSerilogFormatter(environment, application);
            var serilogConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .MinimumLevel.Is(serilogMinLevel);

            if (useRabbit && !string.IsNullOrEmpty(rabbitMQHost) && rabbitPortIsSet)
            {
                var rabbitCfg = new RabbitMQConfiguration
                {
                    Hostname = rabbitMQHost,
                    VHost = rabbitMQVHost,
                    Port = rabbitMQPort,
                    Username = rabbitMQUsername ?? "",
                    Password = rabbitMQPassword ?? "",
                    Protocol = RabbitMQ.Client.Protocols.AMQP_0_9_1,
                    Exchange = rabbitMQExchangeName,
                    ExchangeType = rabbitMQExchangeType
                };

                // Write logs to RabbitMQ / Kibana
                serilogConfiguration.WriteTo.RabbitMQ(rabbitCfg, serilogFormatter);
            }

            // Write logs to file
            if (useFile)
            {
                serilogConfiguration.WriteTo.RollingFile(serilogFormatter,
                    serilogFilePath,
                    fileSizeLimitBytes: 5000000,
                    retainedFileCountLimit: 5,
                    flushToDiskInterval: TimeSpan.FromSeconds(20));
            }

            return serilogConfiguration;
        }
    }


    //public interface ISeriLogger {
    //    void Configure(ILoggerFactory loggerFactory, IConfiguration configuration, IApplicationLifetime appLifetime, IHostingEnvironment env);
    //}

    //public class SeriLogger : ISeriLogger
    //{
    //    public void Configure(ILoggerFactory loggerFactory, IConfiguration configuration, IApplicationLifetime appLifetime, IHostingEnvironment env)
    //    {
    //        // read appsettings
    //        var logCfg = configuration.GetSection("Logging");

    //        loggerFactory.AddConsole(logCfg); // add provider to send logs to System.Console.WriteLine()
    //        loggerFactory.AddDebug(); // add provider to send logs to System.Diagnostics.Debug.WriteLine()

    //        var rabbitSettingsFound = int.TryParse(logCfg["RabbitMQPort"], out var rPort);
    //        bool.TryParse(logCfg["UseRabbit"], out var useRabbit);
    //        string logFilePath = logCfg["LogFilePath"]; ;

    //        var formatter = new NiceSerilogFormatter(AppSettings.Environment.Name, "Consultations");
    //        var logConfig = new LoggerConfiguration()
    //            .MinimumLevel
    //            .Warning();

    //        if (rabbitSettingsFound && useRabbit)
    //        {
    //            var rHost = logCfg["RabbitMQHost"];
             
    //            var rabbit = new RabbitMQConfiguration {
    //                Hostname = rHost,
    //                Port = rPort,
    //                Protocol = RabbitMQ.Client.Protocols.AMQP_0_9_1,
    //                Exchange = "logging.application.serilog",
    //                ExchangeType = "topic"
    //            };

    //            logConfig.WriteTo.RabbitMQ(rabbit, formatter);
    //        }

    //        bool.TryParse(logCfg["UseFile"], out var useFile);

    //        if (useFile) //probably dev only
    //            logConfig.WriteTo.RollingFile(formatter, logFilePath, fileSizeLimitBytes: 5000000, retainedFileCountLimit: 5, flushToDiskInterval: TimeSpan.FromSeconds(20));

    //        Log.Logger = logConfig.CreateLogger();

    //        // add serilog provider (this is the hook)
    //        loggerFactory.AddSerilog();

    //        // clean up on shutdown
    //        appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);
    //    }
    //}
}
