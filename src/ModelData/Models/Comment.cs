namespace ModelData.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; } = null!;

        public string AuthorName { get; set; } = null!;
        public string? AuthorEmail { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsApproved { get; set; }

        public string? UserId { get; set; } = null;
        public ApplicationUser? User { get; set; } = null;
    }

}
