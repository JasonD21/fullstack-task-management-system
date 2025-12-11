using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Data.Entities;
using backend.DTOs.Task;

namespace backend.Services
{
    public class TaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;

        public TaskService(ApplicationDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto, Guid userId)
        {
            var hasAccess = await _context.Projects
                .Where(p => p.Id == createTaskDto.ProjectId)
                .SelectMany(p => p.Workspace.WorkspaceMembers)
                .AnyAsync(wm => wm.UserId == userId);

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("User does not have access to the specified project.");
            }

            var project = await _context.Projects.FindAsync(createTaskDto.ProjectId);
            var task = new TaskItem
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                ProjectId = createTaskDto.ProjectId,
                AssigneeId = createTaskDto.AssigneeId,
                DueDate = createTaskDto.DueDate,
                Priority = Enum.Parse<PriorityLevel>(createTaskDto.Priority)
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // After saving changes, send notification
            var workspace = await _context.Projects
                .Where(p => p.Id == createTaskDto.ProjectId)
                .Select(p => p.Workspace)
                .FirstOrDefaultAsync();

            var currentUser = await _context.Users.FindAsync(userId);

            if (workspace != null && currentUser != null)
            {
                await _notificationService.NotifyTaskCreated(
                    workspace.Id,
                    task.Id,
                    task.Title,
                    currentUser.FullName);
            }

            // Get assignee name if assigned
            string? assigneeName = null;
            if (task.AssigneeId.HasValue)
            {
                var assignee = await _context.Users.FindAsync(task.AssigneeId.Value);
                assigneeName = assignee?.FullName;
            }

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                ProjectId = task.ProjectId,
                ProjectName = project!.Name,
                AssigneeId = task.AssigneeId,
                AssigneeName = assigneeName,
                CommentCount = 0,
                AttachmentCount = 0
            };
        }

        public async Task<List<TaskDto>> GetTasksByProjectAsync(Guid projectId, Guid userId)
        {
            // Check if user has access to the project
            var hasAccess = await _context.Projects
            .Where(p => p.Id == projectId)
            .SelectMany(p => p.Workspace.WorkspaceMembers)
            .AnyAsync(wm => wm.UserId == userId);

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("You don't have access to this project");
            }

            var tasks = await _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    Priority = t.Priority.ToString(),
                    DueDate = t.DueDate,
                    CreatedAt = t.CreatedAt,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project.Name,
                    AssigneeId = t.AssigneeId,
                    AssigneeName = t.Assignee != null ? t.Assignee.FullName : null,
                    CommentCount = t.Comments.Count,
                    AttachmentCount = t.Attachments.Count
                })
                .ToListAsync();

            return tasks;
        }

        public async Task<TaskDto> UpdateTaskStatusAsync(Guid taskId, string newStatus, Guid userId)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                    .ThenInclude(p => p.Workspace)
                    .ThenInclude(w => w.WorkspaceMembers)
                    .FirstOrDefaultAsync(t => t.Id == taskId) ?? throw new KeyNotFoundException("Task not found");

            // Check if user has access to the task's workspace
            var hasAccess = task.Project.Workspace.WorkspaceMembers
                .Any(wm => wm.UserId == userId);

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("You don't have access to this task");
            }

            // Store old status for notification
            var oldStatus = task.Status.ToString();

            task.Status = Enum.Parse<Data.Entities.TaskStatus>(newStatus);
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // After updating status, send notification
            var currentUser = await _context.Users.FindAsync(userId);

            if (currentUser != null)
            {
                await _notificationService.NotifyTaskStatusChanged(
                    task.Project.WorkspaceId,
                    task.Id,
                    task.Title,
                    oldStatus.ToString(),
                    newStatus,
                    currentUser.FullName);
            }

            // Return updated task DTO
            var assigneeName = task.AssigneeId.HasValue
                ? (await _context.Users.FindAsync(task.AssigneeId.Value))?.FullName
                : null;

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                ProjectId = task.ProjectId,
                ProjectName = task.Project.Name,
                AssigneeId = task.AssigneeId,
                AssigneeName = assigneeName,
                CommentCount = task.Comments.Count,
                AttachmentCount = task.Attachments.Count
            };
        }
    }
}