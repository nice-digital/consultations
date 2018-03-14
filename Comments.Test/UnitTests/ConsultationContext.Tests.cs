using Comments.Models;
using Comments.Test.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace Comments.Test.UnitTests
{
    public class ConsultationContext : TestBase
    {
        [Fact]
        public void Comments_IsDeleted_Flag_Filtering_is_working_in_the_context()
        {
            // Arrange
            ResetDatabase();
            var sourceURL = "/consultations/1/1/introduction";
            var commentText = Guid.NewGuid().ToString();

            var locationId = AddLocation(sourceURL);
            AddComment(locationId, commentText, true);

            // Act
            using (var consultationsContext = new ConsultationsContext(_options))
            {
                var filteredLocations = consultationsContext.Location.Where(l =>
                        l.SourceURL.Equals(sourceURL))
                            .Include(l => l.Comment);

                var unfilteredLocations = consultationsContext.Location.Where(l =>
                        l.SourceURL.Equals(sourceURL))
                            .Include(l => l.Comment)
                            .IgnoreQueryFilters();

                //Assert
                filteredLocations.Single().Comment.Count.ShouldBe(0);
                unfilteredLocations.Single().Comment.Count.ShouldBe(1);
            }
        }
    }
}