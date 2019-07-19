using System;
using System.Collections.Generic;
using System.Linq;

namespace Comments.ViewModels
{
	public class ConsultationState
	{
		public ConsultationState(DateTime startDate, DateTime endDate, bool hasQuestions, bool hasUserSuppliedAnswers, bool hasUserSuppliedComments,
			 DateTime? submittedDate, IEnumerable<int> documentIdsWhichSupportComments)
		{
			StartDate = startDate;
			EndDate = endDate;
			HasQuestions = hasQuestions;
			HasUserSuppliedAnswers = hasUserSuppliedAnswers;
			HasUserSuppliedComments = hasUserSuppliedComments;
			SubmittedDate = submittedDate;
			//ConsultationSupportsQuestions = consultationSupportsQuestions;
			//_documentIdsWhichSupportQuestions = documentIdsWhichSupportQuestions;
			_documentIdsWhichSupportComments = documentIdsWhichSupportComments;
		}

		public DateTime StartDate { get; private set; }
		public DateTime EndDate { get; private set; }

		public bool HasQuestions { get; private set; }
		public bool HasUserSuppliedAnswers { get; private set; }
		public bool HasUserSuppliedComments { get; private set; }
		public DateTime? SubmittedDate { get; private set; }

		//public bool ConsultationSupportsQuestions { get; private set; }

		//private readonly IEnumerable<int> _documentIdsWhichSupportQuestions;
		//public IEnumerable<int> DocumentIdsWhichSupportQuestions => _documentIdsWhichSupportQuestions ?? new List<int>();

		private readonly IEnumerable<int> _documentIdsWhichSupportComments;
		public IEnumerable<int> DocumentIdsWhichSupportComments => _documentIdsWhichSupportComments ?? new List<int>();

		public bool HasAnyDocumentsSupportingComments => DocumentIdsWhichSupportComments.Any();

		public bool ConsultationIsOpen => DateTime.Now >= StartDate && DateTime.Now <= EndDate;
		public bool ConsultationHasNotStartedYet => DateTime.Now < StartDate;  //admin's in preview mode can see the consultation before the start date
		public bool ConsultationHasEnded => DateTime.Now > EndDate;
		public bool SupportsSubmission => ConsultationIsOpen && SubmittedDate==null && (HasUserSuppliedAnswers || HasUserSuppliedComments);
		public bool SupportsDownload => (HasUserSuppliedAnswers || HasUserSuppliedComments);

		public bool ShouldShowDrawer => HasQuestions || DocumentIdsWhichSupportComments.Any() || 
		                                HasUserSuppliedAnswers || HasUserSuppliedComments;

		public bool ShouldShowCommentsTab => DocumentIdsWhichSupportComments.Any() ||
		                                     HasUserSuppliedComments;

		public bool ShouldShowQuestionsTab => HasQuestions ||
		                                      HasUserSuppliedAnswers;
	}
}
