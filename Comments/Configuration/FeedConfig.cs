using System;
using NICE.Feeds.Configuration;

namespace Comments.Configuration
{
    public class FeedConfig : IFeedConfig
    {
        public Uri BasePath { get; set; }
        public string ApiKey { get; set; }
        public string Chapter { get; set; }
        public string Detail { get; set; }
        public string List { get; set; }
        public int AppCacheTimeSeconds { get; set; }
    }
}