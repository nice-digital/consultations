using Comments.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using Comments.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NICE.Auth.NetCore.Services;
using NICE.Feeds;
using NICE.Feeds.Configuration;
using NICE.Feeds.Tests.Infrastructure;
using Microsoft.Data.Sqlite;

namespace Comments.Test.Infrastructure
{
    public class TestBase
    {
        protected const string DatabaseName = "testDB";
        protected readonly DbContextOptions<ConsultationsContext> _options;

        protected readonly TestServer _server;
        protected readonly HttpClient _client;
        protected IFeedConfig _feedConfig;

        protected Feed FeedToUse = Feed.ConsultationCommentsPublishedDetailMulitpleDoc;
        protected readonly bool _authenticated = true;
        protected readonly string _displayName = "Benjamin Button";
        protected readonly Guid? _userId = Guid.Empty;
        protected readonly IUserService _fakeUserService;
        protected readonly IHttpContextAccessor _fakeHttpContextAccessor;


        protected readonly DbContextOptionsBuilder<ConsultationsContext> _contextOptions;

        protected readonly ConsultationsContext _context;

        public TestBase(Feed feed) : this()
        {
            FeedToUse = feed;
            _fakeUserService = FakeUserService.Get(_authenticated, _displayName, _userId);
        }
        public TestBase(Feed feed, bool authenticated, Guid userId, string displayName = null) : this()
        {
            FeedToUse = feed;
            _authenticated = authenticated;
            _displayName = displayName;
            _userId = Guid.Empty;
            _fakeUserService = FakeUserService.Get(_authenticated, _displayName, _userId);
        }

	    public TestBase(bool authenticated, Guid? userId = null, string displayName = null) : this()
	    {
			_authenticated = authenticated;
		    _displayName = displayName;
		    _userId = userId;
		    _fakeUserService = FakeUserService.Get(_authenticated, _displayName, _userId);
		}

		public TestBase()
        {
            // Arrange
            _fakeUserService = FakeUserService.Get(_authenticated, _displayName, _userId);
            _fakeHttpContextAccessor = FakeHttpContextAccessor.Get(_authenticated, _displayName, _userId);
            var databaseName = DatabaseName + Guid.NewGuid();

            //SQLiteConnectionStringBuilder sqLiteConnectionStringBuilder = new SQLiteConnectionStringBuilder()
            //{	       
            //    DataSource = "my.db",
            //};

        // var connection = new SqliteConnection(sqLiteConnectionStringBuilder.ConnectionString); //"Data Source=" + DatabaseName + ";"); //"BinaryGuid=False"); //Version=3;
        var connection = new SqliteConnection("DataSource=:memory:");

            connection.Open();

            _options = new DbContextOptionsBuilder<ConsultationsContext>()
                    .UseSqlite(connection)
                    //.UseInMemoryDatabase(databaseName)
                    .Options;

            _context = new ConsultationsContext(_options, _fakeUserService);
            _context.Database.EnsureCreatedAsync();

            var builder = new WebHostBuilder()
                .UseContentRoot("../../../../Comments")
                .ConfigureServices(services =>
                {
                    services.AddEntityFrameworkSqlite();

                    services.TryAddSingleton<ConsultationsContext>(_context);
                    services.TryAddSingleton<ISeriLogger, FakeSerilogger>();
                    services.TryAddSingleton<IAuthenticateService, FakeAuthenticateService>();
                    services.TryAddSingleton<IHttpContextAccessor>(provider => _fakeHttpContextAccessor);
                    services.TryAddTransient<IUserService>(provider => _fakeUserService);
                    services.TryAddTransient<IFeedReaderService>(provider => new FeedReader(FeedToUse));
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
            _server = new TestServer(builder);
            _client = _server.CreateClient();

            _feedConfig = new FeedConfig()
            {
                AppCacheTimeSeconds = 30,
                IndevApiKey = "api key goes here",
                IndevBasePath = new Uri("http://test-indev.nice.org.uk"),
                IndevPublishedChapterFeedPath = "consultation-comments/{0}/document/{1}/chapter-slug/{2}",
                IndevPublishedDetailFeedPath = "consultation-comments/{0}",
                IndevListFeedPath = "consultation-comments-list"
            };
        }

        #region database stuff

        protected void ResetDatabase()
        {
            using (var context = new ConsultationsContext(_options, _fakeUserService))
            {
                context.Database.EnsureDeleted();
                //context.Database.CloseConnection();
                //context.Database.OpenConnection();
            }
        }

        protected void ResetDatabase(IUserService userService)
        {
            using (var context = new ConsultationsContext(_options, userService))
            {
                context.Database.EnsureDeleted();
                //context.Database.CloseConnection();
                //context.Database.OpenConnection();
            }
        }
        protected int AddLocation(string sourceURI, ConsultationsContext passedInContext = null)
        {
            var location = new Location(sourceURI, null, null, null, null, null, null, null, null);
            if (passedInContext != null)
            {
                passedInContext.Location.Add(location);
                passedInContext.SaveChanges();
            }
            else
            {
                using (var context =new ConsultationsContext(_options, _fakeUserService))
                {
                    context.Location.Add(location);
                    context.SaveChanges();
                }
            }

            return location.LocationId;
        }
        protected int AddComment(int locationId, string commentText, bool isDeleted, Guid createdByUserId, ConsultationsContext passedInContext = null)
        {
            var comment = new Comment(locationId, createdByUserId, commentText, Guid.Empty, location: null);
            comment.IsDeleted = isDeleted;
            if (passedInContext != null)
            {
                passedInContext.Comment.Add(comment);
                passedInContext.SaveChanges();
            }
            else
            {
                using (var context = new ConsultationsContext(_options, _fakeUserService))
                {
                    context.Comment.Add(comment);
                    context.SaveChanges();
                }
            }

            return comment.CommentId;
        }
        protected int AddQuestionType(string description, bool hasBooleanAnswer, bool hasTextAnswer, int questionTypeId = 1, ConsultationsContext passedInContext = null)
        {
            var questionType = new QuestionType(description, hasTextAnswer, hasBooleanAnswer, null);
            if (passedInContext != null)
            {
                passedInContext.QuestionType.Add(questionType);
                passedInContext.SaveChanges();
            }
            else
            {
                using (var context = new ConsultationsContext(_options, _fakeUserService))
                {
                    context.QuestionType.Add(questionType);
                    context.SaveChanges();
                }
            }

            return questionType.QuestionTypeId;
        }
        protected int AddQuestion(int locationId, int questionTypeId, string questionText, ConsultationsContext passedInContext = null)
        {
            var question = new Question(locationId, questionText, questionTypeId, null, null, null, null);
            if (passedInContext != null)
            {
                passedInContext.Question.Add(question);
                passedInContext.SaveChanges();
            }
            else
            {
                using (var context = new ConsultationsContext(_options, _fakeUserService))
                {
                    context.Question.Add(question);
                    context.SaveChanges();
                }
            }

            return question.QuestionId;
        }
        protected int AddAnswer(int questionId, Guid userId, string answerText, ConsultationsContext passedInContext = null)
        {
            var answer = new Answer(questionId, userId, answerText, null, null);
            answer.LastModifiedDate = DateTime.Now;
            if (passedInContext != null)
            {
                var a = passedInContext.Answer.Add(answer);
                passedInContext.SaveChanges();
            }
            else
            {
                using (var context = new ConsultationsContext(_options, _fakeUserService))
                {
                    context.Answer.Add(answer);
                    context.SaveChanges();
                }
            }
            
            return answer.AnswerId;
        }
        protected void AddCommentsAndQuestionsAndAnswers(string sourceURI, string commentText, string questionText, string answerText, Guid createdByUserId, ConsultationsContext passedInContext = null)
        {
            var locationId = AddLocation(sourceURI, passedInContext);
            AddComment(locationId, commentText, isDeleted: false, createdByUserId: createdByUserId, passedInContext: passedInContext);
            var questionTypeId = AddQuestionType(description: "text", hasBooleanAnswer: false, hasTextAnswer: true, passedInContext: passedInContext);
            var questionId = AddQuestion(locationId, questionTypeId, questionText, passedInContext);
            AddAnswer(questionId, createdByUserId, answerText, passedInContext);
        }

        protected void SetupTestDataInDB()
        {
            var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var answerText = Guid.NewGuid().ToString();
            var commentText = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid();

            AddCommentsAndQuestionsAndAnswers(sourceURI, commentText, questionText, answerText, userId); //Add records for Foreign key constraints
        }

        #endregion database stuff

        #region Helpers

        protected int RandomNumber()
        {
            var rnd = new Random();
            return rnd.Next(1, int.MaxValue);
        }

        #endregion Helpers
    }

    internal class FakeSerilogger : ISeriLogger
    {
        public void Configure(ILoggerFactory loggerFactory, IConfiguration configuration, IApplicationLifetime appLifetime,
            IHostingEnvironment env)
        {}
    }
}
