using Comments.ViewModels;
using System.Collections.Generic;

namespace Comments.Configuration
{
	public class ConsultationListConfig
{
		public IEnumerable<OptionFilterGroup> OptionFilters { get; set; }

		public IEnumerable<TextFilterGroup> TextFilters { get; set; }
    }
}
