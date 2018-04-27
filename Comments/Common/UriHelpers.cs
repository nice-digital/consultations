using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                    "^consultations:\\/\\/.+\\/consultation\\/(?<consultationId>\\d+)\\/document\\/(?<documentId>\\d+)\\/chapter\\/(?<chapterSlug>.+)$";
            //unescaped version: ^consultations:\/\/.+\/consultation\/(\d+)\/document\/(\d+)\/chapter\/(.+)$

            public const string ConsultationsRelativeUrlRegEx =
                "^\\/(?<consultationId>\\d+)\\/(?<documentId>\\d+)\\/(?<chapterSlug>.+)$";
            //unescaped version: ^consultations:\/\/.+\/consultation\/(\d+)\/document\/(\d+)\/chapter\/(.+)$

            public static ConsultationsUriElements ParseConsultationsUri(string consultationsURI)
            {
                var regex = new Regex(ConsultationsUriRegEx);

                var namedCaptures = regex.MatchNamedCaptures(consultationsURI);

                var consultationId = int.Parse(namedCaptures["consultationId"]);
                int.TryParse(namedCaptures["documentId"], out var documentId);
                var chapterSlug = namedCaptures["chapterSlug"];

                return new ConsultationsUriElements(consultationId, documentId, chapterSlug);
            }

            public static ConsultationsUriElements ParseRelativeUrl(string relativeURL)
            {
                var regex = new Regex(ConsultationsRelativeUrlRegEx);

                var namedCaptures = regex.MatchNamedCaptures(relativeURL);

                var consultationId = int.Parse(namedCaptures["consultationId"]);
                int.TryParse(namedCaptures["documentId"], out var documentId);
                var chapterSlug = namedCaptures["chapterSlug"];

                return new ConsultationsUriElements(consultationId, documentId, chapterSlug);
            }
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


        public static string GetCommentOn(string sourceURI)
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
                return "Chapter";

            if (consultationsUriParts.IsDocumentLevel())
                return "Document";

            if (consultationsUriParts.IsConsultationLevel())
                return "Consultation";

            throw new Exception("Unknown level");
        }
    }
}
