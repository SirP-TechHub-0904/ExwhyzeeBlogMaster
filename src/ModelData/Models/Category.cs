namespace ModelData.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public bool IsPublished { get; set; }
        public int SortOrder { get; set; }

        public bool ShowInMenu { get; set; }
        public bool ShowInFooter { get; set; }
        public bool ShowInHome { get; set; } /////top stories.
        public bool ShowInHomeMain { get; set; }
        public bool ShowInHomeList { get; set; }

        public int? ParentCategoryId { get; set; } = null;
        public Category? ParentCategory { get; set; } = null;
        public ICollection<Category>? Children { get; set; } = new List<Category>();

        public ICollection<Post>? Posts { get; set; } = new List<Post>();
    }

}
