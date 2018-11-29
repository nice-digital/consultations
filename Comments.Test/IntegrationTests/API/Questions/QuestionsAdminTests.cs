using System;
using System.Linq;
using System.Threading.Tasks;
using Comments.Services;
using Comments.Test.Infrastructure;
using Newtonsoft.Json;
using NICE.Feeds.Tests.Infrastructure;
using Shouldly;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;

namespace Comments.Test.IntegrationTests.API.Questions
{
	public static class FakeConsultationServiceFactory //if you want to use this somewhere else, then please move it first. 
	{
		public static IConsultationService Get(int documentCount)
		{
			return new FakeConsultationService(true, documentCount);
		}
	}

	public class QuestionsAdminTests : TestBase
	{
		private static readonly int DocumentCount = 3;
		public QuestionsAdminTests() : base(false, Feed.ConsultationCommentsPublishedDetailMulitpleDoc.FilePath, FakeConsultationServiceFactory.Get(DocumentCount)) { }

		[Fact]
		public async Task GetQuestions()
		{
			//Arrange
			const string sourceURI = "consultations://./consultation/1/document/2";
			var description = Guid.NewGuid().ToString();
			var questionText = Guid.NewGuid().ToString();
			var userId = Guid.Empty;

			var locationId = AddLocation(sourceURI, _context);
			var questionTypeId = AddQuestionType(description, false, true, 1, _context);
			var questionId = AddQuestion(locationId, questionTypeId, questionText, _context);

			var consultationId = 1;

			//Act
			var response = await _client.GetAsync($"/consultations/api/questions?consultationId={consultationId}");
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();
			var deserialisedQuestionAdmin = JsonConvert.DeserializeObject<ViewModels.QuestionAdmin>(responseString);

			//Assert
			deserialisedQuestionAdmin.ConsultationTitle.ShouldBe("Consultation Title");
			deserialisedQuestionAdmin.ConsultationSupportsQuestions.ShouldBeFalse();

			deserialisedQuestionAdmin.Documents.Count().ShouldBe(DocumentCount);

			var documentWithQuestion = deserialisedQuestionAdmin.Documents.First(d => d.DocumentId == 2 && d.SupportsQuestions);
			documentWithQuestion.SupportsQuestions.ShouldBeTrue();
			documentWithQuestion.DocumentQuestions.Single().QuestionId.ShouldBe(questionId);

		}
	}
}
