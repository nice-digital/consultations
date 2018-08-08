using System.Collections.Generic;

namespace Comments.ViewModels
{
    public class ReviewFilterGroup
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public IList<ReviewFilterOption> Options { get; set; }
    }
}
