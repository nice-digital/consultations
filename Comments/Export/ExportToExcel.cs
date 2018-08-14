using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Comments.ViewModels;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using NICE.Feeds.Models.Indev.List;

namespace Comments.Export
{
	public interface IExportToExcel
	{
		Stream ToSpreadsheet(ViewModels.Comment comment);
	}

	public class ExportToExcel : IExportToExcel
	{
		public Stream ToSpreadsheet(ViewModels.Comment comment)
		{
			var stream = new MemoryStream();
			using (var workbook = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
			{
				createSheet(workbook, comment);
			}

			stream.Position = 0;
			
			return stream;
		}

		void AppendHeaderRow(SheetData sheet)
		{
			var headerRow = new Row();

			DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
			cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
			cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Comment ID");
			headerRow.AppendChild(cell);

			cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
			cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
			cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Comment");
			headerRow.AppendChild(cell);

			//DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
			//cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
			//cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Consultation Name");
			//headerRow.AppendChild(cell);

			//cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
			//cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
			//cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Start date");
			//headerRow.AppendChild(cell);

			//cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
			//cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
			//cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("End date");
			//headerRow.AppendChild(cell);

			//cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
			//cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
			//cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("User");
			//headerRow.AppendChild(cell);

			//cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
			//cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
			//cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Comment type");
			//headerRow.AppendChild(cell);

			//cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
			//cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
			//cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Document");
			//headerRow.AppendChild(cell);

			//cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
			//cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
			//cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Chapter");
			//headerRow.AppendChild(cell);

			//cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
			//cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
			//cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Section");
			//headerRow.AppendChild(cell);

			//cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
			//cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
			//cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Comment");
			//headerRow.AppendChild(cell);

			//if (User.IsAdministrator)
			//{
			//	cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
			//	cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
			//	cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Tags");
			//	headerRow.AppendChild(cell);
			//}

			//cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
			//cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
			//cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Response");
			//headerRow.AppendChild(cell);

			sheet.AppendChild(headerRow);
		}

		void AppendDataRow(SheetData sheet, ViewModels.Comment comment)
		{
			var dataRow = new Row();
			Cell cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue(comment.CommentId.ToString());
			dataRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue(comment.CommentText);
			dataRow.AppendChild(cell);

			sheet.AppendChild(dataRow);
		}

		public void ToConvert(ViewModels.Comment comment)
		{
			SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create("C:/Test/TestExcel"+ DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + ".xlsx", SpreadsheetDocumentType.Workbook);

			//add the excel contents...
			createSheet(spreadsheetDocument, comment);
		}

		private void createSheet(SpreadsheetDocument spreadsheetDocument, ViewModels.Comment comment)
		{
			var consultation = new Consultation(null, "My Consultation", null, DateTime.Now, DateTime.Now, null, null, null,
				null, null, null, 1, null, true, true, true, true, null, null, new User(true, "Me", new Guid()));

			// Add a WorkbookPart to the document.
			WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
			workbookpart.Workbook = new Workbook();

			// Add a WorksheetPart to the WorkbookPart.
			WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
			SheetData sheetData = new SheetData();
			worksheetPart.Worksheet = new Worksheet(sheetData);

			// Add Sheets to the Workbook.
			Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
			//Sheets sheets = workbookpart.Workbook.GetFirstChild<Sheets>();

			// Append a new worksheet and associate it with the spreadsheetDocument.
			Sheet sheet = new Sheet()
			{
				Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
				SheetId = 1,
				Name = "Comments"
			};
			sheets.Append(sheet);

			AppendHeaderRow(sheetData);

			AppendDataRow(sheetData, comment);

			workbookpart.WorksheetParts.First().Worksheet.Save();
			workbookpart.Workbook.Save();

			// Close the document.
			spreadsheetDocument.Close();
		}
	}
}
