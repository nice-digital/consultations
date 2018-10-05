using System.Collections.Generic;

namespace Comments.ViewModels
{
    public class FilterGroup
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public IList<FilterOption> Options { get; set; }
    }
}
