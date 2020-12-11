using Comments.Common;
using Comments.Test.Infrastructure;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Comments.Test.IntegrationTests.API.Comments
{
	public class CommentUsingOrganisationSessionCookieTests : TestBase
	{
		private static readonly Guid _sessionId = Guid.Parse("11111111-1111-1111-1111-111111111111");

		public CommentUsingOrganisationSessionCookieTests() : base(enableOrganisationalCommentingFeature: true, testUserType: TestUserType.NotAuthenticated,
			validSessions: new Dictionary<int, Guid>{{1, _sessionId }}, useRealUserService: true, useRealHttpContextAccessor: true)
		{
		}

		[Fact]
		public async Task Create_Comment_With_Invalid_Organisation_Session_Cookie_Returns_401()
		{
			//Arrange
			const int consultationId = 1;
			var comment = new ViewModels.Comment(1, $"consultations://./consultation/{consultationId}/document/1/chapter/introduction", null, null, null, null, null, null, null, 0, DateTime.Now, Guid.Empty.ToString(), "comment text", 1, show: true, section: null);

			var builder = _server.CreateRequest("/consultations/api/Comment");

			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{consultationId}={Guid.NewGuid()}");

			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");
			});

			// Act
			var response = await builder.PostAsync();
			
			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task Create_Comment_With_Valid_Organisation_Session_Cookie_Returns_Correctly()
		{
			//Arrange
			const int consultationId = 1;
			var comment = new ViewModels.Comment(1, $"consultations://./consultation/{consultationId}/document/1/chapter/introduction", null, null, null, null, null, null, null, 0, DateTime.Now, Guid.Empty.ToString(), "comment text", 1, show: true, section: null);

			var builder = _server.CreateRequest("/consultations/api/Comment");

			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{consultationId}={_sessionId}");

			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");
			});

			// Act
			var response = await builder.PostAsync();
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.Created);
			var deserialisedComment = JsonConvert.DeserializeObject<ViewModels.Comment>(responseString);
			deserialisedComment.CommentId.ShouldBeGreaterThan(0);
		}

		[Fact]
		public async Task Create_Comment_With_Valid_Organisation_Session_Cookie_For_Incorrect_Consultation_Returns_403()
		{
			//Arrange
			const int consultationId = 1;
			var comment = new ViewModels.Comment(1, $"consultations://./consultation/2/document/1/chapter/introduction", null, null, null, null, null, null, null, 0, DateTime.Now, Guid.Empty.ToString(), "comment text", 1, show: true, section: null);

			var builder = _server.CreateRequest("/consultations/api/Comment");

			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{consultationId}={_sessionId}");

			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");
			});

			// Act
			var response = await builder.PostAsync();
			
			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
		}
	}
}
