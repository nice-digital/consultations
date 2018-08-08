using System;
using System.Collections.Generic;
using System.Linq;

namespace Comments.ViewModels
{
	public class ConsultationState
	{
		public ConsultationState(DateTime startDate, DateTime endDate, bool hasQuestions, bool hasUserSuppliedAnswers, bool hasUserSuppliedComments, bool userHasSubmitted, bool consultationSupportsQuestions, bool consultationSupportsComments, IEnumerable<int> documentIdsWhichSupportQuestions, IEnumerable<int> documentIdsWhichSupportComments)
		{
			StartDate = startDate;
			EndDate = endDate;
			HasQuestions = hasQuestions;
			HasUserSuppliedAnswers = hasUserSuppliedAnswers;
			HasUserSuppliedComments = hasUserSuppliedComments;
			UserHasSubmitted = userHasSubmitted;
			ConsultationSupportsQuestions = consultationSupportsQuestions;
			ConsultationSupportsComments = consultationSupportsComments;
			_documentIdsWhichSupportQuestions = documentIdsWhichSupportQuestions;
			_documentIdsWhichSupportComments = documentIdsWhichSupportComments;
		}

		public DateTime StartDate { get; private set; }
		public DateTime EndDate { get; private set; }

		public bool HasQuestions { get; private set; }
		public bool HasUserSuppliedAnswers { get; private set; }
		public bool HasUserSuppliedComments { get; private set; }
		public bool UserHasSubmitted { get; private set; }

		public bool ConsultationSupportsQuestions { get; private set; }
		public bool ConsultationSupportsComments { get; private set; }

		private readonly IEnumerable<int> _documentIdsWhichSupportQuestions;
		public IEnumerable<int> DocumentIdsWhichSupportQuestions => _documentIdsWhichSupportQuestions ?? new List<int>();

		private readonly IEnumerable<int> _documentIdsWhichSupportComments;
		public IEnumerable<int> DocumentIdsWhichSupportComments => _documentIdsWhichSupportComments ?? new List<int>();

		public bool ConsultationIsOpen => DateTime.Now >= StartDate && DateTime.Now <= EndDate;
		public bool ConsultationHasNotStartedYet => DateTime.Now < StartDate;  //admin's in preview mode can see the consultation before the start date
		public bool ConsultationHasEnded => DateTime.Now > EndDate;
		public bool SupportsSubmission => ConsultationIsOpen && !UserHasSubmitted && (HasUserSuppliedAnswers || HasUserSuppliedComments);
		public bool SupportsDownload => (HasUserSuppliedAnswers || HasUserSuppliedComments);

		public bool ShouldShowDrawer => ConsultationSupportsQuestions || ConsultationSupportsComments ||
		                                DocumentIdsWhichSupportQuestions.Any() || DocumentIdsWhichSupportComments.Any() || 
		                                HasUserSuppliedAnswers || HasUserSuppliedComments;

		public bool ShouldShowCommentsTab => ConsultationSupportsComments || DocumentIdsWhichSupportComments.Any() ||
		                                     HasUserSuppliedComments;

		public bool ShouldShowQuestionsTab => ConsultationSupportsQuestions || DocumentIdsWhichSupportQuestions.Any() ||
		                                      HasUserSuppliedAnswers;
	}
}
