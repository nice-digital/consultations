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
        public void Comments_IsDeleted_Flag_is_not_Filtering_in_the_context()
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
                var unfilteredLocations = consultationsContext.Location.Where(l =>
                        l.SourceURL.Equals(sourceURL))
                    .Include(l => l.Comment)
                    .IgnoreQueryFilters()
                    .ToList();

                //Assert
                unfilteredLocations.First().Comment.Count.ShouldBe(1);
            }
        }

        [Fact]
        public void Comments_IsDeleted_Flag_is_Filtering_in_the_context()
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
                            .Include(l => l.Comment)
                            .ToList();

                //Assert
                filteredLocations.Single().Comment.Count.ShouldBe(0);
            }
        }
    }
}