using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Comments.Models;
using Comments.Services;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Logging;

namespace Comments.Export
{
	public interface IExportToExcel
	{
		Stream ToSpreadsheet(IEnumerable<Models.Comment> comments, IEnumerable<Models.Answer> answers, IEnumerable<Models.Question> questions);
	}

	public class ExportToExcel : IExportToExcel
	{
		private readonly IUserService _userService;
		private readonly IExportService _exportService;
		private readonly ILogger<ExportToExcel> _logger;

		public ExportToExcel(IUserService userService, IExportService exportService, ILogger<ExportToExcel> logger)
		{
			_userService = userService;
			_exportService = exportService;
			_logger = logger;
		}
		public Stream ToSpreadsheet(IEnumerable<Models.Comment> comments, IEnumerable<Models.Answer> answers, IEnumerable<Models.Question> questions)
		{
			//SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create("C:/Test/TestExcel" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + ".xlsx", SpreadsheetDocumentType.Workbook);

			var stream = new MemoryStream();
			using (var workbook = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
			{
				CreateSheet(workbook, comments, answers, questions);
			}

			stream.Position = 0;

			return stream;
		}

		private void AppendHeaderRow(SheetData sheet)
		{
			var headerRow = new Row();

			DocumentFormat.OpenXml.Spreadsheet.Cell cell;

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("User");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Email Address");
			headerRow.AppendChild(cell);

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
			cell.CellValue = new CellValue("Selected Text");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Comment");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Question Text");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Answer Text");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Represents An Organisation");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue(" Organisation");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Has Tobacco Links");
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Tobacco Link Details");
			headerRow.AppendChild(cell);

			sheet.AppendChild(headerRow);
		}

		private void AppendDataRow(SheetData sheet, IEnumerable<Models.Comment> comments, IEnumerable<Models.Answer> answers, IEnumerable<Models.Question> questions)
		{
			var data = CollateData(comments, answers, questions);

			foreach (var row in data)
			{
				var dataRow = new Row();

				Cell cell;

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.UserName);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.Email);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.ConsultationName);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.DocumentName);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.ChapterTitle);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.Section);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.Quote);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.Comment);
				dataRow.AppendChild(cell);
				
				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.Question);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.Answer);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.RepresentsOrganisation == true ? "Yes" : row.RepresentsOrganisation == false ? "No" : null);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.OrganisationName);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.HasTobaccoLinks == true ? "Yes" : row.HasTobaccoLinks == false ? "No" : null);
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.TobaccoIndustryDetails);
				dataRow.AppendChild(cell);

				sheet.AppendChild(dataRow);
			}
		}

		private List<Excel> CollateData(IEnumerable<Models.Comment> comments, IEnumerable<Models.Answer> answers, IEnumerable<Models.Question> questions)
		{
			List<Excel> excel = new List<Excel>();
			foreach (var comment in comments)
			{
				var locationDetails = _exportService.GetLocationData(comment.Location);
				var excelrow = new Excel()
				{

					ConsultationName = locationDetails.ConsultationName,
					DocumentName = locationDetails.DocumentName,
					ChapterTitle = locationDetails.ChapterName,
					Section = comment.Location.Section,
					Quote = comment.Location.Quote,
					UserName =  _userService.GetDisplayNameForUserId(comment.CreatedByUserId),
					Email = _userService.GetCurrentUser().DisplayName,
					CommentId = comment.CommentId,
					Comment =  comment.CommentText,
					QuestionId = null,
					Question = null,
					AnswerId = null,
					Answer = null,
					OrganisationName =  comment.SubmissionComment.Count > 0 ? comment.SubmissionComment?.First().Submission.OrganisationName : null,
					HasTobaccoLinks =  comment.SubmissionComment.Count> 0 ? comment.SubmissionComment?.First().Submission.HasTobaccoLinks : null,
					TobaccoIndustryDetails = comment.SubmissionComment.Count > 0? comment.SubmissionComment?.First().Submission.TobaccoDisclosure : null,
					Order = comment.Location.Order
				};
				excel.Add(excelrow);
			}

			foreach (var answer in answers)
			{
				var locationDetails = _exportService.GetLocationData(answer.Question.Location);
				var excelrow = new Excel()
				{
					ConsultationName = locationDetails.ConsultationName,
					DocumentName = locationDetails.DocumentName,
					ChapterTitle = locationDetails.ChapterName,
					Section = answer.Question.Location.Section,
					Quote = answer.Question.Location.Quote,
					UserName = _userService.GetDisplayNameForUserId(answer.CreatedByUserId),
					CommentId = null,
					Comment = null,
					QuestionId = answer.Question.QuestionId,
					Question = answer.Question.QuestionText,
					AnswerId = answer.AnswerId,
					Answer = answer.AnswerText,
					OrganisationName = answer.SubmissionAnswer.Count > 0 ? answer.SubmissionAnswer?.First().Submission.OrganisationName : null,
					RepresentsOrganisation = answer.SubmissionAnswer.Count > 0 ? answer.SubmissionAnswer?.First().Submission.RespondingAsOrganisation : null,
					HasTobaccoLinks = answer.SubmissionAnswer.Count > 0 ? answer.SubmissionAnswer?.First().Submission.HasTobaccoLinks : null,
					TobaccoIndustryDetails = answer.SubmissionAnswer.Count > 0 ? answer.SubmissionAnswer?.First().Submission.TobaccoDisclosure : null,
					Order = answer.Question.Location.Order
				};
				excel.Add(excelrow);
			}

			foreach (var question in questions)
			{
				var locationDetails = _exportService.GetLocationData(question.Location);
				var excelrow = new Excel()
				{
					ConsultationName = locationDetails.ConsultationName,
					DocumentName = locationDetails.DocumentName,
					ChapterTitle = locationDetails.ChapterName,
					Section = question.Location.Section,
					Quote = question.Location.Quote,
					UserName = null,
					CommentId = null,
					Comment = null,
					QuestionId = question.QuestionId,
					Question = question.QuestionText,
					AnswerId = null,
					Answer = null,
					OrganisationName = null,
					HasTobaccoLinks = null,
					TobaccoIndustryDetails = null,
					Order = question.Location.Order
				};
				excel.Add(excelrow);
			}

			var orderedData = excel.OrderBy(o => o.UserName).ThenBy(o => o.Order).ToList();

			return orderedData;
		}

		private void CreateSheet(SpreadsheetDocument spreadsheetDocument, IEnumerable<Models.Comment> comments, IEnumerable<Models.Answer> answers, IEnumerable<Models.Question> questions)
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

			AppendDataRow(sheetData, comments, answers, questions);

			workbookpart.WorksheetParts.First().Worksheet.Save();
			workbookpart.Workbook.Save();

			// Close the document.
			spreadsheetDocument.Close();
		}
	}
}
