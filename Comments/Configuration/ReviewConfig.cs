using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comments.ViewModels;

namespace Comments.Configuration
{
    public class ReviewConfig
    {
	    public IEnumerable<TopicListFilterGroup> Filters { get; set; }
	}
}
