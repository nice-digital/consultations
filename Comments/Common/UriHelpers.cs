using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Comments.Models;

namespace Comments.Common
{
    public static class UriHelpers
    {
        public static class ConsultationsUri
        {
            public const string Scheme = "consultations:";
            public const string ConsultationUriFormat = Scheme + "//./consultation/{0}";
            public const string DocumentUriFormat = ConsultationUriFormat + "/document/{1}";
            public const string ChapterUriFormat = DocumentUriFormat + "/chapter/{2}";

            public const string ConsultationsUriRegEx =
                    "^consultations:\\/\\/.+\\/consultation\\/(?<consultationId>\\d+)(\\/document\\/)?(?<documentId>\\d?)(\\/chapter\\/)?(?<chapterSlug>.*)$";
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

            public static string ConvertToConsultationsUri(string relativeURL)
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

                return string.Format(ChapterUriFormat, uriElements.ConsultationId, uriElements.DocumentId,
                    uriElements.ChapterSlug);
            }

        }

        private static ConsultationsUriElements GetElementsFromRegExGroups(GroupCollection groups)
        {
            var consultationId = int.Parse(groups["consultationId"].Value);
            var documentIdText = groups["documentId"].Value;
            var documentId = string.IsNullOrWhiteSpace(documentIdText) ? (int?)null : int.Parse(documentIdText);
            var chapterSlug = groups["chapterSlug"].Value;
            if (string.IsNullOrWhiteSpace(chapterSlug))
                chapterSlug = null;

            return new ConsultationsUriElements(consultationId, documentId, chapterSlug);
        }

        public class ConsultationsUriElements
        {
            public ConsultationsUriElements(int consultationId, int? documentId, string chapterSlug)
            {
                ConsultationId = consultationId;
                DocumentId = documentId;
                ChapterSlug = chapterSlug;
            }
            public int ConsultationId { get; set; }
            public int? DocumentId { get; set; }
            public string ChapterSlug { get; set; }

            public bool IsConsultationLevel()
            {
                return !DocumentId.HasValue;
            }
            public bool IsDocumentLevel()
            {
                return DocumentId.HasValue && string.IsNullOrEmpty(ChapterSlug);
            }
            public bool IsChapterLevel()
            {
                return !string.IsNullOrEmpty(ChapterSlug);
            }
        }


        public static string GetCommentOn(string sourceURI, string rangeStart, string htmlElementId)
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
                    return "Text selection";

                if (!string.IsNullOrWhiteSpace(htmlElementId))
                    return "Section";
                
                return "Chapter";

            }

            if (consultationsUriParts.IsDocumentLevel())
                return "Document";

            if (consultationsUriParts.IsConsultationLevel())
                return "Consultation";
            
            throw new Exception("Unknown level");
        }
    }
}
