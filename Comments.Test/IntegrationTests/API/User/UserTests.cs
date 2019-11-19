using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using Microsoft.AspNetCore.WebUtilities;

namespace Comments.Test.IntegrationTests.API.User
{
    public class UserTestsLoggedIn : TestBase
    {
	    public UserTestsLoggedIn() : base(authenticated: true, userId: Guid.Empty.ToString(), displayName: "Quentin Tarantino") { }

		[Fact]
        public async Task Get_UserLoginDetails_Feed_Returns_Expected_Response_When_logged_in()
        {
            //Arrange (in the base constructor for this one.)

            // Act
            var response = await _client.GetAsync("/consultations/api/User?returnURL=%2f1%2f1%2fintroduction");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            responseString.ShouldMatchApproved();
        }
    }

	public class UserTestsLoggedOut : TestBase
	{
		public UserTestsLoggedOut() : base(authenticated: false) {}

		[Fact]
		public async Task Get_UserLoginDetails_Feed_Returns_Expected_Response_when_logged_out()
		{
			//Arrange (in the base constructor for this one.)

			// Act
			var response = await _client.GetAsync("/consultations/api/User?returnURL=%2f1%2f1%2fintroduction");
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			responseString.ShouldMatchApproved();
		}
	}
}
