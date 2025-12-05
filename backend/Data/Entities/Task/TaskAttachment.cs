namespace backend.Data.Entities
{
    public class TaskAttachment : BaseEntity
    {
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public Guid TaskId { get; set; }
        public Guid UploadedById { get; set; }

        //Navigation props
        public virtual TaskItem TaskItem { get; set; } = null!;
        public virtual User UploadedBy { get; set; } = null!;
    }
}