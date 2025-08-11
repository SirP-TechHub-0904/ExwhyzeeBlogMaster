using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;
using ModelData.Services;

namespace PeopleNewsBlog.Pages
{
    public class CategoryPageModel : PageModel
    {
        private readonly IPostServices _postService;
        private readonly ApplicationDbContext _context;

        public CategoryPageModel(IPostServices postService, ApplicationDbContext context)
        {
            _postService = postService;
            _context = context;
        }

        public Category? Category { get; set; }
        public List<Post> Posts { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string Slug { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 24;
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Slug))
                return NotFound();

            Category = await _context.Categories.FirstOrDefaultAsync(x => x.Slug == Slug);
            if (Category == null)
                return NotFound();

            var allPosts = await _postService.GetPostsByCategory(Category.Id);
            TotalPages = (int)Math.Ceiling(allPosts.Count / (double)PageSize);
            CurrentPage = Page;

            Posts = allPosts
                .OrderByDescending(p => p.PublishedAt)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();


            // === Meta Tag Assignments ===
            var request = HttpContext.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}";
            string imageUrl = Posts.FirstOrDefault()?.PostImages?.FirstOrDefault(pi => pi.IsDefault)?.ImageUrl ?? "/assets/default.jpg";
            string fullImageUrl = imageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? imageUrl
                : $"{baseUrl}{imageUrl}";

            ViewData["Title"] = Category.Title;
            ViewData["MetaTitle"] = Category.Title;
            ViewData["MetaDescription"] = $"Latest posts in {Category.Title}. Discover recent news and insights.";
         
            ViewData["MetaPublished"] = DateTime.UtcNow.ToString("o");
            ViewData["MetaImage"] = fullImageUrl;
            ViewData["MetaUrl"] = $"{baseUrl}/category/{Category.Slug}";

            return Page();
        }


        public async Task<JsonResult> OnGetLoadMoreAsync(string slug, int page)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Slug == slug);
            if (category == null) return new JsonResult(new { success = false });

            var allPosts = await _postService.GetPostsByCategory(category.Id);
            var pageSize = PageSize;
            var pagedPosts = allPosts
                .OrderByDescending(p => p.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new {
                    p.Id,
                    p.Title,
                    p.Slug,
                    p.PostBy,
                    PublishedAt = p.PublishedAt?.ToString("MMMM dd, yyyy hh:mm tt"),
                    p.ShortDescription,
                    ImageUrl = p.PostImages?.FirstOrDefault(pi => pi.IsDefault)?.ImageUrl ?? "/assets/default.jpg",
                    Category = new
                    {
                        Slug = p.Category?.Slug,
                        Title = p.Category?.Title
                    }
                })
                .ToList();

            return new JsonResult(new { success = true, posts = pagedPosts });
        }

    }
}
