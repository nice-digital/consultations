using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Comments.ViewModels
{
	public class Consultation
    {
        public Consultation(NICE.Feeds.Models.Indev.List.ConsultationBase consultation, User user, IEnumerable<BreadcrumbLink> breadcrumbs = null, ConsultationState consultationState = null, IEnumerable<OptionFilterGroup> filters = null)
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
	        HasDocumentsWhichAllowConsultationQuestions = consultation.HasDocumentsWhichAllowConsultationQuestions;
	        SupportsQuestions = consultation.SupportsQuestions;
			PartiallyUpdatedProjectReference = consultation.PartiallyUpdatedProjectReference;
            OrigProjectReference = consultation.OrigProjectReference;
            User = user;
	        Breadcrumbs = breadcrumbs;
	        ConsultationState = consultationState;
	        Filters = filters;
        }

        [JsonConstructor]
        public Consultation(string reference, string title, string consultationName, DateTime startDate, DateTime endDate, string consultationType, string resourceTitleId, string projectType, string productTypeName, string developedAs, string relevantTo, int consultationId, string process, bool allowConsultationComments, bool allowConsultationQuestions, bool supportsComments, bool supportsQuestions, string partiallyUpdatedProjectReference, string origProjectReference, User user)
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
	        HasDocumentsWhichAllowConsultationQuestions = allowConsultationQuestions;
	        SupportsQuestions = supportsQuestions;
			PartiallyUpdatedProjectReference = partiallyUpdatedProjectReference;
            OrigProjectReference = origProjectReference;
            User = user;
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
        public bool HasDocumentsWhichAllowConsultationComments { get; private set; }
	    public bool HasDocumentsWhichAllowConsultationQuestions { get; private set; }
	    public bool SupportsQuestions { get; private set; }
		public string PartiallyUpdatedProjectReference { get; private set; } //not needed?
        public string OrigProjectReference { get; private set; }

        public ViewModels.User User { get; private set; }

		public ConsultationState ConsultationState { get; private set; }

		public IEnumerable<BreadcrumbLink> Breadcrumbs { get; private set; }

	    public IEnumerable<OptionFilterGroup> Filters { get; private set; }
	}
}
