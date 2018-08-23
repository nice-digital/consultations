using System.Linq;
using Comments.Export;
using Comments.Services;
using Microsoft.AspNetCore.Mvc;

namespace Comments.Controllers.Api
{
	[Route("consultations/api/[controller]")]
	public class ExportController: ControllerBase
	{
		private readonly IExportService _exportService;
		private readonly IExportToExcel _exportToExcel;
		public ExportController(IExportService exportService, IExportToExcel exportToExcel)
		{
			_exportService = exportService;
			_exportToExcel = exportToExcel;
		}

		//GET: consultations/api/Export/5 
		[HttpGet("{consultationId}")]
		public IActionResult Get([FromRoute] int consultationId)
		{
			var result = _exportService.GetAllDataForConsulation(consultationId);
			
			//_exportToExcel.ToConvert(result.comment, result.answer, result.question);

			var stream = _exportToExcel.ToSpreadsheet(result.comment, result.answer, result.question);
			return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
		}
	}
}
