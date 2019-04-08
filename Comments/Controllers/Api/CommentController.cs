using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
		/// <summary>
		/// Get single comment from id
		/// </summary>
		/// <param name="commentId">id of the comment</param>
		/// <returns>Return the comment view model</returns>
		[HttpGet("{commentId}")]
        [ProducesResponseType(typeof((Comment, Validate)), StatusCodes.Status200OK)]
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
		/// <summary>
		/// Updates an comment based on comment Id
		/// </summary>
		/// <param name="commentId">Id of the comment</param>
		/// <param name="comment">The values the comment is to be updated with</param>
		/// <returns>Return the comment view model</returns>
		[HttpPut("{commentId}")]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
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
		/// <summary>
		/// Adds a single comment
		/// </summary>
		/// <param name="comment">comment view model</param>
		/// <returns>Returns the comment view model and Validate model</returns>
		[HttpPost]
        [ProducesResponseType(typeof((Comment,Validate)), StatusCodes.Status200OK)]
		public IActionResult PostComment([FromBody] ViewModels.Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _commentService.CreateComment(comment);
            var invalidResult = Validate(result.validate, _logger);

            return invalidResult ?? CreatedAtAction("GetComment", new { id = result.comment.CommentId }, result.comment);
        }

		// DELETE: consultations/api/Comment/5
		/// <summary>
		/// Deletes an individual comment specified by comment Id
		/// </summary>
		/// <param name="commentId">Id of the comment to be deleted</param>
		/// <returns>Returns the number of rows affected</returns>
		[HttpDelete("{commentId}")]
        [ProducesResponseType(typeof((int, Validate)), StatusCodes.Status200OK)]
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
