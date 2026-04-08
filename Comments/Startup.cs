using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Comments.Common;
using Comments.Configuration;
using Comments.Export;
using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using NICE.Feeds;
using NICE.Feeds.Indev;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authentication.Sdk.Extensions;
using ConsultationsContext = Comments.Models.ConsultationsContext;

namespace Comments
{
    public class Startup
    {
        private const string CorsPolicyName = "CorsPolicy";

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

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
            services.TryAddTransient<IRemoteSystemReader>(ServiceProvider =>
                new RemoteSystemReader(apiTokenClient: ServiceProvider.GetRequiredService<IApiTokenClient>()));
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

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new ResponseCacheAttribute() { NoStore = true, Location = ResponseCacheLocation.None });
                options.EnableEndpointRouting = false;
            })
                .AddNewtonsoftJson();

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


            /*if (!Environment.IsDevelopment())
            {
                services.AddHttpsRedirection(options =>
                {
                    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                    options.HttpsPort = 443;
                });
            }
            */
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
                options.KnownProxies.Clear();
            });

            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName,
                    builder => builder.WithOrigins(AppSettings.Environment.CorsOrigin)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.AddOptions();
            if (!Environment.IsIntegrationTest())
            {
                services.AddHttpClient("ssr", client =>
                {
                    client.BaseAddress = new Uri("http://localhost:4000");
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [Obsolete("the reason for the obselete flag here is UseSpaPrerendering has been marked as obselete in 3.1 and dropped in 5.x")]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IHostApplicationLifetime appLifetime, IUrlHelperFactory urlHelperFactory, IFeatureManager featureManager, LinkGenerator linkGenerator)
        {
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "ClientApp", "build")),
                RequestPath = "/consultations"
            });
            app.Use(async (context, next) =>
            {
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Add("Permissions-Policy", "interest-cohort=()");
                    return Task.FromResult(0);
                });
                await next();
            }
            );

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseExceptionHandler(Constants.ErrorPath);

            }
            else
            {
                app.UseExceptionHandler(Constants.ErrorPath);

                app.UseStatusCodePagesWithReExecute(Constants.ErrorPath + "/{0}");
            }

            app.UseCors(CorsPolicyName);

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

            app.UseRouting();

            app.UseForwardedHeaders();
            app.UseAuthentication();
            app.UseAuthorization();

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

            /*if (!env.IsDevelopment() && !env.IsIntegrationTest())
            {
                app.UseHttpsRedirection();
            }
            */

            app.UseEndpoints(endpoints =>
            {
                if (!env.IsIntegrationTest())
                {
                    endpoints.Map("{*path:nonfile}", async context =>
                    {
                        var httpClientFactory = context.RequestServices.GetRequiredService<IHttpClientFactory>();
                        var linkGenerator = context.RequestServices.GetRequiredService<LinkGenerator>();

                        var client = httpClientFactory.CreateClient("ssr");

                        var htmlTemplate = await File.ReadAllTextAsync("ClientApp/build/index.html");

                        try
                        {
                            var payload = new
                            {
                                url = context.Request.Path.ToString(),
                                origin = $"{context.Request.Scheme}://{context.Request.Host}",
                                data = SsrDataBuilder.Build(context, htmlTemplate, linkGenerator)
                            };

                            var response = await client.PostAsJsonAsync("/render", payload);

                            if (!response.IsSuccessStatusCode)
                                throw new Exception("SSR returned non-success");

                            var result = await response.Content.ReadFromJsonAsync<SsrResult>();

                            if (result == null || string.IsNullOrEmpty(result.Html))
                                throw new Exception("Invalid SSR response");

                            context.Response.StatusCode = result.StatusCode;
                            context.Response.ContentType = "text/html";
                            await context.Response.WriteAsync(result.Html);
                        }
                        catch (Exception ex)
                        {
                            var logger = context.RequestServices.GetRequiredService<ILogger<Startup>>();
                            logger.LogError(ex, "SSR failed, falling back to static HTML");

                            context.Response.StatusCode = 200;
                            context.Response.ContentType = "text/html";
                            await context.Response.WriteAsync(htmlTemplate);
                        }
                    });
                }

                endpoints.MapControllerRoute(name: "PublishedRedirectWithoutDocument",
                                                             pattern: "consultations/{consultationId:int}",
                                                             defaults: new { controller = "Redirect", action = "PublishedRedirectWithoutDocument" });

                endpoints.MapControllerRoute(name: "PublishedRedirect",
                                             pattern: "consultations/{consultationId:int}/{documentId:int}",
                                             defaults: new { controller = "Redirect", action = "PublishedDocumentWithoutChapter" });

                endpoints.MapControllerRoute(name: "PreviewRedirect",
                                             pattern: "consultations/preview/{reference}/consultation/{consultationId:int}/document/{documentId:int}",
                                             defaults: new { controller = "Redirect", action = "PreviewDocumentWithoutChapter" });

                endpoints.MapControllerRoute(name: "default",
                                             pattern: "{controller}/{action=Index}/{id?}");

                // endpoints.MapHealthChecks("/health"); //TODO: replace the custom health check controller with this package, which is now supported since the upgrade.


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


            app.UseStaticFiles();

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
