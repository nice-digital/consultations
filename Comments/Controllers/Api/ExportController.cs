using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Comments.ViewModels;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;

namespace Comments.Controllers
{
	[Route("consultations/api/[controller]")]
	public class ExportController: Controller
    {
	    [HttpPost]
	    public IActionResult Post()
	    {
		    var stream = ToSpreadsheet();
		    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
		}

		Stream ToSpreadsheet()
		{
			var consultation = new Consultation(null, "My Consultation", null, DateTime.Now, DateTime.Now, null, null, null,
				null, null, null, 1, null, true, true, true, true, null, null, new User(true, "Me", new Guid()));
			var stream = new MemoryStream();
			using (var workbook = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
			{
				workbook.AddWorkbookPart();
				workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
				workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

				var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
				var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
				sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

				DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook
					.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
				string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

				DocumentFormat.OpenXml.Spreadsheet.Sheet sheet =
					new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = 1, Name = "Comments" };
				sheets.Append(sheet);

				//AppendHeaderRow(sheetData);

				//foreach (var row in rows)
				//{
				//AppendDataRow(sheetData,
				//	row,
				//	consultation,
				//	docs.FirstOrDefault(w => w.Id.Equals(row.DocumentId, StringComparison.OrdinalIgnoreCase)),
				//	session);

				AppendDataRow(sheetData,
					consultation);
				//}

				workbook.WorkbookPart.WorksheetParts.First().Worksheet.Save();
				workbook.WorkbookPart.Workbook.Save();

			}

			stream.Position = 0;
			return stream;
		}

		void AppendDataRow(SheetData sheet, Consultation consultation)
		{
			var dataRow = new Row();
			DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
			cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
			cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(consultation.Title);
			dataRow.AppendChild(cell);
		}
	}
}
