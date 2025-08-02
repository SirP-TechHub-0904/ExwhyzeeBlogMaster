using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;

namespace AdminUI.Areas_Admin_Pages_Posts
{
    [Authorize(Policy = "PostsPolicy")]

    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Post> Posts { get; set; } = new List<Post>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public string Query { get; set; } = "";
        public string Filter { get; set; } = "";
        public string SortBy { get; set; } = "Date"; // Default column
        public bool SortAsc { get; set; } = false;   // Default to descending


        public async Task OnGetAsync(string? query, string? filter, int resultPage = 1, string? sortBy = "Date", bool sortAsc = false)
        {
            Query = query ?? "";
            Filter = filter ?? "";
            SortBy = sortBy!;
            SortAsc = sortAsc;
            CurrentPage = resultPage < 1 ? 1 : resultPage;
            int pageSize = 50;

            var postQuery = _context.Posts
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Query))
            {
                postQuery = postQuery.Where(p =>
                    p.Title.Contains(Query) ||
                    (p.Category != null && p.Category.Title.Contains(Query))
                );
            }
            if (Filter == "published")
            {
                postQuery = postQuery.Where(p => p.IsPublished);
            }
            else if (Filter == "unpublished")
            {
                postQuery = postQuery.Where(p => !p.IsPublished);
            }
            else if (Filter == "scheduled")
            {
                postQuery = postQuery.Where(p => p.IsScheduled);
            }

            // Sorting
            postQuery = SortBy switch
            {
                "Title" => SortAsc ? postQuery.OrderBy(p => p.Title) : postQuery.OrderByDescending(p => p.Title),
                "Date" => SortAsc ? postQuery.OrderBy(p => p.PublishedAt) : postQuery.OrderByDescending(p => p.PublishedAt),
                "Category" => SortAsc ? postQuery.OrderBy(p => p.Category.Title) : postQuery.OrderByDescending(p => p.Category.Title),
                "Published" => SortAsc ? postQuery.OrderByDescending(p => p.IsPublished) : postQuery.OrderBy(p => p.IsPublished),
                "Featured" => SortAsc ? postQuery.OrderByDescending(p => p.IsFeatured) : postQuery.OrderBy(p => p.IsFeatured),
                "Breaking" => SortAsc ? postQuery.OrderByDescending(p => p.IsBreakingNews) : postQuery.OrderBy(p => p.IsBreakingNews),
                "Hero" => SortAsc ? postQuery.OrderByDescending(p => p.ShowInHero) : postQuery.OrderBy(p => p.ShowInHero),
                "Surface" => SortAsc ? postQuery.OrderByDescending(p => p.ShowInSurface) : postQuery.OrderBy(p => p.ShowInSurface),
                "Views" => SortAsc
                    ? postQuery.OrderBy(p => p.ViewCount)
                    : postQuery.OrderByDescending(p => p.ViewCount),
                _ => SortAsc ? postQuery.OrderByDescending(p => p.PublishedAt) : postQuery.OrderBy(p => p.PublishedAt),
            };


            TotalCount = await postQuery.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);

            Posts = await postQuery.Skip((CurrentPage - 1) * pageSize).Take(pageSize).ToListAsync();
        }
    }
}
