using System;
using Comments.Export;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NICE.Auth.NetCore.Services;
using NICE.Feeds;
using NICE.Feeds.Tests.Infrastructure;

namespace Comments.Test.Infrastructure
{
	public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
	{
		protected override IWebHostBuilder CreateWebHostBuilder()
		{
			return WebHost.CreateDefaultBuilder(new string[0])
				.UseEnvironment("Development")
				.UseStartup<TestStartup>();
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureTestServices(services =>
			{
				var serviceProvider = new ServiceCollection()
					.AddEntityFrameworkInMemoryDatabase()
					.BuildServiceProvider();

				//services.AddEntityFrameworkSqlite();
				services.TryAddSingleton<IHttpContextAccessor>(provider => FakeHttpContextAccessor.Get(true, "Benjamin Button", Guid.Empty));
				services.TryAddSingleton<ISeriLogger, FakeSerilogger>();
				services.TryAddSingleton<IAuthenticateService, FakeAuthenticateService>();
				services.TryAddTransient<IUserService>(provider => new FakeUserServiceInstance());

				var contextOptionsBuilder = new DbContextOptionsBuilder<ConsultationsContext>();
				services.TryAddSingleton<IDbContextOptionsBuilderInfrastructure>(contextOptionsBuilder);
				services.TryAddTransient<ICommentService, CommentService>();
				services.TryAddTransient<IConsultationService, ConsultationService>();
				services.TryAddTransient<IFeedReaderService>(provider => new FeedReader(Feed.ConsultationCommentsPublishedDetailMulitpleDoc));
				services.TryAddTransient<IFeedService, FeedService>();
				services.TryAddTransient<IAnswerService, AnswerService>();
				services.TryAddTransient<IQuestionService, QuestionService>();
				services.TryAddTransient<ISubmitService>(provider => new FakeSubmitService());
				services.TryAddTransient<IAdminService, AdminService>();
				services.TryAddTransient<IExportService, ExportService>();
				services.TryAddSingleton<IEncryption, Encryption>();
				services.TryAddTransient<IExportToExcel, ExportToExcel>();

				services.AddDbContext<ConsultationsContext>(options =>
				{
					options.UseInMemoryDatabase("testDB" + Guid.NewGuid());
					options.UseInternalServiceProvider(serviceProvider);
				});

				var sp = services.BuildServiceProvider();

				using (var scope = sp.CreateScope())
				{
					var scopedServices = scope.ServiceProvider;
					var context = scopedServices.GetRequiredService<ConsultationsContext>();

					context.Database.EnsureCreated();

					try
					{
						InitializeDbForTests(context);
					}
					catch (Exception ex)
					{
						//ToDo: log error
					}

				}
			}
			);
		}
		public static void InitializeDbForTests(ConsultationsContext db)
		{
			db.Status.Add(new Status
			{
				Comment = null,
				Answer = null,
				Name = "Test3",
				StatusId = 4
			});

			db.SaveChanges();
		}
	}
}
