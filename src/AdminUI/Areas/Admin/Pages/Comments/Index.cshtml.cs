using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminUI.Areas_Admin_Pages_Comments
{
    [Authorize(Policy = "CommentsPolicy")]

    public class IndexModel : PageModel
    {
        private readonly ModelData.Data.ApplicationDbContext _context;

        public IndexModel(ModelData.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Comment> Comment { get; set; } = default!;
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string Query { get; set; } = "";

        public int TotalCount { get; set; }


        public async Task OnGetAsync(string? query, int resultPage = 1)
        {
            Query = query ?? "";
            CurrentPage = resultPage < 1 ? 1 : resultPage;
            int pageSize = 50;

            var commentQuery = _context.Comments
                .Include(c => c.Post)
                .Include(c => c.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Query))
            {
                commentQuery = commentQuery.Where(c =>
                    (c.User != null && c.User.UserName.Contains(Query)) ||
                    (!string.IsNullOrEmpty(c.AuthorName) && c.AuthorName.Contains(Query)) ||
                    (!string.IsNullOrEmpty(c.Content) && c.Content.Contains(Query)) ||
                    (c.Post != null && c.Post.Title.Contains(Query))
                );
            }

            TotalCount = await commentQuery.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);

            Comment = await commentQuery
                .OrderByDescending(x => x.CreatedAt)
                .Skip((CurrentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostToggleApprovalAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
                return NotFound();

            comment.IsApproved = !comment.IsApproved;
            await _context.SaveChangesAsync();

            // Refresh page
            return RedirectToPage();
        }
    }
}
