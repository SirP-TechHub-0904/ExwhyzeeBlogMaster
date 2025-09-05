using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;
using static AdminUI.Areas_Admin_Pages_Posts.IndexModel;

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
        public int? CategoryId { get; set; }
        public string CategoryTitle { get; set; } = "";
         public IList<CategoryOption> CategoryOptions { get; set; } = new List<CategoryOption>();
        public async Task OnGetAsync(string? query, string? filter, int resultPage = 1, string? sortBy = "Date", bool sortAsc = false, int? categoryId = null)
        {
            Query = query ?? "";
            Filter = filter ?? "";
            SortBy = sortBy!;
            SortAsc = sortAsc;
            CurrentPage = resultPage < 1 ? 1 : resultPage;
            int pageSize = 50;
            CategoryId = categoryId;

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
            if (CategoryId.HasValue && CategoryId.Value > 0)
            {
                postQuery = postQuery.Where(p => p.CategoryId == CategoryId.Value);

                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == CategoryId.Value);

                if (category != null)
                {
                    CategoryTitle = category.Title;
                }
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

            var allCategories = await _context.Categories
        .Include(c => c.ParentCategory)
        .OrderBy(c => c.SortOrder)
        .ToListAsync();
            // Build dropdown options hierarchically
            CategoryOptions = BuildCategoryOptions(allCategories);

            // Apply category filter
            if (CategoryId.HasValue && CategoryId.Value > 0)
            {
                var selected = allCategories.FirstOrDefault(c => c.Id == CategoryId.Value);
                if (selected != null)
                {
                    CategoryTitle = selected.Title;
                }
            }
        }
        private IList<CategoryOption> BuildCategoryOptions(List<Category> allCategories)
        {
            var options = new List<CategoryOption>();
            var topLevel = allCategories
                .Where(c => c.ParentCategoryId == null)
                .OrderBy(c => c.SortOrder)
                .ToList();

            int serial = 1;
            foreach (var cat in topLevel)
            {
                AddCategoryWithChildren(cat, allCategories, options, serial.ToString(), 1);
                serial++;
            }

            return options;
        }

        private void AddCategoryWithChildren(Category cat, List<Category> all, List<CategoryOption> options, string prefix, int depth)
        {
            options.Add(new CategoryOption
            {
                Id = cat.Id,
                DisplayText = $"{prefix}. {cat.Title}"
            });

            var children = all.Where(c => c.ParentCategoryId == cat.Id)
                              .OrderBy(c => c.SortOrder)
                              .ToList();

            for (int i = 0; i < children.Count; i++)
            {
                var childPrefix = $"{prefix}.{i + 1}";
                AddCategoryWithChildren(children[i], all, options, childPrefix, depth + 1);
            }
        }

       
    }
    public class CategoryOption
    {
        public int Id { get; set; }
        public string DisplayText { get; set; } = "";
    }
}
