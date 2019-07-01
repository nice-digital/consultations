using System;
using System.Threading.Tasks;
using Amazon.Comprehend;
using Amazon.Comprehend.Model;
using Amazon.Runtime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comments.Controllers.Api
{
    [Route("consultations/api/[controller]")]
    //[Authorize]
    public class TestController : Controller
    {
	    private readonly Amazon.Comprehend.IAmazonComprehend _amazonComprehend;

	    public TestController(IAmazonComprehend amazonComprehend)
	    {
		    _amazonComprehend = amazonComprehend;
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

	        String text = "It is raining today in Seattle";
	        
			// Call DetectKeyPhrases API
			Console.WriteLine("Calling DetectSentiment");
	        DetectSentimentRequest detectSentimentRequest = new DetectSentimentRequest()
	        {
		        Text = text,
		        LanguageCode = "en"
	        };
	        DetectSentimentResponse detectSentimentResponse = await _amazonComprehend.DetectSentimentAsync(detectSentimentRequest);
	        
			return Content(detectSentimentResponse.Sentiment);
        }
    }
}
