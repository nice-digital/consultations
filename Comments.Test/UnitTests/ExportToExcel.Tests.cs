using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Comments.Export;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;

namespace Comments.Test.UnitTests
{
    public class ExportToExcelTests : TestBase
    {
	 //   [Fact]
	 //   public void DocumentCreated()
	 //   {
		//    ExportToExcel exportToExcel = new ExportToExcel();
		//    var stream = exportToExcel.ToSpreadsheet();

		//    using (FileStream file = new FileStream("C:\\Test\\File.xlsx", FileMode.Create, System.IO.FileAccess.Write))
		//    {
		//	    byte[] bytes = new byte[stream.Length];
		//	    stream.Read(bytes, 0, (int)stream.Length);
		//	    file.Write(bytes, 0, bytes.Length);
		//	    stream.Close();
		//    }
		//}


	    [Fact]
	    public async Task Create_Spreadsheet()
	    {
			// Arrange
		    ResetDatabase();
		    _context.Database.EnsureCreated();
		    var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
		    var userId = Guid.Empty;

			var locationId = AddLocation(sourceURI);
		    var commentId = AddComment(locationId, "This is my comment text", isDeleted: false, createdByUserId: userId);

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
		    var authenticateService = new FakeAuthenticateService(authenticated: true);
		    var context = new ConsultationsContext(_options, userService, _fakeEncryption);
		    var commentService = new CommentService(context, userService, authenticateService, _consultationService);


		    var viewModel = commentService.GetComment(commentId);

			// Act
			//var response = await _client.GetAsync("/consultations/api/export");
			ExportToExcel toExcel = new ExportToExcel();
		    toExcel.ToConvert(viewModel.comment);

	    }

	}
}
