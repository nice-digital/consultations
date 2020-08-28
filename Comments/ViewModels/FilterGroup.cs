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
		public OptionFilterGroup() {}

		public OptionFilterGroup(OptionFilterGroup optionFilterGroupToCopyFrom)
		{
			Id = optionFilterGroupToCopyFrom.Id;
			Title = optionFilterGroupToCopyFrom.Title;
			Options = new List<FilterOption>(optionFilterGroupToCopyFrom.Options);
		}

		public IList<FilterOption> Options { get; set; }	    
    }

	public class TextFilterGroup : FilterGroup
	{
		public string FilterText { get; set; }

		public bool IsSelected { get; set; }

		/// <summary>
		/// These properties get set after filtering.
		/// </summary>
		public int FilteredResultCount { get; set; }
		public int UnfilteredResultCount { get; set; }

	}

	//public class SingleOptionFilterGroup : FilterGroup
	//{
	//	public FilterOption Option { get; set; }
	//}
}
