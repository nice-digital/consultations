using Comments.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Comments.Configuration;
using Comments.Migrations;
using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NICE.Feeds;
using NICE.Feeds.Tests.Infrastructure;
using Microsoft.Data.Sqlite;
using NICE.Identity.Authentication.Sdk.API;
using Answer = Comments.Models.Answer;
using Comment = Comments.Models.Comment;
using FeedConfig = NICE.Feeds.Configuration.FeedConfig;
using Location = Comments.Models.Location;
using Question = Comments.Models.Question;
using QuestionType = Comments.Models.QuestionType;
using Status = Comments.Models.Status;

namespace Comments.Test.Infrastructure
{
	public enum TestUserType
	{
		Administrator,
		IndevUser,
		Authenticated,
		NotAuthenticated,
		CustomFictionalRole,
		ConsultationListTestRole
	}

    public class TestBase
    {
        protected const string DatabaseName = "testDB";
        protected readonly DbContextOptions<ConsultationsContext> _options;

        protected readonly TestServer _server;
        protected readonly HttpClient _client;
        protected FeedConfig _feedConfig;

        protected Feed FeedToUse = Feed.ConsultationCommentsPublishedDetailMulitpleDoc;
        protected readonly bool _authenticated = true;
        protected readonly string _displayName = "Benjamin Button";
        protected readonly string _userId = Guid.Empty.ToString();
        protected readonly IUserService _fakeUserService;
        protected readonly IHttpContextAccessor _fakeHttpContextAccessor;
        protected readonly IAPIService _fakeApiService;

	    protected readonly IConsultationService _consultationService;
        protected readonly DbContextOptionsBuilder<ConsultationsContext> _contextOptions;

        protected readonly ConsultationsContext _context;
	    protected readonly bool _useRealSubmitService = false;

	    protected IEncryption _fakeEncryption;

		public TestBase(Feed feed) : this()
        {
            FeedToUse = feed;
            _fakeUserService = FakeUserService.Get(_authenticated, _displayName, _userId);
	        _consultationService = new FakeConsultationService();
		}
        public TestBase(Feed feed, bool authenticated, string userId, string displayName = null) : this()
        {
            FeedToUse = feed;
            _authenticated = authenticated;
            _displayName = displayName;
            _userId = Guid.Empty.ToString();
            _fakeUserService = FakeUserService.Get(_authenticated, _displayName, _userId);
	        _consultationService = new FakeConsultationService();
		}

	    public TestBase(bool authenticated, string userId = null, string displayName = null) : this()
	    {
			_authenticated = authenticated;
		    _displayName = displayName;
		    _userId = userId;
		    _fakeUserService = FakeUserService.Get(_authenticated, _displayName, _userId);
		}

	    public TestBase(TestUserType testUserType, Feed feed, IList<SubmittedCommentsAndAnswerCount> submittedCommentsAndAnswerCounts = null) : this(false, testUserType, true, submittedCommentsAndAnswerCounts)
	    {
			FeedToUse = feed;
		}

		public TestBase(bool useRealSubmitService = false, TestUserType testUserType = TestUserType.Authenticated, bool useFakeConsultationService = false, IList<SubmittedCommentsAndAnswerCount> submittedCommentsAndAnswerCounts = null)
        {
			AppSettings.AuthenticationConfig = new AuthenticationConfig{ ClientId = "test client id"};
            // Arrange
            _fakeUserService = FakeUserService.Get(_authenticated, _displayName, _userId, testUserType);
			_fakeHttpContextAccessor = FakeHttpContextAccessor.Get(_authenticated, _displayName, _userId, testUserType);
			_fakeApiService = new FakeAPIService();
			_consultationService = new FakeConsultationService();
	        _useRealSubmitService = useRealSubmitService;
	        _fakeEncryption = new FakeEncryption();
			var databaseName = DatabaseName + Guid.NewGuid();

			_options = new DbContextOptionsBuilder<ConsultationsContext>()
					.UseInMemoryDatabase(databaseName)
                    .Options;

	        if (submittedCommentsAndAnswerCounts != null)
	        {
		        _context = new ConsultationListContext(_options, _fakeUserService, _fakeEncryption, submittedCommentsAndAnswerCounts);
	        }
	        else
	        {
				_context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption);
			}

            _context.Database.EnsureCreatedAsync();

			var builder = new WebHostBuilder()
                .UseContentRoot("../../../../Comments")
                .ConfigureServices(services =>
                {
                    services.AddEntityFrameworkSqlite();

                    services.TryAddSingleton<ConsultationsContext>(_context);
                    services.TryAddSingleton<ISeriLogger, FakeSerilogger>();
                    services.TryAddSingleton<IHttpContextAccessor>(provider => _fakeHttpContextAccessor);
                    services.TryAddTransient<IUserService>(provider => _fakeUserService);
                    services.TryAddTransient<IFeedReaderService>(provider => new FeedReader(FeedToUse));

					if (!_useRealSubmitService)
	                {
						services.TryAddTransient<ISubmitService>(provider => new FakeSubmitService());
					}
	                if (useFakeConsultationService)
	                {
		                services.TryAddTransient<IConsultationService>(provider => _consultationService);
	                }
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
	            IndevDraftPreviewChapterFeedPath = "preview/{0}/consultation/{1}/document/{2}/chapter-slug/{3}",
				IndevPublishedDetailFeedPath = "consultation-comments/{0}",
                IndevListFeedPath = "consultation-comments-list"
            };
        }

        #region database stuff

        protected void ResetDatabase()
        {
            using (var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
            {
                context.Database.EnsureDeleted();
			}
        }

        protected void ResetDatabase(IUserService userService)
        {
            using (var context = new ConsultationsContext(_options, userService, _fakeEncryption))
            {
                context.Database.EnsureDeleted();
			}
        }
        protected int AddLocation(string sourceURI, ConsultationsContext passedInContext = null, string order = "0")
        {
            var location = new Location(sourceURI, null, null, null, null, null, null, order, null, null, null);
            if (passedInContext != null)
            {
                passedInContext.Location.Add(location);
                passedInContext.SaveChanges();
            }
            else
            {
                using (var context =new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
                {
                    context.Location.Add(location);
                    context.SaveChanges();
                }
            }

            return location.LocationId;
        }

	    protected int AddStatus(string statusName, int statusIdId = (int)StatusName.Draft, ConsultationsContext passedInContext = null)
	    {
		    var statusModel = new Models.Status("Draft", null, null);
			if (passedInContext != null)
		    {
				passedInContext.Status.Add(statusModel);
				passedInContext.SaveChanges();
		    }
		    else
		    {
			    using (var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
			    {
					context.Status.Add(statusModel);
					context.SaveChanges();
			    }
		    }

		    return statusModel.StatusId;
	    }
		protected int AddComment(int locationId, string commentText, bool isDeleted, string createdByUserId, int status = (int)StatusName.Draft, ConsultationsContext passedInContext = null)
        {
            var comment = new Comment(locationId, createdByUserId, commentText, null, location: null, statusId: status, status: null);
            comment.IsDeleted = isDeleted;
            if (passedInContext != null)
            {
                passedInContext.Comment.Add(comment);
                passedInContext.SaveChanges();
            }
            else
            {
                using (var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
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
                using (var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
                {
                    context.QuestionType.Add(questionType);
                    context.SaveChanges();
                }
            }

            return questionType.QuestionTypeId;
        }
        protected int AddQuestion(int locationId, int questionTypeId, string questionText, ConsultationsContext passedInContext = null)
        {
            var question = new Question(locationId, questionText, questionTypeId, null, null, null);
            if (passedInContext != null)
            {
                passedInContext.Question.Add(question);
                passedInContext.SaveChanges();
            }
            else
            {
                using (var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
                {
                    context.Question.Add(question);
                    context.SaveChanges();
                }
            }

            return question.QuestionId;
        }
        protected int AddAnswer(int questionId, string userId, string answerText, int status = (int)StatusName.Draft, ConsultationsContext passedInContext = null)
        {
            var answer = new Answer(questionId, userId, answerText, null, null, status, null);
            answer.LastModifiedDate = DateTime.Now;
            if (passedInContext != null)
            {
                var a = passedInContext.Answer.Add(answer);
                passedInContext.SaveChanges();
            }
            else
            {
                using (var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
                {
                    context.Answer.Add(answer);
                    context.SaveChanges();
                }
            }

            return answer.AnswerId;
        }
        protected void AddCommentsAndQuestionsAndAnswers(string sourceURI, string commentText, string questionText, string answerText, string createdByUserId, int status = (int)StatusName.Draft, ConsultationsContext passedInContext = null)
        {
            var locationId = AddLocation(sourceURI, passedInContext);
            AddComment(locationId, commentText, isDeleted: false, createdByUserId: createdByUserId, passedInContext: passedInContext, status: status);
			var questionTypeId = 99;
            var questionId = AddQuestion(locationId, questionTypeId, questionText, passedInContext);
            AddAnswer(questionId, createdByUserId, answerText, status, passedInContext);
        }

        protected void SetupTestDataInDB()
        {
            var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var answerText = Guid.NewGuid().ToString();
            var commentText = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid().ToString();

			var locationId = AddLocation(sourceURI);
			AddComment(locationId, commentText, isDeleted: false, createdByUserId: userId);
			var questionTypeId = 99;
			var questionId = AddQuestion(locationId, questionTypeId, questionText);
			AddAnswer(questionId, userId, answerText);
        }

		protected Question GetQuestion()
		{
			using (var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
			{
				return context.Question.FirstOrDefault();
			}
		}

	    protected void AddSubmittedCommentsAndAnswers(string sourceURI, string commentText, string questionText, string answerText, string createdByUserId, ConsultationsContext passedInContext = null)
	    {
		    var locationId = AddLocation(sourceURI, passedInContext);
		    var commentId = AddComment(locationId, commentText, isDeleted: false, createdByUserId: createdByUserId, status: (int)StatusName.Submitted, passedInContext: passedInContext);
			var questionTypeId = 99;
		    var questionId = AddQuestion(locationId, questionTypeId, questionText, passedInContext);
		    var answerId = AddAnswer(questionId, createdByUserId, answerText, (int)StatusName.Submitted, passedInContext);
			var submissionId = AddSubmission(createdByUserId, passedInContext);
		    AddSubmissionComments(submissionId, commentId, passedInContext);
		    AddSubmissionAnswers(submissionId, answerId, passedInContext);
	    }

	    protected void AddSubmittedComments(string sourceURI, string commentText, string questionText, string answerText, string createdByUserId, ConsultationsContext passedInContext = null)
	    {
		    var locationId = AddLocation(sourceURI, passedInContext);
		    var commentId = AddComment(locationId, commentText, isDeleted: false, createdByUserId: createdByUserId, status: (int)StatusName.Submitted, passedInContext: passedInContext);
			var questionTypeId = 99;
		    var questionId = AddQuestion(locationId, questionTypeId, questionText, passedInContext);
		    var submissionId = AddSubmission(createdByUserId, passedInContext);
		    AddSubmissionComments(submissionId, commentId, passedInContext);
	    }

	    protected void AddSubmittedQuestionsWithAnswers(string sourceURI, string commentText, string questionText, string answerText, string createdByUserId, ConsultationsContext passedInContext = null)
	    {
		    var locationId = AddLocation(sourceURI, passedInContext);
			var questionTypeId = 99;
		    var questionId = AddQuestion(locationId, questionTypeId, questionText, passedInContext);
		    var answerId = AddAnswer(questionId, createdByUserId, answerText, (int)StatusName.Submitted, passedInContext);
		    var submissionId = AddSubmission(createdByUserId, passedInContext);
		    AddSubmissionAnswers(submissionId, answerId, passedInContext);
	    }

		protected int AddSubmission(string userId, ConsultationsContext passedInContext = null, bool? organisationExpressionOfInterest = null)
	    {
			var submission = new Models.Submission(userId, DateTime.Now, false, null, false, null, organisationExpressionOfInterest);
			if (passedInContext != null)
			{
				passedInContext.Submission.Add(submission);
				passedInContext.SaveChanges();
			}
			else
			{
				using (var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
				{
					context.Submission.Add(submission);
					context.SaveChanges();
				}
			}

			return submission.SubmissionId;
		}

	    protected int AddSubmissionComments(int submissionId, int commentId, ConsultationsContext passedInContext = null)
	    {
		    var submissionComment = new SubmissionComment(submissionId, commentId);
		    if (passedInContext != null)
		    {
			    passedInContext.SubmissionComment.Add(submissionComment);
			    passedInContext.SaveChanges();
		    }
		    else
		    {
			    using (var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
			    {
				    context.SubmissionComment.Add(submissionComment);
				    context.SaveChanges();
			    }
		    }

		    return submissionComment.SubmissionCommentId;
	    }

	    protected int AddSubmissionAnswers(int submissionId, int answerId, ConsultationsContext passedInContext = null)
	    {
		    var submissionAnswer = new SubmissionAnswer(submissionId, answerId);
		    if (passedInContext != null)
		    {
			    passedInContext.SubmissionAnswer.Add(submissionAnswer);
			    passedInContext.SaveChanges();
		    }
		    else
		    {
			    using (var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
			    {
				    context.SubmissionAnswer.Add(submissionAnswer);
				    context.SaveChanges();
			    }
		    }

		    return submissionAnswer.SubmissionAnswerId;
	    }

	    protected ConsultationListContext CreateContext(IUserService userService, int totalCount = 1)
	    {
		    var consultationListContext = new ConsultationListContext(_options, userService, _fakeEncryption,
			    new List<SubmittedCommentsAndAnswerCount>
			    {
				    new SubmittedCommentsAndAnswerCount
				    {
					    SourceURI = "consultations://./consultation/1",
					    TotalCount = totalCount
				    }
			    });
		    return consultationListContext;
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
