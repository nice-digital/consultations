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
            public const string ConsultationUriFormat = Scheme + "//./consultatation/{0}";
            public const string DocumentUriFormat = ConsultationUriFormat + "document/{1}";
            public const string ChapterUriFormat = DocumentUriFormat + "chapter/{2}";

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


        /// <summary>
        /// returns if the 
        /// </summary>
        /// <param name="sourceURI"></param>
        /// <returns></returns>
        public static string GetCommentOn(Location location)
        {
            ConsultationsUriElements consultationsUriParts;
            if (location.SourceURI.StartsWith(ConsultationsUri.Scheme))
            {
                consultationsUriParts = ConsultationsUri.ParseConsultationsUri(location.SourceURI);
            }
            else if (location.SourceURI.StartsWith("/")) 
            {
                consultationsUriParts = ConsultationsUri.ParseRelativeUrl(location.SourceURI);
            }
            else
            {
                throw new Exception("Invalid source uri");
            }

            if (consultationsUriParts.IsChapterLevel())
            {
                if (!string.IsNullOrWhiteSpace(location.RangeStart))
                    return "Text selection";

                if (!string.IsNullOrWhiteSpace(location.HtmlElementID))
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
