using Comments.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Comments.Common;
using Comments.Configuration;
using Comments.Migrations;
using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NICE.Feeds;
using NICE.Feeds.Tests.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.FeatureManagement;
using NICE.Feeds.Models.Indev.List;
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
        protected readonly IFeatureManager _fakeFeatureManager;
        protected readonly ISessionManager _fakeSessionManager;
		protected readonly IConsultationService _consultationService;
        protected readonly DbContextOptionsBuilder<ConsultationsContext> _contextOptions;

        protected readonly ConsultationsContext _context;
	    protected readonly bool _useRealSubmitService = false;

	    protected IEncryption _fakeEncryption;

	    protected IUrlHelper _urlHelper;

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

	    public TestBase(TestUserType testUserType, Feed feed, IList<SubmittedCommentsAndAnswerCount> submittedCommentsAndAnswerCounts = null, bool bypassAuthentication = true,
		    bool addRoleClaim = true, bool enableOrganisationalCommentingFeature = false)
		    : this(false, testUserType, true, submittedCommentsAndAnswerCounts, bypassAuthentication, addRoleClaim, enableOrganisationalCommentingFeature)
	    {
			FeedToUse = feed;
		}

		/// <summary>
		/// this is the default constructor, albeit with a load of optional parameters.
		/// </summary>
		public TestBase(bool useRealSubmitService = false, TestUserType testUserType = TestUserType.Authenticated, bool useFakeConsultationService = false, IList<SubmittedCommentsAndAnswerCount> submittedCommentsAndAnswerCounts = null,
			bool bypassAuthentication = true, bool addRoleClaim = true, bool enableOrganisationalCommentingFeature = false, Dictionary<int, Guid> validSessions = null,
			bool useRealHttpContextAccessor = false, bool useRealUserService = false, int? organisationIdUserIsLeadOf = null)
        {
	        if (testUserType == TestUserType.NotAuthenticated)
	        {
		        _authenticated = false;
	        }
			AppSettings.AuthenticationConfig = new AuthenticationConfig{ ClientId = "test client id", AuthorisationServiceUri = "http://www.example.com"};
			AppSettings.GlobalNavConfig = new GlobalNavConfig {CookieBannerScript = "//a-fake-cookiebannerscript-url"};
			// Arrange
			_urlHelper = new FakeUrlHelper();
			_fakeHttpContextAccessor = FakeHttpContextAccessor.Get(_authenticated, _displayName, _userId, testUserType, addRoleClaim);
			_fakeApiService = new FakeAPIService();
			if (useRealUserService)
			{
				_fakeUserService = new UserService(_fakeHttpContextAccessor, _fakeApiService);
			}
			else
			{
				_fakeUserService = FakeUserService.Get(_authenticated, _displayName, _userId, testUserType, addRoleClaim, organisationIdUserIsLeadOf: organisationIdUserIsLeadOf);
			}
			_consultationService = new FakeConsultationService();
	        _useRealSubmitService = useRealSubmitService;
	        _fakeEncryption = new FakeEncryption();
			var featureDictionary = new Dictionary<string, bool> { { Constants.Features.OrganisationalCommenting, enableOrganisationalCommentingFeature } };
			_fakeFeatureManager = new FakeFeatureManager(featureDictionary);
	        _fakeSessionManager = new FakeSessionManager(featureDictionary);

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

            if (validSessions != null)
            {
	            AddSessions(ref _context, validSessions);
            }

			var builder = new WebHostBuilder()
                .UseContentRoot("../../../../Comments")
                .ConfigureServices(services =>
                {
                    services.AddEntityFrameworkSqlite();

					services.TryAddSingleton<ConsultationsContext>(_context);
                    services.TryAddSingleton<ISeriLogger, FakeSerilogger>();
                    if (!useRealHttpContextAccessor)
                    {
	                    services.TryAddSingleton<IHttpContextAccessor>(provider => _fakeHttpContextAccessor);
                    }
                    if (!useRealUserService)
                    {
	                    services.TryAddTransient<IUserService>(provider => _fakeUserService);
					}
                    services.TryAddTransient<IFeedReaderService>(provider => new FeedReader(FeedToUse));
                    services.TryAddScoped<IAPIService>(provider => _fakeApiService);
					
					services.AddSingleton<IFeatureManager>(provider => _fakeFeatureManager);
					services.AddSingleton<ISessionManager>(provider => _fakeSessionManager);
					
					if (!_useRealSubmitService)
	                {
						services.TryAddTransient<ISubmitService>(provider => new FakeSubmitService());
					}
	                if (useFakeConsultationService)
	                {
		                services.TryAddTransient<IConsultationService>(provider => _consultationService);
	                }

	                if (bypassAuthentication)
	                {
		                services.AddMvc(opt => opt.Filters.Add(new AllowAnonymousFilter())); //bypass authentication
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

		private void AddSessions(ref ConsultationsContext context, Dictionary<int, Guid> validSessions)
		{
			foreach (var session in validSessions)
			{
				var sourceURI = ConsultationsUri.CreateConsultationURI(session.Key);

				var location = new Location(sourceURI, null, null, null, null, null, null, null, null, null, null);
				context.Location.Add(location);
				context.SaveChanges();

				var organisationAuthorisation = new OrganisationAuthorisation("Carl Spackler", DateTime.UtcNow, 1, location.LocationId, "123412341234");
				context.OrganisationAuthorisation.Add(organisationAuthorisation);
				context.SaveChanges();

				var organisationUser = new OrganisationUser(organisationAuthorisation.OrganisationAuthorisationId, session.Value, DateTime.MaxValue);
				context.OrganisationUser.Add(organisationUser);
				context.SaveChanges();
			}
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
        public int AddLocation(string sourceURI, ConsultationsContext passedInContext = null, string order = "0")
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
		protected int AddComment(int locationId, string commentText, string createdByUserId, int status = (int)StatusName.Draft, ConsultationsContext passedInContext = null, int? organisationUserId = null, int? parentCommentId = null, int? organisationId = null)
        {
            var comment = new Comment(locationId, createdByUserId, commentText, Guid.Empty.ToString(), location: null, statusId: status, status: null, organisationUserId, parentCommentId, organisationId);
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
            var questionType = new QuestionType(description, hasTextAnswer, hasBooleanAnswer, null) { QuestionTypeId = questionTypeId };
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
        protected int AddQuestion(int locationId, int questionTypeId, string questionText, ConsultationsContext passedInContext = null, string createdByUserId = null)
        {
            var question = new Question(locationId, questionText, questionTypeId, null, null, null);
            question.CreatedByUserId = createdByUserId ?? Guid.Empty.ToString();
			question.LastModifiedByUserId = createdByUserId ?? Guid.Empty.ToString();
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
        protected int AddAnswer(int questionId, string userId, string answerText, int status = (int)StatusName.Draft, ConsultationsContext passedInContext = null, int? organisationUserId = null, int? parentAnswerId = null)
        {
            var answer = new Answer(questionId, userId, answerText, null, null, status, null, organisationUserId, parentAnswerId);
            answer.LastModifiedDate = DateTime.Now;
            if (passedInContext != null)
            {
                passedInContext.Answer.Add(answer);
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
            AddComment(locationId, commentText, createdByUserId: createdByUserId, passedInContext: passedInContext, status: status);
			var questionTypeId = 99;
            var questionId = AddQuestion(locationId, questionTypeId, questionText, passedInContext);
            AddAnswer(questionId, createdByUserId, answerText, status, passedInContext);
        }

        protected int SetupTestDataInDB()
        {
            var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var answerText = Guid.NewGuid().ToString();
            var commentText = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid().ToString();

			var locationId = AddLocation(sourceURI);
			AddComment(locationId, commentText, createdByUserId: userId);
			var questionTypeId = 1;
			AddQuestionType("Text question", false, true, questionTypeId);
			var questionId = AddQuestion(locationId, questionTypeId, questionText);
			AddAnswer(questionId, userId, answerText);
			AddStatus("Draft", 1);
			return questionId;
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
		    var commentId = AddComment(locationId, commentText, createdByUserId: createdByUserId, status: (int)StatusName.Submitted, passedInContext: passedInContext);
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
		    var commentId = AddComment(locationId, commentText, createdByUserId: createdByUserId, status: (int)StatusName.Submitted, passedInContext: passedInContext);
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

	    protected List<ConsultationList> AddConsultationsToList()
	    {
		    var consultationList = new List<ConsultationList>();
		    consultationList.Add(new ConsultationList
		    {
			    ConsultationId = 123,
			    ConsultationName = "open consultation",
			    StartDate = DateTime.Now.AddDays(-1),
			    EndDate = DateTime.Now.AddDays(1),
			    Reference = "GID-1",
			    Title = "Consultation title 1",
			    AllowedRole = "ConsultationListTestRole",
			    FirstConvertedDocumentId = null,
			    FirstChapterSlugOfFirstConvertedDocument = null
		    });
		    consultationList.Add(new ConsultationList()
		    {
			    ConsultationId = 124,
			    ConsultationName = "closed consultation",
			    StartDate = DateTime.Now.AddDays(-3),
			    EndDate = DateTime.Now.AddDays(-2),
			    Reference = "GID-2",
			    Title = "Consultation Title 1",
			    AllowedRole = "ConsultationListTestRole",
			    FirstConvertedDocumentId = null,
			    FirstChapterSlugOfFirstConvertedDocument = null
		    });
		    consultationList.Add(new ConsultationList()
		    {
			    ConsultationId = 125,
			    ConsultationName = "upcoming consultation",
			    StartDate = DateTime.Now.AddDays(3),
			    EndDate = DateTime.Now.AddDays(5),
			    Reference = "GID-3",
			    Title = "Consultation Title 3",
			    AllowedRole = "Some other role",
			    FirstConvertedDocumentId = 1,
			    FirstChapterSlugOfFirstConvertedDocument = "my-chapter-slug"
		    });

		    return consultationList;
	    }


	    protected IUserService GetFakeUserService(string userId = null, TestUserType testUserType = TestUserType.Administrator)
	    {
		    return FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId ?? Guid.NewGuid().ToString(), testUserType: testUserType);
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
