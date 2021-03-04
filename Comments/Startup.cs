using Comments.Common;
using Comments.Configuration;
using Comments.Export;
using Comments.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using NICE.Feeds;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authentication.Sdk.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Comments.ViewModels;
using NICE.Feeds.Configuration;
using NICE.Feeds.Indev;
using ConsultationsContext = Comments.Models.ConsultationsContext;

namespace Comments
{
	public class Startup
    {
        ILogger _logger;

        public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> logger)
        {
            Configuration = configuration;
            Environment = env;
            _logger = logger;
        }
        
        public IConfiguration Configuration { get; }

        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
			if (Environment.IsDevelopment())
            {
                AppSettings.Configure(services, Configuration, @"c:\"); 
            }
            else
            {
                AppSettings.Configure(services, Configuration, Environment.ContentRootPath);
            }

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
			services.TryAddSingleton<ISeriLogger, SeriLogger>();
			services.AddHttpClient();

			services.TryAddTransient<IUserService, UserService>();

			var contextOptionsBuilder = new DbContextOptionsBuilder<ConsultationsContext>();
            services.TryAddSingleton<IDbContextOptionsBuilderInfrastructure>(contextOptionsBuilder);

            services.AddDbContext<ConsultationsContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.TryAddTransient<ICommentService, CommentService>();
            services.TryAddTransient<IConsultationService, ConsultationService>();

            // Add authentication before adding the FeedReaderService
            var authConfiguration = AppSettings.AuthenticationConfig.GetAuthConfiguration();
            services.AddAuthentication(authConfiguration, allowNonSecureCookie: Environment.IsDevelopment())
	            .AddScheme<OrganisationCookieAuthenticationOptions, OrganisationCookieAuthenticationHandler>(OrganisationCookieAuthenticationOptions.DefaultScheme, options => { });
            services.AddAuthorisation(authConfiguration);

            services.AddFeatureManagement();

			services.TryAddSingleton<IIndevFeedConfig>(provider => AppSettings.Feed);
			services.TryAddTransient<ICacheService, MemoryCacheService>();
			services.TryAddTransient<IIndevFeedReaderService, IndevFeedReaderService>();
			services.TryAddTransient<IRemoteSystemReader, RemoteSystemReader>();
			services.TryAddTransient<IIndevFeedService, IndevFeedService>();

			services.TryAddTransient<IAnswerService, AnswerService>();
            services.TryAddTransient<IQuestionService, QuestionService>();
	        services.TryAddTransient<ISubmitService, SubmitService>();
			services.TryAddTransient<IAdminService, AdminService>();
	        services.TryAddTransient<IExportService, ExportService>();
			services.TryAddSingleton<IEncryption, Encryption>();
	        services.TryAddTransient<IExportToExcel, ExportToExcel>();
	        services.TryAddTransient<IStatusService, StatusService>();
			services.TryAddTransient<IConsultationListService, ConsultationListService>();
			services.TryAddTransient<IOrganisationService, OrganisationService>();

			services.AddRouting(options => options.LowercaseUrls = true);

			services.AddMvc(options =>
            {
                options.Filters.Add(new ResponseCacheAttribute() { NoStore = true, Location = ResponseCacheLocation.None });
            }); //.SetCompatibilityVersion(CompatibilityVersion.Version_2_2); 

            // In production, static files are served from the pre-built files, rather than proxied via react dev server
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

			// Uncomment this if you want to debug server node
			//if (Environment.IsDevelopment())
			//{
			//    services.AddNodeServices(options =>
			//    {
			//        options.LaunchWithDebugging = true;
			//        options.DebuggingPort = 9229;
			//    });
			//}

			//if (!Environment.IsDevelopment()) //this breaks the tests.
			//{
			//    services.Configure<MvcOptions>(options =>
			//    {
			//        options.Filters.Add(new RequireHttpsAttribute());
			//    });
			//}


	        if (!Environment.IsDevelopment())
	        {
		        services.AddHttpsRedirection(options =>
		        {
			        options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
			        options.HttpsPort = 443;
		        });
	        }

	        services.Configure<ForwardedHeadersOptions>(options =>
			{
				options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
				options.KnownProxies.Clear();
			});

			services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            }); //adding CORS for Warren. todo: maybe move this into the isDevelopment block..
            
            services.AddOptions();

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ISeriLogger seriLogger, IApplicationLifetime appLifetime, IUrlHelperFactory urlHelperFactory, IFeatureManager featureManager)
        {           
            seriLogger.Configure(loggerFactory, Configuration, appLifetime, env);
            var startupLogger = loggerFactory.CreateLogger<Startup>();
            startupLogger.LogWarning("Consultations starting up");


			if (env.IsDevelopment())
            {
	            app.UseDeveloperExceptionPage();
				app.UseExceptionHandler(Constants.ErrorPath);
				loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();

				app.UseStaticFiles(); //uses the wwwroot folder, only for dev. on other service the root is varnish
			}
            else
            {
	            app.UseExceptionHandler(Constants.ErrorPath);

	            app.UseStatusCodePagesWithReExecute(Constants.ErrorPath + "/{0}");
			}

			app.UseCors("CorsPolicy");

            // Because in dev mode we proxy to a react dev server (which has to run in the root e.g. http://localhost:3000)
            // we re-write paths for static files to map them to the root
            if (env.IsDevelopment())
            {
                app.Use((context, next) =>
                {
                    var reqPath = context.Request.Path;
                    if (reqPath.HasValue && reqPath.Value.Contains("."))
                    {
						// Map static files paths to the root, for use within the 
						//if (reqPath.Value.Contains("/consultations"))
						//{
						//	context.Request.Path = reqPath.Value.Replace("/consultations", "");
						//}
						//else if (reqPath.Value.IndexOf("favicon.ico", StringComparison.OrdinalIgnoreCase) == -1 && reqPath.Value.IndexOf("hot-update", StringComparison.OrdinalIgnoreCase) == -1)
						//{
						//	context.Response.StatusCode = 404;
						//	throw new FileNotFoundException($"Path {reqPath.Value} could not be found. Did you mean to load '/consultations{context.Request.Path.Value  }' instead?");
						//}
					}

					return next();
                });
            }

	        app.UseForwardedHeaders();
            app.UseAuthentication();

            app.Use(async (context, next) => 
            {
				//this middleware is here because we have some controller api's that don't have the authorise attribute set. e.g. CommentsController. that controller still needs to work.
				//without authentication. for authenticated users, the default scheme is used. however, we now have 2 schemes which can be used together (idam and organisation cookie).
				//so this middleware combines the multiple authentication schemes we have, setting a single user.
				//it then update the context user values in the DbContext so that the global query filters work.
				var principal = new ClaimsPrincipal();

	            var cookieAuthResult = await context.AuthenticateAsync(OrganisationCookieAuthenticationOptions.DefaultScheme);
	            if (cookieAuthResult?.Principal != null)
	            {
		            principal.AddIdentities(cookieAuthResult.Principal.Identities);
	            }
	            var accountsAuthResult = await context.AuthenticateAsync(AuthenticationConstants.AuthenticationScheme);
	            if (accountsAuthResult?.Principal != null)
	            {
		            principal.AddIdentities(accountsAuthResult.Principal.Identities);
	            }
	            context.User = principal;

	            var consultationsContext = context.RequestServices.GetService<ConsultationsContext>();
	            consultationsContext.ConfigureContext();

				await next();
            });

			app.UseSpaStaticFiles(new StaticFileOptions { RequestPath = "/consultations" });

		    if (!env.IsDevelopment() && !env.IsIntegrationTest())
		    {
			    app.UseHttpsRedirection();
		    }

		    IRouteBuilder defaultRoute = null;
			app.UseMvc(routes =>
            {
                defaultRoute = routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");

	            routes.MapRoute(
		            name: "PublishedRedirectWithoutDocument",
		            template: "consultations/{consultationId:int}",
		            defaults: new { controller = "Redirect", action = "PublishedRedirectWithoutDocument" });

				routes.MapRoute(
		            name: "PublishedRedirect",
		            template: "consultations/{consultationId:int}/{documentId:int}",
		            defaults: new {controller = "Redirect", action = "PublishedDocumentWithoutChapter"});

	            routes.MapRoute(
		            name: "PreviewRedirect",
		            template: "consultations/preview/{reference}/consultation/{consultationId:int}/document/{documentId:int}",
		            defaults: new { controller = "Redirect", action = "PreviewDocumentWithoutChapter" });

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
            app.Use((context, next) =>
            {
                var httpRequestFeature = context.Features.Get<IHttpRequestFeature>();

                if (httpRequestFeature != null && string.IsNullOrEmpty(httpRequestFeature.RawTarget))
                    httpRequestFeature.RawTarget = httpRequestFeature.Path;

                return next();
            });


			app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                spa.UseSpaPrerendering(options =>
                {
                    options.ExcludeUrls = new[] { "/sockjs-node" };
                    // Pass data in from .NET into the SSR. These come through as `params` within `createServerRenderer` within the server side JS code.
                    // See https://docs.microsoft.com/en-us/aspnet/core/spa/angular?tabs=visual-studio#pass-data-from-net-code-into-typescript-code
                    options.SupplyData = async (httpContext, data) =>
                    {
                        data["isHttpsRequest"] = httpContext.Request.IsHttps;
                        var cookiesForSSR = httpContext.Request.Cookies.Where(cookie => cookie.Key.StartsWith(AuthenticationConstants.CookieName) || cookie.Key.StartsWith(Constants.SessionCookieName)).ToList();
                        if (cookiesForSSR.Any())
                        {
                            data["cookies"] = $"{string.Join("; ", cookiesForSSR.Select(cookie => $"{cookie.Key}={cookie.Value}"))};";
                        }
						var user = new User(httpContext.User);
						var isAuthorised = user.IsAuthenticatedByAccounts;
						if (!isAuthorised)
						{
							var pathNoQuery = httpContext.Request.GetUri().AbsolutePath.StripConsultationsFromPath();
							if (ConsultationsUri.IsDocumentPageRelativeUrl(pathNoQuery) || ConsultationsUri.IsReviewPageRelativeUrl(pathNoQuery))
							{
								var consultationUriParts = ConsultationsUri.ParseRelativeUrl(pathNoQuery);
								isAuthorised = user.IsAuthorisedByConsultationId(consultationUriParts.ConsultationId);
							}
						}
						data["isAuthorised"] = isAuthorised;
						data["displayName"] = user.DisplayName;

						var host = httpContext.Request.Host.Host;
						var userRoles = httpContext.User?.Roles(host).ToList() ?? new List<string>();

						var isAdminUser = userRoles.Any(role => AppSettings.ConsultationListConfig.DownloadRoles.AdminRoles.Contains(role));
						var teamRoles = userRoles.Where(role => AppSettings.ConsultationListConfig.DownloadRoles.TeamRoles.Contains(role)).Select(role => role).ToList();
						var isTeamUser = !isAdminUser && teamRoles.Any(); //an admin with team roles is still just considered an admin.
						data["isAdminUser"] = isAdminUser;
						data["isTeamUser"] = isTeamUser;

						var actionContext = new ActionContext {
							HttpContext = httpContext,
							RouteData = new RouteData { Routers = { defaultRoute.Build() } },
							ActionDescriptor = new ActionDescriptor(),
						};
						var urlHelper = urlHelperFactory.GetUrlHelper(actionContext);

						data["signInURL"] = urlHelper.Action(Constants.Auth.LoginAction, Constants.Auth.ControllerName, new { returnUrl = httpContext.Request.Path });
						data["signOutURL"] = urlHelper.Action(Constants.Auth.LogoutAction, Constants.Auth.ControllerName); //auth0 needs logout urls configured. it won't let you redirect dynamically.
						data["registerURL"] = urlHelper.Action(Constants.Auth.LoginAction, Constants.Auth.ControllerName, new { returnUrl = httpContext.Request.Path, goToRegisterPage = true });
						data["requestURL"] = httpContext.Request.Path;
	                    data["accountsEnvironment"] = AppSettings.Environment.AccountsEnvironment;

						data["OrganisationalCommentingFeature"] = await featureManager.IsEnabledAsync(Constants.Features.OrganisationalCommenting);

						//data["user"] = context.User; - possible security implications here, surfacing claims to the front end. might be ok, if just server-side.
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
                   // spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

            //try
            //{
            //    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            //    {
            //         serviceScope.ServiceProvider.GetService<ConsultationsContext>().Database.Migrate();
            //    }
            //}
            //catch(Exception ex)
            //{
            //    startupLogger.LogError(String.Format("EF Migrations Error: {0}", ex));
            //}
		}
    }
}
