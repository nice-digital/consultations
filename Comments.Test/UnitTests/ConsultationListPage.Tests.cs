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
			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList{ ConsultationId = 123});
			var consultationListService = new ConsultationListService(_context, new FakeFeedService(consultationList), new FakeConsultationService());

			//Act
			ConsultationListViewModel viewModel = consultationListService.GetConsultationListViewModel();

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
			var viewModel = consultationListService.GetConsultationListViewModel();

			//Assert
			viewModel.Consultations.First().Responses.ShouldBe(1);
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
			var viewModel = consultationListService.GetConsultationListViewModel();

			//Assert
			viewModel.Consultations.First().Responses.ShouldBe(2);
		}


		[Fact]
		public void ConsultationListPageModelHasCorrectlySetResponseCountWithComments()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var userId = Guid.NewGuid();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var consultationContext = new ConsultationsContext(_options, userService, _fakeEncryption);

			var locationId = AddLocation(sourceURI);
			var commentId = AddComment(locationId, "Just a comment", false, userId, 2);
			var submissionId = AddSubmission(userId);
			AddSubmissionComments(submissionId, commentId);

			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 1 });
			var consultationListService = new ConsultationListService(_context, new FakeFeedService(consultationList), new FakeConsultationService());

			//Act
			var viewModel = consultationListService.GetConsultationListViewModel();

			//Assert
			viewModel.Consultations.First().Responses.ShouldBe(1);
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
			var consultationContext = new ConsultationsContext(_options, userService, _fakeEncryption);

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
			var viewModel = consultationListService.GetConsultationListViewModel();

			//Assert
			viewModel.Consultations.First().Responses.ShouldBe(1);
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
			var viewModel = consultationListService.GetConsultationListViewModel();

			//Assert
			viewModel.Consultations.First().Responses.ShouldBe(2);
		}

		[Fact]
		public void ConsultationListPageModelHasDocumentIdAndChapterSlugPopulatedCorrectly()
		{
			//Arrange
			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 123 });
			var consultationListService = new ConsultationListService(_context, new FakeFeedService(consultationList), new FakeConsultationService());

			//Act
			ConsultationListViewModel viewModel = consultationListService.GetConsultationListViewModel();

			//Assert
			viewModel.Consultations.First().DocumentId.ShouldBe(1);
			viewModel.Consultations.First().ChapterSlug.ShouldBe("my-chapter-slug");
		}

		[Fact]
		public void ConsultationListPageModelHasFilterOptions()
		{
			//Arrange
			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList { ConsultationId = 123  });
			var consultationListService = new ConsultationListService(_context, new FakeFeedService(consultationList), new FakeConsultationService());

			//Act
			ConsultationListViewModel viewModel = consultationListService.GetConsultationListViewModel();

			//Assert

			var firstFilter = viewModel.Filters.First();
			firstFilter.Title.ShouldBe("Status");
			firstFilter.Options.Count.ShouldBe(2);
			firstFilter.Options.First().Label.ShouldBe("Open");
			firstFilter.Options.Skip(1).First().Label.ShouldBe("Closed");
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
