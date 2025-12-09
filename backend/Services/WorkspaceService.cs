using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Data.Entities;
using backend.DTOs.Workspace;
using Microsoft.AspNetCore.Identity;

namespace backend.Services
{
    public class WorkspaceService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public WorkspaceService(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<WorkspaceDto> CreateWorkspaceAsync(Guid ownerId, CreateWorkspaceDto createDto)
        {
            var owner = await _userManager.FindByIdAsync(ownerId.ToString());
            var workspace = new Workspace
            {
                Name = createDto.Name,
                Description = createDto.Description,
                OwnerId = ownerId
            };
            _context.Workspaces.Add(workspace);

            // Add owner as a admin member of the workspace
            var AdminMember = new WorkspaceMember
            {
                WorkspaceId = workspace.Id,
                UserId = ownerId,
                Role = "Admin"
            };
            _context.WorkspaceMembers.Add(AdminMember);

            await _context.SaveChangesAsync();

            return new WorkspaceDto
            {
                Id = workspace.Id,
                Name = workspace.Name,
                Description = workspace.Description,
                CreatedAt = workspace.CreatedAt,
                OwnerId = workspace.OwnerId,
                OwnerName = owner!.FullName
            };
        }

        public async Task<List<WorkspaceDto>> GetUserWorkspaceAsync(Guid userId)
        {
            var workspaces = await _context.WorkspaceMembers
            .Where(wm => wm.UserId == userId)
            .Include(wm => wm.Workspace)
            .ThenInclude(w => w.Owner).Select(wm => new WorkspaceDto
            {
                Id = wm.Workspace.Id,
                Name = wm.Workspace.Name,
                Description = wm.Workspace.Description,
                CreatedAt = wm.Workspace.CreatedAt,
                OwnerId = wm.Workspace.OwnerId,
                OwnerName = wm.Workspace.Owner.FullName
            })
            .ToListAsync();

            return workspaces;
        }
    }
}