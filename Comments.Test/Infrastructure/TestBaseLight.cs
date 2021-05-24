using Comments.Configuration;
using Comments.Models;
using Comments.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using NICE.Identity.Authentication.Sdk.TokenStore;

namespace Comments.Test.Infrastructure
{
	/// <summary>
	/// This is the sugar-free, diet version of TestBase.
	/// </summary>
	public class TestBaseLight
	{

		protected static (TestServer testServer, HttpClient httpClient) InitialiseServerAndClient(ConsultationsContext dbContext, IUserService fakeUserService = null, IConsultationService fakeConsultationService = null)
		{
			AppSettings.AuthenticationConfig = new AuthenticationConfig { ClientId = "test client id", AuthorisationServiceUri = "http://www.example.com" };
			AppSettings.GlobalNavConfig = new GlobalNavConfig { CookieBannerScript = "//a-fake-cookiebannerscript-url" };

			var builder = new WebHostBuilder()
				.UseContentRoot("../../../../Comments")
				.ConfigureServices(services =>
				{
					services.AddEntityFrameworkSqlite();

					services.TryAddSingleton<ConsultationsContext>(dbContext);
					services.TryAddSingleton<IApiTokenStore, FakeApiTokenStore>();

					if (fakeUserService != null)
					{
						services.TryAddTransient<IUserService>(provider => fakeUserService);
					}

					if (fakeConsultationService != null)
					{
						services.TryAddTransient<IConsultationService>(provider => fakeConsultationService);
					}

					services.AddControllersWithViews(opt => opt.Filters.Add(new AllowAnonymousFilter()));
					services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();
				})
				.Configure(app =>
				{
					app.UseStaticFiles();

					app.Use((context, next) =>
					{
						var httpRequestFeature = context.Features.Get<IHttpRequestFeature>();

						if (httpRequestFeature != null && string.IsNullOrEmpty(httpRequestFeature.RawTarget))
							httpRequestFeature.RawTarget = httpRequestFeature.Path;

						return next();
					});

				})
				.UseEnvironment("Production")
				.UseStartup(typeof(Startup));
			var server = new TestServer(builder);

			return (testServer: server, httpClient: server.CreateClient());
		}

		protected static DbContextOptions<ConsultationsContext> GetContextOptions()
		{
			var databaseName = "ConsultationsDB" + Guid.NewGuid();
			return new DbContextOptionsBuilder<ConsultationsContext>()
				.UseInMemoryDatabase(databaseName)
				.EnableSensitiveDataLogging()
				.Options;
		}

	}
}
