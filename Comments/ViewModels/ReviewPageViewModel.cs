using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Comments.ViewModels
{
	[Flags]
	public enum QuestionsOrComments
	{
		Questions = 1,
		Comments = 2
	}

	public class ReviewPageViewModel
    {

		public CommentsAndQuestions CommentsAndQuestions { get; set; }

	    public IEnumerable<TopicListFilterGroup> Filters { get; set; }

	    #region Filter options

	    [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
	    public QuestionsOrComments[] QuestionsOrComments { get; set; } = new QuestionsOrComments[0];

	    public int[] Documents { get; set; }

		#endregion Filter options
	}
}
