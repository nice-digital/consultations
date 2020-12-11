using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Comments.Configuration;
using Comments.Models;
using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.FeatureManagement;
using NICE.Feeds;
using NICE.Feeds.Tests.Infrastructure;
using NICE.Identity.Authentication.Sdk.API;
using Comment = Comments.Models.Comment;
using Location = Comments.Models.Location;

namespace Comments.Test.Infrastructure
{
	/// <summary>
	/// This is the sugar-free, diet version of TestBase.
	/// </summary>
	public class TestBaseLight
	{

		protected static (TestServer testServer, HttpClient httpClient) InitialiseServerAndClient(ConsultationsContext dbContext)
		{
			AppSettings.AuthenticationConfig = new AuthenticationConfig { ClientId = "test client id", AuthorisationServiceUri = "http://www.example.com" };
			AppSettings.GlobalNavConfig = new GlobalNavConfig { CookieBannerScript = "//a-fake-cookiebannerscript-url" };

			var builder = new WebHostBuilder()
				.UseContentRoot("../../../../Comments")
				.ConfigureServices(services =>
				{
					services.AddEntityFrameworkSqlite();

					services.TryAddSingleton<ConsultationsContext>(dbContext);
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

		#region DB functions. similar to the ones in TestBase, but lighter. (they all require a context passed in).

		

		#endregion  DB functions.
	}
}
