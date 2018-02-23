using Comments.Models;
using Comments.Services;
using Comments.ViewModels;
using System;
using System.Linq;
using Comments.Test.Infrastructure;
using Xunit;
using Shouldly;
using ConsultationsContext = Comments.Models.ConsultationsContext;

namespace Comments.Test
{
    public class UnitTests : DatabaseSetup
    {
        [Fact]
        public void Comments_CanBeRead()
        {
            ReinitialiseDatabase();
            var consultationId = Guid.NewGuid();
            var documentId = Guid.NewGuid();
            var commentText = Guid.NewGuid().ToString();

            var locationId = AddLocation(consultationId, documentId);
            AddComment(locationId, commentText);

            DocumentViewModel viewModel;
            using (var consultationsContext = new ConsultationsContext(_options))
            {
                var consultationService = new ConsultationService(consultationsContext);
                viewModel = consultationService.GetAllCommentsAndQuestionsForDocument(consultationId, documentId);
            }

            viewModel.Locations.Single().Comment.Single().CommentText.ShouldBe(commentText);
        }


        [Fact]
        public void CommentsQuestionsAndAnswers_CanBeRead()
        {
            ReinitialiseDatabase();
            var consultationId = Guid.NewGuid();
            var documentId = Guid.NewGuid();
            var commentText = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var answerText = Guid.NewGuid().ToString();

            AddCommentsAndQuestionsAndAnswers(consultationId, documentId, commentText, questionText, answerText);

            DocumentViewModel viewModel;
            using (var consultationsContext = new ConsultationsContext(_options))
            {
                var consultationService = new ConsultationService(consultationsContext);
                viewModel = consultationService.GetAllCommentsAndQuestionsForDocument(consultationId, documentId);
            }

            var location = viewModel.Locations.Single();
            location.Comment.Single().CommentText.ShouldBe(commentText);
            var question = location.Question.Single();
            question.QuestionText.ShouldBe(questionText);
            question.Answer.Single().AnswerText.ShouldBe(answerText);
        }
    }
}
