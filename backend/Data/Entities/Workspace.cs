namespace backend.Data.Entities
{
    public class Workspace : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid OwnerId { get; set; }

        //Navigation props
        public virtual User Owner { get; set; } = null!;
        public virtual ICollection<WorkspaceMember> Members { get; set; } = [];
        public virtual ICollection<Project> Projects { get; set; } = [];
    }
}