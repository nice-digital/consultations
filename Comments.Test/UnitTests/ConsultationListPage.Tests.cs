using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using Comments.Common;
using Comments.Configuration;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using Newtonsoft.Json;
using NICE.Feeds.Models.Indev.List;
using NICE.Feeds.Tests.Infrastructure;
using Shouldly;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;

namespace Comments.Test.UnitTests
{
	public class ConsultationListPageTests : TestBase
	{
		private readonly ConsultationsContext _consultationListContext;

		public ConsultationListPageTests() : base(TestUserType.Administrator, Feed.ConsultationCommentsPublishedDetailMulitpleDoc)
		{
			AppSettings.ConsultationListConfig = TestAppSettings.GetConsultationListConfig();
			AppSettings.Feed.IndevBasePath = new Uri("http://www.nice.org.uk");
			_consultationListContext = new ConsultationListContext(_options, _fakeUserService, _fakeEncryption);
		}

#region earlier tests

		[Fact]
		public void ConsultationListPageModel_HasConsultationsPopulated()
		{
			//Arrange
			var consultationListService = GetConsultationListService();

			//Act
			ConsultationListViewModel viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null) { Status = new List<ConsultationStatus>() }).consultationListViewModel;

			//Assert
			viewModel.Consultations.OrderBy(c => c.ConsultationId).First().ConsultationId.ShouldBe(123);
		}

		[Fact]
		public void ConsultationListPageModel_HasCorrectlySetResponseCount()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var userId = Guid.NewGuid().ToString();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var context = CreateContext(userService);
			AddSubmittedCommentsAndAnswers(sourceURI, "Comment Label", "Question Label", "Answer Label", userId, context);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(context, new FakeFeedService(consultationList), new FakeConsultationService(), GetFakeUserService());
			//Act
			var viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null) { Status = new List<ConsultationStatus>() });

			//Assert
			viewModel.consultationListViewModel.Consultations.First().SubmissionCount.ShouldBe(1);
		}

		[Fact]
		public void ConsultationListPageModel_HasCorrectlySetResponseCountWithMulitpleSubmissions()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var userId = Guid.NewGuid().ToString();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var context = CreateContext(userService, 2);
			AddSubmittedCommentsAndAnswers(sourceURI, "Comment Label", "Question Label", "Answer Label", userId, context);
			AddSubmittedCommentsAndAnswers(sourceURI, "Second Comment Label", "Second Question Label", " Second Answer Label", userId, context);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(context, new FakeFeedService(consultationList), new FakeConsultationService(), GetFakeUserService());

			//Act
			var viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null) { Status = new List<ConsultationStatus>() });

			//Assert
			viewModel.consultationListViewModel.Consultations.First().SubmissionCount.ShouldBe(2);
		}


		[Fact]
		public void ConsultationListPageModel_HasCorrectlySetResponseCountWithComments()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var userId = Guid.NewGuid().ToString();
			var context = CreateContext(_fakeUserService);
			var locationId = AddLocation(sourceURI, context);
			var commentId = AddComment(locationId, "Just a comment", false, userId, 2, context);
			var submissionId = AddSubmission(userId, context);
			AddSubmissionComments(submissionId, commentId, context);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(context, new FakeFeedService(consultationList), new FakeConsultationService(), GetFakeUserService());

			//Act
			var viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null) { Status = new List<ConsultationStatus>() });

			//Assert
			viewModel.consultationListViewModel.Consultations.First().SubmissionCount.ShouldBe(1);
		}



		[Fact]
		public void ConsultationListPageModel_HasCorrectlySetResponseCountWithAnswers()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var userId = Guid.NewGuid().ToString();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var context =  CreateContext(userService);

			var locationId = AddLocation(sourceURI, context);
			var questionTypeId = 99;
			var questionId = AddQuestion(locationId, questionTypeId, "Question Text", context);
			var AnswerId = AddAnswer(questionId, userId, "my answer", 2, context);
			var submissionId = AddSubmission(userId, context);
			AddSubmissionAnswers(submissionId, AnswerId, context);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(context, new FakeFeedService(consultationList), new FakeConsultationService(), GetFakeUserService());

			//Act
			var viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null) { Status = new List<ConsultationStatus>() });

			//Assert
			viewModel.consultationListViewModel.Consultations.First().SubmissionCount.ShouldBe(1);
		}
		
		[Fact]
		public void ConsultationListPageModel_HasDocumentIdAndChapterSlugPopulatedCorrectly()
		{
			//Arrange
			var consultationList = AddConsultationsToList();
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), new FakeConsultationService(), GetFakeUserService());

			//Act
			ConsultationListViewModel viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null) { Status = new List<ConsultationStatus>() }).consultationListViewModel;

			//Assert
			viewModel.Consultations.First().DocumentId.ShouldBe(1);
			viewModel.Consultations.First().ChapterSlug.ShouldBe("my-chapter-slug");
		}

		[Fact]
		public void ConsultationListPageModel_HasFilterOptions()
		{
			//Arrange
			var consultationList = AddConsultationsToList();
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), new FakeConsultationService(), GetFakeUserService());

			//Act
			ConsultationListViewModel viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null) {Status = new List<ConsultationStatus>()}).consultationListViewModel;

			//Assert
			var firstFilter = viewModel.OptionFilters.First();
			firstFilter.Title.ShouldBe("Status");
			firstFilter.Options.Count.ShouldBe(3);
			firstFilter.Options.First().Label.ShouldBe("Open");
			firstFilter.Options.Skip(1).First().Label.ShouldBe("Closed");
			firstFilter.Options.Skip(2).First().Label.ShouldBe("Upcoming");
		}

		[Fact]
		public void ConsultationListPageModel_HasFilterOptionOpenSetToSelected()
		{
			//Arrange
			var consultationList = AddConsultationsToList();
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), new FakeConsultationService(), GetFakeUserService());
			var viewModel = new ConsultationListViewModel(null, null, null, null, null);
			viewModel.Status = new List<ConsultationStatus>(){ ConsultationStatus.Open };

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel).consultationListViewModel;

			//Assert
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Open").IsSelected.ShouldBeTrue();
		}

		[Fact]
		public void ConsultationListPageModel_HasFilterOptionClosedSetToSelected()
		{
			//Arrange
			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 123 });
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), new FakeConsultationService(), GetFakeUserService());
			var viewModel = new ConsultationListViewModel(null, null, null, null, null);
			viewModel.Status = new List<ConsultationStatus>() { ConsultationStatus.Closed };

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel).consultationListViewModel;

			//Assert
			updatedViewModel.OptionFilters.First().Options.Skip(1).First(f => f.Id == "Closed").IsSelected.ShouldBeTrue();
		}

		[Fact]
		public void ConsultationListPageModel_HasFilterOptionUpcomingSetToSelected()
		{
			//Arrange
			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 123 });
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), new FakeConsultationService(), GetFakeUserService());
			var viewModel = new ConsultationListViewModel(null, null, null, null, null);
			viewModel.Status = new List<ConsultationStatus>() { ConsultationStatus.Upcoming };

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel).consultationListViewModel;

			//Assert
			updatedViewModel.OptionFilters.First().Options.Skip(1).First(f => f.Id == "Upcoming").IsSelected.ShouldBeTrue();
		}

		[Fact]
		public void ConsultationListPageModel_WithFilterOptionOpenHasCorrectResultCounts()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Status = new List<ConsultationStatus> {ConsultationStatus.Open}
			};

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel).consultationListViewModel;

			//Assert
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Open").FilteredResultCount.ShouldBe(1);
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Open").UnfilteredResultCount.ShouldBe(3);
		}


		[Fact]
		public void ConsultationListPageModel_WithFilterOptionClosedHasCorrectResultCounts()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Status = new List<ConsultationStatus> { ConsultationStatus.Closed }
			};

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel).consultationListViewModel;

			//Assert
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Closed").FilteredResultCount.ShouldBe(1);
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Closed").UnfilteredResultCount.ShouldBe(3);
		}

		[Fact]
		public void ConsultationListPageModel_WithFilterOptionUpcomingHasCorrectResultCounts()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Status = new List<ConsultationStatus> { ConsultationStatus.Upcoming }
			};

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel).consultationListViewModel;

			//Assert
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Upcoming").FilteredResultCount.ShouldBe(1);
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Upcoming").UnfilteredResultCount.ShouldBe(3);
		}

		[Fact]
		public void ConsultationListPageModel_WithFilterOptionOpenAndClosedHasCorrectResultCounts()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Status = new List<ConsultationStatus> { ConsultationStatus.Closed, ConsultationStatus.Open, ConsultationStatus.Upcoming }
			};

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel).consultationListViewModel;

			//Assert
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Open").FilteredResultCount.ShouldBe(1);
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Open").UnfilteredResultCount.ShouldBe(3);
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Closed").FilteredResultCount.ShouldBe(1);
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Closed").UnfilteredResultCount.ShouldBe(3);
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Upcoming").FilteredResultCount.ShouldBe(1);
			updatedViewModel.OptionFilters.First().Options.First(f => f.Id == "Upcoming").UnfilteredResultCount.ShouldBe(3);
		}

		[Fact]
   		public void ConsultationListPageModel_WithFilterReferenceTextSetToValue()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Keyword = "GID-1"
			};

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel).consultationListViewModel;

			//Assert
			updatedViewModel.TextFilter.FilteredResultCount.ShouldBe(1);
			updatedViewModel.TextFilter.UnfilteredResultCount.ShouldBe(3);
		}

		[Fact]
		public void ConsultationListPageModel_WithFilterReferenceTextSetToPartialValue()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Keyword = "gid"
			};

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel).consultationListViewModel;

			//Assert
			updatedViewModel.TextFilter.FilteredResultCount.ShouldBe(3);
			updatedViewModel.TextFilter.UnfilteredResultCount.ShouldBe(3);
		}

		[Theory]
		[InlineData(null)]
		[InlineData(" ")]
		[InlineData("")]
		public void ConsultationListPageModel_WithFilterReferenceTextSetToNull(string reference)
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Keyword = reference
			};

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel).consultationListViewModel;

			//Assert
			updatedViewModel.TextFilter.IsSelected.ShouldBeFalse();
		}

		[Fact]
		public void ConsultationListPageModel_WithFilterReferenceTextSetToPartialTitle()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Keyword = "consultation title"
			};

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel).consultationListViewModel;

			//Assert
			updatedViewModel.TextFilter.FilteredResultCount.ShouldBe(3);
			updatedViewModel.TextFilter.UnfilteredResultCount.ShouldBe(3);
		}

		[Fact]
		public void ConsultationListPageModel_WithFilterReferenceTextSetToSpecificTitle()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Keyword = "consultation title 3"
			};

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel).consultationListViewModel;

			//Assert
			updatedViewModel.TextFilter.FilteredResultCount.ShouldBe(1);
			updatedViewModel.TextFilter.UnfilteredResultCount.ShouldBe(3);
		}

		[Fact]
		public void ConsultationListPageModel_FilterDataByKeyword()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Keyword = "consultation title 3"
			};

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel);

			//Assert
			updatedViewModel.consultationListViewModel.Consultations.Count(c => c.Show).ShouldBe(1);
		}

		[Fact]
		public void ConsultationListPageModel_FilterDataByOpenStatus()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Status = new List<ConsultationStatus> { ConsultationStatus.Open }
			};

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel);

			//Assert
			updatedViewModel.consultationListViewModel.Consultations.Count(c => c.Show).ShouldBe(1);
		}

		[Fact]
		public void ConsultationListPageModel_FilterDataByOpenandUpcomingStatus()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Status = new List<ConsultationStatus> { ConsultationStatus.Upcoming, ConsultationStatus.Open }
			};

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel);

			//Assert
			updatedViewModel.consultationListViewModel.Consultations.Count(c => c.Show).ShouldBe(2);
		}

		[Fact]
		public void ConsultationListPageModel_FilterDataByOpenStatusAndKeyword()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Keyword = "consultation title 1",
				Status = new List<ConsultationStatus> { ConsultationStatus.Open }
			};

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel);

			//Assert
			updatedViewModel.consultationListViewModel.Consultations.Count(c => c.Show).ShouldBe(1);
		}

		[Fact]
		public void ConsultationListPageModel_FilterDataByGidReferenceKeyword()
		{
			//Arrange
			var consultationListService = GetConsultationListService();
			var viewModel = new ConsultationListViewModel(null, null, null, null, null)
			{
				Keyword = "GID-1"
			};

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel);

			//Assert
			updatedViewModel.consultationListViewModel.Consultations.Count(c => c.Show).ShouldBe(1);
		}

		private List<ConsultationList> AddConsultationsToList()
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

		private IUserService GetFakeUserService(string userId = null, TestUserType testUserType = TestUserType.Administrator)
		{
			return FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId ?? Guid.NewGuid().ToString(), testUserType: testUserType);
		}

		private ConsultationListService GetConsultationListService()
		{
			var consultationList = AddConsultationsToList();
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), new FakeConsultationService(), GetFakeUserService());
			return consultationListService;
		}

		[Theory]
		[InlineData(TestUserType.CustomFictionalRole)]
		[InlineData(TestUserType.Administrator)]
		public void ConsultationListServiceConstructorWorksWithCorrectSecurity(TestUserType testUserType)
		{
			//Arrange
			var consultationList = AddConsultationsToList();
			var userId = Guid.NewGuid().ToString();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId, testUserType: testUserType);
			var consultationContext = new ConsultationsContext(_options, userService, _fakeEncryption);

			//Act 
			var consultationService = new ConsultationListService(consultationContext, new FakeFeedService(consultationList), new FakeConsultationService(), _fakeUserService);

			//Assert
			consultationService.ShouldNotBeNull();
		}

		#endregion earlier tests


		[Fact]
		public void CHTETeamMemberSeeAllConsultationsWhenTheFilterIsUnchecked()
		{
			//Arrange
			var consultationList = AddConsultationsToList();
			var userService = FakeUserService.Get(true, "Jeffrey Goines", Guid.NewGuid().ToString(), TestUserType.ConsultationListTestRole);
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), new FakeConsultationService(), userService);

			//Act
			ConsultationListViewModel viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null) { Status = new List<ConsultationStatus>() }).consultationListViewModel;

			//Assert
			viewModel.Consultations.Count().ShouldBe(3);
			viewModel.Consultations.Count(c => c.Show).ShouldBe(3);
		}

		[Fact]
		public void CHTETeamMemberCanOnlySeeTheirOwnConsultationsWhenTheFilterIsChecked()
		{
			//Arrange
			var consultationList = AddConsultationsToList();
			var userService = FakeUserService.Get(true, "Jeffrey Goines", Guid.Empty.ToString(), TestUserType.ConsultationListTestRole);
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), new FakeConsultationService(), userService);

			//Act
			ConsultationListViewModel viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null)
			{
				Status = new List<ConsultationStatus>(),
				Team = new List<TeamStatus>() { TeamStatus.MyTeam }
			}).consultationListViewModel;
			var serialisedViewModel = JsonConvert.SerializeObject(viewModel); //doing this in order to validate the filters coming back in the model.

			//Assert
			viewModel.Consultations.Count().ShouldBe(3);
			viewModel.Consultations.Count(c => c.Show).ShouldBe(2);
			serialisedViewModel.ShouldMatchApproved(new Func<string, string>[] { Scrubbers.ScrubStartDate, Scrubbers.ScrubEndDate });
		}

	}

	public class ConsultationListFilterTests : TestBase
	{
		public ConsultationListFilterTests(bool useRealSubmitService = false, TestUserType testUserType = TestUserType.Authenticated, bool useFakeConsultationService = false, IList<SubmittedCommentsAndAnswerCount> submittedCommentsAndAnswerCounts = null, bool bypassAuthentication = true) : base(useRealSubmitService, testUserType, useFakeConsultationService, submittedCommentsAndAnswerCounts, bypassAuthentication)
		{
			AppSettings.ConsultationListConfig = TestAppSettings.GetConsultationListConfig();
			AppSettings.Feed.IndevBasePath = new Uri("http://www.nice.org.uk");
		}

		[Fact]
		public void OnlySeeConsultationsWhereCurrentUserHasCommentedOnWhenFilterIsChecked()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();

			var sourceURIConsultation1 = "consultations://./consultation/1/document/1/chapter/introduction";
			var sourceURIConsultation1DocumentLevel = "consultations://./consultation/1/document/1";
			var sourceURIConsultation2 = "consultations://./consultation/2";
			var userId = Guid.Empty.ToString();

			var locationId1 = AddLocation(sourceURIConsultation1, _context);
			AddComment(locationId1, "the current users comment", false, userId, 2, _context);

			var locationId1Doc = AddLocation(sourceURIConsultation1DocumentLevel, _context);
			AddComment(locationId1Doc, "another comment similar source uri", false, userId, 2, _context);

			var locationId2 = AddLocation(sourceURIConsultation2, _context);
			AddComment(locationId2, "a comment that should be filtered out", false, "not the current user's id", 2, _context);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			consultationList.Add(new ConsultationList { ConsultationId = 2 });

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId, TestUserType.Authenticated);

			var context = new ConsultationsContext(_options, userService, _fakeEncryption);

			var consultationListService = new ConsultationListService(context, new FakeFeedService(consultationList), new FakeConsultationService(), userService);

			//Act
			var viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null, null, null)
			{
				Status = new List<ConsultationStatus>(),
				Contribution = new List<ContributionStatus>() { ContributionStatus.HasContributed }
			});
			var serialisedViewModel = JsonConvert.SerializeObject(viewModel); //saves checking all the properties
			

			//Assert
			viewModel.consultationListViewModel.Consultations.Count().ShouldBe(2);
			viewModel.consultationListViewModel.Consultations.Count(c => c.Show).ShouldBe(1);
			serialisedViewModel.ShouldMatchApproved();
		}
	}
}
