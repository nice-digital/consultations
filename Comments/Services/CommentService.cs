using Comments.Models;
using Comments.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using NICE.Feeds;
using NICE.Feeds.Tests.Infrastructure;

namespace Comments.Services
{
    public interface ICommentService
    {
        DocumentViewModel GetAllCommentsAndQuestionsForDocument(int consultationId, int? documentId, string chapterSlug);
        ViewModels.Comment GetComment(int commentId);
        int EditComment(int commentId, ViewModels.Comment comment);
        ViewModels.Comment CreateComment(ViewModels.Comment comment);
        int DeleteComment(int commentId);
        bool EnsureDocumentAndChapterAreValidWithinConsultation(ConsultationDetail consultation, ref int? documentId, ref string chapterSlug);
    }

    public class CommentService : ICommentService
    {
        private readonly ConsultationsContext _context;

        public CommentService(ConsultationsContext consultationsContext)
        {
            _context = consultationsContext;
        }

        public ViewModels.Comment GetComment(int commentId)
        {
            var comment = _context.GetComment(commentId);
            return (comment == null) ? null : new ViewModels.Comment(comment.Location, comment); 
        }

        public int EditComment(int commentId, ViewModels.Comment comment)
        {
            var commentInDatabase = _context.GetComment(commentId);
            commentInDatabase.UpdateFromViewModel(comment);
            return _context.SaveChanges();
        }

        public ViewModels.Comment CreateComment(ViewModels.Comment comment)
        {
            var currentlyLoggedOnUserId = Guid.NewGuid();

            var locationToSave = new Models.Location(comment as ViewModels.Location);
            _context.Location.Add(locationToSave);
            
            var commentToSave = new Models.Comment(comment.LocationId, currentlyLoggedOnUserId, comment.CommentText, comment.LastModifiedByUserId, locationToSave);
            _context.Comment.Add(commentToSave);
            _context.SaveChanges();

            return new ViewModels.Comment(locationToSave, commentToSave);
        }

        public int DeleteComment(int commentId)
        {
            var comment = _context.GetComment(commentId);
            if (comment == null)
                return 0;

            comment.IsDeleted = true;
            return _context.SaveChanges();
        }

        public DocumentViewModel GetAllCommentsAndQuestionsForDocument(int consultationId, int? documentId, string chapterSlug)
        {
            var feedService = new FeedConverterConverterService(new FeedReader(Feed.ConsultationCommentsListDetailMulitpleDoc)); //TODO: remove this reliance on the NICE.FeedTest nuget package.
            var consultation = new ViewModels.ConsultationDetail(feedService.ConvertConsultationDetail(consultationId));
            EnsureDocumentAndChapterAreValidWithinConsultation(consultation, ref documentId, ref chapterSlug);

            feedService = new FeedConverterConverterService(new FeedReader(Feed.ConsultationCommentsChapter)); //TODO: remove this
            var chapterWithHTML = new ViewModels.ChapterWithHTML(feedService.ConvertConsultationChapter(consultationId, documentId.Value, chapterSlug));

            var locations = _context.GetAllCommentsAndQuestionsForDocument(consultationId, documentId.Value);

            var commentsData = new List<ViewModels.Comment>();
            var questionsData = new List<ViewModels.Question>();
            foreach (var location in locations)
            {
                commentsData.AddRange(location.Comment.Select(comment => new ViewModels.Comment(location, comment)));
                questionsData.AddRange(location.Question.Select(question => new ViewModels.Question(location, question)));
            }

            return new DocumentViewModel(consultation, documentId.Value, chapterWithHTML, commentsData, questionsData);
        }

        /// <summary>
        /// This method is called to ensure the documentId and chapter slug have been set and that they belong together.
        /// i.e. the document belongs to the consultation, and the chapter is in the document.
        /// </summary>
        /// <param name="consultation"></param>
        /// <param name="documentId"></param>
        /// <param name="chapterSlug"></param>
        /// <returns></returns>
        public bool EnsureDocumentAndChapterAreValidWithinConsultation(ConsultationDetail consultation, ref int? documentId, ref string chapterSlug)
        {
            if (!documentId.HasValue)
            {
                documentId = 1;
            }
            if (string.IsNullOrEmpty(chapterSlug))
            {
                chapterSlug = "some chapter";
            }
            return true;
        }
    }
}