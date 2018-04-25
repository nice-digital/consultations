using System;
using Comments.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Comments
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();

            //var host = BuildWebHost(args);
            //host.Run();

            //using (var scope = host.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;

            //    try
            //    {
            //        var context = services.GetService<ConsultationsContext>();
            //        if (context.GetService<IRelationalDatabaseCreator>().Exists())
            //        {
            //            context.Database.ExecuteSqlCommand(@"
            //                CREATE TABLE IF NOT EXISTS ""__EFMigrationsHistory"" (
            //                    ""MigrationId"" TEXT NOT NULL CONSTRAINT ""PK___EFMigrationsHistory"" PRIMARY KEY,
            //                    ""ProductVersion"" TEXT NOT NULL
            //                );

            //                INSERT OR IGNORE INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
            //                VALUES ('20180420104648_InitialMigration', '2.0.2');        
            //            ");
            //        }
            //        context.Database.Migrate();
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e);  //TODO: Logging
            //        throw;
            //    }
            //}

            //host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}