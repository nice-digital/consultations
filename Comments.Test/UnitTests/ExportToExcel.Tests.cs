using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Comments.Export;
using Comments.ViewModels;
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
	    public async Task Create_Answer()
	    {
		    // Arrange

		    // Act
		    var response = await _client.PostAsync("/consultations/api/export", null);
	    }

	}
}
