using System;
using System.Collections.Generic;
using System.Linq;
using NICE.Feeds.Models.Indev;

namespace Comments.ViewModels
{
    public class Consultation
    {
        public Consultation(NICE.Feeds.Models.Indev.Consultation consultation)
        {
            Reference = consultation.Reference;
            Title = consultation.Title;
            ConsultationName = consultation.ConsultationName;
            StartDate = consultation.StartDate;
            EndDate = consultation.EndDate;
            ConsultationType = consultation.ConsultationType;
            ProjectType = consultation.ProjectType;
            ProductTypeName = consultation.ProductTypeName;
            DevelopedAs = consultation.DevelopedAs;
            RelevantTo = consultation.RelevantTo;
            ConsultationId = consultation.ConsultationId;
            Process = consultation.Process;
            AllowConsultationComments = consultation.AllowConsultationComments;
            PartiallyUpdatedProjectReference = consultation.PartiallyUpdatedProjectReference;
            OrigProjectReference = consultation.OrigProjectReference;
        }

        public string Reference { get; private set; }
        public string Title { get; private set; }
        public string ConsultationName { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public string ConsultationType { get; private set; }
        public string ResourceTitleId { get; private set; }
        public string ProjectType { get; private set; }
        public string ProductTypeName { get; private set; }
        public string DevelopedAs { get; private set; }
        public string RelevantTo { get; private set; }
        public int ConsultationId { get; private set; }
        public string Process { get; private set; }
        public bool AllowConsultationComments { get; private set; }
        public string PartiallyUpdatedProjectReference { get; private set; } //not needed?
        public string OrigProjectReference { get; private set; }
    }

    public class ConsultationDetail : Consultation
    {
        public ConsultationDetail(NICE.Feeds.Models.Indev.Consultation consultation, IList<Resource> documents) : base(consultation)
        {
            Documents = documents.Select(d => new Document(d)).ToList();
        }

        public IList<Document> Documents { get; set; }
    }

    public class Document
    {
        public Document(Resource resource)
        {
            DocumentId = resource.ConsultationDocumentId;
            SupportsComments = resource.IsConsultationCommentsDocument;

            IsSupportingDocument = resource.ConsultationDocumentId < 0;

            if (resource.Document != null)
            {
                Title = resource.Document.Title;
                if (resource.Document.Chapters != null)
                {
                    Chapters = resource.Document.Chapters.Select(c => new Chapter(c));
                }
            }
        }

        public int DocumentId { get; private set; }
        public bool SupportsComments { get; private set; }
        public bool IsSupportingDocument { get; private set; }
        public string Title { get; private set; }
        public IEnumerable<Chapter> Chapters { get; private set; }
    }

    public class Chapter
    {
        public Chapter(string slug, string title)
        {
            Slug = slug;
            Title = title;
        }
        public Chapter(ChapterInfo chapter)
        {
            Slug = chapter.Slug;
            Title = chapter.Title;
        }
        
        public string Slug { get; protected set; }
        public string Title { get; protected set; }
        
    }

    public class ChapterWithHTML : Chapter
    {
        public ChapterWithHTML(ConsultationChapter chapter) : base(chapter.Slug, chapter.Title)
        {
            Content = chapter.Content;
            Sections = chapter.Sections.Select(s => new ChapterSection(s));
        }

        public string Content { get; set; }
        public IEnumerable<ChapterSection> Sections { get; set; }
    }

    public class ChapterSection
    {
        public ChapterSection(ChapterSections section)
        {
            Slug = section.Slug;
            Title = section.Title;
        }

        public string Slug { get; private set; }
        public string Title { get; private set; }
    }
}
