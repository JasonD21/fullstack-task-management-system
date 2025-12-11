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

        public async Task<List<WorkspaceMemberDto>> GetWorkspaceMembersAsync(Guid workspaceId, Guid userId)
        {
            // Check if user is member of the workspace
            var isMember = await _context.WorkspaceMembers
                .AnyAsync(wm => wm.WorkspaceId == workspaceId && wm.UserId == userId);

            if (!isMember)
            {
                throw new UnauthorizedAccessException("You are not a member of this workspace");
            }

            var members = await _context.WorkspaceMembers
                .Where(wm => wm.WorkspaceId == workspaceId)
                .Include(wm => wm.User)
                .Select(wm => new WorkspaceMemberDto
                {
                    Id = wm.Id,
                    UserId = wm.UserId,
                    UserName = wm.User.FullName,
                    UserEmail = wm.User.Email!,
                    Role = wm.Role,
                    JoinedAt = wm.CreatedAt
                })
                .ToListAsync();

            return members;
        }

        public async Task<WorkspaceMemberDto> AddMemberToWorkspaceAsync(Guid workspaceId, string userEmail, string role, Guid currentUserId)
        {
            // Check if current user is admin of the workspace
            var currentUserRole = await _context.WorkspaceMembers
                .Where(wm => wm.WorkspaceId == workspaceId && wm.UserId == currentUserId)
                .Select(wm => wm.Role)
                .FirstOrDefaultAsync();

            if (currentUserRole != "Admin")
            {
                throw new UnauthorizedAccessException("Only admins can add members");
            }

            var userToAdd = await _userManager.FindByEmailAsync(userEmail) ?? throw new KeyNotFoundException("User not found");

            // Check if user is already a member
            var existingMember = await _context.WorkspaceMembers
                .FirstOrDefaultAsync(wm => wm.WorkspaceId == workspaceId && wm.UserId == userToAdd.Id);

            if (existingMember != null)
            {
                throw new InvalidOperationException("User is already a member of this workspace");
            }

            var member = new WorkspaceMember
            {
                WorkspaceId = workspaceId,
                UserId = userToAdd.Id,
                Role = role
            };

            _context.WorkspaceMembers.Add(member);
            await _context.SaveChangesAsync();

            // Notify the new member
            // (We'll implement email notification later)

            return new WorkspaceMemberDto
            {
                Id = member.Id,
                UserId = member.UserId,
                UserName = userToAdd.FullName,
                UserEmail = userToAdd.Email!,
                Role = member.Role,
                JoinedAt = member.CreatedAt
            };
        }
    }
}