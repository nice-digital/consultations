using System;
using System.Collections.Generic;
using System.Linq;
using Comments.Common;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using NICE.Feeds.Models.Indev.List;
using Shouldly;
using Xunit;

namespace Comments.Test.UnitTests
{
	public class ConsultationListPageTests : TestBase
	{

		[Fact]
		public void ConsultationListPageModelHasConsultationsPopulated()
		{
			//Arrange
			var consultationList = new List<ConsultationList>();
			consultationList.Add(new ConsultationList{ ConsultationId = 123});
			var consultationListService = new ConsultationListService(null, new FakeFeedService(consultationList));

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
			var consultationListService = new ConsultationListService(_context, new FakeFeedService(consultationList));

			//Act
			var viewModel = consultationListService.GetConsultationListViewModel();

			//Assert
			viewModel.Consultations.First().Responses.ShouldBe(1);
		}
	}
}
