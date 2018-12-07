using System;
using System.Collections.Generic;
using System.Text;
using NICE.Feeds;
using NICE.Feeds.Models.Indev.Chapter;
using NICE.Feeds.Models.Indev.Detail;
using NICE.Feeds.Models.Indev.List;

namespace Comments.Test.Infrastructure
{
	public class FakeFeedService : IFeedService
	{
		private readonly IEnumerable<ConsultationList> _consultationList;
		public FakeFeedService() { }
		public FakeFeedService(IEnumerable<ConsultationList> consultationList)
		{
			_consultationList = consultationList;
		}


		public IEnumerable<ConsultationList> GetConsultationList()
		{
			return _consultationList;
		}

		public ConsultationDetail GetIndevConsultationDetailForPublishedProject(int consultationId, PreviewState previewState,
			int? documentId = null)
		{
			return new ConsultationDetail(){};
		}

		public ConsultationPublishedPreviewDetail GetIndevConsultationDetailForDraftProject(int consultationId, int documentId,
			string reference)
		{
			throw new NotImplementedException();
		}

		public ConsultationPublishedChapter GetConsultationChapterForPublishedProject(int consultationId, int documentId,
			string chapterSlug)
		{
			throw new NotImplementedException();
		}

		public ConsultationDraftChapter GetIndevConsultationChapterForDraftProject(int consultationId, int documentId,
			string chapterSlug, string reference)
		{
			throw new NotImplementedException();
		}
	}
}
