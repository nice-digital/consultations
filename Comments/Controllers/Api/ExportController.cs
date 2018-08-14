using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Comments.Export;
using Comments.ViewModels;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;

namespace Comments.Controllers
{
	[Route("consultations/api/[controller]")]
	public class ExportController: ControllerBase
    {
	    [HttpPost]
	    public IActionResult Post([FromBody] ViewModels.Comment comment)
	    {
			ExportToExcel export = new ExportToExcel();
		    var stream = export.ToSpreadsheet(comment);
		    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
		}

	    [HttpGet]
	    public IActionResult Get([FromBody] ViewModels.Comment comment)
	    {
		    ExportToExcel export = new ExportToExcel();
		    var stream = export.ToSpreadsheet(comment);
		    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
		}
	}
}
