﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Threading.Tasks;
using Comments.Common;
using Comments.Configuration;
using Comments.Models;
using Comments.Services;
using Comments.ViewModels;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Logging;
using Answer = Comments.Models.Answer;
using Comment = Comments.Models.Comment;
using Question = Comments.Models.Question;

namespace Comments.Export
{
	public interface IExportToExcel
	{
		Task<Stream> ToSpreadsheet(IEnumerable<Models.Comment> comments, IEnumerable<Models.Answer> answers, IEnumerable<Models.Question> questions);
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
		public async Task<Stream> ToSpreadsheet(IEnumerable<Models.Comment> comments, IEnumerable<Models.Answer> answers, IEnumerable<Models.Question> questions)
		{
			//SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create("C:/Test/TestExcel" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + ".xlsx", SpreadsheetDocumentType.Workbook);

			var stream = new MemoryStream();
			using (var workbook = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
			{
				await CreateSheet(workbook, comments, answers, questions);
			}

			stream.Position = 0;

			return stream;
		}

		private Columns SetUpColumns()
		{
			Columns columns = new Columns(
				new Column // User
				{
					Min = 1,
					Max = 14,
					Width = 25,
					CustomWidth = true
				},
				new Column // Email Address
				{
					Min = 2,
					Max = 2,
					Width = 25,
					CustomWidth = true
				},
				new Column // Consultation Name
				{
					Min = 3,
					Max = 3,
					Width = 25,
					CustomWidth = true
				},
				new Column // Document Name
				{
					Min = 4,
					Max = 4,
					Width = 25,
					CustomWidth = true
				},
				new Column // Chapter Name
				{
					Min = 5,
					Max = 5,
					Width = 25,
					CustomWidth = true
				},
				new Column // SectionHeader
				{
					Min = 6,
					Max = 6,
					Width = 25,
					CustomWidth = true
				},
                new Column // SectionNumber
                {
                    Min = 6,
                    Max = 6,
                    Width = 25,
                    CustomWidth = true
                },
                new Column // Selected text
				{
					Min = 7,
					Max = 7,
					Width = 25,
					CustomWidth = true
				},
				new Column // Comment
				{
					Min = 8,
					Max = 8,
					Width = 50,
					CustomWidth = true
				},
				new Column // Question Text
				{
					Min = 9,
					Max = 9,
					Width = 50,
					CustomWidth = true
				},
				new Column // Yes/No Answer Text
				{
					Min = 10,
					Max = 10,
					Width = 15,
					CustomWidth = true
				},
				new Column // Answer Text
				{
					Min = 11,
					Max = 11,
					Width = 50,
					CustomWidth = true
				},
				new Column // Represent An Organisation
				{
					Min = 12,
					Max = 12,
					Width = 20,
					CustomWidth = true
				},
				new Column // Organisation Name
				{
					Min = 13,
					Max = 13,
					Width = 25,
					CustomWidth = true
				},
				new Column // Has Tobacco Links
				{
					Min = 14,
					Max = 14,
					Width = 20,
					CustomWidth = true
				},
				new Column // Tobacco Link Details
				{
					Min = 15,
					Max = 15,
					Width = 25,
					CustomWidth = true
				},
				new Column // Organisation interested in formal support. might not actually be shown depending on the data.
				{
					Min = 16,
					Max = 16,
					Width = 35,
					CustomWidth = true
				});

			return columns;
		}

		private void AppendHeaderRow(SheetData sheet, bool showOrganisationExpressionOfInterest)
		{
			var headerRow = new Row();

			DocumentFormat.OpenXml.Spreadsheet.Cell cell;

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("User");
			cell.StyleIndex = 2;
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Email Address");
			cell.StyleIndex = 2;
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Consultation Name");
			cell.StyleIndex = 2;
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Document Name");
			cell.StyleIndex = 2;
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Chapter Name");
			cell.StyleIndex = 2;
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Section Header");
			cell.StyleIndex = 2;
			headerRow.AppendChild(cell);

            cell = new Cell();
            cell.DataType = CellValues.String;
            cell.CellValue = new CellValue("Section Number");
            cell.StyleIndex = 2;
            headerRow.AppendChild(cell);

            cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Selected Text");
			cell.StyleIndex = 2;
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Comment");
			cell.StyleIndex = 2;
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Question Text");
			cell.StyleIndex = 2;
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Yes/No Answer");
			cell.StyleIndex = 2;
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Answer Text");
			cell.StyleIndex = 2;
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Represents An Organisation");
			cell.StyleIndex = 2;
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue(" Organisation");
			cell.StyleIndex = 2;
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Has Tobacco Links");
			cell.StyleIndex = 2;
			headerRow.AppendChild(cell);

			cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue("Tobacco Link Details");
			cell.StyleIndex = 2;
			headerRow.AppendChild(cell);

			if (showOrganisationExpressionOfInterest)
			{
				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(Constants.Export.ExpressionOfInterestColumnDescription);
				cell.StyleIndex = 2;
				headerRow.AppendChild(cell);
			}

			sheet.AppendChild(headerRow);
		}

		private void AppendDataRow(SheetData sheet, List<Excel> collatedData, bool showOrganisationExpressionOfInterest)
		{
			foreach (var row in collatedData)
			{
				var dataRow = new Row();

				Cell cell;

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.UserName);
				cell.StyleIndex = 1;
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.Email);
				cell.StyleIndex = 1;
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.ConsultationName);
				cell.StyleIndex = 1;
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.DocumentName);
				cell.StyleIndex = 1;
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.ChapterTitle);
				cell.StyleIndex = 1;
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.SectionHeader);
				cell.StyleIndex = 1;
				dataRow.AppendChild(cell);

                cell = new Cell();
                cell.DataType = CellValues.String;
                cell.CellValue = new CellValue(row.SectionNumber);
                cell.StyleIndex = 1;
                dataRow.AppendChild(cell);

                cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.Quote);
				cell.StyleIndex = 1;
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.Comment);
				cell.StyleIndex = 1;
				dataRow.AppendChild(cell);
				
				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.Question);
				cell.StyleIndex = 1;
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.AnswerBoolean == true ? Constants.Export.Yes : row.AnswerBoolean == false ? Constants.Export.No : null);
				cell.StyleIndex = 1;
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.Answer);
				cell.StyleIndex = 1;
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.RepresentsOrganisation == true ? Constants.Export.Yes : row.RepresentsOrganisation == false ? Constants.Export.No : null);
				cell.StyleIndex = 1;
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.OrganisationName);
				cell.StyleIndex = 1;
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.HasTobaccoLinks == true ? Constants.Export.Yes : row.HasTobaccoLinks == false ? Constants.Export.No : null);
				cell.StyleIndex = 1;
				dataRow.AppendChild(cell);

				cell = new Cell();
				cell.DataType = CellValues.String;
				cell.CellValue = new CellValue(row.TobaccoIndustryDetails);
				cell.StyleIndex = 1;
				dataRow.AppendChild(cell);

				if (showOrganisationExpressionOfInterest)
				{
					cell = new Cell();
					cell.DataType = CellValues.String;
					cell.CellValue = new CellValue(row.OrganisationExpressionOfInterest.HasValue ? (row.OrganisationExpressionOfInterest.Value ? Constants.Export.Yes : Constants.Export.No) : null);
					cell.StyleIndex = 1;
					dataRow.AppendChild(cell);
				}

				sheet.AppendChild(dataRow);
			}
		}
        
		private async Task<(List<Excel> collatedData, bool showOrganisationExpressionOfInterest)> CollateData(IEnumerable<Models.Comment> comments, IEnumerable<Models.Answer> answers, IEnumerable<Models.Question> questions)
		{
            var details = await GetUserDetailsAndEmails(comments, answers);

            List<Excel> excel = new List<Excel>();
            var excelCellLimit = 32767;

            foreach (var comment in comments)
			{
				var locationDetails = await _exportService.GetLocationData(comment.Location);
				var commentOn = CommentOnHelpers.GetCommentOn(comment.Location.SourceURI, comment.Location.RangeStart, comment.Location.HtmlElementID);
                var email = GetEmailForComment(comment, details.userDetailsForUserIds, details.emailAddressesForOrganisationIds);
                var order = comment.Location.Order;
                var userName = GetUserNameForComment(comment, details.userDetailsForUserIds);
                
                var excelrow = new Excel()
				{
					ConsultationName = locationDetails.ConsultationName,
					DocumentName = locationDetails.DocumentName,
					ChapterTitle = locationDetails.ChapterName,
					SectionHeader = commentOn == CommentOn.Section || commentOn == CommentOn.SubSection || commentOn == CommentOn.Selection ? comment.Location.SectionHeader : null,
                    SectionNumber = comment.Location.SectionNumber,
					Quote = commentOn  == CommentOn.Selection ? comment.Location.Quote : null,
					UserName = userName,
					Email = email,
					CommentId = comment.CommentId,
					Comment = comment.CommentText != null && comment.CommentText.Length > 32767 ? SplitLongComment(comment.CommentText, excel, email, userName, order, locationDetails, comment, commentOn, excelCellLimit): comment.CommentText,
					QuestionId = null,
					Question = null,
					AnswerId = null,
					AnswerBoolean = null,
					Answer = null,
					RepresentsOrganisation = comment.SubmissionComment.Count > 0 ? comment.SubmissionComment?.First().Submission.RespondingAsOrganisation : null,
					OrganisationName =  comment.SubmissionComment.Count > 0 ? comment.SubmissionComment?.First().Submission.OrganisationName : null,
					HasTobaccoLinks =  comment.SubmissionComment.Count> 0 ? comment.SubmissionComment?.First().Submission.HasTobaccoLinks : null,
					TobaccoIndustryDetails = comment.SubmissionComment.Count > 0? comment.SubmissionComment?.First().Submission.TobaccoDisclosure : null,
					OrganisationExpressionOfInterest = comment.SubmissionComment.Count > 0 ? comment.SubmissionComment?.First().Submission.OrganisationExpressionOfInterest : null,
					Order = order,
			};
				excel.Add(excelrow);
			}

			foreach (var answer in answers)
			{
				var locationDetails = await _exportService.GetLocationData(answer.Question.Location);
                var email = GetEmailForAnswer(answer, details.userDetailsForUserIds, details.emailAddressesForOrganisationIds);
                var order = answer.Question.Location.Order;
                var userName = GetUserNameForAnswer(answer, details.userDetailsForUserIds);

                var excelrow = new Excel()
				{
					ConsultationName = locationDetails.ConsultationName,
					DocumentName = locationDetails.DocumentName,
					ChapterTitle = locationDetails.ChapterName,
					SectionHeader = answer.Question.Location.SectionHeader,
                    SectionNumber = answer.Question.Location.SectionNumber,
                    Quote = answer.Question.Location.Quote,
					UserName = userName,
					Email = email,
					CommentId = null,
					Comment = null,
					QuestionId = answer.Question.QuestionId,
					Question = answer.Question.QuestionText,
					AnswerId = answer.AnswerId,
					AnswerBoolean = answer.AnswerBoolean,
					Answer = answer.AnswerText != null && answer.AnswerText.Length > 32767 ? SplitLongAnswer(answer.AnswerText, excel, email, userName, order, locationDetails, answer, excelCellLimit) : answer.AnswerText,
                    OrganisationName = answer.SubmissionAnswer.Count > 0 ? answer.SubmissionAnswer?.First().Submission.OrganisationName : null,
					RepresentsOrganisation = answer.SubmissionAnswer.Count > 0 ? answer.SubmissionAnswer?.First().Submission.RespondingAsOrganisation : null,
					HasTobaccoLinks = answer.SubmissionAnswer.Count > 0 ? answer.SubmissionAnswer?.First().Submission.HasTobaccoLinks : null,
					TobaccoIndustryDetails = answer.SubmissionAnswer.Count > 0 ? answer.SubmissionAnswer?.First().Submission.TobaccoDisclosure : null,
					OrganisationExpressionOfInterest = answer.SubmissionAnswer.Count > 0 ? answer.SubmissionAnswer?.First().Submission.OrganisationExpressionOfInterest : null,
					Order = answer.Question.Location.Order
				};
				excel.Add(excelrow);
			}

			foreach (var question in questions)
			{
				var locationDetails = await _exportService.GetLocationData(question.Location);
				var excelrow = new Excel()
				{
					ConsultationName = locationDetails.ConsultationName,
					DocumentName = locationDetails.DocumentName,
					ChapterTitle = locationDetails.ChapterName,
					SectionHeader = question.Location.SectionHeader,
                    SectionNumber = question.Location.SectionNumber,
                    Quote = question.Location.Quote,
					UserName = null,
					Email = null,
					CommentId = null,
					Comment = null,
					QuestionId = question.QuestionId,
					Question = question.QuestionText,
					AnswerId = null,
					AnswerBoolean = null,
					Answer = null,
					RepresentsOrganisation = null,
					OrganisationName = null,
					HasTobaccoLinks = null,
					TobaccoIndustryDetails = null,
					OrganisationExpressionOfInterest = null,
					Order = question.Location.Order
				};
				excel.Add(excelrow);
			}

			var orderedData = excel.OrderBy(o => o.Email).ThenBy(o => o.Order).ToList();

			var showOrganisationExpressionOfInterest = orderedData.Any(data => data.OrganisationExpressionOfInterest.HasValue);

			return (orderedData, showOrganisationExpressionOfInterest);
		}

        private string SplitLongComment(string text, List<Excel> excel, string email, string userName, string order, (string ConsultationName, string DocumentName, string ChapterName) locationDetails, Comment comment, CommentOn commentOn, int excelCellLimit)
        {
            if (text.Length > excelCellLimit)
            {
                var remainingText = "Comment continued... " + text.Substring(excelCellLimit, text.Length - excelCellLimit);
                var incrementedOrder = order + ".1";

                var excelrow = new Excel()
                {
                    ConsultationName = locationDetails.ConsultationName,
                    DocumentName = locationDetails.DocumentName,
                    ChapterTitle = locationDetails.ChapterName,
                    SectionHeader = commentOn == CommentOn.Section || commentOn == CommentOn.SubSection || commentOn == CommentOn.Selection ? comment.Location.SectionHeader : null,
                    SectionNumber = comment.Location.SectionNumber,
                    Quote = commentOn == CommentOn.Selection ? comment.Location.Quote : null,
                    UserName = userName,
                    Email = email,
                    CommentId = comment.CommentId,
                    Comment = remainingText.Length > excelCellLimit ? SplitLongComment(remainingText, excel, email, userName, incrementedOrder, locationDetails, comment, commentOn, excelCellLimit) : remainingText,
                    QuestionId = null,
                    Question = null,
                    AnswerId = null,
                    AnswerBoolean = null,
                    Answer = null,
                    RepresentsOrganisation = comment.SubmissionComment.Count > 0 ? comment.SubmissionComment?.First().Submission.RespondingAsOrganisation : null,
                    OrganisationName = comment.SubmissionComment.Count > 0 ? comment.SubmissionComment?.First().Submission.OrganisationName : null,
                    HasTobaccoLinks = comment.SubmissionComment.Count > 0 ? comment.SubmissionComment?.First().Submission.HasTobaccoLinks : null,
                    TobaccoIndustryDetails = comment.SubmissionComment.Count > 0 ? comment.SubmissionComment?.First().Submission.TobaccoDisclosure : null,
                    OrganisationExpressionOfInterest = comment.SubmissionComment.Count > 0 ? comment.SubmissionComment?.First().Submission.OrganisationExpressionOfInterest : null,
                    Order = incrementedOrder,
                };
                excel.Add(excelrow);
            }

            return text.Substring(0, excelCellLimit);
        }

        private string SplitLongAnswer(string text, List<Excel> excel, string email, string userName, string order, (string ConsultationName, string DocumentName, string ChapterName) locationDetails, Answer answer, int excelCellLimit)
        {
            if (text.Length > excelCellLimit)
            {
                var remainingText = "Answer continued... " + text.Substring(excelCellLimit, text.Length - excelCellLimit);

                var excelrow = new Excel()
                {
                    ConsultationName = locationDetails.ConsultationName,
                    DocumentName = locationDetails.DocumentName,
                    ChapterTitle = locationDetails.ChapterName,
                    SectionHeader = answer.Question.Location.SectionHeader,
                    Quote = answer.Question.Location.Quote,
                    UserName = userName,
                    Email = email,
                    CommentId = null,
                    Comment = null,
                    QuestionId = answer.Question.QuestionId,
                    Question = answer.Question.QuestionText,
                    AnswerId = answer.AnswerId,
                    AnswerBoolean = answer.AnswerBoolean,
                    Answer = remainingText.Length > excelCellLimit ? SplitLongAnswer(remainingText, excel, email, userName, order, locationDetails, answer, excelCellLimit) : remainingText,
                    OrganisationName = answer.SubmissionAnswer.Count > 0 ? answer.SubmissionAnswer?.First().Submission.OrganisationName : null,
                    RepresentsOrganisation = answer.SubmissionAnswer.Count > 0 ? answer.SubmissionAnswer?.First().Submission.RespondingAsOrganisation : null,
                    HasTobaccoLinks = answer.SubmissionAnswer.Count > 0 ? answer.SubmissionAnswer?.First().Submission.HasTobaccoLinks : null,
                    TobaccoIndustryDetails = answer.SubmissionAnswer.Count > 0 ? answer.SubmissionAnswer?.First().Submission.TobaccoDisclosure : null,
                    OrganisationExpressionOfInterest = answer.SubmissionAnswer.Count > 0 ? answer.SubmissionAnswer?.First().Submission.OrganisationExpressionOfInterest : null,
                    Order = order + ".1",
                };
                excel.Add(excelrow);
            }

            return text.Substring(0, excelCellLimit);
        }

        private async Task<(Dictionary<string, (string displayName, string emailAddress)> userDetailsForUserIds, Dictionary<int, string> emailAddressesForOrganisationIds)> GetUserDetailsAndEmails(IEnumerable<Models.Comment> comments, IEnumerable<Models.Answer> answers)
        {
            var userDetailsForUserIds = new Dictionary<string, (string displayName, string emailAddress)>();
            var emailAddressesForOrganisationIds = new Dictionary<int, string>();

            var organisationUserIds = comments.Where(comment => comment.OrganisationUserId.HasValue).Select(comment => comment.OrganisationUserId.Value)
                .Concat(answers.Where(answer => answer.OrganisationUserId.HasValue).Select(answer => answer.OrganisationUserId.Value)).Distinct();

            var currentUser = _userService.GetCurrentUser();
            var currentUserDetails = _userService.GetCurrentUserDetails();
            var userDetailsForOrganisationUser = _exportService.GetOrganisationUsersByOrganisationUserIds(organisationUserIds);

            var userRoles = _userService.GetUserRoles().ToList();
            var isAdminUser = userRoles.Any(role => AppSettings.ConsultationListConfig.DownloadRoles.AdminRoles.Contains(role));
            var hasTeamRole = userRoles.Any(role => AppSettings.ConsultationListConfig.DownloadRoles.TeamRoles.Contains(role));

            if (organisationUserIds.Count() != 0 && !isAdminUser && !hasTeamRole)
            {
            	var commentsSubmittedToLead = comments.Any(c => c.StatusId == (int)StatusName.SubmittedToLead);
                var answersSubmittedToLead = answers.Any(a => a.StatusId == (int)StatusName.SubmittedToLead);
                
                 // When the lead downloads the responses they have sent to NICE, the responses should have the Name and Email Address of that lead against them
                if (currentUser.IsAuthenticatedByAccounts && !commentsSubmittedToLead && !answersSubmittedToLead)
				{

                    var userIds = comments.Select(c => c.SubmissionComment.First().Submission.SubmissionByUserId)
                        .Concat(answers.Select(a => a.SubmissionAnswer.First().Submission.SubmissionByUserId)).Where(user => !string.IsNullOrEmpty(user)).Distinct();

                    // If it is the same lead as submitted to NICE, we can just use their details
                    if (userIds.Count() == 1 && userIds.Single().Equals(currentUserDetails.userId, StringComparison.OrdinalIgnoreCase))
                    {
                        userDetailsForUserIds.Add(currentUserDetails.userId, (currentUserDetails.displayName, currentUserDetails.emailAddress));
                    }
                    else
                    {
                        // If the lead that is downloading isn't the lead that submitted to NICE, we need to get the submitters details
                        // We can't get them the same way as we do for internal staff as external users can't get user details from IdAM, so we add a placeholder name
                        foreach (var userId in userIds)
                        {
                            userDetailsForUserIds.Add(userId, ("Organisation lead", ""));
                        }
                    }
                }
                else
                {
                    // When an organisation code user downloads their own responses, or a lead downloads the original responses that were sent to them, they should see the details of the organisational commenter against each response
                    foreach (var user in userDetailsForOrganisationUser)
                    {
                        emailAddressesForOrganisationIds.Add(user.OrganisationUserId, user.EmailAddress);
                    }
                }
            }
            else
            {
                var userIds = comments.Select(comment => comment.CreatedByUserId).Concat(answers.Select(answer => answer.CreatedByUserId)).Where(user => !string.IsNullOrEmpty(user)).Distinct();

                // When a normal user downloads their own comments, we can get their details without going to IdaM
                if (userIds.Count() == 1 && userIds.Single().Equals(currentUserDetails.userId, StringComparison.OrdinalIgnoreCase))
                {
                    userDetailsForUserIds.Add(currentUserDetails.userId, (currentUserDetails.displayName, currentUserDetails.emailAddress));
                }
                else
                {
                    // When the NICE internal users download responses, they are from multiple users and we need to go to IdAM to get the details for each of the comments
                    userDetailsForUserIds = await _userService.GetUserDetailsForUserIds(userIds);
                }
            }

            return (userDetailsForUserIds, emailAddressesForOrganisationIds);
        }

        private string GetUserNameForComment(Models.Comment comment, Dictionary<string, (string displayName, string emailAddress)> userDetailsForUserIds)
		{
			if (comment.CommentByUserType == UserType.OrganisationalCommenter)
				return null;

            return comment.SubmissionComment != null && comment.SubmissionComment.Count > 0 && userDetailsForUserIds.ContainsKey(comment.SubmissionComment?.First().Submission.SubmissionByUserId)
				? userDetailsForUserIds[comment.SubmissionComment?.First().Submission.SubmissionByUserId].displayName
				: "Not found";
		}

		private string GetEmailForComment(Models.Comment comment, Dictionary<string, (string displayName, string emailAddress)> userDetailsForUserIds, Dictionary<int, string> emailAddressesForOrganisationIds)
		{
			if (comment.CommentByUserType == UserType.OrganisationalCommenter)
			{
                return comment.OrganisationUserId.HasValue && emailAddressesForOrganisationIds.ContainsKey(comment.OrganisationUserId.Value)
					? emailAddressesForOrganisationIds[comment.OrganisationUserId.Value]
					: "Not found";
			}

			return comment.SubmissionComment != null && comment.SubmissionComment.Count > 0 && userDetailsForUserIds.ContainsKey(comment.SubmissionComment?.First().Submission.SubmissionByUserId)
				? userDetailsForUserIds[comment.SubmissionComment?.First().Submission.SubmissionByUserId].emailAddress
				: "Not found";
		}

		private string GetUserNameForAnswer(Models.Answer answer, Dictionary<string, (string displayName, string emailAddress)> userDetailsForUserIds)
		{
			if (answer.AnswerByUserType == UserType.OrganisationalCommenter)
				return null;
			
			return answer.SubmissionAnswer != null && answer.SubmissionAnswer.Count > 0 && userDetailsForUserIds.ContainsKey(answer.SubmissionAnswer?.First().Submission.SubmissionByUserId)
				? userDetailsForUserIds[answer.SubmissionAnswer?.First().Submission.SubmissionByUserId].displayName
				: "Not found";
		}

		private string GetEmailForAnswer(Models.Answer answer, Dictionary<string, (string displayName, string emailAddress)> userDetailsForUserIds, Dictionary<int, string> emailAddressesForOrganisationIds)
		{
			if (answer.AnswerByUserType == UserType.OrganisationalCommenter)
			{
				return answer.OrganisationUserId.HasValue && emailAddressesForOrganisationIds.ContainsKey(answer.OrganisationUserId.Value)
					? emailAddressesForOrganisationIds[answer.OrganisationUserId.Value]
					: "Not found";
			}

			return answer.SubmissionAnswer != null && answer.SubmissionAnswer.Count > 0 && userDetailsForUserIds.ContainsKey(answer.SubmissionAnswer?.First().Submission.SubmissionByUserId)
				? userDetailsForUserIds[answer.SubmissionAnswer?.First().Submission.SubmissionByUserId].emailAddress
				: "Not found";
		}

		private Stylesheet CreateStyleSheet()
		{
			Stylesheet styleSheet = new Stylesheet();

			Fonts fonts = new Fonts(
				new Font( // Index 0 - default
					new FontSize() { Val = 10 }

				),
				new Font( // Index 1 - header
					new FontSize() { Val = 10 },
					new Bold(),
					new Color() { Rgb = "FFFFFF" }

				),
				new Font( // Index 1 - Title
					new FontSize() { Val = 16 },
					new Bold(),
					new Color() { Rgb = "000000" }

				));

			Fills fills = new Fills(
				new Fill(new PatternFill() { PatternType = PatternValues.None }), // Index 0 - default
				new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }), // Index 1 - should be able to remove this as it isn't used, it breaks if I do though
				new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "66666666" } })
					{ PatternType = PatternValues.Solid }) // Index 2 - header
			);

			Borders borders = new Borders(
				new Border(), // index 0 default
				new Border( // index 1 black border
					new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
					new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
					new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
					new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
					new DiagonalBorder())
			);

			CellFormats cellFormats = new CellFormats(
				new CellFormat(), // default
				new CellFormat { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true, Alignment = new Alignment() { WrapText = true } }, // body
				new CellFormat { FontId = 1, FillId = 2, BorderId = 1, ApplyFill = true, Alignment = new Alignment() { WrapText = true } }, // header
				new CellFormat { FontId = 2, FillId = 0, BorderId = 0, ApplyFill = true, Alignment = new Alignment() { WrapText = false } } // title
			);

			styleSheet = new Stylesheet(fonts, fills, borders, cellFormats);

			return styleSheet;
		}

		private async Task CreateSheet(SpreadsheetDocument spreadsheetDocument, IEnumerable<Models.Comment> comments, IEnumerable<Models.Answer> answers, IEnumerable<Models.Question> questions)
		{
			// Add a WorkbookPart to the document.
			WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
			workbookpart.Workbook = new Workbook();

			// Add a WorksheetPart to the WorkbookPart.
			WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
			worksheetPart.Worksheet = new Worksheet();

			// Add a Style to the workbook part
			WorkbookStylesPart stylesPart = workbookpart.AddNewPart<WorkbookStylesPart>();
			stylesPart.Stylesheet = CreateStyleSheet();
			stylesPart.Stylesheet.Save();

			worksheetPart.Worksheet.AppendChild(SetUpColumns());

			// Add Sheets to the Workbook.
			Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());

			//Add relationship -Fix for ipad to be able to read the sheets inside the xlsx file.
			string relationshipId = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart);
			string relationshipType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet";
			spreadsheetDocument.Package.GetPart(spreadsheetDocument.WorkbookPart.Uri).CreateRelationship(new Uri(worksheetPart.Uri.OriginalString.Replace("/xl/", String.Empty).Trim(), UriKind.Relative), TargetMode.Internal, relationshipType);
			spreadsheetDocument.Package.GetPart(spreadsheetDocument.WorkbookPart.Uri).DeleteRelationship(relationshipId);
			PackageRelationshipCollection sheetRelationships = spreadsheetDocument.Package.GetPart(spreadsheetDocument.WorkbookPart.Uri).GetRelationshipsByType(relationshipType);
			relationshipId = sheetRelationships.Where(f => f.TargetUri.OriginalString == worksheetPart.Uri.OriginalString.Replace("/xl/", String.Empty).Trim()).Single().Id;

			// Append a new worksheet and associate it with the spreadsheetDocument.
			Sheet sheet = new Sheet()
			{
				Id = relationshipId,
				SheetId = 1,
				Name = Constants.Export.SheetName
			};
			sheets.Append(sheet);

			workbookpart.Workbook.Save();

			// Add data to the worksheet
			SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

			var ConsultationTitle = await GetConsultationTitle(comments, questions);
			AppendTitleRow(sheetData, ConsultationTitle);

			var collatedDataAndExpressionOfInterestFlag = await CollateData(comments, answers, questions);

			AppendHeaderRow(sheetData, collatedDataAndExpressionOfInterestFlag.showOrganisationExpressionOfInterest);

			AppendDataRow(sheetData, collatedDataAndExpressionOfInterestFlag.collatedData, collatedDataAndExpressionOfInterestFlag.showOrganisationExpressionOfInterest);

			// Add filtering to the worksheet
			var headerCells = collatedDataAndExpressionOfInterestFlag.showOrganisationExpressionOfInterest ? "A3:O3" : "A3:N3";
			AutoFilter autoFilter = new AutoFilter() { Reference = headerCells };

			worksheetPart.Worksheet.Save();
			worksheetPart.Worksheet.Append(autoFilter);
			// Close the document.
			spreadsheetDocument.Close();
		}

		private async Task<string> GetConsultationTitle(IEnumerable<Models.Comment> comments, IEnumerable<Question> questions)
		{
			if (comments.Count() > 0)
				return await _exportService.GetConsultationName(comments.First().Location);
				
			if (questions.Count() > 0)
				return await _exportService.GetConsultationName(questions.First().Location);

			return "";
		}

		private void AppendTitleRow(SheetData sheet, string consultationTitle)
		{
			var titleRow = new Row();

			Cell cell = new Cell();
			cell.DataType = CellValues.String;
			cell.CellValue = new CellValue(consultationTitle + " consultation comments");
			cell.StyleIndex = 3;
			titleRow.AppendChild(cell);

			sheet.AppendChild(titleRow);

			var blankRow = new Row();
			sheet.AppendChild(blankRow);

		}
	}
}
