namespace Comments.ViewModels
{
    public class ReviewFilterOption
    {
		public ReviewFilterOption() { }

		public ReviewFilterOption(string id, string label, bool isSelected)
		{
			Id = id;
			Label = label;
			IsSelected = isSelected;
		}

		public ReviewFilterOption(string id, string label)
		{
			Id = id;
			Label = label;
		}

		public string Id { get; set; }

        public string Label { get; set; }

        public bool IsSelected { get; set; }

		/// <summary>
		/// These properties gets set after filtering.
		/// </summary>
		public int FilteredResultCount { get; set; }
	    public int UnfilteredResultCount { get; set; }
	}
}
