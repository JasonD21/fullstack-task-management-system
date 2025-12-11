using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using backend.DTOs.Comment;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly CommentService _commentService;

        public CommentsController(CommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createDto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var comment = await _commentService.CreateCommentAsync(createDto, userId);

            return CreatedAtAction(nameof(GetCommentsByTask), new { taskId = comment.TaskId }, comment);
        }

        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetCommentsByTask(Guid taskId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var comments = await _commentService.GetCommentsByTaskAsync(taskId, userId);

            return Ok(comments);
        }
    }
}