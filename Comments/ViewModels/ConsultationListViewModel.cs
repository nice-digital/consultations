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

	[Flags]
	public enum ContributionStatus
	{
		HasContributed = 1
	}

	[Flags]
	public enum TeamStatus
	{
		MyTeam = 1
	}

    [Flags]
    public enum HiddenConsultationStatus
    {
        ShowHiddenConsultations = 1
    }

    public class ConsultationListViewModel
	{
		//default constructor needed for model binding.
		public ConsultationListViewModel() {}

		public ConsultationListViewModel(IEnumerable<ConsultationListRow> consultations, IEnumerable<OptionFilterGroup> optionFilters, TextFilterGroup textFilters, IEnumerable<OptionFilterGroup> contributionFilter, IEnumerable<OptionFilterGroup> teamFilter)
		{
			Consultations = consultations;
			OptionFilters = optionFilters;
			TextFilter = textFilters;
			ContributionFilter = contributionFilter;
			TeamFilter = teamFilter;
		}

		public string IndevBasePath => AppSettings.Feed.IndevBasePath.OriginalString; 

		public IEnumerable<ConsultationListRow> Consultations{ get; set; }

		/// <summary>
		/// This property is initialised from appsettings.json, then it gets updated in CommentService with documents and the counts are updated.
		/// </summary>
		public IEnumerable<OptionFilterGroup> OptionFilters { get; set; }

		public TextFilterGroup TextFilter { get; set; }

		public IEnumerable<OptionFilterGroup> ContributionFilter { get; set; }

		public IEnumerable<OptionFilterGroup> TeamFilter { get; set; }

        public IEnumerable<OptionFilterGroup> HiddenConsultationsFilter { get; set; }

        public ViewModels.DownloadUser User { get; set; }

		/// <summary>
		/// This boolean is true when the page is first loaded (1st SSR and 1st client-side hit). it's here in order for the backend to control
		/// the default logic - i.e. regular users should show have the ContributionFilter set to HasContributed by default, but team + admin users should have it unset.
		/// </summary>
		public bool InitialPageView { get; set; }


		#region Filter options from the check boxes

		[JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
		public IEnumerable<ConsultationStatus> Status
		{
			get;
			set;
		}

		public string Keyword { get; set; }

		[JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
		public IEnumerable<ContributionStatus> Contribution
		{
			get;
			set;
		}

		[JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
		public IEnumerable<TeamStatus> Team
		{
			get;
			set;
		}


        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IEnumerable<HiddenConsultationStatus> HiddenConsultations
        {
            get;
            set;
        }
        
        #endregion Filter options
    }
}
