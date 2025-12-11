using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using backend.DTOs.Project;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectService _projectService;

        public ProjectsController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto createDto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var project = await _projectService.CreateProjectAsync(
                createDto.WorkspaceId, createDto, userId);

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        [HttpGet("workspace/{workspaceId}")]
        public async Task<IActionResult> GetProjectsByWorkspace(Guid workspaceId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var projects = await _projectService.GetProjectsByWorkspaceAsync(workspaceId, userId);

            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(Guid id)
        {
            // We'll implement this later
            return Ok(new { message = "Get project by ID - to be implemented" });
        }
    }


}