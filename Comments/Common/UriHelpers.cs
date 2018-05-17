using Comments.Models;
using Comments.ViewModels;
using System;
using System.Text.RegularExpressions;

namespace Comments.Common
{
    public static class ConsultationsUri
    {
        public const string Scheme = "consultations:";
        public const string ConsultationUriFormat = Scheme + "//./consultation/{0}";
        public const string DocumentUriFormat = ConsultationUriFormat + "/document/{1}";
        public const string ChapterUriFormat = DocumentUriFormat + "/chapter/{2}";

        public const string ConsultationsUriRegEx =
                "^consultations:\\/\\/.+\\/consultation\\/(?<consultationId>\\d+)(\\/document\\/)?(?<documentId>\\d?)(\\/chapter\\/)?(?<chapterSlug>.*)$"
            ;
        //unescaped version: ^consultations:\/\/.+\/consultation\/(\d+)(\/document\/)?(\d?)(\/chapter\/)?(.*)$

        public const string ConsultationsRelativeUrlRegEx =
            "^\\/(?<consultationId>\\d+)\\/(?<documentId>\\d+)\\/(?<chapterSlug>.+)$";
        //unescaped version: ^consultations:\/\/.+\/consultation\/(\d+)\/document\/(\d+)\/chapter\/(.+)$

        public static ConsultationsUriElements ParseConsultationsUri(string consultationsURI)
        {
            var regex = new Regex(ConsultationsUriRegEx);
            var groups = regex.Match(consultationsURI).Groups;

            return GetElementsFromRegExGroups(groups);
        }

        public static ConsultationsUriElements ParseRelativeUrl(string relativeURL)
        {
            var regex = new Regex(ConsultationsRelativeUrlRegEx);
            var groups = regex.Match(relativeURL).Groups;

            return GetElementsFromRegExGroups(groups);
        }

        public static string ConvertToConsultationsUri(string relativeURL, CommentOn commentOn)
        {
            ConsultationsUriElements uriElements;
            if (relativeURL.StartsWith(Scheme))
            {
                uriElements = ParseConsultationsUri(relativeURL);
            }
            else if (relativeURL.StartsWith("/"))
            {
                uriElements = ParseRelativeUrl(relativeURL);
            }
            else
            {
                throw new Exception("Unknown uri format");
            }

            if (commentOn == CommentOn.Consultation)
                return string.Format(ConsultationUriFormat, uriElements.ConsultationId);

            if (commentOn == CommentOn.Document)
                return string.Format(DocumentUriFormat, uriElements.ConsultationId, uriElements.DocumentId);

            return string.Format(ChapterUriFormat, uriElements.ConsultationId, uriElements.DocumentId,
                uriElements.ChapterSlug);
        }

        private static ConsultationsUriElements GetElementsFromRegExGroups(GroupCollection groups)
        {
            var consultationId = int.Parse(groups["consultationId"].Value);
            var documentIdText = groups["documentId"].Value;
            var documentId = string.IsNullOrWhiteSpace(documentIdText) ? (int?) null : int.Parse(documentIdText);

            var chapterSlug = groups["chapterSlug"].Value;
            if (string.IsNullOrWhiteSpace(chapterSlug))
                chapterSlug = null;

            return new ConsultationsUriElements(consultationId, documentId, chapterSlug);
        }

	    public static string CreateConsultationURI(int consultationId)
	    {
			return string.Format(ConsultationUriFormat, consultationId);
		}
    }
}
