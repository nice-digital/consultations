using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NICE.Feeds.Models.Indev;
using NICE.Feeds.Models.Indev.Chapter;
using NICE.Feeds.Models.Indev.Detail;

namespace Comments.ViewModels
{
    public class Chapter
    {
        [JsonConstructor]
        public Chapter(string slug, string title)
        {
            Slug = slug;
            Title = title;
        }
        public Chapter(ChapterInfo chapter)
        {
            Slug = chapter.Slug;
            Title = chapter.Title;
        }
        
        public string Slug { get; protected set; }
        public string Title { get; protected set; }
        
    }

    public class ChapterContent : Chapter
    {
        [JsonConstructor]
        public ChapterContent(string slug, string title, string content, IEnumerable<ChapterSection> sections) : base(slug, title)
        {
            Content = content;
            Sections = sections;
        }

        public ChapterContent(ConsultationPublishedChapter chapter) : base(chapter.Slug, chapter.Title)
        {
            Content = chapter.Content;
            Sections = chapter.Sections?.Select(s => new ChapterSection(s));
        }

        public string Content { get; set; }
        public IEnumerable<ChapterSection> Sections { get; set; }
    }

    public class ChapterSection
    {
        [JsonConstructor]
        public ChapterSection(string slug, string title)
        {
            Slug = slug;
            Title = title;
        }

        public ChapterSection(ChapterSections section)
        {
            Slug = section.Slug;
            Title = section.Title;
        }

        public string Slug { get; private set; }
        public string Title { get; private set; }
    }
}