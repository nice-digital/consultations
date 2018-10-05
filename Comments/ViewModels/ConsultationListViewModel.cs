using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using Comments.Models;

namespace Comments.ViewModels
{
	[Flags]
	public enum ConsultationStatus
	{
		Open = 1,
		Closed = 2
	}

	//public enum ReviewSortOrder
	//{
	//	DocumentAsc,
	//	DateDesc
	//}

	public class ConsultationListViewModel
	{
		public ConsultationListViewModel(IEnumerable<ConsultationListRow> consultations, IEnumerable<FilterGroup> filters)
		{
			Consultations = consultations;
			Filters = filters;
		}

		public IEnumerable<ConsultationListRow> Consultations{ get; private set; }

		/// <summary>
		/// This property is initialised from appsettings.json, then it gets updated in CommentService with documents and the counts are updated.
		/// </summary>
		public IEnumerable<FilterGroup> Filters { get; set; }


		#region Filter options from the check boxes


		[JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
		public IEnumerable<ConsultationStatus> Status
		{
			get;
			set;
		}

		//public IEnumerable<int> Document { get; set; }

		//[JsonConverter(typeof(StringEnumConverter))]
		//public ReviewSortOrder Sort { get; set; } = ReviewSortOrder.DocumentAsc;

		#endregion Filter options
	}
}
