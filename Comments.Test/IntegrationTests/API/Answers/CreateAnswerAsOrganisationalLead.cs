using Comments.Models;
using Comments.Test.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Comments.Test.IntegrationTests.API.Comments
{
	public class CreateAnswerAsOrganisationalLead : TestBaseLight
	{
		private const int consultationId = 1;

		[Fact]
		public async Task Create_Answer_As_Lead_Sets_Answer_Fields_Correctly()
		{
			//Arrange
			const int organisationId = 1;
			const int statusId = 1;
			var orgLeadUserId = Guid.NewGuid().ToString();
			
			var fakeUserService = FakeUserService.Get(isAuthenticated: true, userId: orgLeadUserId, displayName: "The Judge", testUserType: TestUserType.Authenticated, organisationUserId: null, organisationIdUserIsLeadOf: organisationId);

			using (var context = new ConsultationsContext(GetContextOptions(), fakeUserService, new FakeEncryption()))
			{
				context.Database.EnsureDeleted();
				var sourceURI = $"consultations://./consultation/{consultationId}/document/1";
				TestBaseDBHelpers.AddStatus(context, "Draft", statusId);

				var locationId = TestBaseDBHelpers.AddLocation(context, sourceURI);
				var questionId = TestBaseDBHelpers.AddQuestion(context, locationId);

				var (_server, _client) = InitialiseServerAndClient(context, fakeUserService);

				var answer = new ViewModels.Answer(0, "answer text", false, DateTime.UtcNow, orgLeadUserId, questionId, statusId);

				var builder = _server.CreateRequest($"/consultations/api/Answer");
				builder.And(request =>
				{
					request.Content = new StringContent(JsonConvert.SerializeObject(answer), Encoding.UTF8, "application/json");
				});

				//Act
				var response = await builder.PostAsync();
				response.EnsureSuccessStatusCode();

				var responseString = await response.Content.ReadAsStringAsync();
				var deserialisedAnswer = JsonConvert.DeserializeObject<ViewModels.Answer>(responseString);

				var answerInDatabase = context.Answer.IgnoreQueryFilters().Single(dbAnswer => dbAnswer.AnswerId.Equals(deserialisedAnswer.AnswerId));

				// Assert
				response.StatusCode.ShouldBe(HttpStatusCode.Created);
				answerInDatabase.CreatedByUserId.ShouldBe(orgLeadUserId);
				answerInDatabase.OrganisationUserId.ShouldBeNull();
				answerInDatabase.ParentAnswerId.ShouldBeNull();
				answerInDatabase.OrganisationId.ShouldBe(organisationId);
			}
		}
	}
}
