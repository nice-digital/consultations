using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NICE.Feeds.Tests.Infrastructure;
using Shouldly;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;

namespace Comments.Test.IntegrationTests.API.Questions
{
	public class QuestionsAdminTests : TestBase
	{
		public QuestionsAdminTests() : base(false, Feed.ConsultationCommentsPublishedDetailMulitpleDoc.FilePath) { }

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
			deserialisedQuestionAdmin.ConsultationTitle.ShouldBe("For consultation comments");
			deserialisedQuestionAdmin.ConsultationSupportsQuestions.ShouldBeFalse();

			deserialisedQuestionAdmin.Documents.Count().ShouldBe(3);

			var documentWithQuestion = deserialisedQuestionAdmin.Documents.First(d => d.DocumentId == 2 && d.SupportsQuestions);
			documentWithQuestion.SupportsQuestions.ShouldBeTrue();
			documentWithQuestion.DocumentQuestions.Single().QuestionId.ShouldBe(questionId);

		}
	}
}
