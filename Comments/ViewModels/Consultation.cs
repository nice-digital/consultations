using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
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

        [JsonConstructor]
        public Consultation(string reference, string title, string consultationName, DateTime startDate, DateTime endDate, string consultationType, string resourceTitleId, string projectType, string productTypeName, string developedAs, string relevantTo, int consultationId, string process, bool allowConsultationComments, string partiallyUpdatedProjectReference, string origProjectReference)
        {
            Reference = reference;
            Title = title;
            ConsultationName = consultationName;
            StartDate = startDate;
            EndDate = endDate;
            ConsultationType = consultationType;
            ResourceTitleId = resourceTitleId;
            ProjectType = projectType;
            ProductTypeName = productTypeName;
            DevelopedAs = developedAs;
            RelevantTo = relevantTo;
            ConsultationId = consultationId;
            Process = process;
            AllowConsultationComments = allowConsultationComments;
            PartiallyUpdatedProjectReference = partiallyUpdatedProjectReference;
            OrigProjectReference = origProjectReference;
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
        [JsonConstructor]
        public ConsultationDetail(string reference, string title, string consultationName, DateTime startDate, DateTime endDate, string consultationType, string resourceTitleId, string projectType, string productTypeName, string developedAs, string relevantTo, int consultationId, string process, bool allowConsultationComments, string partiallyUpdatedProjectReference, string origProjectReference, IList<Document> documents) : base(reference, title, consultationName, startDate, endDate, consultationType, resourceTitleId, projectType, productTypeName, developedAs, relevantTo, consultationId, process, allowConsultationComments, partiallyUpdatedProjectReference, origProjectReference)
        {
            Documents = documents;
        }
        

        public ConsultationDetail(NICE.Feeds.Models.Indev.ConsultationDetail consultation) : base(consultation)
        {
            Documents = consultation.Resources?.Select(r => new Document(r)).ToList();
        }

        public IList<Document> Documents { get; set; }
    }

    public class Document
    {
        [JsonConstructor]
        public Document(int documentId, bool supportsComments, bool isSupportingDocument, string title, IEnumerable<Chapter> chapters)
        {
            DocumentId = documentId;
            SupportsComments = supportsComments;
            IsSupportingDocument = isSupportingDocument;
            Title = title;
            Chapters = chapters;
        }
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
        [JsonConstructor]
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
        [JsonConstructor]
        public ChapterWithHTML(string slug, string title, string content, IEnumerable<ChapterSection> sections) : base(slug, title)
        {
            Content = content;
            Sections = sections;
        }

        public ChapterWithHTML(ConsultationChapter chapter) : base(chapter.Slug, chapter.Title)
        {
            Content = chapter.Content;
            Sections = chapter.Sections?.Select(s => new ChapterSection(s));
        }

        public string Content { get; set; }
        public IEnumerable<ChapterSection> Sections { get; set; }
    }

    public class ChapterSection
    {
        [JsonConstructor]
        public ChapterSection(string slug, string title)
        {
            Slug = slug;
            Title = title;
        }

        public ChapterSection(ChapterSections section)
        {
            Slug = section.Slug;
            Title = section.Title;
        }

        public string Slug { get; private set; }
        public string Title { get; private set; }
    }
}
