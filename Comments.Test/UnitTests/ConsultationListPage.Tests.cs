using System.Collections.Generic;
using System.Linq;
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
			var consultationListService = new ConsultationListService(new FakeFeedService(consultationList));

			//Act
			var viewModel = consultationListService.GetConsultationListViewModel();

			//Assert
			viewModel.Consultations.ConsultationId.ShouldBe(123);
		}
	}
}
