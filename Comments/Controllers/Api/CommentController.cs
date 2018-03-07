using Comments.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Api
{
    [Produces("application/json")]
    [Route("consultations/api/Comment")]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<DocumentController> _logger;

        public CommentController(ICommentService commentService, ILogger<DocumentController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        // GET: api/Comment/5
        [HttpGet("{commentId}")]
        public IActionResult GetComment([FromRoute] int commentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comment = _commentService.GetComment(commentId);

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        // PUT: api/Comment/5
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

            var rowsUpdated = _commentService.EditComment(commentId, comment);

            if (rowsUpdated > 0)
            {
                return NoContent();
            }

            return NotFound();
        }

        // POST: api/Comment
        [HttpPost]
        public IActionResult PostComment([FromBody] ViewModels.Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var savedComment = _commentService.CreateComment(comment);

            return CreatedAtAction("GetComment", new { id = savedComment.CommentId }, savedComment);
        }

        // DELETE: api/Comment/5
        [HttpDelete("{commentId}")]
        public IActionResult DeleteComment([FromRoute] int commentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rowsUpdated = _commentService.DeleteComment(commentId);
            
            if (rowsUpdated < 1)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}