using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using backend.DTOs.Workspace;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkspacesController : ControllerBase
    {
        private readonly WorkspaceService _workspaceService;

        public WorkspacesController(WorkspaceService workspaceService)
        {
            _workspaceService = workspaceService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateWorkspace([FromBody] CreateWorkspaceDto createDto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var workspace = await _workspaceService.CreateWorkspaceAsync(userId, createDto);

            return CreatedAtAction(nameof(GetWorkspace), new { id = workspace.Id }, workspace);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserWorkspace()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var workspaces = await _workspaceService.GetUserWorkspaceAsync(userId);

            if (workspaces == null)
            {
                return NotFound(new { message = "Workspace not found." });
            }

            return Ok(workspaces);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkspace(Guid id)
        {
            return Ok(new { message = "This is a placeholder for GetWorkspace by ID." });
        }
    }
}