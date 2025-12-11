using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Data.Entities;
using backend.DTOs.Project;

namespace backend.Services
{
    public class ProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<ProjectDto> CreateProjectAsync(Guid workspaceId, CreateProjectDto createDto, Guid userId)
        {
            // Check if user is member of the workspace
            var isMember = await _context.WorkspaceMembers
                .AnyAsync(wm => wm.WorkspaceId == workspaceId && wm.UserId == userId);
            if (!isMember)
            {
                throw new UnauthorizedAccessException("You are not a member of this workspace");
            }

            var workspace = await _context.Workspaces.FindAsync(workspaceId);

            var project = new Project
            {
                Name = createDto.Name,
                Description = createDto.Description,
                WorkspaceId = workspaceId,
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                WorkspaceId = project.WorkspaceId,
                WorkspaceName = workspace!.Name,
                TaskCount = 0
            };
        }

        public async Task<List<ProjectDto>> GetProjectsByWorkspaceAsync(Guid workspaceId, Guid userId)
        {
            // Check if user is member of the workspace
            var isMember = await _context.WorkspaceMembers
                .AnyAsync(wm => wm.WorkspaceId == workspaceId && wm.UserId == userId);
            if (!isMember)
            {
                throw new UnauthorizedAccessException("You are not a member of this workspace");
            }

            var projects = await _context.Projects
                .Where(p => p.WorkspaceId == workspaceId)
                .Include(p => p.Workspace)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    WorkspaceId = p.WorkspaceId,
                    WorkspaceName = p.Workspace.Name,
                    TaskCount = p.Tasks.Count
                })
                .ToListAsync();

            return projects;
        }
    }
}