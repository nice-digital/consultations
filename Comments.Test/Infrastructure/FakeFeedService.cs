using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NICE.Feeds;
using NICE.Feeds.Indev;
using NICE.Feeds.Indev.Models;
using NICE.Feeds.Indev.Models.Chapter;
using NICE.Feeds.Indev.Models.Detail;
using NICE.Feeds.Indev.Models.List;

namespace Comments.Test.Infrastructure
{
	public class FakeFeedService : IIndevFeedService
	{
		private readonly IEnumerable<ConsultationList> _consultationList;
		public FakeFeedService() { }
		public FakeFeedService(IEnumerable<ConsultationList> consultationList)
		{
			_consultationList = consultationList;
		}


		public async Task<IEnumerable<ConsultationList>> GetConsultationList()
		{
			return _consultationList;
		}

		public async Task<ConsultationDetail> GetIndevConsultationDetailForPublishedProject(int consultationId, PreviewState previewState,
			int? documentId = null)
		{
			return new ConsultationDetail(){};
		}

		public Task<ConsultationPublishedPreviewDetail> GetIndevConsultationDetailForDraftProject(int consultationId, int documentId,
			string reference)
		{
			throw new NotImplementedException();
		}

		public Task<ConsultationPublishedChapter> GetConsultationChapterForPublishedProject(int consultationId, int documentId,
			string chapterSlug)
		{
			throw new NotImplementedException();
		}

		public Task<ConsultationDraftChapter> GetIndevConsultationChapterForDraftProject(int consultationId, int documentId,
			string chapterSlug, string reference)
		{
			throw new NotImplementedException();
		}
	}
}
