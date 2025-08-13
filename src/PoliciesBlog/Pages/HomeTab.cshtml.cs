using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.DotNet;
using ModelData.Data;
using ModelData.Models;
using ModelData.Services;
using System;

namespace PoliciesBlog.Pages
{

    public class HomeTabModel : PageModel
    {
        private readonly IPostServices _postService;
        private readonly ApplicationDbContext _context;

        public HomeTabModel(IPostServices postService, ApplicationDbContext context)
        {
            _postService = postService;
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string Tab { get; set; }
        public List<Post> Posts { get; set; } = new();


        [BindProperty(SupportsGet = true)]
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 24;
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Tab = Tab?.ToLower() ?? "featured";
            var now = DateTime.UtcNow;
            var recent3 = now.AddDays(-3);
            var recent7 = now.AddDays(-7);

            IQueryable<Post> query = _context.Posts
                                .Include(x => x.PostImages)
.Where(p => p.IsPublished);

            switch (Tab)
            {
                case "popular":
                case "watched":
                    query = query.OrderByDescending(p => p.ViewCount);
                    break;

                case "hot":
                    query = query.Where(p => p.PublishedAt >= recent3)
                                 .OrderByDescending(p => p.ViewCount);
                    break;

                case "trending":
                    query = query.Where(p => p.PublishedAt >= recent7)
                                 .OrderByDescending(p => p.ViewCount);
                    break;

                case "featured":
                default:
                    query = query.Where(p => p.IsFeatured)
                                 .OrderByDescending(p => p.PublishedAt);
                    break;
            }

            TotalPages = (int)Math.Ceiling(query.Count() / (double)PageSize);
            CurrentPage = Page;

            Posts = query
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

            ViewData["Title"] = Tab;
            ViewData["MetaTitle"] = Tab;
            ViewData["MetaDescription"] = $"Latest posts in {Tab}. Discover recent news and insights.";

            ViewData["MetaPublished"] = DateTime.UtcNow.ToString("o");
            ViewData["MetaImage"] = fullImageUrl;
            ViewData["MetaUrl"] = $"{baseUrl}/hometab/{Tab}";

            return Page();
        }


        public async Task<JsonResult> OnGetLoadMoreAsync([FromQuery] string tab, [FromQuery] int page)
        {

            tab = tab?.ToLower() ?? "featured";
            var now = DateTime.UtcNow;
            var recent3 = now.AddDays(-3);
            var recent7 = now.AddDays(-7);

            IQueryable<Post> query = _context.Posts
                .Include(x => x.PostImages)
                .Where(p => p.IsPublished);

            switch (tab)
            {
                case "popular":
                case "watched":
                    query = query.OrderByDescending(p => p.ViewCount);
                    break;

                case "hot":
                    query = query.Where(p => p.PublishedAt >= recent3)
                                 .OrderByDescending(p => p.ViewCount);
                    break;

                case "trending":
                    query = query.Where(p => p.PublishedAt >= recent7)
                                 .OrderByDescending(p => p.ViewCount);
                    break;

                case "featured":
                default:
                    query = query.Where(p => p.IsFeatured)
                                 .OrderByDescending(p => p.PublishedAt);
                    break;
            }

            var pageSize = PageSize;
            var pagedPosts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Slug,
                    p.PostBy,
                    PublishedAt = p.PublishedAt != null
    ? p.PublishedAt.Value.ToString("MMMM dd, yyyy hh:mm tt")
    : null,
                    p.ShortDescription,
                    ImageUrl = p.PostImages.FirstOrDefault(pi => pi.IsDefault).ImageUrl ?? "/assets/default.jpg",
                    Category = p.Category.Title
                })
                .ToListAsync();

            return new JsonResult(new { success = true, posts = pagedPosts });
        }

    }


}
