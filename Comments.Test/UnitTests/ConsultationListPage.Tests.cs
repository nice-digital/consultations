using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using Comments.Common;
using Comments.Configuration;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
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
			_consultationListContext = new ConsultationListContext(_options, _fakeUserService, _fakeEncryption);
		}

		private ConsultationListContext CreateContext(IUserService userService, int totalCount = 1)
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

#region earlier tests

		[Fact]
		public void ConsultationListPageModel_HasConsultationsPopulated()
		{
			//Arrange
			var consultationListService = GetConsultationListService();

			//Act
			ConsultationListViewModel viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null) { Status = new List<ConsultationStatus>() }).consultationListViewModel;

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
			var userId = Guid.NewGuid();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var context = CreateContext(userService);
			AddSubmittedCommentsAndAnswers(sourceURI, "Comment Label", "Question Label", "Answer Label", userId, context);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(context, new FakeFeedService(consultationList), new FakeConsultationService(), GetFakeUserServiceWithAdministratorRights());
			//Act
			var viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null) { Status = new List<ConsultationStatus>() });

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
			var userId = Guid.NewGuid();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var context = CreateContext(userService, 2);
			AddSubmittedCommentsAndAnswers(sourceURI, "Comment Label", "Question Label", "Answer Label", userId, context);
			AddSubmittedCommentsAndAnswers(sourceURI, "Second Comment Label", "Second Question Label", " Second Answer Label", userId, context);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(context, new FakeFeedService(consultationList), new FakeConsultationService(), GetFakeUserServiceWithAdministratorRights());

			//Act
			var viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null) { Status = new List<ConsultationStatus>() });

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
			var userId = Guid.NewGuid();
			var context = CreateContext(_fakeUserService);
			var locationId = AddLocation(sourceURI, context);
			var commentId = AddComment(locationId, "Just a comment", false, userId, 2, context);
			var submissionId = AddSubmission(userId, context);
			AddSubmissionComments(submissionId, commentId, context);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(context, new FakeFeedService(consultationList), new FakeConsultationService(), GetFakeUserServiceWithAdministratorRights());

			//Act
			var viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null) { Status = new List<ConsultationStatus>() });

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
			var userId = Guid.NewGuid();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var context =  CreateContext(userService);

			var locationId = AddLocation(sourceURI, context);
			var questionTypeId = AddQuestionType("Question Type ", false, true, 1, context);
			var questionId = AddQuestion(locationId, questionTypeId, "Question Text", context);
			var AnswerId = AddAnswer(questionId, userId, "my answer", 2, context);
			var submissionId = AddSubmission(userId, context);
			AddSubmissionAnswers(submissionId, AnswerId, context);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(context, new FakeFeedService(consultationList), new FakeConsultationService(), GetFakeUserServiceWithAdministratorRights());

			//Act
			var viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null) { Status = new List<ConsultationStatus>() });

			//Assert
			viewModel.consultationListViewModel.Consultations.First().SubmissionCount.ShouldBe(1);
		}
		
		[Fact]
		public void ConsultationListPageModel_HasDocumentIdAndChapterSlugPopulatedCorrectly()
		{
			//Arrange
			var consultationList = AddConsultationsToList();
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), new FakeConsultationService(), GetFakeUserServiceWithAdministratorRights());

			//Act
			ConsultationListViewModel viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null){ Status = new List<ConsultationStatus>() }).consultationListViewModel;

			//Assert
			viewModel.Consultations.First().DocumentId.ShouldBe(1);
			viewModel.Consultations.First().ChapterSlug.ShouldBe("my-chapter-slug");
		}

		[Fact]
		public void ConsultationListPageModel_HasFilterOptions()
		{
			//Arrange
			var consultationList = AddConsultationsToList();
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), new FakeConsultationService(), GetFakeUserServiceWithAdministratorRights());

			//Act
			ConsultationListViewModel viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null){Status = new List<ConsultationStatus>()}).consultationListViewModel;

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
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), new FakeConsultationService(), GetFakeUserServiceWithAdministratorRights());
			var viewModel = new ConsultationListViewModel(null, null, null);
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
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), new FakeConsultationService(), GetFakeUserServiceWithAdministratorRights());
			var viewModel = new ConsultationListViewModel(null, null, null);
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
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), new FakeConsultationService(), GetFakeUserServiceWithAdministratorRights());
			var viewModel = new ConsultationListViewModel(null, null, null);
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
			var viewModel = new ConsultationListViewModel(null, null, null)
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
			var viewModel = new ConsultationListViewModel(null, null, null)
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
			var viewModel = new ConsultationListViewModel(null, null, null)
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
			var viewModel = new ConsultationListViewModel(null, null, null)
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
			var viewModel = new ConsultationListViewModel(null, null, null)
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
			var viewModel = new ConsultationListViewModel(null, null, null)
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
			var viewModel = new ConsultationListViewModel(null, null, null)
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
			var viewModel = new ConsultationListViewModel(null, null, null)
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
			var viewModel = new ConsultationListViewModel(null, null, null)
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
			var viewModel = new ConsultationListViewModel(null, null, null)
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
			var viewModel = new ConsultationListViewModel(null, null, null)
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
			var viewModel = new ConsultationListViewModel(null, null, null)
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
			var viewModel = new ConsultationListViewModel(null, null, null)
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
			var viewModel = new ConsultationListViewModel(null, null, null)
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
				FirstDocumentIdWhichSupportsCommentsOrQuestions = null,
				FirstChapterSlugOfFirstDocumentWhichSupportsCommentsOrQuestions = null
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
				FirstDocumentIdWhichSupportsCommentsOrQuestions = null,
				FirstChapterSlugOfFirstDocumentWhichSupportsCommentsOrQuestions = null
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
				FirstDocumentIdWhichSupportsCommentsOrQuestions = 1,
				FirstChapterSlugOfFirstDocumentWhichSupportsCommentsOrQuestions = "my-chapter-slug"
			});

			return consultationList;
		}

		private IUserService GetFakeUserServiceWithAdministratorRights()
		{
			var userId = Guid.NewGuid();
			return FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId, testUserType: TestUserType.Administrator);
		}

		private ConsultationListService GetConsultationListService()
		{
			var consultationList = AddConsultationsToList();
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), new FakeConsultationService(), GetFakeUserServiceWithAdministratorRights());
			return consultationListService;
		}

		[Theory]
		[InlineData(TestUserType.CustomFictionalRole)]
		[InlineData(TestUserType.Administrator)]
		public void ConsultationListServiceConstructorWorksWithCorrectSecurity(TestUserType testUserType)
		{
			//Arrange
			var consultationList = AddConsultationsToList();
			var userId = Guid.NewGuid();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId, testUserType: testUserType);
			var consultationContext = new ConsultationsContext(_options, userService, _fakeEncryption);

			//Act 
			var consultationService = new ConsultationListService(consultationContext, new FakeFeedService(consultationList), new FakeConsultationService(), _fakeUserService);

			//Assert
			consultationService.ShouldNotBeNull();
		}

		#endregion earlier tests


		[Fact]
		public void CHTETeamMemberCanOnlySeeTheirOwnConsultations()
		{
			//Arrange
			var consultationList = AddConsultationsToList();
			var userService = FakeUserService.Get(true, "Jeffrey Goines", Guid.NewGuid(), TestUserType.ConsultationListTestRole);
			var consultationListService = new ConsultationListService(_consultationListContext, new FakeFeedService(consultationList), new FakeConsultationService(), userService);

			//Act
			ConsultationListViewModel viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null, null) { Status = new List<ConsultationStatus>() }).consultationListViewModel;

			//Assert
			viewModel.Consultations.Count().ShouldBe(2);
		}

	}

}
