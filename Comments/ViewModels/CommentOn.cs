using Comments.Common;
using System;
using Comments.Models;

namespace Comments.ViewModels
{
    public enum CommentOn
    {
        Consultation,
        Document,
        Chapter,
        Section, //aka html element id..?
        Selection
    }

    public static class CommentOnHelpers
    {
        public static CommentOn GetCommentOn(string commentOnString)
        {
            return (CommentOn)Enum.Parse(typeof(CommentOn), commentOnString);
        }

        public static CommentOn GetCommentOn(string sourceURI, string rangeStart, string htmlElementId)
        {
            ConsultationsUriElements consultationsUriParts;
            if (sourceURI.StartsWith(ConsultationsUri.Scheme))
            {
                consultationsUriParts = ConsultationsUri.ParseConsultationsUri(sourceURI);
            }
            else if (sourceURI.StartsWith("/"))
            {
                consultationsUriParts = ConsultationsUri.ParseRelativeUrl(sourceURI);
            }
            else
            {
                throw new Exception("Invalid source uri");
            }

            if (consultationsUriParts.IsChapterLevel())
            {
                if (!string.IsNullOrWhiteSpace(rangeStart))
                    return CommentOn.Selection;

                if (!string.IsNullOrWhiteSpace(htmlElementId))
                    return CommentOn.Section;

                return CommentOn.Chapter;

            }

            if (consultationsUriParts.IsDocumentLevel())
                return CommentOn.Document;

            if (consultationsUriParts.IsConsultationLevel())
                return CommentOn.Consultation;

            throw new Exception("Unknown level");
        }
    }
}
