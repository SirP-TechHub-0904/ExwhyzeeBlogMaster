using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;
using System.Text.RegularExpressions;

namespace PoliciesBlog.Pages
{
    public class SearchModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public SearchModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string Query { get; set; } = "";

        public List<PublicSearchResultDto> Results { get; set; } = new();

        public async Task OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(Query)) return;

            var q = Query.ToLower();
            var results = new List<PublicSearchResultDto>();

            List<string> ExtractMatches(string rawContent, string keyword, int maxContext = 30)
            {
                var matches = new List<string>();
                if (string.IsNullOrWhiteSpace(rawContent)) return matches;

                string clean = Regex.Replace(rawContent, "<.*?>", "");
                string lower = clean.ToLower();
                int index = 0;

                while ((index = lower.IndexOf(keyword, index)) != -1)
                {
                    int start = Math.Max(0, index - maxContext);
                    int length = Math.Min(clean.Length - start, keyword.Length + 2 * maxContext);
                    string snippet = clean.Substring(start, length);

                    snippet = Regex.Replace(snippet, Regex.Escape(keyword), match => $"<mark>{match.Value}</mark>", RegexOptions.IgnoreCase);
                    matches.Add($"... {snippet} ...");
                    index += keyword.Length;
                }

                return matches;
            }

            // === Posts ===
            var posts = await _context.Posts.Where(p => p.IsPublished).ToListAsync();
            foreach (var post in posts)
            {
                var matches = ExtractMatches(post.Title + " " + post.Content, q);
                foreach (var match in matches)
                {
                    results.Add(new PublicSearchResultDto
                    {
                        Title = post.Title,
                        Snippet = match,
                        SourceType = "Post",
                        Url = $"/postpage?slug={post.Slug}",
                        PublishedAt = post.PublishedAt
                    });
                }
            }

            // === Categories ===
            var categories = await _context.Categories.ToListAsync();
            foreach (var category in categories)
            {
                var matches = ExtractMatches(category.Title, q);
                foreach (var match in matches)
                {
                    results.Add(new PublicSearchResultDto
                    {
                        Title = category.Title,
                        Snippet = match,
                        SourceType = "Category",
                        Url = $"/category/{category.Slug}"
                    });
                }
            }

            // === Pages ===
            var pages = await _context.Pages.Where(p => p.IsPublished).ToListAsync();
            foreach (var page in pages)
            {
                var matches = ExtractMatches(page.Title + " " + page.Content, q);
                foreach (var match in matches)
                {
                    results.Add(new PublicSearchResultDto
                    {
                        Title = page.Title,
                        Snippet = match,
                        SourceType = "Page",
                        Url = $"/webpage?slug={page.Slug}"
                    });
                }
            }

            Results = results;
        }

        public async Task<JsonResult> OnGetLoadMoreAsync(string query, int page)
        {
            const int pageSize = 20;
            var q = query.ToLower();
            var results = new List<PublicSearchResultDto>();

            List<string> ExtractMatches(string rawContent, string keyword, int maxContext = 30)
            {
                var matches = new List<string>();
                if (string.IsNullOrWhiteSpace(rawContent)) return matches;

                string clean = Regex.Replace(rawContent, "<.*?>", "");
                string lower = clean.ToLower();
                int index = 0;

                while ((index = lower.IndexOf(keyword, index)) != -1)
                {
                    int start = Math.Max(0, index - maxContext);
                    int length = Math.Min(clean.Length - start, keyword.Length + 2 * maxContext);
                    string snippet = clean.Substring(start, length);

                    snippet = Regex.Replace(snippet, Regex.Escape(keyword), match => $"<mark>{match.Value}</mark>", RegexOptions.IgnoreCase);
                    matches.Add($"... {snippet} ...");
                    index += keyword.Length;
                }

                return matches;
            }

            // Fetch posts
            var postResults = (await _context.Posts.Where(p => p.IsPublished).ToListAsync())
                .SelectMany(post => ExtractMatches(post.Title + " " + post.Content, q)
                    .Select(match => new PublicSearchResultDto
                    {
                        Title = post.Title,
                        Snippet = match,
                        SourceType = "Post",
                        Url = $"/postpage?slug={post.Slug}",
                        PublishedAt = post.PublishedAt
                    })).ToList();

            // Categories
            var categoryResults = (await _context.Categories.ToListAsync())
                .SelectMany(cat => ExtractMatches(cat.Title, q)
                    .Select(match => new PublicSearchResultDto
                    {
                        Title = cat.Title,
                        Snippet = match,
                        SourceType = "Category",
                        Url = $"/category/{cat.Slug}"
                    })).ToList();

            // Pages
            var pageResults = (await _context.Pages.Where(p => p.IsPublished).ToListAsync())
                .SelectMany(pg => ExtractMatches(pg.Title + " " + pg.Content, q)
                    .Select(match => new PublicSearchResultDto
                    {
                        Title = pg.Title,
                        Snippet = match,
                        SourceType = "Page",
                        Url = $"/webpage?slug={pg.Slug}"
                    })).ToList();

            var all = postResults.Concat(categoryResults).Concat(pageResults).ToList();
            var paged = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new JsonResult(new { success = true, data = paged });
        }

    }

    public class PublicSearchResultDto
    {
        public string Title { get; set; } = "";
        public string Snippet { get; set; } = "";
        public string SourceType { get; set; } = "";
        public string? Url { get; set; }
        public DateTime? PublishedAt { get; set; }
    }

}
