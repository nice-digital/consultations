using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Comprehend;
using Amazon.Comprehend.Model;
using Amazon.Runtime;
using Comments.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Comments.Controllers.Api
{
    [Route("consultations/api/[controller]")]
    //[Authorize]
    public class TestController : Controller
    {
	    private readonly Amazon.Comprehend.IAmazonComprehend _amazonComprehend;
	    private readonly IServiceScopeFactory _serviceScopeFactory;

	    public TestController(IAmazonComprehend amazonComprehend, IServiceScopeFactory serviceScopeFactory)
	    {
		    _amazonComprehend = amazonComprehend;
		    _serviceScopeFactory = serviceScopeFactory;
	    }

	    /// <summary>
        /// GET: /consultations/api/Test
        /// 
        /// this controller is here temporarily to test the Auth.
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {


	        using (var scope = _serviceScopeFactory.CreateScope())
	        {
		        var context = scope.ServiceProvider.GetRequiredService<ConsultationsContext>();

		        var oneCommentHopefully = context.Comment.SingleOrDefault(comment => comment.CommentId.Equals(45013));

		        if (oneCommentHopefully != null)
		        {
			        return Content(oneCommentHopefully.CommentText);
		        }
	        }

				//String text = "It is raining today in Seattle";

				// Call DetectKeyPhrases API
				//Console.WriteLine("Calling DetectSentiment");
				//      DetectSentimentRequest detectSentimentRequest = new DetectSentimentRequest()
				//      {
				//       Text = text,
				//       LanguageCode = "en"
				//      };
				//      DetectSentimentResponse detectSentimentResponse = await _amazonComprehend.DetectSentimentAsync(detectSentimentRequest);

				//return Content(detectSentimentResponse.Sentiment);

				return Content("Done");
        }
    }
}
