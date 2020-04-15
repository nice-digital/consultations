using System;

namespace Comments.Models
{
	public class ConsultationListRow
	{
		public ConsultationListRow(string title, DateTime startDate, DateTime endDate, int? responses, int consultationId, int? documentId, string chapterSlug, string gidReference, string productTypeName)
		{
			Title = title;
			StartDate = startDate;
			EndDate = endDate;
			SubmissionCount = responses;
			ConsultationId = consultationId;
			DocumentId = documentId;
			ChapterSlug = chapterSlug;
			GidReference = gidReference;
			ProductTypeName = productTypeName;
		}

		public string Title { get; private set; }
		public DateTime StartDate { get; private set; }
		public DateTime EndDate { get; private set; }
		public int? SubmissionCount { get; private set; } //nullable now, null if the user doesn't have access to see this or download.
		public int ConsultationId { get; private set; }
		public int? DocumentId { get; private set; }
		public string ChapterSlug { get; private set; }

		public string GidReference { get; private set; }
		public string ProductTypeName { get; private set; }


		public bool IsOpen => StartDate <= DateTime.UtcNow && EndDate > DateTime.UtcNow;
		public bool IsClosed => EndDate < DateTime.UtcNow;
		public bool IsUpcoming => StartDate > DateTime.UtcNow;
		public bool Show { get; set; } = true;

	}
}
