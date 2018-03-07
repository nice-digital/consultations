using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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
}