using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NICE.Feeds;
using NICE.Feeds.Tests.Infrastructure;

namespace Comments.Configuration
{
    /// <summary>
    /// this class is temporary, until we can actually hit real feeds.
    /// </summary>
    public class FakeFeedReaderService : IFeedReaderService
    {
        private readonly Feed _feed;
        public FakeFeedReaderService(Feed feed)
        {
            _feed = feed;
        }
        public FakeFeedReaderService()
        {
            _feed = Feed.ConsultationCommentsListDetailMulitpleDoc;
        }

        public string GetConsultationList()
        {
            return new FeedReader(Feed.ConsultationCommentsListMultiple).GetConsultationList();
        }

        public string GetConsultationChapter(int consultationId, int documentId, string chapterSlug)
        {
            return new FeedReader(Feed.ConsultationCommentsChapter).GetConsultationList();
        }

        public string GetConsultationDetail(int consultationId)
        {
            return new FeedReader(_feed).GetConsultationList();
        }
    }
}
