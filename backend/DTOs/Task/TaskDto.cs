namespace backend.DTOs.Task
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public Guid? AssigneeId { get; set; }
        public string? AssigneeName { get; set; }
        public int CommentCount { get; set; }
        public int AttachmentCount { get; set; }
    }
}