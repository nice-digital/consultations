using System;

namespace Comments.ViewModels
{
	public class ConsultationState
	{
		public ConsultationState(DateTime startDate, DateTime endDate, bool hasQuestions, bool hasUserSuppliedAnswers, bool hasUserSuppliedComments, bool userHasSubmitted)
		{
			StartDate = startDate;
			EndDate = endDate;
			HasQuestions = hasQuestions;
			HasUserSuppliedAnswers = hasUserSuppliedAnswers;
			HasUserSuppliedComments = hasUserSuppliedComments;
			UserHasSubmitted = userHasSubmitted;
		}

		public DateTime StartDate { get; private set; }
		public DateTime EndDate { get; private set; }

		public bool HasQuestions { get; private set; }
		public bool HasUserSuppliedAnswers { get; private set; }
		public bool HasUserSuppliedComments { get; private set; }
		public bool UserHasSubmitted { get; private set; }

		public bool ConsultationIsOpen => DateTime.Now >= StartDate && DateTime.Now <= EndDate;
		public bool ConsultationHasNotStartedYet => DateTime.Now < StartDate;  //admin's in preview mode can see the consultation before the start date
		public bool ConsultationHasEnded => DateTime.Now > EndDate;
		public bool SupportsSubmission => ConsultationIsOpen && !UserHasSubmitted && (HasUserSuppliedAnswers || HasUserSuppliedComments);
		public bool SupportsDownload => (HasUserSuppliedAnswers || HasUserSuppliedComments);
	}
}
