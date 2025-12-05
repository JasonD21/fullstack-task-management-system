namespace backend.Data.Entities
{
    public class WorkspaceMember : BaseEntity
    {
        public Guid WorkspaceId { get; set; }
        public Guid UserId { get; set; }
        public string Role { get; set; } = "Member"; // Admin, Member, Viewer

        //Navigation props
        public virtual Workspace Workspace { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}