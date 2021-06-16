using System;
using Comments.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;


namespace Comments
{
	public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = SeriLogger.GetLoggerConfiguration().CreateLogger();
            var host = CreateHostBuilder(args).Build();


			using (var scope = host.Services.CreateScope())
		    {
		        var services = scope.ServiceProvider;
		        
			    try
		        {
			        var context = services.GetService<ConsultationsContext>();
			        context.Database.Migrate();
				}
		        catch (Exception e)
		        {
			        var logger = services.GetRequiredService<ILogger<Program>>();
			        logger.LogError(e, "An error occurred while migrating the database.");
				}
		    }
	        host.Run();
		}

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
