using NICE.Feeds;
using NICE.Feeds.Tests.Infrastructure;

namespace Comments.Test.Infrastructure
{
    public class FakeFeedReaderService: IFeedReaderService{
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
            return new FeedReader(_feed).GetConsultationList();
        }

        public string GetConsultationChapter(int consultationId, int documentId, string chapterSlug)
        {
            return new FeedReader(_feed).GetConsultationList();
        }

        public string GetConsultationDetail(int consultationId)
        {
            return new FeedReader(_feed).GetConsultationList();
        }
    }
}