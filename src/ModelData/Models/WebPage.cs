namespace ModelData.Models

{
    public class WebPage
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string Content { get; set; } = null!;
        public bool IsPublished { get; set; }
        public bool ShowInMenu { get; set; }
        public int MenuSortOrder { get; set; }
        public bool ShowInFooter { get; set; }
        public int SortOrder { get; set; }


        public bool EnableDirectLink { get; set; }
        public bool DirectLinkOpenInNewTab { get; set; }
        public string? DirectLinkUrl { get; set; }
        public string? ImageUrl { get; set; }

        public int? WebPageFormId { get; set; }
        public WebPageForm? Form { get; set; }
    }

}
