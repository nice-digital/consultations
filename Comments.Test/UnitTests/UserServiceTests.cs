using Comments.Test.Infrastructure;
using Shouldly;
using System;
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
    }
}
