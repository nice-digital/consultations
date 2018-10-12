using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using Comments.Common;
using Comments.Configuration;
using Comments.Models;

namespace Comments.ViewModels
{
	[Flags]
	public enum ConsultationStatus
	{
		Open = 1,
		Closed = 2,
		Upcoming = 3,
	}

	//public enum ReviewSortOrder
	//{
	//	DocumentAsc,
	//	DateDesc
	//}

	public class ConsultationListViewModel
	{
		//default constructor needed for model binding.
		public ConsultationListViewModel() {}

		public ConsultationListViewModel(IEnumerable<ConsultationListRow> consultations, IEnumerable<OptionFilterGroup> optionFilters, TextFilterGroup textFilters)
		{
			Consultations = consultations;
			OptionFilters = optionFilters;
			TextFilter = textFilters;
		}

		public string IndevBasePath => AppSettings.Feed.IndevBasePath.OriginalString; 

		public IEnumerable<ConsultationListRow> Consultations{ get; set; }

		/// <summary>
		/// This property is initialised from appsettings.json, then it gets updated in CommentService with documents and the counts are updated.
		/// </summary>
		public IEnumerable<OptionFilterGroup> OptionFilters { get; set; }

		public TextFilterGroup TextFilter { get; set; }


		#region Filter options from the check boxes


		[JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
		public IEnumerable<ConsultationStatus> Status
		{
			get;
			set;
		}

		public string Keyword { get; set; }

		//[JsonConverter(typeof(StringEnumConverter))]
		//public ReviewSortOrder Sort { get; set; } = ReviewSortOrder.DocumentAsc;

		#endregion Filter options
	}
}
