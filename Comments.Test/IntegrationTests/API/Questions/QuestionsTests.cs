using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comments.Test.Infrastructure;
using Xunit;
using Newtonsoft.Json;
using Shouldly;
using System.Net;
using System.Net.Http;
using System.Text;
using Comments.Models;
using Comments.Services;

namespace Comments.Test.IntegrationTests.API.Questions
{
	public class QuestionsTests : TestBase
	{
		[Fact]
		public async Task Get_Question()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();

			const string sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var description = Guid.NewGuid().ToString();
			var questionText = Guid.NewGuid().ToString();

			var locationId = AddLocation(sourceURI);
			var questionTypeId = 99;
			var questionId = AddQuestion(locationId, questionTypeId, questionText);

			//Act
			var response = await _client.GetAsync($"consultations/api/question/{questionId}");
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();
			var deserialisedQuestion = JsonConvert.DeserializeObject<ViewModels.Question>(responseString);

			//Assert
			response.StatusCode.ShouldBe(HttpStatusCode.OK);
			deserialisedQuestion.QuestionId.ShouldBeGreaterThan(0);
			deserialisedQuestion.QuestionText.ShouldBe(questionText);
		}

		[Fact]
		public async Task Create_Question()
		{
			// Arrange
			ResetDatabase();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var description = Guid.NewGuid().ToString();
			var questionText = Guid.NewGuid().ToString();

			var locationId = AddLocation(sourceURI);
			var questionTypeId = AddQuestionType(description, false, true);

			var location = new Location(sourceURI, null, null, null, null, null, null, null, null, null, null);
			var questionType = new QuestionType(description, false, true, null);
			var question = new Question(locationId, questionText, questionTypeId, null, questionType, new List<Answer>());
			var viewModel = new ViewModels.Question(location, question);

			var content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");

			// Act
			var response = await _client.PostAsync("/consultations/api/question", content);
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.Created);
			var deserialisedQuestion = JsonConvert.DeserializeObject<ViewModels.Question>(responseString);
			deserialisedQuestion.QuestionId.ShouldBeGreaterThan(0);
			deserialisedQuestion.QuestionText.ShouldBe(questionText);
		}

		[Fact]
		public async Task Edit_Question()
		{
			//Arrange
			const string sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var description = Guid.Empty.ToString();
			var questionText = Guid.NewGuid().ToString();
			var userId = Guid.Empty.ToString();

			var locationId = AddLocation(sourceURI, _context);
			var questionTypeId = 99;
			var questionId = AddQuestion(locationId, questionTypeId, questionText, _context);

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var questionService = new QuestionService(_context, userService, _consultationService, null, null);
			var viewModel = questionService.GetQuestion(questionId);

			var updatedQuestionText = Guid.Parse("{FCA1BA48-59F8-43E4-B413-6F89E2F0B73F}").ToString();
			viewModel.question.QuestionText = updatedQuestionText;

			var content = new StringContent(JsonConvert.SerializeObject(viewModel.question), Encoding.UTF8, "application/json");

			//Act
			var response = await _client.PutAsync($"/consultations/api/question/{questionId}", content);
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();

			var result = questionService.GetQuestion(questionId);

			//Assert
			responseString.ShouldMatchApproved(new Func<string, string>[] { Scrubbers.ScrubLastModifiedDate, Scrubbers.ScrubIds });
			result.question.QuestionText.ShouldBe(updatedQuestionText);
		}

		[Fact]
		public async Task Delete_Question()
		{
			//Arrange
			var userId = Guid.Empty.ToString();
			const string sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var description = Guid.NewGuid().ToString();
			var questionText = Guid.NewGuid().ToString();

			var locationId = AddLocation(sourceURI);
			var questionTypeId = 99;
			var questionId = AddQuestion(locationId, questionTypeId, questionText);

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var questionService = new QuestionService(new ConsultationsContext(_options, userService, _fakeEncryption),
				userService, _consultationService, null, null);

			//Act
			var response = await _client.DeleteAsync($"consultations/api/question/{questionId}");
			response.EnsureSuccessStatusCode();

			var result = questionService.GetQuestion(questionId);

			//Assert
			result.validate.NotFound.ShouldBeTrue();
		}
	}
}
