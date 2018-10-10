using Comments.Configuration;
using Comments.ViewModels;
using System.Collections.Generic;

namespace Comments.Test.Infrastructure
{
	public static class TestAppSettings
	{
		public static ConsultationListConfig GetConsultationListConfig()
		{
			return new ConsultationListConfig()
			{
				OptionFilters = new List<OptionFilterGroup>
				{
					new OptionFilterGroup{ Id = "Status", Title = "Status", Options = new List<FilterOption>()
					{
						new FilterOption("Open", "Open"),
						new FilterOption("Closed", "Closed"),
						new FilterOption("Upcoming", "Upcoming"),
					}}
				},
				TextFilters = new List<TextFilterGroup> { new TextFilterGroup() { Id = "Reference", Title = "Reference" } }
			};
		}
	}
}
