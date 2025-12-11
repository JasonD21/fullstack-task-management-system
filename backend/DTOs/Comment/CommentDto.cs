namespace backend.DTOs.Comment
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid TaskId { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string? AuthorAvatar { get; set; }
    }
}