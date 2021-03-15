using System;
using Comments.Common;
using NICE.Feeds.Configuration;
using NICE.Feeds.Indev;

namespace Comments.Configuration
{
    public class FeedConfig : IIndevFeedConfig
	{
		public string ApiKey { get; set; }
		public ApiConfig ApiConfig { get; set; }
		public bool UseIDAM { get; set; }

		public Uri IndevBasePath { get; set; }
		
		public string IndevListFeedPath { get; set; }
		public int CacheDurationSeconds { get; set; }

		public string IndevPublishedChapterFeedPath { get; set; }
        public string IndevDraftPreviewChapterFeedPath { get; set; }
        public string IndevPublishedDetailFeedPath { get; set; }
        public string IndevDraftPreviewDetailFeedPath { get; set; }
        public string IndevPublishedPreviewDetailFeedPath { get; set; }

	}
}
