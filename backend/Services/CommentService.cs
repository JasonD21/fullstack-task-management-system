using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Data.Entities;
using backend.DTOs.Comment;

namespace backend.Services
{
    public class CommentService
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;

        public CommentService(ApplicationDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<CommentDto> CreateCommentAsync(CreateCommentDto createDto, Guid userId)
        {
            // Check if user has access to the task
            var hasAccess = await _context.Tasks
                .Where(t => t.Id == createDto.TaskId)
                .SelectMany(t => t.Project.Workspace.WorkspaceMembers)
                .AnyAsync(wm => wm.UserId == userId);

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("You don't have access to this task");
            }

            var task = await _context.Tasks
                .Include(t => t.Project)
                    .ThenInclude(p => p.Workspace)
                .FirstOrDefaultAsync(t => t.Id == createDto.TaskId);

            var author = await _context.Users.FindAsync(userId);

            var comment = new TaskComment
            {
                Content = createDto.Content,
                TaskId = createDto.TaskId,
                AuthorId = userId
            };

            _context.TaskComments.Add(comment);
            await _context.SaveChangesAsync();

            // Notify workspace members about new comment
            if (task != null && author != null)
            {
                await _notificationService.NotifyUser(
                    task.AssigneeId ?? Guid.Empty,
                    $"New comment on task '{task.Title}' by {author.FullName}",
                    "info");
            }

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                TaskId = comment.TaskId,
                AuthorId = comment.AuthorId,
                AuthorName = author!.FullName,
                AuthorAvatar = author.AvatarUrl
            };
        }

        public async Task<List<CommentDto>> GetCommentsByTaskAsync(Guid taskId, Guid userId)
        {
            // Check if user has access to the task
            var hasAccess = await _context.Tasks
                .Where(t => t.Id == taskId)
                .SelectMany(t => t.Project.Workspace.WorkspaceMembers)
                .AnyAsync(wm => wm.UserId == userId);

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("You don't have access to this task");
            }

            var comments = await _context.TaskComments
                .Where(c => c.TaskId == taskId)
                .Include(c => c.Author)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    TaskId = c.TaskId,
                    AuthorId = c.AuthorId,
                    AuthorName = c.Author.FullName,
                    AuthorAvatar = c.Author.AvatarUrl
                })
                .ToListAsync();

            return comments;
        }
    }
}