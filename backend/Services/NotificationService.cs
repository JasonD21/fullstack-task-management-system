using Microsoft.AspNetCore.SignalR;
using backend.hubs;

namespace backend.Services
{
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyTaskCreated(Guid workspaceId, Guid taskId, string taskTitle, string createdBy)
        {
            await _hubContext.Clients.Group($"workspace-{workspaceId}")
                .SendAsync("TaskCreated", new
                {
                    TaskId = taskId,
                    Title = taskTitle,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                });
        }

        public async Task NotifyTaskStatusChanged(Guid workspaceId, Guid taskId, string taskTitle,
            string oldStatus, string newStatus, string changedBy)
        {
            await _hubContext.Clients.Group($"workspace-{workspaceId}")
                .SendAsync("TaskStatusChanged", new
                {
                    TaskId = taskId,
                    Title = taskTitle,
                    OldStatus = oldStatus,
                    NewStatus = newStatus,
                    ChangedBy = changedBy,
                    ChangedAt = DateTime.UtcNow
                });
        }

        public async Task NotifyUser(Guid userId, string message, string type = "info")
        {
            await _hubContext.Clients.Group($"user-{userId}")
                .SendAsync("Notification", new
                {
                    Message = message,
                    Type = type,
                    SentAt = DateTime.UtcNow
                });
        }
    }
}