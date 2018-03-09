using Comments.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Comments;
using Comments.Configuration;
using Microsoft.Extensions.FileProviders;
using ConsultationsContext = Comments.Models.ConsultationsContext;

namespace comments
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }

        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ConsultationsContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddMvc();

            services.TryAddSingleton<ISeriLogger, SeriLogger>();
            services.TryAddTransient<ICommentService, CommentService>();

            // In production, static files are served from the pre-built files, rather than proxied via react dev server
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            if (Environment.IsDevelopment())
            {
                services.AddNodeServices(options =>
                {
                    options.LaunchWithDebugging = true;
                    options.DebuggingPort = 9229;
                });
            }

            
            services.AddCors(); //adding CORS for Warren. todo: maybe move this into the isDevelopment block..
            services.AddOptions();
            AppSettings.Configure(services, Configuration);
        }

        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ISeriLogger seriLogger, IApplicationLifetime appLifetime)
        {
            seriLogger.Configure(loggerFactory, Configuration, appLifetime, env);
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
            }
            else
            {
                // TODO: Proper error handling URL
                app.UseExceptionHandler("/Home/Error");
            }

            // TODO Which of these paths do we need?
            //app.UsePathBase("/consultations");
           // app.UseStaticFiles(); //this enables the wwwroot folder. 
            app.UseSpaStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "ClientApp/public")),
                RequestPath = ""
            });

            var buildFolder = Path.Combine(env.ContentRootPath, "ClientApp/build");
            //env.IsProduction()
            if (Directory.Exists(buildFolder)) //maybe this should throw errors if it doesn't exist in production...
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(buildFolder),
                    RequestPath = ""
                });
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");

            });

            //// here you can see we make sure it doesn't start with /api, if it does, it'll 404 within .NET if it can't be found
            //app.MapWhen(x => !x.Request.Path.Value.StartsWith("/consultations/api", StringComparison.OrdinalIgnoreCase), builder =>
            //{
            //    builder.UseMvc(routes =>
            //    {
            //        routes.MapSpaFallbackRoute(
            //            name: "spa-fallback",
            //            defaults: new { controller = "Error", action = "Index" });
            //    });
            //});

            // DotNetCore SpaServices requires RawTarget property, which isn't set on a TestServer.
            // So set it here to allow integration tests to work with SSR via SpaServices
            app.Use(async (context, next) =>
            {
                var httpRequestFeature = context.Features.Get<IHttpRequestFeature>();

                if (string.IsNullOrEmpty(httpRequestFeature.RawTarget))
                    httpRequestFeature.RawTarget = httpRequestFeature.Path;

                await next.Invoke();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                
                spa.UseSpaPrerendering(options =>
                {
                    options.ExcludeUrls = new[] { "/sockjs-node" };
                    // Pass data in from .NET into the SSR. These come through as `params` within `createServerRenderer` within the server side JS code.
                    // See https://docs.microsoft.com/en-us/aspnet/core/spa/angular?tabs=visual-studio#pass-data-from-net-code-into-typescript-code
                    options.SupplyData = (context, data) => {
                        data["isHttpsRequest"] = context.Request.IsHttps;
                        // Pass further data in e.g. user/authentication data
                    };
                    options.BootModulePath = $"{spa.Options.SourcePath}/src/server/index.js";
                });

                if (env.IsDevelopment())
                {
                    // Default timeout is 30 seconds so extend it in dev mode because sometimes the react server can take a while to start up
                    spa.Options.StartupTimeout = TimeSpan.FromMinutes(1);

                    // If you have trouble with the react server in dev mode (sometime in can be slow and you get timeout error, then use
                    // `UseProxyToSpaDevelopmentServer` below rather than `UseReactDevelopmentServer`.
                    // This proxies to a manual CRA server (run `npm start` from the ClientApp folder) instead of DotNetCore launching one automatically.
                    // This can be quicker. See https://docs.microsoft.com/en-us/aspnet/core/spa/react?tabs=visual-studio#run-the-cra-server-independently
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
                    //spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}