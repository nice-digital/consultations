using Comments.Test.Infrastructure;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Comments.Services;
using Xunit;

namespace Comments.Test.UnitTests
{
    public class FeatureFlagServiceTests : TestBase
    {
        [Fact]
        public async void Feature_flags_and_values_should_be_returned()
        {
            // Arrange          
            var featureFlagService = new FeatureFlagService(_fakeFeatureManager);

            // Act
            var featureFlags = await featureFlagService.GetFeatureFlags();

			//Assert
			featureFlags.First().Key.ShouldBe("TestFeatureFlag");
			featureFlags.First().Value.ShouldBeFalse();
        }

    }
}
