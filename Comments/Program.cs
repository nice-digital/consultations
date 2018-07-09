using System;
using Comments.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Comments
{
	public class Program
    {
        public static void Main(string[] args)
        {
			var host = BuildWebHost(args);


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

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
