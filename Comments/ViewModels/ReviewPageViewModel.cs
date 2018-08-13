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

	public enum ReviewSortOrder
	{
		DocumentAsc,
		DateDesc
	}

	public class ReviewPageViewModel
    {
	    
	    public CommentsAndQuestions CommentsAndQuestions { get; set; }

		/// <summary>
		/// This property is initialised from appsettings.json, then it gets updated in CommentService with documents and the counts are updated.
		/// </summary>
	    public IEnumerable<ReviewFilterGroup> Filters { get; set; }

		#region Filter options from the check boxes

		
	    [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
	    public IEnumerable<QuestionsOrComments> Type
	    {
		    get;
		    set;
	    }

	    public IEnumerable<int> Document { get; set; }

	    [JsonConverter(typeof(StringEnumConverter))]
	    public ReviewSortOrder Sort { get; set; } = ReviewSortOrder.DocumentAsc;

		#endregion Filter options
	}
}
