using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;

namespace AdminUI.Areas.Admin.Pages.NewsletterAdmin
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<NewsletterSubscription> Subscribers { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string Query { get; set; } = string.Empty;

        public async Task OnGetAsync(string? query, int page = 1)
        {
            Query = query ?? "";
            CurrentPage = page;
            int pageSize = 30;

            var q = _context.NewsletterSubscriptions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(Query))
            {
                q = q.Where(s => s.Email.Contains(Query));
            }

            int totalCount = await q.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            Subscribers = await q
                .OrderByDescending(s => s.SubscribedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var subscriber = await _context.NewsletterSubscriptions.FindAsync(id);
            if (subscriber == null)
                return NotFound();

            _context.NewsletterSubscriptions.Remove(subscriber);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
