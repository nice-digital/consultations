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
		private static readonly Guid _sessionId = Guid.Parse("A1A11A1A-1A1A-11AA-A1A1-111A1A111111");

		public CommentUsingOrganisationSessionCookieTests() : base(enableOrganisationalCommentingFeature: true, testUserType: TestUserType.NotAuthenticated,
			validSessions: new Dictionary<int, Guid>{{1, _sessionId }})
		{
		}

		[Fact]
		public async Task Create_Comment_With_Organisation_Session_Cookie()
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
	}
}
