using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Comments.Common;
using Comments.Models;
using Comments.Services;
using Comments.ViewModels;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Answer = Comments.Models.Answer;

namespace Comments.Export
{
	public interface IExportToExcel
	{
		Stream ToSpreadsheet(IEnumerable<Comments.Models.Location> locations, string consultation, string Document, string chapter);
		void ToConvert(IEnumerable<Comments.Models.Location> locations, string consultation, string Document, string chapter);
	}

	public class ExportToExcel : IExportToExcel
	{
		private readonly IConsultationService _consultationService;
		public ExportToExcel(IConsultationService consultationService)
		{
			_consultationService = consultationService;
		}
		public Stream ToSpreadsheet(IEnumerable<Comments.Models.Location> locations, string consultation, string Document, string chapter)
		{
			var stream = new MemoryStream();
			using (var workbook = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
			{
				createSheet(workbook, locations, consultation, Document, chapter);
			}

			stream.Position = 0;

			return stream;
		}

		void AppendHeaderRow(SheetData sheet)
		{
			var headerRow = new Row();

			DocumentFormat.OpenXml.Spreadsheet.Cell cell;

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Consultation Name");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Document Name");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Chapter Name");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Section");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Quote");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("User ID");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Question ID");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Question Text");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Comment ID");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Comment");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Answer ID");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Answer Text");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Submission Criteria Organisation");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Submission Criteria Tobacco");
			headerRow.AppendChild(cell);

			sheet.AppendChild(headerRow);
		}

		void AppendDataRow(SheetData sheet, IEnumerable<Comments.Models.Location> locations)
		{
			foreach (var location in locations)
			{
				var sourceURI = location.SourceURI;
				ConsultationsUriElements URIElements = ConsultationsUri.ParseConsultationsUri(sourceURI);

				var consultationDetails = _consultationService.GetConsultation(URIElements.ConsultationId, false);
				var documents = _consultationService.GetDocuments(URIElements.ConsultationId);
				var documentDetail = documents.FirstOrDefault(x => x.DocumentId == URIElements.DocumentId);
				var chapterDetail = documentDetail?.Chapters.First(c => c.Slug == URIElements.ChapterSlug);

				ICollection<Models.Answer> answers = new List<Answer>();
				if (location.Question != null && location.Question.Count != 0)
				{
					answers = location.Question.First().Answer;
				}
				
				//var submission = location.Comment.First().SubmissionComment.First().Submission.TobaccoDisclosure;

				var dataRow = new Row();
				Cell cell;

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(consultationDetails.Title);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(documentDetail?.Title);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(chapterDetail?.Title);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(location.Section);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(location.Quote);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(location.Comment.Count >= 1 ? location.Comment.First().CreatedByUserId.ToString(): null);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(location.Question == null || location.Question.Count ==0  ? null : location.Question.First().QuestionId.ToString());
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(location.Question == null || location.Question.Count == 0 ? null : location.Question.First().QuestionText);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(location.Comment.Count >= 1 ? location.Comment.First().CommentId.ToString(): null);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(location.Comment.Count >= 1 ? location.Comment.First().CommentText : null);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(answers == null || answers.Count == 0 ? null : answers.First().AnswerId.ToString());
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(answers == null || answers.Count == 0 ? null : answers?.First().AnswerText);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue("Org");
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue("Tobacco");
				dataRow.AppendChild(cell);

				sheet.AppendChild(dataRow);
			}
		}

		public void ToConvert(IEnumerable<Comments.Models.Location> locations, string consultation, string Document, string chapter)
		{
			SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create("C:/Test/TestExcel"+ DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + ".xlsx", SpreadsheetDocumentType.Workbook);

			//add the excel contents...
			createSheet(spreadsheetDocument, locations, consultation, Document, chapter);
		}

		private void createSheet(SpreadsheetDocument spreadsheetDocument, IEnumerable<Comments.Models.Location> locations, string consultation, string Document, string chapter)
		{
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

			AppendDataRow(sheetData, locations);

			workbookpart.WorksheetParts.First().Worksheet.Save();
			workbookpart.Workbook.Save();

			// Close the document.
			spreadsheetDocument.Close();
		}
	}
}
