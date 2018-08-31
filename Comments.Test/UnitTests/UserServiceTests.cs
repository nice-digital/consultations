using Comments.Test.Infrastructure;
using Shouldly;
using System;
using System.Collections.Generic;
using Comments.Services;
using Xunit;

namespace Comments.Test.UnitTests
{
    public class UserServiceTests : TestBase
    {
        [Fact]
        public void User_Can_be_created_from_context_authenticated()
        {
            // Arrange          
            var displayName = "Benjamin Button";
            var userId = Guid.NewGuid();
            var userService = FakeUserService.Get(true, displayName, userId);

            // Act
            var currentUser = userService.GetCurrentUser();

            //Assert
            currentUser.IsAuthorised.ShouldBe(true);
            currentUser.DisplayName.ShouldBe(displayName);
            currentUser.UserId.ShouldBe(userId);
        }

        [Fact]
        public void User_returns_null_when_not_authenticated()
        {
            // Arrange          
            var userService = FakeUserService.Get(false, null, null);

            // Act
            var currentUser = userService.GetCurrentUser();

            //Assert
            currentUser.ShouldNotBeNull();
            currentUser.IsAuthorised.ShouldBe(false);
            currentUser.DisplayName.ShouldBeNull();
            currentUser.UserId.ShouldBeNull();
        }

	    [Theory]
		[InlineData("1594B24E-2672-4509-9037-831775D39DD9", "Tyler Durden", "1594B24E-2672-4509-9037-831775D39DD9", "Tyler Durden")]
	    [InlineData("1594B24E-2672-4509-9037-831775D39DD9", "Tyler Durden", "00000000-0000-0000-0000-000000000000", null)]
		public void GetDisplayNameForUserId_Returns_Correct_Display_Name(string userIdToInsert, string displayNameToInsert, string userIdToQueryBy, string expectedDisplayName)
	    {
			// Arrange
			var userService = new UserService(null, new FakeAuthenticateService(new Dictionary<Guid, string>{{ Guid.Parse(userIdToInsert), displayNameToInsert } }));

			// Act
		    var actualDisplayName = userService.GetDisplayNameForUserId(Guid.Parse(userIdToQueryBy));

			// Assert
			actualDisplayName.ShouldBe(expectedDisplayName);
	    }

	    [Fact]
	    public void GetDisplayNamesForMultipleUserIds_Returns_Multiple_Display_Names()
	    {
		    // Arrange
		    var tylersUserId = Guid.Parse("1594B24E-2672-4509-9037-831775D39DD9");
		    var marlasUserId = Guid.Parse("13FED6F9-83A0-469E-BAA6-3CFC67A0713F");
			var userService = new UserService(null, new FakeAuthenticateService(new Dictionary<Guid, string>
		    {
			    { tylersUserId, "Tyler Durden" },
				{ Guid.NewGuid(), "Robert Paulson" },
			    { marlasUserId, "Marla Singer" },
			}));
		    var angelFaceUserId = Guid.NewGuid();

			// Act
			var displayNamesReturned = userService.GetDisplayNamesForMultipleUserIds(new List<Guid>(){ tylersUserId, marlasUserId, angelFaceUserId });

		    // Assert
		    displayNamesReturned.Count.ShouldBe(3);
			displayNamesReturned[tylersUserId].ShouldBe("Tyler Durden");
		    displayNamesReturned[marlasUserId].ShouldBe("Marla Singer");
		    displayNamesReturned[angelFaceUserId].ShouldBeNull();
		}
	}
}
