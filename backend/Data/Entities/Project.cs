namespace backend.Data.Entities
{
    public class Project : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid WorkspaceId { get; set; }

        //Navigation props
        public virtual Workspace Workspace { get; set; } = null!;
        public virtual ICollection<TaskItem> Tasks { get; set; } = [];
    }
}