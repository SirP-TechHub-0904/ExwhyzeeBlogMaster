using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelData.Data;
using ModelData.Models;
using ModelData.Services;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AdminUI.Areas.Admin.Pages.SearchPanel
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<SearchResultDto> Results { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public string? Query { get; set; }

        [BindProperty(SupportsGet = true)]
        public int ResultPage { get; set; } = 1;
        public int TotalResults { get; set; } 

        public const int FlatPageSizeConst = 50;
        public int FlatPageSize => FlatPageSizeConst;

        public void OnGet()
        {
            if (string.IsNullOrWhiteSpace(Query)) return;

            var q = Query.ToLower();
            var results = new List<SearchResultDto>();

            // Helper to extract matches with context and highlight
            List<string> ExtractMatches(string rawContent, string keyword, int maxContext = 30)
            {
                var matches = new List<string>();
                if (string.IsNullOrWhiteSpace(rawContent) || string.IsNullOrWhiteSpace(keyword))
                    return matches;

                // Work with a stripped version for locating, but keep raw for slicing
                string cleanContent = SlugManager.StripEverything(rawContent);
                string lowerContent = cleanContent.ToLower();
                string lowerKeyword = keyword.ToLower();

                int index = 0;
                while ((index = lowerContent.IndexOf(lowerKeyword, index)) != -1)
                {
                    int start = Math.Max(0, index - maxContext);
                    int length = Math.Min(cleanContent.Length - start, keyword.Length + 2 * maxContext);

                    string snippet = cleanContent.Substring(start, length);

                    // Highlight using keyword, not Query (which might have case mismatch)
                    snippet = Regex.Replace(
                        snippet,
                        Regex.Escape(keyword),
                        match => $"<mark>{match.Value}</mark>",
                        RegexOptions.IgnoreCase
                    );

                    matches.Add($"... {snippet} ...");
                    index += keyword.Length;
                }

                return matches;
            }


            // === Posts ===
            foreach (var post in _context.Posts)
            {
                var matches = ExtractMatches(post.Title + " " + post.Content, q);
                foreach (var match in matches)
                {
                    results.Add(new SearchResultDto
                    {
                        Id = post.Id,
                        Snippet = match,
                        SourceType = "Post",
                        Title = post.Title,
                        EditUrl = "/Posts/Edit"
                    });
                }
            }

            // === Categories ===
            foreach (var category in _context.Categories)
            {
                var matches = ExtractMatches(category.Title, q);
                foreach (var match in matches)
                {
                    results.Add(new SearchResultDto
                    {
                        Id = category.Id,
                        Snippet = match,
                        SourceType = "Category",
                        Title = category.Title,
                        EditUrl = "/Categories/Edit"
                    });
                }
            }

            // === Comments ===
            foreach (var comment in _context.Comments)
            {
                var matches = ExtractMatches(comment.AuthorName + " " + comment.Content, q);
                foreach (var match in matches)
                {
                    results.Add(new SearchResultDto
                    {
                        Id = comment.Id,
                        Snippet = match,
                        SourceType = "Comment",
                        Title = comment.AuthorName,
                        EditUrl = "/Comments/Edit"
                    });
                }
            }

            // === Media Files ===
            foreach (var media in _context.MediaFiles)
            {
                var matches = ExtractMatches(media.Url + " " + (media.AltText ?? ""), q);
                foreach (var match in matches)
                {
                    results.Add(new SearchResultDto
                    {
                        Id = media.Id,
                        Snippet = match,
                        SourceType = "Media File",
                        Title = media.Url,
                        EditUrl = "/MediaFiles/Edit"
                    });
                }
            }

            // === Web Pages ===
            foreach (var wp in _context.Pages)
            {
                var matches = ExtractMatches(wp.Title + " " + wp.Content, q);
                foreach (var match in matches)
                {
                    results.Add(new SearchResultDto
                    {
                        Id = wp.Id,
                        Snippet = match,
                        SourceType = "Web Page",
                        Title = wp.Title,
                        EditUrl = "/WebPages/Edit"
                    });
                }
            }

            // === Settings ===
            foreach (var s in _context.Settings)
            {
                var matches = ExtractMatches(s.SiteTitle + " " + s.SiteDescription + " " + s.MetaDescription, q);
                foreach (var match in matches)
                {
                    results.Add(new SearchResultDto
                    {
                        Id = s.Id,
                        Snippet = match,
                        SourceType = "Setting",
                        Title = s.SiteTitle ?? "[No Title]",
                        EditUrl = "/Settings/Edit"
                    });
                }
            }

            TotalResults = results.Count;
            Results = results
                .Skip((ResultPage - 1) * FlatPageSize)
                .Take(FlatPageSize)
                .ToList();
        }
    }


    public class SearchResultDto
    {
        public int Id { get; set; }
        public string Snippet { get; set; } = "";
        public string SourceType { get; set; } = ""; // e.g., Post, Category, Comment, etc.
        public string Title { get; set; } = "";
        public string EditUrl { get; set; } = "";
    }

}
