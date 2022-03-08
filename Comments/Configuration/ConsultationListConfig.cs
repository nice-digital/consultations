using System;
using Comments.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Comments.Configuration
{
	public class ConsultationListConfig
	{
		public IEnumerable<OptionFilterGroup> OptionFilters { get; set; }

		public TextFilterGroup TextFilters { get; set; }
		
		public RoleTypes DownloadRoles { get; set; }

		public IEnumerable<OptionFilterGroup> ContributionFilter { get; set; }

		public IEnumerable<OptionFilterGroup> TeamFilter { get; set; }

        public IEnumerable<OptionFilterGroup> HiddenConsultationsFilter { get; set; }

    }

	public class RoleTypes
	{
		public IEnumerable<string> AdminRoles { get; set; }

		public IEnumerable<string> TeamRoles { get; set; }

		public ICollection<string> AllRoles => AdminRoles.Concat(TeamRoles ?? new List<string>()).ToList();
	}
}
