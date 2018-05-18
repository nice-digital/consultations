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
            var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var commentText = Guid.NewGuid().ToString();
            var createdByUserId = Guid.NewGuid();

            var locationId = AddLocation(sourceURI);
            AddComment(locationId, commentText, true, createdByUserId);

            // Act
            using (var consultationsContext = new ConsultationsContext(_options, _fakeUserService))
            {
                var unfilteredLocations = consultationsContext.Location.Where(l =>
                        l.SourceURI.Equals(sourceURI))
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
            var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var commentText = Guid.NewGuid().ToString();
            var createdByUserId = Guid.NewGuid();

            var locationId = AddLocation(sourceURI);
            AddComment(locationId, commentText, true, createdByUserId);

            // Act
            using (var consultationsContext = new ConsultationsContext(_options, _fakeUserService))
            {
                var filteredLocations = consultationsContext.Location.Where(l => l.SourceURI.Equals(sourceURI))
                    .Include(l => l.Comment)
                    .ToList();

                //Assert
                //removed while filtering is commented out.
                filteredLocations.Single().Comment.Count.ShouldBe(0);
            }
        }
    }
}