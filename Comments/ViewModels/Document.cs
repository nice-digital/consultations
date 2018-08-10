using System.Collections.Generic;
using System.Linq;
using Comments.Common;
using Newtonsoft.Json;
using NICE.Feeds.Models.Indev;
using NICE.Feeds.Models.Indev.Detail;

namespace Comments.ViewModels
{
    public class Document
    {
        [JsonConstructor]
        public Document(int consultationId, int documentId, bool convertedDocument, string title, IEnumerable<Chapter> chapters, bool supportsComments, bool supportsQuestions)
        {
	        ConsultationId = consultationId;
			DocumentId = documentId;
            ConvertedDocument = convertedDocument;
            Title = title;
            Chapters = chapters;
	        SupportsComments = supportsComments;
	        SupportsQuestions = supportsQuestions;
        }
        public Document(int consultationId, Resource<DetailCommentDocument> resource)
        {
	        ConsultationId = consultationId;
			DocumentId = resource.ConsultationDocumentId;
            ConvertedDocument = resource.ConvertedDocument;
	        SupportsComments = resource.SupportsComments;
	        SupportsQuestions = resource.SupportsQuestions;
            if (resource.Document != null)
            {
                Title = resource.Document.Title;
                if (resource.Document.Chapters != null)
                {
                    Chapters = resource.Document.Chapters.Select((chapter, index) => new Chapter(chapter));
                }
            }
            else
            {
                Title = resource.Title ?? resource.File.FileName;
            }

            Href = resource.File.Href;
        }

	    public Document(int consultationId, Resource<PreviewCommentDocument> resource)
	    {
		    ConsultationId = consultationId;
		    DocumentId = resource.ConsultationDocumentId;
		    ConvertedDocument = resource.ConvertedDocument;

		    if (resource.Document != null)
		    {
			    Title = resource.Document.Title;
			    if (resource.Document.Chapters != null)
			    {
				    Chapters = resource.Document.Chapters.Select((chapter, index) => new Chapter(chapter));
			    }
		    }
		    else
		    {
			    Title = resource.Title ?? resource.File.FileName;
		    }

		    Href = resource.File.Href;
	    }

		public int ConsultationId { get; private set; }
        public int DocumentId { get; private set; }
        public bool ConvertedDocument { get; private set; }
		public bool SupportsComments { get; private set; }
	    public bool SupportsQuestions { get; private set; }
		public string Title { get; private set; }
        public IEnumerable<Chapter> Chapters { get; private set; }
        public string Href { get; private set; }
	    public string SourceURI => this.ConvertedDocument ? ConsultationsUri.CreateDocumentURI(ConsultationId, DocumentId) : null;
    }
}
