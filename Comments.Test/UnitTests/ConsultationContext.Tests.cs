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
            var consultationId = RandomNumber();
            var documentId = RandomNumber();
            var commentText = Guid.NewGuid().ToString();

            var locationId = AddLocation(consultationId, documentId);
            AddComment(locationId, commentText, true);

            // Act
            using (var consultationsContext = new ConsultationsContext(_options))
            {
                var filteredLocations = consultationsContext.Location.Where(l =>
                        l.ConsultationId.Equals(consultationId) &&
                        (!l.DocumentId.HasValue || l.DocumentId.Equals(documentId)))
                    .Include(l => l.Comment);

                var unfilteredLocations = consultationsContext.Location.Where(l =>
                        l.ConsultationId.Equals(consultationId) &&
                        (!l.DocumentId.HasValue || l.DocumentId.Equals(documentId)))
                    .Include(l => l.Comment)
                    .IgnoreQueryFilters();

                //Assert
                filteredLocations.Single().Comment.Count.ShouldBe(0);
                unfilteredLocations.Single().Comment.Count.ShouldBe(1);
            }
        }
    }
}