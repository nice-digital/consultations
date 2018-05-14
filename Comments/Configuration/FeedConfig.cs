using System;
using NICE.Feeds.Configuration;

namespace Comments.Configuration
{
    public class FeedConfig : IFeedConfig
    {
        public Uri IndevBasePath { get; set; }
        public string IndevApiKey { get; set; }
        //public string Chapter { get; set; }
        //public string Detail { get; set; }
        public string IndevListFeedPath { get; set; }
        public int AppCacheTimeSeconds { get; set; }

        public string IndevPublishedChapterFeedPath { get; set; }
        public string IndevDraftPreviewChapterFeedPath { get; set; }
        public string IndevPublishedDetailFeedPath { get; set; }
        public string IndevDraftPreviewDetailFeedPath { get; set; }
        public string IndevPublishedPreviewDetailFeedPath { get; set; }
    }
}