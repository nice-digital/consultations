//using NICE.Feeds;
//using NICE.Feeds.ConsultationService.Infrastructure;

//namespace Comments.Test.Infrastructure
//{
//    public class FakeFeedReaderService: IFeedReaderService{
//        private readonly Feed _feed;
//        public FakeFeedReaderService(Feed feed)
//        {
//            _feed = feed;
//        }
//        public FakeFeedReaderService()
//        {
//            _feed = Feed.ConsultationCommentsListDetailMulitpleDoc;
//        }

//        public string GetConsultationList()
//        {
//            return new FeedReader(Feed.ConsultationCommentsListMultiple).GetConsultationList();
//        }

//        public string GetConsultationChapter(int consultationId, int documentId, string chapterSlug)
//        {
//            return new FeedReader(Feed.ConsultationCommentsChapter).GetConsultationList();
//        }

//        public string GetConsultationDetail(int consultationId)
//        {
//            return new FeedReader(Feed.ConsultationCommentsListDetailMulitpleDoc).GetConsultationList();
//        }
//    }
//}