namespace backend.DTOs.Comment
{
    public class CreateCommentDto
    {
        public string Content { get; set; } = string.Empty;
        public Guid TaskId { get; set; }
    }
}