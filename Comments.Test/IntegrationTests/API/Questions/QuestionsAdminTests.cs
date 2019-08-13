using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comments.Configuration;
using Comments.Test.Infrastructure;
using Xunit;
using Newtonsoft.Json;
using Shouldly;

namespace Comments.Test.IntegrationTests.API.Questions
{
	public class QuestionsAdminTests : TestBase
	{
		private static readonly int DocumentCount = 1;

		public QuestionsAdminTests() : base(false, TestUserType.Administrator, true)
		{
			AppSettings.ConsultationListConfig.DownloadRoles = new RoleTypes()
			{
				AdminRoles = new List<string>(),
				TeamRoles = new List<string>()
			};
		}

		[Fact]
		public async Task GetQuestions()
		{
			ResetDatabase();

			//Arrange
			const string sourceURI = "consultations://./consultation/1/document/1";
			var description = Guid.NewGuid().ToString();
			var questionText = Guid.NewGuid().ToString();
			var userId = Guid.Empty;

			var locationId = AddLocation(sourceURI, _context);
			var questionTypeId = AddQuestionType(description, false, true, 1, _context);
			var questionId = AddQuestion(locationId, questionTypeId, questionText, _context);

			var consultationId = 1;

			//Act
			var response = await _client.GetAsync($"consultations/api/questions?consultationId={consultationId}");
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();
			var deserialisedQuestionAdmin = JsonConvert.DeserializeObject<ViewModels.QuestionAdmin>(responseString);

			//Assert
			deserialisedQuestionAdmin.ConsultationTitle.ShouldBe("Consultation Title");
			//deserialisedQuestionAdmin.ConsultationSupportsQuestions.ShouldBeFalse();

			deserialisedQuestionAdmin.Documents.Count().ShouldBe(DocumentCount);

			var documentWithQuestion = deserialisedQuestionAdmin.Documents.First(d => d.DocumentId == 1); // && d.SupportsQuestions);
			//documentWithQuestion.SupportsQuestions.ShouldBeTrue();
			documentWithQuestion.DocumentQuestions.Single().QuestionId.ShouldBe(questionId);
		}
	}
}
