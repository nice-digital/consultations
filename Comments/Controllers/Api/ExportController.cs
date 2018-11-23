using System.Linq;
using Comments.Export;
using Comments.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comments.Controllers.Api
{

	public class ExportControllerBase : ControllerBase
	{
		protected readonly IExportService _exportService;
		protected readonly IExportToExcel _exportToExcel;
		public ExportControllerBase(IExportService exportService, IExportToExcel exportToExcel)
		{
			_exportService = exportService;
			_exportToExcel = exportToExcel;
		}
	}

	[Authorize(Roles = "Administrator")]
	[Route("consultations/api/[controller]")]
	public class ExportController: ExportControllerBase
	{
		public ExportController(IExportService exportService, IExportToExcel exportToExcel) : base(exportService, exportToExcel) {}

		//GET: consultations/api/Export/5 
		[HttpGet("{consultationId}")]
		public IActionResult Get([FromRoute] int consultationId)
		{
			var result = _exportService.GetAllDataForConsulation(consultationId);
			
			var stream = _exportToExcel.ToSpreadsheet(result.comment, result.answer, result.question);
			return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
		}
	}

	[Route("consultations/api/[controller]")]
	public class ExportExternalController : ExportControllerBase
	{
		public ExportExternalController(IExportService exportService, IExportToExcel exportToExcel) : base(exportService, exportToExcel) {}

		//GET: consultations/api/ExportExternal/5 
		[HttpGet("{consultationId}")]
		public IActionResult Get([FromRoute] int consultationId)
		{
			var result = _exportService.GetAllDataForConsulationForCurrentUser(consultationId);

			var stream = _exportToExcel.ToSpreadsheet(result.comment, result.answer, result.question);
			return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
		}
	}
}
