namespace backend.Data.Entities
{
    public class TaskComment : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public Guid TaskId { get; set; }
        public Guid AuthorId { get; set; }

        //Navigation props
        public virtual TaskItem Task { get; set; } = null!;
        public virtual User Author { get; set; } = null!;
    }
}