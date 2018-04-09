using Comments.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.SQLite;
using System.Net.Http;
using Comments.Services;
using Microsoft.AspNetCore.Builder;
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

        protected readonly Feed FeedToUse = Feed.ConsultationCommentsListDetailMulitpleDoc;
        protected readonly bool _authenticated = true;
        protected readonly string _displayName = "Benjamin Button";
        protected readonly Guid? _userId = Guid.Empty;
        protected readonly IUserService _fakeUserService;

        public TestBase(Feed feed) : this()
        {
            FeedToUse = feed;
            _fakeUserService = FakeUserService.Get(_authenticated, _displayName, _userId);
        }
        public TestBase(Feed feed, bool authenticated, string displayName = null, Guid? userId = null) : this()
        {
            FeedToUse = feed;
            _authenticated = authenticated;
            _displayName = displayName;
            _userId = userId;
            _fakeUserService = FakeUserService.Get(_authenticated, _displayName, _userId);
        }

        public TestBase()
        {
            // Arrange
            _fakeUserService = FakeUserService.Get(_authenticated, _displayName, _userId);
            var databaseName = DatabaseName + Guid.NewGuid();

            //SQLiteConnectionStringBuilder sqLiteConnectionStringBuilder = new SQLiteConnectionStringBuilder()
            //{
            //    DataSource = "my.db",

            //    // from doc : Determines how GUIDs are stored. 
            //    // If true - GUID columns are stored in binary form, 
            //    // If false - GUID columns are stored as text
            //    BinaryGUID = true
            //};
            
            //using (System.Data.IDbConnection db = new Microsoft.Data.Sqlite.SqliteConnection(sqLiteConnectionStringBuilder))
            //{
            //    var result = connection.Query<MyObject>("the query", theFilter());
            //}

            //var connection = new SqliteConnection(sqLiteConnectionStringBuilder.ConnectionString); //"Data Source=" + DatabaseName + ";"); //"BinaryGuid=False"); //Version=3;

            _options = new DbContextOptionsBuilder<ConsultationsContext>()
                    .UseInMemoryDatabase(databaseName)
                    //.UseSqlite(connection)
                    .Options;

            var builder = new WebHostBuilder()
                .UseContentRoot("../../../../Comments")
                .ConfigureServices(services =>
                {
                    services.AddEntityFrameworkSqlite();
                    services.AddDbContext<ConsultationsContext>(options =>
                        options.UseInMemoryDatabase(databaseName)
                        //options.UseSqlite(connection)
                        );
                    services.TryAddSingleton<ISeriLogger, FakeSerilogger>();
                    services.TryAddSingleton<IAuthenticateService, FakeAuthenticateService>();
                    services.TryAddTransient<IUserService>(provider => _fakeUserService);
                    services.TryAddTransient<IFeedReaderService>(provider => new FeedReader(FeedToUse));;
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
                ApiKey = "api key goes here",
                BasePath = new Uri("http://test-indev.nice.org.uk"),
                Chapter = "consultation-comments/{0}/document/{1}/chapter-slug/{2}",
                Detail = "consultation-comments/{0}",
                List = "consultation-comments-list"
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
        protected int AddLocation(string sourceURI)
        {
            var location = new Location(sourceURI, null, null, null, null, null, null, null, null);
            using (var context = new ConsultationsContext(_options, _fakeUserService))
            {
                context.Location.Add(location);
                context.SaveChanges();
            }
            return location.LocationId;
        }
        protected int AddComment(int locationId, string commentText, bool isDeleted, Guid createdByUserId)
        {
            var comment = new Comment(locationId, createdByUserId, commentText, Guid.Empty, location: null);
            comment.IsDeleted = isDeleted;
            using (var context = new ConsultationsContext(_options, _fakeUserService))
            {
                context.Comment.Add(comment);
                context.SaveChanges();
            }
            return comment.CommentId;
        }
        protected int AddQuestionType(string description, bool hasBooleanAnswer, bool hasTextAnswer, int questionTypeId = 1)
        {
            var questionType = new QuestionType(description, hasTextAnswer, hasBooleanAnswer, null);
            using (var context = new ConsultationsContext(_options, _fakeUserService))
            {
                context.QuestionType.Add(questionType);
                context.SaveChanges();
            }
            return questionType.QuestionTypeId;
        }
        protected int AddQuestion(int locationId, int questionTypeId, string questionText, int questionId = 1)
        {
            var question = new Question(locationId, questionText, questionTypeId, null, null, null, null);
            using (var context = new ConsultationsContext(_options, _fakeUserService))
            {
                context.Question.Add(question);
                context.SaveChanges();
            }
            return question.QuestionId;
        }
        protected int AddAnswer(int questionId, Guid userId, string answerText)
        {
            var answer = new Answer(questionId, userId, answerText, null, null);
            using (var context = new ConsultationsContext(_options, _fakeUserService))
            {
                answer.LastModifiedDate = DateTime.Now;
                context.Answer.Add(answer);
                context.SaveChanges();
            }
            return answer.AnswerId;
        }
        protected void AddCommentsAndQuestionsAndAnswers(string sourceURI, string commentText, string questionText, string answerText, Guid createdByUserId)
        {
            var locationId = AddLocation(sourceURI);
            AddComment(locationId, commentText, isDeleted: false, createdByUserId: createdByUserId);
            var questionTypeId = AddQuestionType(description: "text", hasBooleanAnswer: false, hasTextAnswer: true);
            var questionId = AddQuestion(locationId, questionTypeId, questionText);
            AddAnswer(questionId, createdByUserId, answerText);
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
