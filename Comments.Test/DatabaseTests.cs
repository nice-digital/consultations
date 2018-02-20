using Comments.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Comments.Services;
using Comments.ViewModels;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Comments.Test
{
    public class DatabaseTests
    {
        private readonly DbContextOptions<ConsultationsContext> _options;

        public DatabaseTests()
        {
            _options = new DbContextOptionsBuilder<ConsultationsContext>()
                .UseInMemoryDatabase(databaseName: "test_db")
                .Options;
        }

        private void AddConsultationData(string guidanceReference)
        {
            using (var consultationsContext = new ConsultationsContext(_options))
            {
                consultationsContext.Comment.Add(new Comment{CommentText = guidanceReference});
                consultationsContext.SaveChanges();
            }
        }

        [Fact]
        public void Consultation_is_added_and_received()
        {
            var guidanceReference = Guid.NewGuid().ToString();
            AddConsultationData(guidanceReference);

            DocumentViewModel viewModel;
            using (var consultationsContext = new ConsultationsContext(_options))
            {
                var consultationService = new ConsultationService(consultationsContext);
                viewModel = consultationService.GetAllComments();
            }

            Assert.Equal(viewModel.Comments.Single().CommentText, guidanceReference);
        }
    }
}
