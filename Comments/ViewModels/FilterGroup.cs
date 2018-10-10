using System.Collections.Generic;

namespace Comments.ViewModels
{
	public class FilterGroup
	{
		public string Id { get; set; }

		public string Title { get; set; }
	}


    public class OptionFilterGroup : FilterGroup
	{
        public IList<FilterOption> Options { get; set; }	    
    }

	public class TextFilterGroup : FilterGroup
	{
		public string FilterText { get; set; }

		public bool IsSelected { get; set; }

		/// <summary>
		/// These properties gets set after filtering.
		/// </summary>
		public int FilteredResultCount { get; set; }
		public int UnfilteredResultCount { get; set; }

	}
}
