using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Models;
using ModelData.Services;

namespace AfriRanking.Pages
{
    public class AllNewsModel : PageModel
    {
        private readonly IPostServices _postService;

        public AllNewsModel(IPostServices postService)
        {
            _postService = postService;
        }

        public List<Post> Posts { get; set; } = new();
        public int PageSize { get; set; } = 10;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public async Task<IActionResult> OnGetAsync(int page = 1)
        {
            CurrentPage = page;
            var allPosts = await _postService.GetPublishedPosts();

            var skippedPosts = allPosts.Skip(11).ToList();
            TotalPages = (int)Math.Ceiling(skippedPosts.Count / (double)PageSize);

            Posts = skippedPosts
                .OrderByDescending(p => p.PublishedAt)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return Page();
        }

        public async Task<JsonResult> OnGetLoadMoreAsync(int page)
        {
            var allPosts = await _postService.GetPublishedPosts();
            var skippedPosts = allPosts.Skip(11).ToList();

            var pagedPosts = skippedPosts
                .OrderByDescending(p => p.PublishedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Slug,
                    p.PostBy,
                    PublishedAt = p.PublishedAt?.ToString("MMMM dd, yyyy hh:mm tt"),
                    p.ShortDescription,
                    ImageUrl = p.PostImages?.FirstOrDefault(pi => pi.IsDefault)?.ImageUrl ?? "/assets/default.jpg",
                    Category = p.Category?.Title
                }).ToList();

            return new JsonResult(new { success = true, posts = pagedPosts });
        }
    }
}
