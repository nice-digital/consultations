using System;
using System.Collections.Generic;
using System.Linq;
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
		public ConsultationListPageTests() : base(null)
		{
			AppSettings.ConsultationListConfig = GetConsultationListConfig();
		}

		[Fact]
		public void ConsultationListPageModelHasConsultationsPopulated()
		{
			//Arrange
			var consultationList = AddConsutationsToList();
			var consultationListService = new ConsultationListService(_context, new FakeFeedService(consultationList), new FakeConsultationService());

			//Act
			ConsultationListViewModel viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null) { Status = new List<ConsultationStatus>() });

			//Assert
			viewModel.Consultations.First().ConsultationId.ShouldBe(123);
		}

		[Fact]
		public void ConsultationListPageModelHasCorrectlySetResponseCount()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var userId = Guid.NewGuid();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var consultationContext = new ConsultationsContext(_options, userService, _fakeEncryption);
			AddSubmittedCommentsAndAnswers(sourceURI, "Comment Label", "Question Label", "Answer Label", userId, consultationContext);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(_context, new FakeFeedService(consultationList), new FakeConsultationService());

			//Act
			var viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null) { Status = new List<ConsultationStatus>() });

			//Assert
			viewModel.Consultations.First().SubmissionCount.ShouldBe(1);
		}

		[Fact]
		public void ConsultationListPageModelHasCorrectlySetResponseCountWithMulitpleSubmissions()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var userId = Guid.NewGuid();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var consultationContext = new ConsultationsContext(_options, userService, _fakeEncryption);
			AddSubmittedCommentsAndAnswers(sourceURI, "Comment Label", "Question Label", "Answer Label", userId, consultationContext);
			AddSubmittedCommentsAndAnswers(sourceURI, "Second Comment Label", "Second Question Label", " Second Answer Label", userId, consultationContext);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(_context, new FakeFeedService(consultationList), new FakeConsultationService());

			//Act
			var viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null) { Status = new List<ConsultationStatus>() });

			//Assert
			viewModel.Consultations.First().SubmissionCount.ShouldBe(2);
		}


		[Fact]
		public void ConsultationListPageModelHasCorrectlySetResponseCountWithComments()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var userId = Guid.NewGuid();

			var locationId = AddLocation(sourceURI);
			var commentId = AddComment(locationId, "Just a comment", false, userId, 2);
			var submissionId = AddSubmission(userId);
			AddSubmissionComments(submissionId, commentId);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(_context, new FakeFeedService(consultationList), new FakeConsultationService());

			//Act
			var viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null) { Status = new List<ConsultationStatus>() });

			//Assert
			viewModel.Consultations.First().SubmissionCount.ShouldBe(1);
		}



		[Fact]
		public void ConsultationListPageModelHasCorrectlySetResponseCountWithAnswers()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var userId = Guid.NewGuid();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);

			var locationId = AddLocation(sourceURI);
			var questionTypeId = AddQuestionType("Question Type ", false, true);
			var questionId = AddQuestion(locationId, questionTypeId, "Question Text");
			var AnswerId = AddAnswer(questionId, userId, "my answer", 2);
			var submissionId = AddSubmission(userId);
			AddSubmissionAnswers(submissionId, AnswerId);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(_context, new FakeFeedService(consultationList), new FakeConsultationService());

			//Act
			var viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null) { Status = new List<ConsultationStatus>() });

			//Assert
			viewModel.Consultations.First().SubmissionCount.ShouldBe(1);
		}

		[Fact]
		public void ConsultationListPageModelHasCorrectlySetResponseCountWithMulitpleUsers()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var userId = Guid.NewGuid();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var consultationContext = new ConsultationsContext(_options, userService, _fakeEncryption);
			AddSubmittedCommentsAndAnswers(sourceURI, "Comment Label", "Question Label", "Answer Label", userId, consultationContext);
			AddSubmittedCommentsAndAnswers(sourceURI, "another users Comment Label", "another users Question Label", " another users Answer Label", Guid.NewGuid(), consultationContext);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(_context, new FakeFeedService(consultationList), new FakeConsultationService());

			//Act
			var viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null) { Status = new List<ConsultationStatus>() });

			//Assert
			viewModel.Consultations.First().SubmissionCount.ShouldBe(2);
		}

		[Fact]
		public void ConsultationListPageModelHasDocumentIdAndChapterSlugPopulatedCorrectly()
		{
			//Arrange
			var consultationList = AddConsutationsToList();
			var consultationListService = new ConsultationListService(_context, new FakeFeedService(consultationList), new FakeConsultationService());

			//Act
			ConsultationListViewModel viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null){ Status = new List<ConsultationStatus>() });

			//Assert
			viewModel.Consultations.First().DocumentId.ShouldBe(1);
			viewModel.Consultations.First().ChapterSlug.ShouldBe("my-chapter-slug");
		}

		[Fact]
		public void ConsultationListPageModelHasFilterOptions()
		{
			//Arrange
			var consultationList = AddConsutationsToList();
			var consultationListService = new ConsultationListService(_context, new FakeFeedService(consultationList), new FakeConsultationService());

			//Act
			ConsultationListViewModel viewModel = consultationListService.GetConsultationListViewModel(new ConsultationListViewModel(null, null){Status = new List<ConsultationStatus>()});

			//Assert
			var firstFilter = viewModel.Filters.First();
			firstFilter.Title.ShouldBe("Status");
			firstFilter.Options.Count.ShouldBe(2);
			firstFilter.Options.First().Label.ShouldBe("Open");
			firstFilter.Options.Skip(1).First().Label.ShouldBe("Closed");
		}

		[Fact]
		public void ConsultationListPageModelHasFilterOptionOpenSetToSelected()
		{
			//Arrange
			var consultationList = AddConsutationsToList();
			var consultationListService = new ConsultationListService(_context, new FakeFeedService(consultationList), new FakeConsultationService());
			var viewModel = new ConsultationListViewModel(null, null);
			viewModel.Status = new List<ConsultationStatus>(){ ConsultationStatus.Open };

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel);

			//Assert
			updatedViewModel.Filters.First().Options.First(f => f.Id == "Open").IsSelected.ShouldBeTrue();
		}

		private List<ConsultationList> AddConsutationsToList()
		{
			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList
			{
				ConsultationId = 123,
				ConsultationName = "open consultation",
				StartDate = DateTime.Now.AddDays(-1),
				EndDate = DateTime.Now.AddDays(1)
			});
			consultationList.Add(new ConsultationList()
			{
				ConsultationId = 124,
				ConsultationName = "closed consultation",
				StartDate = DateTime.Now.AddDays(-3),
				EndDate = DateTime.Now.AddDays(-2)
			});

			return consultationList;
		}

		private ConsultationListService AddSubmissions()
		{
			var consultationList = AddConsutationsToList();

			var sourceURI = "consultations://./consultation/123/document/1/chapter/introduction";
			var userId = Guid.NewGuid();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var consultationContext = new ConsultationsContext(_options, userService, _fakeEncryption);

			var locationId = AddLocation(sourceURI);
			var commentId = AddComment(locationId, "Just a comment", false, userId, 2);
			var submissionId = AddSubmission(userId);
			AddSubmissionComments(submissionId, commentId);

			var locationId2 = AddLocation("consultations://./consultation/124/document/1/chapter/introduction");
			var commentId2 = AddComment(locationId2, "Just a comment", false, userId, 2);
			var submissionId2 = AddSubmission(userId);
			AddSubmissionComments(submissionId2, commentId2);

			var consultationListService = new ConsultationListService(consultationContext, new FakeFeedService(consultationList), new FakeConsultationService());

			return consultationListService;
		}


		[Fact]
		public void ConsultationListPageModel_WithFilterOptionOpenHasCorrectResultCounts()
		{
			//Arrange
			var consultationListService = AddSubmissions();
			var viewModel = new ConsultationListViewModel(null, null)
			{
				Status = new List<ConsultationStatus> {ConsultationStatus.Open}
			};

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel);

			//Assert
			updatedViewModel.Filters.First().Options.First(f => f.Id == "Open").FilteredResultCount.ShouldBe(1);
			updatedViewModel.Filters.First().Options.First(f => f.Id == "Open").UnfilteredResultCount.ShouldBe(2);
		}


		[Fact]
		public void ConsultationListPageModel_WithFilterOptionClosedHasCorrectResultCounts()
		{
			//Arrange
			var consultationListService = AddSubmissions();
			var viewModel = new ConsultationListViewModel(null, null)
			{
				Status = new List<ConsultationStatus> { ConsultationStatus.Closed }
			};

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel);

			//Assert
			updatedViewModel.Filters.First().Options.First(f => f.Id == "Closed").FilteredResultCount.ShouldBe(1);
			updatedViewModel.Filters.First().Options.First(f => f.Id == "Closed").UnfilteredResultCount.ShouldBe(2);
		}

		[Fact]
		public void ConsultationListPageModel_WithFilterOptionOpenAndClosedHasCorrectResultCounts()
		{
			//Arrange
			var consultationListService = AddSubmissions();
			var viewModel = new ConsultationListViewModel(null, null)
			{
				Status = new List<ConsultationStatus> { ConsultationStatus.Closed, ConsultationStatus.Open }
			};

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel);

			//Assert
			updatedViewModel.Filters.First().Options.First(f => f.Id == "Open").FilteredResultCount.ShouldBe(1);
			updatedViewModel.Filters.First().Options.First(f => f.Id == "Open").UnfilteredResultCount.ShouldBe(2);
			updatedViewModel.Filters.First().Options.First(f => f.Id == "Closed").FilteredResultCount.ShouldBe(1);
			updatedViewModel.Filters.First().Options.First(f => f.Id == "Closed").UnfilteredResultCount.ShouldBe(2);
		}


		[Fact]
		public void ConsultationListPageModelHasFilterOptionClosedSetToSelected()
		{
			//Arrange
			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 123 });
			var consultationListService = new ConsultationListService(_context, new FakeFeedService(consultationList), new FakeConsultationService());
			var viewModel = new ConsultationListViewModel(null, null);
			viewModel.Status = new List<ConsultationStatus>() { ConsultationStatus.Closed };

			//Act
			var updatedViewModel = consultationListService.GetConsultationListViewModel(viewModel);

			//Assert
			updatedViewModel.Filters.First().Options.Skip(1).First(f => f.Id == "Closed").IsSelected.ShouldBeTrue();
		}


		private static ConsultationListConfig GetConsultationListConfig()
		{
			return new ConsultationListConfig()
			{
				Filters = new List<FilterGroup>()
				{
					new FilterGroup(){ Id = "Status", Title = "Status", Options = new List<FilterOption>()
					{
						new FilterOption("Open", "Open"),
						new FilterOption("Closed", "Closed"),
					}}
				}
			};
		}
	}
}
