using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ModelData.Data;
using ModelData.Models;

namespace AdminUI.Areas.Admin.Pages.Forms
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<WebPageForm> Forms { get; set; } = new();

        public async Task OnGetAsync()
        {
            Forms = await _context.WebPageForms
                .Include(f => f.Fields)
                .Include(f => f.Responses)
                .OrderByDescending(f => f.Id)
                .ToListAsync();
        }
    }
}
