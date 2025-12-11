using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using backend.DTOs.Task;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly TaskService _taskService;

        public TasksController(TaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createDto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var task = await _taskService.CreateTaskAsync(createDto, userId);

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetTasksByProject(Guid projectId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var tasks = await _taskService.GetTasksByProjectAsync(projectId, userId);

            return Ok(tasks);
        }

        [HttpPatch("{taskId}/status")]
        public async Task<IActionResult> UpdateTaskStatus(Guid taskId, [FromBody] UpdateTaskStatusDto updateDto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var task = await _taskService.UpdateTaskStatusAsync(taskId, updateDto.Status, userId);

            return Ok(task);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(Guid id)
        {
            return Ok(new { message = "Get task by ID - to be implemented" });
        }
    }
}