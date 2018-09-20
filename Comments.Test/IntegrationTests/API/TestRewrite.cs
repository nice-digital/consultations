using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Comments.Models;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using Question = Comments.ViewModels.Question;
using Status = Comments.Models.Status;

namespace Comments.Test.IntegrationTests.API
{
    public class TestRewrite : IClassFixture<CustomWebApplicationFactory<Startup>>
	{
		private readonly CustomWebApplicationFactory<Startup> _factory;
		public TestRewrite(CustomWebApplicationFactory<Startup> factory)
		{
			_factory = factory;
		}
		[Fact]
		public async Task Create_Answer()
		{
			// Arrange
			var answer = new ViewModels.Answer(0, "answer text", false, DateTime.Now, Guid.Empty, 1, (int)StatusName.Draft);
			var content = new StringContent(JsonConvert.SerializeObject(answer), Encoding.UTF8, "application/json");

			var client = _factory.CreateClient();
			var host = _factory.Server?.Host;
			SeedData(host);

			var response = await client.PostAsync("/consultations/api/answer", content);
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.Created);
			var deserialisedAnswer = JsonConvert.DeserializeObject<ViewModels.Answer>(responseString);
			deserialisedAnswer.AnswerId.ShouldBeGreaterThan(0);
		}


		private void SeedData(IWebHost host)
		{
			if (host == null) { throw new ArgumentNullException("host"); }
			using (var scope = host.Services.CreateScope())
			{
				var scoptedServices = scope.ServiceProvider;
				var context = scoptedServices.GetRequiredService<ConsultationsContext>();

				try
				{
					context.Status.Add(new Status
					{
						Comment = null,
						Answer = null,
						Name = "Draft",
						StatusId = 1
					});

					context.Status.Add(new Status
					{
						Comment = null,
						Answer = null,
						Name = "Submitted",
						StatusId = 2
					});

					context.SaveChanges();
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					throw;
				}



			}
		}



		[Fact]
		public async Task Create_Question()
		{
			//Arrange
			var question = new Question(
				new Models.Location("consultations://./consultation/1/document/1/chapter/introduction", null, null, null, null,
					null, "quote", null, "section 1", null, null),
				new Models.Question(1, "question text", 1, null, new Models.QuestionType("description", true, false, null), null));

			var content = new StringContent(JsonConvert.SerializeObject(question), Encoding.UTF8, "application/json");

			var client = _factory.CreateClient();


			//Act
			var response = await client.PostAsync("consultations/api/Question", content);
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.Created);
			var deserialisedAnswer = JsonConvert.DeserializeObject<ViewModels.Question>(responseString);
			deserialisedAnswer.QuestionId.ShouldBeGreaterThan(0);
		}


	    
		//protected int AddStatus(string statusName, int statusIdId = (int)StatusName.Draft)
	 //   {
		//    var _authenticated = true;
		//    var _displayName = "Benjamin Button";
		//    var _userId = Guid.Empty;
		//	var _fakeUserService = FakeUserService.Get(_authenticated, _displayName, _userId);
		//    var _fakeEncryption = new FakeEncryption();


		//	var statusModel = new Models.Status("Draft", null, null);
		   
		//	using (var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
		//	{
		//		context.Status.Add(statusModel);
		//		context.SaveChanges();
		//	} 

		//    return statusModel.StatusId;
	 //   }
	}
}
