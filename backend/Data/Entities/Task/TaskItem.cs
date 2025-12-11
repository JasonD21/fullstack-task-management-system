using System.Diagnostics;

namespace backend.Data.Entities
{
    public class TaskItem : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.ToDo;
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
        public DateTime? DueDate { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? AssigneeId { get; set; }

        //Navigation props
        public virtual Project Project { get; set; } = null!;
        public virtual User? Assignee { get; set; }
        public virtual ICollection<TaskComment> Comments { get; set; } = [];
        public virtual ICollection<TaskAttachment> Attachments { get; set; } = [];
    }

    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Review,
        Done
    }

    public enum PriorityLevel
    {
        Low,
        Medium,
        High,
        Critical
    }
}