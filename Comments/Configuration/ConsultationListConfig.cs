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

		public string DownloadRolesCSV { get; set; }

		public ICollection<string> PermittedRolesToDownload => DownloadRolesCSV?.Split(',', StringSplitOptions.RemoveEmptyEntries)?.ToList() ?? new List<string>();
}
}
