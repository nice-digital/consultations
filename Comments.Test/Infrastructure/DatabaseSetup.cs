using System;
using Comments.Models;
using Microsoft.EntityFrameworkCore;

namespace Comments.Test.Infrastructure
{
    public class DatabaseSetup
    {
        protected readonly DbContextOptions<ConsultationsContext> _options;

        public DatabaseSetup()
        {
            _options = new DbContextOptionsBuilder<ConsultationsContext>()
                .UseInMemoryDatabase(databaseName: "test_db")
                .Options;
        }

        protected void ReinitialiseDatabase()
        {
            using (var context = new ConsultationsContext(_options))
            {
                context.Database.EnsureDeleted();
            }
        }

        protected int AddLocation(Guid consultationId, Guid documentId, int locationId = 1)
        {
            using (var context = new ConsultationsContext(_options))
            {
                context.Location.Add(new Models.Location
                {
                    LocationId = locationId,
                    ConsultationId = consultationId,
                    DocumentId = documentId
                });
                context.SaveChanges();
            }
            return locationId;
        }

        protected void AddComment(int locationId, string commentText)
        {
            using (var context = new ConsultationsContext(_options))
            {
                context.Comment.Add(new Comment { LocationId = locationId, CommentText = commentText });
                context.SaveChanges();
            }
        }
        protected int AddQuestionType(int locationId, string description, bool hasBooleanAnswer, bool hasTextAnswer, int questionTypeId = 1)
        {
            using (var context = new ConsultationsContext(_options))
            {
                context.QuestionType.Add(new QuestionType { QuestionTypeId = questionTypeId, Description = description, HasBooleanAnswer = hasBooleanAnswer, HasTextAnswer = hasTextAnswer });
                context.SaveChanges();
            }
            return questionTypeId;
        }

        protected int AddQuestion(int locationId, int questionTypeId, string questionText, int questionId = 1)
        {
            using (var context = new ConsultationsContext(_options))
            {
                context.Question.Add(new Question {QuestionId = 1, LocationId = locationId, QuestionTypeId = questionTypeId, QuestionText = questionText });
                context.SaveChanges();
            }
            return questionId;
        }

        protected void AddAnswer(int questionId, Guid userId, string answerText)
        {
            using (var context = new ConsultationsContext(_options))
            {
                context.Answer.Add(new Answer{ QuestionId = questionId, UserId = userId, AnswerText = answerText});
                context.SaveChanges();
            }
        }

        protected void AddCommentsAndQuestionsAndAnswers(Guid consultationId, Guid documentId, string commentText, string questionText, string answerText)
        {
            var locationId = AddLocation(consultationId, documentId);
            AddComment(locationId, commentText);
            var questionTypeId = AddQuestionType(locationId, description: "text", hasBooleanAnswer: false, hasTextAnswer: true);
            var questionId = AddQuestion(locationId, questionTypeId, questionText);
            AddAnswer(questionId, Guid.Empty, answerText);
        }
    }
}
