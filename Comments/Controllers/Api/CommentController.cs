using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Api
{
    [Produces("application/json")]
    [Route("consultations/api/[controller]")]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentsController> _logger;

        public CommentController(ICommentService commentService, ILogger<CommentsController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        // GET: consultations/api/Comment/5 
        [HttpGet("{commentId}")]
        public IActionResult GetComment([FromRoute] int commentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _commentService.GetComment(commentId);

            var invalidResult = Validate(result.validate, _logger);
            return invalidResult ?? Ok(result.comment);
        }

        // PUT: consultations/api/Comment/5
        [HttpPut("{commentId}")]
        public IActionResult PutComment([FromRoute] int commentId, [FromBody] ViewModels.Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (commentId != comment.CommentId)
            {
                return BadRequest();
            }

            var result = _commentService.EditComment(commentId, comment);
            var invalidResult = Validate(result.validate, _logger);

            return invalidResult ?? Ok(comment);
        }

        // POST: consultations/api/Comment
        [HttpPost("")]
        public IActionResult PostComment([FromBody] ViewModels.Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _commentService.CreateComment(comment);
            var invalidResult = Validate(result.validate, _logger);

            return invalidResult ?? CreatedAtAction("GetComment", new { commentId = result.comment.CommentId }, result.comment);
        }

        // DELETE: consultations/api/Comment/5
        [HttpDelete("{commentId}")]
        public IActionResult DeleteComment([FromRoute] int commentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _commentService.DeleteComment(commentId);
            var invalidResult = Validate(result.validate, _logger);

            return invalidResult ?? Ok(result.rowsUpdated);
        }
    }
}
