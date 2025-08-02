namespace ModelData.Models

{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Content { get; set; } = null!;
        public string? ShortDescription { get; set; } = null!;
        public int? CategoryId { get; set; } = null;
        public Category Category { get; set; } = null!;

        public DateTime Date { get; set; }
        public DateTime? PublishedAt { get; set; }
        public bool IsPublished { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsBreakingNews { get; set; }
        public bool ShowInHero { get; set; }
        public int HeroOrder { get; set; }
        public bool ShowInSurface { get; set; }
        public bool CommentsEnabled { get; set; }
        public int SortOrder { get; set; }
        public int ViewCount { get; set; }

        public ICollection<Comment>? Comments { get; set; } = new List<Comment>(); 
        public ICollection<PostImage>? PostImages { get; set; } = new List<PostImage>();
         

        public string? PostBy { get;set; }


        public bool IsScheduled { get; set; } = false; // True if scheduled for future

        public List<string> Tags { get; set; } = new();
    }

}
