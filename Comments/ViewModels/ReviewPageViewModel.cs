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

		/// <summary>
		/// This property is initialised from appsettings.json, then it gets updated in CommentService with documents and the counts are updated.
		/// </summary>
	    public IEnumerable<TopicListFilterGroup> Filters { get; set; }

	    #region Filter options from the check boxes

	    [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
	    public QuestionsOrComments[] QuestionsOrComments { get; set; } = new QuestionsOrComments[0];

	    public int[] Documents { get; set; }

		#endregion Filter options
	}
}
